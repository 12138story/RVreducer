---
name: rv-build-test
description: 'RV减速器计算引擎的编译、参数化测试与结果验证工作流。使用场景：编译C++ DLL/EXE、参数化测试渐开线齿轮/摆线针轮/曲轴/轴承计算、用JSON参数文件批量测试、对比计算结果与数据库标准值、验证C++与C#互操作、驱动WPF UI填参计算并按Tab导出结果、可选截图。关键词：编译、构建、测试、msbuild、dotnet、参数测试、齿轮计算、轴承计算、摆线轮、曲轴、RVReducer、JSON参数、MCP工具、MySQL轴承库、UI自动化、ISO Example 1、渐开线齿轮副、截图。可与planner/implementer/review/action等Agent协同。'
argument-hint: '描述测试场景，或指定JSON参数文件路径'
user-invocable: true
---

# RV 编译与测试技能

## 何时使用

- 用户要求编译 C++ 项目（RVReducerBasic DLL 或 Win32 测试 EXE）
- 用户提供 JSON 参数文件要求运行计算测试
- 用户需要验证齿轮/摆线轮/曲轴/轴承的计算结果
- 用户要求将计算结果与轴承数据库中的标准值对比
- 用户需要批量测试不同参数组合的计算输出
- 用户要验证 C++ DLL 与 C# WPF 应用的互操作正确性
- 用户要求修改 `Main_TAN.cpp` 测试参数后编译运行
- 用户要求运行通用计算 / 回归验证（`RvCalcCli`：`--call` / `--sequence` / `--registry-check` / `--selfcheck`）
- 用户要求将 ISO 标准参数填入 WPF 渐开线齿轮副页面计算，按 Tab 导出结果，并可选截图

## 前置条件检查

开始任何操作前，必须依次确认以下条件：

1. **VS 构建环境**：`msbuild` 必须在 PATH 中（需 VS Developer Command Prompt 或已配置 vcvars）
2. **解决方案存在**：`RVReducerBasic.sln` 位于工作区根目录
3. **Git 工作区干净**（可选但推荐）：通过 `github-local` MCP 检查 git status
4. **MCP 连接**（可选）：
   - `mysql` MCP → bearings 数据库（用于获取/验证轴承参数）
   - `github-local` MCP → Git 操作（分支管理、文件恢复）
   - `context7` MCP → 查阅库文档（Intel MKL、MSVC 编译选项等）
   - `markitdown` MCP → 转换外部文档为结构化参数

## 完整工作流（8 阶段）

### [0] 环境准备与 Git 保护

```
github-local: git status → 确认工作区干净
github-local: git branch → 可选创建测试分支隔离变更
```

检查关键文件：
- `Main_TAN.cpp` — 将临时修改的测试入口
- `RVReducerBasic/RVReducerBasic.vcxproj` — 项目配置（Win32=EXE, x64=DLL）
- `RVReducerBasic/InterfaceForCSharp.h` — DLL 导出接口定义

### [1] 参数准备

**首选方式：从 MySQL bearings 数据库获取轴承参数**

当测试涉及轴承计算时，通过 MySQL MCP 连接 `bearings` 数据库：
```sql
-- 查询某型号轴承的几何参数
SELECT * FROM bearing_models WHERE model_code = '6208';
-- 获取额定载荷 C, C0
-- 获取几何参数: Dpw, Dw, Z, alpha 等
```

**备选方式：从 JSON 参数文件读取**

JSON 模板位于 `./assets/test_params_template.json`，参数 Schema 见 `./references/parameter_schema.md`。

**参数注入流程**：
1. 读取 JSON 文件中的参数值
2. 将参数映射为 C++ 代码中的硬编码值
3. 修改 `Main_TAN.cpp` 中的对应赋值语句
4. 记录原始值以便后续恢复

### [2] 文档参考（可选）

```markdown
context7: 查询 Intel MKL 并行库文档
context7: 查询 MSVC v143 编译选项
markitdown: 将外部设计规范/PDF转换为参数
```

### [3] 代码生成（参数注入）

