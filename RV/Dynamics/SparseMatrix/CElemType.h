#pragma once
#include "CommonDefine.h"
//typedef std::tuple<
//	CMeshElem//±àºÅ0£¬ÄöºÏµ¥Ôª
//> CInnerElemTypeList;//
class CElemType
{
public:
	int Num = 0;
	int InnerTypeNum = 0;
	~CElemType();
	//static CInnerElemTypeList InnerElemTypeList;
	CElemType();
	template <typename T1, typename T2>
	void AssignData(T1&& InNum, T2&& InInnerNum)
	{
		Num = InNum;
		InnerTypeNum = InInnerNum;
	}
	template <typename T1, typename T2>
	CElemType(T1&& InNum, T2&& InInnerNum)
	{
		AssignData(InNum, InInnerNum);
	}

	//static void ReturnFreedomList(CElemType& InElemType,TanIntVector& OutFreedomNotation);
};

