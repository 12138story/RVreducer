using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;

namespace RvCalc;

/// <summary>单个输出字段的取值</summary>
public sealed class RvOutputValue
{
    public string Name { get; init; } = "";
    public double Value { get; init; }
    public string Unit { get; init; } = "";
    public string? Flag { get; init; }
    public string Slot { get; init; } = "";
    public string? Row { get; init; }
}

/// <summary>一次接口调用的完整结果</summary>
public sealed class RvCallResult
{
    public bool Ok { get; set; }
    public string Interface { get; set; } = "";
    public string Title { get; set; } = "";
    public int ReturnValue { get; set; }
    public Dictionary<string, double> UsedParams { get; } = new();
    public List<RvOutputValue> Outputs { get; } = new();
    public List<string> Warnings { get; } = new();
    public List<string> Errors { get; } = new();
    public Dictionary<string, double[]> Arrays { get; } = new(StringComparer.OrdinalIgnoreCase);
    public Dictionary<string, string[]> Strings { get; } = new(StringComparer.OrdinalIgnoreCase);

    public string ToText()
    {
        var sb = new StringBuilder();
        sb.AppendLine($"══ {Interface} — {Title} ══");
        if (Errors.Count > 0)
        {
            sb.AppendLine("❌ 调用被拒绝：");
            foreach (var e in Errors) sb.AppendLine("   • " + e);
            return sb.ToString().TrimEnd();
        }

        if (UsedParams.Count > 0)
        {
            sb.AppendLine("输入参数：");
            foreach (var kv in UsedParams)
                sb.AppendLine($"   {kv.Key} = {Fmt(kv.Value)}");
        }

        if (Outputs.Count > 0)
        {
            sb.AppendLine("计算结果：");
            string? lastRow = null;
            foreach (var o in Outputs)
            {
                if (o.Row != lastRow)
                {
                    if (o.Row is not null) sb.AppendLine($"  [{o.Row}]");
                    lastRow = o.Row;
                }
                var unit = o.Unit.Length > 0 ? $" {o.Unit}" : "";
                var flag = o.Flag is null ? "" : $"  ⚠ {o.Flag}";
                sb.AppendLine($"   {o.Name} = {Fmt(o.Value)}{unit}{flag}");
            }
        }

        if (Strings.Count > 0)
            foreach (var kv in Strings)
                if (kv.Value.Length > 0)
                    sb.AppendLine($"   {kv.Key}: {string.Join(", ", kv.Value)}");

        if (Warnings.Count > 0)
        {
            sb.AppendLine("⚠ 提示：");
            foreach (var w in Warnings) sb.AppendLine("   • " + w);
        }

        if (Ok) sb.AppendLine("✅ 完成");
        return sb.ToString().TrimEnd();
    }

    private static string Fmt(double v)
    {
        if (double.IsNaN(v)) return "NaN";
        if (double.IsInfinity(v)) return v > 0 ? "+∞" : "-∞";
        var a = Math.Abs(v);
        if (a != 0 && (a < 1e-4 || a >= 1e6)) return v.ToString("0.#####E+00", CultureInfo.InvariantCulture);
        return v.ToString("0.#####", CultureInfo.InvariantCulture);
    }
}

/// <summary>
/// 注册表驱动的通用计算入口：把「语义参数（自然语言键值）」映射到 DLL 数组槽位，
/// 动态调用导出函数，再把扁平输出还原成命名字段。
/// 关键防护：未知参数名/缺必填 → 直接报错，绝不静默使用默认值。
/// </summary>
public sealed class RvCalcRunner : IDisposable
{
    private const double Degree = 0.01745329252; // 与 C++ #define Degree 一致

    /// <summary>宽松 JSON 解析（允许注释/尾逗号/NaN 字面量，便于手写参数与快照文件）</summary>
    private static readonly JsonDocumentOptions JsonDocOptions = new()
    {
        AllowTrailingCommas = true,
        CommentHandling = JsonCommentHandling.Skip,
    };

    public RvInterfaceRegistry Registry { get; }
    public DynamicDllCaller Caller { get; }

    public RvCalcRunner(RvInterfaceRegistry registry, string? dllPath = null)
    {
        Registry = registry;
        Caller = new DynamicDllCaller(dllPath);
    }

    public void Dispose() => Caller.Dispose();

