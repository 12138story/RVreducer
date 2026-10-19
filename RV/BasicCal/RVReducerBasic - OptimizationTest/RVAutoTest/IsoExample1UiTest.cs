using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Automation;

namespace RVAutoTest
{
    /// <summary>
    /// ISO/TR 6336-30:2022 Example 1 — UI 计算与结果导出测试。
    /// 将 ISO 参数填入 WPF 应用渐开线齿轮副页面，触发计算，按 Tab 导出结果并停留在计算结果页。
    /// 截图默认关闭，加 --screenshot 参数时截取所有输出/输入页面。
    /// 用法：--iso-example1-ui [--screenshot]
    /// </summary>
    public static class IsoExample1UiTest
    {
        private const string AppWindowTitle = Program.AppWindowTitle;
        // 每次运行生成独立时间戳文件夹。仅单次运行单线程使用，Run 开头重新初始化。
        private static string ScreenshotDir = "";

        /// <summary>输出页内层 Tab 顺序（截图与导出共用）</summary>
        private static readonly string[] OutputTabs =
            { "应力参数", "几何参数", "接触参数", "滑动参数", "齿廓参数", "特殊参数", "其他参数" };

        // ═══════════════════ 入口 ═══════════════════

        public static int Run(string[] args)
        {
            var workspaceRoot = Program.FindWorkspaceRootForVerify();
            var appExe = Program.FindWpfAppExeForVerify(workspaceRoot);
            if (appExe is null)
            {
                Console.WriteLine("❌ 未找到 RVReducerDesignApp.exe");
                return 2;
            }

            // 每次运行生成时间戳子文件夹：screenshots/iso-example1-ui/<yyyyMMdd-HHmmss>/（同秒重跑自动加序号）
            ScreenshotDir = Program.CreateTimestampedDir(workspaceRoot, "screenshots", "iso-example1-ui");

            // 截图开关：默认不截图，--screenshot 时额外截图；结果导出与 UI 操作始终执行
            bool takeScreenshot = args.Contains("--screenshot");
            Console.WriteLine(takeScreenshot
                ? "📷 截图模式已开启（--screenshot）"
                : "ℹ️ 默认不截图（如需截图请加 --screenshot 参数）");

            Console.WriteLine($"📁 输出目录: {ScreenshotDir}");

            Console.WriteLine($"启动 WPF 应用: {appExe}");
            var (process, window) = UIAutomator.LaunchAndWaitForWindow(appExe, AppWindowTitle);

            bool keepOpen = false; // 成功时保持应用运行（界面停留在计算结果页）
            try
            {
                // ── 1. 展开并选中"渐开线齿轮副"节点 ──
                var invGearNode = UIAutomator.FindTreeItemByName(window, "渐开线齿轮副");
                if (invGearNode is null)
                {
                    Console.WriteLine("❌ 未找到'渐开线齿轮副'树节点");
                    return 2;
                }
                UIAutomator.ClickButton(invGearNode);
                Thread.Sleep(800);
                Console.WriteLine("✅ 已选中'渐开线齿轮副'");

                // ── 2. 填写太阳轮参数 ──
                var sunNode = UIAutomator.FindTreeItemByName(window, "太阳轮");
                if (sunNode is null)
                {
                    Console.WriteLine("❌ 未找到'太阳轮'树节点");
                    return 2;
                }
                UIAutomator.ClickButton(sunNode);
                Thread.Sleep(1000);
                Console.WriteLine("✅ 已选中'太阳轮'，开始填入参数...");
                FillGearParams(window, isSun: true);

                // ── 3. 填写行星轮参数 ──
                var planetNode = UIAutomator.FindTreeItemByName(window, "行星轮");
                if (planetNode is null)
                {
                    Console.WriteLine("❌ 未找到'行星轮'树节点");
                    return 2;
                }
                UIAutomator.ClickButton(planetNode);
                Thread.Sleep(800);
                Console.WriteLine("✅ 已选中'行星轮'，开始填入参数...");
                FillGearParams(window, isSun: false);

                // ── 4. 填写载荷参数 ──
                var torqueNode = UIAutomator.FindTreeItemByName(window, "太阳轮载荷");
                if (torqueNode is null)
                {
                    Console.WriteLine("❌ 未找到'太阳轮载荷'树节点");
                    return 2;
                }
                UIAutomator.ClickButton(torqueNode);
                Thread.Sleep(800);
                Console.WriteLine("✅ 已选中'太阳轮载荷'，填入载荷参数...");
                FillTorqueParams(window);

                // ── 5. 回到渐开线齿轮副，触发计算 ──
                UIAutomator.ClickButton(invGearNode);
                Thread.Sleep(800);
                Console.WriteLine("✅ 回到'渐开线齿轮副'主页面");

                var calButton = UIAutomator.FindElement(window, name: "计算", controlType: ControlType.Button)
                               ?? UIAutomator.FindElement(window, automationId: "CalButton", controlType: ControlType.Button);
                if (calButton is null)
                {
                    Console.WriteLine("⚠️ 未找到'计算'按钮（按名称），尝试模糊查找...");
                    // 查找窗口下所有 Button，模糊匹配
                    var allBtns = UIAutomator.FindElements(window, controlType: ControlType.Button);
                    foreach (var btn in allBtns)
                    {
                        var n = btn.Current.Name ?? "";
                        if (n.Contains("计算") || n.Contains("Calc"))
                        {
                            calButton = btn;
                            Console.WriteLine($"  找到按钮: '{n}'");
                            break;
                        }
                    }
                }
                if (calButton is null)
                {
                    Console.WriteLine("❌ 未找到'计算'按钮");
                    return 2;
                }

                UIAutomator.ClickButton(calButton);
                Thread.Sleep(2000);
                Console.WriteLine("✅ 已触发计算");

                // ── 6. 处理弹窗（如有）——仅处理被测进程的弹窗 ──
                var msgBox = UIAutomator.FindMessageBox(process.Id, 2);
                if (msgBox is not null)
                {
                    var msgText = UIAutomator.GetMessageBoxText(msgBox);
                    Console.WriteLine($"  ⚠️ 弹窗: {msgText}");
                    UIAutomator.ClickMessageBoxButton(msgBox, "确定");
                    Thread.Sleep(500);
                }

                // ── 6.5 验证计算输出（读取"计算结果"外层 Tab → "应力参数"表）──
                VerifyOutput(window);

                // ── 7. 截图（可选，默认不截图）——失败不阻塞后续导出 ──
                if (takeScreenshot)
                {
                    Console.WriteLine("\n📸 开始截图...");
                    try { TakeScreenshots(window); }
                    catch (Exception ex) { Console.WriteLine($"  ⚠️ 截图过程异常，继续导出: {ex.Message}"); }
                }

                // ── 8. 导出计算结果（按 Tab 分类，默认执行）──
                Console.WriteLine("\n📄 导出计算结果...");
                var outputDataPath = Path.Combine(ScreenshotDir, "计算结果汇总.md");
                ExportOutputData(window, outputDataPath);

                // ── 9. 界面停留在计算结果页面 ──
                StayOnOutputPage(window);

                Console.WriteLine($"\n✅ UI 计算与结果导出完成");
                if (takeScreenshot)
                    Console.WriteLine($"✅ 截图已保存至: {ScreenshotDir}");
                else
                    Console.WriteLine("ℹ️ 未截图（默认不截图；如需截图请加 --screenshot 参数）");
                Console.WriteLine($"✅ 计算结果已导出: {outputDataPath}");
                Console.WriteLine("✅ 界面已停留在'渐开线齿轮副 → 计算结果'页面（应用保持运行）");
                keepOpen = true;
                return 0;
            }
            finally
            {
                if (keepOpen)
                {
                    // 成功：保持应用运行，不做任何进程操作（Dispose 可能有副作用）
                    Console.WriteLine("  [诊断] finally: keepOpen=true，未释放进程句柄");
                }
                else
                {
                    // 失败/异常：关闭进程，防止泄漏
                    if (!process.HasExited)
                    {
                        try { process.CloseMainWindow(); process.WaitForExit(5000); } catch { }
                    }
                    if (!process.HasExited)
                    {
                        try { process.Kill(); } catch { }
                    }
                    process.Dispose();
                }
            }
        }

