---
name: frontend-ui-designer
description: '打造具有高设计质量的独特 WPF 桌面前端界面。使用 WPF/XAML/HandyControl 构建页面、组件、仪表盘、设置面板、数据展示界面。生成富有创意、精致的 WPF UI，避免千篇一律的默认风格，强调独特的设计美学与生产级质量。可与 implementer/review/planner/action agent 协作。'
argument-hint: '描述你想要构建的 WPF 页面或组件'
user-invocable: true
---

# Frontend UI Designer — WPF 前端界面设计技能

## 何时使用
- 用户要求构建或美化 WPF 桌面应用界面
- 需要设计 WPF 页面、自定义控件、数据展示面板、设置界面
- 需要为 HandyControl 项目设计主题或自定义样式
- 需要打破 WPF 默认的"灰色矩形"美学，追求独特设计
- 用户要求创建 HTML/CSS/React 等 Web 前端时也可以使用（通过 agent 交接）

## 核心设计哲学

### 1. 设计系统驱动（Design System First）
在写任何 XAML 之前，先定义核心设计 Token（在 `App.xaml` 或资源字典中）：

- **色彩系统**：在 `SolidColorBrush` 资源中定义主色/辅色/中性色/语义色
  ```xml
  <SolidColorBrush x:Key="BrandBrush" Color="#FF326CF3"/>
  <SolidColorBrush x:Key="SurfaceBrush" Color="#FFF5F5F5"/>
  <SolidColorBrush x:Key="TextPrimaryBrush" Color="#FF1A1A2E"/>
  <SolidColorBrush x:Key="TextSecondaryBrush" Color="#FF6B7280"/>
  ```
- **圆角系统**：`CornerRadius` 资源（sm=4/md=8/lg=12/xl=16）
- **阴影系统**：`DropShadowEffect` 资源定义低/中/高 elevation
- **间距系统**：`Thickness` 资源（基于 4px 网格）
- **动效系统**：`Storyboard` + `EasingFunctionBase` 定义缓动曲线
- **排版层级**：`FontSize`/`FontWeight` 资源定义标题/正文/辅助文字层级

### 2. 反同质化视觉（Anti-Generic WPF Aesthetic）
避免以下 WPF 常见陷阱：
- ❌ 默认无样式的灰色矩形控件
- ❌ 千篇一律的浅灰背景 + 深灰文字
- ❌ 纯数据堆砌而无视觉层次
- ❌ 完全不使用任何动效
- ❌ HandyControl 默认主题不做任何自定义
- ✅ **拥抱设计运动**：借鉴 Brutalism（粗边框+高对比）、Swiss Design（精确网格+红色强调）、Neumorphism（柔和浮雕）、Dark Academia（深色+暖金+衬线）等具体美学
- ✅ **纹理与质感**：使用 DrawingBrush/VisualBrush 添加网格纹理、点阵背景；使用 LinearGradientBrush 增加深度
- ✅ **排版驱动**：善用 FontWeight 对比、字符间距、搭配中英文字体
- ✅ **非对称布局**：打破 Grid 的均分，使用负空间创造视觉张力
- ✅ **自定义 HandyControl 主题**：覆盖 SkinDefault 中的 Brush 资源改变整体调性

### 3. 生产级质量（WPF 特有）
- **可访问性**：AutomationProperties、键盘导航（TabIndex/IsTabStop）、FocusVisualStyle
- **性能**：VirtualizingStackPanel、UI 虚拟化、BitmapCache 对复杂渲染启用缓存、避免过多触发器
- **响应式布局**：使用 Viewbox、Grid 星号比例、ScrollViewer 自适应窗口缩放
- **状态完整性**：每个控件考虑 Normal/MouseOver/Pressed/Disabled/Focused 状态（VisualStateManager 或 Trigger）
- **暗色/亮色模式**：通过 HandyControl 的 Theme 切换或自定义 ResourceDictionary.MergedDictionaries 实现
- **加载态**：使用 ProgressBar、等待动画或骨架屏占位

## 工作流程

### Phase 1: 需求分析（使用 planner agent）
当需求模糊或复杂时，先调用 `planner` agent 分析：
```
#agent planner 分析以下 WPF 页面设计需求：[需求描述]
```

### Phase 2: 设计 Token 定义
在 `App.xaml` 或独立资源字典中定义设计系统：

```xml
<!-- 色彩 Token -->
<SolidColorBrush x:Key="ColorBrand" Color="oklch(55% 0.25 260)"/>
<SolidColorBrush x:Key="ColorSurface" Color="oklch(98% 0.005 260)"/>
<SolidColorBrush x:Key="ColorText" Color="oklch(20% 0.02 260)"/>

<!-- 圆角 Token -->
<CornerRadius x:Key="RadiusSm">4</CornerRadius>
<CornerRadius x:Key="RadiusMd">8</CornerRadius>
<CornerRadius x:Key="RadiusLg">12</CornerRadius>

<!-- 间距 Token -->
<Thickness x:Key="Space2">2</Thickness>
<Thickness x:Key="Space4">4</Thickness>
<Thickness x:Key="Space8">8</Thickness>
<Thickness x:Key="Space16">16</Thickness>
```

参考配色方案文件：`[配色方案模板](./references/color-palette.md)`

