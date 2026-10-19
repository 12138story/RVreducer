using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Data;
using System.Diagnostics.Eventing.Reader;
using RVReducerDesignApp;

namespace RVReducerDesignApp
{
    public partial class CTANBasicPage : Page
    {
        public CTANBasicPage()
        {
        }
    }
    public class CTANScrollPage : ScrollViewer
    {
        public CTANBasicPage mPage = new CTANBasicPage();
        public Frame mFrameForPage = new Frame();
        public CTANScrollPage()
        {
            this.Content = mFrameForPage;
            mFrameForPage.Content = mPage;
        }
    }

    //基本页面定义，包括输入文本框表，输出表
    public class CTANBasicContentPageObj
    {
        public static string[] mHeadText = new string[] { "名称", "单位", "数值" };
        public static string[] mShaftAssembleText = new string[]
{
                    "定位面","",
                    "定位面距基准面距离","毫米",
                    "Y方向偏移","毫米",

};
        //输入页
        public CTANGrid mInputPageGrid = new CTANGrid();
        public CTANGridWithControl[] mInputGridList;
        //属性页
        public Grid mPropertyGrid = new Grid();
        public CTANGridWithControl[] mPropertyGridList;
        //输出页
        public CTANGrid mOutputPageGrid = new CTANGrid();
        public CTANDataTable mOutputDataTable = new CTANDataTable();
        //示例页
        public Grid mExamplePageGrid = new Grid();
        //三维显示页        
        public Grid m3DPageGrid = new Grid();
        
        //该页面的数据存储字典
        public CTANDict mDataStorageDict = new CTANDict();
        //public CTANDict mTotalDataStorageDict = new CTANDict();
        //存储树
        public TreeViewItem mTreeViewItem = new TreeViewItem();
        //进行轴组装所需的数据
        //Id
        public int mID;
        //数据持久化唯一键：缺省用 Header；当页面需要“显示同名、存档键仍唯一”时显式赋 mDataKey
        public string? mDataKey;
        // 返回存档/查找使用的稳定键（mDataKey 优先，其次 Header）
        public string GetDataKey()
        {
            if (!string.IsNullOrEmpty(mDataKey)) return mDataKey;
            return mTreeViewItem.Header?.ToString() ?? string.Empty;
        }
        //所属的轴page
        public CTANBasicContentPageObj mBelongedShaftPage;
        public Image mImage = new Image();
        ////测试
        //public CTANGrid mPageGrid = new CTANGrid();
        private void Initializing()
        {
            mTreeViewItem.DataContext = this;
            mInputPageGrid.DataContext = this;
            AssignPageToIDDict(MainWindow.sPageIDDict);
            mBelongedShaftPage = null;
        }
        public CTANBasicContentPageObj()
        {
            Initializing();
        }
        public CTANBasicContentPageObj(string InTreeViewItemHead)
        {
            Initializing();
            mTreeViewItem.Header = InTreeViewItemHead;
        }

