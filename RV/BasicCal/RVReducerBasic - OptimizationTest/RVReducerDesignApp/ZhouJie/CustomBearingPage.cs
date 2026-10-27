using System.Windows;
using System.Windows.Controls;

namespace RVReducerDesignApp
{
    public class CustomBearingPage : CTANBasicContentPageObj
    {
        private CTANComboBox mSelectBearingComboBox;
        private TabControl mInnerAllTab;

        // 预定义类型选项（与 CTANBearing.mBearingTypeText 前几个一致）
        private int[] bearingTypeIds = new int[]
        {
            CTANBearing.DeepGrooveBallBearing,
            CTANBearing.NeedleBearing,
            CTANBearing.TaperedRollerBearing,
            CTANBearing.AngularContactBallBearing
        };
        private string[] bearingTypeNames = new string[]
        {
             "深沟球轴承", "滚针轴承", "圆锥滚子轴承", "角接触球轴承"
        };

        // ============ Tab 1: 几何（原类型尺寸 + 内部数据合并）============
        private string[][] mStructParamTable = new string[][]
        {
            // 0-深沟球: 接触角, 节圆直径Dpw, 滚动体直径Dw, 列数, 滚子数Z, 滚子子类型
            new string[] { "接触角", "度", "节圆直径Dpw", "毫米", "滚动体直径Dw", "毫米", "列数", "", "滚子数Z", "", "滚子子类型", "" },
            // 1-滚针: 接触角, 节圆直径Dpw, 滚动体直径Dw, 滚子总长度L, 滚子倒角r, 列数, 滚子数Z, 滚子子类型
            new string[] { "接触角", "度", "节圆直径Dpw", "毫米", "滚动体直径Dw", "毫米", "滚子总长度L", "毫米", "滚子倒角r", "毫米", "列数", "", "滚子数Z", "", "滚子子类型", "" },
            // 2-圆锥滚子: 接触角, 节圆直径Dpw, 滚动体直径Dw, 滚子总长度L, 滚子倒角r, 大端倒角rl, 小端倒角rs, 滚子锥度角, 列数, 滚子数Z, 滚子子类型
            new string[] { "接触角", "度", "节圆直径Dpw", "毫米", "滚动体直径Dw", "毫米", "滚子总长度L", "毫米", "滚子倒角r", "毫米", "大端倒角rl", "毫米", "小端倒角rs", "毫米", "滚子锥度角", "度", "列数", "", "滚子数Z", "", "滚子子类型", "" },
            // 3-角接触: 接触角, 节圆直径Dpw, 滚动体直径Dw, 列数, 滚子数Z, 滚子子类型
            new string[] { "接触角", "度", "节圆直径Dpw", "毫米", "滚动体直径Dw", "毫米", "列数", "", "滚子数Z", "", "滚子子类型", "" }
        };

        // ============ 已删除：结构Tab(移至修正参数)、内部Tab(合并至几何)、性能Tab(合并至载荷) ============
        private static int[] mRingTypeIds = new int[] { 0, 1 };
        private static string[] mRingTypeNames = new string[] { "加工的", "与轴或箱体集成的" };
        private static int[] mSubTypeIds = new int[] { 0, 1, 2, 3 };
        private static string[] mSubTypeNames = new string[] { "圆柱滚子", "圆锥滚子", "冲压滚针", "调心滚子" };

        // ============ 载荷Tab合并（原性能 + 原载荷）============
        private string[][] mLoadCombinedTable = new string[][]
        {
            // 0-深沟球: Cr, C0r, bm系数 (千牛) + 转速, 径向力, 轴向力
            new string[] { "基本额定动载荷Cr", "千牛", "基本额定静载荷C0r", "千牛", "bm系数", "", "转速", "转每分钟", "径向力", "N", "轴向力", "N" },
            // 1-滚针: Cr, C0r, bm系数 + 转速, 径向力, 轴向力
            new string[] { "基本额定动载荷Cr", "千牛", "基本额定静载荷C0r", "千牛", "bm系数", "", "转速", "转每分钟", "径向力", "N", "轴向力", "N" },
            // 2-圆锥滚子: Cr, C0r, e, Y, Y0, bm系数 + 转速, 径向力, 轴向力
            new string[] { "基本额定动载荷Cr", "千牛", "基本额定静载荷C0r", "千牛", "计算系数e", "", "计算系数Y", "", "计算系数Y0", "", "bm系数", "", "转速", "转每分钟", "径向力", "N", "轴向力", "N" },
            // 3-角接触: Cr, C0r, bm系数 + 转速, 径向力, 轴向力
            new string[] { "基本额定动载荷Cr", "千牛", "基本额定静载荷C0r", "千牛", "bm系数", "", "转速", "转每分钟", "径向力", "N", "轴向力", "N" }
        };

