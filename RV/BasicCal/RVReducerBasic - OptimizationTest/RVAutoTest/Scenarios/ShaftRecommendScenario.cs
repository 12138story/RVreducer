using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Automation;
using RVAutoTest.Models;

namespace RVAutoTest.Scenarios
{
    /// <summary>
    /// 场景 2: 轴推荐 + 曲轴图形截图。
    ///
    /// 说明：RecommondShaftPara 的 DLL 输入依赖完整的 CTANRVSet 数据存储
    /// （分割索引、截面类型、节点坐标等），无法在独立工具中轻松复现。
    /// 因此本场景采用 UIA 驱动真实应用完成轴推荐，并从 UI 读取结果。
    /// </summary>
    public static class ShaftRecommendScenario
    {
        // 齿宽递增序列（重试时依次使用；index 0 的 40/80 是场景1默认值，重试从 index 1 开始增大）
        private static readonly double[] MinWidthOptions = { 40, 60, 80, 100 };
        private static readonly double[] MaxWidthOptions = { 80, 100, 120, 140 };

        /// <summary>执行轴推荐场景</summary>
        /// <param name="outputDir">输出目录</param>
        /// <param name="window">WPF 应用主窗口（可为 null，则跳过 UI 操作）</param>
        /// <param name="config">外部测试配置（null 时使用默认参数）</param>
        public static ShaftRecommendResult Run(string outputDir, AutomationElement? window, TestConfig? config = null)
        {
            var result = new ShaftRecommendResult
            {
                // 默认来自 SetShaftContentDrawer.TmpTest()，可用配置覆盖
                Input = ConfigLoader.ResolveShaft(config)
            };

            Directory.CreateDirectory(outputDir);

            if (window is null)
            {
                Console.WriteLine("⚠️ 无窗口引用，跳过轴推荐 UI 操作");
                return result;
            }

            try
            {
                // ── 1. 打开轴推荐抽屉 ──
                var shaftButton = UIAutomator.FindElement(window, name: "轴推荐");
                if (shaftButton is null)
                {
                    Console.WriteLine("⚠️ 未找到'轴推荐'按钮");
                    return result;
                }
                UIAutomator.ClickButton(shaftButton);
                Thread.Sleep(800);

                // 截图抽屉面板
                var drawerFile = Path.Combine(outputDir, "03-shaft-recommend-input.png");
                UIAutomator.CaptureWindow(window, drawerFile);
                result.Screenshots.Add(drawerFile);
                Console.WriteLine($"  📸 轴推荐抽屉截图: {drawerFile}");

                // 1.5 注入轴推荐输入参数（配置值写入抽屉输入框，使配置真正驱动计算）
                SetShaftInputValues(window, result.Input);

                // ── 2. 执行轴推荐，检测弹窗并自动重试 ──
                // maxAttempts 与齿宽数组长度匹配：首次用默认 40/80，重试依次 60/100→80/120→100/140
                const int maxAttempts = 4;
                for (int attempt = 1; attempt <= maxAttempts; attempt++)
                {
                    // 每次重新打开轴推荐抽屉并定位"确定"按钮
                    // （齿轮推荐重试会切换抽屉，缓存的元素可能失效，不能复用）
                    if (attempt > 1)
                    {
                        var shaftBtn = UIAutomator.FindElement(window, name: "轴推荐");
                        if (shaftBtn is null)
                        {
                            result.ErrorMessage = "未找到'轴推荐'按钮";
                            break;
                        }
                        UIAutomator.ClickButton(shaftBtn);
                        Thread.Sleep(600);
                    }
                    var okButton = UIAutomator.FindElement(window, name: "确定", controlType: ControlType.Button);
                    if (okButton is null)
                    {
                        result.ErrorMessage = "未找到轴推荐'确定'按钮";
                        break;
                    }

                    Console.WriteLine($"  尝试轴推荐 (第 {attempt}/{maxAttempts} 次)...");
                    UIAutomator.ClickButton(okButton);
                    Thread.Sleep(2000); // 等待计算完成

                    // 检测弹窗
                    var messageBox = UIAutomator.FindMessageBox(timeoutSec: 4);
                    if (messageBox is null)
                    {
                        // 成功：清除此前重试期的告警残留，避免误报失败
                        result.ErrorMessage = null;
                        Console.WriteLine("  ✓ 轴推荐未弹出错误弹窗");
                        break; // 成功
                    }

                    var msgText = UIAutomator.GetMessageBoxText(messageBox);
                    Console.WriteLine($"  ⚠️ 检测到弹窗: {msgText}");
                    UIAutomator.ClickMessageBoxButton(messageBox, "确定"); // 关闭弹窗
                    Thread.Sleep(800);

                    // 处理"未找到合适的滚针轴承"：以更大摆线轮齿宽重新执行齿轮推荐再重试
                    if (msgText.Contains("滚针轴承") || msgText.Contains("摆线轮宽度"))
                    {
                        if (attempt < maxAttempts)
                        {
                            // attempt=1→60/100, attempt=2→80/120, attempt=3→100/140
                            double minW = MinWidthOptions[attempt];
                            double maxW = MaxWidthOptions[attempt];
                            Console.WriteLine($"  🔄 以摆线轮齿宽 [{minW:F0}~{maxW:F0}mm] 重新执行齿轮推荐...");
                            try
                            {
                                var widthFailure = GearRecommendScenario.RunGearRecommendUI(window, outputDir, minW, maxW);
                                if (widthFailure is not null)
                                {
                                    // 齿宽设置失败会导致重试无效，记录根因并停止重试
                                    result.ErrorMessage = $"齿宽设置失败({widthFailure})，轴推荐重试无法生效";
                                    break;
                                }
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"  ⚠️ 重新齿轮推荐失败: {ex.Message}");
                                result.ErrorMessage = $"重新齿轮推荐异常: {ex.Message}";
                            }
                        }
                        else
                        {
                            result.ErrorMessage = $"轴推荐仍失败（已重试 {maxAttempts - 1} 次，齿宽已达上限 {MaxWidthOptions[MaxWidthOptions.Length - 1]:F0}mm）: {msgText}";
                            break; // 末次尝试仍失败，停止重试
                        }
                    }
                    else
                    {
                        result.ErrorMessage = $"轴推荐弹窗: {msgText}";
                        break; // 非滚针轴承错误，不再重试
                    }
                }

                // ── 3. 读取轴推荐结果（轴直径/轴长度，位于'曲轴系'节点的输出表格）──
                ReadRecommendedShaftFromUI(window, result);

                // ── 4. 选中左侧树中的"曲轴"节点（确保树滚动到可见位置）──
                SelectCrankshaftTreeItem(window);

                // ── 5. 截取曲轴 2D 图形（主内容区"参数输入"Tab）──
                CaptureCrankshaftDrawing(window, outputDir, result);

                // ── 6. 截图主窗口（含推荐结果）──
                var windowFile = Path.Combine(outputDir, "04-shaft-recommend-output.png");
                UIAutomator.CaptureWindow(window, windowFile);
                result.Screenshots.Add(windowFile);
                Console.WriteLine($"  📸 轴推荐结果截图: {windowFile}");
            }
            catch (Exception ex)
            {
                result.ErrorMessage = ex.Message;   // 保留根因供报告输出
                Console.WriteLine($"⚠️ 轴推荐场景异常: {ex.Message}");
            }

