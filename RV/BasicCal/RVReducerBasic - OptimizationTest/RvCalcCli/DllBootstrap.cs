using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

namespace RvCalcCli;

/// <summary>
/// 运行环境准备：定位工作区、准备 RVReducerBasic.dll 及其 250 个原生依赖。
/// 目标：只要能找到/复制到依赖，RvCalcCli 就能独立运行，无需启动任何其它工具。
/// </summary>
internal static class DllBootstrap
{
    /// <summary>从当前目录向上查找包含 RVReducerBasic.sln 的工作区根目录</summary>
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
            // 忽略：返回 null，由调用方决定是否降级
        }
        return null;
    }

    /// <summary>
    /// 确保 DLL 与依赖就绪：
    ///   ① 运行目录已有 RVReducerBasic.dll 且存在 MKL 依赖 → 直接可用；
    ///   ② 否则从 RVReducerDesignApp 输出目录整体复制全部 DLL（含 MKL 分派、MySQL 客户端等）。
    /// 返回是否就绪；<paramref name="wsRoot"/> 为定位到的工作区根（可能为 null）。
    /// </summary>
    public static bool EnsureDlls(out string? wsRoot, bool quiet = false)
    {
        wsRoot = FindWorkspaceRoot();
        var baseDir = AppContext.BaseDirectory;
        var localDll = Path.Combine(baseDir, "RVReducerBasic.dll");
        var appBin = wsRoot is null ? null : Path.Combine(wsRoot, "RVReducerDesignApp", "bin", "Debug", "net8.0-windows");
        var srcDll = appBin is null ? null : Path.Combine(appBin, "RVReducerBasic.dll");

        bool srcAvailable = srcDll is not null && File.Exists(srcDll);
        bool localReady = File.Exists(localDll) && HasNativeDeps(baseDir);
        bool outdated = localReady && srcAvailable
                        && File.GetLastWriteTimeUtc(srcDll!) > File.GetLastWriteTimeUtc(localDll);

        if (localReady && !outdated)
        {
            if (!quiet) Console.WriteLine($"  ✓ 运行目录已具备 DLL 及依赖: {baseDir}");
            return true;
        }

        if (!srcAvailable)
        {
            if (localReady)
            {
                // 源目录不可用但本地副本完整（例如只分发 CLI 输出目录）：直接使用，仅提示版本可能较旧
                if (!quiet) Console.WriteLine($"  ✓ 使用运行目录现有 DLL（未找到工作区源目录，无法校验版本）: {localDll}");
                return true;
            }
            Console.WriteLine("  ⚠ 未找到工作区根目录（RVReducerBasic.sln），且运行目录缺少 DLL 依赖。");
            Console.WriteLine("    可设置环境变量 RV_DLL_PATH 指向 RVReducerBasic.dll，或把该 DLL 及其全部依赖放到运行目录。");
            return false;
        }

        var copied = 0;
        foreach (var file in Directory.GetFiles(appBin!, "*.dll"))
        {
            var dest = Path.Combine(baseDir, Path.GetFileName(file));
            try
            {
                File.Copy(file, dest, overwrite: true);
                copied++;
            }
            catch (Exception ex)
            {
                // 目标文件已被当前进程加载（如 RVAgentCli 引用并加载了 RVReducerDesignApp.dll）时，
                // 复制会因文件锁失败，此时静默跳过；目标缺失才是真问题，需要提示。
                if (!File.Exists(dest))
                    Console.WriteLine($"  ⚠ 复制 {Path.GetFileName(file)} 失败: {ex.Message}");
            }
        }
        if (!quiet)
            Console.WriteLine($"  ✓ 已从 {appBin} 复制 {copied} 个 DLL 到运行目录" + (outdated ? "（源 DLL 较新，已刷新）" : ""));
        return File.Exists(localDll);
    }

    /// <summary>粗略判断运行目录是否存在 MKL 分派依赖（mkl_*.dll）</summary>
    private static bool HasNativeDeps(string dir)
    {
        try
        {
            return Directory.EnumerateFiles(dir, "mkl_*.dll").Any();
        }
        catch
        {
            return false;
        }
    }
}

/// <summary>
/// 静态 DllImport 通道（仅用于 --selfcheck 与通用动态调用器对比结果）。
/// 这是"另一种调用方式"的参照实现，与 RvCalc 通用调用器相互独立。
/// </summary>
internal static class StaticProbe
{
    [DllImport("RVReducerBasic.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern void CalInvGearStress2(ref int inDivideIndex, ref double inInputData, ref double outData);

    [DllImport("RVReducerBasic.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern void CalBearingLife(ref double inData, ref double outData);

    public static double[] InvGearStress2(int[] divideIndex, double[] input)
    {
        var output = new double[284];
        CalInvGearStress2(ref divideIndex[0], ref input[0], ref output[0]);
        return output;
    }

    public static double[] BearingLife(double[] input)
    {
        var output = new double[20];
        CalBearingLife(ref input[0], ref output[0]);
        return output;
    }
}