Agent 应直接修改 `Main_TAN.cpp` 中的参数：
- `TestRVReducer.mOutputTorque` — 输出扭矩
- `TestRVReducer.mOutputRPM` — 输出转速
- `TestGEPinionData[14]` — 小齿轮几何参数
- `TestGEGearData[14]` — 大齿轮几何参数
- `TestGECommonData[6]` — 齿轮副公共参数
- `TestPHData[8]` / `TestMNData[6]` — 材料参数
- `TestAppData[7]` / `TestAppCommonData[7]` — 应用系数
- `TestAcdataPinion[6]` / `TestAcdataGear[6]` — 公差数据
- `TestAssembleData[5]` — 装配数据

**重要**：修改前通过 `github-local` 确认 git 可恢复，修改后只做编译测试，不提交。

### [4] 编译构建

**Win32 控制台 EXE（快速独立测试）**：
```powershell
msbuild RVReducerBasic.sln /p:Configuration=Debug /p:Platform=Win32 /t:RVReducerBasic
```
产物：`RVReducerBasic/Debug/RVReducerBasic.exe`

**x64 DLL（对接 C# 应用）**：
```powershell
msbuild RVReducerBasic.sln /p:Configuration=Debug /p:Platform=x64 /t:RVReducerBasic
# 或 Release 模式
msbuild RVReducerBasic.sln /p:Configuration=Release /p:Platform=x64 /t:RVReducerBasic
```
产物：`RVReducerDesignApp/bin/<Configuration>/net8.0-windows/RVReducerBasic.dll`（例如 Debug 配置下为 `.../bin/Debug/net8.0-windows/RVReducerBasic.dll`）

> ⚠️ **关键**：`dotnet build` 不支持 C++ 项目，必须使用 `msbuild`。
> Win32|Debug 配置生成 Application(.exe)，x64|Debug 配置生成 DynamicLibrary(.dll)。
> Release|x64 启用全程序优化和 Intel MKL Parallel。

编译错误排查：
- 检查 `.vcxproj` 中 `<ClCompile>` 是否包含所有 .cpp 文件
- 检查 IncludePath/LibraryPath 中的 Intel MKL 路径是否正确
- Win32 和 x64 使用不同的 IncludePath 配置

### [5] 运行测试

**Win32 EXE 方式**：
```powershell
# 直接从输出目录运行
& ".\RVReducerBasic\Debug\RVReducerBasic.exe"
```
Agent 使用 `run_in_terminal` 工具执行，捕获 stdout 输出。

**x64 DLL 方式**：
需要 C# 宿主程序加载 DLL 调用。推荐使用现有的 `RVReducerDesignApp` WPF 应用：
```powershell
dotnet run --project RVReducerDesignApp/RVReducerDesignApp.csproj
```

**输出解析**：
- `std::cout` 输出的数值按行解析
- 齿轮计算结果：`sigma_H`（齿面接触应力）、`sigma_F`（齿根弯曲应力）
- 轴承计算结果：寿命 `L10h`（小时）、`C`（额定动载）、`P`（当量动载荷）
- 摆线轮计算结果：接触应力分布

### [6] 结果验证

**通过 MySQL MCP 对比数据库**：
```sql
-- 对比计算出的轴承寿命与数据库中的标准值
SELECT model_code, L10h_standard, C_rated, P_equivalent
FROM bearing_validation
WHERE model_code = '6208';
```

**对比流程**：
1. 从 stdout 解析出计算结果
2. 通过 MySQL MCP 查询轴承标准值
3. 计算偏差百分比
4. 输出结构化对比报告：

```
┌─────────────┬──────────┬──────────┬──────────┐
│   参数      │ 计算值   │ 标准值   │ 偏差(%)  │
├─────────────┼──────────┼──────────┼──────────┤
│ sigma_H     │ 1234.56  │ 1200.00  │   +2.88  │
│ L10h        │ 8760.00  │ 8500.00  │   +3.06  │
│ C (动载)    │ 29500.00 │ 29100.00 │   +1.37  │
└─────────────┴──────────┴──────────┴──────────┘
```

### [7] 清理恢复

```powershell
# 恢复被修改的源文件
git restore Main_TAN.cpp

# 如果创建了测试分支，切回
git checkout <original-branch>
```

