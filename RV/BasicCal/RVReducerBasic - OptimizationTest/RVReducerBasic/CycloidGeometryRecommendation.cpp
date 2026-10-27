#include "TANPinionCycloidGear.h"
#include <iostream>

// 本文件集中实现摆线轮主要几何参数的推荐与优化求解。

// 根据目标接触应力搜索并圆整摆线轮主要几何参数。
void CPinionCycloidGearTAN::RecommondMainGeometryParaByContact(double InTargetStress,double InPinionTotalTorque, int InGearToothNum, int InNg)
{
	//构建基本方程模型
	double tmpTargetStress = InTargetStress;//末尾多减去了1以保证应力值低于目标应力
	mGear.Z = InGearToothNum;
	if (Ng != InNg)
	{
		Ng = InNg;
		std::vector<tagPinion>(InNg).swap(mPinionList);
	}
	for (int i = 0; i < Ng; i++)
	{
		mPinionList[i].Z = mGear.Z - 1;
		mPinionList[i].AssembleFaceType = AssembleLeftBaseNotation;
	}
	mGear.AssembleFaceType = AssembleLeftBaseNotation;
	DefineStressCalPara(InPinionTotalTorque, 2.06e8, 2.06e8, 1.0);
	//设置方程输入参数，并求解，方程连续
	double* ptmpRzfForBisection = new double[6];//0-1二分初值，2-3二分后续值，4- 牛顿迭代初值，5-牛顿迭代初值对应结果值，6-当前结果
	ptmpRzfForBisection[0] = 1.0;
	ptmpRzfForBisection[1] = 1000.0;
	//Rzf的初定值
	double tmpRzf = 21 * cbrt(abs(InPinionTotalTorque));
	//double tmpRzf = 157.0 / 2;
	//tmpRzf = 10;
	int tmpOtherCoefCount = 3;
	void** pTempOtherCoefPtr = new void* [tmpOtherCoefCount];
	pTempOtherCoefPtr[0] = static_cast<void*> (this);
	pTempOtherCoefPtr[1] = static_cast<void*> (&mPinionTotalTorque);
	pTempOtherCoefPtr[2] = static_cast<void*> (&tmpTargetStress);
	void* tmpSolveFunPtr = static_cast<void*>(&AuxiliaryFunctionForOptimizeGearRadius);
	//搜寻2分区间,如果搜寻不到就确定牛顿迭代初值
	ptmpRzfForBisection[0] = tmpRzf;
	ptmpRzfForBisection[2] = AuxiliaryFunctionForOptimizeGearRadius(&tmpRzf, pTempOtherCoefPtr);
	ptmpRzfForBisection[4] = ptmpRzfForBisection[0];
	ptmpRzfForBisection[5] = abs(ptmpRzfForBisection[2]);
	//增加步长，每次增加步长为按1+a*tmpRzf的公式
	double tmpRzfIncr = 1 + 0.05 * tmpRzf;
	bool IsBisectionRangeSearched = 0;
	if (ptmpRzfForBisection[2] > 0) //要增大rzf
	{
		for (int i = 0; i<200; i++)
		{
			tmpRzf = tmpRzf + tmpRzfIncr;
			ptmpRzfForBisection[3] = AuxiliaryFunctionForOptimizeGearRadius(&tmpRzf, pTempOtherCoefPtr);
			if (ptmpRzfForBisection[5] > abs(ptmpRzfForBisection[3]))
			{
				ptmpRzfForBisection[4] = tmpRzf;
				ptmpRzfForBisection[5] = abs(ptmpRzfForBisection[3]);
			}
			if (ptmpRzfForBisection[3] < 0)
			{
				ptmpRzfForBisection[1] = tmpRzf;
				IsBisectionRangeSearched = 1;
				break;
			}
			else
			{
				ptmpRzfForBisection[0] = tmpRzf;
				ptmpRzfForBisection[2] = ptmpRzfForBisection[3];
			}
		}
	}
	else
	{
		for (int i = 0; i < 200; i++)
		{
			tmpRzfIncr = (tmpRzf) / 199;
			tmpRzf = tmpRzf - tmpRzfIncr;
			ptmpRzfForBisection[3] = AuxiliaryFunctionForOptimizeGearRadius(&tmpRzf, pTempOtherCoefPtr);
			if (ptmpRzfForBisection[5] > abs(ptmpRzfForBisection[3]))
			{
				ptmpRzfForBisection[4] = tmpRzf;
				ptmpRzfForBisection[5] = abs(ptmpRzfForBisection[3]);
			}
			if (ptmpRzfForBisection[3] > 0)
			{
				ptmpRzfForBisection[1] = tmpRzf;
				IsBisectionRangeSearched = 1;
				break;
			}
			else
			{
				ptmpRzfForBisection[0] = tmpRzf;
				ptmpRzfForBisection[2] = ptmpRzfForBisection[3];
			}
		}
	}
	//根据前面搜寻结果确定是二分法还是牛顿迭代法求解

	if(IsBisectionRangeSearched)
	{
		mGear.Rzf = TANFINDROOTBYBISECTIONFORC(tmpSolveFunPtr, ptmpRzfForBisection, &tmpOtherCoefCount, pTempOtherCoefPtr);
	}
	else
	{
		mGear.Rzf = TANFINDROOTBYITERATIONFORC(tmpSolveFunPtr, &(ptmpRzfForBisection[4]), &tmpOtherCoefCount, pTempOtherCoefPtr);
	}

	//std::cout << FindMaxContStress() << std::endl;
	//对求解值圆整
	mGear.Rzf = NSMechanismFunction::ReturnPriorityNumberByLastByRoundUp(mGear.Rzf, PriorityLastNumberAsPoint5StructType);
	int tmpPinionCount = mPinionList.size();
	mPinionList[0].ToothWidth = NSMechanismFunction::ReturnPriorityNumberByLastByRoundUp(mPinionList[0].ToothWidth, PriorityLastNumberAsPoint5StructType);
	for (int i = 1;i < tmpPinionCount; i++)
	{
		mPinionList[i].ToothWidth = mPinionList[0].ToothWidth;
		mPinionList[i].AssembleDist = mPinionList[i - 1].AssembleDist + mPinionList[0].ToothWidth;
	}
	mGear.ToothWidth = mPinionList[0].ToothWidth * tmpPinionCount;
	//tmpGearPairObj->mGear.ToothWidth = tmpGearPairObj->mPinionList[0].ToothWidth * tmpPinionCount;
	CalBasicGeometry();
	std::cout << FindMaxContStress() << std::endl;
	delete[] ptmpRzfForBisection;
	delete[] pTempOtherCoefPtr;
}

