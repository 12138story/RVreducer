<#
.SYNOPSIS
    Generates a WPF ResourceDictionary XAML file with a full design theme skeleton.

.DESCRIPTION
    This script creates a complete ResourceDictionary file for WPF theming,
    including color brushes, corner radii, spacing/thickness resources,
    font sizes, shadow effects, and animation storyboard templates.

.PARAMETER ThemeName
    The name of the theme (e.g., "Light", "Dark", "Blue", "Custom").
    This is used for the x:Key and comments in the generated file.

.PARAMETER OutputPath
    The full path where the generated XAML file will be saved.

.PARAMETER BrandColor
    The primary brand color in hex (e.g., "#0078D4"). Defaults to a blue tone.

.PARAMETER SurfaceColor
    The surface/background color in hex. Defaults to "#FFFFFF" (Light) or "#1E1E1E" (Dark).

.PARAMETER IsDarkTheme
    Switch to indicate this is a dark theme, which changes default surface/text colors.

.EXAMPLE
    .\New-WpfResourceDictionary.ps1 -ThemeName "LightBlue" -OutputPath ".\Themes\LightBlue.xaml"

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
    [string]$SurfaceColor,

    [Parameter(Mandatory = $false)]
    [switch]$IsDarkTheme
)

# ──────────────────────────────────────────────────────────────
# 1. Resolve default colors based on theme mode
# ──────────────────────────────────────────────────────────────
if (-not $SurfaceColor) {
    $SurfaceColor = if ($IsDarkTheme) { "#1E1E1E" } else { "#FFFFFF" }
}

$textPrimary   = if ($IsDarkTheme) { "#F0F0F0" } else { "#1A1A1A" }
$textSecondary = if ($IsDarkTheme) { "#A0A0A0" } else { "#666666" }

# ──────────────────────────────────────────────────────────────
# 2. Derive shade variants for the brand color
# ──────────────────────────────────────────────────────────────
# Simple lighten/darken by appending alpha or using fixed tints
$brandLight = $BrandColor + "33"   # 20 % opacity
$brandMedium = $BrandColor + "80"  # 50 % opacity

# ──────────────────────────────────────────────────────────────
# 3. Build the XAML content
# ──────────────────────────────────────────────────────────────
$timestamp = Get-Date -Format "yyyy-MM-dd HH:mm:ss"

