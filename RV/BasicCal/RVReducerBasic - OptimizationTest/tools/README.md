# tools —— 项目内工具脚本

> 约定：本项目所有脚本、临时文件、生成物一律放在**项目目录内**（不写系统临时目录、不写 C 盘其它位置），
> 以便整个目录可直接拷贝迁移。

| 脚本 | 用途 |
|------|------|
| `patch-source-safe.ps1` | 编码安全地替换源码字符串（RVReducerBasic 源码 UTF-8 with BOM / GBK 混用） |

## patch-source-safe.ps1

```powershell
# 单处替换
pwsh tools/patch-source-safe.ps1 -Path RVReducerBasic/TANPinionCycloidGear.cpp -Old "旧文本" -New "新文本"

# 多处替换（补丁 JSON 也纳入版本管理，推荐）
# tools/patches/xxx.json = [{"old":"...","new":"...","count":1}, ...]
pwsh tools/patch-source-safe.ps1 -Path RVReducerBasic/TANPinionCycloidGear.cpp -PatchFile tools/patches/xxx.json
```

行为要点：

1. **自动探测编码**：严格 UTF-8 解码成功 → UTF-8（按原文件有无 BOM 回写）；失败 → GBK(936)。
   > ⚠️ `RVReducerBasic/*.cpp` **并非全部 GBK**（例如 `TANPinionCycloidGear.cpp` 是 UTF-8 with BOM）。
   > 一律按 GBK 读写会把该文件所有中文行写成乱码。
2. 剥离解码得到的 `U+FEFF`，避免写出"双 BOM"。
3. **命中次数校验**：每处替换必须命中指定次数（默认 1），否则报错且**不修改文件**。
4. 写回前校验无 `U+FFFD`；写回后打印 `git diff --numstat` 与改动 hunk 头，便于确认改动范围。
5. 纯大小写替换也生效（内部用大小写敏感比较）。

## 相关约定（避免迁移失败）

- 快照/结果输出目录用**项目内相对路径**：如 `RV_SNAPSHOT_DIR=screenshots\snapshots`（相对路径按工作区根解析）。
- 测试产物：通用层快照 → `RvCalc/templates/`；`RvCalcCli` 依赖复制 → 各自 `bin/`；Agent 执行过程 → `logs/agent/`。