## 双编译模式速查

| 模式 | 配置 | 产物类型 | 适用场景 |
|------|------|----------|----------|
| **Win32 EXE** | `Debug|Win32` | .exe 控制台程序 | 快速独立测试，直接 stdout 输出 |
| **x64 DLL** | `Debug|x64` / `Release|x64` | .dll 动态库 | C# 互操作测试，WPF 应用集成 |

## 通用计算与回归验证（RvCalcCli/ + RvCalc/）

> ⚠️ **2026-09-14 变更**：原 `RVAutoTest` 全自动测试工具已移除，源码与配置备份在 `_archive/RVAutoTest-source-*.zip`（未纳入 git，请勿删备份）。
> 下方的 4 场景/配置/截图/退出码 内容仅作**历史参考**；等价的计算能力请用下面的 RvCalcCli，
> 但 **ISO Example 1 验证链与 UI 自动化截图无替代**（需要时从备份恢复）。

`RvCalcCli` + `RvCalc` 是注册表驱动的通用计算层：接口布局/参数名/单位/输出字段写在
`RvCalc/interface-registry.json`，运行时动态解析 DLL 导出调用；**新增接口只改 JSON**。

### 命令一览（当前可用）

| 命令 | 说明 |
|------|------|
| `dotnet run --project RvCalcCli -- --list-interfaces` | 列出 14 个接口及参数/输出（供自然语言映射） |
| `... --describe <接口名>` | 查看单个接口参数、单位、输出字段 |
| `... --call <接口名> --args '{...}'` | 通用计算（参数可带单位；`@file.json` 也可） |
| `... --call <接口名> --args '{"arraysFile":"snap.json"}'` | 用整机快照驱动 `CalCrankShaftSystem2` / `RecommondShaftPara` |
| `... --sequence '@{"steps":[...]}'` | 多步同进程（有状态依赖：曲轴系 → 轴推荐 → 应力分布） |
| `... --registry-check` | 注册表 ↔ DLL 导出核对 + 模板就绪检查 |
| `... --selfcheck` | 静态 `DllImport` 与动态调用器结果比对 |
| `... --dump-templates` | 重新生成 `RvCalc/templates/*.json` |

构建：

```powershell
msbuild RVReducerBasic.sln /p:Configuration=Debug /p:Platform=x64   # ① C++ 引擎
dotnet build RVReducerDesignApp/RVReducerDesignApp.csproj           # ② 提供 DLL 与 40+ 原生依赖
dotnet build RvCalcCli/RvCalcCli.csproj                             # ③ 通用计算 CLI
```

回归基线数值与设计原理见 `RvCalc/README.md` 与 `RvCalc/docs/自然语言驱动计算-设计流程与原理.md`。

## （历史）RVAutoTest 全自动测试工具

`RVAutoTest` 是 RV 减速器计算系统的**全自动测试与截图工具**：通过 P/Invoke 直接调用 `RVReducerBasic.dll`，并用 UIA 驱动 WPF 应用（`RVReducerDesignApp`），实现"输入一组参数 → 自动完成全部计算 → 输出结果 JSON + 界面截图"闭环。

### 工具概述：4 个计算场景

| 场景 | 说明 | DLL 接口 |
|------|------|----------|
| ① **齿轮推荐** | 由传动比/扭矩/转速/寿命推荐渐开线+摆线几何参数 | `RecommondPara` |
| ② **渐开线齿轮副** | 太阳轮/行星轮副接触/弯曲应力及几何（各 142 字段） | `CalInvGearStress2` |
| ③ **摆线针轮副** | 摆线接触应力、K1、跨棒距、精度公差（9 字段） | `CalCycloidGearStressByObj` |
| ④ **轴推荐 + 曲轴图形** | 推荐曲轴直径/长度，装配并绘制曲轴 2D 图形 | `RecommondShaftPara` |

> ②③ 场景几何参数默认从 ① 推荐输出**自动映射**（数据闭环），也可在配置文件中单独覆盖。

### 命令一览