    /// <summary>用 JSON 参数调用（值为数字或字符串）</summary>
    /// <remarks>
    /// 支持的顶层键：
    ///   · 标量键            → 语义参数（如 "模数": 2）
    ///   · "params": {...}   → 语义参数（与标量键等价的显式写法）
    ///   · "arrays": {...}   → 数组级输入，按数组名直接给原始数组（整机快照等复杂接口用）
    ///   · "arraysFile": "x.json" → 从文件读数组级输入（文件内可直接是 arrays 对象或含 arrays 字段）
    ///   · 与参数数组同名的顶层数组（如 "div"/"in"/"ddata"）→ 自动识别为数组级输入
    /// 数组级输入先于语义参数写入，因此参数可以再覆盖数组中的单个槽位。
    /// </remarks>
    public RvCallResult RunJson(string interfaceName, string argsJson)
    {
        var args = new Dictionary<string, string>();
        var doubles = new Dictionary<string, double[]>(StringComparer.OrdinalIgnoreCase);
        var ints = new Dictionary<string, int[]>(StringComparer.OrdinalIgnoreCase);
        var desc = Registry.Find(interfaceName);

        void ReadArrays(JsonElement obj)
        {
            if (obj.ValueKind != JsonValueKind.Object || desc is null) return;
            foreach (var p in obj.EnumerateObject())
            {
                if (p.Value.ValueKind != JsonValueKind.Array) continue;
                var arg = desc.Arg(p.Name);
                if (arg is null || arg.Type == "strings") continue;
                var values = p.Value.EnumerateArray()
                    .Select(v => v.ValueKind == JsonValueKind.Number ? v.GetDouble() : double.NaN)
                    .ToArray();
                if (arg.Type == "int") ints[arg.Name] = values.Select(v => (int)Math.Round(v)).ToArray();
                else doubles[arg.Name] = values;
            }
        }

        if (!string.IsNullOrWhiteSpace(argsJson))
        {
            using var doc = JsonDocument.Parse(argsJson, JsonDocOptions);
            if (doc.RootElement.ValueKind != JsonValueKind.Object)
                return Fail(interfaceName, "参数必须是 JSON 对象");
            var root = doc.RootElement;

            // ① 数组文件（整机快照）
            if (root.TryGetProperty("arraysFile", out var af) && af.ValueKind == JsonValueKind.String)
            {
                var file = af.GetString() ?? "";
                if (!File.Exists(file)) return Fail(interfaceName, $"arraysFile 不存在: {file}");
                using var fdoc = JsonDocument.Parse(File.ReadAllText(file), JsonDocOptions);
                var froot = fdoc.RootElement;
                ReadArrays(froot.ValueKind == JsonValueKind.Object
                           && froot.TryGetProperty("arrays", out var inner) && inner.ValueKind == JsonValueKind.Object
                    ? inner
                    : froot);
            }

            // ② 内联数组块
            if (root.TryGetProperty("arrays", out var arrs) && arrs.ValueKind == JsonValueKind.Object)
                ReadArrays(arrs);

            // ③ 顶层：数组 → 数组级输入；标量 → 语义参数
            foreach (var p in root.EnumerateObject())
            {
                if (p.Name is "arrays" or "arraysFile" or "params" or "interface") continue;
                if (p.Value.ValueKind == JsonValueKind.Array)
                {
                    if (desc?.Arg(p.Name) is not null) ReadArrays(root);
                    continue;
                }
                if (p.Value.ValueKind is JsonValueKind.Object or JsonValueKind.Null) continue;
                args[p.Name] = p.Value.ValueKind == JsonValueKind.String
                    ? p.Value.GetString() ?? ""
                    : p.Value.ToString();
            }

            // ④ 显式 params 块
            if (root.TryGetProperty("params", out var ps) && ps.ValueKind == JsonValueKind.Object)
                foreach (var p in ps.EnumerateObject())
                    args[p.Name] = p.Value.ValueKind == JsonValueKind.String
                        ? p.Value.GetString() ?? ""
                        : p.Value.ToString();
        }

        return Run(interfaceName, args,
            doubles.Count > 0 ? doubles : null,
            ints.Count > 0 ? ints : null);
    }

    /// <summary>
    /// 在同一进程内按顺序执行多个接口。
    /// 用于有状态依赖的接口：例如 RecommondShaftPara / GetForceAndForceLocation 需要
    /// 先在同一进程内执行过 CalCrankShaftSystem2（DLL 内部 gRVReducer 保存整机状态）。
    /// </summary>
    public IReadOnlyList<RvCallResult> RunSequence(IEnumerable<(string Interface, string ArgsJson)> steps)
    {
        var results = new List<RvCallResult>();
        foreach (var (iface, argsJson) in steps)
            results.Add(RunJson(iface, argsJson));
        return results;
    }