            SaveJson(result, outputDir, "shaft-recommend-result.json");
            Console.WriteLine("✅ 场景2 轴推荐完成");
            return result;
        }

        /// <summary>
        /// 从 UI 读取轴直径和轴长度。
        /// 轴推荐后 RecommondShaftPara 将结果写入曲轴系（CTANCrankShaftSystem）输出表格，
        /// 该表格显示"轴直径/轴长度"两行。读取步骤：
        ///   1. 选中"曲轴系"树节点（输出表格所在页面）
        ///   2. 切到"计算结果"Tab
        ///   3. 读取输出表格并解析 轴直径/轴长度
        ///   4. 切回"参数输入"Tab（供后续曲轴图形截图）
        /// </summary>
        private static void ReadRecommendedShaftFromUI(AutomationElement window, ShaftRecommendResult result)
        {
            try
            {
                // ── 1. 选中"曲轴系"节点 ──
                var shaftSystemNode = UIAutomator.FindTreeItemByName(window, "曲轴系");
                if (shaftSystemNode is not null)
                {
                    if (!SelectAutomationItem(shaftSystemNode, "曲轴系节点", result))
                        Console.WriteLine("  ⚠️ 选中'曲轴系'失败");
                    else
                        Console.WriteLine("  🖱️ 已选中'曲轴系'节点");
                }
                else
                {
                    result.ErrorMessage = "未找到'曲轴系'树节点（输出表格所在页面）";
                    Console.WriteLine($"  ⚠️ {result.ErrorMessage}");
                }

                // ── 2. 切到"计算结果"Tab ──
                SelectTab(window, "计算结果", result);

                // ── 3. 读取输出表格（带轮询，限定在当前 Tab 内容范围内）──
                var table = FindTableInTab(window, "计算结果");
                if (table is null)
                {
                    result.ErrorMessage = "未找到输出表格（'计算结果'Tab 内无 Table/DataGrid）";
                    Console.WriteLine($"  ⚠️ {result.ErrorMessage}");
                }
                else
                {
                    var labelValues = ReadTableCells(table);
                    Console.WriteLine($"  📋 输出表格内容: [{string.Join(" | ", labelValues.SelectMany(r => r))}]");

                    result.RecommendedDiameter = ParseLabelValue(labelValues, "轴直径");
                    result.RecommendedLength = ParseLabelValue(labelValues, "轴长度");
                    if (result.RecommendedDiameter <= 0 || result.RecommendedLength <= 0)
                        result.ErrorMessage ??= $"轴推荐结果读取失败（直径={result.RecommendedDiameter}, 长度={result.RecommendedLength}）";
                }

                result.CrankShaftCount = 3; // 曲轴数目为减速器设计常量
                Console.WriteLine($"  📊 轴直径 = {result.RecommendedDiameter} mm");
                Console.WriteLine($"  📊 轴长度 = {result.RecommendedLength} mm");

                // ── 4. 切回"参数输入"Tab（供后续曲轴图形截图）──
                SelectTab(window, "参数输入", null);
            }
            catch (Exception ex)
            {
                result.ErrorMessage = $"读取轴推荐结果异常: {ex.Message}";
                Console.WriteLine($"  ⚠️ {result.ErrorMessage}");
            }
        }

