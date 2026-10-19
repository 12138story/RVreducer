using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using RVAutoTest.Models;
using RVAutoTest.Scenarios;

namespace RVAutoTest
{
    /// <summary>
    /// ISO/TR 6336-30:2022 Example 1 渐开线齿轮副验证测试。
    /// 输入参数取自 Annex A（Table 3），期望结果取自 Table 4。
    /// 通过 CalInvGearStress2 计算并与 ISO 标准值对比，输出误差报告。
    /// 用法：--iso-example1
    /// </summary>
    public static class IsoExample1Test
    {
        /// <summary>ISO 期望输出（Table 4）</summary>
        private static class Expected
        {
            public static readonly double SigmaH = 1299;          // 接触应力 N/mm²
            public static readonly double SigmaH0 = 1207;         // 名义接触应力
            public static readonly double[] SigmaHP = { 1338, 1415 }; // 许用接触应力 小/大轮
            public static readonly double[] SigmaF = { 395, 382 };    // 齿根应力
            public static readonly double[] SigmaF0 = { 350, 339 };   // 名义齿根应力
            public static readonly double[] SigmaFP = { 824, 860 };   // 许用齿根应力
            public static readonly double[] SF = { 2.09, 2.25 };      // 齿根安全系数
            public static readonly double[] SH = { 1.03, 1.09 };      // 点蚀安全系数
            public static readonly double[] D = { 141.34, 856.35 };   // 参考直径
            public static readonly double[] Da = { 159.66, 872.35 };  // 齿顶直径
            public static readonly double[] Db = { 132.20, 800.97 };  // 基圆直径
            public static readonly double[] Dw = { 141.67, 858.33 };  // 工作节圆直径
            public static readonly double[] XE = { 0.1178, -0.0275 }; // 产形变位系数
            public static readonly double[] Df = { 120.82, 833.52 };  // 齿根直径
            public static readonly double[] DFf = { 132.25, 839.08 }; // 齿根成形圆直径
            public static readonly double[] DNf = { 132.92, 845.23 }; // 有效齿根直径
            public static readonly double[] Wk = { 38.196, 307.943 };// 跨棒距
            public static readonly double Vw = 2.67;                   // 节线速度 m/s
            public static readonly double Ft = 127352;                // 名义切向载荷 N
            public static readonly double Kv = 1.00;                  // 动载系数
            public static readonly double[] EpsAlpha = { 1.55, 1.55 };// 端面重合度
            public static readonly double EpsBeta = 1.08;             // 纵向重合度
            public static readonly double EpsGamma = 2.63;            // 总重合度
            // ── Annex A.6 分步计算中间量（参考项，不计入最大偏差）──
            public static readonly double U = 6.05882;                // 齿数比 u
            public static readonly double Mt = 8.31412;               // 端面模数 mm
            public static readonly double AlphaT = 20.720;            // 端面压力角 °
            public static readonly double AlphaWt = 21.066;           // 端面啮合角 °
            public static readonly double BetaB = 14.825;             // 基圆螺旋角 °
            public static readonly double InvAlphaT = 0.01663;        // 渐开线函数 invαt
            public static readonly double Pn = 25.133;                // 法向节距 mm
            public static readonly double Pt = 26.120;                // 分度圆节距 mm
            public static readonly double Pbt = 24.430;               // 基圆节距 mm
            public static readonly double Pet = 24.430;               // 端面啮合齿距 mm
            public static readonly double GAlpha = 37.845;            // 接触路径长度 mm
        }

