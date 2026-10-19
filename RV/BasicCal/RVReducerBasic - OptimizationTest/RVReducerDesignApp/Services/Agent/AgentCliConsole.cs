using System;
using System.Collections.Generic;
using System.Text;
using OpenAI.Chat;

namespace RVReducerDesignApp.Services.Agent;

/// <summary>
/// 终端 AI 计算助手（--chat 模式）：
/// 无 WPF 窗口的纯控制台 REPL —— 用户输入自然语言，Agent 自动调用计算工具并回显结果。
/// 与界面内 AI 面板共用 AgentRunner，但工具集剔除了依赖「当前界面工程」的工具，
/// 只保留可独立计算的能力（rv_calc / calculate_inv_gear_stress / calc_bearing_life_by_code / 快照导出）。
/// </summary>
public static class AgentCliConsole
{
    /// <summary>
    /// 终端模式工具集。不含 calc_current_part / get_design_state / calc_crank_shaft_system：
    /// 这三个工具依赖界面状态，无 UI 时会返回哨兵值并误导模型，故从终端模式剔除。
    /// </summary>
    public static List<AgentTool> GetCliTools() =>
    [
        AgentRvCalcTool.Build(),                  // rv_calc：注册表驱动的任意 DLL 接口计算
        AgentCalcTools.BuildInvGearStressTool(),  // calculate_inv_gear_stress：渐开线齿轮副应力（内置默认参数）
        AgentMachineTools.BuildBearingLifeTool(), // calc_bearing_life_by_code：轴承库查询 + 寿命计算
        AgentRvCalcTool.BuildSnapshotExporter(),  // export_whole_machine_snapshot：导出整机快照
    ];

    /// <summary>终端模式系统提示词（替代界面导向的 ProjectContextLoader.BuildSystemPrompt）</summary>
    public static string BuildCliSystemPrompt() =>
        DeepSeekConfig.SystemPrompt + "\n\n" +
        "【运行环境】当前是终端无界面模式（--chat），没有 WPF 界面，也不存在「当前界面工程」。\n" +
        "【工具使用原则】\n" +
        "- 任何计算需求都优先用 rv_calc：先阅读工具描述里的接口清单，选定 interface，" +
        "再把用户给出的参数翻译为该接口声明的参数名（可用中文主名或别名，数值可带单位）放入 params。\n" +
        "- 渐开线齿轮副的接触/弯曲应力：优先用 calculate_inv_gear_stress（内置已验证默认参数，只需给模数、齿数等少量参数）。\n" +
        "- 轴承寿命：用户给出轴承型号时用 calc_bearing_life_by_code，Cr/C0r/e/Y 由工具自动查库，不要自己猜。\n" +
        "- 不要在参数名上反复试错：接口清单里没有的参数名会被工具直接拒绝，先查清单再调用。\n" +
        "- 完成任务后用中文总结关键数值与结论。";

    /// <summary>运行终端 REPL，返回进程退出码（0=正常退出，2=初始化失败）</summary>
    public static int Run()
    {
        if (string.IsNullOrWhiteSpace(DeepSeekConfig.ApiKey))
        {
            Console.WriteLine("❌ 未配置 API Key。请设置环境变量 DEEPSEEK_API_KEY，");
            Console.WriteLine("   或在 WPF 应用「AI 面板」中保存过 API 设置（%APPDATA%\\RVReducerDesignApp\\ai-config.json）。");
            return 2;
        }

        // 尽量让中文正常显示；stdout 被重定向时设置编码会抛异常，忽略即可
        try
        {
            Console.OutputEncoding = Encoding.UTF8;
            Console.InputEncoding = Encoding.UTF8;
        }
        catch
        {
            // 忽略：输出为 GBK 时中文可能乱码，但不影响功能
        }

        AgentRunner runner;
        try
        {
            runner = new AgentRunner(GetCliTools());
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Agent 初始化失败: {ex.Message}");
            return 2;
        }

        Console.WriteLine("RV 减速器 AI 计算助手（终端模式）");
        Console.WriteLine($"  模型: {DeepSeekConfig.Model}");
        Console.WriteLine("  用自然语言描述计算需求（例如：算一下模数 2、太阳轮 23 齿、行星轮 106 齿的渐开线齿轮应力）");
        Console.WriteLine("  输入 exit / quit 退出。");
        Console.WriteLine();

        var history = new List<ChatMessage>();
        string systemPrompt = BuildCliSystemPrompt();

        while (true)
        {
            Console.Write("you> ");
            string? line = Console.ReadLine();
            if (line is null) break;               // stdin 结束（如管道输入）
            if (string.IsNullOrWhiteSpace(line)) continue;
            string trimmed = line.Trim();
            if (trimmed is "exit" or "quit" or "q" or "退出") break;

            try
            {
                string reply = runner.RunAsync(trimmed, history, systemPrompt)
                    .GetAwaiter().GetResult();
                Console.WriteLine(reply);
                Console.WriteLine();
                history.Add(new UserChatMessage(trimmed));
                history.Add(new AssistantChatMessage(reply));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ 执行失败: {ex.GetType().Name}: {ex.Message}");
                Console.WriteLine();
            }
        }

        return 0;
    }
}
