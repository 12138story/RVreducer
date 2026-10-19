#include "CBoundaryCondition.h"

CBoundaryCondition::CBoundaryCondition()
{
}

//CBoundaryCondition::CBoundaryCondition(int InNodeNum, int InNodeFreedom, double InFreedomValue)
//{
//	Num = InNodeNum;
//	FreedomIndex = InNodeFreedom;
//	FreedomValue = InFreedomValue;
//}

CBoundaryCondition::~CBoundaryCondition()
{
}

//void CBoundaryCondition::PushBoundaryValue(int InNodeNum, int InNodeFreedomNotation, double InFreedomValue)
//{
//	//if (InNodeFreedomNotation<1000)
//	//{
//	//	(*this)[InNodeNum][InNodeFreedomNotation] = InFreedomValue;
//	//}
//	//else
//	//{
//	//	bool IsElemDefined=0;
//	//	if ((*this).count(InNodeNum))
//	//	{
//	//		if ((*this)[InNodeNum].count(InNodeFreedomNotation-1000))
//	//		{
//	//			IsElemDefined = 1;
//	//		}
//	//	}
//	//	if (!IsElemDefined)
//	//	{
//	//		(*this)[InNodeNum][InNodeFreedomNotation] = InFreedomValue;
//	//	}
//
//	//}
//	(*this)[InNodeNum][InNodeFreedomNotation] = InFreedomValue;
//	if ((InNodeFreedomNotation>=1000)&& (*this)[InNodeNum].count(InNodeFreedomNotation - 1000))
//	{
//		(*this)[InNodeNum].erase(InNodeFreedomNotation-1000);
//	}
//}
//
//void CBoundaryCondition::EraseBoundaryValue(int InNodeNum, int InNodeFreedomNotation)
//{
//	(*this)[InNodeNum].erase(InNodeFreedomNotation);
//}
