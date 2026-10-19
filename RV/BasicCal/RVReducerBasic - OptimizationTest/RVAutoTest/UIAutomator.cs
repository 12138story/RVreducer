using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Automation;

namespace RVAutoTest
{
    /// <summary>
    /// Microsoft UI Automation 工具类。
    /// 通过 UIA 控件模式（InvokePattern/ValuePattern 等）驱动 WPF 应用，
    /// 无需移动鼠标或模拟点击。
    /// </summary>
    public static class UIAutomator
    {
        // ═══════════════════ 窗口查找 ═══════════════════

        /// <summary>
        /// 启动进程并等待其主窗口出现。
        /// 返回 (进程引用, 主窗口元素)，调用方负责在结束时关闭进程。
        /// </summary>
        public static (Process process, AutomationElement? window) LaunchAndWaitForWindow(
            string exePath, string windowTitle, int timeoutSec = 60)
        {
            var process = Process.Start(new ProcessStartInfo
            {
                FileName = exePath,
                WorkingDirectory = Path.GetDirectoryName(exePath) ?? "",
                UseShellExecute = false
            });

            if (process is null)
                throw new InvalidOperationException($"无法启动进程: {exePath}");

            try
            {
                var window = WaitForWindow(process, windowTitle, timeoutSec);
                return (process, window);
            }
            catch
            {
                // 异常路径兜底清理，防止进程泄漏
                if (!process.HasExited)
                {
                    try { process.Kill(); } catch (Exception) { }
                }
                process.Dispose();
                throw;
            }
        }

        /// <summary>
        /// 等待指定进程的窗口出现。
        /// </summary>
        public static AutomationElement? WaitForWindow(Process process, string windowTitle, int timeoutSec = 60)
        {
            var deadline = DateTime.Now.AddSeconds(timeoutSec);
            while (DateTime.Now < deadline)
            {
                if (process.HasExited)
                    throw new InvalidOperationException($"进程已退出，退出码: {process.ExitCode}");

                process.Refresh();
                if (process.MainWindowHandle != IntPtr.Zero)
                {
                    try
                    {
                        var element = AutomationElement.FromHandle(process.MainWindowHandle);
                        if (element is not null)
                        {
                            // 校验窗口标题（防止误匹配错误窗口），但不匹配时仍接受，仅警告
                            bool titleMatches = string.IsNullOrEmpty(windowTitle) ||
                                element.Current.Name.Contains(windowTitle, StringComparison.OrdinalIgnoreCase);
                            if (!titleMatches)
                            {
                                Console.WriteLine($"⚠️ 窗口标题 '{element.Current.Name}' 与预期 '{windowTitle}' 不匹配，仍继续使用");
                            }
                            WaitForInputIdle(process, 5000);
                            return element;
                        }
                    }
                    catch (Exception)
                    {
                        // 窗口尚未完全就绪，重试
                    }
                }

                Thread.Sleep(500);
            }

            throw new TimeoutException($"等待窗口 '{windowTitle}' 超时 ({timeoutSec}s)");
        }

        /// <summary>等待应用 UI 空闲（输入空闲）</summary>
        public static void WaitForInputIdle(Process process, int timeoutMs = 5000)
        {
            try { process.WaitForInputIdle(timeoutMs); }
            catch (Exception) { /* 某些进程不支持 WaitForInputIdle */ }
        }

        // ═══════════════════ 元素查找 ═══════════════════

        /// <summary>查找单个元素，找不到返回 null（不抛异常）</summary>
        public static AutomationElement? FindElement(
            AutomationElement root,
            string? name = null,
            string? automationId = null,
            ControlType? controlType = null,
            int timeoutSec = 10)
        {
            var deadline = DateTime.Now.AddSeconds(timeoutSec);
            while (DateTime.Now < deadline)
            {
                var conditions = new List<Condition>();
                if (!string.IsNullOrEmpty(name))
                    conditions.Add(new PropertyCondition(AutomationElement.NameProperty, name));
                if (!string.IsNullOrEmpty(automationId))
                    conditions.Add(new PropertyCondition(AutomationElement.AutomationIdProperty, automationId));
                if (controlType is not null)
                    conditions.Add(new PropertyCondition(AutomationElement.ControlTypeProperty, controlType));

                Condition condition = conditions.Count == 0
                    ? Condition.TrueCondition
                    : (conditions.Count == 1 ? conditions[0] : new AndCondition(conditions.ToArray()));

                var element = root.FindFirst(TreeScope.Descendants, condition);
                if (element is not null)
                    return element;

                Thread.Sleep(300);
            }

            return null;
        }

