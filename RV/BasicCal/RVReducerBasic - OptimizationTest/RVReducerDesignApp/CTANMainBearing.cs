using Mysqlx.Notice;
using Mysqlx.Session;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using static iTextSharp.awt.geom.Point2D;
using Frame = System.Windows.Controls.Frame;

namespace RVReducerDesignApp
{
    public class CTANMainBearing : CTANBasicContentPageObj
    {
        [DllImport(@"RVReducerBasic.dll", EntryPoint = "RecommondMainBearing", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern int RecommondMainBearing(ref double Indata, ref double OutDatan);

        // 主轴承自己的轴承列表
        public List<CTANBearing> mBearingList = new List<CTANBearing>();
        private BearingMenuHelper mBearingMenuHelper;
        public int mBearingCount
        {
            get => mBearingMenuHelper?.BearingCount ?? 0;
            set { if (mBearingMenuHelper != null) mBearingMenuHelper.BearingCount = value; }
        }

        public CTANRVSet mBelongedSet;//所属的RV减速器
        private UIMainBearing mUIMainBearing = new UIMainBearing();
        private Frame mFrame = new Frame();
        private CTANButton mButton = new CTANButton("主轴承计算");

        private static string[] mMainBearingInputParaText = new string[]
        {
            "Fx","N",
            "Fy","N",
            "Fz","N",
            "Mx","N·m",
            "My","N·m",
            "Mz","N·m",
            "作用点的Z轴坐标","mm",
            "轴承转速","转/分钟",
            "主轴许用弯曲正应力","Mpa",
            "主轴许用剪应力","Mpa",
            "轴承设计寿命", "小时" ,
            "主轴承类型","",
        };
        private double[] mBearingDrawPara = new double [30];
        public CTANMainBearing() 
        {
            mTreeViewItem.Name = "主轴";
            mTreeViewItem.Header = "主轴";
            SetInputPage();
            SetOutputPage();

            // 使用帮助类创建菜单
            mBearingMenuHelper = new BearingMenuHelper(mBearingList, mTreeViewItem);
            mBearingMenuHelper.SetupContextMenu();
        }
        private void SetInputPage()
        {           
            mInputGridList = new CTANGridWithControl[] { new CTANGridWithControl() };
            mDataStorageDict.SetDataStorageDictionary(0, 2, mMainBearingInputParaText.Count() / 2, mMainBearingInputParaText);
            mInputPageGrid.SetGridRowAndColumn(2, 1);
            mInputGridList[0].SetGridRowAndColumn(mMainBearingInputParaText.Count() / 2 + 1, 3);
            mInputGridList[0].SetGridControlByCreateDisplayTextBox(0, 0, mHeadText.Length, mHeadText);
            mInputGridList[0].SetGridControlByCreateDisplayTextBox(1, 0, 2, mMainBearingInputParaText);
            mInputGridList[0].SetGridControlByCreateInputTextBox(1, mMainBearingInputParaText.Count() / 2 - 1, 2, 1);
            mInputGridList[0].SetGridControlByCreateComboBox("主轴承类型", 2, [0, 1], ["圆锥滚子轴承", "角接触球轴承"]);
            mInputGridList[0].BingdingGridFromDict(1, mMainBearingInputParaText.Count() / 2, 2, mDataStorageDict);
            mInputGridList[0].SetGroupInGrid(0, 0, "输入参数", mInputPageGrid);

            //计算按钮所在网格
            mButton.AddClickEvent(CalculateMainBearingBtn_Clicked);
            CTANGridWithControl mButtonGrid = new CTANGridWithControl();
            mButtonGrid.SetGridRowAndColumn(1, 5);
            mButtonGrid.AssignControlToGrid(0, 2, mButton);
            mInputPageGrid.AssignControlToGrid(1, 0, mButtonGrid);

            //mButton.AddClickEvent(CalculateMainBearingBtn_Clicked);
            //CTANGridWithControl tmpInputMainBearingGrid = new CTANGridWithControl();
            //tmpInputMainBearingGrid.SetGridRowAndColumn(1, 5);
            //tmpInputMainBearingGrid.AssignControlToGrid(0, 2, mButton);
            //mInputPageGrid.Children.Add(tmpInputMainBearingGrid);
            testPara();
        }
        private void testPara()
        {
            mDataStorageDict["Fx"].Value = 1000;
            mDataStorageDict["Fy"].Value = 1000;
            mDataStorageDict["Fz"].Value = 1000;
            mDataStorageDict["Mx"].Value = 1000;
            mDataStorageDict["My"].Value = 1000;
            mDataStorageDict["Mz"].Value = 1000;
            mDataStorageDict["作用点的Z轴坐标"].Value = 10;          
        }
        private void CalculateMainBearingBtn_Clicked(object sender, RoutedEventArgs e)
        {
            double[] inForceVector = new double[12] { mDataStorageDict["Fx"].Value, mDataStorageDict["Fy"].Value, mDataStorageDict["Fz"].Value, 
                                                     mDataStorageDict["Mx"].Value, mDataStorageDict["My"].Value, mDataStorageDict["Mz"].Value, 
                                                     mDataStorageDict["作用点的Z轴坐标"].Value,
                                                     mDataStorageDict["主轴承类型"].Value,mDataStorageDict["轴承转速"].Value,
                                                     mDataStorageDict["主轴许用弯曲正应力"].Value,mDataStorageDict["主轴许用剪应力"].Value,mDataStorageDict["轴承设计寿命"].Value,

            }; 
            double[] OutData = new double[mBearingDrawPara.Length];
            if(mDataStorageDict["轴承转速"].Value == 0)
            {
                MessageBox.Show("请输入轴承转速");
                return;
            }
            RecommondMainBearing(ref inForceVector[0], ref OutData[0]);
            if (OutData[0] == -1 && OutData[1] == -1)
            {
                MessageBox.Show("没有找到合适的轴承，请检查");
            }

            for (int i=0; i< OutData.Length; i++)
            {
                mBearingDrawPara[i] = OutData[i];
            }
            mUIMainBearing.DrawMainBearingPage(mBearingDrawPara, inForceVector[6]); //绘制逻辑
            //树视图中添加轴承
            mBearingMenuHelper.AddBearingsList(mBearingDrawPara);
        }
        private void SetOutputPage()
        {
            mFrame.Content = mUIMainBearing;
            mOutputPageGrid.Children.Add(mFrame);
        }
    }
}