        // ═══════════════════ 参数填入 ═══════════════════

        // ═══════════════════ 参数填入（批量匹配版） ═══════════════════

        /// <summary>收集页面上所有标签→输入框的映射</summary>
        private static Dictionary<string, AutomationElement> BuildLabelEditMap(AutomationElement window)
        {
            var map = new Dictionary<string, AutomationElement>();
            var allEdits = UIAutomator.FindElements(window, controlType: ControlType.Edit);

            // 第1遍：收集所有只读 Edit（标签）及其位置
            var labels = new List<(string text, double y, double right)>();
            foreach (var e in allEdits)
            {
                if (e.TryGetCurrentPattern(ValuePattern.Pattern, out object? vp))
                {
                    var v = (ValuePattern)vp;
                    if (v.Current.IsReadOnly && !string.IsNullOrEmpty(v.Current.Value))
                    {
                        var r = e.Current.BoundingRectangle;
                        if (!r.IsEmpty)
                            labels.Add((v.Current.Value, r.Top + r.Height / 2, r.Right));
                    }
                }
            }

            // 第2遍：为每个标签找最匹配的可编辑输入框
            foreach (var (text, y, right) in labels)
            {
                AutomationElement? best = null;
                double bestScore = double.MaxValue;
                foreach (var e in allEdits)
                {
                    if (e.TryGetCurrentPattern(ValuePattern.Pattern, out object? vp))
                    {
                        var v = (ValuePattern)vp;
                        if (v.Current.IsReadOnly) continue; // 跳过只读标签
                    }
                    var r = e.Current.BoundingRectangle;
                    if (r.IsEmpty) continue;
                    double editCenterY = r.Top + r.Height / 2;
                    double vertDiff = Math.Abs(editCenterY - y);
                    if (r.Left >= right && vertDiff < 25)
                    {
                        double score = vertDiff + (r.Left - right) * 0.01;
                        if (score < bestScore)
                        {
                            bestScore = score;
                            best = e;
                        }
                    }
                }
                if (best is not null && !map.ContainsKey(text))
                    map[text] = best;
            }

            Console.WriteLine($"  [诊断] 标签→输入框映射: 找到 {map.Count} 个可编辑字段");
            return map;
        }

