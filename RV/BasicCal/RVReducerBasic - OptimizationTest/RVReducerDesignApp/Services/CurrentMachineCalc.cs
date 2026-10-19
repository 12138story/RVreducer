using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Windows;

namespace RVReducerDesignApp.Services;

/// <summary>曲轴单个节点的受力/应力</summary>
public sealed class CrankShaftNodeStress
{
    public int Index { get; init; }
    /// <summary>节点轴向坐标 (mm)</summary>
    public double X { get; init; }
    /// <summary>弯曲正应力 (MPa)</summary>
    public double Bending { get; init; }
    /// <summary>剪应力 (MPa)</summary>
    public double Shear { get; init; }
}

/// <summary>曲轴系计算结果（当前工程）</summary>
public sealed class CrankShaftCalcResult
{
    public bool Ok { get; set; }
    public string Message { get; set; } = "";
    public double ShaftDiameter { get; set; }
    public double ShaftLength { get; set; }
    public double MaxBending { get; set; }
    public double MaxShear { get; set; }
    public double AllowBending { get; set; }
    public double AllowShear { get; set; }
    public List<CrankShaftNodeStress> Nodes { get; } = new();

    /// <summary>弯曲安全系数 = 许用弯曲应力 / 最大弯曲应力</summary>
    public double? SafetyBending => AllowBending > 0 && MaxBending > 0 ? AllowBending / MaxBending : null;
    /// <summary>剪切安全系数 = 许用剪应力 / 最大剪应力</summary>
    public double? SafetyShear => AllowShear > 0 && MaxShear > 0 ? AllowShear / MaxShear : null;

    public string ToText()
    {
        if (!Ok) return "❌ 曲轴系计算未完成：" + Message;

        var sb = new StringBuilder();
        sb.AppendLine("══ 曲轴系计算结果（当前工程）══");
        if (ShaftDiameter > 0 || ShaftLength > 0)
            sb.AppendLine($"轴径 = {F(ShaftDiameter)} mm，轴长 = {F(ShaftLength)} mm");
        sb.AppendLine($"最大弯曲正应力 = {F(MaxBending)} MPa" + (AllowBending > 0 ? $"（许用 {F(AllowBending)} MPa → 安全系数 {F(SafetyBending)}）" : "（未设置许用弯曲应力，无法给安全系数）"));
        sb.AppendLine($"最大剪应力     = {F(MaxShear)} MPa" + (AllowShear > 0 ? $"（许用 {F(AllowShear)} MPa → 安全系数 {F(SafetyShear)}）" : "（未设置许用剪应力，无法给安全系数）"));
        if (SafetyBending is { } sbv) sb.AppendLine(sbv >= 1 ? "✅ 弯曲强度满足要求" : "⚠ 弯曲强度不满足要求（安全系数 < 1）");
        if (SafetyShear is { } ssv) sb.AppendLine(ssv >= 1 ? "✅ 剪切强度满足要求" : "⚠ 剪切强度不满足要求（安全系数 < 1）");

        if (Nodes.Count > 0)
        {
            sb.AppendLine($"各节点受力/应力（共 {Nodes.Count} 个节点）：");
            sb.AppendLine("   序号 | 轴向坐标(mm) | 弯曲正应力(MPa) | 剪应力(MPa)");
            foreach (var n in Nodes)
                sb.AppendLine($"   {n.Index,4} | {F(n.X),12} | {F(n.Bending),15} | {F(n.Shear),10}");
        }
        else
        {
            sb.AppendLine("⚠ 未取到节点应力数据（GetForceAndForceLocation 返回 0 个节点）");
        }
        return sb.ToString().TrimEnd();
    }

