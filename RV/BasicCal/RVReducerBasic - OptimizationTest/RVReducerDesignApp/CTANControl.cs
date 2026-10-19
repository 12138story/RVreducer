using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Google.Protobuf.WellKnownTypes;
using Org.BouncyCastle.Crmf;
using Org.BouncyCastle.Utilities.IO;
//控件页，包括grid combobox等控件
namespace RVReducerDesignApp
{
    //public class CTANObject: Object
    //{
    //    public Button GetButton
    //    {
    //        get
    //        {
    //            object tmpObject = this;
    //            return tmpObject as Button;
    //        }
    //        //set
    //        //{
    //        //    ButtonObj
    //        //}
    //    }
    //    public Object SetObject
    //    {
    //        set
    //        {
    //            this = value;
    //        }
    //    }

    //}
    //返回类型
    public interface ITemplateInterface
    {
        public int ReturnTypeNotation();
    }
    public class CTANGrid : Grid, ITemplateInterface
    {
        static double sRowHeight = 20;
        //private Dictionary<string, Object> pmGridObject = new Dictionary<string, Object>();
        //添加按钮到XXX行列;
        //public void AddButtonToGrid(int InButtonRowIndex, int InButtonColumnIndex, string InButtonContent)
        //{
        //    Button TmpButton = new Button();
        //    Grid.SetRow(TmpButton, InButtonRowIndex);
        //    Grid.SetColumn(TmpButton, InButtonColumnIndex);
        //    TmpButton.Content = InButtonContent;
        //    Children.Add(TmpButton);
        //    pmGridObject[InButtonContent] = TmpButton;
        //}
        //设置Grid高度
        public static void SetGridRowHeight(double InRowHeight, Grid IOGrid)
        {
            for (int i = 0; i < IOGrid.RowDefinitions.Count; i++)
            {
                IOGrid.RowDefinitions[i].Height = new GridLength(InRowHeight);
            }
        }
        public void SetGridRowHeight(double InRowHeight)
        {
            SetGridRowHeight(InRowHeight, this);
        }
        public void SetGridDefaultRowHeight()
        {
            SetGridRowHeight(sRowHeight, this);
        }
        //将控件放入Grid网格
        public void AssignControlToGrid<T>(int InRowIndex, int InColumnIndex, T InControl) where T : UIElement
        {
            Grid.SetRow(InControl, InRowIndex);
            Grid.SetColumn(InControl, InColumnIndex);
            this.Children.Add(InControl);
        }
        public static void AssignControlToGrid<T>(int InRowIndex, int InColumnIndex, T InControl, Grid AssignedGrid) where T : UIElement
        {
            Grid.SetRow(InControl, InRowIndex);
            Grid.SetColumn(InControl, InColumnIndex);
            AssignedGrid.Children.Add(InControl);
        }
        ////设置数据字典的值
        //public Object this[string key]
        //{
        //    get
        //    {
        //        if (pmGridObject.TryGetValue(key, out Object value))
        //        {
        //            return value;
        //        }
        //        {
        //            throw new KeyNotFoundException($"键 '{key}' 不存在");
        //        }
        //    }
        //    set
        //    {
        //        pmGridObject[key] = value;
        //    }

