#pragma once
#include "CommonDefine.h"
class CRealConstant
{
public:
	int Num;
	TanDoubleVector ParaList;
	CRealConstant();
	template <typename T1, typename  T2, typename T3>
	void AssignData(T1&& InNum, T2&& InParaCount, T3&& InParaList)
	{
		Num = InNum;
		TanDoubleVector(InParaCount, 0).swap(ParaList);
		for (int i = 0; i < InParaCount; i++)
		{
			ParaList[i] = InParaList[i];
		}
	}
	template <typename T1, typename  T2, typename T3>
	CRealConstant(T1&& InNum, T2&& InParaCount, T3&& InParaList)
	{
		AssignData(InNum, InParaCount, InParaList);

		//Num.first = InNum;
		//Num.second = this;
		//std::vector<double>(InParaCount, 0).swap(ParaList);
		//for (int i=0;i< InParaCount;i++)
		//{
		//	ParaList[i] = InParaList[i];
		//}
	}
	~CRealConstant();
};