        public static int Run(string[] args)
        {
            var workspaceRoot = Program.FindWorkspaceRootForVerify();
            // 每次运行生成时间戳子文件夹：screenshots/iso-example1/<yyyyMMdd-HHmmss>/（同秒重跑自动加序号）
            var outputDir = Program.CreateTimestampedDir(workspaceRoot, "screenshots", "iso-example1");
            Console.WriteLine($"📁 输出目录: {outputDir}");

            Console.WriteLine("========================================");
            Console.WriteLine("  ISO/TR 6336-30:2022 Example 1 渐开线验证");
            Console.WriteLine("  单斜齿渗碳淬火齿轮副 (z=17/103, mn=8, β=15.8°)");
            Console.WriteLine("========================================");

            // 读取参数文件（可选，默认 iso_example1_params.json）；缺失时回退硬编码默认值
            var paramsPath = args.Length >= 2 ? args[1]
                : Path.Combine(Path.GetDirectoryName(AppContext.BaseDirectory) ?? ".", "iso_example1_params.json");
            var isoParams = ConfigLoader.LoadIsoExample1(paramsPath);
            Console.WriteLine(isoParams is null
                ? "📄 未找到 ISO 参数文件，使用硬编码默认值（ISO Annex A Table 3）"
                : $"📄 使用 ISO 参数文件: {paramsPath}");

            // ── 1. 构造 ISO Example 1 输入 ──
            var input = new InvGearInput();
            ApplyIsoParams(input, isoParams);

            // 打印输入摘要
            Console.WriteLine($"\n📥 输入参数:");
            Console.WriteLine($"  齿数 z = {input.太阳轮几何[0]:F0}/{input.行星轮几何[0]:F0}");
            Console.WriteLine($"  法向模数 mn = {input.法向模数}");
            Console.WriteLine($"  压力角 αn = {input.压力角度}°, 螺旋角 β = {input.螺旋角度}°");
            Console.WriteLine($"  中心距 a = {input.中心距}");
            Console.WriteLine($"  变位系数 x = {input.太阳轮几何[4]:F4}/{input.行星轮几何[4]:F4}");
            Console.WriteLine($"  齿宽 b = {input.太阳轮几何[2]:F0}/{input.行星轮几何[2]:F0}");
            Console.WriteLine($"  扭矩 T = {input.扭矩} N·m, 转速 n = {input.转速}, 寿命 = {input.预期寿命} h");

            // ── 2. 调用 DLL 计算 ──
            Console.WriteLine("\n📊 调用 CalInvGearStress2 计算...");
            double[] outData;
            try
            {
                outData = DllInvoker.CallCalInvGearStress2(InvGearInput.GetDivideIndex(), input.ToArray());
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ DLL 调用失败: {ex.Message}");
                return 1;
            }

            const int fieldsPerGear = 142;
            if (outData.Length < fieldsPerGear * 2 || InvGearOutputFieldNames.Fields.Length < fieldsPerGear)
            {
                Console.WriteLine($"❌ 输出/字段表长度异常: out={outData.Length}, fields={InvGearOutputFieldNames.Fields.Length}（期望 284/{fieldsPerGear}）");
                return 1;
            }

            // ── 3. 解析为两齿轮字典 ──
            var pinion = new Dictionary<string, double>();  // 小轮(太阳轮)
            var wheel = new Dictionary<string, double>();   // 大轮(行星轮)
            for (int j = 0; j < fieldsPerGear; j++)
            {
                var name = InvGearOutputFieldNames.Fields[j];
                pinion[name] = outData[j];
                wheel[name] = outData[fieldsPerGear + j];
            }

            // ── 4. 对比 ISO 期望值（收集表格行）──
            var rows = new List<CompareRow>();
            double maxErr = 0;

            void Compare(string label, double computed, double iso, string? unit = null,
                bool isReference = false, string category = "其他")
            {
                // NaN/Infinity/除零防护
                bool valid = !double.IsNaN(computed) && !double.IsNaN(iso) && !double.IsInfinity(computed) && iso != 0;
                double err = valid ? Math.Abs(computed - iso) / Math.Abs(iso) * 100 : double.NaN;
                // 参考项（绝对值小的诊断值 / Annex A.6 分步中间量）不计入最大偏差，避免小基准主导结论
                if (!double.IsNaN(err) && !isReference) maxErr = Math.Max(maxErr, err);
                rows.Add(new CompareRow(category, label, computed, iso, unit ?? "", err, isReference));
            }

            // 应力（小轮/大轮）
            Compare("σH 计算接触应力", pinion["计算接触应力"], Expected.SigmaH, "N/mm²", category: "接触应力");
            Compare("σH0 名义接触应力", pinion["名义接触应力"], Expected.SigmaH0, "N/mm²", category: "接触应力");
            Compare("σHP 许用接触应力(小轮)", pinion["许用接触应力"], Expected.SigmaHP[0], "N/mm²", category: "接触应力");
            Compare("σHP 许用接触应力(大轮)", wheel["许用接触应力"], Expected.SigmaHP[1], "N/mm²", category: "接触应力");

            Compare("σF 齿根应力(小轮)", pinion["计算弯曲应力"], Expected.SigmaF[0], "N/mm²", category: "齿根弯曲应力");
            Compare("σF 齿根应力(大轮)", wheel["计算弯曲应力"], Expected.SigmaF[1], "N/mm²", category: "齿根弯曲应力");
            Compare("σF0 名义齿根应力(小轮)", pinion["名义弯曲应力"], Expected.SigmaF0[0], "N/mm²", category: "齿根弯曲应力");
            Compare("σF0 名义齿根应力(大轮)", wheel["名义弯曲应力"], Expected.SigmaF0[1], "N/mm²", category: "齿根弯曲应力");
            Compare("σFP 许用齿根应力(小轮)", pinion["许用弯曲应力"], Expected.SigmaFP[0], "N/mm²", category: "齿根弯曲应力");
            Compare("σFP 许用齿根应力(大轮)", wheel["许用弯曲应力"], Expected.SigmaFP[1], "N/mm²", category: "齿根弯曲应力");

            // 几何
            Compare("分度圆直径 d(小轮)", pinion["分度圆直径[d]"], Expected.D[0], "mm", category: "几何");
            Compare("分度圆直径 d(大轮)", wheel["分度圆直径[d]"], Expected.D[1], "mm", category: "几何");
            Compare("齿顶圆直径 da(小轮)", pinion["齿顶圆直径[da]"], Expected.Da[0], "mm", category: "几何");
            Compare("齿顶圆直径 da(大轮)", wheel["齿顶圆直径[da]"], Expected.Da[1], "mm", category: "几何");
            Compare("基圆直径 db(小轮)", pinion["基圆直径[db]"], Expected.Db[0], "mm", category: "几何");
            Compare("基圆直径 db(大轮)", wheel["基圆直径[db]"], Expected.Db[1], "mm", category: "几何");
            Compare("工作节圆直径 dw(小轮)", pinion["工作节圆直径[dw]"], Expected.Dw[0], "mm", category: "几何");
            Compare("工作节圆直径 dw(大轮)", wheel["工作节圆直径[dw]"], Expected.Dw[1], "mm", category: "几何");
            Compare("齿根圆直径 df(小轮)", pinion["齿根圆直径[df]"], Expected.Df[0], "mm", category: "几何");
            Compare("齿根圆直径 df(大轮)", wheel["齿根圆直径[df]"], Expected.Df[1], "mm", category: "几何");
            Compare("齿根成形圆直径 dFf(小轮)", pinion["齿根成形圆直径[dFf]"], Expected.DFf[0], "mm", category: "几何");
            Compare("齿根成形圆直径 dFf(大轮)", wheel["齿根成形圆直径[dFf]"], Expected.DFf[1], "mm", category: "几何");
            Compare("有效齿根圆直径 dNf(小轮)", pinion["有效齿根圆直径[dNF]"], Expected.DNf[0], "mm", category: "几何");
            Compare("有效齿根圆直径 dNf(大轮)", wheel["有效齿根圆直径[dNF]"], Expected.DNf[1], "mm", category: "几何");
            Compare("跨棒距 Wk(小轮)", pinion["跨棒距[Wk]"], Expected.Wk[0], "mm", category: "几何");
            Compare("跨棒距 Wk(大轮)", wheel["跨棒距[Wk]"], Expected.Wk[1], "mm", category: "几何");

            // 变位系数（绝对值小的诊断项，作参考不计入最大偏差）
            Compare("产形变位系数 xE(小轮)", pinion["产形齿廓变位系数[xEs]"], Expected.XE[0], "—", isReference: true, category: "变位系数");
            Compare("产形变位系数 xE(大轮)", wheel["产形齿廓变位系数[xEs]"], Expected.XE[1], "—", isReference: true, category: "变位系数");

            // 啮合基础与节距（Annex A.6 分步中间量；作参考项不计入最大偏差）
            // 注：（kisssoft 参数文件运行时）gα 0.45% 偏差为 kisssoft 优化参数（Ca=2μm、x2=0.0275）改变有效齿顶圆/接触路径所致；
            //     invαt 0.03% 为 ISO 文档 5 位有效数字舍入。均为参考项。
            Compare("齿数比 u", pinion["齿数比[u]"], Expected.U, "—", isReference: true, category: "啮合基础");
            Compare("端面模数 mt", pinion["端面模数[mt]"], Expected.Mt, "mm", isReference: true, category: "啮合基础");
            Compare("端面压力角 αt", pinion["端面理论压力角[αt]"], Expected.AlphaT, "°", isReference: true, category: "啮合基础");
            Compare("端面啮合角 αwt", pinion["端面啮合角[αwt]"], Expected.AlphaWt, "°", isReference: true, category: "啮合基础");
            Compare("基圆螺旋角 βb", pinion["基圆螺旋角[βb]"], Expected.BetaB, "°", isReference: true, category: "啮合基础");
            Compare("渐开线函数 invαt", pinion["渐开线函数[invαt]"], Expected.InvAlphaT, "—", isReference: true, category: "啮合基础");
            Compare("法向节距 pn", pinion["法向节距[pn]"], Expected.Pn, "mm", isReference: true, category: "节距");
            Compare("分度圆节距 pt", pinion["分度圆节距[pt]"], Expected.Pt, "mm", isReference: true, category: "节距");
            Compare("基圆节距 pbt", pinion["基圆节距[pbt]"], Expected.Pbt, "mm", isReference: true, category: "节距");
            Compare("端面啮合齿距 pet", pinion["端面啮合齿距[pet]"], Expected.Pet, "mm", isReference: true, category: "节距");
            Compare("接触路径长度 gα", pinion["接触路径长度[gα]"], Expected.GAlpha, "mm", isReference: true, category: "重合度");

            // 载荷/速度
            double ftComputed = input.扭矩 != 0 && pinion["分度圆直径[d]"] != 0
                ? 2.0 * Math.Abs(input.扭矩) * 1000.0 / pinion["分度圆直径[d]"] : double.NaN;
            Compare("名义切向载荷 Ft", ftComputed, Expected.Ft, "N", category: "载荷与速度");
            // 节线速度：DLL 用法向速度 vn，ISO 用节线速度（分度圆切向），作参考
            Compare("节线速度 vw", pinion["法向速度[vn]"], Expected.Vw, "m/s", isReference: true, category: "载荷与速度");

            // 重合度
            Compare("端面重合度 εα", pinion["端面重合度[εα]"], Expected.EpsAlpha[0], "—", category: "重合度");
            Compare("纵向重合度 εβ", pinion["纵向重合度[εβ]"], Expected.EpsBeta, "—", category: "重合度");
            Compare("总重合度 εγ", pinion["总重合度[εγ]"], Expected.EpsGamma, "—", category: "重合度");

            // 安全系数（输出字段不含，从许用/计算应力推导；误差作参考项）
            double shPinion = pinion["计算接触应力"] != 0 ? pinion["许用接触应力"] / pinion["计算接触应力"] : double.NaN;
            double shWheel = wheel["计算接触应力"] != 0 ? wheel["许用接触应力"] / wheel["计算接触应力"] : double.NaN;
            double sfPinion = pinion["计算弯曲应力"] != 0 ? pinion["许用弯曲应力"] / pinion["计算弯曲应力"] : double.NaN;
            double sfWheel = wheel["计算弯曲应力"] != 0 ? wheel["许用弯曲应力"] / wheel["计算弯曲应力"] : double.NaN;
            double ErrOf(double computed, double iso) =>
                !double.IsNaN(computed) && iso != 0 ? Math.Abs(computed - iso) / Math.Abs(iso) * 100 : double.NaN;
            rows.Add(new CompareRow("安全系数", "SH 点蚀安全系数(小轮)", shPinion, Expected.SH[0], "—", ErrOf(shPinion, Expected.SH[0]), true));
            rows.Add(new CompareRow("安全系数", "SH 点蚀安全系数(大轮)", shWheel, Expected.SH[1], "—", ErrOf(shWheel, Expected.SH[1]), true));
            rows.Add(new CompareRow("安全系数", "SF 断齿安全系数(小轮)", sfPinion, Expected.SF[0], "—", ErrOf(sfPinion, Expected.SF[0]), true));
            rows.Add(new CompareRow("安全系数", "SF 断齿安全系数(大轮)", sfWheel, Expected.SF[1], "—", ErrOf(sfWheel, Expected.SF[1]), true));

            // ── 5. 渲染 Markdown 表格并输出 ──
            var inputMd = RenderInputComparisonTable(isoParams);
            var mdTable = inputMd + "\n" + RenderComparisonTable(rows, maxErr);
            Console.WriteLine();
            Console.WriteLine(mdTable);

            // ── 5. 保存结果 ──
            var result = new InvGearResult { Scenario = "iso-example1" };
            result.太阳轮 = pinion;
            result.行星轮 = wheel;
            var path = Path.Combine(outputDir, "iso-example1-result.json");
            File.WriteAllText(path, JsonSerializer.Serialize(result, JsonOptions.Pretty));

            // 对比报告保存
            var report = new
            {
                Scenario = "iso-example1",
                GeneratedAt = DateTime.Now,
                MaxDeviationPct = Math.Round(maxErr, 2),
                Computed = new
                {
                    SigmaH = pinion["计算接触应力"],
                    SigmaF = new[] { pinion["计算弯曲应力"], wheel["计算弯曲应力"] },
                    D = new[] { pinion["分度圆直径[d]"], wheel["分度圆直径[d]"] },
                    Da = new[] { pinion["齿顶圆直径[da]"], wheel["齿顶圆直径[da]"] },
                    Db = new[] { pinion["基圆直径[db]"], wheel["基圆直径[db]"] },
                    Dw = new[] { pinion["工作节圆直径[dw]"], wheel["工作节圆直径[dw]"] },
                    Df = new[] { pinion["齿根圆直径[df]"], wheel["齿根圆直径[df]"] },
                    DFf = new[] { pinion["齿根成形圆直径[dFf]"], wheel["齿根成形圆直径[dFf]"] },
                    DNf = new[] { pinion["有效齿根圆直径[dNF]"], wheel["有效齿根圆直径[dNF]"] },
                    Wk = new[] { pinion["跨棒距[Wk]"], wheel["跨棒距[Wk]"] },
                    XE = new[] { pinion["产形齿廓变位系数[xEs]"], wheel["产形齿廓变位系数[xEs]"] },
                    Ft = ftComputed,
                    Vw = pinion["法向速度[vn]"],
                    EpsAlpha = pinion["端面重合度[εα]"],
                    EpsBeta = pinion["纵向重合度[εβ]"],
                    EpsGamma = pinion["总重合度[εγ]"],
                    // ── Annex A.6 分步中间量 ──
                    U = pinion["齿数比[u]"],
                    Mt = pinion["端面模数[mt]"],
                    AlphaT = pinion["端面理论压力角[αt]"],
                    AlphaWt = pinion["端面啮合角[αwt]"],
                    BetaB = pinion["基圆螺旋角[βb]"],
                    InvAlphaT = pinion["渐开线函数[invαt]"],
                    Pn = pinion["法向节距[pn]"],
                    Pt = pinion["分度圆节距[pt]"],
                    Pbt = pinion["基圆节距[pbt]"],
                    Pet = pinion["端面啮合齿距[pet]"],
                    GAlpha = pinion["接触路径长度[gα]"]
                },
                IsoExpected = new
                {
                    SigmaH = Expected.SigmaH,
                    SigmaF = Expected.SigmaF,
                    D = Expected.D,
                    Da = Expected.Da,
                    Db = Expected.Db,
                    Dw = Expected.Dw,
                    Df = Expected.Df,
                    DFf = Expected.DFf,
                    DNf = Expected.DNf,
                    Wk = Expected.Wk,
                    XE = Expected.XE,
                    Ft = Expected.Ft,
                    Vw = Expected.Vw,
                    EpsAlpha = Expected.EpsAlpha[0],
                    EpsBeta = Expected.EpsBeta,
                    EpsGamma = Expected.EpsGamma,
                    // ── Annex A.6 分步中间量 ──
                    U = Expected.U,
                    Mt = Expected.Mt,
                    AlphaT = Expected.AlphaT,
                    AlphaWt = Expected.AlphaWt,
                    BetaB = Expected.BetaB,
                    InvAlphaT = Expected.InvAlphaT,
                    Pn = Expected.Pn,
                    Pt = Expected.Pt,
                    Pbt = Expected.Pbt,
                    Pet = Expected.Pet,
                    GAlpha = Expected.GAlpha
                }
            };
            var reportPath = Path.Combine(outputDir, "iso-example1-comparison.json");
            File.WriteAllText(reportPath, JsonSerializer.Serialize(report, JsonOptions.Pretty));

            // Markdown 表格报告
            var mdPath = Path.Combine(outputDir, "iso-example1-comparison.md");
            File.WriteAllText(mdPath, mdTable, System.Text.Encoding.UTF8);

            Console.WriteLine($"\n📄 完整结果: {path}");
            Console.WriteLine($"📄 对比报告(JSON): {reportPath}");
            Console.WriteLine($"📄 对比表格(Markdown): {mdPath}");
            Console.WriteLine($"\n📊 最大偏差: {maxErr:F2}%");

            Console.WriteLine("\n========================================");
            int exitCode;
            if (maxErr < 5)
            {
                Console.WriteLine("✅ 偏差 < 5%，与 ISO 标准值高度吻合");
                exitCode = 0;
            }
            else if (maxErr < 15)
            {
                Console.WriteLine("⚠️ 偏差 5%~15%，存在算法或参数差异");
                exitCode = 1;
            }
            else
            {
                Console.WriteLine("❌ 偏差 > 15%，需排查参数映射或算法差异");
                exitCode = 2;
            }
            Console.WriteLine("========================================");
            return exitCode;
        }