| 命令 | 说明 | 详见 |
|------|------|------|
| `dotnet run --project RVAutoTest`（无参数） | 默认：4 场景全自动 + WPF 截图 + `report.json` | 本章 |
| `--config <path>` / `-c <path>` | 指定配置 JSON（`test_params.json`） | 本章 |
| `--iso-example1 [<json>]` | ISO Example 1 DLL 直调验证（无需 UI） | ISO 章节 |
| `--iso-example1-ui [--screenshot]` | ISO Example 1 UI 测试（截图默认关闭） | ISO 章节 |
| `--verify-cycloid-ui` | 调试：驱动 UI 摆线页面计算并与 DLL 直调对比。用法：`dotnet run --project RVAutoTest -- --verify-cycloid-ui` | 本章 |

### 编译与环境要求

```powershell
# ① C++ 核心引擎（生成 RVReducerBasic.dll，x64 Debug）——需 VS 开发者命令行
msbuild RVReducerBasic.sln /p:Configuration=Debug /p:Platform=x64

# ② WPF 应用（生成 RVReducerDesignApp.exe，并将 DLL 依赖复制到输出目录）
dotnet build RVReducerDesignApp/RVReducerDesignApp.csproj

# ③ 测试工具
dotnet build RVAutoTest/RVAutoTest.csproj
```

- 环境：**.NET 8**、**Visual Studio / MSBuild (VC++)**、**MySQL 8.0**（端口 3729，库 `bearings`，账号 root/jay）、**Windows 10/11**（UIA 需桌面会话）。
- 运行时会从 WPF 输出目录整体复制 DLL（含 Intel MKL 等 40+ 原生依赖）到工具目录。

### 配置参数（test_params.json）

配置文件为 JSON，字段 `null` 表示未配置（用默认值/推荐映射）；模板见 `RVAutoTest/test_params_template.json`（含逐字段注释）。四个子配置：

| 配置块 | 字段 |
|--------|------|
| `gearRecommend` | fixElemNotation / totalRatio / outputTorque / outputRPM / targetStress / designLife / usageFactor / allowableStress / planetCount / cycloidMinWidth / cycloidMaxWidth |
| `cycloidGear` | precGrade / torqueScale / pinionZ / rollerRadius / pitchCircleRadius / pinionEModule / pinionWidth / cycloid1Width / cycloid1Eccentricity / cycloid2Width / cycloid2Eccentricity |
| `invGear` | module / pressureAngle / helixAngle / centerDistance / sunTeeth / planetTeeth / planetCount / sunWidth / planetWidth / torque / speed / designLife |
| `shaftRecommend` | allowableBendStress / allowableShearStress / bearingDesignLife |

```powershell
dotnet run --project RVAutoTest/RVAutoTest.csproj -- --config RVAutoTest/test_params.json
```

### 输出结果（screenshots/）

| 文件 | 内容 |
|------|------|
| `report.json` | 汇总报告（Success/Errors + 4 场景结果） |
| `gear-recommend-result.json` | 齿轮推荐 91 字段 |
| `gear-recommend-outdata.json` | 齿轮推荐 DLL 原始输出（供 ②③ 闭环，可忽略） |
| `inv-gear-result.json` | 渐开线齿轮副：太阳轮/行星轮字典各 142 字段 |
| `cycloid-gear-result.json` | 摆线针轮副 9 字段 |
| `shaft-recommend-result.json` | 轴推荐：RecommendedDiameter / RecommendedLength / CrankShaftCount |

### 退出码

| 退出码 | 含义 |
|--------|------|
| `0` | 全部场景成功（`report.json` 中 `Success: true`） |
| `1` | 存在失败场景（`report.json` 中 `Errors` 列出） |
| `2` | 致命错误（配置解析失败等） |

> 注：`--iso-example1` 模式的退出码语义不同（按偏差分级 0/1/2），见 ISO 章节。

## 扩展：外部 DLL 测试

本技能也支持测试外部独立的轴承计算 DLL（如 `api.h` 导出的 `CalBearingLife` 函数）。