    /// <summary>用语义键值对调用接口</summary>
    public RvCallResult Run(string interfaceName, IReadOnlyDictionary<string, string> userArgs)
        => Run(interfaceName, userArgs, null, null);

    /// <summary>用语义键值对 + 可选数组级输入调用接口</summary>
    public RvCallResult Run(string interfaceName,
                            IReadOnlyDictionary<string, string> userArgs,
                            IReadOnlyDictionary<string, double[]>? arrayOverrides,
                            IReadOnlyDictionary<string, int[]>? intOverrides)
    {
        var desc = Registry.Find(interfaceName);
        if (desc is null)
        {
            var names = string.Join(", ", Registry.Interfaces.Select(i => i.Name));
            return Fail(interfaceName, $"未知接口。可用接口：{names}");
        }
        bool hasExternalArrays = (arrayOverrides is { Count: > 0 }) || (intOverrides is { Count: > 0 });
        if (!string.IsNullOrEmpty(desc.TemplateFile) && !Registry.HasTemplate(desc) && !hasExternalArrays)
            return Fail(desc.Name, $"该接口需要模板文件 {desc.TemplateFile}（当前缺失）。请先运行 --dump-templates 生成模板，或用 arrays/arraysFile 提供完整输入数组。");

        // ── 1. 参数名匹配（未匹配的键直接报错，避免静默用默认值）──
        var matched = new Dictionary<RvParamDesc, string>();
        var unknown = new List<string>();
        foreach (var kv in userArgs)
        {
            if (kv.Key.StartsWith('_') || kv.Key.StartsWith("//")) continue;
            var p = desc.Params.FirstOrDefault(p => Matches(p, kv.Key));
            if (p is null) unknown.Add(kv.Key);
            else matched[p] = kv.Value;
        }
        if (unknown.Count > 0)
        {
            var valid = string.Join("、", desc.Params.Select(p => p.Name));
            return Fail(desc.Name, $"无法识别的参数名：{string.Join("、", unknown)}。本接口可用参数：{valid}");
        }

        // ── 2. 组装数组（模板打底 → 写参数）──
        var buffers = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
        foreach (var arg in desc.Args)
        {
            if (arg.Type == "strings")
            {
                buffers[arg.Name] = Array.Empty<double>();
                continue;
            }
            var len = Math.Max(arg.Length, 1);
            if (arg.Type == "int")
            {
                // ⚠ 分割索引等 int 数组同样必须套用模板默认值：全 0 会让 DLL 走错分支
                // （例如 div[2]=每齿轮几何列数、div[29]=输出列数，为 0 时输出错乱甚至崩溃）
                var ia = new int[len];
                if (desc.LoadedTemplate.TryGetValue(arg.Name, out var tpl))
                    for (int k = 0; k < Math.Min(tpl.Length, len); k++)
                        ia[k] = (int)Math.Round(tpl[k]);
                buffers[arg.Name] = ia;
            }
            else
            {
                var arr = new double[len];
                if (desc.LoadedTemplate.TryGetValue(arg.Name, out var tpl))
                    Array.Copy(tpl, arr, Math.Min(tpl.Length, len));
                buffers[arg.Name] = arr;
            }
        }

        var result = new RvCallResult { Interface = desc.Name, Title = desc.Title };
        if (!string.IsNullOrEmpty(desc.TemplateFile) && !Registry.HasTemplate(desc) && hasExternalArrays)
            result.Warnings.Add($"模板 {desc.TemplateFile} 缺失，本次完全依赖外部数组输入");

        // ── 2b. 数组级输入（整机快照 / 原始数组）：先于语义参数写入，参数可再覆盖单个槽位 ──
        if (arrayOverrides is not null)
            foreach (var kv in arrayOverrides)
                ApplyDoubleArray(desc, buffers, kv.Key, kv.Value, result.Warnings);
        if (intOverrides is not null)
            foreach (var kv in intOverrides)
                ApplyIntArray(desc, buffers, kv.Key, kv.Value, result.Warnings);

        // ── 3. 必填校验 + 写槽位 ──
        foreach (var p in desc.Params)
        {
            bool provided = matched.TryGetValue(p, out var rawValue);
            double value;
            if (provided)
            {
                if (!TryParseParam(p, rawValue!, out value, out var err))
                    return Fail(desc.Name, err!);
            }
            else if (p.Required && p.Default is null && !TplHasNonZero(p, buffers))
            {
                return Fail(desc.Name, $"缺少必填参数：{p.Name}" + (p.Unit.Length > 0 ? $"（{p.Unit}）" : ""));
            }
            else
            {
                if (p.Default is { } d && !TplHasNonZero(p, buffers)) value = d;
                else continue; // 模板/默认值已就位
            }

            if (p.Convert == "deg2rad") value *= Degree;
            else if (p.Convert == "nm2nmm") value *= 1000.0;
            foreach (var slot in p.AllSlots)
            {
                var (arrayName, index) = ParseSlot(slot);
                if (!buffers.TryGetValue(arrayName, out var buf))
                    return Fail(desc.Name, $"参数 {p.Name} 的槽位 {slot} 指向未知数组");
                if (buf is int[] ia) { if (index < ia.Length) ia[index] = (int)Math.Round(value); }
                else if (buf is double[] da) { if (index < da.Length) da[index] = value; }
            }
            result.UsedParams[p.Name + (p.Unit.Length > 0 ? $"({p.Unit})" : "")] = value;
        }

        // ── 4. 调用 DLL ──
        RvRawCall raw;
        try
        {
            raw = Caller.Invoke(desc, buffers);
        }
        catch (Exception ex)
        {
            return Fail(desc.Name, $"DLL 调用失败：{ex.GetType().Name}: {ex.Message}");
        }
        result.ReturnValue = raw.ReturnValue;

        foreach (var kv in raw.Doubles) result.Arrays[kv.Key] = kv.Value;
        foreach (var kv in raw.Ints) result.Arrays[kv.Key] = kv.Value.Select(v => (double)v).ToArray();
        foreach (var kv in raw.Strings) result.Strings[kv.Key] = kv.Value;

        // ── 5. 还原输出字段 ──
        foreach (var o in desc.Outputs)
        {
            var (arrayName, index) = ParseSlot(o.Slot);
            double v;
            if (raw.Doubles.TryGetValue(arrayName, out var darr) && index < darr.Length) v = darr[index];
            else if (raw.Ints.TryGetValue(arrayName, out var iarr) && index < iarr.Length) v = iarr[index];
            else continue;
            var row = desc.OutRowWidth is { } w && w > 0 && desc.RowLabels.Count > 0
                ? (index / w < desc.RowLabels.Count ? desc.RowLabels[index / w] : null)
                : null;
            result.Outputs.Add(new RvOutputValue
            {
                Name = o.Name,
                Value = v,
                Unit = o.Unit,
                Slot = o.Slot,
                Row = row,
                Flag = FlagOf(o, v),
            });
        }

        if (desc.Returns == "int" && raw.ReturnValue < 0)
            result.Errors.Add($"DLL 返回码 {raw.ReturnValue}（表示计算失败）");

        var validOutputs = result.Outputs.Where(o => o.Flag is null).ToList();
        if (result.Outputs.Count > 0 && validOutputs.Count == 0)
        {
            var hint = desc.Note is { Length: > 0 } ? $"\n   接口说明：{desc.Note}" : "";
            result.Errors.Add("全部输出字段均为无效值（NaN/∞/失败标记），请检查输入是否物理合理" + hint);
        }
        else if (desc.Outputs.Count == 0)
            result.Warnings.Add("该接口未在注册表中声明输出字段，请查看原始数组");

        result.Ok = result.Errors.Count == 0;
        return result;
    }

