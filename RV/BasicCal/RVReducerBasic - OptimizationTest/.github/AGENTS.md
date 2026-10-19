---
description: RV减速器设计计算系统（C++/WPF）的 Agent 指令。使用场景：在此代码库中进行编码、调试、修改、重构。引导 Agent 了解混合语言架构、构建方式、命名约定和常见陷阱。
---

# RVReducerBasic — Agent Instructions

## 项目概览

RV 减速器设计计算软件，包含：

| 项目 | 语言 | 类型 | 说明 |
|------|------|------|------|
| `RVReducerBasic` | C++ (VC++ v143) | DLL (x64) / EXE (Win32) | 核心计算引擎：渐开线齿轮副、摆线针轮副、曲轴、轴系统、轴承等计算模块 |
| `RVReducerDesignApp` | C# (.NET 8 WPF) | WinExe | 桌面设计应用「摆线针轮行星减速器设计系统」，HandyControl UI，调用 C++ DLL 运算 |
| `RvCalc` | C# (.NET 8) | 类库 | **注册表驱动的通用计算层**：`interface-registry.json` 描述 14 个 DLL 导出，运行时动态解析调用；含输入构建器（`InvGearInput`/`CycloidGearInput`） |
| `RvCalcCli` | C# (.NET 8) | 控制台 | 通用计算命令行：`--call / --sequence / --list-interfaces / --describe / --registry-check / --dump-templates / --selfcheck` |
| `RVAgentCli` | C# (.NET 8) | 控制台 | 终端 AI 助手：自然语言驱动计算（DeepSeek + AgentRunner 精简工具集，无 UI） |
| `DllTest`, `ZTestAddDll`, `AddDll` | C++ | 测试 | C++ 测试项目 |
| `WpfApp1/2/3`, `TestWpfApp`, `BingdingTest` | C# | 测试/辅助 | 各种测试和辅助工具 |
| `ClassLibrary2`, `ClassLibraryRVReducer` | C# | 安装类库 | 安装打包辅助（`Installer1.cs`） |
| `Setup1`, `Setup2`, `SetupRVReducer` | VS 安装项目 | .vdproj | 安装包工程 |
| `InstallRVFileALL` | 部署资源 | 目录 | 发布资源：MySQL 8.0.36、.NET 8 桌面运行时、msiexec 等 |
| `RVReducerFortran` | Fortran | 计算项目 | Fortran 计算工程（`rvreducerfortran.fdz`） |
| `新建文件夹` | C# | 旧版框架 | 早期版本 UI 框架（`CTANBasicPage` 等），当前不使用 |
| `轴建模` | WPF | 页面 | 轴/梁建模页面（`CTANGirderPage.xaml`） |

## 项目功能

### C++ 计算引擎（RVReducerBasic）

| 模块类 | 源文件 | 计算内容 |
|--------|--------|----------|
| `CInvoluteGearTAN` | `TANInvoluteGear.h/.cpp` | 渐开线齿轮副：几何（模数/压力角/螺旋角/中心距）、齿面接触应力 σH、齿根弯曲应力 σF、14 类材料、ISO 标准模数、变位系数优化、公差计算 |
| `CPinionCycloidGearTAN` | `TANPinionCycloidGear.h/.cpp` | 摆线针轮副：摆线几何（齿数 Z、针径 d_rp、分布圆直径 d_p、偏心距 e、针齿直径 d_m）、M 值 `CalcMValue`、齿顶距、摆线四参数、接触应力；内部含 MySQL 调用 |
| `CShaftSystemTAN` / `CShaftTAN` | `TANShaftSystem.h/.cpp`、`TANShaft.h/.cpp` | 轴系统：输入轴、曲轴、输出轴的力与尺寸计算 |
| `CBearingTAN` | `TANBearing.h/.cpp` | 轴承：额定寿命 L10h、静强度安全系数 S0、自定义轴承寿命 |
| `CRVReducerTAN` | `TANRVReducer.h/.cpp` | 减速器总体：总传动比、齿轮副力矩分配、传动比推荐（`RecommandTransmissionRatio`） |
| 辅助计算 | `BearingCalculators.*`、`BearingStaticFactors.*`、`TANCubicSpline.*`、`TANMechanismFunction.*` | 轴承计算辅助、轴承静态因子、三次样条插值、机构学函数 |

