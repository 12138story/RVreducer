# RVAutoTest — RV 减速器全自动测试工具

`RVAutoTest` 是 RV 减速器设计计算系统的**全自动测试与截图工具**。它通过 P/Invoke 直接调用 `RVReducerBasic.dll` 核心计算引擎，并用 UIA 驱动 WPF 应用（`RVReducerDesignApp`）完成界面交互与截图，实现"**输入一组参数 → 自动完成全部计算 → 输出结果 JSON + 界面截图**"的闭环流程。

---

## 1. 功能概览

一次运行自动执行 **4 个计算场景**：

| 序号 | 场景 | 说明 | DLL 接口 |
|------|------|------|----------|
| ① | **齿轮推荐** | 由总体参数（传动比/扭矩/转速/寿命）推荐渐开线+摆线几何参数 | `RecommondPara` |
| ② | **渐开线齿轮副** | 计算太阳轮/行星轮副接触/弯曲应力及几何（各 142 字段） | `CalInvGearStress2` |
| ③ | **摆线针轮副** | 计算摆线接触应力、K1、跨棒距、精度公差等（9 字段） | `CalCycloidGearStressByObj` |
| ④ | **轴推荐 + 曲轴图形** | 推荐曲轴直径/长度，装配并绘制曲轴 2D 图形 | `RecommondShaftPara` |

> ②③ 场景的几何参数默认从 ① 推荐输出**自动映射**（数据闭环），也可在配置文件中单独覆盖。

---

## 2. 目录结构

```
RVAutoTest/
├── RVAutoTest.csproj          # .NET 8 WPF 控制台项目
├── Program.cs                 # 主入口：场景编排、报告生成、退出码
├── DllInvoker.cs              # P/Invoke 封装（DLL 直调）
├── UIAutomator.cs             # UIA 自动化（找元素/点按钮/读表格/截图）
├── ConfigLoader.cs            # 配置加载与校验
├── TestConfig.cs              # 配置模型（各场景配置类）
├── JsonOptions.cs             # JSON 序列化选项（中文直出、支持 NaN/∞）
├── CycloidGearInput.cs        # 摆线针轮副 60 字段输入构建器
├── CycloidGearOnlyTest.cs     # 摆线针轮副独立计算（--cycloid，输入/输出 JSON 落盘，无截图）
├── InvGearInput.cs            # 渐开线齿轮副 174 字段输入构建器
├── VerifyCycloidUi.cs         # 调试工具：驱动 UI 摆线页面计算对比
├── Models/                    # 结果模型（TestResults / InvGearResult / CycloidGearResult）
├── Scenarios/                 # 各场景实现（GearRecommend/InvGear/CycloidGear/ShaftRecommend）
├── test_params.json           # 测试参数配置（可编辑）
├── test_params_template.json  # 配置模板（含字段说明注释）
├── cycloid_params.json        # 摆线针轮副独立计算输入参数（--cycloid）
└── README.md                  # 本文档
```

---

## 3. 环境要求

| 依赖 | 说明 |
|------|------|
| **.NET SDK 8.0** | 编译运行 RVAutoTest 与 WPF 应用 |
| **Visual Studio / MSBuild (VC++)** | 编译 C++ 核心引擎 `RVReducerBasic.dll`（x64） |
| **MySQL 8.0** | 本地数据库，端口 **3729**，库 `bearings`（账号 root/jay），供摆线精度公差与轴承选型查询 |
| **Windows 10/11** | UIA 驱动 WPF 应用，需桌面会话 |

---

## 4. 编译

```powershell
# ① 编译 C++ 核心引擎（生成 RVReducerBasic.dll，x64 Debug）
#   需在 "x64 Native Tools Command Prompt for VS" 中执行：
msbuild RVReducerBasic.sln /p:Configuration=Debug /p:Platform=x64

# ② 编译 WPF 应用（将 DLL 及其依赖复制到输出目录）
dotnet build RVReducerDesignApp/RVReducerDesignApp.csproj

# ③ 编译测试工具
dotnet build RVAutoTest/RVAutoTest.csproj
```

> RVAutoTest.csproj 已配置自动复制 `RVReducerBasic.dll` 到运行目录；`RVReducerBasic.dll` 依赖 Intel MKL 等 40+ 原生 DLL，工具运行时会从 WPF 输出目录整体复制，请确保 ② 已执行。

---

## 5. 运行

### 5.1 使用默认参数

