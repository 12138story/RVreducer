# WPF 配色方案模板

> 5 种独特色彩主题，避免千篇一律的 AI 美学。
> 每种主题包含完整的 WPF `SolidColorBrush` 资源定义，可直接复制到 `App.xaml` 或独立资源字典中使用。

---

## 1. Brutalist Industrial（工业粗野主义）

**氛围**：高对比、粗犷、工业感
**适用场景**：加工参数面板、设备控制界面、工厂监控

```xml
<!-- BrutalistIndustrial.xaml -->
<ResourceDictionary xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
                    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">
    <!-- 品牌色 -->
    <SolidColorBrush x:Key="Brutalist.Brand" Color="#FFD000"/>
    <SolidColorBrush x:Key="Brutalist.Brand.Light" Color="#FFE54C"/>
    <SolidColorBrush x:Key="Brutalist.Brand.Medium" Color="#CCA600"/>

    <!-- 表面 -->
    <SolidColorBrush x:Key="Brutalist.Surface" Color="#1A1A1A"/>
    <SolidColorBrush x:Key="Brutalist.Surface.Alt" Color="#2D2D2D"/>

    <!-- 文字 -->
    <SolidColorBrush x:Key="Brutalist.TextPrimary" Color="#FFFFFF"/>
    <SolidColorBrush x:Key="Brutalist.TextSecondary" Color="#B0B0B0"/>
    <SolidColorBrush x:Key="Brutalist.TextOnBrand" Color="#1A1A1A"/>

    <!-- 语义色 -->
    <SolidColorBrush x:Key="Brutalist.Success" Color="#00CC66"/>
    <SolidColorBrush x:Key="Brutalist.Warning" Color="#FF8800"/>
    <SolidColorBrush x:Key="Brutalist.Error" Color="#FF3300"/>
    <SolidColorBrush x:Key="Brutalist.Info" Color="#3399FF"/>

    <!-- 边框 -->
    <SolidColorBrush x:Key="Brutalist.Border" Color="#FFFFFF"/>
    <SolidColorBrush x:Key="Brutalist.Divider" Color="#444444"/>
    <SolidColorBrush x:Key="Brutalist.Disabled" Color="#555555"/>
</ResourceDictionary>
```

---

## 2. Swiss Precision（瑞士精密）

**氛围**：精确、干净、专业
**适用场景**：测量数据展示、精密仪器界面、技术文档

```xml
<!-- SwissPrecision.xaml -->
<ResourceDictionary xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
                    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">
    <!-- 品牌色 -->
    <SolidColorBrush x:Key="Swiss.Brand" Color="#E60000"/>
    <SolidColorBrush x:Key="Swiss.Brand.Light" Color="#FF4444"/>
    <SolidColorBrush x:Key="Swiss.Brand.Medium" Color="#B80000"/>

    <!-- 表面 -->
    <SolidColorBrush x:Key="Swiss.Surface" Color="#F8F8F8"/>
    <SolidColorBrush x:Key="Swiss.Surface.Alt" Color="#EEEEEE"/>

    <!-- 文字 -->
    <SolidColorBrush x:Key="Swiss.TextPrimary" Color="#1A1A1A"/>
    <SolidColorBrush x:Key="Swiss.TextSecondary" Color="#666666"/>
    <SolidColorBrush x:Key="Swiss.TextOnBrand" Color="#FFFFFF"/>

    <!-- 语义色 -->
    <SolidColorBrush x:Key="Swiss.Success" Color="#008840"/>
    <SolidColorBrush x:Key="Swiss.Warning" Color="#CC7000"/>
    <SolidColorBrush x:Key="Swiss.Error" Color="#CC0000"/>
    <SolidColorBrush x:Key="Swiss.Info" Color="#0066CC"/>

    <!-- 边框 -->
    <SolidColorBrush x:Key="Swiss.Border" Color="#333333"/>
    <SolidColorBrush x:Key="Swiss.Divider" Color="#CCCCCC"/>
    <SolidColorBrush x:Key="Swiss.Disabled" Color="#AAAAAA"/>
</ResourceDictionary>
```

---

## 3. Dark Academia（深色学院）

**氛围**：沉稳、学术、典雅
**适用场景**：报告生成、文档管理、知识库界面

```xml
<!-- DarkAcademia.xaml -->
<ResourceDictionary xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
                    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">
    <!-- 品牌色 -->
    <SolidColorBrush x:Key="Academia.Brand" Color="#C5A55A"/>
    <SolidColorBrush x:Key="Academia.Brand.Light" Color="#D4BB7A"/>
    <SolidColorBrush x:Key="Academia.Brand.Medium" Color="#A88A3E"/>

    <!-- 表面 -->
    <SolidColorBrush x:Key="Academia.Surface" Color="#2C2C2C"/>
    <SolidColorBrush x:Key="Academia.Surface.Alt" Color="#3A3A3A"/>

    <!-- 文字 -->
    <SolidColorBrush x:Key="Academia.TextPrimary" Color="#E8E0D0"/>
    <SolidColorBrush x:Key="Academia.TextSecondary" Color="#A09880"/>
    <SolidColorBrush x:Key="Academia.TextOnBrand" Color="#1A1A1A"/>

    <!-- 语义色 -->
    <SolidColorBrush x:Key="Academia.Success" Color="#7AA86A"/>
    <SolidColorBrush x:Key="Academia.Warning" Color="#C5A040"/>
    <SolidColorBrush x:Key="Academia.Error" Color="#B84A3A"/>
    <SolidColorBrush x:Key="Academia.Info" Color="#6A8AB8"/>

    <!-- 边框 -->
    <SolidColorBrush x:Key="Academia.Border" Color="#5C4A32"/>
    <SolidColorBrush x:Key="Academia.Divider" Color="#4A3A28"/>
    <SolidColorBrush x:Key="Academia.Disabled" Color="#5A5A5A"/>
</ResourceDictionary>
```

