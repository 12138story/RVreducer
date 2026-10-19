using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace RVReducerDesignApp.Services.Agent;

/// <summary>
/// 内置工具集：命令执行、目录列表、文件读写
/// 安全约束：命令与文件操作均限制在工作区沙箱内，拦截破坏性命令。
/// </summary>
public static class AgentToolbox
{
    /// <summary>命令最长执行时间</summary>
    private const int CommandTimeoutSeconds = 120;

    /// <summary>读取文件时单次读取最大字符数</summary>
    private const int DefaultMaxChars = 8000;

    /// <summary>允许执行的命令前缀白名单（仅编译/测试类命令）</summary>
    private static readonly string[] AllowedCommandPrefixes =
    [
        "dotnet ", "msbuild ", "nuget ", "git ", "cmake ", "make ",
    ];

    /// <summary>PowerShell 脚本拼接元字符（命令分隔/管道/重定向/子表达式/续行），命中即拒绝以保证单条命令</summary>
    private static readonly Regex ForbiddenShellMeta = new(
        "[;|&`$()<>%^!\\[\\]\r\n]", RegexOptions.Compiled);

    /// <summary>破坏性命令检测（白名单之外的补充防线，命中则拒绝执行）</summary>
    private static readonly Regex[] DangerousCommandPatterns =
    [
        new(@"\bRemove-Item\b", RegexOptions.IgnoreCase),
        new(@"\brm\s+-rf\b", RegexOptions.IgnoreCase),
        new(@"\bdel(?:\s|$)", RegexOptions.IgnoreCase),
        new(@"\berase(?:\s|$)", RegexOptions.IgnoreCase),
        new(@"\brd\s+/[SQ]", RegexOptions.IgnoreCase),
        new(@"\brmdir\s+/[SQ]", RegexOptions.IgnoreCase),
        new(@"\bformat\s+[A-Za-z]:", RegexOptions.IgnoreCase),
        new(@"\bshutdown\s+[/-]", RegexOptions.IgnoreCase),
        new(@"\breg\s+delete", RegexOptions.IgnoreCase),
        new(@"\bnet\s+user\b", RegexOptions.IgnoreCase),
        new(@"\b(?:Stop|Restart|Reset)-Computer\b", RegexOptions.IgnoreCase),
        new(@"\bInvoke-Expression\b|\biex\s", RegexOptions.IgnoreCase),
        new(@"\bClear-RecycleBin\b", RegexOptions.IgnoreCase),
        new(@"\bSet-MpPreference\b", RegexOptions.IgnoreCase),
    ];

    /// <summary>注册所有可用工具</summary>
    public static List<AgentTool> GetAll() =>
        [RunCommandTool, ListDirectoryTool, ReadFileTool, WriteFileTool,
         AgentCalcTools.BuildInvGearStressTool(),
         AgentRvCalcTool.Build(),
         AgentRvCalcTool.BuildSnapshotExporter(),
         AgentMachineTools.BuildDesignStateTool(),
         AgentMachineTools.BuildPartCalcTool(),
         AgentMachineTools.BuildCrankShaftCalcTool(),
         AgentMachineTools.BuildBearingLifeTool()];

    // ═══════════════════ run_command ═══════════════════
    private static AgentTool RunCommandTool => new()
    {
        Name = "run_command",
        Description =
            "运行编译/测试命令并返回输出，仅支持单个命令（dotnet/msbuild/nuget/git/cmake/make）。" +
            "不支持管道、分号、重定向。working_directory 可留空表示工作区根目录。命令最长 120 秒。",
        Parameters = JsonSchema(new
        {
            type = "object",
            properties = new
            {
                command = new { type = "string", description = "要执行的单个命令" },
                working_directory = new { type = "string", description = "工作目录（可选，须在工作区内）" }
            },
            required = new[] { "command" }
        }),
        ExecuteAsync = async (argsJson, ct) =>
        {
            using var doc = JsonDocument.Parse(argsJson);
            var root = doc.RootElement;
            string command = root.GetProperty("command").GetString() ?? "";
            string? workDir = root.TryGetProperty("working_directory", out var w) ? w.GetString() : null;

            if (string.IsNullOrWhiteSpace(command))
                return "命令为空，无法执行。";
            if (!IsAllowedCommand(command))
                return "⛔ 已拦截：仅允许白名单命令（dotnet/msbuild/nuget/git/cmake/make），且不支持管道/分号/重定向。";
            if (IsDangerousCommand(command))
                return "⛔ 已拦截：该命令包含破坏性/危险操作，被安全策略拒绝。";

            string? sandboxError = workDir != null ? EnsureInWorkspace(workDir) : null;
            if (sandboxError != null)
                return sandboxError;

            return await RunCommandAsync(command, workDir, ct);
        }
    };