    // ═══════════════ 辅助 ═══════════════

    /// <summary>把数组级输入（double 形式）写入缓冲区；必要时按实际长度扩容（整机快照长度由运行数据决定）</summary>
    private static void ApplyDoubleArray(RvInterfaceDesc desc, Dictionary<string, object> buffers,
                                         string name, double[] values, List<string> warnings)
    {
        var arg = desc.Arg(name);
        if (arg is null)
        {
            warnings.Add($"忽略未知数组 “{name}”（该接口无此参数数组，可用：{string.Join("/", desc.Args.Select(a => a.Name))}）");
            return;
        }
        if (arg.Type == "strings") { warnings.Add($"数组 “{name}” 为字符串输出，已忽略输入"); return; }
        if (!buffers.TryGetValue(name, out var buf)) return;
        if (arg.Length > 0 && values.Length < arg.Length)
            warnings.Add($"数组 “{name}” 长度 {values.Length} 小于注册表声明 {arg.Length}，不足部分按 0 处理");

        if (buf is int[] ia)
        {
            if (values.Length > ia.Length)
            {
                var grown = new int[values.Length];
                Array.Copy(ia, grown, ia.Length);
                buffers[name] = grown;
                ia = grown;
            }
            for (int i = 0; i < values.Length; i++) ia[i] = (int)Math.Round(values[i]);
        }
        else if (buf is double[] da)
        {
            if (values.Length > da.Length)
            {
                var grown = new double[values.Length];
                Array.Copy(da, grown, da.Length);
                buffers[name] = grown;
                da = grown;
            }
            Array.Copy(values, da, values.Length);
        }
    }