        // ============ Tab 3: 定位参数（精简）============
        private string[] mLayoutParams = new string[]
        {
            "布置方式", "",
            "装配数量", ""
        };
        private static int[] mLayoutTypeIds = new int[] { 0, 1, 2 };
        private static string[] mLayoutTypeNames = new string[] { "面对面", "背对背", "串联" };

        // ============ Tab 7: 修正参数（含结构内容 + 疲劳载荷极限Cu）============
        private string[] mModifiedLifeParams = new string[]
        {
            "外圈类型", "",
            "内圈类型", "",
            "疲劳载荷极限Cu", "N",
            "润滑油粘度v", "mm²/s",
            "污染等级", "",
            "膜厚比lambda", "",
            "极压添加剂", "",
            "可靠度", "%"
        };
        private static int[] mContamLevelIds = new int[] { 0, 1, 2, 3, 4 };
        private static string[] mContamLevelNames = new string[] { "清洁", "轻度污染", "中度污染", "重度污染", "极重污染" };
        private static int[] mEPAdditiveIds = new int[] { 0, 1 };
        private static string[] mEPAdditiveNames = new string[] { "无", "有" };

        private TabItem mGeometryTab;
        private TabItem mLoadTab;
        private TabItem mLayoutTab;
        private TabItem mModifiedLifeTab;

        public CustomBearingPage(CTANDict sharedDict) : base(InRegisterToPageDictionary: false)
        {
            // 替换为外部参数字典
            this.mDataStorageDict = sharedDict;
            EnsureRequiredDictEntries();
            SetInputPage();
        }

        /// <summary>
        /// 确保新用到的参数字典条目已存在
        /// </summary>
        private void EnsureRequiredDictEntries()
        {
            // 同步"轴承"(CustomBearingPage用)与"轴承类型"(CTANBearing主页面用)
            if (!mDataStorageDict.ContainsKey("轴承"))
                mDataStorageDict.Add("轴承", new CTANDouble() { Value = CTANBearing.DeepGrooveBallBearing });
            if (mDataStorageDict.ContainsKey("轴承类型"))
                mDataStorageDict["轴承"].Value = mDataStorageDict["轴承类型"].Value;

            if (!mDataStorageDict.ContainsKey("外圈类型"))
                mDataStorageDict.Add("外圈类型", new CTANDouble() { Value = 0 });
            if (!mDataStorageDict.ContainsKey("内圈类型"))
                mDataStorageDict.Add("内圈类型", new CTANDouble() { Value = 0 });
            if (!mDataStorageDict.ContainsKey("估算内部细节"))
                mDataStorageDict.Add("估算内部细节", new CTANDouble() { Value = 0 });
            if (!mDataStorageDict.ContainsKey("列数"))
                mDataStorageDict.Add("列数", new CTANDouble() { Value = 1 });
            // 载荷 Tab 键
            if (!mDataStorageDict.ContainsKey("转速"))
                mDataStorageDict.Add("转速", new CTANDouble() { Value = 2000 });
            if (!mDataStorageDict.ContainsKey("径向力"))
                mDataStorageDict.Add("径向力", new CTANDouble() { Value = 1750 });
            if (!mDataStorageDict.ContainsKey("轴向力"))
                mDataStorageDict.Add("轴向力", new CTANDouble() { Value = 740 });
            // 定位参数 Tab 键
            if (!mDataStorageDict.ContainsKey("布置方式"))
                mDataStorageDict.Add("布置方式", new CTANDouble() { Value = 0 });
            if (!mDataStorageDict.ContainsKey("装配数量"))
                mDataStorageDict.Add("装配数量", new CTANDouble() { Value = 1 });
            if (!mDataStorageDict.ContainsKey("Cu"))
                mDataStorageDict.Add("Cu", new CTANDouble() { Value = 0 });
            // 大端/小端倒角（新UI键名）
            if (!mDataStorageDict.ContainsKey("大端倒角rl"))
                mDataStorageDict.Add("大端倒角rl", new CTANDouble() { Value = 0 });
            if (!mDataStorageDict.ContainsKey("小端倒角rs"))
                mDataStorageDict.Add("小端倒角rs", new CTANDouble() { Value = 0 });
            if (!mDataStorageDict.ContainsKey("滚子锥度角"))
                mDataStorageDict.Add("滚子锥度角", new CTANDouble() { Value = 0 });
            // 新UI名称初始化（SetStandartInputTextAndBingdingToDict用参数名为字典键）
            if (!mDataStorageDict.ContainsKey("节圆直径Dpw"))
                mDataStorageDict.Add("节圆直径Dpw", new CTANDouble() { Value = 0 });
            if (!mDataStorageDict.ContainsKey("滚动体直径Dw"))
                mDataStorageDict.Add("滚动体直径Dw", new CTANDouble() { Value = 0 });
            if (!mDataStorageDict.ContainsKey("滚子总长度L"))
                mDataStorageDict.Add("滚子总长度L", new CTANDouble() { Value = 0 });
            if (!mDataStorageDict.ContainsKey("滚子倒角r"))
                mDataStorageDict.Add("滚子倒角r", new CTANDouble() { Value = 0 });
            if (!mDataStorageDict.ContainsKey("滚子子类型"))
                mDataStorageDict.Add("滚子子类型", new CTANDouble() { Value = 0 });
            // 这些旧键不再需要，新UI使用 Cu→疲劳载荷极限Cu, 大端倒角→大端倒角rl, 小端倒角→小端倒角rs
            if (!mDataStorageDict.ContainsKey("疲劳载荷极限Cu"))
                mDataStorageDict.Add("疲劳载荷极限Cu", new CTANDouble() { Value = 0 });
            // 修正寿命参数
            if (!mDataStorageDict.ContainsKey("润滑油粘度v"))
                mDataStorageDict.Add("润滑油粘度v", new CTANDouble() { Value = 0 });
            if (!mDataStorageDict.ContainsKey("污染等级"))
                mDataStorageDict.Add("污染等级", new CTANDouble() { Value = 0 });
            if (!mDataStorageDict.ContainsKey("膜厚比lambda"))
                mDataStorageDict.Add("膜厚比lambda", new CTANDouble() { Value = 0 });
            if (!mDataStorageDict.ContainsKey("极压添加剂"))
                mDataStorageDict.Add("极压添加剂", new CTANDouble() { Value = 0 });
            if (!mDataStorageDict.ContainsKey("可靠度"))
                mDataStorageDict.Add("可靠度", new CTANDouble() { Value = 90 });
        }

