using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using MySql.Data.MySqlClient;
using RvCalc;

namespace RVReducerDesignApp.Services;

/// <summary>轴承库中的一条轴承记录（字段名与数据库列一一对应）</summary>
public sealed class BearingCatalogItem
{
    public string Code { get; init; } = "";
    public string Table { get; init; } = "";
    /// <summary>RV 轴承类型码（与 C++ PreDefine.h 一致）：1001 深沟球 / 1002 滚针 / 1003 圆锥滚子 / 1004 角接触球</summary>
    public int TypeCode { get; init; }
    public string TypeName { get; init; } = "";
    public double d { get; init; }
    public double D { get; init; }
    public double B { get; init; }
    /// <summary>额定动载荷 (kN，数据库原值)</summary>
    public double Cr_kN { get; init; }
    /// <summary>额定静载荷 (kN，数据库原值)</summary>
    public double C0r_kN { get; init; }
    public double e { get; init; }
    public double Y { get; init; }
    public double Y0 { get; init; }

    public double Cr_N => Cr_kN * 1000.0;
    public double C0r_N => C0r_kN * 1000.0;
}

/// <summary>轴承寿命计算结果（含数据来源，便于核对）</summary>
public sealed class BearingLifeByCodeResult
{
    public bool Ok { get; set; }
    public string Message { get; set; } = "";
    public BearingCatalogItem? Bearing { get; set; }
    public double n { get; set; }
    public double Fa { get; set; }
    public double Fr { get; set; }
    public double LifeHours { get; set; }

    public bool LifeValid => LifeHours > 0 && !double.IsNaN(LifeHours) && !double.IsInfinity(LifeHours);

    public string ToText()
    {
        if (!Ok) return "❌ 无法计算：" + Message;
        var b = Bearing!;
        var sb = new System.Text.StringBuilder();
        sb.AppendLine($"══ 轴承 {b.Code} 寿命计算 ══");
        sb.AppendLine($"型号: {b.Code}（{b.TypeName}，数据表 {b.Table}）");
        sb.AppendLine($"尺寸: d={F(b.d)} mm, D={F(b.D)} mm, B={F(b.B)} mm");
        sb.AppendLine($"额定载荷: Cr={F(b.Cr_kN)} kN, C0r={F(b.C0r_kN)} kN" +
                      (b.TypeCode == 1003 ? $", e={F(b.e)}, Y={F(b.Y)}, Y0={F(b.Y0)}" : ""));
        sb.AppendLine($"工况: n={F(n)} r/min, Fa={F(Fa)} N, Fr={F(Fr)} N");
        sb.AppendLine(LifeValid
            ? $"计算寿命 L10h = {F(LifeHours)} h"
            : $"⚠ 寿命无效（{F(LifeHours)} h）：转速越界或载荷超出曲线定义域");
        return sb.ToString().TrimEnd();
    }

    private static string F(double v) => v.ToString("0.####", CultureInfo.InvariantCulture);
}

/// <summary>
/// 轴承库（MySQL: localhost:3729 / bearings）访问层 + 按型号算寿命。
///
/// 数据表与列序（与 C++ TANBearing.cpp 的 ObtainBearingDataVec / CalAndRecommedBearingLife 保持一致）：
///   row[0]=code(型号), row[1]=d, row[2]=D, row[3]=B, row[4]=Cr(kN), row[5]=C0r(kN)
///   圆锥滚子额外：row[6]=e, row[7]=Y, row[8]=Y0
/// ⚠ 数据库 Cr/C0r 单位是 kN，调用 DLL 前需 ×1000 换为 N（本类已处理）。
/// </summary>
public static class BearingCatalog
{
    /// <summary>连接串（可用环境变量 RV_BEARING_DB 覆盖，便于换库/换机器）</summary>
    public static string ConnectionString =>
        Environment.GetEnvironmentVariable("RV_BEARING_DB")
        ?? "Database=bearings;Server=localhost;Port=3729;Password='jay';UserID=root;charset=utf8";

    private sealed record TableInfo(string Table, int TypeCode, string TypeName);

    /// <summary>参与型号检索的表（顺序即优先级）</summary>
    private static readonly TableInfo[] Tables =
    {
        new("taperedrollerbearingsinsinglerows", 1003, "圆锥滚子轴承(单列)"),
        new("deepgrooveball", 1001, "深沟球轴承"),
        new("needlebearing", 1002, "滚针轴承"),
        new("angularcontactballbearingsingle", 1004, "角接触球轴承"),
    };