**流程**：
1. 准备测试 C++ 文件（参考 `main728.cpp` 的 `InData[37]` / `OutData[20]` 接口模式）
2. 通过 MySQL `bearings` 数据库获取轴承参数填入 `InData`
3. 用 `cl.exe` 或 msbuild 编译测试文件并链接 DLL
4. 运行测试，捕获 `OutData`
5. 将 `OutData` 与数据库标准值对比验证

外部轴承测试的参数映射详见 `./references/parameter_schema.md` 的"外部轴承接口"章节。

## ISO Example 1 验证（--iso-example1 / --iso-example1-ui）

用 ISO/TR 6336-30:2022（Annex A）Example 1 单斜齿渗碳淬火齿轮副参数（z=17/103, mn=8, αn=20°, β=15.8°, a=500, T1=9000 N·m）验证 `CalInvGearStress2` 的渐开线齿轮副计算。两种模式：

| 命令 | 模式 | 说明 |
|------|------|------|
| `--iso-example1 [<json>]` | **DLL 直调** | 无需 UI，直接调 DLL 并与 ISO Table 4 对比误差，输出 JSON/MD 报告 |
| `--iso-example1-ui [--screenshot]` | **UI 驱动** | UIA 驱动 WPF 应用填参计算，按 Tab 导出结果，可选截图 |

### 模式一：DLL 直调（--iso-example1）

```powershell
# 使用默认参数文件（iso_example1_params.json）
dotnet run --project RVAutoTest/RVAutoTest.csproj -- --iso-example1

# 指定参数文件
dotnet run --project RVAutoTest/RVAutoTest.csproj -- --iso-example1 RVAutoTest/iso_example1_params.json
```

- 输出：每次运行生成时间戳子文件夹 `screenshots/iso-example1/<yyyyMMdd-HHmmss>/`，内含 `iso-example1-result.json`（完整 284 字段）、`iso-example1-comparison.json`（对比报告）、`iso-example1-comparison.md`（对比表格）。
- 退出码：偏差 <5% → 0；5%~15% → 1；>15% → 2。
- 已知验证结果（2026-08-01，适配参数 x1=0.1178/cx=0.4 后）：最大偏差 **1.11%**（σH 0.07%、σF 0.50%/0.71%、几何 ≤0.27%）。参考诊断项（xE、节线速度等小绝对值字段）不计入最大偏差。

### 模式二：UI 驱动（--iso-example1-ui）

通过 UIA 自动化将 ISO 参数填入 WPF 渐开线齿轮副页面，触发计算，按 Tab 导出结果，并可选择性截图。

#### 功能与默认行为

| 功能 | 默认 | 说明 |
|------|------|------|
| UI 操作（填参数 + 计算） | ✅ 始终执行 | 自动填入太阳轮/行星轮/载荷参数并点击"计算" |
| 计算结果参数存储 | ✅ 始终执行 | 遍历 7 个输出 Tab，按 Tab 分类导出 Markdown |
| 截图 | ❌ 默认不截图 | 加 `--screenshot` 参数才截图 |
| 界面停留 | ✅ 始终执行 | 测试后界面停在"计算结果"页，应用保持运行 |

#### 命令用法

```powershell
# 前置：编译 C++ DLL
msbuild RVReducerBasic.sln /p:Configuration=Debug /p:Platform=x64 /t:RVReducerBasic

# 前置：编译 WPF 应用（生成 RVReducerDesignApp.exe，UI 测试必需）
dotnet build RVReducerDesignApp/RVReducerDesignApp.csproj -c Debug

# 默认模式：UI 填参 + 计算 + 结果导出 + 界面停留（不截图）
dotnet run --project RVAutoTest/RVAutoTest.csproj -- --iso-example1-ui

# 截图模式：额外截取所有输出页/输入页
#   7 个输出 Tab + 3 个输入页 + 最终输出 = 11 张 PNG
dotnet run --project RVAutoTest/RVAutoTest.csproj -- --iso-example1-ui --screenshot
```

> `--iso-example1-ui` 与 `--iso-example1` 均为精确匹配（`args.Contains`），程序先判断长参数为防御性写法；`--screenshot` 需配合 `--iso-example1-ui` 使用。

#### 参数来源

