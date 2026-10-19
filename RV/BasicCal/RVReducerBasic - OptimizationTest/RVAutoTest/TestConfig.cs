using RVAutoTest.Models;

namespace RVAutoTest
{
    /// <summary>
    /// RVAutoTest 外部化测试配置（从 test_params.json 反序列化）。
    /// 字段为可空类型（double?）：null 表示未配置，将使用默认值。
    /// 使用 double? 而非 0 哨兵，确保 0 是合法值（如 FixElemNotation=0 表示针齿壳固定）也能生效。
    /// </summary>
    public class TestConfig
    {
        /// <summary>齿轮推荐输入参数（null → 默认值）</summary>
        public GearRecommendConfig? GearRecommend { get; set; }

        /// <summary>轴推荐输入参数（null → 默认值）</summary>
        public ShaftRecommendConfig? ShaftRecommend { get; set; }

        /// <summary>摆线针轮副输入参数（null → 默认值/推荐映射）</summary>
        public CycloidGearConfig? CycloidGear { get; set; }

        /// <summary>渐开线齿轮副输入参数（null → 默认值/推荐映射）</summary>
        public InvGearConfig? InvGear { get; set; }
    }

    /// <summary>渐开线齿轮副配置（所有字段可空，null=未配置/推荐映射）</summary>
    public class InvGearConfig
    {
        public double? Torque { get; set; }          // 扭矩 N·m
        public double? Speed { get; set; }           // 转速 r/min
        public double? DesignLife { get; set; }      // 预期寿命 h
        public double? Module { get; set; }          // 法向模数 mm
        public double? PressureAngle { get; set; }   // 压力角（度）
        public double? HelixAngle { get; set; }      // 螺旋角（度）
        public double? CenterDistance { get; set; }  // 中心距 mm
        public double? PlanetCount { get; set; }     // 行星轮个数
        public double? SunTeeth { get; set; }        // 太阳轮齿数
        public double? PlanetTeeth { get; set; }     // 行星轮齿数
        public double? SunWidth { get; set; }        // 太阳轮设计齿宽 mm
        public double? PlanetWidth { get; set; }     // 行星轮设计齿宽 mm
    }

    /// <summary>摆线针轮副配置（所有字段可空，null=未配置/推荐映射）</summary>
    public class CycloidGearConfig
    {
        public double? PinionZ { get; set; }          // 针轮齿数
        public double? RollerRadius { get; set; }     // 滚针半径 mm
        public double? PitchCircleRadius { get; set; }// 针轮分度圆半径 mm
        public double? PinionEModule { get; set; }    // 针轮弹性模量 MPa
        public double? PinionWidth { get; set; }      // 针轮设计齿宽 mm
        public double? Cycloid1Width { get; set; }    // 摆线轮1设计齿宽 mm
        public double? Cycloid1Eccentricity { get; set; } // 摆线轮1偏心距 mm
        public double? Cycloid2Width { get; set; }    // 摆线轮2设计齿宽 mm
        public double? Cycloid2Eccentricity { get; set; } // 摆线轮2偏心距 mm
        public double? PrecGrade { get; set; }        // 摆线轮精度等级（0~12，默认5）
        public double? TorqueScale { get; set; }      // 力矩单位转换系数（N·m→N·mm，默认1000；设为1关闭）
    }

    /// <summary>齿轮推荐配置（所有字段可空，null=未配置）</summary>
    public class GearRecommendConfig
    {
        public double? FixElemNotation { get; set; }  // 固定部件: 0=针齿壳, 1=曲轴
        public double? TotalRatio { get; set; }       // 总传动比
        public double? OutputTorque { get; set; }     // 输出力矩 N·m
        public double? OutputRPM { get; set; }        // 输出转速 r/min
        public double? TargetStress { get; set; }     // 目标应力
        public double? DesignLife { get; set; }       // 设计寿命 h
        public double? UsageFactor { get; set; }      // 使用系数
        public double? AllowableStress { get; set; }  // 材料许用极限接触应力
        public double? PlanetCount { get; set; }      // 行星轮个数
        public double? CycloidMinWidth { get; set; }  // 摆线轮最小齿宽 mm
        public double? CycloidMaxWidth { get; set; }  // 摆线轮最大齿宽 mm
    }

    /// <summary>轴推荐配置（所有字段可空，null=未配置）</summary>
    public class ShaftRecommendConfig
    {
        public double? AllowableBendStress { get; set; }  // 许用弯曲正应力 MPa
        public double? AllowableShearStress { get; set; } // 许用剪应力 MPa
        public double? BearingDesignLife { get; set; }    // 轴承设计寿命 h
    }
}