        /// <summary>查找多个元素</summary>
        public static List<AutomationElement> FindElements(
            AutomationElement root,
            string? name = null,
            ControlType? controlType = null)
        {
            var conditions = new List<Condition>();
            if (!string.IsNullOrEmpty(name))
                conditions.Add(new PropertyCondition(AutomationElement.NameProperty, name));
            if (controlType is not null)
                conditions.Add(new PropertyCondition(AutomationElement.ControlTypeProperty, controlType));

            Condition condition = conditions.Count == 0
                ? Condition.TrueCondition
                : (conditions.Count == 1 ? conditions[0] : new AndCondition(conditions.ToArray()));

            return root.FindAll(TreeScope.Descendants, condition).Cast<AutomationElement>().ToList();
        }

        /// <summary>查找元素，找不到抛异常</summary>
        public static AutomationElement RequireElement(
            AutomationElement root,
            string? name = null,
            string? automationId = null,
            ControlType? controlType = null,
            int timeoutSec = 10)
        {
            var element = FindElement(root, name, automationId, controlType, timeoutSec);
            if (element is null)
            {
                var desc = string.Join(", ", new[]
                {
                    name is null ? null : $"Name='{name}'",
                    automationId is null ? null : $"AutomationId='{automationId}'",
                    controlType is null ? null : $"ControlType='{controlType.ProgrammaticName}'"
                }.Where(s => s is not null));
                throw new InvalidOperationException($"未找到元素: {desc}");
            }
            return element;
        }

        // ═══════════════════ 交互操作 ═══════════════════

        /// <summary>
        /// 通过 UIA InvokePattern 或 TogglePattern 触发按钮 — 不移动鼠标。
        /// </summary>
        public static void ClickButton(AutomationElement element)
        {
            if (element is null)
                throw new ArgumentNullException(nameof(element));

            // 优先 InvokePattern
            if (element.TryGetCurrentPattern(InvokePattern.Pattern, out object? invokePattern))
            {
                ((InvokePattern)invokePattern!).Invoke();
                return;
            }

            // 其次 TogglePattern (ToggleButton)
            if (element.TryGetCurrentPattern(TogglePattern.Pattern, out object? togglePattern))
            {
                ((TogglePattern)togglePattern!).Toggle();
                return;
            }

            // 最后 SelectionItemPattern
            if (element.TryGetCurrentPattern(SelectionItemPattern.Pattern, out object? selPattern))
            {
                ((SelectionItemPattern)selPattern!).Select();
                return;
            }

            throw new InvalidOperationException($"元素 '{element.Current.Name}' 不支持任何可点击模式");
        }

        /// <summary>通过 UIA ValuePattern 设置文本框值</summary>
        public static void SetTextBoxValue(AutomationElement element, string value)
        {
            if (element is null)
                throw new ArgumentNullException(nameof(element));

            if (!element.TryGetCurrentPattern(ValuePattern.Pattern, out object? pattern))
                throw new InvalidOperationException($"元素 '{element.Current.Name}' 不支持 ValuePattern");

            var valuePattern = (ValuePattern)pattern!;
            valuePattern.SetValue(value);
        }

        /// <summary>读取元素文本（ValuePattern 或 Name）</summary>
        public static string GetText(AutomationElement element)
        {
            if (element is null) return "";

            if (element.TryGetCurrentPattern(ValuePattern.Pattern, out object? valuePattern))
                return ((ValuePattern)valuePattern!).Current.Value ?? "";

            return element.Current.Name ?? "";
        }

