using OpenAI;
using OpenAI.Chat;
using System.ClientModel;
using System.Runtime.CompilerServices;

namespace RVReducerDesignApp.Services;

/// <summary>
/// DeepSeek 对话 API 服务（基于 OpenAI .NET SDK 2.12.0）
/// 封装非流式与流式对话调用，支持多轮对话历史
/// </summary>
public class DeepSeekChatService
{
    /// <summary>
    /// 默认系统提示词（领域知识 + 动态项目文档），惰性初始化、跨实例共享
    /// </summary>
    private static readonly string DefaultSystemPrompt =
        ProjectContextLoader.BuildSystemPrompt();

    private readonly ChatClient _client;

    public DeepSeekChatService()
    {
        _client = new ChatClient(
            model: DeepSeekConfig.Model,
            credential: new ApiKeyCredential(DeepSeekConfig.ApiKey),
            options: new OpenAIClientOptions
            {
                Endpoint = new Uri(DeepSeekConfig.BaseUrl),
                NetworkTimeout = TimeSpan.FromSeconds(60)
            });
    }

    /// <summary>
    /// 非流式对话 — 一次返回完整结果
    /// </summary>
    /// <param name="userMessage">用户当前消息</param>
    /// <param name="history">可选的多轮对话历史</param>
    /// <param name="ct">取消令牌</param>
    /// <returns>助手回复文本；出错时返回以"❌"开头的错误信息</returns>
    public async Task<string> ChatAsync(
        string userMessage,
        List<ChatMessage>? history = null,
        string? systemPrompt = null,
        CancellationToken ct = default)
    {
        var messages = BuildMessages(userMessage, history, systemPrompt);

        try
        {
            ChatCompletion completion = await _client.CompleteChatAsync(messages, cancellationToken: ct);
            return completion.Content.FirstOrDefault()?.Text ?? "";
        }
        catch (OperationCanceledException)
        {
            throw; // 用户取消操作，不应吞没
        }
        catch (ClientResultException ex) when (ex.Status == 401 || ex.Status == 403)
        {
            return "❌ API Key 无效或无权访问，请检查 DEEPSEEK_API_KEY 环境变量配置。";
        }
        catch (ClientResultException ex) when (ex.Status == 429)
        {
            return "⏳ API 请求过于频繁（速率限制），请稍后重试。";
        }
        catch (ClientResultException ex)
        {
            return $"⚠️ API 调用失败 (HTTP {ex.Status}): {ex.Message}";
        }
        catch (Exception ex)
        {
            return $"❌ 未知错误: {ex.GetType().Name}: {ex.Message}";
        }
    }

    /// <summary>
    /// 流式对话 — 逐段返回文本，适合打字机效果。
    /// 注意：调用方应在 await foreach 外套 try-catch 处理异常。
    /// </summary>
    public async IAsyncEnumerable<string> ChatStreamAsync(
        string userMessage,
        List<ChatMessage>? history = null,
        string? systemPrompt = null,
        [EnumeratorCancellation] CancellationToken ct = default)
    {
        var messages = BuildMessages(userMessage, history, systemPrompt);

        await foreach (StreamingChatCompletionUpdate update
                       in _client.CompleteChatStreamingAsync(messages, cancellationToken: ct))
        {
            foreach (var contentPart in update.ContentUpdate)
            {
                if (!string.IsNullOrEmpty(contentPart.Text))
                    yield return contentPart.Text;
            }
        }
    }

    /// <summary>构建消息列表：系统提示词 + 历史 + 当前用户消息</summary>
    private static List<ChatMessage> BuildMessages(
        string userMessage, List<ChatMessage>? history, string? systemPrompt)
    {
        var messages = new List<ChatMessage>();

        // 系统提示词放在最前面，定义 AI 角色和项目背景
        messages.Add(new SystemChatMessage(
            systemPrompt ?? DefaultSystemPrompt));

        if (history != null)
            messages.AddRange(history);

        messages.Add(new UserChatMessage(userMessage));
        return messages;
    }
}
