# SOURCE ENCODING: gbk
#include "TANShaft.h"
CShaftTAN::CShaftTAN()
{
	Initialization();
}
CShaftTAN::~CShaftTAN()
{
	
	//DeleteDatadSpace();
}
void CShaftTAN::Initialization()
{
	mShaftDirect.Set(3);
	mpSecRealConst.Set(4);
	mCalSecTypeToDataCount = mDefaultSecTypeToDataCount;
	std::vector<double>().swap(mOriginGeoemtryNode);
	std::vector<int>().swap(mOriginSecRealConstType);
	std::vector<CTanVector>().swap(mOriginSecRealConst);
	std::set<double>().swap(mNodeZCoorList);
	std::map<double, tagBCNode*>().swap(mNodeBCListZCoorToBCObjPtr);
	std::map<int, tagBCNode*>().swap(mNodeBCListNumToBCObjPtr);
	mNodeCount = 0;
}
 int CShaftTAN::mDefaultSecTypeToDataCount[3] = {4,
	1,
	4 };

//int CTANShaft::DeleteDatadSpace()
//{
//	if (mIsDataCreatByNewMethod!=0)
//	{
//		delete[] mpNodeCoorList;
//		delete[] mpNodeResultData;
//		delete[] mpSecCreatTypeList;
//		delete[] mpBCNodeList;
//		delete[] mpBCNodeFreedomIndex;
//		delete[] mpBCNodeData;
//		std::vector<double>(0).swap(mpSecRealConst);
//	}
//	mIsDataCreatByNewMethod = 0;
//	return 0;
//}
//
//int CTANShaft::CreatDataSpace(int InNodeCount,int InBCCount)
//{
//	DeleteDatadSpace();
//	mNodeCount = InNodeCount;
//	mpNodeCoorList = new double[3*InNodeCount];
//	mpNodeResultData = new double[mResultTypeCount*InNodeCount];
//	mpSecCreatTypeList = new int[InNodeCount-1];
//	mBCCount = InBCCount;
//	mpBCNodeList = new int[InBCCount];
//	mpBCNodeFreedomIndex= new int[InBCCount];
//	mpBCNodeData = new double[InBCCount];
//	mIsDataCreatByNewMethod = 1;
//	return 0;
//}

void CShaftTAN::DefineOrginGeometry(int InNodeCount, double* InNodeZCoorList, double* InShaftDirect, int* InSecTypeList, double* InSecTypeDataList)
{
	int i = 0;
	for (i = 0; i < 3; i++)
	{
		mShaftDirect[i] = InShaftDirect[i];
	}
	mOriginGeoemtryNode.resize(InNodeCount);
	for (i = 0; i < InNodeCount; i++)
	{
		mOriginGeoemtryNode[i]=InNodeZCoorList[i];
	}
	double* TempSecDataPtr = InSecTypeDataList;
	mOriginSecRealConst.resize(InNodeCount - 1);
	mOriginSecRealConstType.resize(InNodeCount - 1);
	for (i = 0; i < InNodeCount - 1; i++)
	{
		mOriginSecRealConst[i]=CTanVector(4);
		TempSecDataPtr = TempSecDataPtr + GenRealConstBySecType(InSecTypeList[i], TempSecDataPtr, &(mOriginSecRealConst[i][0]));
		mOriginSecRealConstType[i] = 0;
	}
}

void CShaftTAN::InsertGeometyPara(int InNodeCount, double* InNodeZCoorList, double* InShaftDirect, int* InSecTypeList, double* InSecTypeDataList)
{
	int i = 0;
	for (i = 0; i < 3; i++)
	{
		mShaftDirect[i] = InShaftDirect[i];
	}
	for (i=0;i<InNodeCount;i++)
	{
		mNodeZCoorList.insert(InNodeZCoorList[i]);
	}

	double* TempSecDataPtr = InSecTypeDataList;
	for (i=0;i< InNodeCount-1;i++)
	{
		mRealConstList[InNodeZCoorList[i+1]] = CTanVector(4);
		TempSecDataPtr= TempSecDataPtr + GenRealConstBySecType(InSecTypeList[i], TempSecDataPtr, &(mRealConstList[InNodeZCoorList[i+1]][0]));
	}
	mZCoorBegin = *(mNodeZCoorList.begin());
	mZCoorEnd = *(mNodeZCoorList.rbegin());
}

