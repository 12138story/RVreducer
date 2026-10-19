# SOURCE ENCODING: utf-8
﻿#include "TANPinionCycloidGear.h"
#include<iostream>
//CRVReducerTAN::FCalTransRatio
#define _CycloidGearPara \
int&zb = mGear.Z;\
int&zg = mPinionList[0].Z;\
double& ee = mOtherCoef.ee;\
double& rz = mGear.rz;\
double& Rz = mGear.Rzf;\
double& K1 = mOtherCoef.K1;\
double& K2 = mOtherCoef.K2;\
double& T_b = mGear.Torque;\

tagRVGearBasicTAN::tagRVGearBasicTAN()
{
	ShaftForceVector.Set(6);
}

void tagRVGearBasicTAN::DefineLocationPara(double InDistBetweenAssembleEdgeToShaftLeft, int InAssembleFaceType)
{
	AssembleDist = InDistBetweenAssembleEdgeToShaftLeft;
	AssembleFaceType = InAssembleFaceType;
}

void tagRVGearBasicTAN::DefineLocationPara(double* InData)
{
	AssembleDist = InData[0];
	AssembleFaceType = static_cast<int>(InData[1]);

}

double tagRVGearBasicTAN::CalGearRevoluteSpeed(double InPinionSpeed, double InRotatingArmSpeed, double GearToPinionToothRatio)
{
	return (InPinionSpeed + (-1 + GearToPinionToothRatio)*InRotatingArmSpeed)/ GearToPinionToothRatio;
}

double tagRVGearBasicTAN::CalPinionRevoluteSpeed(double InGearSpeed, double InRotatingArmSpeed, double GearToPinionToothRatio)
{
	return (GearToPinionToothRatio * (InGearSpeed - InRotatingArmSpeed) + InRotatingArmSpeed);
}


void CPinionCycloidGearTAN::CopyCycloidWidthConstraintPara(double* InData)
{	
	targetMinToothWidth = InData[0];
	targetMaxToothWidth = InData[1];
	EnableWidthConstraintCheckBox = InData[2];//默认没有启用候选框
}

CPinionCycloidGearTAN::CPinionCycloidGearTAN()
{
	Clear();

}

void CPinionCycloidGearTAN::Clear()
{
	std::vector<tagPinion>(2).swap(mPinionList);
	Ng = 2;
}

void CPinionCycloidGearTAN::CalBasicGeometry()
{
	_CycloidGearPara
	mGear.rm = ee * zb;//r'b
	mOtherCoef.K1 = mGear.rm / Rz;//K1
	//摆线轮参数计算
	mPinionList[0].rm = ee * zg;//r'g
	mPinionList[0].r = mOtherCoef.ee / mOtherCoef.K1;
	//mPinionList[0].R = Rz * zg / double(mPinionList[0].Z);
	mPinionList[0].R = Rz * zg / zb; //应该是zb
	mPinionList[0].ra = Rz + ee - rz;
	mPinionList[0].rf = Rz - ee - rz;
	mPinionList[0].h = 2 * ee;
	mOtherCoef.K2 = CalK2(zb, rz, Rz);
	mPinionList[0].rmg = Rz;
	
	for (int i = 1;i < Ng;i++)
	{
		mPinionList[i].rm = mPinionList[0].rm;
		mPinionList[i].r = mPinionList[0].r;
		mPinionList[i].R = mPinionList[0].R;
		mPinionList[i].ra = mPinionList[0].ra;
		mPinionList[i].rf = mPinionList[0].rf;
		mPinionList[i].h = mPinionList[0].h;
		mPinionList[i].rmg = mPinionList[0].rmg;

	}
	//计算齿宽
	mOtherCoef.BB = 0;
	for (int i = 0;i < Ng;i++)
	{
		mPinionList[i].BB = CalGearDesignToothWidth(mPinionList[i].AssembleDist, mPinionList[i].AssembleFaceType, mPinionList[i].ToothWidth,
			mGear.AssembleDist, mGear.AssembleFaceType, mGear.ToothWidth);
		mOtherCoef.BB = mOtherCoef.BB + mPinionList[i].BB;
	}
}

void CPinionCycloidGearTAN::CalBasicGeometry(int& zb,//中心轮齿数
	int& zg,//摆线行星轮齿数
	double& rz,//滚针半径
	double& Rz,//摆线轮理论齿廓的平均半径，分度圆半径
	double& ee, //偏心距
	CPinionCycloidGearTAN & OutReducer)
{
	OutReducer.mGear.Z = zb;
	for (int i = 0; i < Ng; i++)
	{
		OutReducer.mPinionList[i].Z = zg;
	}
	OutReducer.mGear.rz = rz;
	OutReducer.mGear.Rzf = Rz;
	OutReducer.mOtherCoef.ee = ee;
	//OutReducer.mGear.Z = zb;
	//OutReducer.mPinion.Z = zg;
	//OutReducer.mOtherCoef.ee = ee;
	//OutReducer.mGear.rz = rz;
	////OutReducer.mPinion.rmg = Rz;
	//OutReducer.mGear.Rzf = Rz;
	OutReducer.CalBasicGeometry();
	//OutReducer.mGear.rm = ee * zb;//r'b
	//OutReducer.mOtherCoef.K1 = OutReducer.mGear.rm/Rz;//K1
	//OutReducer.mPinion.rm = ee * zg;//r'g
	//OutReducer.mPinion.r = OutReducer.mOtherCoef.ee / OutReducer.mOtherCoef.K1;
	//OutReducer.mPinion.R = Rz *zg/double(OutReducer.mPinion.Z);	
	//OutReducer.mPinion.ra = Rz + ee - rz;
	//OutReducer.mPinion.rf = Rz - ee - rz;
	//OutReducer.mPinion.h = 2 * ee;
	//OutReducer.mOtherCoef.K2 = Rz / rz * sin(pi / zb);
}
double CPinionCycloidGearTAN::CalK1(int & zb, double & Rzf, double & ee)
{
	return ee * zb / Rzf;
}
double CPinionCycloidGearTAN::CalK2(int & zb, double & rz, double & Rzf)
{
	return Rzf / rz * sin(Pi / zb);
}
double CPinionCycloidGearTAN::ChooseRoundEccentricCoef(int & zb, double & Rzf, double & IOK1)
{
	double ee = ceil(2 * IOK1 * Rzf / zb) / 2;
	IOK1 = CalK1(zb, Rzf, ee);
	return ee;
	
}

