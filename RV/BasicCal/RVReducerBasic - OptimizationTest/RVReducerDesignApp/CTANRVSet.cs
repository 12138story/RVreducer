using Mysqlx.Session;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace RVReducerDesignApp
{

    public class CTANRVSet : CTANBasicContentPageObj
    {
        [DllImport(@"RVReducerBasic.dll", EntryPoint = "CalTotalTorqueAndRPM", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        //[DllImport(@"InterpSurfTANDLL.dll", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        //[DllImport(@"libiomp5md.dll", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern void CalTotalTorqueAndRPM(ref double pInPara, ref double pOutPara);
        //public static string[] mHeadText = new string[] { "名称", "单位", "数值" };
        static string[] mInputParaText = new string[]{"减速器输出部件","",
            "减速器输出力矩", "牛米" ,
            "减速器输出转速", "转/分钟",
            "太阳轮齿数", "个",
            "行星轮齿数", "个",
            "摆线轮齿数", "个",
            "针轮齿数", "个"
        };
        static string[] mOutPutHeadName = new string[]{
            "太阳轮力矩", "牛米",
            "太阳轮转速", "转/分钟",
            "摆线轮力矩", "牛米",
            "摆线轮转速", "转/分钟"
        };
        public static int AssembleLeftBaseNotation = 1;
        public static int AssembleMiddleBaseNotation = 0;
        public static int AssembleRightBaseNotation = -1;
        int mRVSetDataColumnCount = 3;
        //数据分割文件,
        //0-单个零部件分割索引int序列起始点
        //1-inputdata数据按零部件子系统分割序列起始点
        //2-output数据按零部件子系统分割序列起始点
        public int[,] mDivisionIndexList = new int[,] { {0 , 0 , 0 },//第一行，总体数据
                                                      { 0 , 0, 0 }, //渐开线
                                                      { 0, 0, 0 },//摆线轮 
                                                      { 0, 0, 0 }, //轴承数据
                                                      { 0, 0, 0 },//轴起始
                                                      { 0,0,0},//总数据量
                                                       };
        public int[] mBearingComponentDivisionIndexList = new int[] { 0,//轴承数目
                                                            20,//单个轴承输入数据存储量
                                                            20,//单个轴承输出数据存储量
                                                            0,//轴承输入总数据存储量
                                                            0,//轴承输出总的数据量
                                                            10,//定位尺寸起始索引
                                                              };
        public static int[] mRVSetDivisionIndex = new int[]{
                                                            10,//RV总体参数输入存储量
                                                            10//RV总体参数输出存储量
                                                            };
        public int[] mPartsDivisionIndexList;

        //public TreeViewItem mRVTree = new TreeViewItem() { Header = "减速器" };
        //public CTANRVBasic mRVBasic = new CTANRVBasic();
        public CTANInvGearPair mInvGearPair=new CTANInvGearPair();
        public CTANCycloidGearPair mCycloidGearPair=new CTANCycloidGearPair();
        //public CTANControlGrid mIOputGrid = new CTANControlGrid();
        //基本输入网格
        //public CTANGridWithControl mInputGrid=new CTANGridWithControl();
        //TreeViewItem mInvGearTree = new TreeViewItem() { Header = "渐开线齿轮副" };
        public CTANCrankShaftSystem mCrankShaftSystem= new CTANCrankShaftSystem();
        public CTANMainBearing mMainBearing = new CTANMainBearing();

        public SetGearContentDrawer mSetGearContentDrawer = new SetGearContentDrawer();
        public SetShaftContentDrawer mSetShaftContentDrawer = new SetShaftContentDrawer();
        //整个减速器的数据存储
        public double[] mInputDoubleDataStorage; //用于DLL输入数据存储
        public double[] mOutputDoubleDataStorage;//用于DLL输出数据存储 
        public bool isRecommendShaft = false;
        ////测试页
        //CTANBasicContentPageObj mTestPage = new CTANBasicContentPageObj("测试页") ;


        public CTANRVSet ()
        {
            //生成树结构
            mTreeViewItem.Header = "减速器";
            mTreeViewItem.Items.Add(mCycloidGearPair.mTreeViewItem);
            mCycloidGearPair.mBelongedSet = this;
            mTreeViewItem.Items.Add(mInvGearPair.mTreeViewItem);
            mInvGearPair.mBelongedSet = this;
            mTreeViewItem.Items.Add(mCrankShaftSystem.mTreeViewItem);
            mCrankShaftSystem.mBelongedSet = this;
            mCrankShaftSystem.mCrankShaft.mBelongedSet= this;
            mTreeViewItem.Items.Add(mMainBearing.mTreeViewItem);
            mMainBearing.mBelongedSet = this;

            mSetGearContentDrawer.mRVSet = this;
            mSetShaftContentDrawer.mRVSet = this;

            SetDataAndPageLayout();
            SetCrossComponentBinding();
            //数据存储空间申请
            //mInputDoubleDataStorage = new double[mDivisionIndexList[1 + mRVSetDataColumnCount]];
            mDivisionIndexList[1,2] = mOutPutHeadName.Count() / 2;

            //生成初速存储数组
            mInputDoubleDataStorage = new double[2000];
            mOutputDoubleDataStorage = new double[1000];
            UpdataRVSetDataDivisionIndex();
            //mOutputDoubleDataStorage = new double[mOutPutHeadName.Count()/2];

            //UpdataRVSetDataDivisionIndex();
            ////测试页
            //SetTestTreeItem();
        }
        public void UpdataRVSetDataDivisionIndex()
        {
            //轴承初始测试,直接定义轴承
            mBearingComponentDivisionIndexList[0] = mCrankShaftSystem.mBearingList.Count;
            if (isRecommendShaft == true) //当推荐轴的时候，轴承数为4个
            {
                mBearingComponentDivisionIndexList[0] = 4;
                
            }                    
            //更新轴承总数据
            mBearingComponentDivisionIndexList[3] = mBearingComponentDivisionIndexList[0] * mBearingComponentDivisionIndexList[1];
            mBearingComponentDivisionIndexList[4] = mBearingComponentDivisionIndexList[0] * mBearingComponentDivisionIndexList[2];
            //分割序列
            mDivisionIndexList[0, 0] = 0;//总体起始
            mDivisionIndexList[1 , 0] = 0;//渐开线起始
            mDivisionIndexList[2 , 0] = CTANInvGearPair.mDivisionIndexList.Length;//摆线轮起始
            mDivisionIndexList[3 , 0] = mDivisionIndexList[2 , 0] + mCycloidGearPair.mDivisionIndexList.Length;//轴承起始
            mDivisionIndexList[4 , 0] = mDivisionIndexList[3 , 0] + mBearingComponentDivisionIndexList.Length;//轴起始
            //输入序列
            //mDivisionIndexList[1 + 1 * mRVSetDataColumnCount] = mInputDoubleDataStorage.Length;
            mDivisionIndexList[0, 1] = 0;//总体起始
            mDivisionIndexList[1, 1] = mDivisionIndexList[0, 1]+ mRVSetDivisionIndex[0];//渐开线起始
            mDivisionIndexList[2, 1] = mDivisionIndexList[1, 1] + CTANInvGearPair.mDivisionIndexList[0];//摆线轮起始
            mDivisionIndexList[3, 1] = mDivisionIndexList[2, 1] + mCycloidGearPair.mDivisionIndexList[0];//轴承起始
            mDivisionIndexList[4, 1] = mDivisionIndexList[3, 1] + mBearingComponentDivisionIndexList[3];//轴起始
            mDivisionIndexList[5, 1] = mDivisionIndexList[4, 1] + mCrankShaftSystem.mCrankShaft.GetInputDataLength();
            //输出序列
            //mDivisionIndexList[2 + 1 * mRVSetDataColumnCount] = mOutputDoubleDataStorage.Length;
            mDivisionIndexList[0, 2] = 0;//总体起始
            mDivisionIndexList[1, 2] = mDivisionIndexList[0, 2]+ mRVSetDivisionIndex[1];//渐开线起始
            mDivisionIndexList[2, 2] = mDivisionIndexList[1, 2] + CTANInvGearPair.mDivisionIndexList[1];//摆线轮起始
            mDivisionIndexList[3, 2] = mDivisionIndexList[2, 2] + mCycloidGearPair.mDivisionIndexList[1];//轴承起始
            mDivisionIndexList[4, 2] = mDivisionIndexList[3, 2] + mBearingComponentDivisionIndexList[4];//轴起始
            mDivisionIndexList[5, 2] = mDivisionIndexList[4, 2] + mCrankShaftSystem.mCrankShaft.GetOutputDataLength();           
            if (mInputDoubleDataStorage == null || mInputDoubleDataStorage.Length < mDivisionIndexList[5 , 1])
            {
                mInputDoubleDataStorage = new double[mDivisionIndexList[5 , 1]];
            }
            if (mOutputDoubleDataStorage == null || mOutputDoubleDataStorage.Length < mDivisionIndexList[5 , 2])
            {
                mOutputDoubleDataStorage = new double[mDivisionIndexList[5 , 2]];
            }
            //复制计算所需的分割序列
            int PartsDivisionIndexCount = CTANInvGearPair.mDivisionIndexList.Length 
                + mCycloidGearPair.mDivisionIndexList.Length
                + mBearingComponentDivisionIndexList.Length
                + (mCrankShaftSystem.mCrankShaft.GetDivisionDataLength());
            mPartsDivisionIndexList = new int[PartsDivisionIndexCount];
            CTANInvGearPair.mDivisionIndexList.CopyTo(mPartsDivisionIndexList, 0);
            mCycloidGearPair.mDivisionIndexList.CopyTo(mPartsDivisionIndexList, mDivisionIndexList[2, 0]);
            mBearingComponentDivisionIndexList.CopyTo(mPartsDivisionIndexList, mDivisionIndexList[3, 0]);
            if (mCrankShaftSystem.mCrankShaft.mIntDivisionIndexList != null)
            {
                mCrankShaftSystem.mCrankShaft.mIntDivisionIndexList.CopyTo(mPartsDivisionIndexList, mDivisionIndexList[4, 0]);
            }
        }
        public void SetDataAndPageLayout()
        {
            
            SetInputPage();
            SetOutputPage();


            //mOutputPageGrid.Children.Add(mOutputDataTable.mDataGrid);
            mDataStorageDict["减速器输出部件"].Value = 0;
            //mOutputDataTable["太阳轮力矩",2] = 100.0;
            //mDataStorageDict["太阳轮力矩"].Value = 250;
            //mPageGrid.Children.Add(mIOputGrid);
            mDataStorageDict["减速器输出力矩"].Value = -8820;
            mDataStorageDict["减速器输出转速"].Value = 15;
            //mDataStorageDict["太阳轮齿数"].Value = 10;
            //mDataStorageDict["行星轮齿数"].Value = 38;
            //mDataStorageDict["摆线轮齿数"].Value = 39;
            //mDataStorageDict["针轮齿数"].Value = 40;

        }
        public override void UpdateDllInputDoubleList(int InStartIndex, double[] IODoubleStorageData)
        {
            IODoubleStorageData[InStartIndex + 0] = mDataStorageDict["减速器输出部件"].Value;
            IODoubleStorageData[InStartIndex + 1] = mDataStorageDict["减速器输出力矩"].Value;
            IODoubleStorageData[InStartIndex + 2] = mDataStorageDict["减速器输出转速"].Value;
            IODoubleStorageData[InStartIndex + 3] = mDataStorageDict["太阳轮齿数"].Value;
            IODoubleStorageData[InStartIndex + 4] = mDataStorageDict["行星轮齿数"].Value;
            IODoubleStorageData[InStartIndex + 5] = mDataStorageDict["摆线轮齿数"].Value;
            IODoubleStorageData[InStartIndex + 6] = mDataStorageDict["针轮齿数"].Value;
            //IODoubleStorageData[InStartIndex + 6] = mDataStorageDict["曲轴相位角"].Value;
        }
        public override void UpdateDllOutputDoubleList(int InStartIndex, double[] IODoubleStorageData)
        {
            mDataStorageDict["太阳轮力矩"].OutValue = IODoubleStorageData[InStartIndex + 0];
            mDataStorageDict["太阳轮转速"].OutValue = IODoubleStorageData[InStartIndex + 1];
            mDataStorageDict["摆线轮力矩"].OutValue = IODoubleStorageData[InStartIndex + 2];
            mDataStorageDict["摆线轮转速"].OutValue = IODoubleStorageData[InStartIndex + 3];
        }
        public void UpdateDllInputDoubleList()
        {
            UpdateDllInputDoubleList(0, mInputDoubleDataStorage);
        }
        public void UpdateDllOutputDoubleList()
        {
            UpdateDllOutputDoubleList(0, mOutputDoubleDataStorage);
        }
        public void UpdateAllPartsDllInputData()
        {
            this.UpdateDllInputDoubleList();
            mInvGearPair.UpdateDllInputDoubleList();
            mCycloidGearPair.UpdateDllInputDoubleList();
            if (isRecommendShaft == false)//推荐轴的时候下面代码需注释。
            {
                for (int i = 0; i < mBearingComponentDivisionIndexList[0]; i++)
                {
                    mCrankShaftSystem.mBearingList[i].mBelongedSet = this;
                    mCrankShaftSystem.mBearingList[i].UpdateDllInputDoubleList(mDivisionIndexList[3, 1] + mBearingComponentDivisionIndexList[1] * i, mInputDoubleDataStorage);
                }
            }

            mCrankShaftSystem.mCrankShaft.UpdateDllInputDoubleList(mDivisionIndexList[4, 1], mInputDoubleDataStorage);
        }
        public void UpdateAllPartsDllOutputData()
        {
            UpdateDllOutputDoubleList();
            mInvGearPair.UpdateDllOutputDoubleList();
            mCycloidGearPair.UpdateDllOutputDoubleList();
        }
        public override void CalPageData()
        {
            UpdateDllInputDoubleList();
            CalTotalTorqueAndRPM(ref mInputDoubleDataStorage[0], ref mOutputDoubleDataStorage[0]);
            UpdateDllOutputDoubleList();
        }
        private void SetInputPage()
        {
            //生成Group网格
            mInputGridList = new CTANGridWithControl[] { new CTANGridWithControl() };
            //生成数据存储字典
            mDataStorageDict.SetDataStorageDictionary(0, 2, mInputParaText.Count() / 2, mInputParaText);
            //页面网格划分
            mInputPageGrid.SetGridRowAndColumn(1, 1);
            //设定网格数
            mInputGridList[0].SetGridRowAndColumn(mInputParaText.Count() / 2 + 1, 3);
            //设定表头
            mInputGridList[0].SetGridControlByCreateDisplayTextBox(0, 0, mHeadText.Length, mHeadText);
            //设定输入显示项
            mInputGridList[0].SetGridControlByCreateDisplayTextBox(1, 0, 2, mInputParaText);
            //设定输入文本框
            mInputGridList[0].SetGridControlByCreateInputTextBox(2, mInputParaText.Count() / 2 - 1, 2, 1);//要减一行，因为第一项是下拉项
            //设定下拉项
            mInputGridList[0].SetGridControlByCreateComboBox(1, 2, [0, 1], ["转臂输出", "针齿壳输出"]);
            //进行数据绑定
            mInputGridList[0].BingdingGridFromDict(1, mInputParaText.Count() / 2, 2, mDataStorageDict);

            //封装到Group并放进PageGrid
            mInputGridList[0].SetGroupInGrid(0, 0, "输入参数", mInputPageGrid);
            ////测试代码
            //CTANGridWithControl TestControl = new CTANGridWithControl();
            //TestControl.SetGridRowAndColumn(1, 1);
            ////TestControl.SetGridRowHeight(100);
            //CTANBearing TestBearing=new CTANBearing();
            //CTANGridWithControl.RemoveFromParent(TestBearing.mBearingInputGridTable[0][0].mGroupBox);
            //TestBearing.mBearingInputGridTable[0][0].SetGroupInGrid(0, 0, "", TestControl);
            ////mPageGrid = TestControl;
            //mPageGrid.Children.Add(TestControl);
            SetInputDataBinding();
        }
        private void SetInputDataBinding()
        {
            mInvGearPair.mGears[0].mDataStorageDict["齿数"].SetDataBingingToTANDouble(mDataStorageDict["太阳轮齿数"]);
            mInvGearPair.mGears[1].mDataStorageDict["齿数"].SetDataBingingToTANDouble(mDataStorageDict["行星轮齿数"]);
            mCycloidGearPair.mCycloidPinionList[1].mDataStorageDict["齿数"].SetDataBingingToTANDouble(mDataStorageDict["摆线轮齿数"]);
            mCycloidGearPair.mGear.mDataStorageDict["齿数"].SetDataBingingToTANDouble(mDataStorageDict["针轮齿数"]);
        }
        private void SetOutDataBinding()
        {
            mDataStorageDict["太阳轮转速"].SetDataBingingToTANDoubleOneWay(mInvGearPair.mTorque.mDataStorageDict["转速"]);
            mDataStorageDict["太阳轮力矩"].SetDataBingingToTANDoubleOneWay(mInvGearPair.mTorque.mDataStorageDict["扭矩"]);
            mDataStorageDict["摆线轮力矩"].SetDataBingingToTANDoubleOneWay(mCycloidGearPair.mCycloidPinionList[1].mDataStorageDict["摆线轮传递总力矩"]);

            mInvGearPair.mTorque.mDataStorageDict["转速"].SetDataBingingToTANDouble(mMainBearing.mDataStorageDict["轴承转速"]);
        }
        private void SetCrossComponentBinding()
        {
            // SetShaftContentDrawer <-> MainBearing 绑定
            mSetShaftContentDrawer.mDataStorageDict["许用弯曲正应力"].SetDataBingingToTANDouble(mMainBearing.mDataStorageDict["主轴许用弯曲正应力"]);
            mSetShaftContentDrawer.mDataStorageDict["许用剪应力"].SetDataBingingToTANDouble(mMainBearing.mDataStorageDict["主轴许用剪应力"]);
            mSetShaftContentDrawer.mDataStorageDict["轴承设计寿命"].SetDataBingingToTANDouble(mMainBearing.mDataStorageDict["轴承设计寿命"]);

            mSetGearContentDrawer.mDataStorageDict["减速器输出力矩"].SetDataBingingToTANDouble(mDataStorageDict["减速器输出力矩"]);
            mSetGearContentDrawer.mDataStorageDict["减速器输出转速"].SetDataBingingToTANDouble(mDataStorageDict["减速器输出转速"]);
        }
        private void SetOutputPage()
        {
            //设置输出页面
            mOutputPageGrid.SetGridRowAndColumn(2, 1);
            mOutputDataTable.SetGearStandardOutputTable(mHeadText, mOutPutHeadName, 2);
            mDataStorageDict.SetDataStorageDictionary(0, 2, mOutPutHeadName);
            BindingDictToOutputTable(2);
            mOutputPageGrid.Children.Add(mOutputDataTable.mDataGrid);

            SetOutDataBinding();
        }
        ////生成测试页面
        //private void SetTestTreeItem()
        //{
        //    mTreeViewItem.Items.Add(mTestPage.mTreeViewItem);
        //    mTestPage.mInputPageGrid.SetGridRowAndColumn(2, 1);
        //    //mTestPage.mPageGrid.Children.Add(mInvGearPair.mGears[0].mInputGridList[0].mGroupBox);
        //    //mTestPage.mPageGrid.Children.Add(mInvGearPair.mPageGrid);
        //    //mInvGearPair.mGears[0].mInputGridList[0].SetGroupInGrid(0, 0, "几何参数",mTestPage.mPageGrid);
        //    //mTestPage.mPageGrid.Children.Add(mInvGearPair.mGears[0].mInputGridList[0].mGroupBox);
        //}
        //public void TestFunc()
        //{
        //    TestDict.SetDataStorageDictionary(0, 2, mHeadName.Count() / 2, mHeadName);
        //    TestGrid.SetGridRowAndColumn(mHeadName.Count() / 2 + mHeadText.Count() / 2, 3);
        //    TestGrid.SetGridControlByCreateDisplayTextBox(0, 0, 2, mHeadText);
        //    TestGrid.SetGridControlByCreateDisplayTextBox(1, 0, 2, mHeadName);
        //    TestGrid.SetGridControlByCreateInputTextBox(2, mHeadName.Count() / 2-1, 2, 1);
        //    CTANComboBox tmpTestComboBox= new CTANComboBox();
        //    tmpTestComboBox.SetComboBoxWithKeyValue([0, 1], ["转臂输出", "针齿壳输出"]);
        //    //TestGrid.mControlTable[1][2]=tmpTestComboBox;
        //    TestGrid.SetGridControlByCreateComboBox(1, 2, [0, 1], ["转臂输出", "针齿壳输出"]);
        //    //TestDict.SetDataBindingWithTextBoxMatrix(1,
        //    //    mIOputGrid.mGrid.RowDefinitions.Count - 1,
        //    //    0,
        //    //    2,
        //    //    mIOputGrid.mTextBox);
        //    //CTANTextBox testBox = TestGrid.mControlTable[1][2];
        //    //CTANTextBox testBox2 = TestGrid.mControlTable[2][2];
        //    //testBox.Text = "";
        //    //TestGrid.AssignComboBoxToGrid(1, 2, tmpTestComboBox);
        //    //TestGrid.BingdingInputFromDict()
        //    TestGrid.BingdingInputControlFromDict(2, 2, TestDict);
        //    TestDict["减速器输出力矩"].Value = 10;
        //    //TestDict["减速器输出部件"].SetDataBingingFromTanObject(TestGrid.mControlTable[1][2]);
        //    //TestDict["减速器输出部件"].Value = 1;
        //    //TestDict["减速器输出部件"].SetDataBingingToTextBox(TestGrid.mControlTable[2][2]);

        //    //TestDict["减速器输出力矩"].SetDataBingingToTextBox(TestGrid.mControlTable[2][2]);
        //    //TestDict["减速器输出力矩"].SetDataBingingFromTextBox(TestGrid.mControlTable[3][2]);
        //    //TestDict["减速器输出力矩"].Value = 1;
        //}

    }
}
