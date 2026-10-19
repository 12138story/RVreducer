using System.IO;
using System.Text;

namespace RVReducerDesignApp.Services;

/// <summary>
/// 项目上下文加载器
/// 从工作区自动读取项目说明文档，拼接到系统提示词中，
/// 使 AI 能够基于本项目的真实信息回答，而非凭空臆造。
/// </summary>
public static class ProjectContextLoader
{
    /// <summary>工作区根目录的标识文件</summary>
    private const string WorkspaceMarker = "RVReducerBasic.sln";

    /// <summary>每个文档最大读取字符数</summary>
    private const int MaxCharsPerDoc = 6000;

    /// <summary>项目上下文总字符上限（防止 token 爆炸）</summary>
    private const int MaxTotalChars = 20000;

    /// <summary>相对于工作区根的项目文档候选路径</summary>
    private static readonly string[] DocRelativePaths =
    {
        @"RvCalc\README.md",
        @".github\skills\rv-build-test\SKILL.md",
    };

    /// <summary>
    /// 构建完整系统提示词：静态领域知识 + 动态项目文档 + 回答约束
    /// </summary>
    public static string BuildSystemPrompt()
    {
        var sb = new StringBuilder();

        // 1. 静态领域知识（定义 AI 角色）
        sb.AppendLine(DeepSeekConfig.SystemPrompt);
        sb.AppendLine();

        // 2. 工具使用原则（避免选错工具导致反复尝试/轮次耗尽）
        sb.AppendLine("【工具使用原则】");
        sb.AppendLine("- 计算【当前这个设计工程】的**某个部件**（摆线针轮副 / 渐开线齿轮副 / 总体 / 曲轴系）：用 calc_current_part（part=部件名）。" +
                      "它直接用界面当前参数计算，**不需要也不要自己填参数**。");
        sb.AppendLine("- 查看当前工程的参数（用户问“某某参数是多少”）：用 get_design_state。");
        sb.AppendLine("- 计算【当前这个设计工程】的曲轴系/轴应力（含安全系数与各节点受力）：用 calc_crank_shaft_system；先用 get_design_state 确认前提是否满足。");
        sb.AppendLine("- 查询轴承库/算轴承寿命（给型号即可）：用 calc_bearing_life_by_code（需要型号 + 转速 n + 径向力 Fr，轴向力 Fa 可选）。" +
                      "尺寸与 Cr/C0r/e/Y 由工具自动从库中读取，不要自己估值或猜测。");
        sb.AppendLine("- 计算任意 DLL 接口（渐开线齿轮副、摆线针轮副、齿轮推荐、公差反查等）：用 rv_calc（可指定 interface 与 params）。");
        sb.AppendLine("- 导出当前整机数据以便外部复现：用 export_whole_machine_snapshot。");
        sb.AppendLine("- 不要用 run_command 做数值计算：它只能运行 dotnet/build/git 等编译测试类命令，不支持管道、引号参数与计算表达式。");
        sb.AppendLine("- 若前提不满足（例如尚未设计曲轴），直接告知用户需要先做什么，不要反复尝试其它工具。");
        sb.AppendLine();

        // 2. 动态项目上下文（来自真实文档）
        string projectContext = BuildProjectContext();
        if (!string.IsNullOrEmpty(projectContext))
        {
            sb.AppendLine("【本项目背景】以下是本项目的实际说明文档：");
            sb.AppendLine(projectContext);
            sb.AppendLine();
            sb.AppendLine("回答要求：涉及本项目的问题，请优先参考上述【本项目背景】回答；");
            sb.AppendLine("若文档中未涉及，请明确说明“项目文档中未找到相关信息”，不要编造本项目内容。");
        }

        return sb.ToString();
    }

    /// <summary>读取并拼接项目文档内容</summary>
    private static string BuildProjectContext()
    {
        string? workspaceRoot = FindWorkspaceRoot();
        if (workspaceRoot == null)
            return string.Empty;

        var sb = new StringBuilder();
        int totalChars = 0;

        foreach (string relPath in DocRelativePaths)
        {
            string fullPath = Path.Combine(workspaceRoot, relPath);
            if (!File.Exists(fullPath))
                continue;

            string content;
            try
            {
                content = File.ReadAllText(fullPath, Encoding.UTF8);
            }
            catch (Exception ex)
            {
                // 单文档读取失败不影响整体，记录并跳过
                System.Diagnostics.Debug.WriteLine($"读取项目文档失败 {fullPath}: {ex.Message}");
                continue;
            }

            // 裁剪长度，避免超出上限
            if (content.Length > MaxCharsPerDoc)
                content = content[..MaxCharsPerDoc];

            sb.AppendLine($"===== 文档: {relPath} =====");
            sb.AppendLine(content);
            sb.AppendLine();

            totalChars += content.Length;
            if (totalChars >= MaxTotalChars)
                break;
        }

        return sb.ToString();
    }

    /// <summary>从运行目录向上查找工作区根目录（包含 RVReducerBasic.sln）</summary>
    private static string? FindWorkspaceRoot()
    {
        var dir = new DirectoryInfo(AppContext.BaseDirectory);
        while (dir != null)
        {
            if (File.Exists(Path.Combine(dir.FullName, WorkspaceMarker)))
                return dir.FullName;
            dir = dir.Parent;
        }
        return null;
    }
}
