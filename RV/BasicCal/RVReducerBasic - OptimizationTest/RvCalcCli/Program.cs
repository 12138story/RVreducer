using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using RvCalc;

namespace RvCalcCli;

/// <summary>
/// RvCalcCli —— 注册表驱动的通用计算命令行。
///
/// 用法：
///   --list-interfaces                列出全部接口及参数/输出
///   --describe &lt;接口名&gt;            查看单个接口详情
///   --call &lt;接口名&gt; [--args '{...}'|@file]   通用计算
///   --registry-check                 注册表 ↔ DLL 导出核对
///   --dump-templates                 重新生成输入模板（需工作区）
///   --selfcheck                      静态 DllImport 与通用调用器结果比对
/// </summary>
internal static class Program
{
    private static int Main(string[] args)
    {
        try
        {
            if (args.Length == 0 || args.Contains("--help") || args.Contains("-h"))
            {
                PrintHelp();
                return 0;
            }

            if (args.Contains("--list-interfaces")) return RunList();
            if (args.Contains("--registry-check")) return RunRegistryCheck();
            if (args.Contains("--describe")) return RunDescribe(NextValue(args, "--describe"));
            if (args.Contains("--call")) return RunCall(args);
            if (args.Contains("--sequence")) return RunSequence(NextValue(args, "--sequence"));
            if (args.Contains("--dump-templates")) return DevTools.DumpTemplates();
            if (args.Contains("--selfcheck")) return DevTools.SelfCheck();

            Console.WriteLine($"❌ 未知参数: {string.Join(' ', args)}");
            PrintHelp();
            return 2;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ 致命错误: {ex.GetType().Name}: {ex.Message}");
            return 2;
        }
    }

    private static void PrintHelp()
    {
        Console.WriteLine("RvCalcCli — RVReducerBasic.dll 通用计算命令行");
        Console.WriteLine("  --list-interfaces                 列出全部接口及参数/输出");
        Console.WriteLine("  --describe <接口名>              查看单个接口详情");
        Console.WriteLine("  --call <接口名> [--args '{...}']  通用计算（--args 支持内联 JSON 或 @文件路径）");
        Console.WriteLine("  --sequence '<json>|@file'         多步同进程调用（有状态依赖的接口，如曲轴系→轴推荐）");
        Console.WriteLine("  --registry-check                  注册表 ↔ DLL 导出核对");
        Console.WriteLine("  --dump-templates                  重新生成 RvCalc/templates/*.json（需工作区）");
        Console.WriteLine("  --selfcheck                       静态 DllImport 与通用调用器结果比对");
        Console.WriteLine();
        Console.WriteLine("示例：");
        Console.WriteLine("  RvCalcCli --call CalBearingLife --args '{\"轴承类型\":\"圆锥滚子\",\"转速\":\"2000 rpm\",\"Cr\":\"23.8 kN\",\"C0r\":\"19.5 kN\",\"径向力Fr\":\"1750 N\",\"e\":0.35,\"Y\":1.7,\"Y0\":0.9}'");
        Console.WriteLine("  RvCalcCli --call CalInvGearStress2 --args @args.json");
    }

    // ═══════════════ 模式 ═══════════════

    private static int RunList()
    {
        var registry = LoadRegistry(out _);
        Console.WriteLine(registry.Describe());
        WarnTemplates(registry);
        return 0;
    }

