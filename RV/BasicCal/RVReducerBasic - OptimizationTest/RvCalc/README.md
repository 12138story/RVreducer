# RvCalc — 通用计算层（独立类库）

用「注册表 + 动态调用」代替「每个接口写一段 C#」：接口的输入数组布局、参数名、单位、输出字段
全部写在 `interface-registry.json` 里，运行时解析 `RVReducerBasic.dll` 的导出函数并调用。
**新增接口只改 JSON，不改代码。**

> 设计背景、设计原理（错误不静默/输出体检/有状态依赖/过程与结果分离等）、端到端流程图与踩坑清单，
> 见 **[docs/自然语言驱动计算-设计流程与原理.md](docs/自然语言驱动计算-设计流程与原理.md)**。

---

## 1. 工程结构与依赖方向

```
RvCalc/                     ← 独立类库（net8.0-windows），零业务项目依赖
├── RvCalc.csproj
├── RvInterfaceRegistry.cs    注册表模型 + JSON 加载（模板缺失只告警不抛）
├── DynamicDllCaller.cs       NativeLibrary.Load/GetExport + 函数指针，手动封送
├── RvCalcRunner.cs           语义参数 → 槽位映射、单位换算、输出体检
├── InvGearInput.cs           渐开线 174 字段输入构建器（模板来源）
├── CycloidGearInput.cs       摆线 60 字段输入构建器（模板来源）
├── AngleUtil.cs              角度换算
├── interface-registry.json   14 个导出的语义描述
└── templates/*.json          已验证的默认输入数组

RvCalcCli/                  ← 独立命令行（exe），仅引用 RvCalc
├── Program.cs                --call / --list-interfaces / --describe / --registry-check
├── DevTools.cs               --dump-templates / --selfcheck
└── DllBootstrap.cs           定位工作区、准备 DLL 及 250 个原生依赖、静态 DllImport 参照

RvCalcCli  →  RvCalc  ←  RVReducerDesignApp      （单向 ProjectReference）
```

- `RvCalc` 不依赖任何业务项目；`RvCalcCli` 也不依赖它们。
- `RVReducerDesignApp` 通过 `ProjectReference` 引用 `RvCalc`（复用输入构建器与 Agent 工具）。
- 想彻底解耦：删掉该 `ProjectReference` 即可（见第 4 节）。

> 注：旧工具 `RVAutoTest`（UIA 驱四场景 + 截图 + ISO 验证）已于 2026-09-14 移除，
> 源码与配置备份在 `_archive/RVAutoTest-source-*.zip`；其计算类能力已由 `RvCalcCli` 覆盖。

---

## 2. 编译与运行

```powershell
# 依赖准备顺序：C++ 引擎(x64) → WPF 应用(Debug) → 任意消费者
msbuild RVReducerBasic.sln /p:Configuration=Debug /p:Platform=x64   # 生成 RVReducerBasic.dll
dotnet build RVReducerDesignApp/RVReducerDesignApp.csproj           # 提供 DLL + 250 个原生依赖

dotnet build RvCalc/RvCalc.csproj
dotnet build RvCalcCli/RvCalcCli.csproj
dotnet run  --project RvCalcCli -- --list-interfaces
```

> 运行目录若缺少 `RVReducerBasic.dll` 或其原生依赖，`RvCalcCli` 会自动从
> `RVReducerDesignApp/bin/Debug/net8.0-windows` 整体复制全部 DLL（含 MKL 分派、MySQL 客户端）。
> 也可用环境变量 `RV_DLL_PATH` 直接指定 DLL 路径（此时无需工作区源码）。

---

## 3. 常用命令

