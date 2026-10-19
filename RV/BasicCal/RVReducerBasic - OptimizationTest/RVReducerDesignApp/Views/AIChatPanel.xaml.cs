using OpenAI.Chat;
using RVReducerDesignApp.Services;
using RVReducerDesignApp.Services.Agent;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace RVReducerDesignApp.Views;

/// <summary>
/// AI 聊天面板 UserControl
/// 维护多轮对话历史，调用 DeepSeekChatService 获取 AI 回复
/// </summary>
public partial class AIChatPanel : UserControl
{
    private const int MaxHistoryRounds = 20; // 保留最近 20 轮对话

    private bool _isRunning;
    private AgentRunner? _agentRunner; // 懒加载，避免未配置 API Key 时启动崩溃
    private readonly List<ChatMessage> _history = [];
    private readonly ObservableCollection<ChatBubble> _bubbles = [];

    public AIChatPanel()
    {
        InitializeComponent();
        MessagesList.ItemsSource = _bubbles;
        InitSettingsPanel();
    }

    /// <summary>用当前 DeepSeekConfig 初始化 API 设置面板</summary>
    private void InitSettingsPanel()
    {
        BaseUrlTextBox.Text = DeepSeekConfig.BaseUrl;
        ApiKeyPasswordBox.Password = DeepSeekConfig.ApiKey;
        ModelComboBox.ItemsSource = DeepSeekConfig.AvailableModels;
        ModelComboBox.Text = DeepSeekConfig.Model;
        UpdateSettingsHint();
    }

    private void UpdateSettingsHint()
    {
        SettingsHint.Text = string.IsNullOrWhiteSpace(DeepSeekConfig.ApiKey)
            ? "⚠️ 尚未配置 API Key：请填写后点击“保存设置”。（默认从环境变量 DEEPSEEK_API_KEY 读取）"
            : $"✅ 当前配置：{DeepSeekConfig.BaseUrl} · 模型 {DeepSeekConfig.Model}";
    }

    /// <summary>保存 API 设置（接口地址 / 密钥 / 模型）</summary>
    private void SaveSettings_Click(object sender, RoutedEventArgs e)
    {
        string baseUrl = BaseUrlTextBox.Text.Trim();
        if (!string.IsNullOrWhiteSpace(baseUrl) &&
            !baseUrl.StartsWith("http", StringComparison.OrdinalIgnoreCase))
        {
            SettingsHint.Text = "❌ API 地址必须以 http:// 或 https:// 开头。";
            return;
        }

        string apiKey = ApiKeyPasswordBox.Password.Trim();
        if (string.IsNullOrWhiteSpace(apiKey))
        {
            SettingsHint.Text = "❌ API Key 不能为空。";
            return;
        }

        string model = (ModelComboBox.SelectedItem?.ToString() ?? ModelComboBox.Text).Trim();
        if (string.IsNullOrWhiteSpace(model))
        {
            SettingsHint.Text = "❌ 请选择或输入模型名称。";
            return;
        }

        DeepSeekConfig.Save(apiKey, baseUrl, model);

        // 配置已变更：丢弃旧 AgentRunner，下次发送时按新配置重建
        _agentRunner = null;

        UpdateSettingsHint();
        _bubbles.Add(new ChatBubble
        {
            IsUser = false,
            Content = $"✅ API 设置已更新：{DeepSeekConfig.BaseUrl}\n模型：{DeepSeekConfig.Model}",
        });
        ChatScrollViewer.ScrollToEnd();
    }

    /// <summary>发送按钮点击</summary>
    private async void SendButton_Click(object sender, RoutedEventArgs e)
    {
        await SendMessageAsync();
    }

