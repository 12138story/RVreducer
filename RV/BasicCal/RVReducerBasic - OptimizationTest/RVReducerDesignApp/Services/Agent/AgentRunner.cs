using OpenAI;
using OpenAI.Chat;
using System.ClientModel;

namespace RVReducerDesignApp.Services.Agent;

/// <summary>
/// Agent 循环引擎：像 Copilot CLI 一样，根据自然语言自主调用工具、
/// 查看结果、继续决策，直到完成用户任务。
/// </summary>
public class AgentRunner
{
    private const int MaxIterations = 15; // 最大工具调用轮次，防止死循环

    private readonly ChatClient _client;
    private readonly List<AgentTool> _tools;
    private readonly List<ChatTool> _chatTools;
    private readonly string _defaultSystemPrompt =
        ProjectContextLoader.BuildSystemPrompt();

    /// <summary>执行过程事件（UI 订阅以显示"正在执行…"）</summary>
    public event Action<AgentEvent>? OnEvent;

    /// <param name="tools">自定义工具集；为 null 时使用 AgentToolbox.GetAll()（界面模式全套工具）。
    /// 终端模式（--chat）可传入 AgentCliConsole.GetCliTools() 精简集。</param>
    public AgentRunner(IEnumerable<AgentTool>? tools = null)
    {
        _client = new ChatClient(
            model: DeepSeekConfig.Model,
            credential: new ApiKeyCredential(DeepSeekConfig.ApiKey),
            options: new OpenAIClientOptions
            {
                Endpoint = new Uri(DeepSeekConfig.BaseUrl),
                NetworkTimeout = TimeSpan.FromSeconds(180)
            });

        _tools = (tools ?? AgentToolbox.GetAll()).ToList();
        _chatTools = _tools.Select(t => ChatTool.CreateFunctionTool(
                functionName: t.Name,
                functionDescription: t.Description,
                functionParameters: t.Parameters))
            .ToList();
    }

    /// <summary>
    /// 执行 Agent 任务，返回最终答复文本
    /// </summary>
    public async Task<string> RunAsync(
        string userMessage,
        List<ChatMessage>? history = null,
        string? systemPrompt = null,
        CancellationToken ct = default)
    {
        var messages = new List<ChatMessage>
        {
            new SystemChatMessage(BuildAgentPrompt(systemPrompt))
        };
        if (history != null)
            messages.AddRange(history);
        messages.Add(new UserChatMessage(userMessage));

        for (int step = 0; step < MaxIterations; step++)
        {
            var options = new ChatCompletionOptions();
            foreach (var tool in _chatTools)
                options.Tools.Add(tool);

            ChatCompletion completion =
                await _client.CompleteChatAsync(messages, options, ct);

            // 模型没有要求调用工具 → 任务完成（空 content 兜底）
            if (completion.ToolCalls.Count == 0)
                return completion.Content.FirstOrDefault()?.Text ?? "";

            // 记录 assistant 的工具调用消息
            messages.Add(new AssistantChatMessage(completion));

            // 逐个执行工具并回填结果
            foreach (ChatToolCall call in completion.ToolCalls)
            {
                AgentTool? tool = _tools.FirstOrDefault(t => t.Name == call.FunctionName);
                string args = call.FunctionArguments.ToString();

                OnEvent?.Invoke(AgentEvent.ForToolCall(call.FunctionName, args));

                string result;
                if (tool is null)
                {
                    result = $"未知工具: {call.FunctionName}";
                }
                else
                {
                    try
                    {
                        result = await tool.ExecuteAsync(args, ct);
                    }
                    catch (OperationCanceledException) when (ct.IsCancellationRequested)
                    {
                        throw; // 用户取消，向上传播
                    }
                    catch (Exception ex)
                    {
                        result = $"工具执行失败: {ex.Message}";
                    }
                }

                // 截断超长输出，避免超出上下文
                if (result.Length > 12000)
                    result = result[..12000] + "\n...[输出已截断]";

                OnEvent?.Invoke(AgentEvent.ForToolResult(call.FunctionName, result));
                messages.Add(ChatMessage.CreateToolMessage(call.Id, result));
            }
        }

        return "⚠️ 已达到最大执行轮次，任务可能未完成。请尝试拆分成更小的步骤。\n" +
               "提示：计算【当前工程】的曲轴系请用 calc_crank_shaft_system；计算任意 DLL 接口用 rv_calc；" +
               "run_command 只能跑编译/测试类命令，不能做数值计算。";
    }

    private string BuildAgentPrompt(string? customPrompt)
    {
        var basePrompt = customPrompt ?? _defaultSystemPrompt;
        // 按实际注入的工具动态生成清单，避免提示词里出现当前会话不可用的工具名
        var toolList = string.Join("\n", _tools.Select(t => $"- {t.Name}：{OneLine(t.Description, 200)}"));
        return basePrompt + "\n\n" +
            "【执行模式】你是一个可以操作本项目的智能体，可用工具：\n" + toolList + "\n" +
            "规则：1) 需要执行操作时先调用工具，看到结果后再决定下一步；\n" +
            "2) 不要臆造命令执行结果或计算数值，必须基于工具返回的真实输出；\n" +
            "3) 任务完成后用中文总结结果。";
    }

    private static string OneLine(string s, int max)
    {
        s = s.Replace('\n', ' ');
        return s.Length <= max ? s : s[..max] + "…";
    }
}
