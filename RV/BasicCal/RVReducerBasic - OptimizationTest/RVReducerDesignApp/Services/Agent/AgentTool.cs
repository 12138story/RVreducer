namespace RVReducerDesignApp.Services.Agent;

/// <summary>
/// Agent 工具定义：名称、描述、参数 JSON Schema、执行函数
/// </summary>
public class AgentTool
{
    /// <summary>工具名称（模型调用时使用，如 run_command）</summary>
    public required string Name { get; init; }

    /// <summary>工具功能描述（告诉模型何时使用）</summary>
    public required string Description { get; init; }

    /// <summary>参数 JSON Schema（OpenAI function calling 格式）</summary>
    public required BinaryData Parameters { get; init; }

    /// <summary>执行函数：输入参数 JSON，返回结果文本</summary>
    public required Func<string, CancellationToken, Task<string>> ExecuteAsync { get; init; }
}

/// <summary>Agent 执行事件类型（供 UI 显示执行过程）</summary>
public enum AgentEventKind
{
    /// <summary>模型决定调用工具</summary>
    ToolCall,
    /// <summary>工具执行结果</summary>
    ToolResult,
    /// <summary>过程中的错误</summary>
    Error,
}

/// <summary>Agent 执行过程事件</summary>
public class AgentEvent
{
    public required AgentEventKind Kind { get; init; }
    public required string ToolName { get; init; }
    public required string Message { get; init; }

    public static AgentEvent ForToolCall(string name, string args) =>
        new() { Kind = AgentEventKind.ToolCall, ToolName = name, Message = args };

    public static AgentEvent ForToolResult(string name, string output) =>
        new() { Kind = AgentEventKind.ToolResult, ToolName = name, Message = output };

    public static AgentEvent ForError(string name, string message) =>
        new() { Kind = AgentEventKind.Error, ToolName = name, Message = message };
}
