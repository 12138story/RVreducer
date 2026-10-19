using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media.Animation;

namespace RVReducerDesignApp
{
    //public class DoubleConverter : IValueConverter
    //{
    //    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    //    {
    //        if (value == null)
    //            return "";

    //        double doubleValue = (double)value;
    //        return doubleValue.ToString(culture);
    //    }

    //    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    //    {
    //        string stringValue = value as string;
    //        //double doubleValue= stringValue.To;
    //        return double.Parse(stringValue);

    //    }
    //}
    public class CTANDouble : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private double _value;
        public double Value
        {
            get { return _value; }
            set
            {
                _value = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged.Invoke(this, new PropertyChangedEventArgs("Value"));
                }
            }
        }
        //public string Text
        //{
        //    get { return _value.ToString(); }
        //    set
        //    {
        //        if (Double.TryParse(value, out _value))
        //        {
        //            // 转换成功，result变量现在包含转换后的double值
        //            // 可以在这里使用result变量
        //        }
        //        else
        //        {
        //            // 转换失败，inputString可能不是有效的double格式
        //            // 在这里处理错误情况
        //            _value = 0;
        //        }

        //    }
    }
    //public class CTANDict<T0,T1>: Dictionary<T0, T1>
    public class CTANDict: Dictionary<string, CTANDouble>
    {
        //赋值
        public void AssignValue(ref string InKey,ref CTANDouble InValue)
        {
            if (!this.ContainsKey(InKey))
            {
                this.Add(InKey, InValue);
            }
            else
            {
                // 键已存在，可以选择更新值或者执行其他操作
                this[InKey] = InValue; // 更新已存在的键的值
            }
        }
        //设置数据存储字典
        public void SetDataStorageDictionary(int InStartRowIndex, ref List<List<TextBox>> InBoxHeadTable, ref Dictionary<string, CTANDouble> OutDictionary)
        {
            for (int i = InStartRowIndex; i < InBoxHeadTable.Count; i++)
            {
                OutDictionary.Add(InBoxHeadTable[i][0].Text, new CTANDouble() { Value = 0 });
            }
        }
        public void SetDataStorageDictionary(int InStartRowIndex, ref List<List<TextBox>> InBoxHeadTable)
        {
            for (int i = InStartRowIndex; i < InBoxHeadTable.Count; i++)
            {
                this.Add(InBoxHeadTable[i][0].Text, new CTANDouble() { Value = 0 });
            }
        }
    }
}