        /// <summary>
        /// 将 ISO Example 1 (Table 3) 参数填入 InvGearInput（覆盖默认值）。
        /// 优先从参数模型读取，缺失字段回退 ISO 硬编码默认值（Annex A Table 3）。
        /// 注意：刀具类型、装配安装类型等字段有意保持默认、未参与 ISO 映射。
        /// 齿厚偏差 E_sns/E_sni（分齿轮 E_sns1/E_sni1/E_sns2/E_sni2）与 Y圆直径 d_y
        /// 已支持 kisssoft 优化参数映射，优先级：分齿轮 > 单一 E_sns > DLL 默认值（见下方实现）。
        /// </summary>
        private static void ApplyIsoParams(InvGearInput input, IsoExample1Params? p)
        {
            var g = p?.Input?.Geometry;
            var q = p?.Input?.Quality;
            var m = p?.Input?.Material;
            var a = p?.Input?.Application;
            var l = p?.Input?.Load;

            // ── 公共几何 [62..70] ──
            input.法向模数 = g?.mn ?? 8.0;
            input.压力角度 = g?.αn ?? 20.0;    // 度
            input.螺旋角度 = g?.β ?? 15.8;            // 度
            input.中心距 = g?.a ?? 500.0;
            input.齿轮副类型 = 1;                                  // 一般外啮合
            input.行星轮个数 = 1;                                  // 单对啮合
            input.驱动齿轮 = 0;                                    // 小齿轮驱动
            input.触点间隙j_bn = 0;

            // ── 太阳轮(小轮)几何 [0..30] ──
            var sun = input.太阳轮几何;
            sun[0] = g?.z1 ?? 17;       // 齿数
            sun[1] = -1;                         // 旋向 Left
            sun[2] = g?.b1 ?? 100;  // 设计齿宽
            sun[3] = 1;                          // 齿顶高系数 ha*
            sun[4] = g?.x1 ?? 0.14522; // 齿廓变位系数 x（Annex A Table A.1: 0.14522）

            // ── DLL 齿根高推导 ──
            // DLL 内部 h_fP = (h_ax + cx) × mn，h_ax 默认 1.0。
            // ISO 直接指定 hfP=1.4×mn（绝对值 11.2mm），故 cx = hfP/mn − 1.0。
            // 优先级：hfP_mn（ISO 齿条齿根高）> cx（JSON）> 默认 0.25。
            double mnEff = g?.mn ?? 8.0;
            double hfP_coef;          // 齿条刀齿顶高系数 hfP/mn（无量纲）
            double cxDll;             // DLL 顶隙系数（使 h_fP = hfP）
            if (g?.hfP_mn != null && mnEff != 0.0)
            {
                hfP_coef = g.hfP_mn.Value / mnEff;
                cxDll = hfP_coef - 1.0;    // h_ax 默认 1.0
                Console.WriteLine($"  🔧 从 hfP_mn 推导: hfP/mn={hfP_coef:F2}, cx(DLL)={cxDll:F2}");
            }
            else
            {
                // 回退方案：使 hfP_coef 与 cxDll 自洽（cxDll = hfP_coef − 1.0）
                cxDll = g?.cx ?? 0.25;
                hfP_coef = 1.0 + cxDll;      // 确保与 DLL 内部 h_fP = (1.0+cx)×mn 一致
            }

            // ρfP/mn 系数（4 处共用，提取避免重复）
            double ρfP_coef = g?.ρfP_mn != null && mnEff != 0.0 ? g.ρfP_mn.Value / mnEff : 0.39;

            sun[5] = cxDll;
            sun[6] = ρfP_coef;
            sun[7] = 0;                          // 挖根量 pr
            sun[8] = g?.Ca ?? 70;     // 齿顶修形量 Ca (μm)
            sun[9] = g?.Cf ?? 0;     // 齿根修形量 Cf
            sun[11] = g?.k1 ?? 2;// 跨齿数 k
            sun[12] = 0;                         // 齿顶倒角量 hK
            sun[22] = 0;                         // 刀具类型 (0=RackCutter, As cut)
            sun[23] = hfP_coef;                  // 齿条刀齿顶高系数 hfP/mn
            sun[24] = ρfP_coef;

            // ── 行星轮(大轮)几何 [0..30] ──
            var planet = input.行星轮几何;
            planet[0] = g?.z2 ?? 103;    // 齿数
            planet[1] = 1;                       // 旋向 Right
            planet[2] = g?.b2 ?? 100;// 设计齿宽
            planet[3] = 1;
            planet[4] = g?.x2 ?? 0.0; // 变位系数 x=0
            planet[5] = cxDll;                   // 与太阳轮相同的 cx（同刀具）
            planet[6] = ρfP_coef;
            planet[8] = g?.Ca ?? 70;  // Ca
            planet[9] = g?.Cf ?? 0;  // Cf
            planet[11] = g?.k2 ?? 13; // 跨齿数 k
            planet[23] = hfP_coef;
            planet[24] = ρfP_coef;

            // ── 齿厚偏差（分齿轮，kisssoft 优化值；缺省回退单一 E_sns）──
            // ISO xE=0.1178 由跨棒距 Wk 反算；DLL 用名义 x+齿厚偏差。
            // 优先级：E_sns1/E_sni1/E_sns2/E_sni2（kisssoft 分齿轮）> E_sns（单一）> DLL 默认值。
            // ⚠️ 回退终点是 InvGearInput 的 DLL 测试默认（sun[15]=-0.0670 等，非 0）；
            //    "缺省"≠"无齿厚偏差"，若需 0 偏差请显式填 0。
            double sunSns  = g?.E_sns1 ?? g?.E_sns ?? sun[15];
            double sunSni  = g?.E_sni1 ?? g?.E_sns1 ?? g?.E_sns ?? sun[16];
            double gearSns = g?.E_sns2 ?? g?.E_sns ?? planet[15];
            double gearSni = g?.E_sni2 ?? g?.E_sns2 ?? g?.E_sns ?? planet[16];
            sun[15] = sunSns; sun[16] = sunSni;
            planet[15] = gearSns; planet[16] = gearSni;
            if (g?.E_sns != null || g?.E_sns1 != null || g?.E_sni1 != null || g?.E_sns2 != null || g?.E_sni2 != null)
                Console.WriteLine($"  🔧 齿厚偏差: 小轮 E_sns/E_sni={sunSns}/{sunSni}, 大轮 E_sns/E_sni={gearSns}/{gearSni} mm");

            // ── Y 圆直径 d_y（kisssoft 可设 0；缺省 InvGearInput 默认 25）──
            if (g?.d_y is double dy)
            {
                sun[21] = dy;
                planet[21] = dy;
            }

            // ── 材料 [93..108]（渗碳钢 Eh, MQ）──
            var sunMat = input.太阳轮材料;
            sunMat[0] = m?.E ?? 206000;  // E (MPa)
            sunMat[1] = m?.ν ?? 0.3;        // v
            sunMat[2] = 500;                            // 屈服强度
            sunMat[3] = (m?.ρ ?? 7830) * 1e-9; // 密度 (kg/m³ → kg/mm³, 1m³=1e9mm³)
            sunMat[4] = m?.σHlim ?? 1500;   // σHlim
            sunMat[5] = m?.σFlim ?? 500;       // σFlim
            sunMat[6] = 500;
            sunMat[7] = 1080;         // 抗拉强度
            input.行星轮材料 = (double[])sunMat.Clone();

            var sunMfg = input.太阳轮加工;
            sunMfg[0] = 9;        // 材料类型 (Eh)
            sunMfg[1] = 1;        // 加工质量 MQ
            sunMfg[2] = m?.HRC ?? 60;   // 表面硬度 HRC
            sunMfg[3] = m?.HRC_core ?? 30;   // 芯部硬度 HRC
            sunMfg[4] = 0;        // 喷丸 No
            sunMfg[5] = 0;        // 有限点蚀 No
            input.行星轮加工 = (double[])sunMfg.Clone();

            // ── 应用 [125..148] ──
            input.公共应用[0] = a?.KA ?? 1.0; // 使用系数 KA
            input.公共应用[1] = a?.Kγ ?? 1.0; // 均载系数 Kγ
            input.公共应用[2] = a?.s ?? 0.0; // 轴承间距偏移量
            input.公共应用[3] = a?.ν40 ?? 320; // 润滑油粘度 ν40
            input.公共应用[5] = a?.SHmin ?? 1.0;   // 接触最小安全系数
            input.公共应用[6] = a?.SFmin ?? 1.0;  // 弯曲最小安全系数

            // Annex A Table A.4 应用参数（影响 KHβ/KFβ 载荷分布）
            if (a?.fcp is double fcp) { input.太阳轮应用[1] = fcp; input.行星轮应用[1] = fcp; }
            if (a?.hm is double hm) { input.太阳轮应用[2] = hm; input.行星轮应用[2] = hm; }
            if (a?.pmc is double pmc) { input.太阳轮应用[3] = pmc; input.行星轮应用[3] = pmc; }

            // ── 公差 [149..160] ──
            var sunTol = input.太阳轮公差;
            sunTol[0] = q?.Q ?? 5;     // 精度等级
            sunTol[1] = q?.fpt1 ?? 8.5;  // 端面节线偏差 ƒpT
            sunTol[2] = q?.ffα ?? 9.5;   // 齿廓偏差 ƒfα
            sunTol[3] = q?.fHβ1 ?? 8.0; // 螺旋线偏差 fHβ
            sunTol[4] = q?.RaH ?? 1.0;    // 齿面 Ra
            sunTol[5] = q?.RaF ?? 3.0;   // 齿根 Ra

            var planetTol = input.行星轮公差;
            planetTol[0] = q?.Q ?? 5;
            planetTol[1] = q?.fpt2 ?? 9.0; // ƒpT
            planetTol[2] = q?.ffα ?? 9.5;   // ƒfα
            planetTol[3] = q?.fHβ2 ?? 9.0; // fHβ
            planetTol[4] = q?.RaH ?? 1.0;   // Ra
            planetTol[5] = q?.RaF ?? 3.0;   // Ra

            // ── 载荷 [161..163] ──
            input.扭矩 = l?.T1 ?? 9000;   // N·m (ISO 9.000 kNm)
            input.转速 = l?.n1 ?? 360;
            input.预期寿命 = l?.Lh ?? 50000;

            // ── 装配 [164..169] ──
            var asm = input.装配;
            asm[0] = 5;                       // 安装类型
            asm[1] = 0;                       // 是否刚性轴
            asm[2] = 1;                       // 行星轮个数
            asm[3] = a?.dsh ?? 100; // 太阳轮轴直径 dsh
            asm[4] = 0;                       // 齿宽中点至轴承跨距中点距离
            asm[5] = a?.l ?? 125; // 轴承跨距 l
        }