void CShaftTAN::TransformGeometyParaToOrderSet()
{
	mNodeZCoorList.clear();

	int i = 0;
	int TempNodeCount = mOriginGeoemtryNode.size();

	for (i = 0; i < TempNodeCount; i++)
	{
		mNodeZCoorList.insert(mOriginGeoemtryNode[i]);
	}
	for (i = 0; i < TempNodeCount - 1; i++)
	{
		mRealConstList[mOriginGeoemtryNode[i + 1]] = CTanVector(4);
		std::copy(mOriginSecRealConst[i].begin(), mOriginSecRealConst[i].end(), &(mRealConstList[mOriginGeoemtryNode[i + 1]][0]));
	}
	mZCoorBegin = *(mNodeZCoorList.begin());
	mZCoorEnd = *(mNodeZCoorList.rbegin());
}



void CShaftTAN::InsertActionPointList(std::vector<tagBCNode>& InBCList)
{
	std::set<double>::iterator it;
	std::set<double>::iterator itBegin;
	std::pair<std::set<double>::iterator, bool> SetJudgeIterator;
	std::pair<std::map<double, CTanVector>::iterator, bool> MapJudgeIterator;
	std::map<double, CTanVector>::iterator MapIterator;
	int TempNodeIndex;
	//插入所有元素
	for (int i=0;i< InBCList.size();i++)
	{
		if (InBCList[i].ZCoor< mZCoorBegin)
		{
			InBCList[i].ZCoor = mZCoorBegin;
		}
		if (InBCList[i].ZCoor > mZCoorEnd)
		{
			InBCList[i].ZCoor = mZCoorEnd;
		}
		//插入且判断是否已经有数据;
		SetJudgeIterator = mNodeZCoorList.emplace(InBCList[i].ZCoor);
		//插入且未有数据
		itBegin = mNodeZCoorList.begin();
		if (SetJudgeIterator.second)
		{
			MapJudgeIterator=mRealConstList.insert(std::make_pair(InBCList[i].ZCoor, CTanVector(4)));
			MapIterator = MapJudgeIterator.first;
			MapIterator++;
			std::copy(MapIterator->second.begin(), MapIterator->second.end(), &(MapJudgeIterator.first->second[0]));
			//mRealConstList[InBCList[i].ZCoor]
			//TempNodeIndex = std::distance(itBegin, it);
			//前后都要补充单元截面属性
			//mRealConstList.insert(CTanVector(4));
		}
	}
	//再在总体集合里面找边界条件值对应位置
	for (int i = 0; i < InBCList.size(); i++)
	{
		mNodeBCListZCoorToBCObjPtr[InBCList[i].ZCoor] = &InBCList[i];
		//it = mNodeZCoorList.find(InBCList[i].ZCoor);
		//TempNodeIndex = std::distance(itBegin, it );
		//InBCList[i].NodeNum = TempNodeIndex;
		//mNodeBCListNumToObjPtr[TempNodeIndex] = &InBCList[i];
	}
}

void CShaftTAN::ClearCalculatorData()
{
	mNodeZCoorList.clear();
	mRealConstList.clear();
	mNodeBCListZCoorToBCObjPtr.clear();
	mNodeBCListNumToBCObjPtr.clear();
	mProcessor.DeleteDataSpace();
}

void CShaftTAN::DefineNodeList(int InNodeCount,  double* InNodeCoor)
{
	for (int i=0;i< InNodeCount;i++)
	{
		mProcessor.InsertNode(i, InNodeCoor[0+i*3], InNodeCoor[1+ i * 3], InNodeCoor[2 + i * 3]);
	}
}

//void CTANShaft::GenElemList()
//{
//	mProcessor.InsertElemType(1, 1);
//	for (int i=0;i< mNodeCount -1;i++)
//	{
//		
//	}
//}

//void CTANShaft::DefineSecList(int * SecTypeList, double * InSecTypeDataList)
//{
//	for (int i = 0; i < mProcessor.NodeList.size()-1; i++)
//	{
//		//mProcessor.InsertRealConst(i, InNodeCoor[0 + i * 3], InNodeCoor[1 + i * 3], InNodeCoor[2 + i * 3]);
//	}
//}