    /// <summary>输入框回车（Ctrl+Enter 发送）</summary>
    private void InputTextBox_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter && Keyboard.Modifiers == ModifierKeys.Control)
        {
            e.Handled = true;
            _ = SendMessageAsync();
        }
    }

    private async Task SendMessageAsync()
    {
        string userText = InputTextBox.Text.Trim();
        if (string.IsNullOrWhiteSpace(userText) || _isRunning)
            return;

        // 未配置 API Key 时引导用户先完成设置
        if (string.IsNullOrWhiteSpace(DeepSeekConfig.ApiKey))
        {
            SettingsExpander.IsExpanded = true;
            _bubbles.Add(new ChatBubble
            {
                IsUser = false,
                Content = "⚠️ 尚未配置 API Key，请在上方【API 设置】中填写接口地址、密钥与模型后点击“保存设置”。",
            });
            ChatScrollViewer.ScrollToEnd();
            return;
        }

        // 懒加载 AgentRunner（首次使用时创建），配置错误时仅提示不崩溃
        if (_agentRunner is null)
        {
            try
            {
                _agentRunner = new AgentRunner();
            }
            catch (InvalidOperationException ex)
            {
                _bubbles.Add(new ChatBubble { IsUser = false, Content = $"⚠️ {ex.Message}" });
                return;
            }
        }

        // 添加用户气泡
        _bubbles.Add(new ChatBubble { IsUser = true, Content = userText });
        InputTextBox.Clear();

        // 气泡只呈现最终结果；工具调用过程记录到日志文件（logs/agent/），避免聊天区刷屏
        var agentBubble = new ChatBubble { IsUser = false, Content = "⏳ 正在执行…" };
        _bubbles.Add(agentBubble);
        var process = new StringBuilder();
        int step = 0;

        // 进入加载状态
        SetLoading(true);
        _isRunning = true;

        // 订阅执行过程事件：只累积到日志，气泡上仅显示进度
        void OnAgentEvent(AgentEvent ev)
        {
            switch (ev.Kind)
            {
                case AgentEventKind.ToolCall:
                    step++;
                    process.AppendLine($"### {step}. 调用 [{ev.ToolName}]");
                    process.AppendLine("```json");
                    process.AppendLine(ev.Message ?? "");
                    process.AppendLine("```");
                    break;
                case AgentEventKind.ToolResult:
                    process.AppendLine($"**{ev.ToolName} 返回：**");
                    process.AppendLine("```");
                    process.AppendLine(ev.Message ?? "");
                    process.AppendLine("```");
                    break;
                default:
                    process.AppendLine($"**{ev.ToolName} 错误：** {ev.Message}");
                    break;
            }
            process.AppendLine();

            var status = $"⏳ 正在执行…（已调用 {step} 次工具：{ev.ToolName}）";
            _ = Dispatcher.InvokeAsync(() =>
            {
                agentBubble.Content = status;
                ChatScrollViewer.ScrollToEnd();
            });
        }

        _agentRunner.OnEvent += OnAgentEvent;
        try
        {
            // Agent 自主执行：调用工具 → 看结果 → 继续 → 直到完成
            string reply = await _agentRunner.RunAsync(userText, _history);

            // 过程写入项目内文件（logs/agent/agent-<时间戳>.md）
            string? logPath = null;
            try { logPath = AgentRunLog.Save(userText, process.ToString(), reply); } catch { }

            var display = new StringBuilder();
            display.Append(string.IsNullOrWhiteSpace(reply) ? "（本次未生成文字答复）" : reply);
            if (logPath is not null)
                display.Append("\n\n---\n📄 过程与工具调用已存入：").Append(AgentRunLog.ToRelative(logPath));

            await Dispatcher.InvokeAsync(() =>
            {
                agentBubble.Content = display.ToString();
                ChatScrollViewer.ScrollToEnd();
            });

            if (!string.IsNullOrWhiteSpace(reply))
            {
                // 本轮加入对话历史
                _history.Add(new UserChatMessage(userText));
                _history.Add(new AssistantChatMessage(reply));
                TrimHistory();
            }
        }
        catch (OperationCanceledException)
        {
            // 用户取消，忽略
        }
        catch (Exception ex)
        {
            // 异常也写进过程日志，界面上只给简短提示 + 日志路径
            string? logPath = null;
            try { logPath = AgentRunLog.Save(userText, process.ToString(), "（执行过程中出现异常）"); } catch { }

            var text = $"❌ 执行异常：{ex.Message}";
            if (logPath is not null)
                text += $"\n\n---\n📄 过程与工具调用已存入：{AgentRunLog.ToRelative(logPath)}";

            await Dispatcher.InvokeAsync(() => { agentBubble.Content = text; });
        }
        finally
        {
            _agentRunner.OnEvent -= OnAgentEvent;
            _isRunning = false;
            SetLoading(false);
        }
    }

    private static string Truncate(string s, int max) =>
        s.Length <= max ? s : s[..max] + "…";

    private void SetLoading(bool isLoading)
    {
        SendButton.IsEnabled = !isLoading;
        InputTextBox.IsEnabled = !isLoading;
        LoadingBar.Visibility = isLoading ? Visibility.Visible : Visibility.Collapsed;
    }

    /// <summary>裁剪对话历史，保留最近 MaxHistoryRounds 轮</summary>
    private void TrimHistory()
    {
        while (_history.Count > MaxHistoryRounds * 2)
        {
            _history.RemoveAt(0);
        }
    }
}

/// <summary>聊天气泡 ViewModel（用于 ItemsControl 绑定）</summary>
public class ChatBubble : INotifyPropertyChanged
{
    private bool _isUser;
    public bool IsUser
    {
        get => _isUser;
        set { _isUser = value; OnPropertyChanged(nameof(IsUser)); }
    }

    private string _content = string.Empty;
    public string Content
    {
        get => _content;
        set { _content = value; OnPropertyChanged(nameof(Content)); }
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    private void OnPropertyChanged(string name) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