    // ═══════════════════ list_directory ═══════════════════
    private static AgentTool ListDirectoryTool => new()
    {
        Name = "list_directory",
        Description = "列出指定目录下的文件和子文件夹，帮助了解项目结构。",
        Parameters = JsonSchema(new
        {
            type = "object",
            properties = new { path = new { type = "string", description = "目录绝对路径" } },
            required = new[] { "path" }
        }),
        ExecuteAsync = (argsJson, _) =>
        {
            using var doc = JsonDocument.Parse(argsJson);
            string path = doc.RootElement.GetProperty("path").GetString() ?? "";

            string? sandboxError = EnsureInWorkspace(path);
            if (sandboxError != null)
                return Task.FromResult(sandboxError);
            if (!Directory.Exists(path))
                return Task.FromResult($"目录不存在: {path}");

            var sb = new StringBuilder();
            foreach (var d in Directory.GetDirectories(path))
                sb.AppendLine($"[目录] {Path.GetFileName(d)}");
            foreach (var f in Directory.GetFiles(path))
                sb.AppendLine($"[文件] {Path.GetFileName(f)}");
            return Task.FromResult(sb.Length == 0 ? "(空目录)" : sb.ToString());
        }
    };

    // ═══════════════════ read_file ═══════════════════
    private static AgentTool ReadFileTool => new()
    {
        Name = "read_file",
        Description = "读取文本文件内容（如 .cs/.cpp/.json/.md）。仅限工作区内文件。",
        Parameters = JsonSchema(new
        {
            type = "object",
            properties = new
            {
                path = new { type = "string", description = "文件绝对路径" },
                max_chars = new { type = "integer", description = $"最多读取字符数（默认 {DefaultMaxChars}）" }
            },
            required = new[] { "path" }
        }),
        ExecuteAsync = (argsJson, _) =>
        {
            using var doc = JsonDocument.Parse(argsJson);
            string path = doc.RootElement.GetProperty("path").GetString() ?? "";
            int maxChars = doc.RootElement.TryGetProperty("max_chars", out var m)
                ? Math.Max(0, m.GetInt32()) : DefaultMaxChars;

            string? sandboxError = EnsureInWorkspace(path);
            if (sandboxError != null)
                return Task.FromResult(sandboxError);
            if (!File.Exists(path))
                return Task.FromResult($"文件不存在: {path}");

            string content = File.ReadAllText(path, Encoding.UTF8);
            if (content.Length > maxChars)
                content = content[..maxChars] + "\n...[已截断]";
            return Task.FromResult(content);
        }
    };

    // ═══════════════════ write_file ═══════════════════
    private static AgentTool WriteFileTool => new()
    {
        Name = "write_file",
        Description = "写入文本内容到文件（覆盖）。仅限工作区内路径。用于创建或修改代码文件。",
        Parameters = JsonSchema(new
        {
            type = "object",
            properties = new
            {
                path = new { type = "string", description = "文件绝对路径" },
                content = new { type = "string", description = "要写入的完整内容" }
            },
            required = new[] { "path", "content" }
        }),
        ExecuteAsync = (argsJson, _) =>
        {
            using var doc = JsonDocument.Parse(argsJson);
            string path = doc.RootElement.GetProperty("path").GetString() ?? "";
            string content = doc.RootElement.GetProperty("content").GetString() ?? "";

            string? sandboxError = EnsureInWorkspace(path);
            if (sandboxError != null)
                return Task.FromResult(sandboxError);

            string? dir = Path.GetDirectoryName(path);
            if (!string.IsNullOrEmpty(dir))
                Directory.CreateDirectory(dir);
            File.WriteAllText(path, content, Encoding.UTF8);
            return Task.FromResult($"已写入 {path} ({content.Length} 字符)");
        }
    };

    // ═══════════════════ 辅助 ═══════════════════
    private static BinaryData JsonSchema(object obj) =>
        BinaryData.FromString(JsonSerializer.Serialize(obj));

    /// <summary>危险命令拦截（白名单之外的补充防线）</summary>
    private static bool IsDangerousCommand(string command) =>
        DangerousCommandPatterns.Any(p => p.IsMatch(command));