- 输入参数模型：`RVAutoTest/iso_example1_params.json`（ISO Annex A Tables A.1-A.5 + DLL 适配值）
- 关键适配值（DLL 内部语义与 ISO 不同，需换算）：
  - `x1 = 0.1178`（产形变位 xE，DLL 齿形系数用名义变位）
  - `cx = 0.4`（DLL 齿根高 = (1.0+cx)×mn = 1.4mn = ISO hfP）
  - 齿条刀齿顶高系数 `1.4`、齿顶圆半径系数 `0.39`
  - ComboBox：旋向=左旋、材料类型=Eh、加工质量=MQ、接触区控制=不控制、齿廓补偿=是、精度等级=5级

#### 输出文件

每次运行生成时间戳子文件夹 `screenshots/iso-example1-ui/<yyyyMMdd-HHmmss>/`，内含：

| 文件 | 说明 |
|------|------|
| `计算结果汇总.md` | 按 7 个 Tab（应力/几何/接触/滑动/齿廓/特殊/其他）分类的计算结果 |
| `01~07-output-*.png` | 7 个输出 Tab 截图（仅 `--screenshot`） |
| `08~10-*-input.png` | 输入页截图（仅 `--screenshot`） |
| `11-invgear-output-final.png` | 最终输出截图（仅 `--screenshot`） |

#### 关键实现说明（维护参考）

- **UI 结构**：渐开线齿轮副页面使用**内层 TabControl**（几何/物性/加工/应用/精度/安装 6 个 Tab），填参数需逐 Tab 切换；几何 Tab 含"其他几何参数"Expander（齿条刀参数）。输出页在外层"计算结果"Tab 中，内含 7 个内层输出 Tab。
- **标签匹配**：WPF 只读 TextBox 在 UIA 中是 `ControlType.Edit`，文本在 `ValuePattern.Value`，需按位置匹配右侧可编辑输入框（见 `IsoExample1UiTest.BuildLabelEditMap` 批量映射；`UIAutomator.FindEditAfterLabel` 为其他场景的单字段查找）。
- **实现文件**：`RVAutoTest/IsoExample1UiTest.cs`（UI 自动化主逻辑）、`RVAutoTest/IsoExample1Test.cs`（DLL 直调对比）、`RVAutoTest/UIAutomator.cs`（UIA 工具）。

## 故障排除

| 现象 | 原因 | 解决 |
|------|------|------|
| `msbuild` 未找到 | 未在 VS 开发环境 | 从 VS Developer Command Prompt 启动，或运行 `vcvars64.bat` |
| C++ 编译错误 C1083 | Include 路径不对 | 检查 `.vcxproj` 的 `<IncludePath>`，确认 MKL 路径存在 |
| 链接错误 LNK1104 | 找不到 lib 文件 | 检查 `<LibraryPath>`，确认 MySQL 和 MKL 的 lib 路径 |
| DLL 加载失败 | C# 找不到 DLL | 确保 DLL 在运行目录或系统 PATH 中 |
| stdout 无输出 | Win32 EXE 编译配置不对 | 确认使用 `Debug|Win32`（ConfigurationType=Application） |
| MySQL 连接失败 | easy-mysql-mcp 未启动 | 检查 MCP 配置中的 MYSQL_HOST/PORT 是否正确 |
| UI 测试找不到字段 | WPF 只读 TextBox 的 UIA Name 为空 | 依赖 `ValuePattern.Value` 匹配标签文本（见 `IsoExample1UiTest.BuildLabelEditMap` 批量映射） |
| UI 测试只填到几何参数 | 渐开线齿轮副页面用内层 TabControl 分页 | 需逐 Tab 切换填充（几何/物性/加工/应用/精度/安装），非滚动 |
| 测试后 WPF 应用被关闭 | `dotnet run` 的进程树清理 | 成功时 `keepOpen=true` 不关闭进程；建议直接运行 `RVAutoTest.exe` |

## 协作 Agent

| Agent | 在本技能中的角色 |
|-------|-----------------|
| `planner` | 分析需求，确定测试范围和参数来源 |
| `implementer` | 执行代码修改、编译、运行 |
| `review` | 审查测试代码质量和结果合理性 |
| `action` | 生成测试报告和运行脚本 |