@"
<ResourceDictionary xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
                    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
                    xmlns:media="clr-namespace:System.Windows.Media;assembly=PresentationCore">

    <!--
    ============================================================
    Theme : $ThemeName
    Generated : $timestamp
    ============================================================
    -->

    <!-- ===== 1. Color Brushes ===== -->
    <SolidColorBrush x:Key="$($ThemeName).Brand"           Color="$BrandColor" />
    <SolidColorBrush x:Key="$($ThemeName).Brand.Light"     Color="$brandLight" />
    <SolidColorBrush x:Key="$($ThemeName).Brand.Medium"    Color="$brandMedium" />
    <SolidColorBrush x:Key="$($ThemeName).Surface"         Color="$SurfaceColor" />
    <SolidColorBrush x:Key="$($ThemeName).Surface.Alt"     Color="$((Get-Date).ToString('ignored'))" />
    <SolidColorBrush x:Key="$($ThemeName).TextPrimary"     Color="$textPrimary" />
    <SolidColorBrush x:Key="$($ThemeName).TextSecondary"   Color="$textSecondary" />
    <SolidColorBrush x:Key="$($ThemeName).TextOnBrand"     Color="#FFFFFF" />
    <SolidColorBrush x:Key="$($ThemeName).Success"         Color="#2E7D32" />
    <SolidColorBrush x:Key="$($ThemeName).Warning"         Color="#F9A825" />
    <SolidColorBrush x:Key="$($ThemeName).Error"           Color="#D32F2F" />
    <SolidColorBrush x:Key="$($ThemeName).Info"            Color="#1976D2" />
    <SolidColorBrush x:Key="$($ThemeName).Divider"         Color="#E0E0E0" />
    <SolidColorBrush x:Key="$($ThemeName).Disabled"        Color="#9E9E9E" />

    <!-- ===== 2. CornerRadius Resources ===== -->
    <!-- Based on a 4 px design grid -->
    <CornerRadius x:Key="$($ThemeName).CornerRadius.xs">2</CornerRadius>
    <CornerRadius x:Key="$($ThemeName).CornerRadius.sm">4</CornerRadius>
    <CornerRadius x:Key="$($ThemeName).CornerRadius.md">8</CornerRadius>
    <CornerRadius x:Key="$($ThemeName).CornerRadius.lg">12</CornerRadius>
    <CornerRadius x:Key="$($ThemeName).CornerRadius.xl">16</CornerRadius>
    <CornerRadius x:Key="$($ThemeName).CornerRadius.round">9999</CornerRadius>

    <!-- ===== 3. Thickness / Spacing Resources (4 px grid) ===== -->
    <Thickness x:Key="$($ThemeName).Spacing.xxs">2</Thickness>
    <Thickness x:Key="$($ThemeName).Spacing.xs">4</Thickness>
    <Thickness x:Key="$($ThemeName).Spacing.sm">8</Thickness>
    <Thickness x:Key="$($ThemeName).Spacing.md">12</Thickness>
    <Thickness x:Key="$($ThemeName).Spacing.lg">16</Thickness>
    <Thickness x:Key="$($ThemeName).Spacing.xl">24</Thickness>
    <Thickness x:Key="$($ThemeName).Spacing.xxl">32</Thickness>
    <Thickness x:Key="$($ThemeName).Spacing.xxxl">48</Thickness>

    <!-- Padding presets -->
    <Thickness x:Key="$($ThemeName).Padding.Compact">4,2</Thickness>
    <Thickness x:Key="$($ThemeName).Padding.Default">8,4</Thickness>
    <Thickness x:Key="$($ThemeName).Padding.Comfortable">12,8</Thickness>
    <Thickness x:Key="$($ThemeName).Padding.Page">24,16</Thickness>

    <!-- Margin presets -->
    <Thickness x:Key="$($ThemeName).Margin.Stack">0,0,0,8</Thickness>
    <Thickness x:Key="$($ThemeName).Margin.Section">0,0,0,24</Thickness>

    <!-- ===== 4. FontSize Resources ===== -->
    <sys:Double xmlns:sys="clr-namespace:System;assembly=mscorlib"
                x:Key="$($ThemeName).FontSize.h1">32</sys:Double>
    <sys:Double xmlns:sys="clr-namespace:System;assembly=mscorlib"
                x:Key="$($ThemeName).FontSize.h2">28</sys:Double>
    <sys:Double xmlns:sys="clr-namespace:System;assembly=mscorlib"
                x:Key="$($ThemeName).FontSize.h3">24</sys:Double>
    <sys:Double xmlns:sys="clr-namespace:System;assembly=mscorlib"
                x:Key="$($ThemeName).FontSize.h4">20</sys:Double>
    <sys:Double xmlns:sys="clr-namespace:System;assembly=mscorlib"
                x:Key="$($ThemeName).FontSize.h5">18</sys:Double>
    <sys:Double xmlns:sys="clr-namespace:System;assembly=mscorlib"
                x:Key="$($ThemeName).FontSize.h6">16</sys:Double>
    <sys:Double xmlns:sys="clr-namespace:System;assembly=mscorlib"
                x:Key="$($ThemeName).FontSize.Body">14</sys:Double>
    <sys:Double xmlns:sys="clr-namespace:System;assembly=mscorlib"
                x:Key="$($ThemeName).FontSize.Caption">12</sys:Double>
    <sys:Double xmlns:sys="clr-namespace:System;assembly=mscorlib"
                x:Key="$($ThemeName).FontSize.Small">10</sys:Double>

    <!-- ===== 5. Shadow Effect Resources ===== -->
    <!-- Low elevation (cards, buttons) -->
    <DropShadowEffect x:Key="$($ThemeName).Shadow.Low"
                      BlurRadius="4"
                      Direction="270"
                      Opacity="0.15"
                      ShadowDepth="2"
                      Color="#000000" />

    <!-- Medium elevation (popups, dropdowns) -->
    <DropShadowEffect x:Key="$($ThemeName).Shadow.Medium"
                      BlurRadius="8"
                      Direction="270"
                      Opacity="0.20"
                      ShadowDepth="4"
                      Color="#000000" />

    <!-- High elevation (dialogs, modals) -->
    <DropShadowEffect x:Key="$($ThemeName).Shadow.High"
                      BlurRadius="16"
                      Direction="270"
                      Opacity="0.25"
                      ShadowDepth="8"
                      Color="#000000" />

    <!-- ===== 6. Animation Storyboard Templates ===== -->

    <!-- Fade In -->
    <Storyboard x:Key="$($ThemeName).Animation.FadeIn">
        <DoubleAnimation Storyboard.TargetProperty="Opacity"
                         From="0"
                         To="1"
                         Duration="0:0:0.3"
                         FillBehavior="HoldEnd" />
    </Storyboard>

    <!-- Fade Out -->
    <Storyboard x:Key="$($ThemeName).Animation.FadeOut">
        <DoubleAnimation Storyboard.TargetProperty="Opacity"
                         From="1"
                         To="0"
                         Duration="0:0:0.2"
                         FillBehavior="HoldEnd" />
    </Storyboard>

    <!-- Slide In From Right -->
    <Storyboard x:Key="$($ThemeName).Animation.SlideInRight">
        <ThicknessAnimation Storyboard.TargetProperty="Margin"
                            From="200,0,0,0"
                            To="0"
                            Duration="0:0:0.3"
                            DecelerationRatio="0.2"
                            FillBehavior="HoldEnd" />
        <DoubleAnimation Storyboard.TargetProperty="Opacity"
                         From="0"
                         To="1"
                         Duration="0:0:0.3"
                         FillBehavior="HoldEnd" />
    </Storyboard>

    <!-- Scale In (pop-in effect) -->
    <Storyboard x:Key="$($ThemeName).Animation.ScaleIn">
        <DoubleAnimation Storyboard.TargetProperty="RenderTransform.ScaleX"
                         From="0.9"
                         To="1"
                         Duration="0:0:0.25"
                         DecelerationRatio="0.2"
                         FillBehavior="HoldEnd" />
        <DoubleAnimation Storyboard.TargetProperty="RenderTransform.ScaleY"
                         From="0.9"
                         To="1"
                         Duration="0:0:0.25"
                         DecelerationRatio="0.2"
                         FillBehavior="HoldEnd" />
        <DoubleAnimation Storyboard.TargetProperty="Opacity"
                         From="0"
                         To="1"
                         Duration="0:0:0.25"
                         FillBehavior="HoldEnd" />
    </Storyboard>

    <!-- Pulse (attention attractor) -->
    <Storyboard x:Key="$($ThemeName).Animation.Pulse"
                AutoReverse="True"
                RepeatBehavior="Forever">
        <DoubleAnimation Storyboard.TargetProperty="Opacity"
                         From="1"
                         To="0.5"
                         Duration="0:0:0.8"
                         FillBehavior="HoldEnd" />
    </Storyboard>

</ResourceDictionary>
"@ | Out-File -FilePath $OutputPath -Encoding utf8

Write-Host "✅ ResourceDictionary generated successfully!" -ForegroundColor Green
Write-Host "   Theme   : $ThemeName" -ForegroundColor Cyan
Write-Host "   Output  : $OutputPath" -ForegroundColor Cyan
Write-Host "   Mode    : $(if ($IsDarkTheme) { 'Dark' } else { 'Light' })" -ForegroundColor Cyan