        /// <summary>
        /// 选择 ComboBox 中的某个选项。
        /// 通过 SelectionItemPattern 选择，不移动鼠标。
        /// </summary>
        public static void SelectComboBoxItem(AutomationElement comboBox, string itemText)
        {
            if (comboBox is null)
                throw new ArgumentNullException(nameof(comboBox));

            // 展开下拉框
            if (comboBox.TryGetCurrentPattern(ExpandCollapsePattern.Pattern, out object? expandPattern))
                ((ExpandCollapsePattern)expandPattern!).Expand();

            Thread.Sleep(300);

            // 在下拉项中查找目标项
            var items = comboBox.FindAll(TreeScope.Descendants,
                new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.ListItem));
            foreach (AutomationElement item in items)
            {
                if (item.Current.Name.Contains(itemText, StringComparison.OrdinalIgnoreCase))
                {
                    if (item.TryGetCurrentPattern(SelectionItemPattern.Pattern, out object? selPattern))
                    {
                        ((SelectionItemPattern)selPattern!).Select();
                        return;
                    }
                }
            }

            throw new InvalidOperationException($"ComboBox 中未找到选项 '{itemText}'");
        }

        // ═══════════════════ 截图 ═══════════════════

        /// <summary>向上查找顶层窗口并返回其 IntPtr 句柄</summary>
        private static IntPtr GetWindowHandle(AutomationElement element)
        {
            var current = element;
            while (current is not null)
            {
                if (current.Current.ControlType == ControlType.Window)
                    break;
                current = TreeWalker.ControlViewWalker.GetParent(current);
            }
            if (current is null) current = element;
            return new IntPtr(current.Current.NativeWindowHandle);
        }

        /// <summary>将窗口从最小化恢复并置顶，为 CopyFromScreen 回退减少遮挡风险</summary>
        private static void BringWindowToFront(IntPtr hwnd)
        {
            if (hwnd == IntPtr.Zero) return;
            try { ShowWindow(hwnd, SW_RESTORE); SetForegroundWindow(hwnd); Thread.Sleep(200); }
            catch (Exception) { }
        }

        /// <summary>截取元素所在的屏幕区域并保存为 PNG（PrintWindow 优先，CopyFromScreen 回退）</summary>
        public static void CaptureElement(AutomationElement element, string filePath)
        {
            if (element is null)
                throw new ArgumentNullException(nameof(element));

            var rect = element.Current.BoundingRectangle;
            if (rect.IsEmpty || rect.Width <= 0 || rect.Height <= 0)
                throw new InvalidOperationException($"元素 '{element.Current.Name}' 的边界矩形无效");

            var width = (int)Math.Ceiling(rect.Width);
            var height = (int)Math.Ceiling(rect.Height);

            using var bitmap = new Bitmap(width, height);
            using (var g = Graphics.FromImage(bitmap))
            {
                // 策略 1: PrintWindow 从窗口 DC 直接渲染，不受其他窗口遮挡影响
                var hwnd = GetWindowHandle(element);
                bool printOk = false;
                if (hwnd != IntPtr.Zero)
                {
                    var hdc = g.GetHdc();
                    printOk = PrintWindow(hwnd, hdc, 0);
                    g.ReleaseHdc(hdc);
                }

                // 策略 2: PrintWindow 对 WPF/DirectX 可能失败，回退 CopyFromScreen（先置顶窗口）
                if (!printOk)
                {
                    if (hwnd != IntPtr.Zero) BringWindowToFront(hwnd);
                    var left = Math.Max(0, (int)rect.Left);
                    var top = Math.Max(0, (int)rect.Top);
                    g.CopyFromScreen(left, top, 0, 0, new Size(width, height));
                }
            }
            bitmap.Save(filePath, ImageFormat.Png);
        }

        /// <summary>截取整个窗口并保存为 PNG</summary>
        public static void CaptureWindow(AutomationElement window, string filePath)
        {
            CaptureElement(window, filePath);
        }