    public string ToJson() => JsonSerializer.Serialize(new
    {
        ok = Ok,
        message = Message,
        shaftDiameter = ShaftDiameter,
        shaftLength = ShaftLength,
        maxBending = MaxBending,
        maxShear = MaxShear,
        allowBending = AllowBending,
        allowShear = AllowShear,
        safetyBending = SafetyBending,
        safetyShear = SafetyShear,
        nodes = Nodes.Select(n => new { n.Index, n.X, n.Bending, n.Shear }),
        timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
    }, new JsonSerializerOptions { WriteIndented = true });

    private static string F(double v) => v.ToString("0.####", CultureInfo.InvariantCulture);
    private static string F(double? v) => v is { } d ? d.ToString("0.####", CultureInfo.InvariantCulture) : "-";
}

/// <summary>部件计算结果（当前工程）</summary>
public sealed class PartCalcResult
{
    public bool Ok { get; set; }
    public string Part { get; set; } = "";
    public string Message { get; set; } = "";
    public List<string> Inputs { get; } = new();
    public List<string> Outputs { get; } = new();

    public string ToText()
    {
        if (!Ok) return $"❌ {Part}计算未完成：{Message}";
        var sb = new StringBuilder();
        sb.AppendLine($"══ {Part}计算结果（当前工程）══");
        if (Inputs.Count > 0)
        {
            sb.AppendLine("输入（当前界面值）：");
            foreach (var s in Inputs) sb.AppendLine("   " + s);
        }
        sb.AppendLine("结果：");
        foreach (var s in Outputs) sb.AppendLine("   " + s);

        var suspicious = Outputs.Where(IsSuspicious).ToList();
        if (suspicious.Count > 0)
        {
            sb.AppendLine();
            sb.AppendLine($"⚠ 其中 {suspicious.Count} 项的数值不是正常结果（≥1e9 / NaN / ∞），通常表示当前工程的参数还没填完整，" +
                          "或该部件尚未真正参与计算。请先在界面补全参数，不要把上面的这些数字当成结论。");
        }
        return sb.ToString().TrimEnd();
    }

    /// <summary>判断结果行里的数值是否像「未计算 / 无效」（NaN、∞、≥1e9）。</summary>
    private static bool IsSuspicious(string line)
    {
        if (line.Contains("NaN") || line.Contains("∞")) return true;
        foreach (System.Text.RegularExpressions.Match m in
                 System.Text.RegularExpressions.Regex.Matches(line, @"-?\d+(?:\.\d+)?(?:[eE][+-]?\d+)?"))
        {
            if (double.TryParse(m.Value, NumberStyles.Float, CultureInfo.InvariantCulture, out var v) &&
                (double.IsNaN(v) || double.IsInfinity(v) || Math.Abs(v) >= 1e9))
                return true;
        }
        return false;
    }
}

/// <summary>
/// 对「当前工程设计状态」直接执行曲轴系计算（供应用内 Agent 使用，无需导出快照）：
///   ① 用当前界面的曲轴数据构建轴段输入
///   ② 刷新整机分割索引与各部件 DLL 输入
///   ③ 调用 CalCrankShaftSystem2（同一进程内，满足 DLL 的全局状态依赖）
///   ④ 调 GetForceAndForceLocation 取回各节点弯曲/剪应力
///   ⑤ 结合「许用弯曲正应力 / 许用剪应力」给出安全系数
/// 注意：字典项是 WPF 依赖属性，必须在 UI 线程访问，本类会自动 marshal。
/// </summary>
public static class CurrentMachineCalc
{
    /// <summary>计算当前工程的曲轴系（自动切到 UI 线程）</summary>
    /// <param name="allowBendingOverride">许用弯曲正应力 (MPa)；为 null 或 ≤0 时用界面输入值</param>
    /// <param name="allowShearOverride">许用剪应力 (MPa)；为 null 或 ≤0 时用界面输入值</param>
    public static CrankShaftCalcResult CalcCrankShaftSystem(double? allowBendingOverride = null, double? allowShearOverride = null)
    {
        var dispatcher = Application.Current?.Dispatcher;
        if (dispatcher is not null && !dispatcher.CheckAccess())
            return dispatcher.Invoke(() => CalcCore(allowBendingOverride, allowShearOverride));
        return CalcCore(allowBendingOverride, allowShearOverride);
    }

