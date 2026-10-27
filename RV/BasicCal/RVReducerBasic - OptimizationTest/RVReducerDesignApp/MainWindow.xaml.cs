using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Linq;
using System.IO;
using static Org.BouncyCastle.Math.EC.ECCurve;
using RVReducerDesignApp.TANUI;
using RVReducerDesignApp;
using RVReducerDesignApp.Views;
using Mysqlx.Session;
//using RVReducerDesignApp;

namespace RVReducerDesignApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //页面ID列表
        public static Dictionary<int, CTANBasicContentPageObj> sPageIDDict = new Dictionary<int, CTANBasicContentPageObj>();
        ////基本页面
        //public static UITANInsertSymmetryComponent sUISymmetryComponentInsert = new UITANInsertSymmetryComponent();


        double mHeadHeight = 26;
        double mContentRightWidth = 3;
        double mContentRightRatio = 0;
        double mContentLeftWidth = 1;
        double mContentLeftRatio = 0;
        CTANRVSet mRVElementSet = new CTANRVSet();
        public TreeView mTreeView = new TreeView();

        CTANScrollPage mInputPage = new CTANScrollPage();
        CTANScrollPage mOutputPage = new CTANScrollPage();
        CTANScrollPage mExamplePage = new CTANScrollPage();
        CTANScrollPage m3DPage = new CTANScrollPage();
        CTANScrollPage mPropertyPage = new CTANScrollPage();
        AIChatPanel mAIChatPanel = new();

        public MainWindow()
        {
            mRVElementSet.mCrankShaftSystem.mBelongedSet = mRVElementSet;
            //添加树视图
            InitializeComponent();
            mContentRightRatio = mContentRightWidth / (mContentRightWidth + mContentLeftWidth);
            mContentLeftRatio = mContentLeftWidth / (mContentRightWidth + mContentLeftWidth);
            Grid.SetRow(mTreeView, 0);
            MainWindowIndexGrid.Children.Add(mTreeView);
            mTreeView.Items.Add(mRVElementSet.mTreeViewItem);
            //添加属性视图
            Grid.SetRow(mPropertyPage, 1);
            MainWindowIndexGrid.Children.Add(mPropertyPage);
            //添加Frame框架视图
            RefreshProperties();
            //ContentGrid.Children.Add(mContentPage);
            mInputTab.Content = mInputPage;
            mOutputTab.Content = mOutputPage;
            mExampleTab.Content = mExamplePage;
            m3DTab.Content = m3DPage;
            mAITab.Content = mAIChatPanel;
            m3DTab.Visibility = Visibility.Collapsed; // 不启用3D页面
            ////测试代码
            //CTANBasicControl TestTANControl = new CTANBasicControl();
            //TestTANControl.mControlObj = new Button();
            //myFrame.Content = mContentPage;
            this.AddHandler(Window.SizeChangedEvent, new RoutedEventHandler(Window_SizeChanged));
            this.AddHandler(TreeViewItem.SelectedEvent, new RoutedEventHandler(MainContentGridRefresh));
            //mContentTab.Selected() += MainContentGridRefresh;
            mContentTab.SelectionChanged += MainContentGridRefresh;
            for (int i = 0; i < mTreeView.Items.Count; i++)
            {
                var item0 = mTreeView.Items[i] as TreeViewItem;
                item0.IsExpanded = true;
                for (int j = 0;j < item0.Items.Count; j++)
                {
                    var item1 = item0.Items[j] as TreeViewItem;
                    item1.IsExpanded = true;
                }
                
            }

        }

        //private void TestButton_Click(object sender, RoutedEventArgs e)
        //{
        //    UIZHOURecommand uIZHOURecommand = new UIZHOURecommand();
        //    uIZHOURecommand.mSetWindow.mRVSet = mRVElementSet;
        //    uIZHOURecommand.mSetWindow.SetUI();
        //    uIZHOURecommand.ShowDialog();

        //}
        private void CalButton_Click(object sender, RoutedEventArgs e)
        {
            TreeViewItem TmpTreeItem = mTreeView.SelectedItem as TreeViewItem;
            if (TmpTreeItem!=null)
            {
                CTANBasicContentPageObj TmpTANBasic = TmpTreeItem.DataContext as CTANBasicContentPageObj;
                TmpTANBasic.CalPageData();
            }
        }
        //private void MainContentGridRefresh()
        //{

        //}
        private void MainContentGridRefresh(object sender, RoutedEventArgs e)
        {
            TreeViewItem TempTreeViewItem = (TreeViewItem)mTreeView.SelectedItem;
            RefreshProperties();
            //更新控件
            
                //(TreeViewItem)e.OriginalSource;
            if (TempTreeViewItem != null)
            {
                (TempTreeViewItem.DataContext as CTANBasicContentPageObj).UpdatePageControlContent();
                CTANScrollPage Tmppage;
                var selectedTab = mContentTab.SelectedItem as TabItem;
                if (selectedTab == null) { e.Handled = true; return; }
                //CTANScrollPage Tmppage = ReturnCurrentContentScrollPage();
                //Tmppage.mPage.Content = (TempTreeViewItem.DataContext as CTANBasicContentPageObj).mPageGrid;
                if (selectedTab.Name == "mInputTab")
                {
                    Tmppage = ReturnCurrentContentScrollPage();
                    // 检查当前页面对象是否有内层TabControl
                    if ((TempTreeViewItem.DataContext as CTANBasicContentPageObj) is CTANInvGear invGear)
                    {
                        // 渐开线齿轮使用内层TabControl
                        // 确保内层TabControl已创建
                        // 这里假设CTANInvGear已经创建了mInnerTabControl
                        Tmppage.mPage.Content = invGear.mInputPageGrid;
                    }
                    else
                    {
                        // 其他页面使用原来的mInputPageGrid
                        Tmppage.mPage.Content = (TempTreeViewItem.DataContext as CTANBasicContentPageObj).mInputPageGrid;
                    }

                }
                else if (selectedTab.Name == "mOutputTab")
                {
                    Tmppage = ReturnCurrentContentScrollPage();
                    Tmppage.mPage.Content = (TempTreeViewItem.DataContext as CTANBasicContentPageObj).mOutputPageGrid;
                }
                else if (selectedTab.Name == "mExampleTab")
                {
                    Tmppage = ReturnCurrentContentScrollPage();
                    Tmppage.mPage.Content = (TempTreeViewItem.DataContext as CTANBasicContentPageObj).mExamplePageGrid;
                }
                else if (selectedTab.Name == "m3DTab")
                {
                    Tmppage = ReturnCurrentContentScrollPage();
                    Tmppage.mPage.Content = (TempTreeViewItem.DataContext as CTANBasicContentPageObj).m3DPageGrid;
                    var currentPageObj = TempTreeViewItem.DataContext as CTANBasicContentPageObj;
                    if(currentPageObj is CTANCrankShaft TmpCrankShaft)
                    {
                        TmpCrankShaft.mShaftDesignPage.Draw3D();
                    }
                }
                //测试属性视图
                mPropertyPage.mPage.Content = (TempTreeViewItem.DataContext as CTANBasicContentPageObj).mPropertyGrid;
            }
            e.Handled = true;
        }
        private void RefreshProperties()
        {
            //mContentTab.Width = mContentRightRatio * this.Width - 20;
            //MyToolbar.Width = this.Width - 20;
            CTANScrollPage TmpPage = ReturnCurrentContentScrollPage();
            if (TmpPage != null)
            {
                TmpPage.Width = mContentTab.Width-10;
                TmpPage.mFrameForPage.Width = TmpPage.Width - 30;

            }

        }
        private void Window_SizeChanged(object sender, RoutedEventArgs e)
        {

            RefreshProperties();
            // 在这里添加你的缩放逻辑
            // 例如：更新UI元素的位置或大小，或者其他需要的操作
            //mRVElementSet.mCrankShaftSystem.mCrankShaft.mShaftDesignPage.InitCentering();
        }
        private CTANScrollPage ReturnCurrentContentScrollPage()
        {
            return (mContentTab.SelectedContent) as CTANScrollPage;
        }

        private void BearingDataBtn_Click(object sender, RoutedEventArgs e)
        {
            WPFBearings.BearingDataWindow TmpBearingData = new WPFBearings.BearingDataWindow();
            TmpBearingData.IsReturnData = false;
            TmpBearingData.ShowDialog();
        }
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new Microsoft.Win32.SaveFileDialog()
            {
                InitialDirectory = AppDataStorage.AppDataPath,
                FileName = "ShaftData.json",
                Filter = "JSON Files (*.json)|*.json|PDF Files (*.pdf)|*.pdf|All Files (*.*)|*.*"
            };

            bool? result = dlg.ShowDialog();
            if (result == true)
            {
                try
                {
                    string chosenPath = dlg.FileName; // 完整路径（或相对）
                    AppDataStorage.SaveAllData(
                        mRVElementSet.mCrankShaftSystem.mCrankShaft.mShaftDesignPage.currentShaft,
                        MainWindow.sPageIDDict.Values,
                        chosenPath // 现在可以传入完整路径或文件名
                    );
                    MessageBox.Show($"保存完成：{chosenPath}", "保存成功", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"保存失败：{ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        // 打开按钮：传入完整路径给 LoadAllDataFromPath
        private void OpenButton_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new Microsoft.Win32.OpenFileDialog();
            openFileDialog.InitialDirectory = AppDataStorage.AppDataPath;
            openFileDialog.Filter = "JSON Files (*.json)|*.json|PDF Files (*.pdf)|*.pdf|All Files (*.*)|*.*";
            openFileDialog.FileName = "ShaftData.json";

            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    string chosenFile = openFileDialog.FileName; // 完整路径
                    string ext = System.IO.Path.GetExtension(chosenFile).ToLowerInvariant();
                    if (ext == ".pdf")
                    {
                        // PDF 为只读导出格式，当前不支持从 PDF 恢复数据
                        MessageBox.Show("选中的文件是 PDF 格式。当前版本的应用把 PDF 作为只读保存格式，暂不支持从 PDF 恢复数据。请使用 JSON 文件进行恢复。", "无法打开 PDF", MessageBoxButton.OK, MessageBoxImage.Information);
                        return;
                    }

                    var savedData = AppDataStorage.LoadAllDataFromPath(chosenFile);
                    if (savedData != null)
                    {
                        // 恢复 Shaft 并绘制
                        var page = mRVElementSet.mCrankShaftSystem.mCrankShaft.mShaftDesignPage;
                        page.currentShaft = savedData.ShaftData;
                        if (page.currentShaft != null)
                        {
                            page.currentShaft.RebuildComponentDict();
                            if (page.Pcanvas.ActualWidth == 0)
                            {
                                RoutedEventHandler handler = null;
                                handler = (s, e) =>
                                {
                                    page.currentShaft.DrawShaftWithComponents(
                                        page.annotationCanvas,
                                        page.Pcanvas
                                    );
                                    page.Pcanvas.Loaded -= handler; // 只执行一次
                                };
                                page.Pcanvas.Loaded += handler;
                            }
                            else
                            {
                                page.currentShaft.DrawShaftWithComponents(
                                    page.annotationCanvas,
                                    page.Pcanvas
                                );
                            }
                        }
                        else
                        {
                            MessageBox.Show("读取到的文件中没有 ShaftData，无法恢复曲轴。", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                        // 恢复页面数据字典
                        DisplayLoadedData(savedData.PageDictData);
                        var sys = mRVElementSet.mCrankShaftSystem;

                        // 绑定所有的圆锥滚子轴承
                        var tapBearings = page.currentShaft.Bearings.Where(b => b.BearingType == "圆锥滚子轴承").OrderBy(b => b.BaseOffsetX).ToList();
                        var tapered0 = sys.mBearingList.FirstOrDefault(b => b.mTreeViewItem.Header.ToString() == "左侧圆锥滚子轴承");
                        if (tapered0 != null && tapBearings.Count > 0) tapered0.CanvasGraphic = tapBearings[0];

                        var tapered1 = sys.mBearingList.FirstOrDefault(b => b.mTreeViewItem.Header.ToString() == "右侧圆锥滚子轴承");
                        if (tapered1 != null && tapBearings.Count > 1) tapered1.CanvasGraphic = tapBearings[1];

                        // 绑定所有的摆线轮
                        var cycWheels = page.currentShaft.CycloidWheels.OrderBy(c => c.BaseOffsetX).ToList();
                        var logicCyc0 = sys.mCrankShaftSystemCycloidPinionList.FirstOrDefault(c => c.mTreeViewItem.Header.ToString() == "左侧摆线轮");
                        if (logicCyc0 != null && cycWheels.Count > 0) logicCyc0.CanvasGraphic = cycWheels[0];

                        var logicCyc1 = sys.mCrankShaftSystemCycloidPinionList.FirstOrDefault(c => c.mTreeViewItem.Header.ToString() == "右侧摆线轮");
                        if (logicCyc1 != null && cycWheels.Count > 1) logicCyc1.CanvasGraphic = cycWheels[1];

                        // 绑定所有的滚针轴承
                        var needleBearings = page.currentShaft.Bearings.Where(b => b.BearingType == "滚针轴承").OrderBy(b => b.BaseOffsetX).ToList();
                        var brg0 = sys.mCrankShaftSystemSlewingBearingList.FirstOrDefault(b => b.mTreeViewItem.Header.ToString() == "左侧摆线轮滚针轴承");
                        if (brg0 != null && needleBearings.Count > 0) brg0.CanvasGraphic = needleBearings[0];

                        var brg1 = sys.mCrankShaftSystemSlewingBearingList.FirstOrDefault(b => b.mTreeViewItem.Header.ToString() == "右侧摆线轮滚针轴承");
                        if (brg1 != null && needleBearings.Count > 1) brg1.CanvasGraphic = needleBearings[1];

                        // 绑定齿轮(如有行星轮)
                        var gears = page.currentShaft.Gears.OrderBy(g => g.BaseOffsetX).ToList();
                        var logicGear = sys.mCrankShaftSystemGearList.FirstOrDefault(g => g.mTreeViewItem.Header.ToString() == "曲轴行星轮");
                        if (logicGear != null && gears.Count > 0) logicGear.CanvasGraphic = gears[0];
                    }
                    else
                    {
                        MessageBox.Show("未能读取文件或文件格式不正确。", "读取失败", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"读取失败：{ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void DisplayLoadedData(Dictionary<string, Dictionary<string, double>> bigDict)
        {
            // 1. 先进行一次性的树视图节点恢复（移出 foreach 循环，只执行一次）
            bool needsTreeRestore = bigDict.ContainsKey("左侧圆锥滚子轴承") || bigDict.ContainsKey("左侧摆线轮") || bigDict.ContainsKey("曲轴行星轮");
            if (needsTreeRestore)
            {
                var sys = mRVElementSet.mCrankShaftSystem;
                var treeViewItem = sys.mTreeViewItem;

                // --- 恢复圆锥滚子轴承 ---
                var bearingList = sys.mBearingList;
                CTANBearing tapered0 = bearingList.FirstOrDefault(b => b?.mTreeViewItem?.Header?.ToString() == "左侧圆锥滚子轴承");
                CTANBearing tapered1 = bearingList.FirstOrDefault(b => b?.mTreeViewItem?.Header?.ToString() == "右侧圆锥滚子轴承");

                if (tapered0 == null)
                {
                    tapered0 = new CTANBearing();
                    tapered0.mTreeViewItem.Header = "左侧圆锥滚子轴承";
                    tapered0.mTreeViewItem.Name = "左侧圆锥滚子轴承";
                    tapered0.mBelongedSet = mRVElementSet;
                    MainWindow.sPageIDDict[tapered0.mID] = tapered0;
                    bearingList.Add(tapered0);
                    treeViewItem.Items.Add(tapered0.mTreeViewItem);
                    sys.mBearingCount++;
                }
                if (tapered1 == null)
                {
                    tapered1 = new CTANBearing();
                    tapered1.mTreeViewItem.Header = "右侧圆锥滚子轴承";
                    tapered1.mTreeViewItem.Name = "右侧圆锥滚子轴承";
                    tapered1.mBelongedSet = mRVElementSet;
                    MainWindow.sPageIDDict[tapered1.mID] = tapered1;
                    bearingList.Add(tapered1);
                    treeViewItem.Items.Add(tapered1.mTreeViewItem);
                    sys.mBearingCount++;
                }

                // --- 恢复齿轮 ---
                var crankShaftSystemGearList = sys.mCrankShaftSystemGearList;
                CTANInvGear gearItem = crankShaftSystemGearList.FirstOrDefault(b => b?.mTreeViewItem?.Header?.ToString() == "曲轴行星轮");
                if (gearItem == null)
                {
                    gearItem = new CTANInvGear();
                    gearItem.mTreeViewItem.Header = "曲轴行星轮";
                    gearItem.mTreeViewItem.Name = "曲轴行星轮";
                    MainWindow.sPageIDDict[gearItem.mID] = gearItem;

                    // 【关键修复】：必须绑定主界面的UI和数据字典
                    var invPlanetaryGear = mRVElementSet.mInvGearPair.mGears[1];
                    gearItem.mInputPageGrid = invPlanetaryGear.mInputPageGrid;
                    gearItem.mDataStorageDict = invPlanetaryGear.mDataStorageDict;

                    crankShaftSystemGearList.Add(gearItem);
                    treeViewItem.Items.Add(gearItem.mTreeViewItem);
                }

                // --- 恢复摆线轮及滚针轴承 ---
                var crankShaftSystemCycloidList = sys.mCrankShaftSystemCycloidPinionList;
                var crankShaftSystemCycloidWheelBearingsList = sys.mCrankShaftSystemSlewingBearingList;
                var cycloidList = mRVElementSet.mCycloidGearPair;
                var cycloidWheelBearingsList = mRVElementSet.mCycloidGearPair.mSlewingBearingList;

                CTANCycloidPinion cycloidWheel0Item = crankShaftSystemCycloidList.FirstOrDefault(b => b?.mTreeViewItem?.Header?.ToString() == "左侧摆线轮");
                CTANCycloidPinion cycloidWheel1Item = crankShaftSystemCycloidList.FirstOrDefault(b => b?.mTreeViewItem?.Header?.ToString() == "右侧摆线轮");

                if (cycloidWheel0Item == null)
                {
                    cycloidWheel0Item = new CTANCycloidPinion();
                    cycloidWheel0Item.mTreeViewItem.Header = "左侧摆线轮";
                    cycloidWheel0Item.mTreeViewItem.Name = "左侧摆线轮";
                    MainWindow.sPageIDDict[cycloidWheel0Item.mID] = cycloidWheel0Item;

                    // 【关键修复】：必须绑定主界面的UI和数据字典
                    cycloidWheel0Item.mInputPageGrid = cycloidList.mCycloidPinionList[0].mInputPageGrid;
                    cycloidWheel0Item.mDataStorageDict = cycloidList.mCycloidPinionList[0].mDataStorageDict;

                    crankShaftSystemCycloidList.Add(cycloidWheel0Item); // 删除了原来重复添加的一行
                    treeViewItem.Items.Add(cycloidWheel0Item.mTreeViewItem);

                    CTANSlewingBearing cycloidWheel0BearingItem = new CTANSlewingBearing();
                    cycloidWheel0BearingItem.mTreeViewItem.Header = "左侧摆线轮滚针轴承";
                    cycloidWheel0BearingItem.mTreeViewItem.Name = "左侧摆线轮滚针轴承";
                    cycloidWheel0BearingItem.mBelongedSet = mRVElementSet;
                    MainWindow.sPageIDDict[cycloidWheel0BearingItem.mID] = cycloidWheel0BearingItem;

                    // 【关键修复】：必须绑定主界面的UI和数据字典
                    cycloidWheel0BearingItem.mInputPageGrid = cycloidWheelBearingsList[0].mInputPageGrid;
                    cycloidWheel0BearingItem.mDataStorageDict = cycloidWheelBearingsList[0].mDataStorageDict;

                    crankShaftSystemCycloidWheelBearingsList.Add(cycloidWheel0BearingItem);
                    cycloidWheel0Item.mTreeViewItem.Items.Add(cycloidWheel0BearingItem.mTreeViewItem);
                }
                if (cycloidWheel1Item == null)
                {
                    cycloidWheel1Item = new CTANCycloidPinion();
                    cycloidWheel1Item.mTreeViewItem.Header = "右侧摆线轮";
                    cycloidWheel1Item.mTreeViewItem.Name = "右侧摆线轮";
                    MainWindow.sPageIDDict[cycloidWheel1Item.mID] = cycloidWheel1Item;

                    // 【关键修复】：必须绑定主界面的UI和数据字典
                    cycloidWheel1Item.mInputPageGrid = cycloidList.mCycloidPinionList[1].mInputPageGrid;
                    cycloidWheel1Item.mDataStorageDict = cycloidList.mCycloidPinionList[1].mDataStorageDict;

                    crankShaftSystemCycloidList.Add(cycloidWheel1Item); // 删除了原来重复添加的一行
                    treeViewItem.Items.Add(cycloidWheel1Item.mTreeViewItem);

                    CTANSlewingBearing cycloidWheel1BearingItem = new CTANSlewingBearing();
                    cycloidWheel1BearingItem.mTreeViewItem.Header = "右侧摆线轮滚针轴承";
                    cycloidWheel1BearingItem.mTreeViewItem.Name = "右侧摆线轮滚针轴承";
                    cycloidWheel1BearingItem.mBelongedSet = mRVElementSet;
                    MainWindow.sPageIDDict[cycloidWheel1BearingItem.mID] = cycloidWheel1BearingItem;

                    // 【关键修复】：必须绑定主界面的UI和数据字典
                    cycloidWheel1BearingItem.mInputPageGrid = cycloidWheelBearingsList[1].mInputPageGrid;
                    cycloidWheel1BearingItem.mDataStorageDict = cycloidWheelBearingsList[1].mDataStorageDict;

                    crankShaftSystemCycloidWheelBearingsList.Add(cycloidWheel1BearingItem);
                    cycloidWheel1Item.mTreeViewItem.Items.Add(cycloidWheel1BearingItem.mTreeViewItem);
                }

                // --- 绑定前端图形 ---
                var currentShaft = sys.mCrankShaft.mShaftDesignPage.currentShaft;
                if (currentShaft != null)
                {
                    var tapBearings = currentShaft.Bearings.Where(b => b.BearingType == "圆锥滚子轴承").OrderBy(b => b.BaseOffsetX).ToList();
                    if (tapered0 != null && tapBearings.Count > 0) tapered0.CanvasGraphic = tapBearings[0];
                    if (tapered1 != null && tapBearings.Count > 1) tapered1.CanvasGraphic = tapBearings[1];

                    var needleBearings = currentShaft.Bearings.Where(b => b.BearingType == "滚针轴承").OrderBy(b => b.BaseOffsetX).ToList();
                    var brg0 = crankShaftSystemCycloidWheelBearingsList.FirstOrDefault(b => b?.mTreeViewItem?.Header?.ToString() == "左侧摆线轮滚针轴承");
                    if (brg0 != null && needleBearings.Count > 0) brg0.CanvasGraphic = needleBearings[0];

                    var brg1 = crankShaftSystemCycloidWheelBearingsList.FirstOrDefault(b => b?.mTreeViewItem?.Header?.ToString() == "右侧摆线轮滚针轴承");
                    if (brg1 != null && needleBearings.Count > 1) brg1.CanvasGraphic = needleBearings[1];

                    var cycloids = currentShaft.CycloidWheels.OrderBy(c => c.BaseOffsetX).ToList();
                    if (cycloidWheel0Item != null && cycloids.Count > 0) cycloidWheel0Item.CanvasGraphic = cycloids[0];
                    if (cycloidWheel1Item != null && cycloids.Count > 1) cycloidWheel1Item.CanvasGraphic = cycloids[1];

                    var gearsList = currentShaft.Gears.OrderBy(g => g.BaseOffsetX).ToList();
                    if (gearItem != null && gearsList.Count > 0) gearItem.CanvasGraphic = gearsList[0];
                }
            }

            // 2. 数据节点已全部恢复，统一且唯一的执行一次数据字典的注入
            foreach (var kv in bigDict)
            {
                string pageName = kv.Key;
                var dataDict = kv.Value;

                var pageObj = MainWindow.sPageIDDict.Values.FirstOrDefault(p =>
                    p?.GetDataKey() == pageName || p?.mTreeViewItem?.Header?.ToString() == pageName);
                if (pageObj != null)
                {
                    foreach (var dkv in dataDict)
                    {
                        if (pageObj.mDataStorageDict.ContainsKey(dkv.Key))
                            pageObj.mDataStorageDict[dkv.Key].Value = dkv.Value;
                        else
                            pageObj.mDataStorageDict.Add(dkv.Key, new CTANDouble() { Value = dkv.Value });
                    }
                }
            }
        }
        private void Help_Click(object sender, RoutedEventArgs e)
        {
            UIHelpWindow uIHelpWindow = new UIHelpWindow();
            uIHelpWindow.ShowDialog();
        }
        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            Window.GetWindow((Button)sender)?.Close();//点击按钮后就关闭窗口
        }
        //推荐功能设置的抽屉的实现
        private void UIZHOURecommend_Toggle_Checked(object sender, RoutedEventArgs e)
        {
            // 如果 DrawerContentHost 已经有内容，可以直接返回或刷新
            if (DrawerContentHost.Content != null)
            {
                DrawerTopInContainer.IsOpen = true;
                return;
            }

            // 创建 SetWindow（注意：不要调用 ShowDialog）
            var setWindow = mRVElementSet.mSetGearContentDrawer;
            // 注入关闭 Drawer 的动作
            setWindow.CloseHostAction = () => {
                DrawerTopInContainer.IsOpen = false; 
                // 如果需要清空内容： DrawerContentHost.Content = null;
            };
            // 将 SetWindow 的 mInputPageGrid 放到 Drawer 中显示
            DrawerContentHost.Content = setWindow.mInputPageGrid;

            // 打开 Drawer（ToggleButton 也会同步）
            DrawerTopInContainer.IsOpen = true;
        }
        // 取消时关闭 Drawer（可选）
        private void UIZHOURecommend_Toggle_Unchecked(object sender, RoutedEventArgs e)
        {
            DrawerTopInContainer.IsOpen = false;
            // 如果你想在关闭时清空内容，可以取消下面这行注释：
            //DrawerContentHost.Content = null;
        }
        private void ShaftButton_Checked(object sender, RoutedEventArgs e)
        {
            // 如果 DrawerContentHost 已经有内容，可以直接返回或刷新
            if (DrawerContentHostShaft.Content != null)
            {
                DrawerTopInContainerShaft.IsOpen = true;
                return;
            }

            // 创建 SetWindow
            var setWindow = mRVElementSet.mSetShaftContentDrawer;
            // 注入关闭 Drawer 的动作
            setWindow.CloseHostActionShaft = () => {
                DrawerTopInContainerShaft.IsOpen = false;
                // 如果需要清空内容： DrawerContentHost.Content = null;
            };
            // 将 SetWindow 的 mInputPageGrid 放到 Drawer 中显示
            DrawerContentHostShaft.Content = setWindow.mInputPageGrid;

            // 打开 Drawer（ToggleButton 也会同步）
            DrawerTopInContainerShaft.IsOpen = true;
        }
        private void ShaftButton_Unchecked(object sender, RoutedEventArgs e)
        {
            DrawerTopInContainerShaft.IsOpen = false;
        }

        // 切换到内置 AI 助手页面。
        private void AIButton_Click(object sender, RoutedEventArgs e)
        {
            mContentTab.SelectedItem = mAITab;
        }


    }
}
