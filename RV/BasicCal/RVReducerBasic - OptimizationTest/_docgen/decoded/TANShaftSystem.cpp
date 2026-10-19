# SOURCE ENCODING: gbk
#include "TANShaftSystem.h"

//double CShaftSystemTAN::CalElemForceLocation(int InElemType, double* InElemDataList)
//{
//	double ResultForceLocationCoor;
//	double* TempLocationParaList = new double[4] {0};
//	//double TempForceLocation = 0;
//	if (InElemType==1)//渐开线齿轮
//	{
//		mComponentList
//
//	}
//	delete [] TempLocationParaList
//	return 0.0;
//}
#define FreedomUX 1
#define FreedomUY 2
#define FreedomUZ 3
#define FreedomROTX 4
#define FreedomROTY 5
#define FreedomROTZ 6
#define FreedomFX 1001
#define FreedomFY 1002
#define FreedomFZ 1003
#define FreedomMX 1004
#define FreedomMY 1005
#define FreedomMZ 1006

CShaftSystemTAN::CShaftSystemTAN()
{
	std::vector<int>(6,0).swap(mFixFreedomIndexCount);

}

void CShaftSystemTAN::Clear()
{
	mShaft.Initialization();
	std::vector<int>().swap(mComponentNumList);
	std::vector<tagShaftComponent>().swap(mComponentList);
	std::vector<CShaftTAN::tagBCNode>().swap(mBCNodeList);
	std::map<double, CBearingTAN*>().swap(mBearingLocateCoorToPtrList);
	std::vector<int>(6, 0).swap(mFixFreedomIndexCount);
}

void CShaftSystemTAN::AddBearingToElemList(CBearingTAN& InBearing)
{
	int TempBearingNotation=0;
	//if (InBearing.mBearingType== DeepGrooveBallBearing)
	//{
	//	TempBearingNotation = 1001;
	//}
	//else if (InBearing.mBearingType == NeedleBearing)
	//{
	//	TempBearingNotation = 1102;
	//}
	//else if (InBearing.mBearingType == TaperedRollerBearing)
	//{
	//	TempBearingNotation = 1103;
	//}
	//TempBearingNotation = InBearing.mBearingType;
	mComponentList.emplace_back(tagShaftComponent(InBearing.mBearingType, static_cast<void *> (&InBearing)));
}


void CShaftSystemTAN::CalTotalExtendFa()
{
	mTotalExtendFa = 0;
	mTotalFixBearingCount = 0;
	for (int i = 0; i < mComponentList.size(); i++)
	{
		if (mComponentList[i].ComponentType == InvGearNotation)
		{
			CInvoluteGearTAN::CSingleInvoluteTAN* TempElem = static_cast<CInvoluteGearTAN::CSingleInvoluteTAN*>(mComponentList[i].ComponentPtr);
			mTotalExtendFa = mTotalExtendFa + TempElem->ShaftForceVector[2];
		}
		else if (mComponentList[i].ComponentType == CycloidPinionNotation)
		{
			CPinionCycloidGearTAN::tagPinion* TempElem = static_cast<CPinionCycloidGearTAN::tagPinion*>(mComponentList[i].ComponentPtr);
			mTotalExtendFa = mTotalExtendFa + TempElem->ShaftForceVector[2];
		}
	}
}


void CShaftSystemTAN::MapComponentListByNum(std::vector<int>& InComponentNumList, std::vector<int>& InAllComponentTypeList, std::vector<void*>& InAllComponentList)
{
	for (int i=0;i< InComponentNumList.size();i++)
	{
		
		mComponentList.emplace_back(CShaftSystemTAN::tagShaftComponent(InAllComponentTypeList[InComponentNumList[i]],
			InAllComponentList[InComponentNumList[i]]));
		if (InAllComponentTypeList[InComponentNumList[i]]==InvGearNotation)
		{
			PushBackInvGear(static_cast<CInvoluteGearTAN::CSingleInvoluteTAN*>(InAllComponentList[InComponentNumList[i]]));
		}
		else if (InAllComponentTypeList[InComponentNumList[i]] == CycloidPinionNotation)
		{
			PushBackCycloidPinion(static_cast<CPinionCycloidGearTAN::tagPinion*>(InAllComponentList[InComponentNumList[i]]));
		}
		else
		{
			PushBackBearing(static_cast<CBearingTAN*>(InAllComponentList[InComponentNumList[i]]));
		}
	}
}