        /// <summary>填写齿轮输入参数（太阳轮或行星轮页面）— 逐内层 Tab 切换填充</summary>
        private static void FillGearParams(AutomationElement window, bool isSun)
        {
            string prefix = isSun ? "太阳轮" : "行星轮";

            // 各内层 Tab 对应的 (文本字段, ComboBox)
            var tabs = new (string tab, Dictionary<string, string> fields, List<(string label, string text)> combos)[]
            {
                ("几何参数",
                    new Dictionary<string, string>
                    {
                        ["法向模数"] = "8",
                        ["压力角"] = "20",
                        ["螺旋角"] = "15.8",
                        ["中心距"] = "500",
                        ["齿数"] = isSun ? "17" : "103",
                        ["设计齿宽"] = "100",
                        ["齿顶高系数"] = "1",
                        ["齿廓变位系数"] = isSun ? "0.1178" : "0",
                        ["顶隙系数"] = "0.4",
                        ["齿根圆角系数"] = "0.39",
                        ["挖根量"] = "0",
                        ["齿顶修形量"] = "70",
                        ["齿根修形量"] = "0",
                        ["跨齿数"] = isSun ? "2" : "13",
                        ["齿顶倒角量"] = "0",
                    },
                    new List<(string, string)> { ("旋向", "左旋") }),
                ("物性参数",
                    new Dictionary<string, string>
                    {
                        ["弹性模量"] = "206000",
                        ["泊松比"] = "0.3",
                        ["屈服强度"] = "500",
                        ["抗拉强度"] = "1080",
                        ["试验齿轮接触疲劳极限应力"] = "1500",
                        ["试验齿轮弯曲疲劳极限应力"] = "500",
                        ["密度"] = "7.83e-6",
                    },
                    new List<(string, string)>()),
                ("加工参数",
                    new Dictionary<string, string>
                    {
                        ["表面硬度(HRC)"] = "60",
                        ["芯部硬度(HRC)"] = "30",
                    },
                    new List<(string, string)>
                    {
                        ("材料类型", "Eh表面硬化铸钢"),
                        ("加工质量", "MQ中等要求材料及热处理质量"),
                    }),
                ("应用参数",
                    new Dictionary<string, string>
                    {
                        ["使用系数"] = "1",
                        ["均载系数"] = "1",
                        ["润滑油粘度(v40)"] = "320",
                        ["接触最小安全系数"] = "1",
                        ["弯曲最小安全系数"] = "1",
                        ["行星轮个数"] = "1",
                    },
                    new List<(string, string)>
                    {
                        ("修形方法", "基于3维接触分析进行适当修形"),
                        ("螺旋线修形方式", "不修形"),
                        ("是否对接触区进行控制", "不控制"),
                        ("齿廓修形是否能够补偿变形", "是"),
                    }),
                ("精度参数",
                    new Dictionary<string, string>
                    {
                        ["接触表面粗糙度(Ra)"] = "1",
                        ["齿根表面粗糙度(Ra)"] = "3",
                    },
                    new List<(string, string)> { ("精度等级", "5级") }),
                ("安装参数",
                    new Dictionary<string, string>
                    {
                        ["太阳轮轴直径"] = "100",
                        ["太阳轮齿宽中点至轴承跨距中点距离"] = "0",
                        ["轴承跨距"] = "125",
                    },
                    new List<(string, string)>()),
            };

            foreach (var (tab, fields, combos) in tabs)
            {
                if (!SelectInnerTab(window, tab))
                {
                    Console.WriteLine($"  ⚠️ 未找到内层 Tab '{tab}'，跳过");
                    continue;
                }
                Thread.Sleep(500);

                var map = BuildLabelEditMap(window);
                foreach (var kv in fields)
                    SetMappedEdit(map, kv.Key, kv.Value);
                foreach (var (label, text) in combos)
                    SetComboByText(window, text, label);
            }

            // ── "几何参数" Tab 的"其他几何参数" Expander（齿条刀参数影响 σF）──
            SelectInnerTab(window, "几何参数");
            Thread.Sleep(400);
            ExpandExpander(window, "其他几何参数");
            Thread.Sleep(400);
            var geoMap = BuildLabelEditMap(window);
            // 齿条刀齿顶高系数 hfP/mn=1.4、齿顶圆半径系数 ρfP/mn=0.39、行星轮个数=1
            SetMappedEdit(geoMap, "齿条刀的齿顶高系数", "1.4");
            SetMappedEdit(geoMap, "齿条刀的齿顶圆半径系数", "0.39");
            SetMappedEdit(geoMap, "行星轮个数", "1");
            SetMappedEdit(geoMap, "齿轮副类型", "1");

            Console.WriteLine($"  ✅ {prefix} 参数填入完成");
        }