double CPinionCycloidGearTAN::ChooseRoundPinionToothCoverRadiusCoef(int & zb, double & Rzf, double & IOK2)
{
	double rz = ceil(2 * Rzf / IOK2 * sin(Pi / double(zb))) / 2;
	IOK2 = CalK2(zb, rz, Rzf);
	return rz;
}

void CPinionCycloidGearTAN::CalBasicGeometryByRzK1K2WithRound()
{
	CalBasicGeometryByRzK1K2WithRound(mGear.Z, mPinionList[0].Z, mGear.Rzf, mOtherCoef.K1, mOtherCoef.K2, *this);
	////double& K1 = mOtherCoef.K1;
	////double& K2 = mOtherCoef.K2;
	////int& zb = mGear.Z;
	////int& zg = mPinion.Z;	
	//////double& ee = mOtherCoef.ee;
	//////double& rz = mGear.rz;
	////double& Rz = mGear.Rzf;
	//_CycloidGearPara
	//mOtherCoef.ee = ChooseRoundEccentricCoef(mGear.Z, mGear.Rzf, mOtherCoef.K1);
	//mGear.rz = ChooseRoundPinionToothCoverRadiusCoef(mGear.Z, mGear.Rzf, mOtherCoef.K2);
	////double ee = ceil(2 * K1 * Rz / zb) / 2;
	//////double TempK1 = ee * zb / Rz;
	////double rz = ceil(2 * Rz / K2 * sin(pi / double(zb))) / 2;
	//CalBasicGeometry();
	////CalBasicGeometry(zb, zg, rz, Rz, ee, OutReducer);
}
void CPinionCycloidGearTAN::CalBasicGeometryByRzK1K2WithRound(int& zb,//中心轮齿数
	int& zg,//摆线行星轮齿数
	double& Rz,//针轮中心圆半径，分度圆半径
	double& K1,//短幅系数
	double& K2,//针径系数
	CPinionCycloidGearTAN& OutReducer)
{
	OutReducer.mOtherCoef.K1 = K1;
	OutReducer.mOtherCoef.K2 = K2;
	OutReducer.mGear.Rzf = Rz;
	OutReducer.mGear.Z = zb;
	for (int i = 0;i < Ng;i++)
	{
		OutReducer.mPinionList[i].Z = zg;
	}
	
	OutReducer.mOtherCoef.ee= ChooseRoundEccentricCoef(mGear.Z, mGear.Rzf, mOtherCoef.K1);
	OutReducer.mGear.rz = ChooseRoundPinionToothCoverRadiusCoef(mGear.Z, mGear.Rzf, mOtherCoef.K2);
	////double& ee = mOtherCoef.ee;
	//double& rz = mGear.rz;
	//double& Rz = mGear.Rzf;
	//double ee = ceil(2 * K1 * Rz / zb)/2;
	////double TempK1 = ee * zb / Rz;
	//double rz = ceil(2 * Rz / K2 * sin(pi / double(zb))) / 2;
	OutReducer.CalBasicGeometry();
	

}



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

double CPinionCycloidGearTAN::CalOneLocationNominalContStress(int& InZb, double & InToothWidth,double& Inrz, double& InRzf,  double & InK1, double& InK2, double & InPinThetab, double & InEd, double & InFMax)
{
	double TempSS = CalCoefS(InPinThetab, InK1, InK2);
	double TempTT = CalCoefT(InZb, InPinThetab, InK1);
	double TempRh0 = CalCycloidCurveCurvatureRadius(InRzf, TempSS, TempTT);
	double TempRelativeCurvature = CalRelativeCurvature(Inrz, TempRh0);
	double TempFi = CalOneContForce(InPinThetab, TempSS, InFMax);
	return CalNominalContStressBasic(InToothWidth, TempRelativeCurvature, InEd, TempFi);
}

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

double CPinionCycloidGearTAN::FindMaxContStress(double InTotalPinionTorque,
	double* InPinionModuleList,
	double InGearModule,
	double InKp)
{
	DefineStressCalPara(InTotalPinionTorque, InPinionModuleList, InGearModule, InKp);
	return FindMaxContStress(*this);
}

