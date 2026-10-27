#include "TANPinionCycloidGear.h"

// 接触强度计算使用的对象成员别名，与原实现保持一致。
#define _CycloidGearPara \
int&zb = mGear.Z;\
int&zg = mPinionList[0].Z;\
double& ee = mOtherCoef.ee;\
double& rz = mGear.rz;\
double& Rz = mGear.Rzf;\
double& K1 = mOtherCoef.K1;\
double& K2 = mOtherCoef.K2;\
double& T_b = mGear.Torque;\

// 本文件集中实现扭矩分配、接触力、曲率和最大接触应力计算。
// 根据摆线轮数量分配输入端总扭矩，并同步计算针齿壳承受的总扭矩。
void CPinionCycloidGearTAN::CalPinAndGearTorque(double InPinionTotalTorque)
{
	for (int i = 0; i < Ng; i++)
	{
		mPinionList[i].Torque = abs(mPinionList[i].BB / mOtherCoef.BB * InPinionTotalTorque);
		mPinionList[i].TorqueDirect = NSTanBasiceFunction::Fsign(InPinionTotalTorque);

	}	
	mGear.TorqueDirect = -mPinionList[0].TorqueDirect;
	mGear.Torque = mGear.rm / mPinionList[0].rm * InPinionTotalTorque;
	mPinionTotalTorque = InPinionTotalTorque;
	mGearTotalTorque = mGear.Torque * mGear.TorqueDirect;
}

// 使用各摆线轮独立的弹性模量初始化接触强度计算参数。
void CPinionCycloidGearTAN::DefineStressCalPara(double InPinionTotalTorque, double* InPinionModuleList, double InGearModule, double InKp)
{
	mPinionTotalTorque = InPinionTotalTorque;
	mGear.EModule = InGearModule;
	for (int i = 0;i < Ng;i++)
	{
		mPinionList[i].EModule = InPinionModuleList[i];
	}
	Kp = InKp;

}

// 使用统一的摆线轮弹性模量初始化接触强度计算参数。
void CPinionCycloidGearTAN::DefineStressCalPara(double InPinionTotalTorque, double InPinionModule, double InGearModule, double InKp)
{
	mPinionTotalTorque = InPinionTotalTorque;
	mGear.EModule = InGearModule;
	for (int i = 0;i < Ng;i++)
	{
		mPinionList[i].EModule = InPinionModule;
	}
	Kp = InKp;

}

// 根据单片摆线轮扭矩和几何参数计算最大接触力。
double CPinionCycloidGearTAN::CalContForceMax(int& zg,//摆线轮齿数
	double& Rz,//针轮中心圆半径，分度圆半径？
	double InPinionTorque,//总传递力矩
	double& K1,//短幅系数
	double& Kp//均载系数
)
{
	double ThisPinionTorque = Kp * InPinionTorque;
	return 4* ThisPinionTorque /(K1*zg*Rz);
}

double CPinionCycloidGearTAN::CalCoefS(double & PinThetab, double & K1, double & K2)
{
	return 1.0 + pow(K1, 2.0) - 2 * K1*cos(PinThetab);
}

double CPinionCycloidGearTAN::CalOneContForce( double & PinThetab, double & SS, double& FMax)
{
	return FMax * sin(PinThetab) / pow(SS, 0.5);
}

double CPinionCycloidGearTAN::CalCoefT(int & Inzb, double & InPinThetab, double & InK1)
{
	return InK1 * (1 + Inzb)*cos(InPinThetab) - (1 + Inzb * pow(InK1, 2));
}

double CPinionCycloidGearTAN::CalCycloidCurveCurvatureRadius(double & InRz, double & InS, double & InT)
{
	return InRz * pow(InS, 1.5) / InT;
}

double CPinionCycloidGearTAN::CalRelativeCurvature(double & Inrz, double & InRho0)
{
	return 1 / Inrz - 1 / (Inrz + InRho0);
}