    private static CrankShaftCalcResult CalcCore(double? allowBendingOverride, double? allowShearOverride)
    {
        var set = WholeMachineSnapshot.CurrentSet;
        if (set is null)
            return new CrankShaftCalcResult { Ok = false, Message = "尚未创建设计工程（未找到减速器数据对象）" };

        var sys = set.mCrankShaftSystem;
        var crank = sys.mCrankShaft;
        var shaft = crank.mShaftDesignPage.currentShaft;
        if (shaft is null)
            return new CrankShaftCalcResult
            {
                Ok = false,
                Message = "尚未设计曲轴：请先在「曲轴系 → 曲轴」中设计轴并装配，或先执行「轴推荐」。",
            };

        try
        {
            // ① 曲轴轴段输入（与界面「曲轴系计算」同一链路）
            crank.UpdateClassInputData();
            // ② 整机分割索引 + 各部件输入
            set.UpdataRVSetDataDivisionIndex();
            set.UpdateAllPartsDllInputData();
            // ③ 曲轴系计算
            CTANCrankShaftSystem.CalCrankShaftSystem2(
                ref set.mDivisionIndexList[0, 0],
                ref set.mPartsDivisionIndexList[0],
                ref set.mInputDoubleDataStorage[0],
                ref set.mOutputDoubleDataStorage[0]);
            set.UpdateAllPartsDllOutputData();

            // ④ 节点应力：X 每节点重复两次，Y 前 n 为弯曲、后 n 为剪切
            int count = 0;
            var xs = new double[400];
            var ys = new double[400];
            CTANCrankShaftSystem.GetForceAndForceLocation(ref count, ref xs[0], ref ys[0]);

            var result = new CrankShaftCalcResult { Ok = true, Message = "计算完成" };
            for (int i = 0; i < count && i < 200; i++)
            {
                result.Nodes.Add(new CrankShaftNodeStress
                {
                    Index = i,
                    X = xs[Math.Min(i * 2, xs.Length - 1)],
                    Bending = ys[i],
                    Shear = ys[i + count],
                });
            }
            result.MaxBending = result.Nodes.Count > 0 ? result.Nodes.Max(n => Math.Abs(n.Bending)) : 0;
            result.MaxShear = result.Nodes.Count > 0 ? result.Nodes.Max(n => Math.Abs(n.Shear)) : 0;

            // ⑤ 许用应力与轴径/轴长（来自界面数据字典；允许调用方用参数覆盖许用值）
            result.AllowBending = allowBendingOverride is > 0 ? allowBendingOverride.Value : GetDict(sys, "许用弯曲正应力");
            result.AllowShear = allowShearOverride is > 0 ? allowShearOverride.Value : GetDict(sys, "许用剪应力");
            result.ShaftDiameter = GetDict(sys, "轴直径");
            result.ShaftLength = GetDict(sys, "轴长度");

            // 字典里没有时按轴段几何兜底
            var sections = shaft.OriginalAxisSections;
            if (sections is { Count: > 0 })
            {
                if (result.ShaftDiameter <= 0)
                    result.ShaftDiameter = sections.Max(s => s.BaseOuterDiameter);
                if (result.ShaftLength <= 0)
                    result.ShaftLength = sections.Max(s => s.BaseOffsetX + s.CurrentLength);
            }
            return result;
        }
        catch (Exception ex)
        {
            return new CrankShaftCalcResult { Ok = false, Message = ex.GetType().Name + ": " + ex.Message };
        }
    }

