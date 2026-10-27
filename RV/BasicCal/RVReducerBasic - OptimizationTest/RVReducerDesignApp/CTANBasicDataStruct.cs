using iTextSharp.text.pdf;
using iTextSharp.text;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media.Animation;
using Paragraph = iTextSharp.text.Paragraph;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Win32;

namespace RVReducerDesignApp
{
    public class CTANConstantValue
    {
        public const double Degree = 0.017453292519943295769236907684886127134428718885417;
    }
    //输出表格
    public class CTANDataTable : DataTable
    {
        private Dictionary<string, int> pmRowStringToRowIndexDict = new Dictionary<string, int>();
        private int pmDescribeTextColCount = 0;//用于描述的文本列数量
        public DataGrid mDataGrid = new DataGrid();
        public CTANDataTable()
        {

            mDataGrid.SetBinding(DataGrid.ItemsSourceProperty, new Binding() { Source = this });
            mDataGrid.IsReadOnly = true;
            mDataGrid.RowHeaderWidth = 0;
            mDataGrid.AutoGenerateColumns = false;
            mDataGrid.CanUserAddRows = false;
            mDataGrid.CanUserDeleteRows = false;
            mDataGrid.SelectionUnit = DataGridSelectionUnit.Cell;
            mDataGrid.GridLinesVisibility = DataGridGridLinesVisibility.Horizontal;

        }
        public void AddSingleColumnHead(string InColumnHead, System.Type InDataType)
        {
            this.Columns.Add(InColumnHead, InDataType);
            DataGridTextColumn TmpColumn = new DataGridTextColumn() { Header = InColumnHead };
            mDataGrid.Columns.Add(TmpColumn);
            TmpColumn.Binding = new Binding(InColumnHead);
            TmpColumn.Width = new DataGridLength(1, DataGridLengthUnitType.Star);
        }
        public void SetColumnHeadListByStringList(string[] InHeadList, Type[] InDataTypeList)
        {
            this.Columns.Clear();
            for (int i = 0; i < InHeadList.Length; i++)
            {
                AddSingleColumnHead(InHeadList[i], InDataTypeList[i]);
            }
        }
        public void SetGearStandardOutputTable(string[] InHeadList, string[] InDescribeTextList, int InEachRowDescribeTextCount)
        {

            pmDescribeTextColCount = InEachRowDescribeTextCount;
            Type[] TmpTypeList = new Type[InHeadList.Count()];
            for (int i = 0; i < InEachRowDescribeTextCount; i++)
            {
                TmpTypeList[i] = typeof(string);
            }
            for (int i = InEachRowDescribeTextCount; i < InHeadList.Count(); i++)
            {
                TmpTypeList[i] = typeof(double);
            }
            SetColumnHeadListByStringList(InHeadList, TmpTypeList);
            pmRowStringToRowIndexDict.Clear();
            DataRow TmpRow;
            for (int i = 0; i < InDescribeTextList.Count() / InEachRowDescribeTextCount; i++)
            {
                pmRowStringToRowIndexDict.Add(InDescribeTextList[i * InEachRowDescribeTextCount], i);
                TmpRow = this.NewRow();
                for (int j = 0; j < InEachRowDescribeTextCount; j++)
                {
                    TmpRow[InHeadList[j]] = InDescribeTextList[j + i * InEachRowDescribeTextCount];
                }
                this.Rows.Add(TmpRow);
            }

        }
        public double this[string InRowindex, int InColumnIndex]
        {
            get
            {
                int TmpRowIndex = pmRowStringToRowIndexDict[InRowindex];
                return (double)this.Rows[TmpRowIndex][InColumnIndex];
            }
            set
            {
                int TmpRowIndex = pmRowStringToRowIndexDict[InRowindex];
                this.Rows[TmpRowIndex][InColumnIndex] = value;
            }
        }
        public double this[int InRowindex, int InColumnIndex]
        {
            get
            {
                return (double)this.Rows[InRowindex][InColumnIndex];
            }
            set
            {
                this.Rows[InRowindex][InColumnIndex] = value;
            }
        }
    }