        /// <summary>渲染输入参数对比表（PDF Annex A 原始值 vs 程序适配值）</summary>
        private static string RenderInputComparisonTable(IsoExample1Params? p)
        {
            var sb = new System.Text.StringBuilder();
            var g = p?.Input?.Geometry;
            var m = p?.Input?.Material;
            var a_ = p?.Input?.Application;
            var l = p?.Input?.Load;

            string V(double? val, string fallback) => val?.ToString() ?? fallback;

            sb.AppendLine("## 输入参数对比（Annex A 原始值 vs 程序使用值）");
            sb.AppendLine();
            sb.AppendLine("| 符号 | 含义 | Annex A 原始值 | 程序使用值 | 适配? | 说明 |");
            sb.AppendLine("|------|------|---------------|-----------|-------|------|");

            void G(string sym, string desc, string iso, string used, string adapt = "-", string note = "")
                => sb.AppendLine($"| {sym} | {desc} | {iso} | {used} | {adapt} | {note} |");

            sb.AppendLine("| | **几何 (Table A.1)** | | | | |");
            G("z1", "小轮齿数", "17", V(g?.z1, "17"));
            G("z2", "大轮齿数", "103", V(g?.z2, "103"));
            G("mn", "法向模数", "8.0 mm", V(g?.mn, "8.0"));
            G("αn", "法向压力角", "20.0°", V(g?.αn, "20.0"));
            G("β", "螺旋角", "15.8°", V(g?.β, "15.8"));
            G("b1", "小轮齿宽", "100 mm", V(g?.b1, "100"));
            G("b2", "大轮齿宽", "100 mm", V(g?.b2, "100"));
            G("a", "中心距", "500 mm", V(g?.a, "500"));
            G("Wk1", "跨棒距(小)", "38.196 mm", V(g?.Wk1, "38.196"));
            G("Wk2", "跨棒距(大)", "307.943 mm", V(g?.Wk2, "307.943"));
            G("k1", "跨齿数(小)", "2", V(g?.k1, "2"));
            G("k2", "跨齿数(大)", "13", V(g?.k2, "13"));
            G("x1", "变位系数(小)", "(0.14522)", V(g?.x1, "0.14522"), g?.x1 != 0.14522 ? "⚠️" : "-", g?.x1 != 0.14522 ? "用产形变位 xE" : "");
            G("x2", "变位系数(大)", "0", V(g?.x2, "0"));
            G("da1", "齿顶圆(小)", "159.66 mm", V(g?.da1, "159.66"));
            G("da2", "齿顶圆(大)", "872.35 mm", V(g?.da2, "872.35"));
            G("hfP", "齿条齿根高", "11.2 mm (1.4×mn)", V(g?.hfP_mn, "11.2"));
            G("ρfP", "齿条圆角半径", "3.12 mm (0.39×mn)", V(g?.ρfP_mn, "3.12"));
            G("Ca", "齿顶修形", "70 μm", V(g?.Ca, "70"));
            G("Cf", "齿根修形", "0", V(g?.Cf, "0"));
            G("spr", "残余挖根", "0", "0");
            G("q", "加工余量", "0", "0");
            G("cx", "顶隙系数", "0.25 (标准)", V(g?.cx, "0.25"), g?.cx != 0.25 ? "⚠️" : "-", g?.cx != 0.25 ? "使 DLL h_fx 匹配 ISO hfP" : "");

            // 质量 (Table A.2)
            var q = p?.Input?.Quality;
            sb.AppendLine("| | **质量 (Table A.2)** | | | | |");
            G("Q", "精度等级", "5", V(q?.Q, "5"));
            G("fpt1", "齿距偏差(小)", "8.5 μm", V(q?.fpt1, "8.5"));
            G("fpt2", "齿距偏差(大)", "9.0 μm", V(q?.fpt2, "9.0"));
            G("ffα", "齿廓偏差", "9.5 μm", V(q?.ffα, "9.5"));
            G("fHβ1", "螺旋线偏差(小)", "8.0 μm", V(q?.fHβ1, "8.0"));
            G("fHβ2", "螺旋线偏差(大)", "9.0 μm", V(q?.fHβ2, "9.0"));
            G("RaH", "齿面粗糙度", "1.0 μm", V(q?.RaH, "1.0"));
            G("RaF", "齿根粗糙度", "3.0 μm", V(q?.RaF, "3.0"));

            // 材料 (Table A.3)
            sb.AppendLine("| | **材料 (Table A.3)** | | | | |");
            G("E", "弹性模量", "206000 MPa", V(m?.E, "206000"));
            G("ν", "泊松比", "0.3", V(m?.ν, "0.3"));
            G("σS", "屈服强度", "500 MPa", V(m?.σS, "500"));
            G("ρ", "密度", "7830 kg/m³", V(m?.ρ, "7830"));
            G("σHlim", "接触疲劳极限", "1500 MPa", V(m?.σHlim, "1500"));
            G("σFlim", "弯曲疲劳极限", "500 MPa", V(m?.σFlim, "500"));
            G("HRC", "表面硬度", "60", V(m?.HRC, "60"));
            G("HRC_core", "芯部硬度", "30", V(m?.HRC_core, "30"));

            // 应用 (Table A.4)
            sb.AppendLine("| | **应用 (Table A.4)** | | | | |");
            G("KA", "使用系数", "1.0", V(a_?.KA, "1.0"));
            G("Kγ", "均载系数", "1.0", V(a_?.Kγ, "1.0"));
            G("l", "轴承跨距", "125 mm", V(a_?.l, "125"));
            G("s", "轴承偏移", "0", V(a_?.s, "0"));
            G("dsh", "轴径", "100 mm", V(a_?.dsh, "100"));
            G("SHmin", "接触最小安全系数", "1.0", V(a_?.SHmin, "1.0"));
            G("SFmin", "弯曲最小安全系数", "1.0", V(a_?.SFmin, "1.0"));
            G("ν40", "润滑油粘度", "320 mm²/s", V(a_?.ν40, "320"));
            G("fcp", "接触区控制", "No", V(a_?.fcp, "0"), a_?.fcp != null && a_.fcp != 0 ? "⚠️" : "-", a_?.fcp != null && a_.fcp != 0 ? "DLL 默认 Yes" : "");
            G("hm", "螺旋线修形", "None", V(a_?.hm, "0"), a_?.hm != null && a_.hm != 1 ? "⚠️" : "-", a_?.hm != null && a_.hm != 1 ? "DLL 默认 1" : "");
            G("pmc", "齿廓变形补偿", "Yes", V(a_?.pmc, "1"), a_?.pmc != null && a_.pmc != 1 ? "⚠️" : "-", a_?.pmc != null && a_.pmc != 1 ? "DLL 默认 No" : "");

            // 载荷 (Table A.5)
            sb.AppendLine("| | **载荷 (Table A.5)** | | | | |");
            G("T1", "输入扭矩", "9000 N·m", V(l?.T1, "9000"));
            G("n1", "输入转速", "360 r/min", V(l?.n1, "360"));
            G("Lh", "设计寿命", "50000 h", V(l?.Lh, "50000"));

            sb.AppendLine();
            sb.AppendLine("> ⚠️ = 适配参数（因 DLL 输入语义与 ISO 不同，需调整使计算匹配 Annex A 期望值）");
            sb.AppendLine();
            return sb.ToString();
        }

