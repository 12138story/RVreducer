#include "CFEMPreprocessor.h"
CInnerElemTypeList CFEMPreprocessor::InnerElemTypeList = CInnerElemTypeList();
CFEMPreprocessor::CFEMPreprocessor()
{
}

CFEMPreprocessor::~CFEMPreprocessor()
{
	DeleteDataSpace(*this);
}


void CFEMPreprocessor::DeleteDataSpace(CFEMPreprocessor & IOFEMPreprocessPara)
{
	NSTanBasicObjOpt::DeleteListObj(IOFEMPreprocessPara.NodeList);
	NSTanBasicObjOpt::DeleteListObj(IOFEMPreprocessPara.ElemList);
	NSTanBasicObjOpt::DeleteListObj(IOFEMPreprocessPara.ElemTypeList);
	NSTanBasicObjOpt::DeleteListObj(IOFEMPreprocessPara.MatList);
	NSTanBasicObjOpt::DeleteListObj(IOFEMPreprocessPara.RealConsList);
	IOFEMPreprocessPara.BCCondition.clear();
}

void CFEMPreprocessor::DeleteDataSpace()
{
	DeleteDataSpace(*this);
}

void CFEMPreprocessor::ConstructPrepDataMap(TanNodeListType& InNodeList,
	TanElemListType& InElemList,
	TanElemTypeListType& InElemTypeList,
	TanMatListType& InMatList,
	TanRealConsListType& InRealConsList)
{
	//给所有单元赋予参数
	for (auto iter = InElemList.begin(); iter != InElemList.end(); iter++)
	{
		(*iter->second).AssignParaIndex(InNodeList, InElemTypeList, InMatList, InRealConsList,InnerElemTypeList);
	}
	size_t ElemCount = InElemList.size();
	size_t NodeCountInOneElemCount = 0;
	//size_t i = 0;
	TanElemListType::iterator i = InElemList.begin();
	//size_t i = InElemList.begin();
	size_t j = 0;
	CNode* NodeAddress = 0;
	int InnerElemTypeIndex = 0;
	for (i = InElemList.begin(); i != InElemList.end(); i++)
	{
		NodeCountInOneElemCount = i->second->NodeNumList.size();
		for (j = 0; j < NodeCountInOneElemCount; j++)
		{
			InnerElemTypeIndex = i->second->ElemTypeNum.second->InnerTypeNum;
			//NodeAddress = i->second->ElemNum.second->NodeAddressList[j];
			NodeAddress = i->second->NodeAddressList[j];
			NodeAddress->AssignFreedom(
				InnerElemTypeList[InnerElemTypeIndex].FreedomNotationlist,
				NodeAddress->FreedomList,
				i->second->FreedomAddressList[j]
			);
			//InElemList[i]->ElemTypeNum
		}
	}
}

void CFEMPreprocessor::ConstructPrepDataMap()
{
	ConstructPrepDataMap(NodeList, ElemList, ElemTypeList, MatList, RealConsList);
}




