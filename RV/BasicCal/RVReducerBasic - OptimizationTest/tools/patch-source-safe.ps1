<#
.SYNOPSIS
    编码安全地替换源码字符串（RVReducerBasic 源码编码并不统一：UTF-8 with BOM / GBK 混用）。

.DESCRIPTION
    为什么需要它：
      · RVReducerBasic/*.cpp 并非全部 GBK —— 例如 TANPinionCycloidGear.cpp 是 UTF-8 with BOM。
        若一律按 GBK 读写，会把该文件所有中文行写成乱码（每个中文行都变成 diff）。
      · [Text.Encoding]::GetString() 会把 BOM 解成 U+FEFF，直接 WriteAllText 会写出"双 BOM"。
    本脚本的做法：
      1) 用严格 UTF-8 解码探测：成功 → UTF-8（按原文件有无 BOM 回写）；失败 → GBK(936)
      2) 剥离解码得到的首字符 U+FEFF，避免双 BOM
      3) 每处替换必须命中指定次数（默认 1），否则报错不改文件
      4) 写回前校验无 U+FFFD；写回后打印 git diff 统计与 hunk 头，便于确认改动范围

.PARAMETER Path
    目标文件（相对于当前目录或绝对路径；建议用项目内相对路径）

.PARAMETER Old
    被替换的字面量文本（可多行）

.PARAMETER New
    替换后的文本

.PARAMETER Count
    期望命中次数，默认 1

.PARAMETER PatchFile
    补丁 JSON 文件，内容为数组：[{"old":"...","new":"...","count":1}, ...]
    （与 -Old/-New 二选一；用 JSON 便于把补丁也纳入版本管理）

.PARAMETER NoGitCheck
    跳过 git diff 统计输出

.EXAMPLE
    pwsh tools/patch-source-safe.ps1 -Path RVReducerBasic/TANPinionCycloidGear.cpp -Old "a" -New "b"

.EXAMPLE
    pwsh tools/patch-source-safe.ps1 -Path RVReducerBasic/TANPinionCycloidGear.cpp -PatchFile tools/patches/fix-cycloid.json
#>
[CmdletBinding()]
param(
    [Parameter(Mandatory = $true)][string]$Path,
    [string]$Old,
    [string]$New,
    [int]$Count = 1,
    [string]$PatchFile,
    [switch]$NoGitCheck
)

$ErrorActionPreference = 'Stop'

if (-not (Test-Path $Path)) { throw "目标文件不存在: $Path" }
$full = (Resolve-Path $Path).Path

# ── 1. 探测编码 ──
$raw = [IO.File]::ReadAllBytes($full)
$hasBom = ($raw.Length -ge 3 -and $raw[0] -eq 0xEF -and $raw[1] -eq 0xBB -and $raw[2] -eq 0xBF)
try {
    $encIn = [Text.UTF8Encoding]::new($false, $true)
    $text = $encIn.GetString($raw)
    $encOut = [Text.UTF8Encoding]::new($hasBom)
    Write-Host ("编码: UTF-8{0}" -f ($(if ($hasBom) { ' with BOM' } else { '' })))
}
catch {
    $encIn = [Text.Encoding]::GetEncoding(936)
    $text = $encIn.GetString($raw)
    $encOut = $encIn
    Write-Host '编码: GBK(936)'
}

# GetString() 保留 BOM 为 U+FEFF —— 剥离，否则写出双 BOM
if ($text.Length -gt 0 -and $text[0] -eq [char]0xFEFF) { $text = $text.Substring(1) }

# ── 2. 组织替换列表 ──
$patches = @()
if ($PatchFile) {
    if (-not (Test-Path $PatchFile)) { throw "补丁文件不存在: $PatchFile" }
    $patches = Get-Content -LiteralPath $PatchFile -Raw -Encoding UTF8 | ConvertFrom-Json
    Write-Host ("补丁条目: {0} 条（来自 {1}）" -f @($patches).Count, $PatchFile)
}
else {
    if ($null -eq $Old) { throw '需要 -Old/-New，或使用 -PatchFile' }
    $patches = @([pscustomobject]@{ old = $Old; new = $New; count = $Count })
}

$updated = $text
$applied = 0
foreach ($p in $patches) {
    $o = [string]$p.old
    $n = [string]$p.new
    $c = if ($null -ne $p.count) { [int]$p.count } else { 1 }
    if ([string]::IsNullOrEmpty($o)) { throw '补丁 old 文本为空' }

    $found = ([regex]::Matches($updated, [regex]::Escape($o))).Count
    if ($found -ne $c) {
        throw ("替换未命中预期次数：期望 {0} 次，实际 {1} 次。文件未修改。old 开头：{2}" -f $c, $found, $o.Substring(0, [Math]::Min(60, $o.Length)))
    }
    $updated = $updated.Replace($o, $n)
    $applied++
    Write-Host ("  ✓ 替换 {0} 处（共 {1} 字符 → {2} 字符）" -f $c, $o.Length, $n.Length)
}

# ── 3. 安全检查后写回 ──
if ($updated -ceq $text) { Write-Host '内容无变化，未写文件。'; exit 0 }
if ($updated.Contains([char]0xFFFD)) { throw '替换结果中出现 U+FFFD（编码损坏），已中止，文件未修改' }

[IO.File]::WriteAllText($full, $updated, $encOut)
Write-Host ("✓ 已写回: {0}  ({1} → {2} 字符)" -f $full, $text.Length, $updated.Length)

# ── 4. 变更范围核对 ──
if (-not $NoGitCheck) {
    $repo = Split-Path -Parent $full
    while ($repo -and -not (Test-Path (Join-Path $repo '.git'))) { $repo = Split-Path -Parent $repo }
    if ($repo) {
        Push-Location $repo
        try {
            $rel = [IO.Path]::GetRelativePath($repo, $full).Replace('\', '/')
            Write-Host "── git diff 统计（$rel）──"
            git diff --numstat -- $rel | ForEach-Object { Write-Host "  $_" }
            Write-Host '── 改动 hunk（确认是否只落在目标位置）──'
            git diff -U0 -- $rel | Select-String -Pattern '^@@' | ForEach-Object { Write-Host "  $($_.Line)" }
        }
        finally { Pop-Location }
    }
}
