using System;
using System.IO;
using System.Text.Json;
using RVAutoTest.Models;

namespace RVAutoTest.Scenarios
{
    /// <summary>
    /// 场景 3: 摆线针轮副计算。
    /// 通过 DLL CalCycloidGearStressByObj 计算摆线针轮副的接触应力等参数。
    /// 输入可来自齿轮推荐输出映射（数据闭环）或手动配置。
    /// </summary>
    public static class CycloidGearScenario
    {
        /// <summary>执行摆线针轮副计算场景</summary>
        /// <param name="outputDir">输出目录</param>
        /// <param name="config">外部配置（可为 null）</param>
        /// <param name="gearRecommendOutput">齿轮推荐输出（可为 null，非空时自动映射几何/载荷段）</param>
        public static CycloidGearResult Run(string outputDir, TestConfig? config, double[]? gearRecommendOutput = null)
        {
            var result = new CycloidGearResult();
            Directory.CreateDirectory(outputDir);

            // ── 1. 构建输入：推荐映射 + 配置覆盖 ──
            var input = new CycloidGearInput();
            if (gearRecommendOutput is not null)
                MapFromRecommend(input, gearRecommendOutput);
            ApplyConfig(input, config);

            // ── 2. 计算（力矩换算 + DLL 调用 + 解析，共享逻辑见 Calculate）──
            result = Calculate(input);

            // DLL 失败标记检测：FindMaxContStress 在所有 θ 点应力 <= 1e-6 时返回 1e10；NaN/∞ 亦视为异常
            bool abnormal = result.接触应力 >= 1e9
                || double.IsNaN(result.接触应力)
                || double.IsInfinity(result.接触应力);
            if (abnormal)
            {
                Console.WriteLine($"  ⚠️ 摆线接触应力异常 {result.接触应力:E0}（DLL 失败标记/数值异常，" +
                                  "可能与力矩单位 N·m/N·mm 或曲率数值问题有关，UI 应用同样受影响）");
            }
            else
            {
                Console.WriteLine($"  📊 摆线针轮副: 接触应力={result.接触应力:F2} MPa, K1={result.短幅系数K1:F4}, h={result.全齿高h:F3}");
            }

            // ── 4. 保存结果 ──
            var path = Path.Combine(outputDir, "cycloid-gear-result.json");
            File.WriteAllText(path, JsonSerializer.Serialize(result, JsonOptions.Pretty));
            Console.WriteLine($"  📄 摆线针轮结果: {path}");
            Console.WriteLine("✅ 场景3 摆线针轮副计算完成");
            return result;
        }

        /// <summary>
        /// 执行摆线针轮副计算：力矩单位换算（N·m→N·mm，按 input.TorqueScale）+ DLL 调用 + 结果解析。
        /// 注意：会就地修改 input 的力矩字段（乘以 TorqueScale）。
        /// </summary>
        /// <param name="input">摆线针轮副输入（力矩字段为 N·m）</param>
        public static CycloidGearResult Calculate(CycloidGearInput input)
        {
            // DLL 摆线接触应力公式 FMax=4*T/(K1*zg*Rz) 期望力矩 T 单位为 N·mm（量纲 T/Rz 得 N），
            // 但 UI/推荐系统以 N·m 传递（如 764.4 N·m）。不转换则应力偏小 ~31.6 倍，
            // UI 应用因此常显示 1e10 失败标记。默认 ×1000 修正，可用 TorqueScale 覆盖。
            double torqueScale = input.TorqueScale;
            if (torqueScale <= 0)
                throw new InvalidOperationException($"TorqueScale 非法: {torqueScale}（应为正数，1=关闭转换）");
            if (torqueScale != 1.0)
            {
                input.Cycloid1Torque *= torqueScale;
                input.Cycloid2Torque *= torqueScale;
                Console.WriteLine($"  🔧 摆线力矩已按 ×{torqueScale:0.###} 转换（N·m→N·mm，DLL 期望单位）");
            }

            double[] outData;
            try
            {
                outData = DllInvoker.CallCalCycloidGearStressByObj(CycloidGearInput.GetDivideIndex(), input.ToArray());
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"摆线针轮副 DLL 调用失败: {ex.Message}", ex);
            }

            try
            {
                return CycloidGearResult.FromOutData(outData);
            }
            catch (ArgumentException ex)
            {
                throw new InvalidOperationException(ex.Message, ex);
            }
        }

        /// <summary>从齿轮推荐输出映射摆线针轮输入（数据闭环）</summary>
        private static void MapFromRecommend(CycloidGearInput input, double[] outData)
        {
            // 滚针 (OutData[71..76]): 齿数, 滚针半径, 设计齿宽, 针轮分度圆半径, 弹性模量, 使用系数
            if (outData.Length > 76)
            {
                input.PinionZ = outData[71];
                input.RollerRadius = outData[72];
                input.PinionWidth = outData[73];
                input.PitchCircleRadius = outData[74];
                input.PinionEModule = outData[75];
                input.PinionKp = outData[76];
            }
            // 摆线轮 1 (OutData[77..82]): 齿数, 偏心距, 设计齿宽, 弹性模量, 使用系数, 传递总力矩
            if (outData.Length > 82)
            {
                input.Cycloid1Z = outData[77];
                input.Cycloid1Eccentricity = outData[78];
                input.Cycloid1Width = outData[79];
                input.Cycloid1EModule = outData[80];
                input.Cycloid1Kp = outData[81];
                input.Cycloid1Torque = outData[82];
            }
            // 摆线轮 2 (OutData[83..88])
            if (outData.Length > 88)
            {
                input.Cycloid2Z = outData[83];
                input.Cycloid2Eccentricity = outData[84];
                input.Cycloid2Width = outData[85];
                input.Cycloid2EModule = outData[86];
                input.Cycloid2Kp = outData[87];
                input.Cycloid2Torque = outData[88];
            }
        }

        /// <summary>应用外部配置（覆盖默认/推荐值）</summary>
        private static void ApplyConfig(CycloidGearInput input, TestConfig? config)
        {
            var cfg = config?.CycloidGear;
            if (cfg is null) return;

            if (cfg.PinionZ is double v0) input.PinionZ = v0;
            if (cfg.RollerRadius is double v1) input.RollerRadius = v1;
            if (cfg.PitchCircleRadius is double v2) input.PitchCircleRadius = v2;
            if (cfg.PinionEModule is double v3) input.PinionEModule = v3;
            if (cfg.PinionWidth is double v4) input.PinionWidth = v4;
            if (cfg.Cycloid1Width is double v5) input.Cycloid1Width = v5;
            if (cfg.Cycloid1Eccentricity is double v6) input.Cycloid1Eccentricity = v6;
            if (cfg.Cycloid2Width is double v7) input.Cycloid2Width = v7;
            if (cfg.Cycloid2Eccentricity is double v8) input.Cycloid2Eccentricity = v8;
            if (cfg.PrecGrade is double v9)
            {
                input.Cycloid1PrecGrade = v9;
                input.Cycloid2PrecGrade = v9;
            }
            if (cfg.TorqueScale is double ts) input.TorqueScale = ts;
        }
    }
}