    //转换器
    public class CTANConverter
    {
        public static ReverseSignConverter sReverseSignConverter = new ReverseSignConverter();
        public static String2ComboIntValueConverter sString2ComboIntValueConverter = new String2ComboIntValueConverter();
        public static Double2IntConverter sDouble2ComboIntValueConverter = new Double2IntConverter();
        public static Double2StringConverter sDouble2StringConverter = new Double2StringConverter();
        public static String2DoubleConverter sString2DoubleConverter = new String2DoubleConverter();
        public static CycloidTeethNumberConverter sCycloidTeethNumberConverter = new CycloidTeethNumberConverter();
        public class ReverseSignConverter : IValueConverter
        {
            public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            {
                if (value is double d)
                {
                    return -(double)d;
                }
                else if (value is int d1)
                {
                    return -(int)d1;
                }
                else
                {
                    return 0;
                }
            }

            public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            {
                if (value is double d)
                {
                    return -(double)d;
                }
                else if (value is int d1)
                {
                    return -(int)d1;
                }
                else
                {
                    return 0;
                }

            }
        }
        public class Double2IntConverter : IValueConverter
        {
            public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            {
                return (int)((double)value);
            }

            public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            {
                return (double)(int)value;

            }
        }
        public class String2ComboIntValueConverter : IValueConverter
        {
            public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            {
                return int.Parse(value as string);
            }

            public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            {
                return value.ToString();
            }
        }
        //double到string的转换器
        public class Double2StringConverter : IValueConverter
        {
            public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            {
                return sConvertDouble2String((double)value);
            }

            public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            {
                string str = value as string;
                if (sTryConvertString2Double(str, out double result))
                {
                    return result;
                }
                else// 不合法就不更新 Source（保留原值）
                {
                    return Binding.DoNothing;
                }
            }
        }
        public class String2DoubleConverter : IValueConverter
        {
            public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            {
                string str = value as string;
                if (sTryConvertString2Double(str, out double result))
                {
                    return result;
                }
                else// 不合法就不更新 Source（保留原值）
                {
                    return Binding.DoNothing;
                }
            }
            public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            {
                return sConvertDouble2String((double)value);
            }
        }
        //add
        public class CycloidTeethNumberConverter : IValueConverter
        {
            public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            {
                return (double)value + 1;
            }

