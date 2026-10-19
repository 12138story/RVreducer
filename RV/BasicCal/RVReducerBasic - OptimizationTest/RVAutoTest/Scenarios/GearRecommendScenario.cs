using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading;
using System.Windows.Automation;
using RVAutoTest.Models;

namespace RVAutoTest.Scenarios
{
    /// <summary>
    /// 场景 1: 齿轮推荐。
    /// 通过 DLL RecommondPara 获取精确计算结果，
    /// 并通过 UIA 驱动 WPF 应用截图 UI 面板。
    /// </summary>
    public static class GearRecommendScenario
    {
        /// <summary>执行齿轮推荐场景</summary>
        /// <param name="outputDir">输出目录</param>
        /// <param name="window">WPF 应用主窗口（可为 null，则跳过 UI 截图）</param>
        /// <param name="config">外部测试配置（null 时使用默认参数）</param>
        public static GearRecommendResult Run(string outputDir, AutomationElement? window, TestConfig? config = null)
        {
            var result = new GearRecommendResult();

            // ── 1. 构建输入参数（默认来自 SetGearContentDrawer.TmpTest()，可用配置覆盖）──
            var input = ConfigLoader.ResolveGear(config);
            result.Input = input;

            double[] inData = new double[12]
            {
                input.FixElemNotation, input.TotalRatio, input.OutputTorque,
                input.OutputRPM, input.TargetStress, input.DesignLife,
                input.UsageFactor, input.AllowableStress, input.PlanetCount,
                input.CycloidMinWidth, input.CycloidMaxWidth, 1  // 启用约束
            };

            // ── 2. 直接调用 DLL 计算 ──
            double[] outData;
            try
            {
                outData = DllInvoker.CallRecommondPara(inData);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"齿轮推荐 DLL 调用失败: {ex.Message}", ex);
            }

            // 保存原始 OutData 到磁盘，供后续渐开线/摆线计算数据闭环
            Directory.CreateDirectory(outputDir);
            File.WriteAllText(Path.Combine(outputDir, "gear-recommend-outdata.json"),
                JsonSerializer.Serialize(outData, JsonOptions.Pretty));

            // ── 3. 解析输出（完整字段，映射参考 SetGearContentDrawer.UpdateRVSetParameters）──
            result.法向模数 = outData[0];
            result.压力角 = outData[1];
            result.螺旋角 = outData[2];
            result.中心距 = outData[3];
            result.齿轮副类型 = outData[4];
            result.行星轮个数 = outData[5];
            result.驱动齿轮 = outData[6];
            result.触点间隙_j_bn = outData[7];
            result.转速 = outData[8];

            // 齿轮1 (OutData[9..39]) 和 齿轮2 (OutData[40..70])，步长 31
            for (int i = 0; i < 2; i++)
            {
                int baseIdx = 9 + i * 31;
                var gearDict = i == 0 ? result.齿轮1 : result.齿轮2;
                for (int f = 0; f < GearOutputFieldNames.GearFields.Length; f++)
                {
                    gearDict[GearOutputFieldNames.GearFields[f]] = outData[baseIdx + f];
                }
            }

            // 滚针 (OutData[71..76])
            for (int f = 0; f < GearOutputFieldNames.CycloidGearFields.Length; f++)
            {
                result.滚针[GearOutputFieldNames.CycloidGearFields[f]] = outData[71 + f];
            }

            // 摆线轮1 (OutData[77..82]) 和 摆线轮2 (OutData[83..88])，步长 6
            for (int i = 0; i < 2; i++)
            {
                int baseIdx = 77 + i * 6;
                var cycDict = i == 0 ? result.摆线轮1 : result.摆线轮2;
                for (int f = 0; f < GearOutputFieldNames.CycloidPinionFields.Length; f++)
                {
                    cycDict[GearOutputFieldNames.CycloidPinionFields[f]] = outData[baseIdx + f];
                }
            }

            // 其他 (OutData[89..90])
            result.扭矩 = outData[89];
            result.最终转速 = outData[90];

            // ── 4. UIA 操作与截图（尽力而为，失败不阻塞）──
            if (window is not null)
            {
                try
                {
                    // 1. 打开齿轮推荐抽屉
                    var gearButton = UIAutomator.FindElement(window, name: "齿轮推荐");
                    if (gearButton is null) { Console.WriteLine("⚠️ 未找到'齿轮推荐'按钮"); }
                    else
                    {
                        UIAutomator.ClickButton(gearButton);
                        Thread.Sleep(800);

                        // 2. 勾选"启用摆线轮齿宽约束"
                        var cb = UIAutomator.FindElement(window, name: "启用摆线轮齿宽约束");
                        if (cb is not null) { UIAutomator.SetCheckBox(cb, true); Thread.Sleep(500); Console.WriteLine("  ☑️ 已勾选齿宽约束"); }

                        // 3. 设置摆线轮齿宽（使用配置值，与 DLL 计算一致）
                        SetCycloidWidth(window, input.CycloidMinWidth, input.CycloidMaxWidth);

                        // 4. 截图抽屉面板（点确定前，此时抽屉已打开且参数已填）
                        var panelFile = Path.Combine(outputDir, "01-gear-recommend-panel.png");
                        UIAutomator.CaptureWindow(window, panelFile);
                        result.Screenshots.Add(panelFile);
                        Console.WriteLine($"  📸 齿轮推荐抽屉面板截图: {panelFile}");

                        // 5. 点"确定"执行推荐
                        var okButton = UIAutomator.FindElement(window, name: "确定", controlType: ControlType.Button);
                        if (okButton is not null) { UIAutomator.ClickButton(okButton); Thread.Sleep(1000); }
                        else { Console.WriteLine("  ⚠️ 未找到'确定'按钮"); }
                    }

                    // 6. 截图整个窗口（显示推荐结果）
                    var windowFile = Path.Combine(outputDir, "02-gear-recommend-output.png");
                    UIAutomator.CaptureWindow(window, windowFile);
                    result.Screenshots.Add(windowFile);
                    Console.WriteLine($"  📸 齿轮推荐结果截图: {windowFile}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"⚠️ 齿轮推荐 UI 操作失败: {ex.Message}");
                }
            }

            // ── 5. 保存结果 ──
            SaveJson(result, outputDir, "gear-recommend-result.json");
            Console.WriteLine("✅ 场景1 齿轮推荐完成");
            return result;
        }