---

## 4. Tech Noir（赛博暗夜）

**氛围**：科技感、未来、霓虹
**适用场景**：数据仪表盘、监控大屏、实时数据展示

```xml
<!-- TechNoir.xaml -->
<ResourceDictionary xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
                    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">
    <!-- 品牌色 -->
    <SolidColorBrush x:Key="TechNoir.Brand" Color="#00FFF0"/>
    <SolidColorBrush x:Key="TechNoir.Brand.Light" Color="#66FFF5"/>
    <SolidColorBrush x:Key="TechNoir.Brand.Medium" Color="#00CCBF"/>

    <!-- 表面 -->
    <SolidColorBrush x:Key="TechNoir.Surface" Color="#0D0D1A"/>
    <SolidColorBrush x:Key="TechNoir.Surface.Alt" Color="#1A1A33"/>

    <!-- 文字 -->
    <SolidColorBrush x:Key="TechNoir.TextPrimary" Color="#E0E0FF"/>
    <SolidColorBrush x:Key="TechNoir.TextSecondary" Color="#8888AA"/>
    <SolidColorBrush x:Key="TechNoir.TextOnBrand" Color="#0D0D1A"/>

    <!-- 语义色 -->
    <SolidColorBrush x:Key="TechNoir.Success" Color="#00FF88"/>
    <SolidColorBrush x:Key="TechNoir.Warning" Color="#FFAA00"/>
    <SolidColorBrush x:Key="TechNoir.Error" Color="#FF3366"/>
    <SolidColorBrush x:Key="TechNoir.Info" Color="#00CCFF"/>

    <!-- 边框 -->
    <SolidColorBrush x:Key="TechNoir.Border" Color="#333366"/>
    <SolidColorBrush x:Key="TechNoir.Divider" Color="#222244"/>
    <SolidColorBrush x:Key="TechNoir.Disabled" Color="#444466"/>
</ResourceDictionary>
```

---

## 5. Nature-Grounded（大地质感）

**氛围**：自然、舒适、柔和
**适用场景**：环保/农业类应用、舒适工作模式、长时间使用的界面

```xml
<!-- NatureGrounded.xaml -->
<ResourceDictionary xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
                    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">
    <!-- 品牌色 -->
    <SolidColorBrush x:Key="Nature.Brand" Color="#4A7C59"/>
    <SolidColorBrush x:Key="Nature.Brand.Light" Color="#6B9E7A"/>
    <SolidColorBrush x:Key="Nature.Brand.Medium" Color="#3A6A49"/>

    <!-- 表面 -->
    <SolidColorBrush x:Key="Nature.Surface" Color="#F5F0EB"/>
    <SolidColorBrush x:Key="Nature.Surface.Alt" Color="#EDE6DC"/>

    <!-- 文字 -->
    <SolidColorBrush x:Key="Nature.TextPrimary" Color="#2D2D2D"/>
    <SolidColorBrush x:Key="Nature.TextSecondary" Color="#6B6B6B"/>
    <SolidColorBrush x:Key="Nature.TextOnBrand" Color="#FFFFFF"/>

    <!-- 语义色 -->
    <SolidColorBrush x:Key="Nature.Success" Color="#5A8A5A"/>
    <SolidColorBrush x:Key="Nature.Warning" Color="#D4A574"/>
    <SolidColorBrush x:Key="Nature.Error" Color="#B85A4A"/>
    <SolidColorBrush x:Key="Nature.Info" Color="#6A8A9A"/>

    <!-- 边框 -->
    <SolidColorBrush x:Key="Nature.Border" Color="#8C7A6B"/>
    <SolidColorBrush x:Key="Nature.Divider" Color="#D0C8B8"/>
    <SolidColorBrush x:Key="Nature.Disabled" Color="#B0A898"/>
</ResourceDictionary>
```

---

## 在 App.xaml 中使用

```xml
<Application.Resources>
    <ResourceDictionary>
        <ResourceDictionary.MergedDictionaries>
            <!-- 选择一种主题 -->
            <ResourceDictionary Source="Themes/BrutalistIndustrial.xaml"/>
            <!-- 或 -->
            <!-- <ResourceDictionary Source="Themes/SwissPrecision.xaml"/> -->
            <!-- 或 -->
            <!-- <ResourceDictionary Source="Themes/DarkAcademia.xaml"/> -->
            <!-- 或 -->
            <!-- <ResourceDictionary Source="Themes/TechNoir.xaml"/> -->
            <!-- 或 -->
            <!-- <ResourceDictionary Source="Themes/NatureGrounded.xaml"/> -->

            <!-- HandyControl 主题（建议在自定义主题之后加载） -->
            <ResourceDictionary Source="pack://application:,,,/HandyControl;component/Themes/SkinDefault.xaml"/>
            <ResourceDictionary Source="pack://application:,,,/HandyControl;component/Themes/Theme.xaml"/>
        </ResourceDictionary.MergedDictionaries>
    </ResourceDictionary>
</Application.Resources>
```

> **注意**：自定义主题与 HandyControl 主题的加载顺序会影响最终效果。通常先加载自定义主题（基础色），再加载 HandyControl 主题（控件默认样式），这样 HandyControl 的控件可以使用自定义主题中定义的 Brush 资源。