        private void SetInputPage()
        {
            mInputPageGrid.SetGridRowAndColumn(2, 1);

            // 第一行：上方类型切换控件
            CTANGridWithControl topGrid = new CTANGridWithControl();
            topGrid.SetGridRowAndColumn(1, 3);
            topGrid.SetGridControlByCreateDisplayTextBox(0, 0, "自定义轴承");

            mSelectBearingComboBox = new CTANComboBox(bearingTypeIds, bearingTypeNames);

            // 将ComboBox的数据与"轴承"字典绑定
            if (mDataStorageDict != null && mDataStorageDict.ContainsKey("轴承"))
            {
                mDataStorageDict["轴承"].SetDataBingingToComboBox(mSelectBearingComboBox);
            }

            mSelectBearingComboBox.SelectionChanged += OnBearingTypeSelectionChanged;

            topGrid.AssignControlToGrid(0, 1, mSelectBearingComboBox);
            mInputPageGrid.AssignControlToGrid(0, 0, topGrid);

            // 第二行：TabControl 容器（4个Tab）
            mInnerAllTab = new TabControl
            {
                TabStripPlacement = Dock.Top,
                Margin = new Thickness(5)
            };
            Grid.SetRow(mInnerAllTab, 1);
            mInputPageGrid.Children.Add(mInnerAllTab);

            mGeometryTab = new TabItem { Header = "几何", Name = "tabGeometry" };
            mLoadTab = new TabItem { Header = "载荷", Name = "tabLoad" };
            mLayoutTab = new TabItem { Header = "定位参数", Name = "tabLayout" };
            mModifiedLifeTab = new TabItem { Header = "修正参数", Name = "tabModifiedLife" };

            mInnerAllTab.Items.Add(mGeometryTab);
            mInnerAllTab.Items.Add(mLoadTab);
            mInnerAllTab.Items.Add(mLayoutTab);
            mInnerAllTab.Items.Add(mModifiedLifeTab);

            // 若没有初始选择项，触发初始页面刷新
            if (mSelectBearingComboBox.Items.Count > 0 && mSelectBearingComboBox.SelectedIndex == -1)
            {
                mSelectBearingComboBox.SelectedIndex = 0;
            }
            else if (mSelectBearingComboBox.SelectedIndex >= 0)
            {
                OnBearingTypeSelectionChanged(mSelectBearingComboBox, null);
            }
        }

        private void OnBearingTypeSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int index = mSelectBearingComboBox.SelectedIndex;
            if (index < 0 || index >= mStructParamTable.Length) return;

            // --- Tab 1: 几何（类型尺寸 + 内部数据合并）---
            string[] structParams = mStructParamTable[index];
            int structRowCount = structParams.Length / 2;
            CTANGridWithControl geometryGrid = new CTANGridWithControl();
            geometryGrid.SetStandartDisplayText(mHeadText, structParams, 2);
            geometryGrid.SetStandartInputTextAndBingdingToDict(
                [1, structRowCount, 2, 1],
                structParams, 2, mDataStorageDict);
            mGeometryTab.Content = geometryGrid;
            // 将"滚子子类型"替换为下拉框
            ReplaceWithComboBox(geometryGrid, "滚子子类型", 2, mSubTypeIds, mSubTypeNames);

