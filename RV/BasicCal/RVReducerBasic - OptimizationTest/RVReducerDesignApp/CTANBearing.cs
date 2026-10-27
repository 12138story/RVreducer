using HandyControl.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using MessageBox = System.Windows.MessageBox;
using Window = System.Windows.Window;

namespace RVReducerDesignApp
{
    
    public class CTANBearing : CTANBasicContentPageObj
    {
        [DllImport(@"RVReducerBasic.dll", EntryPoint = "RecommondBearingLife", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern int RecommondBearingLife(ref double Indata, ref double OutData, IntPtr[] OutCodes, int maxCount, int maxLen);
        [DllImport(@"RVReducerBasic.dll", EntryPoint = "CalBearingLife", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern void CalBearingLife(ref double Indata, ref double OutData);
        [DllImport(@"RVReducerBasic.dll", EntryPoint = "CalCustomBearingLife", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern int CalCustomBearingLife(ref double Indata, ref double OutData);
        public WPFBearings.BearingDataWindow mBearingData;
        //= new WPFBearings.BearingDataWindow();
        //public TreeViewItem mCycloidGearTree = new TreeViewItem() { Header = "摆线轮" };
        public static string[] mDeepGrooveBallAndNeedleBearingInputParaText = new string[]{
            "基本额定动载荷Cr", "千牛" ,
            "基本额定静载荷C0r", "千牛"
        };
        public static string[] mTaperedRollerBearingInputParaText = new string[]{
            "基本额定动载荷Cr", "千牛" ,
            "基本额定静载荷C0r", "千牛",
            "计算系数e","",
            "计算系数Y","",
            "计算系数Y0",""
        };
        public static string[] mDeepGrooveBallBearingStructParaText = new string[]{
            "轴承宽度B", "毫米" ,
            "轴承内径d", "毫米",
            "轴承外径D", "毫米",
        };
        public static string[] mNeedleBearingStructParaText = new string[]{
            "轴承宽度Bc", "毫米",
            "轴承内径Fw", "毫米",
            "轴承外径Ew", "毫米"
        };
        public static string[] mTaperedRollerBearingStructParaText = new string[]{
            "轴承宽度T", "毫米" ,
            "作用点距离左端面距离a", "毫米",
            "轴承内径d", "毫米",
            "轴承外径D", "毫米",
            "轴承宽度B", "毫米",
            "轴承宽度C", "毫米",
        };
        public static string[] mAngularContactBallBearingStructParaText = new string[]{
            "轴承宽度B", "毫米" ,
            "轴承内径d", "毫米",
            "轴承外径D", "毫米",
            "接触角","度"
        };
        private static string[] mBearingLocationParaText = new string[]
        {
            "轴承定位面", "" ,
            "轴承定位面距基准面距离", "毫米" ,        
            "轴承朝向", ""
        };
        private static string[] mInputLoadText = new string[]{
            "转速","转每分钟",
            "径向力", "牛" ,
            "轴向力", "牛",
            "预期寿命","小时"
        };
        private static string[] mOtherText = new string[]{
            "轴承类型",
            "轴承作用点对应面类型",
            "Y方向偏移"
        };
        public static string[] mBearingTypeText = new string[]{
            "深沟球轴承","滚针轴承","圆锥滚子轴承", "角接触球轴承","自定义轴承"
        };
        public static string[] mCustomBearingInputParaText = new string[]{
            "类型","",
            "列数","",
            "轴承内径d", "毫米",
            "轴承外径D", "毫米",
            "轴承宽度B", "毫米",
            "基本额定动载荷Cr", "千牛" ,
            "基本额定静载荷C0r", "千牛",
            "接触角","deg",
            "bm系数","",
            "滚子数Z","",
            "节圆直径Dpw", "毫米",
            "滚动体直径Dw", "毫米",
            "滚子总长度L", "毫米",
            "滚子倒角r", "毫米",
        };
        //0-深沟球轴承，1-滚针轴承，2-圆锥滚子轴承
        //public CTANControlGrid[,] mInputTextGridTable = new CTANControlGrid[3, 3];
        //轴承定位数据
        public double[] mLocationData=new double[4];

        public Grid[] mContentGridList=new Grid[3] { new Grid(), new Grid(), new Grid() };
        //不同轴承的内容布局网格
        public CTANGrid[] mBearingInputGridList;
        //轴承的所有内容控件
        public CTANGridWithControl[][] mBearingInputGridTable;
        
        private ContentControl mInputContent=new ContentControl();
        //private ComboBox pmBearingComboBox = new ComboBox();
        //private Grid pmBearingHeadGrid = new Grid();
        //轴承类型和定位标识符
        public static int DeepGrooveBallBearing = 1001;
        public static int NeedleBearing = 1002;
        public static int TaperedRollerBearing = 1003;
        public static int AngularContactBallBearing = 1004;
        public static int CustomBearing = 1005;

        public static int BearingLeftBase = 1;
        public static int BearingMiddleBase = 0;
        public static int BearingRightBase = -1;
        //轴承朝向，对于圆锥滚子轴承，确定了附加轴向力方向
        public static int BearingExForceToRightNotation = 1;
        public static int BearingExForceToLeftNotation = -1;
        private static BitmapImage[] pmTaperedRollerBearingImageSource = new BitmapImage[4] {
            new BitmapImage(new Uri(@"\Resource\Figure\Bearing\DeepGrooveBallBearing.png", UriKind.RelativeOrAbsolute)),
            new BitmapImage(new Uri(@"\Resource\Figure\Bearing\NeedleBearing.png", UriKind.RelativeOrAbsolute)),
            new BitmapImage(new Uri(@"\Resource\Figure\Bearing\TaperedRollerBearing.png", UriKind.RelativeOrAbsolute)),
            new BitmapImage(new Uri(@"\Resource\Figure\Bearing\AngularContactBallBearing.png", UriKind.RelativeOrAbsolute))
        };
        public CTANRVSet mBelongedSet;//所属的RV减速器
        private CTANButton mImportBearingButton=new CTANButton("从数据库导入");//从数据库导入按钮
        private CTANButton mBearingRecommendButton = new CTANButton("轴承推荐");//推荐轴承
        private CTANButton mBearingCalculateButton = new CTANButton("轴承寿命计算");//推荐轴承
        private CTANTextBox mLifeTextBox;
        //输出页面
        private BearingRecommondPage mBearingRecommondPage = new BearingRecommondPage();
        private Frame mFrameForBearingRecommond = new Frame();
        private CTANComboBox mSelectBearingComboBox;
        //public static int[] mDivisionIndexList = new int[]
        //{
        //    20,//单个轴承输入数据存储量
        //    10,//定位尺寸起始索引
        //};

        private Image pmImg=new Image();
        private Image mImagePropertyPage;// 在类中添加一个成员变量来保存属性页面的图片控件
        public DrawBearing CanvasGraphic { get; set; }
        public CTANBearing()
        {
            //mTreeViewItem = new TreeViewItem() { Header = "转臂轴承", Name = "转臂轴承" };
            mTreeViewItem.DataContext = this;
            mTreeViewItem.Header = "轴承";
            mTreeViewItem.Name = "轴承";

            //定义数据存储字典
            //mPageGrid.RowDefinitions[1].Height = new GridLength(1, GridUnitType.Auto);
            DefineDataStorageDictionary();
            SetInputPage();
            SetBearingPropertyPage();
            mDataStorageDict["轴承作用点对应面类型"].Value = BearingLeftBase;

            mBearingRecommondPage.OutAssignDictPtr = mDataStorageDict; // 传递数据字典
            mDataStorageDict["转速"].Value = 2000;
            mDataStorageDict["轴向力"].Value = 740;
            mDataStorageDict["径向力"].Value = 1750;
            mDataStorageDict["预期寿命"].Value = 6700;
            //轴承结构参数

            //2D页面
            mExamplePageGrid.Children.Add(pmImg);

            //输出页面
            mFrameForBearingRecommond.Content = mBearingRecommondPage;
            mOutputPageGrid.Children.Add(mFrameForBearingRecommond);

        }
        private void SetBearingPropertyPage()
        {
            mImagePropertyPage = new Image();
            // 设置放大镜
            Magnifier customMagnifier = new Magnifier
            {
                Scale = 6,
                HorizontalOffset = -99,
                VerticalOffset = -99
            };
            Magnifier.SetInstance(mImagePropertyPage, customMagnifier);
            mPropertyGrid.Children.Add(mImagePropertyPage); ;
        }
        private void DefineDataStorageDictionary()
        {
            mDataStorageDict.SetDataStorageDictionary(0, 2, mInputLoadText.Count() / 2, mInputLoadText);
            mDataStorageDict.SetDataStorageDictionary(0, 2, mTaperedRollerBearingInputParaText.Count() / 2, mTaperedRollerBearingInputParaText);
            mDataStorageDict.SetDataStorageDictionary(0, 2, mDeepGrooveBallBearingStructParaText.Count() / 2, mDeepGrooveBallBearingStructParaText);
            mDataStorageDict.SetDataStorageDictionary(0, 2, mNeedleBearingStructParaText.Count() / 2, mNeedleBearingStructParaText);
            mDataStorageDict.SetDataStorageDictionary(0, 2, mTaperedRollerBearingStructParaText.Count() / 2, mTaperedRollerBearingStructParaText);
            mDataStorageDict.SetDataStorageDictionary(0, 2, mBearingLocationParaText.Count() / 2, mBearingLocationParaText);
            mDataStorageDict.SetDataStorageDictionary(0, 2, mAngularContactBallBearingStructParaText.Count() / 2, mAngularContactBallBearingStructParaText);
            mDataStorageDict.SetDataStorageDictionary(0, 2, mCustomBearingInputParaText.Count() / 2, mCustomBearingInputParaText);
            
            for (int i = 0; i < mOtherText.Count(); i++)
            {
                mDataStorageDict.Add(mOtherText[i], new CTANDouble() { Value = 0 });
            }
        }
        
        ////单个轴承输入页面生成
        //private void SetInputPageGridForOneBearing(string[] InGroupHead,string[][] InStringTable,out CTANGridWithControl[] OutControlGridList, out CTANGrid OutGrid)
        //{
        //    //生成布局网格
        //    int tmpControlGridCount = InStringTable.GetLength(0);
        //    int tmpControlGridRowCount = 0 ; //Grid控件行数+1因为有1行要放表头
        //    int tmpControlGridColumnCount = 3; //Grid控件列数，3列 有1列放输入框
        //    OutGrid =new CTANGrid();
        //    OutGrid.SetGridRowAndColumn(tmpControlGridCount, 1);
        //    OutControlGridList= new CTANGridWithControl[InGroupHead.Length];
        //    //设定表头
        //    for (int i = 0; i < InStringTable.GetLength(0); i++)
        //    {                
        //        tmpControlGridRowCount = InStringTable[i].Length/2 + 1;//Grid控件行数+1因为有1行要放表头
        //        OutControlGridList[i] = new CTANGridWithControl(tmpControlGridRowCount, tmpControlGridColumnCount);
        //        //设定表头
        //        OutControlGridList[i].SetGridControlByCreateDisplayTextBox(0, 0, mHeadText.Length, mHeadText);
        //        //设定输入项及对应显示项
        //        OutControlGridList[i].CreatPureTextObjByInputTextStringList(1, [0, 1], [2], 0, InStringTable[i]);
        //        //进行数据绑定
        //        OutControlGridList[i].BingdingGridFromDict(1, InStringTable[i].Count() / 2, 2, mDataStorageDict);
        //        //封装到Group并放进OutGrid
        //        OutControlGridList[i].SetGroupInGrid(i, 0, InGroupHead[i], OutGrid);
        //    }
        //}


        //所有轴承页面生成
        private void SetInputPageGridForAllBearing()
        {
            
            int tmpDefinedBearingTypeCount = mBearingTypeText.Length;
            CTANBasicFunction.CreatObjectList(tmpDefinedBearingTypeCount,out mBearingInputGridList);

            string[][] tmpStringTable= new string[4][];
            //CTANBasicFunction.CreatObjectList(tmpDefinedBearingTypeCount, out mBearingInputGridTable);
            //mBearingInputGridTable =new CTANGridWithControl[tmpStringTable.GetLength(0)][];
            CTANBasicFunction.CreatObjectTable(tmpDefinedBearingTypeCount, 4, out mBearingInputGridTable);
            //深沟球轴承
            tmpStringTable[0] = mDeepGrooveBallAndNeedleBearingInputParaText;//强度参数
            tmpStringTable[1] = mDeepGrooveBallBearingStructParaText;//结构参数
            tmpStringTable[2] = mInputLoadText;//载荷参数
            tmpStringTable[3] = mBearingLocationParaText;//定位参数

            //CTANBasicFunction.CreatObjectList(tmpStringTable.Length, out mBearingInputGridTable[0]);
            CTANGridWithControl.SetStandartInputPageGrid(["强度参数", "结构参数", "载荷参数", "定位参数"], mHeadText, tmpStringTable,mDataStorageDict, mBearingInputGridTable[0], mBearingInputGridList[0]);
            //滚针轴承
            tmpStringTable[1] = mNeedleBearingStructParaText;
            //CTANBasicFunction.CreatObjectList(tmpStringTable.Length, out mBearingInputGridTable[1]);
            CTANGridWithControl.SetStandartInputPageGrid(["强度参数", "结构参数", "载荷参数", "定位参数"], mHeadText, tmpStringTable, mDataStorageDict, mBearingInputGridTable[1], mBearingInputGridList[1]);
            //设定圆锥滚子轴承
            tmpStringTable[0] = mTaperedRollerBearingInputParaText;
            tmpStringTable[1] = mTaperedRollerBearingStructParaText;
            CTANGridWithControl.SetStandartInputPageGrid(["强度参数", "结构参数", "载荷参数", "定位参数"], mHeadText, tmpStringTable, mDataStorageDict, mBearingInputGridTable[2], mBearingInputGridList[2]);
            //设定角接触球轴承
            tmpStringTable[0] = mDeepGrooveBallAndNeedleBearingInputParaText;//强度参数
            tmpStringTable[1] = mAngularContactBallBearingStructParaText;
            CTANGridWithControl.SetStandartInputPageGrid(["强度参数", "结构参数", "载荷参数", "定位参数"], mHeadText, tmpStringTable, mDataStorageDict, mBearingInputGridTable[3], mBearingInputGridList[3]);
            
            //自定义轴承
            CTANGridWithControl.SetStandartInputPageGrid(["强度参数", "结构参数", "载荷参数", "定位参数"], mHeadText, tmpStringTable, mDataStorageDict, mBearingInputGridTable[4], mBearingInputGridList[4]);
            for (int i = 0; i < tmpDefinedBearingTypeCount; i++)
            {
                SetLocationComboBoxes(mBearingInputGridTable[i][3]);
                Grid.SetRow(mBearingInputGridList[i], 1);
                Grid.SetColumn(mBearingInputGridList[i], 0);
                //for (int j = 0; j < mBearingInputGridTable[i].Length; j++)
                //{
                //    mBearingInputGridTable[i][j].SetGridDefaultRowHeight();
                //}

            }


        }
        // 新增：设置定位参数的下拉框
        private void SetLocationComboBoxes(CTANGridWithControl InLocationGrid)
        {
            // 注意：由于 AssignControlToGrid 只在位置为空时才添加控件，
            // 需要先替换掉原有的 TextBox，这里我们直接操作控件

            // 获取"轴承定位面"所在的行索引
            int rowIndex1 = InLocationGrid.mRowNameToIndex["轴承定位面"];
            // 获取"轴承朝向"所在的行索引  
            int rowIndex2 = InLocationGrid.mRowNameToIndex["轴承朝向"];

            // 移除原有控件并创建新的 ComboBox
            ReplaceTextBoxWithComboBox(InLocationGrid, rowIndex1, 2,
                [-1, 0, 1], ["右端面", "中间点", "左端面"], "轴承定位面");
            ReplaceTextBoxWithComboBox(InLocationGrid, rowIndex2, 2,
                [-1, 1], ["反向", "正向"], "轴承朝向");
        }

        // 新增：替换 TextBox 为 ComboBox
        private void ReplaceTextBoxWithComboBox(CTANGridWithControl InGrid, int InRowIndex, int InColumnIndex,
            int[] InComboBoxID, string[] InComboBoxDisplayString, string InBindingKey)
        {
            // 移除原有控件
            if (InGrid.mControlTable[InRowIndex][InColumnIndex] != null)
            {
                var oldControl = InGrid.mControlTable[InRowIndex][InColumnIndex].mObject as UIElement;
                if (oldControl != null)
                {
                    InGrid.Children.Remove(oldControl);
                }
                InGrid.mControlTable[InRowIndex][InColumnIndex] = null;
            }

            // 创建新的 ComboBox
            CTANComboBox tmpComboBox = new CTANComboBox(InComboBoxID, InComboBoxDisplayString);

            // 添加到控件表
            InGrid.mControlTable[InRowIndex][InColumnIndex] = new CTANObject();
            InGrid.mControlTable[InRowIndex][InColumnIndex].SetStorageObj(tmpComboBox);

            // 添加到 Grid
            Grid.SetRow(tmpComboBox, InRowIndex);
            Grid.SetColumn(tmpComboBox, InColumnIndex);
            InGrid.Children.Add(tmpComboBox);

            // 绑定数据
            mDataStorageDict[InBindingKey].SetDataBingingToComboBox(tmpComboBox);
        }
        //设置输入页面
        private void SetInputPage()
        {
            //生成所有轴承的输入界面
            SetInputPageGridForAllBearing();
            //轴承导入界面的生成
            mInputPageGrid.SetGridRowAndColumn(3, 1);
            CTANGridWithControl tmpFirstRowInputGrid = new CTANGridWithControl();
            tmpFirstRowInputGrid.SetGridRowAndColumn(1, 3);
            //轴承类型下拉项
            tmpFirstRowInputGrid.SetGridControlByCreateDisplayTextBox(0, 0, "轴承类型");
            mSelectBearingComboBox = new CTANComboBox([DeepGrooveBallBearing, NeedleBearing, TaperedRollerBearing, AngularContactBallBearing, CustomBearing], mBearingTypeText);
            tmpFirstRowInputGrid.AssignControlToGrid(0, 1, mSelectBearingComboBox);
            //将ID值与输入Grid网络对应
            mSelectBearingComboBox.mIDToContentDict = new Dictionary<int, object>();
            mSelectBearingComboBox.mIDToContentDict[DeepGrooveBallBearing] = mBearingInputGridList[0];
            mSelectBearingComboBox.mIDToContentDict[NeedleBearing] = mBearingInputGridList[1];
            mSelectBearingComboBox.mIDToContentDict[TaperedRollerBearing] = mBearingInputGridList[2];
            mSelectBearingComboBox.mIDToContentDict[AngularContactBallBearing] = mBearingInputGridList[3];
            mSelectBearingComboBox.mIDToContentDict[CustomBearing] = mBearingInputGridList[4];

            // 为自定义轴承创建专属页面（如果可用），并覆盖默认映射
            try
            {
                var customPage = new RVReducerDesignApp.CustomBearingPage(mDataStorageDict);
                mSelectBearingComboBox.mIDToContentDict[CustomBearing] = customPage.mInputPageGrid;
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"CustomBearingPage 创建失败: {ex.Message}\n{ex.StackTrace}", "错误");
            }
            //数据同步
            mDataStorageDict["轴承类型"].SetDataBingingToComboBox(mSelectBearingComboBox);
            //设定轴承切换消息函数
            mSelectBearingComboBox.SelectionChanged += SelectedBearingType_Changed;
            //设定“从数据库导入”按钮事件
            mImportBearingButton.AddClickEvent(ImportDataBtn_Clicked);
            //导入按钮所在网格
            CTANGridWithControl tmpImportBearingGrid = new CTANGridWithControl();
            tmpImportBearingGrid.SetGridRowAndColumn(1, 2);
            tmpImportBearingGrid.AssignControlToGrid(0, 1, mImportBearingButton);
            tmpFirstRowInputGrid.AssignControlToGrid(0, 2, tmpImportBearingGrid);
            //导入第一行
            mInputPageGrid.Children.Add(tmpFirstRowInputGrid);
            //导入轴承输入控件
            Grid.SetRow(mInputContent, 1);
            mInputPageGrid.Children.Add(mInputContent);
            //在第三行添加推荐轴承和计算按钮
            mBearingRecommendButton.AddClickEvent(RecommendBearingBtn_Clicked);
            mBearingCalculateButton.AddClickEvent(CalculateBearingBtn_Clicked);
            CTANGridWithControl tmpRecommendBearingGrid = new CTANGridWithControl();
            tmpRecommendBearingGrid.SetGridRowAndColumn(1, 7);
            tmpRecommendBearingGrid.AssignControlToGrid(0, 3, mBearingCalculateButton);
            tmpRecommendBearingGrid.AssignControlToGrid(0, 1, mBearingRecommendButton);
            // 添加结果标签
            CTANTextBox resultLabel = new CTANTextBox(true, "轴承寿命：");
            tmpRecommendBearingGrid.AssignControlToGrid(0, 5, resultLabel);

            // 添加结果显示框
            mLifeTextBox = new CTANTextBox(true, "未计算");
            mLifeTextBox.Width = 80; // 设置宽度
            tmpRecommendBearingGrid.AssignControlToGrid(0, 6, mLifeTextBox);
            Grid.SetRow(tmpRecommendBearingGrid, 2);
            mInputPageGrid.Children.Add(tmpRecommendBearingGrid);
        }

        private void RecommendBearingBtn_Clicked(object sender, RoutedEventArgs e)
        {
            double[] Indata = new double[5] 
            {
                (int)mDataStorageDict["轴承类型"].Value,
                mDataStorageDict["转速"].Value, 
                mDataStorageDict["轴向力"].Value,
                mDataStorageDict["径向力"].Value, 
                mDataStorageDict["预期寿命"].Value 
            };
            int maxCount = 100; // 最多100个轴承
            int maxLen = 64;    // 每个编号最多64字节

            double[] OutData = new double[maxCount * 9];
            //型号string
            IntPtr[] OutCodes = new IntPtr[maxCount];
            for (int i = 0; i < maxCount; i++)
                OutCodes[i] = Marshal.AllocHGlobal(maxLen);

            int actualCount = RecommondBearingLife(ref Indata[0], ref OutData[0], OutCodes, maxCount, maxLen);

            //将结果转为 C# 字符串
            List<string> codes = new List<string>();
            for (int i = 0; i < actualCount; i++)
            {
                string code = Marshal.PtrToStringAnsi(OutCodes[i]);
                codes.Add(code);
            }

            //释放内存
            for (int i = 0; i < maxCount; i++)
                Marshal.FreeHGlobal(OutCodes[i]);

            //构造 BearingData 列表
            List<BearingRecommondPage.BearingData> bearingList = new List<BearingRecommondPage.BearingData>();

            for (int i = 0; i < actualCount; i++)
            {
                int baseIndex = 0;
                if (mDataStorageDict["轴承类型"].Value == TaperedRollerBearing)
                {
                    mBearingRecommondPage.GenerateColumns(true);
                    baseIndex = i * 9; // 圆锥滚子轴承每个型号暂时设置 9 个数据
                    bearingList.Add(new BearingRecommondPage.BearingData
                    {
                        Code = codes[i],
                        d = OutData[baseIndex + 0],
                        D = OutData[baseIndex + 1],
                        B = OutData[baseIndex + 2],
                        Cr = OutData[baseIndex + 3],
                        C0r = OutData[baseIndex + 4],
                        e = OutData[baseIndex + 5],
                        Y = OutData[baseIndex + 6],
                        Yo = OutData[baseIndex + 7],
                        Life = OutData[baseIndex + 8],
                    });
                }
                else
                {
                    baseIndex = i * 6; // 每个型号有 6 个数据
                    bearingList.Add(new BearingRecommondPage.BearingData
                    {
                        Code = codes[i],
                        d = OutData[baseIndex + 0],
                        D = OutData[baseIndex + 1],
                        B = OutData[baseIndex + 2],
                        Cr = OutData[baseIndex + 3],
                        C0r = OutData[baseIndex + 4],
                        Life = OutData[baseIndex + 5],
                    });
                }
                
            }
            mBearingRecommondPage.LoadData(bearingList);
            System.Windows.MessageBox.Show("推荐成功！请前往计算结果页面查看");
        }
        private void CalculateBearingBtn_Clicked(object sender, RoutedEventArgs e)
        {
            // 自定义轴承分支：使用 CalCustomBearingLife
            if ((int)mDataStorageDict["轴承类型"].Value == CustomBearing)
            {
                if (mDataStorageDict["径向力"].Value == 0 && mDataStorageDict["轴向力"].Value == 0)
                {
                    System.Windows.MessageBox.Show("请先在「载荷」Tab中输入径向力或轴向力！");
                    return;
                }
                if (mDataStorageDict["转速"].Value <= 0)
                {
                    System.Windows.MessageBox.Show("请先在「载荷」Tab中输入转速！");
                    return;
                }

                double[] customIndata = BuildCustomBearingInData();
                double[] OutData = new double[20];
                int result = CalCustomBearingLife(ref customIndata[0], ref OutData[0]);

                if (result == 0 && OutData[0] > 0)
                {
                    mLifeTextBox.Text = $"L10h: {OutData[10]:F2} h | S0: {OutData[4]:F2}";
                }
                else
                {
                    mLifeTextBox.Text = "计算失败";
                    System.Windows.MessageBox.Show("自定义轴承寿命计算失败，请检查参数设置。");
                }
                return;
            }

            // 标准轴承分支：使用原有 CalBearingLife
            double[] Indata = new double[9]
            {
                (int)mDataStorageDict["轴承类型"].Value,
                mDataStorageDict["转速"].Value,
                mDataStorageDict["轴向力"].Value,
                mDataStorageDict["径向力"].Value,
                mDataStorageDict["基本额定动载荷Cr"].Value,
                mDataStorageDict["基本额定静载荷C0r"].Value,
                mDataStorageDict["计算系数e"].Value,
                mDataStorageDict["计算系数Y"].Value,
                mDataStorageDict["计算系数Y0"].Value,
            };
            if(mDataStorageDict["基本额定动载荷Cr"].Value == 0 || mDataStorageDict["基本额定静载荷C0r"].Value == 0)
            {
                System.Windows.MessageBox.Show("请先输入轴承强度和结构参数！");
                return;
            }
            else
            {
                double[] OutData = new double[10];//目前只有一个寿命返回值，暂定10，后续根据实际情况调整
                CalBearingLife(ref Indata[0], ref OutData[0]);
                mLifeTextBox.Text = $"{OutData[0]:F2}" + "h";
            }

        }
        //选择的轴承类型改变
        private void SelectedBearingType_Changed(object sender, RoutedEventArgs e)
        {
            mInputContent.Content = mSelectBearingComboBox.mIDToContentDict[(int)mDataStorageDict["轴承类型"].Value] as Grid;
            if (mSelectBearingComboBox.SelectedIndex == 4) //4为自定义轴承
            {
                pmImg.Source = null;
                mImagePropertyPage.Source = null;
            }
            else 
            {
                pmImg.Source = pmTaperedRollerBearingImageSource[mSelectBearingComboBox.SelectedIndex];
                mImagePropertyPage.Source = pmTaperedRollerBearingImageSource[mSelectBearingComboBox.SelectedIndex];
            }

                
            pmImg.Height = 500;            
            e.Handled = true;
        }
        //从数据库导入事件
        private void ImportDataBtn_Clicked(object sender, RoutedEventArgs e)
        {
            int tmpType = (int)mDataStorageDict["轴承类型"].Value; //需要修改！数据库页面直接获取这个值
            WPFBearings.BearingDataWindow TmpBearingData = new WPFBearings.BearingDataWindow();
            TmpBearingData.OutAssignDictPtr = mDataStorageDict;
            TmpBearingData.IsReturnData = false;
            TmpBearingData.ShowDialog();
        }
        //private void SelectedGearType_Changed(object sender, RoutedEventArgs e)
        //{
        //    mPageGrid.Children.Remove(mDisplayContent);
        //    mDisplayContent = mContentGridList[pmBearingComboBox.SelectedIndex];
        //    mPageGrid.Children.Add(mDisplayContent);
        //    pmImg.Source = pmTaperedRollerBearingImageSource[pmBearingComboBox.SelectedIndex];
        //    pmImg.Height = 500;            
        //    e.Handled = true;
        //}
        private void ImportButtonClicked(object sender, RoutedEventArgs e)
        {
            mBearingData=new WPFBearings.BearingDataWindow();
            mBearingData.OutAssignDictPtr = mDataStorageDict;
            mBearingData.ShowDialog();
            
            //if (mBearingData.mSelectObj.IsBearingSelected)
            //{
            //    mDataStorageDict["轴承类型"].Value = mBearingData.mSelectObj.mSelectBearingType;
            //    mDataStorageDict["轴承类型"].Value
            //}
        }
        public override void UpdateDllInputDoubleList(int InStartIndex, double[] IODllInputData)
        {
            IODllInputData[InStartIndex + 0] = mDataStorageDict["轴承类型"].Value;         
            int tmpStartIndex = InStartIndex + 1;
            //强度计算参数，0-转速（通过后端计算），1-计算系数e，2-计算系数Y，3-计算系数Y0,4-额定动载荷Cr，5-额定静载荷C0R
            IODllInputData[tmpStartIndex + 1] = mDataStorageDict["基本额定动载荷Cr"].Value;
            IODllInputData[tmpStartIndex + 2] = mDataStorageDict["基本额定静载荷C0r"].Value;
            if ((int)mDataStorageDict["轴承类型"].Value == CTANBearing.TaperedRollerBearing)
            {
                //IODllInputData[InStartIndex + 0] = mDataStorageDict["转速"].Value;
                IODllInputData[tmpStartIndex + 3] = mDataStorageDict["计算系数e"].Value;
                IODllInputData[tmpStartIndex + 4] = mDataStorageDict["计算系数Y"].Value;
                IODllInputData[tmpStartIndex + 5] = mDataStorageDict["计算系数Y0"].Value;
            }

            tmpStartIndex = mBelongedSet.mBearingComponentDivisionIndexList[5] + InStartIndex;
            //定位参数
            IODllInputData[tmpStartIndex + 0] = mDataStorageDict["轴承定位面"].Value;
            IODllInputData[tmpStartIndex + 1] = mDataStorageDict["轴承定位面距基准面距离"].Value;
            IODllInputData[tmpStartIndex + 2] = mDataStorageDict["轴承作用点对应面类型"].Value;
            IODllInputData[tmpStartIndex + 3] = mDataStorageDict["作用点距离左端面距离a"].Value;
            IODllInputData[tmpStartIndex + 4] = mDataStorageDict["轴承朝向"].Value;

            if ((int)mDataStorageDict["轴承类型"].Value == CTANBearing.DeepGrooveBallBearing || (int)mDataStorageDict["轴承类型"].Value == CTANBearing.AngularContactBallBearing)
            {
                IODllInputData[tmpStartIndex + 5] = mDataStorageDict["轴承宽度B"].Value;
            }
            else if ((int)mDataStorageDict["轴承类型"].Value == CTANBearing.TaperedRollerBearing)
            {
                IODllInputData[tmpStartIndex + 5] = mDataStorageDict["轴承宽度T"].Value;
            }
            else if ((int)mDataStorageDict["轴承类型"].Value == CTANBearing.NeedleBearing)
            {
                IODllInputData[tmpStartIndex + 5] = mDataStorageDict["轴承宽度Bc"].Value;
            }
        }
        public override double ReturnGeometryDiameter()
        {
            int tmpbearingType = Convert.ToInt32(mDataStorageDict["轴承类型"].Value);
            if (tmpbearingType == DeepGrooveBallBearing)
            {
                return mDataStorageDict["轴承外径D"].Value;
            }
            else if (tmpbearingType == NeedleBearing)
            {
                return mDataStorageDict["轴承外径Ew"].Value;
            }
            else
            {
                return mDataStorageDict["轴承外径D"].Value;
            }
        }
        public override double ReturnBearingInnerDiameter()
        {
            int tmpbearingType = Convert.ToInt32(mDataStorageDict["轴承类型"].Value);
            if (tmpbearingType == DeepGrooveBallBearing)
            {
                return mDataStorageDict["轴承内径d"].Value;
            }
            else if (tmpbearingType == NeedleBearing)
            {
                return mDataStorageDict["轴承内径Fw"].Value;
            }
            else
            {
                return mDataStorageDict["轴承内径d"].Value;
            }
            
        }
        public override double ReturnGeometryWidth()
        {
            int tmpbearingType = Convert.ToInt32(mDataStorageDict["轴承类型"].Value);
            if (tmpbearingType == DeepGrooveBallBearing || tmpbearingType == AngularContactBallBearing)
            {
                return mDataStorageDict["轴承宽度B"].Value;
            }
            else if (tmpbearingType == NeedleBearing)
            {
                return mDataStorageDict["轴承宽度Bc"].Value;
            }
            else
            {
                return mDataStorageDict["轴承宽度T"].Value;
            }
        }
        public override double ReturnAssembleDistance()
        {
            return mDataStorageDict["轴承定位面距基准面距离"].Value;
        }
        public override int ReturnAssembleFaceType()
        {
            int tmpAssebleFaceType = Convert.ToInt32(mDataStorageDict["轴承定位面"].Value);
            if (tmpAssebleFaceType == CTANRVSet.AssembleLeftBaseNotation)
            {
                //return "左端面";
                return 1;
            }
            else if (tmpAssebleFaceType == CTANRVSet.AssembleMiddleBaseNotation)
            {
                //return "中心基准面";
                return 0;
            }
            else
            {
                return -1;
            }
        }
        /// <summary>
        /// 将 CTANBearing 类型 ID 映射到 CalCustomBearingLife 的 typeCode (0-7)
        /// </summary>
        public static int MapToCustomBearingTypeCode(int tanTypeId)
        {
            if (tanTypeId == DeepGrooveBallBearing) return 0;       // DeepGrooveBall
            if (tanTypeId == AngularContactBallBearing) return 1;    // AngularContact
            if (tanTypeId == NeedleBearing) return 3;                // RadialRoller
            if (tanTypeId == TaperedRollerBearing) return 3;         // RadialRoller
            return -1;                                                
        }

        /// <summary>
        /// 将 CTANBearing 类型 ID 映射到滚子子类型 (subType)
        /// 1=圆锥滚子, 2=冲压滚针, 0=默认
        /// </summary>
        public static int MapToSubType(int tanTypeId)
        {
            if (tanTypeId == TaperedRollerBearing) return 1;
            if (tanTypeId == NeedleBearing) return 2;
            return 0;
        }

        // CalCustomBearingLife 输入数组索引常量
        private const int CBI_TYPE_CODE = 0;
        private const int CBI_LAYOUT = 1;
        private const int CBI_ASSEMBLY_COUNT = 2;
        private const int CBI_CR = 3;
        private const int CBI_C0R = 4;
        private const int CBI_DPW = 5;
        private const int CBI_DW = 6;
        private const int CBI_ALPHA = 7;
        private const int CBI_ROWS = 8;
        private const int CBI_Z = 9;
        private const int CBI_BM = 10;
        private const int CBI_L_TOTAL = 11;
        private const int CBI_R = 12;
        private const int CBI_SUB_TYPE = 13;
        private const int CBI_BETA = 14;
        private const int CBI_RL = 15;
        private const int CBI_RS = 16;
        private const int CBI_FR = 28;
        private const int CBI_FA = 29;
        private const int CBI_N = 30;
        private const int CBI_CU = 31;
        private const int CBI_V = 32;
        private const int CBI_CONTAM = 33;
        private const int CBI_LAMBDA = 34;
        private const int CBI_EP = 35;
        private const int CBI_RELIABILITY = 36;
        /// <summary>
        /// 安全地从字典读取值，键不存在时返回 0
        /// </summary>
        private double SafeGetDictValue(string key)
        {
            return mDataStorageDict.ContainsKey(key) ? mDataStorageDict[key].Value : 0;
        }
        /// <summary>
        /// 从共享字典构建 CalCustomBearingLife 所需的 37 元素输入数组
        /// 注意：字典中 Cr/C0r 以千牛为单位存储，此处转换为牛（×1000）
        /// </summary>
        public double[] BuildCustomBearingInData()
        {
            double[] data = new double[37];
            int typeId;
            // 优先使用"轴承"（CustomBearingPage 下拉框所选实际类型 1001-1004），回退到"轴承类型"
            if (mDataStorageDict.ContainsKey("轴承"))
                typeId = (int)mDataStorageDict["轴承"].Value;
            else if (mDataStorageDict.ContainsKey("轴承类型"))
                typeId = (int)mDataStorageDict["轴承类型"].Value;
            else
                typeId = DeepGrooveBallBearing;

            data[CBI_TYPE_CODE] = MapToCustomBearingTypeCode(typeId);
            if(data[CBI_TYPE_CODE] ==-1)
            {
                System.Windows.MessageBox.Show("不支持的轴承类型，请检查输入！");
                return data;
            }
            data[CBI_LAYOUT] = SafeGetDictValue("布置方式");
            data[CBI_ASSEMBLY_COUNT] = SafeGetDictValue("装配数量");
            data[CBI_CR] = SafeGetDictValue("基本额定动载荷Cr") ;
            data[CBI_C0R] = SafeGetDictValue("基本额定静载荷C0r");
            data[CBI_DPW] = SafeGetDictValue("节圆直径Dpw");
            data[CBI_DW] = SafeGetDictValue("滚动体直径Dw");
            data[CBI_ALPHA] = SafeGetDictValue("接触角");
            data[CBI_ROWS] = SafeGetDictValue("列数");
            data[CBI_Z] = SafeGetDictValue("滚子数Z");
            data[CBI_BM] = SafeGetDictValue("bm系数");
            data[CBI_L_TOTAL] = SafeGetDictValue("滚子总长度L");
            data[CBI_R] = SafeGetDictValue("滚子倒角r");
            data[CBI_SUB_TYPE] = SafeGetDictValue("滚子子类型");
            data[CBI_BETA] = SafeGetDictValue("滚子锥度角");
            data[CBI_RL] = SafeGetDictValue("大端倒角rl");     // ← 原来崩溃的位置
            data[CBI_RS] = SafeGetDictValue("小端倒角rs");
            data[CBI_FR] = SafeGetDictValue("径向力");
            data[CBI_FA] = SafeGetDictValue("轴向力");
            data[CBI_N] = SafeGetDictValue("转速");
            data[CBI_CU] = SafeGetDictValue("疲劳载荷极限Cu");
            data[CBI_V] = SafeGetDictValue("润滑油粘度v");
            data[CBI_CONTAM] = SafeGetDictValue("污染等级");
            data[CBI_LAMBDA] = SafeGetDictValue("膜厚比lambda");
            data[CBI_EP] = SafeGetDictValue("极压添加剂");
            data[CBI_RELIABILITY] = SafeGetDictValue("可靠度");

            return data;
        }
    }
    public class BearingMenuHelper
    {
        private int mBearingId = 0;

        /// <summary>
        /// 轴承列表引用
        /// </summary>
        public List<CTANBearing> BearingList { get; private set; }

        /// <summary>
        /// 父级树节点引用
        /// </summary>
        public TreeViewItem ParentTreeViewItem { get; private set; }

        /// <summary>
        /// 轴承计数
        /// </summary>
        public int BearingCount { get; set; } = 0;

        /// <summary>
        /// 所属的RV减速器
        /// </summary>
        public CTANRVSet BelongedSet { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="bearingList">轴承列表</param>
        /// <param name="parentTreeViewItem">父级树节点</param>
        public BearingMenuHelper(List<CTANBearing> bearingList, TreeViewItem parentTreeViewItem)
        {
            BearingList = bearingList ?? new List<CTANBearing>();
            ParentTreeViewItem = parentTreeViewItem;
        }
#region 树视图轴承菜单栏
        /// <summary>
        /// 创建轴承操作右键菜单并绑定到父级树节点
        /// </summary>
        public void SetupContextMenu()
        {
            ParentTreeViewItem.ContextMenu = new ContextMenu();

            MenuItem[] menuItems = new MenuItem[]
            {
                new MenuItem() { Header = "添加轴承" },
                new MenuItem() { Header = "删除轴承" },
                new MenuItem() { Header = "重命名轴承" },
            };

            foreach (var item in menuItems)
            {
                ParentTreeViewItem.ContextMenu.Items.Add(item);
            }

            ParentTreeViewItem.ContextMenu.AddHandler(MenuItem.ClickEvent, new RoutedEventHandler(MenuItem_Click));
        }
        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            MenuItem menuItem = e.Source as MenuItem;
            if (menuItem == null) return;

            string menuHeader = menuItem.Header.ToString();

            switch (menuHeader)
            {
                case "添加轴承":
                    AddBearing();
                    break;
                case "删除轴承":
                    DeleteBearing();
                    break;
                case "重命名轴承":
                    RenameBearing();
                    break;
            }
        }
        private void AddBearing()
        {
            mBearingId++;
            CTANBearing newBearing = new CTANBearing();
            newBearing.mTreeViewItem.Header = "轴承" + mBearingId.ToString();
            newBearing.mTreeViewItem.Name = "轴承";
            newBearing.mBelongedSet = BelongedSet;

            BearingList.Add(newBearing);
            ParentTreeViewItem.Items.Add(newBearing.mTreeViewItem);
            BearingCount++;
        }
        private void DeleteBearing()
        {
            // 使用倒序遍历避免删除时索引错乱
            for (int i = BearingList.Count - 1; i >= 0; i--)
            {
                if (BearingList[i].mTreeViewItem.IsSelected)
                {
                    if (BearingList[i].mTreeViewItem.Name == "轴承")
                    {
                        ParentTreeViewItem.Items.Remove(BearingList[i].mTreeViewItem);
                        BearingList.RemoveAt(i);
                        BearingCount--;
                    }
                }
            }
        }
        private void RenameBearing()
        {
            for (int i = 0; i < BearingList.Count; i++)
            {
                var treeItem = BearingList[i].mTreeViewItem;
                if (treeItem.IsSelected)
                {
                    string oldHeader = treeItem.Header?.ToString() ?? "";
                    Window owner = Window.GetWindow(treeItem) ?? Application.Current?.MainWindow;

                    string newName;
                    bool isNameValid;

                    do
                    {
                        newName = UIRenameDialog.ShowDialog(owner, "重命名轴承", oldHeader);

                        if (string.IsNullOrWhiteSpace(newName) || newName == oldHeader)
                            break;

                        isNameValid = true;
                        foreach (var bearing in BearingList)
                        {
                            if (bearing.mTreeViewItem != treeItem &&
                                bearing.mTreeViewItem.Header?.ToString() == newName)
                            {
                                MessageBox.Show(owner, "轴承名称已存在，请使用其他名称。", "名称重复",
                                              MessageBoxButton.OK, MessageBoxImage.Warning);
                                isNameValid = false;
                                break;
                            }
                        }

                        if (isNameValid)
                        {
                            treeItem.Header = newName;

                            CTANBearing model = treeItem.DataContext as CTANBearing;
                            if (model != null)
                            {
                                var t = model.GetType();
                                var prop = t.GetProperty("Name") ?? t.GetProperty("mName") ?? t.GetProperty("DisplayName");
                                if (prop != null && prop.CanWrite)
                                {
                                    prop.SetValue(model, newName, null);
                                }
                            }
                        }

                    } while (!isNameValid);
                }
            }
        }
        #endregion
#region 树视图自动添加轴承
        public void AddBearingsList(double[] InputParams)
        {
            var bearingList = BearingList;
            var treeViewItem = ParentTreeViewItem;

            CTANBearing angularContactBall0 = bearingList.FirstOrDefault(b => b.mTreeViewItem.Header.ToString() == "左侧角接触球轴承");
            CTANBearing angularContactBall1 = bearingList.FirstOrDefault(b => b.mTreeViewItem.Header.ToString() == "右侧角接触球轴承");
            //CTANBearing needle = bearingList.FirstOrDefault(b => b.mTreeViewItem.Header.ToString() == "推荐的滚针轴承");
            CTANBearing tapered0 = bearingList.FirstOrDefault(b => b.mTreeViewItem.Header.ToString() == "左侧圆锥滚子轴承");
            CTANBearing tapered1 = bearingList.FirstOrDefault(b => b.mTreeViewItem.Header.ToString() == "右侧圆锥滚子轴承");

            if (InputParams[20] == 1003)
            {
                if (angularContactBall0 != null)
                {
                    treeViewItem.Items.Remove(angularContactBall0.mTreeViewItem);
                    bearingList.Remove(angularContactBall0);
                    BearingCount--;
                }
                if (angularContactBall1 != null)
                {
                    treeViewItem.Items.Remove(angularContactBall1.mTreeViewItem);
                    bearingList.Remove(angularContactBall1);
                    BearingCount--;
                }

                if (tapered0 == null)
                {
                    tapered0 = new CTANBearing();
                    tapered0.mTreeViewItem.Header = "左侧圆锥滚子轴承";
                    tapered0.mTreeViewItem.Name = "左侧圆锥滚子轴承";
                    bearingList.Add(tapered0);
                    treeViewItem.Items.Add(tapered0.mTreeViewItem);
                    BearingCount++;
                }
                if (tapered1 == null)
                {
                    tapered1 = new CTANBearing();
                    tapered1.mTreeViewItem.Header = "右侧圆锥滚子轴承";
                    tapered1.mTreeViewItem.Name = "右侧圆锥滚子轴承";
                    bearingList.Add(tapered1);
                    treeViewItem.Items.Add(tapered1.mTreeViewItem);
                    BearingCount++;
                }



                var tapereds = new[] { tapered0, tapered1 };
                //更新两个圆锥滚子轴承的定位参数 
                tapered0.mDataStorageDict["轴承定位面"].Value = InputParams[2];
                tapered0.mDataStorageDict["轴承定位面距基准面距离"].Value = InputParams[3];//第一个圆锥滚子轴承
                tapered0.mDataStorageDict["轴承朝向"].Value = InputParams[4];
                tapered1.mDataStorageDict["轴承定位面"].Value = InputParams[5];
                tapered1.mDataStorageDict["轴承定位面距基准面距离"].Value = InputParams[6];//第二个圆锥滚子轴承
                tapered1.mDataStorageDict["轴承朝向"].Value = InputParams[7];

                for (int i = 0; i < 2; i++)
                {
                    var tapered = tapereds[i];
                    tapered.mDataStorageDict["轴承内径d"].Value = InputParams[8];
                    tapered.mDataStorageDict["轴承外径D"].Value = InputParams[9];
                    tapered.mDataStorageDict["轴承宽度T"].Value = InputParams[10];
                    tapered.mDataStorageDict["轴承宽度B"].Value = InputParams[11];
                    tapered.mDataStorageDict["轴承宽度C"].Value = InputParams[12];
                    tapered.mDataStorageDict["基本额定动载荷Cr"].Value = InputParams[13];
                    tapered.mDataStorageDict["基本额定静载荷C0r"].Value = InputParams[14];
                    tapered.mDataStorageDict["计算系数e"].Value = InputParams[15];
                    tapered.mDataStorageDict["计算系数Y"].Value = InputParams[16];
                    tapered.mDataStorageDict["计算系数Y0"].Value = InputParams[17];
                    tapered.mDataStorageDict["作用点距离左端面距离a"].Value = InputParams[18];
                    tapered.mDataStorageDict["轴承类型"].Value = CTANBearing.TaperedRollerBearing;
                }
            }
            else if (InputParams[20] == 1004)
            {
                if (tapered0 != null)
                {
                    treeViewItem.Items.Remove(tapered0.mTreeViewItem);
                    bearingList.Remove(tapered0);
                    BearingCount--;
                }
                if (tapered1 != null)
                {
                    treeViewItem.Items.Remove(tapered1.mTreeViewItem);
                    bearingList.Remove(tapered1);
                    BearingCount--;
                }
                if (angularContactBall0 == null)
                {
                    angularContactBall0 = new CTANBearing();
                    angularContactBall0.mTreeViewItem.Header = "左侧角接触球轴承";
                    angularContactBall0.mTreeViewItem.Name = "左侧角接触球轴承";
                    bearingList.Add(angularContactBall0);
                    treeViewItem.Items.Add(angularContactBall0.mTreeViewItem);
                    BearingCount++;
                }
                if (angularContactBall1 == null)
                {
                    angularContactBall1 = new CTANBearing();
                    angularContactBall1.mTreeViewItem.Header = "右侧角接触球轴承";
                    angularContactBall1.mTreeViewItem.Name = "右侧角接触球轴承";
                    bearingList.Add(angularContactBall1);
                    treeViewItem.Items.Add(angularContactBall1.mTreeViewItem);
                    BearingCount++;
                }

                var tapereds = new[] { angularContactBall0, angularContactBall1 };
                //更新两个圆锥滚子轴承的定位参数 
                angularContactBall0.mDataStorageDict["轴承定位面"].Value = InputParams[2];
                angularContactBall0.mDataStorageDict["轴承定位面距基准面距离"].Value = InputParams[3];
                angularContactBall0.mDataStorageDict["轴承朝向"].Value = InputParams[4];
                angularContactBall1.mDataStorageDict["轴承定位面"].Value = InputParams[5];
                angularContactBall1.mDataStorageDict["轴承定位面距基准面距离"].Value = InputParams[6];
                angularContactBall1.mDataStorageDict["轴承朝向"].Value = InputParams[7];

                for (int i = 0; i < 2; i++)
                {
                    var angularContactBall = tapereds[i];
                    angularContactBall.mDataStorageDict["轴承内径d"].Value = InputParams[8];
                    angularContactBall.mDataStorageDict["轴承外径D"].Value = InputParams[9];
                    angularContactBall.mDataStorageDict["轴承宽度B"].Value = InputParams[10];
                    angularContactBall.mDataStorageDict["接触角"].Value = Math.Round(InputParams[11] * 180 / Math.PI);
                    angularContactBall.mDataStorageDict["基本额定动载荷Cr"].Value = InputParams[13];
                    angularContactBall.mDataStorageDict["基本额定静载荷C0r"].Value = InputParams[14];
                    angularContactBall.mDataStorageDict["作用点距离左端面距离a"].Value = InputParams[18];
                    angularContactBall.mDataStorageDict["轴承类型"].Value = CTANBearing.AngularContactBallBearing;
                }
            }

            
        }
#endregion
    }
}
