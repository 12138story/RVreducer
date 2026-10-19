using System;
using RVAutoTest.Models;

namespace RVAutoTest
{
    /// <summary>
    /// 渐开线齿轮副输入（174 字段），字段布局参考 CTANInvGearPair.Convert*Input 方法。
    /// 分割索引（34 元素）参考 CTANInvGearPair.mDivisionIndexList。
    /// 默认值：几何参考 CTANInvGearPair.TestInput（ZJ 31 字段格式），
    /// 材料/加工/应用/公差/载荷参考 MAIN_TAN.cpp（TestPHData/TestMNData/TestAppData 等）。
    /// </summary>
    public class InvGearInput
    {
        // ── 分段常量（与 mDivisionIndexList 一致）──
        public const int TotalInput = 174;
        public const int GeometryPerGear = 31;    // 单齿轮几何字段数
        public const int CommonGeometryCount = 9; // 公共几何字段数
        public const int MaterialCount = 8;       // 单齿轮材料字段数
        public const int ManufactureCount = 8;    // 单齿轮加工字段数（前 6 有效）
        public const int AppCount = 8;            // 单齿轮/公共应用字段数
        public const int ToleranceCount = 6;      // 单齿轮公差字段数
        public const int LoadCount = 3;           // 载荷字段数
        public const int AssembleCount = 6;       // 装配字段数
        public const int LocCount = 2;            // 单齿轮定位字段数

        // ── 几何 [0..92]：太阳轮31 + 行星轮31 + 公共9 ──
        // 31 字段顺序与 GearOutputFieldNames.GearFields 一致
        // （齿数,旋向,设计齿宽,齿顶高系数,齿廓变位系数,顶隙系数,齿根圆角系数,挖根量,齿顶修形量,
        //   齿根修形量,是否使用滚动角,跨齿数,齿顶倒角量,轮幅外圆直径,齿轮齿顶高变化[k·mn],
        //   齿厚的偏差上限[E_sns],齿厚的偏差下限[E_sni],齿顶圆容差上限,齿顶圆容差下限,
        //   最大机械加工余量[q_max],最小机械加工余量[q_min],Y圆直径[d_y],刀具类型,
        //   齿条刀的齿顶高系数,齿条刀的齿顶圆半径系数,插齿刀齿数,插齿刀加工中心距,
        //   插齿刀具齿顶直径,插齿刀基圆直径,插齿刀齿顶形状直径,插齿刀具参数横向压力角[alpha_wt0]）
        public double[] 太阳轮几何 { get; set; } = new double[GeometryPerGear];   // [0..30]
        public double[] 行星轮几何 { get; set; } = new double[GeometryPerGear];   // [31..61]
        /// <summary>公共几何 [62..70]：法向模数,压力角(度),螺旋角(度),中心距,齿轮副类型,行星轮个数,驱动齿轮,触点间隙,转速</summary>
        public double[] 公共几何 { get; set; } = new double[CommonGeometryCount]; // [62..70]

        // ── 材料+加工 [93..124] ──
        /// <summary>太阳轮材料 [93..100]：弹性模量,泊松比,屈服强度,密度,试验齿轮接触疲劳极限应力,试验齿轮弯曲疲劳极限应力,屈服强度,抗拉强度</summary>
        public double[] 太阳轮材料 { get; set; } = new double[MaterialCount];     // [93..100]
        /// <summary>太阳轮加工 [101..108]：材料类型,加工质量,表面硬度(HRC),芯部硬度(HRC),表面是否喷丸,是否允许表面出现有限点蚀</summary>
        public double[] 太阳轮加工 { get; set; } = new double[ManufactureCount];  // [101..108]
        public double[] 行星轮材料 { get; set; } = new double[MaterialCount];     // [109..116]
        public double[] 行星轮加工 { get; set; } = new double[ManufactureCount];  // [117..124]

