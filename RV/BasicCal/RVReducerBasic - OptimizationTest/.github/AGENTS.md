---
description: RV减速器设计计算系统（C++/WPF）的 Agent 指令。使用场景：在此代码库中进行编码、调试、修改、重构。引导 Agent 了解混合语言架构、构建方式、命名约定和常见陷阱。
---

# RVReducerBasic — Agent Instructions

## 项目概览

RV 减速器设计计算软件，包含：

| 项目 | 语言 | 类型 | 说明 |
|------|------|------|------|
| `RVReducerBasic` | C++ (VC++ v143) | DLL (x64) / EXE (Win32) | 核心计算引擎，包含齿轮、轴承、轴、摆线轮等计算模块 |
| `RVReducerDesignApp` | C# (.NET 8 WPF) | WinExe | 桌面设计应用，调用 C++ DLL 进行运算，使用 HandyControl UI 库 |
| `WpfApp1/2/3`, `TestWpfApp`, `BingdingTest` | C# | 测试/辅助 | 各种测试和辅助工具 |
| `DllTest`, `ZTestAddDll`, `AddDll` | C++ | 测试 | C++ 测试项目 |

## 构建命令

> ⚠️ **关键**：C++ 项目无法通过 `dotnet build` 编译，需使用 `msbuild`（VS Developer Command Prompt）。

```bash
# 完整构建（含 C++ 项目）
msbuild RVReducerBasic.sln

# 仅构建 WPF C# 项目
dotnet build RVReducerBasic.sln
```

VS Code 已配置任务（`.vscode/tasks.json`）：
- `build` → `dotnet build`（仅 C# 项目）
- `publish` → `dotnet publish`
- `watch` → `dotnet watch run`

调试配置（`.vscode/launch.json`）：启动 `RVReducerDesignApp`（WPF 应用），预构建任务为 `build`。

## 关键架构

### C++ ↔ C# 互操作

- C++ DLL 通过 `__declspec(dllexport)` 导出 C 风格函数
- C# 通过 `[DllImport]` P/Invoke 调用（详见 `CTANRVSet.cs`）
- 数据传递方式：`double[]` 数组 + 分割索引（`mDivisionIndexList`）
- 接口定义在 `InterfaceForCSharp.h` / `InterfaceForCSharp.cpp`

### C# UI 框架

- 自定义 UI 控件框架，基类为 `CTANBasicContentPageObj`
- 核心控件：`CTANGrid`、`CTANComboBox`、`CTANControl`、`CTANScrollPage`
- WPF 使用 **HandyControl**（`hc:` 前缀）作为第三方 UI 库
- 不使用 MVVM 模式，采用代码后置（code-behind）+ 自定义控件树方式
- 数据存储：`CTANRVSet` 作为顶级容器，包含齿轮副 `CTANInvGearPair`、摆线轮 `CTANCycloidGearPair`、曲轴 `CTANCrankShaftSystem`、轴承 `CTANMainBearing` 等

### C++ 计算引擎

- 核心数据结构：`CRVReducerTAN`（`TANRVReducer.h`）
- 子模块：`CInvoluteGearTAN`（渐开线齿轮）、`CPinionCycloidGearTAN`（摆线针轮）、`CShaftSystemTAN`（轴系统）、`CBearingTAN`（轴承）
- `Main_TAN.cpp` 包含控制台测试入口

## 命名约定

| 范围 | 模式 | 示例 |
|------|------|------|
| C++ 类 | `C` + 名称 + `TAN` | `CRVReducerTAN`, `CInvoluteGearTAN` |
| C# 类 | `CTAN` + 名称 | `CTANRVSet`, `CTANInvGearPair` |
| C# UI 控件 | `CTAN` + 控件类型 | `CTANGrid`, `CTANComboBox` |
| 成员变量 | `m` + PascalCase | `mInputDoubleDataStorage`, `mTreeView` |
| 方法参数 | `In`/`Out` 前缀 | `InData`, `OutData`, `InGear` |
| 静态成员 | `s` + PascalCase | `sPageIDDict`, `sRowHeight` |
| 局部变量 | camelCase | `tmpColumnCount`, `TempData` |

## 编码约定

- 修改日志记录在 `修改历史.txt` / `修改记录.txt` 中
- C# 使用 `using` 静态导入、`ref` 参数传递数组到 DLL
- C++ 使用角度制，`Degree` 常量（`#define Degree 0.01745329252`）用于弧度转换
- 代码注释多为中文
- 所有 `double[]` 传入 DLL 时使用 `ref double` 作为参数

## 常见陷阱

1. **C++ 项目编译失败**：`dotnet build` 不支持 C++ 项目，需用 `msbuild` 或在 VS IDE 中构建
2. **C# 可空性警告**：项目中 `Nullable` 启用，大量 CS8600/CS8602/CS8618 警告，多存在于 DLL 互操作代码中
3. **新 .cpp 文件未注册**：在 `RVReducerBasic.vcxproj` 的 `<ClCompile>` 项中添加新文件
4. **C++ 前向声明**：`InterfaceForCSharp.cpp` 中的 `static` 函数在使用前需前向声明
5. **DLL 路径**：C# 通过 DllImport 加载 `RVReducerBasic.dll`，需确保 DLL 在运行目录或系统路径中

## 相关文件

- [RVReducerBasic 核心计算引擎](../RVReducerBasic/) — 齿轮、轴承、轴的计算逻辑
- [RVReducerDesignApp WPF 应用](../RVReducerDesignApp/) — UI 层，所有 XAML 和代码后置
- [InterfaceForCSharp.h](../RVReducerBasic/InterfaceForCSharp.h) — C++/C# 互操作接口定义
- [CTANRVSet.cs](../RVReducerDesignApp/CTANRVSet.cs) — C# 顶级数据容器及 DLL 导入声明
- [修改历史.txt](../RVReducerBasic/修改历史.txt) — C++ 侧修改记录
- [修改记录.txt](../RVReducerDesignApp/修改记录.txt) — C# 侧修改记录
- [Main_TAN.cpp](../Main_TAN.cpp) — C++ 控制台测试入口

## 工作流参考

此仓库配合以下用户级 Agent 使用：

| Agent | 用途 |
|-------|------|
| `planner` | 梳理需求、调研代码库、制定实施计划 |
| `implementer` | 按计划编写/修改代码，自动编译验证，调用 review 审查 |
| `review` | 代码质量和安全审查 |
| `action` | 创建运行脚本和使用说明 |