```powershell
# ① 看有哪些接口、每个接口能传什么参数
RvCalcCli --list-interfaces
RvCalcCli --describe CalBearingLife

# ② 通用调用（参数名支持中文主名/别名；数值可带单位 kN、N·m、rpm、μm、°）
RvCalcCli --call CalBearingLife --args '{"轴承类型":"圆锥滚子","转速":"2000 rpm","Cr":"23.8 kN","C0r":"19.5 kN","径向力Fr":"1750 N","轴向力Fa":"740 N","e":0.35,"Y":1.7,"Y0":0.9}'
RvCalcCli --call CalInvGearStress2 --args '{"法向模数":5,"太阳轮齿数":20,"行星轮齿数":106,"中心距":315,"扭矩":500,"转速":1000}'
RvCalcCli --call CalCycloidGearStressByObj --args '{"摆线轮总力矩":"764.4 N·m"}'
RvCalcCli --call CalInvGearStress2 --args @args.json      # 参数也可写在文件里

# ③ 维护
RvCalcCli --registry-check      # 注册表 ↔ DLL 导出核对 + 模板就绪检查
RvCalcCli --dump-templates      # 重新生成 templates/*.json
RvCalcCli --selfcheck           # 静态 DllImport 与通用调用器结果比对

# ④ 多步同进程调用（有状态依赖的接口：DLL 用全局对象保存整机状态）
RvCalcCli --sequence '@{"steps":[{"interface":"CalCrankShaftSystem2","args":{"arraysFile":"snap.json"}},{"interface":"RecommondShaftPara","args":{"arraysFile":"snap.json"}},{"interface":"GetForceAndForceLocation","args":{}}]}'
```

### 3.1 整机接口（CalCrankShaftSystem2 / RecommondShaftPara）

这两个接口需要 4~5 组数组（分割索引 + 零部件索引 + 整机输入 + 输出），不是几组标量参数能表达的，
所以用「整机快照」驱动：

```powershell
# ① 从界面导出快照（三种方式任选）
#   a) 无界面导出（未设计曲轴时 meta.partial=true，仅供查看结构）
$env:RV_SNAPSHOT_DIR = "screenshots\snapshots"   # 相对路径按工作区根解析（项目内，便于整体迁移）
#      设置后，界面里每完成一次「曲轴系计算/轴推荐」就自动导出快照 + 曲轴系计算结果 JSON
#   b) 启动参数：RVReducerDesignApp.exe --export-snapshot <输出路径>
#   c) 应用内 Agent 工具：export_whole_machine_snapshot

# ② 用快照驱动（数组名与注册表 args 同名，自动识别并按其真实长度分配）
RvCalcCli --call CalCrankShaftSystem2 --args '{"arraysFile":"RvCalc/templates/wholeMachine-snapshot.json"}'

# ③ 有状态依赖的接口用 --sequence（必须先在同一进程算过曲轴系）
RvCalcCli --sequence '@{"steps":[
  {"interface":"CalCrankShaftSystem2","args":{"arraysFile":"snap.json"}},
  {"interface":"RecommondShaftPara","args":{"arraysFile":"snap.json"}},
  {"interface":"GetForceAndForceLocation","args":{}}]}'
```

要点：

- 快照含 `meta.partial` 与 `meta.warnings`：轴段未设计时会标 `true`，此时不足以驱动曲轴系计算。
- 快照导出默认「原样导出当前数组」（`recompute=false`）：保证与 DLL 实际收到的数组一致。
- 数组长度由快照决定，调用器会自动按实际长度分配（比注册表声明长时扩容，短时补 0 并提示）。
- `CalCrankShaftSystem2` 在 DLL 里没有写回总体输出（源码中 `CalTotalTorqueAndRPM` 被注释），
  因此 out[0..3] 无意义；曲轴应力分布请用紧接其后的 `GetForceAndForceLocation`。

行为约定：

| 情况 | 行为 |
|------|------|
| 参数名不认识 | 直接拒绝，并列出该接口可用参数名（**不再静默用默认值**） |
| 缺必填参数 | 拒绝并指出缺哪个（模板已有非零值则视为已提供） |
| 单位不匹配 / 无法识别的单位 | 拒绝并提示期望单位 |
| 输出 NaN / ∞ / ≥1e9 / 非规格化极小值 | 该字段标记为可疑（后者通常表示「该槽位未被 DLL 写入」） |
| 全部输出均无效 | 判定调用被拒绝，附接口说明 |
| DLL 缺失或依赖不全 | 提示编译步骤，或改用 `RV_DLL_PATH` |

