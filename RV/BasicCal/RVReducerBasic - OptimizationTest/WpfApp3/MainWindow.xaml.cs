using System.ComponentModel;
using System.Data;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfApp3
{
    //public class MyControl : DependencyObject
    //{
    //    public static readonly DependencyProperty TextProperty =
    //        DependencyProperty.Register(
    //            nameof(Text),
    //            typeof(string),
    //            typeof(MyControl));

    //    public string Text
    //    {
    //        get => (string)GetValue(TextProperty);
    //        set => SetValue(TextProperty, value);
    //    }
    //}
    //public class CTANDouble : DependencyObject
    ////public class CTANDouble : DependencyObject
    //{
    //    public static implicit operator CTANDouble(double InValue)
    //    {
    //        CTANDouble Tmp = new CTANDouble();
    //        Tmp.Value = InValue;
    //        return Tmp;
    //    }
    //    public static implicit operator double(CTANDouble InTANDouble)
    //    {
    //        return InTANDouble.Value;
    //    }
    //    //public double get()
    //    //{
    //    //    return (double)GetValue(ValueProperty);
    //    //}
    //    //public void set(double value)
    //    //{
    //    //    SetValue(ValueProperty, value);
    //    //}
    //    public double Value
    //    {
    //        get { return (double)GetValue(ValueProperty); }
    //        set
    //        {
    //            SetValue(ValueProperty, value);
    //        }
    //    }
    //    public static DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(double), typeof(CTANDouble));
    //    public static bool IsDataInRectangleRange(int InJudgeXValue, int InJudgeYValue, int[] InRange)
    //    {
    //        if (InJudgeXValue >= InRange[0] && InJudgeXValue <= InRange[1] && InJudgeYValue >= InRange[2] && InJudgeYValue <= InRange[3])
    //        {
    //            return true;
    //        }
    //        else
    //        {
    //            return false;
    //        }
    //    }
    // }
    [ContentProperty("Value")]
    public class TestDoubleClass : DependencyObject, INotifyPropertyChanged
    {
        // 依赖属性 - 基本绑定
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register(
                "Value",
                typeof(double),
                typeof(TestDoubleClass),
                new PropertyMetadata(0.0, OnValuePropertyChanged));
        public TestDoubleClass()
        {
            //Text = "22";
        }
        public string Text
        {
            get => ((double)GetValue(ValueProperty)).ToString();
            set
            {
                if (double.TryParse(value as string, out double result))
                {
                    // 转换成功，使用 result
                    if (result != Value)
                    {
                        SetValue(ValueProperty, result);
                    }
                }
                else
                {
                }

                //OnPropertyChanged(nameof(Value));
                //OnPropertyChanged(nameof(Value));
                //OnPropertyChanged(nameof(TextProperty));
            }
        }
        public double Value
        {
            get
            {
                return (double)GetValue(ValueProperty);
                //return Convert.ToDouble(GetValue(TextProperty));
            }
            set
            {
                SetValue(ValueProperty, value);
                //OnPropertyChanged(nameof(Value));
                //OnPropertyChanged(nameof(Value));
                //OnPropertyChanged(nameof(TextProperty));
            }
        }
        //INotifyPropertyChanged 实现
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        private static void OnValuePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (TestDoubleClass)d;
            //string newValue = (string)e.NewValue;
            control.OnPropertyChanged(nameof(Text));

            // 设置普通属性的值，这里可以加转换逻辑
            //control.Value = double.Parse(newValue);
        }
    }
    public class CTANDouble : DependencyObject, INotifyPropertyChanged
    //public class CTANDouble : DependencyObject
    {
        public static int sBingToObj = 0;//数据源为词典
        public static int sBingFromObj = 1;//数据源为控件
        public CTANDouble()
        {
        }
        public string String
        {
            get { return (string)GetValue(StringProperty); }
            set
            {
                SetValue(StringProperty, value);
                OnPropertyChanged();
            }
        }

        public double Value
        {
            get { return Convert.ToDouble(GetValue(StringProperty)); }
            set
            {
                SetValue(StringProperty, value.ToString());
                OnPropertyChanged();
            }
        }
        public static DependencyProperty StringProperty = DependencyProperty.Register("string", typeof(string), typeof(CTANDouble), new PropertyMetadata(string.Empty));
        //public static DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(double), typeof(CTANDouble));
        #region 绑定到对象
        //将数据绑定到TANDouble
        //TANDouble为目标
        public void SetDataBingingToTANDouble(CTANDouble InBindingDouble)
        {
            BindingOperations.SetBinding(InBindingDouble, CTANDouble.StringProperty, new Binding("Value") { Source = this, Mode = BindingMode.TwoWay });
        }
        //TANDouble为源
        public void SetDataBingingFromTANDouble(CTANDouble InBindingDouble)
        {
            BindingOperations.SetBinding(this, CTANDouble.StringProperty, new Binding("Value") { Source = InBindingDouble, Mode = BindingMode.TwoWay });
        }
        //TextBox为源
        //将数据绑定到文本框
        //TextBox为目标
        public void SetDataBingingToTextBox(TextBox InBindingBox)
        {
            BindingOperations.SetBinding(InBindingBox, TextBox.TextProperty,
                new Binding("String") { Source = this, Mode = BindingMode.TwoWay ,UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });
        }
        //TextBox为源
        public void SetDataBingingFromTextBox(TextBox InBindingBox)
        {
            BindingOperations.SetBinding(this, CTANDouble.StringProperty,
                new Binding("Text") { Source = InBindingBox, Mode = BindingMode.TwoWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });
        }
        //将字典数据库与TextBoxMatrix绑定,且有方向
        //InBingDirect=sBingToObj,控件为目标，sBingFromObj 控件为源
        public void SetDataBindingWithTextBox(int InBingDirect, TextBox InBindingBox)
        {
            if (InBingDirect == sBingToObj)
            {
                //TextBox为目标
                SetDataBingingToTextBox(InBindingBox);
            }
            else
            {
                //TextBox为源
                SetDataBingingFromTextBox(InBindingBox);
            }
        }
        //INotifyPropertyChanged 实现
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        ////将数据绑定到Combobox
        ////Combobox为目标
        //public void SetDataBingingToComboBox(ComboBox InBindingBox)
        //{
        //    InBindingBox.SetBinding(
        //        ComboBox.SelectedValueProperty,
        //        new Binding(".Value")
        //        {
        //            Source = this,
        //            UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
        //            Converter = new CTANConverter.ComboValueDoubleConverter()
        //        }
        //        );
        //}
        ////将数据绑定到Combobox
        ////Combobox为源
        //public void SetDataBingingFromComboBox(ComboBox InBindingBox)
        //{
        //    //BindingOperations.SetBinding(this, CTANDouble.ValueProperty,
        //    //    new Binding("SelectedValue") { Source = InBindingBox, Mode = BindingMode.TwoWay });

        //}


        ////将数据绑定到CTANObject,控件为目标
        //public void SetDataBingingToTanObject(CTANObject InBindingObj)
        //{
        //    if (InBindingObj.mObjectNotation == CTANObject.sTextBoxNotation)
        //    {
        //        SetDataBingingToTextBox(InBindingObj.mObject as TextBox);
        //    }
        //    else if (InBindingObj.mObjectNotation == CTANObject.sComboBoxNotation)
        //    {
        //        SetDataBingingToComboBox(InBindingObj.mObject as ComboBox);
        //    }
        //}
        ////将数据绑定到CTANObject,控件为源
        //public void SetDataBingingFromTanObject(CTANObject InBindingObj)
        //{
        //    if (InBindingObj.mObjectNotation == CTANObject.sTextBoxNotation)
        //    {
        //        SetDataBingingFromTextBox(InBindingObj.mObject as TextBox);
        //    }
        //    else if (InBindingObj.mObjectNotation == CTANObject.sComboBoxNotation)
        //    {
        //        SetDataBingingFromComboBox(InBindingObj.mObject as ComboBox);
        //    }
        //}
        #endregion 绑定到对象
        public static bool IsDataInRectangleRange(int InJudgeXValue, int InJudgeYValue, int[] InRange)
        {
            if (InJudgeXValue >= InRange[0] && InJudgeXValue <= InRange[1] && InJudgeYValue >= InRange[2] && InJudgeYValue <= InRange[3])
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
    //public class CTANDict<T
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        DataTable TestTable = new DataTable("ParentTable");
        DataGrid TestGrid= new DataGrid();
        CTANDouble TestDouble= new CTANDouble();
        private static BitmapImage pmTaperedRollerBearingImageSource = new BitmapImage(new Uri(@"\Resource\Figure\Bearing\TaperedRollerBearing.png", UriKind.RelativeOrAbsolute));
        private Image pmImg = new Image();
        ContentControl MyContentControl=new ContentControl();
        public double DoubleValue { get; set; }
        TextBox[] tmpTextList = new TextBox[5];
        public MainWindow()
        {
            InitializeComponent();
            
            //TestTable.Columns.Add("名称", typeof(string));
            //TestTable.Columns.Add("单位", typeof(string));
            //TestTable.Columns.Add("数值", typeof(double));
            //TestDouble = 1.0;
            //TestTable.Rows.Add("减速器输出力矩", "牛米", 1.00);
            //TextBox1.SetBinding(TextBox.TextProperty, new Binding(".Rows[0][\"名称\"]") { Source = TestTable,Mode=BindingMode.TwoWay });
            //TestTable.Rows[0]["名称"] = "111";
            //TestGrid.Children.Add(TestDouble);
            ////dataSet = new DataSet();
            ////dataSet.Tables.Add(TestTable);

            //TestGrid.AutoGenerateColumns = false;
            ////for (int i = 0; i < TestTable.Columns.Count;i++)
            ////{
            ////    TestTable.Columns[i].Unique = true;
            ////}
            ////TestGrid.SetBinding
            ////TestGrid.AutoGenerateColumns = false;

            ////TestGrid.ItemsSource = TestTable.DefaultView;
            ////DataGridTextColumn TestColumn = new DataGridTextColumn();
            //TestGrid.SetBinding(DataGrid.ItemsSourceProperty, new Binding() { Source = TestTable });
            //for (int i = 0; i < TestTable.Columns.Count; i++)
            //{
            //    TestGrid.Columns.Add(new DataGridTextColumn() { Header = TestTable.Columns[i].ColumnName });
            //    (TestGrid.Columns[i] as DataGridTextColumn).Binding = new Binding(TestTable.Columns[i].ColumnName);
            //}

            ////TestGrid.Columns.Add(new DataGridTextColumn());
            ////TestGrid.Columns.Add(new DataGridTextColumn());
            ////TestColumn.Binding;
            ////TestGrid.Columns.Add(TestColumn);
            ////TestGrid.SetBinding(DataGrid.ItemsSourceProperty, new Binding() { Source = TestTable });
            ////TestColumn.Binding = new Binding("单位");
            //TestGrid.Columns[0].Width = new DataGridLength(1, DataGridLengthUnitType.Star);
            ////TestGrid.AutoGenerateColumns = false;
            ////TextBox1.SetBinding(TextBox.TextProperty, new Binding("Text") { Source = TextBox2, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });
            ////BindingOperations.SetBinding(TestColumn, DataGridTextColumn.It)

            ////TestColumn.B(DataGridTextColumn.Proper)
            ////TestGrid.Columns[0].Bind


            ////TestGrid.ItemsSource = TestTable.DefaultView;
            //TestGrid.RowHeaderWidth = 0;
            //TestGrid.IsReadOnly = true;
            //TestGrid.ColumnWidth = new DataGridLength(1, DataGridLengthUnitType.Star);
            //Grid.SetRow(TestGrid, 1);
            //windowGrid.Children.Add(TestGrid);
            pmImg.Source = pmTaperedRollerBearingImageSource;
            //Grid.SetRow(pmImg, 1);
            //windowGrid.Children.Add(pmImg);
            Grid.SetRow(MyContentControl, 1);
            windowGrid.Children.Add(MyContentControl);
            MyContentControl.Content = pmImg;
            for (int i = 0; i < tmpTextList.Length; i++)
            {
                tmpTextList[i]= new TextBox();
                tmpTextList[i].Text=i.ToString();
            }
            //MyControl TstControl= new MyControl();
            //BindingOperations.SetBinding(this, MyControl.TextProperty,
            //    new Binding("Text") { Source = TextBox1, Mode = BindingMode.TwoWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });
            TestDouble.SetDataBingingToTextBox(TextBox1);
            
            TestDouble.String = "0";
            TextBox1.Text = "2.2222";
            //TestDouble.Value = 222;
            double bbb = TestDouble.Value;
            BindingOperations.SetBinding(TextBox2, TextBox.TextProperty,
                new Binding("Value") { Source = TestDouble, Mode = BindingMode.TwoWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });
            BindingOperations.SetBinding(TextBox3, TextBox.TextProperty,
                new Binding("String") { Source = TestDouble, Mode = BindingMode.TwoWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });
            TestDoubleClass TestDoubleXX = new TestDoubleClass();
            //TestDoubleXX = 5;

            //TestGrid.Columns[0].Width = DataGridLength.Auto;
            //TestGrid.Columns[1].Width = DataGridLength.Auto;
            //TestGrid.Columns[2].Width = DataGridLength.Auto;
            //TestGrid.AutoGenerateColumns = false;
            //TestGrid.AllowUserToAddRows = false;

            //TestGrid.Columns.Add();

            //TestGrid.AutoGenerateColumns = false;
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MyContentControl.Content = tmpTextList[1];
        }
        private void Button_Click2(object sender, RoutedEventArgs e)
        {
            MyContentControl.Content = tmpTextList[2];
        }
        //private void MakeDataRelation()
        //{
        //    // DataRelation requires two DataColumn
        //    // (parent and child) and a name.
        //    DataColumn parentColumn =
        //        dataSet.Tables["ParentTable"].Columns["名称"];
        //    DataColumn childColumn =
        //        dataSet.Tables["ChildTable"].Columns["ParentID"];
        //    DataRelation relation = new
        //        DataRelation("parent2Child", parentColumn, childColumn);
        //    dataSet.Tables["ChildTable"].ParentRelations.Add(relation);
        //}
    }
    public class MainViewModel : INotifyPropertyChanged
    {
        private object _currentContent;
        public object CurrentContent
        {
            get => _currentContent;
            set
            {
                if (_currentContent != value)
                {
                    _currentContent = value;
                    OnPropertyChanged(nameof(CurrentContent));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}