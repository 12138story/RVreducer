using MySql.Data.MySqlClient;
using System;
using System.Collections;
using System.ComponentModel;
using System.Configuration.Install;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;

namespace ClassLibraryRVReducer
{
    [RunInstaller(true)]
    public partial class Installer1 : System.Configuration.Install.Installer
    {
        public Installer1()
        {
        }

        public override void Install(IDictionary stateSaver)
        {
            base.Install(stateSaver);
            RemoveOldMySQLService();
            InsertMySql();
            CreatDataBase();
            Log("安装成功！");
        }
        private void RemoveOldMySQLService()
        {
            string result = CommandHelper.Execute("sc query MySQLRV", 5000);

            if (!result.Contains("FAILED") && !result.Contains("not exist"))
            {
                Log("检测到已存在的 MySQLRV 服务，准备删除…");
                CommandHelper.Execute("sc stop MySQLRV", 10000);
                CommandHelper.Execute("sc delete MySQLRV", 10000);
                Thread.Sleep(2000);
                Log("旧 MySQLRV 服务已删除。");
            }
        }

        // ================================
        // 公用方法：路径格式化 / 占位符替换
        // ================================

        /// <summary>
        /// 将 Windows 路径转换为配置文件能识别的格式，如 C:\app → C:/app
        /// </summary>
        private string NormalizePath(string path)
        {
            if (string.IsNullOrWhiteSpace(path)) return "";
            return path.Replace("\\", "/").TrimEnd('/');
        }

        /// <summary>
        /// 替换文件中的占位符，如 %BaseDir%
        /// </summary>
        private void ReplacePathInFile(string filePath, string placeholder, string physicalRoot)
        {
            if (!File.Exists(filePath))
            {
                Log($"未找到文件：{filePath}");
                return;
            }

            string text = File.ReadAllText(filePath, Encoding.UTF8);
            text = text.Replace(placeholder, NormalizePath(physicalRoot));
            var utf8NoBom = new System.Text.UTF8Encoding(false);
            File.WriteAllText(filePath, text, utf8NoBom);


            Log($"替换占位符 {placeholder} → {NormalizePath(physicalRoot)}");
        }

        // ================================
        // 设置 root 密码
        // ================================

        protected void SetMysqlRootPassword()
        {
            string mysqlcon = "Data Source='localhost';Port='3729';Database='';User Id='root';Password='';"; // 初始无密码
            MySqlConnection conn = new MySqlConnection(mysqlcon);
            try
            {
                conn.Open();
                string sql = "ALTER USER 'root'@'localhost' IDENTIFIED BY 'jay';";
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                cmd.ExecuteNonQuery();
                Log("MySQL root 密码设置为 jay 成功！");
            }
            catch (Exception ex)
            {
                Log("MySQL 密码设置失败！" + ex.Message);
                throw;
            }
            finally
            {
                conn.Close();
            }
        }

        // ================================
        // 安装 MySQL
        // ================================

        protected void InsertMySql()
        {
            string physicalRoot = this.Context.Parameters["targetdir"]; // C:\xxx\
            string appPath = physicalRoot + "\\mysql-8.0.36-winx64\\";

            // 1. 替换 my.ini 中的路径
            ReplacePathInFile(appPath + "my.ini", "%BaseDir%", physicalRoot);

            // 2. 确保 data 目录存在
            string dataDir = Path.Combine(appPath, "data");
            if (!Directory.Exists(dataDir))
            {
                Directory.CreateDirectory(dataDir);
                Log($"创建数据目录：{dataDir}");
            }
            Log("创建 MySQL Windows 服务…");
            // 3. 初始化数据库（如果未初始化）
            string ibDataFile = Path.Combine(dataDir, "ibdata1");
            if (!File.Exists(ibDataFile))
            {
                Log("初始化 MySQL 数据目录...");
                string initResult = CommandHelper.Execute(
                    Path.Combine(appPath, "bin\\mysqld.exe"),
                    $"--defaults-file=\"{appPath}my.ini\" --initialize-insecure --console",
                    20000
                );
                Log(initResult);
                Thread.Sleep(3000);
            }
            // 2. 创建 Windows 服务
            string info1 = CommandHelper.Execute(appPath + "bin\\mysqld.exe",
                " install MySQLRV --defaults-file=\"" + appPath + "my.ini\"",
                 20000);

            Log(info1);
            Thread.Sleep(3000);

            Log("启动 MySQL 服务 (net start MySQLRV)…");

            // 3. 启动服务
            string info2 = CommandHelper.Execute("net start MySQLRV", 20000);
            Log(info2);

            Log("等待 MySQL 启动…");
            Thread.Sleep(5000);

            // 4. 连接测试 (初始无密码)
            MySqlConnection con = new MySqlConnection("Data Source='localhost';Port='3729';Database='';User Id='root';Password='';");
            try
            {
                con.Open();
                con.Close();
                Log("连接成功！");

                // 5. 设置 root 密码
                SetMysqlRootPassword();

                // 6. 测试密码是否生效
                MySqlConnection conWithPwd = new MySqlConnection("Data Source='localhost';Port='3729';Database='';User Id='root';Password='jay';");
                conWithPwd.Open();
                conWithPwd.Close();
                Log("使用密码 jay 连接成功！");
            }
            catch (Exception ex)
            {
                Log("连接失败！" + ex.Message);
            }
        }