    /// <summary>
    /// 计算当前工程的某个部件（等价于在界面上选中该部件后点「计算」）：
    ///   摆线针轮副 / 渐开线齿轮副 / 总体（减速器总体力矩与转速）/ 曲轴系
    /// 自动切到 UI 线程（数据字典是 WPF 依赖属性）。
    /// </summary>
    public static PartCalcResult CalcPart(string part)
    {
        var dispatcher = Application.Current?.Dispatcher;
        if (dispatcher is not null && !dispatcher.CheckAccess())
            return dispatcher.Invoke(() => CalcPartCore(part));
        return CalcPartCore(part);
    }

    private static PartCalcResult CalcPartCore(string part)
    {
        var key = (part ?? "").Trim();
        var set = WholeMachineSnapshot.CurrentSet;
        if (set is null)
            return new PartCalcResult { Ok = false, Part = key, Message = "尚未创建设计工程（未找到减速器数据对象）" };
        if (key.Length == 0)
            return new PartCalcResult { Ok = false, Part = "(未指定)", Message = "请指定部件：摆线针轮副 / 渐开线齿轮副 / 总体 / 曲轴系" };

        try
        {
            if (key.Contains("摆线")) return CalcCycloid(set);
            if (key.Contains("渐开线") || key.Contains("齿轮副") || key.Contains("齿轮")) return CalcInvGear(set);
            if (key.Contains("曲轴") || key.Contains("轴系")) return CalcCrankShaftAsPart();
            if (key.Contains("总体") || key.Contains("减速器")) return CalcTotal(set);
            return new PartCalcResult { Ok = false, Part = key, Message = "无法识别的部件；可用：摆线针轮副 / 渐开线齿轮副 / 总体 / 曲轴系" };
        }
        catch (Exception ex)
        {
            return new PartCalcResult { Ok = false, Part = key, Message = ex.GetType().Name + ": " + ex.Message };
        }
    }

    // ── 摆线针轮副 ──
    private static PartCalcResult CalcCycloid(CTANRVSet set)
    {
        var pair = set.mCycloidGearPair;
        pair.CalPageData();

        var r = new PartCalcResult { Ok = true, Part = "摆线针轮副" };
        var dict = pair.mDataStorageDict;
        foreach (var name in new[] { "齿数", "滚针半径", "分度圆半径", "偏心距", "设计齿宽", "精度等级", "摆线轮传递总力矩", "弹性模量", "使用系数" })
        {
            if (dict.TryGetValue(name, out var item) && item is not null)
                r.Inputs.Add($"{name} = {Fmt(item.Value)}");
        }

        // 输出名来自 CTANCycloidGearPair.mOutPutHeadName（名称/单位成对）
        var names = CTANCycloidGearPair.mOutPutHeadName;
        for (int i = 0; i * 2 < names.Length; i++)
        {
            var name = names[i * 2];
            var unit = i * 2 + 1 < names.Length ? names[i * 2 + 1] : "";
            if (string.IsNullOrWhiteSpace(name) || !dict.TryGetValue(name, out var item) || item is null) continue;
            r.Outputs.Add(Format(name, unit, item.OutValue));
        }
        return r;
    }

    // ── 渐开线齿轮副（太阳轮 / 行星轮）──
    private static PartCalcResult CalcInvGear(CTANRVSet set)
    {
        var pair = set.mInvGearPair;
        pair.CalPageData();

        var r = new PartCalcResult { Ok = true, Part = "渐开线齿轮副" };
        var inputNames = new[] { "法向模数", "压力角", "螺旋角", "中心距", "齿数", "设计齿宽", "行星轮个数" };
        foreach (var name in inputNames)
        {
            var vals = new List<string>();
            foreach (var g in pair.mGears)
                if (g.mDataStorageDict.TryGetValue(name, out var it) && it is not null)
                    vals.Add(Fmt(it.Value));
            if (vals.Count > 0) r.Inputs.Add($"{name} = {string.Join(" / ", vals)}");
        }

        // 只抽取关键字段（应力/几何/重合度/跨棒距等），避免输出 142×2 行刷屏
        var keep = new[] { "应力", "安全系数", "传动比", "齿数比", "模数", "中心距离", "直径", "全齿高", "重合度", "跨棒距", "变位", "精度" };
        var names = CTANInvGear.mOutPutHeadName;
        var labels = new[] { "太阳轮", "行星轮" };
        for (int gi = 0; gi < pair.mGears.Length; gi++)
        {
            var dict = pair.mGears[gi].mDataStorageDict;
            r.Outputs.Add($"[{labels[Math.Min(gi, labels.Length - 1)]}]");
            for (int i = 0; i * 2 < names.Length; i++)
            {
                var name = names[i * 2];
                if (string.IsNullOrWhiteSpace(name) || !keep.Any(k => name.Contains(k))) continue;
                var unit = i * 2 + 1 < names.Length ? names[i * 2 + 1] : "";
                if (!dict.TryGetValue(name, out var item) || item is null) continue;
                r.Outputs.Add(Format(name, unit, item.OutValue));
            }
        }
        return r;
    }