void CShaftSystemTAN::PushBackInvGear(CInvoluteGearTAN::CSingleInvoluteTAN* InInvGearPtr)
{
	double TempForceLoction = CalSymmetryElemForceLocation(InInvGearPtr->AssembleDist, InInvGearPtr->AssembleFaceType, InInvGearPtr->b_Design);
	mBCNodeList.emplace_back(CShaftTAN::tagBCNode()) ;
	CShaftTAN::tagBCNode* it = &(mBCNodeList.back());
	it->ZCoor = TempForceLoction;
	//渐开线齿轮只提供力
	it->FreedomIndexList.resize(6);
	it->FreedomIndexList[0] = 1001;
	it->FreedomIndexList[1] = 1002;
	it->FreedomIndexList[2] = 1003;
	it->FreedomIndexList[3] = 1004;
	it->FreedomIndexList[4] = 1005;
	it->FreedomIndexList[5] = 1006;
	//mBCNodeList[TempForceLoction].FreedomIndexList.emplace_back(1001);
	//mBCNodeList[TempForceLoction].FreedomIndexList.emplace_back(1002);
	//mBCNodeList[TempForceLoction].FreedomIndexList.emplace_back(1003);
	//mBCNodeList[TempForceLoction].FreedomIndexList.emplace_back(1004);
	//mBCNodeList[TempForceLoction].FreedomIndexList.emplace_back(1005);
	//mBCNodeList[TempForceLoction].FreedomIndexList.emplace_back(1006);
	it->FreedomValueList.resize(6);
	std::copy(InInvGearPtr->ShaftForceVector.begin(), InInvGearPtr->ShaftForceVector.end(), &(it->FreedomValueList[0]));
	for (int i = 0; i < 6; i++)
	{
		it->FreedomValueList[i] = it->FreedomValueList[i] * 1000;
	}
	mTotalExtendFa = mTotalExtendFa + it->FreedomValueList[2];
}

void CShaftSystemTAN::PushBackCycloidPinion(CPinionCycloidGearTAN::tagPinion* InCycloidPinionPtr)
{
	double TempForceLoction = CalSymmetryElemForceLocation(InCycloidPinionPtr->AssembleDist, InCycloidPinionPtr->AssembleFaceType, InCycloidPinionPtr->ToothWidth);
	mBCNodeList.emplace_back(CShaftTAN::tagBCNode());
	CShaftTAN::tagBCNode* it = &(mBCNodeList.back());
	it->ZCoor = TempForceLoction;
	//渐开线齿轮只提供力
	it->FreedomIndexList.resize(6);
	it->FreedomIndexList[0] = 1001;
	it->FreedomIndexList[1] = 1002;
	it->FreedomIndexList[2] = 1003;
	it->FreedomIndexList[3] = 1004;
	it->FreedomIndexList[4] = 1005;
	it->FreedomIndexList[5] = 1006;
	it->FreedomValueList.resize(6);
	std::copy(InCycloidPinionPtr->ShaftForceVector.begin(), InCycloidPinionPtr->ShaftForceVector.end(), &(it->FreedomValueList[0]));
	for (int i = 0; i < 6; i++)
	{
		it->FreedomValueList[i] = it->FreedomValueList[i] * 1000;
	}
	mTotalExtendFa = mTotalExtendFa + it->FreedomValueList[2];
}