        // ================================
        // 创建 bearings 数据库 并执行 SQL
        // ================================

        // ================================
        // 创建 bearings 数据库 并执行 SQL
        // ================================

        protected void CreatDataBase()
        {
            string physicalRoot = this.Context.Parameters["targetdir"];

            // 基础连接字符串（不包含具体数据库，用于建库）
            string baseConStr = "Data Source='localhost';Port='3729';User Id='root';Password='jay';";
            // 目标数据库连接字符串（指定了 Database='bearings'，用于执行表脚本）
            string dbConStr = "Data Source='localhost';Port='3729';Database='bearings';User Id='root';Password='jay';";

            FileInfo file = new FileInfo(physicalRoot + "\\net8.0-windows\\bearings.sql");

            if (!file.Exists)
            {
                Log("未找到 SQL 文件：" + file.FullName);
                return;
            }

            string sql = file.OpenText().ReadToEnd();

            try
            {
                // 1. 先用无数据库的连接，主动创建 bearings 数据库
                using (MySqlConnection baseConn = new MySqlConnection(baseConStr))
                {
                    baseConn.Open();
                    using (MySqlCommand cmd = new MySqlCommand("CREATE DATABASE IF NOT EXISTS `bearings` CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci;", baseConn))
                    {
                        cmd.ExecuteNonQuery();
                    }
                    Log("成功创建 bearings 数据库（或该数据库已存在）。");
                }

                // 2. 切换到带有 Database='bearings' 的连接来执行 SQL 脚本
                using (MySqlConnection dbConn = new MySqlConnection(dbConStr))
                {
                    MySqlScript script = new MySqlScript(dbConn)
                    {
                        Query = sql
                    };

                    int count = script.Execute();
                    Log("bearings 数据库 SQL 脚本执行完成！");

                    // 3. 验证数据库表是否创建成功
                    if (dbConn.State != System.Data.ConnectionState.Open)
                    {
                        dbConn.Open();
                    }
                    using (MySqlCommand dbcom = new MySqlCommand("select count(*) from needlebearing", dbConn))
                    {
                        dbcom.ExecuteScalar();
                    }
                    Log("bearings 数据库创建并验证成功！");
                }
            }
            catch (Exception ex2)
            {
                Log("bearings 数据库创建失败！" + ex2.Message);
            }

            // 无论成败，最后启动 .NET 运行时安装包
            try
            {
                Process.Start(Path.Combine(physicalRoot, "windowsdesktop-runtime-8.0.22-win-x64.exe"));
            }
            catch (Exception ex)
            {
                Log("尝试启动.NET运行库安装程序失败：" + ex.Message);
            }
        }

        // ================================
        // 写日志
        // ================================

        protected void Log(string line)
        {
            string physicalRoot = this.Context.Parameters["targetdir"];
            string filePath = physicalRoot + "Install_log.txt";

            string text = DateTime.Now.ToString("[yyyy-MM-dd HH:mm:ss] ") + line;

            File.AppendAllLines(filePath, new string[] { text });
        }
    }

    // ================================
    // 命令执行工具类
    // ================================

    public static class CommandHelper
    {
        public static string Execute(string fileName, string arguments, int timeout)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = fileName,
                Arguments = arguments,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
                StandardOutputEncoding = Encoding.GetEncoding("GBK"),
                StandardErrorEncoding = Encoding.GetEncoding("GBK")
            };

            using (Process process = new Process { StartInfo = startInfo })
            {
                try
                {
                    StringBuilder outputBuilder = new StringBuilder();
                    StringBuilder errorBuilder = new StringBuilder();

                    process.OutputDataReceived += (sender, e) =>
                    {
                        if (!string.IsNullOrEmpty(e.Data))
                            outputBuilder.AppendLine(e.Data);
                    };
                    process.ErrorDataReceived += (sender, e) =>
                    {
                        if (!string.IsNullOrEmpty(e.Data))
                            errorBuilder.AppendLine(e.Data);
                    };

                    process.Start();
                    process.BeginOutputReadLine();
                    process.BeginErrorReadLine();

                    bool isCompleted = process.WaitForExit(timeout);
                    if (!isCompleted)
                    {
                        process.Kill();
                        return $"命令执行超时（超时时间：{timeout} 毫秒）";
                    }

                    string result = outputBuilder.ToString().Trim();
                    string error = errorBuilder.ToString().Trim();
                    if (!string.IsNullOrEmpty(error))
                        result += $"\n错误信息：{error}";

                    return result;
                }
                catch (Exception ex)
                {
                    return $"命令执行失败：{ex.Message}";
                }
            }
        }

        public static string Execute(string command, int timeout)
        {
            return Execute("cmd.exe", $"/c \"{command}\"", timeout);
        }
    }
}