int CShaftTAN::GenRealConstBySecType(int InSecType, double * InSecTypeData, double * OutRealConst)
{
	if (InSecType==0)
	{
		std::copy(InSecTypeData, InSecTypeData + 4, OutRealConst);
	}
	else if (InSecType == 1)
	{
		double& Diameter = InSecTypeData[0];
		OutRealConst[0] =Pi * pow(Diameter, 4) / 64;
		OutRealConst[1] = OutRealConst[0];
		OutRealConst[2] = OutRealConst[1] * 2;
		OutRealConst[3] = Pi * pow(Diameter / 2, 2);
		//return 1;
	}
	return mCalSecTypeToDataCount[InSecType];
	
}

void CShaftTAN::DefineMat(double InEx, double InPRXY)
{
	mProcessor.InsertMat(1, (std::string)"EX", InEx);
	mProcessor.InsertMat(1, (std::string)"PRXY", InPRXY);
}

void CShaftTAN::CalBeamStruct(int InNodeCount, double * InNodeCoorList, double * InMatEXPRXY,int* InSecTypeList,double* InSecDataList, int InBCCount,int* InBCFreedomIndex, double * InBCValueList)
{
	mProcessor.DeleteDataSpace(mProcessor);
	//CreatDataSpace(InNodeCount, InBCCount);
	DefineNodeList(InNodeCount, InNodeCoorList);
	//for (int i=0;i< InNodeCount; i++)
	//{
	//	mProcessor.InsertNode(i, InNodeCoorList[0 + i * 3], InNodeCoorList[1 + i * 3], InNodeCoorList[2 + i * 3]);
	//}
	mProcessor.InsertElemType(1, 1);
	DefineMat(InMatEXPRXY[0], InMatEXPRXY[1]);
	//给每段生成实常数和单元
	double* SecDataStartIndex = InSecDataList;
	int* pTempNodeNumList = new int[2];
	for (int i=0;i< InNodeCount-1;i++)
	{
		SecDataStartIndex= SecDataStartIndex + GenRealConstBySecType(InSecTypeList[i], InSecDataList, &(mpSecRealConst[0]));
		mProcessor.InsertRealConst(i, 4, &(mpSecRealConst[0]));
		pTempNodeNumList[0] = i;
		pTempNodeNumList[1] = pTempNodeNumList[0] + 1;
		mProcessor.InsertElem(i, 1, 1, i, 2, pTempNodeNumList);
	}
	//插入边界条件
	for (int i=0;i< InBCCount;i++)
	{
		mProcessor.InsertBoundaryCondition(InBCFreedomIndex[2 * i], InBCFreedomIndex[2 * i + 1], InBCValueList[i]);
	}
	mSolver.SolveFEMModel(0,mProcessor);
	//添加边界条件
	delete[] pTempNodeNumList;
}

void CShaftTAN::CalBeamStruct()
{
	mProcessor.DeleteDataSpace(mProcessor);
	//插入节点
	int i = 0;
	std::set<double>::iterator TempSetIteratorBegin = mNodeZCoorList.begin();
	std::set<double>::iterator TempSetIteratorLoop;
	double TempZCoor;
	//std::set<int>::iterator TempSetIteratorBegin;
	for (TempSetIteratorLoop = mNodeZCoorList.begin(); TempSetIteratorLoop != mNodeZCoorList.end(); TempSetIteratorLoop++)
	{		
		TempZCoor = *TempSetIteratorLoop;
		mProcessor.InsertNode(i, TempZCoor *mShaftDirect[0], TempZCoor * mShaftDirect[1], TempZCoor * mShaftDirect[2]);
		i++;

	}
	//插入梁单元类型，欧拉梁
	mProcessor.InsertElemType(1, 1);
	//定义梁
	mProcessor.InsertMat(1, (std::string)"EX", mEX);
	mProcessor.InsertMat(1, (std::string)"PRXY", mPRXY);
	mNodeCount = i;
	i = 0;
	//给每段插入实常数和单元
	int* pTempNodeNumList = new int[2]{ 0,0 };
	for (std::map<double, CTanVector>::iterator j= mRealConstList.begin(); j!= mRealConstList.end(); j++)
	{
		mProcessor.InsertRealConst(i, 4, &(j->second[0]));
		pTempNodeNumList[0] = i;
		pTempNodeNumList[1] = pTempNodeNumList[0] + 1;
		mProcessor.InsertElem(i, 1, 1, i, 2, pTempNodeNumList);
		i++;
	}
	int k = 0;
	//给边界条件赋予节点号
	std::set<double>::iterator it;
	std::set<double>::iterator itBegin;
	std::map < double, tagBCNode*>::iterator MapIt;
	int TempNodeIndex;
	itBegin = mNodeZCoorList.begin();
	for (MapIt = mNodeBCListZCoorToBCObjPtr.begin(); MapIt != mNodeBCListZCoorToBCObjPtr.end(); MapIt++)
	{
		it = mNodeZCoorList.find(MapIt->first);
		TempNodeIndex = std::distance(itBegin, it );
		MapIt->second->NodeNum = TempNodeIndex;
		mNodeBCListNumToBCObjPtr[TempNodeIndex] = MapIt->second;
	}
	for (std::map<int, tagBCNode*>::iterator it = mNodeBCListNumToBCObjPtr.begin(); it != mNodeBCListNumToBCObjPtr.end(); it++)
	{
		for (k = 0; k < it->second->FreedomIndexList.size(); k++)
		{
			mProcessor.InsertBoundaryCondition(it->first, it->second->FreedomIndexList[k], it->second->FreedomValueList[k]);
		}
		
	}
	mSolver.SolveFEMModel(0, mProcessor);
	delete[] pTempNodeNumList;
}

