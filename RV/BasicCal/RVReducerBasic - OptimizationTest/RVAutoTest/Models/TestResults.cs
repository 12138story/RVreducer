using System;
using System.Collections.Generic;

namespace RVAutoTest.Models
{
    /// <summary>
    /// 齿轮推荐结果（完整字段，中文字段名与 UI 字典一致）。
    /// 字段映射参考 SetGearContentDrawer.UpdateRVSetParameters。
    /// </summary>
    public class GearRecommendResult
    {
        public string Scenario { get; set; } = "gear-recommend";
        public DateTime Timestamp { get; set; } = DateTime.Now;

        // 输入参数
        public GearRecommendInput Input { get; set; } = new();

        // ── 基础参数 (OutData[0..8]) ──
        public double 法向模数 { get; set; }             // OutData[0]
        public double 压力角 { get; set; }               // OutData[1]
        public double 螺旋角 { get; set; }               // OutData[2]
        public double 中心距 { get; set; }               // OutData[3]
        public double 齿轮副类型 { get; set; }           // OutData[4]
        public double 行星轮个数 { get; set; }           // OutData[5]
        public double 驱动齿轮 { get; set; }             // OutData[6]
        public double 触点间隙_j_bn { get; set; }        // OutData[7] 触点间隙[j_bn]
        public double 转速 { get; set; }                 // OutData[8]

        // ── 齿轮1 / 齿轮2 (OutData[9..70], 每齿轮31字段, 步长31) ──
        public Dictionary<string, double> 齿轮1 { get; set; } = new();
        public Dictionary<string, double> 齿轮2 { get; set; } = new();

        // ── 滚针 (OutData[71..76]) ──
        public Dictionary<string, double> 滚针 { get; set; } = new();

        // ── 摆线轮1 / 摆线轮2 (OutData[77..88], 每轮6字段, 步长6) ──
        public Dictionary<string, double> 摆线轮1 { get; set; } = new();
        public Dictionary<string, double> 摆线轮2 { get; set; } = new();

        // ── 其他 (OutData[89..90]) ──
        public double 扭矩 { get; set; }                 // OutData[89]
        public double 最终转速 { get; set; }             // OutData[90]

        public List<string> Screenshots { get; set; } = new();
    }

    /// <summary>
    /// 齿轮推荐输出字段字典键（与 UI 字典 mDataStorageDict 保持一致，含特殊字符）。
    /// </summary>
    public static class GearOutputFieldNames
    {
        // 齿轮字段（31 个，顺序与 UpdateRVSetParameters 一致）
        public static readonly string[] GearFields = new[]
        {
            "齿数", "旋向", "设计齿宽", "齿顶高系数", "齿廓变位系数", "顶隙系数",
            "齿根圆角系数", "挖根量", "齿顶修形量", "齿根修形量", "是否使用滚动角",
            "跨齿数", "齿顶倒角量", "轮幅外圆直径", "齿轮齿顶高变化[k·mn]",
            "齿厚的偏差上限[E_sns]", "齿厚的偏差下限[E_sni]", "齿顶圆容差上限",
            "齿顶圆容差下限", "最大机械加工余量[q_max]", "最小机械加工余量[q_min]",
            "Y圆直径[d_y]", "刀具类型", "齿条刀的齿顶高系数", "齿条刀的齿顶圆半径系数",
            "插齿刀齿数", "插齿刀加工中心距", "插齿刀具齿顶直径", "插齿刀基圆直径",
            "插齿刀齿顶形状直径", "插齿刀具参数横向压力角[alpha_wt0]"
        };

        // 滚针字段（6 个）
        public static readonly string[] CycloidGearFields = new[]
        {
            "齿数", "滚针半径", "设计齿宽", "针轮分度圆半径", "弹性模量", "使用系数"
        };

        // 摆线轮字段（6 个）
        public static readonly string[] CycloidPinionFields = new[]
        {
            "齿数", "偏心距", "设计齿宽", "弹性模量", "使用系数", "摆线轮传递总力矩"
        };
    }

