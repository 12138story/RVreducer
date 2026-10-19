using System.IO;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace RvCalc;

/// <summary>DLL 导出的一个数组参数（in/out/inout）</summary>
public sealed class RvArgDesc
{
    /// <summary>数组名（与注册表 params/outputs 中 slot 的前缀一致，如 "in"/"out"/"div"）</summary>
    public string Name { get; set; } = "";
    /// <summary>元素类型：double | int | strings（char** 字符串数组输出）</summary>
    public string Type { get; set; } = "double";
    /// <summary>数组长度（strings 类型可忽略）</summary>
    public int Length { get; set; }
    /// <summary>方向：in | out | inout</summary>
    public string Direction { get; set; } = "in";
    /// <summary>strings 类型：最大条数（取返回值为准，二者较小者）</summary>
    public int MaxCount { get; set; }
    /// <summary>strings 类型：单条最大长度（含结束符）</summary>
    public int MaxLen { get; set; } = 64;

    public bool IsInput => Direction is "in" or "inout";
    public bool IsOutput => Direction is "out" or "inout";
}

/// <summary>对外暴露的语义参数（自然语言/JSON 键 → 数组槽位）</summary>
public sealed class RvParamDesc
{
    /// <summary>中文主名称（同时也是自然语言匹配的首选键）</summary>
    public string Name { get; set; } = "";
    /// <summary>别名（英文/缩写/同义词）</summary>
    public List<string> Alias { get; set; } = new();
    /// <summary>写入槽位，如 "in[62]"；多槽位用 Slots</summary>
    public string? Slot { get; set; }
    /// <summary>多槽位写入（如行星轮个数同时写公共几何/应用段/装配段）</summary>
    public List<string> Slots { get; set; } = new();
    /// <summary>number | enum</summary>
    public string Type { get; set; } = "number";
    /// <summary>enum 取值表（中文名 → 数值）</summary>
    public Dictionary<string, double> Options { get; set; } = new();
    /// <summary>单位（仅用于输出/文档展示）</summary>
    public string Unit { get; set; } = "";
    /// <summary>缺省值：用户未提供且模板中该槽为 0 时写入</summary>
    public double? Default { get; set; }
    /// <summary>是否必填（缺省且无 Default 时报错）</summary>
    public bool Required { get; set; }
    /// <summary>单位换算：none | deg2rad（输入按角度给，写入时乘 Degree）</summary>
    public string Convert { get; set; } = "none";
    public string? Desc { get; set; }

    public IEnumerable<string> AllSlots =>
        Slots.Count > 0 ? Slots : (Slot is null ? Array.Empty<string>() : new[] { Slot });
}

/// <summary>输出字段</summary>
public sealed class RvOutputDesc
{
    public string Name { get; set; } = "";
    public string Slot { get; set; } = "";
    public string Unit { get; set; } = "";
    public string? Desc { get; set; }
    /// <summary>小于该值视为无效（如寿命 ≤0）</summary>
    public double? InvalidBelow { get; set; }
    /// <summary>大于该值视为无效（如应力 ≥1e9 失败标记）</summary>
    public double? InvalidAbove { get; set; }
}

/// <summary>一个 DLL 导出的完整描述</summary>
public sealed class RvInterfaceDesc
{
    public string Name { get; set; } = "";
    public string Title { get; set; } = "";
    public string Summary { get; set; } = "";
    /// <summary>void | int</summary>
    public string Returns { get; set; } = "void";
    public List<RvArgDesc> Args { get; set; } = new();
    public List<RvParamDesc> Params { get; set; } = new();
    public List<RvOutputDesc> Outputs { get; set; } = new();
    /// <summary>内联模板（数组名 → 初值）</summary>
    public Dictionary<string, double[]> Template { get; set; } = new();
    /// <summary>外部模板文件（相对注册表所在目录），结构 {"in":[...],"div":[...]}</summary>
    public string? TemplateFile { get; set; }
    /// <summary>low | medium | high；high 表示需要完整输入（如整机/轴系）</summary>
    public string Complexity { get; set; } = "low";
    public string? Note { get; set; }
    /// <summary>输出数组的行宽（如 142），用于把扁平输出按行展开</summary>
    public int? OutRowWidth { get; set; }
    /// <summary>行标签（如 ["太阳轮","行星轮"]）</summary>
    public List<string> RowLabels { get; set; } = new();

    [JsonIgnore] internal Dictionary<string, double[]> LoadedTemplate { get; set; } = new();

    public RvArgDesc? Arg(string name) =>
        Args.FirstOrDefault(a => string.Equals(a.Name, name, StringComparison.OrdinalIgnoreCase));
}

/// <summary>接口注册表：从 JSON 加载，提供名称查找与自然语言描述</summary>
public sealed class RvInterfaceRegistry
{
    public int Version { get; set; } = 1;
    public string? Note { get; set; }
    public List<RvInterfaceDesc> Interfaces { get; set; } = new();

    [JsonIgnore] public string BaseDirectory { get; private set; } = "";

    /// <summary>加载过程中的非致命问题（如模板文件缺失）</summary>
    public List<string> LoadWarnings { get; } = new();

    /// <summary>接口名 → 模板是否就绪</summary>
    public bool HasTemplate(RvInterfaceDesc desc) => desc.LoadedTemplate.Count > 0;