double CPinionCycloidGearTAN::FindMaxContStress()
{
	return FindMaxContStress(*this);
}

double CPinionCycloidGearTAN::CalBearingForce(int InCrankAxleCount,double InK_d,double InShaftAxisOffsetDistance)
{
	_CycloidGearPara

	double K_y;
	double F_torque;
	double MaxBearingF=0;
	for (int i = 0;i < Ng;i++)
	{
		mPinionList[i].F_t = mPinionList[i].Torque * zb / (K1 * Rz * zg) * 1000;
		K_y = CalCoefK_y();
		mPinionList[i].F_r = mPinionList[i].F_t * K_y;
		mPinionList[i].F_Total = mPinionList[i].F_t * sqrt(1 + pow(K_y, 2));
		F_torque = mPinionList[i].Torque / abs(InShaftAxisOffsetDistance) * 1000;
		mPinionList[i].mBearingF_t = InK_d * mPinionList[i].F_t / static_cast<double>(InCrankAxleCount);
		mPinionList[i].mBearingF_r = InK_d * mPinionList[i].F_r / static_cast<double>(InCrankAxleCount);
		mPinionList[i].mBearingF_torque = InK_d * F_torque / static_cast<double>(InCrankAxleCount);
		mPinionList[i].mMaxBearingF = InK_d * (abs(F_torque) + abs(mPinionList[i].F_Total)) / static_cast<double>(InCrankAxleCount);
		if (MaxBearingF < mPinionList[i].mMaxBearingF)
		{
			MaxBearingF = mPinionList[i].mMaxBearingF;
		}
	}
	return MaxBearingF;
}

//void CPinionCycloidGearTAN::CalBearingForceByRotAng(
//	double InRotAng,
//	double InAssembleAng)
//{
//	double Fr;
//	double Ft;
//	double FTorque;
//	CTanVector TempForceDist(3);
//	CTanVector TempTorque(3);
//	for (int i = 0;i < Ng;i++)
//	{
//		Fr = mPinionList[i].mBearingF_r;
//		Ft = mPinionList[i].mBearingF_t;
//		FTorque = mPinionList[i].mBearingF_torque;
//		mPinionList[i].ShaftForceVector[0] = mPinionList[i].TorqueDirect * (Fr * cos(InAssembleAng)) - Ft * sin(InAssembleAng) - FTorque * sin(InRotAng);
//		mPinionList[i].ShaftForceVector[1] = mPinionList[i].TorqueDirect * (Ft * cos(InAssembleAng) + FTorque * cos(InRotAng) - Fr * sin(InAssembleAng));
//		mPinionList[i].ShaftForceVector[2] = 0;
//
//		TempForceDist[0] = mOtherCoef.ee * cos(InAssembleAng);
//		TempForceDist[1] = mOtherCoef.ee * sin(InAssembleAng);
//		TempForceDist[2] = 0;
//		TempForceDist.CrossProduct(mPinionList[i].ShaftForceVector, TempTorque);
//		std::copy(TempTorque.begin(), TempTorque.end(), &(mPinionList[i].ShaftForceVector[3]));
//
//	}
//
//}

void CPinionCycloidGearTAN::CalShaftBearingForceByRotAng(
	int InPinionIndex,
	double InRotAng,
	double InAssembleAng)
{
	double Fr;
	double Ft;
	double FTorque;
	CTanVector TempForceDist(3);
	CTanVector TempTorque(3);
	int i = InPinionIndex;
	Fr = mPinionList[i].mBearingF_r;
	Ft = mPinionList[i].mBearingF_t;
	FTorque = mPinionList[i].mBearingF_torque;
	mPinionList[i].ShaftForceVector[0] = mPinionList[i].TorqueDirect * (Fr * cos(InAssembleAng)) - Ft * sin(InAssembleAng) - FTorque * sin(InRotAng);
	mPinionList[i].ShaftForceVector[1] = mPinionList[i].TorqueDirect * (Ft * cos(InAssembleAng) + FTorque * cos(InRotAng) - Fr * sin(InAssembleAng));
	mPinionList[i].ShaftForceVector[2] = 0;

	TempForceDist[0] = mOtherCoef.ee * cos(InAssembleAng);
	TempForceDist[1] = mOtherCoef.ee * sin(InAssembleAng);
	TempForceDist[2] = 0;
	TempForceDist.CrossProduct(mPinionList[i].ShaftForceVector, TempTorque);
	std::copy(TempTorque.begin(), TempTorque.end(), &(mPinionList[i].ShaftForceVector[3]));

}

//void CPinionCycloidGearTAN::DefineLocationPara(double InDistBetweenAssembleEdgeToShaftLeft, int InAssembleFaceType)
//{
//	mPinion.AssembleFaceType = InAssembleFaceType;
//	mPinion.AssembleDist = InDistBetweenAssembleEdgeToShaftLeft;
//}
//
//void CPinionCycloidGearTAN::DefineLocationPara(double InDistBetweenAssembleEdgeToShaftLeft, int InAssembleFaceType)
//{
//	mGear.AssembleFaceType = InAssembleFaceType;
//	mGear.AssembleDist = InDistBetweenAssembleEdgeToShaftLeft;
//}

