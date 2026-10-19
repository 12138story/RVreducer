using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Globalization;
using System.Linq;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace RVReducerDesignApp
{

    public partial class ReducerPage:BasicPage
    {
        int mInputDataCount = 7;
        int mOutputDataCount = 4;
        string[] mHeadName = new string[]{"名称","单位",
            "减速器输出部件","",
            "减速器输出力矩", "牛米" ,
            "减速器输出转速", "转/分钟",
            "太阳轮齿数", "个",
            "行星轮齿数", "个",
            "摆线轮齿数", "个",
            "针轮齿数", "个",
            "太阳轮力矩", "牛米",
            "太阳轮转速", "转/分钟",
            "摆线轮力矩", "牛米",
            "摆线轮转速", "转/分钟"};
        List<TextBox> mIOTextBox=new List<TextBox>();
        ComboBox mExportElement = new ComboBox();
        Dictionary<string, CTANDouble> mDataStorageDict = new Dictionary<string, CTANDouble>();
        public Grid mPageGrid = new Grid();
        public List<List<TextBox>> mTextBlock = new List<List<TextBox>>();
        CTANRVBasic TestGrid= new CTANRVBasic();
        //double InValue = 4;
        //double OutValue = 4;
        //public double DoubleValue { get; set; }
        public ReducerPage()
        {
            InitializeComponent();
            //TestGrid.SetDataAndPageLayout();
            Content = TestGrid.mPageGrid;
            
            //CreatTextGridLayout(mInputDataCount + mOutputDataCount + 1, 3, ref mTextBlock, ref mPageGrid);
            ////CreatBasicPageLayout(mInputDataCount + mOutputDataCount + 1, 3);
            //Content = mPageGrid;
            //SetGridRowHeight(ref mPageGrid);
            //SetNameToGridTextBlock(ref mHeadName, ref mTextBlock, ref mPageGrid);
            //for (int i = 0; i < mInputDataCount + mOutputDataCount - 1; i++)
            //{

            //    mIOTextBox.Add(new TextBox());
            //    //mIOTextBox[i].DataContext = mIOTextBox[i+2].Text;
            //    Grid.SetRow(mIOTextBox[i], i + 2);
            //    Grid.SetColumn(mIOTextBox[i], mPageGrid.ColumnDefinitions.Count - 1);
            //    mPageGrid.Children.Add(mIOTextBox[i]);
            //}
            //for (int i = mInputDataCount - 1; i < mInputDataCount + mOutputDataCount - 1; i++)
            //{
            //    mIOTextBox[i].IsReadOnly = true;
            //    mIOTextBox[i].BorderThickness = new Thickness(0);
            //}
            //mExportElement.Items.Add("转臂输出");
            //mExportElement.Items.Add("针齿壳输出");
            //Grid.SetRow(mExportElement, 1);
            //Grid.SetColumn(mExportElement, mPageGrid.ColumnDefinitions.Count - 1);
            //mPageGrid.Children.Add(mExportElement);
            ////数据绑定
            ////设置数据存储字典
            //SetDataStorageDictionary(1, ref mTextBlock, ref mDataStorageDict);
            //for (int i = 0; i < mIOTextBox.Count; i++)
            //{
            //    mIOTextBox[i].SetBinding(TextBox.TextProperty,
            //        new Binding("[" + mTextBlock[i+2][0].Text + "].Value") { Source = mDataStorageDict, 
            //            UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });
            //}
        }
        public void CalPageData()
        {
            double[] TempIOputData = new double[mInputDataCount + mOutputDataCount];
            //double[] TempOutputData = new double[4];
            if (mExportElement.SelectedIndex == 0)
            {
                TempIOputData[0] = 0;
            }
            else
            {
                TempIOputData[0] = 1;
            }
            TempIOputData[1] = mDataStorageDict["减速器输出力矩"].Value;
            TempIOputData[2] = mDataStorageDict["减速器输出转速"].Value;
            TempIOputData[3] = mDataStorageDict["太阳轮齿数"].Value;
            TempIOputData[4] = mDataStorageDict["行星轮齿数"].Value;
            TempIOputData[5] = mDataStorageDict["摆线轮齿数"].Value;
            TempIOputData[6] = mDataStorageDict["针轮齿数"].Value;
            //TempInputData[1] = Convert.ToDouble(mInputBoxList[1].Text);

            //InterfaceInputDataList[1] = 2;
            //InterfaceOutputDataList[0] = 5;
            MainWindow.CalTotalTorqueAndRPM(ref TempIOputData[0], ref TempIOputData[7]);
            mDataStorageDict["太阳轮力矩"].Value= TempIOputData[7] ;
            mDataStorageDict["太阳轮转速"].Value = TempIOputData[8];
            mDataStorageDict["摆线轮力矩"].Value = TempIOputData[9];
            mDataStorageDict["摆线轮转速"].Value = TempIOputData[10];

        }
        //public void TestFun()
        //{

        //    int t = 0;
        //    InValue = 11;
        //}
    }
}
