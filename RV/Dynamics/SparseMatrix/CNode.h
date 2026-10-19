#pragma once
#include "CTanVector.h"

class CNode
{
public:
	int Num = 0;
	CTanVector NodeCoor;
	TanIntVector ElemNumList;//考虑是否将单元也加进节点对象中
	bool IsAssignFreedom = 0;
	bool IsAssignDirichletBoundaryCondition = 0;
	struct tagNodeFreedom
	{
		int FreedomNotation = 0; //自由度标识，前一位为自由度标识，后一位为值，ux, uy, uz, Rotx, Roty, Rotz, 对应标识符1，2，3，4，5，6；
		double* FreedomValue = 0;
		int ToFullMatrixIndex = 0;
	};
	std::map<int, tagNodeFreedom> FreedomList; //自由度标识，前一位为自由度标识，后一位为值，ux, uy, uz, Rotx, Roty, Rotz, 对应标识符1，2，3，4，5，6；
	//std::map<int, int> ToFullMatrixIndex;
	CNode();
	template <typename T1, typename T2, typename T3, typename T4>
	void AssignData(T1&& InNodeNum, T2&& InX, T3&& InY, T4&& InZ)
	{
		Num = (int)InNodeNum;
		NodeCoor.Set(4);
		NodeCoor[0] = (double)InX;
		NodeCoor[1] = (double)InY;
		NodeCoor[2] = (double)InZ;
		NodeCoor[3] = 1.0;
	}
	template <typename T1>
	void AssignData(T1&& InNodeNum, double* InCoor)
	{
		Num = (int)InNodeNum;
		NodeCoor.Set(4, 3, InCoor);
		NodeCoor[3] = 1.0;
	}
	template <typename T1, typename T2, typename T3, typename T4>
	CNode(T1&& InNodeNum, T2&& InX, T3&& InY, T4&& InZ)
	{
		AssignData(InNodeNum, InX, InY, InZ);
	}
	template <typename T1>
	CNode(T1&& InNodeNum, double* InCoor)
	{
		AssignData(InNodeNum, InCoor);
	}

	~CNode();
	static void AssignFreedom(TanIntVector& InFreedomNotationlist,
		std::map<int, tagNodeFreedom>& OutFreedomList, 
		std::vector<CNode::tagNodeFreedom*>& OutElemToNodeFreedom);

};

