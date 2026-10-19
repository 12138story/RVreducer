using MySql.Data.MySqlClient;
using NPOI.SS.UserModel;
using NPOI.XSSF.Streaming;
using RVReducerDesignApp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using MessageBox = System.Windows.Forms.MessageBox;
using TextBox = System.Windows.Controls.TextBox;
using SaveFileDialog = Microsoft.Win32.SaveFileDialog;

namespace WPFBearings
{   
    public partial class BearingDataWindow : Window
    {
        private DatabaseHelper dbHelper;
        private string currentTableName;

        public bool IsReturnData = true;
        public CTANDict OutAssignDictPtr;

        private DataRowView selectedRow;
        public int mSelectBearingType;
        public bool IsBearingSelected = false;
        public double[] mSelectBearingData = new double[50];

        public static string Bname;
        public static string State;
        public static List<string> DataTrans = new List<string>();

        public const string ConnectionString = "Database=bearings;Server=localhost;Port=3729;Password='jay';UserID=root;charset=utf8";

        // 当前显示的分组索引
        private int _currentGroupIndex = 0;
        // 定义多组数据表名称
        private List<List<string>> _tableGroups = new List<List<string>>
        {
            // 第1组：国标轴承
            new List<string>
            {
                "调心球轴承",
                "滚针轴承",
                "圆锥滚子轴承单列",
                "圆锥滚子轴承双列",
                "圆锥滚子轴承四列",
                "圆柱滚子轴承单列",
                "圆柱滚子轴承双列",
                "圆柱滚子轴承无外圈",
                "圆柱滚子轴承无内圈",
                "圆柱滚子轴承轧机用四列",
                "fag"
            },
            // 第2组：其他类轴承（根据你的实际数据表修改）
            new List<string>
            {
                "滚针轴承类型",
                "角接触球轴承类型",
                "圆锥滚子轴承类型",
            },
            //第三组：自定义轴承
            new List<string>
            {
                "自定义轴承",
            },
        };
        // 在构造函数或 Window_Loaded 中调用初始化
        private void InitializeTableGroups()
        {
            LoadGroup(_currentGroupIndex);
        }
        // 加载指定分组的数据表到 ListBox
        private void LoadGroup(int groupIndex)
        {
            Wlistbox.Items.Clear();
            foreach (var tableName in _tableGroups[groupIndex])
            {
                Wlistbox.Items.Add(new ListBoxItem { Content = tableName });
            }
        }
        // 单选按钮的选中事件 (替换了原来的 SwitchGroup_Click)
        private void RbGroup_Checked(object sender, RoutedEventArgs e)
        {
            // 防止在窗体初始化阶段事件意外触发
            if (!this.IsLoaded) return;

            if (sender == RbGroup1)
            {
                _currentGroupIndex = 0;
            }
            else if (sender == RbGroup2)
            {
                _currentGroupIndex = 1;
            }
            else if (sender == RbGroup3)
            {
                _currentGroupIndex = 2;
            }
            LoadGroup(_currentGroupIndex);

            // 清空 DataGrid（因为切换了分组，之前选中的表不再显示）
            if (Wdatalist != null)
            {
                Wdatalist.ItemsSource = null;
            }
        }
        public BearingDataWindow()
        {
            InitializeComponent();
            InitializeTableGroups();
            //string connectionString = "Database=Bearings;Server=localhost;Port=3306;Password='jay';UserID=root;charset=utf8";
            dbHelper = new DatabaseHelper(ConnectionString);//连接数据库

            //EventManager.RegisterClassHandler(typeof(TextBox), TextBox.TextChangedEvent, new RoutedEventHandler(this.UpdateTable));//监听所有TextBox的内容发生改变的事件（潜在问题并非是Wselectgrid控件内的，而是该窗口下的所有TextBox）
            // 不再使用 RegisterClassHandler（会影响整个应用的所有 TextBox，并且难以取消）
            // 改为仅订阅本窗口相关的 TextBox 的 TextChanged 事件
            // 使用 lambda 转发以匹配 UpdateTable(object, RoutedEventArgs)
            Wmininnerdiameter.TextChanged += (s, e) => UpdateTable(s, e);
            Wmaxinnerdiameter.TextChanged += (s, e) => UpdateTable(s, e);
            Wminoutsidediameter.TextChanged += (s, e) => UpdateTable(s, e);
            Wmaxoutsidediameter.TextChanged += (s, e) => UpdateTable(s, e);
            Wmindynamicloads.TextChanged += (s, e) => UpdateTable(s, e);
            Wmaxdynamicloads.TextChanged += (s, e) => UpdateTable(s, e);
            //Pdrawbeams.Navigate(new GirderPage()); //默认加载GiderPage，即梁绘制界面
        }
        private Dictionary<string, string> tableMapping = new Dictionary<string, string>
        {
            {"调心球轴承", "selfAligningBallBearings"},
            {"滚针轴承", "needleBearing"},        
            {"圆锥滚子轴承单列", "taperedRollerBearingsInSingleRows"},
            {"圆锥滚子轴承双列", "taperedRollerBearingsInDoubleRows"},
            {"圆锥滚子轴承四列", "taperedRollerBearingsInFourRows"},
            {"圆柱滚子轴承单列", "cylindricalRollerBearingsInSingleRows"},
            {"圆柱滚子轴承双列", "cylindricalRollerBearingsInDoubleRows"},
            {"圆柱滚子轴承无外圈", "cylindricalRollerBearingsWithoutOuterRings"},
            {"圆柱滚子轴承无内圈", "cylindricalRollerBearingsWithoutInnerRings"},
            {"圆柱滚子轴承轧机用四列", "cylindricalRollerBearingsRollingMillWithFourRows"},
            {"fag", "_fag2012"},

            {"滚针轴承类型", "needlerollerbearingtype"},
            {"角接触球轴承类型", "angularcontactballbearingtype"},
            {"圆锥滚子轴承类型", "taperrollerbearingtype"},

            {"自定义轴承", "custombearing"},
        };

