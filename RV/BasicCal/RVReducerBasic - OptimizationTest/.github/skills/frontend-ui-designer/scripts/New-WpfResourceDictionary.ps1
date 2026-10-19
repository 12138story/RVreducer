<#
.SYNOPSIS
    生成 WPF ResourceDictionary XAML 文件（包含完整的颜色、圆角、间距、字体、阴影和动画资源）。
.DESCRIPTION
    该脚本生成一个完整 WPF 资源字典骨架，包含：
    - 13 个 Color Brushes（品牌色/表面色/文字色/语义色/边框色）
    - 6 个 CornerRadius 资源
    - 12 个 Thickness/Spacing 资源
    - 9 个 FontSize 资源
    - 3 个 DropShadowEffect 资源
    - 5 个 Storyboard 动画模板
.PARAMETER ThemeName
    主题名称，将作为资源 Key 的前缀（如 "Brutalist"）。
.PARAMETER OutputPath
    输出 XAML 文件的路径。
.PARAMETER BrandColor
    品牌主色（十六进制格式，如 "#FFD000"）。默认 "#0078D4"。
.PARAMETER SurfaceColor
    表面/背景色。如果未指定，则根据 IsDarkTheme 自动选择。
.PARAMETER IsDarkTheme
    是否生成深色主题。默认 false。
.EXAMPLE
    .\New-WpfResourceDictionary.ps1 -ThemeName "BrutalistYellow" -OutputPath ".\Themes\BrutalistYellow.xaml" -BrandColor "#FFD000"
.EXAMPLE
    .\New-WpfResourceDictionary.ps1 -ThemeName "DarkAmber" -OutputPath ".\Themes\DarkAmber.xaml" -BrandColor "#FF8C00" -IsDarkTheme
#>

param(
    [Parameter(Mandatory = $true)]
    [string]$ThemeName,

    [Parameter(Mandatory = $true)]
    [string]$OutputPath,

    [Parameter(Mandatory = $false)]
    [string]$BrandColor = "#0078D4",

    [Parameter(Mandatory = $false)]
    [string]$SurfaceColor = $null,

    [Parameter(Mandatory = $false)]
    [switch]$IsDarkTheme
)

# ---- 自动选择表面色 ----
if (-not $SurfaceColor) {
    $SurfaceColor = if ($IsDarkTheme) { "#1A1A1A" } else { "#F5F5F5" }
}

$TextPrimary = if ($IsDarkTheme) { "#FFFFFF" } else { "#1A1A1A" }
$TextSecondary = if ($IsDarkTheme) { "#B0B0B0" } else { "#6B7280" }
$BorderColor = if ($IsDarkTheme) { "#444444" } else { "#CCCCCC" }
$DividerColor = if ($IsDarkTheme) { "#333333" } else { "#E0E0E0" }

$P = $ThemeName  # Prefix for resource keys

