using System.ComponentModel;
using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
//using WpfApp2.BasicPage;
namespace RVReducerDesignApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>

    public partial class MainWindow : Window
    {
        //总体参数页面
        //List<string> TempString =new List<string>(){"111",{"222"},{"333"} };
        //List<double> TempString = new List<double>() { 11,22,33};
        //double[] TempString = new double [] { 11, 22, 33 };
        //double TempString = 44444;
        //double TempString2 = 0;
        double mHeadHeight = 26;
        double mContentRightWidth = 4;
        double mContentRightRatio = 0;        
        double mContentLeftWidth = 1;
        double mContentLeftRatio = 0;

        TreeView RVTreeView=new TreeView();
        TreeViewItem RVTree = new TreeViewItem() { Header = "减速器" };
        TreeViewItem InvGearTree = new TreeViewItem() { Header = "渐开线齿轮副" };
        TreeViewItem CycloidGearTree = new TreeViewItem() { Header = "摆线针轮轮副" };
        //double mIndexGridWidthCoef = 1;
        //double mContentGridWidthCoef = 4;
        //double mHeadGridHightCoef = 1;
        //double mContentGridHightCoef = 4;
        //double mIndexGridWidthRatio = 0.0;
        //double mContentGridWidthRatio = 0.0;
        //double mHeadGridHightRatio = 0.0;
        //double mContentGridHightRatio = 0.0;
        //double mWindowWidth=0.0;
        //double mWindowHeight=0.0;
        TotalPage mTotalPage = new TotalPage();
        ReducerPage mReducerPage = new ReducerPage();

        [DllImport(@"RVReducerBasic.dll", EntryPoint = "CalTotalTorqueAndRPM", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        //[DllImport(@"InterpSurfTANDLL.dll", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        //[DllImport(@"libiomp5md.dll", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern void CalTotalTorqueAndRPM(ref double pInPara, ref double pOutPara);
        [DllImport(@"ZTestAddDll.dll", EntryPoint = "CalAdd", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern void CalAdd(ref double pInPara, ref double pOutPara);
        Button myButton = new Button();
        public MainWindow()
        {
            
            InitializeComponent();
            TestTab1.Content = mReducerPage;
            mContentRightRatio = mContentRightWidth/(mContentRightWidth + mContentLeftWidth);
            mContentLeftRatio = mContentLeftWidth / (mContentRightWidth + mContentLeftWidth);
            
            InvGearTree.Items.Add("太阳轮");
            InvGearTree.Items.Add("行星轮");
            RVTree.Items.Add(InvGearTree);
            //RVTree.IsSelected = true;
            RVTreeView.Items.Add(RVTree);
            //CycloidGearTree.Items.Add("太阳轮");


            Grid.SetRow(RVTreeView, 0);
            MainWindowIndexGrid.Children.Add(RVTreeView);
            this.AddHandler(Window.SizeChangedEvent, new RoutedEventHandler(Window_SizeChanged));
            //this.AddHandler(TreeView.PreviewMouseDownEvent, new RoutedEventHandler(TreeViewItem_Selected));
            this.AddHandler(TreeViewItem.SelectedEvent, new RoutedEventHandler(TreeViewItem_Selected));
            //myFrame.Content = TestPage;
            //myFrame.Width = myFrame.Width;
            //Binding bingding = new Binding();
            //bingding.Source = TempString;
            //bingding.Path = new PropertyPath(TempString.Length);
            //OpenButton.SetBinding(Button.ContentProperty, new Binding("[1]") { Source = TempString });
            //TempString2.
            //BindingOperations.SetBinding(TempString2,ValueProperty, new Binding(".") { Source = TempString });
            //OpenButton.SetBinding(Button.ContentProperty, new Binding(".") { Source = TempString });
            //TempString = "111";
            //OpenButton.Content = "111";
            //RVTree.Select

            //this.AddHandler(Button.ClickEvent, new RoutedEventHandler(MainWindowButton_Click));
            //double TempValue = MyMainGrid.Width;
            //mIndexGridWidthRatio = mIndexGridWidthCoef / (mIndexGridWidthCoef + mContentGridWidthCoef);
            //mContentGridWidthRatio = mContentGridWidthCoef / (mIndexGridWidthCoef + mContentGridWidthCoef);
            //mHeadGridHightRatio = mHeadGridHightCoef / (mHeadGridHightCoef + mContentGridHightCoef);
            //mContentGridHightRatio = mContentGridHightCoef / (mHeadGridHightCoef + mContentGridHightCoef);


            //TreeViewItem item1= new TreeViewItem() { Header="北京"};
            //TreeViewItem item11 = new TreeViewItem() { Header = "故宫" };
            //item11.Items.Add("南大门");
            //item11.Items.Add("神武门");
            //item1.Items.Add(item11);
            //// 设置ListBox的属性
            //item1.Width = 200;
            //item1.Height = 100;
            //Grid.SetColumnSpan(item1, 2);
            //Grid.SetRow(item1, 0);


            //myButton.Width = 200;
            //myButton.Height = 50;
            ////Grid.SetRow(myButton, 0);
            ////TextBlock txt1 = new TextBlock();
            ////txt1.Text = "2005 Products Shipped";
            ////txt1.FontSize = 20;
            ////txt1.FontWeight = FontWeights.Bold;
            ////Grid.SetColumnSpan(txt1, 3);

            //Grid.SetRow(myButton, 1);
            //Grid.SetColumn(myButton, 1);
            //MyToolbar.Width = 200;
            //MyGrid.Children.Add(myButton);

            //MyGrid.AddHandler(myButton);
            //Grid MyGrid;
            //Content = item1;
            //this.Show();

            //// 创建一个ListBox
            //ListBox listBox = new ListBox();
            //// 添加列表项
            //listBox.Items.Add("项目1");
            //listBox.Items.Add("项目2");
            //listBox.Items.Add("项目3");

            //// 设置ListBox的属性
            //listBox.Width = 200;
            //listBox.Height = 100;
            //Content = listBox;
            ////l.HorizontalAlignment = HorizontalAlignment.Center;
            ////l.VerticalAlignment = VerticalAlignment.Center;
            ////l.Visibility = Visibility.Visible;

            ////Button myButton=new Button();
            ////myButton.Width = 200;
            ////myButton.Height = 50;
            ////myButton.Margin = new Thickness(20);
            ////myButton.Click += MyButton_Click;
            ////this.Content = myButton;
        }
        //private void MyButton_Click(object sender, RoutedEventArgs e)
        //{
        //    double[] TestData = new double[3] { 5, 2, 0 };
        //    double[] TestOutData = new double[3] { 1, 2, 0 };
        //    CalAdd(ref TestData[0],ref TestData[2]);


        //    //unsafe
        //    //{
        //    //    getMemory();
        //    //    IntPtr OutPutData = GCHandle.ToIntPtr(TestData[2]));
        //    //        //& (TestData[2]);

        //    //}
        //    //Button btn = (Button)e.OriginalSource;
        //    //btn.Content = TestData[2];
        //    //MessageBox.Show("按钮被点击了！");
        //}
        private void Window_SizeChanged(object sender, RoutedEventArgs e)
        {
            
            //Window TempWindow = (Window)e.OriginalSource;
            //mWindowWidth = TempWindow.Width;
            //mWindowHeight = TempWindow.Height;
            //MyMainGrid.Width = newWidth;
            //MyMainGrid.Height = newHeight;
            
            RefreshProperties();
            
            //mTotalPage.Width = mContentRightWidth * mWindowWidth;
            //mTotalPage.Height = mContentRightWidth * mWindowHeight;
            //myFrame.Width = mTotalPage.Width;
            //myFrame.Height = mTotalPage.Height;
            // 在这里添加你的缩放逻辑
            // 例如：更新UI元素的位置或大小，或者其他需要的操作
        }
        private void CalButton_Click(object sender, RoutedEventArgs e)
        {
            if (myFrame.HasContent)
            {
                TreeViewItem TempTree = RVTreeView.SelectedItem as TreeViewItem;
                if(TempTree.Header=="减速器")
                {
                    ReducerPage TempPage = myFrame.Content as ReducerPage;
                    TempPage.CalPageData();
                }
                
                //if (TempTree.Header)
                //    Page TempPage = myFrame.Content as Page;
                //TempPage.Cal
            }


            //TestTotalPage.Width= ContentGrid.RowDefinitions.Height. Width;
            //TestTotalPage.Height = ContentGrid.Height;
            //myFrame.Content = TestTotalPage;
            //Page_Change.Content = new Frame();
            //{
            //    Content = TestTotalPage;
            //}
            //Page_Change.Content = new Frame();
            //{
            //    Content = TestTotalPage;
            //}
        }
        private void TestButton_Click(object sender, RoutedEventArgs e)
        {

            //mTotalPage.Width = myFrame.Width;
            //myFrame.Content = mTotalPage;
            //myFrame.Content.Width =myFrame.Width
            e.Handled = true;
            //RefreshProperties();
            //Button TempButton = (Button)e.OriginalSource;
            //if ((string)TempButton.Content == "计算")
            //{
            //    mTotalPage.Width= myFrame.Width;
            //    myFrame.Content = mTotalPage;
            //    //myFrame.Content.Width =myFrame.Width
            //    e.Handled = true;
            //    RefreshProperties();
            //}
           


            

            //TestTotalPage.Width= ContentGrid.RowDefinitions.Height. Width;
            //TestTotalPage.Height = ContentGrid.Height;
            //myFrame.Content = TestTotalPage;
            //Page_Change.Content = new Frame();
            //{
            //    Content = TestTotalPage;
            //}
            //Page_Change.Content = new Frame();
            //{
            //    Content = TestTotalPage;
            //}



        }
        private void DisplayTotalPage()
        {
            //myFrame.Content.Width =myFrame.Width

            //TestPage.Width = 100;
            //TestPage.Height = 80;


            //mTotalPage.Width = myFrame.Width;
            //myFrame.Content = mTotalPage;
            myFrame.Content = mReducerPage;
            RefreshProperties();
            //TestPage.TestFun();



        }
        private void RefreshProperties()
        {
            myFrame.Width = mContentRightRatio * this.Width-20;
            myFrame.Height = this.Height - mHeadHeight;
            //mTotalPage.Width = myFrame.Width;
            //mTotalPage.Height = myFrame.Height;            
            MyToolbar.Width = this.Width - 20;
            if (myFrame.HasContent)
            {
                Page TempPage=myFrame.Content as Page;
                TempPage.Width = myFrame.Width;
                TempPage.Height = myFrame.Height;
            }

        }
        private void TreeViewItem_Selected(object sender, RoutedEventArgs e)
        {

            //DisplayTotalPage();
            TreeViewItem TempTreeViewItem = (TreeViewItem)e.OriginalSource;
            if (TempTreeViewItem != null)
            {
                if(TempTreeViewItem.Header== "减速器")
                {
                    DisplayTotalPage();
                }
            }
            //TreeView TempTreeViewItem = (TreeView)e.OriginalSource;
            //RVTree.Select
            e.Handled = true;
        }
        public static void SetGridRowAndColumn(int InRowCount,int InColumnCount,ref Grid IOGrid)
        {

            List<RowDefinition> TempRowList = new List<RowDefinition>(InRowCount);

            for (int i = 0; i < InRowCount; i++) TempRowList.Add(new RowDefinition());
            for (int i = 0; i < InRowCount; i++)
            {
                IOGrid.RowDefinitions.Add(TempRowList[i]);
            }
            List<ColumnDefinition> TempColumnList = new List<ColumnDefinition>(InColumnCount);
            for (int i = 0; i < InColumnCount; i++) TempColumnList.Add(new ColumnDefinition());
            for (int i = 0; i < InColumnCount; i++)
            {
                IOGrid.ColumnDefinitions.Add(TempColumnList[i]);
            }
        }
        //private void MainWindowButton_Click(object sender, RoutedEventArgs e)
        //{
        //    Button TempButton = (Button) e.OriginalSource;
        //    if ((string)TempButton.Content == "计算")
        //    {
        //        myFrame.Content = mTotalPage;
        //        e.Handled = true;
        //    }
        //}
    }
}