        //}
        //public Button GetButton(string key)
        //{
        //    if (pmGridObject.TryGetValue(key, out Object value))
        //    {
        //        return value as Button;
        //    }
        //    else
        //    {
        //        throw new KeyNotFoundException($"键 '{key}' 不存在");
        //    }
        //}
        //设置Grid行和列数目
        public static void SetGridRowAndColumn(int InRowCount, int InColumnCount, CTANGrid IOGrid)
        {
            IOGrid.Clear();
            for (int i = 0; i < InRowCount; i++)
            {
                IOGrid.RowDefinitions.Add(new RowDefinition());
                IOGrid.RowDefinitions[i].Height = new GridLength(1, GridUnitType.Auto);
            }
            for (int i = 0; i < InColumnCount; i++)
            {
                IOGrid.ColumnDefinitions.Add(new ColumnDefinition());
            }
        }
        public void SetGridRowAndColumn(int InRowCount, int InColumnCount)
        {
            CTANGrid.SetGridRowAndColumn(InRowCount, InColumnCount, this);
        }
        public void CopyRowStyle(Grid InCopiedGrid)
        {
            for (int i = 0; i < InCopiedGrid.RowDefinitions.Count; i++)
            {
                RowDefinitions[i].Height = InCopiedGrid.RowDefinitions[i].Height;
            }
        }
        //清空
        public void Clear()
        { 
            RowDefinitions.Clear();
            ColumnDefinitions.Clear();
        }
        public int ReturnTypeNotation()
        {
            return CTANObject.sTANGControlGridNotation;
        }

    }
    //文本控件
    public class CTANTextBox : TextBox, ITemplateInterface
    {
        //public static int sDisplayBoxType = 0;
        //public static int sInputBoxType = 1;
        public CTANTextBox()
        {
            this.Background = Brushes.Transparent; //给输入文本框设置透明背景
        }
        //InCreatNotation 创建方法
        public CTANTextBox(bool InCreatNotation)
        {
            SetTextBoxIsReadOnly(InCreatNotation);
        }
        public CTANTextBox(bool InIsReadOnly, string InText)
        {
            SetTextBoxIsReadOnly(InIsReadOnly);
            SetTextInBox(InText);
            this.Background = Brushes.Transparent; //给显示文本框设置透明背景
        }
        public void SetTextInBox(string InText)
        {
            this.Text = InText;
        }
        private void SetTextBoxIsReadOnly(bool InIsReadOnly)
        {
            if (InIsReadOnly)
            {
                SetStyleAsTextBlock();
            }
            else
            {
                SetStyleAsInputText();
            }
        }
        ////设置文本框在Grid中的行列
        //public void SetBoxInGridRowColumn(int InRowIndex, int InColumnIndex)
        //{
        //    Grid.SetRow(this, InRowIndex);
        //    Grid.SetColumn(this, InColumnIndex);
        //}
        //设置文本框样式,将TextBox设为TextBlock样式
        private void SetStyleAsTextBlock()
        {
            this.IsReadOnly = true;
            this.BorderThickness = new Thickness(0);
        }
        private void SetStyleAsInputText()
        {
            this.IsReadOnly = false;
        }
        //复制
        public CTANTextBox Clone()
        {
            CTANTextBox ret = new CTANTextBox();
            ret.Text = this.Text;
            ret.IsReadOnly = this.IsReadOnly;
            ret.BorderThickness = this.BorderThickness;
            return ret;
        }
        //隐式转换为CTANObject
        public static implicit operator CTANObject(CTANTextBox myTextBox)
        {
            CTANObject tmpObj = new CTANObject(myTextBox);
            return tmpObj;
        }
        //返回类型
        public int ReturnTypeNotation()
        {
            return CTANObject.sTextBoxNotation; 
        }
    }
    //下拉控件
    public class CTANComboBox : ComboBox, ITemplateInterface
    {
        //public int mTypeNotation = CTANObject.sComboBoxNotation;
        public Dictionary<int, object> mIDToContentDict;
        public CTANComboBox()
        {         
        }
        public CTANComboBox(int[] InComboID, string[] InComboString)
        {
            SetComboBoxWithKeyValue(InComboID, InComboString);
        }
        //给Combobox设置下拉项和对应ID
        public static void SetComboBoxWithKeyValue(int[] InComboID, string[] InComboString, ComboBox IOComboBox)
        {
            Dictionary<string, int> TmpDict = new Dictionary<string, int>();
            for (int i = 0; i < InComboString.Count(); i++)
            {
                TmpDict.Add(InComboString[i], InComboID[i]);
            }
            IOComboBox.SelectedValuePath = "Value";
            IOComboBox.DisplayMemberPath = "Key";
            IOComboBox.ItemsSource = TmpDict;
        }
        public void SetComboBoxWithKeyValue(int[] InComboID, string[] InComboString)
        {
            SetComboBoxWithKeyValue(InComboID, InComboString, this);
        }
        public void SetBoxInGridRowColumn(int InRowIndex, int InColumnIndex)
        {
            Grid.SetRow(this, InRowIndex);
            Grid.SetColumn(this, InColumnIndex);
        }
        public CTANComboBox Clone()
        {
            CTANComboBox ret = new CTANComboBox();
            ret.ItemsSource = this.Items;
            ret.SelectedValuePath = this.SelectedValuePath;
            ret.DisplayMemberPath = this.DisplayMemberPath;
            return ret;
        }
        //隐式转换为CTANObject
        public static implicit operator CTANObject(CTANComboBox myComboBox)
        {
            CTANObject tmpObj = new CTANObject(myComboBox);
            tmpObj.mObjectNotation = CTANObject.sComboBoxNotation;
            return tmpObj;
        }
        //返回类型
        public int ReturnTypeNotation()
        {
            return CTANObject.sComboBoxNotation; 
        }
    }
    //按钮控件
    public class CTANButton : Button, ITemplateInterface
    {
        public delegate void TButtonClickEventDelegate(object? sender, RoutedEventArgs e);
        public RoutedEventHandler pTButtonClickEventHander;
        public CTANButton()
        {
            //this.Click += pTButtonClickEventHander;
        }
        public CTANButton(string InDisplayText)
        {
            this.Content = InDisplayText;
        }
        public void AddClickEvent(RoutedEventHandler InClickEvent)
        {
            Click += InClickEvent;
            pTButtonClickEventHander += InClickEvent;
        }
        public void CopyClickEvent(CTANButton InTButton)
        {
            this.pTButtonClickEventHander = InTButton.pTButtonClickEventHander;
            this.Click += this.pTButtonClickEventHander;

        }
        public CTANButton Clone()
        {
            CTANButton ret = new CTANButton();
            ret.Content = this.Content;
            //复制消息处理
            ret.CopyClickEvent(this);
            return ret;
        }
        public int ReturnTypeNotation()
        {
            return CTANObject.sButtonNotation;
        }
    }
    //一般控件容器
    public class CTANObject
    {
        public static int sTextBoxNotation = 0;
        public static int sComboBoxNotation = 1;
        public static int sButtonNotation = 2;
        public static int sTANGControlGridNotation = 10;
        //Object对象标识
        public int mObjectNotation = -1;//未定义对象
        public object mObject;
        public CTANObject()
        {
        }
        public CTANObject(TextBox InObj)
        {
            SetStorageObj(InObj);
        }
        public CTANObject(ComboBox InObj)
        {
            SetStorageObj(InObj);
        }
        public CTANObject(Button InObj)
        {
            SetStorageObj(InObj);
        }
        public void SetStorageObj(TextBox InObj)
        {
            mObjectNotation = sTextBoxNotation;
            mObject = InObj;
        }
        public void SetStorageObj(ComboBox InObj)
        {
            mObjectNotation = sComboBoxNotation;
            mObject = InObj;
        }
        public void SetStorageObj(Grid InObj)
        {
            mObjectNotation = sTANGControlGridNotation;
            mObject = InObj;
        }
        public void SetStorageObj(Button InObj)
        {
            mObjectNotation = sButtonNotation;
            mObject = InObj;
        }
        public void SetStorageObj<T>(T InObj) where T : ITemplateInterface
        {
            mObject = InObj;
            mObjectNotation = InObj.ReturnTypeNotation();
        }
        //克隆
        public CTANObject Clone()
        {
            CTANObject ret = new CTANObject();
            ret.mObjectNotation = this.mObjectNotation;
            if (this.mObjectNotation == sTextBoxNotation)
            {
                CTANTextBox tmpObj = (this.mObject as CTANTextBox).Clone();
                ret.mObject = tmpObj;
            }
            else if (this.mObjectNotation == sComboBoxNotation)
            {
                CTANComboBox tmpObj = (this.mObject as CTANComboBox).Clone();
                ret.mObject = tmpObj;
            }
                if (this.mObjectNotation == sButtonNotation)
            {
                CTANButton tmpObj = (this.mObject as CTANButton).Clone();
                ret.mObject = tmpObj;
            }
            return ret;
        }
        //设置
        //隐式转换为CTANTextBox
        public static implicit operator CTANTextBox(CTANObject myObj)
        {
            CTANTextBox tmpTextBox = (myObj.mObject) as CTANTextBox;
            return tmpTextBox;
        }
        //隐式转换为CTANCombobox
        public static implicit operator CTANComboBox(CTANObject myObj)
        {
            CTANComboBox tmpBox = (myObj.mObject) as CTANComboBox;
            return tmpBox;
        }
        //隐式转换为CTANButton
        public static implicit operator CTANButton(CTANObject myObj)
        {
            CTANButton tmpButton = (myObj.mObject) as CTANButton;
            return tmpButton;
        }
        //与Grid相关
        public void CloneToGrid(int InRowIndex,int InColumnIndex,CTANGridWithControl AssignGrid)
        {
            CTANObject ret = new CTANObject();
            ret.mObjectNotation = this.mObjectNotation;
            if (this.mObjectNotation == sTextBoxNotation)
            {
                CTANTextBox tmpObj = (this.mObject as CTANTextBox).Clone();
                ret.mObject = tmpObj;
                AssignGrid.AssignControlToGrid(InRowIndex, InColumnIndex, tmpObj);
            }
            else if (this.mObjectNotation == sComboBoxNotation)
            {
                CTANComboBox tmpObj = (this.mObject as CTANComboBox).Clone();
                ret.mObject = tmpObj;
                AssignGrid.AssignControlToGrid(InRowIndex, InColumnIndex, tmpObj);
            }
            if (this.mObjectNotation == sButtonNotation)
            {
                CTANButton tmpObj = (this.mObject as CTANButton).Clone();
                ret.mObject = tmpObj;
                AssignGrid.AssignControlToGrid(InRowIndex, InColumnIndex, tmpObj);
            }          
        }
    }
    public class CTANContentControl : ContentControl, INotifyPropertyChanged
    {
        public object Content
        {
            get => base.Content;
            set
            {
                base.Content = value;
                OnPropertyChanged(nameof(Content));
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }

