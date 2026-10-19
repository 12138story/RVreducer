using System;
using System.Collections.Generic;

namespace RVAutoTest.Models
{
    /// <summary>
    /// 摆线针轮副计算结果（9 中文字段，与 CTANCycloidGearPair.mOutPutHeadName 一致）。
    /// 由 DLL CalCycloidGearStressByObj 输出。
    /// </summary>
    public class CycloidGearResult
    {
        public string Scenario { get; set; } = "cycloid-gear";
        public DateTime Timestamp { get; set; } = DateTime.Now;

        // ── 输出 (OutData[0..8]) ──
        public double 接触应力 { get; set; }            // [0] MPa
        public double 全齿高h { get; set; }             // [1]
        public double 短幅系数K1 { get; set; }          // [2]
        public double 齿顶距D_T { get; set; }           // [3]
        public double 跨棒距M_Mf { get; set; }          // [4]
        public double 单个齿距偏差f_pt { get; set; }    // [5] μm
        public double 齿距累积总偏差F_p { get; set; }   // [6] μm
        public double 齿向偏差F_β { get; set; }         // [7] μm
        public double 径向跳动F_r { get; set; }         // [8] μm

        public List<string> Screenshots { get; set; } = new();

        /// <summary>
        /// 从 DLL 输出数组（9 元素）构造结果对象。
        /// </summary>
        /// <param name="outData">CalCycloidGearStressByObj 输出（长度至少 9）</param>
        /// <exception cref="ArgumentException">输出长度不足 9 时抛出</exception>
        public static CycloidGearResult FromOutData(double[] outData)
        {
            if (outData is null || outData.Length < 9)
                throw new ArgumentException($"摆线针轮副输出长度异常: {outData?.Length ?? 0}（期望 9）");

            return new CycloidGearResult
            {
                接触应力 = outData[0],
                全齿高h = outData[1],
                短幅系数K1 = outData[2],
                齿顶距D_T = outData[3],
                跨棒距M_Mf = outData[4],
                单个齿距偏差f_pt = outData[5],
                齿距累积总偏差F_p = outData[6],
                齿向偏差F_β = outData[7],
                径向跳动F_r = outData[8]
            };
        }
    }
}
