using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Windows.Automation;
using RVAutoTest.Models;
using RVAutoTest.Scenarios;

namespace RVAutoTest
{
    /// <summary>
    /// RV 减速器全自动推荐与截图工具。
    ///
    /// 执行流程:
    ///   1. 复制所需 DLL 到运行目录
    ///   2. 启动 RVReducerDesignApp WPF 应用
    ///   3. 依次执行: 齿轮推荐 / 轴推荐+曲轴图形 / 圆锥滚子轴承寿命
    ///   4. 生成汇总 report.json
    ///
    /// 输出目录: ./screenshots/
    /// </summary>
    internal static class Program
    {
        /// <summary>WPF 应用主窗口标题（MainWindow.xaml 的 Title 属性）</summary>
        internal const string AppWindowTitle = "摆线针轮行星减速器设计系统";

        /// <summary>ISO 测试输出目录时间戳格式（每次运行独立文件夹）</summary>
        internal const string RunTimestampFormat = "yyyyMMdd-HHmmss";

        /// <summary>
        /// 创建带时间戳的唯一输出子目录，返回完整路径。
        /// 路径 = 工作区根 + subPath + <时间戳>；同秒重复运行时追加 -2/-3 序号保证唯一。
        /// </summary>
        internal static string CreateTimestampedDir(string workspaceRoot, params string[] subPath)
        {
            var baseDir = Path.Combine(new[] { workspaceRoot }
                .Concat(subPath)
                .Concat(new[] { DateTime.Now.ToString(RunTimestampFormat) }).ToArray());
            var dir = baseDir;
            for (int i = 2; Directory.Exists(dir); i++)
                dir = $"{baseDir}-{i}";
            Directory.CreateDirectory(dir);
            return dir;
        }

        [STAThread]
        private static int Main(string[] args)
        {
            try
            {
                // 临时验证：驱动 UI 摆线页面计算并与 DLL 直调对比
                if (args.Contains("--verify-cycloid-ui"))
                    return VerifyCycloidUi.Run(args);

                // ISO Example 1 UI 截图测试（先匹配长参数，避免被 --iso-example1 子串捕获）
                if (args.Contains("--iso-example1-ui"))
                    return IsoExample1UiTest.Run(args);

                // ISO/TR 6336-30 Example 1 渐开线齿轮副验证测试（DLL 直调，无需 UI）
                if (args.Contains("--iso-example1"))
                    return IsoExample1Test.Run(args);

                // 摆线针轮副独立计算（DLL 直调，无需 UI/截图，输入/输出落盘）
                if (args.Contains("--cycloid"))
                    return CycloidGearOnlyTest.Run(args);

                return Run(args);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ 致命错误: {ex.Message}");
                return 2;
            }
        }

        /// <summary>主执行逻辑（被 Main 包裹以统一错误处理）</summary>
        private static int Run(string[] args)
        {
            Console.WriteLine("========================================");
            Console.WriteLine("  RV 全自动推荐与截图工具 v1.0");
            Console.WriteLine("========================================");

            // ── 1. 解析命令行参数 (--config <path>) ──
            var configPath = ParseConfigArg(args);
            TestConfig? config = null;
            if (!string.IsNullOrEmpty(configPath))
            {
                try
                {
                    config = ConfigLoader.Load(configPath);
                    if (config is null)
                        throw new InvalidOperationException($"配置文件不存在: {configPath}");
                    Console.WriteLine($"📄 使用配置文件: {configPath}");

                    // 数值合理性校验
                    var validationError = ConfigLoader.Validate(config);
                    if (validationError is not null)
                        throw new InvalidOperationException($"配置校验失败: {validationError}");
                }
                catch (InvalidOperationException ex)
                {
                    Console.WriteLine($"❌ 配置加载失败: {ex.Message}");
                    return 2;
                }
            }
            else
            {
                Console.WriteLine("📄 使用默认参数（未指定 --config）");
            }

            // ── 2. 确定路径 ──
            var workspaceRoot = FindWorkspaceRoot();
            var outputDir = Path.Combine(workspaceRoot, "screenshots");
            Directory.CreateDirectory(outputDir);
            Console.WriteLine($"工作区根目录: {workspaceRoot}");
            Console.WriteLine($"输出目录: {outputDir}");

            // ── 3. 复制 DLL 到运行目录 ──
            EnsureDllsAvailable(workspaceRoot);

            // ── 4. 启动 WPF 应用 ──
            var appExe = FindWpfAppExe(workspaceRoot);
            AutomationElement? window = null;
            Process? appProcess = null;

            if (appExe is not null)
            {
                Console.WriteLine($"启动 WPF 应用: {appExe}");
                try
                {
                    (appProcess, window) = UIAutomator.LaunchAndWaitForWindow(appExe, AppWindowTitle);
                    Console.WriteLine("✅ WPF 应用窗口就绪");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"⚠️ 启动 WPF 应用失败: {ex.Message}");
                    Console.WriteLine("    将仅执行 DLL 计算，跳过 UI 截图");
                }
            }
            else
            {
                Console.WriteLine("⚠️ 未找到 RVReducerDesignApp.exe，将仅执行 DLL 计算");
            }