    // ── 总体（减速器力矩/转速分配）──
    private static PartCalcResult CalcTotal(CTANRVSet set)
    {
        set.CalPageData();   // CTANRVSet.CalPageData → CalTotalTorqueAndRPM
        var r = new PartCalcResult { Ok = true, Part = "减速器总体" };
        var dict = set.mDataStorageDict;
        foreach (var name in new[] { "减速器输出部件", "减速器输出力矩", "减速器输出转速", "太阳轮齿数", "行星轮齿数", "摆线轮齿数", "针轮齿数" })
            if (dict.TryGetValue(name, out var it) && it is not null) r.Inputs.Add($"{name} = {Fmt(it.Value)}");
        foreach (var name in new[] { "太阳轮力矩", "太阳轮转速", "摆线轮力矩", "摆线轮转速" })
            if (dict.TryGetValue(name, out var it) && it is not null) r.Outputs.Add($"{name} = {Fmt(it.OutValue)}");
        return r;
    }

    // ── 曲轴系（复用已有实现）──
    private static PartCalcResult CalcCrankShaftAsPart()
    {
        var c = CalcCore(null, null);
        var r = new PartCalcResult { Ok = c.Ok, Part = "曲轴系", Message = c.Message };
        if (!c.Ok) return r;
        r.Inputs.Add($"轴径 = {Fmt(c.ShaftDiameter)} mm，轴长 = {Fmt(c.ShaftLength)} mm");
        if (c.AllowBending > 0) r.Inputs.Add($"许用弯曲应力 = {Fmt(c.AllowBending)} MPa，许用剪应力 = {Fmt(c.AllowShear)} MPa");
        r.Outputs.Add($"最大弯曲正应力 = {Fmt(c.MaxBending)} MPa" + (c.SafetyBending is { } sb ? $"，安全系数 = {Fmt(sb)}" : ""));
        r.Outputs.Add($"最大剪应力 = {Fmt(c.MaxShear)} MPa" + (c.SafetyShear is { } ss ? $"，安全系数 = {Fmt(ss)}" : ""));
        r.Outputs.Add($"节点数 = {c.Nodes.Count}" + (c.Nodes.Count > 0 ? "（明细见 calc_crank_shaft_system 或日志文件）" : ""));
        return r;
    }

    private static string Format(string name, string unit, double value) =>
        string.IsNullOrWhiteSpace(unit) ? $"{name} = {Fmt(value)}" : $"{name} = {Fmt(value)} {unit}";

    private static string Fmt(double v) => v.ToString("0.####", CultureInfo.InvariantCulture);

    /// <summary>
    /// 当前工程设计状态描述（各部件关键参数），供 Agent 报告/判断前提，避免自己臆造参数。
    /// </summary>
    public static string DescribeCurrentState()
    {
        var dispatcher = Application.Current?.Dispatcher;
        if (dispatcher is not null && !dispatcher.CheckAccess())
            return dispatcher.Invoke(DescribeCurrentStateCore);
        return DescribeCurrentStateCore();
    }