    /// <summary>
    /// 按型号检索轴承（先精确匹配 code，再模糊匹配）。
    /// 型号比较忽略大小写与空格，因此 "30238" / "30238 " / "30238x2" 都能命中。
    /// </summary>
    public static IReadOnlyList<BearingCatalogItem> Find(string code)
    {
        var key = (code ?? "").Trim();
        if (key.Length == 0) return Array.Empty<BearingCatalogItem>();

        var exact = new List<BearingCatalogItem>();
        var fuzzy = new List<BearingCatalogItem>();
        var errors = new List<string>();

        foreach (var t in Tables)
        {
            try
            {
                using var conn = new MySqlConnection(ConnectionString);
                conn.Open();
                // SELECT * + 按列序号读取：与 C++ TANBearing.cpp 的 row[0..8] 完全一致，
                // 不依赖列名（各表列名不同：型号列名是中文，外径列名是 "_D" 等）；表很小（数百行）
                using var cmd = new MySqlCommand($"SELECT * FROM `{t.Table}`", conn);
                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    var item = Read(reader, t);
                    if (item.Code.Length == 0) continue;
                    var normalized = Normalize(item.Code);
                    if (normalized == Normalize(key)) exact.Add(item);
                    else if (normalized.Contains(Normalize(key))) fuzzy.Add(item);
                }
            }
            catch (Exception ex)
            {
                errors.Add($"{t.Table}: {ex.Message}");
            }
        }

        if (exact.Count == 0 && fuzzy.Count == 0 && errors.Count == Tables.Length)
            throw new InvalidOperationException("轴承库连接失败：" + string.Join(" | ", errors));

        return exact.Count > 0 ? exact : fuzzy;
    }

    /// <summary>按型号查库并计算寿命 L10h（同一进程内调用 DLL）</summary>
    public static BearingLifeByCodeResult CalcLife(string code, double n, double fa, double fr)
    {
        var result = new BearingLifeByCodeResult { n = n, Fa = fa, Fr = fr };
        IReadOnlyList<BearingCatalogItem> found;
        try
        {
            found = Find(code);
        }
        catch (Exception ex)
        {
            result.Message = ex.Message;
            return result;
        }

        if (found.Count == 0)
        {
            result.Message = $"轴承库中未找到型号 “{code}”。可换成库中存在的型号，或先用 recommend 接口按工况筛选。";
            return result;
        }
        if (found.Count > 1)
        {
            var list = string.Join("、", found.Take(10).Select(x => x.Code));
            result.Message = $"型号 “{code}” 命中 {found.Count} 条（{list}），请给出更精确的型号。";
            return result;
        }

        var b = found[0];
        result.Bearing = b;

        try
        {
            var registryPath = RvInterfaceRegistry.ResolvePath(
                System.IO.Path.Combine(AppContext.BaseDirectory, "RvCalc", "interface-registry.json"));
            if (registryPath is null) { result.Message = "未找到 RvCalc/interface-registry.json"; return result; }

            var registry = RvInterfaceRegistry.Load(registryPath);
            using var runner = new RvCalcRunner(registry);
            var call = runner.Run("CalBearingLife", new Dictionary<string, string>
            {
                ["轴承类型"] = b.TypeName,
                ["转速"] = n.ToString(CultureInfo.InvariantCulture),
                ["轴向力Fa"] = fa.ToString(CultureInfo.InvariantCulture),
                ["径向力Fr"] = fr.ToString(CultureInfo.InvariantCulture),
                ["额定动载荷Cr"] = b.Cr_N.ToString(CultureInfo.InvariantCulture),
                ["额定静载荷C0r"] = b.C0r_N.ToString(CultureInfo.InvariantCulture),
                ["系数e"] = b.e.ToString(CultureInfo.InvariantCulture),
                ["系数Y"] = b.Y.ToString(CultureInfo.InvariantCulture),
                ["系数Y0"] = b.Y0.ToString(CultureInfo.InvariantCulture),
            });

            if (!call.Ok)
            {
                result.Message = string.Join("；", call.Errors.Count > 0 ? call.Errors : new List<string> { "DLL 计算失败" });
                return result;
            }

            var life = call.Outputs.FirstOrDefault(o => o.Name.StartsWith("寿命"));
            if (life is null) { result.Message = "未取到寿命输出"; return result; }
            result.LifeHours = life.Value;
            result.Ok = true;
            return result;
        }
        catch (Exception ex)
        {
            result.Message = ex.GetType().Name + ": " + ex.Message;
            return result;
        }
    }

    private static string Normalize(string s) => s.Trim().Replace(" ", "").ToUpperInvariant();

    /// <summary>按列序号读取（与 C++ row[0..8] 一致）：0=型号 1=d 2=D 3=B 4=Cr(kN) 5=C0r(kN) 6=e 7=Y 8=Y0</summary>
    private static BearingCatalogItem Read(System.Data.IDataRecord r, TableInfo t)
    {
        double Get(int ordinal)
        {
            try
            {
                if (ordinal >= r.FieldCount || r.IsDBNull(ordinal)) return 0;
                return Convert.ToDouble(r.GetValue(ordinal), CultureInfo.InvariantCulture);
            }
            catch
            {
                return 0;
            }
        }

        return new BearingCatalogItem
        {
            Code = r.FieldCount > 0 && !r.IsDBNull(0) ? r.GetValue(0)?.ToString()?.Trim() ?? "" : "",
            Table = t.Table,
            TypeCode = t.TypeCode,
            TypeName = t.TypeName,
            d = Get(1), D = Get(2), B = Get(3),
            Cr_kN = Get(4), C0r_kN = Get(5),
            e = t.TypeCode == 1003 ? Get(6) : 0,
            Y = t.TypeCode == 1003 ? Get(7) : 0,
            Y0 = t.TypeCode == 1003 ? Get(8) : 0,
        };
    }
}
