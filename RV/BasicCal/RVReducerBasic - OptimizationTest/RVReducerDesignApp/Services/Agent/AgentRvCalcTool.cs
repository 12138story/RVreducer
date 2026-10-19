using System.IO;
using System.Text.Json;
using RvCalc;

namespace RVReducerDesignApp.Services.Agent;

/// <summary>
/// 通用计算工具：由 RvCalc 注册表驱动，用自然语言参数调用 RVReducerBasic.dll 的任意导出接口。
///
/// 与 <see cref="AgentCalcTools"/> 的差别：
///   AgentCalcTools  = 为某一个接口写死的 C# 代码（不全面时就要新增代码）
///   本工具          = 参数名/槽位/单位全部来自 RvCalc/interface-registry.json，
///                     新增接口只改 JSON，不改代码；未知参数名/缺必填直接报错而非静默用默认值。
/// </summary>
public static class AgentRvCalcTool
{
    private static readonly object Gate = new();
    private static RvInterfaceRegistry? _registry;
    private static RvCalcRunner? _runner;
    private static string? _loadError;

    /// <summary>构建 rv_calc 工具</summary>
    public static AgentTool Build()
    {
        EnsureLoaded();
        return new AgentTool
        {
            Name = "rv_calc",
            Description =
                "调用 RVReducerBasic.dll 的任意计算接口（通用注册表驱动）。" +
                "用法：先按下面的接口清单确定 interface 名，再把该接口声明的「参数名」放进 params。" +
                "参数名可用中文主名或别名；数值可带单位（如 \"23.8 kN\"）；也可只给部分参数，其余用已验证的默认模板。\n\n" +
                (IsReady
                    ? Registry!.Describe()
                    : $"⚠ 计算注册表不可用：{_loadError}\n请检查 RvCalc/interface-registry.json 是否存在。"),
            Parameters = JsonSchema(new
            {
                type = "object",
                properties = new
                {
                    @interface = new
                    {
                        type = "string",
                        description = "接口名（DLL 导出名），如 CalBearingLife / CalInvGearStress2 / CalCycloidGearStressByObj / RecommondPara",
                    },
                    @params = new
                    {
                        type = "object",
                        description = "该接口的参数键值对，键用接口清单里声明的参数名（中文或别名），值为数字或字符串",
                    },
                },
                required = new[] { "interface" },
            }),
            ExecuteAsync = (argsJson, _) =>
            {
                EnsureLoaded();
                if (!IsReady)
                    return Task.FromResult($"⚠ 计算不可用：{_loadError}");

                try
                {
                    using var doc = JsonDocument.Parse(argsJson);
                    var root = doc.RootElement;
                    var iface = root.TryGetProperty("interface", out var i) ? i.GetString() : null;
                    if (string.IsNullOrWhiteSpace(iface))
                        return Task.FromResult("错误：缺少 interface 参数");

                    var paramsJson = root.TryGetProperty("params", out var p) && p.ValueKind == JsonValueKind.Object
                        ? p.GetRawText()
                        : "{}";

                    lock (Gate)
                    {
                        var result = Runner!.RunJson(iface, paramsJson);
                        return Task.FromResult(result.ToText());
                    }
                }
                catch (Exception ex)
                {
                    return Task.FromResult($"计算失败: {ex.GetType().Name}: {ex.Message}");
                }
            },
        };
    }

    /// <summary>构建 export_whole_machine_snapshot 工具（自然语言导出整机快照）</summary>
    public static AgentTool BuildSnapshotExporter() => new()
    {
        Name = "export_whole_machine_snapshot",
        Description =
            "把当前界面上的整机设计数据（总体/渐开线/摆线/轴承/曲轴）导出为 JSON 快照。" +
            "此后可用 RvCalcCli 或 rv_calc 工具以 arraysFile 指向该快照，在无界面环境下复现 " +
            "CalCrankShaftSystem2 / RecommondShaftPara 等需要整机数据的计算。" +
            "注意：需先完成曲轴设计/轴推荐，否则快照缺轴段数据（meta.partial=true），不足以驱动曲轴系计算。",
        Parameters = JsonSchema(new
        {
            type = "object",
            properties = new
            {
                output_path = new { type = "string", description = "快照输出路径（留空则写到 工作区/RvCalc/templates/wholeMachine-snapshot.json）" },
            },
            required = Array.Empty<string>(),
        }),
        ExecuteAsync = (argsJson, _) =>
        {
            try
            {
                string? path = null;
                if (!string.IsNullOrWhiteSpace(argsJson))
                {
                    using var doc = JsonDocument.Parse(argsJson);
                    if (doc.RootElement.TryGetProperty("output_path", out var p) && p.ValueKind == JsonValueKind.String)
                        path = p.GetString();
                }

                var full = WholeMachineSnapshot.Export(path);
                var json = File.ReadAllText(full);
                var partial = json.Contains("\"partial\": true");
                var hint = partial
                    ? "⚠ meta.partial=true：轴段数据缺失（尚未设计曲轴/未执行轴推荐），该快照不足以驱动 CalCrankShaftSystem2。\n"
                    : "";
                return Task.FromResult(
                    $"✅ 整机快照已导出: {full}\n{hint}" +
                    $"用法: RvCalcCli --call CalCrankShaftSystem2 --args '{{\"arraysFile\":\"{full}\"}}'");
            }
            catch (Exception ex)
            {
                return Task.FromResult($"导出失败: {ex.GetType().Name}: {ex.Message}");
            }
        },
    };

    /// <summary>接口清单（供 UI/调试展示）</summary>
    public static string Describe() => IsReady ? Registry!.Describe() : $"⚠ {_loadError}";

    private static bool IsReady => _runner is not null && _registry is not null;

    private static RvInterfaceRegistry? Registry => _registry;

    private static RvCalcRunner? Runner => _runner;

    private static void EnsureLoaded()
    {
        lock (Gate)
        {
            if (IsReady) return;
            try
            {
                var path = RvInterfaceRegistry.ResolvePath(
                    Path.Combine(AppContext.BaseDirectory, "RvCalc", "interface-registry.json"));
                if (path is null)
                {
                    _loadError = "未找到 RvCalc/interface-registry.json（应随应用一起复制到输出目录）";
                    return;
                }
                var registry = RvInterfaceRegistry.Load(path);
                var runner = new RvCalcRunner(registry);
                _registry = registry;
                _runner = runner;
                _loadError = null;
            }
            catch (Exception ex)
            {
                _loadError = $"{ex.GetType().Name}: {ex.Message}";
            }
        }
    }

    /// <summary>构造 OpenAI function calling 的 JSON Schema</summary>
    private static BinaryData JsonSchema(object schema) =>
        BinaryData.FromString(JsonSerializer.Serialize(schema));
}