            public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            {
                return (double)value - 1;
            }
        }
        private static string sConvertDouble2String(double InValue)
        {
            if (InValue < 1e8)
            {
                return InValue.ToString();
            }
            else
            {
                return InValue.ToString("E", CultureInfo.InvariantCulture);  // 科学计数法显示

            }
        }
        private static bool sTryConvertString2Double(string InString, out double value)
        {
            // 处理空输入
            if (string.IsNullOrWhiteSpace(InString))
            {
                value = 0;
                return true;
            }
            //// 处理只输入小数点的情况
            //if (InString == ".")
            //{
            //    value = 0;
            //    return true;
            //}

            // 处理科学计数法格式
            if (InString.EndsWith(".") || InString.EndsWith("e") || InString.EndsWith("E") ||
                InString.EndsWith("e+") || InString.EndsWith("e-") ||
                InString.EndsWith("E+") || InString.EndsWith("E-"))
            {
                value = 0;
                return false;
            }
            if (InString.Contains(".") && InString.EndsWith("0"))
            {
                value = 0;
                return false; // 返回 false 会导致 ConvertBack 返回 Binding.DoNothing，从而不更新源，保留界面文本
            }

            // 尝试用多种格式解析
            if (double.TryParse(InString, NumberStyles.Any, CultureInfo.InvariantCulture, out value))
            {
                return true;
            }
            else
            {
                // 处理带千位分隔符的情况
                InString = InString.Replace(",", string.Empty);
                return double.TryParse(InString, NumberStyles.Any, CultureInfo.InvariantCulture, out value);
            }
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
        // 依赖属性 - 基本绑定
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register(
                "Value",
                typeof(double),
                typeof(CTANDouble),
                new PropertyMetadata(0.0, OnValuePropertyChanged));
        //INotifyPropertyChanged 实现
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        //依赖属性更改运行函数
        private static void OnValuePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var tmpValue = (CTANDouble)d;
            tmpValue.OnPropertyChanged(nameof(Text));
            // 设置普通属性的值，这里可以加转换逻辑
            //control.Value = double.Parse(newValue);
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
            }
        }
        public double Value
        {
            get
            {
                return (double)GetValue(ValueProperty);
            }
            set
            {
                SetValue(ValueProperty, value);
            }
        }
        public double OutValue
        {
            get
            {
                return (double)GetValue(ValueProperty);
            }
            set
            {
                // 四舍五入到小数点后4位
                double roundedValue = Math.Round(value, 4, MidpointRounding.AwayFromZero);
                SetValue(ValueProperty, roundedValue);
            }
        }
        //public static DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(double), typeof(CTANDouble));
        #region 绑定到对象
        //将数据绑定到TANDouble
        //TANDouble为目标
        public void SetDataBingingToTANDouble(CTANDouble InBindingDouble)
        {
            BindingOperations.SetBinding(InBindingDouble, CTANDouble.ValueProperty, new Binding("Value") { Source = this, Mode = BindingMode.TwoWay });
        }
        //TANDouble为源
        public void SetDataBingingFromTANDouble(CTANDouble InBindingDouble)
        {
            BindingOperations.SetBinding(this, CTANDouble.ValueProperty, new Binding("Value") { Source = InBindingDouble, Mode = BindingMode.TwoWay });
        }
        public void SetDataBingingToTANDoubleOneWay(CTANDouble InBindingDouble)
        {
            BindingOperations.SetBinding(InBindingDouble, CTANDouble.ValueProperty, new Binding("Value") { Source = this, Mode = BindingMode.OneWay });
        }
        //TextBox为源
        //将数据绑定到文本框
        //TextBox为目标
        public void SetDataBingingToTextBox(TextBox InBindingBox)
        {
            BindingOperations.SetBinding(InBindingBox, TextBox.TextProperty,
                new Binding("Value") { Source = this, Mode = BindingMode.TwoWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged, Converter = CTANConverter.sDouble2StringConverter });
        }
        //TextBox为源
        public void SetDataBingingFromTextBox(TextBox InBindingBox)
        {
            BindingOperations.SetBinding(this, CTANDouble.ValueProperty,
                new Binding("Value") { Source = InBindingBox, Mode = BindingMode.TwoWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged, Converter = CTANConverter.sString2DoubleConverter });
        }
        //将字典数据库与TextBoxMatrix绑定,且有方向
        //InBingDirect=sBingToObj,控件为目标，sBingFromObj 控件为源
        //public void SetDataBindingWithTextBox(int InBingDirect, TextBox InBindingBox)
        //{
        //    if (InBingDirect == sBingToObj)
        //    {
        //        //TextBox为目标
        //        SetDataBingingToTextBox(InBindingBox);
        //    }
        //    else
        //    {
        //        //TextBox为源
        //        SetDataBingingFromTextBox(InBindingBox);
        //    }
        //}


        //将数据绑定到Combobox
        //Combobox为目标
        public void SetDataBingingToComboBox(ComboBox InBindingBox)
        {
            //BindingOperations.SetBinding(InBindingBox, ComboBox.SelectedValueProperty,
            //    new Binding("Text") { Source = this, Mode = BindingMode.TwoWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged, Converter = new CTANConverter.ComboFromValueStringConverter() });
            BindingOperations.SetBinding(InBindingBox, ComboBox.SelectedValueProperty,
                new Binding("Value") { Source = this, Mode = BindingMode.TwoWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged, Converter = CTANConverter.sDouble2ComboIntValueConverter });
        }
        //将数据绑定到Combobox
        //Combobox为源
        public void SetDataBingingFromComboBox(ComboBox InBindingBox)
        {
            BindingOperations.SetBinding(this, CTANDouble.ValueProperty,
                new Binding("SelectedValue") { Source = InBindingBox, Mode = BindingMode.TwoWay });

        }