    //public class CTANContentControl : INotifyPropertyChanged
    //{
    //    private object _currentContent;
    //    public object CurrentContent
    //    {
    //        get => _currentContent;
    //        set
    //        {
    //            if (_currentContent != value)
    //            {
    //                _currentContent = value;
    //                OnPropertyChanged(nameof(CurrentContent));
    //            }
    //        }
    //    }

    //    public event PropertyChangedEventHandler PropertyChanged;
    //    private void OnPropertyChanged(string propertyName) =>
    //        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    //}


    //带控件的Grid，设定了一个默认高度，20
    public class CTANGridWithControl : CTANGrid
    {

        //public CTANGrid mGrid = new CTANGrid();
        public CTANObject[][]? mControlTable;//控件存储表
        public CTANContentControl[][]? mContentPanelTable;//用于占位的控件列表
        public GroupBox mGroupBox = new GroupBox();
        public Dictionary<string, int> mRowNameToIndex = new Dictionary<string, int>();
        //int[][] mControlNotation;//控件标识符，0-标准展示文本，1-标准输入文本，2-combox
        public CTANGridWithControl()
        {

        }
        public CTANGridWithControl(int InRowCount, int InColumnCount)
        {
            SetGridRowAndColumn(InRowCount, InColumnCount);
        }
        //清空
        new public void Clear()
        {
            base.Clear();
            mContentPanelTable = null;
            mControlTable = null;
            mGroupBox.Content = null;
            mRowNameToIndex.Clear();
        }