        /// <summary>选择内层 TabItem（按标题）</summary>
        private static bool SelectInnerTab(AutomationElement root, string tabName)
        {
            var tab = UIAutomator.FindElement(root, name: tabName, controlType: ControlType.TabItem, timeoutSec: 5);
            if (tab is null) return false;
            try
            {
                if (tab.TryGetCurrentPattern(SelectionItemPattern.Pattern, out object? sel))
                {
                    ((SelectionItemPattern)sel).Select();
                    return true;
                }
            }
            catch { }
            return false;
        }

        /// <summary>展开 Expander（如"其他几何参数"）</summary>
        private static void ExpandExpander(AutomationElement root, string expanderName)
        {
            var exp = UIAutomator.FindElement(root, name: expanderName, controlType: ControlType.Group, timeoutSec: 5);
            if (exp is null)
                exp = UIAutomator.FindElement(root, name: expanderName, timeoutSec: 5);
            if (exp is null)
            {
                Console.WriteLine($"  ⚠️ 未找到 Expander '{expanderName}'");
                return;
            }
            try
            {
                if (exp.TryGetCurrentPattern(ExpandCollapsePattern.Pattern, out object? ec))
                {
                    var e = (ExpandCollapsePattern)ec;
                    if (e.Current.ExpandCollapseState != ExpandCollapseState.Expanded)
                    {
                        e.Expand();
                        Console.WriteLine($"  🔧 已展开 '{expanderName}'");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"  ⚠️ 展开 '{expanderName}' 失败: {ex.Message}");
            }
        }

        /// <summary>从映射中设置 Edit 值</summary>
        private static void SetMappedEdit(Dictionary<string, AutomationElement> map, string label, string value)
        {
            if (map.TryGetValue(label, out var edit))
            {
                try
                {
                    UIAutomator.SetTextBoxValue(edit, value);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"  ⚠️ 设置 '{label}' = {value} 失败: {ex.Message}");
                }
            }
            else
            {
                Console.WriteLine($"  ⚠️ 未找到字段 '{label}' 对应的输入框，跳过");
            }
        }

        /// <summary>填写载荷参数（太阳轮载荷页面）</summary>
        private static void FillTorqueParams(AutomationElement window)
        {
            var map = BuildLabelEditMap(window);
            SetMappedEdit(map, "扭矩", "9000");
            SetMappedEdit(map, "转速", "360");
            SetMappedEdit(map, "预期寿命", "50000");
            Console.WriteLine("  ✅ 载荷参数填入完成");
        }

