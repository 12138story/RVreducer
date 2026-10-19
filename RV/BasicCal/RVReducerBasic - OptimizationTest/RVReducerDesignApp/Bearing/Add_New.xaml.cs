using HandyControl.Tools.Extension;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace WPFBearings
{
    public partial class Add_New : Window
    {
        private const string ConnectionString = BearingDataWindow.ConnectionString;

        private List<string> _queryData;       // 主窗口传递的选中行【全列数据】
        private List<string> _loadedData;     // 数据库加载的匹配行数据
        private string _firstColumnName;      // 表第一列列名
        private bool _isDataLoaded = false;   // 数据加载标记

        private List<ColumnInfo> _allColumnInfos = new List<ColumnInfo>();
        private int _currentPage = 1;
        public string Abb = BearingDataWindow.Bname;

        private class ColumnInfo
        {
            public string Name { get; set; }
            public string DataType { get; set; }
            public bool IsNullable { get; set; }
        }
        //核心代码
        public Add_New()
        {
            InitializeComponent();
            this.Title = $"当前表：{BearingDataWindow.Bname}";
            this.Closing += Add_New_Closing;

            lock (BearingDataWindow.DataTrans)
            {
                _queryData = new List<string>(BearingDataWindow.DataTrans);
                BearingDataWindow.DataTrans.Clear();
            }

            _loadedData = new List<string>();
            _firstColumnName = null;
            _isDataLoaded = false;

            if (BearingDataWindow.State != null)
            {
                switch (BearingDataWindow.State)
                {
                    case "Add":
                        GetColumnTypes();
                        ShowPage1();
                        PopulateDataToUI();
                        break;

                    case "Revise":
                        GetColumnTypes();
                        ShowPage1();
                        // 加载待修改数据（允许空值，全列组合条件定位）
                        if (!LoadDataFromDatabase())
                        {
                            MessageBox.Show("加载待修改数据失败");
                            this.Close();
                        }
                        else
                        {
                            PopulateDataToUI();
                        }
                        break;

                    default:
                        MessageBox.Show($"不支持的操作状态: {BearingDataWindow.State}");
                        this.Close();
                        break;
                }
            }
            else
            {
                MessageBox.Show("操作状态为空");
                this.Close();
            }
        }

        /// <summary>
        /// 全列组合条件定位数据
        /// 即使第一列空，也能通过多列条件定位，避免误改
        /// </summary>
        private bool LoadDataFromDatabase()
        {
            if (_allColumnInfos.Count == 0 || _queryData == null || _queryData.Count != _allColumnInfos.Count)
            {
                MessageBox.Show("列信息与传递的数据不匹配，无法定位");
                return false;
            }

            try
            {
                using (var conn = new MySqlConnection(ConnectionString))
                {
                    conn.Open();
                    var whereConditions = new List<string>();
                    var cmdParams = new List<MySqlParameter>();

                    for (int i = 0; i < _allColumnInfos.Count; i++)
                    {
                        string colName = _allColumnInfos[i].Name;
                        string colValue = _queryData[i]?.Trim();
                        string paramName = $"@param{i}";

                        // 核心修复：区分空字符串和NULL
                        if (colValue == null)
                        {
                            // 传递的是NULL → 匹配数据库的NULL
                            whereConditions.Add($"`{colName}` IS NULL");
                        }
                        else if (colValue == string.Empty)
                        {
                            // 传递的是空字符串 → 匹配数据库的''
                            whereConditions.Add($"`{colName}` = ''");
                        }
                        else
                        {
                            // 其他值 → 参数化匹配
                            whereConditions.Add($"`{colName}` = {paramName}");
                            cmdParams.Add(new MySqlParameter(paramName, colValue));
                        }
                    }

                    string selectQuery = $"SELECT * FROM `{Abb}` WHERE {string.Join(" AND ", whereConditions)}";
                    using (var cmd = new MySqlCommand(selectQuery, conn))
                    {
                        cmd.Parameters.AddRange(cmdParams.ToArray());

                        using (var reader = cmd.ExecuteReader())
                        {
                            int matchCount = 0;
                            while (reader.Read())
                            {
                                matchCount++;
                                if (matchCount > 1)
                                {
                                    MessageBox.Show($"找到{matchCount}条匹配数据，无法精准修改（请确保选中行数据唯一）");
                                    return false;
                                }

                                _loadedData.Clear();
                                for (int i = 0; i < reader.FieldCount; i++)
                                {
                                    _loadedData.Add(reader.IsDBNull(i) ? "" : reader[i].ToString());
                                }
                                _firstColumnName = _allColumnInfos[0].Name;
                            }

                            if (matchCount == 0)
                            {
                                MessageBox.Show("未找到与选中行完全匹配的数据（可能是NULL/空字符串的差异）");
                                return false;
                            }

                            _isDataLoaded = true;
                            return true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"加载数据出错: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// 确认修改按钮
        /// </summary>
        private void YES_Click(object sender, RoutedEventArgs e)
        {
            bool isOperationSuccess = false;
            MySqlConnection conn = null;

            try
            {
                conn = new MySqlConnection(ConnectionString);
                conn.Open();

                var validColumns = new List<string>();
                var validValues = new List<object>();

                for (int i = 0; i < 36; i++)
                {
                    var colTB = i < 18 ? FindName($"X{i + 1}") as TextBox : FindName($"XX{i - 17}") as TextBox;
                    var valTB = i < 18 ? FindName($"T{i + 1}") as TextBox : FindName($"TT{i - 17}") as TextBox;

                    if (colTB != null && valTB != null && !string.IsNullOrWhiteSpace(colTB.Text) && colTB.Text != "0")
                    {
                        string colName = colTB.Text;
                        var columnInfo = _allColumnInfos.FirstOrDefault(c => c.Name == colName);
                        if (columnInfo == null) continue;

                        string inputText = valTB.Text.Trim();
                        object dbValue;

                        if (string.IsNullOrEmpty(inputText))
                        {
                            dbValue = columnInfo.IsNullable ? DBNull.Value :
                                      (columnInfo.DataType.Contains("int") ? (object)0 : "");
                        }
                        else
                        {
                            if (columnInfo.DataType.Contains("int"))
                            {
                                if (!int.TryParse(inputText, out int intVal))
                                {
                                    MessageBox.Show($"列「{colName}」请输入整数！");
                                    return;
                                }
                                dbValue = intVal;
                            }
                            else if (columnInfo.DataType.Contains("decimal"))
                            {
                                if (!decimal.TryParse(inputText, out decimal decVal))
                                {
                                    MessageBox.Show($"列「{colName}」请输入数值！");
                                    return;
                                }
                                dbValue = decVal;
                            }
                            else
                            {
                                dbValue = inputText;
                            }
                        }

                        validColumns.Add(colName);
                        validValues.Add(dbValue);
                    }
                }

                if (validColumns.Count == 0)
                {
                    MessageBox.Show("无有效列名");
                    return;
                }

                if (BearingDataWindow.State == "Add")
                {
                    string sql = $"INSERT INTO `{Abb}` ({string.Join(", ", validColumns.Select(c => $"`{c}`"))}) VALUES ({string.Join(", ", validColumns.Select((c, i) => $"@param{i}"))})";
                    using (var cmd = new MySqlCommand(sql, conn))
                    {
                        for (int i = 0; i < validValues.Count; i++)
                            cmd.Parameters.AddWithValue($"@param{i}", validValues[i]);
                        cmd.ExecuteNonQuery();
                    }
                    MessageBox.Show("1行已插入");
                    isOperationSuccess = true;
                }
                else if (BearingDataWindow.State == "Revise" && _isDataLoaded)
                {
                    var setClauses = validColumns.Select((col, i) => $"`{col}` = @param{i}").ToList();
                    var whereConditions = new List<string>();
                    var cmdParams = new List<MySqlParameter>();

                   
                    for (int i = 0; i < validValues.Count; i++)
                    {
                        cmdParams.Add(new MySqlParameter($"@param{i}", validValues[i]));
                    }

                    int paramIndex = validValues.Count;
                    for (int i = 0; i < _allColumnInfos.Count; i++)
                    {
                        string colName = _allColumnInfos[i].Name;
                        string colValue = _queryData[i]?.Trim();

                        if (string.IsNullOrWhiteSpace(colValue))
                        {
                            whereConditions.Add($"`{colName}` IS NULL");
                        }
                        else
                        {
                            whereConditions.Add($"`{colName}` = @whereParam{paramIndex}");
                            cmdParams.Add(new MySqlParameter($"@whereParam{paramIndex}", colValue));
                            paramIndex++;
                        }
                    }

                    string sql = $"UPDATE `{Abb}` SET {string.Join(", ", setClauses)} WHERE {string.Join(" AND ", whereConditions)}";
                    using (var cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddRange(cmdParams.ToArray());
                        int rowsAffected = cmd.ExecuteNonQuery();
                        MessageBox.Show($"{rowsAffected}行已更新");
                        isOperationSuccess = true;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"操作错误: {ex.Message}");
                isOperationSuccess = false;
            }
            finally
            {
                if (conn != null && conn.State == ConnectionState.Open)
                    conn.Close();

                if (this.IsLoaded)
                    this.DialogResult = isOperationSuccess;
                this.Close();
            }
        }

        private void Add_New_Closing(object sender, CancelEventArgs e)
        {
            if (!this.IsLoaded || this.DialogResult.HasValue) return;

            if (BearingDataWindow.State == "Revise" && _isDataLoaded)
            {
                var result = MessageBox.Show("放弃更改并关闭？", "确认", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.No)
                {
                    e.Cancel = true;
                }
                else
                {
                    this.DialogResult = false;
                }
            }
            else
            {
                this.DialogResult = false;
            }
        }

        private void No_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        private void PopulateDataToUI()
        {
            if (BearingDataWindow.State == "Add")
            {
                for (int i = 0; i < 36; i++)
                {
                    TextBox inputTB = i < 18 ? FindName($"T{i + 1}") as TextBox : FindName($"TT{i - 17}") as TextBox;
                    if (inputTB != null) inputTB.Text = "";
                }
                return;
            }

            if (_loadedData == null) return;
            for (int i = 0; i < Math.Min(_loadedData.Count, 36); i++)
            {
                TextBox inputTB = i < 18 ? FindName($"T{i + 1}") as TextBox : FindName($"TT{i - 17}") as TextBox;
                if (inputTB != null) inputTB.Text = _loadedData[i] ?? "";
            }
        }

        private void GetColumnTypes()
        {
            _allColumnInfos.Clear();
            try
            {
                using (var conn = new MySqlConnection(ConnectionString))
                {
                    conn.Open();
                    string query = @"SELECT COLUMN_NAME, DATA_TYPE, IS_NULLABLE 
                                    FROM INFORMATION_SCHEMA.COLUMNS 
                                    WHERE TABLE_SCHEMA = @dbName AND TABLE_NAME = @tableName 
                                    ORDER BY ORDINAL_POSITION;";
                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@dbName", new MySqlConnectionStringBuilder(ConnectionString).Database);
                        cmd.Parameters.AddWithValue("@tableName", Abb);
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                string colName = reader["COLUMN_NAME"].ToString();
                                if (!string.IsNullOrEmpty(colName) && colName != "0")
                                {
                                    _allColumnInfos.Add(new ColumnInfo
                                    {
                                        Name = colName,
                                        DataType = reader["DATA_TYPE"].ToString(),
                                        IsNullable = reader["IS_NULLABLE"].ToString().Equals("YES", StringComparison.OrdinalIgnoreCase)
                                    });
                                }
                            }
                        }
                    }
                }

                for (int i = 0; i < Math.Min(_allColumnInfos.Count, 36); i++)
                {
                    bool isFirstPage = i < 18;
                    string labelName = isFirstPage ? $"X{i + 1}" : $"XX{i - 17}";
                    var labelTB = FindName(labelName) as TextBox;
                    if (labelTB != null)
                    {
                        labelTB.Text = _allColumnInfos[i].Name;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"获取列名错误: {ex.Message}");
            }
        }
//第一页
        public void ShowPage1()
        {
            _currentPage = 1;

            for (int i = 0; i < 18; i++)
            {
                var labelTB = FindName($"X{i + 1}") as TextBox;
                var inputTB = FindName($"T{i + 1}") as TextBox;
                bool isVisible = i < _allColumnInfos.Count && !string.IsNullOrEmpty(_allColumnInfos[i].Name);
                labelTB.Visibility = isVisible ? Visibility.Visible : Visibility.Collapsed;
                inputTB.Visibility = isVisible ? Visibility.Visible : Visibility.Collapsed;
            }

            for (int i = 1; i <= 18; i++)
            {
                var labelTB = FindName($"XX{i}") as TextBox;
                var inputTB = FindName($"TT{i}") as TextBox;
                labelTB.Visibility = Visibility.Collapsed;
                inputTB.Visibility = Visibility.Collapsed;
            }

            NEXT.Visibility = _allColumnInfos.Count > 18 ? Visibility.Visible : Visibility.Collapsed;
            BEFORE.Visibility = Visibility.Collapsed;
        }
//第二页
        public void ShowPage2()
        {
            _currentPage = 2;

            for (int i = 1; i <= 18; i++)
            {
                var labelTB = FindName($"X{i}") as TextBox;
                var inputTB = FindName($"T{i}") as TextBox;
                labelTB.Visibility = Visibility.Collapsed;
                inputTB.Visibility = Visibility.Collapsed;
            }

            for (int i = 0; i < 18; i++)
            {
                int colIndex = 18 + i;
                var labelTB = FindName($"XX{i + 1}") as TextBox;
                var inputTB = FindName($"TT{i + 1}") as TextBox;
                bool isVisible = colIndex < _allColumnInfos.Count && !string.IsNullOrEmpty(_allColumnInfos[colIndex].Name);
                labelTB.Visibility = isVisible ? Visibility.Visible : Visibility.Collapsed;
                inputTB.Visibility = isVisible ? Visibility.Visible : Visibility.Collapsed;
            }

            NEXT.Visibility = Visibility.Collapsed;
            BEFORE.Visibility = Visibility.Visible;
        }
//下一页
        private void NEXT_Click(object sender, RoutedEventArgs e)
        {
            if (_allColumnInfos.Count > 18)
            {
                ShowPage2();
            }
            else
            {
                MessageBox.Show("没有第二页");
            }
        }
//前一页
        private void BEFORE_Click(object sender, RoutedEventArgs e)
        {
            ShowPage1();
        }
       
    }
}