```powershell
dotnet run --project RVAutoTest/RVAutoTest.csproj
```

### 5.2 使用自定义参数（推荐）

```powershell
dotnet run --project RVAutoTest/RVAutoTest.csproj -- --config RVAutoTest/test_params.json
```

> 运行时会自动启动 WPF 应用（窗口标题"摆线针轮行星减速器设计系统"）。**请勿手动操作窗口**，全程约 1~2 分钟，完成后自动关闭。

### 5.3 命令行参数

| 参数 | 说明 |
|------|------|
| `--config <path>` / `-c <path>` / `--config=<path>` | 指定配置 JSON 路径；缺省用内置默认参数 |
| `--verify-cycloid-ui` | 调试模式：驱动 UI 摆线页面计算并读取输出（用于对比 DLL 直调与 UI 结果） |
| `--iso-example1` | ISO/TR 6336-30:2022 Example 1 渐开线齿轮副验证（DLL 直调，无需 UI，可跟参数文件路径） |
| `--cycloid [<json>]` | 摆线针轮副独立计算（DLL 直调，无需 UI/截图；输入/输出 JSON 落盘，可跟参数文件路径） |

### 5.4 ISO/TR 6336-30 Example 1 验证

用 ISO/TR 6336-30:2022（Annex A）的 Example 1 单斜齿渗碳淬火齿轮副参数
（z=17/103, mn=8, αn=20°, β=15.8°, a=500, x=0.145/0, T1=9000 N·m）驱动 `CalInvGearStress2`，
将结果与 ISO 标准期望值（σH/σF/几何）对比输出误差报告：

```powershell
# 使用默认参数文件（iso_example1_params.json）
dotnet run --project RVAutoTest/RVAutoTest.csproj -- --iso-example1

# 指定参数文件
dotnet run --project RVAutoTest/RVAutoTest.csproj -- --iso-example1 RVAutoTest/iso_example1_params.json
```

输出：每次运行生成时间戳子文件夹 `screenshots/iso-example1/<yyyyMMdd-HHmmss>/`，
内含 `iso-example1-result.json`（完整 284 字段）、`iso-example1-comparison.json`（对比报告）、`iso-example1-comparison.md`（对比表格）。
退出码：偏差 <5% → 0；5%~15% → 1；>15% → 2。

**已知验证结果**（2026-08-01）：
- ✅ 几何 100% 吻合（分度圆/齿顶圆/基圆/工作节圆直径偏差 0.00%）→ 输入参数映射正确
- ✅ σH0 名义接触应力 0.04%、σHP/σFP 许用应力 <1.3% → 应力公式正确
- ⚠️ σH 计算接触应力 4.37% 偏小（1242 vs 1299）；σF 齿根应力 13.85%/9.28% 偏小
- 原因：DLL 载荷系数（Kv/KHβ/KFβ）与产形变位系数（xEs=0.1335 vs ISO 0.1178）的算法实现差异

### 5.4 退出码

| 退出码 | 含义 |
|--------|------|
| `0` | 全部场景成功（`report.json` 中 `Success: true`） |
| `1` | 存在失败场景（`report.json` 中 `Errors` 列出具体原因） |
| `2` | 致命错误（配置解析失败等） |

### 5.5 摆线针轮副独立计算（--cycloid）

只做摆线针轮副计算：从 `cycloid_params.json`（或命令行指定文件）读取输入参数，
调用 `CalCycloidGearStressByObj` 计算，将**输入参数与输出参数分别写入 JSON 文件**；
不启动 WPF、不截图。

```powershell
# 使用默认参数文件（RVAutoTest/cycloid_params.json，已复制到运行目录）
dotnet run --project RVAutoTest/RVAutoTest.csproj -- --cycloid

# 指定参数文件
dotnet run --project RVAutoTest/RVAutoTest.csproj -- --cycloid RVAutoTest/cycloid_params.json
```

输出：每次运行生成时间戳子文件夹 `screenshots/cycloid-gear/<yyyyMMdd-HHmmss>/`，内含：

| 文件 | 内容 |
|------|------|
| `cycloid-input.json` | 输入参数（力矩为 N·m；DLL 实际输入 = 力矩 × TorqueScale 得 N·mm） |
| `cycloid-result.json` | 输出参数 9 字段（接触应力、K1、齿顶距、跨棒距、精度公差） |