            // --- Tab 2: 载荷（性能 + 载荷合并）---
            string[] loadCombinedParams = mLoadCombinedTable[index];
            int loadCombinedRowCount = loadCombinedParams.Length / 2;
            CTANGridWithControl loadGrid = new CTANGridWithControl();
            loadGrid.SetStandartDisplayText(mHeadText, loadCombinedParams, 2);
            loadGrid.SetStandartInputTextAndBingdingToDict(
                [1, loadCombinedRowCount, 2, 1],
                loadCombinedParams, 2, mDataStorageDict);
            mLoadTab.Content = loadGrid;

            // --- Tab 3: 定位参数 ---
            BuildLayoutTab();

            // --- Tab 4: 修正参数（原修正参数 + 结构内容）---
            BuildModifiedLifeTab();
        }

        /// <summary>
        /// 构建"修正参数"Tab：外圈类型、内圈类型（下拉框）+ 疲劳载荷极限Cu + 润滑油粘度、污染等级、膜厚比、极压添加剂、可靠度
        /// </summary>
        private void BuildModifiedLifeTab()
        {
            int modifiedRowCount = mModifiedLifeParams.Length / 2; // 8行参数
            CTANGridWithControl modifiedGrid = new CTANGridWithControl();
            modifiedGrid.SetStandartDisplayText(mHeadText, mModifiedLifeParams, 2);
            modifiedGrid.SetStandartInputTextAndBingdingToDict(
                [1, modifiedRowCount, 2, 1],
                mModifiedLifeParams, 2, mDataStorageDict);

            // 将"外圈类型"/"内圈类型"/"污染等级"/"极压添加剂"替换为下拉框
            ReplaceWithComboBox(modifiedGrid, "外圈类型", 2, mRingTypeIds, mRingTypeNames);
            ReplaceWithComboBox(modifiedGrid, "内圈类型", 2, mRingTypeIds, mRingTypeNames);
            ReplaceWithComboBox(modifiedGrid, "污染等级", 2, mContamLevelIds, mContamLevelNames);
            ReplaceWithComboBox(modifiedGrid, "极压添加剂", 2, mEPAdditiveIds, mEPAdditiveNames);

            mModifiedLifeTab.Content = modifiedGrid;
        }

        /// <summary>
        /// 将Grid中指定参数行的输入控件替换为下拉框
        /// </summary>
        private void ReplaceWithComboBox(CTANGridWithControl grid, string paramName, int columnIndex, int[] ids, string[] displayStrings)
        {
            if (!grid.mRowNameToIndex.ContainsKey(paramName)) return;
            int rowIndex = grid.mRowNameToIndex[paramName];

            // 移除旧控件（如果有）
            if (grid.mControlTable[rowIndex][columnIndex] != null)
            {
                var oldControl = grid.mControlTable[rowIndex][columnIndex].mObject as UIElement;
                if (oldControl != null)
                {
                    grid.Children.Remove(oldControl);
                }
                grid.mControlTable[rowIndex][columnIndex] = null;
            }

            // 创建下拉框
            CTANComboBox comboBox = new CTANComboBox(ids, displayStrings);
            grid.mControlTable[rowIndex][columnIndex] = new CTANObject();
            grid.mControlTable[rowIndex][columnIndex].SetStorageObj(comboBox);
            Grid.SetRow(comboBox, rowIndex);
            Grid.SetColumn(comboBox, columnIndex);
            grid.Children.Add(comboBox);

            // 绑定字典数据
            mDataStorageDict[paramName].SetDataBingingToComboBox(comboBox);
        }

        /// <summary>
        /// 构建"定位参数"Tab：布置方式（下拉框）、装配数量
        /// </summary>
        private void BuildLayoutTab()
        {
            int layoutRowCount = mLayoutParams.Length / 2; // 2行参数
            CTANGridWithControl layoutGrid = new CTANGridWithControl();
            layoutGrid.SetStandartDisplayText(mHeadText, mLayoutParams, 2);
            layoutGrid.SetStandartInputTextAndBingdingToDict(
                [1, layoutRowCount, 2, 1],
                mLayoutParams, 2, mDataStorageDict);

            // 将"布置方式"的输入控件替换为下拉框
            ReplaceWithComboBox(layoutGrid, "布置方式", 2, mLayoutTypeIds, mLayoutTypeNames);

            mLayoutTab.Content = layoutGrid;
        }
    }
}