        //将数据绑定到CTANObject,控件为目标
        public void SetDataBingingToTanObject(CTANObject InBindingObj)
        {
            if (InBindingObj.mObjectNotation == CTANObject.sTextBoxNotation)
            {
                SetDataBingingToTextBox(InBindingObj.mObject as TextBox);
            }
            else if (InBindingObj.mObjectNotation == CTANObject.sComboBoxNotation)
            {
                SetDataBingingToComboBox(InBindingObj.mObject as ComboBox);
            }
        }
        //将数据绑定到CTANObject,控件为源
        public void SetDataBingingFromTanObject(CTANObject InBindingObj)
        {
            if (InBindingObj.mObjectNotation == CTANObject.sTextBoxNotation)
            {
                SetDataBingingFromTextBox(InBindingObj.mObject as TextBox);
            }
            else if (InBindingObj.mObjectNotation == CTANObject.sComboBoxNotation)
            {
                SetDataBingingFromComboBox(InBindingObj.mObject as ComboBox);
            }
        }
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
    //public class CTANDict<T0,T1>: Dictionary<T0, T1>
    public class CTANDict : Dictionary<string, CTANDouble>
    {
        public static int sBingToControl = 0;//数据源为词典
        public static int sBingFromControl = 1;//数据源为控件
        //赋值
        public void AssignValue(string InKey, double InValue)
        {
            if (!this.ContainsKey(InKey))
            {
                this.Add(InKey, new CTANDouble() { Value = InValue });
            }
            else
            {
                // 键已存在，可以选择更新值或者执行其他操作
                this[InKey].Value = InValue; // 更新已存在的键的值
            }
        }
        public void SetDataStorageDictionary(int InStartRowIndex, List<List<TextBox>> InBoxHeadTable)
        {
            for (int i = InStartRowIndex; i < InBoxHeadTable.Count; i++)
            {
                this.AssignValue(InBoxHeadTable[i][0].Text, 0.0);
            }
        }
        public void SetDataStorageDictionary(int InStartRowIndex, int IncreStep, string[] InHeadList)
        {
            SetDataStorageDictionary(InStartRowIndex, IncreStep, InHeadList.Length / IncreStep, InHeadList);
        }
        public void SetDataStorageDictionary(int InStartRowIndex, int IncreStep, int InRowEndCount, string[] InHeadList)
        {
            for (int i = InStartRowIndex; i < InRowEndCount; i++)
            {
                this.AssignValue(InHeadList[InStartRowIndex + IncreStep * i], 0.0);
            }
        }
        //新，将字典数据库与TextBoxMatrix绑定
        public void SetDataBindingWithTextBoxMatrix(
            int InTextBoxMatrixStartRowIndex,
            int InTextBoxMatrixEndRowIndex,
            int InTextBoxBindindHeadColumnIndex,
            int InTextBoxBindindValueColumnIndex,
            List<List<TextBox>> InTextBoxTable)
        {
            string TmpString;
            for (int i = InTextBoxMatrixStartRowIndex; i < InTextBoxMatrixEndRowIndex + 1; i++)
            {
                //TextBox为目标
                InTextBoxTable[i][InTextBoxBindindValueColumnIndex].SetBinding(
                    TextBox.TextProperty,
                    new Binding("[" + InTextBoxTable[i][InTextBoxBindindHeadColumnIndex].Text + "].Value")
                    {
                        Source = this,
                        UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
                    }
                    );
            }
        }
        //将字典数据库与TextBoxMatrix绑定,且有方向
        //InTextBoxMatrixStartRowIndex=要进行绑定的文本矩阵行起始
        //InTextBoxMatrixEndRowIndex=要进行绑定的文本矩阵行结束
        //InTextBoxBindindHeadColumnIndex=要进行绑定的文本矩阵关键字Key所在列
        //InTextBoxBindindValueColumnIndex=要进行绑定的文本矩阵值Value所在列
        public void SetDataBindingWithTextBoxTable(
            int InBingDirect,
            int InTextBoxMatrixStartRowIndex,
            int InTextBoxMatrixEndRowIndex,
            int InTextBoxBindindHeadColumnIndex,
            int InTextBoxBindindValueColumnIndex,
            TextBox[][] InTextBoxTable)
        {
            string TmpKey;
            if (InBingDirect == sBingToControl)
            {

                for (int i = InTextBoxMatrixStartRowIndex; i < InTextBoxMatrixEndRowIndex + 1; i++)
                {
                    TmpKey = InTextBoxTable[i][InTextBoxBindindValueColumnIndex].Text;
                    this[TmpKey].SetDataBingingToTextBox(InTextBoxTable[i][InTextBoxBindindValueColumnIndex]);
                    //TextBox为目标  
                }
            }
            else
            {
                for (int i = InTextBoxMatrixStartRowIndex; i < InTextBoxMatrixEndRowIndex + 1; i++)
                {
                    TmpKey = InTextBoxTable[i][InTextBoxBindindValueColumnIndex].Text;
                    //TextBox为源
                    this[TmpKey].SetDataBingingFromTextBox(InTextBoxTable[i][InTextBoxBindindValueColumnIndex]);
                }
            }

        }