// 根据针齿数的经验关系推荐针径系数 K2。
double CPinionCycloidGearTAN::RecommandK2ByGearTooth(int InZb)
{
	return -0.0243421* InZb + 3.14211;
}

// 计算给定针轮中心圆半径下的应力偏差，供求根算法调用。
double CPinionCycloidGearTAN::AuxiliaryFunctionForOptimizeGearRadius(double* InRzf, void** InRVPairObj)
{
	CPinionCycloidGearTAN* tmpGearPairObj = static_cast<CPinionCycloidGearTAN*> (InRVPairObj[0]);
	tmpGearPairObj->mGear.Rzf = *InRzf;
	tmpGearPairObj->mGear.ToothWidth = tmpGearPairObj->mGear.WidthCoef * 2 * tmpGearPairObj->mGear.Rzf;
	int tmpPinionCount = tmpGearPairObj->mPinionList.size();
	tmpGearPairObj->mPinionList[0].ToothWidth = tmpGearPairObj->mGear.ToothWidth / tmpPinionCount;

	if (tmpGearPairObj->EnableWidthConstraintCheckBox == 1 && tmpGearPairObj->mPinionList[0].ToothWidth > tmpGearPairObj->targetMaxToothWidth
		&& tmpGearPairObj->targetMaxToothWidth != 0)
	{
		tmpGearPairObj->mPinionList[0].ToothWidth = tmpGearPairObj->targetMaxToothWidth;
	}
	if (tmpGearPairObj->EnableWidthConstraintCheckBox == 1 && tmpGearPairObj->mPinionList[0].ToothWidth < tmpGearPairObj->targetMinToothWidth)
	{
		tmpGearPairObj->mPinionList[0].ToothWidth = tmpGearPairObj->targetMinToothWidth;
	}

	//tmpGearPairObj->mGear.ToothWidth = tmpGearPairObj->mPinionList[0].ToothWidth * tmpPinionCount;
	for (int i = 1;i < tmpPinionCount; i++)
	{
		tmpGearPairObj->mPinionList[i].ToothWidth = tmpGearPairObj->mPinionList[0].ToothWidth;
		tmpGearPairObj->mPinionList[i].AssembleDist = tmpGearPairObj->mPinionList[i - 1].AssembleDist + tmpGearPairObj->mPinionList[0].ToothWidth;
	}
	tmpGearPairObj->mOtherCoef.K1 = 0.65;
	tmpGearPairObj->mOtherCoef.ee = tmpGearPairObj->ChooseRoundEccentricCoef(tmpGearPairObj->mGear.Z, tmpGearPairObj->mGear.Rzf, tmpGearPairObj->mOtherCoef.K1);
	tmpGearPairObj->mOtherCoef.K2 = RecommandK2ByGearTooth(tmpGearPairObj->mGear.Z);
	tmpGearPairObj->mGear.rz = tmpGearPairObj->ChooseRoundPinionToothCoverRadiusCoef(tmpGearPairObj->mGear.Z, tmpGearPairObj->mGear.Rzf, tmpGearPairObj->mOtherCoef.K2);
	tmpGearPairObj->CalBasicGeometry();
	double& tmpPinionTotalTorque= *static_cast<double*> (InRVPairObj[1]);
	tmpGearPairObj->CalPinAndGearTorque(tmpPinionTotalTorque);
	double& tmpTargetStress = *static_cast<double*> (InRVPairObj[2]);
	//tmpGearPairObj->CalK1(tmpGearPairObj->mGear.Z, tmpGearPairObj->mGear.Rzf, tmpGearPairObj->mOtherCoef.ee);
	double tmpResultStress = tmpGearPairObj->FindMaxContStress();
	return tmpGearPairObj->FindMaxContStress() - tmpTargetStress;
}