退出码：`0` 成功；`1` DLL 调用失败或结果无效（接触应力 ≤0 / ≥1e9 失败标记 / NaN）；`2` 致命错误（DLL 依赖缺失等）。

---

## 6. 参数配置（test_params.json）

配置文件为 JSON 格式，字段**缺省或为 `0` 时使用默认值**（配置不合法时工具会报错并中止）。完整字段与注释见 `test_params_template.json`。

```jsonc
{
  "gearRecommend": {              // ① 齿轮推荐
    "fixElemNotation": 1,         //   固定部件: 0=针齿壳固定, 1=曲轴固定
    "totalRatio": 153,            //   总传动比
    "outputTorque": -784,         //   输出力矩 (N·m)
    "outputRPM": 15,              //   输出转速 (r/min)
    "targetStress": 1000,         //   目标接触应力
    "designLife": 50000,          //   设计寿命 (h)
    "usageFactor": 1.2,           //   使用系数
    "allowableStress": 1500,      //   材料许用极限接触应力
    "planetCount": 3,             //   行星轮个数
    "cycloidMinWidth": 40,        //   摆线轮最小齿宽 (mm)
    "cycloidMaxWidth": 80         //   摆线轮最大齿宽 (mm)
  },

  "cycloidGear": {                // ③ 摆线针轮副（缺省→用推荐结果映射）
    "precGrade": 5,               //   精度等级 0~12（默认5，0 级在 DLL 数据库反查会失败）
    "torqueScale": 1000,          //   力矩缩放：DLL 期望 N·mm 而推荐输出 N·m，默认 ×1000；设 1 关闭
    "pinionZ": 40,                //   针轮齿数
    "rollerRadius": 4,            //   滚针半径 (mm)
    "pitchCircleRadius": 98.5,    //   针轮分度圆半径 (mm)
    "pinionWidth": 80,            //   针轮设计齿宽 (mm)
    "cycloid1Eccentricity": 2,    //   摆线轮1偏心距 (mm)
    "cycloid2Eccentricity": 2     //   摆线轮2偏心距 (mm)
  },

  "invGear": {                    // ② 渐开线齿轮副（缺省→用推荐结果映射）
    "module": 2,                  //   法向模数 (mm)
    "pressureAngle": 25,          //   压力角 (度)
    "helixAngle": 0,              //   螺旋角 (度)
    "centerDistance": 42,         //   中心距 (mm)
    "sunTeeth": 23,               //   太阳轮齿数
    "planetTeeth": 106,           //   行星轮齿数
    "planetCount": 3,             //   行星轮个数
    "sunWidth": 100,              //   太阳轮设计齿宽 (mm)
    "planetWidth": 100,           //   行星轮设计齿宽 (mm)
    "torque": -2295,              //   输入扭矩 (N·m)
    "speed": 5.124,               //   输入转速 (r/min)
    "designLife": 50000           //   设计寿命 (h)
  },

  "shaftRecommend": {             // ④ 轴推荐
    "allowableBendStress": 4,     //   许用弯曲正应力 (MPa)
    "allowableShearStress": 12,   //   许用剪应力 (MPa)
    "bearingDesignLife": 6700     //   轴承设计寿命 (h)
  }
}
```

> **注意**：`fixElemNotation`、`torque` 等允许为 0/负值（是合法输入），因此配置采用"`null` 表示未配置"而非"0 哨兵"。

---

## 7. 输出结果

所有输出写入工作区根目录 **`screenshots/`** 文件夹。

### 7.1 汇总报告 `report.json`

```jsonc
{
  "ToolVersion": "1.0.0",
  "GeneratedAt": "...",
  "Success": true,                // 全部场景成功与否
  "Errors": [],                   // 失败场景的原因列表
  "GearRecommend": { ... },       // 场景①结果
  "InvGear": { ... },             // 场景②结果
  "CycloidGear": { ... },         // 场景③结果
  "ShaftRecommend": { ... }       // 场景④结果
}
```

### 7.2 各场景结果文件

| 文件 | 内容 |
|------|------|
| `gear-recommend-result.json` | 齿轮推荐 91 字段（模数/压力角/中心距 + 齿轮1/2 各 31 + 滚针 + 摆线轮 + 扭矩/转速） |
| `gear-recommend-outdata.json` | 齿轮推荐 DLL 原始输出数组（供 ②③ 数据闭环，可忽略） |
| `inv-gear-result.json` | 渐开线齿轮副：太阳轮/行星轮字典各 142 字段 |
| `cycloid-gear-result.json` | 摆线针轮副 9 字段（见下） |
| `shaft-recommend-result.json` | 轴推荐：`RecommendedDiameter`、`RecommendedLength`、`CrankShaftCount` |