    /// <summary>把数组级输入（int 形式）写入缓冲区</summary>
    private static void ApplyIntArray(RvInterfaceDesc desc, Dictionary<string, object> buffers,
                                      string name, int[] values, List<string> warnings)
    {
        var arg = desc.Arg(name);
        if (arg is null)
        {
            warnings.Add($"忽略未知数组 “{name}”（该接口无此参数数组）");
            return;
        }
        if (!buffers.TryGetValue(name, out var buf) || buf is not int[] ia) return;
        if (arg.Length > 0 && values.Length < arg.Length)
            warnings.Add($"数组 “{name}” 长度 {values.Length} 小于注册表声明 {arg.Length}，不足部分按 0 处理");
        if (values.Length > ia.Length)
        {
            var grown = new int[values.Length];
            Array.Copy(ia, grown, ia.Length);
            buffers[name] = grown;
            ia = grown;
        }
        Array.Copy(values, ia, values.Length);
    }

    private static bool Matches(RvParamDesc p, string key)
    {
        var k = RvInterfaceRegistry.Normalize(key);
        if (k.Length == 0) return false;
        if (RvInterfaceRegistry.Normalize(p.Name) == k) return true;
        if (p.Alias.Any(a => RvInterfaceRegistry.Normalize(a) == k)) return true;
        if (p.Options.Keys.Any(o => RvInterfaceRegistry.Normalize(o) == k)) return true;
        // 允许"圆锥滚子轴承"这类带后缀的说法
        return p.Options.Keys.Any(o => k.Contains(RvInterfaceRegistry.Normalize(o)));
    }

    private static readonly Dictionary<string, (string Dim, double Factor)> Units = new(StringComparer.OrdinalIgnoreCase)
    {
        ["n"] = ("force", 1), ["kn"] = ("force", 1000), ["kgf"] = ("force", 9.80665),
        ["mm"] = ("len", 1), ["cm"] = ("len", 10), ["m"] = ("len", 1000), ["um"] = ("len", 0.001),
        ["n.m"] = ("torque", 1), ["nm"] = ("torque", 1), ["nmm"] = ("torque", 0.001),
        ["mpa"] = ("stress", 1), ["gpa"] = ("stress", 1000), ["kpa"] = ("stress", 0.001),
        ["r/min"] = ("speed", 1), ["rpm"] = ("speed", 1),
        ["h"] = ("time", 1), ["hour"] = ("time", 1),
        ["deg"] = ("angle", 1),
        ["kg/mm3"] = ("density", 1), ["kg/m3"] = ("density", 1e-9),
        ["hrc"] = ("hardness", 1), ["hv"] = ("hardness", 1),
    };

    /// <summary>规范化单位文本：去空白、全角/上标符号统一、小写</summary>
    private static string NormUnit(string u)
    {
        var sb = new StringBuilder();
        foreach (var ch in u)
        {
            if (char.IsWhiteSpace(ch)) continue;
            sb.Append(ch switch
            {
                '³' => '3', '²' => '2', 'μ' or 'µ' => 'u', '°' => 'd', '·' or '⋅' or '×' => '.',
                _ => char.ToLowerInvariant(ch),
            });
        }
        var s = sb.ToString();
        if (s is "d" or "deg" or "度") return "deg";
        return s.Replace("n.mm", "nmm").Replace("n.m", "nm");
    }