            // ── 5. 执行场景 ──
            var report = new Report { GeneratedAt = DateTime.Now };

            try
            {
                try { report.GearRecommend = GearRecommendScenario.Run(outputDir, window, config); ValidateGearResult(report); }
                catch (Exception ex) { report.Errors.Add($"齿轮推荐场景异常: {ex.Message}"); Console.WriteLine($"❌ 齿轮推荐场景异常: {ex.Message}"); }

                // 读取齿轮推荐原始 OutData（数据闭环：渐开线/摆线几何从推荐结果映射）
                // 仅当本场景成功时读取，避免复用上一次运行的过期文件；用 JsonOptions.Pretty 兼容 NaN/Infinity 字面量
                double[]? gearOutData = null;
                var gearOutDataPath = Path.Combine(outputDir, "gear-recommend-outdata.json");
                if (report.GearRecommend is not null && File.Exists(gearOutDataPath))
                {
                    try { gearOutData = JsonSerializer.Deserialize<double[]>(File.ReadAllText(gearOutDataPath), JsonOptions.Pretty); }
                    catch (Exception ex) { Console.WriteLine($"⚠️ 读取齿轮推荐 OutData 失败: {ex.Message}"); }
                }
                if (gearOutData is null)
                {
                    Console.WriteLine("⚠️ 无有效齿轮推荐 OutData（本场景失败或文件缺失），渐开线/摆线将使用默认几何");
                }

                try { report.InvGear = InvGearScenario.Run(outputDir, config, gearOutData); ValidateInvGearResult(report); }
                catch (Exception ex) { report.Errors.Add($"渐开线齿轮副场景异常: {ex.Message}"); Console.WriteLine($"❌ 渐开线齿轮副场景异常: {ex.Message}"); }

                try { report.CycloidGear = CycloidGearScenario.Run(outputDir, config, gearOutData); ValidateCycloidGearResult(report); }
                catch (Exception ex) { report.Errors.Add($"摆线针轮副场景异常: {ex.Message}"); Console.WriteLine($"❌ 摆线针轮副场景异常: {ex.Message}"); }

                try { report.ShaftRecommend = ShaftRecommendScenario.Run(outputDir, window, config); ValidateShaftResult(report); }
                catch (Exception ex) { report.Errors.Add($"轴推荐场景异常: {ex.Message}"); Console.WriteLine($"❌ 轴推荐场景异常: {ex.Message}"); }

                // 暂时跳过轴承寿命计算场景（先完成推荐部分需求）
                // try { report.BearingLife = BearingLifeScenario.Run(outputDir, window, config); ValidateBearingResult(report); }
                // catch (Exception ex) { report.Errors.Add($"轴承寿命场景异常: {ex.Message}"); Console.WriteLine($"❌ 轴承寿命场景异常: {ex.Message}"); }

                // ── 7. 汇总报告 ──
                report.Success = report.Errors.Count == 0;
                var reportPath = Path.Combine(outputDir, "report.json");
                var reportJson = JsonSerializer.Serialize(report, JsonOptions.Pretty);
                File.WriteAllText(reportPath, reportJson);

                // ── 8. 完成 ──
                Console.WriteLine("");
                Console.WriteLine("========================================");
                if (report.Success)
                    Console.WriteLine("✅ 全自动测试完成！");
                else
                {
                    Console.WriteLine("❌ 测试完成，但存在失败场景！");
                    foreach (var err in report.Errors)
                        Console.WriteLine($"   - {err}");
                }
                Console.WriteLine($"📁 输出目录: {outputDir}");
                Console.WriteLine($"📄 汇总报告: {reportPath}");
                Console.WriteLine("========================================");
                return report.Success ? 0 : 1;
            }
            finally
            {
                // ── 9. 关闭应用（finally 保证任何路径都清理进程，防止泄漏）──
                if (appProcess is not null)
                {
                    try
                    {
                        if (!appProcess.HasExited)
                        {
                            appProcess.CloseMainWindow();
                            appProcess.WaitForExit(5000);
                        }
                    }
                    catch (Exception) { }
                    if (!appProcess.HasExited)
                    {
                        try { appProcess.Kill(); } catch (Exception) { }
                    }
                    appProcess.Dispose();
                }
            }
        }