void CShaftTAN::GetForceAndForceLocation()
{
	int tmpNodeSize = mProcessor.NodeList.size();
	std::vector<double> InBendingNormalStress(2 * tmpNodeSize, 0);
	std::vector<double> InShearStress(2 * tmpNodeSize, 0);
	//取出Z作用点坐标
	std::vector<double> tmpNodeZCoorList(tmpNodeSize, 0);
	int j = 0;
	for (auto value : mNodeZCoorList)
	{
		tmpNodeZCoorList[j] = value;
		j++;
	}
	
	// -----------------------------
	// 右侧截断（从第一个节点到当前节点i，包含i节点）
	// -----------------------------
	std::vector<CTanVector> NodeForce_Right(tmpNodeSize, CTanVector(3));
	std::vector<CTanVector> NodeMoment_Right(tmpNodeSize, CTanVector(3));

	for (int i = 0; i < tmpNodeSize; ++i)//节点数
	{
		// 先初始化
		CTanVector sumForce(3);   // 当前节点累计合力
		CTanVector sumMoment(3);  // 当前节点累计力矩

		sumForce[0] = sumForce[1] = sumForce[2] = 0;
		sumMoment[0] = sumMoment[1] = sumMoment[2] = 0;

		for (int j = 0; j <= i; ++j)
		{
			// 节点j的受力
			CTanVector force(3);
			force[0] = mSolver.TotalFValue[6 * j + 0];
			force[1] = mSolver.TotalFValue[6 * j + 1];
			force[2] = mSolver.TotalFValue[6 * j + 2];
			// 力臂（i节点到j节点的距离，只在Z方向）
			CTanVector lever(3);
			lever[0] = 0;
			lever[1] = 0;
			lever[2] = -(tmpNodeZCoorList[i] - tmpNodeZCoorList[j]);

			// 力矩 = 力臂 × 力
			CTanVector moment(3);
			lever.CrossProduct(force, moment);

			// 累加力
			sumForce[0] += force[0];
			sumForce[1] += force[1];
			sumForce[2] += force[2];

			// 累加力矩
			sumMoment[0] += moment[0];
			sumMoment[1] += moment[1];
			sumMoment[2] += moment[2];
			// 如果有外加力矩（如Mz），累加到z方向
			sumMoment[0] += mSolver.TotalFValue[6 * j + 3];
			sumMoment[1] += mSolver.TotalFValue[6 * j + 4];
			sumMoment[2] += mSolver.TotalFValue[6 * j + 5];
		}
		NodeForce_Right[i] = sumForce;
		NodeMoment_Right[i] = sumMoment;
	}

	// -----------------------------
	// 左侧截断（不包含节点 i，从第一个节点到节点i-1）
	// -----------------------------
	std::vector<CTanVector> NodeForce_Left(tmpNodeSize, CTanVector(3));
	std::vector<CTanVector> NodeMoment_Left(tmpNodeSize, CTanVector(3));

	for (int i = 0; i < tmpNodeSize; ++i)
	{
		CTanVector sumForce(3), sumMoment(3);
		sumForce[0] = sumForce[1] = sumForce[2] = 0;
		sumMoment[0] = sumMoment[1] = sumMoment[2] = 0;

		// 只累加到i-1节点（不包含i）
		for (int j = 0; j < i; ++j)
		{
			// 节点 j 受力
			CTanVector force(3);
			force[0] = mSolver.TotalFValue[6 * j + 0];
			force[1] = mSolver.TotalFValue[6 * j + 1];
			force[2] = mSolver.TotalFValue[6 * j + 2];

			// 力臂（当前节点i到j节点的距离）
			CTanVector lever(3);
			lever[0] = 0;
			lever[1] = 0;
			lever[2] = -(tmpNodeZCoorList[i] - tmpNodeZCoorList[j]);

			CTanVector moment(3);
			lever.CrossProduct(force, moment);

			// 累加力
			sumForce[0] += force[0];
			sumForce[1] += force[1];
			sumForce[2] += force[2];

			// 累加力矩
			sumMoment[0] += moment[0];
			sumMoment[1] += moment[1];
			sumMoment[2] += moment[2];

			// 外加力矩
			sumMoment[0] += mSolver.TotalFValue[6 * j + 3];
			sumMoment[1] += mSolver.TotalFValue[6 * j + 4];
			sumMoment[2] += mSolver.TotalFValue[6 * j + 5];
		}
		NodeForce_Left[i] = sumForce;
		NodeMoment_Left[i] = sumMoment;
	}

	// 用累计的合力和合力矩计算应力
	for (int i = 0; i < tmpNodeSize - 1; ++i)
	{
		CTanVector& TotalForce_Left = NodeForce_Left[i];   // 当前节点左侧累计合力
		CTanVector& TotalMoment_Left = NodeMoment_Left[i]; // 当前节点左侧累计力矩
		CTanVector& TotalForce_Right = NodeForce_Right[i];   // 当前节点右侧累计合力
		CTanVector& TotalMoment_Right = NodeMoment_Right[i]; // 当前节点右侧累计力矩

		double tmpI = mProcessor.RealConsList[i]->ParaList[0];//惯性矩
		double tmpJ = mProcessor.RealConsList[i]->ParaList[2];//极惯性矩
		double tmpA = mProcessor.RealConsList[i]->ParaList[3];//截面面积
		double tmpR = sqrt(tmpA / PI);
		double tmpWb = tmpI / tmpR; //抗弯截面系数
		double tmpWt = tmpJ / tmpR;//抗扭截面系数

		double sigmaA_Left = TotalForce_Left[2] / tmpA;//轴向力Fz产生的轴向正应力
		double sigmaB_Left = sqrt(TotalMoment_Left[0] * TotalMoment_Left[0] + TotalMoment_Left[1] * TotalMoment_Left[1]) / tmpWb; // 弯曲正应力（Mx/My）
		double tauT_Left = TotalMoment_Left[2] / tmpWt; // 扭矩Mz产生的剪应力

		double sigmaA_Right = TotalForce_Right[2] / tmpA;//轴向力Fz产生的轴向正应力
		double sigmaB_Right = sqrt(TotalMoment_Right[0] * TotalMoment_Right[0] + TotalMoment_Right[1] * TotalMoment_Right[1]) / tmpWb; // 弯曲正应力（Mx/My）
		double tauT_Right = TotalMoment_Right[2] / tmpWt; // 扭矩Mz产生的剪应力

		//最后应力还要÷1000，才是对应的单位！
		sigmaB_Left = sigmaB_Left / 1000;
		tauT_Left = tauT_Left / 1000;

		sigmaB_Right = sigmaB_Right / 1000;
		tauT_Right = tauT_Right / 1000;

		InBendingNormalStress[i] = sigmaB_Left;
		InBendingNormalStress[i + tmpNodeSize] = sigmaB_Right;
		InShearStress[i] = tauT_Left;
		InShearStress[i + tmpNodeSize] = tauT_Right;
	}
	BendingNormalStress = InBendingNormalStress;
	ShearStress = InShearStress;	
}
