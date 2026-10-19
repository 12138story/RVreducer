<#
.SYNOPSIS
    RV 减速器计算引擎编译脚本
.DESCRIPTION
    使用 msbuild 编译 RVReducerBasic 项目。
    支持 Win32 EXE（控制台测试）和 x64 DLL（C# 互操作）两种目标。
.PARAMETER Target
    编译目标: Win32EXE, x64DLL, Both (默认: Both)
.PARAMETER Configuration
    编译配置: Debug, Release (默认: Debug)
.PARAMETER SolutionPath
    解决方案文件路径 (默认: 当前目录下的 RVReducerBasic.sln)
.PARAMETER ProjectName
    要编译的项目名称 (默认: RVReducerBasic)
.EXAMPLE
    .\build_rv.ps1 -Target Win32EXE
    .\build_rv.ps1 -Target x64DLL -Configuration Release
    .\build_rv.ps1 -Target Both -Configuration Debug
.NOTES
    需要安装 Visual Studio 2022 (v143 工具集) 和 Intel oneAPI MKL。
    msbuild 必须在 PATH 中（从 VS Developer Command Prompt 启动）。
#>

param(
    [ValidateSet("Win32EXE", "x64DLL", "Both")]
    [string]$Target = "Both",

    [ValidateSet("Debug", "Release")]
    [string]$Configuration = "Debug",

    [string]$SolutionPath = "",
    [string]$ProjectName = "RVReducerBasic"
)

$ErrorActionPreference = "Stop"
$ScriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path

# 默认解决方案路径
if (-not $SolutionPath) {
    $SolutionPath = Join-Path $ScriptDir "..\..\..\..\RVReducerBasic.sln"
    $SolutionPath = Resolve-Path $SolutionPath -ErrorAction SilentlyContinue
    if (-not $SolutionPath) {
        Write-Error "找不到 RVReducerBasic.sln，请指定 -SolutionPath 参数"
        exit 1
    }
}

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  RVReducerBasic 编译工具" -ForegroundColor Cyan
Write-Host "  目标: $Target | 配置: $Configuration" -ForegroundColor Cyan
Write-Host "  解决方案: $SolutionPath" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan

# 检查 msbuild 是否可用
$msbuild = Get-Command msbuild -ErrorAction SilentlyContinue
if (-not $msbuild) {
    Write-Host "❌ 未找到 msbuild。请在 VS Developer Command Prompt 中运行此脚本。" -ForegroundColor Red
    Write-Host "   或者从开始菜单启动 'Developer Command Prompt for VS 2022'" -ForegroundColor Yellow
    Write-Host "   或在 PowerShell 中运行: & '`$env:ProgramFiles\Microsoft Visual Studio\2022\*\Common7\Tools\Launch-VsDevShell.ps1'" -ForegroundColor Yellow
    exit 1
}
Write-Host "✓ msbuild 路径: $($msbuild.Source)" -ForegroundColor Green

# 编译函数
function Build-Project {
    param(
        [string]$Platform,
        [string]$Config,
        [string]$Description
    )

    Write-Host ""
    Write-Host ">>> 编译 $Description ($Config|$Platform) ..." -ForegroundColor Yellow

    $args = @(
        $SolutionPath,
        "/t:$ProjectName",
        "/p:Configuration=$Config",
        "/p:Platform=$Platform",
        "/p:GenerateFullPaths=true",
        "/consoleloggerparameters:NoSummary"
    )

    $proc = Start-Process -FilePath "msbuild" -ArgumentList $args -NoNewWindow -Wait -PassThru

    if ($proc.ExitCode -eq 0) {
        Write-Host "✓ $Description 编译成功 (退出码: $($proc.ExitCode))" -ForegroundColor Green
        return $true
    } else {
        Write-Host "✗ $Description 编译失败 (退出码: $($proc.ExitCode))" -ForegroundColor Red
        return $false
    }
}

$successCount = 0
$failCount = 0

# Win32 EXE 编译
if ($Target -eq "Win32EXE" -or $Target -eq "Both") {
    $result = Build-Project -Platform "Win32" -Config $Configuration -Description "Win32 控制台 EXE"
    if ($result) {
        $successCount++
        # 确认产物
        $exePath = Join-Path $ScriptDir "..\..\..\..\RVReducerBasic\$Configuration\RVReducerBasic.exe"
        if (Test-Path $exePath) {
            Write-Host "  产物: $exePath" -ForegroundColor Gray
        }
    } else {
        $failCount++
    }
}

# x64 DLL 编译
if ($Target -eq "x64DLL" -or $Target -eq "Both") {
    $result = Build-Project -Platform "x64" -Config $Configuration -Description "x64 DLL"
    if ($result) {
        $successCount++
        # 确认产物 (DLL 输出到 WPF 应用的 bin 目录)
        $dllPath = Join-Path $ScriptDir "..\..\..\..\RVReducerDesignApp\bin\$Configuration\net8.0-windows\RVReducerBasic.dll"
        if (Test-Path $dllPath) {
            Write-Host "  产物: $dllPath" -ForegroundColor Gray
        }
    } else {
        $failCount++
    }
}

# 汇总
Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
if ($failCount -eq 0) {
    Write-Host "✅ 全部编译成功 ($successCount 个目标)" -ForegroundColor Green
    exit 0
} else {
    Write-Host "❌ 编译完成: $successCount 成功, $failCount 失败" -ForegroundColor Red
    exit 1
}