        //设置行列布局
        new public void SetGridRowAndColumn(int InRowCount, int InColumnCount)
        {
            Clear();
            base.SetGridRowAndColumn(InRowCount, InColumnCount);
            mControlTable = new CTANObject[InRowCount][];
            //生成行背景容器
            for (int i = 0; i < InRowCount; i++)
            {
                if (i % 2 == 0)
                {
                    Border rowBackground = new Border
                    {
                        Background = new SolidColorBrush(Color.FromRgb(200, 200, 200)),
                    };

                    Grid.SetRow(rowBackground, i);
                    Grid.SetColumnSpan(rowBackground, InColumnCount);

                    // 确保永远在最底层
                    Panel.SetZIndex(rowBackground, -1000);

                    // 关键：在所有控件添加之前添加进去
                    this.Children.Add(rowBackground);
                }
                if (i % 2 != 0)
                {
                    Border rowBackground1 = new Border
                    {
                        //Background = new SolidColorBrush(Color.FromRgb(255, 255, 255)),
                        Background = Brushes.WhiteSmoke
                    };

                    Grid.SetRow(rowBackground1, i);
                    Grid.SetColumnSpan(rowBackground1, InColumnCount);

                    // 确保永远在最底层
                    Panel.SetZIndex(rowBackground1, -1000);

                    // 关键：在所有控件添加之前添加进去
                    this.Children.Add(rowBackground1);
                }
            }
            for (int i = 0; i < InRowCount; i++)
            {
                mControlTable[i] = new CTANObject[InColumnCount];
            }
            SetGridDefaultRowHeight();
        }
        public void SetGridRowAndColumnWithContentTable(int InRowCount, int InColumnCount)
        {
            SetGridRowAndColumn(InRowCount, InColumnCount);
            mContentPanelTable = new CTANContentControl[InRowCount][];
            for (int i = 0; i < InRowCount; i++)
            {
                mContentPanelTable[i] = new CTANContentControl[InColumnCount];
                for (int j = 0; j < InColumnCount; j++)
                {
                    mContentPanelTable[i][j] = new CTANContentControl();
                    base.AssignControlToGrid(i, j, mContentPanelTable[i][j]);
                }
            }

        }
        #region 设置Grid中的控件
        //控件放置
        //将单个控件放入Grid位置
        new public void AssignControlToGrid<T>(int InRowIndex, int InColumnIndex, T InControl) where T : UIElement, ITemplateInterface
        {
            if (mControlTable[InRowIndex][InColumnIndex] == null)
            {
                mControlTable[InRowIndex][InColumnIndex] = new CTANObject();
                mControlTable[InRowIndex][InColumnIndex].SetStorageObj(InControl);

                if (mContentPanelTable == null)
                {
                    base.AssignControlToGrid(InRowIndex, InColumnIndex, InControl);
                }
                else
                {
                    mContentPanelTable[InRowIndex][InColumnIndex].Content = InControl;
                }
            }
        }
        public void AssignControlToGrid<T>(string InRowName, int InColumnIndex, T InControl) where T : UIElement, ITemplateInterface
        {
            AssignControlToGrid(mRowNameToIndex[InRowName], InColumnIndex, InControl);
        }
        //给Grid设置创建控件,通用函数
        public void SetGridControlByCreateControl<T>(int InRowIndex, int InColumnIndex) where T : UIElement, ITemplateInterface, new()
        {
            T tmpControl = new T();
            AssignControlToGrid(InRowIndex, InColumnIndex, tmpControl);
        }
        public void SetGridControlByCreateControl<T>(int InRowIndex, int InColumnIndex, out T OutObj) where T : UIElement, ITemplateInterface, new()
        {
            OutObj = new T();
            AssignControlToGrid(InRowIndex, InColumnIndex, OutObj);
        }
        //给Grid创建显示文本，单个
        public void SetGridControlByCreateDisplayTextBox(int InRowIndex, int InColumnIndex, string InText)
        {
            CTANTextBox tmpTextBox = new CTANTextBox(true, InText);
            mRowNameToIndex[InText] = InRowIndex;
            AssignControlToGrid(InRowIndex, InColumnIndex, tmpTextBox);
        }