    /// <summary>尝试拆分“数值 + 单位后缀”，并换算到参数声明的单位</summary>
    private static bool TryParseWithUnit(string raw, string? paramUnit, out double value, out string? error)
    {
        value = 0; error = null;
        var m = System.Text.RegularExpressions.Regex.Match(raw,
            @"^\s*(?<num>[-+]?[0-9]*\.?[0-9]+(?:[eE][-+]?[0-9]+)?)\s*(?<unit>.*)$");
        if (!m.Success) { error = $"需要数值，收到 “{raw}”"; return false; }
        if (!double.TryParse(m.Groups["num"].Value, NumberStyles.Float, CultureInfo.InvariantCulture, out var num))
        { error = $"数值解析失败：“{raw}”"; return false; }

        var givenUnit = NormUnit(m.Groups["unit"].Value);
        if (givenUnit.Length == 0) { value = num; return true; }

        if (!Units.TryGetValue(givenUnit, out var given))
        { error = $"无法识别的单位 “{m.Groups["unit"].Value.Trim()}”"; return false; }

        var pUnit = NormUnit(paramUnit ?? "");
        if (pUnit.Length == 0 || !Units.TryGetValue(pUnit, out var target))
        {
            // 参数未声明单位（如数量/系数）：仅接受无量纲输入
            if (given.Dim is "len" or "force" or "torque" or "stress" or "speed" or "time")
            { error = $"参数未声明单位，请去掉后缀 “{m.Groups["unit"].Value.Trim()}”"; return false; }
            value = num;
            return true;
        }
        if (target.Dim != given.Dim)
        { error = $"单位不匹配：期望 {paramUnit}，收到 {m.Groups["unit"].Value.Trim()}"; return false; }

        value = num * given.Factor / target.Factor;
        return true;
    }

    private static bool TryParseParam(RvParamDesc p, string raw, out double value, out string? error)
    {
        error = null;
        raw = raw.Trim();
        if (p.Type == "enum" && p.Options.Count > 0)
        {
            var exact = p.Options.FirstOrDefault(o => RvInterfaceRegistry.Normalize(o.Key) == RvInterfaceRegistry.Normalize(raw));
            if (exact.Key is not null) { value = exact.Value; return true; }
            // 双向包含匹配：既允许“圆锥滚子轴承”匹配“圆锥滚子”，也允许反向的简写
            var nr = RvInterfaceRegistry.Normalize(raw);
            var partial = p.Options.Where(o =>
            {
                var no = RvInterfaceRegistry.Normalize(o.Key);
                return nr.Length > 0 && no.Length > 0 && (nr.Contains(no) || no.Contains(nr));
            }).ToList();
            if (partial.Count == 1) { value = partial[0].Value; return true; }
            if (TryParseWithUnit(raw, null, out value, out _)) return true;
            error = $"参数 {p.Name} 取值 “{raw}” 无法识别，可选：{string.Join(" / ", p.Options.Keys)}";
            return false;
        }
        return TryParseWithUnit(raw, p.Unit, out value, out error);
    }

    private static bool TplHasNonZero(RvParamDesc p, Dictionary<string, object> buffers)
    {
        foreach (var slot in p.AllSlots)
        {
            var (arrayName, index) = ParseSlot(slot);
            if (!buffers.TryGetValue(arrayName, out var buf)) continue;
            if (buf is double[] da && index < da.Length && da[index] != 0) return true;
            if (buf is int[] ia && index < ia.Length && ia[index] != 0) return true;
        }
        return false;
    }

    internal static (string ArrayName, int Index) ParseSlot(string slot)
    {
        var s = slot.Trim();
        int open = s.IndexOf('[');
        int close = s.LastIndexOf(']');
        if (open < 0 || close < open)
            throw new FormatException($"槽位格式非法（应为 name[index]）：{slot}");
        var name = s[..open].Trim();
        if (!int.TryParse(s[(open + 1)..close], out var index))
            throw new FormatException($"槽位下标非法：{slot}");
        return (name, index);
    }

    private static string? FlagOf(RvOutputDesc o, double v)
    {
        if (double.IsNaN(v)) return "NaN（计算未收敛或输入越界）";
        if (double.IsInfinity(v)) return "∞（参数导致发散，如螺旋角为 0 的导程）";
        // 非规格化极小值 = 该输出槽位根本没被 DLL 写入（常见于类型码/输入不匹配导致提前 return）
        if (v != 0 && Math.Abs(v) < 1e-300) return "非规格化极小值（该槽位未被 DLL 写入，通常是输入不匹配）";
        if (o.InvalidBelow is { } lo && v < lo) return $"低于有效下界 {lo}";
        if (o.InvalidAbove is { } hi && v > hi) return $"超过有效上界 {hi}（常见 1e9 失败标记）";
        if (Math.Abs(v) >= 1e9) return "≥1e9（DLL 失败标记）";
        return null;
    }

    private RvCallResult Fail(string iface, string error)
    {
        var r = new RvCallResult { Interface = iface, Ok = false };
        var d = Registry.Find(iface);
        if (d is not null) r.Title = d.Title;
        r.Errors.Add(error);
        return r;
    }
}
