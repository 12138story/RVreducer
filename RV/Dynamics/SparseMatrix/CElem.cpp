#include "CElem.h"

CElem::CElem()
{
}

//CElem::CElem(int InElemNum, int InElemTypeNum, int InMatNum, int InRealConsNum, int InNodeNumCount, int * InNodeNumList)
//{
//	ElemNum.first = InElemNum;
//	ElemNum.second = this;
//	ElemTypeNum.first = InElemTypeNum;
//	MatNum.first = InMatNum;
//	RealConsNum.first = InRealConsNum;
//	NodeCount = InNodeNumCount;
//	std::vector<int>(InNodeNumCount, 0).swap(NodeNumList);
//	NodeNumList.assign(InNodeNumList, InNodeNumList + InNodeNumCount);
//}

CElem::~CElem()
{
}

void CElem::AssignParaIndex(TanNodeListType& InTotalNumList, TanElemTypeListType& InElemTypeList, TanMatListType& InMatList, TanRealConsListType& InRealConsList, CInnerElemTypeList& InInnerElemTypeList)
{
	if (InElemTypeList.count(ElemTypeNum.first))
	{
		ElemTypeNum.second = InElemTypeList[ElemTypeNum.first];
	}
	if (InMatList.count(MatNum.first))
	{
		MatNum.second = InMatList[MatNum.first];
	}
	if (InRealConsList.count(RealConsNum.first))
	{
		RealConsNum.second = InRealConsList[RealConsNum.first];
	}
	std::vector<CNode*>(NodeCount, 0).swap(NodeAddressList);
	size_t SingleNodeFreedomCount = InInnerElemTypeList[ElemTypeNum.second->InnerTypeNum].FreedomNotationlist.size();
	std::vector <std::vector<CNode::tagNodeFreedom*>>(NodeCount, std::vector<CNode::tagNodeFreedom*>(SingleNodeFreedomCount,0)).swap(FreedomAddressList);
	for (int i = 0; i < NodeNumList.size(); i++)
	{
		NodeAddressList[i] = InTotalNumList[NodeNumList[i]];
	}
	
}

void CElem::GenStiffMatrix(CElem & InElem, CInnerElemTypeList& InInnerElemTypeList, CSparseMatrix & OutStiffMatrix)
{
	(*InInnerElemTypeList[InElem.ElemTypeNum.second->InnerTypeNum].pGenStiffMatrix)(
		InElem.NodeAddressList,
		InElem.MatNum.second,
		InElem.RealConsNum.second,
		OutStiffMatrix);
}

void CElem::GenStiffMatrix(CInnerElemTypeList & InInnerElemTypeList, CSparseMatrix & OutStiffMatrix)
{
	GenStiffMatrix(*this, InInnerElemTypeList, OutStiffMatrix);
}
