using System;
using System.IO;
using System.Text.Json;
using RVAutoTest.Models;

namespace RVAutoTest
{
    /// <summary>
    /// 测试配置加载器：从 JSON 文件读取输入参数，并与默认值合并。
    /// 配置字段缺失/null 时使用默认值（与原有硬编码一致）。
    /// </summary>
    public static class ConfigLoader
    {
        /// <summary>
        /// 加载配置 JSON 文件。
        /// </summary>
        /// <param name="configPath">配置文件路径；null/空串/文件不存在时返回 null（表示用默认配置）</param>
        /// <exception cref="InvalidOperationException">JSON 格式错误时抛出，含位置提示</exception>
        public static TestConfig? Load(string? configPath)
        {
            if (string.IsNullOrWhiteSpace(configPath) || !File.Exists(configPath))
                return null;

            try
            {
                var json = File.ReadAllText(configPath);
                return JsonSerializer.Deserialize<TestConfig>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    ReadCommentHandling = JsonCommentHandling.Skip,
                    AllowTrailingCommas = true
                });
            }
            catch (JsonException ex)
            {
                throw new InvalidOperationException(
                    $"配置文件 '{configPath}' 解析失败: {ex.Message} (行 {ex.LineNumber}, 列 {ex.BytePositionInLine})", ex);
            }
        }

        /// <summary>
        /// 加载 ISO/TR 6336-30 Example 1 参数文件；文件缺失/解析失败时返回 null（用硬编码默认）。
        /// </summary>
        public static IsoExample1Params? LoadIsoExample1(string? configPath)
        {
            if (string.IsNullOrWhiteSpace(configPath) || !File.Exists(configPath))
                return null;
            try
            {
                var json = File.ReadAllText(configPath);
                return JsonSerializer.Deserialize<IsoExample1Params>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    ReadCommentHandling = JsonCommentHandling.Skip,
                    AllowTrailingCommas = true
                });
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"⚠️ ISO 参数文件解析失败: {ex.Message}（将使用硬编码默认值）");
                return null;
            }
        }

        /// <summary>
        /// 加载摆线针轮副参数文件（cycloid_params.json），反序列化为 CycloidGearInput。
        /// 文件缺失/解析失败时返回 null（调用方回退 CycloidGearInput 默认值）；
        /// JSON 中的 _ 前缀注释键被忽略，缺失字段保留默认值。
        /// </summary>
        public static CycloidGearInput? LoadCycloidParams(string? configPath)
        {
            if (string.IsNullOrWhiteSpace(configPath) || !File.Exists(configPath))
                return null;
            try
            {
                var json = File.ReadAllText(configPath);
                return JsonSerializer.Deserialize<CycloidGearInput>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    ReadCommentHandling = JsonCommentHandling.Skip,
                    AllowTrailingCommas = true
                });
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"⚠️ 摆线参数文件解析失败: {ex.Message}（将使用默认值）");
                return null;
            }
        }

        /// <summary>解析齿轮推荐输入：配置值 + 默认值补全（null 字段用默认值）</summary>
        public static GearRecommendInput ResolveGear(TestConfig? config)
        {
            var input = GetDefaultGearInput();
            var cfg = config?.GearRecommend;
            if (cfg is null) return input;

            if (cfg.FixElemNotation is double fe) input.FixElemNotation = fe;
            if (cfg.TotalRatio is double tr) input.TotalRatio = tr;
            if (cfg.OutputTorque is double ot) input.OutputTorque = ot;
            if (cfg.OutputRPM is double rpm) input.OutputRPM = rpm;
            if (cfg.TargetStress is double ts) input.TargetStress = ts;
            if (cfg.DesignLife is double dl) input.DesignLife = dl;
            if (cfg.UsageFactor is double uf) input.UsageFactor = uf;
            if (cfg.AllowableStress is double as_) input.AllowableStress = as_;
            if (cfg.PlanetCount is double pc) input.PlanetCount = pc;
            if (cfg.CycloidMinWidth is double minW) input.CycloidMinWidth = minW;
            if (cfg.CycloidMaxWidth is double maxW) input.CycloidMaxWidth = maxW;
            return input;
        }

        /// <summary>解析轴推荐输入：配置值 + 默认值补全（null 字段用默认值）</summary>
        public static ShaftRecommendInput ResolveShaft(TestConfig? config)
        {
            var input = GetDefaultShaftInput();
            var cfg = config?.ShaftRecommend;
            if (cfg is null) return input;

            if (cfg.AllowableBendStress is double bs) input.AllowableBendStress = bs;
            if (cfg.AllowableShearStress is double ss) input.AllowableShearStress = ss;
            if (cfg.BearingDesignLife is double bl) input.BearingDesignLife = bl;
            return input;
        }

        /// <summary>
        /// 校验配置数值的合理性，返回错误描述；无错误返回 null。
        /// </summary>
        public static string? Validate(TestConfig? config)
        {
            var gear = config?.GearRecommend;
            if (gear is not null)
            {
                if (gear.CycloidMinWidth is double minW && gear.CycloidMaxWidth is double maxW && minW >= maxW)
                    return $"摆线轮齿宽约束非法: 最小齿宽({minW}) 必须小于最大齿宽({maxW})";
                if (gear.TotalRatio is double tr && tr <= 0)
                    return $"总传动比非法: {tr} (应 > 0)";
                if (gear.UsageFactor is double uf && uf <= 0)
                    return $"使用系数非法: {uf} (应 > 0)";
                if (gear.DesignLife is double dl && dl <= 0)
                    return $"设计寿命非法: {dl} (应 > 0)";
            }
            var shaft = config?.ShaftRecommend;
            if (shaft is not null)
            {
                if (shaft.AllowableBendStress is double bs && bs <= 0)
                    return $"许用弯曲正应力非法: {bs} (应 > 0)";
                if (shaft.AllowableShearStress is double ss && ss <= 0)
                    return $"许用剪应力非法: {ss} (应 > 0)";
                if (shaft.BearingDesignLife is double bl && bl <= 0)
                    return $"轴承设计寿命非法: {bl} (应 > 0)";
            }
            var cyc = config?.CycloidGear;
            if (cyc is not null)
            {
                // 精度等级非法会触发 C++ ObtainFourCycloidPara 越界崩溃（.NET 无法捕获），必须校验
                if (cyc.PrecGrade is double pg && (pg < 0 || pg > 12))
                    return $"摆线精度等级非法: {pg}（仅支持 0~12，0 级在 DLL f_p 反查时会失败，建议 5 级）";
                if (cyc.TorqueScale is double ts && ts <= 0)
                    return $"摆线力矩缩放系数非法: {ts}（应 > 0，1 表示关闭 N·m→N·mm 转换）";
                if (cyc.PinionZ is double pz && pz <= 0)
                    return $"针轮齿数非法: {pz} (应 > 0)";
                if (cyc.RollerRadius is double rr && rr <= 0)
                    return $"滚针半径非法: {rr} (应 > 0)";
                if (cyc.PitchCircleRadius is double pcr && pcr <= 0)
                    return $"针轮分度圆半径非法: {pcr} (应 > 0)";
                if (cyc.Cycloid1Eccentricity is double e1 && e1 <= 0)
                    return $"摆线轮1偏心距非法: {e1} (应 > 0)";
                if (cyc.Cycloid2Eccentricity is double e2 && e2 <= 0)
                    return $"摆线轮2偏心距非法: {e2} (应 > 0)";
            }
            var inv = config?.InvGear;
            if (inv is not null)
            {
                if (inv.Module is double m && m <= 0)
                    return $"法向模数非法: {m} (应 > 0)";
                if (inv.CenterDistance is double cd && cd <= 0)
                    return $"中心距非法: {cd} (应 > 0)";
                if (inv.SunTeeth is double st && st <= 0)
                    return $"太阳轮齿数非法: {st} (应 > 0)";
                if (inv.PlanetTeeth is double pt && pt <= 0)
                    return $"行星轮齿数非法: {pt} (应 > 0)";
                if (inv.PlanetCount is double pc2 && pc2 <= 0)
                    return $"行星轮个数非法: {pc2} (应 > 0)";
            }
            return null;
        }

        /// <summary>默认齿轮推荐输入（与原有硬编码一致，来自 SetGearContentDrawer.TmpTest()）</summary>
        public static GearRecommendInput GetDefaultGearInput() => new()
        {
            FixElemNotation = 1,      // 1=曲轴固定
            TotalRatio = 153,         // 总传动比
            OutputTorque = -784.0,    // N·m
            OutputRPM = 15.0,         // r/min
            TargetStress = 1000,      // 目标应力
            DesignLife = 50000,       // h
            UsageFactor = 1.2,        // 使用系数
            AllowableStress = 1500,   // 材料许用极限接触应力
            PlanetCount = 3,          // 行星轮个数
            CycloidMinWidth = 40,     // 摆线轮最小齿宽 mm
            CycloidMaxWidth = 80      // 摆线轮最大齿宽 mm
        };

        /// <summary>默认轴推荐输入（来自 SetShaftContentDrawer.TmpTest()）</summary>
        public static ShaftRecommendInput GetDefaultShaftInput() => new()
        {
            AllowableBendStress = 4,   // 许用弯曲正应力 MPa
            AllowableShearStress = 12, // 许用剪应力 MPa
            BearingDesignLife = 6700   // 轴承设计寿命 h
        };
    }
}
