#include "TANInvoluteGear.h"

// 集中实现渐开线齿轮参数推荐与变位优化，保留原有计算接口。

// 模块职责：调整变位系数，使两轮的弯曲或接触安全系数趋于一致。
void CInvoluteGearTAN::OptimizeProfileShiftToEqualLife(int OptTargetNotation)
{
	//总和变位系数
	CalProfileShiftSumByCenterDistance();
	//mPinion.x = mPinion.CalMinProfileShift();//小轮的最小高变位系数
	//mGear.CalMinProfileShift();
	//mGear.x = tmpX_Sum - mPinion.x;
	int OtherCoefCount = 1;
	void** pTempOtherCoefPtr = new void* [1];
	pTempOtherCoefPtr[0] = static_cast<void*> (this);
	void* SolveFunPtr = 0;
	double* tmpXRange = new double[4];//前两个用于X的二分求解，后两个存储X极值对应的求解函数值，以便判断是否异号，异号则需要迭代求解，否则直接取较小的函数值作为结果
	tmpXRange[0] = mPinion.CalMinProfileShift();
	tmpXRange[1] = mPinion.CalMaxProfileShift();
	bool IsNeedIter = 0;//是否需要迭代
	if (OptTargetNotation == ProfileShiftOptByBendStressNotation)
	{	tmpXRange[2] = AuxiliaryFunctionForOptimizeProfileShiftByBending(&(tmpXRange[0]), pTempOtherCoefPtr);
		tmpXRange[3] = AuxiliaryFunctionForOptimizeProfileShiftByBending(&(tmpXRange[1]), pTempOtherCoefPtr);
		if (tmpXRange[2] * tmpXRange[3] > 0)
		{
			if (abs(tmpXRange[2]) < abs(tmpXRange[3]))
			{
				mPinion.x = tmpXRange[0];
			}
			else
			{
				mPinion.x = tmpXRange[1];
			}
		}
		else//表明需要迭代求解
		{
			IsNeedIter = 1;
			SolveFunPtr = static_cast<void*>(&AuxiliaryFunctionForOptimizeProfileShiftByBending);
		}
	}
	else
	{
		tmpXRange[2] = AuxiliaryFunctionForOptimizeProfileShiftByContact(&(tmpXRange[0]), pTempOtherCoefPtr);
		tmpXRange[3] = AuxiliaryFunctionForOptimizeProfileShiftByContact(&(tmpXRange[1]), pTempOtherCoefPtr);
		if (tmpXRange[2] * tmpXRange[3] > 0)
		{
			if (abs(tmpXRange[2]) < abs(tmpXRange[3]))
			{
				mPinion.x = tmpXRange[0];
			}
			else
			{
				mPinion.x = tmpXRange[1];
			}
		}
		else//表明需要迭代求解
		{
			IsNeedIter = 1;
			SolveFunPtr = static_cast<void*>(&AuxiliaryFunctionForOptimizeProfileShiftByContact);
		}
	}
	if (IsNeedIter)
	{
		mPinion.x = TANFINDROOTBYBISECTIONFORC(SolveFunPtr, tmpXRange, &OtherCoefCount, pTempOtherCoefPtr);
	}
	mGear.x = x_Sum - mPinion.x;
	//double* tmpXlist = new double[20];
	//std::vector < double > tmpResult(20);
	//for (int i = 0;i < 20;i++)
	//{
	//	tmpXlist[i] = 0.05 * i;
	//	tmpResult[i]=AuxiliaryFunctionForOptimizeProfileShift(&(tmpXlist[i]), pTempOtherCoefPtr);

	//}

	delete[] pTempOtherCoefPtr;
	delete[] tmpXRange;

}
// 模块职责：按接触强度要求搜索模数及变位组合，推荐主要几何参数。
void CInvoluteGearTAN::RecommondMainGeometryParaByContact(double InU, double InK, double InT1, double InPinionRPM, double InDesignLife, double InsigmaHLim, int InMeshSign, int InN_p)
{
	double tmpUSum1 = 0;
	//u = InU;
	tmpUSum1 = InU + InMeshSign;

	fy_a_test = fy_d_test / (0.5 * tmpUSum1);
	sigma_Hlim = InsigmaHLim;
	sigma_HP_test = sigma_Hlim * 0.9;
	K_Sum_test = InK;
	//初定中心距离
	a_w_min_test = A_a_test * tmpUSum1 * pow(InK * InT1 / (fy_a_test * InU * pow(sigma_HP_test, 2)), 1 / 3.0);
	d_1_min_test = pow(2, 1 / 3.0) * A_d_test * a_w_min_test / (tmpUSum1 * A_a_test) ;
	double tmpa_w = NSMechanismFunction::ReturnPriorityNumberByLastByRoundUp(a_w_min_test, PriorityLastNumberStructType);
	//自定义推荐逻辑,主要为确定合适的mn
	double tmpmn_Test = 0.03 * tmpa_w;//推荐模数
	double* tmpMnList = &(NSInvoluteStaticArrary::GearStandardModule[14]);//小于1的模数不考虑
	int tmpMnCount = 36;
	int tmpMnBaseIndex = NSTanBasiceFunction::FindValueShortestIndexInDoubleList(tmpmn_Test, tmpMnCount, tmpMnList);
	//生成推荐的mn索引序列
	int tmpSearMnCount = 3;//总共搜寻多少个mn,需要为奇数
	std::vector<int> tmpTestMnIndexVectorList;
	NSTanBasiceFunction::GenerateArithmeticSequenceContainInputValueInRange(tmpMnBaseIndex, 1, tmpSearMnCount, 0, tmpMnCount-1, tmpTestMnIndexVectorList);
	//CTanVector tmpTestMnVectorList(tmpSearMnCount);
	CTanVector tmpResult(tmpSearMnCount);
	double tmpOptS = 0;//优化的安全系数
	double tmpOptMn = 0;
	double tmpCurrentS = 0;//当前安全系数
	double tmpOptPinionX = 0;
	double tmpOptGearX = 0;
	double tmpOptXSum = 0;
	//std::vector<double> tmpTestMnVectorList(5);
	//std::vector<double> tmpTestMnVectorList(5);
	for (int i = 0; i < tmpSearMnCount; i++)
	{
		OptimizeGearTooth(InU, tmpa_w, tmpMnList[tmpMnBaseIndex + i]);
		SetGeometryParaByMnAndDefaultPara(tmpMnList[tmpMnBaseIndex + i], tmpa_w, InN_p);
		DefineLoadPara(InT1, InPinionRPM, InDesignLife, 0, 0, InN_p);
		//以接触应力为基准进行判断
		OptimizeProfileShiftToEqualLife(ProfileShiftOptByContStressNotation);
		tmpCurrentS = min(mPinion.S_H, mGear.S_H);
		std::cout << mPinion.sigma_H << std::endl;
		std::cout << mGear.sigma_H << std::endl;
		//搜寻解，使安全系数最大
		if (tmpCurrentS > tmpOptS)
		{
			tmpOptS = tmpCurrentS;
			tmpOptMn = tmpMnList[tmpMnBaseIndex + i];
			tmpOptPinionX = mPinion.x;
			tmpOptGearX = mGear.x;
			tmpOptXSum = x_Sum;
		}

		//tmpTestMnVectorList[i] = tmpMnList[tmpTestMnIndexVectorList[i]];
	}
	SetGeometryParaByMnAndDefaultPara(tmpOptMn, tmpa_w, InN_p);
	mPinion.x = tmpOptPinionX;
	mGear.x = tmpOptGearX;
	x_Sum = tmpOptXSum;
	//mn = tmpMnList[tmpMnBaseIndex+2];


	//mPinion.Z = round(2 * tmpa_w / (mn * (1 + u)));
	//mGear.Z = round(u * mPinion.Z);
	//if (mPinion.Z < 15)
	//{
	//	alpha_n = 25 * Degree;
	//}
	//else
	//{
	//	alpha_n = 20 * Degree;
	//}
	//mPinion.b_Design = fy_a_test * tmpa_w;
	//mGear.b_Design = fy_a_test * tmpa_w;
	//a_w = tmpa_w;
	//mPinion.X_E = 0.1795;
	//mGear.X_E = -0.2555;
	//mGear.b_Design = 12.4;
	//n_P = 3;
	//mPinion.Z = 10;
	//mGear.Z = 38;
	//alpha_n = 25 * Degree;
	//mn = 1.75;
	//mPinion.b_Design = 7;
	//mGear.b_Design = 12.4;
	//a_w = 42;
	//n_P = 3;
	//mPinion.Rhf_fPx = 0.25;
	//mGear.Rhf_fPx = 0.25;
	//mPinion.X_E = 0.1795;
	//mGear.X_E = -0.2555;
	//n_P = 3;
	//mPinion.N_m = n_P;
	//std::cout << mPinion.x << std::endl;



	InterfaceCalGeometryAndStrength();
	//std::cout << mPinion.x << std::endl;
	//std::cout << mGear.x << std::endl;
	//std::cout << mPinion.sigma_F << std::endl;
	//std::cout << mGear.sigma_F << std::endl;
	//std::cout << mPinion.S_F << std::endl;
	//std::cout << mGear.S_F << std::endl;
	//std::cout << "mPinion.S_H" << std::endl;
	//std::cout << mPinion.S_H << std::endl;
	//std::cout << mGear.S_H << std::endl;


	//std::cout << mPinion.sigma_H << std::endl;
	//std::cout << mGear.sigma_H << std::endl;


	//double tmpmn = tmpMnList[tmpMnBaseIndex];
	//int tmpz1 = round(2 * tmpa_w / (tmpmn * (1 + u)));
	//int tmpz2 = round(u * tmpz1);
	//u = double(tmpz2)/ tmpz1;
	//double tmpb = fy_a_test * tmpa_w;

}
// 模块职责：按模数、中心距及默认规则设置初始齿轮几何参数。
void CInvoluteGearTAN::SetGeometryParaByMnAndDefaultPara(double InMn, double InA_w, int InN_p)
{
	mn = InMn;
	a_w = InA_w;
	mPinion.Z = round(2 * InA_w / (mn * (1 + u)));
	mGear.Z = round(u * mPinion.Z);
	if (mPinion.Z < 15)
	{
		alpha_n = 25 * Degree;
	}
	else
	{
		alpha_n = 20 * Degree;
	}
	mPinion.b_Design = fy_a_test * InA_w;
	mGear.b_Design = fy_a_test * InA_w;
	a_w = InA_w;
	n_P = InN_p;
	CalGearPairGeometryZJ();
	//OptimizeProfileShiftToEqualLife();
	//mPinion.x = mPinion.CalMinProfileShift();
	//mGear.x = x_Sum - mPinion.x;

}
// 模块职责：按传动比推荐齿数组合，并按选项调整公约数关系。
void CInvoluteGearTAN::OptimizeGearTooth(double InTranRatio, double InA_w, double InMn, int InToothNumOptNotation)
{
	//齿数的理论公式
	double tmpZ1 = 2 * InA_w / (InMn * (1 + InTranRatio));
	//double tmpZ2 = tmpZ1 * InTranRatio;
	mPinion.Z = round(tmpZ1);
	mGear.Z = round(InTranRatio * tmpZ1);
	if (InToothNumOptNotation == ToothNumOptNotationWithGCD)
	{
		//找互质数
		mGear.Z = NSTanBasiceFunction::ReturnShortedGCDNum(mPinion.Z, mGear.Z);
	}
	u = double(mGear.Z) / mPinion.Z;
	//mGear.Z = InTranRatio*
}
// 模块职责：返回两轮弯曲安全系数之差，供变位优化求根使用。
double CInvoluteGearTAN::AuxiliaryFunctionForOptimizeProfileShiftByBending(double* InPinionx, void** InGearObj)
{
	CInvoluteGearTAN* tmpGearPairObj = static_cast<CInvoluteGearTAN*> (InGearObj[0]);
	tmpGearPairObj->mPinion.x = *InPinionx;
	tmpGearPairObj->mGear.x = tmpGearPairObj->x_Sum - tmpGearPairObj->mPinion.x;
	tmpGearPairObj->CalGearPairGeometry();
	tmpGearPairObj->InterfaceCalGeometryAndStrength();
	//return tmpGearPairObj->mPinion.sigma_F- tmpGearPairObj->mGear.sigma_F;
	return tmpGearPairObj->mPinion.S_F - tmpGearPairObj->mGear.S_F;
}

// 模块职责：返回两轮接触安全系数之差，供变位优化求根使用。
double CInvoluteGearTAN::AuxiliaryFunctionForOptimizeProfileShiftByContact(double* InPinionx, void** InGearObj)
{
	CInvoluteGearTAN* tmpGearPairObj = static_cast<CInvoluteGearTAN*> (InGearObj[0]);
	tmpGearPairObj->mPinion.x = *InPinionx;
	tmpGearPairObj->mGear.x = tmpGearPairObj->x_Sum - tmpGearPairObj->mPinion.x;
	tmpGearPairObj->CalGearPairGeometry();
	tmpGearPairObj->InterfaceCalGeometryAndStrength();
	return tmpGearPairObj->mPinion.S_H - tmpGearPairObj->mGear.S_H;;
}