        public void CopyDict(CTANDict InDict)
        {
            this.Clear();
            foreach (var item in InDict)
            {
                this.Add(item.Key, item.Value);
            }
        }
        //public void BindingToDict(CTANDict InDict)
        //{
        //    foreach (var item in this)
        //    {
        //        BindingOperations.SetBinding(item.Value,
        //            CTANDouble.ValueProperty,
        //            new Binding("["+ item.Key+ "].Value") { Source = InDict, Mode = BindingMode.TwoWay });



        //        //BindingOperations.SetBinding(InSourceDict[TmpBindingKey],CTANDouble.ValueProperty,
        //        //    new Binding("[" + InBindingColumnIndex.ToString() + "]") { Source = mOutputDataTable.Rows[i], Mode = BindingMode.OneWayToSource });
        //    }
        //}
    }
    public class AppSavedData
    {
        public Shaft ShaftData { get; set; }
        public Dictionary<string, Dictionary<string, double>> PageDictData { get; set; }
    }

    public static class AppDataStorage
    {
        public static readonly string AppDataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "RVReducerDesignApp", "Properties");
        // 根据你的项目环境调整路径
        //public static readonly string AppDataPath = Path.Combine("D:\\Workspace\\softdevelop\\RV\\BasicCal\\RVReducerBasic - OptimizationTest\\RVReducerDesignApp\\Properties");

        public static void SaveAllData(
            Shaft shaft,
            IEnumerable<CTANBasicContentPageObj> allPages,
            string fileName = "AllData.json")
        {
            string filePath;
            if (Path.IsPathRooted(fileName))
                filePath = fileName;
            else
                filePath = Path.Combine(AppDataPath, fileName);

            string ext = Path.GetExtension(filePath).ToLowerInvariant();
            if (ext == ".pdf")
                SaveAllDataAsPdfToPath(shaft, allPages, filePath);
            else
                SaveAllDataToPath(shaft, allPages, filePath);
        }

        // 汇总可持久化页面数据，并对未命名页面和空参数给出可定位的处理结果。
        private static Dictionary<string, Dictionary<string, double>> CollectPageData(
            IEnumerable<CTANBasicContentPageObj> allPages)
        {
            if (allPages == null)
                throw new ArgumentNullException(nameof(allPages), "待保存的页面集合不能为空。");

            var bigDict = new Dictionary<string, Dictionary<string, double>>();
            foreach (var page in allPages)
            {
                if (page == null)
                    continue;

                var dataStorage = page.mDataStorageDict;
                string pageName = page.GetDataKey();

                // 部分基础页面在初始化时只是无标题占位对象，不属于工程数据。
                if (string.IsNullOrWhiteSpace(pageName))
                {
                    if (dataStorage == null || dataStorage.Count == 0)
                        continue;

                    throw new InvalidOperationException("存在包含数据但没有名称的页面，无法安全保存工程文件。");
                }

                var pageDict = new Dictionary<string, double>();
                if (dataStorage != null)
                {
                    foreach (var item in dataStorage)
                    {
                        if (item.Value == null)
                            throw new InvalidOperationException($"页面“{pageName}”中的参数“{item.Key}”尚未初始化。");

                        pageDict[item.Key] = item.Value.Value;
                    }
                }

                bigDict[pageName] = pageDict;
            }

            return bigDict;
        }