        /// <summary>通过 ComboBox 文本选择某个选项（标签为只读 Edit，按 Y 位置匹配 ComboBox）。返回是否成功</summary>
        private static bool SetComboByText(AutomationElement root, string itemText, string labelText)
        {
            // 找标签（只读 Edit，Value=标签文本）
            var edits = UIAutomator.FindElements(root, controlType: ControlType.Edit);
            AutomationElement? label = null;
            double labelY = 0;
            foreach (var e in edits)
            {
                if (e.TryGetCurrentPattern(ValuePattern.Pattern, out object? vp))
                {
                    var v = (ValuePattern)vp;
                    if (v.Current.IsReadOnly && v.Current.Value == labelText)
                    {
                        label = e;
                        labelY = e.Current.BoundingRectangle.Top + e.Current.BoundingRectangle.Height / 2;
                        break;
                    }
                }
            }
            if (label is null)
            {
                Console.WriteLine($"  ⚠️ 未找到标签 '{labelText}'，跳过 ComboBox 设置");
                return false;
            }

            var combos = UIAutomator.FindElements(root, controlType: ControlType.ComboBox);
            AutomationElement? best = null;
            double bestDist = double.MaxValue;
            foreach (var cb in combos)
            {
                var r = cb.Current.BoundingRectangle;
                if (r.IsEmpty) continue;
                double comboY = r.Top + r.Height / 2;
                double dist = Math.Abs(comboY - labelY);
                if (dist < bestDist && dist < 30)
                {
                    bestDist = dist;
                    best = cb;
                }
            }

            if (best is null)
            {
                Console.WriteLine($"  ⚠️ 未找到 '{labelText}' 旁的 ComboBox，跳过");
                return false;
            }

            try
            {
                UIAutomator.SelectComboBoxItem(best, itemText);
                Console.WriteLine($"  🔧 ComboBox '{labelText}' → '{itemText}'");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"  ⚠️ ComboBox '{labelText}' 选择 '{itemText}' 失败: {ex.Message}");
                return false;
            }
        }

        // ═══════════════════ 截图 ═══════════════════

