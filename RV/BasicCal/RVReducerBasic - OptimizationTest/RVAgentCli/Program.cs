using System;
using System.Threading;
using RvCalcCli;
using RVReducerDesignApp.Services.Agent;

int Main()
{
    // 计算 DLL 及 MKL/VTK 依赖就绪（从 RVReducerDesignApp Debug 输出目录整体复制）
    if (!DllBootstrap.EnsureDlls(out _, quiet: true))
    {
        Console.Error.WriteLine("❌ 计算 DLL 依赖缺失：请先编译 RVReducerDesignApp (Debug)。");
        return 2;
    }

    // 控制台程序本就无 WPF 同步上下文，清空以保险（避免 await 续体回 Dispatcher）
    SynchronizationContext.SetSynchronizationContext(null);
    return AgentCliConsole.Run();
}

return Main();
