using System;
using System.Linq;
using System.Text.Json;

namespace RVReducerDesignApp.Services.Agent;

/// <summary>
/// 面向「当前设计工程」的工具（区别于 rv_calc 的通用 DLL 接口调用）：
///   · get_design_state          —— 当前工程概况（是否已设计曲轴/轴径/总体参数）
///   · calc_crank_shaft_system   —— 直接算当前工程的曲轴系，返回最大应力、安全系数与各节点应力
/// 这样用户说“帮我算下当前这个减速器工程的曲轴系统”时，Agent 无需导出快照、也无需 shell 命令。
/// </summary>
public static class AgentMachineTools
{
    /// <summary>计算当前工程的指定部件（摆线针轮副 / 渐开线齿轮副 / 总体 / 曲轴系）</summary>
    public static AgentTool BuildPartCalcTool() => new()
    {
        Name = "calc_current_part",
        Description =
            "计算【当前设计工程】的某个部件，等价于在界面上选中该部件后点「计算」。" +
            "part 取值：摆线针轮副 / 渐开线齿轮副 / 总体 / 曲轴系。" +
            "摆线针轮副、渐开线齿轮副、总体都直接用界面当前参数，**不需要也不要自己填参数**；" +
            "曲轴系需先设计曲轴（否则会返回明确提示）。" +
            "若用户问“某某参数是多少”，先用 get_design_state。",
        Parameters = JsonSchema(new
        {
            type = "object",
            properties = new
            {
                part = new { type = "string", description = "部件名：摆线针轮副 / 渐开线齿轮副 / 总体 / 曲轴系" },
            },
            required = new[] { "part" },
        }),
        ExecuteAsync = (argsJson, _) =>
        {
            try
            {
                using var doc = JsonDocument.Parse(string.IsNullOrWhiteSpace(argsJson) ? "{}" : argsJson);
                var part = doc.RootElement.TryGetProperty("part", out var p) ? p.GetString() ?? "" : "";
                if (string.IsNullOrWhiteSpace(part))
                    return Task.FromResult("错误：缺少 part（可用：摆线针轮副 / 渐开线齿轮副 / 总体 / 曲轴系）");
                return Task.FromResult(CurrentMachineCalc.CalcPart(part).ToText());
            }
            catch (Exception ex)
            {
                return Task.FromResult($"计算失败: {ex.GetType().Name}: {ex.Message}");
            }
        },
    };

    /// <summary>当前设计工程概况</summary>
    public static AgentTool BuildDesignStateTool() => new()
    {
        Name = "get_design_state",
        Description =
            "查看当前设计工程的概况：是否已设计曲轴、轴径/轴长、许用弯曲/剪应力、以及各主要部件是否可用。" +
            "在任何“计算当前工程”的任务前先用它判断前提是否满足。",
        Parameters = JsonSchema(new { type = "object", properties = new { }, required = Array.Empty<string>() }),
        ExecuteAsync = (_, _) =>
        {
            try
            {
                return Task.FromResult(CurrentMachineCalc.DescribeCurrentState());
            }
            catch (Exception ex)
            {
                return Task.FromResult($"读取设计概况失败: {ex.GetType().Name}: {ex.Message}");
            }
        },
    };