void CShaftSystemTAN::PushBackBearing(CBearingTAN* InBearingPtr)
{
	if (InBearingPtr->mBearingType==DeepGrooveBallBearing)
	{
		double TempForceLoction = CalSymmetryElemForceLocation(InBearingPtr->AssembleDist, InBearingPtr->AssembleFaceType, InBearingPtr->Width);
		mBCNodeList.emplace_back(CShaftTAN::tagBCNode());
		CShaftTAN::tagBCNode* it = &(mBCNodeList.back());
		it->ZCoor = TempForceLoction;
		//渐开线齿轮只提供力
		it->FreedomIndexList.resize(3);
		it->FreedomIndexList[0] = 1;
		it->FreedomIndexList[1] = 2;
		//it->FreedomIndexList[2] = 3;
		//it->FreedomIndexList[3] = 4;
		//it->FreedomIndexList[4] = 5;
		mFixFreedomIndexCount[0]++;
		mFixFreedomIndexCount[1]++;
		//mFixFreedomIndexCount[2]++;
		//mFixFreedomIndex[3] = 1; 
		//mFixFreedomIndex[4] = 1;
		std::vector<double>(3, 0).swap(it->FreedomValueList);
		mBearingLocateCoorToPtrList[TempForceLoction]= InBearingPtr;
		mTotalFixBearingCount = mTotalFixBearingCount + 1;

	}
	else if (InBearingPtr->mBearingType == NeedleBearing)
	{
		double TempForceLoction = CalSymmetryElemForceLocation(InBearingPtr->AssembleDist, InBearingPtr->AssembleFaceType, InBearingPtr->Width);
		mBCNodeList.emplace_back(CShaftTAN::tagBCNode());
		CShaftTAN::tagBCNode* it = &(mBCNodeList.back());
		it->ZCoor = TempForceLoction;
		//渐开线齿轮只提供力
		it->FreedomIndexList.resize(2);
		it->FreedomIndexList[0] = 1;
		it->FreedomIndexList[1] = 2;
		//it->FreedomIndexList[2] = 4;
		//it->FreedomIndexList[3] = 5;
		std::vector<double>(2, 0).swap(it->FreedomValueList);
		mFixFreedomIndexCount[0]++;
		mFixFreedomIndexCount[1]++;
		mBearingLocateCoorToPtrList[TempForceLoction] = InBearingPtr;
		//mFixFreedomIndex[3] = 1;
		//mFixFreedomIndex[4] = 1;
		//mTotalFixBearingCount = mTotalFixBearingCount + 1;
	}
	else if (InBearingPtr->mBearingType == TaperedRollerBearing || InBearingPtr->mBearingType == AngularContactBallBearing)
	{
		double TempForceLoction = CalUnsymmetryElemForceLocation(InBearingPtr->AssembleDist, InBearingPtr->AssembleFaceType, InBearingPtr->AssembleForcePointDistPara, InBearingPtr->AssebleOrientation);
		if (NSTanBasiceFunction::Fsign(InBearingPtr->AssebleOrientation)== NSTanBasiceFunction::Fsign(mTotalExtendFa))//轴承拆卸方向与力作用方向一致，轴承不受轴向力
		{
			mBCNodeList.emplace_back(CShaftTAN::tagBCNode());
			CShaftTAN::tagBCNode* it = &(mBCNodeList.back());
			it->ZCoor = TempForceLoction;
			//渐开线齿轮只提供力
			it->FreedomIndexList.resize(2);
			it->FreedomIndexList[0] = 1;
			it->FreedomIndexList[1] = 2;
			//it->FreedomIndexList[2] = 4;
			//it->FreedomIndexList[3] = 5;
			std::vector<double>(2, 0).swap(it->FreedomValueList);
			mFixFreedomIndexCount[0]++;
			mFixFreedomIndexCount[1]++;
			//mFixFreedomIndex[3] = 1;
			//mFixFreedomIndex[4] = 1;
		}
		else 
		{
			mBCNodeList.emplace_back(CShaftTAN::tagBCNode());
			CShaftTAN::tagBCNode* it = &(mBCNodeList.back());
			it->ZCoor = TempForceLoction;
			//渐开线齿轮只提供力
			it->FreedomIndexList.resize(2);
			it->FreedomIndexList[0] = 1;
			it->FreedomIndexList[1] = 2;
			//it->FreedomIndexList[2] = 3;
			//it->FreedomIndexList[3] = 4;
			//it->FreedomIndexList[4] = 5;
			mFixFreedomIndexCount[0]++;
			mFixFreedomIndexCount[1]++;
			//mFixFreedomIndexCount[2]++;
			//mFixFreedomIndex[3] = 1;
			//mFixFreedomIndex[4] = 1;
			std::vector<double>(2, 0).swap(it->FreedomValueList);
			mTotalFixBearingCount = mTotalFixBearingCount + 1;
		}
		mBearingLocateCoorToPtrList[TempForceLoction] = InBearingPtr;
	}
}

