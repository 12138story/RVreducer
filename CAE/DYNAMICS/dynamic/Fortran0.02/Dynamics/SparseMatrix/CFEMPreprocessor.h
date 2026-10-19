#pragma once
#include "CTanVector.h"
#include "CNode.h"
#include "CElem.h"
#include "CInnerElemTypeList.h"
#include "CBoundaryCondition.h"
typedef std::map<int, CElem*> TanElemListType;
class CFEMPreprocessor
{
private:
	//使用map的添加元素基础函数
	template <typename T0,typename T1,typename T2, class ...Args>
	void InsertObjBaseFunction(T1&& InObjNum, T2&& MapList,Args&& ... InData)
	{
		T0*& MapElem = MapList[InObjNum];
		if (MapElem)
		{
			MapElem->AssignData(InObjNum, InData...);
		}
		else
		{
			MapElem = new T0(InObjNum, InData...);
		}
	}
	template <typename T0, typename T1, typename T2>
	void DeleteObjBaseFunction(T1&& InObjNum, T2&& MapList)
	{
		T0 MapKeyIter = MapList.find(InObjNum);
		if (MapKeyIter!= MapList.end())
		{
			delete MapKeyIter->second;
			MapList.erase(MapKeyIter);
		}
	}

public:
	TanNodeListType NodeList;
	TanElemListType ElemList;
	TanElemTypeListType ElemTypeList;
	TanMatListType MatList;
	TanRealConsListType RealConsList;
	CBoundaryCondition BCCondition;
	static CInnerElemTypeList InnerElemTypeList;
	CFEMPreprocessor();
	~CFEMPreprocessor();
	static void DeleteDataSpace(CFEMPreprocessor& IOFEMPreprocessPara);
	void  DeleteDataSpace();
	//添加元素
	template <typename T1, class ...Args>
	void InsertNode(T1&& InNodeNum, Args&& ... InData)
	{
		InsertObjBaseFunction<CNode>(InNodeNum, NodeList, InData...);
	}	
	template <typename T1, class ...Args>
	//T1&& InElemNum,	T2&& InElemTypeNum, T3&& InMatNum, T4&& InRealConsNum, T5&& InNodeNumCount, int* InNodeNumList
	void InsertElem(T1&& InElemNum, Args&& ... InData)
	{
		InsertObjBaseFunction<CElem>(InElemNum, ElemList, InData...);
	}
	template <typename T1, class ...Args>
	void InsertElemType(T1&& InElemTypeNum, Args&& ... InData)
	{
		InsertObjBaseFunction<CElemType>(InElemTypeNum, ElemTypeList, InData...);
	}
	template <typename T1, class ...Args>
	void InsertMat(T1&& InMatNum, Args&& ... InData)
	{
		InsertObjBaseFunction<CMaterials>(InMatNum, MatList, InData...);
	}
	template <typename T1, class ...Args>
	void InsertRealConst(T1&& InRealConstNum, Args&& ... InData)
	{
		InsertObjBaseFunction<CRealConstant>(InRealConstNum, RealConsList, InData...);
	}
	template <class ...Args>
	void InsertBoundaryCondition(Args&& ... InData)
	{
		BCCondition.PushBoundaryValue(InData...);
	}
	//删除元素
	template<typename T1>
	void DeleteNode(T1&& NodeNum)
	{
		DeleteObjBaseFunction<TanNodeListType::iterator>(NodeNum, NodeList);
	}
	template<typename T1>
	void DeleteElem(T1&& ElemNum)
	{
		DeleteObjBaseFunction<TanElemListType::iterator>(ElemNum, ElemList);
	}
	template<typename T1>
	void DeleteElemType(T1&& ElemTypeNum)
	{
		DeleteObjBaseFunction<TanElemTypeListType::iterator>(ElemTypeNum, ElemTypeList);
	}
	template<typename T1>
	void DeleteMat(T1&& MatNum)
	{
		DeleteObjBaseFunction<TanMatListType::iterator>(MatNum, MatList);
	}
	template<typename T1>
	void DeleteRealConst(T1&& RealConstNum)
	{
		DeleteObjBaseFunction<TanRealConsListType::iterator>(RealConstNum, RealConsList);
	}
	template<typename T1, typename T2>
	void DeleteBoundaryCondition(T1&& NodeNum, T2&& FreedomNotation)
	{
		BCCondition.DeleteBoundaryValue(NodeNum, FreedomNotation);
	}

	//构建了单元自由度到节点自由度的映射，以及节点到单元编号映射
	static void ConstructPrepDataMap(
		TanNodeListType& InNodeList,
		TanElemListType& InElemList,
		TanElemTypeListType& InElemTypeList,
		TanMatListType& MatList,
		TanRealConsListType& RealConsList
		);
	void ConstructPrepDataMap();
};