void CPinionCycloidGearTAN::CalGearLeftAndRightLocation(double& InAssembleDist, int& InAssembleType, double& InToothWidth, double& OutLeft, double& OutRight)
{
	if (InAssembleType == AssembleLeftBaseNotation)
	{
		OutLeft = InAssembleDist;

	}
	else if (InAssembleType == AssembleRightBaseNotation)
	{
		OutLeft = InAssembleDist- InToothWidth;
	}
	else
	{
		OutLeft = InAssembleDist - InToothWidth/2;
	}
	OutRight = InToothWidth + OutLeft;
}

double CPinionCycloidGearTAN::CalGearDesignToothWidth(double& InPinionAssembleDist, int& InPinionAssembleType, double& InPinionToothWidth, double& InGearAssembleDist, int& InGearAssembleType, double& InGearToothWidth)
{
	double TempPinionLeft;
	double TempPinionRight;
	CalGearLeftAndRightLocation(InPinionAssembleDist, InPinionAssembleType, InPinionToothWidth, TempPinionLeft, TempPinionRight);
	double TempGearLeft;
	double TempGearRight;
	CalGearLeftAndRightLocation(InGearAssembleDist, InGearAssembleType, InGearToothWidth, TempGearLeft, TempGearRight);
	if (TempPinionLeft< TempGearLeft)
	{
		TempPinionLeft = TempGearLeft;
	}
	if (TempPinionRight > TempGearRight)
	{
		TempPinionRight = TempGearRight;
	}
	return (TempPinionRight- TempPinionLeft);
}

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

double CPinionCycloidGearTAN::RecommandK2ByGearTooth(int InZb)
{
	return -0.0243421* InZb + 3.14211;
}

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

