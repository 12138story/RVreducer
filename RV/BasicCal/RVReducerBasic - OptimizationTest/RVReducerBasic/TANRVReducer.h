#pragma once
#include "TANInvoluteGear.h"
#include "TANPinionCycloidGear.h"
#include "TANShaftSystem.h"
#include "TANBearing.h"
#define RVShellFixNotation 0 //针壳固定符号
#define RVCrankShaftFixNotation 1 //曲轴固定符号



class CRVReducerTAN
{
public:
	//行星轮总成
	CInvoluteGearTAN mFirstGearPair;
	//摆线轮总成
	CPinionCycloidGearTAN mSecondGearPair;
	////输入轴
	//CShaftTAN mInputShaft;
	////行星轴
	//CShaftTAN mCrankAxle;
	////输出轴
	//CShaftTAN mOutputShaft;
	//输入轴
	CShaftSystemTAN mInputShaftSystem;
	//行星轴
	CShaftSystemTAN mCrankAxleSystem;
	//输出轴
	CShaftSystemTAN mOutputShaftSystem;
	//轴承
	std::vector<CBearingTAN> mBearingList;
	//固定标识符号
	int mFixElemNotation = 0;//0-针齿壳固定，1-曲轴固定
	double mTransmissionRatio = 0;
	double mFirstPairMotorInputTorque = 0;//减速器驱动力矩
	double mFirstPairLoadTorqueForShaft = 0;//轮齿受到的载荷力矩	
	double mFirstPairGearLoadTorqueForShaft = 0;
	double mFirstPairPinionRPM = 0;
	double mFirstPairGearRPM = 0;
	double mFirstPairCrankShaftBearingRPM = 0;
	double mSecondPairPinionOutputTorque = 0;
	double mSecondPairPinionTorqueForShaft = 0;
	double mSecondPairGearLoadTorqueForShaft = 0;
	double mSecondPairPinionRPM = 0;
	double mSecondPairGearRPM = 0;
	double mSecondPairPinionBearingRPM = 0;
	double mOutputTorque = 0;
	double mOutputRPM = 0;
	double mFirstPairGearRatio = 0;
	double mSecondPairGearRatio = 0;
	double mTotalTransmissRatioWithShellFix = 0;//0-针齿壳固定时传动比
	double mTotalTransmissRatioWithShaftFix = 0;//1-曲轴固定时传动比
	double mDesignLife = 0;//设计寿命
	void Clear();
	//总传动比
	void CalTotalTransRatio();
	void CalTotalTransRatio(double InFirstContRatio,int InZg);
	//各构件受力
	void CalGearPairTorque();
	//传动比推荐
	void RecommandTransmissionRatio(int InFixElemNotation, double InTotalTranRatio);
};

