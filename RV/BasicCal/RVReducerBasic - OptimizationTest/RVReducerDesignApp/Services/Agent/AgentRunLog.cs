using System;
using System.IO;
using System.Text;

namespace RVReducerDesignApp.Services.Agent;

/// <summary>
/// Agent 执行记录：把「工具调用过程与分析」写入项目内文件（默认 &lt;工作区&gt;/logs/agent/），
/// 聊天界面只呈现最终结果，避免过程刷屏。
/// </summary>
public static class AgentRunLog
{
    /// <summary>写入一次执行的完整记录，返回文件绝对路径</summary>
    public static string Save(string question, string process, string answer)
    {
        var dir = Path.Combine(WorkspaceRoot(), "logs", "agent");
        Directory.CreateDirectory(dir);

        var path = Path.Combine(dir, $"agent-{DateTime.Now:yyyyMMdd-HHmmss}.md");
        for (int i = 2; File.Exists(path); i++)
            path = Path.Combine(dir, $"agent-{DateTime.Now:yyyyMMdd-HHmmss}-{i}.md");

        var sb = new StringBuilder();
        sb.AppendLine("# Agent 执行记录");
        sb.AppendLine();
        sb.AppendLine($"- 时间：{DateTime.Now:yyyy-MM-dd HH:mm:ss}");
        sb.AppendLine($"- 问题：{question}");
        sb.AppendLine();
        sb.AppendLine("## 执行过程（工具调用与分析素材）");
        sb.AppendLine();
        sb.AppendLine(string.IsNullOrWhiteSpace(process) ? "（本次未调用工具）" : process.TrimEnd());
        sb.AppendLine();
        sb.AppendLine("## 最终答复");
        sb.AppendLine();
        sb.AppendLine(string.IsNullOrWhiteSpace(answer) ? "（无文字答复）" : answer);

        // UTF-8 无 BOM，便于跨工具查看
        File.WriteAllText(path, sb.ToString(), new UTF8Encoding(false));
        return path;
    }

    /// <summary>转成相对工作区的路径（用于在聊天里展示，避免长绝对路径）</summary>
    public static string ToRelative(string fullPath)
    {
        try
        {
            var rel = Path.GetRelativePath(WorkspaceRoot(), fullPath);
            return rel.StartsWith("..") ? fullPath : rel.Replace('\\', '/');
        }
        catch
        {
            return fullPath;
        }
    }

    private static string WorkspaceRoot() =>
        WholeMachineSnapshot.FindWorkspaceRoot() ?? AppContext.BaseDirectory;
}