可使用 `[New-WpfResourceDictionary.ps1](./scripts/New-WpfResourceDictionary.ps1)` 脚本一键生成资源字典骨架。

### Phase 3: 布局架构
- 明确内容层次（主要区域 / 次要面板 / 辅助信息）
- 使用 `Grid` 定义宏观布局骨架，嵌套 `StackPanel`/`WrapPanel` 处理微观排列
- 设计负空间（Margin/Padding 分配）
- 考虑窗口缩放行为（最小尺寸、内容自适应）

### Phase 4: 组件实现（使用 implementer agent）
使用 `implementer` agent 进行代码实现：
```
#agent implementer 按照设计 Token 实现以下 WPF 组件：[描述]
```

组件实现原则：
- **样式驱动**：使用 `<Style>` 封装可复用外观
- **模板分离**：复杂控件使用 `<ControlTemplate>` + `<DataTemplate>`
- **HandyControl 增强**：善用 `hc:BorderElement.CornerRadius`、`hc:IconElement.Geometry`、`hc:VisualElement.HighlightBrush` 等附加属性
- **状态完整**：每个组件覆盖默认/悬停/按下/禁用/聚焦 5 种状态，参考 [组件状态清单](./references/component-checklist.md)
- **动效点缀**：Button 悬停颜色过渡、面板展开折叠动画、数据加载骨架屏

### Phase 5: 审查与打磨（使用 review agent）
实现完成后自动调用 `review` agent 审查：
```
#agent review 审查以上 WPF 页面的设计质量：
1. 是否符合定义的设计 Token
2. 控件状态是否完整
3. 布局是否合理（无溢出、对齐正确）
4. 色彩应用是否协调
5. 是否有冗余样式代码
6. 是否使用 XAML 资源复用（Style/Template 而非重复属性）
```

### Phase 6: 运行验证（使用 action agent）
验证通过后调用 `action` agent 编写运行脚本：
```
#agent action 为这个 WPF 项目编写运行/构建说明
```

## Agent 协作流程图

```
用户需求
    │
    ▼
┌─────────────┐
│  planner    │  ← 复杂需求先分析
└──────┬──────┘
       │
       ▼
┌─────────────────────────────────────────────┐
│  Phase 2: 设计 Token 定义                    │
│  色彩/圆角/阴影/间距/排版/动效               │
│  参考 color-palette.md + 脚本生成骨架        │
├─────────────────────────────────────────────┤
│  Phase 3: 布局架构                           │
│  Grid 骨架 → 嵌套面板                        │
├─────────────────────────────────────────────┤
│  Phase 4: 组件实现 → #agent implementer     │
│  Style/ControlTemplate/DataTemplate          │
│  HandyControl 增强属性                       │
│  每个组件 5 种状态                            │
└─────────────────────┬───────────────────────┘
                      │
                      ▼
┌─────────────┐     ┌─────────────┐
│ implementer │ ──► │   review    │  ← 最多迭代 3 次
└─────────────┘     └──────┬──────┘
                           │ 通过
                           ▼
                    ┌─────────────┐
                    │   action    │  ← 运行/构建脚本
                    └─────────────┘
```

## MCP / 工具集成（免费开源）

本技能可借助以下工具和 MCP server 提升能力：

| 工具/MCP | 用途 | 状态 | 安装方式 |
|----------|------|------|---------|
| `#tool:edit` | 编辑 XAML/C# 文件 | ✅ 已可用 | — |
| `#tool:execute` | 运行 dotnet build 验证编译 | ✅ 已可用 | — |
| `#tool:web` | 查阅 HandyControl/NET 文档 | ✅ 已可用 | — |
| `#tool:search` | 搜索代码中的样式/资源 | ✅ 已可用 | — |
| **Puppeteer MCP** ⭐ | HTML mockup → 截图生成设计预览 | ❌ 需安装 | `npx -y @modelcontextprotocol/server-puppeteer` |
| **Playwright MCP** | PDF 设计稿输出 + 视觉分析 | ❌ 需安装 | `npx @playwright/mcp@latest` |
| **Figma MCP** | 从 Figma 设计稿导入 Token/样式 | ❌ 按需 | 社区包 |

> 安装任何 MCP server 前请确保 Node.js 18+ 已安装。

## 参考资源
- [WPF 配色方案模板](./references/color-palette.md) — 预定义的独特配色方案（5 套主题）
- [WPF 组件状态清单](./references/component-checklist.md) — 常见控件的状态检查表
- [资源字典生成脚本](./scripts/New-WpfResourceDictionary.ps1) — 一键生成 XAML 资源骨架
- HandyControl 官方文档
- WPF 性能最佳实践（MS Learn）

## 示例用法
> `/frontend-ui-designer 帮我设计一个 RV 减速器参数设置面板，使用 Brutalist 风格，带粗边框和高对比色`

> `/frontend-ui-designer 美化这个 WPF DataGrid，添加行悬停高亮、斑马纹、和分页控件`

> `/frontend-ui-designer 为 HandyControl 项目创建一个自定义主题，深色模式，工业风格`

> `/frontend-ui-designer 设计一个轴承参数录入页面，用卡片分组代替默认的灰色边框`

> `/frontend-ui-designer 创建一个 WPF 仪表盘，左侧导航树 + 右侧内容区，带平滑页面切换动画`