测试入口（注意区分多个同名文件）：
- `RVReducerBasic/Main_TAN.cpp` — 控制台测试入口，被 vcxproj 编入 **Win32 EXE** 配置（x64 配置排除）
- `RVReducerBasic/MAIN-Optimization.cpp` — 优化计算测试入口，同样编入 Win32 配置
- 根目录 `MAIN_TAN.cpp` — 独立测试文件，未挂接任何工程，需自行加入工程或单独编译
- ⚠️ `Main_TAN.cpp` 与 `MAIN-Optimization.cpp` 均含 `main()`，Win32 配置下同时编译会链接冲突（重复 main），测试时需在 `.vcxproj` 中临时排除其一

### DLL 导出接口（`InterfaceForCSharp.h`）

C++ DLL 通过 `__declspec(dllexport)` 导出 C 风格函数；C# 侧调用见 `RvCalc/DynamicDllCaller.cs`（运行时按导出名解析，无需 `[DllImport]`）
与接口描述表 `RvCalc/interface-registry.json`；传统 P/Invoke 封装见 `RVReducerDesignApp/CTANRVSet.cs`。

| 函数 | 功能 |
|------|------|
| `CalTotalTorqueAndRPM` | 减速器总体力矩/转速分配 |
| `CalInvGearTolerence` | 渐开线齿轮公差 |
| `CalInvGearStress2` | 渐开线齿轮应力（σH/σF） |
| `CalCycloidGearStressByObj` | 摆线轮接触应力 |
| `CalCrankShaftSystem2` | 曲轴系统计算 |
| `CalGeometry` | 几何计算 |
| `RecommondPara` | 齿轮参数推荐（模数/压力角/螺旋角/中心距/齿数/齿宽） |
| `GetForceAndForceLocation` | 力及作用位置输出 |
| `RecommondBearingLife` | 轴承寿命推荐（MySQL 轴承库） |
| `RecommondShaftPara` | 轴参数推荐（直径/长度） |
| `CalBearingLife` | 标准轴承寿命计算 |
| `RecommondMainBearing` | 主轴承推荐 |
| `CalCustomBearingLife` | 自定义轴承寿命计算（返回状态码） |
| `Testfunc` | 测试桩函数（空实现，无实际功能） |

### WPF 应用功能（RVReducerDesignApp）

- **主窗口**：「摆线针轮行星减速器设计系统」，左侧树导航 + 右侧多 Tab 内容区
- **树结构**：减速器 → 摆线轮 `CTANCycloidGearPair` / 渐开线齿轮副 `CTANInvGearPair` / 曲轴 `CTANCrankShaftSystem` / 轴承 `CTANMainBearing`
- **Tab 页**：输入（参数）/ 输出（结果）/ 示例 / 3D（曲轴 3D 绘制 `Draw3D`）
- **工具栏**：打开 / 保存 / 退出 / 计算 / 齿轮推荐（抽屉）/ 轴推荐（抽屉）/ 轴承数据库 / 帮助
- **推荐抽屉**：齿轮推荐 `DrawerTopInContainer`、轴推荐 `DrawerTopInContainerShaft`（ToggleButton 开关）
- **轴承数据库**：`Bearing/BearingDataWindow`，连接 MySQL `bearings` 库（localhost:3729），支持查询/新增/编辑/删除，导出 CSV/TXT/XLSX/JSON/SQL/XML/Markdown
- **保存/打开**：`AppDataStorage`，JSON 可完整保存/恢复；PDF 仅导出只读，不支持从 PDF 恢复
- **数据流**：`double[]` 数组 + 分割索引 `mDivisionIndexList` 划分各子系统，P/Invoke `ref double` 传参
- **计算**： 需要在 树结构 中选中 渐开线齿轮副 或者 摆线针轮副 后，再点击计算，计算结果呈现在对应的 输出结果页

## 操作说明

### 开发者操作（构建 / 运行 / 测试 / 调试）

**一键启动 WPF 应用**：

