using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace RVReducerDesignApp.Services;

/// <summary>
/// 整机输入快照：把 CTANRVSet 的全套 DLL 输入/分割索引导出成 JSON，供 RvCalc 通用调用器直接驱动
/// 需要「整机数据」的接口（CalCrankShaftSystem2 / RecommondShaftPara）。
///
/// 快照结构（与 RvCalc/interface-registry.json 的数组名一一对应）：
///   div    : 6×3 分割索引矩阵展平（行优先，flat[i*3+j]），共 18 个 int
///   idata  : 零部件分割索引（InputIntData）
///   ddata  : 整机输入数据（InputDoubleData，含总体/渐开线/摆线/轴承/轴段）
///   output : 整机输出数据（in/out，RecommondShaftPara 的 OutputData）
///   out    : 纯输出数组（按真实输出长度分配，CalCrankShaftSystem2 的 OutData）
/// </summary>
public static class WholeMachineSnapshot
{
    private static readonly JsonSerializerOptions JsonOpts = new()
    {
        WriteIndented = true,
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        NumberHandling = JsonNumberHandling.AllowNamedFloatingPointLiterals,
    };

    private static CTANRVSet? _current;

    /// <summary>由 CTANRVSet 构造时登记，供 Agent/命令随时导出当前设计态</summary>
    public static void Register(CTANRVSet set) => _current = set;

    public static bool HasCurrent => _current is not null;

    /// <summary>当前整机数据对象（未创建设计工程时为 null）</summary>
    public static CTANRVSet? CurrentSet => _current;

    /// <summary>生成快照 JSON 文本</summary>
    /// <param name="set">整机数据；为 null 时用最近登记的实例</param>
    /// <param name="interfaceName">写入快照的目标接口名（仅记录用途）</param>
    public static string CaptureJson(CTANRVSet? set = null, string? interfaceName = null, bool recompute = true)
    {
        var target = set ?? _current
                     ?? throw new InvalidOperationException("当前没有整机数据（尚未创建设计工程）");

        var warnings = new List<string>();

        // 默认复算一次分割索引与各部件输入，保证快照自洽；
        // 在 DLL 调用前后抓取时应传 recompute:false——因为进入这一步前调用方刚构建过数组，
        // 而复算可能改变状态（例如轴推荐期间 isRecommendShaft=true 会影响轴承段）。
        bool partial = false;
        if (recompute)
        {
            target.UpdataRVSetDataDivisionIndex();
            try
            {
                target.UpdateAllPartsDllInputData();
            }
            catch (Exception ex)
            {
                partial = true;
                warnings.Add("整机输入数据不完整（通常是尚未设计曲轴/未执行轴推荐）：" + ex.GetType().Name + ": " + ex.Message);
            }
        }

        int inLen = target.mDivisionIndexList[5, 1];
        int outLen = target.mDivisionIndexList[5, 2];
        var div = Flatten(target.mDivisionIndexList);
        var idata = target.mPartsDivisionIndexList ?? Array.Empty<int>();
        var ddata = Slice(target.mInputDoubleDataStorage, inLen);
        var output = Slice(target.mOutputDoubleDataStorage, outLen);

        var payload = new Dictionary<string, object?>
        {
            ["interface"] = interfaceName ?? "CalCrankShaftSystem2",
            ["meta"] = new Dictionary<string, object?>
            {
                ["source"] = "RVReducerDesignApp.WholeMachineSnapshot",
                ["timestamp"] = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                ["partial"] = partial,
                ["warnings"] = warnings,
                ["divLayout"] = "6x3 int[,] row-major -> flat[18], index = i*3+j",
                ["inputDataLength"] = inLen,
                ["outputDataLength"] = outLen,
                ["idataLength"] = idata.Length,
                ["ddataLength"] = ddata.Length,
                ["note"] = "div=分割索引(展平) / idata=零部件分割索引 / ddata=整机输入 / output=整机输出(in-out) / out=纯输出",
            },
            ["arrays"] = new Dictionary<string, object?>
            {
                ["div"] = div,
                ["idata"] = idata,
                ["ddata"] = ddata,
                ["output"] = output,
                ["out"] = new double[Math.Max(outLen, 32)],
            },
        };
        return JsonSerializer.Serialize(payload, JsonOpts);
    }