    /// <summary>白名单校验：须为单条白名单命令（不含脚本拼接元字符）</summary>
    private static bool IsAllowedCommand(string command)
    {
        string trimmed = command.Trim();

        // 含 PowerShell 脚本拼接元字符 → 拒绝（保证单条命令，防 ;/|/>/$() 拼接绕过）
        if (ForbiddenShellMeta.IsMatch(trimmed))
            return false;

        return AllowedCommandPrefixes.Any(p =>
            trimmed.StartsWith(p, StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>解析路径并逐级解析符号链接/目录联接，返回最终真实路径</summary>
    private static string ResolveFullPath(string path)
    {
        string full = Path.GetFullPath(path);
        string? root = Path.GetPathRoot(full);
        if (string.IsNullOrEmpty(root))
            return full;

        try
        {
            string[] segments = Path.GetRelativePath(root, full)
                .Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar,
                       StringSplitOptions.RemoveEmptyEntries);
            string current = root;
            foreach (var segment in segments)
            {
                current = Path.Combine(current, segment);
                FileSystemInfo? info =
                    File.Exists(current) ? new FileInfo(current)
                    : Directory.Exists(current) ? new DirectoryInfo(current) : null;
                var target = info?.ResolveLinkTarget(returnFinalTarget: true);
                if (target != null)
                    current = target.FullName;
            }
            return current;
        }
        catch
        {
            return full; // 解析失败（如跨盘路径）时保持原路径，由 EnsureInWorkspace 后续校验
        }
    }

    /// <summary>校验路径位于工作区内（逐级解析符号链接后）；通过返回 null，否则返回错误信息</summary>
    private static string? EnsureInWorkspace(string path)
    {
        string? root = FindWorkspaceRoot();
        if (root == null)
            return "无法定位工作区根目录，操作被拒绝。";

        string full = ResolveFullPath(path);
        string rootFull = Path.GetFullPath(root);
        string rootPrefix = rootFull.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)
                             + Path.DirectorySeparatorChar;

        if (!full.Equals(rootFull, StringComparison.OrdinalIgnoreCase) &&
            !full.StartsWith(rootPrefix, StringComparison.OrdinalIgnoreCase))
            return $"拒绝访问工作区外路径: {path}";
        return null;
    }

    private static async Task<string> RunCommandAsync(
        string command, string? workDir, CancellationToken ct)
    {
        // 用 -EncodedCommand（UTF-16LE base64）避免引号/特殊字符注入
        string encoded = Convert.ToBase64String(Encoding.Unicode.GetBytes(command));
        var psi = new ProcessStartInfo("powershell",
            $"-NoProfile -ExecutionPolicy Bypass -EncodedCommand {encoded}")
        {
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true,
            WorkingDirectory = workDir ?? FindWorkspaceRoot() ?? Environment.CurrentDirectory
        };

        using var process = Process.Start(psi);
        if (process is null)
            return "无法启动进程";

        try
        {
            var outputTask = process.StandardOutput.ReadToEndAsync(ct);
            var errorTask = process.StandardError.ReadToEndAsync(ct);
            using var timeoutCts = CancellationTokenSource.CreateLinkedTokenSource(ct);
            timeoutCts.CancelAfter(TimeSpan.FromSeconds(CommandTimeoutSeconds));

            await process.WaitForExitAsync(timeoutCts.Token);
            string stdout = await outputTask;
            string stderr = await errorTask;

            // 输出封顶，避免超大构建日志
            const int MaxOutputChars = 12000;
            var sb = new StringBuilder();
            if (!string.IsNullOrWhiteSpace(stdout))
                sb.AppendLine("【输出】\n" + Truncate(stdout, MaxOutputChars));
            if (!string.IsNullOrWhiteSpace(stderr))
                sb.AppendLine("【错误】\n" + Truncate(stderr, MaxOutputChars));
            sb.AppendLine($"【退出码】{process.ExitCode}");
            return sb.ToString();
        }
        catch (OperationCanceledException) when (ct.IsCancellationRequested)
        {
            try { process.Kill(entireProcessTree: true); } catch { /* 已退出 */ }
            throw; // 用户取消，向上传播
        }
        catch (OperationCanceledException)
        {
            try { process.Kill(entireProcessTree: true); } catch { /* 已退出 */ }
            return $"⏳ 命令执行超时（{CommandTimeoutSeconds}s），已终止。";
        }
    }

    private static string Truncate(string s, int max) =>
        s.Length <= max ? s : s[..max] + "…";

    private static string? FindWorkspaceRoot()
    {
        var dir = new DirectoryInfo(AppContext.BaseDirectory);
        while (dir != null)
        {
            if (File.Exists(Path.Combine(dir.FullName, "RVReducerBasic.sln")))
                return dir.FullName;
            dir = dir.Parent;
        }
        return null;
    }
}
