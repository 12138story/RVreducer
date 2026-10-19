using HandyControl.Controls;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using MessageBox = HandyControl.Controls.MessageBox;
using Window = System.Windows.Window;

namespace RVReducerDesignApp
{
    public partial class UIHelpWindow : Window
    {
        readonly string HelpFolder;
        Dictionary<string, string> toc = new Dictionary<string, string>(); // title -> filename
        string currentTopicFile = null;

        public UIHelpWindow()
        {
            InitializeComponent();

            // 更可靠地定位 Help 文件夹：优先使用可执行文件位置
            var exeDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            HelpFolder = Path.Combine(!string.IsNullOrEmpty(exeDir) ? exeDir : AppDomain.CurrentDomain.BaseDirectory, "Help");

            LoadTOC();

            // 如果有条目，自动显示第一个主题
            if (toc.Count > 0)
            {
                ShowTopic(null);
            }
        }

        void LoadTOC()
        {
            try
            {
                Debug.WriteLine($"HelpFolder = {HelpFolder}");

                if (!Directory.Exists(HelpFolder))
                {
                    Directory.CreateDirectory(HelpFolder);
                    Debug.WriteLine("Created Help folder because it did not exist.");
                }

                var files = Directory.GetFiles(HelpFolder, "*.*")
                    .Where(f => f.EndsWith(".html", StringComparison.OrdinalIgnoreCase) ||
                                f.EndsWith(".htm", StringComparison.OrdinalIgnoreCase) ||
                                f.EndsWith(".md", StringComparison.OrdinalIgnoreCase))
                    .OrderBy(f => f)
                    .ToArray();

                // 如果没有任何帮助文件，创建默认的 GettingStarted.md（但要避免与同名目录冲突）
                if (files.Length == 0)
                {
                    var desired = Path.Combine(HelpFolder, "GettingStarted.md");
                    string defaultMdPath = desired;

                    if (Directory.Exists(desired))
                    {
                        // 如果存在同名目录，寻找可用文件名
                        var alt = GetAvailableFilePath(HelpFolder, "GettingStarted", ".md");
                        if (alt == null)
                        {
                            MessageBox.Show($"无法在 {HelpFolder} 中创建帮助文件（找不到可用文件名）。", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                        }
                        defaultMdPath = alt;
                        Debug.WriteLine($"Found directory blocking desired file name; using alternative path: {defaultMdPath}");
                    }
                    else if (File.Exists(desired))
                    {
                        defaultMdPath = desired;
                    }

                    if (!File.Exists(defaultMdPath))
                    {
                        File.WriteAllText(defaultMdPath, DefaultGettingStartedMarkdown(), Encoding.UTF8);
                        Debug.WriteLine($"Wrote default help file to {defaultMdPath}");
                    }

                    files = new[] { defaultMdPath };
                }

                toc.Clear();
                TOCList.Items.Clear();

                foreach (var f in files)
                {
                    var title = Path.GetFileNameWithoutExtension(f);
                    // 如果有重复的去重（以防文件系统有冲突）
                    if (!toc.ContainsKey(title))
                    {
                        toc[title] = f;
                        TOCList.Items.Add(title);
                        Debug.WriteLine($"Help file added: {f}");
                    }
                    else
                    {
                        // 处理同名标题（可选：跳过或使用带编号的标题）
                        var altTitle = GetUniqueTitle(toc.Keys, title);
                        toc[altTitle] = f;
                        TOCList.Items.Add(altTitle);
                        Debug.WriteLine($"Help file added with alt title: {f} -> {altTitle}");
                    }
                }

                Debug.WriteLine($"LoadTOC: found {toc.Count} topics.");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"LoadTOC exception: {ex}");
                MessageBox.Show($"加载帮助目录失败：{ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // 生成一个不与现有 keys 冲突的唯一标题（在重复标题时使用）
        private string GetUniqueTitle(IEnumerable<string> existingKeys, string baseTitle)
        {
            if (!existingKeys.Contains(baseTitle)) return baseTitle;
            for (int i = 1; i < 1000; i++)
            {
                var candidate = $"{baseTitle}_{i}";
                if (!existingKeys.Contains(candidate)) return candidate;
            }
            return baseTitle + "_" + Guid.NewGuid().ToString("N").Substring(0, 6);
        }

        // 获取可用文件路径（当目标路径被目录占用时使用）
        private string GetAvailableFilePath(string folder, string baseName, string extension)
        {
            string candidate = Path.Combine(folder, baseName + extension);
            if (!File.Exists(candidate) && !Directory.Exists(candidate)) return candidate;

            for (int i = 1; i < 1000; i++)
            {
                candidate = Path.Combine(folder, $"{baseName}_{i}{extension}");
                if (!File.Exists(candidate) && !Directory.Exists(candidate)) return candidate;
            }

            return null;
        }

        public void ShowTopic(string topic)
        {
            if (string.IsNullOrEmpty(topic))
            {
                if (toc.ContainsKey("GettingStarted")) topic = "GettingStarted";
                else if (toc.Count > 0) topic = toc.Keys.First();
            }

            if (topic != null && toc.TryGetValue(topic, out var file))
            {
                currentTopicFile = file;
                try
                {
                    DisplayFile(file);
                    TOCList.SelectedItem = topic;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"ShowTopic.DisplayFile exception: {ex}");
                    MessageBox.Show($"显示帮助内容失败：{ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show($"未找到帮助主题：{topic}", "帮助", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        void DisplayFile(string fullPath)
        {
            try
            {
                if (string.IsNullOrEmpty(fullPath)) return;

                if (!File.Exists(fullPath))
                {
                    MessageBox.Show($"帮助文件不存在：{fullPath}", "帮助", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (fullPath.EndsWith(".md", StringComparison.OrdinalIgnoreCase))
                {
                    string md = File.ReadAllText(fullPath, Encoding.UTF8);
                    string html;
                    try
                    {
                        // 调用外部渲染器（可能依赖 Markdig）
                        html = HelpContentLoader.RenderMarkdownToHtml(md);
                    }
                    catch (Exception mdEx)
                    {
                        Debug.WriteLine($"Markdown render failed: {mdEx}");
                        // 回退：把 Markdown 原文包在 <pre> 中显示，避免空白
                        html = "<!doctype html><html><head><meta charset='utf-8'/></head><body><pre style='white-space:pre-wrap;'>" +
                               System.Net.WebUtility.HtmlEncode(md) +
                               "</pre></body></html>";
                    }

                    // 渲染后的 HTML 写到应用目录下的 temp 子目录（保持在项目内，便于整个目录拷贝迁移）
                    string tempDir = Path.Combine(AppContext.BaseDirectory, "temp");
                    Directory.CreateDirectory(tempDir);
                    string temp = Path.Combine(tempDir, $"rvhelp_{Guid.NewGuid():N}.html");
                    File.WriteAllText(temp, html, Encoding.UTF8);
                    Debug.WriteLine($"Navigating WebBrowser to temp file: {temp}");
                    HelpBrowser.Navigate(new Uri(temp, UriKind.Absolute));
                }
                else
                {
                    Debug.WriteLine($"Navigating WebBrowser to html file: {fullPath}");
                    HelpBrowser.Navigate(new Uri(fullPath, UriKind.Absolute));
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"DisplayFile exception: {ex}");
                throw;
            }
        }

        private string DefaultGettingStartedMarkdown()
        {
            return @"# 开始使用

这是 RV Reducer 设计工具的快速上手指南。

## 基本流程

1. 在左侧树形结构中选择要编辑的项（如轴、齿轮、轴承）。
2. 在“参数输入”标签页中填写参数。
3. 点击“计算”按钮查看“计算结果”标签页。
4. 使用“保存/打开”来保存工程数据（JSON）。

## 上下文帮助

- 可在“帮助”窗口中使用搜索功能快速定位到说明文本。

";
        }

        private void TOCList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var title = TOCList.SelectedItem as string;
            if (title != null && toc.TryGetValue(title, out var file))
            {
                currentTopicFile = file;
                try
                {
                    DisplayFile(file);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"TOCList_SelectionChanged.DisplayFile exception: {ex}");
                }
            }
        }

        private void SearchBox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                var kw = SearchBox.Text?.Trim();
                if (string.IsNullOrEmpty(kw)) return;

                var results = toc.Where(kv =>
                {
                    var file = kv.Value;
                    if (kv.Key.IndexOf(kw, StringComparison.OrdinalIgnoreCase) >= 0) return true;
                    try
                    {
                        var content = File.ReadAllText(file, Encoding.UTF8);
                        return content.IndexOf(kw, StringComparison.OrdinalIgnoreCase) >= 0;
                    }
                    catch { return false; }
                }).Select(kv => kv.Key).ToList();

                if (results.Count == 0)
                {
                    MessageBox.Show("未找到匹配项。", "搜索", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    TOCList.Items.Clear();
                    foreach (var t in results) TOCList.Items.Add(t);
                }
            }
        }

        private void BtnOpenExternal_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(currentTopicFile) && File.Exists(currentTopicFile))
            {
                try
                {
                    Process.Start(new ProcessStartInfo()
                    {
                        FileName = currentTopicFile,
                        UseShellExecute = true
                    });
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"BtnOpenExternal exception: {ex}");
                    MessageBox.Show("无法在外部打开文件：" + ex.Message, "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void BtnPrint_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                dynamic activeX = HelpBrowser.Document;
                activeX.execCommand("Print", true, null);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"BtnPrint exception: {ex}");
                MessageBox.Show("打印失败：" + ex.Message, "打印", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}