double CPinionCycloidGearTAN::CalEquivalentElasticModules(double & InE1, double & InE2)
{
	return 2 * InE1*InE2 / (InE1 + InE2);
}

double CPinionCycloidGearTAN::CalNominalContStressBasic(double & InToothWidth, double & InRelativeCurvature, double & InEd, double & InFi)
{
	return 0.418*sqrt(InFi*InEd *InRelativeCurvature / InToothWidth);
}

// 组合接触力、曲率和等效弹性模量，计算指定转角位置的名义接触应力。
double CPinionCycloidGearTAN::CalOneLocationNominalContStress(int& InZb, double & InToothWidth,double& Inrz, double& InRzf,  double & InK1, double& InK2, double & InPinThetab, double & InEd, double & InFMax)
{
	double TempSS = CalCoefS(InPinThetab, InK1, InK2);
	double TempTT = CalCoefT(InZb, InPinThetab, InK1);
	double TempRh0 = CalCycloidCurveCurvatureRadius(InRzf, TempSS, TempTT);
	double TempRelativeCurvature = CalRelativeCurvature(Inrz, TempRh0);
	double TempFi = CalOneContForce(InPinThetab, TempSS, InFMax);
	return CalNominalContStressBasic(InToothWidth, TempRelativeCurvature, InEd, TempFi);
}

// 将接触应力取负值，适配现有的一维最小值搜索接口。
double CPinionCycloidGearTAN::AuxiliaryForMinNegativeContStress(double* InPinThetab,
	void** InOtherCoef)
{
	int* ZbPtr = static_cast<int*>(InOtherCoef[0]);
	double* BPtr = static_cast<double*>(InOtherCoef[1]);
	double* EdPtr = static_cast<double*>(InOtherCoef[2]);
	double* RzfPtr = static_cast<double*>(InOtherCoef[3]);
	double* rzPtr = static_cast<double*>(InOtherCoef[4]);
	double* K1Ptr = static_cast<double*>(InOtherCoef[5]);
	double* K2Ptr = static_cast<double*>(InOtherCoef[6]);
	double* FmaxPtr = static_cast<double*>(InOtherCoef[7]);
	double SCoef = CalCoefS(*InPinThetab, *K1Ptr, *K2Ptr);
	double TCoef = CalCoefT(*ZbPtr, *InPinThetab, *K1Ptr);
	double Rho0 = (*RzfPtr)*pow(SCoef, 1.5) / TCoef;
	double RelativeCurvature = CalRelativeCurvature(*rzPtr, Rho0);
	double Fi = CalOneContForce(*InPinThetab, SCoef, *FmaxPtr);
	double ContStress = CalNominalContStressBasic(*BPtr, RelativeCurvature, *EdPtr, Fi);
	//double RelativeCurvature=
	//std::cout << ContStress<<std::endl;
	return -ContStress;
}



