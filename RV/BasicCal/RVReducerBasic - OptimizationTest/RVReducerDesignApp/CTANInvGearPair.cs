using HandyControl.Controls;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Reflection.PortableExecutable;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using TabControl = System.Windows.Controls.TabControl;
using TabItem = System.Windows.Controls.TabItem;
namespace RVReducerDesignApp
{
    public class CTANTorque : CTANBasicContentPageObj
    {
        
        public static string[] mGeometryParaText = new string[]{"扭矩","牛米",
            "转速", "转每分钟",
            "预期寿命","小时"
        };
        //public Grid mPageGrid = new Grid();
        //public CTANControlGrid mTorqueInputGrid = new CTANControlGrid();
        //public CTANDict mDataStorageDict = new CTANDict();
        public CTANTorque()
        {
            mTreeViewItem.Header = "太阳轮载荷";
            mTreeViewItem.Name = "太阳轮载荷";
            SetInputPage();
            //界面布局,创建界面网络

            //mTorqueInputGrid.SetStandartInputGrid(1, mHeadText, mGeometryParaText);
            //mDataStorageDict.SetDataBindingWithTextBoxMatrix(1,
            //    mTorqueInputGrid.mGrid.RowDefinitions.Count - 1,
            //    0,
            //    2,
            //    mTorqueInputGrid.mTextBox);
            //Grid.SetRow(mTorqueInputGrid.mGrid,0);
            //mPageGrid.Children.Add(mTorqueInputGrid.mGrid);
            ////几何参数
            //SetInvGearGrid(TmpPageAddIndex, "几何参数", mGeometryParaText, mGroupBoxList, mPairParaTextGridList[TmpPageAddIndex]);
            //CTANPageLayoutFunction.ReplaceTheGridTextBoxByComboBox(mPairParaTextGridList[TmpPageAddIndex].mTextBox[4][2],
            //    [-1, 1],
            //    ["左旋", "右旋"],
            //    mPairParaTextGridList[TmpPageAddIndex].mTextBox[4][0].Text,
            //    mSprialDirectCombo,
            //    mDataStorageDict,
            //    mPairParaTextGridList[TmpPageAddIndex].mGrid);
            //mDataStorageDict["旋向"].Value = 0;
            //TmpPageAddIndex++;
        }
        private void SetInputPage()
        {
            //tmpGridIndex = 0;
            //tmpCorrentSetInputStringList = mGeometryParaText;
            //mSpiralDirectCombo = mInputGridList[tmpGridIndex].SetGridControlByCreateComboBox(4, 2, [-1, 1], ["左旋", "右旋"]);
            //mInputGridList[tmpGridIndex].SetStandartInputGroup("几何参数", mHeadText, tmpCorrentSetInputStringList, mDataStorageDict);
            mDataStorageDict.SetDataStorageDictionary(0, 2, mGeometryParaText.Count() / 2, mGeometryParaText);
            mInputPageGrid.SetGridRowAndColumn(1, 1);
            CTANBasicFunction.CreatObjectList(1, out mInputGridList);
            mInputGridList[0].SetStandartInputGroup("几何参数", mHeadText, mGeometryParaText, mDataStorageDict);
            mInputPageGrid.Children.Add(mInputGridList[0].mGroupBox);
        }
    }
    //[StructLayout(LayoutKind.Sequential)]
    //public struct InvGearObjInC
    //{
    //    public double[,] GeometryData;
    //    public double[,] MaterialData;
    //    public double[,] AppData;
    //    public double[,] TolerenceData;
    //    public double[,] LoadData;
    //    public double[,] AssembleData;
    //    public double[,] LocData;
    //}
    public class CTANInvGear : CTANBasicContentPageObj
    {
        [DllImport(@"RVReducerBasic.dll", EntryPoint = "CalInvGearTolerence", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern void CalInvGearTolerence(ref double pInPara,ref double pOutPara);
        //public CTANDict mDataStorageDict = new CTANDict();
        //public static string[] mHeadText = new string[] { "名称", "单位", "数值" };
        CTANComboBox[] mComboBoxList;
        public static string[] mGeometryParaText = new string[]{
            "法向模数","毫米",
            "压力角", "度" ,
            "螺旋角", "度",
            "旋向", "",
            "中心距", "毫米",
            "齿数", "个",
            "设计齿宽", "毫米",
            "齿顶高系数", "",
            "齿廓变位系数","",
            "顶隙系数", "",
            "齿根圆角系数", "",
            "挖根量", "毫米",
            "齿顶修形量", "微米",
            "齿根修形量", "微米",            
            "跨齿数", "",
            "齿顶倒角量", "毫米",
            "轮幅外圆直径","毫米",           
        };
        public static string[] mOtherGeometryParaText = new string[]{
            "是否使用滚动角","",
            "齿轮齿顶高变化[k·mn]","",
            "齿厚的偏差上限[E_sns]","",
            "齿厚的偏差下限[E_sni]","",
            "齿顶圆容差上限","",
            "齿顶圆容差下限","",
            "最大机械加工余量[q_max]","",
            "最小机械加工余量[q_min]","",
            "Y圆直径[d_y]","",
            "刀具类型","",
            "齿条刀的齿顶高系数","",
            "齿条刀的齿顶圆半径系数","",
            "插齿刀齿数","",
            "插齿刀加工中心距","毫米",
            "插齿刀具齿顶直径","毫米",
            "插齿刀基圆直径","毫米",
            "插齿刀齿顶形状直径","毫米",
            "插齿刀具参数横向压力角[alpha_wt0]","度",
            "齿轮副类型","",
            "行星轮个数","",
            "驱动齿轮","",
            "触点间隙[j_bn]","",
        };
        //public static string[] mMaterialParaText = new string[]{"弹性模量","兆帕",
        //    "泊松比", "" ,
        //    "屈服强度", "兆帕",
        //    "抗拉强度", "兆帕",            
        //    "试验齿轮接触疲劳极限应力", "兆帕",
        //    "试验齿轮弯曲疲劳极限应力", "兆帕",
        //    "密度", "千克每立方毫米",
        //    "",""//占位，用于添加导入按钮
        //};
        public static string[] mMaterialParaText = new string[]{"弹性模量","兆帕",
            "泊松比", "" ,
            "屈服强度", "兆帕",
            "抗拉强度", "兆帕",
            "试验齿轮接触疲劳极限应力", "兆帕",
            "试验齿轮弯曲疲劳极限应力", "兆帕",
            "密度", "千克每立方毫米",
            "",""//占位，用于添加导入按钮
        };
        public static string[] mManufactureParaText = new string[]{"材料类型","",
            "加工质量", "" ,
            "表面硬度(HRC)", "",
            "芯部硬度(HRC)", "",
            "表面是否喷丸", "",
            "是否允许表面出现有限点蚀", ""
        };
        public static string[] mApplicationParaText = new string[]{"使用系数","",
            "均载系数", "" ,
            "润滑油粘度(v40)", "平方毫米每秒",            
            "接触最小安全系数", "",
            "弯曲最小安全系数", "",
            "行星轮个数", "个",
            "修形方法","",
            "螺旋线修形方式", "",
            "是否对接触区进行控制", "",
            "齿廓修形是否能够补偿变形", "",            
        };
        public static string[] mTolerenceParaText = new string[]{"精度等级","",
            "端面节线偏差", "微米" ,
            "齿廓偏差", "微米",
            "螺旋线偏差", "微米",
            "接触表面粗糙度(Ra)", "微米",
            "齿根表面粗糙度(Ra)", "微米"
        };
        public static string[] mAssembleParaText = new string[]{"安装类型","",
            "是否为刚性轴", "" ,
            "太阳轮轴直径", "毫米",
            "太阳轮齿宽中点至轴承跨距中点距离", "毫米",
            "轴承跨距", "毫米"
        };
        public static string[] mOutPutHeadName = new string[]
        {
            "名义接触应力","兆帕",//sigma_H0
            "计算接触应力","兆帕",//sigma_H
            "许用接触应力","兆帕",//sigma_HP
            "名义弯曲应力","兆帕",//sigma_F0
            "计算弯曲应力","兆帕",//sigma_F
            "许用弯曲应力","兆帕",//sigma_FP      
            "传动比[i]", "",
            "齿数比[u]", "",
            "端面模数[mt]","毫米",
            "轴向模数[mx]","毫米",
            "端面理论压力角[αt]","度",
            "端面啮合角[αwt]","度",
            "基圆螺旋角[βb]","度",
            "中心距离[aw]","毫米",
            "齿廓变位总系数[Σx]","",
            "产形齿廓变位系数[xEs]","",
            "产形齿廓变位系数[xEi]","",
            "等效齿数[zn]","",
            "基准齿条齿顶高[haP]","毫米",
            "齿根高系数[hfx]","",
            "基准齿条齿根高[hfP]","毫米",
            "分度圆直径[d]","毫米",
            "基圆直径[db]","毫米",
            "齿顶圆直径[da]","毫米",
            "d_a[max]","",
            "d_a[min]","",
            "齿顶成形圆直径[dFa]","毫米",
            "d_Fa[max]","",
            "d_Fa[min]","",
            "有效齿顶圆直径[dNa]","",
            "工作节圆直径[dw]","毫米",
            "有效齿根圆直径[dNF]","",
            "齿根成形圆直径[dFf]","毫米",
            "d_Ff[max]","",
            "d_Ff[min]","",
            "齿根圆直径[df]","毫米",
            "d_f[max]","",
            "d_f[min]","",
            "理论齿顶间隙[c]","毫米",
            "储备(dNf-dFf)/2[cF]", "毫米",
            "齿顶高[ha]","毫米",
            "h_a[max]","",
            "h_a[min]","",
            "齿根高[hf]","毫米",
            "h_f[max]","",
            "h_f[min]","",
            "全齿高[h]","毫米",
            "分度圆节距[pt]","毫米",
            "法向节距[pn]","毫米",            
            "基圆节距[pbt]","毫米",
            "基圆法向节距[pbn]","毫米",
            "端面啮合齿距[pet]","毫米",
            "法向啮合齿距[pen]","毫米",
            "导程[pz]","毫米",
            "轴向节距[px]","毫米",
            "[ω1]","",
            "法向速度[vn]","",
            "齿顶滑动速度[vga]","米/秒",
            "齿根处的滑动速度[vgf]","",
            "齿根上的比滑[ζf]","",
            "齿顶上的滑动系数[Kga]","",
            "齿根上的滑动系数[Kgf]","",
            "端面重合度[εα]","",
            "纵向重合度[εβ]","",
            "总重合度[εγ]","",
            "接触路径长度[gα]","毫米",
            "端面作用角[φα]","度",
            "纵向作用角[φβ]","度",
            "总作用角[φγ]","度",
            "间隙角[φj]","度",
            "[T2A]","",
            "[T1B]","",
            "[T1C]","",
            "[T2C]","",
            "[T2D]","",
            "[T1E]","",
            "[T1T2]","",
            "[ρA1]","",
            "[ρA2]","",
            "[ρB1]","",
            "[ρC1]","",
            "[ρC2]","",
            "[ρD2]","",
            "[ρE1]","",
            "[ρE2]","",            
            "平均齿廓直径[dm]","毫米",
            "基圆半径[rb]","毫米",
            "平均齿廓半径[rm]","毫米",
            "齿顶圆半径[ra]","毫米",
            "齿根圆半径[rf]","毫米",
            "渐开线函数[invαt]","",
            "齿距角[τ]","度",                                                
            "跨棒距[Wk]","毫米",
            "分度圆法向齿厚[sn]","",
            "接触齿顶路径的长度[ga]","",
            "接触齿根路径的长度[gf]","",
            "纵向重合弧长度[gβ]","",
            "参与路径[lmax]","",
            "节圆处的周向间隙[jwt]","",
            "分度圆处的圆周间隙[jt]","",
            "径向间隙[jr]","",            
            "齿顶修形量[Ca]","微米",
            "刀尖圆角半径[ρfp]","毫米",
            "齿条型刀具齿顶高[hFaP0]", "",
            "无根切变位系数[xEmin]", "",
            "齿轮副中齿的工作深度[hw]","",            
            "非零背差的齿廓偏移系数之和[ΣxE]","",          
            "分度圆端面齿厚[st]","",
            "分度圆齿厚半角[ψ]","度",
            "基圆齿厚半角[ψb]","度",
            "分度圆齿槽宽[et]","",
            "分度圆齿槽宽半角[η]","度",
            "基圆齿槽宽半角[ηb]","度",
            "分度圆法向齿槽宽[en]","",
            "分度圆上的最大正常齿厚度[sns]","",
            "分度圆上的最小正常齿厚度[sni]","",
            "齿厚公差[Tsn]","",
            "齿厚公差[Tsnv]","",
            "[xEsV]", "",
            "[xEiV]", "",
            "V圆直径[dv]","",
            "点Y到节点C的距离[gαy]","",
            "滑动速度[vg]","",
            "滑动系数[Kg]","",
            "比滑[ζ]","",
            "Y圆螺旋角[βy]","度",
            "Y圆导程角[γy]","度",
            "Y圆端面压力角[αyt]","度",
            "Y圆法向压力角[αyn]","度",
            "Y圆渐开线上Y点滚动角[ξy]","度",
            "Y圆渐开线曲率半径[ρy]","",
            "Y圆渐开线函数[invαyt]","",
            "Y圆端面齿距[pyt]","",
            "Y圆法向齿距[pyn]","",
            "Y圆端面齿厚[syt]","",
            "Y圆齿厚半角[ψy]","度",
            "Y圆齿槽宽[eyt]","",
            "Y圆齿槽宽半角[ηy]","度",
            "Y圆法向齿厚[syn]","",
            "Y圆法向齿槽宽[eyn]","",
            "起评点","",
            "终评点","",
        };
        public static string[] mOutStressName = new string[]
        {
            "名义接触应力","兆帕",
            "计算接触应力","兆帕",
            "许用接触应力","兆帕",
            "名义弯曲应力","兆帕",
            "计算弯曲应力","兆帕",
            "许用弯曲应力","兆帕"
        };

        public static string[] mOutGeometryName = new string[]
        {
            "分度圆直径[d]","毫米",
            "端面模数[mt]","毫米",
            "轴向模数[mx]","毫米",
            "导程[pz]","毫米",
            "基圆螺旋角[βb]","度",
            "端面理论压力角[αt]","度",
            "基圆半径[rb]","毫米",
            "基圆直径[db]","毫米",
            "齿距角[τ]","度",
            "法向节距[pn]","毫米",
            "分度圆节距[pt]","毫米",
            "轴向节距[px]","毫米",                
            "基圆节距[pbt]","毫米",
            "基圆法向节距[pbn]","毫米",
            "端面啮合齿距[pet]","毫米",
            "法向啮合齿距[pen]","毫米",
            "V圆直径[dv]","",
            "齿顶圆半径[ra]","毫米",
            "齿根圆半径[rf]","毫米",
            "齿顶圆直径[da]","毫米",
            "d_a[max]","",
            "d_a[min]","",
            "齿根圆直径[df]","毫米",
            "d_f[max]","",
            "d_f[min]","",
            "全齿高[h]","毫米",
            "齿顶高[ha]","毫米",
            "h_a[max]","",
            "h_a[min]","",
            "齿根高[hf]","毫米",
            "h_f[max]","",
            "h_f[min]","",
            "齿根高系数[hfx]","",
            "等效齿数[zn]","",
            "基准齿条齿顶高[haP]","毫米",
            "基准齿条齿根高[hfP]","毫米",
            "起评点","",
            "终评点","",
        };

        public static string[] mOutContactName = new string[]
        {
            //接触参数
            "传动比[i]", "",
            "齿数比[u]", "",
            "端面啮合角[αwt]","度",
            "渐开线函数[invαt]","",
            "工作节圆直径[dw]","毫米",
            "中心距离[aw]","毫米",
            "齿轮副中齿的工作深度[hw]","",
            "理论齿顶间隙[c]","毫米",
            "齿廓变位总系数[Σx]","",    
            "非零背差的齿廓偏移系数之和[ΣxE]","",
            "产形齿廓变位系数[xEs]","",
            "产形齿廓变位系数[xEi]","",
            "有效齿顶圆直径[dNa]","",
            "有效齿根圆直径[dNF]","",
            "储备(dNf-dFf)/2[cF]", "毫米",
            "接触路径长度[gα]","毫米",
            "接触齿顶路径的长度[ga]","",
            "接触齿根路径的长度[gf]","",
            "端面作用角[φα]","度",
            "端面重合度[εα]","",
            "纵向作用角[φβ]","度",
            "纵向重合度[εβ]","",
            "纵向重合弧长度[gβ]","",
            "总作用角[φγ]","度",
            "总重合度[εγ]","",
            "参与路径[lmax]","",
            "间隙角[φj]","度",
            "节圆处的周向间隙[jwt]","",
            "分度圆处的圆周间隙[jt]","",
            "径向间隙[jr]","",
            "[T1C]","",
            "[T2C]","",
            "[T2A]","",
            "[T1E]","",
            "[T1B]","",
            "[T2D]","",
            "[T1T2]","",
            "[ρA1]","",
            "[ρA2]","",
            "[ρB1]","",
            "[ρC1]","",
            "[ρC2]","",
            "[ρD2]","",
            "[ρE1]","",
            "[ρE2]","",
        };
        public static string[] mOutOthersName = new string[]
        {
            //其他参数
            "[ω1]","",
            "跨棒距[Wk]","毫米",
            "分度圆上的最大正常齿厚度[sns]","",
            "分度圆上的最小正常齿厚度[sni]","",
            "齿顶成形圆直径[dFa]","毫米",
            "d_Fa[max]","",
            "d_Fa[min]","",
            "齿根成形圆直径[dFf]","毫米",
            "d_Ff[max]","",
            "d_Ff[min]","",
            "无根切变位系数[xEmin]", "",
            "[xEsV]", "",
            "[xEiV]", "",
            "齿厚公差[Tsn]","",
            "齿厚公差[Tsnv]","",
            "齿顶修形量[Ca]","微米",
            "刀尖圆角半径[ρfp]","毫米",
            "齿条型刀具齿顶高[hFaP0]", "",
        };
        public static string[] mOutSlideName = new string[]
        {
            //滑动条件
            "法向速度[vn]","",
            "滑动速度[vg]","",
            "齿顶滑动速度[vga]","米/秒",
            "齿根处的滑动速度[vgf]","",
            "滑动系数[Kg]","",
            "齿顶上的滑动系数[Kga]","",
            "齿根上的滑动系数[Kgf]","",
            "比滑[ζ]","",
            "齿根上的比滑[ζf]","",
        };
        public static string[] mOutSpecialName = new string[]
        {
            // 特殊计算参数（Y圆）  
            "点Y到节点C的距离[gαy]","",
            "Y圆螺旋角[βy]","度",
            "Y圆导程角[γy]","度",
            "Y圆端面压力角[αyt]","度",
            "Y圆法向压力角[αyn]","度",
            "Y圆渐开线上Y点滚动角[ξy]","度",
            "Y圆渐开线曲率半径[ρy]","",
            "Y圆渐开线函数[invαyt]","",
            "Y圆端面齿距[pyt]","",
            "Y圆法向齿距[pyn]","",
            "Y圆端面齿厚[syt]","",
            "Y圆齿厚半角[ψy]","度",
            "Y圆齿槽宽[eyt]","",
            "Y圆齿槽宽半角[ηy]","度",
            "Y圆法向齿厚[syn]","",
            "Y圆法向齿槽宽[eyn]",""
        };
        public static string[] mOutToothProfileName = new string[]
        {
            // 齿廓参数
            "平均齿廓直径[dm]","毫米",
            "平均齿廓半径[rm]","毫米",
            "分度圆端面齿厚[st]","",
            "分度圆齿厚半角[ψ]","度",
            "基圆齿厚半角[ψb]","度",
            "分度圆齿槽宽[et]","",
            "分度圆齿槽宽半角[η]","度",
            "基圆齿槽宽半角[ηb]","度",
            "分度圆法向齿厚[sn]","",
            "分度圆法向齿槽宽[en]","",   
        };
        public static string[] mMaterialTypeText = new string[]{"St正火低碳钢",
            "St[Cast]铸钢",
            "GTS[perl]黑心可锻铸铁（珠光体）",
            "GGG[perl]球墨铸铁(珠光体)",
            "GGG[bai]球墨铸铁(贝氏体)",
            "GGG[ferr]球墨铸铁(铁素体)",
            "GG灰铸铁",
            "V整体淬硬锻钢" ,
            "V[Cast]整体淬硬铸钢", 
            "Eh表面硬化铸钢",
            "IF火焰或感应淬火钢",
            "NTnitr调质-气体渗氮渗氮钢",
            "NVnitr调质-气体渗氮调质钢",
            "NVnitrcar调质或正火-碳氮共渗钢调质钢"
        };
        public static string[] mOtherText = new string[]
        {
            "轮齿是否反向弯折",
            "齿坯类型",
            "轴承间距偏移量"
        };
        //public static string[] mShaftAssembleText = new string[]
        //{
        //            "定位面","",
        //            "定位面距基准面距离","毫米"
                    
        //};
        public CTANButton mImportMaterialBtn=new CTANButton();
        public CTANComboBox mTolerenceCombo = new CTANComboBox();
        public CTANComboBox mSpiralDirectCombo;
        private TabControl mInnerTabControl;
        private TabControl mOutTabControl;
        public CTANDataTable[] mOutputTables; // 添加这个数组声明

        public DrawGear CanvasGraphic { get; set; }
        public CTANInvGear()
        {             
            SetInputPage();
            SetPropertyPage();
            SetOutputPage();
            mDataStorageDict["轮齿是否反向弯折"].Value = 0;
            mDataStorageDict["齿坯类型"].Value = 1;
            mDataStorageDict["轴承间距偏移量"].Value = 0;            
            mDataStorageDict["名义接触应力"].Value = 50;
            SetExamplePage();
        }
        private void SetExamplePage()
        {
            mImage.Source = new BitmapImage(new Uri(@"Resource\Figure\Gear\InvoluteGear.png", UriKind.RelativeOrAbsolute));
            mExamplePageGrid.Children.Add(mImage);
        }
        private void mTolerenceCombo_SelectionChanged(object sender, RoutedEventArgs e)
        {
            if (mDataStorageDict["法向模数"].Value>0&& mDataStorageDict["设计齿宽"].Value > 0 && mDataStorageDict["齿数"].Value > 0)
            {
                double[] TmpIOData = new double[10];
                TmpIOData[0] = mDataStorageDict["精度等级"].Value;
                TmpIOData[1] = mDataStorageDict["法向模数"].Value;
                TmpIOData[2] = mDataStorageDict["设计齿宽"].Value;
                TmpIOData[3] = mDataStorageDict["齿数"].Value;
                TmpIOData[4] = mDataStorageDict["螺旋角"].Value;
                CalInvGearTolerence(ref TmpIOData[0],ref TmpIOData[5]);
                mDataStorageDict["端面节线偏差"].Value = TmpIOData[5];
                mDataStorageDict["齿廓偏差"].Value = TmpIOData[6];
                mDataStorageDict["螺旋线偏差"].Value = TmpIOData[7];
                //mDataStorageDict["法向模数"].Value
            }
        }
        public void ImportGeneralMaterialParaBtn_Clicked(object sender, RoutedEventArgs e)
        {
            mDataStorageDict["弹性模量"].Value = 2.06e5;
            mDataStorageDict["泊松比"].Value = 0.3;
            mDataStorageDict["屈服强度"].Value = 500;
            mDataStorageDict["抗拉强度"].Value = 1080;
            mDataStorageDict["试验齿轮接触疲劳极限应力"].Value = 1500;
            mDataStorageDict["试验齿轮弯曲疲劳极限应力"].Value = 500;
            mDataStorageDict["密度"].Value = 7.83e-6;
            e.Handled = true;
        }
        public void SetPropertyPage()
        {
            Image mImagePropertyPage = new Image();
            mImagePropertyPage.Source = new BitmapImage(new Uri(@"Resource\Figure\Gear\InvoluteGear.png", UriKind.RelativeOrAbsolute));
            Magnifier customMagnifier = new Magnifier
            {
                Scale = 6,                  // 放大倍数
                HorizontalOffset = -99,     // 水平偏移
                VerticalOffset = -99       // 垂直偏移
            };
            Magnifier.SetInstance(mImagePropertyPage, customMagnifier);
            mPropertyGrid.Children.Add(mImagePropertyPage);
        }
        private void DefineDataStorageDictionary()
        {
            mDataStorageDict.SetDataStorageDictionary(0, 2, mGeometryParaText.Count() / 2, mGeometryParaText);
            mDataStorageDict.SetDataStorageDictionary(0, 2, mOtherGeometryParaText.Count() / 2, mOtherGeometryParaText);
            mDataStorageDict.SetDataStorageDictionary(0, 2, mMaterialParaText.Count() / 2 , mMaterialParaText);
            mDataStorageDict.SetDataStorageDictionary(0, 2, mManufactureParaText.Count() / 2, mManufactureParaText);
            mDataStorageDict.SetDataStorageDictionary(0, 2, mApplicationParaText.Count() / 2, mApplicationParaText);
            mDataStorageDict.SetDataStorageDictionary(0, 2, mTolerenceParaText.Count() / 2, mTolerenceParaText);
            mDataStorageDict.SetDataStorageDictionary(0, 2, mAssembleParaText.Count() / 2, mAssembleParaText);
            mDataStorageDict.SetDataStorageDictionary(0, 2, mShaftAssembleText.Count() / 2, mShaftAssembleText);
            for (int i = 0; i < mOtherText.Count(); i++)
            {
                mDataStorageDict.Add(mOtherText[i], new CTANDouble() { Value = 0 });

            }
        }
        private void SetInputPage()
        {
            DefineDataStorageDictionary();           
            CreatInputContentInnerTabControl();
            int tmpDisplayColumnCount = 2;
            int[] tmpInputTextRangeTalbe = new int[4] { 1, 0, tmpDisplayColumnCount, 1 };
            int tmpInputRowCount = 0;
            int tmpBindingKeyIncre = 2;
            int tmpInputColumnIndex = tmpDisplayColumnCount;
            string[] tmpCorrentSetInputStringList;
            int tmpGridIndex = 0;
            //几何参数框
            tmpGridIndex = 0;
            tmpCorrentSetInputStringList = mGeometryParaText;
            //mSpiralDirectCombo = mInputGridList[tmpGridIndex].SetGridControlByCreateComboBox(4, 2, [-1, 1], ["左旋", "右旋"]);           
            mInputGridList[tmpGridIndex].SetStandartDisplayText(mHeadText, tmpCorrentSetInputStringList, tmpDisplayColumnCount);
            mInputGridList[tmpGridIndex].SetGridControlByCreateComboBox("旋向", tmpInputColumnIndex, [-1, 1], ["左旋", "右旋"]);           
            mInputGridList[tmpGridIndex].SetStandartInputGroup("几何参数", mHeadText, tmpCorrentSetInputStringList, mDataStorageDict);
            //其他几何参数
            tmpGridIndex++;
            tmpCorrentSetInputStringList = mOtherGeometryParaText;
            mInputGridList[tmpGridIndex].SetStandartDisplayText(mHeadText, tmpCorrentSetInputStringList, tmpDisplayColumnCount);
            mInputGridList[tmpGridIndex].SetGridControlByCreateComboBox("是否使用滚动角", tmpInputColumnIndex, [0, 1], ["否", "是"]);
            mInputGridList[tmpGridIndex].SetGridControlByCreateComboBox("驱动齿轮", tmpInputColumnIndex, [0, 1], ["小齿轮", "大齿轮"]);
            mInputGridList[tmpGridIndex].SetGridControlByCreateComboBox("齿轮副类型", tmpInputColumnIndex, [1, 2, 3, 4, 1001],
                ["一般外啮合", "内啮合，太阳轮比行星轮", "内啮合，行星轮比内齿圈,内齿圈固定", "内啮合，行星轮比内齿圈,内齿圈转动", "人字齿外啮合"]);
            mInputGridList[tmpGridIndex].SetGridControlByCreateComboBox("刀具类型", tmpInputColumnIndex, [0, 1], ["齿条刀", "插齿刀"]);
            mInputGridList[tmpGridIndex].SetStandartInputGroup("其他几何参数", mHeadText, tmpCorrentSetInputStringList, mDataStorageDict);
            //物性参数
            tmpGridIndex++;
            tmpCorrentSetInputStringList = mMaterialParaText;
            mInputGridList[tmpGridIndex].SetStandartDisplayText(mHeadText, tmpCorrentSetInputStringList, tmpDisplayColumnCount);
            mImportMaterialBtn.Content = "设为一般合金钢参数";
            mImportMaterialBtn.AddClickEvent(ImportGeneralMaterialParaBtn_Clicked);
            tmpInputRowCount = tmpCorrentSetInputStringList.Length / tmpDisplayColumnCount;
            mInputGridList[tmpGridIndex].AssignControlToGrid(tmpInputRowCount, tmpInputColumnIndex, mImportMaterialBtn);
            tmpInputTextRangeTalbe[1] = tmpInputRowCount;
            mInputGridList[tmpGridIndex].SetStandartInputTextAndBingdingToDict("物性参数", tmpInputTextRangeTalbe, tmpCorrentSetInputStringList, tmpBindingKeyIncre, mDataStorageDict);
            //mInputGridList[1].SetStandartInputGroup("物性参数", mHeadText, mMaterialParaText, mDataStorageDict);
            ////加工参数
            tmpGridIndex++;
            tmpCorrentSetInputStringList = mManufactureParaText;
            mInputGridList[tmpGridIndex].SetStandartDisplayText(mHeadText, tmpCorrentSetInputStringList, tmpDisplayColumnCount);
            mInputGridList[tmpGridIndex].SetGridControlByCreateComboBox("材料类型", tmpInputColumnIndex, mMaterialTypeText);
            mInputGridList[tmpGridIndex].SetGridControlByCreateComboBox("加工质量", tmpInputColumnIndex, ["ML最低要求材料及热处理质量", "MQ中等要求材料及热处理质量", "ME很高要求材料及热处理质量"]);
            mInputGridList[tmpGridIndex].SetGridControlByCreateComboBox("表面是否喷丸", tmpInputColumnIndex, ["否", "是"]);
            mInputGridList[tmpGridIndex].SetGridControlByCreateComboBox("是否允许表面出现有限点蚀", tmpInputColumnIndex, ["否", "是"]);
            tmpInputRowCount = tmpCorrentSetInputStringList.Length / 2;
            tmpInputTextRangeTalbe[1] = tmpInputRowCount;
            mInputGridList[tmpGridIndex].SetStandartInputTextAndBingdingToDict("加工参数", tmpInputTextRangeTalbe, tmpCorrentSetInputStringList, tmpBindingKeyIncre, mDataStorageDict);
            ////应用参数
            tmpGridIndex++;
            tmpCorrentSetInputStringList = mApplicationParaText;
            mInputGridList[tmpGridIndex].SetStandartDisplayText(mHeadText, tmpCorrentSetInputStringList, tmpDisplayColumnCount);
            mInputGridList[tmpGridIndex].SetGridControlByCreateComboBox("修形方法", tmpInputColumnIndex, [1, 2, 3], ["基于3维接触分析进行适当修形", "基于经验进行修形", "不修形"]);
            mInputGridList[tmpGridIndex].SetGridControlByCreateComboBox("螺旋线修形方式", tmpInputColumnIndex,
                [1, 3, 4, 5, 6], ["不修形", "仅中心鼓形修形", "仅螺旋线修正", "螺旋线修正且中心鼓形修形", "齿端修薄"]);
            mInputGridList[tmpGridIndex].SetGridControlByCreateComboBox("是否对接触区进行控制", tmpInputColumnIndex, ["不控制", "接触区控制良好"]);
            mInputGridList[tmpGridIndex].SetGridControlByCreateComboBox("齿廓修形是否能够补偿变形", tmpInputColumnIndex, ["否", "是"]);
            tmpInputRowCount = tmpCorrentSetInputStringList.Length / 2;
            tmpInputTextRangeTalbe[1] = tmpInputRowCount;
            mInputGridList[tmpGridIndex].SetStandartInputTextAndBingdingToDict("应用参数", tmpInputTextRangeTalbe, tmpCorrentSetInputStringList, tmpBindingKeyIncre, mDataStorageDict);
            ////精度参数
            tmpGridIndex++;
            tmpCorrentSetInputStringList = mTolerenceParaText;
            mInputGridList[tmpGridIndex].SetStandartDisplayText(mHeadText, tmpCorrentSetInputStringList, tmpDisplayColumnCount);
            mTolerenceCombo.SetComboBoxWithKeyValue([3, 4, 5, 6, 7], ["3级", "4级", "5级", "6级", "7级"]);
            mTolerenceCombo.SelectionChanged += mTolerenceCombo_SelectionChanged;
            mInputGridList[tmpGridIndex].AssignControlToGrid("精度等级", tmpInputColumnIndex, mTolerenceCombo);
            tmpInputRowCount = tmpCorrentSetInputStringList.Length / 2;
            tmpInputTextRangeTalbe[1] = tmpInputRowCount;
            mInputGridList[tmpGridIndex].SetStandartInputTextAndBingdingToDict("精度参数", tmpInputTextRangeTalbe, tmpCorrentSetInputStringList, tmpBindingKeyIncre, mDataStorageDict);
            ////安装参数
            tmpGridIndex++;
            tmpCorrentSetInputStringList = mAssembleParaText;
            mInputGridList[tmpGridIndex].SetStandartDisplayText(mHeadText, tmpCorrentSetInputStringList, tmpDisplayColumnCount);
            mInputGridList[tmpGridIndex].SetGridControlByCreateComboBox("安装类型", tmpInputColumnIndex, [1, 2, 3, 4, 5], ["A", "B", "C", "D", "E"]);
            mInputGridList[tmpGridIndex].SetGridControlByCreateComboBox("是否为刚性轴", tmpInputColumnIndex, [0, 1], ["否", "是"]);
            tmpInputRowCount = tmpCorrentSetInputStringList.Length / 2;
            tmpInputTextRangeTalbe[1] = tmpInputRowCount;
            mInputGridList[tmpGridIndex].SetStandartInputTextAndBingdingToDict("安装参数", tmpInputTextRangeTalbe, tmpCorrentSetInputStringList, tmpBindingKeyIncre, mDataStorageDict);
            ////定位参数
            tmpGridIndex++;
            tmpCorrentSetInputStringList = mShaftAssembleText;
            mInputGridList[tmpGridIndex].SetStandartDisplayText(mHeadText, tmpCorrentSetInputStringList, tmpDisplayColumnCount);
            mInputGridList[tmpGridIndex].SetGridControlByCreateComboBox("定位面", 2, [-1, 0, 1], ["右端面", "中间点", "左端面"]);
            tmpInputRowCount = tmpCorrentSetInputStringList.Length / 2;
            tmpInputTextRangeTalbe[1] = tmpInputRowCount;
            mInputGridList[tmpGridIndex].SetStandartInputTextAndBingdingToDict("定位参数", tmpInputTextRangeTalbe, tmpCorrentSetInputStringList, tmpBindingKeyIncre, mDataStorageDict);

            SetInputContentInnerTabControlContent();
            InitSomePara();
        }
        //private void SetInputPage()
        //{
        //    DefineDataStorageDictionary();
        //    CreatInputContentGridWithContentTable();
        //    int tmpDisplayColumnCount = 2;
        //    int[] tmpInputTextRangeTalbe = new int[4] { 1, 0, tmpDisplayColumnCount, 1 };
        //    int tmpInputRowCount = 0;
        //    int tmpBindingKeyIncre = 2;
        //    int tmpInputColumnIndex = tmpDisplayColumnCount;
        //    string[] tmpCorrentSetInputStringList;
        //    int tmpGridIndex = 0;
        //    //几何参数框
        //    tmpGridIndex = 0;
        //    tmpCorrentSetInputStringList = mGeometryParaText;
        //    //mSpiralDirectCombo = mInputGridList[tmpGridIndex].SetGridControlByCreateComboBox(4, 2, [-1, 1], ["左旋", "右旋"]);           
        //    mInputGridList[tmpGridIndex].SetStandartDisplayText(mHeadText, tmpCorrentSetInputStringList, tmpDisplayColumnCount);
        //    mInputGridList[tmpGridIndex].SetGridControlByCreateComboBox("旋向", tmpInputColumnIndex, [-1, 1], ["左旋", "右旋"]);
        //    mInputGridList[tmpGridIndex].SetGridControlByCreateComboBox("是否使用滚动角", tmpInputColumnIndex, [0, 1], ["否", "是"]);
        //    mInputGridList[tmpGridIndex].SetStandartInputGroup("几何参数", mHeadText, tmpCorrentSetInputStringList, mDataStorageDict);
        //    //其他几何参数
        //    tmpGridIndex++;
        //    tmpCorrentSetInputStringList = mOtherGeometryParaText;
        //    mInputGridList[tmpGridIndex].SetStandartDisplayText(mHeadText, tmpCorrentSetInputStringList, tmpDisplayColumnCount);
        //    mInputGridList[tmpGridIndex].SetGridControlByCreateComboBox("驱动齿轮", tmpInputColumnIndex, [0, 1], ["小齿轮", "大齿轮"]);
        //    mInputGridList[tmpGridIndex].SetGridControlByCreateComboBox("齿轮副类型", tmpInputColumnIndex, [1, 2, 3, 4, 1001],
        //        ["一般外啮合", "内啮合，太阳轮比行星轮", "内啮合，行星轮比内齿圈,内齿圈固定", "内啮合，行星轮比内齿圈,内齿圈转动", "人字齿外啮合"]);
        //    mInputGridList[tmpGridIndex].SetGridControlByCreateComboBox("刀具类型", tmpInputColumnIndex, [0, 1], ["齿条刀", "插齿刀"]);
        //    mInputGridList[tmpGridIndex].SetStandartInputGroup("其他几何参数", mHeadText, tmpCorrentSetInputStringList, mDataStorageDict);
        //    //物性参数
        //    tmpGridIndex++;
        //    tmpCorrentSetInputStringList = mMaterialParaText;
        //    mInputGridList[tmpGridIndex].SetStandartDisplayText(mHeadText, tmpCorrentSetInputStringList, tmpDisplayColumnCount);
        //    mImportMaterialBtn.Content = "设为一般合金钢参数";
        //    mImportMaterialBtn.AddClickEvent(ImportGeneralMaterialParaBtn_Clicked);
        //    tmpInputRowCount = tmpCorrentSetInputStringList.Length / tmpDisplayColumnCount;
        //    mInputGridList[tmpGridIndex].AssignControlToGrid(tmpInputRowCount, tmpInputColumnIndex, mImportMaterialBtn);
        //    tmpInputTextRangeTalbe[1] = tmpInputRowCount;
        //    mInputGridList[tmpGridIndex].SetStandartInputTextAndBingdingToDict("物性参数", tmpInputTextRangeTalbe, tmpCorrentSetInputStringList, tmpBindingKeyIncre, mDataStorageDict);
        //    //mInputGridList[1].SetStandartInputGroup("物性参数", mHeadText, mMaterialParaText, mDataStorageDict);
        //    ////加工参数
        //    tmpGridIndex++;
        //    tmpCorrentSetInputStringList = mManufactureParaText;
        //    mInputGridList[tmpGridIndex].SetStandartDisplayText(mHeadText, tmpCorrentSetInputStringList, tmpDisplayColumnCount);
        //    mInputGridList[tmpGridIndex].SetGridControlByCreateComboBox("材料类型", tmpInputColumnIndex, mMaterialTypeText);
        //    mInputGridList[tmpGridIndex].SetGridControlByCreateComboBox("加工质量", tmpInputColumnIndex, ["ML最低要求材料及热处理质量", "MQ中等要求材料及热处理质量", "ME很高要求材料及热处理质量"]);
        //    mInputGridList[tmpGridIndex].SetGridControlByCreateComboBox("表面是否喷丸", tmpInputColumnIndex, ["否", "是"]);
        //    mInputGridList[tmpGridIndex].SetGridControlByCreateComboBox("是否允许表面出现有限点蚀", tmpInputColumnIndex, ["否", "是"]);
        //    tmpInputRowCount = tmpCorrentSetInputStringList.Length / 2;
        //    tmpInputTextRangeTalbe[1] = tmpInputRowCount;
        //    mInputGridList[tmpGridIndex].SetStandartInputTextAndBingdingToDict("加工参数", tmpInputTextRangeTalbe, tmpCorrentSetInputStringList, tmpBindingKeyIncre, mDataStorageDict);
        //    ////应用参数
        //    tmpGridIndex++;
        //    tmpCorrentSetInputStringList = mApplicationParaText;
        //    mInputGridList[tmpGridIndex].SetStandartDisplayText(mHeadText, tmpCorrentSetInputStringList, tmpDisplayColumnCount);
        //    mInputGridList[tmpGridIndex].SetGridControlByCreateComboBox("修形方法", tmpInputColumnIndex, [1, 2, 3], ["基于3维接触分析进行适当修形", "基于经验进行修形", "不修形"]);
        //    mInputGridList[tmpGridIndex].SetGridControlByCreateComboBox("螺旋线修形方式", tmpInputColumnIndex,
        //        [1, 3, 4, 5, 6], ["不修形", "仅中心鼓形修形", "仅螺旋线修正", "螺旋线修正且中心鼓形修形", "齿端修薄"]);
        //    mInputGridList[tmpGridIndex].SetGridControlByCreateComboBox("是否对接触区进行控制", tmpInputColumnIndex, ["不控制", "接触区控制良好"]);
        //    mInputGridList[tmpGridIndex].SetGridControlByCreateComboBox("齿廓修形是否能够补偿变形", tmpInputColumnIndex, ["否", "是"]);
        //    tmpInputRowCount = tmpCorrentSetInputStringList.Length / 2;
        //    tmpInputTextRangeTalbe[1] = tmpInputRowCount;
        //    mInputGridList[tmpGridIndex].SetStandartInputTextAndBingdingToDict("应用参数", tmpInputTextRangeTalbe, tmpCorrentSetInputStringList, tmpBindingKeyIncre, mDataStorageDict);
        //    ////精度参数
        //    tmpGridIndex++;
        //    tmpCorrentSetInputStringList = mTolerenceParaText;
        //    mInputGridList[tmpGridIndex].SetStandartDisplayText(mHeadText, tmpCorrentSetInputStringList, tmpDisplayColumnCount);
        //    mTolerenceCombo.SetComboBoxWithKeyValue([3, 4, 5, 6, 7], ["3级", "4级", "5级", "6级", "7级"]);
        //    mTolerenceCombo.SelectionChanged += mTolerenceCombo_SelectionChanged;
        //    mInputGridList[tmpGridIndex].AssignControlToGrid("精度等级", tmpInputColumnIndex, mTolerenceCombo);
        //    tmpInputRowCount = tmpCorrentSetInputStringList.Length / 2;
        //    tmpInputTextRangeTalbe[1] = tmpInputRowCount;
        //    mInputGridList[tmpGridIndex].SetStandartInputTextAndBingdingToDict("精度参数", tmpInputTextRangeTalbe, tmpCorrentSetInputStringList, tmpBindingKeyIncre, mDataStorageDict);
        //    ////安装参数
        //    tmpGridIndex++;
        //    tmpCorrentSetInputStringList = mAssembleParaText;
        //    mInputGridList[tmpGridIndex].SetStandartDisplayText(mHeadText, tmpCorrentSetInputStringList, tmpDisplayColumnCount);
        //    tmpInputRowCount = tmpCorrentSetInputStringList.Length / 2;
        //    tmpInputTextRangeTalbe[1] = tmpInputRowCount;
        //    mInputGridList[tmpGridIndex].SetStandartInputTextAndBingdingToDict("安装参数", tmpInputTextRangeTalbe, tmpCorrentSetInputStringList, tmpBindingKeyIncre, mDataStorageDict);

        //    ////初始化
        //    mDataStorageDict["旋向"].Value = -1;
        //    mDataStorageDict["材料类型"].Value = 9;
        //    mDataStorageDict["加工质量"].Value = 1;
        //    mDataStorageDict["修形方法"].Value = 1;
        //    mDataStorageDict["螺旋线修形方式"].Value = 1;
        //    mDataStorageDict["是否对接触区进行控制"].Value = 0;
        //    mDataStorageDict["齿轮副类型"].Value = 1;
        //    mDataStorageDict["驱动齿轮"].Value = 0;
        //    mDataStorageDict["刀具类型"].Value = 0;
        //    mDataStorageDict["是否使用滚动角"].Value = 0;
        //}
        public override void UpdatePageControlContent()
        {
            //移动组件
            for (int i = 0; i < mInputGridList.Length; i++)
            {
                mInputGridList[i].DisplayObjectFromGridTable([1], 2, 2, mInputGridList[i].mControlTable);
                //mInputGridList[i].DisplayObjectFromGrid([1], 2, 2, mInputGridList[i]);
            }
        }
        //创建带Content容器的Grid网格
        private void CreatInputContentGridWithContentTable()
        {
            //CTANBasicFunction.CreatObjectList(6, out mComboBoxList);
            CTANBasicFunction.CreatObjectList(7, out mInputGridList);
            mInputGridList[0].SetGridRowAndColumnWithContentTable(mGeometryParaText.Length / 2 + 1, 3);
            mInputGridList[1].SetGridRowAndColumnWithContentTable(mOtherGeometryParaText.Length / 2 + 1, 3);
            mInputGridList[2].SetGridRowAndColumnWithContentTable(mMaterialParaText.Length / 2 + 1, 3);//最后一行要放入"设为一般合金钢参数"按钮
            mInputGridList[3].SetGridRowAndColumnWithContentTable(mManufactureParaText.Length / 2 + 1, 3);
            mInputGridList[4].SetGridRowAndColumnWithContentTable(mApplicationParaText.Length / 2 + 1, 3);
            mInputGridList[5].SetGridRowAndColumnWithContentTable(mTolerenceParaText.Length / 2 + 1, 3);
            mInputGridList[6].SetGridRowAndColumnWithContentTable(mAssembleParaText.Length / 2 + 1, 3);            
            mInputPageGrid.SetGridRowAndColumn(7, 0);
            for (int i = 0; i < 7; i++)
            {
                if (i == 1)
                {
                    Expander otherGeoExpander = new Expander
                    {
                        Header = "其他几何参数",
                        IsExpanded = false, // 默认折叠
                        Content = mInputGridList[i].mGroupBox,
                    };
                    Grid.SetRow(otherGeoExpander, i);
                    Grid.SetColumn(otherGeoExpander, 0);
                    mInputPageGrid.Children.Add(otherGeoExpander);
                    continue;
                }
                mInputGridList[i].SetGroupInGrid(i, 0, mInputPageGrid);
            }
        }
        private void CreatInputContentInnerTabControl()
        {
            // 创建内层TabControl
            mInnerTabControl = new TabControl
            {
                TabStripPlacement = Dock.Top,
                Margin = new Thickness(5)
            };
            CTANBasicFunction.CreatObjectList(8, out mInputGridList);
            mInputGridList[0].SetGridRowAndColumnWithContentTable(mGeometryParaText.Length / 2 + 1, 3);
            mInputGridList[1].SetGridRowAndColumnWithContentTable(mOtherGeometryParaText.Length / 2 + 1, 3);
            mInputGridList[2].SetGridRowAndColumnWithContentTable(mMaterialParaText.Length / 2 + 1, 3);//最后一行要放入"设为一般合金钢参数"按钮
            mInputGridList[3].SetGridRowAndColumnWithContentTable(mManufactureParaText.Length / 2 + 1, 3);
            mInputGridList[4].SetGridRowAndColumnWithContentTable(mApplicationParaText.Length / 2 + 1, 3);
            mInputGridList[5].SetGridRowAndColumnWithContentTable(mTolerenceParaText.Length / 2 + 1, 3);
            mInputGridList[6].SetGridRowAndColumnWithContentTable(mAssembleParaText.Length / 2 + 1, 3);
            mInputGridList[7].SetGridRowAndColumnWithContentTable(mShaftAssembleText.Length / 2 + 1, 3);
        }
        private void SetInputContentInnerTabControlContent()
        {
            TabItem geometryTab = new TabItem
            {
                Header = "几何参数",
                Name = "tabGeometry"
            };
            // 创建容器来容纳两个内容
            StackPanel geometryContainer = new StackPanel
            {
                Orientation = Orientation.Vertical,
                Margin = new Thickness(5),
                //Background = Brushes.WhiteSmoke
            };
            // 检查并移除可能的父元素（防止重复归属）
            if (mInputGridList[0].Parent != null)
            {
                if (mInputGridList[0].Parent is Panel parentPanel)
                {
                    parentPanel.Children.Remove(mInputGridList[0]);
                }
                else if (mInputGridList[0].Parent is ContentControl contentControl)
                {
                    if (contentControl.Content == mInputGridList[0])
                    {
                        contentControl.Content = null;
                    }
                }
            }
            // 添加主要几何参数网格
            geometryContainer.Children.Add(mInputGridList[0]);
            // 添加其他几何参数Expander
            Expander otherGeometryExpander = new Expander
            {
                Header = "其他几何参数",
                IsExpanded = false,
                Margin = new Thickness(0, 10, 0, 0)
            };
            otherGeometryExpander.Content = mInputGridList[1];
            geometryContainer.Children.Add(otherGeometryExpander);

            geometryTab.Content = geometryContainer;
            mInnerTabControl.Items.Add(geometryTab);

            TabItem materialTab = new TabItem
            {
                Header = "物性参数",
                Name = "tabMaterial"
            };
            materialTab.Content = mInputGridList[2];
            mInnerTabControl.Items.Add(materialTab);

            TabItem manufactureTab = new TabItem
            {
                Header = "加工参数",
                Name = "tabManufacture"
            };
            manufactureTab.Content = mInputGridList[3];
            mInnerTabControl.Items.Add(manufactureTab);

            TabItem applicationTab = new TabItem
            {
                Header = "应用参数",
                Name = "tabApplication"
            };
            applicationTab.Content = mInputGridList[4];
            mInnerTabControl.Items.Add(applicationTab);

            TabItem toleranceTab = new TabItem
            {
                Header = "精度参数",
                Name = "tabTolerance"
            };
            toleranceTab.Content = mInputGridList[5];
            mInnerTabControl.Items.Add(toleranceTab);

            TabItem assemblyTab = new TabItem
            {
                Header = "安装参数",
                Name = "tabAssembly"
            };
            assemblyTab.Content = mInputGridList[6];
            mInnerTabControl.Items.Add(assemblyTab);

            TabItem locateTab = new TabItem
            {
                Header = "定位参数",
                Name = "tabLocate"
            };
            locateTab.Content = mInputGridList[7];
            mInnerTabControl.Items.Add(locateTab);

            // 将内层TabControl放入mInputPageGrid中
            mInputPageGrid.Children.Clear();
            mInputPageGrid.Children.Add(mInnerTabControl);

            // 设置默认选中的Tab
            if (mInnerTabControl.Items.Count > 0)
            {
                mInnerTabControl.SelectedIndex = 0;
            }
        }
        private void InitSomePara()
        {
            ////初始化
            mDataStorageDict["旋向"].Value = -1;
            mDataStorageDict["材料类型"].Value = 9;
            mDataStorageDict["加工质量"].Value = 1;
            mDataStorageDict["修形方法"].Value = 1;
            mDataStorageDict["螺旋线修形方式"].Value = 1;
            mDataStorageDict["是否对接触区进行控制"].Value = 0;
            mDataStorageDict["齿轮副类型"].Value = 1;
            mDataStorageDict["驱动齿轮"].Value = 0;
            mDataStorageDict["刀具类型"].Value = 0;
            mDataStorageDict["是否使用滚动角"].Value = 0;
        }
        //private void SetOutputPage()
        //{
        //    mOutputPageGrid.SetGridRowAndColumn(2, 1);
        //    mOutputDataTable.SetGearStandardOutputTable(mHeadText, mOutPutHeadName, 2);
        //    mDataStorageDict.SetDataStorageDictionary(0, 2, mOutPutHeadName.Count() / 2, mOutPutHeadName);
        //    BindingDictToOutputTable(2);
        //    mOutputPageGrid.Children.Add(mOutputDataTable.mDataGrid);
        //}
        private void SetOutputPage()
        {
            // 清除原有内容
            mOutputPageGrid.Children.Clear();

            // 定义标签页和对应的输出名称
            var tabDefinitions = new[]
            {
                new { Header = "应力参数", Names = mOutStressName },
                new { Header = "几何参数", Names = mOutGeometryName },
                new { Header = "接触参数", Names = mOutContactName },
                new { Header = "滑动参数", Names = mOutSlideName },
                new { Header = "齿廓参数", Names = mOutToothProfileName },
                new { Header = "特殊参数", Names = mOutSpecialName },
                new { Header = "其他参数", Names = mOutOthersName },
            };

            // 初始化输出表格数组
            CTANBasicFunction.CreatObjectList(tabDefinitions.Length, out mOutputTables);

            // 创建TabControl
            mOutTabControl = new TabControl
            {
                TabStripPlacement = Dock.Top,
                Margin = new Thickness(5)
            };

            // 创建每个标签页
            for (int i = 0; i < tabDefinitions.Length; i++)
            {
                // 创建标签页
                TabItem tabItem = new TabItem
                {
                    Header = tabDefinitions[i].Header,
                    Name = $"tabOutput{i}"
                };
                // 创建并初始化数据表格
                mOutputTables[i] = new CTANDataTable();
                mOutputTables[i].SetGearStandardOutputTable(mHeadText, tabDefinitions[i].Names, 2);
                mDataStorageDict.SetDataStorageDictionary(0, 2, tabDefinitions[i].Names.Count() / 2, tabDefinitions[i].Names);
                tabItem.Content = mOutputTables[i].mDataGrid;
                mOutTabControl.Items.Add(tabItem);
            }
            BindingDictToOutputTableZJ(mOutputTables, mDataStorageDict, 2);
            mOutputPageGrid.Children.Add(mOutTabControl);           
            // 设置默认选中的Tab
            if (mOutTabControl.Items.Count > 0)
            {
                mOutTabControl.SelectedIndex = 0;
            }
        }
        public override double ReturnGeometryDiameter()
        {
            ////需要修改，目前为简化算法
            //double mn = mDataStorageDict["法向模数"].Value;
            //double ha1 = mDataStorageDict["齿顶高系数"].Value;
            //return mn * (2 * ha1 + mDataStorageDict["齿数"].Value);
            return mDataStorageDict["齿顶圆直径[da]"].Value;
        }
    }
    public class CTANInvGearPair: CTANBasicContentPageObj
    {
        [DllImport(@"RVReducerBasic.dll", EntryPoint = "CalGeometry", CallingConvention = CallingConvention.Cdecl)]
        public static extern void CalGeometry(ref double pInPara, ref double pOutPara);
        [DllImport(@"RVReducerBasic.dll", EntryPoint = "CalInvGearStress", CallingConvention = CallingConvention.Cdecl)]
        public static extern void CalInvGearStress(
            [In, Out] double[,] InGeometrydata,
            [In, Out] double[,] InMaterialAndManufactureData,
            [In, Out] double[,] InAppData,
            [In, Out] double[,] InTolerenceData,
            double[,] InLoadData,
            double[,] InAssembleData,
            double[,] InLocData,
            [In, Out] double[,] OutData);
        [DllImport(@"RVReducerBasic.dll", EntryPoint = "CalInvGearStress2", CallingConvention = CallingConvention.Cdecl)]
        public static extern void CalInvGearStress2(
            ref int InDividIndexList,
            ref double InInputData,
            ref double OutData);
        public CTANRVSet mBelongedSet;//所属的RV减速器
        static int sGearParaStartIndex = 2;
        //public static int[] mDivisionIndexList = new int[] {123,12,//总输入量和输出量
        //                                                    14,3,42,42,
        //                                                    8,4,32,74,
        //                                                    8,3,24,98,
        //                                                    6,2,12,110,
        //                                                    3,1,3,113,
        //                                                    6,1,6,119,
        //                                                    2,2,4,123,//通过渐开线参数分割索引表定义,输入序列29个
        //                                                    6,2,12,12};//输出序列4个
        public static int[] mDivisionIndexList = new int[] {174,280,//总输入量和输出量
                                                            31,3,93,93,
                                                            8,4,32,125,
                                                            8,3,24,149,
                                                            6,2,12,161,
                                                            3,1,3,164,
                                                            6,1,6,170,
                                                            2,2,4,174,//通过渐开线参数分割索引表定义,输入序列29个
                                                            142,2,284,284};//输出序列4个


        public CTANInvGear[] mGears = new CTANInvGear[2] { new(), new() };
        //public CTANControlGrid[] mContentGrid = new CTANControlGrid[6]; 
        public CTANTorque mTorque = new CTANTorque();
        private static string[] mHeadText = new string[] { "名称", "单位", "太阳轮", "行星轮" };
        private TabControl mPairInnerTab;
        private TabControl mOutPairTabControl;
        private CTANDataTable[] mOutputPairTables;
        //private double mIntroduceHeadFontSize = 15;

        public CTANInvGearPair()
        {
            mGears[0].mTreeViewItem.Header = "太阳轮";
            mGears[0].mTreeViewItem.Name = "太阳轮";
            mGears[1].mTreeViewItem.Header = "行星轮";
            mGears[1].mTreeViewItem.Name = "行星轮";
            mTreeViewItem.Header = "渐开线齿轮副";
            mTreeViewItem.Name = "渐开线齿轮副";
            mTreeViewItem.Items.Add(mGears[0].mTreeViewItem);
            mTreeViewItem.Items.Add(mGears[1].mTreeViewItem);
            mTreeViewItem.Items.Add(mTorque.mTreeViewItem);
            
            SetInputPage();
            SetOutputPage();

            //数据存储空间申请
            CTANInvGearPair.mDivisionIndexList[1 + 4 * 7] = CTANInvGear.mOutPutHeadName.Count() / 2;

            ////计算输入数据

            mGears[1].mDataStorageDict["名义接触应力"].Value = 100;
            TestInput();

        }
        private void SetInputPage()
        {
            //生成显示文本
            int tmpGearGridCount = mGears[0].mInputGridList.Length;
            CTANBasicFunction.CreatObjectList(tmpGearGridCount, out mInputGridList);
            // 创建内层TabControl
            mPairInnerTab = new TabControl
            {
                TabStripPlacement = Dock.Top,
                Margin = new Thickness(5)
            };
            for (int i = 0; i < tmpGearGridCount; i++)
            {
                mInputGridList[i].CopyAndModifyInputColumnFromGrid([1], [0, 1], 4, mGears[0].mInputGridList[i]);
                mInputGridList[i].SetGridControlByCreateDisplayTextBox(0, 0, mHeadText.Length, mHeadText);
                mInputGridList[i].CopyRowStyle(mGears[0].mInputGridList[i]);                
            }
            SetInputContentInnerTabControlContent();

            // 将内层TabControl放入mInputPageGrid中
            mInputPageGrid.Children.Clear();
            mInputPageGrid.Children.Add(mPairInnerTab);
            SetDataBinding();
        }
        //private void SetInputPage()
        //{
        //    //生成显示文本
        //    int tmpGearGridCount = mGears[0].mInputGridList.Length;
        //    CTANBasicFunction.CreatObjectList(tmpGearGridCount, out mInputGridList);
        //    mInputPageGrid.SetGridRowAndColumn(tmpGearGridCount, 0);
        //    for (int i = 0; i < tmpGearGridCount; i++)
        //    {
        //        mInputGridList[i].CopyAndModifyInputColumnFromGrid([1], [0, 1], 4, mGears[0].mInputGridList[i]);
        //        mInputGridList[i].SetGridControlByCreateDisplayTextBox(0, 0, mHeadText.Length, mHeadText);
        //        mInputGridList[i].CopyRowStyle(mGears[0].mInputGridList[i]);

        //        if (i == 1)
        //        {
        //            Expander otherGeoExpander = new Expander
        //            {
        //                Header = "其他几何参数",
        //                IsExpanded = false, // 默认折叠
        //                Content = mInputGridList[i].mGroupBox,
        //            };
        //            Grid.SetRow(otherGeoExpander, i);
        //            Grid.SetColumn(otherGeoExpander, 0);
        //            mInputPageGrid.Children.Add(otherGeoExpander);
        //        }
        //        else
        //        {
        //            mInputGridList[i].SetGroupInGrid(i, 0, mInputPageGrid);
        //        }
        //    }
        //    SetDataBinding();
        //}
        //public override void UpdatePageControlContent()
        //{
        //    //移动组件
        //    for (int i = 0; i < mGears[0].mInputGridList.Length; i++)
        //    {
        //        mInputGridList[i].MoveObjectFromGrid([1], 2, 2, mGears[0].mInputGridList[i]);
        //        mInputGridList[i].MoveObjectFromGrid([1], 2, 3, mGears[1].mInputGridList[i]);
        //    }
        //}
        private void SetInputContentInnerTabControlContent()
        {
            // 几何参数Tab
            TabItem geometryTab = new TabItem
            {
                Header = "几何参数",
                Name = "tabGeometry"
            };

            // 创建容器来容纳两个内容
            StackPanel geometryContainer = new StackPanel
            {
                Orientation = Orientation.Vertical,
                Margin = new Thickness(5)
            };

            // 检查并移除可能的父元素（防止重复归属）
            if (mInputGridList[0].Parent != null)
            {
                if (mInputGridList[0].Parent is Panel parentPanel)
                {
                    parentPanel.Children.Remove(mInputGridList[0]);
                }
                else if (mInputGridList[0].Parent is ContentControl contentControl)
                {
                    if (contentControl.Content == mInputGridList[0])
                    {
                        contentControl.Content = null;
                    }
                }
            }

            // 添加主要几何参数网格
            geometryContainer.Children.Add(mInputGridList[0]);

            // 添加其他几何参数Expander
            Expander otherGeometryExpander = new Expander
            {
                Header = "其他几何参数",
                IsExpanded = false,
                Margin = new Thickness(0, 10, 0, 0)
            };

            // 检查并移除mInputGridList[1]的父元素
            if (mInputGridList[1].Parent != null)
            {
                if (mInputGridList[1].Parent is Panel parentPanel)
                {
                    parentPanel.Children.Remove(mInputGridList[1]);
                }
                else if (mInputGridList[1].Parent is ContentControl contentControl)
                {
                    if (contentControl.Content == mInputGridList[1])
                    {
                        contentControl.Content = null;
                    }
                }
            }

            otherGeometryExpander.Content = mInputGridList[1];
            geometryContainer.Children.Add(otherGeometryExpander);

            geometryTab.Content = geometryContainer;
            mPairInnerTab.Items.Add(geometryTab);

            // 物性参数Tab
            TabItem materialTab = new TabItem
            {
                Header = "物性参数",
                Name = "tabMaterial"
            };

            if (mInputGridList[2].Parent != null)
            {
                if (mInputGridList[2].Parent is Panel parentPanel)
                {
                    parentPanel.Children.Remove(mInputGridList[2]);
                }
            }

            materialTab.Content = mInputGridList[2];
            mPairInnerTab.Items.Add(materialTab);

            // 加工参数Tab
            TabItem manufactureTab = new TabItem
            {
                Header = "加工参数",
                Name = "tabManufacture"
            };

            if (mInputGridList[3].Parent != null)
            {
                if (mInputGridList[3].Parent is Panel parentPanel)
                {
                    parentPanel.Children.Remove(mInputGridList[3]);
                }
            }

            manufactureTab.Content = mInputGridList[3];
            mPairInnerTab.Items.Add(manufactureTab);

            // 应用参数Tab
            TabItem applicationTab = new TabItem
            {
                Header = "应用参数",
                Name = "tabApplication"
            };

            if (mInputGridList[4].Parent != null)
            {
                if (mInputGridList[4].Parent is Panel parentPanel)
                {
                    parentPanel.Children.Remove(mInputGridList[4]);
                }
            }

            applicationTab.Content = mInputGridList[4];
            mPairInnerTab.Items.Add(applicationTab);

            // 精度参数Tab
            TabItem toleranceTab = new TabItem
            {
                Header = "精度参数",
                Name = "tabTolerance"
            };

            if (mInputGridList[5].Parent != null)
            {
                if (mInputGridList[5].Parent is Panel parentPanel)
                {
                    parentPanel.Children.Remove(mInputGridList[5]);
                }
            }

            toleranceTab.Content = mInputGridList[5];
            mPairInnerTab.Items.Add(toleranceTab);

            // 安装参数Tab
            TabItem assemblyTab = new TabItem
            {
                Header = "安装参数",
                Name = "tabAssembly"
            };

            if (mInputGridList[6].Parent != null)
            {
                if (mInputGridList[6].Parent is Panel parentPanel)
                {
                    parentPanel.Children.Remove(mInputGridList[6]);
                }
            }
            assemblyTab.Content = mInputGridList[6];
            mPairInnerTab.Items.Add(assemblyTab);

            TabItem locateTab = new TabItem
            {
                Header = "定位参数",
                Name = "tabLocate"
            };
            locateTab.Content = mInputGridList[7];
            mPairInnerTab.Items.Add(locateTab);

            if (mInputGridList[7].Parent != null)
            {
                if (mInputGridList[7].Parent is Panel parentPanel)
                {
                    parentPanel.Children.Remove(mInputGridList[7]);
                }
            }

            // 设置默认选中的Tab
            if (mPairInnerTab.Items.Count > 0)
            {
                mPairInnerTab.SelectedIndex = 0;
            }
        }
        public override void UpdatePageControlContent()
        {
            //移动组件
            for (int i = 0; i < mGears[0].mInputGridList.Length; i++)
            {
                mInputGridList[i].DisplayObjectFromGridTable([1], 2, 2, mGears[0].mInputGridList[i].mControlTable);
                mInputGridList[i].DisplayObjectFromGridTable([1], 2, 3, mGears[1].mInputGridList[i].mControlTable);
                //mInputGridList[i].DisplayObjectFromGrid([1], 2, 2, mGears[0].mInputGridList[i]);
                //mInputGridList[i].DisplayObjectFromGrid([1], 2, 3, mGears[1].mInputGridList[i]);
            }
        }
        private void SetDataBinding()
        {
            BindingGearCommonData("法向模数");
            BindingGearCommonData("压力角");
            BindingGearCommonData("螺旋角");
            BindingGearCommonData("中心距");
            BindingGearCommonData("使用系数");
            BindingGearCommonData("均载系数");
            BindingGearCommonData("润滑油粘度(v40)");
            BindingGearCommonData("行星轮个数");
            BindingGearCommonData("是否对接触区进行控制");

            BindingGearCommonData("齿轮副类型");
            BindingGearCommonData("驱动齿轮");
            BindingGearCommonData("触点间隙[j_bn]");
            //旋向相反绑定
            BindingOperations.SetBinding(mGears[1].mDataStorageDict["旋向"], CTANDouble.ValueProperty, new Binding("Value")
            {
                Source = mGears[0].mDataStorageDict["旋向"],
                Mode = BindingMode.TwoWay,
                Converter = CTANConverter.sReverseSignConverter,
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
            });
            //mGears[0].mSpiralDirectCombo.SetBinding(ComboBox.SelectedIndexProperty, new Binding("SelectedIndex") { Source = mGears[1].mSpiralDirectCombo, Converter = new ReverseINTConverter() });
        }   
        private void BindingGearCommonData(string InDataName)
        {
            mGears[0].mDataStorageDict[InDataName].SetDataBingingToTANDouble(mGears[1].mDataStorageDict[InDataName]);
            //BindingOperations.SetBinding(mGears[0].mDataStorageDict[InDataName], CTANDouble.ValueProperty,
            //    new Binding("Value") { Source = mGears[1].mDataStorageDict[InDataName],Mode= BindingMode.TwoWay });
        }
        private void TestInput()
        {

            //mGears[0].mDataStorageDict["法向模数"].Value = 1.5;
            //mGears[0].mDataStorageDict["齿数"].Value = 16;
            //mGears[1].mDataStorageDict["齿数"].Value = 43;
            //mGears[0].mDataStorageDict["设计齿宽"].Value = 14;
            //mGears[1].mDataStorageDict["设计齿宽"].Value = 14.5;
            //mGears[0].mDataStorageDict["压力角"].Value = 20;
            //mGears[0].mDataStorageDict["螺旋角"].Value = 25;
            //mGears[0].mDataStorageDict["旋向"].Value = -1;
            //mGears[0].mDataStorageDict["中心距"].Value = 48.9;
            //mGears[0].mDataStorageDict["齿顶高系数"].Value = 1;
            //mGears[1].mDataStorageDict["齿顶高系数"].Value = 1;
            //mGears[0].mDataStorageDict["顶隙系数"].Value = 0.25;
            //mGears[1].mDataStorageDict["顶隙系数"].Value = 0.25;
            //mGears[0].mDataStorageDict["切向变位系数"].Value = 0;
            //mGears[1].mDataStorageDict["切向变位系数"].Value = 0;
            //mGears[0].mDataStorageDict["高变位系数"].Value = 0.3215;
            //mGears[1].mDataStorageDict["高变位系数"].Value = -0.2709;
            //mGears[0].mDataStorageDict["齿根圆角系数"].Value = 0.38;
            //mGears[1].mDataStorageDict["齿根圆角系数"].Value = 0.38;
            //mGears[0].mDataStorageDict["挖根量"].Value = 0;
            //mGears[1].mDataStorageDict["挖根量"].Value = 0;
            //mGears[0].mDataStorageDict["齿顶倒角量"].Value = 0;
            //mGears[0].mDataStorageDict["齿顶修形量"].Value = 0;
            //mGears[0].mDataStorageDict["齿根修形量"].Value = 0;
            //mGears[0].mDataStorageDict["跨齿数"].Value = 2;
            //mGears[1].mDataStorageDict["齿顶倒角量"].Value = 0;
            //mGears[1].mDataStorageDict["齿顶修形量"].Value = 0;
            //mGears[1].mDataStorageDict["齿根修形量"].Value = 0;
            //mGears[1].mDataStorageDict["跨齿数"].Value = 4;

//下面这组参数会导致不平衡，数值为3800多，这组参数曾用于验证校核！
            //mGears[0].mDataStorageDict["法向模数"].Value = 8;
            //mGears[0].mDataStorageDict["压力角"].Value = 20;
            //mGears[0].mDataStorageDict["螺旋角"].Value = 15.8;
            //mGears[0].mDataStorageDict["旋向"].Value = -1;
            //mGears[0].mDataStorageDict["中心距"].Value = 500;
            //mGears[0].mDataStorageDict["齿数"].Value = 17;
            //mGears[1].mDataStorageDict["齿数"].Value = 103;
            //mGears[0].mDataStorageDict["设计齿宽"].Value = 100;
            //mGears[1].mDataStorageDict["设计齿宽"].Value = 7.1;
            //mGears[0].mDataStorageDict["齿顶高系数"].Value = 1;
            //mGears[1].mDataStorageDict["齿顶高系数"].Value = 1;
            //mGears[0].mDataStorageDict["齿廓变位系数"].Value = 0.1178;
            //mGears[1].mDataStorageDict["齿廓变位系数"].Value = 0.0274;
            //mGears[0].mDataStorageDict["顶隙系数"].Value = 0.4;
            //mGears[1].mDataStorageDict["顶隙系数"].Value = 0.4;
            //mGears[0].mDataStorageDict["齿根圆角系数"].Value = 0.39;
            //mGears[1].mDataStorageDict["齿根圆角系数"].Value = 0.39;
            //mGears[0].mDataStorageDict["挖根量"].Value = 0;
            //mGears[1].mDataStorageDict["挖根量"].Value = 0;
            //mGears[0].mDataStorageDict["齿顶修形量"].Value = 70;
            //mGears[1].mDataStorageDict["齿顶修形量"].Value = 70;
            //mGears[0].mDataStorageDict["齿根修形量"].Value = 0;
            //mGears[1].mDataStorageDict["齿根修形量"].Value = 0;
            //mGears[0].mDataStorageDict["是否使用滚动角"].Value = 0;    //0 不使用滚动角；1 使用滚动角
            //mGears[1].mDataStorageDict["是否使用滚动角"].Value = 0;
            //mGears[0].mDataStorageDict["跨齿数"].Value = 2;
            //mGears[1].mDataStorageDict["跨齿数"].Value = 13;

            mGears[0].mDataStorageDict["法向模数"].Value = 2;
            mGears[0].mDataStorageDict["压力角"].Value = 20;
            mGears[0].mDataStorageDict["螺旋角"].Value = 0;
            mGears[0].mDataStorageDict["旋向"].Value = -1;/////////
            mGears[0].mDataStorageDict["中心距"].Value = 130;
            mGears[0].mDataStorageDict["齿数"].Value = 23;
            mGears[1].mDataStorageDict["齿数"].Value = 106;
            mGears[0].mDataStorageDict["设计齿宽"].Value = 20;
            mGears[1].mDataStorageDict["设计齿宽"].Value = 20;
            mGears[0].mDataStorageDict["齿顶高系数"].Value = 1;
            mGears[1].mDataStorageDict["齿顶高系数"].Value = 1;
            mGears[0].mDataStorageDict["齿廓变位系数"].Value = 0.5142;
            mGears[1].mDataStorageDict["齿廓变位系数"].Value = 0;
            mGears[0].mDataStorageDict["顶隙系数"].Value = 0.25;
            mGears[1].mDataStorageDict["顶隙系数"].Value = 0.25;
            mGears[0].mDataStorageDict["齿根圆角系数"].Value = 0.384;
            mGears[1].mDataStorageDict["齿根圆角系数"].Value = 0.384;
            mGears[0].mDataStorageDict["挖根量"].Value = 0;
            mGears[1].mDataStorageDict["挖根量"].Value = 0;
            mGears[0].mDataStorageDict["齿顶修形量"].Value = 0;
            mGears[1].mDataStorageDict["齿顶修形量"].Value = 0;
            mGears[0].mDataStorageDict["齿根修形量"].Value = 0;
            mGears[1].mDataStorageDict["齿根修形量"].Value = 0;
            mGears[0].mDataStorageDict["是否使用滚动角"].Value = 0;    //0 不使用滚动角；1 使用滚动角
            mGears[1].mDataStorageDict["是否使用滚动角"].Value = 0;
            mGears[0].mDataStorageDict["跨齿数"].Value = 2;
            mGears[1].mDataStorageDict["跨齿数"].Value = 4;
            mGears[0].mDataStorageDict["齿顶倒角量"].Value = 0;
            mGears[1].mDataStorageDict["齿顶倒角量"].Value = 0;
            mGears[0].mDataStorageDict["轮幅外圆直径"].Value = 0;
            mGears[1].mDataStorageDict["轮幅外圆直径"].Value = 0;
            mGears[0].mDataStorageDict["齿轮齿顶高变化[k·mn]"].Value = 0;
            mGears[1].mDataStorageDict["齿轮齿顶高变化[k·mn]"].Value = 0;
            mGears[0].mDataStorageDict["齿厚的偏差上限[E_sns]"].Value = -0.0670;
            mGears[1].mDataStorageDict["齿厚的偏差上限[E_sns]"].Value = -0.0617;
            mGears[0].mDataStorageDict["齿厚的偏差下限[E_sni]"].Value = -0.0926;
            mGears[1].mDataStorageDict["齿厚的偏差下限[E_sni]"].Value = -0.0947;
            mGears[0].mDataStorageDict["齿顶圆容差上限"].Value = 0;
            mGears[1].mDataStorageDict["齿顶圆容差上限"].Value = 0;
            mGears[0].mDataStorageDict["齿顶圆容差下限"].Value = 0;
            mGears[1].mDataStorageDict["齿顶圆容差下限"].Value = 0;
            mGears[0].mDataStorageDict["最大机械加工余量[q_max]"].Value = 0;
            mGears[1].mDataStorageDict["最大机械加工余量[q_max]"].Value = 0;
            mGears[0].mDataStorageDict["最小机械加工余量[q_min]"].Value = 0;
            mGears[1].mDataStorageDict["最小机械加工余量[q_min]"].Value = 0;
            mGears[0].mDataStorageDict["Y圆直径[d_y]"].Value = 25;
            mGears[1].mDataStorageDict["Y圆直径[d_y]"].Value = 67;
            mGears[0].mDataStorageDict["刀具类型"].Value = 0; //0 RackCutter； 1 ShaperCutter
            mGears[1].mDataStorageDict["刀具类型"].Value = 0;
            mGears[0].mDataStorageDict["齿条刀的齿顶高系数"].Value = 1.4;
            mGears[1].mDataStorageDict["齿条刀的齿顶高系数"].Value = 1.4;
            mGears[0].mDataStorageDict["齿条刀的齿顶圆半径系数"].Value = 0.39;
            mGears[1].mDataStorageDict["齿条刀的齿顶圆半径系数"].Value = 0.39;
            mGears[0].mDataStorageDict["插齿刀齿数"].Value = 0;
            mGears[1].mDataStorageDict["插齿刀齿数"].Value = 0;
            mGears[0].mDataStorageDict["插齿刀加工中心距"].Value = 0;
            mGears[1].mDataStorageDict["插齿刀加工中心距"].Value = 0;
            mGears[0].mDataStorageDict["插齿刀具齿顶直径"].Value = 0;
            mGears[1].mDataStorageDict["插齿刀具齿顶直径"].Value = 0;
            mGears[0].mDataStorageDict["插齿刀基圆直径"].Value = 0;
            mGears[1].mDataStorageDict["插齿刀基圆直径"].Value = 0;
            mGears[0].mDataStorageDict["插齿刀齿顶形状直径"].Value = 0;
            mGears[1].mDataStorageDict["插齿刀齿顶形状直径"].Value = 0;
            mGears[0].mDataStorageDict["插齿刀具参数横向压力角[alpha_wt0]"].Value = 0;
            mGears[1].mDataStorageDict["插齿刀具参数横向压力角[alpha_wt0]"].Value = 0;
            mGears[0].mDataStorageDict["齿轮副类型"].Value = 1;
            mGears[0].mDataStorageDict["驱动齿轮"].Value = 0;//0 mPinion； 1 mGear
            mGears[0].mDataStorageDict["触点间隙[j_bn]"].Value = 0;
            mGears[0].mImportMaterialBtn.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
            mGears[1].mImportMaterialBtn.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
            for (int i = 0;i<2;i++)
            {
                mGears[i].mDataStorageDict["齿廓修形是否能够补偿变形"].Value = 0;
                mGears[i].mDataStorageDict["表面硬度(HRC)"].Value = 60;
                mGears[i].mDataStorageDict["芯部硬度(HRC)"].Value = 30;
                mGears[i].mDataStorageDict["使用系数"].Value = 1.0;
                mGears[i].mDataStorageDict["均载系数"].Value = 1.0;
                mGears[i].mDataStorageDict["润滑油粘度(v40)"].Value = 320.0;
                mGears[i].mDataStorageDict["接触最小安全系数"].Value = 1.0;
                mGears[i].mDataStorageDict["弯曲最小安全系数"].Value = 1.0;
                mGears[i].mDataStorageDict["行星轮个数"].Value = 3;
                //mGears[i].mTolerenceCombo.SelectedIndex = 0;
                mGears[i].mTolerenceCombo.SelectedValue = 5;
                mGears[i].mDataStorageDict["接触表面粗糙度(Ra)"].Value = 1;
                mGears[i].mDataStorageDict["齿根表面粗糙度(Ra)"].Value = 3;
                //mGears[i].mDataStorageDict["行星轮个数"].Value = 3;
                mGears[i].mDataStorageDict["安装类型"].Value = 5;
                mGears[i].mDataStorageDict["是否为刚性轴"].Value = 0;
                mGears[i].mDataStorageDict["太阳轮轴直径"].Value = 17;
                mGears[i].mDataStorageDict["太阳轮齿宽中点至轴承跨距中点距离"].Value = 0;
                mGears[i].mDataStorageDict["轴承跨距"].Value = 34;
            }
            mTorque.mDataStorageDict["扭矩"].Value = 9000;
            mTorque.mDataStorageDict["转速"].Value = 360;
            mTorque.mDataStorageDict["预期寿命"].Value = 50000;
            mGears[0].mDataStorageDict["定位面"].Value = -1;
            mGears[0].mDataStorageDict["定位面距基准面距离"].Value = 9;
            mGears[1].mDataStorageDict["定位面"].Value = 1;
            mGears[1].mDataStorageDict["定位面距基准面距离"].Value = 5.45;
            mGears[1].mImportMaterialBtn.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
            //mGears[0].ImportGeneralMaterialParaBtn_Clicked();
            //add
            double[] test_mGears = new double[1000];           
            int oneGearParaCount = 31;
            for (int i = 0; i < 2; i++)
            {
                test_mGears[i * oneGearParaCount] = mGears[i].mDataStorageDict["齿数"].Value;
                test_mGears[1 + i * oneGearParaCount] = mGears[i].mDataStorageDict["旋向"].Value;
                test_mGears[2 + i * oneGearParaCount] = mGears[i].mDataStorageDict["设计齿宽"].Value;
                test_mGears[3 + i * oneGearParaCount] = mGears[i].mDataStorageDict["齿顶高系数"].Value;
                test_mGears[4 + i * oneGearParaCount] = mGears[i].mDataStorageDict["齿廓变位系数"].Value;
                test_mGears[5 + i * oneGearParaCount] = mGears[i].mDataStorageDict["顶隙系数"].Value;
                test_mGears[6 + i * oneGearParaCount] = mGears[i].mDataStorageDict["齿根圆角系数"].Value;
                test_mGears[7 + i * oneGearParaCount] = mGears[i].mDataStorageDict["挖根量"].Value;
                test_mGears[8 + i * oneGearParaCount] = mGears[i].mDataStorageDict["齿顶修形量"].Value;
                test_mGears[9 + i * oneGearParaCount] = mGears[i].mDataStorageDict["齿根修形量"].Value;
                test_mGears[10 + i * oneGearParaCount] = mGears[i].mDataStorageDict["是否使用滚动角"].Value;
                test_mGears[11 + i * oneGearParaCount] = mGears[i].mDataStorageDict["跨齿数"].Value;
                test_mGears[12 + i * oneGearParaCount] = mGears[i].mDataStorageDict["齿顶倒角量"].Value;
                test_mGears[13 + i * oneGearParaCount] = mGears[i].mDataStorageDict["轮幅外圆直径"].Value;
                test_mGears[14 + i * oneGearParaCount] = mGears[i].mDataStorageDict["齿轮齿顶高变化[k·mn]"].Value;
                test_mGears[15 + i * oneGearParaCount] = mGears[i].mDataStorageDict["齿厚的偏差上限[E_sns]"].Value;
                test_mGears[16 + i * oneGearParaCount] = mGears[i].mDataStorageDict["齿厚的偏差下限[E_sni]"].Value;
                test_mGears[17 + i * oneGearParaCount] = mGears[i].mDataStorageDict["齿顶圆容差上限"].Value;
                test_mGears[18 + i * oneGearParaCount] = mGears[i].mDataStorageDict["齿顶圆容差下限"].Value;
                test_mGears[19 + i * oneGearParaCount] = mGears[i].mDataStorageDict["最大机械加工余量[q_max]"].Value;
                test_mGears[20 + i * oneGearParaCount] = mGears[i].mDataStorageDict["最小机械加工余量[q_min]"].Value;
                test_mGears[21 + i * oneGearParaCount] = mGears[i].mDataStorageDict["Y圆直径[d_y]"].Value;
                test_mGears[22 + i * oneGearParaCount] = mGears[i].mDataStorageDict["刀具类型"].Value;
                test_mGears[23 + i * oneGearParaCount] = mGears[i].mDataStorageDict["齿条刀的齿顶高系数"].Value;
                test_mGears[24 + i * oneGearParaCount] = mGears[i].mDataStorageDict["齿条刀的齿顶圆半径系数"].Value;
                test_mGears[25 + i * oneGearParaCount] = mGears[i].mDataStorageDict["插齿刀齿数"].Value;
                test_mGears[26 + i * oneGearParaCount] = mGears[i].mDataStorageDict["插齿刀加工中心距"].Value;
                test_mGears[27 + i * oneGearParaCount] = mGears[i].mDataStorageDict["插齿刀具齿顶直径"].Value;
                test_mGears[28 + i * oneGearParaCount] = mGears[i].mDataStorageDict["插齿刀基圆直径"].Value;
                test_mGears[29 + i * oneGearParaCount] = mGears[i].mDataStorageDict["插齿刀齿顶形状直径"].Value;
                test_mGears[30 + i * oneGearParaCount] = mGears[i].mDataStorageDict["插齿刀具参数横向压力角[alpha_wt0]"].Value;
            }
            int tmpTwo = 62;
            test_mGears[tmpTwo] = mGears[0].mDataStorageDict["法向模数"].Value;
            test_mGears[tmpTwo + 1] = mGears[0].mDataStorageDict["压力角"].Value;
            test_mGears[tmpTwo + 2] = mGears[0].mDataStorageDict["螺旋角"].Value;
            test_mGears[tmpTwo + 3] = mGears[0].mDataStorageDict["中心距"].Value;
            test_mGears[tmpTwo + 4] = mGears[0].mDataStorageDict["齿轮副类型"].Value;
            test_mGears[tmpTwo + 5] = mGears[0].mDataStorageDict["行星轮个数"].Value;
            test_mGears[tmpTwo + 6] = mGears[0].mDataStorageDict["驱动齿轮"].Value;
            test_mGears[tmpTwo + 7] = mGears[0].mDataStorageDict["触点间隙[j_bn]"].Value;
            test_mGears[tmpTwo + 8] = mTorque.mDataStorageDict["转速"].Value;
            
            int totalAbovePara = 71;//输入参数个数
            //输出结果
            CalGeometry(ref test_mGears[0], ref test_mGears[totalAbovePara]);
            for (int k = 0; k < 2; k++)
            {
                mGears[k].mDataStorageDict["端面啮合角[αwt]"].OutValue = test_mGears[totalAbovePara];// 测试结果
                mGears[k].mDataStorageDict["传动比[i]"].OutValue = test_mGears[totalAbovePara + 1];
                mGears[k].mDataStorageDict["齿数比[u]"].OutValue = test_mGears[totalAbovePara + 2];
                mGears[k].mDataStorageDict["端面理论压力角[αt]"].OutValue = test_mGears[totalAbovePara + 3];
                mGears[k].mDataStorageDict["中心距离[aw]"].OutValue = test_mGears[totalAbovePara + 4];
                mGears[k].mDataStorageDict["齿轮副中齿的工作深度[hw]"].OutValue = test_mGears[totalAbovePara + 5];
                mGears[k].mDataStorageDict["齿廓变位总系数[Σx]"].OutValue = test_mGears[totalAbovePara + 6];
                mGears[k].mDataStorageDict["非零背差的齿廓偏移系数之和[ΣxE]"].OutValue = test_mGears[totalAbovePara + 7];
                mGears[k].mDataStorageDict["接触路径长度[gα]"].OutValue = test_mGears[totalAbovePara + 8];
                mGears[k].mDataStorageDict["端面重合度[εα]"].OutValue = test_mGears[totalAbovePara + 9];
                mGears[k].mDataStorageDict["纵向重合度[εβ]"].OutValue = test_mGears[totalAbovePara + 10];
                mGears[k].mDataStorageDict["纵向重合弧长度[gβ]"].OutValue = test_mGears[totalAbovePara + 11];
                mGears[k].mDataStorageDict["总重合度[εγ]"].OutValue = test_mGears[totalAbovePara + 12];
                mGears[k].mDataStorageDict["参与路径[lmax]"].OutValue = test_mGears[totalAbovePara + 13];
                mGears[k].mDataStorageDict["节圆处的周向间隙[jwt]"].OutValue = test_mGears[totalAbovePara + 14];
                mGears[k].mDataStorageDict["分度圆处的圆周间隙[jt]"].OutValue = test_mGears[totalAbovePara + 15];
                mGears[k].mDataStorageDict["径向间隙[jr]"].OutValue = test_mGears[totalAbovePara + 16];
                mGears[k].mDataStorageDict["[ω1]"].OutValue = test_mGears[totalAbovePara + 17];
                mGears[k].mDataStorageDict["法向速度[vn]"].OutValue = test_mGears[totalAbovePara + 18];
                mGears[k].mDataStorageDict["点Y到节点C的距离[gαy]"].OutValue = test_mGears[totalAbovePara + 19];
                mGears[k].mDataStorageDict["滑动速度[vg]"].OutValue = test_mGears[totalAbovePara + 20];
                mGears[k].mDataStorageDict["滑动系数[Kg]"].OutValue = test_mGears[totalAbovePara + 21];
                mGears[k].mDataStorageDict["[ρA1]"].OutValue = test_mGears[totalAbovePara + 22];
                mGears[k].mDataStorageDict["[ρE2]"].OutValue = test_mGears[totalAbovePara + 23];
                mGears[k].mDataStorageDict["[ρC1]"].OutValue = test_mGears[totalAbovePara + 24];
                mGears[k].mDataStorageDict["[T1C]"].OutValue = test_mGears[totalAbovePara + 25];
                mGears[k].mDataStorageDict["[ρC2]"].OutValue = test_mGears[totalAbovePara + 26];
                mGears[k].mDataStorageDict["[T2C]"].OutValue = test_mGears[totalAbovePara + 27];
                mGears[k].mDataStorageDict["[ρA2]"].OutValue = test_mGears[totalAbovePara + 28];
                mGears[k].mDataStorageDict["[T2A]"].OutValue = test_mGears[totalAbovePara + 29];
                mGears[k].mDataStorageDict["[ρE1]"].OutValue = test_mGears[totalAbovePara + 30];
                mGears[k].mDataStorageDict["[T1E]"].OutValue = test_mGears[totalAbovePara + 31];
                mGears[k].mDataStorageDict["[ρB1]"].OutValue = test_mGears[totalAbovePara + 32];
                mGears[k].mDataStorageDict["[T1B]"].OutValue = test_mGears[totalAbovePara + 33];
                mGears[k].mDataStorageDict["[ρD2]"].OutValue = test_mGears[totalAbovePara + 34];
                mGears[k].mDataStorageDict["[T2D]"].OutValue = test_mGears[totalAbovePara + 35];
                mGears[k].mDataStorageDict["[T1T2]"].OutValue = test_mGears[totalAbovePara + 36];
                mGears[k].mDataStorageDict["纵向重合弧长度[gβ]"].OutValue = test_mGears[totalAbovePara + 37];
                mGears[k].mDataStorageDict["齿顶修形量[Ca]"].OutValue = test_mGears[totalAbovePara + 38];

            }
            int tmpCount = totalAbovePara + 39;
            int mGearIndex = 96; //下面for循环中的数据个数
            for (int k = 0; k < 2; k++)
            {
                mGears[k].mDataStorageDict["分度圆直径[d]"].OutValue = test_mGears[tmpCount + k * mGearIndex];
                mGears[k].mDataStorageDict["端面模数[mt]"].OutValue = test_mGears[1 + tmpCount + k * mGearIndex];
                mGears[k].mDataStorageDict["轴向模数[mx]"].OutValue = test_mGears[2 + tmpCount + k * mGearIndex];
                mGears[k].mDataStorageDict["导程[pz]"].OutValue = test_mGears[3 + tmpCount + k * mGearIndex];
                mGears[k].mDataStorageDict["基圆螺旋角[βb]"].OutValue = test_mGears[4 + tmpCount + k * mGearIndex];
                mGears[k].mDataStorageDict["基圆直径[db]"].OutValue = test_mGears[5 + tmpCount + k * mGearIndex];
                mGears[k].mDataStorageDict["有效齿顶圆直径[dNa]"].OutValue = test_mGears[6 + tmpCount + k * mGearIndex];
                mGears[k].mDataStorageDict["Y圆螺旋角[βy]"].OutValue = test_mGears[7 + tmpCount + k * mGearIndex];
                mGears[k].mDataStorageDict["Y圆导程角[γy]"].OutValue = test_mGears[8 + tmpCount + k * mGearIndex];
                mGears[k].mDataStorageDict["Y圆端面压力角[αyt]"].OutValue = test_mGears[9 + tmpCount + k * mGearIndex];
                mGears[k].mDataStorageDict["有效齿根圆直径[dNF]"].OutValue = test_mGears[10 + tmpCount + k * mGearIndex];
                mGears[k].mDataStorageDict["Y圆法向压力角[αyn]"].OutValue = test_mGears[11 + tmpCount + k * mGearIndex];
                mGears[k].mDataStorageDict["Y圆渐开线上Y点滚动角[ξy]"].OutValue = test_mGears[12 + tmpCount + k * mGearIndex];
                mGears[k].mDataStorageDict["Y圆渐开线曲率半径[ρy]"].OutValue = test_mGears[13 + tmpCount + k * mGearIndex];
                mGears[k].mDataStorageDict["Y圆渐开线函数[invαyt]"].OutValue = test_mGears[14 + tmpCount + k * mGearIndex];
                mGears[k].mDataStorageDict["渐开线函数[invαt]"].OutValue = test_mGears[15 + tmpCount + k * mGearIndex];
                mGears[k].mDataStorageDict["齿距角[τ]"].OutValue = test_mGears[16 + tmpCount + k * mGearIndex];
                mGears[k].mDataStorageDict["分度圆节距[pt]"].OutValue = test_mGears[17 + tmpCount + k * mGearIndex];
                mGears[k].mDataStorageDict["法向节距[pn]"].OutValue = test_mGears[18 + tmpCount + k * mGearIndex];
                mGears[k].mDataStorageDict["Y圆端面齿距[pyt]"].OutValue = test_mGears[19 + tmpCount + k * mGearIndex];
                mGears[k].mDataStorageDict["Y圆法向齿距[pyn]"].OutValue = test_mGears[20 + tmpCount + k * mGearIndex];
                mGears[k].mDataStorageDict["轴向节距[px]"].OutValue = test_mGears[21 + tmpCount + k * mGearIndex];
                mGears[k].mDataStorageDict["基圆节距[pbt]"].OutValue = test_mGears[22 + tmpCount + k * mGearIndex];
                mGears[k].mDataStorageDict["基圆法向节距[pbn]"].OutValue = test_mGears[23 + tmpCount + k * mGearIndex];
                mGears[k].mDataStorageDict["端面啮合齿距[pet]"].OutValue = test_mGears[24 + tmpCount + k * mGearIndex];
                mGears[k].mDataStorageDict["法向啮合齿距[pen]"].OutValue = test_mGears[25 + tmpCount + k * mGearIndex];
                mGears[k].mDataStorageDict["V圆直径[dv]"].OutValue = test_mGears[26 + tmpCount + k * mGearIndex];
                mGears[k].mDataStorageDict["基准齿条齿顶高[haP]"].OutValue = test_mGears[27 + tmpCount + k * mGearIndex];
                mGears[k].mDataStorageDict["基准齿条齿根高[hfP]"].OutValue = test_mGears[28 + tmpCount + k * mGearIndex];
                mGears[k].mDataStorageDict["全齿高[h]"].OutValue = test_mGears[29 + tmpCount + k * mGearIndex];
                mGears[k].mDataStorageDict["工作节圆直径[dw]"].OutValue = test_mGears[30 + tmpCount + k * mGearIndex];
                mGears[k].mDataStorageDict["Y圆端面齿厚[syt]"].OutValue = test_mGears[31 + tmpCount + k * mGearIndex];
                mGears[k].mDataStorageDict["分度圆端面齿厚[st]"].OutValue = test_mGears[32 + tmpCount + k * mGearIndex];
                mGears[k].mDataStorageDict["Y圆齿厚半角[ψy]"].OutValue = test_mGears[33 + tmpCount + k * mGearIndex];
                mGears[k].mDataStorageDict["分度圆齿厚半角[ψ]"].OutValue = test_mGears[34 + tmpCount + k * mGearIndex];
                mGears[k].mDataStorageDict["基圆齿厚半角[ψb]"].OutValue = test_mGears[35 + tmpCount + k * mGearIndex];
                mGears[k].mDataStorageDict["Y圆齿槽宽[eyt]"].OutValue = test_mGears[36 + tmpCount + k * mGearIndex];
                mGears[k].mDataStorageDict["分度圆齿槽宽[et]"].OutValue = test_mGears[37 + tmpCount + k * mGearIndex];
                mGears[k].mDataStorageDict["Y圆齿槽宽半角[ηy]"].OutValue = test_mGears[38 + tmpCount + k * mGearIndex];
                mGears[k].mDataStorageDict["分度圆齿槽宽半角[η]"].OutValue = test_mGears[39 + tmpCount + k * mGearIndex];
                mGears[k].mDataStorageDict["基圆齿槽宽半角[ηb]"].OutValue = test_mGears[40 + tmpCount + k * mGearIndex];
                mGears[k].mDataStorageDict["Y圆法向齿厚[syn]"].OutValue = test_mGears[41 + tmpCount + k * mGearIndex];
                mGears[k].mDataStorageDict["分度圆法向齿厚[sn]"].OutValue = test_mGears[42 + tmpCount + k * mGearIndex];
                mGears[k].mDataStorageDict["Y圆法向齿槽宽[eyn]"].OutValue = test_mGears[43 + tmpCount + k * mGearIndex];
                mGears[k].mDataStorageDict["分度圆法向齿槽宽[en]"].OutValue = test_mGears[44 + tmpCount + k * mGearIndex];
                mGears[k].mDataStorageDict["分度圆上的最大正常齿厚度[sns]"].OutValue = test_mGears[45 + tmpCount + k * mGearIndex];
                mGears[k].mDataStorageDict["分度圆上的最小正常齿厚度[sni]"].OutValue = test_mGears[46 + tmpCount + k * mGearIndex];
                mGears[k].mDataStorageDict["产形齿廓变位系数[xEs]"].OutValue = test_mGears[47 + tmpCount + k * mGearIndex];
                mGears[k].mDataStorageDict["产形齿廓变位系数[xEi]"].OutValue = test_mGears[48 + tmpCount + k * mGearIndex];
                mGears[k].mDataStorageDict["跨棒距[Wk]"].OutValue = test_mGears[49 + tmpCount + k * mGearIndex];
                mGears[k].mDataStorageDict["[xEsV]"].OutValue = test_mGears[50 + tmpCount + k * mGearIndex];
                mGears[k].mDataStorageDict["[xEiV]"].OutValue = test_mGears[51 + tmpCount + k * mGearIndex];
                mGears[k].mDataStorageDict["无根切变位系数[xEmin]"].OutValue = test_mGears[52 + tmpCount + k * mGearIndex];

                mGears[k].mDataStorageDict["齿顶圆直径[da]"].OutValue = test_mGears[53 + tmpCount + k * mGearIndex];
                mGears[k].mDataStorageDict["齿顶成形圆直径[dFa]"].OutValue = test_mGears[54 + tmpCount + k * mGearIndex];
                mGears[k].mDataStorageDict["齿根圆直径[df]"].OutValue = test_mGears[55 + tmpCount + k * mGearIndex];//基于齿条加工的齿根圆[dfE]应该就是max和min值    
                mGears[k].mDataStorageDict["齿顶高[ha]"].OutValue = test_mGears[56 + tmpCount + k * mGearIndex];
                mGears[k].mDataStorageDict["齿根成形圆直径[dFf]"].OutValue = test_mGears[57 + tmpCount + k * mGearIndex];
                mGears[k].mDataStorageDict["齿根高[hf]"].OutValue = test_mGears[58 + tmpCount + k * mGearIndex];
                mGears[k].mDataStorageDict["d_a[max]"].OutValue = test_mGears[59 + tmpCount + k * mGearIndex];
                mGears[k].mDataStorageDict["d_Fa[max]"].OutValue = test_mGears[60 + tmpCount + k * mGearIndex];
                mGears[k].mDataStorageDict["d_f[max]"].OutValue = test_mGears[61 + tmpCount + k * mGearIndex];
                mGears[k].mDataStorageDict["h_a[max]"].OutValue = test_mGears[62 + tmpCount + k * mGearIndex];
                mGears[k].mDataStorageDict["d_Ff[max]"].OutValue = test_mGears[63 + tmpCount + k * mGearIndex];
                mGears[k].mDataStorageDict["h_f[max]"].OutValue = test_mGears[64 + tmpCount + k * mGearIndex];
                mGears[k].mDataStorageDict["d_a[min]"].OutValue = test_mGears[65 + tmpCount + k * mGearIndex];
                mGears[k].mDataStorageDict["d_Fa[min]"].OutValue = test_mGears[66 + tmpCount + k * mGearIndex];
                mGears[k].mDataStorageDict["d_f[min]"].OutValue = test_mGears[67 + tmpCount + k * mGearIndex];
                mGears[k].mDataStorageDict["h_a[min]"].OutValue = test_mGears[68 + tmpCount + k * mGearIndex];
                mGears[k].mDataStorageDict["d_Ff[min]"].OutValue = test_mGears[69 + tmpCount + k * mGearIndex];
                mGears[k].mDataStorageDict["h_f[min]"].OutValue = test_mGears[70 + tmpCount + k * mGearIndex];

                mGears[k].mDataStorageDict["接触齿根路径的长度[gf]"].OutValue = test_mGears[71 + tmpCount + k * mGearIndex];
                mGears[k].mDataStorageDict["接触齿顶路径的长度[ga]"].OutValue = test_mGears[72 + tmpCount + k * mGearIndex];
                mGears[k].mDataStorageDict["等效齿数[zn]"].OutValue = test_mGears[73 + tmpCount + k * mGearIndex];
                mGears[k].mDataStorageDict["齿根高系数[hfx]"].OutValue = test_mGears[74 + tmpCount + k * mGearIndex];
                mGears[k].mDataStorageDict["平均齿廓直径[dm]"].OutValue = test_mGears[75 + tmpCount + k * mGearIndex];
                mGears[k].mDataStorageDict["基圆半径[rb]"].OutValue = test_mGears[76 + tmpCount + k * mGearIndex];
                mGears[k].mDataStorageDict["平均齿廓半径[rm]"].OutValue = test_mGears[77 + tmpCount + k * mGearIndex];
                mGears[k].mDataStorageDict["齿顶圆半径[ra]"].OutValue = test_mGears[78 + tmpCount + k * mGearIndex];
                mGears[k].mDataStorageDict["齿根圆半径[rf]"].OutValue = test_mGears[79 + tmpCount + k * mGearIndex];
                mGears[k].mDataStorageDict["储备(dNf-dFf)/2[cF]"].OutValue = test_mGears[80 + tmpCount + k * mGearIndex];
                mGears[k].mDataStorageDict["齿厚公差[Tsn]"].OutValue = test_mGears[81 + tmpCount + k * mGearIndex];
                mGears[k].mDataStorageDict["齿厚公差[Tsnv]"].OutValue = test_mGears[82 + tmpCount + k * mGearIndex];
                mGears[k].mDataStorageDict["刀尖圆角半径[ρfp]"].OutValue = test_mGears[83 + tmpCount + k * mGearIndex];
                mGears[k].mDataStorageDict["齿条型刀具齿顶高[hFaP0]"].OutValue = test_mGears[84 + tmpCount + k * mGearIndex];
                mGears[k].mDataStorageDict["理论齿顶间隙[c]"].OutValue = test_mGears[85 + tmpCount + k * mGearIndex];
                mGears[k].mDataStorageDict["端面作用角[φα]"].OutValue = test_mGears[86 + tmpCount + k * mGearIndex];
                mGears[k].mDataStorageDict["纵向作用角[φβ]"].OutValue = test_mGears[87 + tmpCount + k * mGearIndex];
                mGears[k].mDataStorageDict["总作用角[φγ]"].OutValue = test_mGears[88 + tmpCount + k * mGearIndex];
                mGears[k].mDataStorageDict["间隙角[φj]"].OutValue = test_mGears[89 + tmpCount + k * mGearIndex];
                mGears[k].mDataStorageDict["齿根处的滑动速度[vgf]"].OutValue = test_mGears[90 + tmpCount + k * mGearIndex];
                mGears[k].mDataStorageDict["齿顶滑动速度[vga]"].OutValue = test_mGears[91 + tmpCount + k * mGearIndex];
                mGears[k].mDataStorageDict["齿根上的滑动系数[Kgf]"].OutValue = test_mGears[92 + tmpCount + k * mGearIndex];
                mGears[k].mDataStorageDict["齿顶上的滑动系数[Kga]"].OutValue = test_mGears[93 + tmpCount + k * mGearIndex];
                mGears[k].mDataStorageDict["比滑[ζ]"].OutValue = test_mGears[94 + tmpCount + k * mGearIndex];
                mGears[k].mDataStorageDict["齿根上的比滑[ζf]"].OutValue = test_mGears[95 + tmpCount + k * mGearIndex];
            }


        }
        void ConvertGeometryInput(int InStartIndex, double[] IODllInputData)
        {
            //double[,] mDllGeometryData = new double[3, 14];
            //double[,] mDllMateriaData = new double[4, 8];
            //double[,] mDllAppData = new double[3, 8];
            //double[,] mDllTolerenceData = new double[2, 6];
            //double[,] mDllLoadData = new double[1, 3];
            //double[,] mDllAssembleData = new double[1, 6];
            //double[,] mDllLocData = new double[2, 2];
            int tmpStartIndex = InStartIndex;
            int tmpColumnCount = CTANInvGearPair.mDivisionIndexList[sGearParaStartIndex];
            for (int i = 0; i < 2; i++)
            {
                IODllInputData[tmpStartIndex + 0] = mGears[i].mDataStorageDict["齿数"].Value;
                IODllInputData[tmpStartIndex + 1] = mGears[i].mDataStorageDict["旋向"].Value;
                IODllInputData[tmpStartIndex + 2] = mGears[i].mDataStorageDict["设计齿宽"].Value;
                IODllInputData[tmpStartIndex + 3] = mGears[i].mDataStorageDict["齿顶高系数"].Value;
                IODllInputData[tmpStartIndex + 4] = mGears[i].mDataStorageDict["高变位系数"].Value;
                IODllInputData[tmpStartIndex + 5] = mGears[i].mDataStorageDict["顶隙系数"].Value;
                IODllInputData[tmpStartIndex + 6] = mGears[i].mDataStorageDict["齿根圆角系数"].Value;
                IODllInputData[tmpStartIndex + 7] = mGears[i].mDataStorageDict["挖根量"].Value;
                IODllInputData[tmpStartIndex + 8] = mGears[i].mDataStorageDict["齿顶修形量"].Value;
                IODllInputData[tmpStartIndex + 9] = mGears[i].mDataStorageDict["齿根修形量"].Value;
                IODllInputData[tmpStartIndex + 10] = mGears[i].mDataStorageDict["切向变位系数"].Value;
                IODllInputData[tmpStartIndex + 11] = mGears[i].mDataStorageDict["跨齿数"].Value;
                IODllInputData[tmpStartIndex + 12] = mGears[i].mDataStorageDict["齿顶倒角量"].Value;
                IODllInputData[tmpStartIndex + 13] = 0;//轮幅外圆直径
                tmpStartIndex = tmpStartIndex + tmpColumnCount;
            }
            IODllInputData[tmpStartIndex + 0] = mGears[0].mDataStorageDict["法向模数"].Value;
            IODllInputData[tmpStartIndex + 1] = mGears[0].mDataStorageDict["压力角"].Value * CTANConstantValue.Degree;
            IODllInputData[tmpStartIndex + 2] = mGears[0].mDataStorageDict["螺旋角"].Value * CTANConstantValue.Degree;
            IODllInputData[tmpStartIndex + 3] = mGears[0].mDataStorageDict["中心距"].Value;
            IODllInputData[tmpStartIndex + 4] = 1;//齿轮副类型，一般外啮合齿轮副
            IODllInputData[tmpStartIndex + 5] = mGears[0].mDataStorageDict["行星轮个数"].Value;
        }
        void ConvertGeometryInputZJ(int InStartIndex, double[] IODllInputData)
        {
            //double[,] mDllGeometryData = new double[3, 14];
            //double[,] mDllMateriaData = new double[4, 8];
            //double[,] mDllAppData = new double[3, 8];
            //double[,] mDllTolerenceData = new double[2, 6];
            //double[,] mDllLoadData = new double[1, 3];
            //double[,] mDllAssembleData = new double[1, 6];
            //double[,] mDllLocData = new double[2, 2];
            int tmpStartIndex = InStartIndex;
            int tmpColumnCount = CTANInvGearPair.mDivisionIndexList[sGearParaStartIndex];
            for (int i = 0; i < 2; i++)
            {
                IODllInputData[tmpStartIndex + 0] = mGears[i].mDataStorageDict["齿数"].Value;
                IODllInputData[tmpStartIndex + 1] = mGears[i].mDataStorageDict["旋向"].Value;
                IODllInputData[tmpStartIndex + 2] = mGears[i].mDataStorageDict["设计齿宽"].Value;
                IODllInputData[tmpStartIndex + 3] = mGears[i].mDataStorageDict["齿顶高系数"].Value;
                IODllInputData[tmpStartIndex + 4] = mGears[i].mDataStorageDict["齿廓变位系数"].Value;
                IODllInputData[tmpStartIndex + 5] = mGears[i].mDataStorageDict["顶隙系数"].Value;
                IODllInputData[tmpStartIndex + 6] = mGears[i].mDataStorageDict["齿根圆角系数"].Value;
                IODllInputData[tmpStartIndex + 7] = mGears[i].mDataStorageDict["挖根量"].Value;
                IODllInputData[tmpStartIndex + 8] = mGears[i].mDataStorageDict["齿顶修形量"].Value;
                IODllInputData[tmpStartIndex + 9] = mGears[i].mDataStorageDict["齿根修形量"].Value;
                IODllInputData[tmpStartIndex + 10] = mGears[i].mDataStorageDict["是否使用滚动角"].Value;
                IODllInputData[tmpStartIndex + 11] = mGears[i].mDataStorageDict["跨齿数"].Value;
                IODllInputData[tmpStartIndex + 12] = mGears[i].mDataStorageDict["齿顶倒角量"].Value;
                IODllInputData[tmpStartIndex + 13] = mGears[i].mDataStorageDict["轮幅外圆直径"].Value;

                IODllInputData[tmpStartIndex + 14] = mGears[i].mDataStorageDict["齿轮齿顶高变化[k·mn]"].Value;
                IODllInputData[tmpStartIndex + 15] = mGears[i].mDataStorageDict["齿厚的偏差上限[E_sns]"].Value;
                IODllInputData[tmpStartIndex + 16] = mGears[i].mDataStorageDict["齿厚的偏差下限[E_sni]"].Value;
                IODllInputData[tmpStartIndex + 17] = mGears[i].mDataStorageDict["齿顶圆容差上限"].Value;
                IODllInputData[tmpStartIndex + 18] = mGears[i].mDataStorageDict["齿顶圆容差下限"].Value;
                IODllInputData[tmpStartIndex + 19] = mGears[i].mDataStorageDict["最大机械加工余量[q_max]"].Value;
                IODllInputData[tmpStartIndex + 20] = mGears[i].mDataStorageDict["最小机械加工余量[q_min]"].Value;
                IODllInputData[tmpStartIndex + 21] = mGears[i].mDataStorageDict["Y圆直径[d_y]"].Value;
                IODllInputData[tmpStartIndex + 22] = mGears[i].mDataStorageDict["刀具类型"].Value;
                IODllInputData[tmpStartIndex + 23] = mGears[i].mDataStorageDict["齿条刀的齿顶高系数"].Value;
                IODllInputData[tmpStartIndex + 24] = mGears[i].mDataStorageDict["齿条刀的齿顶圆半径系数"].Value;
                IODllInputData[tmpStartIndex + 25] = mGears[i].mDataStorageDict["插齿刀齿数"].Value;
                IODllInputData[tmpStartIndex + 26] = mGears[i].mDataStorageDict["插齿刀加工中心距"].Value;
                IODllInputData[tmpStartIndex + 27] = mGears[i].mDataStorageDict["插齿刀具齿顶直径"].Value;
                IODllInputData[tmpStartIndex + 28] = mGears[i].mDataStorageDict["插齿刀基圆直径"].Value;
                IODllInputData[tmpStartIndex + 29] = mGears[i].mDataStorageDict["插齿刀齿顶形状直径"].Value;
                IODllInputData[tmpStartIndex + 30] = mGears[i].mDataStorageDict["插齿刀具参数横向压力角[alpha_wt0]"].Value;             
                tmpStartIndex = tmpStartIndex + tmpColumnCount;
            }
            IODllInputData[tmpStartIndex + 0] = mGears[0].mDataStorageDict["法向模数"].Value;
            IODllInputData[tmpStartIndex + 1] = mGears[0].mDataStorageDict["压力角"].Value * CTANConstantValue.Degree;
            IODllInputData[tmpStartIndex + 2] = mGears[0].mDataStorageDict["螺旋角"].Value * CTANConstantValue.Degree;
            IODllInputData[tmpStartIndex + 3] = mGears[0].mDataStorageDict["中心距"].Value;
            IODllInputData[tmpStartIndex + 4] = 1;//齿轮副类型，一般外啮合齿轮副
            IODllInputData[tmpStartIndex + 5] = mGears[0].mDataStorageDict["行星轮个数"].Value;
            IODllInputData[tmpStartIndex + 6] = mGears[0].mDataStorageDict["驱动齿轮"].Value;
            IODllInputData[tmpStartIndex + 7] = mGears[0].mDataStorageDict["触点间隙[j_bn]"].Value;
            IODllInputData[tmpStartIndex + 8] = mTorque.mDataStorageDict["转速"].Value;
        }
        void ConvertMaterialInput(int InStartIndex, double[] IODllInputData)
        {
            int tmpStartIndex = InStartIndex;
            int tmpColumnCount = CTANInvGearPair.mDivisionIndexList[sGearParaStartIndex + 4*1];
            for (int i = 0; i < 2; i++)
            {
                //材料和加工参数tmpStartIndex +
                IODllInputData[tmpStartIndex + 0] = mGears[i].mDataStorageDict["弹性模量"].Value;
                IODllInputData[tmpStartIndex + 1] = mGears[i].mDataStorageDict["泊松比"].Value;
                IODllInputData[tmpStartIndex + 2] = mGears[i].mDataStorageDict["屈服强度"].Value;
                IODllInputData[tmpStartIndex + 3] = mGears[i].mDataStorageDict["密度"].Value;
                IODllInputData[tmpStartIndex + 4] = mGears[i].mDataStorageDict["试验齿轮接触疲劳极限应力"].Value;
                IODllInputData[tmpStartIndex + 5] = mGears[i].mDataStorageDict["试验齿轮弯曲疲劳极限应力"].Value;
                IODllInputData[tmpStartIndex + 6] = IODllInputData[tmpStartIndex + 2];
                IODllInputData[tmpStartIndex + 7] = mGears[i].mDataStorageDict["抗拉强度"].Value;
                tmpStartIndex = tmpStartIndex + tmpColumnCount;
                IODllInputData[tmpStartIndex + 0] = mGears[i].mDataStorageDict["材料类型"].Value + 1;
                IODllInputData[tmpStartIndex + 1] = mGears[i].mDataStorageDict["加工质量"].Value;
                IODllInputData[tmpStartIndex + 2] = mGears[i].mDataStorageDict["表面硬度(HRC)"].Value;
                IODllInputData[tmpStartIndex + 3] = mGears[i].mDataStorageDict["芯部硬度(HRC)"].Value;
                IODllInputData[tmpStartIndex + 4] = mGears[i].mDataStorageDict["表面是否喷丸"].Value;
                IODllInputData[tmpStartIndex + 5] = mGears[i].mDataStorageDict["是否允许表面出现有限点蚀"].Value;
                tmpStartIndex = tmpStartIndex + tmpColumnCount;
            }
        }
        void ConverAppInput(int InStartIndex, double[] IODllInputData)
        {
            int tmpStartIndex = InStartIndex;
            int tmpColumnCount = CTANInvGearPair.mDivisionIndexList[sGearParaStartIndex + 4 * 2];
            for (int i = 0; i < 2; i++)
            {
                //材料和加工参数tmpStartIndex +
                IODllInputData[tmpStartIndex + 0] = mGears[i].mDataStorageDict["轮齿是否反向弯折"].Value;
                IODllInputData[tmpStartIndex + 1] = mGears[i].mDataStorageDict["是否对接触区进行控制"].Value;
                IODllInputData[tmpStartIndex + 2] = mGears[i].mDataStorageDict["螺旋线修形方式"].Value;
                IODllInputData[tmpStartIndex + 3] = mGears[i].mDataStorageDict["齿廓修形是否能够补偿变形"].Value;
                IODllInputData[tmpStartIndex + 4] = mGears[i].mDataStorageDict["行星轮个数"].Value;
                IODllInputData[tmpStartIndex + 5] = mGears[i].mDataStorageDict["齿坯类型"].Value;
                IODllInputData[tmpStartIndex + 6] = mGears[i].mDataStorageDict["接触最小安全系数"].Value;
                IODllInputData[tmpStartIndex + 7] = mGears[i].mDataStorageDict["弯曲最小安全系数"].Value;
                tmpStartIndex = tmpStartIndex + tmpColumnCount;
            }
            IODllInputData[tmpStartIndex + 0] = mGears[0].mDataStorageDict["使用系数"].Value;
            IODllInputData[tmpStartIndex + 1] = mGears[0].mDataStorageDict["均载系数"].Value;
            IODllInputData[tmpStartIndex + 2] = mGears[0].mDataStorageDict["轴承间距偏移量"].Value;
            IODllInputData[tmpStartIndex + 3] = mGears[0].mDataStorageDict["润滑油粘度(v40)"].Value;
            IODllInputData[tmpStartIndex + 4] = mGears[0].mDataStorageDict["修形方法"].Value;
            IODllInputData[tmpStartIndex + 5] = mGears[0].mDataStorageDict["接触最小安全系数"].Value;
            IODllInputData[tmpStartIndex + 6] = mGears[0].mDataStorageDict["弯曲最小安全系数"].Value;
        }
        void ConvertTolerenceInput(int InStartIndex, double[] IODllInputData)
        {
            int tmpStartIndex = InStartIndex;
            int tmpColumnCount = CTANInvGearPair.mDivisionIndexList[sGearParaStartIndex + 4 * 3];
            for (int i = 0; i < 2; i++)
            {
                //精度数据
                IODllInputData[tmpStartIndex + 0] = mGears[i].mDataStorageDict["精度等级"].Value;
                IODllInputData[tmpStartIndex + 1] = mGears[i].mDataStorageDict["端面节线偏差"].Value;
                IODllInputData[tmpStartIndex + 2] = mGears[i].mDataStorageDict["齿廓偏差"].Value;
                IODllInputData[tmpStartIndex + 3] = mGears[i].mDataStorageDict["螺旋线偏差"].Value;
                IODllInputData[tmpStartIndex + 4] = mGears[i].mDataStorageDict["接触表面粗糙度(Ra)"].Value;
                IODllInputData[tmpStartIndex + 5] = mGears[i].mDataStorageDict["齿根表面粗糙度(Ra)"].Value;
                tmpStartIndex = tmpStartIndex + tmpColumnCount;
            }
        }
        void ConvertLoadInput(int InStartIndex, double[] IODllInputData)
        {
            int tmpStartIndex = InStartIndex;
            IODllInputData[tmpStartIndex + 0] = mTorque.mDataStorageDict["扭矩"].Value;
            IODllInputData[tmpStartIndex + 1] = mTorque.mDataStorageDict["转速"].Value;
            IODllInputData[tmpStartIndex + 2] = mTorque.mDataStorageDict["预期寿命"].Value;
        }
        void ConvertAssembleInput(int InStartIndex, double[] IODllInputData)
        {
            int tmpStartIndex = InStartIndex;
            IODllInputData[tmpStartIndex + 0] = mGears[0].mDataStorageDict["安装类型"].Value;
            IODllInputData[tmpStartIndex + 1] = mGears[0].mDataStorageDict["是否为刚性轴"].Value;
            IODllInputData[tmpStartIndex + 2] = mGears[0].mDataStorageDict["行星轮个数"].Value;
            IODllInputData[tmpStartIndex + 3] = mGears[0].mDataStorageDict["太阳轮轴直径"].Value;
            IODllInputData[tmpStartIndex + 4] = mGears[0].mDataStorageDict["太阳轮齿宽中点至轴承跨距中点距离"].Value;
            IODllInputData[tmpStartIndex + 5] = mGears[0].mDataStorageDict["轴承跨距"].Value;
        }
        void ConvertLocInput(int InStartIndex, double[] IODllInputData)
        {
            int tmpStartIndex = InStartIndex;
            int tmpColumnCount = CTANInvGearPair.mDivisionIndexList[sGearParaStartIndex + 4 * 6];
            for (int i = 0; i < 2; i++)
            {
                //精度数据
                IODllInputData[tmpStartIndex + 0] = mGears[i].mDataStorageDict["定位面距基准面距离"].Value;
                IODllInputData[tmpStartIndex + 1] = mGears[i].mDataStorageDict["定位面"].Value;
                tmpStartIndex = tmpStartIndex + tmpColumnCount;
            }
        }


        public override void UpdateDllInputDoubleList(int InStartIndex, double[] IODllInputData)
        {
            int tmpStartIndex = InStartIndex;
            int tmpInitialIndexOffset = sGearParaStartIndex - 1;
            ConvertGeometryInputZJ(tmpStartIndex, IODllInputData);
            
            tmpStartIndex = InStartIndex + CTANInvGearPair.mDivisionIndexList[tmpInitialIndexOffset + 4 * 1];
            ConvertMaterialInput(tmpStartIndex , IODllInputData);
            tmpStartIndex = InStartIndex + CTANInvGearPair.mDivisionIndexList[tmpInitialIndexOffset + 4 * 2];
            ConverAppInput(tmpStartIndex, IODllInputData);
            tmpStartIndex = InStartIndex + CTANInvGearPair.mDivisionIndexList[tmpInitialIndexOffset + 4 * 3];
            ConvertTolerenceInput(tmpStartIndex, IODllInputData);
            tmpStartIndex = InStartIndex + CTANInvGearPair.mDivisionIndexList[tmpInitialIndexOffset + 4 * 4];
            ConvertLoadInput(tmpStartIndex, IODllInputData);
            tmpStartIndex = InStartIndex + CTANInvGearPair.mDivisionIndexList[tmpInitialIndexOffset + 4 * 5];
            ConvertAssembleInput(tmpStartIndex, IODllInputData);
            tmpStartIndex = InStartIndex + CTANInvGearPair.mDivisionIndexList[tmpInitialIndexOffset + 4 * 6];
            ConvertLocInput(tmpStartIndex, IODllInputData);
            tmpStartIndex = InStartIndex + CTANInvGearPair.mDivisionIndexList[tmpInitialIndexOffset + 4 * 7];
        }
        
        void ConvertStressOutput(int InStartIndex, double[] IODllOutputData)
        {
            int tmpStartIndex = InStartIndex;
            int tmpColumnCount = CTANInvGearPair.mDivisionIndexList[sGearParaStartIndex + 4 * 7];
            for (int i = 0; i < 2; i++)
            {
                for (int j = 0; j < tmpColumnCount; j++)
                {
                    mGears[i].mDataStorageDict[CTANInvGear.mOutPutHeadName[2 * j]].OutValue = IODllOutputData[tmpStartIndex+j+i* tmpColumnCount];
                }
            }
        }
        public override void UpdateDllOutputDoubleList(int InStartIndex, double[] IODllOutputData)
        {
            ConvertStressOutput(InStartIndex, IODllOutputData);
        }


        public void UpdateDllInputDoubleList()
        {
            UpdateDllInputDoubleList(mBelongedSet.mDivisionIndexList[1, 1], mBelongedSet.mInputDoubleDataStorage);
        }
        public void UpdateDllOutputDoubleList()
        {
            UpdateDllOutputDoubleList(mBelongedSet.mDivisionIndexList[1, 2], mBelongedSet.mOutputDoubleDataStorage);
        }
        public override void CalPageData()
        {

            UpdateDllInputDoubleList();
            CalInvGearStress2(ref mBelongedSet.mPartsDivisionIndexList[mBelongedSet.mDivisionIndexList[1, 0]], 
                ref mBelongedSet.mInputDoubleDataStorage[mBelongedSet.mDivisionIndexList[1, 1]], 
                ref mBelongedSet.mOutputDoubleDataStorage[mBelongedSet.mDivisionIndexList[1, 2]]);
            UpdateDllOutputDoubleList();
        }
        private void SetOutputPage()
        {
            // 清除原有内容
            mOutputPageGrid.Children.Clear();

            // 定义标签页和对应的输出名称
            var tabDefinitions = new[]
            {
                new { Header = "应力参数", Names = CTANInvGear.mOutStressName },
                new { Header = "几何参数", Names = CTANInvGear.mOutGeometryName },
                new { Header = "接触参数", Names = CTANInvGear.mOutContactName },
                new { Header = "滑动参数", Names = CTANInvGear.mOutSlideName },
                new { Header = "齿廓参数", Names = CTANInvGear.mOutToothProfileName },
                new { Header = "特殊参数", Names = CTANInvGear.mOutSpecialName },
                new { Header = "其他参数", Names = CTANInvGear.mOutOthersName },
            };

            // 初始化输出表格数组
            CTANBasicFunction.CreatObjectList(tabDefinitions.Length, out mOutputPairTables);

            // 创建TabControl
            mOutPairTabControl = new TabControl
            {
                TabStripPlacement = Dock.Top,
                Margin = new Thickness(5)
            };

            // 创建每个标签页
            for (int i = 0; i < tabDefinitions.Length; i++)
            {
                // 创建标签页
                TabItem tabItem = new TabItem
                {
                    Header = tabDefinitions[i].Header,
                    Name = $"tabOutput{i}"
                };
                // 创建并初始化数据表格
                mOutputPairTables[i] = new CTANDataTable();
                mOutputPairTables[i].SetGearStandardOutputTable(mHeadText, tabDefinitions[i].Names, 2);
                tabItem.Content = mOutputPairTables[i].mDataGrid;
                mOutPairTabControl.Items.Add(tabItem);
            }
            BindingDictToOutputTableZJ(mOutputPairTables, mGears[0].mDataStorageDict, 2);
            BindingDictToOutputTableZJ(mOutputPairTables, mGears[1].mDataStorageDict, 3);
            for (int i = 0; i < 2; i++)
            {
                for(int j = 0; j < mOutputPairTables.Length; j++)
                {
                    mGears[i].mOutputTables[j].mDataGrid.SetBinding(DataGrid.ItemsSourceProperty, new Binding() { Source = this.mOutputPairTables[j] });
                    DataGridTextColumn TmpColumn = mGears[i].mOutputTables[j].mDataGrid.Columns[2] as DataGridTextColumn;
                    TmpColumn.Binding = new Binding(mOutputPairTables[j].Columns[2 + i].ColumnName);
                }
            }
            mOutputPageGrid.Children.Add(mOutPairTabControl);
            // 设置默认选中的Tab
            if (mOutPairTabControl.Items.Count > 0)
            {
                mOutPairTabControl.SelectedIndex = 0;
            }
        }
        //private void SetOutputPage()
        //{
        //    mOutputPageGrid.SetGridRowAndColumn(2, 1);
        //    mOutputDataTable.SetGearStandardOutputTable(mHeadText,CTANInvGear.mOutPutHeadName, 2);
        //    BindingDictToOutputTable(mGears[0].mDataStorageDict,2);
        //    BindingDictToOutputTable(mGears[1].mDataStorageDict, 3);
        //    for(int i = 0; i < 2; i++)
        //    {
        //        mGears[i].mOutputDataTable.mDataGrid.SetBinding(DataGrid.ItemsSourceProperty, new Binding() { Source = this.mOutputDataTable });
        //        DataGridTextColumn TmpColumn = mGears[i].mOutputDataTable.mDataGrid.Columns[2] as DataGridTextColumn;
        //        TmpColumn.Binding = new Binding(mOutputDataTable.Columns[2+i].ColumnName);
        //    }
        //    mOutputPageGrid.Children.Add(mOutputDataTable.mDataGrid);
        //}
    }
}