        private void LoadData(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(currentTableName))
                {
                    // 没有选择表名，直接返回
                    return;
                }

                string query = $"SELECT * FROM `{currentTableName}`";
                DataTable dataTable = dbHelper.GetDataTable(query);
                Wdatalist.ItemsSource = dataTable.DefaultView;
            }
            catch (Exception ex) { MessageBox.Show("Error: " + ex.Message); }
        }
        private void SelectTable(object sender, SelectionChangedEventArgs e)
        {
            ListBoxItem selectedItem = (ListBoxItem)Wlistbox.SelectedItem;
            if (selectedItem != null)
            {
                string selectedText = selectedItem.Content.ToString();// 获取选中的中文名称
                if (tableMapping.ContainsKey(selectedText))// 检查字典中是否有对应的键
                {
                    currentTableName = tableMapping[selectedText]; // 获取对应的英文表名
                    Bname = currentTableName;
                    if (Woutsidediameter.IsChecked == true || Winnerdiameter.IsChecked == true || Wdynamicloads.IsChecked == true)
                    {
                        UpdateTable(sender, e); //根据checkbox来刷新表格
                        
                        Array.Clear(mSelectBearingData);
                        IsBearingSelected = false;
                    }
                    else
                    {
                        LoadData(sender, e); //仅仅根据表名刷新表格
                    }
                }
                else
                {
                    MessageBox.Show("Error:未找到匹配的表名！");
                }
            }
            //else { MessageBox.Show("Error: SelectTable方法需要修正！"); }
        }

        /// <summary>
        /// 通过输入参数范围更新表格
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void UpdateTable(object sender, RoutedEventArgs e)
        {
            // 先做保护检查：窗口未加载、没有选择表名或 dbHelper 为 null 时直接返回
            if (!this.IsLoaded || string.IsNullOrEmpty(currentTableName) || dbHelper == null)
            {
                return;
            }
            string query = $"SELECT * FROM `{currentTableName}` WHERE 1=1";
            query += BuildFilterClause();
            try
            {
                DataTable dataTable = dbHelper.GetDataTable(query);
                Wdatalist.ItemsSource = dataTable.DefaultView;
            }
            catch (Exception ex)
            {
                // 捕获并显示发生在整个方法中的异常，方便排查
                MessageBox.Show("Error:sjk代码存在问题！" + ex.Message);
            }
        }
        // 确认按钮点击事件
        private void Wdrawbeams_Click(object sender, RoutedEventArgs e)
        {
            selectedRow = Wdatalist.SelectedItem as DataRowView;
            if (selectedRow != null)
            {
                //SelectedD = selectedRow["d"].ToString();
                //SelectedDMax = selectedRow["_D"].ToString();
                //SelectedB = selectedRow["B"].ToString();
                if (OutAssignDictPtr != null)
                {
                    // 数据库 bearings 中 Cr/C0r 单位即为千牛(kN)，与字典/UI 单位一致，直接赋值（不要再 ×1000）
                    OutAssignDictPtr["基本额定动载荷Cr"].Value = double.Parse(selectedRow["Cr"].ToString());
                    OutAssignDictPtr["基本额定静载荷C0r"].Value = double.Parse(selectedRow["C0r"].ToString());
                    //if (currentTableName == "deepgrooveball"|| currentTableName == "needlerollerbearingtype") //等待数据库完善后再修改
                    if (currentTableName == "deepgrooveball")
                    {
                        OutAssignDictPtr["轴承宽度B"].Value = double.Parse(selectedRow["B"].ToString());
                        OutAssignDictPtr["轴承内径d"].Value = double.Parse(selectedRow["d"].ToString());
                        OutAssignDictPtr["轴承外径D"].Value = double.Parse(selectedRow["_D"].ToString());
                        OutAssignDictPtr["轴承类型"].Value = RVReducerDesignApp.CTANBearing.DeepGrooveBallBearing;
                    }
                    else if (currentTableName == "taperedRollerBearingsInSingleRows")
                    {
                        OutAssignDictPtr["计算系数e"].Value = double.Parse(selectedRow["e"].ToString());
                        OutAssignDictPtr["计算系数Y"].Value = double.Parse(selectedRow["Y"].ToString());
                        OutAssignDictPtr["计算系数Y0"].Value = double.Parse(selectedRow["Y0"].ToString());
                        OutAssignDictPtr["轴承内径d"].Value = double.Parse(selectedRow["d"].ToString());
                        OutAssignDictPtr["轴承外径D"].Value = double.Parse(selectedRow["_D"].ToString());
                        OutAssignDictPtr["轴承宽度T"].Value = double.Parse(selectedRow["T"].ToString());
                        OutAssignDictPtr["作用点距离左端面距离a"].Value = double.Parse(selectedRow["a"].ToString());
                        OutAssignDictPtr["轴承宽度B"].Value = double.Parse(selectedRow["B"].ToString());
                        OutAssignDictPtr["轴承宽度C"].Value = double.Parse(selectedRow["C"].ToString());
                        OutAssignDictPtr["轴承类型"].Value = RVReducerDesignApp.CTANBearing.TaperedRollerBearing;
                        mSelectBearingType = RVReducerDesignApp.CTANBearing.TaperedRollerBearing;
                        IsBearingSelected = true;
                    }
                    else if (currentTableName == "needleBearing" )
                    {
                        OutAssignDictPtr["轴承宽度Bc"].Value = double.Parse(selectedRow["Bc"].ToString());
                        OutAssignDictPtr["轴承内径Fw"].Value = double.Parse(selectedRow["FW"].ToString());
                        OutAssignDictPtr["轴承外径Ew"].Value = double.Parse(selectedRow["EW"].ToString());
                        OutAssignDictPtr["轴承类型"].Value = RVReducerDesignApp.CTANBearing.NeedleBearing;
                    }
                    else if (currentTableName == "custombearing")
                    {
                        string typeName = selectedRow["类型"].ToString();
                        int typeId = typeName switch
                        {
                            "深沟球轴承" => CTANBearing.DeepGrooveBallBearing,
                            "滚针轴承" => CTANBearing.NeedleBearing,
                            "圆锥滚子轴承" => CTANBearing.TaperedRollerBearing,
                            "角接触球轴承" => CTANBearing.AngularContactBallBearing,
                            _ => CTANBearing.DeepGrooveBallBearing // 默认
                        };

                        // 根据轴承类型确定维度键名
                        string innerKey = typeId == CTANBearing.NeedleBearing ? "轴承内径Fw" : "轴承内径d";
                        string outerKey = typeId == CTANBearing.NeedleBearing ? "轴承外径Ew" : "轴承外径D";
                        string widthKey;
                        if (typeId == CTANBearing.NeedleBearing) widthKey = "轴承宽度Bc";
                        else if (typeId == CTANBearing.TaperedRollerBearing) widthKey = "轴承宽度T";
                        else widthKey = "轴承宽度B";

                        // 设置轴承类型（CustomBearingPage 使用 "轴承" 键）
                        if (!OutAssignDictPtr.ContainsKey("轴承"))
                            OutAssignDictPtr.Add("轴承", new CTANDouble() { Value = typeId });
                        else
                            OutAssignDictPtr["轴承"].Value = typeId;

                        OutAssignDictPtr["类型"].Value = typeId;
                        OutAssignDictPtr["列数"].Value = Convert.ToDouble(selectedRow["列数"]);

                        // 类型感知的维度映射
                        OutAssignDictPtr[innerKey].Value = Convert.ToDouble(selectedRow["内径"]);
                        OutAssignDictPtr[outerKey].Value = Convert.ToDouble(selectedRow["外径"]);
                        OutAssignDictPtr[widthKey].Value = Convert.ToDouble(selectedRow["宽度"]);

                        // 同步到通用维度键（便于其他模块使用）
                        if (innerKey != "轴承内径d" && OutAssignDictPtr.ContainsKey("轴承内径d"))
                            OutAssignDictPtr["轴承内径d"].Value = OutAssignDictPtr[innerKey].Value;
                        if (outerKey != "轴承外径D" && OutAssignDictPtr.ContainsKey("轴承外径D"))
                            OutAssignDictPtr["轴承外径D"].Value = OutAssignDictPtr[outerKey].Value;
                        if (widthKey != "轴承宽度B" && OutAssignDictPtr.ContainsKey("轴承宽度B"))
                            OutAssignDictPtr["轴承宽度B"].Value = OutAssignDictPtr[widthKey].Value;

                        // 圆锥滚子轴承额外参数
                        if (typeId == CTANBearing.TaperedRollerBearing)
                        {
                            if (OutAssignDictPtr.ContainsKey("轴承宽度B"))
                                OutAssignDictPtr["轴承宽度B"].Value = Convert.ToDouble(selectedRow["宽度"]);
                            if (OutAssignDictPtr.ContainsKey("轴承宽度C"))
                                OutAssignDictPtr["轴承宽度C"].Value = Convert.ToDouble(selectedRow["宽度"]);
                        }

                        OutAssignDictPtr["bm系数"].Value = Convert.ToDouble(selectedRow["bm系数"]);
                        OutAssignDictPtr["接触角"].Value = Convert.ToDouble(selectedRow["接触角"]);
                        OutAssignDictPtr["滚子数Z"].Value = Convert.ToDouble(selectedRow["滚子数Z"]);
                        OutAssignDictPtr["节圆直径Dpw"].Value = Convert.ToDouble(selectedRow["节圆直径Dpw"]);
                        OutAssignDictPtr["滚动体直径Dw"].Value = Convert.ToDouble(selectedRow["滚动体直径Dw"]);
                        OutAssignDictPtr["滚子总长度L"].Value = Convert.ToDouble(selectedRow["滚子总长度L"]);
                        OutAssignDictPtr["滚子倒角r"].Value = Convert.ToDouble(selectedRow["滚子倒角r"]);
                        OutAssignDictPtr["滚子子类型"].Value = Convert.ToDouble(selectedRow["滚子子类型"]);

                        // 结构列
                        TrySetFromDb("外圈类型", "外圈类型");
                        TrySetFromDb("内圈类型", "内圈类型");
                        // 内部列
                        TrySetFromDb("估算内部细节", "估算内部细节");
                        // 圆锥滚子专用列
                        TrySetFromDb("计算系数e", "计算系数e");
                        TrySetFromDb("计算系数Y", "计算系数Y");
                        TrySetFromDb("计算系数Y0", "计算系数Y0");

                        // 新增列
                        TrySetFromDb("轴承代号", "轴承代号");
                        TrySetFromDb("滚子锥度角", "滚子锥度角");
                        TrySetFromDb("大端倒角rl", "大端倒角rl");
                        TrySetFromDb("小端倒角rs", "小端倒角rs");
                        TrySetFromDb("布置方式", "布置方式");
                        TrySetFromDb("装配数量", "装配数量");
                        TrySetFromDb("疲劳载荷极限Cu", "疲劳载荷极限Cu");
                        // 修正寿命列
                        TrySetFromDb("润滑油粘度v", "润滑油粘度v");
                        TrySetFromDb("污染等级", "污染等级");
                        TrySetFromDb("膜厚比lambda", "膜厚比lambda");
                        TrySetFromDb("极压添加剂", "极压添加剂");
                        TrySetFromDb("可靠度", "可靠度");
                        // 工况参数列
                        TrySetFromDb("转速", "转速");
                        TrySetFromDb("径向力", "径向力");
                        TrySetFromDb("轴向力", "轴向力");

                        OutAssignDictPtr["轴承类型"].Value = RVReducerDesignApp.CTANBearing.CustomBearing;
                    }
                }
                this.DialogResult = true;
                this.Close();

            }
            else
            {
                MessageBox.Show("请选择一行数据后再点击确定。");
            }
        }
        /// <summary>
        /// 安全地从 DataRow 读取值写入 Dict（如果列存在且值有效）
        /// </summary>
        private void TrySetFromDb(string dictKey, string dbColumn)
        {
            if (selectedRow.Row.Table.Columns.Contains(dbColumn) && OutAssignDictPtr.ContainsKey(dictKey))
            {
                string val = selectedRow[dbColumn]?.ToString();
                if (double.TryParse(val, out double d))
                    OutAssignDictPtr[dictKey].Value = d;
            }
        }

        private string BuildFilterClause()
        {
            string dmin = Wmininnerdiameter.Text;
            string dmax = Wmaxinnerdiameter.Text;
            string Dmin = Wminoutsidediameter.Text;
            string Dmax = Wmaxoutsidediameter.Text;
            string Crmin = Wmindynamicloads.Text;
            string Crmax = Wmaxdynamicloads.Text;

            bool validdMin = double.TryParse(dmin, out double dMin);
            bool validdMax = double.TryParse(dmax, out double dMax);
            bool validDMin = double.TryParse(Dmin, out double DMin);
            bool validDMax = double.TryParse(Dmax, out double DMax);
            bool validCrMin = double.TryParse(Crmin, out double CrMin);
            bool validCrMax = double.TryParse(Crmax, out double CrMax);

            StringBuilder whereClause = new StringBuilder();

            if (Winnerdiameter.IsChecked == true)
            {
                if (validdMin && validdMax) whereClause.Append($" AND d >= {dMin} AND d <= {dMax}");
                else if (validdMin) whereClause.Append($" AND d >= {dMin}");
                else if (validdMax) whereClause.Append($" AND d <= {dMax}");
            }
            if (Woutsidediameter.IsChecked == true)
            {
                if (validDMin && validDMax) whereClause.Append($" AND _D >= {DMin} AND _D <= {DMax}");
                else if (validDMin) whereClause.Append($" AND _D >= {DMin}");
                else if (validDMax) whereClause.Append($" AND _D <= {DMax}");
            }
            if (Wdynamicloads.IsChecked == true)
            {
                if (validCrMin && validCrMax) whereClause.Append($" AND Cr >= {CrMin} AND Cr <= {CrMax}");
                else if (validCrMin) whereClause.Append($" AND Cr >= {CrMin}");
                else if (validCrMax) whereClause.Append($" AND Cr <= {CrMax}");
            }

            return whereClause.ToString();
        }
        //CheckBox的使用////////////////////////////////////////////////////////start 
        private void Woutsidediameter_checked(object sender, RoutedEventArgs e) { Wminoutsidediameter.IsEnabled = true; Wmaxoutsidediameter.IsEnabled = true; }
        private void Winnerdiameter_checked(object sender, RoutedEventArgs e) { Wmininnerdiameter.IsEnabled = true; Wmaxinnerdiameter.IsEnabled = true; }
        private void Wdynamicloads_checked(object sender, RoutedEventArgs e) { Wmindynamicloads.IsEnabled = true; Wmaxdynamicloads.IsEnabled = true; }
        private void Woutsidediameter_Unchecked(object sender, RoutedEventArgs e) { Wminoutsidediameter.IsEnabled = false; Wmaxoutsidediameter.IsEnabled = false; }
        private void Winnerdiameter_Unchecked(object sender, RoutedEventArgs e) { Wmininnerdiameter.IsEnabled = false; Wmaxinnerdiameter.IsEnabled = false; }
        private void Wdynamicloads_Unchecked(object sender, RoutedEventArgs e) { Wmindynamicloads.IsEnabled = false; Wmaxdynamicloads.IsEnabled = false; }
        //CheckBox的使用 ////////////////////////////////////////////////////////end
        //private void Wdrawbeams_Click(object sender, RoutedEventArgs e)
        //{
        //    CTANGirderPage girderPage = new CTANGirderPage();
        //    Pdrawbeams.Navigate(girderPage);
        //}
        /// <summary>
        /// 修改按钮
        /// </summary>
        private void Revise_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (Wdatalist.SelectedIndex < 0)
                {
                    MessageBox.Show("请先选择一行要修改的数据！");
                    return;
                }

                DataRowView selectedRow = (DataRowView)Wdatalist.Items[Wdatalist.SelectedIndex];

                if (Bname == "custombearing")
                {
                    // 自定义轴承：使用专用编辑窗口
                    var editWindow = new CustomBearingEditWindow("Revise", Bname, ConnectionString, selectedRow);
                    bool? result = editWindow.ShowDialog();
                    if (result == true)
                    {
                        UpdateTable(null, null);
                    }
                    return;
                }

                // 传递选中行的【全列数据】，即使第一列空也能精准定位
                DataTrans.Clear();
                foreach (var item in selectedRow.Row.ItemArray)
                {
                    DataTrans.Add(item?.ToString() ?? string.Empty);
                }

                State = "Revise";
                Add_New addNewWindow = new Add_New();
                bool? reviseResult = addNewWindow.ShowDialog();
                if (reviseResult == true)
                {
                    UpdateTable(null, null);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"修改数据错误: {ex.Message}");
            }
        }
        private void Add_Click(object sender, RoutedEventArgs e)
        {
            if (Bname == null)
                MessageBox.Show("错误:未找到匹配的表名！");
            else if (Bname == "custombearing")
            {
                // 自定义轴承：使用专用编辑窗口
                var editWindow = new CustomBearingEditWindow("Add", Bname, ConnectionString);
                bool? result = editWindow.ShowDialog();
                if (result == true)
                {
                    UpdateTable(null, null);
                }
            }
            else
            {
                State = "Add";
                Add_New addNewWindow = new Add_New();
                bool? result = addNewWindow.ShowDialog();
                if (result == true)
                {
                    UpdateTable(null, null);
                }
            }
        }
        /// <summary>
        /// 删除按钮
        /// </summary>
        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (Wdatalist.SelectedIndex < 0)
                {
                    MessageBox.Show("请先选择一行要删除的数据！");
                    return;
                }

                DataRowView selectedRow = (DataRowView)Wdatalist.Items[Wdatalist.SelectedIndex];
                DataTable table = selectedRow.Row.Table;

                var whereConditions = new List<string>();
                var cmdParams = new List<MySqlParameter>();

                for (int i = 0; i < table.Columns.Count; i++)
                {
                    string colName = table.Columns[i].ColumnName;
                    object colValue = selectedRow.Row[i];
                    string paramName = $"@param{i}";

                    if (colValue == DBNull.Value)
                    {

                        whereConditions.Add($"`{colName}` IS NULL");
                    }
                    else if (colValue is string strVal && string.IsNullOrEmpty(strVal))
                    {

                        whereConditions.Add($"`{colName}` = ''");
                    }
                    else
                    {

                        whereConditions.Add($"`{colName}` = {paramName}");
                        cmdParams.Add(new MySqlParameter(paramName, colValue.ToString().Trim()));
                    }
                }

                MessageBoxResult confirmResult = (MessageBoxResult)MessageBox.Show("确定要删除选中的这条数据吗？\n此操作无法恢复。", "确认删除", (MessageBoxButtons)MessageBoxButton.YesNo, (MessageBoxIcon)MessageBoxImage.Warning);
                if (confirmResult != MessageBoxResult.Yes)
                {
                    return;
                }

                string deleteQuery = $"DELETE FROM `{currentTableName}` WHERE {string.Join(" AND ", whereConditions)}";
                using (var conn = new MySqlConnection(ConnectionString))
                {
                    conn.Open();
                    using (var cmd = new MySqlCommand(deleteQuery, conn))
                    {
                        cmd.Parameters.AddRange(cmdParams.ToArray());
                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected == 1)
                        {
                            MessageBox.Show("数据删除成功！");
                        }
                        else if (rowsAffected == 0)
                        {
                            MessageBox.Show("数据删除失败，可能该行数据与数据库中存储的形式不匹配（如NULL/空字符串差异）");
                        }
                        else
                        {
                            MessageBox.Show($"警告：意外删除{rowsAffected}条数据（请确保选中行数据唯一）");
                        }
                    }
                }

                UpdateTable(null, null);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"删除数据错误: {ex.Message}");
            }
        }
