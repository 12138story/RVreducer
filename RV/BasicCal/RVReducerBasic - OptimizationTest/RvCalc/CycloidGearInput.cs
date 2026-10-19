using System;

namespace RvCalc
{
    /// <summary>
    /// 摆线针轮副输入构建器。
    /// 构建 CalCycloidGearStressByObj 的 60 元素输入数组 + 5 元素分割索引。
    /// 字段布局参考 CTANCycloidGearPair / InterfaceForCSharp.ConvertInputDataToCycloidObj。
    /// </summary>
    public class CycloidGearInput
    {
        // ── 针轮 (索引 0..7) ──
        public double PinionZ { get; set; } = 40;          // [0] 齿数
        public double RollerRadius { get; set; } = 4;      // [1] 滚针半径 rz (mm)
        public double PitchCircleRadius { get; set; } = 98.5; // [2] 针轮分度圆半径 Rzf (mm)
        public double PinionEModule { get; set; } = 206000;  // [3] 弹性模量 (MPa)
        public double PinionWidth { get; set; } = 80;      // [4] 针轮设计齿宽 (mm)
        public double PinionKp { get; set; } = 1;          // [5] 使用系数
        public double PinionLocDist { get; set; } = 0;     // [6] 定位面距基准面距离
        public double PinionLocFace { get; set; } = 0;     // [7] 定位面

        // ── 摆线轮 1 (索引 20..30) ──
        public double Cycloid1Z { get; set; } = 39;        // [20] 齿数
        public double Cycloid1Width { get; set; } = 40;    // [21] 设计齿宽 (mm)
        public double Cycloid1EModule { get; set; } = 206000; // [22] 弹性模量
        public double Cycloid1Eccentricity { get; set; } = 2; // [23] 偏心距 ee (mm)
        public double Cycloid1Kp { get; set; } = 1;        // [24] 使用系数
        public double Cycloid1Torque { get; set; } = 764.4;  // [25] 摆线轮传递总力矩 (N·m)
        public double Cycloid1LocDist { get; set; } = 29;  // [26] 定位面距基准面距离
        public double Cycloid1LocFace { get; set; } = 1;   // [27] 定位面
        public double Cycloid1MValueDia { get; set; } = 0; // [28] 量棒直径
        public double Cycloid1MValueTol { get; set; } = 0; // [29] 量棒公差
        public double Cycloid1PrecGrade { get; set; } = 5; // [30] 精度等级（默认5级，0级在f_p反查时易失败）

        // ── 摆线轮 2 (索引 40..50) ──
        public double Cycloid2Z { get; set; } = 39;
        public double Cycloid2Width { get; set; } = 40;
        public double Cycloid2EModule { get; set; } = 206000;
        public double Cycloid2Eccentricity { get; set; } = 2;
        public double Cycloid2Kp { get; set; } = 1;
        public double Cycloid2Torque { get; set; } = 764.4;
        public double Cycloid2LocDist { get; set; } = 53;
        public double Cycloid2LocFace { get; set; } = -1;
        public double Cycloid2MValueDia { get; set; } = 0;
        public double Cycloid2MValueTol { get; set; } = 0;
        // 注意：C++ ConvertInputDataToCycloidObj 仅对第一个摆线块（InputData[20+10]）调用 ObtainFourCycloidPara，
        // 索引 50 的精度等级不被 DLL 消费，仅作占位/未来扩展；实际生效的是 Cycloid1PrecGrade。
        public double Cycloid2PrecGrade { get; set; } = 5;

        /// <summary>
        /// 力矩单位转换系数：DLL 摆线接触应力公式期望 N·mm，而推荐/UI 以 N·m 传递，
        /// 默认 ×1000 换算（N·m→N·mm）；设 1 关闭。
        /// 该字段不进入 ToArray()（为预处理参数，非 DLL 输入）。
        /// </summary>
        public double TorqueScale { get; set; } = 1000.0;

        /// <summary>构建 60 元素输入数组</summary>
        public double[] ToArray()
        {
            var data = new double[60];
            // 针轮段 [0..7]
            data[0] = PinionZ; data[1] = RollerRadius; data[2] = PitchCircleRadius;
            data[3] = PinionEModule; data[4] = PinionWidth; data[5] = PinionKp;
            data[6] = PinionLocDist; data[7] = PinionLocFace;
            // 摆线轮 1 [20..30]
            data[20] = Cycloid1Z; data[21] = Cycloid1Width; data[22] = Cycloid1EModule;
            data[23] = Cycloid1Eccentricity; data[24] = Cycloid1Kp; data[25] = Cycloid1Torque;
            data[26] = Cycloid1LocDist; data[27] = Cycloid1LocFace;
            data[28] = Cycloid1MValueDia; data[29] = Cycloid1MValueTol; data[30] = Cycloid1PrecGrade;
            // 摆线轮 2 [40..50]
            data[40] = Cycloid2Z; data[41] = Cycloid2Width; data[42] = Cycloid2EModule;
            data[43] = Cycloid2Eccentricity; data[44] = Cycloid2Kp; data[45] = Cycloid2Torque;
            data[46] = Cycloid2LocDist; data[47] = Cycloid2LocFace;
            data[48] = Cycloid2MValueDia; data[49] = Cycloid2MValueTol; data[50] = Cycloid2PrecGrade;
            return data;
        }

        /// <summary>分割索引（5 元素）：[0]=总输入量60, [1]=输出量9, [2]=摆线轮数2, [3]=摆线轮起始20, [4]=单摆线轮数据量20</summary>
        public static int[] GetDivideIndex() => new[] { 60, 9, 2, 20, 20 };
    }
}
