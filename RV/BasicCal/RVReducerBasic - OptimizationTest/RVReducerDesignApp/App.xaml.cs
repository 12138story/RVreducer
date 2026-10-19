using System;
using System.Configuration;
using System.Data;
using System.Text;
using System.Windows;

namespace RVReducerDesignApp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// 支持无界面启动参数：
        ///   --export-snapshot [输出路径]                   导出整机输入快照
        ///   --bearing-lookup &lt;型号&gt; [--n N] [--fa Fa] [--fr Fr]  查轴承库并算寿命（无需界面，便于脚本/自检）
        ///   --calc-part &lt;部件&gt;                             按默认参数算某部件（摆线针轮副/渐开线齿轮副/总体）
        /// 命中时执行后退出（0=成功，2=失败），不会创建主窗口。
        /// </summary>
        protected override void OnStartup(StartupEventArgs e)
        {
            int partIdx = Array.IndexOf(e.Args, "--calc-part");
            if (partIdx >= 0 && partIdx + 1 < e.Args.Length)
            {
                try
                {
                    var set = new CTANRVSet();
                    set.mCrankShaftSystem.mBelongedSet = set;
                    var part = e.Args[partIdx + 1];
                    var result = Services.CurrentMachineCalc.CalcPart(part);
                    Console.WriteLine($"[calc-part] part={part} ok={result.Ok} outputs={result.Outputs.Count}");
                    foreach (var line in result.ToText().Split('\n').Take(28)) Console.WriteLine(line.TrimEnd('\r'));
                    Shutdown(result.Ok ? 0 : 2);
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine("[calc-part] failed: " + ex);
                    Shutdown(2);
                }
                return;
            }

            int lookIdx = Array.IndexOf(e.Args, "--bearing-lookup");
            if (lookIdx >= 0 && lookIdx + 1 < e.Args.Length)
            {
                try
                {
                    double ArgOf(string flag, double fallback)
                    {
                        var i = Array.IndexOf(e.Args, flag);
                        return i >= 0 && i + 1 < e.Args.Length &&
                               double.TryParse(e.Args[i + 1], System.Globalization.NumberStyles.Float,
                                   System.Globalization.CultureInfo.InvariantCulture, out var v)
                            ? v
                            : fallback;
                    }

                    var code = e.Args[lookIdx + 1];
                    var result = Services.BearingCatalog.CalcLife(code, ArgOf("--n", 1000), ArgOf("--fa", 0), ArgOf("--fr", 0));
                    // 注意：WinExe 的 stdout 被重定向时没有控制台句柄，设置 OutputEncoding 会抛
                    // IOException(句柄无效)，因此这里只输出 ASCII 安全的一行摘要（中文详情见应用内工具/结果文本）。
                    if (result.Ok && result.Bearing is { } b)
                    {
                        Console.WriteLine($"[bearing-lookup] code={b.Code} table={b.Table} d={b.d} D={b.D} B={b.B} " +
                                          $"Cr={b.Cr_kN}kN C0r={b.C0r_kN}kN e={b.e} Y={b.Y} Y0={b.Y0} " +
                                          $"n={result.n} Fa={result.Fa} Fr={result.Fr} L10h={result.LifeHours:0.####}");
                    }
                    else
                    {
                        Console.WriteLine($"[bearing-lookup] FAILED code={code} reason={result.Message}");
                    }
                    Shutdown(result.Ok ? 0 : 2);
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine("[bearing-lookup] failed: " + ex);
                    Shutdown(2);
                }
                return;
            }

            int idx = Array.IndexOf(e.Args, "--export-snapshot");
            if (idx >= 0)
            {
                string? outPath = idx + 1 < e.Args.Length && !e.Args[idx + 1].StartsWith("--")
                    ? e.Args[idx + 1]
                    : null;
                try
                {
                    var set = new CTANRVSet();
                    set.mCrankShaftSystem.mBelongedSet = set; // 与 MainWindow 构造时保持一致
                    var path = Services.WholeMachineSnapshot.Export(outPath, set);
                    Console.WriteLine($"[snapshot] exported: {path}");
                    Shutdown(0);
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine("[snapshot] failed: " + ex);
                    Shutdown(2);
                }
                return;
            }

            base.OnStartup(e);
        }
    }

}
