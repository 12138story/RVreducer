using System;
using System.ComponentModel;
using System.Globalization;
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

namespace BingdingTest
{
    public class ComboFromValueDoubleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return double.Parse(value as string);

        }
    }
    public class TestConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;

        }
    }
    public class ComboFromValueStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (double.TryParse(value as string, out double result))
            {
                // 转换成功，使用 result
                return result;
            }
            else
            {
                return 0;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value.ToString();
        }
    }
    public class DoubleStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double tmpValue = (double)value;
            if (tmpValue<1e8)
            {
                return value?.ToString();
            }
            else
            {
                return tmpValue.ToString("E", CultureInfo.InvariantCulture);  // 科学计数法显示
                
            }

            //return value?.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string str = value as string;
            if (TryParseDouble(str, out double result))
                return result;

            // 不合法就不更新 Source（保留原值）
            return Binding.DoNothing;
        }
        public static bool TryParseDouble(string input, out double value)
        {
            // 处理空输入
            if (string.IsNullOrWhiteSpace(input))
            {
                value = 0;
                return true;
            }
            // 处理只输入小数点的情况
            if (input == ".")
            {
                value = 0;
                return true;
            }

            // 处理科学计数法格式
            if (input.EndsWith(".") || input.EndsWith("e") || input.EndsWith("E") ||
                input.EndsWith("e+") || input.EndsWith("e-") ||
                input.EndsWith("E+") || input.EndsWith("E-"))
            {
                value = 0;
                return false;
            }
            // 尝试用多种格式解析
            if (double.TryParse(input, NumberStyles.Any, CultureInfo.InvariantCulture, out value))
            {
                return true;
            }
            else
            {
                // 处理带千位分隔符的情况
                input = input.Replace(",", string.Empty);
                return double.TryParse(input, NumberStyles.Any, CultureInfo.InvariantCulture, out value);
            }
            
        }
    }
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 

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
                    if(result!= Value)
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
        public class TestStringClass : DependencyObject, INotifyPropertyChanged
    {
        // 依赖属性 - 基本绑定
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register(
                "Text",
                typeof(string),
                typeof(TestStringClass),
                new PropertyMetadata(string.Empty, OnTextPropertyChanged));
        public TestStringClass()
        {
            //Text = "22";
        }
        public string Text
        {
            get => (string)GetValue(TextProperty);
            set
            { 
                SetValue(TextProperty, value);
                //OnPropertyChanged(nameof(Value));
                //OnPropertyChanged(nameof(Value));
                //OnPropertyChanged(nameof(TextProperty));
            }
        }
        public double Value
        {
            get
            {
                if (double.TryParse(GetValue(TextProperty) as string, out double result))
                {
                    // 转换成功，使用 result
                   return result;
                }
                else
                {
                    return 0;
                }
                //return Convert.ToDouble(GetValue(TextProperty));
            }
            set
            {
                SetValue(TextProperty, value.ToString());
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


        private static void OnTextPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (TestStringClass)d;
            //string newValue = (string)e.NewValue;
            control.OnPropertyChanged(nameof(Value));

            // 设置普通属性的值，这里可以加转换逻辑
            //control.Value = double.Parse(newValue);
        }








        // 依赖属性 - 验证绑定
        public static readonly DependencyProperty ValidatedValueProperty =
            DependencyProperty.Register(
                "ValidatedValue",
                typeof(string),
                typeof(MainWindow),
                new PropertyMetadata(string.Empty));

        public string ValidatedValue
        {
            get => (string)GetValue(ValidatedValueProperty);
            set => SetValue(ValidatedValueProperty, value);
        }

        // 重置按钮点击事件
        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            Text = "初始值 3.14";
            ValidatedValue = "5.67";
        }

        // 输入验证方法 - 只允许数字和小数点
        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            var textBox = sender as TextBox;
            string newText = textBox.Text + e.Text;

            // 允许数字和小数点
            foreach (char c in e.Text)
            {
                if (!char.IsDigit(c) && c != '.')
                {
                    e.Handled = true;
                    return;
                }
            }

            // 防止多个小数点
            if (e.Text == ".")
            {
                // 检查文本框中是否已包含小数点
                if (textBox.Text.Contains("."))
                {
                    e.Handled = true;
                    return;
                }

                // 防止小数点作为第一个字符
                if (textBox.Text.Length == 0)
                {
                    e.Handled = true;
                    return;
                }
            }
        }


    }
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
            TestDoubleClass tmpClass1 =new TestDoubleClass();
            TestDoubleClass tmpClass2 = new TestDoubleClass();
            //BindingOperations.SetBinding(TestBox, TextBox.TextProperty, new Binding("TextValue") { Source = tmpClass1, Mode = BindingMode.TwoWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });
            //TestStringClass tmpClass2 = new TestStringClass();
            //BindingOperations.SetBinding(tmpClass2, TestStringClass.TextValueProperty, new Binding("TextValue") { Source = tmpClass1, Mode = BindingMode.TwoWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });
            //BindingOperations.SetBinding(DisplayBox, TextBox.TextProperty, new Binding("TextValue") { Source = tmpClass2, Mode = BindingMode.TwoWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });



            BindingOperations.SetBinding(TestBox, TextBox.TextProperty, new Binding("Value") { Source = tmpClass1, Mode = BindingMode.TwoWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,Converter=new DoubleStringConverter() });

            BindingOperations.SetBinding(tmpClass2, TestDoubleClass.ValueProperty, new Binding("Value") { Source = tmpClass1, Mode = BindingMode.TwoWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged, Converter = new TestConverter() });


            //BindingOperations.SetBinding(TestBox, TextBox.TextProperty, new Binding("Value") { Source = tmpClass2, Mode = BindingMode.TwoWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged, Converter = new ComboFromValueDoubleConverter() });
            //BindingOperations.SetBinding(DisplayBox, TextBox.TextProperty, new Binding("Text") { Source = tmpClass2, Mode = BindingMode.TwoWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged, Converter = new ComboFromValueDoubleConverter() });
            BindingOperations.SetBinding(DisplayBox, TextBox.TextProperty, new Binding("Value") { Source = tmpClass2, Mode = BindingMode.TwoWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged, Converter = new DoubleStringConverter() });
            //BindingOperations.SetBinding(tmpClass1, TestStringClass.TextProperty, new Binding("Text") { Source = TestBox, Mode = BindingMode.TwoWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });
            //TestStringClass tmpClass2 = new TestStringClass();
            //BindingOperations.SetBinding(tmpClass2, TestStringClass.TextProperty, new Binding("Text") { Source = tmpClass1, Mode = BindingMode.TwoWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });
            //BindingOperations.SetBinding(DisplayBox, TextBox.TextProperty, new Binding("Value") { Source = tmpClass2, Mode = BindingMode.TwoWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });
            TestBox.Text = "100";

            //BindingOperations.SetBinding(tmpClass, TestStringClass.TextValueProperty, new Binding("Text") { Source = TestBox, Mode = BindingMode.TwoWay });
            tmpClass1.Text = "2";
            TestBox.Text = "1";
            DisplayBox.Text = tmpClass2.Text;

            //ResetButton_Click(null, null);
        }


    }

}