using Mysqlx.Crud;
using Mysqlx.Session;
using RVReducerDesignApp.TANUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using WPFBearings;
using Microsoft.VisualBasic; // 用于 InputBox，项目需引用 Microsoft.VisualBasic
//曲轴
namespace RVReducerDesignApp
{

    public class CTANCrankShaft : CTANBasicContentPageObj
    {
        //参数输入页面
        public CTANGirderPage mShaftDesignPage = new CTANGirderPage();
        public Frame mFrameForShaftDesignPage = new Frame();

        //参数输出页面
        public UILIneChart mLineChartPage = new UILIneChart();
        public Frame mFrameForLIneChartPage = new Frame();
        //3D测试页面
        public Test3DPage m3DTestPage = new Test3DPage();
        public Frame mFrameForPage = new Frame();

        // 添加公共属性暴露目标Frame
        public Frame Target3DFrame => mFrameForPage;

        // 新增父级系统引用
        public CTANCrankShaftSystem ParentCrankShaftSystem { get; set; }
        public static string[] mInputParaText = new string[]{"曲轴数目","根"};
        private static string[] mCrankShaftParaText = new string[]{
            "偏心量", "mm",
            "相位角", "°",
        };

        //我的定义
        public CTANRVSet mBelongedSet;//所属的RV减速器
        public double[] mShaftDirect = new double[3] { 0, 0, 1 };
        public double[] mShaftMat = new double[2] { 2.1 * Math.Pow(10, 8), 0.3 };

        //分割数据，前10个用于存储基本数据
        //0-节点数，也用于输出计算结果，总计算结果数据=6*节点数，（最开始为各个节点位移）
        //1-IntShaftRCDataCountIndex-截面类型对应的数据长度起始索引,在mIntDivisionIndexList数组中
        //2-IntShaftTypeInIntStartIndex-轴各轴段截面类型起始索引,在mIntDivisionIndexList数组中
        //3-IntCoorStartInIntListIndex-节点坐标起始索引,在mInputDoubleDataStorage数组中
        //4-IntShaftDirectStartInIntListIndex-轴指向起始索引,在mInputDoubleDataStorage数组中
        //5-IntShaftMatStartInIntListIndex-截面材料参数索引，目前只定义一种材料,在mInputDoubleDataStorage数组中
        //6-IntSecRealConstStartInIntListIndex- 实际截面实常数起始索引,在mInputDoubleDataStorage数组中
        public int[] mIntDivisionIndexList;//分割数据，包含轴的截面类型数据,前面10个为基本指向数据，0-节点数，1-截面类型对应的数据长度起始索引，2-轴各轴段截面类型起始索引
        public double[] mInputDoubleDataStorage;

        public int IntShaftRCDataCountIndex = 1;//轴截面类型对应实常数数据量在mIntDivisionIndexList的起始索引所在偏移量。
        public int IntShaftTypeInIntStartIndex = 2;//实际轴截面类型列表在mIntDivisionIndexList的起始索引所在偏移量。
        public int IntCoorStartInIntListIndex = 3;//节点坐标起始索引,在int形数组
        public int IntShaftDirectStartInIntListIndex = 4;//轴指向起始索引
        public int IntShaftMatStartInIntListIndex = 5;//截面材料参数索引，目前只定义一种材料
        public int IntSecRealConstStartInIntListIndex = 6;//实际截面实常数起始索引

        //int IntOutputDataStartInIntListIndex = 7;//输出数据计数开始，输出数据
        //List<double> mNodeList = new List<double>();