        // ═══════════════════ Win32 API ═══════════════════

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool PrintWindow(IntPtr hwnd, IntPtr hdc, uint nFlags);

        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hwnd);

        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hwnd, int nCmdShow);

        private const int SW_RESTORE = 9;

        // ═══════════════════ 树遍历 ═══════════════════

        /// <summary>
        /// 按路径查找 TreeViewItem（例如 "曲轴" 或 "轴承"）。
        /// 递归搜索整个窗口树。
        /// </summary>
        public static AutomationElement? FindTreeItemByName(AutomationElement root, string name, int timeoutSec = 10)
        {
            var deadline = DateTime.Now.AddSeconds(timeoutSec);
            while (DateTime.Now < deadline)
            {
                var treeItems = root.FindAll(TreeScope.Descendants,
                    new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.TreeItem));

                foreach (AutomationElement item in treeItems)
                {
                    if (item.Current.Name == name)
                        return item;
                }

                Thread.Sleep(300);
            }
            return null;
        }

        // ═══════════════════ MessageBox / 弹窗 ═══════════════════

        /// <summary>
        /// 检测被测进程上的弹出 MessageBox 窗口（WPF MessageBox 是独立顶层窗口）。
        /// 若指定 processId，仅返回该进程的窗口，避免误操作桌面其他应用的对话框。
        /// 找不到返回 null。
        /// </summary>
        public static AutomationElement? FindMessageBox(int? processId = null, int timeoutSec = 3)
        {
            var desktop = AutomationElement.RootElement;
            var deadline = DateTime.Now.AddSeconds(timeoutSec);
            while (DateTime.Now < deadline)
            {
                try
                {
                    var windows = desktop.FindAll(TreeScope.Children,
                        new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Window));
                    foreach (AutomationElement w in windows)
                    {
                        // 按进程过滤（防止误点其他应用的对话框）
                        if (processId is int pid)
                        {
                            int winPid = 0;
                            try { winPid = w.Current.ProcessId; } catch (Exception) { continue; }
                            if (winPid != pid) continue;
                        }
                        var name = w.Current.Name ?? "";
                        if (name.Contains("提示") || name.Contains("错误") || name.Contains("警告") ||
                            name.Contains("未找到") || name.Contains("确认"))
                        {
                            if (!name.Contains("摆线针轮行星减速器"))
                                return w;
                        }
                    }
                }
                catch (Exception) { }
                Thread.Sleep(200);
            }
            return null;
        }

        /// <summary>读取 MessageBox 中的文本内容</summary>
        public static string GetMessageBoxText(AutomationElement messageBox)
        {
            var texts = messageBox.FindAll(TreeScope.Descendants,
                new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Text));
            var sb = new System.Text.StringBuilder();
            foreach (AutomationElement t in texts)
            {
                var name = t.Current.Name;
                if (!string.IsNullOrEmpty(name))
                {
                    sb.Append(name);
                    sb.Append(' ');
                }
            }
            return sb.ToString().Trim();
        }

        /// <summary>点击 MessageBox 上的按钮（如 "确定"），并等待弹窗关闭</summary>
        public static void ClickMessageBoxButton(AutomationElement messageBox, string buttonName = "确定")
        {
            if (messageBox is null) return;
            var btn = messageBox.FindFirst(TreeScope.Descendants,
                new PropertyCondition(AutomationElement.NameProperty, buttonName));
            if (btn is not null)
            {
                ClickButton(btn);
                Thread.Sleep(500);
            }
        }

        // ═══════════════════ 复选框 ═══════════════════

        /// <summary>
        /// 设置复选框勾选状态（通过 TogglePattern 或 SelectionItemPattern，不移动鼠标）。
        /// </summary>
        public static void SetCheckBox(AutomationElement element, bool isChecked)
        {
            if (element is null)
                throw new ArgumentNullException(nameof(element));

            if (element.TryGetCurrentPattern(TogglePattern.Pattern, out object? togglePattern))
            {
                var toggle = (TogglePattern)togglePattern!;
                var desired = isChecked ? ToggleState.On : ToggleState.Off;
                if (toggle.Current.ToggleState != desired)
                    toggle.Toggle();
                return;
            }

            if (element.TryGetCurrentPattern(SelectionItemPattern.Pattern, out object? selPattern))
            {
                var sel = (SelectionItemPattern)selPattern!;
                if (isChecked) sel.Select();
                return;
            }

            throw new InvalidOperationException($"元素 '{element.Current.Name}' 不支持复选框模式");
        }

        /// <summary>读取复选框勾选状态（null 表示无法确定）</summary>
        public static bool? GetCheckBoxState(AutomationElement element)
        {
            if (element is null) return null;
            if (element.TryGetCurrentPattern(TogglePattern.Pattern, out object? togglePattern))
                return ((TogglePattern)togglePattern!).Current.ToggleState == ToggleState.On;
            return null;
        }

        // ═══════════════════ 标签→输入框 定位 ═══════════════════

        /// <summary>
        /// 通过几何位置匹配，找到标签文本右侧/同行的 Edit 输入框。
        /// 用于定位"摆线轮最小齿宽"等标签后的数值输入框。
        /// 基于 BoundingRectangle 匹配，不依赖 UIA 树结构。
        /// </summary>
        public static AutomationElement? FindEditAfterLabel(AutomationElement root, string labelText, int timeoutSec = 3)
        {
            // 策略 1: 标准方案——标签为 ControlType.Text
            var label = FindElement(root, labelText, controlType: ControlType.Text, timeoutSec: timeoutSec);
            if (label is null)
            {
                // 策略 2: 回退——标签是只读 Edit（WPF 只读 TextBox）
                // WPF TextBox UIA 中 Name=""，文本在 ValuePattern.Value 中
                var allEdits = FindElements(root, controlType: ControlType.Edit);
                AutomationElement? labelEdit = null;
                foreach (var e in allEdits)
                {
                    // 尝试从 ValuePattern 读取文本（WPF 只读 TextBox 的 Value=文本）
                    string? valText = null;
                    bool isReadOnly = false;
                    if (e.TryGetCurrentPattern(ValuePattern.Pattern, out object? vp))
                    {
                        var v = (ValuePattern)vp;
                        valText = v.Current.Value;
                        isReadOnly = v.Current.IsReadOnly;
                    }
                    if (valText == labelText && isReadOnly)
                    {
                        labelEdit = e;
                        break;
                    }
                }

                if (labelEdit is null) return null;

                var labelEditRect = labelEdit.Current.BoundingRectangle;
                if (labelEditRect.IsEmpty) return null;

                double labelEditCenterY = labelEditRect.Top + labelEditRect.Height / 2;
                AutomationElement? bestEdit = null;
                double bestScoreEd = double.MaxValue;
                foreach (var e in allEdits)
                {
                    if (e == labelEdit) continue;
                    var r = e.Current.BoundingRectangle;
                    if (r.IsEmpty) continue;
                    double editCenterY = r.Top + r.Height / 2;
                    double verticalDiff = Math.Abs(editCenterY - labelEditCenterY);
                    // 必须在标签右侧，且同行容差内；排除只读控件（可能是单位标签）
                    bool isEditable = true;
                    if (e.TryGetCurrentPattern(ValuePattern.Pattern, out object? vp2))
                        isEditable = !((ValuePattern)vp2).Current.IsReadOnly;
                    if (isEditable && r.Left >= labelEditRect.Right && verticalDiff < 30)
                    {
                        double score = verticalDiff + (r.Left - labelEditRect.Right);
                        if (score < bestScoreEd)
                        {
                            bestScoreEd = score;
                            bestEdit = e;
                        }
                    }
                }
                return bestEdit;
            }

            var labelRect = label.Current.BoundingRectangle;
            if (labelRect.IsEmpty) return null;

            var edits = FindElements(root, controlType: ControlType.Edit);
            AutomationElement? best = null;
            double bestScore = double.MaxValue;
            double labelCenterY = labelRect.Top + labelRect.Height / 2;
            foreach (var edit in edits)
            {
                var r = edit.Current.BoundingRectangle;
                if (r.IsEmpty) continue;

                double editCenterY = r.Top + r.Height / 2;
                double verticalDiff = Math.Abs(editCenterY - labelCenterY);

                if (r.Left >= labelRect.Right && verticalDiff < 30)
                {
                    double score = verticalDiff + (r.Left - labelRect.Right);
                    if (score < bestScore)
                    {
                        bestScore = score;
                        best = edit;
                    }
                }
            }
            return best;
        }
    }
}
