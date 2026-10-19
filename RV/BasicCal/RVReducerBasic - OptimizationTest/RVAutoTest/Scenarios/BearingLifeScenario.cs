using System;
using System.IO;
using System.Text.Json;
using System.Threading;
using System.Windows.Automation;
using RVAutoTest.Models;

namespace RVAutoTest.Scenarios
{
    /// <summary>
    /// 场景 3: 圆锥滚子轴承寿命计算。
    /// 通过 DLL CalCustomBearingLife 直接计算（可靠、精确），
    /// 并通过 UIA 驱动 UI 展示（尽力而为截图）。
    ///
    /// 输入参考：SKF 30202 圆锥滚子轴承典型参数。
    /// </summary>
    public static class BearingLifeScenario
    {
        // ═══════════════════ DLL 输入索引常量 ═══════════════════
        // 与 CTANBearing.BuildCustomBearingInData 保持一致
        private const int I_TYPE_CODE = 0;
        private const int I_LAYOUT = 1;
        private const int I_ASSEMBLY_COUNT = 2;
        private const int I_CR = 3;
        private const int I_C0R = 4;
        private const int I_DPW = 5;
        private const int I_DW = 6;
        private const int I_ALPHA = 7;
        private const int I_ROWS = 8;
        private const int I_Z = 9;
        private const int I_BM = 10;
        private const int I_L_TOTAL = 11;
        private const int I_R = 12;
        private const int I_SUB_TYPE = 13;
        private const int I_BETA = 14;
        private const int I_RL = 15;
        private const int I_RS = 16;
        private const int I_FR = 28;
        private const int I_FA = 29;
        private const int I_N = 30;
        private const int I_CU = 31;
        private const int I_V = 32;
        private const int I_CONTAM = 33;
        private const int I_LAMBDA = 34;
        private const int I_EP = 35;
        private const int I_RELIABILITY = 36;

        /// <summary>执行圆锥滚子轴承寿命场景</summary>
        public static BearingLifeResult Run(string outputDir, AutomationElement? window)
        {
            var result = new BearingLifeResult();
            Directory.CreateDirectory(outputDir);

            // ── 1. 构建 SKF 30202 圆锥滚子轴承输入参数 ──
            var input = new double[37];
            input[I_TYPE_CODE] = 3.0;       // RadialRoller 大类
            input[I_LAYOUT] = 0.0;          // 布置方式
            input[I_ASSEMBLY_COUNT] = 1.0;  // 装配数量
            input[I_CR] = 18500.0;          // 基本额定动载荷 Cr (N) [SKF Explorer 30202 官方值]
            input[I_C0R] = 14600.0;         // 基本额定静载荷 C0r (N) [SKF Explorer 30202 官方值]
            input[I_DPW] = 24.7;            // 节圆直径 Dpw (mm)
            input[I_DW] = 4.8;              // 滚动体直径 Dw (mm)
            input[I_ALPHA] = 13.0;          // 接触角 α (°) [SKF 官方公称接触角]
            input[I_ROWS] = 1.0;            // 列数
            input[I_Z] = 14.0;              // 每列滚子数 Z
            input[I_BM] = 1.15;             // bm 结构系数
            input[I_L_TOTAL] = 9.0;         // 滚子总长 L (mm)
            input[I_R] = 0.34;              // 倒角 r (mm)
            input[I_SUB_TYPE] = 1.0;        // subType=1 圆锥滚子
            input[I_BETA] = 4.183;          // 滚子锥度角 β (°)
            input[I_RL] = 0.28;             // 大端倒角 rl (mm)
            input[I_RS] = 0.28;             // 小端倒角 rs (mm)
            input[I_FR] = 1500.0;           // 径向力 Fr (N)
            input[I_FA] = 300.0;            // 轴向力 Fa (N)
            input[I_N] = 1000.0;            // 转速 n (rpm)
            input[I_CU] = 0.0;              // 疲劳极限载荷 Cu (N)
            input[I_V] = 0.0;               // 润滑油工作黏度 (mm²/s)
            input[I_CONTAM] = 0.0;          // 污染等级
            input[I_LAMBDA] = 0.0;          // 膜厚比
            input[I_EP] = 0.0;              // 极压添加剂
            input[I_RELIABILITY] = 90.0;    // 可靠度 (%)
            result.Input = input;

            // ── 2. 直接调用 DLL 计算 ──
            // 说明: DllInvoker.CallCalCustomBearingLife 内部已校验返回值 (0=成功)，
            // 成功后 OutData[0] 恒为 1.0（对应 C++ PackOutputData 契约）。
            try
            {
                result.Output = DllInvoker.CallCalCustomBearingLife(input);

                Console.WriteLine($"  📊 L10h = {result.L10h:F2} h");
                Console.WriteLine($"  📊 S0 = {result.S0:F2}");
                Console.WriteLine($"  📊 C = {result.C:F1} N, P = {result.P:F1} N");
                Console.WriteLine($"  📊 L10r = {result.L10r:F2} (10^6转)");
                Console.WriteLine($"  📊 Lnm = {result.Lnm:F2} (10^6转)");
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"圆锥滚子轴承 DLL 调用失败: {ex.Message}", ex);
            }

            // ── 3. UIA 驱动 UI（尽力而为）──
            if (window is not null)
            {
                try { CaptureBearingUI(window, outputDir, result); }
                catch (Exception ex)
                {
                    Console.WriteLine($"⚠️ 轴承 UI 截图失败: {ex.Message}");
                }
            }

            // ── 4. 保存结果 ──
            SaveJson(result, outputDir, "tapered-bearing-result.json");
            Console.WriteLine("✅ 场景3 圆锥滚子轴承寿命完成");
            return result;
        }

        /// <summary>
        /// 通过 UIA 定位轴承树节点并截图。
        /// 由于自定义轴承的完整填表操作复杂且脆弱，此处仅做定位与窗口截图。
        /// </summary>
        private static void CaptureBearingUI(
            AutomationElement window, string outputDir, BearingLifeResult result)
        {
            // 尝试在左侧树中定位"轴承"节点
            var bearingNode = UIAutomator.FindTreeItemByName(window, "轴承");
            if (bearingNode is not null)
            {
                // 截取轴承节点所在区域（包含树）
                var treeFile = Path.Combine(outputDir, "06-bearing-tree.png");
                UIAutomator.CaptureElement(bearingNode, treeFile);
                result.Screenshots.Add(treeFile);
                Console.WriteLine($"  📸 轴承树节点截图: {treeFile}");
            }

            // 截取整个窗口（可能包含轴承数据窗口）
            var windowFile = Path.Combine(outputDir, "07-bearing-life-result.png");
            UIAutomator.CaptureWindow(window, windowFile);
            result.Screenshots.Add(windowFile);
            Console.WriteLine($"  📸 轴承计算结果截图: {windowFile}");
        }

        private static void SaveJson(object data, string outputDir, string fileName)
        {
            var path = Path.Combine(outputDir, fileName);
            var json = JsonSerializer.Serialize(data, JsonOptions.Pretty);
            File.WriteAllText(path, json);
            Console.WriteLine($"  📄 结果已保存: {path}");
        }
    }
}