        // ── 应用 [125..148] ──
        /// <summary>太阳轮应用 [125..132]：轮齿是否反向弯折,是否对接触区进行控制,螺旋线修形方式,齿廓修形是否能够补偿变形,行星轮个数,齿坯类型,接触最小安全系数,弯曲最小安全系数</summary>
        public double[] 太阳轮应用 { get; set; } = new double[AppCount];          // [125..132]
        public double[] 行星轮应用 { get; set; } = new double[AppCount];          // [133..140]
        /// <summary>公共应用 [141..148]：使用系数,均载系数,轴承间距偏移量,润滑油粘度(v40),修形方法,接触最小安全系数,弯曲最小安全系数</summary>
        public double[] 公共应用 { get; set; } = new double[AppCount];            // [141..148]

        // ── 公差 [149..160] ──
        /// <summary>太阳轮公差 [149..154]：精度等级,端面节线偏差,齿廓偏差,螺旋线偏差,接触表面粗糙度(Ra),齿根表面粗糙度(Ra)</summary>
        public double[] 太阳轮公差 { get; set; } = new double[ToleranceCount];    // [149..154]
        public double[] 行星轮公差 { get; set; } = new double[ToleranceCount];    // [155..160]

        // ── 载荷 [161..163]：扭矩, 转速, 预期寿命 ──
        public double[] 载荷 { get; set; } = new double[LoadCount];               // [161..163]

        // ── 装配 [164..169]：安装类型,是否为刚性轴,行星轮个数,太阳轮轴直径,太阳轮齿宽中点至轴承跨距中点距离,轴承跨距 ──
        public double[] 装配 { get; set; } = new double[AssembleCount];           // [164..169]

        // ── 定位 [170..173]：定位面距基准面距离, 定位面 ──
        public double[] 太阳轮定位 { get; set; } = new double[LocCount];          // [170..171]
        public double[] 行星轮定位 { get; set; } = new double[LocCount];          // [172..173]

        // ── 便捷属性（载荷）──
        public double 扭矩 { get => 载荷[0]; set => 载荷[0] = value; }
        /// <summary>转速（r/min），同步写入载荷段与公共几何[70]</summary>
        public double 转速 { get => 载荷[1]; set { 载荷[1] = value; 公共几何[8] = value; } }
        public double 预期寿命 { get => 载荷[2]; set => 载荷[2] = value; }

        // ── 便捷属性（公共几何）──
        public double 法向模数 { get => 公共几何[0]; set => 公共几何[0] = value; }
        /// <summary>压力角（度），ToArray 时转为弧度</summary>
        public double 压力角度 { get => 公共几何[1]; set => 公共几何[1] = value; }
        /// <summary>螺旋角（度），ToArray 时转为弧度</summary>
        public double 螺旋角度 { get => 公共几何[2]; set => 公共几何[2] = value; }
        public double 中心距 { get => 公共几何[3]; set => 公共几何[3] = value; }
        public double 齿轮副类型 { get => 公共几何[4]; set => 公共几何[4] = value; }
        /// <summary>行星轮个数，同步写入公共几何、两齿轮应用段与装配段（UI 中为公共绑定值）</summary>
        public double 行星轮个数 { get => 公共几何[5]; set { 公共几何[5] = value; 太阳轮应用[4] = value; 行星轮应用[4] = value; 装配[2] = value; } }
        public double 驱动齿轮 { get => 公共几何[6]; set => 公共几何[6] = value; }
        public double 触点间隙j_bn { get => 公共几何[7]; set => 公共几何[7] = value; }

        public InvGearInput()
        {
            SetDefaults();
        }