```bash
# 方式一（推荐）：直接对应用工程运行（需先编译 x64 C++ DLL 并复制到运行目录）
dotnet watch run --project RVReducerDesignApp/RVReducerDesignApp.csproj

# 方式二：普通运行
dotnet run --project RVReducerDesignApp/RVReducerDesignApp.csproj
```

> 注：`.vscode/tasks.json` 的 `watch` 任务当前为 `dotnet watch run --project RVReducerBasic.sln`；因解决方案含多个可运行 C# 项目，该任务可能报「多个可运行项目」而无法启动，一键运行时请优先使用上面的直接工程命令。

**编译 C++ 引擎**（必须用 `msbuild`，`dotnet build` 不支持 C++）：

```powershell
# x64 DLL（对接 C# 应用；产物复制到 RVReducerDesignApp/bin/Debug/net8.0-windows/）
msbuild RVReducerBasic.sln /p:Configuration=Debug /p:Platform=x64 /t:RVReducerBasic
# Win32 EXE（控制台测试）
msbuild RVReducerBasic.sln /p:Configuration=Debug /p:Platform=Win32 /t:RVReducerBasic
```

> ⚠️ **MKL 依赖**：`RVReducerBasic.dll`（x64 Debug）依赖 40+ 个 Intel MKL 分派 DLL（如 `mkl_avx2.2.dll`），复制 DLL 时需整体复制 `RVReducerDesignApp/bin/Debug/net8.0-windows` 目录下全部 DLL，否则运行报「找不到指定的模块」(0x8007007E)。

**调试**：`.vscode/launch.json`「启动 RVReducerDesignApp (WPF)」，预构建任务为 `build`。

**通用计算 / 回归验证**（`RvCalcCli`，需先编译 RVReducerDesignApp Debug 提供 DLL 源）：

```bash
dotnet build RvCalcCli/RvCalcCli.csproj
dotnet run --project RvCalcCli -- --list-interfaces          # 14 个接口及参数/输出
dotnet run --project RvCalcCli -- --registry-check           # 注册表 ↔ DLL 导出核对
dotnet run --project RvCalcCli -- --selfcheck                # 静态 DllImport 与动态调用结果比对
dotnet run --project RvCalcCli -- --call CalBearingLife --args '{"轴承类型":"圆锥滚子","转速":2000,"Cr":"23.8 kN","C0r":"19.5 kN","径向力Fr":1750,"e":0.35,"Y":1.7,"Y0":0.9}'
dotnet run --project RvCalcCli -- --sequence '@{"steps":[{"interface":"CalCrankShaftSystem2","args":{"arraysFile":"snap.json"}},{"interface":"GetForceAndForceLocation","args":{}}]}'
```

> ⚠️ 旧工具 `RVAutoTest`（UIA 驱动的 4 场景全自动测试 + 截图 + `report.json` + ISO Example 1 验证）已于 2026-09-14 移除，
> 源码/配置备份在 `_archive/RVAutoTest-source-*.zip`。计算类能力已由 `RvCalcCli`/`RvCalc` 覆盖；
> **ISO 6336-30 Example 1 验证链与 UI 自动化截图目前无替代**（需要时从备份恢复）。

**终端自然语言 AI 助手**（`RVAgentCli`，DeepSeek 驱动，无需 UI 填参）：

```bash
dotnet run --project RVAgentCli        # 交互式 REPL：输入自然语言，exit/quit 退出
"算一下模数2、太阳轮23齿、行星轮106齿的渐开线齿轮应力" | dotnet run --project RVAgentCli   # 管道单问单答
```

- 架构：复用 RVReducerDesignApp 的 `AgentRunner`（DeepSeek function calling，最多 15 轮）+ 终端精简工具集
  `rv_calc` / `calculate_inv_gear_stress` / `calc_bearing_life_by_code` / `export_whole_machine_snapshot`
  （工具集与提示词见 `RVReducerDesignApp/Services/Agent/AgentCliConsole.cs`；入口 `RVAgentCli/Program.cs`，
  复用 `RvCalcCli/DllBootstrap.cs` 自动复制 RVReducerBasic.dll + MKL/VTK 依赖）。
