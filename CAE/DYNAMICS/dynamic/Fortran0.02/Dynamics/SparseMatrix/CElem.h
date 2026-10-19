#pragma once
#include "CommonDefine.h"
#include "CNode.h"
#include "CElemType.h"
#include "CMaterials.h"
#include "CRealConstant.h"
#include "CInnerElemTypeList.h"
typedef std::map<int, CNode*> TanNodeListType;
typedef std::map<int, CElemType*> TanElemTypeListType;
typedef std::map<int, CMaterials*> TanMatListType;
typedef std::map<int, CRealConstant*> TanRealConsListType;
class CElem
{
public:
	int NodeCount=0;
	//std::pair<int, CElem*> ElemNum;
	int ElemNum;
	std::pair<int, CElemType*> ElemTypeNum;
	std::pair<int, CMaterials*> MatNum;
	std::pair<int, CRealConstant*> RealConsNum;

	TanIntVector NodeNumList;
	std::vector<CNode*> NodeAddressList;
	std::vector <std::vector<CNode::tagNodeFreedom*>> FreedomAddressList;
	//std::map<int, > NodeNumList;
	CElem();

	template <typename T1, typename  T2,typename T3, typename T4, typename T5>
	//int InElemNum, int InElemTypeNum, int InMatNum, int InRealConsNum, int InNodeNumCount, int* InNodeNumList
	void AssignData(T1&& InElemNum,
		T2&& InElemTypeNum, T3&& InMatNum, T4&& InRealConsNum, T5&& InNodeNumCount, int* InNodeNumList)
	{
		ElemNum = InElemNum;
		ElemTypeNum.first = InElemTypeNum;
		MatNum.first = InMatNum;
		RealConsNum.first = InRealConsNum;
		NodeCount = InNodeNumCount;
		std::vector<int>(InNodeNumCount, 0).swap(NodeNumList);
		NodeNumList.assign(InNodeNumList, InNodeNumList + InNodeNumCount);
	}
	template <typename T1, typename  T2, typename T3, typename T4, typename T5>
	CElem(T1&& InElemNum,
		T2&& InElemTypeNum, T3&& InMatNum, T4&& InRealConsNum, T5&& InNodeNumCount, int* InNodeNumList)
	{
		AssignData(InElemNum, InElemTypeNum, InMatNum, InRealConsNum, InNodeNumCount, InNodeNumList);
	}

	~CElem();
	//øº¬«‘ı√¥ºÏ—È±‡∫≈ «∑Ò¥Ê‘⁄.
	void AssignParaIndex(TanNodeListType& InTotalNumList,
		TanElemTypeListType& InElemTypeList,
		TanMatListType& InMatList,
		TanRealConsListType& InRealConsList,
		CInnerElemTypeList& InInnerElemTypeList);
	static void GenStiffMatrix(CElem& InElem, CInnerElemTypeList& InInnerElemTypeList, CSparseMatrix& OutStiffMatrix);
	void GenStiffMatrix(CInnerElemTypeList& InInnerElemTypeList, CSparseMatrix& OutStiffMatrix);
	//static void GenStiffMatrixCSRDiscrte(CElem& InElem,CInnerElemTypeList& InInnerElemTypeList,CSparseMatrixDiscreteCSR& OutStiffMatrix);
	//void GenStiffMatrixCSRDiscrte(CInnerElemTypeList& InInnerElemTypeList, CSparseMatrixDiscreteCSR& OutStiffMatrix);
};