void CShaftSystemTAN::CountFixedFreedomList(std::vector<CShaftTAN::tagBCNode>& InExistBCNode, std::vector<int>& OutFixFreedomIndexCount)
{
	//初始化
	std::vector<int>(6, 0).swap(OutFixFreedomIndexCount);
	//对各自由度计数
	for (int i = 0;i < InExistBCNode.size();i++)
	{
		for (int j = 0;j < InExistBCNode[i].FreedomIndexList.size();j++)
		{
			if (InExistBCNode[i].FreedomIndexList[j] == FreedomUX)
			{
				OutFixFreedomIndexCount[0]++;
			}
			else if (InExistBCNode[i].FreedomIndexList[j] == FreedomUY)
			{
				OutFixFreedomIndexCount[1]++;
			}
			else if (InExistBCNode[i].FreedomIndexList[j] == FreedomUZ)
			{
				OutFixFreedomIndexCount[2]++;
			}
			else if (InExistBCNode[i].FreedomIndexList[j] == FreedomROTX)
			{
				OutFixFreedomIndexCount[3]++;
			}
			else if (InExistBCNode[i].FreedomIndexList[j] == FreedomROTY)
			{
				OutFixFreedomIndexCount[4]++;
			}
			else if (InExistBCNode[i].FreedomIndexList[j] == FreedomROTZ)
			{
				OutFixFreedomIndexCount[5]++;
			}
		}
	}
}

bool CShaftSystemTAN::SupplementFixedFreedomToBCNode(double InSupplementNodeZCoor, std::vector<int>& InFixFreedomIndexCountList, std::vector<CShaftTAN::tagBCNode>& OutSupplementBCNode)
{
	//补充约束
	// 初始化
	std::vector<CShaftTAN::tagBCNode>(1).swap(OutSupplementBCNode);

	OutSupplementBCNode[0].ZCoor = InSupplementNodeZCoor;
		//沿X方向约束
	if (InFixFreedomIndexCountList[0] == 0)
	{
		OutSupplementBCNode[0].FreedomIndexList.emplace_back(FreedomUX);
		OutSupplementBCNode[0].FreedomValueList.emplace_back(0);
	}
	if (InFixFreedomIndexCountList[1] == 0)
	{
		OutSupplementBCNode[0].FreedomIndexList.emplace_back(FreedomUY);
		OutSupplementBCNode[0].FreedomValueList.emplace_back(0);
	}
	if (InFixFreedomIndexCountList[2] == 0)
	{
		OutSupplementBCNode[0].FreedomIndexList.emplace_back(FreedomUZ);
		OutSupplementBCNode[0].FreedomValueList.emplace_back(0);
	}
	//绕X方向转动
	if ((InFixFreedomIndexCountList[1] < 2) && (InFixFreedomIndexCountList[3] == 0))
	{
		OutSupplementBCNode[0].FreedomIndexList.emplace_back(FreedomROTX);
		OutSupplementBCNode[0].FreedomValueList.emplace_back(0);
	}
	//绕Y方向转动
	if ((InFixFreedomIndexCountList[0] < 2) && (InFixFreedomIndexCountList[4] == 0))
	{
		OutSupplementBCNode[0].FreedomIndexList.emplace_back(FreedomROTY);
		OutSupplementBCNode[0].FreedomValueList.emplace_back(0);
	}
	//绕Z方向转动
	if (InFixFreedomIndexCountList[5] == 0)
	{
		OutSupplementBCNode[0].FreedomIndexList.emplace_back(FreedomROTZ);
		OutSupplementBCNode[0].FreedomValueList.emplace_back(0);
	}
	if (OutSupplementBCNode[0].FreedomIndexList.size() > 0)
	{
		return 1;//需要补充自由度
	}
	else
	{
		return 0;//不需要补充自由度
	}
}