        //存储截面类型和数据长度的数组
        //0-代表为输入极为实际常数，数据量4;
        //1-输入为圆，数据量1;
        //2-输入为偏心圆，数据量4;
        public static int[] mSecTypeToRCDataList = new int []{ 4,1,4};
        public CTANCrankShaft()
        {
            mTreeViewItem.Name = "曲轴";
            mTreeViewItem.Header = "曲轴";
            SetInputPage_ZhouJie();
            mDataStorageDict.SetDataStorageDictionary(0, 2, mInputParaText.Count() / 2, mInputParaText);
            mDataStorageDict.SetDataStorageDictionary(0, 2, mCrankShaftParaText.Count() / 2, mCrankShaftParaText);
            mDataStorageDict["曲轴数目"].Value = 3;         
        }
        //设定输入页面
        void SetInputPage()
        {
            //CTANBasicFunction.CreatObjectList(1, out mInputGridList);
            //mInputPageGrid.SetGridRowAndColumn(2, 1);
            //mInputGridList[0].SetGridRowAndColumn(1, 3);
            ////添加按钮
            //CTANButton tmpButton = new CTANButton("测试曲轴");
            //tmpButton.AddClickEvent(TestShaftButton_Click);
            //mInputGridList[0].AssignControlToGrid(0, 1, tmpButton);            
            //tmpButton = new CTANButton("设计曲轴");
            //tmpButton.AddClickEvent(DesignShaftButton_Click);
            //mInputGridList[0].AssignControlToGrid(0, 2, tmpButton);
            //mInputPageGrid.SetGridRowAndColumn(2, 0);
            //Frame tmpframe = new Frame();
            //tmpframe.Content = mShaftDesignPage;
            //mInputPageGrid.AssignControlToGrid(0, 0, tmpframe);
            //mInputPageGrid.AssignControlToGrid(1, 0, mInputGridList[0]);

        }
        void SetInputPage_ZhouJie()
        {
            mShaftDesignPage.ParentCrankShaft = this;
            mFrameForShaftDesignPage.Content = mShaftDesignPage;
            mInputPageGrid.Children.Add(mFrameForShaftDesignPage);

            mFrameForPage.Content = m3DTestPage;
            m3DPageGrid.Children.Add(mFrameForPage);       
            
            mFrameForLIneChartPage.Content = mLineChartPage;
            mOutputPageGrid.Children.Add(mFrameForLIneChartPage);
        }
        //将数据更新到mIntDivisionIndexList和mInputDoubleDataStorage
        public void UpdateClassInputData()
        {
            // 核心修改：为了保证底层应力计算正确，必须使用原始未分割的轴段（OriginalAxisSections）
            // 如果使用被齿轮或轴承切割并含有偏心段的 AxisSections，会导致底层节点数暴增且受力分析完全错乱！
            var sourceAxisSections = mShaftDesignPage.currentShaft.OriginalAxisSections;
            if (sourceAxisSections == null || sourceAxisSections.Count == 0)
            {
                // 做个后备处理，防止某些情况下OriginalAxisSections没数据
                sourceAxisSections = mShaftDesignPage.currentShaft.AxisSections;
            }

            //const int IntNodeCountStartInIntListIndex = 0;//mDivisionIndexList 序列，0-节点数起始
            //int IntCoorStartInIntListIndex = 1;//mDivisionIndexList 序列，1-节点坐标起始索引
            //int IntShaftDirectStartInIntListIndex = 2;//mDivisionIndexList 序列，2-轴指向起始索引
            //int IntShaftMatStartInIntListIndex = 3;//mDivisionIndexList 序列，3-截面材料参数索引，目前只定义一种材料
            //int IntShaftSecTypeStartInIntListIndex = 4;//mDivisionIndexList 序列，3-截面类型起始索引
            //int IntSecRealConstStartInIntListIndex = 5;//mDivisionIndexList 序列，4-截面实常数起始索引
            //List<double> testData= new List<double>();//轴的所有数据
            int tmpShaftSecCount = sourceAxisSections.Count();
            int tmpStartIndex = 0;
            //申请整形输入数组空间
            mIntDivisionIndexList = new int[10 + mSecTypeToRCDataList.Length + tmpShaftSecCount];
            //整形数据转换
            //写入节点数
            mIntDivisionIndexList[0] = tmpShaftSecCount + 1;//节点数           
            //写入截面类型对应的数据长度
            mIntDivisionIndexList[IntShaftRCDataCountIndex] = 10;//1-截面类型对应的数据长度起始索引
            tmpStartIndex = mIntDivisionIndexList[1];
            Array.Copy(mSecTypeToRCDataList, 0, mIntDivisionIndexList, tmpStartIndex, mSecTypeToRCDataList.Length);
            //写入实际轴的截面类型
            mIntDivisionIndexList[IntShaftTypeInIntStartIndex] = mIntDivisionIndexList[1] + mSecTypeToRCDataList.Length;//2-轴各轴段截面类型起始索引
            tmpStartIndex = mIntDivisionIndexList[2];
            for (int i = 0; i < tmpShaftSecCount; i++)
            {
                //mIntDivisionIndexList[tmpStartIndex + i] = sourceAxisSections[i].GetSectionType();
                
                mIntDivisionIndexList[tmpStartIndex + i] = 1;// 修复数组错位Bug：强制按照通轴(Type 1)计算，避免 C++ 误读 SectionType=2 导致读取越界乱码
            }
            //双精度数据转换
            //写入节点坐标
            tmpStartIndex = 0;
            mIntDivisionIndexList[IntCoorStartInIntListIndex] = tmpStartIndex;
            //写入轴的基础数据
            List<double> tmpData = new List<double>();//轴的所有数据临时存储
            tmpData.Add(0);//轴段起点偏移基准面的值
            tmpStartIndex++;
            for (int i = 0; i < tmpShaftSecCount; i++)
            {
                // 此处必须使用 sourceAxisSections
                tmpData.Add(sourceAxisSections[i].BaseWidth + sourceAxisSections[i].CurrentLength);
                tmpStartIndex++;
            }
            //写入轴向指向
            mIntDivisionIndexList[IntShaftDirectStartInIntListIndex] = tmpStartIndex;
            for (int i = 0; i < mShaftDirect.Length; i++)
            {
                tmpData.Add(mShaftDirect[i]);
                tmpStartIndex++;
            }
            //写入材料参数
            mIntDivisionIndexList[IntShaftMatStartInIntListIndex] = tmpStartIndex;
            for (int i = 0; i < mShaftMat.Length; i++)
            {
                tmpData.Add(mShaftMat[i]);
                tmpStartIndex++;
            }
            //写入截面实常数参数
            mIntDivisionIndexList[IntSecRealConstStartInIntListIndex] = tmpStartIndex;
            for (int i = 0; i < tmpShaftSecCount; i++)
            {
                // 此处必须使用 sourceAxisSections
                PushSecRCDataToList(sourceAxisSections[i], tmpData);
                tmpStartIndex++;
            }
            mInputDoubleDataStorage = tmpData.ToArray();
        }
        //各个数据长度
        public int GetInputDataLength()
        {
            return CTANBasicFunction.GetDataListLength(mInputDoubleDataStorage);
        }
        public int GetOutputDataLength()
        {
            if (mIntDivisionIndexList == null)
            {
                return 0;
            }
            else
            {
                return mIntDivisionIndexList[0] * 6;
            }
        }
        public int GetDivisionDataLength()
        {
            return CTANBasicFunction.GetDataListLength(mIntDivisionIndexList);
        }
        private void PushSecRCDataToList(DrawAxisDimension InAxisSec, List<double> OutListData)
        {
            // 无论实际轴是否有偏心，这里统一作为标准通轴(Type=1)传入1个外径参数，防止与 DLL 期望长度不匹配
            OutListData.Add(InAxisSec.BaseOuterDiameter);
            //int secType = InAxisSec.GetSectionType();
            //if (secType == 1)
            //{
            //    // 类型 1：普通圆，对应 1 个数据（外径）
            //    OutListData.Add(InAxisSec.BaseOuterDiameter);
            //}
            //else if (secType == 2)
            //{
            //    // 类型 2：偏心圆，必须严格对应 4 个数据！
            //    // 请根据你 C++ 底层 `DefineOrginGeometry` 的解析顺序传入，通常是外径、内径、偏心量、相位等
            //    OutListData.Add(InAxisSec.BaseOuterDiameter);
            //    OutListData.Add(InAxisSec.BaseInnerDiameter);
            //    OutListData.Add(InAxisSec.EccentricDiameter); // 偏心量
            //    OutListData.Add(InAxisSec.Phase);             // 相位角
            //}
            //else if (secType == 0)
            //{
            //    // 如果有类型 0，也要补全 4 个数据
            //    OutListData.Add(InAxisSec.BaseOuterDiameter);
            //    OutListData.Add(0);
            //    OutListData.Add(0);
            //    OutListData.Add(0);
            //}
        }
        public override void  UpdateDllInputDoubleList(int InStartIndex, double[] IODllInputData)
        {            
            mInputDoubleDataStorage.CopyTo(IODllInputData, InStartIndex);
        }
    }

