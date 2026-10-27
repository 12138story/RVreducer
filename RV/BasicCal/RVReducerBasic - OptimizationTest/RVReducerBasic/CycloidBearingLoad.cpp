#include "TANPinionCycloidGear.h"

// 轴承载荷计算使用的对象成员别名，与原实现保持一致。
#define _CycloidGearPara \
int&zb = mGear.Z;\
int&zg = mPinionList[0].Z;\
double& ee = mOtherCoef.ee;\
double& rz = mGear.rz;\
double& Rz = mGear.Rzf;\
double& K1 = mOtherCoef.K1;\
double& K2 = mOtherCoef.K2;\
double& T_b = mGear.Torque;\

// 本文件集中实现摆线轮轴承载荷分配、力矩转换和轴向重合宽度计算。

// 按曲柄轴数量和载荷分配系数计算各摆线轮轴承的最大载荷。
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

// 将局部径向、切向载荷转换为指定安装角下的轴系六维载荷。
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

// 根据安装基准、安装距离和齿宽计算零件的左右轴向边界。
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

// 计算摆线轮与针齿壳轴向区间的有效重合宽度。
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