    /// <summary>齿轮推荐输入</summary>
    public class GearRecommendInput
    {
        public double FixElemNotation { get; set; }  // 固定部件 (0=针齿壳, 1=曲轴)
        public double TotalRatio { get; set; }       // 总传动比
        public double OutputTorque { get; set; }     // 输出力矩 N·m
        public double OutputRPM { get; set; }        // 输出转速 r/min
        public double TargetStress { get; set; }     // 目标应力
        public double DesignLife { get; set; }       // 设计寿命 h
        public double UsageFactor { get; set; }      // 使用系数
        public double AllowableStress { get; set; }  // 材料许用极限接触应力
        public double PlanetCount { get; set; }      // 行星轮个数
        public double CycloidMinWidth { get; set; }  // 摆线轮最小齿宽
        public double CycloidMaxWidth { get; set; }  // 摆线轮最大齿宽
    }

    /// <summary>轴推荐结果</summary>
    public class ShaftRecommendResult
    {
        public string Scenario { get; set; } = "shaft-recommend";
        public DateTime Timestamp { get; set; } = DateTime.Now;

        public ShaftRecommendInput Input { get; set; } = new();

        public double RecommendedDiameter { get; set; }  // 推荐轴直径 mm
        public double RecommendedLength { get; set; }    // 推荐轴长度 mm
        public double CrankShaftCount { get; set; }      // 曲轴数目

        /// <summary>UI 操作/解析失败时的根因描述（成功时为空）</summary>
        public string? ErrorMessage { get; set; }

        public List<string> Screenshots { get; set; } = new();
    }

    /// <summary>轴推荐输入</summary>
    public class ShaftRecommendInput
    {
        public double AllowableBendStress { get; set; }  // 许用弯曲正应力 MPa
        public double AllowableShearStress { get; set; } // 许用剪应力 MPa
        public double BearingDesignLife { get; set; }    // 轴承设计寿命 h
    }

    /// <summary>圆锥滚子轴承寿命结果</summary>
    public class BearingLifeResult
    {
        public string Scenario { get; set; } = "tapered-bearing-life";
        public DateTime Timestamp { get; set; } = DateTime.Now;

        public double[] Input { get; set; } = new double[37];   // InData
        public double[] Output { get; set; } = new double[20];  // OutData

        // 便捷属性
        public double StatusCode => Output[0];        // 1.0=成功
        public double C0 => Output[1];                // 基本额定静载 N
        public double P0 => Output[3];                // 当量静载荷 N
        public double S0 => Output[4];                // 静态安全系数
        public double C => Output[5];                 // 基本额定动载 N
        public double X => Output[6];                 // 动载系数 X
        public double Y => Output[7];                 // 动载系数 Y
        public double P => Output[8];                 // 当量动载荷 N
        public double L10r => Output[9];              // 基本寿命 10^6 转
        public double L10h => Output[10];             // 基本寿命 小时
        public double Kappa => Output[11];            // 黏度比
        public double Ec => Output[12];               // 污染系数
        public double A1 => Output[13];               // 可靠度系数
        public double AIso => Output[14];             // 系统修正系数
        public double Lnm => Output[15];              // 修正寿命 10^6 转

        public string UiDisplayedLife { get; set; } = "";  // UI 上显示的寿命文本
        public double UiL10hDeviationPct { get; set; } = 0; // UI 与 DLL 结果偏差

        public List<string> Screenshots { get; set; } = new();
    }

    /// <summary>汇总报告</summary>
    public class Report
    {
        public string ToolVersion { get; set; } = "1.0.0";
        public DateTime GeneratedAt { get; set; } = DateTime.Now;

        /// <summary>整体是否成功（任一场景失败则为 false）</summary>
        public bool Success { get; set; } = true;

        /// <summary>收集的错误信息</summary>
        public List<string> Errors { get; set; } = new();

        public GearRecommendResult? GearRecommend { get; set; }
        public ShaftRecommendResult? ShaftRecommend { get; set; }
        public InvGearResult? InvGear { get; set; }
        public CycloidGearResult? CycloidGear { get; set; }
        public BearingLifeResult? BearingLife { get; set; }
    }
}
