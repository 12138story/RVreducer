using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace RVReducerDesignApp
{
    class CTANBasicFunction
    {
         public static int GetDataListLength<T>(T[] InDataList)
        {
            if (InDataList == null)
            {
                return 0;
            }
            else
            {
                return InDataList.Length;
            }
        }
        //public static int GetDataListLength(int[] InDataList)
        //{
        //    if (InDataList == null)
        //    {
        //        return 0;
        //    }
        //    else
        //    {
        //        return InDataList.Length;
        //    }
        //}
        //创建任意对象1维和2维列表
        public static void CreatObjectList<T>(int InObjectCount, out T[] OutObjectList) where T : new()
        {
            OutObjectList = new T[InObjectCount];
            for (int i = 0; i < InObjectCount; i++)
            {
                OutObjectList[i] = new T();
            }
        }
        public static void CreatObjectTable<T>(int InObjectRowCount, int InObjectColumnCount, out T[][] OutObjectTable) where T : new()
        {
            OutObjectTable = new T[InObjectRowCount][];
            for (int i = 0; i < InObjectRowCount; i++)
            {
                OutObjectTable[i]=new T[InObjectColumnCount];
                for (int j = 0; j < InObjectColumnCount; j++)
                {
                    OutObjectTable[i][j] = new T();
                }
            }
        }
        //将小于等于2个元素的1维数组扩展到2元素数值
        //InList=[]无元素，扩展到[InMinValue,InMaxValue]
        //InList=[Var1]1个元素，扩展到[Var1,InMaxValue]
        //InList=[Var1,Var2]2个元素，等于原数组[Var1,Var2]
        public static T[] Return2ElemIntListByRange<T>(T[] InList,T InMinValue, T InMaxValue) where T : struct
        {
            T[] tmpList=new T[2];
            if (InList.Length == 0)
            {
                tmpList[0] = InMinValue;
                tmpList[1] = InMaxValue;
            }
            else if (InList.Length == 1)
            {
                tmpList[0] = InList[0];
                tmpList[1] = InMaxValue;
            }
            else
            {
                tmpList[0] = InList[0];
                tmpList[1] = InList[1];
            }
            return tmpList;
        }
}
}
