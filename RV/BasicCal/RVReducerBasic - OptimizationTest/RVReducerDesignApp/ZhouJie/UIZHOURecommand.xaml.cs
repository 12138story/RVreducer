using Microsoft.Win32;
using Mysqlx.Session;
using RVReducerDesignApp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TrayNotify;
namespace RVReducerDesignApp
{
    /// <summary>
    /// UIZHOURecommand.xaml 的交互逻辑
    /// </summary>
    //public partial class UIZHOURecommand : Window
    //{
    //    public SetGearContentDrawer mSetWindow = new SetGearContentDrawer();

    //    public UIZHOURecommand()
    //    {
    //        InitializeComponent();
    //        this.Content = mSetWindow.mInputPageGrid;    //显示UI
    //    }
    //}
    public class SetGearContentDrawer : CTANBasicContentPageObj
    {
        [DllImport(@"RVReducerBasic.dll", EntryPoint = "RecommondPara", CallingConvention = CallingConvention.Cdecl)]
        public static extern void RecommondPara(ref double InInputData, ref double OutputData);
        private CTANButton mButton = new CTANButton("确定");
        public CTANRVSet mRVSet;
        public Action CloseHostAction; // 在构造或公开成员中设置
        static string[] mUIZHOUParaText = new string[]
        {
            "减速器固定部件","",
            "总传动比","",
            "减速器输出力矩", "牛米" ,
            "减速器输出转速", "转/分钟",
            "目标应力", "",
            "设计寿命", "h",
            "使用系数", "",
            "材料许用极限接触应力", "",
            "行星轮个数", "",

        };
        static string[] mCycloidWidthConstraintText = new string[]
        {
            "摆线轮最小齿宽","mm",
            "摆线轮最大齿宽","mm",
        };
        // 齿宽约束相关控件                                                           
        private CheckBox mEnableWidthConstraintCheckBox;
        private CTANGridWithControl mWidthConstraintGrid;
        private GroupBox mWidthConstraintGroupBox;

        public SetGearContentDrawer()
        {          
            mTreeViewItem.Name = "减速器齿轮推荐输入参数";
            mTreeViewItem.Header = "减速器齿轮推荐输入参数";//应对计算->保存无Header报错
            SetUI();
        }
        private void TmpTest()
        {
            mDataStorageDict["减速器固定部件"].Value = 1;
            mDataStorageDict["总传动比"].Value = 153;
            mDataStorageDict["减速器输出力矩"].Value = -784.0;
            mDataStorageDict["减速器输出转速"].Value = 15;
            mDataStorageDict["目标应力"].Value = 1000;
            mDataStorageDict["设计寿命"].Value = 50000;
            mDataStorageDict["使用系数"].Value = 1.2;
            mDataStorageDict["材料许用极限接触应力"].Value = 1500;
            mDataStorageDict["行星轮个数"].Value = 3;
            mDataStorageDict["摆线轮最小齿宽"].Value = 40;
            mDataStorageDict["摆线轮最大齿宽"].Value = 80;
        }
        public void SetUI()
        {
            mInputGridList = new CTANGridWithControl[] { new CTANGridWithControl() };
            mDataStorageDict.SetDataStorageDictionary(0, 2, mUIZHOUParaText.Count() / 2, mUIZHOUParaText);
            mInputPageGrid.SetGridRowAndColumn(3, 1);
            mInputGridList[0].SetGridRowAndColumn(mUIZHOUParaText.Count() / 2 + 1, 3);
            //设定表头
            mInputGridList[0].SetGridControlByCreateDisplayTextBox(0, 0, mHeadText.Length, mHeadText);
            //设定输入显示项
            mInputGridList[0].SetGridControlByCreateDisplayTextBox(1, 0, 2, mUIZHOUParaText);
            //设定输入文本框
            mInputGridList[0].SetGridControlByCreateInputTextBox(2, mUIZHOUParaText.Count() / 2 - 1, 2, 1);//要减一行，因为第一项是下拉项
            //设定下拉项
            mInputGridList[0].SetGridControlByCreateComboBox(1, 2, [0, 1], ["针齿壳固定", "曲轴固定"]);
            //进行数据绑定
            mInputGridList[0].BingdingGridFromDict(1, mUIZHOUParaText.Count() / 2, 2, mDataStorageDict);
            //封装到Group并放进PageGrid
            mInputGridList[0].SetGroupInGrid(0, 0, "输入参数（齿轮推荐）", mInputPageGrid);

            SetupWidthConstraintUI();
          
            //计算按钮所在网格
            CTANGridWithControl mButtonGrid = new CTANGridWithControl();
            mButtonGrid.SetGridRowAndColumn(1, 5);
            mButtonGrid.AssignControlToGrid(0, 2, mButton);
            mInputPageGrid.AssignControlToGrid(2, 0, mButtonGrid);

            mButton.AddClickEvent(Button_Click);//添加按钮点击事件
            TmpTest();
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            double[] Indata = new double[mUIZHOUParaText.Length / 2 + 3];
            double[] OutData = new double[2000];
            int index = 0;
            // 只遍历偶数索引的项（参数名）
            for (int i = 0; i < mUIZHOUParaText.Length; i += 2)
            {
                string key = mUIZHOUParaText[i];
                if (mDataStorageDict.ContainsKey(key)) // 检查是否为偶数
                {
                    Indata[index] = mDataStorageDict[key].Value;
                    index++;
                }
            }

            //处理摆线轮参数
            if (mEnableWidthConstraintCheckBox.IsChecked == true) Indata[Indata.Length - 1] = 1; 
            else Indata[Indata.Length - 1] = 0;
            Indata[Indata.Length - 2] = mDataStorageDict["摆线轮最大齿宽"].Value;
            Indata[Indata.Length - 3] = mDataStorageDict["摆线轮最小齿宽"].Value;
            if (mDataStorageDict["摆线轮最大齿宽"].Value < mDataStorageDict["摆线轮最小齿宽"].Value)
            {
                MessageBox.Show("请重新输入摆线轮齿宽约束条件，目前摆线轮最大齿宽必须大于摆线轮最小齿宽！");
                return;
            }

            RecommondPara(ref Indata[0], ref OutData[0]);

            UpdateRVSetParameters(OutData);// 更新UI显示
            // 推荐后跳转到渐开线齿轮副输入页面
            if (Application.Current.MainWindow is MainWindow mainWindow)
            {
                mainWindow.mContentTab.SelectedItem = mainWindow.mInputTab;
                mRVSet.mInvGearPair.mTreeViewItem.IsSelected = true;
            }
            CloseHostAction?.Invoke();
        }
 