        /// <summary>
        /// 设置默认值。
        /// 几何参考 CTANInvGearPair.TestInput（ZJ 31 字段格式）；
        /// 材料/加工/应用/公差/载荷参考 MAIN_TAN.cpp 测试数据。
        /// </summary>
        public void SetDefaults()
        {
            // ── 太阳轮几何 [0..30]（与 GearOutputFieldNames.GearFields 顺序一致）──
            double[] sun = 太阳轮几何;
            sun[0] = 23;        // 齿数
            sun[1] = -1;        // 旋向
            sun[2] = 20;        // 设计齿宽 (mm)
            sun[3] = 1;         // 齿顶高系数
            sun[4] = 0.5142;    // 齿廓变位系数
            sun[5] = 0.25;      // 顶隙系数
            sun[6] = 0.384;     // 齿根圆角系数
            sun[7] = 0;         // 挖根量
            sun[8] = 0;         // 齿顶修形量
            sun[9] = 0;         // 齿根修形量
            sun[10] = 0;        // 是否使用滚动角 (0=不使用, 1=使用)
            sun[11] = 2;        // 跨齿数
            sun[12] = 0;        // 齿顶倒角量
            sun[13] = 0;        // 轮幅外圆直径
            sun[14] = 0;        // 齿轮齿顶高变化[k·mn]
            sun[15] = -0.0670;  // 齿厚的偏差上限[E_sns]
            sun[16] = -0.0926;  // 齿厚的偏差下限[E_sni]
            sun[17] = 0;        // 齿顶圆容差上限
            sun[18] = 0;        // 齿顶圆容差下限
            sun[19] = 0;        // 最大机械加工余量[q_max]
            sun[20] = 0;        // 最小机械加工余量[q_min]
            sun[21] = 25;       // Y圆直径[d_y]
            sun[22] = 0;        // 刀具类型 (0=RackCutter, 1=ShaperCutter)
            sun[23] = 1.4;      // 齿条刀的齿顶高系数
            sun[24] = 0.39;     // 齿条刀的齿顶圆半径系数
            sun[25] = 0;        // 插齿刀齿数
            sun[26] = 0;        // 插齿刀加工中心距
            sun[27] = 0;        // 插齿刀具齿顶直径
            sun[28] = 0;        // 插齿刀基圆直径
            sun[29] = 0;        // 插齿刀齿顶形状直径
            sun[30] = 0;        // 插齿刀具参数横向压力角[alpha_wt0]

            // ── 行星轮几何 [0..30] ──
            double[] planet = 行星轮几何;
            planet[0] = 106;    // 齿数
            planet[1] = 1;      // 旋向（与太阳轮相反）
            planet[2] = 20;     // 设计齿宽 (mm)
            planet[3] = 1;      // 齿顶高系数
            planet[4] = 0;      // 齿廓变位系数
            planet[5] = 0.25;   // 顶隙系数
            planet[6] = 0.384;  // 齿根圆角系数
            planet[7] = 0;      // 挖根量
            planet[8] = 0;      // 齿顶修形量
            planet[9] = 0;      // 齿根修形量
            planet[10] = 0;     // 是否使用滚动角
            planet[11] = 4;     // 跨齿数
            planet[12] = 0;     // 齿顶倒角量
            planet[13] = 0;     // 轮幅外圆直径
            planet[14] = 0;     // 齿轮齿顶高变化[k·mn]
            planet[15] = -0.0617; // 齿厚的偏差上限[E_sns]
            planet[16] = -0.0947; // 齿厚的偏差下限[E_sni]
            planet[17] = 0;     // 齿顶圆容差上限
            planet[18] = 0;     // 齿顶圆容差下限
            planet[19] = 0;     // 最大机械加工余量[q_max]
            planet[20] = 0;     // 最小机械加工余量[q_min]
            planet[21] = 67;    // Y圆直径[d_y]
            planet[22] = 0;     // 刀具类型
            planet[23] = 1.4;   // 齿条刀的齿顶高系数
            planet[24] = 0.39;  // 齿条刀的齿顶圆半径系数
            planet[25] = 0;     // 插齿刀齿数
            planet[26] = 0;     // 插齿刀加工中心距
            planet[27] = 0;     // 插齿刀具齿顶直径
            planet[28] = 0;     // 插齿刀基圆直径
            planet[29] = 0;     // 插齿刀齿顶形状直径
            planet[30] = 0;     // 插齿刀具参数横向压力角[alpha_wt0]

            // ── 公共几何 [62..70] ──
            法向模数 = 2;
            压力角度 = 20;      // 度（ToArray 时转弧度）
            螺旋角度 = 0;       // 度（ToArray 时转弧度）
            中心距 = 130;
            齿轮副类型 = 1;     // 一般外啮合齿轮副
            行星轮个数 = 3;     // 同步写 公共几何[5]/应用[4]×2/装配[2]
            驱动齿轮 = 0;       // 0=太阳轮(小齿轮) 驱动, 1=行星轮(大齿轮)
            触点间隙j_bn = 0;
            // 公共几何[8]（转速）由 转速 属性写入

            // ── 材料+加工 [93..124]（参考 MAIN_TAN.cpp TestPHData/TestMNData）──
            double[] sunMat = 太阳轮材料;
            sunMat[0] = 206000;    // 弹性模量 (MPa)
            sunMat[1] = 0.3;       // 泊松比
            sunMat[2] = 500;       // 屈服强度 (MPa)
            sunMat[3] = 7.83e-6;   // 密度 (kg/mm³)
            sunMat[4] = 1500;      // 试验齿轮接触疲劳极限应力 (MPa)
            sunMat[5] = 500;       // 试验齿轮弯曲疲劳极限应力 (MPa)
            sunMat[6] = 500;       // = 屈服强度（与 DLL 输入一致）
            sunMat[7] = 1080;      // 抗拉强度 (MPa)

            double[] sunMfg = 太阳轮加工;
            sunMfg[0] = 9;         // 材料类型（DLL 内部 +1 → 10）
            sunMfg[1] = 1;         // 加工质量
            sunMfg[2] = 60;        // 表面硬度(HRC)
            sunMfg[3] = 30;        // 芯部硬度(HRC)
            sunMfg[4] = 0;         // 表面是否喷丸
            sunMfg[5] = 0;         // 是否允许表面出现有限点蚀
            // [6..7] 未使用，保持 0

            行星轮材料 = (double[])sunMat.Clone();
            行星轮加工 = (double[])sunMfg.Clone();

            // ── 应用 [125..148]（参考 MAIN_TAN.cpp TestAppData/TestAppCommonData）──
            double[] sunApp = 太阳轮应用;
            sunApp[0] = 0;         // 轮齿是否反向弯折
            sunApp[1] = 1;         // 是否对接触区进行控制
            sunApp[2] = 1;         // 螺旋线修形方式
            sunApp[3] = 0;         // 齿廓修形是否能够补偿变形
            sunApp[4] = 3;         // 行星轮个数（与 公共几何[5] 一致）
            sunApp[5] = 1;         // 齿坯类型
            sunApp[6] = 1;         // 接触最小安全系数
            sunApp[7] = 1;         // 弯曲最小安全系数
            行星轮应用 = (double[])sunApp.Clone();

            double[] commonApp = 公共应用;
            commonApp[0] = 1.25;   // 使用系数
            commonApp[1] = 1.0;    // 均载系数
            commonApp[2] = 0.0;    // 轴承间距偏移量
            commonApp[3] = 320;    // 润滑油粘度(v40)
            commonApp[4] = 1.0;    // 修形方法
            commonApp[5] = 1.0;    // 接触最小安全系数
            commonApp[6] = 1.0;    // 弯曲最小安全系数
            // [7] 未使用，保持 0

            // ── 公差 [149..160]（参考 MAIN_TAN.cpp TestAcdataPinion/TestAcdataGear）──
            double[] sunTol = 太阳轮公差;
            sunTol[0] = 5;         // 精度等级
            sunTol[1] = 5.5;       // 端面节线偏差 (μm)
            sunTol[2] = 6.0;       // 齿廓偏差 (μm)
            sunTol[3] = 5.5;       // 螺旋线偏差 (μm)
            sunTol[4] = 1.0;       // 接触表面粗糙度(Ra) (μm)
            sunTol[5] = 3.0;       // 齿根表面粗糙度(Ra) (μm)

            double[] planetTol = 行星轮公差;
            planetTol[0] = 5;      // 精度等级
            planetTol[1] = 6.0;    // 端面节线偏差 (μm)
            planetTol[2] = 6.0;    // 齿廓偏差 (μm)
            planetTol[3] = 5.5;    // 螺旋线偏差 (μm)
            planetTol[4] = 1.0;    // 接触表面粗糙度(Ra) (μm)
            planetTol[5] = 3.0;    // 齿根表面粗糙度(Ra) (μm)

            // ── 载荷 [161..163]（参考 MAIN_TAN.cpp DefineLoadPara）──
            扭矩 = -2295;          // 扭矩 (N·m)
            转速 = 5.1241830065359473; // 转速 (r/min)，同时写入公共几何[70]
            预期寿命 = 50000;      // 预期寿命 (h)

            // ── 装配 [164..169]（参考 CTANInvGearPair.TestInput）──
            double[] asm = 装配;
            asm[0] = 5;            // 安装类型
            asm[1] = 0;            // 是否为刚性轴
            asm[2] = 3;            // 行星轮个数
            asm[3] = 17;           // 太阳轮轴直径 (mm)
            asm[4] = 0;            // 太阳轮齿宽中点至轴承跨距中点距离 (mm)
            asm[5] = 34;           // 轴承跨距 (mm)

            // ── 定位 [170..173]（参考 CTANInvGearPair.TestInput）──
            太阳轮定位[0] = 9;     // 定位面距基准面距离
            太阳轮定位[1] = -1;    // 定位面
            行星轮定位[0] = 5.45;  // 定位面距基准面距离
            行星轮定位[1] = 1;     // 定位面
        }