        //给Grid创建显示文本
        //连续地在多列创建显示文本
        public void SetGridControlByCreateDisplayTextBox(int InStartRowIndex, int InStartColumnIndex, int InColumnCount, string[] InTextList)
        {
            int tmpTextCount = InTextList.Length;//总文本数
            int tmpRowCount = tmpTextCount / InColumnCount;//循环总行数
            int k = 0;
            for (int i = 0; i < tmpRowCount; i++)
            {
                for (int j = 0; j < InColumnCount; j++)
                {
                    SetGridControlByCreateDisplayTextBox(InStartRowIndex + i, InStartColumnIndex + j, InTextList[k]);
                    k++;
                }
            }
        }
        //可间断地在多列创建显示文本
        public void SetGridControlByCreateDisplayTextBox(int InStartRowIndex, int[] InColumnList, string[] InTextList)
        {
            SetGridControlByCreateDisplayTextBox(InStartRowIndex, InColumnList, 0, InTextList);
        }
        //可间断地在多列创建显示文本,文本序列起始有偏移
        public void SetGridControlByCreateDisplayTextBox(int InStartRowIndex, int[] InColumnList, int InTextStartIndex, string[] InTextList)
        {
            int tmpTextCount = InTextList.Length - InTextStartIndex;//总文本数
            int tmpColumnCount = InColumnList.Length;
            int tmpRowCount = tmpTextCount / tmpColumnCount;//循环总行数
            int k = InTextStartIndex;
            for (int i = 0; i < tmpRowCount; i++)
            {
                for (int j = 0; j < tmpColumnCount; j++)
                {
                    SetGridControlByCreateDisplayTextBox(InStartRowIndex + i, InColumnList[j], InTextList[k]);
                    k++;
                }
            }
        }

        //给Grid设置创建输入文本
        //public void SetGridControlByCreateInputTextBox(int InRowIndex, int InColumnIndex)
        //{
        //    CTANTextBox tmpTextBox = new CTANTextBox(CTANTextBox.sInputBoxType);
        //    AssignControlToGrid(InRowIndex, InColumnIndex, (TextBox)tmpTextBox);
        //}
        //连续地在多列创建输入文本
        public void SetGridControlByCreateInputTextBox(int InStartRowIndex, int InRowCount, int InStartColumnIndex, int InColumnCount)
        {
            for (int i = 0; i < InRowCount; i++)
            {
                for (int j = 0; j < InColumnCount; j++)
                {
                    SetGridControlByCreateControl<CTANTextBox>(InStartRowIndex + i, InStartColumnIndex + j);
                }
            }
        }
        //间断地在多列创建输入文本
        public void SetGridControlByCreateInputTextBox(int InStartRowIndex, int InRowCount, int[] InColumnIndexList)
        {
            for (int i = 0; i < InRowCount; i++)
            {
                for (int j = 0; j < InColumnIndexList.Length; j++)
                {
                    SetGridControlByCreateControl<CTANTextBox>(InStartRowIndex + i, InColumnIndexList[j]);
                }
            }
        }
        //间断地在多列创建输入文本,假如已定义构件就跳过
        public void SetGridControlByCreateInputTextBoxWithJump(int InStartRowIndex, int InRowCount, int[] InColumnIndexList)
        {
            for (int i = 0; i < InRowCount; i++)
            {
                for (int j = 0; j < InColumnIndexList.Length; j++)
                {
                    if (mControlTable[InStartRowIndex + i][InColumnIndexList[j]] == null)
                    {
                        SetGridControlByCreateControl<CTANTextBox>(InStartRowIndex + i, InColumnIndexList[j]);
                    }
                }
            }
        }
        //给Grid创建Combobox
        public CTANComboBox SetGridControlByCreateComboBox(int InRowIndex, int InColumnIndex, int[] InComboBoxID, string[] InComboBoxDisplayString)
        {

            CTANComboBox tmpComboBox = new CTANComboBox(InComboBoxID, InComboBoxDisplayString);
            AssignControlToGrid(InRowIndex, InColumnIndex, tmpComboBox);
            return tmpComboBox;
        }
        public CTANComboBox SetGridControlByCreateComboBox(int InRowIndex, int InColumnIndex, string[] InComboBoxDisplayString)
        {
            int[] tmpComboBoxID = new int[InComboBoxDisplayString.Length];
            for (int i = 0; i < InComboBoxDisplayString.Length; i++)
            {
                tmpComboBoxID[i] = i;
            }
            return SetGridControlByCreateComboBox(InRowIndex, InColumnIndex, tmpComboBoxID, InComboBoxDisplayString);
        }
        public CTANComboBox SetGridControlByCreateComboBox(string InRowName, int InColumnIndex, int[] InComboBoxID, string[] InComboBoxDisplayString)
        {
            return SetGridControlByCreateComboBox(mRowNameToIndex[InRowName], InColumnIndex, InComboBoxID, InComboBoxDisplayString);
        }
        public CTANComboBox SetGridControlByCreateComboBox(string InRowName, int InColumnIndex, string[] InComboBoxDisplayString)
        {
            return SetGridControlByCreateComboBox(mRowNameToIndex[InRowName], InColumnIndex, InComboBoxDisplayString);
        }
        //给Grid设置创建按钮

        public void SetGridControlByCreateButton(int InRowIndex, int InColumnIndex, string InButtonText)
        {
            CTANButton tmpControl = new CTANButton(InButtonText);
            AssignControlToGrid<CTANButton>(InRowIndex, InColumnIndex, tmpControl);
        }
        #endregion 设置Grid中的控件