        /// <summary>对比表格行</summary>
        private sealed record CompareRow(
            string Category, string Label, double Computed, double Iso,
            string Unit, double ErrPct, bool IsReference);

        /// <summary>
        /// 渲染 Markdown 对比表格（按类别分组，含计算值/ISO标准值/单位/误差%）。
        /// </summary>
        private static string RenderComparisonTable(List<CompareRow> rows, double maxErr)
        {
            var sb = new System.Text.StringBuilder();
            sb.AppendLine("# ISO/TR 6336-30:2022 Example 1 渐开线计算对比报告");
            sb.AppendLine();
            sb.AppendLine("> 单斜齿渗碳淬火齿轮副：z=17/103, mn=8, αn=20°, β=15.8°, a=500, T1=9000 N·m, n=360");
            sb.AppendLine($"> 最大偏差：**{maxErr:F2}%**　[参考]=诊断项不计入最大偏差");
            sb.AppendLine();

            foreach (var group in rows.GroupBy(r => r.Category))
            {
                sb.AppendLine($"## {group.Key}");
                sb.AppendLine();
                sb.AppendLine("| 参数 | 计算值 | ISO 标准值 | 单位 | 误差(%) | 备注 |");
                sb.AppendLine("|------|--------|-----------|------|---------|------|");
                foreach (var r in group)
                {
                    var errText = double.IsNaN(r.ErrPct) ? "—" : r.ErrPct.ToString("F2");
                    var remark = r.IsReference ? "参考" : (r.ErrPct <= 1 ? "✅ 吻合" : (r.ErrPct <= 5 ? "良好" : "⚠️ 偏差"));
                    var computed = double.IsNaN(r.Computed) || double.IsInfinity(r.Computed) ? "—" : r.Computed.ToString("F2");
                    var iso = double.IsNaN(r.Iso) ? "—" : r.Iso.ToString("F2");
                    sb.AppendLine($"| {r.Label} | {computed} | {iso} | {r.Unit} | {errText} | {remark} |");
                }
                sb.AppendLine();
            }
            return sb.ToString();
        }
    }
}


