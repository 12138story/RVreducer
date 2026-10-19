using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;

namespace RVReducerDesignApp
{
    class CTANPageLayoutFunction
    {
        public static int mRowHeight = 20;

        public static void CreatTextGridLayout(int InRowCount, int InColumnCount, ref List<List<TextBox>> IOTextBox, ref Grid IOGrid)
        {
            SetGridRowAndColumn(InRowCount, InColumnCount, ref IOGrid);
            for (int i = 0; i < InRowCount; i++)
            {
                IOTextBox.Add(new List<TextBox>());
                for (int j = 0; j < InColumnCount - 1; j++)
                {
                    IOTextBox[i].Add(new TextBox());
                    IOTextBox[i][j].BorderThickness = new Thickness(0);
                    IOTextBox[i][j].IsReadOnly = true;
                }
            }
            for (int i = 0; i < InRowCount; i++)
            {
                for (int j = 0; j < InColumnCount - 1; j++)
                {
                    Grid.SetRow(IOTextBox[i][j], i);
                    Grid.SetColumn(IOTextBox[i][j], j);
                    IOGrid.Children.Add(IOTextBox[i][j]);
                }
            }
        }
        //public void CreatBasicPageLayout(int InRowCount, int InColumnCount)
        //{
        //    SetGridRowAndColumn(InRowCount, InColumnCount, ref mPageGrid);
        //    for (int i = 0; i < InRowCount; i++)
        //    {
        //        mTextBlock.Add(new List<TextBox>());
        //        for (int j = 0; j < InColumnCount - 1; j++)
        //        {
        //            mTextBlock[i].Add(new TextBox());
        //            mTextBlock[i][j].BorderThickness=new Thickness(0);
        //            mTextBlock[i][j].IsReadOnly = true;
        //        }
        //    }
        //    for (int i = 0; i < InRowCount; i++)
        //    {                
        //        for (int j = 0; j < InColumnCount - 1; j++)
        //        {
        //            Grid.SetRow(mTextBlock[i][j],i);
        //            Grid.SetColumn(mTextBlock[i][j], j);                    
        //            mPageGrid.Children.Add(mTextBlock[i][j]);
        //        }
        //    }
        //    Content = mPageGrid;
        //}
        private static void SetGridRowAndColumn(int InRowCount, int InColumnCount, ref Grid IOGrid)
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
        public static void SetNameToGridTextBox(ref string[] InStringList, ref List<List<TextBox>> IOTextBox, ref Grid IOGrid)
        {
            int TempTextColumnCount = IOGrid.ColumnDefinitions.Count - 1;
            for (int i = 0; i < IOGrid.RowDefinitions.Count; i++)
            {
                for (int j = 0; j < TempTextColumnCount; j++)
                {
                    IOTextBox[i][j].Text = InStringList[i * TempTextColumnCount + j];
                }
            }
        }
        public static void SetGridRowHeight(int InRowHeight, ref Grid IOGrid)
        {
            for (int i = 0; i < IOGrid.RowDefinitions.Count; i++)
            {
                IOGrid.RowDefinitions[i].Height = new GridLength(InRowHeight);
            }
        }
        //统一的高度设置，高度为mHeight数值
        public static void SetGridRowHeight(ref Grid IOGrid)
        {
            for (int i = 0; i < IOGrid.RowDefinitions.Count; i++)
            {
                IOGrid.RowDefinitions[i].Height = new GridLength(mRowHeight);
            }
        }
        //添加textbox
        public static void AddTextBoxToGrid(int InBoxStartIndex,int InBoxAddCount,int InGridStartIndex,int InGridColumnIndex,ref List<TextBox> InBox, ref Grid IOGrid )
        {
            int TempBoxIndex = 0;
            for (int i=0;i< InBoxAddCount;i++ )
            {
                TempBoxIndex=InBoxStartIndex+i;
                Grid.SetRow(InBox[TempBoxIndex], InGridStartIndex+i);
                Grid.SetColumn(InBox[TempBoxIndex], InGridColumnIndex);
                IOGrid.Children.Add(InBox[TempBoxIndex]);
            }
        }
        //创建和添加TextBox
        public static void CreatAndAddTextBoxToGrid(int InBoxAddCount, int InGridStartIndex, int InGridColumnIndex, ref List<TextBox> IOBox, ref Grid IOGrid)
        {
            for (int i = 0; i < InBoxAddCount; i++)
            {
                IOBox.Add(new TextBox());
                Grid.SetRow(IOBox[i], InGridStartIndex + i);
                Grid.SetColumn(IOBox[i], InGridColumnIndex);
                IOGrid.Children.Add(IOBox[i]);
            }
        }
        public static void CreatAndAddTextBoxToGrid(int InInsertIndex,int InBoxAddCount, int InGridStartIndex, int InGridColumnIndex, ref List<TextBox> IOBox, ref Grid IOGrid)
        {
            int TempBoxIndex = 0;
            if (InInsertIndex == IOBox.Count)
            {
                for (int i = 0; i < InBoxAddCount; i++)
                {
                    TempBoxIndex = InInsertIndex + i;
                    IOBox.Add(new TextBox());
                    Grid.SetRow(IOBox[TempBoxIndex], InGridStartIndex + i);
                    Grid.SetColumn(IOBox[TempBoxIndex], InGridColumnIndex);
                    IOGrid.Children.Add(IOBox[TempBoxIndex]);
                }
            }
            else
            {
                for (int i = 0; i < InBoxAddCount; i++)
                {
                    TempBoxIndex = InInsertIndex + i;
                    IOBox.Insert(TempBoxIndex, new TextBox());
                    Grid.SetRow(IOBox[TempBoxIndex], InGridStartIndex + i);
                    Grid.SetColumn(IOBox[TempBoxIndex], InGridColumnIndex);
                    IOGrid.Children.Add(IOBox[TempBoxIndex]);
                }
            }

        }
        //将List中的TextBox设为TextBlock样式
        public static void SetTexBoxAsTextBlockInList(int InStartIndex,int InEndIndex,ref List<TextBox> IOBox)
        {
            for(int i= InStartIndex; i< InEndIndex+1; i++ )
            {
                IOBox[i].IsReadOnly = true;
                IOBox[i].BorderThickness = new Thickness(0);
            }
        }
        public static void SetTexBoxAsTextBlockInList(ref List<TextBox> IOBox)
        {
            for (int i = 0; i < IOBox.Count; i++)
            {
                IOBox[i].IsReadOnly = true;
                IOBox[i].BorderThickness = new Thickness(0);
            }
        }
    }
}
