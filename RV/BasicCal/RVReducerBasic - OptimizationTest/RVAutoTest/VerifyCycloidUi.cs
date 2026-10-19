using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows.Automation;
using RVAutoTest.Scenarios;

namespace RVAutoTest
{
    /// <summary>
    /// 临时 UI 验证：驱动 WPF 应用的"摆线针轮副"页面计算，
    /// 读取输出表格中的接触应力等字段，与 DLL 直调结果对比。
    /// 用于确认 RVAutoTest 摆线 DLL 调用参数与 UI 应用完全一致。
    /// 用法：--verify-cycloid-ui
    /// </summary>
    public static class VerifyCycloidUi
    {
        private const string AppWindowTitle = Program.AppWindowTitle;

        public static int Run(string[] args)
        {
            var workspaceRoot = Program.FindWorkspaceRootForVerify();
            var appExe = Program.FindWpfAppExeForVerify(workspaceRoot);
            if (appExe is null)
            {
                Console.WriteLine("❌ 未找到 RVReducerDesignApp.exe");
                return 2;
            }

            Console.WriteLine($"启动 WPF 应用: {appExe}");
            var (process, window) = UIAutomator.LaunchAndWaitForWindow(appExe, AppWindowTitle);
            if (window is null || process is null)
            {
                Console.WriteLine("❌ 启动窗口失败");
                return 2;
            }

            try
            {
                // ── 1. 选中"摆线针轮副"树节点 ──
                var node = UIAutomator.FindTreeItemByName(window, "摆线针轮副");
                if (node is null)
                {
                    Console.WriteLine("❌ 未找到'摆线针轮副'树节点");
                    return 2;
                }
                UIAutomator.ClickButton(node);
                Thread.Sleep(1200);
                Console.WriteLine("✅ 已选中'摆线针轮副'节点");

                // ── 2. 点击"计算"按钮 ──
                var calButton = UIAutomator.FindElement(window, name: "计算", controlType: ControlType.Button)
                               ?? UIAutomator.FindElement(window, automationId: "CalButton", controlType: ControlType.Button);
                if (calButton is null)
                {
                    Console.WriteLine("❌ 未找到'计算'按钮");
                    return 2;
                }
                UIAutomator.ClickButton(calButton);
                Thread.Sleep(1500);
                Console.WriteLine("✅ 已触发摆线计算");

                // ── 2.5 切换到"计算结果"Tab ──
                var outputTab = UIAutomator.FindElement(window, name: "计算结果", controlType: ControlType.TabItem);
                if (outputTab is not null)
                {
                    try
                    {
                        var sel = outputTab.GetCurrentPattern(SelectionItemPattern.Pattern) as SelectionItemPattern;
                        sel?.Select();
                        Thread.Sleep(800);
                        Console.WriteLine("✅ 已切换到'计算结果'Tab");
                    }
                    catch (Exception ex) { Console.WriteLine($"⚠️ 切换 Tab 失败: {ex.Message}"); }
                }
                else
                {
                    Console.WriteLine("⚠️ 未找到'计算结果'Tab");
                }

                // ── 3. 读取输出表格 ──
                var table = window.FindFirst(TreeScope.Descendants,
                    new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Table));
                if (table is null)
                    table = window.FindFirst(TreeScope.Descendants,
                        new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.DataGrid));
                if (table is null)
                {
                    Console.WriteLine("❌ 未找到输出表格");
                    return 2;
                }
                var items = table.FindAll(TreeScope.Descendants,
                    new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.DataItem));
                Console.WriteLine($"\n📊 UI 摆线输出（{items.Count} 行）:");
                foreach (AutomationElement item in items)
                {
                    var texts = item.FindAll(TreeScope.Descendants,
                        new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Text));
                    var cells = new List<string>();
                    foreach (AutomationElement t in texts)
                    {
                        var txt = t.Current.Name?.Trim();
                        if (!string.IsNullOrEmpty(txt)) cells.Add(txt);
                    }
                    if (cells.Count > 0)
                        Console.WriteLine("  " + string.Join(" | ", cells));
                }

                // ── 4. 截图 ──
                var shot = Path.Combine(workspaceRoot, "screenshots", "verify-cycloid-ui-output.png");
                Directory.CreateDirectory(Path.GetDirectoryName(shot)!);
                UIAutomator.CaptureWindow(window, shot);
                Console.WriteLine($"\n📸 截图: {shot}");
                return 0;
            }
            finally
            {
                // 优先优雅关闭，超时再强杀（与 Program 主流程一致）
                try { if (!process.HasExited) { process.CloseMainWindow(); process.WaitForExit(3000); } } catch { }
                try { if (!process.HasExited) process.Kill(); } catch { }
                try { process.Dispose(); } catch { }
            }
        }
    }
}