    private static readonly JsonSerializerOptions JsonOpts = new()
    {
        PropertyNameCaseInsensitive = true,
        ReadCommentHandling = JsonCommentHandling.Skip,
        AllowTrailingCommas = true,
        Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        NumberHandling = JsonNumberHandling.AllowNamedFloatingPointLiterals,
    };

    /// <summary>加载注册表并读取外部模板文件</summary>
    public static RvInterfaceRegistry Load(string path)
    {
        if (!File.Exists(path))
            throw new FileNotFoundException($"接口注册表不存在: {path}", path);

        var registry = JsonSerializer.Deserialize<RvInterfaceRegistry>(File.ReadAllText(path), JsonOpts)
                       ?? throw new InvalidDataException($"接口注册表解析失败: {path}");
        registry.BaseDirectory = Path.GetDirectoryName(Path.GetFullPath(path)) ?? "";

        foreach (var itf in registry.Interfaces)
        {
            itf.LoadedTemplate = new Dictionary<string, double[]>(itf.Template, StringComparer.OrdinalIgnoreCase);
            if (itf.TemplateFile is { Length: > 0 })
            {
                var file = Path.Combine(registry.BaseDirectory, itf.TemplateFile);
                if (!File.Exists(file))
                {
                    // 非致命：模板缺失只在真正调用该接口时报错（便于先登记、后补模板）
                    registry.LoadWarnings.Add($"接口 {itf.Name} 的模板文件缺失: {file}");
                    continue;
                }

                var extra = JsonSerializer.Deserialize<Dictionary<string, double[]>>(File.ReadAllText(file), JsonOpts)
                            ?? new Dictionary<string, double[]>();
                foreach (var kv in extra)
                    itf.LoadedTemplate[kv.Key] = kv.Value;
            }
        }
        return registry;
    }

    /// <summary>在若干候选路径中查找注册表文件</summary>
    public static string? ResolvePath(params string?[] candidates)
    {
        foreach (var c in candidates)
            if (!string.IsNullOrWhiteSpace(c) && File.Exists(c))
                return Path.GetFullPath(c!);

        // 从程序目录向上找 RvCalc/interface-registry.json
        var dir = new DirectoryInfo(AppContext.BaseDirectory);
        for (int i = 0; i < 8 && dir is not null; i++, dir = dir.Parent)
        {
            var p = Path.Combine(dir.FullName, "RvCalc", "interface-registry.json");
            if (File.Exists(p)) return p;
        }
        return null;
    }

    public RvInterfaceDesc? Find(string nameOrAlias)
    {
        if (string.IsNullOrWhiteSpace(nameOrAlias)) return null;
        var key = Normalize(nameOrAlias);
        return Interfaces.FirstOrDefault(i => Normalize(i.Name) == key)
            ?? Interfaces.FirstOrDefault(i => key.Length >= 4 && Normalize(i.Name).Contains(key))
            ?? Interfaces.FirstOrDefault(i => Normalize(i.Title) == key);
    }

    /// <summary>规范文本：去空白/下划线/连字符/括号，统一小写</summary>
    public static string Normalize(string s)
    {
        var sb = new StringBuilder(s.Length);
        foreach (var ch in s)
        {
            if (char.IsWhiteSpace(ch)) continue;
            if (ch is '_' or '-' or '(' or ')' or '（' or '）' or '[' or ']' or '：' or ':' or '，' or ',' or '。')
                continue;
            sb.Append(char.ToLowerInvariant(ch));
        }
        return sb.ToString();
    }

    /// <summary>自然语言可读的接口清单（供 Agent 系统提示/--list-interfaces 使用）</summary>
    public string Describe(string? only = null)
    {
        var sb = new StringBuilder();
        var list = only is null ? Interfaces : Interfaces.Where(i => i.Name == only || Normalize(i.Title) == Normalize(only)).ToList();
        foreach (var i in list)
        {
            sb.AppendLine($"● {i.Name} — {i.Title}");
            if (i.Summary.Length > 0) sb.AppendLine($"  说明: {i.Summary}");
            if (i.Complexity != "low") sb.AppendLine($"  复杂度: {i.Complexity}" + (i.Note is null ? "" : $"（{i.Note}）"));
            var req = i.Params.Where(p => p.Required).ToList();
            var opt = i.Params.Where(p => !p.Required).ToList();
            if (req.Count > 0)
                sb.AppendLine("  必填: " + string.Join("、", req.Select(P)));
            if (opt.Count > 0)
                sb.AppendLine("  可选: " + string.Join("、", opt.Select(P)));
            if (i.Outputs.Count > 0)
                sb.AppendLine("  输出: " + string.Join("、", i.Outputs.Select(o => o.Unit.Length > 0 ? $"{o.Name}({o.Unit})" : o.Name)));
            sb.AppendLine($"  调用参数键: {i.Name}");
            sb.AppendLine();
        }
        return sb.ToString().TrimEnd();

        static string P(RvParamDesc p)
        {
            var s = p.Unit.Length > 0 ? $"{p.Name}({p.Unit})" : p.Name;
            if (p.Type == "enum" && p.Options.Count > 0)
                s += "[" + string.Join("/", p.Options.Keys) + "]";
            if (p.Default is { } d) s += $"={d}";
            return s;
        }
    }
}