        //更新RV参数
        private void UpdateRVSetParameters(double[] OutData)
        {
            mRVSet.mInvGearPair.mGears[0].mDataStorageDict["法向模数"].OutValue = OutData[0];
            mRVSet.mInvGearPair.mGears[0].mDataStorageDict["压力角"].OutValue = OutData[1];
            mRVSet.mInvGearPair.mGears[0].mDataStorageDict["螺旋角"].OutValue = OutData[2];
            mRVSet.mInvGearPair.mGears[0].mDataStorageDict["中心距"].OutValue = OutData[3];
            mRVSet.mInvGearPair.mGears[0].mDataStorageDict["齿轮副类型"].OutValue = OutData[4];
            mRVSet.mInvGearPair.mGears[0].mDataStorageDict["行星轮个数"].OutValue = OutData[5];
            mRVSet.mInvGearPair.mGears[0].mDataStorageDict["驱动齿轮"].OutValue = OutData[6];
            mRVSet.mInvGearPair.mGears[0].mDataStorageDict["触点间隙[j_bn]"].OutValue = OutData[7];
            mRVSet.mInvGearPair.mTorque.mDataStorageDict["转速"].OutValue = OutData[8];
            int tmpUp = 9;
            int tmpDown = 31;
            for (int i = 0; i < 2; i++)
            {
                mRVSet.mInvGearPair.mGears[i].mDataStorageDict["齿数"].OutValue = OutData[i * tmpDown + tmpUp + 0];
                mRVSet.mInvGearPair.mGears[i].mDataStorageDict["旋向"].OutValue = OutData[i * tmpDown + tmpUp + 1];
                mRVSet.mInvGearPair.mGears[i].mDataStorageDict["设计齿宽"].OutValue = OutData[i * tmpDown + tmpUp + 2];
                mRVSet.mInvGearPair.mGears[i].mDataStorageDict["齿顶高系数"].OutValue = OutData[i * tmpDown + tmpUp + 3];
                mRVSet.mInvGearPair.mGears[i].mDataStorageDict["齿廓变位系数"].OutValue = OutData[i * tmpDown + tmpUp + 4];
                mRVSet.mInvGearPair.mGears[i].mDataStorageDict["顶隙系数"].OutValue = OutData[i * tmpDown + tmpUp + 5];
                mRVSet.mInvGearPair.mGears[i].mDataStorageDict["齿根圆角系数"].OutValue = OutData[i * tmpDown + tmpUp + 6];
                mRVSet.mInvGearPair.mGears[i].mDataStorageDict["挖根量"].OutValue = OutData[i * tmpDown + tmpUp + 7];
                mRVSet.mInvGearPair.mGears[i].mDataStorageDict["齿顶修形量"].OutValue = OutData[i * tmpDown + tmpUp + 8];
                mRVSet.mInvGearPair.mGears[i].mDataStorageDict["齿根修形量"].OutValue = OutData[i * tmpDown + tmpUp + 9];
                mRVSet.mInvGearPair.mGears[i].mDataStorageDict["是否使用滚动角"].OutValue = OutData[i * tmpDown + tmpUp + 10];
                mRVSet.mInvGearPair.mGears[i].mDataStorageDict["跨齿数"].OutValue = OutData[i * tmpDown + tmpUp + 11];
                mRVSet.mInvGearPair.mGears[i].mDataStorageDict["齿顶倒角量"].OutValue = OutData[i * tmpDown + tmpUp + 12];
                mRVSet.mInvGearPair.mGears[i].mDataStorageDict["轮幅外圆直径"].OutValue = OutData[i * tmpDown + tmpUp + 13];
                mRVSet.mInvGearPair.mGears[i].mDataStorageDict["齿轮齿顶高变化[k·mn]"].OutValue = OutData[i * tmpDown + tmpUp + 14];
                mRVSet.mInvGearPair.mGears[i].mDataStorageDict["齿厚的偏差上限[E_sns]"].OutValue = OutData[i * tmpDown + tmpUp + 15];
                mRVSet.mInvGearPair.mGears[i].mDataStorageDict["齿厚的偏差下限[E_sni]"].OutValue = OutData[i * tmpDown + tmpUp + 16];
                mRVSet.mInvGearPair.mGears[i].mDataStorageDict["齿顶圆容差上限"].OutValue = OutData[i * tmpDown + tmpUp + 17];
                mRVSet.mInvGearPair.mGears[i].mDataStorageDict["齿顶圆容差下限"].OutValue = OutData[i * tmpDown + tmpUp + 18];
                mRVSet.mInvGearPair.mGears[i].mDataStorageDict["最大机械加工余量[q_max]"].OutValue = OutData[i * tmpDown + tmpUp + 19];
                mRVSet.mInvGearPair.mGears[i].mDataStorageDict["最小机械加工余量[q_min]"].OutValue = OutData[i * tmpDown + tmpUp + 20];
                mRVSet.mInvGearPair.mGears[i].mDataStorageDict["Y圆直径[d_y]"].OutValue = OutData[i * tmpDown + tmpUp + 21];
                mRVSet.mInvGearPair.mGears[i].mDataStorageDict["刀具类型"].OutValue = OutData[i * tmpDown + tmpUp + 22];
                mRVSet.mInvGearPair.mGears[i].mDataStorageDict["齿条刀的齿顶高系数"].OutValue = OutData[i * tmpDown + tmpUp + 23];
                mRVSet.mInvGearPair.mGears[i].mDataStorageDict["齿条刀的齿顶圆半径系数"].OutValue = OutData[i * tmpDown + tmpUp + 24];
                mRVSet.mInvGearPair.mGears[i].mDataStorageDict["插齿刀齿数"].OutValue = OutData[i * tmpDown + tmpUp + 25];
                mRVSet.mInvGearPair.mGears[i].mDataStorageDict["插齿刀加工中心距"].OutValue = OutData[i * tmpDown + tmpUp + 26];
                mRVSet.mInvGearPair.mGears[i].mDataStorageDict["插齿刀具齿顶直径"].OutValue = OutData[i * tmpDown + tmpUp + 27];
                mRVSet.mInvGearPair.mGears[i].mDataStorageDict["插齿刀基圆直径"].OutValue = OutData[i * tmpDown + tmpUp + 28];
                mRVSet.mInvGearPair.mGears[i].mDataStorageDict["插齿刀齿顶形状直径"].OutValue = OutData[i * tmpDown + tmpUp + 29];
                mRVSet.mInvGearPair.mGears[i].mDataStorageDict["插齿刀具参数横向压力角[alpha_wt0]"].OutValue = OutData[i * tmpDown + tmpUp + 30];
            }
            //滚针
            tmpUp = 71;
            mRVSet.mCycloidGearPair.mGear.mDataStorageDict["齿数"].OutValue = OutData[tmpUp + 0];
            mRVSet.mCycloidGearPair.mGear.mDataStorageDict["滚针半径"].OutValue = OutData[tmpUp + 1];
            mRVSet.mCycloidGearPair.mGear.mDataStorageDict["设计齿宽"].OutValue = OutData[tmpUp + 2];
            mRVSet.mCycloidGearPair.mGear.mDataStorageDict["针轮分度圆半径"].OutValue = OutData[tmpUp + 3];
            mRVSet.mCycloidGearPair.mGear.mDataStorageDict["弹性模量"].OutValue = OutData[tmpUp + 4];
            mRVSet.mCycloidGearPair.mGear.mDataStorageDict["使用系数"].OutValue = OutData[tmpUp + 5];
            //摆线轮
            tmpUp = 77;
            tmpDown = 6;
            for (int i = 0; i < 2; i++)
            {
                mRVSet.mCycloidGearPair.mCycloidPinionList[i].mDataStorageDict["齿数"].OutValue = OutData[i * tmpDown + tmpUp + 0];
                mRVSet.mCycloidGearPair.mCycloidPinionList[i].mDataStorageDict["偏心距"].OutValue = OutData[i * tmpDown + tmpUp + 1];
                mRVSet.mCycloidGearPair.mCycloidPinionList[i].mDataStorageDict["设计齿宽"].OutValue = OutData[i * tmpDown + tmpUp + 2];
                mRVSet.mCycloidGearPair.mCycloidPinionList[i].mDataStorageDict["弹性模量"].OutValue = OutData[i * tmpDown + tmpUp + 3];
                mRVSet.mCycloidGearPair.mCycloidPinionList[i].mDataStorageDict["使用系数"].OutValue = OutData[i * tmpDown + tmpUp + 4];
                mRVSet.mCycloidGearPair.mCycloidPinionList[i].mDataStorageDict["摆线轮传递总力矩"].OutValue = OutData[i * tmpDown + tmpUp + 5];
            }
            //其他参数
            tmpUp = tmpUp + 2 * tmpDown;
            mRVSet.mInvGearPair.mTorque.mDataStorageDict["扭矩"].OutValue = OutData[tmpUp + 0];
            mRVSet.mInvGearPair.mTorque.mDataStorageDict["转速"].OutValue = OutData[tmpUp + 1];

        }
        private void SetupWidthConstraintUI()
        {
            mDataStorageDict.SetDataStorageDictionary(0, 2, mCycloidWidthConstraintText.Count() / 2, mCycloidWidthConstraintText);
            // 创建主容器Grid
            Grid constraintContainerGrid = new Grid();
            constraintContainerGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            constraintContainerGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            // ========== 复选框触发器 ==========
            mEnableWidthConstraintCheckBox = new CheckBox
            {
                Content = "启用摆线轮齿宽约束",
                IsChecked = false,
                Margin = new Thickness(10, 5, 10, 5),
                FontWeight = FontWeights.Bold,
                VerticalContentAlignment = VerticalAlignment.Center
            };
            mEnableWidthConstraintCheckBox.Checked += OnWidthConstraintCheckBoxChanged;
            mEnableWidthConstraintCheckBox.Unchecked += OnWidthConstraintCheckBoxChanged;

            Grid.SetRow(mEnableWidthConstraintCheckBox, 0);
            constraintContainerGrid.Children.Add(mEnableWidthConstraintCheckBox);
            // ========== 齿宽约束输入Grid ==========
            mWidthConstraintGrid = new CTANGridWithControl();
            mWidthConstraintGrid.SetGridRowAndColumn(mCycloidWidthConstraintText.Count() / 2 + 1, 3);
            // 设定表头
            mWidthConstraintGrid.SetGridControlByCreateDisplayTextBox(0, 0, mHeadText.Length, mHeadText);
            // 设定输入显示项
            mWidthConstraintGrid.SetGridControlByCreateDisplayTextBox(1, 0, 2, mCycloidWidthConstraintText);
            // 设定输入文本框
            mWidthConstraintGrid.SetGridControlByCreateInputTextBox(1, mCycloidWidthConstraintText.Count() / 2, 2, 1);
            // 进行数据绑定
            mWidthConstraintGrid.BingdingGridFromDict(1, mCycloidWidthConstraintText.Count() / 2, 2, mDataStorageDict);
            // 将约束Grid放入GroupBox
            mWidthConstraintGroupBox = new GroupBox
            {
                Header = "摆线轮齿宽约束参数",
                Content = mWidthConstraintGrid,
                Margin = new Thickness(5),
                Padding = new Thickness(5),
                IsEnabled = false  // 默认禁用
            };
            Grid.SetRow(mWidthConstraintGroupBox, 1);
            constraintContainerGrid.Children.Add(mWidthConstraintGroupBox);
            // 将整个约束区域放入主页面
            mInputPageGrid.AssignControlToGrid(1, 0, constraintContainerGrid);
            // 设置初始禁用状态的视觉效果
            UpdateWidthConstraintVisualState(false);
        }
        //复选框状态改变事件处理
        private void OnWidthConstraintCheckBoxChanged(object sender, RoutedEventArgs e)
        {
            bool isEnabled = mEnableWidthConstraintCheckBox.IsChecked == true;
            mWidthConstraintGroupBox.IsEnabled = isEnabled;
            UpdateWidthConstraintVisualState(isEnabled);
        }
        //更新齿宽约束区域的视觉状态
        private void UpdateWidthConstraintVisualState(bool isEnabled)
        {
            if (mWidthConstraintGroupBox != null)
            {
                // 设置透明度来显示启用/禁用状态
                mWidthConstraintGroupBox.Opacity = isEnabled ? 1.0 : 0.5;

                // 更新GroupBox标题颜色
                if (isEnabled)
                {
                    mWidthConstraintGroupBox.Foreground = new SolidColorBrush(Colors.Black);
                }
                else
                {
                    mWidthConstraintGroupBox.Foreground = new SolidColorBrush(Colors.Gray);
                }
            }
        }
    }
    public class SetShaftContentDrawer : CTANBasicContentPageObj
    {
        public CTANRVSet mRVSet;
        private CTANButton mButton = new CTANButton("确定");
        public Action CloseHostActionShaft; // 在构造或公开成员中设置
        static string[] mShaftContentDrawerParaText = new string[]
        {
            "许用弯曲正应力","Mpa",
            "许用剪应力","Mpa",
            "轴承设计寿命", "小时" ,
        };
        private void TmpTest()
        {
            mDataStorageDict["许用弯曲正应力"].Value = 4;
            mDataStorageDict["许用剪应力"].Value = 12;
            mDataStorageDict["轴承设计寿命"].Value = 6700;
        }
        public SetShaftContentDrawer()
        {
            mTreeViewItem.Name = "减速器轴推荐输入参数";
            mTreeViewItem.Header = "减速器轴推荐输入参数";//应对计算->保存无Header报错
            mDataStorageDict.SetDataStorageDictionary(0, 2, mShaftContentDrawerParaText.Count() / 2, mShaftContentDrawerParaText);
            SetUI();
            TmpTest();
        }
        public void SetUI()
        {
            mInputGridList = new CTANGridWithControl[] { new CTANGridWithControl() };
            mInputPageGrid.SetGridRowAndColumn(2, 1);
            mInputGridList[0].SetGridRowAndColumn(mShaftContentDrawerParaText.Count() / 2 + 1, 3);
            //设定表头
            mInputGridList[0].SetGridControlByCreateDisplayTextBox(0, 0, mHeadText.Length, mHeadText);
            //设定输入显示项
            mInputGridList[0].SetGridControlByCreateDisplayTextBox(1, 0, 2, mShaftContentDrawerParaText);
            //设定输入文本框
            mInputGridList[0].SetGridControlByCreateInputTextBox(1, mShaftContentDrawerParaText.Count() / 2, 2, 1);
            //进行数据绑定
            mInputGridList[0].BingdingGridFromDict(1, mShaftContentDrawerParaText.Count() / 2, 2, mDataStorageDict);
            //封装到Group并放进PageGrid
            mInputGridList[0].SetGroupInGrid(0, 0, "输入参数（轴推荐）", mInputPageGrid);
            //计算按钮所在网格
            CTANGridWithControl mButtonGrid = new CTANGridWithControl();
            mButtonGrid.SetGridRowAndColumn(1, 5);
            mButtonGrid.AssignControlToGrid(0, 2, mButton);
            mInputPageGrid.AssignControlToGrid(1, 0, mButtonGrid);
            //添加按钮点击事件
            mButton.AddClickEvent(Button_Click);
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            mRVSet.mCrankShaftSystem.mDataStorageDict["许用弯曲正应力"] = mDataStorageDict["许用弯曲正应力"];
            mRVSet.mCrankShaftSystem.mDataStorageDict["许用剪应力"] = mDataStorageDict["许用剪应力"];
            mRVSet.mCrankShaftSystem.mDataStorageDict["轴承设计寿命"] = mDataStorageDict["轴承设计寿命"];
            mRVSet.mCrankShaftSystem.ShaftRecommondBut_Click(sender, e);
            // 装配后跳转到曲轴参数输入页面
            if (Application.Current.MainWindow is MainWindow mainWindow)
            {
                mainWindow.mContentTab.SelectedItem = mainWindow.mInputTab;
                mRVSet.mCrankShaftSystem.mCrankShaft.mTreeViewItem.IsSelected = true;
            }
            CloseHostActionShaft?.Invoke();
        }
    }
}
