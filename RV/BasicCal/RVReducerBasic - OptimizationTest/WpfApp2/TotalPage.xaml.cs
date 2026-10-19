using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
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
using static System.Net.Mime.MediaTypeNames;
public class Person
{
    public string Name { get; set; }
    public string Age { get; set; }
    public TextBox IOBox { get; set; }
}
public class DataTemple
{
    public string Name { get; set; }
    public string UnitName { get; set; }
    public double Value {  get; set; }
}
namespace RVReducerDesignApp
{
    /// <summary>
    /// TotalPage.xaml 的交互逻辑
    /// </summary>
    public partial class TotalPage : Page
    {
        List<DataTemple> TotalData = new List<DataTemple>();
        DataGrid TotalInputDataView = new DataGrid();
        DataTable TotalDataInputTable = new DataTable();
        DataGrid TotalOutputDataView = new DataGrid();
        DataTable TotalDataOutputTable = new DataTable();
        List<Person> TestData = new List<Person>();
        List<TextBox> mIOputBoxList = new List<TextBox>();
        //List<TextBlock> mOutputBoxList = new List<TextBlock>();
        ComboBox mExportElement = new ComboBox();
        Grid PageGrid = new Grid();
        
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
        int mInputDataCount = 6;
        int mOutputDataCount = 4;
        //Dictionary<string,double> TotalDataStorageDict=new Dictionary<string,double>();
        double[] TotalDataStorage=new double[11];
        //TotalDataInputTable.Rows.Add("减速器输出力矩", "牛米", 1.00);
        //TotalDataInputTable.Rows.Add("减速器输出转速", "转/分钟", 2.0);
        //TotalDataInputTable.Rows.Add("太阳轮齿数", "个", 0);
        //TotalDataInputTable.Rows.Add("行星轮齿数", "个", 0);
        //TotalDataInputTable.Rows.Add("摆线轮齿数", "个", 0);
        //TotalDataInputTable.Rows.Add("针轮齿数", "个", 0);
        //TotalDataOutputTable.Rows.Add("太阳轮力矩", "牛米", 0);
        //TotalDataOutputTable.Rows.Add("太阳轮转速", "转/分钟", 0);
        //TotalDataOutputTable.Rows.Add("摆线轮力矩", "牛米", 0);
        //TotalDataOutputTable.Rows.Add("摆线轮转速", "转/分钟", 0);
        double[] InterfaceInputDataList = new double[1000];
        double[] InterfaceOutputDataList= new double[1000];
        //DataTable TotalDataInputTable=new DataTable();
        public TotalPage()
        {
            InitializeComponent();


            //TotalDataInputTable.Columns.Add("名称", typeof(string));
            //TotalDataInputTable.Columns.Add("单位", typeof(string));
            //TotalDataInputTable.Columns.Add("数值", typeof(double));            
            //TotalDataInputTable.Rows.Add("减速器输出力矩", "牛米", 1.00);
            //TotalDataInputTable.Rows.Add("减速器输出转速", "转/分钟", 2.0);
            //TotalDataInputTable.Rows.Add("太阳轮齿数", "个", 0);
            //TotalDataInputTable.Rows.Add("行星轮齿数", "个", 0);
            //TotalDataInputTable.Rows.Add("摆线轮齿数", "个", 0);
            //TotalDataInputTable.Rows.Add("针轮齿数", "个", 0);
            //TotalDataInputTable.Columns[0].ReadOnly = true;
            //TotalDataInputTable.Columns[1].ReadOnly = true;
            //SetSingleTableBasicStyle(ref TotalInputDataView, ref TotalDataInputTable);

            //TotalDataOutputTable.Columns.Add("名称", typeof(string));
            //TotalDataOutputTable.Columns.Add("单位", typeof(string));
            //TotalDataOutputTable.Columns.Add("数值", typeof(double));
            //TotalDataOutputTable.Rows.Add("太阳轮力矩", "牛米", 0);
            //TotalDataOutputTable.Rows.Add("太阳轮转速", "转/分钟", 0);
            //TotalDataOutputTable.Rows.Add("摆线轮力矩", "牛米", 0);
            //TotalDataOutputTable.Rows.Add("摆线轮转速", "转/分钟", 0);
            //TotalDataOutputTable.Rows[2]["数值"] = 100;
            //SetSingleTableBasicStyle(ref TotalOutputDataView, ref TotalDataOutputTable);
            //TotalOutputDataView.IsReadOnly=true;





            //TotalInputDataView.RowHeaderWidth = 0;



            this.AddHandler(Window.SizeChangedEvent, new RoutedEventHandler(Window_SizeChanged));

#pragma region 布局表格
            int TempRowCount = mHeadName.Length/2;
            MainWindow.SetGridRowAndColumn(TempRowCount, 3,ref PageGrid);
            //int TempGridRowCount = 4;

            //List<RowDefinition> TempRowList = new List<RowDefinition>();

            //for (int i = 0; i < TempGridRowCount; i++) TempRowList.Add(new RowDefinition());
            //for (int i = 0; i < TempGridRowCount; i++)
            //{
            //    PageGrid.RowDefinitions.Add(TempRowList[i]);
            //}
            //int TempGridColumnCount = 3;
            //List<ColumnDefinition> TempColumnList = new List<ColumnDefinition>();
            //for (int i = 0; i < TempGridColumnCount; i++) TempColumnList.Add(new RowDefinition());
            //for (int i = 0; i < TempGridColumnCount; i++)
            //{
            //    PageGrid.RowDefinitions.Add(TempColumnList[i]);
            //}
            //TempRowList[0].Height = new GridLength(14);
            //TempRowList[1].Height = GridLength.Auto;
            //TempRowList[2].Height = new GridLength(30);
            ////PageGrid.VerticalAlignment = VerticalAlignment.Center;
            //TempRowList[3].Height = GridLength.Auto;
            //            TextBlock GridInputHead = new TextBlock();
            //            GridInputHead.Text = "输入参数";
            //            TextBlock GridOutputHead = new TextBlock();
            //            GridOutputHead.Text = "计算结果";
            //            GridOutputHead.VerticalAlignment = VerticalAlignment.Bottom;
            //            Grid.SetRow(GridInputHead, 0);
            //            Grid.SetRow(TotalInputDataView, 1);
            //            Grid.SetRow(GridOutputHead, 2);
            //            Grid.SetRow(TotalOutputDataView, 3);
            //#pragma endregion
            //            PageGrid.Children.Add(GridInputHead);
            //            PageGrid.Children.Add(TotalInputDataView);
            //            PageGrid.Children.Add(GridOutputHead);
            //            PageGrid.Children.Add(TotalOutputDataView);

            //PageGrid.RowDefinitions[0].Height = new GridLength(14);
            //PageGrid.RowDefinitions[1].Height = GridLength.Auto;
            //PageGrid.RowDefinitions[2].Height = new GridLength(30);
            //PageGrid.RowDefinitions[3].Height = GridLength.Auto;
            for(int i=0; i< PageGrid.ColumnDefinitions.Count;i++) PageGrid.ColumnDefinitions[i].Width = new GridLength(1, GridUnitType.Star);
            //定义文本框
            List<TextBlock> PageGridTextName = new List<TextBlock>();
            List<TextBlock> PageGridTextUnit = new List<TextBlock>();
            for (int i = 0; i < PageGrid.RowDefinitions.Count; i++)
            { 
                PageGridTextName.Add(new TextBlock());
                PageGridTextUnit.Add(new TextBlock());
            }
            //PageGridTextName[0].Text = "输入参数";
            //PageGridTextUnit[0].Text = "单位";
            for (int i = 0; i < PageGrid.RowDefinitions.Count; i++)
            {
                PageGridTextName[i].Text = mHeadName[2 *i];
                PageGridTextUnit[i].Text= mHeadName[2 * i+1];

            }
            //PageGridTextName[1].Text = "减速器输出力矩";
            //PageGridTextName[2].Text = "计算结果";
            //PageGridTextName[3].Text = "摆线轮齿数";
            for (int i = 0; i < PageGrid.RowDefinitions.Count; i++)
            {
                PageGrid.RowDefinitions[i].Height = new GridLength(20);
                Grid.SetRow(PageGridTextName[i], i);
                Grid.SetRow(PageGridTextUnit[i], i);
                Grid.SetColumn(PageGridTextUnit[i], 1);
                PageGrid.Children.Add(PageGridTextName[i]);
                PageGrid.Children.Add(PageGridTextUnit[i]);
            }
            //Grid.SetColumnSpan(PageGridTextName[0], 3);
            //List<TextBox> InputBoxList = new List<TextBox>();
            //定义输入输出
            for (int i = 0; i < TempRowCount-2; i++) mIOputBoxList.Add(new TextBox());
            for (int i = 0; i < TempRowCount - 2; i++)
            {
                Grid.SetRow(mIOputBoxList[i], i + 2);
                Grid.SetColumn(mIOputBoxList[i], 2);
                PageGrid.Children.Add(mIOputBoxList[i]); 
            }
            
            //List<TextBlock> OutputBoxList = new List<TextBlock>();
            //for (int i = 0; i < mOutputDataCount; i++) mOutputBoxList.Add(new TextBlock());
            for (int i = 0; i < mOutputDataCount; i++)
            {
                //Grid.SetRow(mOutputBoxList[i], i + 2+ mInputDataCount);
                //Grid.SetColumn(mOutputBoxList[i], 2);
                //PageGrid.Children.Add(mOutputBoxList[i]);
                //mIOputBoxList[TempRowCount-i-1].BorderThickness = new Thickness(0);
                mIOputBoxList[mIOputBoxList.Count - i - 1].IsReadOnly = true;
                mIOputBoxList[mIOputBoxList.Count - i - 1].BorderThickness = new Thickness(0);
            }
            mExportElement.Items.Add("转臂输出");
            mExportElement.Items.Add("针齿壳输出");
            Grid.SetRow(mExportElement, 1);
            Grid.SetColumn(mExportElement, 2);
            PageGrid.Children.Add(mExportElement);


            ////定义数据
            //for (int i = 1; i < mHeadName.Length/2; i++)
            //{
            //    TotalDataStorageDict.Add(mHeadName[2 * i], 0);
            //}
            //绑定数据
            //string TempBindingProperty;
            //for (int i = 0; i < mIOputBoxList.Count; i++)
            //{
            //    //TempBindingProperty = "[" + mHeadName[2 * i] + "]";
            //    mIOputBoxList[i].SetBinding(TextBox.TextProperty,
            //        new Binding("[" +(i+1).ToString() + "]") { Source = TotalDataStorage, Mode=BindingMode.TwoWay });

            //}


            //TotalDataStorageDict["减速器输出力矩"] = 0.1;

            //List<string> TempKey = new List<string>(TotalDataStorageDict.Keys);
            //TotalDataStorageDict["减速器输出力矩"] = 0.2;
            //TotalDataStorageDict["减速器输出力矩"] = 0.2;
            //for (int i = 2; i < mHeadName.Length / 2; i++)
            //{
            //    mIOputBoxList[i].SetBinding(TextBox.TextProperty, new Binding(TotalDataStorageDict) { Source = TotalDataStorageDict });

            //}

            //定义下拉框

            //InterfaceInputDataList[0] = 1;
            //InterfaceInputDataList[1] = 2;
            //InterfaceOutputDataList[0] = 5;
            //Binding Tempbingding = new Binding("MyDataProperty");
            //mInputBoxList[0].SetBinding(TextBlock.TextProperty, InterfaceInputDataList[0]);
            //mInputBoxList[0].Text= InterfaceInputDataList[0].ToString();
            //PageGrid.Children.



            //Grid.SetColumnSpan(PageGridTextName[2], 3);
            //for (int i = 0; i < PageGrid.RowDefinitions.Count; i++) PageGrid.Children.Add(PageGridTextName[i]);




            //PageGridTextName[0].Da
            //PageGridTextName[0].Sor
            //TextBlock GridInputHead = new TextBlock();
            //GridInputHead.Text = "输入参数";


            //TextBlock GridOutputHead = new TextBlock();
            //GridOutputHead.Text = "计算结果";
            //GridOutputHead.VerticalAlignment = VerticalAlignment.Bottom;
            //Grid.SetRow(GridInputHead, 0);
            //Grid.SetColumnSpan(GridInputHead, 3);
            ////Grid.SetRow(TotalInputDataView, 1);
            //Grid.SetRow(GridOutputHead, 2);
            //Grid.SetColumnSpan(GridOutputHead, 3);
            //PageGrid.Children.Add(GridInputHead);
            ////PageGrid.Children.Add(TotalInputDataView);
            //PageGrid.Children.Add(GridOutputHead);
            ////PageGrid.Children.Add(TotalOutputDataView);
            ////Grid.SetRow(TotalOutputDataView, 3);
            //TotalDataStorageDict["减速器输出力矩"] = 0.2;
            mIOputBoxList[0].Text = "-784";
            mIOputBoxList[1].Text = "15";
            mIOputBoxList[2].Text = "10";
            mIOputBoxList[3].Text = "38";
            mIOputBoxList[4].Text = "39";
            mIOputBoxList[5].Text = "40";
            //for (int i = 0; i < mIOputBoxList.Count; i++)
            //{
            //    //TempBindingProperty = "[" + mHeadName[2 * i] + "]";
            //    mIOputBoxList[i].SetBinding(TextBox.TextProperty,
            //        new Binding(".") { Source = TotalDataStorage[i + 1] });

            //}
            this.Content = PageGrid;
            //this.TotalParaDataView.Columns[1].IsReadOnly = false;


            //TestListView.DisplayMemberPath = "Name";
            //TestListView.SelectedValuePath = "Age";
            //TestListView.ItemsSource = people;
            //TestListView.Row = 0;
            //TotalDataGrid.Columns.Add
            //TotalDataGrid.Items.Add()

            //double TempValue = MyMainGrid.Width;

        }
        private void Window_SizeChanged(object sender, RoutedEventArgs e)
        {
            Page TempPage = (Page)e.OriginalSource;
            PageGrid.Width = TempPage.Width;
            //CalPageData();
            //Page TempWindow = (Page)e.OriginalSource;
            ////double TableWidth = TempWindow.Width - 10;
            //TotalInputDataView.Width = TempWindow.Width;
            //PageGrid.Height = TempWindow.Height;
            //SetSingleTableWidth(ref TotalInputDataView,TempWindow.Width - 2);
            //SetSingleTableWidth(ref TotalOutputDataView, TempWindow.Width - 2);
            ////TotalOutputDataView.Columns[2].IsReadOnly = true;
            //CalPageData();
            ////Total
            //TotalInputDataView.Columns[0].IsReadOnly = true;
            //TotalInputDataView.Columns[1].IsReadOnly = true;
            //TotalOutputDataView.IsReadOnly = true;
            ////TotalParaDataView.Columns[2].IsReadOnly = true;
            ////TotalParaDataView.Columns[2].
            //TotalInputDataView.SelectionUnit = DataGridSelectionUnit.Cell;
            ////DataGridRow rowContainer = (DataGridRow)TotalParaDataView.ItemContainerGenerator.ContainerFromIndex(2);
            ////DataGridCellsPresenter presenter = GetVisualChild<DataGridCellsPresenter>(rowContainer);
            ////DataGridCell cell = (DataGridCell)presenter.ItemContainerGenerator.ContainerFromIndex(columnIndex)
            ////DataGridCell cell = TotalParaDataView.Items[0];
            ////new DataGridCellInfo(TotalParaDataView.Items[0], TotalParaDataView.Columns[2]);
            ////TotalParaDataView[1,1].IsReadOnly=true;
            ////TotalParaDataView.ColumnWidth = TotalParaDataView.Width / 3;
            ////this.Content = TotalParaDataView;
            ////MyMainGrid.Width = newWidth;
            ////MyMainGrid.Height = newHeight;


            //// 在这里添加你的缩放逻辑
            //// 例如：更新UI元素的位置或大小，或者其他需要的操作
        }