        /// <summary>校验齿轮推荐结果是否有效</summary>
        private static void ValidateGearResult(Report report)
        {
            var r = report.GearRecommend;
            if (r is null) return;
            bool pinionTeethOk = r.齿轮1.TryGetValue("齿数", out double pinionTeeth) && pinionTeeth > 0;
            if (r.法向模数 <= 0 || !pinionTeethOk)
            {
                report.Errors.Add("齿轮推荐结果无效（模数或齿数为 0/负值），可能 DLL 调用失败");
            }
        }

        /// <summary>校验轴推荐结果是否有效</summary>
        private static void ValidateShaftResult(Report report)
        {
            var r = report.ShaftRecommend;
            if (r is null) return;

            // 汇入 UI 操作/解析的根因
            if (!string.IsNullOrEmpty(r.ErrorMessage))
                report.Errors.Add($"轴推荐场景异常: {r.ErrorMessage}");

            if (r.RecommendedDiameter <= 0 || r.RecommendedLength <= 0)
                report.Errors.Add("轴推荐结果无效（直径或长度为 0），可能未成功读取 UI 结果");
        }

        /// <summary>校验轴承寿命结果是否有效</summary>
        private static void ValidateBearingResult(Report report)
        {
            var r = report.BearingLife;
            if (r is not null && (r.StatusCode <= 0 || r.L10h <= 0))
            {
                report.Errors.Add($"轴承寿命计算无效（状态码: {r.StatusCode}, L10h: {r.L10h:F2}）");
            }
        }

        /// <summary>校验渐开线齿轮副结果是否有效</summary>
        private static void ValidateInvGearResult(Report report)
        {
            var r = report.InvGear;
            if (r is null) return;
            bool hasSun = GetInvValue(r.太阳轮, "计算接触应力", out double sunStress) && sunStress > 0;
            bool hasPlanet = GetInvValue(r.行星轮, "计算接触应力", out double planetStress) && planetStress > 0;
            if (!hasSun || !hasPlanet)
                report.Errors.Add("渐开线齿轮副结果无效（接触应力缺失或为 0），可能 DLL 调用失败");
        }

        /// <summary>校验摆线针轮副结果是否有效</summary>
        private static void ValidateCycloidGearResult(Report report)
        {
            var r = report.CycloidGear;
            if (r is null) return;
            if (r.接触应力 <= 0)
                report.Errors.Add("摆线针轮副结果无效（接触应力为 0），可能 DLL 调用失败");
        }

