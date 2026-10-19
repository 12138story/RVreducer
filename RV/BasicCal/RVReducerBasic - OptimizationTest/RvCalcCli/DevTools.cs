using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using RvCalc;

namespace RvCalcCli;

/// <summary>开发/维护类子命令：生成输入模板、静态与通用调用器的结果自检。</summary>
internal static class DevTools
{
    private static readonly JsonSerializerOptions JsonOpts = new()
    {
        WriteIndented = true,
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        NumberHandling = JsonNumberHandling.AllowNamedFloatingPointLiterals,
    };

    /// <summary>
    /// 重新生成 RvCalc/templates/*.json。
    /// 模板来自 RvCalc 内的输入构建器（InvGearInput / CycloidGearInput），与既有算例保持一致。
    /// </summary>
    public static int DumpTemplates()
    {
        var wsRoot = DllBootstrap.FindWorkspaceRoot();
        if (wsRoot is null)
        {
            Console.WriteLine("❌ 未找到工作区根目录（RVReducerBasic.sln），--dump-templates 需在工作区内运行");
            return 2;
        }

        var dir = Path.Combine(wsRoot, "RvCalc", "templates");
        Directory.CreateDirectory(dir);
        Console.WriteLine($"模板输出目录: {dir}");

        // ① 渐开线齿轮副（CalInvGearStress2）：174 输入 + 34 分割索引
        var invGear = new InvGearInput();
        var invArray = invGear.ToArray();
        Write(dir, "invGearStress2.json", new Dictionary<string, double[]>
        {
            ["div"] = InvGearInput.GetDivideIndex().Select(v => (double)v).ToArray(),
            ["in"] = invArray,
        });

        // ② 渐开线几何（CalGeometry）：取 0..70，压力角/螺旋角按角度传入
        var geometry = new double[71];
        Array.Copy(invArray, geometry, 71);
        geometry[63] = invGear.公共几何[1];
        geometry[64] = invGear.公共几何[2];
        Write(dir, "calGeometry.json", new Dictionary<string, double[]> { ["in"] = geometry });

        // ③ 摆线针轮副（CalCycloidGearStressByObj）：60 输入 + 5 分割索引
        var cycloid = new CycloidGearInput();
        var cycloidArray = cycloid.ToArray();
        cycloidArray[25] *= cycloid.TorqueScale; // 力矩按 DLL 量纲（N·mm）存入模板
        cycloidArray[45] *= cycloid.TorqueScale;
        Write(dir, "cycloid.json", new Dictionary<string, double[]>
        {
            ["div"] = CycloidGearInput.GetDivideIndex().Select(v => (double)v).ToArray(),
            ["in"] = cycloidArray,
        });

        Console.WriteLine("✅ 已生成 3 个模板：invGearStress2 / calGeometry / cycloid");
        Console.WriteLine("   （CalCrankShaftSystem2 / RecommondShaftPara 需要整机输入快照，见注册表 Note）");
        return 0;
    }

    /// <summary>同一输入分别走静态 DllImport 与通用动态调用器，比对结果是否一致。</summary>
    public static int SelfCheck()
    {
        DllBootstrap.EnsureDlls(out _);

        // ── 渐开线齿轮副 ──
        var input = new InvGearInput();
        input.法向模数 = 5;
        input.太阳轮几何[0] = 20;
        input.行星轮几何[0] = 106;
        input.中心距 = (20 + 106) * 5 / 2.0;
        input.扭矩 = 500;
        input.转速 = 1000;
        input.预期寿命 = 20000;

        var arr = input.ToArray();
        var div = InvGearInput.GetDivideIndex();

        Console.WriteLine("── 渐开线齿轮副（静态 DllImport）──");
        var staticOut = StaticProbe.InvGearStress2(div, arr);
        Console.WriteLine($"  太阳轮 σH={staticOut[1]:F4}  σF={staticOut[4]:F4}  mt={staticOut[8]:F4}  d={staticOut[21]:F4}");

        var registry = LoadRegistry();
        var desc = registry.Find("CalInvGearStress2")!;
        using var caller = new DynamicDllCaller();
        var raw = caller.Invoke(desc, new Dictionary<string, object>
        {
            ["div"] = (int[])div.Clone(),
            ["in"] = (double[])arr.Clone(),
            ["out"] = new double[284],
        });
        var dynOut = raw.Doubles["out"];
        Console.WriteLine("── 渐开线齿轮副（通用动态调用器）──");
        Console.WriteLine($"  太阳轮 σH={dynOut[1]:F4}  σF={dynOut[4]:F4}  mt={dynOut[8]:F4}  d={dynOut[21]:F4}");

        bool same = Math.Abs(staticOut[1] - dynOut[1]) < 1e-6 && Math.Abs(staticOut[21] - dynOut[21]) < 1e-6;

        // ── 轴承寿命 ──
        // 注意：类型码取 C++ PreDefine.h 的 100x（1003=圆锥滚子），传 0..4 会落到 else 分支不写输出
        var bearingIn = new double[] { 1003, 2000, 740, 1750, 23800, 19500, 0.35, 1.7, 0.9 };
        Console.WriteLine("── 轴承寿命（静态 DllImport）──");
        Console.WriteLine($"  L10h={StaticProbe.BearingLife((double[])bearingIn.Clone())[0]:G6} h");

        var bDesc = registry.Find("CalBearingLife")!;
        var bRaw = caller.Invoke(bDesc, new Dictionary<string, object>
        {
            ["in"] = (double[])bearingIn.Clone(),
            ["out"] = new double[20],
        });
        Console.WriteLine("── 轴承寿命（通用动态调用器）──");
        Console.WriteLine($"  L10h={bRaw.Doubles["out"][0]:G6} h");

        Console.WriteLine(same ? "✅ 两条路径结果一致" : "❌ 两条路径结果不一致（问题在动态调用器/封送）");
        return same ? 0 : 1;
    }

    private static RvInterfaceRegistry LoadRegistry()
    {
        var path = RvInterfaceRegistry.ResolvePath(
            Path.Combine(AppContext.BaseDirectory, "RvCalc", "interface-registry.json"))
            ?? throw new FileNotFoundException("未找到 interface-registry.json");
        return RvInterfaceRegistry.Load(path);
    }

    private static void Write(string dir, string fileName, Dictionary<string, double[]> payload)
    {
        var path = Path.Combine(dir, fileName);
        File.WriteAllText(path, JsonSerializer.Serialize(payload, JsonOpts));
        var summary = string.Join(", ", payload.Select(kv => $"{kv.Key}[{kv.Value.Length}]"));
        Console.WriteLine($"   写入 {fileName}  ({summary})");
    }
}
