using System.IO;
using System.Runtime.InteropServices;

namespace RvCalc;

/// <summary>一次原始 DLL 调用的结果（按数组名索引）</summary>
public sealed class RvRawCall
{
    public int ReturnValue { get; set; }
    public Dictionary<string, double[]> Doubles { get; } = new(StringComparer.OrdinalIgnoreCase);
    public Dictionary<string, int[]> Ints { get; } = new(StringComparer.OrdinalIgnoreCase);
    public Dictionary<string, string[]> Strings { get; } = new(StringComparer.OrdinalIgnoreCase);
}

/// <summary>
/// 通用动态 DLL 调用器：不依赖 [DllImport] 静态声明，按导出名在运行时解析并调用。
/// 所有参数统一按指针（IntPtr）传递，由本类负责分配/复制/释放非托管内存，
/// 因此新增接口只需在注册表里描述，无需新增 C# 代码。
/// </summary>
public sealed class DynamicDllCaller : IDisposable
{
    /// <summary>统一原生函数签名：最多 5 个指针参数，返回 int（void 函数的返回值忽略）</summary>
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate int RawFn(IntPtr a, IntPtr b, IntPtr c, IntPtr d, IntPtr e);

    [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    private static extern bool SetDllDirectory(string? lpPathName);

    private readonly IntPtr _lib;
    private readonly Dictionary<string, RawFn> _cache = new(StringComparer.Ordinal);

    public string DllPath { get; }

    public DynamicDllCaller(string? dllPath = null)
    {
        DllPath = Resolve(dllPath) ?? throw new DllNotFoundException(
            "未找到 RVReducerBasic.dll。请先编译 C++ 引擎（x64 Debug）与 WPF 应用，" +
            "或设置环境变量 RV_DLL_PATH 指向该 DLL。");
        try
        {
            SetDllDirectory(Path.GetDirectoryName(DllPath));
            _lib = NativeLibrary.Load(DllPath);
        }
        catch (Exception ex)
        {
            throw new DllNotFoundException(
                $"加载 {DllPath} 失败: {ex.Message}\n" +
                "提示：RVReducerBasic.dll 依赖 40+ 个 Intel MKL 分派 DLL，需与它同目录。", ex);
        }
    }

    /// <summary>按优先级解析 DLL 路径</summary>
    public static string? Resolve(string? explicitPath = null)
    {
        var candidates = new List<string?>();
        if (!string.IsNullOrWhiteSpace(explicitPath)) candidates.Add(explicitPath);
        candidates.Add(Environment.GetEnvironmentVariable("RV_DLL_PATH"));
        candidates.Add(Path.Combine(AppContext.BaseDirectory, "RVReducerBasic.dll"));

        // 从程序目录向上找工程里的 WPF 输出目录（含完整 MKL 依赖集合）
        var dir = new DirectoryInfo(AppContext.BaseDirectory);
        for (int i = 0; i < 8 && dir is not null; i++, dir = dir.Parent)
        {
            candidates.Add(Path.Combine(dir.FullName, "RVReducerDesignApp", "bin", "Debug", "net8.0-windows", "RVReducerBasic.dll"));
            candidates.Add(Path.Combine(dir.FullName, "RVReducerDesignApp", "bin", "Release", "net8.0-windows", "RVReducerBasic.dll"));
        }

        foreach (var c in candidates)
            if (!string.IsNullOrWhiteSpace(c) && File.Exists(c))
                return Path.GetFullPath(c!);
        return null;
    }

    /// <summary>导出的函数名是否存在于 DLL 中</summary>
    public bool HasExport(string exportName) =>
        NativeLibrary.TryGetExport(_lib, exportName, out _);

    /// <summary>
    /// 按注册表描述调用导出函数。
    /// <paramref name="buffers"/> 的键为参数名（如 "in"/"out"/"div"），值为 double[] 或 int[]；
    /// 输入数组需已填充数据，输出数组由本方法回填。
    /// 返回 null 表示该参数为字符串数组输出。
    /// </summary>
    public RvRawCall Invoke(RvInterfaceDesc desc, IDictionary<string, object> buffers)
    {
        if (!HasExport(desc.Name))
            throw new EntryPointNotFoundException($"DLL 中不存在导出函数 {desc.Name}（{DllPath}）");
        if (desc.Args.Count > 5)
            throw new NotSupportedException($"接口 {desc.Name} 有 {desc.Args.Count} 个参数，通用调用器最多支持 5 个");

        var fn = GetFunction(desc.Name);
        var allocations = new List<IntPtr>();
        var stringsPlans = new List<(RvArgDesc Arg, IntPtr Buffer, int MaxCount, int MaxLen)>();
        IntPtr[] argv = new IntPtr[5];

        try
        {
            for (int i = 0; i < desc.Args.Count; i++)
            {
                var arg = desc.Args[i];
                if (!buffers.TryGetValue(arg.Name, out var data) || data is null)
                    throw new ArgumentException($"接口 {desc.Name} 缺少参数数组 {arg.Name}");

                if (arg.Type == "strings")
                {
                    int maxCount = Math.Max(1, arg.MaxCount);
                    int maxLen = Math.Max(8, arg.MaxLen);
                    var block = Marshal.AllocHGlobal(maxCount * IntPtr.Size);
                    allocations.Add(block);
                    for (int k = 0; k < maxCount; k++)
                    {
                        var s = Marshal.AllocHGlobal(maxLen);
                        allocations.Add(s);
                        Marshal.Copy(new byte[maxLen], 0, s, maxLen);
                        Marshal.WriteIntPtr(block, k * IntPtr.Size, s);
                    }
                    stringsPlans.Add((arg, block, maxCount, maxLen));
                    argv[i] = block;
                }
                else if (arg.Type == "int")
                {
                    var arr = (int[])data;
                    // 容量取「注册表声明长度」与「实际提供长度」的较大者：
                    // 整机快照等场景下数组长度由运行数据决定，且必须容下 DLL 的真实写入范围。
                    int len = Math.Max(arg.Length, arr.Length);
                    if (len < 1) len = 1;
                    var ptr = Marshal.AllocHGlobal(len * sizeof(int));
                    allocations.Add(ptr);
                    if (arg.IsInput) Marshal.Copy(arr, 0, ptr, Math.Min(arr.Length, len));
                    argv[i] = ptr;
                }
                else
                {
                    var arr = (double[])data;
                    int len = Math.Max(arg.Length, arr.Length);
                    if (len < 1) len = 1;
                    var ptr = Marshal.AllocHGlobal(len * sizeof(double));
                    allocations.Add(ptr);
                    if (arg.IsInput) Marshal.Copy(arr, 0, ptr, Math.Min(arr.Length, len));
                    argv[i] = ptr;
                }
            }

            int ret = fn(argv[0], argv[1], argv[2], argv[3], argv[4]);

            var result = new RvRawCall { ReturnValue = ret };
            for (int i = 0; i < desc.Args.Count; i++)
            {
                var arg = desc.Args[i];
                if (!arg.IsOutput) continue;

                if (arg.Type == "strings")
                {
                    var plan = stringsPlans.First(p => p.Arg.Name == arg.Name);
                    int count = Math.Min(Math.Max(ret, 0), plan.MaxCount);
                    var arr = new string[count];
                    for (int k = 0; k < count; k++)
                    {
                        var s = Marshal.ReadIntPtr(plan.Buffer, k * IntPtr.Size);
                        arr[k] = Marshal.PtrToStringAnsi(s) ?? "";
                    }
                    result.Strings[arg.Name] = arr;
                }
                else if (arg.Type == "int")
                {
                    int len = Math.Max(arg.Length, ((int[])buffers[arg.Name]).Length);
                    var arr = new int[len];
                    Marshal.Copy(argv[i], arr, 0, len);
                    result.Ints[arg.Name] = arr;
                }
                else
                {
                    int len = Math.Max(arg.Length, ((double[])buffers[arg.Name]).Length);
                    var arr = new double[len];
                    Marshal.Copy(argv[i], arr, 0, len);
                    result.Doubles[arg.Name] = arr;
                }
            }
            return result;
        }
        finally
        {
            foreach (var p in allocations) Marshal.FreeHGlobal(p);
        }
    }

    private RawFn GetFunction(string exportName)
    {
        if (_cache.TryGetValue(exportName, out var cached)) return cached;
        var ptr = NativeLibrary.GetExport(_lib, exportName);
        var fn = Marshal.GetDelegateForFunctionPointer<RawFn>(ptr);
        _cache[exportName] = fn;
        return fn;
    }

    public void Dispose() => NativeLibrary.Free(_lib);
}
