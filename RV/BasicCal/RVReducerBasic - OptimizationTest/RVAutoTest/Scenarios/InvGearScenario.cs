using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using RVAutoTest.Models;

namespace RVAutoTest.Scenarios
{
    /// <summary>
    /// 场景: 渐开线齿轮副计算。
    /// 通过 DLL CalInvGearStress2 计算太阳轮/行星轮齿轮副的接触/弯曲应力与几何参数。
    /// 输出 284 元素（每齿轮 142 字段，字段名与 CTANInvGearPair.mOutPutHeadName 一致）。
    /// 输入可来自齿轮推荐输出映射（数据闭环）或默认值，由外部配置覆盖。
    /// </summary>
    public static class InvGearScenario
    {
        /// <summary>执行渐开线齿轮副计算场景</summary>
        /// <param name="outputDir">输出目录</param>
        /// <param name="config">外部配置（可为 null）</param>
        /// <param name="gearRecommendOutput">齿轮推荐输出（可为 null，非空时自动映射几何段）</param>
        public static InvGearResult Run(string outputDir, TestConfig? config, double[]? gearRecommendOutput = null)
        {
            var result = new InvGearResult();
            Directory.CreateDirectory(outputDir);

            // ── 1. 构建输入：推荐映射 + 配置覆盖 ──
            var input = new InvGearInput();
            if (gearRecommendOutput is not null)
                input.MapFromRecommend(gearRecommendOutput);
            ApplyConfig(input, config);

            // ── 2. 调用 DLL 计算（174 输入 → 284 输出）──
            double[] outData;
            try
            {
                outData = DllInvoker.CallCalInvGearStress2(InvGearInput.GetDivideIndex(), input.ToArray());
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"渐开线齿轮副 DLL 调用失败: {ex.Message}", ex);
            }

            // ── 3. 解析 284 → 太阳轮字典 + 行星轮字典（每齿轮 142 字段）──
            const int fieldsPerGear = 142;
            if (outData.Length < fieldsPerGear * 2)
                throw new InvalidOperationException($"渐开线齿轮副输出长度异常: {outData.Length}（期望 284）");
            if (InvGearOutputFieldNames.Fields.Length < fieldsPerGear)
                throw new InvalidOperationException(
                    $"渐开线输出字段表长度异常: {InvGearOutputFieldNames.Fields.Length}（期望 ≥ {fieldsPerGear}）");

            for (int j = 0; j < fieldsPerGear; j++)
            {
                var name = InvGearOutputFieldNames.Fields[j];
                result.太阳轮[name] = outData[j];
                result.行星轮[name] = outData[fieldsPerGear + j];
            }

            // ── 打印关键应力 ──
            Console.WriteLine($"  📊 渐开线齿轮副: 太阳轮σH={Get(result.太阳轮, "计算接触应力"):F2} MPa, " +
                              $"行星轮σH={Get(result.行星轮, "计算接触应力"):F2} MPa");

            // ── 4. 保存结果 ──
            var path = Path.Combine(outputDir, "inv-gear-result.json");
            File.WriteAllText(path, JsonSerializer.Serialize(result, JsonOptions.Pretty));
            Console.WriteLine($"  📄 渐开线齿轮副结果: {path}");
            Console.WriteLine("✅ 渐开线齿轮副计算完成");
            return result;
        }

        /// <summary>应用外部配置（覆盖默认/推荐值）</summary>
        private static void ApplyConfig(InvGearInput input, TestConfig? config)
        {
            var cfg = config?.InvGear;
            if (cfg is null) return;

            // 载荷段
            if (cfg.Torque is double v0) input.扭矩 = v0;
            if (cfg.Speed is double v1) input.转速 = v1;
            if (cfg.DesignLife is double v2) input.预期寿命 = v2;

            // 公共几何
            if (cfg.Module is double v3) input.法向模数 = v3;
            if (cfg.PressureAngle is double v4) input.压力角度 = v4;
            if (cfg.HelixAngle is double v5) input.螺旋角度 = v5;
            if (cfg.CenterDistance is double v6) input.中心距 = v6;
            if (cfg.PlanetCount is double v7) input.行星轮个数 = v7;

            // 齿轮几何（索引 0=齿数, 2=设计齿宽）
            if (cfg.SunTeeth is double v8) input.太阳轮几何[0] = v8;
            if (cfg.PlanetTeeth is double v9) input.行星轮几何[0] = v9;
            if (cfg.SunWidth is double v10) input.太阳轮几何[2] = v10;
            if (cfg.PlanetWidth is double v11) input.行星轮几何[2] = v11;
        }

        /// <summary>字典取值（键不存在时返回 NaN）</summary>
        private static double Get(Dictionary<string, double> dict, string key)
            => dict.TryGetValue(key, out double v) ? v : double.NaN;
    }
}
