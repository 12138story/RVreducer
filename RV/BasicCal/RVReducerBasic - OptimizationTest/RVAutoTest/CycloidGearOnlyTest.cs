using System;
using System.IO;
using System.Text.Json;
using RVAutoTest.Models;
using RVAutoTest.Scenarios;

namespace RVAutoTest
{
    /// <summary>
    /// 摆线针轮副独立计算模式（--cycloid）。
    /// 从 cycloid_params.json 读取输入参数（缺失字段用内置默认值），调用 DLL
    /// CalCycloidGearStressByObj 计算，将输入参数与输出参数分别写入 JSON 文件；
    /// 不启动 WPF、不截图。
    /// 用法：--cycloid [<参数文件路径>]（缺省用运行目录 cycloid_params.json）
    /// </summary>
    public static class CycloidGearOnlyTest
    {
        public static int Run(string[] args)
        {
            var workspaceRoot = Program.FindWorkspaceRootForVerify();

            // 复制 DLL 依赖（RVReducerBasic.dll + Intel MKL 等原生依赖）到运行目录
            try
            {
                Program.EnsureDllsAvailable(workspaceRoot);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ DLL 依赖准备失败: {ex.Message}");
                return 2;
            }

            // 每次运行生成时间戳子文件夹：screenshots/cycloid-gear/<yyyyMMdd-HHmmss>/
            var outputDir = Program.CreateTimestampedDir(workspaceRoot, "screenshots", "cycloid-gear");
            Console.WriteLine($"📁 输出目录: {outputDir}");

            Console.WriteLine("========================================");
            Console.WriteLine("  摆线针轮副计算（--cycloid）");
            Console.WriteLine("========================================");

            // ── 1. 读取输入参数 ──
            var paramsPath = args.Length >= 2 ? args[1]
                : Path.Combine(Path.GetDirectoryName(AppContext.BaseDirectory) ?? ".", "cycloid_params.json");
            var input = ConfigLoader.LoadCycloidParams(paramsPath) ?? new CycloidGearInput();
            Console.WriteLine(File.Exists(paramsPath)
                ? $"📄 使用参数文件: {paramsPath}"
                : "📄 未找到参数文件，使用内置默认值");

            double torqueScale = input.TorqueScale;
            if (torqueScale <= 0)
                throw new InvalidOperationException($"TorqueScale 非法: {torqueScale}（应为正数，1=关闭转换）");

            // 记录用户语义输入（缩放前，力矩单位 N·m）
            double cycloid1TorqueNm = input.Cycloid1Torque;
            double cycloid2TorqueNm = input.Cycloid2Torque;
            var inputRecord = new
            {
                _说明 = "摆线针轮副计算输入参数（力矩原始单位 N·m；DLL 实际输入 = 力矩 × TorqueScale 得 N·mm）。",
                参数文件 = paramsPath,
                input
            };
            // 注意：必须在 Calculate 之前序列化（Calculate 会就地缩放力矩为 N·mm），确保落盘保留 N·m 语义
            var inputJson = JsonSerializer.Serialize(inputRecord, JsonOptions.Pretty);

            // 打印输入摘要（缩放前 N·m 语义）
            Console.WriteLine($"\n📥 输入参数:");
            Console.WriteLine($"  针轮: Z={input.PinionZ:F0}, 滚针半径={input.RollerRadius:F3}, 分度圆半径={input.PitchCircleRadius:F3}, 齿宽={input.PinionWidth:F0}");
            Console.WriteLine($"  摆线轮1: Z={input.Cycloid1Z:F0}, 齿宽={input.Cycloid1Width:F0}, 偏心距={input.Cycloid1Eccentricity:F3}, 力矩={cycloid1TorqueNm:F2} N·m");
            Console.WriteLine($"  摆线轮2: Z={input.Cycloid2Z:F0}, 齿宽={input.Cycloid2Width:F0}, 偏心距={input.Cycloid2Eccentricity:F3}, 力矩={cycloid2TorqueNm:F2} N·m");
            Console.WriteLine($"  精度等级: {input.Cycloid1PrecGrade:F0}, 力矩缩放系数: {torqueScale:0.###}");

            // ── 2. 计算（力矩换算 + DLL 调用 + 解析，共享逻辑见 CycloidGearScenario.Calculate）──
            Console.WriteLine("\n📊 调用 CalCycloidGearStressByObj 计算...");
            CycloidGearResult result;
            try
            {
                result = CycloidGearScenario.Calculate(input);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ DLL 调用失败: {ex.Message}");
                return 1;
            }

            // ── 3. 保存输入参数 ──
            var inputPath = Path.Combine(outputDir, "cycloid-input.json");
            File.WriteAllText(inputPath, inputJson);
            Console.WriteLine($"  📄 输入参数: {inputPath}");

            // ── 4. 保存输出参数 ──
            var resultPath = Path.Combine(outputDir, "cycloid-result.json");
            File.WriteAllText(resultPath, JsonSerializer.Serialize(result, JsonOptions.Pretty));
            Console.WriteLine($"  📄 输出参数: {resultPath}");

            // ── 5. 结果摘要与校验 ──
            // DLL 失败标记（FindMaxContStress 全 θ 点应力 <=1e-6 时返回 1e10）或 NaN/Infinity 均视为结果无效，返回非零退出码
            bool resultInvalid = result.接触应力 <= 0
                || result.接触应力 >= 1e9
                || double.IsNaN(result.接触应力)
                || double.IsInfinity(result.接触应力);
            if (resultInvalid)
            {
                Console.WriteLine($"  ❌ 摆线针轮副结果无效（接触应力={result.接触应力:E0}，DLL 失败标记/数值异常，" +
                                  "可能与力矩单位 N·m/N·mm 或曲率数值问题有关）");
                return 1;
            }

            Console.WriteLine($"  📊 摆线针轮副: 接触应力={result.接触应力:F2} MPa, K1={result.短幅系数K1:F4}, h={result.全齿高h:F3}");
            Console.WriteLine($"       齿顶距 D_T={result.齿顶距D_T:F3}, 跨棒距 M_Mf={result.跨棒距M_Mf:F3}");
            Console.WriteLine($"       公差(μm): f_pt={result.单个齿距偏差f_pt:F3}, F_p={result.齿距累积总偏差F_p:F3}, F_β={result.齿向偏差F_β:F3}, F_r={result.径向跳动F_r:F3}");

            Console.WriteLine("\n✅ 摆线针轮副计算完成（无截图）");
            Console.WriteLine($"📁 输出目录: {outputDir}");
            return 0;
        }
    }
}
