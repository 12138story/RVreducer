using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;

namespace RVReducerDesignApp.Services.Agent;

/// <summary>
/// 计算类工具：通过 P/Invoke 直接调用 RVReducerBasic.dll 的渐开线齿轮副计算接口
/// （与 RvCalc 通用调用器等价的专用实现，保留用于最常用的渐开线应力计算）
/// </summary>
public static class AgentCalcTools
{
    /// <summary>角度(°) → 弧度 系数，与 C++ #define Degree 一致</summary>
    private const double Degree = 0.01745329252;

    // ═══════════════════ P/Invoke ═══════════════════
    // InDivideIndex: 34 元素分割索引；InInputData: 174 元素输入；OutData: 284 元素输出（142×2 齿轮）
    [DllImport("RVReducerBasic.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern void CalInvGearStress2(ref int inDivideIndex, ref double inInputData, ref double outData);

    /// <summary>构建 calculate_inv_gear_stress 工具</summary>
    public static AgentTool BuildInvGearStressTool() => new()
    {
        Name = "calculate_inv_gear_stress",
        Description =
            "计算渐开线齿轮副（太阳轮/行星轮）的接触应力与弯曲应力（ISO 6336 方法）。" +
            "参数：法向模数 module、太阳轮齿数 sun_teeth 为必填；" +
            "建议同时提供 torque（N·m）、speed（r/min）以获得有意义的应力。其余参数有默认值。",
        Parameters = JsonSchema(new
        {
            type = "object",
            properties = new
            {
                module = new { type = "number", description = "法向模数 mn (mm)，必填" },
                sun_teeth = new { type = "integer", description = "太阳轮（小齿轮）齿数，必填" },
                planet_teeth = new { type = "integer", description = "行星轮（大齿轮）齿数，默认 106" },
                pressure_angle = new { type = "number", description = "压力角（度），默认 20" },
                helix_angle = new { type = "number", description = "螺旋角（度），默认 0" },
                center_distance = new { type = "number", description = "中心距 (mm)，缺省按 (z1+z2)*mn/2 推导" },
                torque = new { type = "number", description = "输入扭矩 (N·m)，默认 100" },
                speed = new { type = "number", description = "转速 (r/min)，默认 0" },
                life_hours = new { type = "number", description = "设计寿命 (h)，默认 50000" },
                planet_count = new { type = "integer", description = "行星轮个数，默认 3" }
            },
            required = new[] { "module", "sun_teeth" }
        }),
        ExecuteAsync = (argsJson, _) =>
        {
            try
            {
                using var doc = JsonDocument.Parse(argsJson);
                var root = doc.RootElement;

                // 输入校验（防止 DLL 侧除零/无意义结果）
                double mn = GetDouble(root, "module", 2);
                double sunTeeth = GetDouble(root, "sun_teeth", 23);
                double planetCount = GetDouble(root, "planet_count", 3);
                double torque = GetDouble(root, "torque", 100);

                if (mn <= 0) return Task.FromResult("错误：法向模数必须为正数");
                if (sunTeeth < 3) return Task.FromResult("错误：太阳轮齿数至少为 3");
                if (planetCount < 1) return Task.FromResult("错误：行星轮个数至少为 1");
                if (torque < 0) return Task.FromResult("错误：扭矩不能为负（如需反向请说明方向）");

                return Task.FromResult(RunInvGearStress(argsJson));
            }
            catch (Exception ex)
            {
                return Task.FromResult($"计算失败: {ex.GetType().Name}: {ex.Message}");
            }
        }
    };

    // ═══════════════════ 计算主流程 ═══════════════════
    private static string RunInvGearStress(string argsJson)
    {
        using var doc = JsonDocument.Parse(argsJson);
        var root = doc.RootElement;

        double[] input = BuildInput(root);
        int[] divideIndex = GetDivideIndex();
        double[] output = new double[284];

        CalInvGearStress2(ref divideIndex[0], ref input[0], ref output[0]);

        return FormatOutput(output);
    }

    // ═══════════════════ 174 字段输入构建 ═══════════════════
    private static double[] BuildInput(JsonElement root)
    {
        double mn = GetDouble(root, "module", 2);
        double sunTeeth = GetDouble(root, "sun_teeth", 23);
        double planetTeeth = GetDouble(root, "planet_teeth", 106);
        double pressureAngle = GetDouble(root, "pressure_angle", 20);
        double helixAngle = GetDouble(root, "helix_angle", 0);
        double centerDist = GetDouble(root, "center_distance", (sunTeeth + planetTeeth) * mn / 2);
        double planetCount = GetDouble(root, "planet_count", 3);
        double torque = GetDouble(root, "torque", 100);
        double speed = GetDouble(root, "speed", 0);
        double life = GetDouble(root, "life_hours", 50000);

        var data = new double[174];

        // ── 太阳轮几何 [0..30]（与 InvGearInput.SetDefaults 一致）──
        Copy(new double[]
        {
            23, -1, 20, 1, 0.5142, 0.25, 0.384, 0, 0, 0, 0, 2, 0, 0, 0,
            -0.0670, -0.0926, 0, 0, 0, 0, 25, 0, 1.4, 0.39, 0, 0, 0, 0, 0, 0,
        }, data, 0);
        data[0] = sunTeeth; // 太阳轮齿数

        // ── 行星轮几何 [31..61] ──
        Copy(new double[]
        {
            106, 1, 20, 1, 0, 0.25, 0.384, 0, 0, 0, 0, 4, 0, 0, 0,
            -0.0617, -0.0947, 0, 0, 0, 0, 67, 0, 1.4, 0.39, 0, 0, 0, 0, 0, 0,
        }, data, 31);
        data[31] = planetTeeth; // 行星轮齿数

        // ── 公共几何 [62..70] ──
        data[62] = mn;                      // 法向模数
        data[63] = pressureAngle * Degree;  // 压力角（弧度）
        data[64] = helixAngle * Degree;     // 螺旋角（弧度）
        data[65] = centerDist;              // 中心距
        data[66] = 1;                       // 齿轮副类型（一般外啮合）
        data[67] = planetCount;             // 行星轮个数
        data[68] = 0;                       // 驱动齿轮（0=太阳轮）
        data[69] = 0;                       // 触点间隙
        data[70] = speed;                   // 转速

        // ── 材料+加工 [93..124]（20CrMnTi 类默认）──
        Copy(new double[] { 206000, 0.3, 500, 7.83e-6, 1500, 500, 500, 1080 }, data, 93);  // 太阳轮材料
        Copy(new double[] { 9, 1, 60, 30, 0, 0, 0, 0 }, data, 101);                        // 太阳轮加工
        Copy(new double[] { 206000, 0.3, 500, 7.83e-6, 1500, 500, 500, 1080 }, data, 109); // 行星轮材料
        Copy(new double[] { 9, 1, 60, 30, 0, 0, 0, 0 }, data, 117);                        // 行星轮加工

        // ── 应用 [125..148] ──
        Copy(new double[] { 0, 1, 1, 0, planetCount, 1, 1, 1 }, data, 125);  // 太阳轮应用
        Copy(new double[] { 0, 1, 1, 0, planetCount, 1, 1, 1 }, data, 133);  // 行星轮应用
        Copy(new double[] { 1.25, 1.0, 0.0, 320, 1.0, 1.0, 1.0, 0 }, data, 141); // 公共应用

        // ── 公差 [149..160] ──
        Copy(new double[] { 5, 5.5, 6.0, 5.5, 1.0, 3.0 }, data, 149); // 太阳轮公差
        Copy(new double[] { 5, 6.0, 6.0, 5.5, 1.0, 3.0 }, data, 155); // 行星轮公差

        // ── 载荷 [161..163] ──
        data[161] = torque; // 扭矩 (N·m)
        data[162] = speed;  // 转速 (r/min)
        data[163] = life;   // 预期寿命 (h)

        // ── 装配 [164..169] ──
        Copy(new double[] { 5, 0, planetCount, 17, 0, 34 }, data, 164);

        // ── 定位 [170..173] ──
        data[170] = 9;
        data[171] = -1;
        data[172] = 5.45;
        data[173] = 1;

        return data;
    }

    /// <summary>
    /// 分割索引（34 元素），与 InvGearInput.GetDivideIndex() 一致。
    /// ⚠ index 29 必须是 142（输出列数），否则 DLL 第二齿轮块输出越界错乱。
    /// </summary>
    private static int[] GetDivideIndex() => new[]
    {
        174, 284,                       // 总输入, 总输出
        31, 3, 93, 93,                  // 几何
        8, 4, 32, 125,                  // 材料+加工
        8, 3, 24, 149,                  // 应用
        6, 2, 12, 161,                  // 公差
        3, 1, 3, 164,                   // 载荷
        6, 1, 6, 170,                   // 装配
        2, 2, 4, 142,                   // 定位（index 29 = 输出列数 142）
        142, 2, 284, 284                // 输出
    };

    // ═══════════════════ 输出格式化 ═══════════════════
    // 输出索引对照 InvGearOutputFieldNames.Fields：
    // [1]=计算接触应力 [2]=许用接触应力 [4]=计算弯曲应力 [5]=许用弯曲应力
    // [21]=分度圆直径[d] [23]=齿顶圆直径[da] [35]=齿根圆直径[df] [46]=全齿高[h]
    private static string FormatOutput(double[] output)
    {
        // output[i*142+j]：i=0 太阳轮, i=1 行星轮
        var sb = new StringBuilder();

        for (int g = 0; g < 2; g++)
        {
            string label = g == 0 ? "太阳轮" : "行星轮";
            double 计算接触 = output[g * 142 + 1];
            double 许用接触 = output[g * 142 + 2];
            double 计算弯曲 = output[g * 142 + 4];
            double 许用弯曲 = output[g * 142 + 5];

            double sh = 计算接触 > 0 && 许用接触 > 0 ? 许用接触 / 计算接触 : double.NaN;
            double sf = 计算弯曲 > 0 && 许用弯曲 > 0 ? 许用弯曲 / 计算弯曲 : double.NaN;

            sb.AppendLine($"【{label}】");
            sb.AppendLine($"  计算接触应力 σH = {计算接触:F2} MPa（许用 {许用接触:F2}，安全系数 SH = {sh:F2}）");
            sb.AppendLine($"  计算弯曲应力 σF = {计算弯曲:F2} MPa（许用 {许用弯曲:F2}，安全系数 SF = {sf:F2}）");

            // 关键几何（仅太阳轮展示，避免冗余）
            if (g == 0)
            {
                sb.AppendLine($"  分度圆直径 d = {output[21]:F2} mm，齿顶圆 da = {output[23]:F2}，齿根圆 df = {output[35]:F2}，全齿高 h = {output[46]:F2}");
            }
        }

        // 异常告警：两个齿轮的接触应力都检查
        bool abnormal = false;
        for (int g = 0; g < 2; g++)
        {
            double sh = output[g * 142 + 1];
            if (sh >= 1e9 || sh <= 0 || double.IsNaN(sh) || double.IsInfinity(sh))
                abnormal = true;
        }
        if (abnormal)
            sb.AppendLine("⚠️ 警告：计算接触应力异常，可能输入参数组合不物理（如中心距/齿数不匹配），结果仅供调试参考。");

        return sb.ToString();
    }

    // ═══════════════════ 辅助 ═══════════════════
    private static BinaryData JsonSchema(object obj) =>
        BinaryData.FromString(JsonSerializer.Serialize(obj));

    private static double GetDouble(JsonElement root, string name, double fallback) =>
        root.TryGetProperty(name, out var e) && e.ValueKind == JsonValueKind.Number
            ? e.GetDouble()
            : fallback;

    private static void Copy(double[] source, double[] dest, int offset) =>
        Array.Copy(source, 0, dest, offset, source.Length);
}