        /// <summary>
        /// 通过 UIA 打开齿轮推荐抽屉并执行推荐。
        /// 操作顺序：打开抽屉 → 勾选"启用摆线轮齿宽约束" → 设置摆线轮齿宽 → 点"确定"执行推荐。
        /// 供 Run 与 ShaftRecommendScenario 复用（轴推荐失败重试时以更大齿宽重新执行）。
        /// </summary>
        /// <param name="minWidth">摆线轮最小齿宽 (mm)，默认 40</param>
        /// <param name="maxWidth">摆线轮最大齿宽 (mm)，默认 80</param>
        /// <returns>齿宽设置失败原因（成功时返回 null）</returns>
        public static string? RunGearRecommendUI(AutomationElement window, string outputDir,
            double minWidth = 40, double maxWidth = 80)
        {
            if (minWidth <= 0 || maxWidth <= 0 || minWidth >= maxWidth)
                throw new ArgumentException($"齿宽参数非法: min={minWidth}, max={maxWidth}");

            // 1. 打开齿轮推荐抽屉
            var gearButton = UIAutomator.FindElement(window, name: "齿轮推荐");
            if (gearButton is null)
            {
                Console.WriteLine("⚠️ 未找到'齿轮推荐'按钮，跳过 UI 操作");
                return "未找到'齿轮推荐'按钮";
            }
            UIAutomator.ClickButton(gearButton);
            Thread.Sleep(800);

            // 2. 勾选"启用摆线轮齿宽约束"复选框（防止摆线轮宽度过窄导致轴推荐失败）
            var widthConstraintCheckBox = UIAutomator.FindElement(window, name: "启用摆线轮齿宽约束");
            if (widthConstraintCheckBox is not null)
            {
                UIAutomator.SetCheckBox(widthConstraintCheckBox, true);
                Thread.Sleep(500);
                Console.WriteLine("  ☑️ 已勾选'启用摆线轮齿宽约束'");
            }
            else
            {
                Console.WriteLine("  ⚠️ 未找到'启用摆线轮齿宽约束'复选框");
            }

            // 3. 设置摆线轮齿宽输入值
            var widthFailure = SetCycloidWidth(window, minWidth, maxWidth);

            // 4. 点"确定"执行推荐
            var okButton = UIAutomator.FindElement(window, name: "确定", controlType: ControlType.Button);
            if (okButton is not null)
            {
                UIAutomator.ClickButton(okButton);
                Thread.Sleep(1000); // 等待推荐计算完成
            }
            else
            {
                Console.WriteLine("  ⚠️ 未找到'确定'按钮，未执行齿轮推荐");
                return "未找到齿轮推荐'确定'按钮";
            }

            return widthFailure;
        }

        /// <summary>
        /// 通过 UIA 设置摆线轮最小/最大齿宽输入框（几何匹配定位）。
        /// </summary>
        /// <returns>失败原因（全部设置成功时返回 null）</returns>
        private static string? SetCycloidWidth(AutomationElement window, double minWidth, double maxWidth)
        {
            var failures = new List<string>();
            try
            {
                var minEdit = UIAutomator.FindEditAfterLabel(window, "摆线轮最小齿宽");
                var maxEdit = UIAutomator.FindEditAfterLabel(window, "摆线轮最大齿宽");

                if (minEdit is not null)
                {
                    UIAutomator.SetTextBoxValue(minEdit, minWidth.ToString("F0"));
                    Console.WriteLine($"  ⌨️ 设置摆线轮最小齿宽 = {minWidth:F0} mm");
                }
                else
                {
                    Console.WriteLine("  ⚠️ 未定位到'摆线轮最小齿宽'输入框");
                    failures.Add("未定位到'摆线轮最小齿宽'输入框");
                }

                if (maxEdit is not null)
                {
                    UIAutomator.SetTextBoxValue(maxEdit, maxWidth.ToString("F0"));
                    Console.WriteLine($"  ⌨️ 设置摆线轮最大齿宽 = {maxWidth:F0} mm");
                }
                else
                {
                    Console.WriteLine("  ⚠️ 未定位到'摆线轮最大齿宽'输入框");
                    failures.Add("未定位到'摆线轮最大齿宽'输入框");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"  ⚠️ 设置摆线轮齿宽失败: {ex.Message}");
                failures.Add($"设置异常: {ex.Message}");
            }

            return failures.Count > 0 ? string.Join("; ", failures) : null;
        }

        private static void SaveJson(object data, string outputDir, string fileName)
        {
            Directory.CreateDirectory(outputDir);
            var path = Path.Combine(outputDir, fileName);
            var json = JsonSerializer.Serialize(data, JsonOptions.Pretty);
            File.WriteAllText(path, json);
            Console.WriteLine($"  📄 结果已保存: {path}");
        }
    }
}