### 7.3 摆线针轮副结果字段（`cycloid-gear-result.json`）

| 字段 | 含义 | 单位 |
|------|------|------|
| `接触应力` | 摆线最大接触应力（已做力矩单位修正） | MPa |
| `全齿高h` | 摆线齿全齿高 | mm |
| `短幅系数K1` | 短幅系数 | — |
| `齿顶距D_T` | 齿顶距 | mm |
| `跨棒距M_Mf` | 跨棒距（量棒测量值） | mm |
| `单个齿距偏差f_pt` | 齿距偏差（查数据库精度表） | μm |
| `齿距累积总偏差F_p` | 齿距累积总偏差 | μm |
| `齿向偏差F_β` | 齿向偏差 | μm |
| `径向跳动F_r` | 径向跳动 | μm |

### 7.4 截图

| 文件 | 内容 |
|------|------|
| `01-gear-recommend-panel.png` | 齿轮推荐抽屉面板 |
| `02-gear-recommend-output.png` | 齿轮推荐结果 |
| `03-shaft-recommend-input.png` | 轴推荐抽屉面板 |
| `04-shaft-recommend-output.png` | 轴推荐结果 |
| `05-crankshaft-drawing.png` | 曲轴 2D 图形（由推荐轴数据装配绘制） |

---

## 8. 已知问题与注意事项

1. **摆线力矩单位（重要）**
   摆线 DLL 公式 `FMax = 4·T/(K1·zg·Rz)` 期望力矩单位为 **N·mm**，但 UI/推荐系统以 **N·m** 传递，导致应力偏小约 31.6 倍。本工具默认对摆线力矩 ×1000 修正（`torqueScale` 配置）。**UI 应用未做此修正**，可能显示异常值（如 `1e10` 失败标记）。

2. **摆线精度等级**
   默认 `precGrade = 5`。等级 0 时 DLL 内部对数据库 `f_p` 表反查必然失败（数据不一致），请勿使用 0。

3. **MySQL 依赖**
   摆线精度公差查询依赖本地 MySQL（`bearings` 库，端口 3729，root/jay）。MySQL 未启动或账号不符会导致摆线场景失败。

4. **渐开线直齿轮**
   螺旋角 β=0（直齿轮）时部分输出为 `∞`/`NaN`，工具已兼容（JSON 支持命名浮点字面量）。

5. **轴推荐 UI 稳定性**
   轴推荐依赖 UIA 驱动真实应用，偶发"未找到确定按钮"（UI 状态问题）。失败时 `report.json` 的 `Errors` 会给出具体原因，可重试。

---

## 9. 常见问题（FAQ）

**Q1: 报错"未找到 RVReducerDesignApp.exe"？**
先执行 `dotnet build RVReducerDesignApp/RVReducerDesignApp.csproj`（Debug），确保应用已编译。

**Q2: 摆线场景报 MySQL 连接失败？**
启动本地 MySQL 服务并确认端口 3729、账号 root/jay、库 `bearings` 存在。

**Q3: 退出码 1，report.json 的 Errors 有内容？**
按 `Errors` 中描述定位：齿轮推荐 DLL 调用失败、渐开线/摆线输出无效、轴推荐读取失败等。多为输入参数非法或依赖缺失。

**Q4: 如何只查看某个场景？**
当前工具为整体编排运行；调试单个摆线 UI 计算可用 `--verify-cycloid-ui`。

**Q5: 修改了参数但结果没变？**
确认配置文件路径正确（`--config` 指定），并检查字段名与模板一致（JSON 大小写不敏感）。

---

## 10. 扩展指南（开发者）

- **新增场景**：在 `Scenarios/` 添加 `XxxScenario.cs`，在 `Models/TestResults.cs` 的 `Report` 增加对应结果属性，并在 `Program.cs` 场景编排区加入 try-catch 调用。
- **新增输入参数**：在 `TestConfig.cs` 增加配置类字段，在 `ConfigLoader.Validate` 中加合法性校验，在 `test_params_template.json` 中登记字段说明。
- **调试 UI 自动化**：用 `--verify-cycloid-ui` 作为参考模板，查看 `UIAutomator.cs` 提供的元素查找/表格读取/截图方法。