void CShaftSystemTAN::CalShaftSystemFirst()
{
	//补充自由度
	//绕Y轴转动方向
	mShaft.TransformGeometyParaToOrderSet();
	mShaft.InsertActionPointList(mBCNodeList);
	//如果约束不全，补充约束自由度,在轴末端
	std::vector<CShaftTAN::tagBCNode> TempBCNodeList;

	std::set<double>::iterator TempIt = mShaft.mNodeZCoorList.begin();
	TempIt++;
	double mSupplementBCNodeZCoor= (mShaft.mZCoorBegin + (*TempIt)) / 2;//补充自由度的点
	if (SupplementFixedFreedomToBCNode(mSupplementBCNodeZCoor, mFixFreedomIndexCount, TempBCNodeList))
	{
		mShaft.InsertActionPointList(TempBCNodeList);
	}

//	TempBCNodeList.emplace_back(CShaftTAN::tagBCNode());
//	TempBCNodeList[0].ZCoor = TempZCoor;
//	//TempBCNodeList[0].NodeNum = 0;
//	//TempBCNodeList[0].
//// 
//	//沿X方向约束
//	if (mFixFreedomIndexCount[0] ==0)
//	{	
//		TempBCNodeList[0].FreedomIndexList.emplace_back(1);
//		TempBCNodeList[0].FreedomValueList.emplace_back(0);
//	}
//	if (mFixFreedomIndexCount[1] == 0)
//	{
//		TempBCNodeList[0].FreedomIndexList.emplace_back(2);
//		TempBCNodeList[0].FreedomValueList.emplace_back(0);
//	}
//	if (mFixFreedomIndexCount[2] == 0)
//	{
//		TempBCNodeList[0].FreedomIndexList.emplace_back(3);
//		TempBCNodeList[0].FreedomValueList.emplace_back(0);
//	}
//	//绕X方向转动
//	if ((mFixFreedomIndexCount[1] < 2) && (mFixFreedomIndexCount[3] == 0))
//	{
//		TempBCNodeList[0].FreedomIndexList.emplace_back(4);
//		TempBCNodeList[0].FreedomValueList.emplace_back(0);
//	}
//	//绕Y方向转动
//	if ((mFixFreedomIndexCount[0] < 2) && (mFixFreedomIndexCount[4] == 0))
//	{
//		TempBCNodeList[0].FreedomIndexList.emplace_back(5);
//		TempBCNodeList[0].FreedomValueList.emplace_back(0);
//	}
//	//绕Z方向转动
//	if (mFixFreedomIndexCount[5] == 0)
//	{
//		TempBCNodeList[0].FreedomIndexList.emplace_back(6);
//		TempBCNodeList[0].FreedomValueList.emplace_back(0);
//	}
//	if (TempBCNodeList[0].FreedomIndexList.size()>0)
//	{
//		mShaft.InsertActionPointList(TempBCNodeList);
//	}

	mShaft.CalBeamStruct();
}