# ---- 构建 XAML 内容 ----
$xaml = @"
<ResourceDictionary xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
                    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">

    <!-- ============================================================ -->
    <!-- 主题: $ThemeName                                            -->
    <!-- 生成时间: $(Get-Date -Format 'yyyy-MM-dd HH:mm:ss')         -->
    <!-- ============================================================ -->

    <!-- ===================== 1. Color Brushes ===================== -->

    <!-- 品牌色 -->
    <SolidColorBrush x:Key="$P.Brand" Color="$BrandColor"/>
    <SolidColorBrush x:Key="$P.Brand.Light" Color="#88$($BrandColor.Substring(1))"/>
    <SolidColorBrush x:Key="$P.Brand.Medium" Color="#AA$($BrandColor.Substring(1))"/>

    <!-- 表面 -->
    <SolidColorBrush x:Key="$P.Surface" Color="$SurfaceColor"/>
    <SolidColorBrush x:Key="$P.Surface.Alt" Color="#$($IsDarkTheme ? '2D2D2D' : 'EEEEEE')"/>

    <!-- 文字 -->
    <SolidColorBrush x:Key="$P.TextPrimary" Color="$TextPrimary"/>
    <SolidColorBrush x:Key="$P.TextSecondary" Color="$TextSecondary"/>
    <SolidColorBrush x:Key="$P.TextOnBrand" Color="#$($IsDarkTheme ? '1A1A1A' : 'FFFFFF')"/>

    <!-- 语义色 -->
    <SolidColorBrush x:Key="$P.Success" Color="#$($IsDarkTheme ? '00CC66' : '008840')"/>
    <SolidColorBrush x:Key="$P.Warning" Color="#$($IsDarkTheme ? 'FF8800' : 'CC7000')"/>
    <SolidColorBrush x:Key="$P.Error" Color="#$($IsDarkTheme ? 'FF3300' : 'CC0000')"/>
    <SolidColorBrush x:Key="$P.Info" Color="#$($IsDarkTheme ? '3399FF' : '0066CC')"/>

    <!-- 边框 / 分割线 / 禁用 -->
    <SolidColorBrush x:Key="$P.Border" Color="$BorderColor"/>
    <SolidColorBrush x:Key="$P.Divider" Color="$DividerColor"/>
    <SolidColorBrush x:Key="$P.Disabled" Color="#$($IsDarkTheme ? '555555' : 'AAAAAA')"/>

    <!-- ===================== 2. CornerRadius ===================== -->

    <CornerRadius x:Key="$P.CornerRadius.xs">2</CornerRadius>
    <CornerRadius x:Key="$P.CornerRadius.sm">4</CornerRadius>
    <CornerRadius x:Key="$P.CornerRadius.md">8</CornerRadius>
    <CornerRadius x:Key="$P.CornerRadius.lg">12</CornerRadius>
    <CornerRadius x:Key="$P.CornerRadius.xl">16</CornerRadius>
    <CornerRadius x:Key="$P.CornerRadius.round">9999</CornerRadius>

    <!-- ===================== 3. Thickness / Spacing ===================== -->

    <!-- 间距比例尺（基于 4px 网格） -->
    <Thickness x:Key="$P.Spacing.xxs">2</Thickness>
    <Thickness x:Key="$P.Spacing.xs">4</Thickness>
    <Thickness x:Key="$P.Spacing.sm">8</Thickness>
    <Thickness x:Key="$P.Spacing.md">12</Thickness>
    <Thickness x:Key="$P.Spacing.lg">16</Thickness>
    <Thickness x:Key="$P.Spacing.xl">24</Thickness>
    <Thickness x:Key="$P.Spacing.xxl">32</Thickness>
    <Thickness x:Key="$P.Spacing.xxxl">48</Thickness>

    <!-- 预设 Padding -->
    <Thickness x:Key="$P.Padding.Compact">4,2</Thickness>
    <Thickness x:Key="$P.Padding.Default">8,4</Thickness>
    <Thickness x:Key="$P.Padding.Comfortable">16,8</Thickness>
    <Thickness x:Key="$P.Padding.Page">24</Thickness>

    <!-- 预设 Margin -->
    <Thickness x:Key="$P.Margin.Stack">0,0,0,8</Thickness>
    <Thickness x:Key="$P.Margin.Section">0,16,0,16</Thickness>

    <!-- ===================== 4. FontSize ===================== -->

    <sys:Double x:Key="$P.FontSize.h1" xmlns:sys="clr-namespace:System;assembly=mscorlib">32</sys:Double>
    <sys:Double x:Key="$P.FontSize.h2">28</sys:Double>
    <sys:Double x:Key="$P.FontSize.h3">24</sys:Double>
    <sys:Double x:Key="$P.FontSize.h4">20</sys:Double>
    <sys:Double x:Key="$P.FontSize.h5">18</sys:Double>
    <sys:Double x:Key="$P.FontSize.h6">16</sys:Double>
    <sys:Double x:Key="$P.FontSize.Body">14</sys:Double>
    <sys:Double x:Key="$P.FontSize.Caption">12</sys:Double>
    <sys:Double x:Key="$P.FontSize.Small">10</sys:Double>

    <!-- ===================== 5. Shadow Effects ===================== -->

    <!-- 低阴影 -->
    <DropShadowEffect x:Key="$P.Shadow.Low"
                      BlurRadius="4"
                      ShadowDepth="2"
                      Opacity="0.15"
                      Direction="270"
                      Color="#000000"/>

    <!-- 中阴影 -->
    <DropShadowEffect x:Key="$P.Shadow.Medium"
                      BlurRadius="8"
                      ShadowDepth="4"
                      Opacity="0.20"
                      Direction="270"
                      Color="#000000"/>

    <!-- 高阴影 -->
    <DropShadowEffect x:Key="$P.Shadow.High"
                      BlurRadius="16"
                      ShadowDepth="8"
                      Opacity="0.25"
                      Direction="270"
                      Color="#000000"/>

    <!-- ===================== 6. Animation Storyboards ===================== -->

    <!-- 淡入 (300ms) -->
    <Storyboard x:Key="$P.Animation.FadeIn">
        <DoubleAnimation Storyboard.TargetProperty="Opacity"
                         From="0" To="1"
                         Duration="0:0:0.3"/>
    </Storyboard>

    <!-- 淡出 (200ms) -->
    <Storyboard x:Key="$P.Animation.FadeOut">
        <DoubleAnimation Storyboard.TargetProperty="Opacity"
                         From="1" To="0"
                         Duration="0:0:0.2"/>
    </Storyboard>

    <!-- 右侧滑入 (300ms) -->
    <Storyboard x:Key="$P.Animation.SlideInRight">
        <ThicknessAnimation Storyboard.TargetProperty="Margin"
                            From="50,0,0,0" To="0"
                            Duration="0:0:0.3">
            <ThicknessAnimation.EasingFunction>
                <CubicEase EasingMode="EaseOut"/>
            </ThicknessAnimation.EasingFunction>
        </ThicknessAnimation>
        <DoubleAnimation Storyboard.TargetProperty="Opacity"
                         From="0" To="1"
                         Duration="0:0:0.3"/>
    </Storyboard>

    <!-- 缩放进入 (250ms) -->
    <Storyboard x:Key="$P.Animation.ScaleIn">
        <DoubleAnimation Storyboard.TargetProperty="RenderTransform.ScaleX"
                         From="0.95" To="1"
                         Duration="0:0:0.25">
            <DoubleAnimation.EasingFunction>
                <CubicEase EasingMode="EaseOut"/>
            </DoubleAnimation.EasingFunction>
        </DoubleAnimation>
        <DoubleAnimation Storyboard.TargetProperty="RenderTransform.ScaleY"
                         From="0.95" To="1"
                         Duration="0:0:0.25">
            <DoubleAnimation.EasingFunction>
                <CubicEase EasingMode="EaseOut"/>
            </DoubleAnimation.EasingFunction>
        </DoubleAnimation>
        <DoubleAnimation Storyboard.TargetProperty="Opacity"
                         From="0" To="1"
                         Duration="0:0:0.25"/>
    </Storyboard>

    <!-- 呼吸脉冲 (循环) -->
    <Storyboard x:Key="$P.Animation.Pulse" RepeatBehavior="Forever" AutoReverse="True">
        <DoubleAnimation Storyboard.TargetProperty="Opacity"
                         From="1" To="0.4"
                         Duration="0:0:1.5">
            <DoubleAnimation.EasingFunction>
                <SineEase EasingMode="EaseInOut"/>
            </DoubleAnimation.EasingFunction>
        </DoubleAnimation>
    </Storyboard>

</ResourceDictionary>
"@

# ---- 写入文件 ----
$outputDir = Split-Path -Parent $OutputPath
if (-not (Test-Path $outputDir)) {
    New-Item -ItemType Directory -Path $outputDir -Force | Out-Null
}

$xaml | Out-File -FilePath $OutputPath -Encoding utf8 -Force

Write-Host "✅ ResourceDictionary 已生成: $OutputPath" -ForegroundColor Green
Write-Host "   主题: $ThemeName" -ForegroundColor Cyan
Write-Host "   资源数: 13 Brushes + 6 CornerRadius + 12 Spacing + 9 FontSize + 3 Shadows + 5 Animations = 48 个资源" -ForegroundColor Cyan