        //将页面放入ID字典
        public void AssignPageToIDDict(Dictionary<int, CTANBasicContentPageObj> IOIDDict)
        {
            for (int i = 0; i < 10000000; i++)
            {
                if(!IOIDDict.ContainsKey(i))
                {
                    mID = i;
                    IOIDDict.Add(i, this);
                    break;
                }
            }
        }
        public void ExportPageToStringIDDict(Dictionary<string, int> OutDict)
        {
            OutDict.Add(mTreeViewItem.Header as string, mID);
        }
        //删除轴所属关系
        public void DeleteFromShaft()
        {
            if(mBelongedShaftPage!=null)
            {
                (mBelongedShaftPage as CTANCrankShaftSystem).mComponentList.Remove(mID);
                mBelongedShaftPage = null;
            }
        }
        public void AssignToShaft(CTANCrankShaftSystem InShaftPage)
        {
            DeleteFromShaft();
            InShaftPage.mComponentList.Add(mID, this);
            mBelongedShaftPage = InShaftPage;
        }
        public void BindingDictToOutputTable(CTANDict InSourceDict, int InBindingColumnIndex)
        {
            string TmpBindingKey;
            for (int i = 0; i < mOutputDataTable.Rows.Count; i++)
            {
                TmpBindingKey = (string)mOutputDataTable.Rows[i][0];
                BindingOperations.SetBinding(InSourceDict[TmpBindingKey],
                    CTANDouble.ValueProperty,
                    new Binding("[" + InBindingColumnIndex.ToString() + "]") { Source = mOutputDataTable.Rows[i], Mode = BindingMode.OneWayToSource });
            }
        }
        public void BindingDictToOutputTableZJ(CTANDataTable[] mOutputTables, CTANDict InSourceDict, int InBindingColumnIndex)
        {
            string TmpBindingKey;
            for (int j = 0; j < mOutputTables.Length; j++)
            {
                for (int i = 0; i < mOutputTables[j].Rows.Count; i++)
                {
                    TmpBindingKey = (string)mOutputTables[j].Rows[i][0];
                    BindingOperations.SetBinding(InSourceDict[TmpBindingKey],
                        CTANDouble.ValueProperty,
                        new Binding("[" + InBindingColumnIndex.ToString() + "]") { Source = mOutputTables[j].Rows[i], Mode = BindingMode.OneWayToSource });
                }
            }
        }
        public virtual double ReturnAssembleDistance()
        {
            return mDataStorageDict["定位面距基准面距离"].Value;
        }
        public virtual double ReturnGeometryWidth()
        {
            return mDataStorageDict["设计齿宽"].Value;
        }
        public virtual double ReturnGeometryDiameter()
        {
            return mDataStorageDict["轴承外径D"].Value;
        }
        public virtual double ReturnBearingInnerDiameter()
        {
            return mDataStorageDict["轴承内径"].Value;
        }
        public virtual double ReturnCycloidGearRf()
        {
            return mDataStorageDict["摆线轮齿根圆直径"].Value;
        }
        public virtual int ReturnAssembleFaceType()
        {
            int tmpAssebleFaceType = Convert.ToInt32(mDataStorageDict["定位面"].Value);
            if (tmpAssebleFaceType==CTANRVSet.AssembleLeftBaseNotation)
            {
                return 1;
            }
            else if (tmpAssebleFaceType == CTANRVSet.AssembleMiddleBaseNotation)
            {
                return 0;
            }
            else
            {
                return -1;
            }
        }
        public virtual double ReturnYDirectionOffset()
        {
            return mDataStorageDict["Y方向偏移"].Value;
        }
        public virtual string ReturnBearingType()
        {
            int tmpBearingType = Convert.ToInt32(mDataStorageDict["轴承类型"].Value);
            {
                if (tmpBearingType == CTANBearing.DeepGrooveBallBearing)
                {
                    return "深沟球轴承";
                }
                else if (tmpBearingType == CTANBearing.NeedleBearing)
                {
                    return "滚针轴承";
                }
                else 
                {
                    return "圆锥滚子轴承";
                }
            }
        }
            
        public virtual double ReturnTaperedBearingB()
        {
            return mDataStorageDict["轴承宽度B"].Value;
        }
        public virtual double ReturnTaperedBearingC()
        {
            return mDataStorageDict["轴承宽度C"].Value;
        }
        public virtual int ReturnTaperedDirection()
        {
            int tmpTaperedDirection = Convert.ToInt32(mDataStorageDict["轴承朝向"].Value);
            if (tmpTaperedDirection == CTANBearing.BearingExForceToRightNotation)
            {
                //return "右侧受力";
                return 1;
            }
            else
            {
                return -1;
                //return "左侧受力";
            }
        }
        public void BindingDictToOutputTable(int InBindingColumnIndex)
        {
            BindingDictToOutputTable(mDataStorageDict, InBindingColumnIndex);
        }
        public virtual void CalPageData()
        {

        }
        public virtual void UpdatePageControlContent()
        {
        }
        public virtual void UpdateDllInputDoubleList(int DllInputStartIndex, double[] IODllInputData)
        {

        }
        public virtual void UpdateDllOutputDoubleList(int DllInputStartIndex, double[] IODllOutputData)
        {

        }

    }
}