        #region 设置数据绑定
        //绑定到字典
        //字典为目标
        public void BingdingInputControlToDict(int InKeyRowIndex, int InInputControlColumnIndex, CTANDict InBindingDict)
        {
            CTANTextBox tmpBox = this.mControlTable[InKeyRowIndex][0];
            string tmpKey = tmpBox.Text;
            InBindingDict[tmpKey].SetDataBingingFromTanObject(mControlTable[InKeyRowIndex][InInputControlColumnIndex]);

        }
        //字典为源
        public void BingdingInputControlFromDict(int InKeyRowIndex, int InInputControlColumnIndex, CTANDict InBindingDict)
        {
            CTANTextBox tmpBox = this.mControlTable[InKeyRowIndex][0];
            string tmpKey = tmpBox.Text;
            InBindingDict[tmpKey].SetDataBingingToTanObject(mControlTable[InKeyRowIndex][InInputControlColumnIndex]);
        }
        public void BingdingGridFromDict(int InKeyRowStartIndex, int InKeyRowCount, int InInputControlColumnIndex, CTANDict InBindingDict)
        {
            for (int i = 0; i < InKeyRowCount; i++)
            {
                BingdingInputControlFromDict(InKeyRowStartIndex + i, InInputControlColumnIndex, InBindingDict);
            }
        }
        public void BingdingGridFromDict(string[] InKeyTextList, int InInputControlColumnIndex, CTANDict InBindingDict)
        {
            for (int i = 0; i < InKeyTextList.Length; i++)
            {
                BingdingInputControlFromDict(mRowNameToIndex[InKeyTextList[i]], InInputControlColumnIndex, InBindingDict);
            }
        }
        #endregion 设置数据绑定
        #region 设置排版样式
        //创建纯文本控件的显示和输入Grid
        //int InStringStartIndex 从字符串数组哪一个索引开始进行显示
        //string[] InStringList 需要显示的字符串数组
        public void CreatPureTextObjByInputTextStringList(int InStringDisplayStartRowIndex, int[] InStringDisplayColumnList, int[] InInputBoxColumnIndexList, int InStringStartIndex, string[] InStringList)
        {
            int tmpDisplayBoxInEachRowCount = InStringDisplayColumnList.Length;
            int tmpRowCount = (InStringList.Length - InStringStartIndex) / tmpDisplayBoxInEachRowCount;
            int tmpColumnCount = InInputBoxColumnIndexList.Length;
            //根据InStringList创建显示文本
            this.SetGridControlByCreateDisplayTextBox(InStringDisplayStartRowIndex, InStringDisplayColumnList, InStringStartIndex, InStringList);
            //根据InStringList创建输入文本
            this.SetGridControlByCreateInputTextBox(InStringDisplayStartRowIndex, tmpRowCount, InInputBoxColumnIndexList);
        }
        #endregion 设置排版样式
        #region 其他
        public void SetGroup(string InGroupText)
        {
            mGroupBox.Header = InGroupText;
            mGroupBox.Content = this as Grid;
        }
        public void SetGroupInGrid(int InRowIndex, int InColumnIndex, Grid InGrid)
        {
            mGroupBox.Content = this as Grid;
            Grid.SetRow(mGroupBox, InRowIndex);
            Grid.SetColumn(mGroupBox, InColumnIndex);
            InGrid.Children.Add(mGroupBox);
        }
        public void SetGroupInGrid(int InRowIndex, int InColumnIndex, string InText, Grid InGrid)
        {
            SetGroup(InText);
            SetGroupInGrid(InRowIndex, InColumnIndex, InGrid);
        }
        //从父容器清除构件
        public static void RemoveFromParent(UIElement element)
        {
            DependencyObject parent = VisualTreeHelper.GetParent(element);
            if (parent == null)
            {
                parent = LogicalTreeHelper.GetParent(element);
            }
            if (parent is Panel panel)
            {
                panel.Children.Remove(element);
            }
            else if (parent is ContentControl contentControl)
            {
                contentControl.Content = null;
            }
        }
        #endregion 其他
        #region 标准输入布局构型
        //设定显示项和表头,为了生成RowTextToIndex字典
        public static void SetStandartDisplayText(string[] InStringHead, string[] InStringTable, int InDisplayColumnCount, CTANGridWithControl OutControlGrid)
        {
            int tmpTotalColumnCount = InStringHead.Length;
            if (OutControlGrid.mControlTable == null)
            {
                OutControlGrid.SetGridRowAndColumn(InStringTable.Length / InDisplayColumnCount + 1, tmpTotalColumnCount);
            }
            //设定表头
            OutControlGrid.SetGridControlByCreateDisplayTextBox(0, 0, tmpTotalColumnCount, InStringHead);
            //设定输入项及对应显示项
            OutControlGrid.SetGridControlByCreateDisplayTextBox(1, [0, 1], 0, InStringTable);

        }
        public void SetStandartDisplayText(string[] InStringHead, string[] InStringTable, int InDisplayColumnCount)
        {
            SetStandartDisplayText(InStringHead, InStringTable, InDisplayColumnCount, this);
        }
        //设定输入文本框，绑定数据,根据字段确定是否并放入GroupBox
        public static void SetStandartInputTextAndBingdingToDict(int[] InputTextCreatRangeTable,//inputtext创建范围，0-行起始索引，1-行数，2-列起始索引，3-列数
            string[] InBindingKeyList, int InBindingKeyIncre,
            CTANDict InBindingDict, CTANGridWithControl OutControlGrid)
        {
            //创建输入框
            OutControlGrid.SetGridControlByCreateInputTextBox(InputTextCreatRangeTable[0], InputTextCreatRangeTable[1], InputTextCreatRangeTable[2], InputTextCreatRangeTable[3]);
            //绑定数据
            OutControlGrid.BingdingGridFromDict(InBindingKeyList, 2, InBindingDict);

        }
        public void SetStandartInputTextAndBingdingToDict(int[] InputTextCreatRangeTable,//inputtext创建范围，0-行起始索引，1-行数，2-列起始索引，3-列数
           string[] InBindingKeyList, int InBindingKeyIncre,
           CTANDict InBindingDict)
        {
            SetStandartInputTextAndBingdingToDict(InputTextCreatRangeTable, InBindingKeyList, InBindingKeyIncre, InBindingDict, this);
        }
        public static void SetStandartInputTextAndBingdingToDict(string InGroupText,
            int[] InputTextCreatRangeTable,//inputtext创建范围，0-行起始索引，1-行数，2-列起始索引，3-列数
            string[] InBindingKeyList, int InBindingKeyIncre,
            CTANDict InBindingDict,
            CTANGridWithControl OutControlGrid)
        {
            SetStandartInputTextAndBingdingToDict(InputTextCreatRangeTable, InBindingKeyList, InBindingKeyIncre, InBindingDict, OutControlGrid);
            OutControlGrid.SetGroup(InGroupText);
        }
        public void SetStandartInputTextAndBingdingToDict(string InGroupText,
            int[] InputTextCreatRangeTable,//inputtext创建范围，0-行起始索引，1-行数，2-列起始索引，3-列数
            string[] InBindingKeyList, int InBindingKeyIncre,
            CTANDict InBindingDict)
        {
            SetStandartInputTextAndBingdingToDict(InGroupText, InputTextCreatRangeTable, InBindingKeyList, InBindingKeyIncre, InBindingDict, this);
        }
        public static void SetStandartInputGroup(string InGroupHead, string[] InStringHead, string[] InStringTable, CTANDict InBindingDict, CTANGridWithControl OutControlGrid)
        {
            ////OutControlGrid = new CTANGridWithControl();
            ////OutControlGrid.mGroupBox.Content = null;
            //int tmpControlGridColumnCount = 3; //Grid控件列数，3列 有1列放输入框
            //int tmpControlGridRowCount = InStringTable.Length / 2 + 1;//Grid控件行数+1因为有1行要放表头
            ////如果没有设定行列数，就重新进行行列数设定,以避免覆盖原有内容
            //if(OutControlGrid.mControlTable == null)
            //{
            //    OutControlGrid.SetGridRowAndColumn(tmpControlGridRowCount, tmpControlGridColumnCount);
            //}

            ////设定表头
            //OutControlGrid.SetGridControlByCreateDisplayTextBox(0, 0, InStringHead.Length, InStringHead);
            ////设定输入项及对应显示项
            //OutControlGrid.CreatPureTextObjByInputTextStringList(1, [0, 1], [2], 0, InStringTable);
            int tmpTotalColumnCount = InStringHead.Length;
            int tmpDisplayColumnCount = 2;
            int tmpInputColumnCount = tmpTotalColumnCount - tmpDisplayColumnCount;
            int tmpInputRowCount = InStringTable.Length / tmpDisplayColumnCount;
            //设定显示文本
            SetStandartDisplayText(InStringHead, InStringTable, tmpDisplayColumnCount, OutControlGrid);
            //设定输入文本绑定并装进Group
            SetStandartInputTextAndBingdingToDict(InGroupHead, [1, tmpInputRowCount, tmpDisplayColumnCount, 1], InStringTable, tmpDisplayColumnCount, InBindingDict, OutControlGrid);
            //进行数据绑定
            //OutControlGrid.BingdingGridFromDict(1, InStringTable.Count() / 2, 2, InBindingDict);
            //封装进Group
            //OutControlGrid.SetGroup(InGroupHead);
        }
        public void SetStandartInputGroup(string InGroupHead, string[] InStringHead, string[] InStringTable, CTANDict InBindingDict)
        {
            SetStandartInputGroup(InGroupHead, InStringHead, InStringTable, InBindingDict, this);
        }
        //生成系列的OutControlGrid
        public static void SetStandartInputGroupList(string[] InGroupHead, string[] InStringHead, string[][] InStringTable, CTANDict InBindingDict, CTANGridWithControl[] OutControlGridList)
        {
            int tmpControlGridCount = InStringTable.GetLength(0);
            for (int i = 0; i < tmpControlGridCount; i++)
            {
                SetStandartInputGroup(InGroupHead[0], InStringHead, InStringTable[i], InBindingDict, OutControlGridList[i]);
            }
        }
        //生成系列的OutControlGrid并放入一个grid网格中
        public static void SetStandartInputPageGrid<T>(string[] InGroupHead, string[] InStringHead, string[][] InStringTable, CTANDict InBindingDict, CTANGridWithControl[] OutControlGridList, T OutGrid) where T : CTANGrid
        {
            SetStandartInputGroupList(InGroupHead, InStringHead, InStringTable, InBindingDict, OutControlGridList);
            //OutGrid = new T();
            int tmpControlGridCount = InStringTable.GetLength(0);
            OutGrid.SetGridRowAndColumn(tmpControlGridCount, 1);
            if (typeof(T) == typeof(CTANGridWithControl))
            {
                for (int i = 0; i < InStringTable.GetLength(0); i++)
                {
                    OutControlGridList[i].SetGroup(InGroupHead[i]);
                    OutGrid.AssignControlToGrid(i, 0, OutControlGridList[i].mGroupBox);
                }
            }
            else
            {
                for (int i = 0; i < InStringTable.GetLength(0); i++)
                {
                    OutControlGridList[i].SetGroupInGrid(i, 0, InGroupHead[i], OutGrid);
                }
            }

        }
        #endregion 标准输入布局构型
        #region 复制和移动构件
        //从一个构件中重新设置行列
        //复制行列样式
        //InCopyRowIndexStartAndEnd=[]无元素，复制所有行
        //InCopyRowIndexStartAndEnd=[StartIndex]1个元素，从行索引StartIndex开始复制
        //InCopyRowIndexStartAndEnd=[StartIndex,EndIndex]2个元素，从行索引StartIndex复制到行索引EndIndex
        public void CopyAndModifyInputColumnFromGrid(int[] InCopyRowIndexStartAndEnd, int[] InCopyColumnIndexList, int InSetColumnCount, CTANGridWithControl InCopiedGrid)
        {
            int tmpTotalRowCount = InCopiedGrid.mControlTable.GetLength(0);
            int tmpTotalColumnCount = InCopiedGrid.mControlTable[0].Length;
            if (InCopiedGrid.mContentPanelTable == null)
            {
                SetGridRowAndColumn(tmpTotalRowCount, InSetColumnCount);
            }
            else
            {
                SetGridRowAndColumnWithContentTable(tmpTotalRowCount, InSetColumnCount);
            }
            CopyRowStyle(InCopiedGrid);
            //复制构件
            int tmpj;
            int[] tmpRowCopyRange = CTANBasicFunction.Return2ElemIntListByRange(InCopyRowIndexStartAndEnd,0, InCopiedGrid.mControlTable.GetLength(0));
            for (int i = tmpRowCopyRange[0]; i < tmpRowCopyRange[1]; i++)
            {
                for (int j = 0; j < InCopyColumnIndexList.Length; j++)
                {
                    tmpj = InCopyColumnIndexList[j];
                    InCopiedGrid.mControlTable[i][tmpj].CloneToGrid(i, tmpj, this);
                }
            }

            //复制GroupBox
            mGroupBox.Header = InCopiedGrid.mGroupBox.Header;
            if (InCopiedGrid.mGroupBox.Content != null)
            {
                mGroupBox.Content = this;
            }
        }
        ////移动行列样式
        ////InMoveRowIndexStartAndEnd=[]无元素，移动所有行
        ////InMoveRowIndexStartAndEnd=[StartIndex]1个元素，从行索引StartIndex开始移动
        ////InMoveRowIndexStartAndEnd=[StartIndex,EndIndex]2个元素，从行索引StartIndex移动到行索引EndIndex
        //public void MoveObjectFromGrid(int[] InMoveRowIndexStartAndEnd, int InFromColumnIndex, int InToColumnIndex, CTANGridWithControl InMovedGrid)
        //{