        /// <summary>从渐开线输出字典取字段值</summary>
        private static bool GetInvValue(Dictionary<string, double> dict, string key, out double value)
            => dict.TryGetValue(key, out value);

        /// <summary>解析命令行参数中的 --config &lt;path&gt;（支持 --config=path 形式）</summary>
        private static string? ParseConfigArg(string[] args)
        {
            for (int i = 0; i < args.Length; i++)
            {
                if (args[i] == "--config" || args[i] == "-c")
                {
                    // --config path 或 --config=path
                    if (i + 1 < args.Length)
                        return args[i + 1];
                    throw new InvalidOperationException("--config 参数缺少文件路径");
                }
                if (args[i].StartsWith("--config=", StringComparison.OrdinalIgnoreCase))
                    return args[i]["--config=".Length..];
            }
            return null;
        }

        /// <summary>从当前运行目录向上查找工作区根目录</summary>
        /// <summary>供 UI 验证类使用：定位工作区根目录（找不到时返回 AppContext.BaseDirectory）</summary>
        public static string FindWorkspaceRootForVerify()
        {
            try { return FindWorkspaceRoot(); }
            catch { return AppContext.BaseDirectory; }
        }

        /// <summary>供 UI 验证类使用：定位 WPF 应用 exe</summary>
        public static string? FindWpfAppExeForVerify(string workspaceRoot)
            => FindWpfAppExe(workspaceRoot);

        private static string FindWorkspaceRoot()
        {
            var current = AppContext.BaseDirectory;
            var root = new DirectoryInfo(current);
            while (root is not null)
            {
                if (File.Exists(Path.Combine(root.FullName, "RVReducerBasic.sln")))
                    return root.FullName;
                root = root.Parent;
            }
            throw new InvalidOperationException(
                $"无法定位工作区根目录（未找到 RVReducerBasic.sln），请从工作区内运行本工具");
        }

        /// <summary>
        /// 将 WPF 应用输出目录中的所有 DLL 复制到本工具运行目录。
        /// 注意：RVReducerBasic.dll 依赖 Intel MKL（40+ 个 CPU 分派 DLL）、MySQL 客户端等，
        /// 逐个列举容易遗漏，因此直接复制整个目录的全部 DLL。
        /// </summary>
        internal static void EnsureDllsAvailable(string workspaceRoot)
        {
            var appBinDir = Path.Combine(workspaceRoot,
                "RVReducerDesignApp", "bin", "Debug", "net8.0-windows");

            if (!Directory.Exists(appBinDir))
            {
                throw new InvalidOperationException(
                    $"未找到应用输出目录: {appBinDir}，请先编译 RVReducerDesignApp (Debug)");
            }

            var coreDll = Path.Combine(appBinDir, "RVReducerBasic.dll");
            if (!File.Exists(coreDll))
                throw new InvalidOperationException($"必需 DLL 不存在: {coreDll}，请先编译 RVReducerBasic (Debug|x64)");

            // 复制全部 DLL（含 MKL 分派、MySQL 客户端、VC 运行库等原生依赖）
            var copied = 0;
            foreach (var file in Directory.GetFiles(appBinDir, "*.dll"))
            {
                var dest = Path.Combine(AppContext.BaseDirectory, Path.GetFileName(file));
                try
                {
                    File.Copy(file, dest, overwrite: true);
                    copied++;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"  ⚠️ 复制 {Path.GetFileName(file)} 失败: {ex.Message}");
                }
            }
            Console.WriteLine($"  ✓ 已复制 {copied} 个 DLL 到运行目录");
        }

        /// <summary>查找 WPF 应用可执行文件</summary>
        private static string? FindWpfAppExe(string workspaceRoot)
        {
            var exePath = Path.Combine(workspaceRoot,
                "RVReducerDesignApp", "bin", "Debug", "net8.0-windows", "RVReducerDesignApp.exe");
            return File.Exists(exePath) ? exePath : null;
        }
    }
}