单位换算支持：`kN/N/kgf`、`mm/cm/m/μm`、`N·m/N·mm`、`MPa/GPa/kPa`、`r/min/rpm`、`h`、`°`、`kg/m³↔kg/mm³`。

---

## 4. 解耦 / 删除依赖

| 目标 | 操作 |
|------|------|
| 在别处复用通用计算 | 引用 `RvCalc.csproj`（或直接 `--call` 调用 `RvCalcCli`） |
| 让 RVAutoTest 不再参与 | 删除 `RVAutoTest/RVAutoTest.csproj` 里的 `<ProjectReference Include="..\RvCalc\RvCalc.csproj" />` 及 `None` 拷贝项；删除 `RVAutoTest/InvGearInput.cs`‑相关 using（构建器已迁到 RvCalc） |
| 让 WPF 应用不引入通用计算 | 删除 `RVReducerDesignApp.csproj` 的 `ProjectReference`/`None` 与 `Services/Agent/AgentRvCalcTool.cs`，并把 `AgentToolbox.GetAll()` 中的 `AgentRvCalcTool.Build()` 去掉 |
| 完全移除 RVAutoTest 项目 | 从 `RVReducerBasic.sln` 删除其 `Project(...)` 与对应 `ProjectConfigurationPlatforms` 条目即可；`RvCalc`/`RvCalcCli` 与 C++ 引擎、WPF 应用均不依赖它 |

---

## 5. 已知边界

- `CalCrankShaftSystem2` / `RecommondShaftPara` 需要整机快照（见 3.1）。两者依赖 **DLL 进程内全局状态**
  （`gRVReducer`）：`CalCrankShaftSystem2` 会把整机（含轴系）装入全局对象，因此 `RecommondShaftPara` /
  `GetForceAndForceLocation` 必须先在同一进程执行过它（用 `--sequence`）。无界面下用快照复现时，
  `b_Design` 等来自全局对象的量取决于「先前算过哪些接口」，与界面连续操作的结果可能略有差异
  （实测轴长差 6.0 mm）；如需完全一致，请在序列里先跑与界面相同的接口链。
- `GetForceAndForceLocation` 依赖同一进程内先执行过 `CalCrankShaftSystem2`，单独调用返回 0 节点。
- 摆线四项精度公差依赖 MySQL（`localhost:3729`，库 `bearings`）：查库失败时 DLL 现返回 -1（不再崩溃），
  C# 侧会将其标为无效。数据库不可用时其余摆线结果仍然有效。
- 大端/小端、单位等约定以 DLL 实际期望为准（例如 `Cr/C0r` 传 N，数据库里的 kN 值需 ×1000）。
- 部分原生异常（如轴段数据缺失导致的访问违例）会以 `SEHException` 形式返回；已作为「DLL 调用失败」
  报告，不会终止宿主进程。

---

## 6. 新增接口的流程

1. 在 `RVReducerBasic/InterfaceForCSharp.cpp` 找到该导出函数的实现，确认：
   - 参数顺序与类型（`double*` / `int*` / `char**`）；
   - 输入数组长度与分段起点（`ConvertInputDataTo*Obj` 里的下标）；
   - 输出数组长度与每个字段的含义。
2. 在 `interface-registry.json` 追加一条：`args`（数组长度/方向）、`params`（参数名→槽位→单位）、
   `outputs`（字段名→槽位→单位/有效范围）、必要时 `templateFile`。
3. `RvCalcCli --registry-check` 确认导出名与模板就绪；用 `--call` 跑一组已知算例核对数值。
4. 若涉及代码生成模板，在 `DevTools.DumpTemplates()` 中补一段并重跑 `--dump-templates`。