        /// <summary>选中树节点或 Tab 项（统一 TryGetCurrentPattern，失败时记录到 ErrorMessage）</summary>
        private static bool SelectAutomationItem(AutomationElement element, string describe, ShaftRecommendResult? result)
        {
            try
            {
                if (element.TryGetCurrentPattern(SelectionItemPattern.Pattern, out object? selObj))
                {
                    ((SelectionItemPattern)selObj!).Select();
                    Thread.Sleep(500);
                    return true;
                }
                var msg = $"'{describe}'不支持选中模式";
                if (result is not null) result.ErrorMessage ??= msg;
                Console.WriteLine($"  ⚠️ {msg}");
                return false;
            }
            catch (Exception ex)
            {
                var msg = $"选中'{describe}'失败: {ex.Message}";
                if (result is not null) result.ErrorMessage ??= msg;
                Console.WriteLine($"  ⚠️ {msg}");
                return false;
            }
        }

        /// <summary>切换到指定 Tab（name 为空或 Tab 未找到时仅记录日志）</summary>
        private static bool SelectTab(AutomationElement window, string tabName, ShaftRecommendResult? result)
        {
            var tab = UIAutomator.FindElement(window, name: tabName, controlType: ControlType.TabItem);
            if (tab is null)
            {
                var msg = $"未找到Tab '{tabName}'";
                if (result is not null) result.ErrorMessage ??= msg;
                Console.WriteLine($"  ⚠️ {msg}");
                return false;
            }
            var ok = SelectAutomationItem(tab, $"Tab '{tabName}'", result);
            if (ok) Console.WriteLine($"  ✅ 已切换到'{tabName}'Tab");
            return ok;
        }