        private static void TakeScreenshots(AutomationElement window)
        {
            int seq = 0;

            // ── 输出页截图（外层"计算结果"Tab + 内层输出 Tab 遍历）──
            // 先回到"渐开线齿轮副"树节点（输出属于齿轮副）
            var invGearNode = UIAutomator.FindTreeItemByName(window, "渐开线齿轮副");
            if (invGearNode is not null)
            {
                UIAutomator.ClickButton(invGearNode);
                Thread.Sleep(500);
            }

            // 切换到外层"计算结果"Tab
            if (SelectOuterTab(window, "计算结果"))
            {
                Thread.Sleep(500);
                Console.WriteLine("  ✅ 已切换到'计算结果'外层 Tab");

                // 遍历内层输出 Tab
                foreach (var tab in OutputTabs)
                {
                    if (!SelectInnerTab(window, tab))
                    {
                        Console.WriteLine($"  ⚠️ 未找到输出 Tab '{tab}'，跳过");
                        continue;
                    }
                    Thread.Sleep(400);
                    try
                    {
                        var path = Path.Combine(ScreenshotDir, $"{++seq:D2}-output-{tab}.png");
                        UIAutomator.CaptureWindow(window, path);
                        Console.WriteLine($"  📸 [{seq}] 输出-{tab}: {path}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"  ⚠️ 截图输出-{tab} 失败: {ex.Message}");
                    }
                }
            }
            else
            {
                Console.WriteLine("  ⚠️ 未找到'计算结果'外层 Tab，改为直接截当前窗口");
                try
                {
                    var path = Path.Combine(ScreenshotDir, $"{++seq:D2}-invgear-output.png");
                    UIAutomator.CaptureWindow(window, path);
                    Console.WriteLine($"  📸 [{seq}] 渐开线齿轮副输出: {path}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"  ⚠️ 截图失败: {ex.Message}");
                }
            }

            // ── 输入页截图 ──
            // 太阳轮输入页
            var sunNode = UIAutomator.FindTreeItemByName(window, "太阳轮");
            if (sunNode is not null)
            {
                UIAutomator.ClickButton(sunNode);
                Thread.Sleep(600);
                try
                {
                    var path = Path.Combine(ScreenshotDir, $"{++seq:D2}-sun-gear-input.png");
                    UIAutomator.CaptureWindow(window, path);
                    Console.WriteLine($"  📸 [{seq}] 太阳轮输入页: {path}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"  ⚠️ 截图太阳轮失败: {ex.Message}");
                }
            }

            // 行星轮输入页
            var planetNode = UIAutomator.FindTreeItemByName(window, "行星轮");
            if (planetNode is not null)
            {
                UIAutomator.ClickButton(planetNode);
                Thread.Sleep(600);
                try
                {
                    var path = Path.Combine(ScreenshotDir, $"{++seq:D2}-planet-gear-input.png");
                    UIAutomator.CaptureWindow(window, path);
                    Console.WriteLine($"  📸 [{seq}] 行星轮输入页: {path}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"  ⚠️ 截图行星轮失败: {ex.Message}");
                }
            }

            // 太阳轮载荷页
            var torqueNode = UIAutomator.FindTreeItemByName(window, "太阳轮载荷");
            if (torqueNode is not null)
            {
                UIAutomator.ClickButton(torqueNode);
                Thread.Sleep(600);
                try
                {
                    var path = Path.Combine(ScreenshotDir, $"{++seq:D2}-torque-input.png");
                    UIAutomator.CaptureWindow(window, path);
                    Console.WriteLine($"  📸 [{seq}] 太阳轮载荷页: {path}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"  ⚠️ 截图载荷失败: {ex.Message}");
                }
            }

            // 回到渐开线齿轮副输出页（外层"计算结果"Tab，内层"应力参数"）
            if (invGearNode is not null)
            {
                UIAutomator.ClickButton(invGearNode);
                Thread.Sleep(500);
                SelectOuterTab(window, "计算结果");
                Thread.Sleep(500);
                SelectInnerTab(window, "应力参数");
                Thread.Sleep(400);
                try
                {
                    var path = Path.Combine(ScreenshotDir, $"{++seq:D2}-invgear-output-final.png");
                    UIAutomator.CaptureWindow(window, path);
                    Console.WriteLine($"  📸 [{seq}] 渐开线齿轮副输出(终): {path}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"  ⚠️ 截图最终输出失败: {ex.Message}");
                }
            }
        }

        // ═══════════════════ 输出验证 ═══════════════════

        /// <summary>切换到"计算结果"外层 Tab，读取输出表格中的关键应力值并打印</summary>
        private static void VerifyOutput(AutomationElement window)
        {
            if (!SelectOuterTab(window, "计算结果"))
            {
                Console.WriteLine("  ⚠️ 验证输出：未找到'计算结果'Tab");
                return;
            }
            Thread.Sleep(600);

            // 读输出表格（DataGrid / Table）
            var table = window.FindFirst(TreeScope.Descendants,
                new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.DataGrid));
            if (table is null)
                table = window.FindFirst(TreeScope.Descendants,
                    new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Table));

            if (table is null)
            {
                Console.WriteLine("  ⚠️ 验证输出：未找到输出表格");
                return;
            }

            // 读取表头（第 0 行）和值（第 1 行）定位应力字段
            // 应力表列: 名称|单位|太阳轮|行星轮
            var items = table.FindAll(TreeScope.Descendants,
                new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.DataItem));
            Console.WriteLine($"  [验证] 输出表格共 {items.Count} 行");
            int printed = 0;
            foreach (AutomationElement item in items)
            {
                var cells = item.FindAll(TreeScope.Descendants,
                    new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Text));
                var cellTexts = new List<string>();
                foreach (AutomationElement c in cells)
                {
                    var t = c.Current.Name;
                    if (!string.IsNullOrEmpty(t)) cellTexts.Add(t);
                }
                if (cellTexts.Count >= 2)
                {
                    string name = cellTexts[0];
                    if (name.Contains("接触应力") || name.Contains("弯曲应力"))
                    {
                        string sun = cellTexts.Count > 2 ? cellTexts[2] : "";
                        string planet = cellTexts.Count > 3 ? cellTexts[3] : "";
                        Console.WriteLine($"  [验证] {name}: 太阳轮={sun} 行星轮={planet}");
                        if (++printed >= 6) break;
                    }
                }
            }

            // 回到"参数输入"Tab
            SelectOuterTab(window, "参数输入");
        }

        // ═══════════════════ 结果导出 ═══════════════════

        /// <summary>将计算结果按不同 Tab 分类导出到 Markdown 文件（每 Tab 一个章节）</summary>
        private static void ExportOutputData(AutomationElement window, string outputPath)
        {
            if (!SelectOuterTab(window, "计算结果"))
            {
                Console.WriteLine("  ⚠️ 导出输出：未找到'计算结果'Tab");
                return;
            }
            Thread.Sleep(500);

            var sb = new System.Text.StringBuilder();
            sb.AppendLine("# ISO/TR 6336-30:2022 Example 1 — 计算结果汇总");
            sb.AppendLine();
            sb.AppendLine($"> 生成时间: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
            sb.AppendLine("> 输入: z1=17, z2=103, mn=8mm, αn=20°, β=15.8°, a=500mm, x1=0.1178");
            sb.AppendLine("> 载荷: T1=9000 N·m, n1=360 r/min, Lh=50000 h");
            sb.AppendLine();

            foreach (var tab in OutputTabs)
            {
                if (!SelectInnerTab(window, tab))
                {
                    Console.WriteLine($"  ⚠️ 导出：未找到输出 Tab '{tab}'，跳过");
                    continue;
                }
                Thread.Sleep(400);

                var rows = ReadOutputTable(window);
                sb.AppendLine($"## {tab}（{rows.Count} 项）");
                sb.AppendLine();
                sb.AppendLine("| 名称 | 单位 | 太阳轮 | 行星轮 |");
                sb.AppendLine("|------|------|--------|--------|");
                foreach (var row in rows)
                {
                    string Md(string s) => s.Replace("|", "\\|").Replace("\n", " ").Trim();
                    string name = row.Count > 0 ? Md(row[0]) : "";
                    string unit = row.Count > 1 ? Md(row[1]) : "";
                    string sun = row.Count > 2 ? Md(row[2]) : "";
                    string planet = row.Count > 3 ? Md(row[3]) : "";
                    sb.AppendLine($"| {name} | {unit} | {sun} | {planet} |");
                }
                sb.AppendLine();
                Console.WriteLine($"  📄 已导出 Tab '{tab}': {rows.Count} 行");
            }

            File.WriteAllText(outputPath, sb.ToString(), System.Text.Encoding.UTF8);
        }

        /// <summary>读取当前可见输出 DataGrid 的所有行数据（每行: 名称/单位/太阳轮/行星轮）</summary>
        private static List<List<string>> ReadOutputTable(AutomationElement window)
        {
            var result = new List<List<string>>();
            var table = window.FindFirst(TreeScope.Descendants,
                new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.DataGrid));
            if (table is null)
                table = window.FindFirst(TreeScope.Descendants,
                    new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Table));
            if (table is null) return result;

            try
            {
                var items = table.FindAll(TreeScope.Descendants,
                    new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.DataItem));
                foreach (AutomationElement item in items)
                {
                    var row = new List<string>();
                    try
                    {
                        var cells = item.FindAll(TreeScope.Descendants,
                            new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Text));
                        foreach (AutomationElement c in cells)
                        {
                            var t = c.Current.Name;
                            if (!string.IsNullOrEmpty(t)) row.Add(t);
                        }
                    }
                    catch (Exception) { /* 陈旧元素，跳过该行 */ }
                    if (row.Count > 0) result.Add(row);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"  ⚠️ 读取输出表格失败: {ex.Message}");
            }
            return result;
        }

        /// <summary>界面停留在"渐开线齿轮副 → 计算结果 → 应力参数"页面</summary>
        private static void StayOnOutputPage(AutomationElement window)
        {
            var invGearNode = UIAutomator.FindTreeItemByName(window, "渐开线齿轮副");
            if (invGearNode is not null)
            {
                UIAutomator.ClickButton(invGearNode);
                Thread.Sleep(500);
            }
            SelectOuterTab(window, "计算结果");
            Thread.Sleep(500);
            SelectInnerTab(window, "应力参数");
            Thread.Sleep(400);
            Console.WriteLine("  ✅ 已停留在'渐开线齿轮副 → 计算结果 → 应力参数'");
        }

        /// <summary>选择外层 TabItem（参数输入/计算结果/2D图例/3D模型）</summary>
        private static bool SelectOuterTab(AutomationElement root, string tabName)
        {
            return SelectInnerTab(root, tabName);
        }
    }
}