    private static int RunDescribe(string? name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            Console.WriteLine("用法: --describe <接口名>");
            return 2;
        }
        var registry = LoadRegistry(out _);
        var text = registry.Describe(name);
        Console.WriteLine(text.Length == 0 ? $"未找到接口 {name}" : text);
        return text.Length == 0 ? 1 : 0;
    }

    private static int RunCall(string[] args)
    {
        var registry = LoadRegistry(out _);

        var name = NextValue(args, "--call");
        if (string.IsNullOrWhiteSpace(name))
        {
            Console.WriteLine("用法: --call <接口名> [--args '{...}' | --args @file.json]");
            return 2;
        }

        var argsJson = "{}";
        var raw = NextValue(args, "--args");
        if (!string.IsNullOrWhiteSpace(raw))
        {
            if (raw.StartsWith('@'))
            {
                var file = raw[1..];
                if (!File.Exists(file))
                {
                    Console.WriteLine($"❌ 参数文件不存在: {file}");
                    return 2;
                }
                argsJson = File.ReadAllText(file);
            }
            else argsJson = raw;
        }

        using var runner = new RvCalcRunner(registry);
        Console.WriteLine($"DLL: {runner.Caller.DllPath}");
        var result = runner.RunJson(name, argsJson);
        Console.WriteLine();
        Console.WriteLine(result.ToText());
        return result.Ok ? 0 : 1;
    }

    /// <summary>
    /// 多步同进程调用：某些接口依赖同一进程内的前置计算（DLL 用全局对象保存整机状态）。
    /// 输入格式：{"steps":[{"interface":"CalCrankShaftSystem2","args":{...}},
    ///                        {"interface":"RecommondShaftPara","args":{...}}, ...]}
    /// </summary>
    private static int RunSequence(string? spec)
    {
        if (string.IsNullOrWhiteSpace(spec))
        {
            Console.WriteLine("用法: --sequence '{\"steps\":[{\"interface\":..,\"args\":{..}}, ...]}' （或 @file.json）");
            return 2;
        }

        string json;
        if (spec.StartsWith('@'))
        {
            var file = spec[1..];
            if (!File.Exists(file)) { Console.WriteLine($"❌ 步骤文件不存在: {file}"); return 2; }
            json = File.ReadAllText(file);
        }
        else json = spec;

        var registry = LoadRegistry(out _);
        using var runner = new RvCalcRunner(registry);
        Console.WriteLine($"DLL: {runner.Caller.DllPath}");

        List<(string Interface, string ArgsJson)> steps;
        try { steps = ParseSteps(json); }
        catch (Exception ex) { Console.WriteLine($"❌ 步骤解析失败: {ex.Message}"); return 2; }

        int failures = 0;
        for (int i = 0; i < steps.Count; i++)
        {
            Console.WriteLine();
            Console.WriteLine($"──────── 步骤 {i + 1}/{steps.Count}: {steps[i].Interface} ────────");
            var result = runner.RunJson(steps[i].Interface, steps[i].ArgsJson);
            Console.WriteLine(result.ToText());
            if (!result.Ok) failures++;
        }

        Console.WriteLine();
        Console.WriteLine(failures == 0 ? "✅ 全部步骤完成" : $"⚠ {failures}/{steps.Count} 个步骤未通过");
        return failures == 0 ? 0 : 1;
    }

    /// <summary>解析步骤列表：支持 {"steps":[{"interface":...,"args":{...}}]} 或直接数组</summary>
    private static List<(string Interface, string ArgsJson)> ParseSteps(string json)
    {
        using var doc = JsonDocument.Parse(json, new JsonDocumentOptions
        {
            AllowTrailingCommas = true,
            CommentHandling = JsonCommentHandling.Skip,
        });
        var root = doc.RootElement;
        var array = root.ValueKind == JsonValueKind.Array
            ? root
            : root.TryGetProperty("steps", out var s) && s.ValueKind == JsonValueKind.Array
                ? s
                : throw new FormatException("需要 {\"steps\": [...]} 或数组");

        var steps = new List<(string, string)>();
        foreach (var step in array.EnumerateArray())
        {
            var iface = step.TryGetProperty("interface", out var n) ? n.GetString() : null;
            if (string.IsNullOrWhiteSpace(iface)) throw new FormatException("步骤缺少 interface");
            string argsJson = "{}";
            if (step.TryGetProperty("args", out var a))
                argsJson = a.ValueKind == JsonValueKind.String ? a.GetString() ?? "{}" : a.GetRawText();
            steps.Add((iface!, argsJson));
        }
        return steps;
    }

    private static int RunRegistryCheck()
    {
        var registry = LoadRegistry(out _);
        using var caller = new DynamicDllCaller();

        int problems = 0;
        Console.WriteLine($"DLL: {caller.DllPath}");
        Console.WriteLine();
        foreach (var itf in registry.Interfaces)
        {
            var exists = caller.HasExport(itf.Name);
            var tpl = itf.TemplateFile is null ? "" : (registry.HasTemplate(itf) ? "模板✓" : "模板✗");
            var mark = exists ? "✓" : "✗";
            if (!exists) problems++;
            Console.WriteLine($"  {mark} {itf.Name,-28} 参数{itf.Args.Count} 输出字段{itf.Outputs.Count} {tpl}");
        }

        foreach (var known in new[]
                 {
                     "CalTotalTorqueAndRPM", "CalInvGearTolerence", "CalInvGearStress2", "CalCycloidGearStressByObj",
                     "CalCrankShaftSystem2", "Testfunc", "CalGeometry", "RecommondPara", "GetForceAndForceLocation",
                     "RecommondBearingLife", "RecommondShaftPara", "CalBearingLife", "RecommondMainBearing",
                     "CalCustomBearingLife",
                 })
        {
            if (registry.Find(known) is null)
            {
                Console.WriteLine($"  ⚠ DLL 导出 {known} 未在注册表中登记");
                problems++;
            }
        }

        WarnTemplates(registry);
        Console.WriteLine(problems == 0 ? "✅ 注册表与 DLL 导出一致" : $"⚠ 存在 {problems} 处不一致");
        return problems == 0 ? 0 : 1;
    }

    // ═══════════════ 辅助 ═══════════════

    private static RvInterfaceRegistry LoadRegistry(out string registryPath)
    {
        registryPath = RvInterfaceRegistry.ResolvePath(
                           Path.Combine(AppContext.BaseDirectory, "RvCalc", "interface-registry.json"))
                       ?? throw new FileNotFoundException(
                           "未找到 interface-registry.json（应随项目复制到 <输出目录>/RvCalc/ 下）");

        DllBootstrap.EnsureDlls(out _);
        var registry = RvInterfaceRegistry.Load(registryPath);
        Console.WriteLine($"注册表: {registryPath}");
        return registry;
    }

    private static void WarnTemplates(RvInterfaceRegistry registry)
    {
        foreach (var w in registry.LoadWarnings)
            Console.WriteLine($"   ⚠ {w}");
    }

    private static string? NextValue(string[] args, string flag)
    {
        for (int i = 0; i < args.Length - 1; i++)
            if (args[i] == flag) return args[i + 1];
        return null;
    }
}