    /// <summary>计算当前工程的曲轴系（最大应力 / 安全系数 / 各节点受力）</summary>
    public static AgentTool BuildCrankShaftCalcTool() => new()
    {
        Name = "calc_crank_shaft_system",
        Description =
            "计算【当前设计工程】的曲轴系：同一进程内完成 曲轴系计算 + 各节点应力提取，" +
            "返回最大弯曲正应力、最大剪应力、两者的安全系数（许用/最大）以及每个节点的弯曲/剪应力表。" +
            "若提示未设计曲轴，请先用 get_design_state 确认，并告知用户需要先设计曲轴或执行轴推荐。" +
            "注意：这是应用内的真实计算，不要用 run_command 去做计算。",
        Parameters = JsonSchema(new
        {
            type = "object",
            properties = new
            {
                include_nodes = new { type = "boolean", description = "是否输出各节点明细，默认 true" },
                allow_bending_mpa = new { type = "number", description = "许用弯曲正应力 (MPa)。界面未填写许用应力时用它才能给出安全系数" },
                allow_shear_mpa = new { type = "number", description = "许用剪应力 (MPa)。界面未填写时用它才能给出安全系数" },
            },
            required = Array.Empty<string>(),
        }),
        ExecuteAsync = (argsJson, _) =>
        {
            bool includeNodes = true;
            double? allowBending = null, allowShear = null;
            try
            {
                if (!string.IsNullOrWhiteSpace(argsJson))
                {
                    using var doc = JsonDocument.Parse(argsJson);
                    var root = doc.RootElement;
                    if (root.TryGetProperty("include_nodes", out var v) && v.ValueKind is JsonValueKind.False or JsonValueKind.True)
                        includeNodes = v.GetBoolean();
                    if (root.TryGetProperty("allow_bending_mpa", out var ab) && ab.ValueKind == JsonValueKind.Number)
                        allowBending = ab.GetDouble();
                    if (root.TryGetProperty("allow_shear_mpa", out var asr) && asr.ValueKind == JsonValueKind.Number)
                        allowShear = asr.GetDouble();
                }
            }
            catch
            {
                // 参数解析失败按默认值处理
            }

            var result = CurrentMachineCalc.CalcCrankShaftSystem(allowBending, allowShear);
            var text = result.ToText();
            if (!includeNodes && result.Nodes.Count > 0)
            {
                var firstLine = text.IndexOf("各节点受力/应力", StringComparison.Ordinal);
                if (firstLine > 0) text = text[..firstLine].TrimEnd() + $"\n（已省略 {result.Nodes.Count} 个节点明细）";
            }
            return Task.FromResult(text);
        },
    };

    /// <summary>按型号查轴承库并计算寿命</summary>
    public static AgentTool BuildBearingLifeTool() => new()
    {
        Name = "calc_bearing_life_by_code",
        Description =
            "按【轴承型号】查轴承库（MySQL bearings）并计算寿命 L10h。" +
            "参数：型号（如 30238、6208）+ 转速 n、轴向力 Fa、径向力 Fr。" +
            "尺寸与 Cr/C0r/e/Y/Y0 都从库里自动读取（库中 Cr/C0r 为 kN，内部会换成 N），无需自己估价。" +
            "若型号未命中会返回候选；若缺少工况参数（n/Fr）请先向用户确认，不要自己臆造。",
        Parameters = JsonSchema(new
        {
            type = "object",
            properties = new
            {
                code = new { type = "string", description = "轴承型号，如 30238" },
                n = new { type = "number", description = "转速 (r/min)" },
                fa = new { type = "number", description = "轴向力 Fa (N)，默认 0" },
                fr = new { type = "number", description = "径向力 Fr (N)" },
            },
            required = new[] { "code", "n", "fr" },
        }),
        ExecuteAsync = (argsJson, _) =>
        {
            try
            {
                using var doc = JsonDocument.Parse(string.IsNullOrWhiteSpace(argsJson) ? "{}" : argsJson);
                var root = doc.RootElement;
                string code = root.TryGetProperty("code", out var c) ? c.GetString() ?? "" : "";
                if (string.IsNullOrWhiteSpace(code))
                    return Task.FromResult("错误：缺少型号 code");

                double GetNum(string name, double fallback = 0) =>
                    root.TryGetProperty(name, out var v) && v.ValueKind == JsonValueKind.Number ? v.GetDouble() : fallback;

                double n = GetNum("n");
                double fr = GetNum("fr");
                double fa = GetNum("fa");
                if (n <= 0) return Task.FromResult("错误：转速 n 必须大于 0");
                if (fr <= 0 && fa <= 0) return Task.FromResult("错误：至少给出 Fr 或 Fa（否则载荷为 0，寿命无意义）");

                var result = BearingCatalog.CalcLife(code, n, fa, fr);
                return Task.FromResult(result.ToText());
            }
            catch (Exception ex)
            {
                return Task.FromResult($"查库/计算失败: {ex.GetType().Name}: {ex.Message}");
            }
        },
    };

    private static BinaryData JsonSchema(object schema) =>
        BinaryData.FromString(JsonSerializer.Serialize(schema));
}
