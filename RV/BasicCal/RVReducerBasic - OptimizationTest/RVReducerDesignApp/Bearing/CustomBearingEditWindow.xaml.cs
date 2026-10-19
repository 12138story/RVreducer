using MySql.Data.MySqlClient;
using RVReducerDesignApp;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace WPFBearings
{
    public partial class CustomBearingEditWindow : Window
    {
        private readonly string _mode;              // "Add" 或 "Revise"
        private readonly string _tableName;          // 数据库表名
        private readonly string _connectionString;
        private readonly DataRowView _sourceRow;     // Revise 模式下的数据行

        private readonly CTANDict _dataDict = new CTANDict();
        private CustomBearingPage _customPage;
        private CTANComboBox _typeComboBox;

        // 轴承类型名称 → ID 映射
        private static readonly Dictionary<string, int> TypeNameToId = new Dictionary<string, int>
        {
            { "深沟球轴承", CTANBearing.DeepGrooveBallBearing },
            { "滚针轴承", CTANBearing.NeedleBearing },
            { "圆锥滚子轴承", CTANBearing.TaperedRollerBearing },
            { "角接触球轴承", CTANBearing.AngularContactBallBearing },
        };

        // 轴承类型 ID → 名称 映射
        private static readonly Dictionary<int, string> TypeIdToName = TypeNameToId.ToDictionary(kv => kv.Value, kv => kv.Key);

        // ============== 维度键名映射（根据轴承类型） ==============
        private static string GetInnerDiaKey(int typeId) =>
            typeId == CTANBearing.NeedleBearing ? "轴承内径Fw" : "轴承内径d";

        private static string GetOuterDiaKey(int typeId) =>
            typeId == CTANBearing.NeedleBearing ? "轴承外径Ew" : "轴承外径D";

        private static string GetWidthKey(int typeId)
        {
            if (typeId == CTANBearing.NeedleBearing) return "轴承宽度Bc";
            if (typeId == CTANBearing.TaperedRollerBearing) return "轴承宽度T";
            return "轴承宽度B";
        }

        // ============== 构造函数 ==============
        public CustomBearingEditWindow(string mode, string tableName, string connectionString, DataRowView sourceRow = null)
        {
            InitializeComponent();

            _mode = mode;
            _tableName = tableName;
            _connectionString = connectionString;
            _sourceRow = sourceRow;

            this.Title = mode == "Add" ? "添加自定义轴承" : "修改自定义轴承";

            try
            {
                InitializeDict();
                CreateCustomBearingPage();
                LoadDataIfRevise();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"初始化失败: {ex.Message}");
                this.DialogResult = false;
                this.Close();
            }
        }

        /// <summary>
        /// 初始化 CTANDict，确保所有必要的键存在
        /// </summary>
        private void InitializeDict()
        {
            // 必需的键
            string[] requiredKeys =
            {
                "轴承", "列数",
                "轴承内径d", "轴承外径D", "轴承宽度B",
                "轴承内径Fw", "轴承外径Ew", "轴承宽度Bc",
                "轴承宽度T", "轴承宽度C",
                "基本额定动载荷Cr", "基本额定静载荷C0r", "bm系数",
                "计算系数e", "计算系数Y", "计算系数Y0",
                "接触角", "滚子数",
                "滚子节圆直径", "滚子直径", "滚子总长度", "滚子倒角半径",
                "外圈类型", "内圈类型", "估算内部细节",
                // 新增列
                "轴承代号", "布置方式", "装配数量", "Cu",
                "大端倒角", "小端倒角", "滚子锥度角",
                "润滑油粘度v", "污染等级", "膜厚比lambda", "极压添加剂", "可靠度",
                // CustomBearingPage 新UI键名
                "节圆直径Dpw", "滚动体直径Dw", "滚子总长度L", "滚子倒角r", "滚子数Z", "滚子子类型",
                "大端倒角rl", "小端倒角rs", "疲劳载荷极限Cu",
                // 工况参数
                "转速", "径向力", "轴向力"
            };

            foreach (var key in requiredKeys)
            {
                if (!_dataDict.ContainsKey(key))
                    _dataDict.Add(key, new CTANDouble { Value = 0 });
            }

            // 默认轴承类型为深沟球
            _dataDict["轴承"].Value = CTANBearing.DeepGrooveBallBearing;
        }

        /// <summary>
        /// 创建 CustomBearingPage 并将其 Grid 嵌入窗口
        /// </summary>
        private void CreateCustomBearingPage()
        {
            _customPage = new CustomBearingPage(_dataDict);

            // 获取页面中的 ComboBox 引用
            FindTypeComboBox();

            // 将页面的 mInputPageGrid 添加到 ContentBorder
            ContentBorder.Child = _customPage.mInputPageGrid;
        }

        /// <summary>
        /// 从 CustomBearingPage 中提取轴承类型 ComboBox 的引用
        /// </summary>
        private void FindTypeComboBox()
        {
            // 递归查找子元素中的 CTANComboBox
            _typeComboBox = FindChildControl<CTANComboBox>(_customPage.mInputPageGrid);
        }

        private T FindChildControl<T>(DependencyObject parent) where T : class
        {
            int count = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < count; i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                if (child is T typed)
                    return typed;

                var result = FindChildControl<T>(child);
                if (result != null)
                    return result;
            }
            return null;
        }

        /// <summary>
        /// Revise 模式：从数据库行填充数据到 Dict
        /// </summary>
        private void LoadDataIfRevise()
        {
            if (_mode != "Revise" || _sourceRow == null) return;

            // 1. 读取类型名称 → 转换 ID
            string typeName = _sourceRow["类型"]?.ToString() ?? "深沟球轴承";
            if (TypeNameToId.TryGetValue(typeName, out int typeId))
            {
                _dataDict["轴承"].Value = typeId;
            }

            // 2. 填充各字段
            SetDictFromDb("列数", "列数");
            SetDictFromDb("轴承内径d", "内径");    // 默认使用深沟球键
            SetDictFromDb("轴承外径D", "外径");
            SetDictFromDb("轴承宽度B", "宽度");
            SetDictFromDb("接触角", "接触角");
            SetDictFromDb("基本额定动载荷Cr", "Cr");
            SetDictFromDb("基本额定静载荷C0r", "C0r");
            SetDictFromDb("bm系数", "bm系数");
            SetDictFromDb("滚子数Z", "滚子数Z");
            SetDictFromDb("节圆直径Dpw", "节圆直径Dpw");
            SetDictFromDb("滚动体直径Dw", "滚动体直径Dw");
            SetDictFromDb("滚子总长度L", "滚子总长度L");
            SetDictFromDb("滚子倒角r", "滚子倒角r");
            SetDictFromDb("滚子子类型", "滚子子类型");

            // 3. 读取新加的列
            SetDictFromDb("外圈类型", "外圈类型");
            SetDictFromDb("内圈类型", "内圈类型");
            SetDictFromDb("估算内部细节", "估算内部细节");
            SetDictFromDb("计算系数e", "计算系数e");
            SetDictFromDb("计算系数Y", "计算系数Y");
            SetDictFromDb("计算系数Y0", "计算系数Y0");

            // 读取新增列
            SetDictFromDb("轴承代号", "轴承代号");
            SetDictFromDb("滚子锥度角", "滚子锥度角");
            SetDictFromDb("大端倒角rl", "大端倒角rl");
            SetDictFromDb("小端倒角rs", "小端倒角rs");
            SetDictFromDb("布置方式", "布置方式");
            SetDictFromDb("装配数量", "装配数量");
            SetDictFromDb("疲劳载荷极限Cu", "疲劳载荷极限Cu");
            // 修正寿命列
            SetDictFromDb("润滑油粘度v", "润滑油粘度v");
            SetDictFromDb("污染等级", "污染等级");
            SetDictFromDb("膜厚比lambda", "膜厚比lambda");
            SetDictFromDb("极压添加剂", "极压添加剂");
            SetDictFromDb("可靠度", "可靠度");
            // 工况参数
            SetDictFromDb("转速", "转速");
            SetDictFromDb("径向力", "径向力");
            SetDictFromDb("轴向力", "轴向力");

            // 4. 根据类型同步到类型特定的维度键
            SyncDimensionKeysForType(typeId);

            // 5. 依据库值刷新 Cr/C0r 可选输入勾选态：
            //    值>0 → 勾选（手动输入）；值=0 → 不勾选（自动计算，DLL 端据此解算）
            _customPage.RefreshRatedLoadManualStatesFromDict();
        }

        /// <summary>
        /// 从 DB 行读取值写入 Dict
        /// </summary>
        private void SetDictFromDb(string dictKey, string dbColumn)
        {
            if (_sourceRow.Row.Table.Columns.Contains(dbColumn) && _dataDict.ContainsKey(dictKey))
            {
                string val = _sourceRow[dbColumn]?.ToString();
                if (double.TryParse(val, out double d))
                    _dataDict[dictKey].Value = d;
            }
        }

        /// <summary>
        /// 根据轴承类型，将通用维度值同步到类型特定的键
        /// </summary>
        private void SyncDimensionKeysForType(int typeId)
        {
            // 内径
            string innerKey = GetInnerDiaKey(typeId);
            if (_dataDict.ContainsKey(innerKey) && _dataDict.ContainsKey("轴承内径d"))
                _dataDict[innerKey].Value = _dataDict["轴承内径d"].Value;

            // 外径
            string outerKey = GetOuterDiaKey(typeId);
            if (_dataDict.ContainsKey(outerKey) && _dataDict.ContainsKey("轴承外径D"))
                _dataDict[outerKey].Value = _dataDict["轴承外径D"].Value;

            // 宽度
            string widthKey = GetWidthKey(typeId);
            if (_dataDict.ContainsKey(widthKey) && _dataDict.ContainsKey("轴承宽度B"))
                _dataDict[widthKey].Value = _dataDict["轴承宽度B"].Value;
        }

        // ==================== 保存到数据库 ====================

        private void BtnOK_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // 收集数据
                var values = CollectDbValues();

                using (var conn = new MySqlConnection(_connectionString))
                {
                    conn.Open();

                    if (_mode == "Add")
                    {
                        InsertRecord(conn, values);
                    }
                    else // Revise
                    {
                        UpdateRecord(conn, values);
                    }
                }

                this.DialogResult = true;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"保存失败: {ex.Message}");
            }
        }

        /// <summary>
        /// 从 Dict 收集所有数据库列的值
        /// </summary>
        private Dictionary<string, object> CollectDbValues()
        {
            int typeId = (int)_dataDict["轴承"].Value;
            string typeName = TypeIdToName.TryGetValue(typeId, out var name) ? name : "深沟球轴承";

            var values = new Dictionary<string, object>
            {
                ["类型"] = typeName,
                ["列数"] = _dataDict["列数"].Value,
                ["内径"] = GetDictValueSafe(GetInnerDiaKey(typeId)),
                ["外径"] = GetDictValueSafe(GetOuterDiaKey(typeId)),
                ["宽度"] = GetDictValueSafe(GetWidthKey(typeId)),
                // Cr/C0r 可选输入：未勾选(值=0)为“自动计算”标记，原样入库；勾选时值为手动输入
                ["Cr"] = _dataDict["基本额定动载荷Cr"].Value,
                ["C0r"] = _dataDict["基本额定静载荷C0r"].Value,
                ["接触角"] = _dataDict["接触角"].Value,
                ["bm系数"] = _dataDict["bm系数"].Value,
                ["滚子数Z"] = _dataDict["滚子数Z"].Value,
                ["节圆直径Dpw"] = _dataDict["节圆直径Dpw"].Value,
                ["滚动体直径Dw"] = _dataDict["滚动体直径Dw"].Value,
                ["滚子总长度L"] = _dataDict["滚子总长度L"].Value,
                ["滚子倒角r"] = _dataDict["滚子倒角r"].Value,
                ["滚子子类型"] = _dataDict["滚子子类型"].Value,
                // 结构列
                ["外圈类型"] = _dataDict["外圈类型"].Value,
                ["内圈类型"] = _dataDict["内圈类型"].Value,
                // 内部列
                ["估算内部细节"] = _dataDict["估算内部细节"].Value,
                // 圆锥滚子专用列
                ["计算系数e"] = _dataDict["计算系数e"].Value,
                ["计算系数Y"] = _dataDict["计算系数Y"].Value,
                ["计算系数Y0"] = _dataDict["计算系数Y0"].Value,
                // 新列
                ["轴承代号"] = _dataDict["轴承代号"].Value,
                ["滚子锥度角"] = _dataDict["滚子锥度角"].Value,
                ["大端倒角rl"] = _dataDict["大端倒角rl"].Value,
                ["小端倒角rs"] = _dataDict["小端倒角rs"].Value,
                ["布置方式"] = _dataDict["布置方式"].Value,
                ["装配数量"] = _dataDict["装配数量"].Value,
                ["疲劳载荷极限Cu"] = _dataDict["疲劳载荷极限Cu"].Value,
                // 修正寿命列
                ["润滑油粘度v"] = _dataDict["润滑油粘度v"].Value,
                ["污染等级"] = _dataDict["污染等级"].Value,
                ["膜厚比lambda"] = _dataDict["膜厚比lambda"].Value,
                ["极压添加剂"] = _dataDict["极压添加剂"].Value,
                ["可靠度"] = _dataDict["可靠度"].Value,
                // 工况参数
                ["转速"] = _dataDict["转速"].Value,
                ["径向力"] = _dataDict["径向力"].Value,
                ["轴向力"] = _dataDict["轴向力"].Value,
            };

            return values;
        }

        private double GetDictValueSafe(string key)
        {
            return _dataDict.ContainsKey(key) ? _dataDict[key].Value : 0;
        }

        private void InsertRecord(MySqlConnection conn, Dictionary<string, object> values)
        {
            string columns = string.Join(", ", values.Keys.Select(c => $"`{c}`"));
            string paramsList = string.Join(", ", values.Keys.Select((c, i) => $"@p{i}"));

            string sql = $"INSERT INTO `{_tableName}` ({columns}) VALUES ({paramsList})";

            using (var cmd = new MySqlCommand(sql, conn))
            {
                int i = 0;
                foreach (var kv in values)
                {
                    cmd.Parameters.AddWithValue($"@p{i}", kv.Value ?? DBNull.Value);
                    i++;
                }
                cmd.ExecuteNonQuery();
            }
        }

        private void UpdateRecord(MySqlConnection conn, Dictionary<string, object> values)
        {
            // 构建 SET 子句
            var setClauses = values.Keys.Select((col, i) => $"`{col}` = @p{i}").ToList();

            // 构建 WHERE 子句（使用原始行数据定位）
            var whereConditions = new List<string>();
            var whereParams = new List<MySqlParameter>();
            int paramIndex = values.Count;

            for (int i = 0; i < _sourceRow.Row.Table.Columns.Count; i++)
            {
                string colName = _sourceRow.Row.Table.Columns[i].ColumnName;
                object colValue = _sourceRow.Row[i];

                if (colValue == DBNull.Value || colValue == null)
                {
                    whereConditions.Add($"`{colName}` IS NULL");
                }
                else if (colValue is string strVal && string.IsNullOrEmpty(strVal))
                {
                    whereConditions.Add($"`{colName}` = ''");
                }
                else
                {
                    whereConditions.Add($"`{colName}` = @w{paramIndex}");
                    whereParams.Add(new MySqlParameter($"@w{paramIndex}", colValue.ToString()));
                    paramIndex++;
                }
            }

            string sql = $"UPDATE `{_tableName}` SET {string.Join(", ", setClauses)} WHERE {string.Join(" AND ", whereConditions)}";

            using (var cmd = new MySqlCommand(sql, conn))
            {
                int i = 0;
                foreach (var kv in values)
                {
                    cmd.Parameters.AddWithValue($"@p{i}", kv.Value ?? DBNull.Value);
                    i++;
                }
                cmd.Parameters.AddRange(whereParams.ToArray());
                cmd.ExecuteNonQuery();
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}
