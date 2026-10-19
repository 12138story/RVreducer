<#
.SYNOPSIS
    RV 减速器计算测试运行脚本
.DESCRIPTION
    运行编译产物（Win32 EXE 或通过 C# WPF 应用调用 x64 DLL）并捕获输出。
.PARAMETER Mode
    运行模式: Win32EXE, WpfApp, Auto (默认: Auto 自动检测)
.PARAMETER Configuration
    编译配置: Debug, Release (默认: Debug)
.PARAMETER FilterPattern
    输出过滤正则表达式（用于只提取关键计算结果行）
.PARAMETER OutputFile
    将结果保存到指定文件路径
.EXAMPLE
    .\run_test.ps1 -Mode Win32EXE
    .\run_test.ps1 -Mode WpfApp -Configuration Release
    .\run_test.ps1 -FilterPattern "sigma_H|L10h|寿命"
    .\run_test.ps1 -OutputFile "test_results.txt"
.NOTES
    Win32EXE 模式直接运行控制台程序。
    WpfApp 模式通过 dotnet run 启动 WPF 应用（用于 DLL 测试）。
#>

param(
    [ValidateSet("Win32EXE", "WpfApp", "Auto")]
    [string]$Mode = "Auto",

    [ValidateSet("Debug", "Release")]
    [string]$Configuration = "Debug",

    [string]$FilterPattern = "",
    [string]$OutputFile = ""
)

$ErrorActionPreference = "Stop"
$ScriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$WorkspaceRoot = Resolve-Path (Join-Path $ScriptDir "..\..\..\..")

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  RVReducerBasic 测试运行工具" -ForegroundColor Cyan
Write-Host "  模式: $Mode | 配置: $Configuration" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan

# 自动检测模式
if ($Mode -eq "Auto") {
    $exePath = Join-Path $WorkspaceRoot "RVReducerBasic\$Configuration\RVReducerBasic.exe"
    $dllPath = Join-Path $WorkspaceRoot "RVReducerDesignApp\bin\$Configuration\net8.0-windows\RVReducerBasic.dll"

    if (Test-Path $exePath) {
        $Mode = "Win32EXE"
        Write-Host "自动检测: 发现 Win32 EXE → 使用 Win32EXE 模式" -ForegroundColor Gray
    } elseif (Test-Path $dllPath) {
        $Mode = "WpfApp"
        Write-Host "自动检测: 发现 x64 DLL → 使用 WpfApp 模式" -ForegroundColor Gray
    } else {
        Write-Host "❌ 未找到编译产物。请先运行 build_rv.ps1 编译项目。" -ForegroundColor Red
        Write-Host "💡 提示：如刚才用 Release 编译，请加 -Configuration Release 参数" -ForegroundColor Gray
        Write-Host "   检查路径: $exePath" -ForegroundColor Gray
        Write-Host "   检查路径: $dllPath" -ForegroundColor Gray
        exit 1
    }
}

# 用于收集输出
$stdout = ""
$stderr = ""
$exitCode = 0

switch ($Mode) {
    "Win32EXE" {
        $exePath = Join-Path $WorkspaceRoot "RVReducerBasic\$Configuration\RVReducerBasic.exe"

        if (-not (Test-Path $exePath)) {
            Write-Host "❌ 找不到 EXE: $exePath" -ForegroundColor Red
            exit 1
        }

        Write-Host ">>> 运行 Win32 EXE: $exePath" -ForegroundColor Yellow
        Write-Host ""

        # 运行并捕获输出
        $pinfo = New-Object System.Diagnostics.ProcessStartInfo
        $pinfo.FileName = $exePath
        $pinfo.RedirectStandardOutput = $true
        $pinfo.RedirectStandardError = $true
        $pinfo.UseShellExecute = $false
        $pinfo.CreateNoWindow = $true
        $pinfo.WorkingDirectory = Split-Path $exePath -Parent

        $process = New-Object System.Diagnostics.Process
        $process.StartInfo = $pinfo
        $process.Start() | Out-Null

        $stdout = $process.StandardOutput.ReadToEnd()
        $stderr = $process.StandardError.ReadToEnd()
        $process.WaitForExit()
        $exitCode = $process.ExitCode
    }

    "WpfApp" {
        $csprojPath = Join-Path $WorkspaceRoot "RVReducerDesignApp\RVReducerDesignApp.csproj"

        if (-not (Test-Path $csprojPath)) {
            Write-Host "❌ 找不到 C# 项目: $csprojPath" -ForegroundColor Red
            exit 1
        }

        Write-Host ">>> 通过 dotnet run 启动 WPF 应用: $csprojPath" -ForegroundColor Yellow
        Write-Host "⚠️  WpfApp 模式仅用于手动交互测试，无法自动捕获计算输出。" -ForegroundColor Yellow
        Write-Host "   自动化验证请使用 Win32EXE 模式（msbuild /p:Platform=Win32 编译为控制台 EXE）。" -ForegroundColor Yellow
        Write-Host ""

        # dotnet run
        $proc = Start-Process -FilePath "dotnet" -ArgumentList @(
            "run", "--project", $csprojPath, "--configuration", $Configuration
        ) -NoNewWindow -Wait -PassThru
        $exitCode = $proc.ExitCode
        $stdout = "WPF 应用已启动。输出需要在应用 UI 中查看。"
    }
}

# 应用过滤器
if ($FilterPattern -and $stdout) {
    Write-Host ""
    Write-Host "--- 过滤结果 (模式: $FilterPattern) ---" -ForegroundColor Cyan
    $filtered = $stdout -split "`r?`n" | Select-String -Pattern $FilterPattern
    if ($filtered) {
        $filtered | ForEach-Object { Write-Host $_.Line -ForegroundColor White }
    } else {
        Write-Host "(无匹配行)" -ForegroundColor Gray
    }
}

# 完整输出
Write-Host ""
if ($stdout) {
    Write-Host "--- 标准输出 ---" -ForegroundColor Cyan
    Write-Host $stdout
}
if ($stderr) {
    Write-Host "--- 标准错误 ---" -ForegroundColor Red
    Write-Host $stderr
}

# 保存到文件
if ($OutputFile) {
    $outPath = if ([System.IO.Path]::IsPathRooted($OutputFile)) {
        $OutputFile
    } else {
        Join-Path $WorkspaceRoot $OutputFile
    }
    $stdout | Out-File -FilePath $outPath -Encoding UTF8
    Write-Host "✓ 结果已保存到: $outPath" -ForegroundColor Green
}

# 退出码
Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
if ($exitCode -eq 0) {
    Write-Host "✅ 测试完成 (退出码: $exitCode)" -ForegroundColor Green
} else {
    Write-Host "❌ 测试异常退出 (退出码: $exitCode)" -ForegroundColor Red
}

exit $exitCode