- ⚠️ 工具集**不含** `calc_current_part` / `get_design_state` / `calc_crank_shaft_system`（依赖界面状态，无 UI 会返回哨兵值）。
- API Key：环境变量 `DEEPSEEK_API_KEY`，或 WPF AI 面板保存的 `%APPDATA%\RVReducerDesignApp\ai-config.json`。
- 构建：`dotnet build RVAgentCli/RVAgentCli.csproj`（需先构建 RVReducerDesignApp Debug 提供 DLL 源）。
- 为什么是独立控制台项目而非 `RVReducerDesignApp.exe --chat`：WinExe 重定向时无控制台句柄（stdout 静默丢失）、
  PowerShell 不等待 GUI 程序且争抢 stdin，实测不可靠，故单独建控制台宿主。

**确定性命令行计算**（`RvCalcCli`，不经过 AI，脚本/回归用）：

前提：RVReducerDesignApp Debug 已构建；`RvCalcCli` 首次运行会由 `DllBootstrap` 自动从 `RVReducerDesignApp/bin/Debug/net8.0-windows` 复制全部 DLL 依赖（含 MKL 分派）。

1. **查接口**：`RvCalcCli --describe <接口名>`（或 `--list-interfaces`），确认参数键名/单位/必填项
2. **取模板**：复制 `RvCalc/templates/*.json` 的完整输入模板（`div` 分割索引 + `in` 数值数组）
   - ⚠️ 不要只填必填参数：174 字段齿轮输入缺省填充不完整会算出 NaN（实测仅给模数/齿数 → σH=NaN）
3. **改参**：把自然语言需求映射为模板 `in` 数组数值，写入 JSON 文件（UTF-8）
4. **计算**：`RvCalcCli --call <接口名> --args @args.json`；退出码 0=成功、2=失败
5. **解读**：读输出并按自然语言汇报；失败则按报错修参重试

快速通道（免填参）：`RVReducerDesignApp.exe --calc-part 摆线针轮副|渐开线齿轮副|总体|曲轴系` / `--bearing-lookup <型号> [--n N] [--fa Fa] [--fr Fr]` / `--export-snapshot [路径]`（退出码 0/2，不弹 UI 窗口）。

注意事项：
- 终端为 GBK，中文输出会乱码 → 重定向到文件后按 UTF-8 读
- 多步有状态计算（如曲轴系→轴推荐）用 `--sequence`（同进程保持状态）
- 冒烟基准：模板 `invGearStress2.json` → 计算接触应力 σH=3335.61 MPa、齿数比 4.6087、太阳轮分度圆 46 mm

**参数化测试与结果验证**：完整工作流（JSON 参数注入、MySQL 轴承库对比、双编译模式）见技能 [`rv-build-test`](./skills/rv-build-test/SKILL.md)。

### 软件使用操作（WPF 设计流程）

1. **选择部件**：左侧树选中 摆线轮 / 渐开线齿轮副 / 曲轴 / 轴承
2. **输入参数**：在「输入」Tab 填写几何、材料、载荷等参数
3. **计算**：点击工具栏「计算」（对选中节点调用 `CalPageData()`）
4. **查看结果**：切到「输出」Tab 查看结果；曲轴可切「3D」Tab 查看三维图形（自动调用 `Draw3D`）
5. **参数推荐**：工具栏「齿轮推荐」/「轴推荐」打开顶部抽屉，设置目标（总传动比、输出力矩/转速、目标应力、设计寿命、使用系数、许用应力、行星轮个数等）并确定，推荐结果回填输入页
6. **轴承数据库**：点「轴承数据库」打开独立窗口，按表查询参数、增删改、导出
7. **保存/打开**：工具栏「保存」导出 JSON/PDF 轴数据；「打开」从 JSON 恢复（PDF 只读不可恢复）

## 关键架构

### C++ ↔ C# 互操作

- C++ DLL 通过 `__declspec(dllexport)` 导出 C 风格函数
- C# 通过 `[DllImport]` P/Invoke 调用（详见 `CTANRVSet.cs`、`RVAutoTest/DllInvoker.cs`）
- 数据传递方式：`double[]` 数组 + 分割索引（`mDivisionIndexList`）
- 接口定义在 `InterfaceForCSharp.h` / `InterfaceForCSharp.cpp`