    public class CTANCrankShaftSystem : CTANBasicContentPageObj
    {
        [DllImport(@"RVReducerBasic.dll", EntryPoint = "CalCrankShaft", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe void CalCrankShaft(ref int IOStartIndex, ref double pInputData, ref double pOutPara);

        public CTANCrankShaft mCrankShaft=new CTANCrankShaft();
        public List<CTANBearing> mBearingList=new List<CTANBearing>();
        public CTANRVSet mBelongedSet;//所属的RV减速器
                                      
        private BearingMenuHelper mBearingMenuHelper;// 使用帮助类管理轴承菜单
        // 通过帮助类访问轴承计数                                        
        public int mBearingCount
        {
            get => mBearingMenuHelper?.BearingCount ?? 0;
            set { if (mBearingMenuHelper != null) mBearingMenuHelper.BearingCount = value; }
        }
        //创建曲轴系下的组件列表
        public List<CTANCycloidPinion> mCrankShaftSystemCycloidPinionList = new List<CTANCycloidPinion>();
        public List<CTANSlewingBearing> mCrankShaftSystemSlewingBearingList = new List<CTANSlewingBearing>();
        public List<CTANInvGear> mCrankShaftSystemGearList = new List<CTANInvGear>();


        public Dictionary<int, CTANBasicContentPageObj> mComponentList= new Dictionary<int, CTANBasicContentPageObj>();
        [DllImport(@"RVReducerBasic.dll", EntryPoint = "CalCrankShaftSystem", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern void CalCrankShaftSystem(ref int InputIntIndexDividIndex, ref int InputDoubleIndexDividIndex, ref int InputIntData, ref double InputDoubleData, ref double OutputData);
        [DllImport(@"RVReducerBasic.dll", EntryPoint = "CalCrankShaftSystem2", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern void CalCrankShaftSystem2(ref int InputIntIndexDividIndex,  ref int InputIntData, ref double InputDoubleData, ref double OutputData);
        [DllImport(@"RVReducerBasic.dll", EntryPoint = "GetForceAndForceLocation", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern void GetForceAndForceLocation(ref int CoordinateNumber, ref double tmpForceLocation, ref double OutputData);
        [DllImport(@"RVReducerBasic.dll", EntryPoint = "RecommondShaftPara", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern void RecommondShaftPara(ref int InputIntIndexDividIndex, ref int InputIntData, ref double InputDoubleData, ref double OutputData, ref double OutData);
        private CTANButton mShaftRecommondButton = new CTANButton("轴推荐");
        static string[] mOutPutShaftRecommondParaText = new string[]{
            "轴直径", "",
            "轴长度", "毫米",
        };
        static string[] mInPutShaftRecommondParaText = new string[]{
            "许用弯曲正应力","Mpa",
            "许用剪应力","Mpa",
            "轴承设计寿命", "小时" ,
        };
        public CTANCrankShaftSystem() 
        {
            mTreeViewItem.Name = "曲轴系";
            mTreeViewItem.Header = "曲轴系";
            mTreeViewItem.Items.Add(mCrankShaft.mTreeViewItem);
            this.mTreeViewItem.ContextMenu= new ContextMenu();
            // 使用帮助类创建菜单
            mBearingMenuHelper = new BearingMenuHelper(mBearingList, mTreeViewItem);
            mBearingMenuHelper.SetupContextMenu();

            mCrankShaft.ParentCrankShaftSystem= this;

            SetInputPage();
            SetOutputPage();
        }

        //曲轴计算
        public override void CalPageData()
        {
            double[] tmpOutData = new double[200];          
            if (mCrankShaft.mShaftDesignPage.currentShaft == null)
            {
                MessageBox.Show("请先设计曲轴!");
                return;
            }
            mCrankShaft.UpdateClassInputData();
            mBelongedSet.UpdataRVSetDataDivisionIndex();
            mBelongedSet.UpdateAllPartsDllInputData();
            //mBelongedSet.mCycloidGearPair.CalPageData();
            CalCrankShaftSystem2(
                ref mBelongedSet.mDivisionIndexList[0,0],
                ref mBelongedSet.mPartsDivisionIndexList[0],
                ref mBelongedSet.mInputDoubleDataStorage[0],
                ref mBelongedSet.mOutputDoubleDataStorage[0]);
            mBelongedSet.UpdateAllPartsDllOutputData();

            PlottingAxialStresses();//绘制轴应力图(目前只针对通轴进行应力分析）
            // 可选钩子：设环境变量 RV_SNAPSHOT_DIR 后，每次完成曲轴系计算都导出一份「整机快照」，
            // 供 RvCalcCli 在无界面下复现同一算例（CalCrankShaftSystem2 / RecommondShaftPara）。
            Services.WholeMachineSnapshot.TryAutoExport("CalCrankShaftSystem2");
        }
        private void SetInputPage()
        {
            mDataStorageDict.SetDataStorageDictionary(0, 2, mInPutShaftRecommondParaText);
            //mInputPageGrid.SetGridRowAndColumn(1,1);
            //mShaftRecommondButton.AddClickEvent(ShaftRecommondBut_Click);
            //mInputPageGrid.Children.Add(mShaftRecommondButton);
        }
        private void SetOutputPage()
        {
            //设置输出页面
            mOutputPageGrid.SetGridRowAndColumn(2, 1);
            mOutputDataTable.SetGearStandardOutputTable(mHeadText, mOutPutShaftRecommondParaText, 2);
            mDataStorageDict.SetDataStorageDictionary(0, 2, mOutPutShaftRecommondParaText);
            BindingDictToOutputTable(2);
            mOutputPageGrid.Children.Add(mOutputDataTable.mDataGrid);
        }
        public void ShaftRecommondBut_Click(object sender, RoutedEventArgs e)
        {          
            mBelongedSet.isRecommendShaft = true;
            //校核轴是否满足要求！
            double[] tmpShaftData = new double[100];
            tmpShaftData[0] = 11111111111111;//初始直径
            tmpShaftData[1] = 22222222222222;//初始轴长
            tmpShaftData[2] = mDataStorageDict["许用弯曲正应力"].Value; //许用弯曲正应力；
            tmpShaftData[3] = mDataStorageDict["许用剪应力"].Value; //许用剪应力；
            tmpShaftData[4] = mDataStorageDict["轴承设计寿命"].Value;//轴承设计寿命

            UpdateShaftDiameterAndLength(tmpShaftData[0], tmpShaftData[1]);
            mBelongedSet.UpdataRVSetDataDivisionIndex();
            mBelongedSet.UpdateAllPartsDllInputData();

            RecommondShaftPara(ref mBelongedSet.mDivisionIndexList[0, 0],
                    ref mBelongedSet.mPartsDivisionIndexList[0],
                    ref mBelongedSet.mInputDoubleDataStorage[0],
                    ref mBelongedSet.mOutputDoubleDataStorage[0],
                    ref tmpShaftData[0]);
            // 可选钩子：设 RV_SNAPSHOT_DIR 后，把「DLL 实际收到的数组」原样导出（不复算），
            // 便于 RvCalcCli 在无界面下复现本次轴推荐。
            Services.WholeMachineSnapshot.TryAutoExport("RecommondShaftPara");

            mDataStorageDict["轴直径"].Value = tmpShaftData[0];//输出直径
            mDataStorageDict["轴长度"].Value = tmpShaftData[1];//输出长度

            mCrankShaft.mShaftDesignPage.Assembly(tmpShaftData);

            // 装配后跳转到曲轴参数输入页面
            if (Application.Current.MainWindow is MainWindow mainWindow)
            {
                mainWindow.mContentTab.SelectedItem = mainWindow.mInputTab;
                mCrankShaft.mTreeViewItem.IsSelected = true;
            }


            PlottingAxialStresses();//绘制轴应力图(目前只针对通轴进行应力分析）

            ////更新参数使得曲轴得以计算
            //mBelongedSet.mInputDoubleDataStorage[39] =
            mBelongedSet.isRecommendShaft = false;
            // 可选钩子：设环境变量 RV_SNAPSHOT_DIR 后，轴推荐完成即导出整机快照（含轴径/轴长）
            Services.WholeMachineSnapshot.TryAutoExport("RecommondShaftPara");
            // 同时导出曲轴系计算结果（最大应力/安全系数/各节点应力），便于外部核对
            Services.CurrentMachineCalc.TryAutoDump();
        }
        private void PlottingAxialStresses()
        {
            ////获取计算的应力值进行绘图////
            int[] CoordinateNumber = new int[1];
            double[] tmpForce = new double[200];
            double[] tmpForceLocation = new double[200];
            GetForceAndForceLocation(ref CoordinateNumber[0], ref tmpForceLocation[0], ref tmpForce[0]);

            int tmp = CoordinateNumber[0];
            tmpForceLocation = tmpForceLocation.Take(tmp).ToArray();
            tmpForce = tmpForce.Take(tmp * 2).ToArray();//两种应力

            double[] tmpBendingNormalStress = new double[tmp];
            double[] tmpShearStress = new double[tmp];
            for (int i = 0; i < tmp; i++)
            {
                tmpBendingNormalStress[i] = tmpForce[i];
                tmpShearStress[i] = tmpForce[i + tmp];
            }
            int j = 0;
            double[] YBendingNormalStress = new double[tmp];
            double[] YShearStress = new double[tmp];
            for (int i = 0; i < tmp / 2; i++)
            {
                YBendingNormalStress[j] = tmpBendingNormalStress[i];
                YShearStress[j] = tmpShearStress[i];
                j++;
                YBendingNormalStress[j] = tmpBendingNormalStress[i + tmp / 2];
                YShearStress[j] = tmpShearStress[i + tmp / 2];
                j++;
            }

            double[][] X = new double[][]
            {
                tmpForceLocation,
                tmpForceLocation
            };
            double[][] Y = new double[][]
            {
                YBendingNormalStress,
                YShearStress
            };
            string[][] CODE = new string[][]
            {
                new string[]{"偏离基准距离X/mm", "应力F/MPa", "轴受力图", "弯曲正应力", "剪应力"},
                new string[]{"偏离基准距离X/mm", "应力F/MPa", "轴受力图", "弯曲正应力", "剪应力"},
            };
            mCrankShaft.mLineChartPage.Load(X, Y, CODE);
        }
        private void UpdateShaftDiameterAndLength(double tmpDiameter, double tmpLength)
        {
            //int tmpShaftSecCount = mCrankShaft.mShaftDesignPage.currentShaft.AxisSections.Count();
            int tmpShaftSecCount = 1;
            int tmpStartIndex = 0;
            //申请整形输入数组空间
            mCrankShaft.mIntDivisionIndexList = new int[10 + CTANCrankShaft.mSecTypeToRCDataList.Length + tmpShaftSecCount];
            //整形数据转换
            //写入节点数
            mCrankShaft.mIntDivisionIndexList[0] = tmpShaftSecCount + 1;//节点数           
            //写入截面类型对应的数据长度
            mCrankShaft.mIntDivisionIndexList[mCrankShaft.IntShaftRCDataCountIndex] = 10;//1-截面类型对应的数据长度起始索引
            tmpStartIndex = mCrankShaft.mIntDivisionIndexList[1];
            Array.Copy(CTANCrankShaft.mSecTypeToRCDataList, 0, mCrankShaft.mIntDivisionIndexList, tmpStartIndex, CTANCrankShaft.mSecTypeToRCDataList.Length);
            //写入实际轴的截面类型
            mCrankShaft.mIntDivisionIndexList[mCrankShaft.IntShaftTypeInIntStartIndex] = mCrankShaft.mIntDivisionIndexList[1] + CTANCrankShaft.mSecTypeToRCDataList.Length;//2-轴各轴段截面类型起始索引
            tmpStartIndex = mCrankShaft.mIntDivisionIndexList[2];
            for (int i = 0; i < tmpShaftSecCount; i++)
            {
                //mCrankShaft.mIntDivisionIndexList[tmpStartIndex + i] = mCrankShaft.mShaftDesignPage.currentShaft.AxisSections[i].GetSectionType();
                mCrankShaft.mIntDivisionIndexList[tmpStartIndex + i] = 1;
            }
            //双精度数据转换
            //写入节点坐标
            tmpStartIndex = 0;
            mCrankShaft.mIntDivisionIndexList[mCrankShaft.IntCoorStartInIntListIndex] = tmpStartIndex;
            //写入轴的基础数据
            List<double> tmpData = new List<double>();//轴的所有数据临时存储
            tmpData.Add(0);//轴段起点偏移基准面的值
            tmpStartIndex++;
            for (int i = 0; i < tmpShaftSecCount; i++)
            {
                tmpData.Add(tmpLength);
                tmpStartIndex++;
            }
            //写入轴向指向
            mCrankShaft.mIntDivisionIndexList[mCrankShaft.IntShaftDirectStartInIntListIndex] = tmpStartIndex;
            for (int i = 0; i < mCrankShaft.mShaftDirect.Length; i++)
            {
                tmpData.Add(mCrankShaft.mShaftDirect[i]);

                tmpStartIndex++;
            }
            //写入材料参数
            mCrankShaft.mIntDivisionIndexList[mCrankShaft.IntShaftMatStartInIntListIndex] = tmpStartIndex;
            for (int i = 0; i < mCrankShaft.mShaftMat.Length; i++)
            {
                tmpData.Add(mCrankShaft.mShaftMat[i]);
                tmpStartIndex++;
            }
            //写入截面实常数参数
            mCrankShaft.mIntDivisionIndexList[mCrankShaft.IntSecRealConstStartInIntListIndex] = tmpStartIndex;
            for (int i = 0; i < tmpShaftSecCount; i++)
            {
                tmpData.Add(tmpDiameter);
                tmpStartIndex++;
            }
            mCrankShaft.mInputDoubleDataStorage = tmpData.ToArray();
        }
    }
}
