using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;

namespace RVReducerDesignApp
{
    //基础输入
    class CTANRVBasic
    {
        static int mInputDataCount = 7;
        static int mOutputDataCount = 4;
        static string[] mHeadName = new string[]{"名称","单位",
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
        //数据存储词典
        CTANDict mDataStorageDict = new CTANDict();
        //一般输入构件
        List<TextBox> mDataIOTextBoxList = new List<TextBox>();
        //输出构件
        ComboBox mExportElement = new ComboBox();
        public Grid mPageGrid = new Grid();
        public List<List<TextBox>> mDataHeadTable = new List<List<TextBox>>();
        public CTANRVBasic()
        {
            SetDataAndPageLayout();
        }
        //页面布局及数据绑定设定
        public void SetDataAndPageLayout()
        {
            //界面布局,创建界面网络
            CTANPageLayoutFunction.CreatTextGridLayout(mInputDataCount + mOutputDataCount + 1, 3, ref mDataHeadTable, ref mPageGrid);
            //设置表格宽度
            CTANPageLayoutFunction.SetGridRowHeight(ref mPageGrid);
            //设置表头
            CTANPageLayoutFunction.SetNameToGridTextBox(ref mHeadName, ref mDataHeadTable, ref mPageGrid);
            //创建输入列
            CTANPageLayoutFunction.CreatAndAddTextBoxToGrid(mPageGrid.RowDefinitions.Count - 2, 2, mPageGrid.ColumnDefinitions.Count - 1, ref mDataIOTextBoxList, ref mPageGrid);
            //设置输出栏不可选
            CTANPageLayoutFunction.SetTexBoxAsTextBlockInList(mDataIOTextBoxList.Count - mOutputDataCount,
                mDataIOTextBoxList.Count - 1, ref mDataIOTextBoxList);
            //设置转臂输出构件
            mExportElement.Items.Add("转臂输出");
            mExportElement.Items.Add("针齿壳输出");
            Grid.SetRow(mExportElement, 1);
            Grid.SetColumn(mExportElement, mPageGrid.ColumnDefinitions.Count - 1);
            mPageGrid.Children.Add(mExportElement);
            //数据绑定
            //设置数据存储字典
            mDataStorageDict.SetDataStorageDictionary(1, ref mDataHeadTable);
            for (int i = 0; i < mDataIOTextBoxList.Count; i++)
            {
                mDataIOTextBoxList[i].SetBinding(
                    TextBox.TextProperty,
                    new Binding("[" + mDataHeadTable[i + 2][0].Text + "].Value")
                    {
                        Source = mDataStorageDict,
                        UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
                    });
            }
            mDataStorageDict["减速器输出力矩"].Value = 750;
            mDataStorageDict["减速器输出转速"].Value = 15;
            mDataStorageDict["太阳轮齿数"].Value = 10;
            mDataStorageDict["行星轮齿数"].Value = 38;
            mDataStorageDict["摆线轮齿数"].Value = 39;
            mDataStorageDict["针轮齿数"].Value = 40;
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
            mDataStorageDict["太阳轮力矩"].Value = TempIOputData[7];
            mDataStorageDict["太阳轮转速"].Value = TempIOputData[8];
            mDataStorageDict["摆线轮力矩"].Value = TempIOputData[9];
            mDataStorageDict["摆线轮转速"].Value = TempIOputData[10];

        }
    }
}