### C# UI 框架

- 自定义 UI 控件框架，基类为 `CTANBasicContentPageObj`
- 核心控件：`CTANGrid`、`CTANTextBox`、`CTANComboBox`、`CTANButton`（`CTANControl.cs`）、`CTANScrollPage`（`CTANPageLayout.cs`）
- WPF 使用 **HandyControl**（`hc:` 前缀）作为第三方 UI 库
- 不使用 MVVM 模式，采用代码后置（code-behind）+ 自定义控件树方式
- 数据存储：`CTANRVSet` 作为顶级容器，包含齿轮副 `CTANInvGearPair`、摆线轮 `CTANCycloidGearPair`、曲轴 `CTANCrankShaftSystem`、轴承 `CTANMainBearing` 等

### C++ 计算引擎

- 核心数据结构：`CRVReducerTAN`（`TANRVReducer.h`）；子模块与测试入口详见上文「项目功能 → C++ 计算引擎」

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
6. **MKL 依赖**：`RVReducerBasic.dll` 依赖 40+ 个 Intel MKL 分派 DLL，复制/部署时必须整体复制 WPF 输出目录下的全部 DLL
7. **日志文件 GBK 编码**：`修改历史.txt` / `修改记录.txt` 为 GBK/GB2312 编码，直接读取显示为乱码，需按 GBK 编码读取
8. **多同名测试文件 + 双 main 冲突**：`RVReducerBasic/Main_TAN.cpp` 与 `MAIN-Optimization.cpp` 均含 `main()` 且都被编入 Win32，同时启用会链接失败（重复 main）；根目录另有一个未挂接工程的 `MAIN_TAN.cpp`。三者勿混淆

## 相关文件

- [RVReducerBasic 核心计算引擎](../RVReducerBasic/) — 齿轮、轴承、轴、摆线轮的计算逻辑
- [RVReducerDesignApp WPF 应用](../RVReducerDesignApp/) — UI 层，所有 XAML 和代码后置
- [InterfaceForCSharp.h](../RVReducerBasic/InterfaceForCSharp.h) — C++/C# 互操作接口定义（DLL 导出清单）
- [CTANRVSet.cs](../RVReducerDesignApp/CTANRVSet.cs) — C# 顶级数据容器及 DLL 导入声明
- [DllInvoker.cs](../RVAutoTest/DllInvoker.cs) — 自动化测试的 DLL P/Invoke 封装
- [Bearing 轴承数据库](../RVReducerDesignApp/Bearing/) — MySQL 轴承库窗口（`BearingDataWindow`）
- [ZhouJie UI 窗口](../RVReducerDesignApp/ZhouJie/) — 推荐/3D/帮助等辅助窗口
- [轴建模页面](../轴建模/CTANGirderPage.xaml) — 轴/梁建模页面
- [修改历史.txt](../RVReducerBasic/修改历史.txt) — C++ 侧修改记录（GBK 编码）
- [修改记录.txt](../RVReducerDesignApp/修改记录.txt) — C# 侧修改记录（GBK 编码）
- [MAIN_TAN.cpp](../MAIN_TAN.cpp) — 根目录独立控制台测试文件（未挂接任何工程）

## 技能（Skills）

- [`rv-build-test`](./skills/rv-build-test/SKILL.md) — C++ 编译、参数化测试与结果验证工作流（JSON 参数、MySQL 轴承库对比、C++/C# 互操作验证）
- [`frontend-ui-designer`](./skills/frontend-ui-designer/SKILL.md) — WPF/HandyControl 界面设计规范与设计系统

## 工作流参考

此仓库配合以下用户级 Agent 使用：

| Agent | 用途 |
|-------|------|
| `planner` | 梳理需求、调研代码库、制定实施计划 |
| `implementer` | 按计划编写/修改代码，自动编译验证，调用 review 审查 |
| `review` | 代码质量和安全审查 |
| `action` | 创建运行脚本和使用说明 |