        /// <summary>
        /// 查找输出表格（带轮询）。
        /// WPF 主内容区同一时刻只显示一个页面（Tab 切换时其它页内容不在可视树），
        /// 因此整窗查找即可命中当前页的表格，且比限定 TabControl 作用域更可靠。
        /// </summary>
        private static AutomationElement? FindTableInTab(AutomationElement window, string tabName)
        {
            return UIAutomator.FindElement(window, controlType: ControlType.Table, timeoutSec: 5)
                ?? UIAutomator.FindElement(window, controlType: ControlType.DataGrid, timeoutSec: 5);
        }

        /// <summary>读取表格所有单元格文本（每行一个标签列表，供按行解析）</summary>
        private static List<List<string>> ReadTableCells(AutomationElement table)
        {
            var rows = new List<List<string>>();
            var items = table.FindAll(TreeScope.Descendants,
                new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.DataItem));
            foreach (AutomationElement item in items)
            {
                var rowCells = new List<string>();
                var texts = item.FindAll(TreeScope.Descendants,
                    new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Text));
                foreach (AutomationElement t in texts)
                {
                    var txt = t.Current.Name?.Trim();
                    if (!string.IsNullOrEmpty(txt)) rowCells.Add(txt);
                }
                var customs = item.FindAll(TreeScope.Descendants,
                    new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Custom));
                foreach (AutomationElement c in customs)
                {
                    if (c.TryGetCurrentPattern(ValuePattern.Pattern, out object? vp))
                    {
                        var val = ((ValuePattern)vp!).Current.Value?.Trim();
                        if (!string.IsNullOrEmpty(val)) rowCells.Add(val);
                    }
                }
                if (rowCells.Count > 0) rows.Add(rowCells);
            }
            return rows;
        }

        /// <summary>
        /// 在按行分组的单元格列表中查找含关键字的行，返回该行内第一个可解析为 double 的值。
        /// 支持"138.6637"、"138.6637 mm"（数值+单位同格）形式；使用 InvariantCulture 避免区域性差异。
        /// 返回 0 表示未找到或解析失败。
        /// </summary>
        private static double ParseLabelValue(List<List<string>> rows, string keyword)
        {
            foreach (var row in rows)
            {
                if (!row.Any(c => c.Contains(keyword))) continue;
                foreach (var cell in row)
                {
                    if (TryParseNumeric(cell, out double value))
                        return value;
                }
            }
            return 0;
        }

        /// <summary>解析数字：先尝试整串，失败则提取数字前缀（处理"数值+单位同格"）</summary>
        private static readonly Regex NumberPattern = new(@"-?\d+(\.\d+)?", RegexOptions.Compiled);

        private static bool TryParseNumeric(string s, out double value)
        {
            if (double.TryParse(s, NumberStyles.Float, CultureInfo.InvariantCulture, out value))
                return true;
            var m = NumberPattern.Match(s);
            if (m.Success && double.TryParse(m.Value, NumberStyles.Float, CultureInfo.InvariantCulture, out value))
                return true;
            value = 0;
            return false;
        }

        /// <summary>
        /// 将轴推荐输入参数写入抽屉输入框（配置值真正驱动 UI 计算）。
        /// 通过 FindEditAfterLabel 几何定位"许用弯曲正应力/许用剪应力/轴承设计寿命"输入框。
        /// </summary>
        private static void SetShaftInputValues(AutomationElement window, Models.ShaftRecommendInput input)
        {
            try
            {
                var bendEdit = UIAutomator.FindEditAfterLabel(window, "许用弯曲正应力");
                var shearEdit = UIAutomator.FindEditAfterLabel(window, "许用剪应力");
                var lifeEdit = UIAutomator.FindEditAfterLabel(window, "轴承设计寿命");

                if (bendEdit is not null)
                {
                    UIAutomator.SetTextBoxValue(bendEdit, input.AllowableBendStress.ToString("F0"));
                    Console.WriteLine($"  ⌨️ 许用弯曲正应力 = {input.AllowableBendStress:F0} MPa");
                }
                if (shearEdit is not null)
                {
                    UIAutomator.SetTextBoxValue(shearEdit, input.AllowableShearStress.ToString("F0"));
                    Console.WriteLine($"  ⌨️ 许用剪应力 = {input.AllowableShearStress:F0} MPa");
                }
                if (lifeEdit is not null)
                {
                    UIAutomator.SetTextBoxValue(lifeEdit, input.BearingDesignLife.ToString("F0"));
                    Console.WriteLine($"  ⌨️ 轴承设计寿命 = {input.BearingDesignLife:F0} h");
                }
                if (bendEdit is null && shearEdit is null && lifeEdit is null)
                    Console.WriteLine("  ⚠️ 未定位到轴推荐输入框（可能抽屉未打开或布局不同）");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"  ⚠️ 设置轴推荐输入参数失败: {ex.Message}");
            }
        }

        /// <summary>
        /// 选中左侧树视图中的"曲轴"节点（确保树滚动到该节点并展开其父节点"曲轴系"）。
        /// Assembly 后 UI 已跳转，但树视图可能未完全展开到曲轴可见位置。
        /// </summary>
        private static void SelectCrankshaftTreeItem(AutomationElement window)
        {
            try
            {
                // 先展开"曲轴系"父节点（确保"曲轴"子节点在 UIA 树中可用）
                var shaftSystemNode = UIAutomator.FindTreeItemByName(window, "曲轴系");
                if (shaftSystemNode is not null)
                {
                    try
                    {
                        var expandPattern = (ExpandCollapsePattern)shaftSystemNode.GetCurrentPattern(ExpandCollapsePattern.Pattern);
                        if (expandPattern.Current.ExpandCollapseState != ExpandCollapseState.Expanded)
                            expandPattern.Expand();
                        Thread.Sleep(400);
                        Console.WriteLine("  📂 已展开'曲轴系'节点");
                    }
                    catch (Exception) { /* 可能已展开或模式不可用 */ }
                }

                // 选中"曲轴"子节点（WPF 会自动滚动到该节点）
                var crankshaftNode = UIAutomator.FindTreeItemByName(window, "曲轴");
                if (crankshaftNode is not null)
                {
                    try
                    {
                        var selPattern = (SelectionItemPattern)crankshaftNode.GetCurrentPattern(SelectionItemPattern.Pattern);
                        selPattern.Select();
                        Thread.Sleep(500);
                        Console.WriteLine("  🖱️ 已选中'曲轴'节点，树视图已滚动");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"  ⚠️ 选中'曲轴'节点失败: {ex.Message}");
                    }
                }
                else
                {
                    Console.WriteLine("  ⚠️ 未找到'曲轴'节点");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"  ⚠️ 选择曲轴树节点失败: {ex.Message}");
            }
        }

        /// <summary>
        /// 截取曲轴 2D 图形。
        /// 截取全窗口（PrintWindow 已保证无遮挡）；轴推荐后 UI 已自动跳转到曲轴输入页面，
        /// 树视图选中曲轴节点后，曲轴图形在主内容区清晰可见。
        /// </summary>
        private static void CaptureCrankshaftDrawing(
            AutomationElement window, string outputDir, ShaftRecommendResult result)
        {
            var drawingFile = Path.Combine(outputDir, "05-crankshaft-drawing.png");
            try
            {
                Thread.Sleep(1000); // 等待曲轴 Canvas 渲染
                UIAutomator.CaptureWindow(window, drawingFile);
                result.Screenshots.Add(drawingFile);
                Console.WriteLine($"  📸 曲轴图形截图: {drawingFile}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"  ⚠️ 曲轴图形截图失败: {ex.Message}");
            }
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
