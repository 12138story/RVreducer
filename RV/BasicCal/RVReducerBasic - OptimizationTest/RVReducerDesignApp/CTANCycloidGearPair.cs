using HandyControl.Controls;
using Org.BouncyCastle.Asn1.Cmp;
using Org.BouncyCastle.Tls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace RVReducerDesignApp
{
    public class CTANCycloidPinion : CTANBasicContentPageObj
    {
        public DrawCycloidWheel CanvasGraphic { get; set; }
        //public TreeViewItem mCycloidGearTree = new TreeViewItem() { Header = "摆线轮" };
        //public static int mDoubleSorageDataNum = 20;
        //public double[] mDoubleStorageData = new double[mDoubleSorageDataNum];
        public static string[] mInputParaText = new string[]{"齿数","个",
            "偏心距", "毫米" ,
            "设计齿宽", "毫米",
            "量棒直径","毫米",
            "量棒公差","毫米",
            "精度等级","毫米",           
            //"摆线轮数目","个",
            "弹性模量", "兆帕",
            "使用系数","",
            "摆线轮传递总力矩","牛米"
        };
        public CTANCycloidGearPair mParentPage;
        public static string[] mNeedOtherParaText = new string[] { "摆线轮顶圆直径", "毫米",
         "摆线轮齿根圆直径", "毫米"};


        //非输入数据
        //public static string[] mNoInputParaText = new string[] { "曲轴相位角"};
        //public CTANControlGrid[] mPairParaTextGridList = new CTANControlGrid[1] { new() };

        public CTANCycloidPinion()
        {
            mTreeViewItem.Header = "摆线轮";
            mTreeViewItem.Name = "摆线轮";
            //mTreeViewItem = new TreeViewItem() { Header = "摆线轮", Name = "摆线轮" };
            mTreeViewItem.DataContext = this;
            
            //建立输入界面
            SetInputPage();
            SetPropertyPage();
            mDataStorageDict["齿数"].Value = 29;
            mDataStorageDict["偏心距"].Value = 0.4;
            mDataStorageDict["设计齿宽"].Value = 8;
            mDataStorageDict["量棒直径"].Value = 3;
            mDataStorageDict["量棒公差"].Value = 0.002;
            mDataStorageDict["精度等级"].Value = 5;           
            //mDataStorageDict["摆线轮数目"].Value = 2;
            mDataStorageDict["弹性模量"].Value = 2.06e5;
            mDataStorageDict["使用系数"].Value = 1.0; 
            mDataStorageDict["摆线轮传递总力矩"].Value = 759.4039215686275;
            //mDataStorageDict["曲轴相位角"].Value = 0.0;
            SetExamplePage();
        }
        private void SetExamplePage()
        {
            mImage.Source = new BitmapImage(new Uri(@"Resource\Figure\Gear\CycloidalWheel.png", UriKind.RelativeOrAbsolute));
            mExamplePageGrid.Children.Add(mImage);
        }
        public void SetPropertyPage()
        {
            Image mImagePropertyPage = new Image();
            mImagePropertyPage.Source = new BitmapImage(new Uri(@"Resource\Figure\Gear\CycloidalWheel.png", UriKind.RelativeOrAbsolute));
            Magnifier customMagnifier = new Magnifier
            {
                Scale = 6,                  // 放大倍数
                HorizontalOffset = -99,     // 水平偏移
                VerticalOffset = -99       // 垂直偏移
            };
            Magnifier.SetInstance(mImagePropertyPage, customMagnifier);
            mPropertyGrid.Children.Add(mImagePropertyPage);
            //CTANBasicFunction.CreatObjectList(1, out mPropertyGridList);
            //mPropertyGridList[0].SetStandartDisplayText(mHeadText, mShaftAssembleText, 2);
            //mPropertyGridList[0].SetGridControlByCreateComboBox("定位面", 2, [-1, 0, 1], ["右端面", "中间点", "左端面"]);
            //mPropertyGridList[0].SetStandartInputGroup("安装参数", mHeadText, mShaftAssembleText, mDataStorageDict);
            //mPropertyGrid.Children.Add(mPropertyGridList[0].mGroupBox);
        }
        private void SetInputPage()
        {
            //生成Group网格
            CTANBasicFunction.CreatObjectList(2, out mInputGridList);
            //mInputGridList = new CTANGridWithControl[] {new CTANGridWithControl()};
            //生成数据存储字典
            mDataStorageDict.SetDataStorageDictionary(0, 2, mInputParaText);
            mDataStorageDict.SetDataStorageDictionary(0, 2, mNeedOtherParaText);//暂时没有放入UI中 
            mDataStorageDict.SetDataStorageDictionary(0, 2, mShaftAssembleText);
            //页面网格划分
            mInputPageGrid.SetGridRowAndColumn(2, 1);

            mInputGridList[0].SetGridRowAndColumnWithContentTable(mInputParaText.Length / 2 + 1, 3);
            mInputGridList[1].SetGridRowAndColumnWithContentTable(mShaftAssembleText.Length / 2 + 1, 3);

            mInputGridList[0].SetGroupInGrid(0, 0, mInputPageGrid);
            mInputGridList[1].SetGroupInGrid(1, 0, mInputPageGrid);

            mInputGridList[0].SetStandartDisplayText(mHeadText, mInputParaText, 2);
            mInputGridList[0].SetStandartInputGroup("几何参数", mHeadText, mInputParaText, mDataStorageDict);

            mInputGridList[1].SetStandartDisplayText(mHeadText, mShaftAssembleText, 2);
            mInputGridList[1].SetGridControlByCreateComboBox("定位面", 2, [-1, 0, 1], ["右端面", "中间点", "左端面"]);
            mInputGridList[1].SetStandartInputGroup("定位参数", mHeadText, mShaftAssembleText, mDataStorageDict);
        }
        public void UpdateStorageData(int InStartIndex, double[] IODoubleStorageData)
        {
            IODoubleStorageData[InStartIndex + 0] = mDataStorageDict["齿数"].Value;
            IODoubleStorageData[InStartIndex + 1] = mDataStorageDict["设计齿宽"].Value;
            //IODoubleStorageData[InStartIndex + 2] = mDataStorageDict["弹性模量"].Value * 1000;
            IODoubleStorageData[InStartIndex + 2] = mDataStorageDict["弹性模量"].Value;
            IODoubleStorageData[InStartIndex + 3] = mDataStorageDict["偏心距"].Value;
            IODoubleStorageData[InStartIndex + 4] = mDataStorageDict["使用系数"].Value;
            IODoubleStorageData[InStartIndex + 5] = mDataStorageDict["摆线轮传递总力矩"].Value;
            IODoubleStorageData[InStartIndex + 6] = mDataStorageDict["定位面距基准面距离"].Value;
            IODoubleStorageData[InStartIndex + 7] = mDataStorageDict["定位面"].Value;
            IODoubleStorageData[InStartIndex + 8] = mDataStorageDict["量棒直径"].Value;
            IODoubleStorageData[InStartIndex + 9] = mDataStorageDict["量棒公差"].Value;
            IODoubleStorageData[InStartIndex + 10] = mDataStorageDict["精度等级"].Value;
            //IODoubleStorageData[InStartIndex + 6] = mDataStorageDict["曲轴相位角"].Value;
        }
        public override double ReturnGeometryDiameter()
        {
            //mDataStorageDict["摆线轮顶圆直径"].Value = mParentPage.mGear.mDataStorageDict["滚针半径"].Value * 2 - mDataStorageDict["偏心距"].Value;
            mDataStorageDict["摆线轮顶圆直径"].Value = 2 * (mParentPage.mGear.mDataStorageDict["针轮分度圆半径"].Value + mDataStorageDict["偏心距"].Value - mParentPage.mGear.mDataStorageDict["滚针半径"].Value);
            return mDataStorageDict["摆线轮顶圆直径"].Value;
        }
        public override double ReturnCycloidGearRf()
        {
            mDataStorageDict["摆线轮齿根圆直径"].Value = 2 * (mParentPage.mGear.mDataStorageDict["针轮分度圆半径"].Value - mDataStorageDict["偏心距"].Value - mParentPage.mGear.mDataStorageDict["滚针半径"].Value);
            return mDataStorageDict["摆线轮齿根圆直径"].Value;
        }
        //
    }
    public class CTANCycloidGear : CTANBasicContentPageObj
    {
        //public TreeViewItem mCycloidGearTree = new TreeViewItem() { Header = "摆线轮" };
        //public static int mDoubleSorageDataNum = 20;
        //public double[] mDoubleStorageData = new double[mDoubleSorageDataNum];
        public static string[] mInputParaText = new string[]{"齿数","个",
            "滚针半径", "毫米" ,
            "设计齿宽", "毫米",
            "针轮分度圆半径", "毫米",
            "弹性模量", "兆帕",
            "使用系数",""
        };
        //public CTANControlGrid[] mPairParaTextGridList = new CTANControlGrid[1] { new() };
        public CTANCycloidGear()
        {
            mTreeViewItem.Header = "针轮";
            mTreeViewItem.Name = "针轮";
            //mTreeViewItem = new TreeViewItem() { Header = "针轮", Name = "针轮" };
            mTreeViewItem.DataContext = this;
            SetInputPage();
            SetPropertyPage();
            mDataStorageDict["齿数"].Value = 30;
            mDataStorageDict["滚针半径"].Value = 1.5;
            mDataStorageDict["设计齿宽"].Value = 16;
            mDataStorageDict["针轮分度圆半径"].Value = 21;
            mDataStorageDict["弹性模量"].Value = 2.06e5;
            mDataStorageDict["使用系数"].Value = 1.0;
            SetExamplePage();
            //SetOutputPage();
        }
        private void SetOutputPage()
        {
            CTANBasicFunction.CreatObjectList(1, out mPropertyGridList);
            mPropertyGridList[0].SetStandartDisplayText(mHeadText, mShaftAssembleText, 2);
            mPropertyGridList[0].SetGridControlByCreateComboBox("定位面", 2, [-1, 0, 1], ["右端面", "中间点", "左端面"]);
            mPropertyGridList[0].SetStandartInputGroup("安装参数", mHeadText, mShaftAssembleText, mDataStorageDict);
            mOutputPageGrid.Children.Add(mPropertyGridList[0].mGroupBox);
        }
        private void SetExamplePage()
        {
            mImage.Source = new BitmapImage(new Uri(@"Resource\Figure\Gear\Pinwheel.png", UriKind.RelativeOrAbsolute));
            mExamplePageGrid.Children.Add(mImage);
        }
        private void SetInputPage()
        {
            //生成Group网格
            CTANBasicFunction.CreatObjectList(2, out mInputGridList);
            //页面网格划分
            mInputPageGrid.SetGridRowAndColumn(2, 1);
            //生成数据存储字典
            mDataStorageDict.SetDataStorageDictionary(0, 2, mInputParaText);
            mDataStorageDict.SetDataStorageDictionary(0, 2, mShaftAssembleText);

            mInputGridList[0].SetGridRowAndColumnWithContentTable(mInputParaText.Length / 2 + 1, 3);
            mInputGridList[1].SetGridRowAndColumnWithContentTable(mShaftAssembleText.Length / 2 + 1, 3);

            mInputGridList[0].SetGroupInGrid(0, 0, mInputPageGrid);
            mInputGridList[1].SetGroupInGrid(1, 0, mInputPageGrid);


            mInputGridList[0].SetStandartDisplayText(mHeadText, mInputParaText, 2);
            mInputGridList[0].SetStandartInputGroup("几何参数", mHeadText, mInputParaText, mDataStorageDict);

            mInputGridList[1].SetStandartDisplayText(mHeadText, mShaftAssembleText, 2);
            mInputGridList[1].SetGridControlByCreateComboBox("定位面", 2, [-1, 0, 1], ["右端面", "中间点", "左端面"]);
            mInputGridList[1].SetStandartInputGroup("定位参数", mHeadText, mShaftAssembleText, mDataStorageDict);

        }
        public void SetPropertyPage()
        {
            Image mImagePropertyPage = new Image();
            mImagePropertyPage.Source = new BitmapImage(new Uri(@"Resource\Figure\Gear\Pinwheel.png", UriKind.RelativeOrAbsolute));
            Magnifier customMagnifier = new Magnifier
            {
                Scale = 6,                  // 放大倍数
                HorizontalOffset = -99,     // 水平偏移
                VerticalOffset = -99       // 垂直偏移
            };
            Magnifier.SetInstance(mImagePropertyPage, customMagnifier);
            mPropertyGrid.Children.Add(mImagePropertyPage); 
        }
        public void UpdateStorageData(int InStartIndex,double[] IODoubleStorageData)
        {
            IODoubleStorageData[InStartIndex + 0] = mDataStorageDict["齿数"].Value;
            IODoubleStorageData[InStartIndex + 1] = mDataStorageDict["滚针半径"].Value;
            IODoubleStorageData[InStartIndex + 2] = mDataStorageDict["针轮分度圆半径"].Value;
            //IODoubleStorageData[InStartIndex + 3] = mDataStorageDict["弹性模量"].Value*1000;
            IODoubleStorageData[InStartIndex + 3] = mDataStorageDict["弹性模量"].Value;//在编写技术手册的时候法向这里多乘以1000了，暂时改掉！！！！
            IODoubleStorageData[InStartIndex + 4] = mDataStorageDict["设计齿宽"].Value;
            IODoubleStorageData[InStartIndex + 5] = mDataStorageDict["使用系数"].Value;
            IODoubleStorageData[InStartIndex + 6] = mDataStorageDict["定位面距基准面距离"].Value;
            IODoubleStorageData[InStartIndex + 7] = mDataStorageDict["定位面"].Value;

        }

    }
    public class CTANSlewingBearing: CTANBearing
    {
        public CTANSlewingBearing()
        {
            mTreeViewItem.Header = "轴承";
            mTreeViewItem.Name = "轴承";
            ////不需要结构参数，进行隐藏

            //for (int i = 0; i < mBearingInputGridList.GetLength(0); i++)
            //{
            //    mBearingInputGridList[i].Children.Remove(mBearingInputGridTable[i][2].mGroupBox);
            //    //mBearingInputGridList[i]
            //    //mBearingInputGridList[i].Children.Remove(mInputTextGridTable[i, 2].mGroupBox);
            //}
        }
    }


    public class CTANCycloidGearPair : CTANBasicContentPageObj
    {

        //[DllImport(@"RVReducerBasic.dll", EntryPoint = "CalCycloidGearStress", CallingConvention = CallingConvention.Cdecl)]
        //public static extern void CalCycloidGearStress(ref double InputData,ref double OutputData);
        [DllImport(@"RVReducerBasic.dll", EntryPoint = "CalCycloidGearStressByObj", CallingConvention = CallingConvention.Cdecl)]
        public static extern void CalCycloidGearStressByObj(ref int InDividIndexList, ref double InInputData, ref double OutputData);
        public static string[] mOutPutHeadName = new string[]  
        {
             "接触应力","兆帕",
             "全齿高h","",
             "短幅系数K1", "",
             "齿顶距D_T","",
             "跨棒距M_Mf","",
             "单个齿距偏差±f_pt","μm",
             "齿距累积总偏差F_p","μm",
             "齿向偏差F_β","μm",
             "径向跳动F_r","μm",
        };
        //int mCyCloidPinionNum = 2;//摆线轮数目
        public CTANRVSet mBelongedSet;//所属的RV减速器
        public CTANCycloidPinion[] mCycloidPinionList;
        public CTANCycloidGear mGear =new CTANCycloidGear();
        public CTANSlewingBearing[] mSlewingBearingList;
        public int[] mDivisionIndexList = new int[] { 0,//总的输入数据量
                                                        5,//输出数据存储量  
                                                        0,//摆线轮数目
                                                        20,//单个大轮输入数据存储量
                                                        20};//单个小轮输入数据存储量           
                                                        //0};//总的数据量///60;1;2;20;20
        public CTANCycloidGearPair()
        {

            SetInputPage(2);
            SetOutputPage();
            SetDataBinding();         
            //mGear.mDataStorageDict["定位面距基准面距离"].Value = 29;
            //mGear.mDataStorageDict["定位面"].Value = 1;
            //mCycloidPinionList[0].mDataStorageDict["定位面距基准面距离"].Value = 29;
            //mCycloidPinionList[0].mDataStorageDict["定位面"].Value = 1;
            //mCycloidPinionList[1].mDataStorageDict["定位面距基准面距离"].Value = 53;
            //mCycloidPinionList[1].mDataStorageDict["定位面"].Value = -1;
        }
        private void SetDataBinding()
        {
            mCycloidPinionList[1].mDataStorageDict["偏心距"].SetDataBingingToTANDouble(mCycloidPinionList[0].mDataStorageDict["偏心距"]);
            mCycloidPinionList[1].mDataStorageDict["设计齿宽"].SetDataBingingToTANDouble(mCycloidPinionList[0].mDataStorageDict["设计齿宽"]);
            mCycloidPinionList[1].mDataStorageDict["量棒直径"].SetDataBingingToTANDouble(mCycloidPinionList[0].mDataStorageDict["量棒直径"]);
            mCycloidPinionList[1].mDataStorageDict["量棒公差"].SetDataBingingToTANDouble(mCycloidPinionList[0].mDataStorageDict["量棒公差"]);
            mCycloidPinionList[1].mDataStorageDict["精度等级"].SetDataBingingToTANDouble(mCycloidPinionList[0].mDataStorageDict["精度等级"]);
            mCycloidPinionList[1].mDataStorageDict["弹性模量"].SetDataBingingToTANDouble(mCycloidPinionList[0].mDataStorageDict["弹性模量"]);
            mCycloidPinionList[1].mDataStorageDict["使用系数"].SetDataBingingToTANDouble(mCycloidPinionList[0].mDataStorageDict["使用系数"]);
            mCycloidPinionList[1].mDataStorageDict["摆线轮传递总力矩"].SetDataBingingToTANDouble(mCycloidPinionList[0].mDataStorageDict["摆线轮传递总力矩"]);
            mCycloidPinionList[1].mDataStorageDict["齿数"].SetDataBingingToTANDouble(mCycloidPinionList[0].mDataStorageDict["齿数"]);
            //绑定摆线轮和针轮齿数差1
            BindingOperations.SetBinding(mGear.mDataStorageDict["齿数"], CTANDouble.ValueProperty, new Binding("Value")
            {
                Source = mCycloidPinionList[0].mDataStorageDict["齿数"],
                Mode = BindingMode.TwoWay,
                Converter = CTANConverter.sCycloidTeethNumberConverter,
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
            });
            mGear.mDataStorageDict["使用系数"].SetDataBingingToTANDouble(mCycloidPinionList[1].mDataStorageDict["使用系数"]);
            mGear.mDataStorageDict["弹性模量"].SetDataBingingToTANDouble(mCycloidPinionList[1].mDataStorageDict["弹性模量"]);
        }
        private void SetInputPage(int InPinionCount)
        {
            mTreeViewItem.Items.Clear();
            mTreeViewItem.Header = "摆线针轮副";
            mTreeViewItem.Name = "摆线针轮副";
            mTreeViewItem.Items.Add(mGear.mTreeViewItem);
            mDivisionIndexList[2] = InPinionCount;
            mCycloidPinionList = new CTANCycloidPinion[mDivisionIndexList[2]];
            mSlewingBearingList = new CTANSlewingBearing[mDivisionIndexList[2]];
            for (int i = 0; i < mDivisionIndexList[2]; i++)
            {
                mSlewingBearingList[i] = new CTANSlewingBearing();
                mSlewingBearingList[i].mTreeViewItem.Header = "摆线轮" + i.ToString() + "轴承";
                mSlewingBearingList[i].mTreeViewItem.Name = "摆线轮" + i.ToString() + "轴承";
                mCycloidPinionList[i] = new CTANCycloidPinion();
                mCycloidPinionList[i].mTreeViewItem.Header = "摆线轮" + i.ToString();
                mCycloidPinionList[i].mTreeViewItem.Name = "摆线轮" + i.ToString();
                mCycloidPinionList[i].mTreeViewItem.Items.Add(mSlewingBearingList[i].mTreeViewItem);
                mTreeViewItem.Items.Add(mCycloidPinionList[i].mTreeViewItem);
                mCycloidPinionList[i].mParentPage = this;
            }
            
            //获取存储数据内存
            mDivisionIndexList[0] = mDivisionIndexList[3] + mDivisionIndexList[4] * mDivisionIndexList[2];//输入存储量
            mDivisionIndexList[1] = mOutPutHeadName.Count() / 2;
            SetCopyPage();
        }
        private void SetCopyPage()
        {
            mInputPageGrid.Children.Clear();
            int totalRows = 1 + mDivisionIndexList[2];
            mInputPageGrid.SetGridRowAndColumn(totalRows, 1);

            // 1) 针轮（mGear）——创建新的 CTANGridWithControl 并绑定到 mGear.mDataStorageDict
            try
            {
                CTANGridWithControl gearClone = new CTANGridWithControl();
                int gearRows = CTANCycloidGear.mInputParaText.Count() / 2 + 1;
                gearClone.SetGridRowAndColumn(gearRows, 3);
                gearClone.SetGridControlByCreateDisplayTextBox(0, 0, mHeadText.Length, mHeadText);
                gearClone.CreatPureTextObjByInputTextStringList(1, new int[] { 0, 1 }, new int[] { 2 }, 0, CTANCycloidGear.mInputParaText);
                gearClone.BingdingGridFromDict(1, CTANCycloidGear.mInputParaText.Count() / 2, 2, mGear.mDataStorageDict);
                // 将组添加到指定行（第0行）
                gearClone.SetGroupInGrid(0, 0, "针轮 输入参数", mInputPageGrid);
            }
            catch (Exception ex)
            {
                // 若你的 CTANGridWithControl API 有差异，请根据实际签名调整上述调用
                Console.WriteLine("Create gear clone failed: " + ex.Message);
            }

            // 2) 每个摆线轮（mCycloidPinionList[i]）——创建新的 CTANGridWithControl 并绑定到对应的 mDataStorageDict
            for (int i = 0; i < mDivisionIndexList[2]; i++)
            {
                try
                {
                    CTANGridWithControl pinionClone = new CTANGridWithControl();
                    //// 合并摆线轮的输入参数数组与额外需要的参数数组（与原 SetInputPage 行为一致）
                    //var pinionCombined = CTANCycloidPinion.mInputParaText.Concat(CTANCycloidPinion.mNeedOtherParaText).ToArray();
                    var pinionCombined = CTANCycloidPinion.mInputParaText;
                    int pinionRows = pinionCombined.Count() / 2 + 1;
                    pinionClone.SetGridRowAndColumn(pinionRows, 3);
                    pinionClone.SetGridControlByCreateDisplayTextBox(0, 0, mHeadText.Length, mHeadText);
                    pinionClone.CreatPureTextObjByInputTextStringList(1, new int[] { 0, 1 }, new int[] { 2 }, 0, pinionCombined);
                    // 绑定到该摆线轮自己的数据字典（因此单独页面保持不变）
                    pinionClone.BingdingGridFromDict(1, pinionCombined.Count() / 2, 2, mCycloidPinionList[i].mDataStorageDict);
                    // 添加到摆线针轮副页面（第 i+1 行）
                    pinionClone.SetGroupInGrid(i + 1, 0, "摆线轮" + i.ToString() + " 输入参数", mInputPageGrid);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Create pinion clone failed for index " + i.ToString() + " : " + ex.Message);
                }
            }
        }
        private void SetOutputPage()
        {
            mOutputPageGrid.SetGridRowAndColumn(2, 1);
            mOutputDataTable.SetGearStandardOutputTable(mHeadText, mOutPutHeadName, 2);
            mDataStorageDict.SetDataStorageDictionary(0, 2, mOutPutHeadName.Count() / 2, mOutPutHeadName);
            BindingDictToOutputTable(mDataStorageDict, 2);
            mOutputPageGrid.Children.Add(mOutputDataTable.mDataGrid);
            
        }
        public override void UpdateDllInputDoubleList(int DllInputStartIndex, double[] IODllInputData)
        {
            mGear.UpdateStorageData(DllInputStartIndex, IODllInputData);
            //IODllInputData[DllInputStartIndex + CTANCycloidGear.mDoubleSorageDataNum] = mCyCloidPinionNum;
            int tmpLoopStartIndex = DllInputStartIndex + mDivisionIndexList[3];
            //double tmpCrankShaftAngle = 2 * Math.PI / mCyCloidPinionNum;
            for (int i = 0; i < mDivisionIndexList[2]; i++)
            {
                ////先要更新曲轴相位角
                //mCycloidPinionList[i].mDataStorageDict["曲轴相位角"].Value = tmpCrankShaftAngle;
                mCycloidPinionList[i].UpdateStorageData(tmpLoopStartIndex, IODllInputData);
                tmpLoopStartIndex = tmpLoopStartIndex + mDivisionIndexList[4];
            }
        }
        public override void UpdateDllOutputDoubleList(int DllInputStartIndex, double[] IODllOutputData)
        {
            for (int i = 0; i < mOutPutHeadName.Count() / 2; i++)
            {
                mDataStorageDict[mOutPutHeadName[i * 2]].OutValue = IODllOutputData[DllInputStartIndex + i];
            }
        }
        public void UpdateDllInputDoubleList()
        {
            UpdateDllInputDoubleList(mBelongedSet.mDivisionIndexList[2, 1], mBelongedSet.mInputDoubleDataStorage);
        }
        public void UpdateDllOutputDoubleList()
        {
            UpdateDllOutputDoubleList(mBelongedSet.mDivisionIndexList[2, 2], mBelongedSet.mOutputDoubleDataStorage);
        }
        public override void CalPageData()
        {
            UpdateDllInputDoubleList();
            CalCycloidGearStressByObj(ref mBelongedSet.mPartsDivisionIndexList[mBelongedSet.mDivisionIndexList[2, 0]], 
                ref mBelongedSet.mInputDoubleDataStorage[mBelongedSet.mDivisionIndexList[2, 1]],
                ref mBelongedSet.mOutputDoubleDataStorage[mBelongedSet.mDivisionIndexList[2, 2]]);
            UpdateDllOutputDoubleList();           
        }





    }
}
