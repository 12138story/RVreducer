# SOURCE ENCODING: gbk
#include "TANRVReducer.h"
#define _RVReducerPara \
int&zp1 = mFirstGearPair.mPinion.Z;\
int&zg1 = mFirstGearPair.mGear.Z;\
int& zp2= mSecondGearPair.mPinionList[0].Z;\
int& zg2 =mSecondGearPair.mGear.Z;\
double& ee = mSecondGearPair.mOtherCoef.ee;\
double& rz = mSecondGearPair.mGear.rz;\
double& Rz = mSecondGearPair.mGear.Rzf;\
double& K1 = mSecondGearPair.mOtherCoef.K1;\
double& K2 = mSecondGearPair.mOtherCoef.K2;\
double& T_b = mSecondGearPair.mGear.Torque;\

void CRVReducerTAN::Clear()
{
	mSecondGearPair.Clear();
	std::vector<CBearingTAN>().swap(mBearingList);
	mCrankAxleSystem.Clear();
}
void CRVReducerTAN::CalTotalTransRatio()
{
	_RVReducerPara
	mFirstPairGearRatio = static_cast<double>( mFirstGearPair.mGear.Z )/mFirstGearPair.mPinion.Z;
	mSecondPairGearRatio = static_cast<double>( mSecondGearPair.mGear.Z ) / mSecondGearPair.mPinionList[0].Z;
	mTotalTransmissRatioWithShellFix = (1 + mFirstPairGearRatio * zg2);//注意根据力矩计算，前面要加负号，而根据转速计算，前面不加负号
	mTotalTransmissRatioWithShaftFix = -mFirstPairGearRatio * zg2;//这里饶振刚的书里P17-5有误
	if (mFixElemNotation== RVShellFixNotation)
	{
		mTransmissionRatio = mTotalTransmissRatioWithShellFix;
	}
	else
	{
		mTransmissionRatio = mTotalTransmissRatioWithShaftFix;
	}
}

void CRVReducerTAN::CalTotalTransRatio(double InFirstContRatio, int InZg)
{
	_RVReducerPara
	mFirstPairGearRatio = InFirstContRatio;
	zg2 = InZg;
	for (int i = 0;i < mSecondGearPair.mPinionList.size();i++)
	{
		mSecondGearPair.mPinionList[i].Z = zg2 - 1;
	}
	mSecondPairGearRatio = static_cast<double>(mSecondGearPair.mGear.Z) / mSecondGearPair.mPinionList[0].Z;
	mTotalTransmissRatioWithShellFix = (1 + mFirstPairGearRatio * zg2);
	mTotalTransmissRatioWithShaftFix = - mFirstPairGearRatio * zg2;//这里饶振刚的书里P17-5有误
	if (mFixElemNotation == RVShellFixNotation)
	{
		mTransmissionRatio = mTotalTransmissRatioWithShellFix;
	}
	else
	{
		mTransmissionRatio = mTotalTransmissRatioWithShaftFix;
	}

}

void CRVReducerTAN::CalGearPairTorque()
{
	_RVReducerPara

	mFirstPairMotorInputTorque = mOutputTorque / mTransmissionRatio;
	mFirstPairLoadTorqueForShaft = - mFirstPairMotorInputTorque;
	mFirstPairGearLoadTorqueForShaft = mFirstPairLoadTorqueForShaft * mFirstPairGearRatio;//注意，这里是以曲轴中心为基准
	if (mFixElemNotation == 0)
	{
		mSecondPairGearLoadTorqueForShaft = zg2 * mFirstPairGearRatio * mFirstPairMotorInputTorque;
		//转速
		mFirstPairPinionRPM = mOutputRPM * mTransmissionRatio;//w1
		mFirstPairGearRPM = tagRVGearBasicTAN::CalGearRevoluteSpeed(mFirstPairPinionRPM, mOutputRPM, -mFirstPairGearRatio);
		mSecondPairPinionRPM = mOutputRPM;
		mSecondPairGearRPM = 0;
		mFirstPairCrankShaftBearingRPM = mSecondPairPinionRPM - mFirstPairGearRPM;
		mSecondPairPinionBearingRPM = mFirstPairCrankShaftBearingRPM;
	}
	else
	{
		mSecondPairGearLoadTorqueForShaft = mOutputTorque;//由于只有针齿壳输出，所以这里的针输出力矩就是设定输出力矩
		//转速
		mFirstPairPinionRPM = mOutputRPM * mTransmissionRatio;//w1
		mFirstPairGearRPM = - mFirstPairPinionRPM / mFirstPairGearRatio;
		mSecondPairPinionRPM = 0;
		mSecondPairGearRPM = mOutputRPM;
		mFirstPairCrankShaftBearingRPM = mSecondPairPinionRPM - mFirstPairGearRPM;
		mSecondPairPinionBearingRPM = mFirstPairCrankShaftBearingRPM;
	}
	mSecondPairPinionTorqueForShaft = - 1 / mSecondPairGearRatio * mSecondPairGearLoadTorqueForShaft;
	//mSecondPairPinionOutputTorque = 1 / mSecondPairGearRatio * mSecondPairGearLoadTorqueForShaft;
	mSecondPairPinionOutputTorque = - mSecondPairPinionTorqueForShaft;
	////mSecondPairPinionTorqueForShaft = -mSecondPairPinionOutputTorque;
	////轴承转速
	//mFirstPairPinionRPM = mOutputRPM * mTransmissionRatio;
	//mFirstPairCrankShaftBearingRPM = mFirstPairPinionRPM / mFirstPairGearRatio;//曲柄支撑轴承的相对转速
	//mSecondPairPinionBearingRPM = zp2 * mOutputRPM;
 }

void CRVReducerTAN::RecommandTransmissionRatio(int InFixElemNotation, double InTotalTranRatio)
{
	mFixElemNotation = InFixElemNotation;
	//根据固定零部件，确定推荐传动比基准
	double tmpBaseTotalTransRatio = 0;
	if (mFixElemNotation == RVShellFixNotation)
	{
		tmpBaseTotalTransRatio = abs(InTotalTranRatio) - 1;
	}
	else
	{
		tmpBaseTotalTransRatio = abs(InTotalTranRatio);
	}
	//用于传动比插值公式(a0 + a1*x)*(b0 + b1*x)的4个系数
	double a0 = 0;
	double a1 = 0;
	double b0 = 0;
	double b1 = 0;
	double tmpX = 0;
	//分3段进行推荐，分段阈值为50，160
	if (tmpBaseTotalTransRatio < 50)
	{
		a0 = 0.0625;
		a1 = 1.9375;
		b0 = 16;
		b1 = 9;
	}
	else if (tmpBaseTotalTransRatio > 160)
	{
		a0 = 4;
		a1 = 1;
		b0 = 40;
		b1 = 20;
	}
	else
	{
		a0 = 2;
		a1 = 2;
		b0 = 25;
		b1 = 15;
	}
	tmpX = (-(a1 * b0) - a0 * b1 + sqrt(pow((a1 * b0 - a0 * b1), 2) + 4 * a1 * b1 * tmpBaseTotalTransRatio)) / (2 * a1 * b1);
	mSecondGearPair.mGear.Z = round((b0 + b1 * tmpX)/2)*2;//针轮必须为偶数
	mSecondGearPair.mPinionList[0].Z = mSecondGearPair.mGear.Z - 1;
	mFirstPairGearRatio = tmpBaseTotalTransRatio / mSecondGearPair.mGear.Z;
	CalTotalTransRatio(mFirstPairGearRatio, mSecondGearPair.mGear.Z);
	
}