void CShaftSystemTAN::CalShaftSystem()
{
	//第一次轴计算，得到各轴承径向力,轴承轴向约束未添加
	CalShaftSystemFirst();
	//std::set<double>::iterator TempIt;
	//根据轴承径向力，计算附加轴向力
	int TempBearingLocateIndex;
	CTanVector TempForceVector(2);
	int TempFixBearingCount=0;
	std::map<CBearingTAN*, double> TempBearingAdditionalFa;//轴承附加轴向力
	//计算轴承附加力和实际轴向力
	mTotalFa = mTotalExtendFa;
	for (std::map<double,CBearingTAN*>::iterator i= mBearingLocateCoorToPtrList.begin();i!= mBearingLocateCoorToPtrList.end();i++)
	{
		//找到轴承对应序列
		//std::set<double>::iterator TempIt = mShaft.mNodeZCoorList.find(mBearingLocateCoorList[i]);
		TempBearingLocateIndex = NSTanBasiceFunction::FindSetElemIndex(i->first, mShaft.mNodeZCoorList);
		TempForceVector[0] = mShaft.mSolver.TotalFValue[6 * TempBearingLocateIndex];
		TempForceVector[1] = mShaft.mSolver.TotalFValue[6 * TempBearingLocateIndex+1];
		i->second->Fr= TempForceVector.ObtainNorm2();
		i->second->Fa_ex = i->second->CalAdditionalAxisForce(i->second->Fr);
		mTotalFa = mTotalFa + i->second->Fa_ex;
	}
	std::vector< std::map<double, CBearingTAN*>::iterator> tmpFixBearingPtr;//固定轴承指针
	std::vector< std::map<double, CBearingTAN*>::iterator> tmpUnFixBearingPtr;//非固定轴承指针
	//根据轴承附加力和总体附加力确定约束轴承,确定轴向约束数量
	for (std::map<double, CBearingTAN*>::iterator i = mBearingLocateCoorToPtrList.begin(); i != mBearingLocateCoorToPtrList.end(); i++)
	{
		if (i->second->JudgeBearingIsAxisConstrain(mTotalFa))
		{
			TempFixBearingCount = TempFixBearingCount + 1;
			tmpFixBearingPtr.emplace_back(i);
		}
		else
		{
			tmpUnFixBearingPtr.emplace_back(i);
		}
		 //+ ->JudgeBearingIsAxisConstrain(mTotalFa);
	}
	//根据轴承固定类型添加约束
	//附加固定约束
	bool IsAxisConstrainPointExist = 0;
	std::vector<CShaftTAN::tagBCNode> TempSupplementBCList(mBearingLocateCoorToPtrList.size());
	for (int i = 0; i < tmpFixBearingPtr.size(); i++)
	{
		//TempSupplementBCList[0].ZCoor = tmpFixBearingPtr[i]->first;
		mShaft.mNodeBCListZCoorToBCObjPtr[tmpFixBearingPtr[i]->first]->FreedomIndexList.emplace_back(FreedomUZ);
		mShaft.mNodeBCListZCoorToBCObjPtr[tmpFixBearingPtr[i]->first]->FreedomValueList.emplace_back(0);
		mFixFreedomIndexCount[2]++;
		//mShaft.mNodeBCListZCoorToBCObjPtr[i->first]->FreedomIndexList.emplace_back(FreedomFZ);
		//mShaft.mNodeBCListZCoorToBCObjPtr[i->first]->FreedomValueList.emplace_back(i->second->Fa);
		//+ ->JudgeBearingIsAxisConstrain(mTotalFa);
	}
	//附加力约束
	for (int i = 0; i < tmpUnFixBearingPtr.size(); i++)
	{
		//TempSupplementBCList[0].ZCoor = tmpUnFixBearingPtr[i]->first;
		mShaft.mNodeBCListZCoorToBCObjPtr[tmpUnFixBearingPtr[i]->first]->FreedomIndexList.emplace_back(FreedomFZ);
		mShaft.mNodeBCListZCoorToBCObjPtr[tmpUnFixBearingPtr[i]->first]->FreedomValueList.emplace_back(tmpUnFixBearingPtr[i]->second->Fa_ex);
		//mFixFreedomIndexCount[2]++;
		//mShaft.mNodeBCListZCoorToBCObjPtr[i->first]->FreedomIndexList.emplace_back(FreedomFZ);
		//mShaft.mNodeBCListZCoorToBCObjPtr[i->first]->FreedomValueList.emplace_back(i->second->Fa);
		//+ ->JudgeBearingIsAxisConstrain(mTotalFa);
	}
	//mShaft.InsertActionPointList(TempSupplementBCList);

	//计算实际轴承力,并添加轴承约束,力约束
	//for (std::map<double, CBearingTAN*>::iterator i = mBearingLocateCoorToPtrList.begin(); i != mBearingLocateCoorToPtrList.end(); i++)
	//{
	//	i->second->CalRealAxisForce(mTotalFa, TempFixBearingCount);
	//	mShaft.mNodeBCListZCoorToBCObjPtr[i->first]->FreedomIndexList.emplace_back(FreedomFZ);
	//	mShaft.mNodeBCListZCoorToBCObjPtr[i->first]->FreedomValueList.emplace_back(i->second->Fa);

	//}
	////mFixFreedomIndexCount;
	////根据总外力方向确定附加的轴向约束
	//bool IsAxisConstrainPointExist=0;
	//std::vector<int> TempFreedomIndexList;
	//std::vector<double> TempFreedomValueList;
	//double TempConstrainZCoor = 0;
	//if (mTotalFa < 0)//总体向左移动,z向约束附加到最左侧
	//{
	//	TempConstrainZCoor=*(mShaft.mNodeZCoorList.begin());		
	//}
	//else//总体向右移动，z向约束附加到最右侧
	//{
	//	TempConstrainZCoor = *(mShaft.mNodeZCoorList.rbegin());
	//}
	////补充UZ向约束,有问题，应该考虑用轴承位移来进行约束，后续再进行更改
	////std::vector<int> tmp2ndFixFreedomIndexCount(mFixFreedomIndexCount);//第二次计算时的固定自由度计数
	//for (int i = 0; i < mBCNodeList.size(); i++)
	//{
	//	//Z向已有约束点，则不新建
	//	if (abs(TempConstrainZCoor - mBCNodeList[i].ZCoor) < DefaultTolerance)
	//	{
	//		IsAxisConstrainPointExist = 1;
	//		TempFreedomIndexList.emplace_back(FreedomUZ);
	//		TempFreedomValueList.emplace_back(0);
	//		for (int j = 0; j < mBCNodeList[i].FreedomIndexList.size(); j++)
	//		{
	//			//将现有和UZ无关的自由度值复制
	//			if ((mBCNodeList[i].FreedomIndexList[j] != FreedomUZ) && (mBCNodeList[i].FreedomIndexList[j] != FreedomFZ))
	//			{
	//				TempFreedomIndexList.emplace_back(mBCNodeList[i].FreedomIndexList[j]);
	//				TempFreedomValueList.emplace_back(mBCNodeList[i].FreedomValueList[j]);
	//			}
	//		}
	//		TempFreedomIndexList.swap(mBCNodeList[i].FreedomIndexList);
	//		TempFreedomValueList.swap(mBCNodeList[i].FreedomValueList);
	//		mFixFreedomIndexCount[2]++;
	//	}
	//}
	//if (!IsAxisConstrainPointExist)
	//{
	//	mBCNodeList.emplace_back(CShaftTAN::tagBCNode());
	//	mBCNodeList.back().ZCoor = TempConstrainZCoor;
	//	mBCNodeList.back().FreedomIndexList.emplace_back(FreedomUZ);
	//	mBCNodeList.back().FreedomValueList.emplace_back(0);
	//	mFixFreedomIndexCount[2]++;
	//}

	mShaft.ClearCalculatorData();
	mShaft.TransformGeometyParaToOrderSet();

	std::vector<CShaftTAN::tagBCNode> TempBCNodeList;
	if (SupplementFixedFreedomToBCNode(mSupplementBCNodeZCoor, mFixFreedomIndexCount, TempBCNodeList))
	{
		mShaft.InsertActionPointList(TempBCNodeList);
	}
	//补充剩余约束
	mShaft.InsertActionPointList(mBCNodeList);
	mShaft.CalBeamStruct();
	if (IsMainBearing == true)
	{
		NSTanIOStream::PrintMatrixInConsole(30, 1, &mShaft.mSolver.TotalUValue[0]);
		NSTanIOStream::PrintMatrixInConsole(30, 1, &mShaft.mSolver.TotalFValue[0]);
		mShaft.GetForceAndForceLocation();
	}
	else
	{
		NSTanIOStream::PrintMatrixInConsole(42, 1, &mShaft.mSolver.TotalUValue[0]);
		NSTanIOStream::PrintMatrixInConsole(42, 1, &mShaft.mSolver.TotalFValue[0]);
		mShaft.GetForceAndForceLocation();
	}

}
double CShaftSystemTAN::CalUnsymmetryElemForceLocation(double InDistBetweenAssembleEdgeToShaftLeft, int InAssembleFaceType, double InElemForcePointDist, int InElemAxisDirectIndex)
{
	double ResultForceLocateCoor;
	double TempBearingForcePointDist;
	TempBearingForcePointDist = NSTanBasiceFunction::Fsign(InAssembleFaceType)*InElemForcePointDist;
	ResultForceLocateCoor = InDistBetweenAssembleEdgeToShaftLeft + NSTanBasiceFunction::Fsign(InElemAxisDirectIndex) * TempBearingForcePointDist;
	//if (InAssembleFaceType == 0)
	//{
	//	TempBearingForcePointDist = InElemForcePointDist;
	//}
	//else
	//{
	//	TempBearingForcePointDist = -InElemForcePointDist;
	//}

	//if (InElemAxisDirectIndex == 1)
	//{
	//	ResultForceLocateCoor = InDistBetweenAssembleEdgeToShaftLeft + TempBearingForcePointDist;
	//}
	//else
	//{
	//	ResultForceLocateCoor = InDistBetweenAssembleEdgeToShaftLeft - TempBearingForcePointDist;
	//}
	return ResultForceLocateCoor;
}

double CShaftSystemTAN::CalSymmetryElemForceLocation(double InDistBetweenAssembleEdgeToShaftLeft, int InAssembleFaceType, double InElemWidth)
{
	if (InAssembleFaceType == AssembleLeftBaseNotation)
	{
		return InElemWidth / 2 + InDistBetweenAssembleEdgeToShaftLeft;
	}
	else if (InAssembleFaceType == AssembleRightBaseNotation)
	{
		return InDistBetweenAssembleEdgeToShaftLeft - InElemWidth / 2;
	}
	else
	{
		return InDistBetweenAssembleEdgeToShaftLeft;
	};
}