    private static string DescribeCurrentStateCore()
    {
        var set = WholeMachineSnapshot.CurrentSet;
        if (set is null) return "当前没有设计工程（未找到减速器数据对象）。";

        var sb = new StringBuilder();
        sb.AppendLine("══ 当前设计工程参数 ══");

        // 总体
        sb.AppendLine("[总体]");
        foreach (var n in new[] { "减速器输出部件", "减速器输出力矩", "减速器输出转速", "太阳轮齿数", "行星轮齿数", "摆线轮齿数", "针轮齿数" })
            AppendDict(set, sb, n);

        // 渐开线齿轮副（太阳轮 / 行星轮）
        sb.AppendLine("[渐开线齿轮副] 顺序＝太阳轮 / 行星轮");
        foreach (var n in new[] { "法向模数", "压力角", "螺旋角", "中心距", "齿数", "设计齿宽", "行星轮个数" })
        {
            var vals = new List<string>();
            foreach (var g in set.mInvGearPair.mGears)
                if (g.mDataStorageDict.TryGetValue(n, out var it) && it is not null)
                    vals.Add(it.Value.ToString("0.####", CultureInfo.InvariantCulture));
            if (vals.Count > 0) sb.AppendLine($"   {n} = {string.Join(" / ", vals)}");
        }

        // 摆线针轮副
        sb.AppendLine("[摆线针轮副]");
        foreach (var n in new[] { "齿数", "滚针半径", "分度圆半径", "偏心距", "设计齿宽", "精度等级", "摆线轮传递总力矩" })
            AppendDict(set.mCycloidGearPair, sb, n);

        // 曲轴
        var shaft = set.mCrankShaftSystem.mCrankShaft.mShaftDesignPage.currentShaft;
        var sections = shaft?.OriginalAxisSections;
        sb.AppendLine("[曲轴系]");
        sb.AppendLine(shaft is null
            ? "   未设计（需先在「曲轴系 → 曲轴」设计轴或执行轴推荐；否则曲轴系计算不可用）"
            : $"   已设计：轴段 {sections?.Count ?? 0} 段，最大外径 {(sections is { Count: > 0 } ? sections.Max(s => s.BaseOuterDiameter) : 0):0.####} mm，" +
              $"总长 {(sections is { Count: > 0 } ? sections.Sum(s => s.CurrentLength) : 0):0.####} mm，轴承 {set.mCrankShaftSystem.mBearingList.Count} 个");

        return sb.ToString().TrimEnd();
    }

    private static void AppendDict(object owner, StringBuilder sb, string key)
    {
        var v = GetDict(owner, key);
        if (Math.Abs(v) > double.Epsilon) sb.AppendLine($"   {key} = {v.ToString("0.####", CultureInfo.InvariantCulture)}");
    }

    /// <summary>自动导出钩子：设 RV_SNAPSHOT_DIR 时把曲轴系计算结果一并写入该目录（便于外部核对/回归）</summary>
    public static string? TryAutoDump()
    {
        try
        {
            var dir = Environment.GetEnvironmentVariable("RV_SNAPSHOT_DIR");
            if (string.IsNullOrWhiteSpace(dir)) return null;
            var result = CalcCrankShaftSystem();
            var path = Path.Combine(dir, $"crankShaft-calc-{DateTime.Now:yyyyMMdd-HHmmss}.json");
            Directory.CreateDirectory(dir);
            File.WriteAllText(path, result.ToJson());
            return path;
        }
        catch
        {
            return null; // 自动导出失败不得影响界面
        }
    }

    private static double GetDict(object owner, string key)
    {
        try
        {
            var dictProp = owner.GetType().GetProperty("mDataStorageDict");
            if (dictProp?.GetValue(owner) is CTANDict dict && dict.TryGetValue(key, out var item) && item is not null)
                return item.Value;
        }
        catch
        {
            // 字典缺失/键不存在：按 0 处理（调用方据此提示"未设置"）
        }
        return 0;
    }
}