        /// <summary>
        /// 从齿轮推荐输出（RecommondPara OutData）映射几何段（数据闭环）。
        /// 太阳轮 31 字段 ← OutData[9..39]；行星轮 31 字段 ← OutData[40..70]；
        /// 公共 9 字段 ← OutData[0..8]（压力角/螺旋角为角度，ToArray 时转弧度）。
        /// </summary>
        /// <param name="outData">齿轮推荐输出数组</param>
        /// <exception cref="ArgumentException">输出数组长度不足 71 时抛出</exception>
        public void MapFromRecommend(double[] outData)
        {
            if (outData is null || outData.Length < 71)
                throw new ArgumentException("齿轮推荐输出数组长度不足 71，无法映射渐开线几何段");

            // 齿轮1（太阳轮）: OutData[9..39]
            for (int i = 0; i < GeometryPerGear; i++)
                太阳轮几何[i] = outData[9 + i];
            // 齿轮2（行星轮）: OutData[40..70]
            for (int i = 0; i < GeometryPerGear; i++)
                行星轮几何[i] = outData[40 + i];

            // 公共 9 字段 [62..70] ← OutData[0..8]
            公共几何[0] = outData[0]; // 法向模数
            公共几何[1] = outData[1]; // 压力角（度）
            公共几何[2] = outData[2]; // 螺旋角（度）
            公共几何[3] = outData[3]; // 中心距
            公共几何[4] = outData[4]; // 齿轮副类型
            公共几何[5] = outData[5]; // 行星轮个数
            公共几何[6] = outData[6]; // 驱动齿轮
            公共几何[7] = outData[7]; // 触点间隙[j_bn]
            公共几何[8] = outData[8]; // 转速

            // 同步行星轮个数（公共几何 → 应用/装配）
            行星轮个数 = 公共几何[5];
            // 同步转速（公共几何 → 载荷段）
            if (outData[8] != 0)
                转速 = outData[8];

            // 载荷扭矩可选映射（OutData[89]=扭矩，仅当推荐值非零时覆盖默认，避免零值清空）
            if (outData.Length > 90 && outData[89] != 0)
                扭矩 = outData[89];
        }