// 遍历各摆线轮并搜索最大接触应力，同时记录对应轮号和转角。
double CPinionCycloidGearTAN::FindMaxContStress(
	CPinionCycloidGearTAN& IOGearObj)
{
	IOGearObj.CalPinAndGearTorque(IOGearObj.mPinionTotalTorque);
	void** pTempOptCoef;
	pTempOptCoef = new void*[8];
	pTempOptCoef[0] = static_cast<void*>(&IOGearObj.mGear.Z);
	
	pTempOptCoef[3] = static_cast<void*>(&IOGearObj.mGear.Rzf);
	pTempOptCoef[4] = static_cast<void*>(&IOGearObj.mGear.rz);
	pTempOptCoef[5] = static_cast<void*>(&IOGearObj.mOtherCoef.K1);
	pTempOptCoef[6] = static_cast<void*>(&IOGearObj.mOtherCoef.K2);
	double InTotalPinionTorque = 0;
	for (int i = 0;i < IOGearObj.Ng;i++)
	{
		InTotalPinionTorque = InTotalPinionTorque + IOGearObj.mPinionList[i].TorqueDirect*IOGearObj.mPinionList[i].Torque;
	}

	void* TEMPFUNPTR;
	double* pTempPinThetab = new double[1];
	int TempPinThetaCount = 1;
	int TempOtherCoefCount = 8;
	double TempAng;
	double TempMaxStress = 0.0;
	double TempMaxStressStorage = 0;
	double TempAngIncr = 0.05;
	double MaxStress = 0;
	double MaxStressPinTheta = 0;
	int MaxStressPinNum = 0;
	for (int i = 0;i < IOGearObj.Ng;i++)
	{
		TempAng=0.001;
		pTempOptCoef[1] = static_cast<void*>(&IOGearObj.mPinionList[i].BB);
		IOGearObj.mPinionList[i].FMax = CalContForceMax(IOGearObj.mPinionList[i].Z,
			IOGearObj.mGear.Rzf,
			IOGearObj.mPinionList[i].Torque,//单个小轮传递力矩
			IOGearObj.mOtherCoef.K1,
			IOGearObj.Kp);
		pTempOptCoef[7] = static_cast<void*>(&IOGearObj.mPinionList[i].FMax);
		IOGearObj.mPinionList[i].EquvalentEModule = CalEquivalentElasticModules(IOGearObj.mPinionList[i].EModule, IOGearObj.mGear.EModule);
		pTempOptCoef[2] = static_cast<void*>(&IOGearObj.mPinionList[i].EquvalentEModule);
		TEMPFUNPTR = static_cast<void*>(&CPinionCycloidGearTAN::AuxiliaryForMinNegativeContStress);
		//先等距搜索对应最大应力点处的值
		do
		{
			TempMaxStressStorage = AuxiliaryForMinNegativeContStress(&TempAng, pTempOptCoef);
			if (TempMaxStressStorage < TempMaxStress)
			{
				TempMaxStress = TempMaxStressStorage;
				pTempPinThetab[0] = TempAng;
			}
			TempAng = TempAng + TempAngIncr;
		} while (TempAng < Pi);
		IOGearObj.mPinionList[i].MaxStress = -TANFINDMINVALUEWITHOUTCONSTRAINFORC(TEMPFUNPTR, &TempPinThetaCount, pTempPinThetab, &TempOtherCoefCount, pTempOptCoef);
		IOGearObj.mPinionList[i].MaxStressTheta = pTempPinThetab[0];

		if (!std::isnan(IOGearObj.mPinionList[i].MaxStress))
		{
			if (MaxStress < mPinionList[i].MaxStress)
			{
				MaxStress = mPinionList[i].MaxStress;
				MaxStressPinTheta = mPinionList[i].MaxStressTheta;
				MaxStressPinNum = i;
			}
		}
	}
	if (MaxStress > DefaultTolerance)
	{
		IOGearObj.mMaxContStress = MaxStress;
		IOGearObj.mMaxContStressPinTheta = MaxStressPinTheta;
		IOGearObj.mMaxContStressPinNum = MaxStressPinNum;
	}
	else
	{
		IOGearObj.mMaxContStress = 1e10;
	}
	delete[] pTempOptCoef;
	delete[] pTempPinThetab;
	return IOGearObj.mMaxContStress;
}

// 便捷入口：先设置强度参数，再执行最大接触应力搜索。
double CPinionCycloidGearTAN::FindMaxContStress(double InTotalPinionTorque,
	double* InPinionModuleList,
	double InGearModule,
	double InKp)
{
	DefineStressCalPara(InTotalPinionTorque, InPinionModuleList, InGearModule, InKp);
	return FindMaxContStress(*this);
}

// 使用对象当前参数执行最大接触应力搜索。
double CPinionCycloidGearTAN::FindMaxContStress()
{
	return FindMaxContStress(*this);
}
