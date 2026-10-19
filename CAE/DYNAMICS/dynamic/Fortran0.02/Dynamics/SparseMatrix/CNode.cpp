#include "CNode.h"

CNode::CNode()
{
	NodeCoor.Set(4,0.0);
	NodeCoor[3] = 1.0;
}

//CNode::CNode(int InNodeNum, double InX, double InY, double InZ)
//{
//	CreateNode(InNodeNum, InX, InY, InZ);
//
//}
//
//CNode::CNode(int InNodeNum, double * InCoor)
//{
//	CreateNode(InNodeNum, InCoor);
//}

//void CNode::CreateNode(int InNodeNum, double InX, double InY, double InZ)
//{
//	Num = InNodeNum;
//	NodeCoor.Set(4);
//	NodeCoor[0] = InX;
//	NodeCoor[1] = InY;
//	NodeCoor[2] = InZ;
//	NodeCoor[3] = 1.0;
//}

//void CNode::CreateNode(int InNodeNum, double * InCoor)
//{
//	Num = InNodeNum;
//	NodeCoor.Set(4, 3, InCoor);
//	NodeCoor[3] = 1.0;
//}

CNode::~CNode()
{
}

void CNode::AssignFreedom(TanIntVector & InFreedomNotationlist,
	std::map<int, tagNodeFreedom>& OutFreedomList,
	std::vector<CNode::tagNodeFreedom*>& OutElemToNodeFreedom)
{
	for (size_t i=0; i< InFreedomNotationlist.size();i++)
	{
		OutFreedomList[InFreedomNotationlist[i]] = tagNodeFreedom{ InFreedomNotationlist[i],0,0 };
		OutElemToNodeFreedom[i]=&OutFreedomList[InFreedomNotationlist[i]];
	}

}