        /// <summary>构建 174 元素输入数组（压力角/螺旋角自动转弧度）</summary>
        public double[] ToArray()
        {
            var data = new double[TotalInput];

            // ── 几何 [0..92] ──
            Array.Copy(太阳轮几何, 0, data, 0, GeometryPerGear);
            Array.Copy(行星轮几何, 0, data, GeometryPerGear, GeometryPerGear);
            Array.Copy(公共几何, 0, data, GeometryPerGear * 2, CommonGeometryCount);
            // 压力角/螺旋角（度 → 弧度）
            data[63] = AngleUtil.ToRad(data[63]);
            data[64] = AngleUtil.ToRad(data[64]);

            // ── 材料+加工 [93..124] ──
            Array.Copy(太阳轮材料, 0, data, 93, MaterialCount);
            Array.Copy(太阳轮加工, 0, data, 101, ManufactureCount);
            Array.Copy(行星轮材料, 0, data, 109, MaterialCount);
            Array.Copy(行星轮加工, 0, data, 117, ManufactureCount);

            // ── 应用 [125..148] ──
            Array.Copy(太阳轮应用, 0, data, 125, AppCount);
            Array.Copy(行星轮应用, 0, data, 133, AppCount);
            Array.Copy(公共应用, 0, data, 141, AppCount);

            // ── 公差 [149..160] ──
            Array.Copy(太阳轮公差, 0, data, 149, ToleranceCount);
            Array.Copy(行星轮公差, 0, data, 155, ToleranceCount);

            // ── 载荷 [161..163] ──
            Array.Copy(载荷, 0, data, 161, LoadCount);

            // ── 装配 [164..169] ──
            Array.Copy(装配, 0, data, 164, AssembleCount);

            // ── 定位 [170..173] ──
            Array.Copy(太阳轮定位, 0, data, 170, LocCount);
            Array.Copy(行星轮定位, 0, data, 172, LocCount);

            return data;
        }

