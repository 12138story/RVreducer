using System.IO;
using System.Text.Json;

namespace RVReducerDesignApp.Services;

/// <summary>
/// DeepSeek API 配置
/// 默认值：API Key 取自环境变量 DEEPSEEK_API_KEY，端点与模型使用内置默认值；
/// 可在 AI 页面「API 设置」中覆盖，覆盖值持久化到 %APPDATA%\RVReducerDesignApp\ai-config.json。
/// </summary>
public static class DeepSeekConfig
{
    private const string DefaultBaseUrl = "https://api.deepseek.com";
    private const string DefaultModel = "deepseek-v4-flash";

    private static readonly string ConfigDir = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "RVReducerDesignApp");
    private static readonly string ConfigPath = Path.Combine(ConfigDir, "ai-config.json");

    /// <summary>API Key</summary>
    public static string ApiKey { get; private set; }

    /// <summary>API 端点（OpenAI 兼容格式）</summary>
    public static string BaseUrl { get; private set; }

    /// <summary>当前使用的模型</summary>
    public static string Model { get; private set; }

    /// <summary>UI 下拉框可选的模型列表</summary>
    public static readonly string[] AvailableModels =
    {
        "deepseek-v4-flash",
        "deepseek-v4-pro",
        "deepseek-chat",
        "deepseek-reasoner",
    };

    static DeepSeekConfig()
    {
        ApiKey = Environment.GetEnvironmentVariable("DEEPSEEK_API_KEY") ?? "";
        BaseUrl = DefaultBaseUrl;
        Model = DefaultModel;
        LoadPersisted();
    }

    /// <summary>应用新配置并持久化到本地配置文件</summary>
    public static void Save(string apiKey, string baseUrl, string model)
    {
        ApiKey = (apiKey ?? "").Trim();
        BaseUrl = string.IsNullOrWhiteSpace(baseUrl) ? DefaultBaseUrl : baseUrl.Trim();
        Model = string.IsNullOrWhiteSpace(model) ? DefaultModel : model.Trim();

        try
        {
            Directory.CreateDirectory(ConfigDir);
            File.WriteAllText(ConfigPath, JsonSerializer.Serialize(
                new { ApiKey, BaseUrl, Model },
                new JsonSerializerOptions { WriteIndented = true }));
        }
        catch
        {
            // 持久化失败不影响本次会话中的配置生效
        }
    }

    private static void LoadPersisted()
    {
        try
        {
            if (!File.Exists(ConfigPath))
                return;
            using var doc = JsonDocument.Parse(File.ReadAllText(ConfigPath));
            JsonElement root = doc.RootElement;
            if (root.TryGetProperty("ApiKey", out var key) && key.ValueKind == JsonValueKind.String)
                ApiKey = key.GetString() ?? ApiKey;
            if (root.TryGetProperty("BaseUrl", out var url) && url.ValueKind == JsonValueKind.String)
                BaseUrl = url.GetString() ?? BaseUrl;
            if (root.TryGetProperty("Model", out var model) && model.ValueKind == JsonValueKind.String)
                Model = model.GetString() ?? Model;
        }
        catch
        {
            // 配置文件损坏时忽略，退回默认值
        }
    }

    /// <summary>
    /// 领域知识系统提示词（不含项目文档，项目文档由 ProjectContextLoader 动态拼接）
    /// </summary>
    public const string SystemPrompt =
        "你是 RV 减速器设计专家助手，精通以下领域：\n" +
        "- 渐开线齿轮副（太阳轮/行星轮）的设计计算，包括接触强度、弯曲强度校核（ISO 6336）\n" +
        "- 摆线针轮副的设计计算，包括摆线轮齿形、针齿销、跨棒距、K1 系数\n" +
        "- 曲轴（偏心轴）设计，包括转臂轴承选型与寿命计算\n" +
        "- 轴承选型（深沟球轴承、圆锥滚子轴承、滚针轴承、角接触球轴承等）\n" +
        "- 轴系设计与强度分析\n" +
        "请用中文回答，回答应简洁、专业、准确。对于不确定的问题，请诚实说明。";
}
