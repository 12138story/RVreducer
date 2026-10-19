#pragma once
#include "CommonDefine.h"
class CBoundaryCondition:
	public std::map<int, std::map<int, double>>
{
public:
	//int Num=0;//节点编号
	//int FreedomIndex = 0;//节点自由度编号
	//double FreedomValue = 0;//节点自由度值
	CBoundaryCondition();
	//CBoundaryCondition();
	~CBoundaryCondition();
	template <typename T1, typename T2, typename T3>
	void PushBoundaryValue(T1&& InNodeNum, T2&& InNodeFreedomNotation, T3&& InFreedomValue)
	{
		(*this)[InNodeNum][InNodeFreedomNotation] = InFreedomValue;
		if ((InNodeFreedomNotation >= 1000) && (*this)[InNodeNum].count(InNodeFreedomNotation - 1000))
		{
			(*this)[InNodeNum].erase(InNodeFreedomNotation - 1000);
		}
	}
	template <typename T1, typename T2>
	void DeleteBoundaryValue(T1&& InNodeNum, T2&& InNodeFreedomNotation)
	{
		(*this)[InNodeNum].erase(InNodeFreedomNotation);
	}
};