        private void SetSingleTableWidth(ref DataGrid InDataGridView,double InTableTotalWidth)
        {
            //InDataGridView.RowHeaderWidth = 0;
            //InDataGridView.Columns[0].Width = InTableTotalWidth / 3;
            //InDataGridView.Columns[1].Width = InTableTotalWidth / 3;
            //InDataGridView.Columns[2].Width = InTableTotalWidth / 3;
         
            //TotalOutputDataView.RowHeaderWidth = 0;
            //TotalOutputDataView.Columns[0].Width = InTableTotalWidth / 3;
            //TotalOutputDataView.Columns[1].Width = InTableTotalWidth / 3;
            //TotalOutputDataView.Columns[2].Width = InTableTotalWidth / 3;
        }
        private void SetSingleTableBasicStyle(ref DataGrid InDataGridView,ref DataTable InDataTable)
        {
            InDataGridView.SelectionUnit = DataGridSelectionUnit.Cell;
            InDataGridView.CanUserAddRows = false;
            InDataGridView.ItemsSource = InDataTable.DefaultView;
        }
        public void CalPageData()
        {
            double[] TempIOputData = new double[11];
            //double[] TempOutputData = new double[4];
            if (mExportElement.SelectedIndex == 0)
            {
                TempIOputData[0] = 0;
            }
            else 
            {
                TempIOputData[0] = 1;
            }

            TempIOputData[1] = Convert.ToDouble(mIOputBoxList[0].Text);
            TempIOputData[2] = Convert.ToDouble(mIOputBoxList[1].Text);
            TempIOputData[3] = Convert.ToDouble(mIOputBoxList[2].Text);
            TempIOputData[4] = Convert.ToDouble(mIOputBoxList[3].Text);
            TempIOputData[5] = Convert.ToDouble(mIOputBoxList[4].Text);
            TempIOputData[6] = Convert.ToDouble(mIOputBoxList[5].Text);
            //TempInputData[1] = Convert.ToDouble(mInputBoxList[1].Text);

            //InterfaceInputDataList[1] = 2;
            //InterfaceOutputDataList[0] = 5;
            MainWindow.CalTotalTorqueAndRPM(ref TempIOputData[0], ref TempIOputData[7]);
            for (int i = 0; i < mIOputBoxList.Count; i++)
            {
                mIOputBoxList[i].Text= TempIOputData[i+1].ToString();
            }

            //MainWindow.CalAdd(ref InterfaceInputDataList[0], ref InterfaceOutputDataList[0]);
            //mOutputBoxList[0].Text= InterfaceOutputDataList[0].ToString();
            //InterfaceInputDataList[0] = (double)TotalDataInputTable.Rows[0]["数值"];
            //InterfaceInputDataList[1] = (double)TotalDataInputTable.Rows[1]["数值"];
            //MainWindow.CalTotalTorqueAndRPM(ref InterfaceInputDataList[0], ref InterfaceOutputDataList[0]);
            //TotalDataOutputTable.Rows[0]["数值"] = InterfaceDataList[2];
        }
    }
}