double CPinionCycloidGearTAN::CalCoefK_y()
{
	_CycloidGearPara
	double AngDev = 2 * Pi / zb;
	double& R = Rz;
	double theta = -AngDev / 2;
	double PositionVector_X;
	double PositionVector_Y;
	double VectorNormalValue;
	double fy = 0;
	double K_y=0;
	for (int i=0;i< zb/2.0;i++)
	{
		theta = theta+ AngDev;
		PositionVector_X = R * sin(theta);
		PositionVector_Y = mGear.rm - R * cos(theta);
		VectorNormalValue = sqrt(pow(PositionVector_X, 2) + pow(PositionVector_Y, 2));
		fy = acos(PositionVector_Y / VectorNormalValue);
		K_y = K_y + 2 * sin( 2*fy) / static_cast<double>(zb);
	}
	return K_y;
}
//计算跨棒距
double CPinionCycloidGearTAN::CalcMValue(int Z, int d_rp, double r_p, double e, double d_m, double tolerance)
{
	double Rt_prime = d_m / 2.0; // 真实量棒直径除以2
	double delta_r_p = 0.0, delta_r_rp = 0.0;  // 明确初始化
	int z_b = Z + 1; 
	//double r_p = d_p / 2.0; // 针齿中心圆半径，对应字母Rz/Rzf
	double k1 = CalK1(z_b, r_p, e);
	double r_fg = r_p - e - d_rp / 2.0; //摆线轮齿根圆半径
	//double RL = CalcRL(r_p, r_z / 2.0, delta_r_p, delta_r_rp, e, z_b, Z, k1);// 量棒中心距摆线轮中心距离
	//double RL0 = r_fg + d_m / 2.0; // 量棒中心距摆线轮中心距离，这种计算方法只在针齿直径和量棒直径一样时有效
	std::pair<double, double> derivatives = calculateRLWithOptimization(
		r_p, d_rp / 2.0, delta_r_p, delta_r_rp, e, z_b, Z,
		Rt_prime, tolerance);
	double RL = derivatives.first;
	double di = derivatives.second;
	int n = CalcCrossToothNumber(Z); // 跨测间隔齿数
	double theta = 2 * PI * n / Z; // 两量棒中心夹角（弧度）
	double ptheta = PI - theta; //平角（弧度）
	double length1 = cos(ptheta)* RL;
	mPinionList[0].M = RL + length1 + di;
	mPinionList[1].M = mPinionList[0].M;
	//double centerDistance = 2 * RL * sin(theta / 2.0);
	//double M = centerDistance + d_m;
	return mPinionList[0].M; //能精确到小数点后一位。
}
// 计算测量圆半径RL和量棒直径dt，包含迭代优化过程
std::pair<double, double> CPinionCycloidGearTAN::calculateRLWithOptimization(double r_p, double r_rp, double delta_r_p, double delta_r_rp,double a, double z_p, double z_c, double Rt_prime,double tolerance)
{
	// 计算短幅系数K1
	double K1 = a * z_p / (r_p + delta_r_p);

	// 初始拐点 φ_A
	double phiA = calculatePhiA(K1, z_p);

	// 设置初始差分步长
	double delta_phi = 0.05;
	double delta_phi2 = delta_phi / 2.0;
	double KA = 0.0;
	double KA_prime = 0.0;
	// 计算拐点坐标
	Point A = calculateToothProfile(phiA, r_p, r_rp, delta_r_p, delta_r_rp, a, z_p, z_c);

	// 迭代优化法线斜率KA
	for (int i = 0; i < 1000; ++i) 
	{
		//差分计算导数
		Point A_delta_phi = calculateToothProfile(phiA + delta_phi, r_p, r_rp, delta_r_p, delta_r_rp, a, z_p, z_c);
		double delta_x = A_delta_phi.x - A.x;
		double delta_y = A_delta_phi.y - A.y;
		KA = -delta_x / delta_y;

		// 差分计算导数
		Point A_delta_phi2 = calculateToothProfile(phiA + delta_phi2, r_p, r_rp, delta_r_p, delta_r_rp, a, z_p, z_c);
		double delta_x2 = A_delta_phi2.x - A.x;
		double delta_y2 = A_delta_phi2.y - A.y;
		KA_prime = -delta_x2 / delta_y2;

		// 检查收敛性
		if (fabs(KA - KA_prime) < 0.0001)
		{
			KA = KA_prime;
			break;
		}
		else delta_phi2 /= 2.0;
	}

	// 计算测量圆半径RL
	double RL = -A.y / KA + A.x;
	double xB = RL;
	// 计算理论量棒半径Rt
	double Rt = sqrt((A.x - RL) * (A.x - RL) + A.y * A.y);

	// 迭代调整phiA，使量棒直径逼近目标值
	double step = 0.001; // 设置步长
	double RL_adj = 0;
	int j = 0;
	for (j = 0; j < 10000; ++j) {

		if (fabs(Rt - Rt_prime) > tolerance) 
		{
			if (Rt< Rt_prime) {
				phiA += step; // 增加phiA使量棒直径增加
			}
			else phiA -= step; // 减小phiA使量棒直径减小

			// 计算调整后的拐点坐标
			Point A_adj = calculateToothProfile(phiA, r_p, r_rp, delta_r_p, delta_r_rp, a, z_p, z_c);
			// 迭代优化法线斜率KA
			for (int i = 0; i < 1000; ++i)
			{
				//差分计算导数
				Point A_delta_phi = calculateToothProfile(phiA + delta_phi, r_p, r_rp, delta_r_p, delta_r_rp, a, z_p, z_c);
				double delta_x = A_delta_phi.x - A_adj.x;
				double delta_y = A_delta_phi.y - A_adj.y;
				KA = -delta_x / delta_y;

				// 差分计算导数
				Point A_delta_phi2 = calculateToothProfile(phiA + delta_phi2, r_p, r_rp, delta_r_p, delta_r_rp, a, z_p, z_c);
				double delta_x2 = A_delta_phi2.x - A_adj.x;
				double delta_y2 = A_delta_phi2.y - A_adj.y;
				KA_prime = -delta_x2 / delta_y2;

				// 检查收敛性
				if (fabs(KA - KA_prime) < 0.0001)
				{
					KA = KA_prime;
					break;
				}
				else delta_phi2 /= 2.0;
			}
			RL_adj = -A_adj.y / KA + A_adj.x;
			Rt = sqrt((A_adj.x - RL_adj) * (A_adj.x - RL_adj) + A_adj.y * A_adj.y);
			xB = RL_adj;
		}
		else
		{
			//Rt = Rt_prime;
			RL = xB;
			break;
		}
	}
	return std::make_pair(RL, 2 * Rt);
}
// 计算曲率半径公式中的 φ_A
double CPinionCycloidGearTAN::calculatePhiA(double K1, double z_p) {
	double cos_phi = (1 + z_p * K1 * K1) / (K1 * (z_p + 1));
	return acos(cos_phi);
}
// 计算齿廓曲线方程 (1) 中的 x 和 y
CPinionCycloidGearTAN::Point CPinionCycloidGearTAN::calculateToothProfile(double phi, double r_p, double r_rp, double delta_r_p, double delta_r_rp, double a, double z_p, double z_c) 
{
	double iH = z_p / z_c;
	double K1 = a * z_p / (r_p + delta_r_p);
	double F = 1 /sqrt(1 + K1 * K1 - 2 * K1 * cos(phi));

	double term1 = r_p - r_rp * F;
	double term2 = a - K1 * r_rp * F;                    

	double x = term1 * cos((1 - iH) * phi) - term2 * cos(iH * phi);
	double y = term1 * sin((1 - iH) * phi) + term2 * sin(iH * phi);

	return { x, y };
}
// 计算跨测齿数（适配奇数/偶数齿数）
int CPinionCycloidGearTAN::CalcCrossToothNumber(int z)
{
	int n;
	if (z % 2 == 1) // 奇数齿
	{
		//n = (z + 1) / 2; // 间隔齿数，保证对称
		n = (z - 1) / 2 - 1; //实际需求
	}
	else // 偶数齿
	{
		n = z / 2;//暂时没有偶数齿的！暂定
		std::cout << "若需计算偶数齿数，请联系作者！" << std::endl;
	}
	return n;
}
//计算齿顶距
double CPinionCycloidGearTAN::CalToothTipDistanceValue(int Z, double d_rp, double Rz, double e)
{
	//通过齿顶圆半径来求
	double theta = 2 * PI / Z; // 两量棒中心夹角（弧度）
	double φ = theta / 2.0;
	double rz = d_rp / 2.0; //针齿半径
	//double Rz = d_p / 2.0; // 针齿中心圆半径，对应字母Rz/Rzf
	double r_ag = Rz + e - rz;
	double length1 = cos(φ) * r_ag;
	mPinionList[0].DT = length1 + r_ag;
	mPinionList[1].DT = mPinionList[0].DT;
	////通过曲率来求
	//double maxSearchDist = 2 * PI;
	//findNearestVerticalTangent(phiA, maxSearchDist, step, 1e-10, 1e-8, true, r_p, r_rp, delta_r_p, delta_r_rp, a, z_p, z_c);
	return mPinionList[0].DT;
}
void CPinionCycloidGearTAN::ObtainFourCycloidPara(int level, double r, double mn, double b)
{
	double d = 2.0 * r;
	std::vector<FourCycloidPara> resultCycloidData;
	FourCycloidPara para; // 单个结果对象
	MYSQL* mysql = mysql_init(NULL);// 初始化连接环境
	mysql = mysql_real_connect(mysql, "localhost", "root", "jay", "bearings", 3729, NULL, 0);// 连接数据库服务器
	mysql_set_character_set(mysql, "utf8");	// 设置字符集（避免中文乱码，根据数据库实际编码调整）
	// 校验输入参数（防止无效查询）
	if (level < 0 || level > 12) {
		std::cerr << "无效精度等级：" << level << "（仅支持0~12级）" << std::endl;
		mysql_close(mysql);

	}
	if (d <= 0 || mn <= 0 || b <= 0) {
		std::cerr << "无效参数：d=" << d << ", mn=" << mn << ", b=" << b << "（需大于0）" << std::endl;
		mysql_close(mysql);

	}
	// 先查Fr和Fp，并交换两个的值；再找到更换后的Fp值（在同一d范围内查找）所对应的精度等级，用该精度等级查Fpt值，最后查Fbeta值
	// 步骤1：查询原始F_p和F_r
	para.F_p = querySingleValue(mysql, "f_p", level, d, mn, b);   // 齿距累积总偏差
	para.F_r = querySingleValue(mysql, "f_r", level, d, mn, b);   // 径向跳动
	// 步骤2：交换F_p和F_r的值
	double tmpExchange = para.F_p;
	para.F_p = para.F_r;
	para.F_r = tmpExchange;
	// 步骤3：根据交换后的F_p值，反查同一d范围下的精度等级newLevel
	int newLevel = reverseQueryLevel(mysql, para.F_p, d, mn);
	if (newLevel == -1) {
		std::cerr << "反查精度等级失败，终止流程" << std::endl;
		mysql_close(mysql);

	}
	// 步骤4：用newLevel查询f_pt和F_beta
	para.f_pt = querySingleValue(mysql, "f_pt", newLevel, d, mn, b);        // 齿距偏差
	para.F_beta = querySingleValue(mysql, "f_beta", newLevel, d, mn, b);     // 齿向偏差
	// 验证查询结果（非异常值则加入结果）
	if (para.f_pt >= 0 && para.F_p >= 0 && para.F_beta >= 0 && para.F_r >= 0) {
		resultCycloidData.push_back(para);
	}
	else {
		std::cerr << "部分表查询结果异常，未返回有效数据" << std::endl;
	}
	mysql_close(mysql);	// 释放资源
	mFourCycloidPara.F_beta = resultCycloidData[0].F_beta;
	mFourCycloidPara.F_p = resultCycloidData[0].F_p;
	mFourCycloidPara.F_r = resultCycloidData[0].F_r;
	mFourCycloidPara.f_pt = resultCycloidData[0].f_pt;
}
// 工具函数实现1：查询单个表的单个值
double CPinionCycloidGearTAN::querySingleValue(MYSQL* mysql, const std::string& tableName, int level, double d, double mn, double b)
{
	// 拼接精度等级字段名（0~12级，对应表中`0`~`12`字段）
	std::string gradeField = "`" + std::to_string(level) + "`";
	std::string sql;
	// 根据表名拼接不同的查询条件（核心：匹配d/mn/b的区间）
	if (tableName == "f_pt") { // 齿距偏差表（d+mn）
		sql = "SELECT " + gradeField + " FROM " + tableName +
			" WHERE d_min < " + std::to_string(d) + " AND d_max >= " + std::to_string(d) +
			" AND m_min < " + std::to_string(mn) + " AND m_max >= " + std::to_string(mn);
	}
	else if (tableName == "f_p") { // 齿距累积总偏差表（d+mn）
		sql = "SELECT " + gradeField + " FROM " + tableName +
			" WHERE d_min < " + std::to_string(d) + " AND d_max >= " + std::to_string(d) +
			" AND m_min < " + std::to_string(mn) + " AND m_max >= " + std::to_string(mn);
	}
	else if (tableName == "f_beta") { // 齿向偏差表（d+b）
		sql = "SELECT " + gradeField + " FROM " + tableName +
			" WHERE d_min < " + std::to_string(d) + " AND d_max >= " + std::to_string(d) +
			" AND b_min < " + std::to_string(b) + " AND b_max >= " + std::to_string(b);
	}
	else if (tableName == "f_r") { // 径向跳动表（d+mn）
		sql = "SELECT " + gradeField + " FROM " + tableName +
			" WHERE d_min < " + std::to_string(d) + " AND d_max >= " + std::to_string(d) +
			" AND m_min < " + std::to_string(mn) + " AND m_max >= " + std::to_string(mn);
	}
	else {
		std::cerr << "未知表名：" << tableName << std::endl;
		return -1.0; // 异常值标记
	}
	// 执行SQL
	if (mysql_query(mysql, sql.c_str()) != 0) {
		std::cerr << "查询失败（表：" << tableName << "）：" << mysql_error(mysql) << std::endl;
		return -1.0;
	}
	// 获取结果集（单值查询仅需一行一列）
	MYSQL_RES* result = mysql_store_result(mysql);
	if (!result) {
		std::cerr << "获取结果集失败（表：" << tableName << "）：" << mysql_error(mysql) << std::endl;
		return -1.0;
	}
	// 解析单值
	double value = -1.0;
	MYSQL_ROW row = mysql_fetch_row(result);
	if (row != NULL && row[0] != NULL) {
		value = atof(row[0]); // 转换为double
	}
	else {
		std::cerr << "未找到匹配数据（表：" << tableName << "，d=" << d << ", mn=" << mn << ", b=" << b << "）" << std::endl;
	}
	// 释放结果集
	mysql_free_result(result);
	return value;
}
// 工具函数现2：根据Fp值反查精度等级
int CPinionCycloidGearTAN::reverseQueryLevel(MYSQL* mysql, double targetFp, double d, double mn)
{
	if (targetFp < 0) {
		std::cerr << "无效的目标Fp值：" << targetFp << std::endl;
		return -1;
	}

	// 1. 先查询同一d范围下，f_p表中所有等级的Fp值（0~12级）
	std::string sql = "SELECT `0`, `1`, `2`, `3`, `4`, `5`, `6`, `7`, `8`, `9`, `10`, `11`, `12` FROM f_p "
		"WHERE d_min < " + std::to_string(d) + " AND d_max >= " + std::to_string(d);

	if (mysql_query(mysql, sql.c_str()) != 0) {
		std::cerr << "反查等级失败：" << mysql_error(mysql) << std::endl;
		return -1;
	}

	MYSQL_RES* result = mysql_store_result(mysql);
	if (!result) {
		std::cerr << "反查等级-获取结果集失败：" << mysql_error(mysql) << std::endl;
		return -1;
	}

	// 2. 解析所有等级的Fp值，匹配targetFp
	MYSQL_ROW row;
	int newLevel = -1;
	int rowCount = 0;               // 匹配的行数
	while ((row = mysql_fetch_row(result)) != NULL) {
		rowCount++;
		// 遍历当前行的0~12级Fp值（第5列开始：`0`~`12`）
		for (int i = 0; i <= 12; ++i) {
			if (row[i] == NULL) continue;
			double fpValue = atof(row[i]);
			if (fabs(fpValue - targetFp) < 1e-6)
			{
				newLevel = i;
				goto END_SEARCH; // 找到精确匹配，直接退出
			}
		}
	}
END_SEARCH:
	// 3. 结果校验
	if (newLevel == -1) {
		std::cerr << "未找到匹配的精度等级（targetFp=" << targetFp << ", d=" << d << ", mn=" << mn << "）" << std::endl;
	}
	else {
		std::cout << "反查精度等级成功：targetFp=" << targetFp << " → level=" << newLevel << std::endl;
	}

	mysql_free_result(result);
	return newLevel;
}
//static double normalizePhi(double phi) {
//	const double TWO_PI = 2.0 * PI;
//	double r = fmod(phi, TWO_PI);
//	if (r < 0) r += TWO_PI;
//	return r;
//}
//void CPinionCycloidGearTAN::computeDerivativesAnalytic(
//	double phi,
//	double r_p, double r_rp, double delta_r_p, double delta_r_rp,
//	double a, double z_p, double z_c,
//	double& dx_dphi, double& dy_dphi)
//{
//	double iH = z_p / z_c;
//	double K1 = a * z_p / (r_p + delta_r_p);
//	double cosphi = cos(phi);
//	double sinphi = sin(phi);
//	double D = 1.0 + K1 * K1 - 2.0 * K1 * cosphi;
//	double D32 = pow(max(D, 1e-18), 1.5);
//
//	double F = 1.0 / sqrt(D);
//
//	double term1 = r_p - r_rp * F;
//	double term2 = a - K1 * r_rp * F;
//
//	// dF/dphi = -K1*sinφ / D^{3/2}
//	double dF_dphi = -K1 * sinphi / D32;
//	double dterm1 = -r_rp * dF_dphi;
//	double dterm2 = -K1 * r_rp * dF_dphi;
//
//	double alpha = (1.0 - iH);
//	double beta = iH;
//
//	dx_dphi = dterm1 * cos(alpha * phi)
//		+ term1 * (-alpha * sin(alpha * phi))
//		- dterm2 * cos(beta * phi)
//		+ term2 * (beta * sin(beta * phi));
//
//	dy_dphi = dterm1 * std::sin(alpha * phi)
//		+ term1 * (alpha * std::cos(alpha * phi))
//		+ dterm2 * std::sin(beta * phi)
//		+ term2 * (beta * std::cos(beta * phi));
//}
//double CPinionCycloidGearTAN::bisection_root(const std::function<double(double)>& f, double a, double b, double tol, int maxIter)
//{
//	double fa = f(a), fb = f(b);
//	if (!(std::isfinite(fa) && std::isfinite(fb) && fa * fb <= 0.0)) return std::numeric_limits<double>::quiet_NaN();
//	for (int iter = 0; iter < maxIter; ++iter) {
//		double m = 0.5 * (a + b);
//		double fm = f(m);
//		if (!std::isfinite(fm)) return std::numeric_limits<double>::quiet_NaN();
//		if (std::fabs(fm) < tol || (b - a) * 0.5 < tol) return m;
//		if (fa * fm <= 0.0) {
//			b = m; fb = fm;
//		}
//		else {
//			a = m; fa = fm;
//		}
//	}
//	return 0.5 * (a + b);
//}
//CPinionCycloidGearTAN::VerticalTangentResult CPinionCycloidGearTAN::findNearestVerticalTangent(
//	double phiA,
//	double maxSearchDist,
//	double step,
//	double root_tol,
//	double dy_eps,
//	bool wrap2pi,
//	double r_p, double r_rp, double delta_r_p, double delta_r_rp,
//	double a, double z_p, double z_c)
//{
//	VerticalTangentResult result;
//	result.found = false;
//	result.phi = 0.0;
//	result.pt = { 0.0, 0.0 };
//	result.dx = result.dy = 0.0;
//
//	auto f = [&](double phi)->double {
//		double dx, dy;
//		computeDerivativesAnalytic(phi, r_p, r_rp, delta_r_p, delta_r_rp, a, z_p, z_c, dx, dy);
//		return dx;
//		};
//
//	auto computeDy = [&](double phi)->double {
//		double dx, dy;
//		computeDerivativesAnalytic(phi, r_p, r_rp, delta_r_p, delta_r_rp, a, z_p, z_c, dx, dy);
//		return dy;
//		};
//
//	// check phiA itself first
//	double dxA = f(phiA);
//	double dyA = computeDy(phiA);
//	if (std::isfinite(dxA) && std::fabs(dxA) < root_tol && std::fabs(dyA) > dy_eps) {
//		CPinionCycloidGearTAN::Point P = calculateToothProfile(phiA, r_p, r_rp, delta_r_p, delta_r_rp, a, z_p, z_c);
//		result.found = true;
//		result.phi = phiA;
//		result.pt = P;
//		result.dx = dxA;
//		result.dy = dyA;
//		return result;
//	}
//
//	// expand both sides
//	double left_phi_prev = phiA;
//	double right_phi_prev = phiA;
//	double left_f_prev = dxA;
//	double right_f_prev = dxA;
//
//	int maxSteps = (int)std::ceil(maxSearchDist / step);
//	for (int i = 1; i <= maxSteps; ++i) {
//		// right side
//		double right_phi = phiA + i * step;
//		if (wrap2pi) right_phi = normalizePhi(right_phi);
//		double fr = f(right_phi);
//		if (std::isfinite(fr) && std::isfinite(right_f_prev)) {
//			if (fr == 0.0) {
//				double dyr = computeDy(right_phi);
//				if (std::fabs(dyr) > dy_eps) {
//					CPinionCycloidGearTAN::Point P = calculateToothProfile(right_phi, r_p, r_rp, delta_r_p, delta_r_rp, a, z_p, z_c);
//					result.found = true; result.phi = right_phi; result.pt = P; result.dx = fr; result.dy = dyr;
//					return result;
//				}
//			}
//			else if (right_f_prev * fr < 0.0) {
//				double a_br = right_phi_prev;
//				double b_br = right_phi;
//				auto flambda = [&](double phi)->double { return f(phi); };
//				double root = bisection_root(flambda, a_br, b_br, root_tol, 100);
//				if (std::isfinite(root)) {
//					double dyr = computeDy(root);
//					if (std::fabs(dyr) > dy_eps) {
//						CPinionCycloidGearTAN::Point P = calculateToothProfile(root, r_p, r_rp, delta_r_p, delta_r_rp, a, z_p, z_c);
//						result.found = true; result.phi = root; result.pt = P; result.dx = f(root); result.dy = dyr;
//						return result;
//					}
//				}
//			}
//		}
//		right_phi_prev = right_phi;
//		right_f_prev = fr;
//
//		// left side
//		double left_phi = phiA - i * step;
//		if (wrap2pi) left_phi = normalizePhi(left_phi);
//		double fl = f(left_phi);
//		if (std::isfinite(fl) && std::isfinite(left_f_prev)) {
//			if (fl == 0.0) {
//				double dyl = computeDy(left_phi);
//				if (std::fabs(dyl) > dy_eps) {
//					CPinionCycloidGearTAN::Point P = calculateToothProfile(left_phi, r_p, r_rp, delta_r_p, delta_r_rp, a, z_p, z_c);
//					result.found = true; result.phi = left_phi; result.pt = P; result.dx = fl; result.dy = dyl;
//					return result;
//				}
//			}
//			else if (left_f_prev * fl < 0.0) {
//				double a_br = left_phi;
//				double b_br = left_phi_prev;
//				auto flambda = [&](double phi)->double { return f(phi); };
//				double root = bisection_root(flambda, a_br, b_br, root_tol, 100);
//				if (std::isfinite(root)) {
//					double dyl = computeDy(root);
//					if (std::fabs(dyl) > dy_eps) {
//						CPinionCycloidGearTAN::Point P = calculateToothProfile(root, r_p, r_rp, delta_r_p, delta_r_rp, a, z_p, z_c);
//						result.found = true; result.phi = root; result.pt = P; result.dx = f(root); result.dy = dyl;
//						return result;
//					}
//				}
//			}
//		}
//		left_phi_prev = left_phi;
//		left_f_prev = fl;
//	}
//
//	// not found
//	return result;
//}