        /// <summary>
        /// 分割索引（34 元素），布局与运行时修正后的 CTANInvGearPair.mDivisionIndexList 一致：
        /// [0]=174 总输入, [1]=284 总输出；
        /// 每子系统 4 值 {列数, 行数, 数据量, 累计末端索引}。
        ///
        /// ⚠ 注意 index 29：C++ DLL 用 InDivideIndex[1+4*7]=InDivideIndex[29] 作为输出列数（142），
        /// 而 C# 构造函数会把 index 29 从静态值 174（定位累计末端）覆盖为 mOutPutHeadName.Count()/2=142。
        /// 因此此处 index 29 必须为 142，否则 DLL 按 i*174+j 写输出，第二齿轮块越界错乱。
        /// </summary>
        public static int[] GetDivideIndex() => new[]
        {
            174, 284,                       // 总输入, 总输出
            31, 3, 93, 93,                  // 几何: 31列×3行=93, 起始0
            8, 4, 32, 125,                  // 材料+加工: 8×4=32, 起始93
            8, 3, 24, 149,                  // 应用: 8×3=24, 起始125
            6, 2, 12, 161,                  // 公差: 6×2=12, 起始149
            3, 1, 3, 164,                   // 载荷: 3×1=3, 起始161
            6, 1, 6, 170,                   // 装配: 6×1=6, 起始164
            2, 2, 4, 142,                   // 定位: 2×2=4, 起始170；index 29 = 输出列数 142（C++ 约定）
            142, 2, 284, 284                // 输出: 142列×2行=284
        };
    }
}