    /// <summary>
    /// 自动导出钩子：环境变量 RV_SNAPSHOT_DIR 存在时，在整机计算（曲轴系计算/轴推荐）完成后自动导出快照。
    /// 这是获取「完整」快照的推荐方式：先设计好曲轴，再点一次曲轴系计算即可。
    /// 任何异常都不会影响界面流程。
    /// </summary>
    public static string? TryAutoExport(string interfaceName = "CalCrankShaftSystem2", bool recompute = false)
    {
        try
        {
            var dir = ResolveSnapshotDir();
            if (string.IsNullOrWhiteSpace(dir)) return null;
            var path = Path.Combine(dir, $"wholeMachine-{interfaceName}-{DateTime.Now:yyyyMMdd-HHmmss}.json");
            return Export(path, null, interfaceName, recompute);
        }
        catch
        {
            return null; // 自动导出失败不得影响界面
        }
    }

    /// <summary>
    /// 解析快照输出目录（环境变量 RV_SNAPSHOT_DIR）。
    /// 相对路径按工作区根解析（例：RV_SNAPSHOT_DIR=screenshots\snapshots）——避免产物落到项目外，
    /// 保证整个项目目录可直接拷贝迁移。
    /// </summary>
    public static string? ResolveSnapshotDir()
    {
        var dir = Environment.GetEnvironmentVariable("RV_SNAPSHOT_DIR");
        if (string.IsNullOrWhiteSpace(dir)) return null;
        if (Path.IsPathRooted(dir)) return dir;
        var root = FindWorkspaceRoot();
        return root is null ? Path.GetFullPath(dir) : Path.Combine(root, dir);
    }

    /// <summary>导出快照到文件；未指定路径时写入 &lt;工作区&gt;/RvCalc/templates/wholeMachine-snapshot.json</summary>
    public static string Export(string? outputPath = null, CTANRVSet? set = null, string? interfaceName = null, bool recompute = true)
    {
        var json = CaptureJson(set, interfaceName, recompute);
        var path = outputPath;
        if (string.IsNullOrWhiteSpace(path))
        {
            var root = FindWorkspaceRoot() ?? AppContext.BaseDirectory;
            path = Path.Combine(root, "RvCalc", "templates", "wholeMachine-snapshot.json");
        }
        var full = Path.GetFullPath(path);
        Directory.CreateDirectory(Path.GetDirectoryName(full)!);
        File.WriteAllText(full, json);
        return full;
    }

    /// <summary>从运行目录向上查找包含 RVReducerBasic.sln 的工作区根目录</summary>
    public static string? FindWorkspaceRoot()
    {
        try
        {
            var dir = new DirectoryInfo(AppContext.BaseDirectory);
            while (dir is not null)
            {
                if (File.Exists(Path.Combine(dir.FullName, "RVReducerBasic.sln")))
                    return dir.FullName;
                dir = dir.Parent;
            }
        }
        catch
        {
            // 忽略：返回 null 由调用方降级
        }
        return null;
    }

    /// <summary>int[,] → 行优先展平（mDivisionIndexList[i, j] → flat[i*3+j]）</summary>
    private static int[] Flatten(int[,] matrix)
    {
        int rows = matrix.GetLength(0);
        int cols = matrix.GetLength(1);
        var flat = new int[rows * cols];
        for (int i = 0; i < rows; i++)
            for (int j = 0; j < cols; j++)
                flat[i * cols + j] = matrix[i, j];
        return flat;
    }

    private static double[] Slice(double[]? source, int length)
    {
        if (source is null) return new double[Math.Max(length, 0)];
        var len = length > 0 ? length : source.Length;
        var result = new double[len];
        Array.Copy(source, result, Math.Min(source.Length, len));
        return result;
    }
}