        private static void SaveAllDataToPath(Shaft shaft, IEnumerable<CTANBasicContentPageObj> allPages, string filePath)
        {
            var bigDict = CollectPageData(allPages);

            var saveObj = new AppSavedData
            {
                ShaftData = shaft,
                PageDictData = bigDict,
            };

            string json = JsonConvert.SerializeObject(saveObj, Formatting.Indented);

            string dir = Path.GetDirectoryName(filePath);
            if (!string.IsNullOrEmpty(dir))
                Directory.CreateDirectory(dir);

            File.WriteAllText(filePath, json);
        }

        private static void SaveAllDataAsPdfToPath(Shaft shaft, IEnumerable<CTANBasicContentPageObj> allPages, string filePath)
        {
            // Ensure codepage provider is registered (required under .NET Core/.NET 5+ for some font encodings)
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            var bigDict = CollectPageData(allPages);

            var saveObj = new AppSavedData
            {
                ShaftData = shaft,
                PageDictData = bigDict,
            };

            string json = JsonConvert.SerializeObject(saveObj, Formatting.Indented);

            string dir = Path.GetDirectoryName(filePath);
            if (!string.IsNullOrEmpty(dir))
                Directory.CreateDirectory(dir);

            var logSb = new StringBuilder();
            logSb.AppendLine($"PDF export started at {DateTime.UtcNow:u}");
            logSb.AppendLine($"Target file: {filePath}");

            BaseFont chosenBaseFont = null;
            Exception fontException = null;

            try
            {
                chosenBaseFont = TryFindAndCreateBaseFont(logSb, out fontException);
            }
            catch (Exception ex)
            {
                fontException = ex;
                logSb.AppendLine("Error during font search: " + ex.ToString());
            }

            // If automatic search failed, optionally prompt user to pick a font
            if (chosenBaseFont == null)
            {
                Application.Current?.Dispatcher?.Invoke(() =>
                {
                    var ask = MessageBox.Show(
                        "未能在系统中自动找到可嵌入的中文字体。\n是否手动选择字体文件(.ttf/.otf/.ttc)？",
                        "选择字体",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Question);

                    if (ask == MessageBoxResult.Yes)
                    {
                        var ofd = new OpenFileDialog();
                        ofd.Title = "选择中文字体文件 (.ttf/.otf/.ttc)";
                        ofd.Filter = "Font files (*.ttf;*.otf;*.ttc)|*.ttf;*.otf;*.ttc|All files (*.*)|*.*";
                        if (ofd.ShowDialog() == true)
                        {
                            string userFont = ofd.FileName;
                            logSb.AppendLine($"User selected font: {userFont}");
                            try
                            {
                                if (string.Equals(Path.GetExtension(userFont), ".ttc", StringComparison.OrdinalIgnoreCase))
                                {
                                    for (int idx = 0; idx < 10; idx++)
                                    {
                                        try
                                        {
                                            var pathWithIdx = userFont + "," + idx;
                                            chosenBaseFont = BaseFont.CreateFont(pathWithIdx, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
                                            fontException = null;
                                            logSb.AppendLine($"Succeeded with user ttc index {idx}: {pathWithIdx}");
                                            break;
                                        }
                                        catch (Exception exIdx)
                                        {
                                            fontException = exIdx;
                                            logSb.AppendLine($"User ttc index {idx} failed: {exIdx.Message}");
                                            continue;
                                        }
                                    }
                                }
                                else
                                {
                                    chosenBaseFont = BaseFont.CreateFont(userFont, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
                                    fontException = null;
                                    logSb.AppendLine($"Succeeded with user font: {userFont}");
                                }
                            }
                            catch (Exception eUser)
                            {
                                fontException = eUser;
                                logSb.AppendLine($"User font embedding failed: {eUser}");
                            }
                        }
                    }
                });
            }

            // Create PDF and write content (guarantee at least one page)
            using (var fs = new FileStream(filePath, FileMode.Create, FileAccess.Write))
            {
                using (var document = new Document(PageSize.A4, 36, 36, 36, 36))
                {
                    PdfWriter.GetInstance(document, fs);
                    document.Open();

                    iTextSharp.text.Font pdfFont = null;
                    bool embedded = false;

                    if (chosenBaseFont != null)
                    {
                        pdfFont = new iTextSharp.text.Font(chosenBaseFont, 10, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
                        embedded = true;
                        logSb.AppendLine("Using embedded base font.");
                    }
                    else
                    {
                        // Fall back to CJK or Helvetica (no Chinese)
                        try
                        {
                            chosenBaseFont = BaseFont.CreateFont("STSongStd-Light", "UniGB-UCS2-H", BaseFont.NOT_EMBEDDED);
                            pdfFont = new iTextSharp.text.Font(chosenBaseFont, 10, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
                            embedded = false;
                            logSb.AppendLine("Fell back to STSongStd-Light (not embedded).");
                        }
                        catch (Exception exCjk)
                        {
                            fontException = fontException ?? exCjk;
                            chosenBaseFont = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
                            pdfFont = new iTextSharp.text.Font(chosenBaseFont, 10, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
                            embedded = false;
                            logSb.AppendLine("Fell back to Helvetica (no Chinese support). Exception: " + exCjk.Message);
                        }
                    }

                    // Title
                    var titleFont = new iTextSharp.text.Font(pdfFont.BaseFont, 12, iTextSharp.text.Font.BOLD, BaseColor.BLACK);
                    document.Add(new Paragraph("Shaft Data Export", titleFont) { SpacingAfter = 8f });

                    // Warning if Chinese font embedding failed
                    if (!embedded)
                    {
                        string warn = "警告：未能嵌入中文字体，导出的 PDF 可能无法正确显示中文。";
                        string warnEng = "Warning: Failed to embed a Chinese font; Chinese characters may not display correctly in the exported PDF.";
                        var warnFont = new iTextSharp.text.Font(pdfFont.BaseFont, 9, iTextSharp.text.Font.ITALIC, BaseColor.RED);
                        document.Add(new Paragraph(warn, warnFont));
                        document.Add(new Paragraph(warnEng, warnFont) { SpacingAfter = 8f });

                        if (fontException != null)
                        {
                            var exPara = new Paragraph("Font error: " + fontException.Message, warnFont) { SpacingAfter = 8f };
                            document.Add(exPara);
                            logSb.AppendLine("Font exception: " + fontException.ToString());
                        }
                    }

                    // JSON body (auto-wrapped/paginated)
                    if (!string.IsNullOrEmpty(json))
                    {
                        var paragraph = new Paragraph(json, pdfFont)
                        {
                            Alignment = Element.ALIGN_LEFT,
                            Leading = 12f
                        };
                        document.Add(paragraph);
                    }
                    else
                    {
                        document.Add(new Paragraph("（空数据）", pdfFont));
                    }

                    document.Close();
                }
            }

            // write log
            try
            {
                string logPath = Path.ChangeExtension(filePath, ".log");
                File.WriteAllText(logPath, logSb.ToString(), Encoding.UTF8);
            }
            catch { }

            if (fontException != null && chosenBaseFont == null)
            {
                Application.Current?.Dispatcher?.Invoke(() =>
                {
                    MessageBox.Show("PDF 导出成功，但未能嵌入中文字体。请查看同目录的 .log 获取详情，或手动把中文 ttf/otf 放到项目目录或系统字体目录后再次导出。", "导出完成（字体未嵌入）", MessageBoxButton.OK, MessageBoxImage.Warning);
                });
            }
        }

        // Try to find system fonts and create embeddable BaseFont. Log attempts to logSb, return created BaseFont or null.
        private static BaseFont TryFindAndCreateBaseFont(StringBuilder logSb, out Exception lastException)
        {
            // Ensure encoding provider registered
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            lastException = null;

            var prioritized = new List<string>
        {
            Path.Combine(AppDataPath, "msyh.ttf"),
            Path.Combine(AppDataPath, "msyhbd.ttf"),
            Path.Combine(AppDataPath, "simhei.ttf"),
            Path.Combine(AppDataPath, "simsun.ttf"),
            Path.Combine(AppDataPath, "simsun.ttc")
        };

            string windowsFonts = Environment.GetFolderPath(Environment.SpecialFolder.Fonts);
            if (!string.IsNullOrEmpty(windowsFonts))
            {
                prioritized.Add(Path.Combine(windowsFonts, "msyh.ttf"));
                prioritized.Add(Path.Combine(windowsFonts, "msyhbd.ttf"));
                prioritized.Add(Path.Combine(windowsFonts, "simhei.ttf"));
                prioritized.Add(Path.Combine(windowsFonts, "simsun.ttf"));
                prioritized.Add(Path.Combine(windowsFonts, "simsun.ttc"));
            }

            foreach (var p in prioritized.Where(p => !string.IsNullOrEmpty(p)))
            {
                if (!File.Exists(p))
                {
                    logSb.AppendLine($"Candidate not exists: {p}");
                    continue;
                }

                try
                {
                    if (string.Equals(Path.GetExtension(p), ".ttc", StringComparison.OrdinalIgnoreCase))
                    {
                        for (int idx = 0; idx < 10; idx++)
                        {
                            try
                            {
                                var pWithIdx = p + "," + idx;
                                var bf = BaseFont.CreateFont(pWithIdx, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
                                logSb.AppendLine($"Succeeded with candidate ttc index {idx}: {pWithIdx}");
                                lastException = null;
                                return bf;
                            }
                            catch (Exception exIdx)
                            {
                                logSb.AppendLine($"Candidate ttc index {idx} failed: {exIdx.Message}");
                                lastException = exIdx;
                            }
                        }
                    }
                    else
                    {
                        var bf = BaseFont.CreateFont(p, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
                        logSb.AppendLine($"Succeeded with candidate font: {p}");
                        lastException = null;
                        return bf;
                    }
                }
                catch (Exception ex)
                {
                    logSb.AppendLine($"Candidate font {p} failed: {ex.Message}");
                    lastException = ex;
                }
            }

            // Enumerate system fonts if prior list fails
            if (!string.IsNullOrEmpty(windowsFonts) && Directory.Exists(windowsFonts))
            {
                string[] exts = new[] { "*.ttf", "*.otf", "*.ttc" };
                var allFiles = new List<string>();
                foreach (var e in exts)
                {
                    try { allFiles.AddRange(Directory.GetFiles(windowsFonts, e, SearchOption.TopDirectoryOnly)); }
                    catch (Exception ex) { logSb.AppendLine($"Enumerate fonts failed for {e}: {ex.Message}"); }
                }

                string[] keywords = new[] { "msyh", "microsoftyahei", "simsun", "simhei", "kai", "fang", "hei" };
                var ordered = allFiles.OrderByDescending(f => keywords.Any(k => Path.GetFileName(f).ToLower().Contains(k))).ThenBy(f => f).ToList();

                foreach (var file in ordered)
                {
                    try
                    {
                        if (string.Equals(Path.GetExtension(file), ".ttc", StringComparison.OrdinalIgnoreCase))
                        {
                            for (int idx = 0; idx < 10; idx++)
                            {
                                try
                                {
                                    var pWithIdx = file + "," + idx;
                                    var bf = BaseFont.CreateFont(pWithIdx, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
                                    logSb.AppendLine($"Succeeded with system ttc {file} index {idx}");
                                    lastException = null;
                                    return bf;
                                }
                                catch (Exception exIdx)
                                {
                                    lastException = exIdx;
                                    // continue
                                }
                            }
                        }
                        else
                        {
                            var bf = BaseFont.CreateFont(file, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
                            logSb.AppendLine($"Succeeded with system font: {file}");
                            lastException = null;
                            return bf;
                        }
                    }
                    catch (Exception ex)
                    {
                        lastException = ex;
                        // continue try next font
                    }
                }
            }

            return null;
        }

        public static AppSavedData LoadAllDataFromPath(string filePath)
        {
            if (!File.Exists(filePath))
                return null;

            string ext = Path.GetExtension(filePath).ToLowerInvariant();
            if (ext == ".pdf")
                throw new NotSupportedException("当前版本不支持从 PDF 恢复数据，请使用 JSON 文件。");

            string json = File.ReadAllText(filePath);
            return JsonConvert.DeserializeObject<AppSavedData>(json);
        }
    }
}