#region 导出方法
        private void Export_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(currentTableName))
            {
                MessageBox.Show("请先从左侧列表中选择一个轴承类型。");
                return;
            }

            string query = $"SELECT * FROM `{currentTableName}` WHERE 1=1";
            query += BuildFilterClause();

            try
            {
                DataTable dataTable = dbHelper.GetDataTable(query);

                if (dataTable.Rows.Count == 0)
                {
                    MessageBox.Show("没有符合条件的数据可供导出。");
                    return;
                }

                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Title = "导出数据";
                saveFileDialog.FileName = $"{currentTableName}_export_{DateTime.Now:yyyyMMddHHmmss}";
                saveFileDialog.Filter = "CSV 文件 (*.csv)|*.csv|TXT 文件 (*.txt)|*.txt|Excel 文件 (*.xlsx)|*.xlsx|JSON 文件 (*.json)|*.json|SQL 脚本 (*.sql)|*.sql|XML 文件 (*.xml)|*.xml|Markdown 表格 (*.md)|*.md|所有文件 (*.*)|*.*";
                saveFileDialog.DefaultExt = ".csv";

                if (saveFileDialog.ShowDialog() == true)
                {
                    string filePath = saveFileDialog.FileName;
                    string extension = Path.GetExtension(filePath).ToLower();

                    switch (extension)
                    {
                        case ".csv": ExportToCsv(dataTable, filePath); break;
                        case ".txt": ExportToTxt(dataTable, filePath); break;
                        case ".xlsx": ExportToExcel(dataTable, filePath); break;
                        case ".json": ExportToJson(dataTable, filePath); break;
                        case ".sql": ExportToSql(dataTable, filePath); break;
                        case ".xml": ExportToXml(dataTable, filePath); break;
                        case ".md": ExportToMarkdown(dataTable, filePath); break;
                        default: MessageBox.Show("不支持的文件格式。"); break;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"导出数据错误: {ex.Message}");
            }
        }

        private void ExportToCsv(DataTable dataTable, string filePath)
        {
            StringBuilder sb = new StringBuilder();
            IEnumerable<string> columnNames = dataTable.Columns.Cast<DataColumn>().Select(column => EscapeCsvField(column.ColumnName));
            sb.AppendLine(string.Join(",", columnNames));
            foreach (DataRow row in dataTable.Rows)
            {
                IEnumerable<string> fields = row.ItemArray.Select(field => EscapeCsvField(field.ToString()));
                sb.AppendLine(string.Join(",", fields));
            }
            File.WriteAllText(filePath, sb.ToString(), new UTF8Encoding(true));
            MessageBox.Show($"CSV 文件已成功导出到:\n{filePath}");
        }

        private void ExportToTxt(DataTable dataTable, string filePath)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(string.Join("\t", dataTable.Columns.Cast<DataColumn>().Select(col => col.ColumnName)));
            foreach (DataRow row in dataTable.Rows)
            {
                var fields = row.ItemArray.Select(field => field.ToString().Replace("\r\n", " ").Replace("\n", " "));
                sb.AppendLine(string.Join("\t", fields));
            }
            File.WriteAllText(filePath, sb.ToString(), new UTF8Encoding(true));
            MessageBox.Show($"TXT 文件已成功导出到:\n{filePath}");
        }

        private void ExportToExcel(DataTable dataTable, string filePath)
        {
            using (IWorkbook workbook = new SXSSFWorkbook(100))
            {
                ISheet sheet = workbook.CreateSheet("数据");
                IRow headerRow = sheet.CreateRow(0);
                for (int i = 0; i < dataTable.Columns.Count; i++)
                {
                    headerRow.CreateCell(i).SetCellValue(dataTable.Columns[i].ColumnName);
                }
                for (int i = 0; i < dataTable.Rows.Count; i++)
                {
                    IRow dataRow = sheet.CreateRow(i + 1);
                    for (int j = 0; j < dataTable.Columns.Count; j++)
                    {
                        dataRow.CreateCell(j).SetCellValue(dataTable.Rows[i][j].ToString());
                    }
                }
                for (int i = 0; i < dataTable.Columns.Count; i++)
                {
                    sheet.SetColumnWidth(i, 20 * 256);
                }
                using (FileStream fs = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                {
                    workbook.Write(fs);
                }
            }
            MessageBox.Show($"Excel 文件已成功导出到:\n{filePath}");
        }

        private void ExportToJson(DataTable dataTable, string filePath)
        {
            var data = dataTable.AsEnumerable().Select(row =>
                dataTable.Columns.Cast<DataColumn>().ToDictionary(col => col.ColumnName, col => row[col])
            ).ToList();
            string json = JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(filePath, json, Encoding.UTF8);
            MessageBox.Show($"JSON 文件已成功导出到:\n{filePath}");
        }

        private void ExportToSql(DataTable dataTable, string filePath)
        {
            StringBuilder sqlBuilder = new StringBuilder();
            string tableName = Path.GetFileNameWithoutExtension(filePath);
            sqlBuilder.AppendLine($"-- SQL 导出脚本");
            sqlBuilder.AppendLine($"-- 生成时间: {DateTime.Now}");
            sqlBuilder.AppendLine($"-- 表名: {tableName}");
            sqlBuilder.AppendLine();
            sqlBuilder.AppendLine($"-- 创建表结构");
            sqlBuilder.AppendLine($"CREATE TABLE `{tableName}` (");
            for (int i = 0; i < dataTable.Columns.Count; i++)
            {
                DataColumn col = dataTable.Columns[i];
                string sqlType = MapDotNetTypeToSqlType(col.DataType);
                string columnDef = $"    `{col.ColumnName}` {sqlType}";
                if (i < dataTable.Columns.Count - 1) { columnDef += ","; }
                sqlBuilder.AppendLine(columnDef);
            }
            sqlBuilder.AppendLine(");");
            sqlBuilder.AppendLine();
            sqlBuilder.AppendLine($"-- 插入数据");
            string columnNames = string.Join(", ", dataTable.Columns.Cast<DataColumn>().Select(c => $"`{c.ColumnName}`"));
            foreach (DataRow row in dataTable.Rows)
            {
                var values = row.ItemArray.Select(value => FormatSqlValue(value));
                string valuesStr = string.Join(", ", values);
                sqlBuilder.AppendLine($"INSERT INTO `{tableName}` ({columnNames}) VALUES ({valuesStr});");
            }
            File.WriteAllText(filePath, sqlBuilder.ToString(), new UTF8Encoding(true));
            MessageBox.Show($"SQL 脚本已成功导出到:\n{filePath}");
        }

        private void ExportToXml(DataTable dataTable, string filePath)
        {
            if (string.IsNullOrEmpty(dataTable.TableName))
            {
                dataTable.TableName = !string.IsNullOrEmpty(currentTableName) ? currentTableName : "ExportData";
            }
            dataTable.WriteXml(filePath);
            MessageBox.Show($"XML 文件已成功导出到:\n{filePath}");
        }

        private void ExportToMarkdown(DataTable dataTable, string filePath)
        {
            StringBuilder mdBuilder = new StringBuilder();
            mdBuilder.AppendLine($"| {string.Join(" | ", dataTable.Columns.Cast<DataColumn>().Select(c => c.ColumnName))} |");
            mdBuilder.AppendLine($"| {string.Join(" | ", dataTable.Columns.Cast<DataColumn>().Select(_ => "---"))} |");
            foreach (DataRow row in dataTable.Rows)
            {
                var values = row.ItemArray.Select(v => v.ToString().Replace("|", "\\|"));
                mdBuilder.AppendLine($"| {string.Join(" | ", values)} |");
            }
            File.WriteAllText(filePath, mdBuilder.ToString(), new UTF8Encoding(true));
            MessageBox.Show($"Markdown 表格已成功导出到:\n{filePath}");
        }

        private string MapDotNetTypeToSqlType(Type dotNetType)
        {
            if (dotNetType == typeof(string)) return "VARCHAR(255)";
            if (dotNetType == typeof(int) || dotNetType == typeof(long)) return "INT";
            if (dotNetType == typeof(decimal) || dotNetType == typeof(double) || dotNetType == typeof(float)) return "DECIMAL(18, 4)";
            if (dotNetType == typeof(DateTime)) return "DATETIME";
            if (dotNetType == typeof(bool)) return "TINYINT(1)";
            return "TEXT";
        }

        private string FormatSqlValue(object value)
        {
            if (value == DBNull.Value || value == null) return "NULL";
            if (value is string) return $"'{value.ToString().Replace("'", "''")}'";
            if (value is DateTime) return $"'{((DateTime)value).ToString("yyyy-MM-dd HH:mm:ss")}'";
            if (value is bool) return ((bool)value) ? "1" : "0";
            return value.ToString();
        }

        private string EscapeCsvField(string field)
        {
            if (field.Contains(",") || field.Contains("\"") || field.Contains("\r") || field.Contains("\n"))
            {
                return $"\"{field.Replace("\"", "\"\"")}\"";
            }
            return field;
        }
#endregion
    }
}