        //    CopyRowStyle(InMovedGrid);
        //    //移动构件
        //    int[] tmpRowCopyRange = CTANBasicFunction.Return2ElemIntListByRange(InMoveRowIndexStartAndEnd, 0, InMovedGrid.mControlTable.GetLength(0));
        //    if (InMovedGrid.mContentPanelTable != null)
        //    {
        //        for (int i = tmpRowCopyRange[0]; i < tmpRowCopyRange[1]; i++)
        //        {
        //            mContentPanelTable[i][InToColumnIndex].Content = InMovedGrid.mControlTable[i][InFromColumnIndex].mObject;
        //            //mContentPanelTable[i][InToColumnIndex].Content = InMovedGrid.mContentPanelTable[i][InFromColumnIndex].Content;
        //        }
        //    }

        //}
        //显示mControlTable中的控件列表
        public void DisplayObjectFromGridTable(int[] InMoveRowIndexStartAndEnd, int InFromColumnIndex, int InToColumnIndex, CTANObject[][] InDisplayGridObjTable)
        {

            //CopyRowStyle(InMovedGrid);
            //移动构件
            int[] tmpRowCopyRange = CTANBasicFunction.Return2ElemIntListByRange(InMoveRowIndexStartAndEnd, 0, InDisplayGridObjTable.GetLength(0));
            for (int i = tmpRowCopyRange[0]; i < tmpRowCopyRange[1]; i++)
            {
                mContentPanelTable[i][InToColumnIndex].Content = null;
                mContentPanelTable[i][InToColumnIndex].Content = InDisplayGridObjTable[i][InFromColumnIndex].mObject;
            }
            
        }

        #endregion 复制和移动构件
        #region 重载运算符
        public CTANObject[] this[int index]
        {
            get => mControlTable[index];
        }
        public CTANObject[] this[string index]
        {
            get => mControlTable[mRowNameToIndex[index]];
        }
        #endregion 重载运算符
    }


}



