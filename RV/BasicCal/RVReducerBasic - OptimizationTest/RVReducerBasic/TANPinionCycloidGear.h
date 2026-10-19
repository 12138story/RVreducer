#pragma once
#include "PreDefine.h"
#include "CTanVector.h"
#include "TANMechanismFunction.h"
#include <cmath>
#include <functional>
#include <limits>
#include <iostream>
#include <mysql.h>
#include <vector>
#include <string> 
//double pi = 3.14159;
//	double Omega;//转速
//	int Z;//齿数
//	//double mn;//参考法向模数
//	//double mt;//端面模数
//	//double dm;//节圆直径
//	//double h;//齿高
//	//double ra;//齿顶圆半径
//	double rm;//节圆半径
struct tagRVGearBasicTAN {
	double M; //跨棒距
	double DT; //齿顶距
	double Omega;//转速
	int Z;//齿数
	//double mn;//参考法向模数
	//double mt;//端面模数
	//double dm;//节圆直径
	//double h;//齿高
	//double ra;//齿顶圆半径
	double rm;//节圆半径
	double BB;//单个齿轮有效齿宽
	double Torque;
	double EModule = 0;//弹性模量
	int AssembleFaceType = AssembleMiddleBaseNotation;//安装面类型
	int TorqueDirect;//力矩旋向标识，每个齿轮由轮齿提供的力矩方向，1，指向右方，和z轴正向一致，-1指向左方，
	double AssembleDist = 0;//安装面距离
	double ToothWidth = 0;
	CTanVector ShaftForceVector;
	tagRVGearBasicTAN();
	//double rf;//齿根圆半径
	void DefineLocationPara(double InDistBetweenAssembleEdgeToShaftLeft, int InAssembleFaceType);
	void DefineLocationPara(double*InData);
	static double CalGearRevoluteSpeed(double InPinionSpeed, double InRotatingArmSpeed, double GearToPinionToothRatio);
	static double CalPinionRevoluteSpeed(double InGearSpeed, double InRotatingArmSpeed, double GearToPinionToothRatio);
};
class CPinionCycloidGearTAN{
public:
	
	struct tagPinion:tagRVGearBasicTAN {
		//double Omega;//转速
		//int Z;//齿数
		//double rm;//节圆半径
		double rmg;//摆线轮理论齿廓的平均半径，分度圆半径
		double r;//外啮法形成的滚圆半径
		double R;//外啮合法的基圆半径
		double ra;//摆线轮顶圆半径
		double rf;//摆线轮根圆半径
		double h;//摆线轮齿高
		//double EModule = 0;//弹性模量
		double EModule = 0;//弹性模量
		double FMax = 0;//最大接触力
		double EquvalentEModule = 0;
		double MaxStress;//最大接触应力
		double MaxStressTheta;//最大接触应力对应的转角

		double F_t;//圆周力
		double F_r;//径向力
		double F_Total;//作用点总合力;
		double mBearingF_t;
		double mBearingF_r;
		double mBearingF_torque;
		double mMaxBearingF;
	} ;//摆线论，小轮
	struct tagGear:tagRVGearBasicTAN {
		//double Omega;//转速
		//int Z;//齿数
		//double rm;//节圆半径
		double rz = 0;//滚针半径
		double Rzf = 0;//针轮分度圆直径,对应书中的Rz × 应该是半径吧
		double EModule = 0;//弹性模量
		//double EModule = 0;//弹性模量
		double WidthCoef = 0.15;//齿宽系数,默认为0.15
	} mGear;//针轮，大轮
	struct tagRotArm {
	} mRotArm;//转臂
	struct tagOtherCoef {
		double K1 = 0.65;//短幅系数
		double K2 = 0;//针径系数
		double ee = 0;//偏心距
		double BB = 0;//单对齿轮副计算齿宽
		//double TotalTorque = 0;
		
			
		//力参数
		//double T_g;//单轮所受力矩,摆线轮关于转动中心的力矩

	}mOtherCoef;
	//齿轮副数据
	// 			//力参数
	int Ng = 2;//摆线轮数目,默认为2


	double mPinionTotalTorque=0;//摆线轮总力矩，有大小和方向
	double mGearTotalTorque = 0;//摆线轮总力矩，有大小和方向
	double mPinAngInMaxConStress;//摆线轮总力矩
	double mMaxContStress;//最大接触应力
	double mMaxContStressPinTheta;//最大接触应力对应的摆线轮相位
	double mMaxContStressPinNum;//最大接触应力对应的摆线轮号

	double Kp = 0;//使用系数

	double EnableWidthConstraintCheckBox = 0;//默认没有启用候选框
	double targetMinToothWidth = 0;
	double targetMaxToothWidth = 0;
	void CopyCycloidWidthConstraintPara(double* InData);
	std::vector<tagPinion> mPinionList;
	CPinionCycloidGearTAN();
	void Clear();
	//CTanVector mBearingForceVector;
#pragma region 几何计算
	//传动比计算
	//template<typename T>
	//double FCalTransRatio(T&& OmgeaB,//中心轮转速
	//	T&& OmgeaG,//摆线行星轮转速
	//	T&& OmgeaH//转臂转速
	//) {		return (OmgeaG - OmgeaH) / (OmgeaB - OmgeaH);	}
	//定义基本参数
	//定义大轮基本参数
	template<typename T0, typename T1, typename T2, typename T3>
	void DefineGearBasicGeometry(T0&& zb,
		T1&& rz,//滚针半径
		T2&& Rz,//摆线轮理论齿廓的平均半径，分度圆半径
		T3&& GearWidth,//针轮齿宽)
		tagGear& OutGear)
	{
		OutGear.Z = zb;
		OutGear.ToothWidth = GearWidth;
		OutGear.rz = rz;
		//OutReducer.mPinion.rmg = Rz;
		OutGear.Rzf = Rz;

	}
	template<typename T0, typename T1>
	void DefinePinionBasicGeometry(T0&& zg,//摆线行星轮齿数
		T1&& PinionWidth,//针轮齿宽)
		tagPinion& OutPinion)
	{
		OutPinion.Z = zg;
		OutPinion.ToothWidth = PinionWidth;
	}
	//齿轮副基本参数
	template<typename T0, typename T1, typename T2, typename T3, typename T4,typename T5,typename T6>
	void DefineBasicGeometryPara(T0&& zb,
		T1&& zg,//摆线行星轮齿数
		T2&& rz,//滚针半径
		T3&& Rz,//摆线轮理论齿廓的平均半径，大轮针轮分度圆半径
		T4&& ee, //偏心距
		T5&& PinionWidthList,//摆线轮齿宽列表
		T6&& GearWidth,//针轮齿宽)
		int InNg,
		CPinionCycloidGearTAN& OutReducer)	{
		OutReducer.mGear.Z = zb;		
		OutReducer.mOtherCoef.ee = ee;
		OutReducer.mGear.ToothWidth = GearWidth;
		OutReducer.mGear.rz = rz;
		//OutReducer.mPinion.rmg = Rz;
		OutReducer.mGear.Rzf = Rz;
		//给针轮赋值
		//初始化
		if (Ng != InNg)
		{
			Ng = InNg;
			std::vector<tagPinion>(Ng).swap(mPinionList);
		}		
		for (int i = 0; i < Ng; i++)
		{
			OutReducer.mPinionList[i].Z = zg;
			OutReducer.mPinionList[i].ToothWidth = PinionWidthList[i];
		}

		
		
	}
	template<typename T0, typename T1, typename T2, typename T3, typename T4, typename T5,typename T6>
	void DefineBasicGeometryPara(T0&& zb,//中心轮，大轮齿数
		T1&& zg,//摆线行星轮齿数
		T2&& rz,//滚针半径
		T3&& Rz,//摆线轮理论齿廓的平均半径，大轮针轮分度圆半径
		T4&& ee, //偏心距
		T5&& PinionWidthList,//摆线轮齿宽列表
		T6&& GearWidth,//针轮齿宽)
		int InNg)
	{
		DefineBasicGeometryPara(zb, zg, rz, Rz, ee, PinionWidthList, GearWidth, InNg,*this);
	}
	//根据K1K2定义基本参数
	template<typename T0, typename T1, typename T2, typename T3, typename T4>
	void DefineBasicGeometryParaByK1K2(T0&& zb,
		T1&& zg,//摆线行星轮齿数
		T2&& Rz,
		T3&& K1,//摆线轮理论齿廓的平均半径，分度圆半径
		T4&& K2, //针半系数 )
		CPinionCycloidGearTAN& OutReducer)
	{
		OutReducer.mGear.Z = zb;		
		//OutReducer.mPinion.rmg = Rz;
		OutReducer.mGear.Rzf = Rz;
		OutReducer.mOtherCoef.K1 = K1;
		OutReducer.mOtherCoef.K2 = K2;
		OutReducer.mOtherCoef.ee = (2 * K1 * Rz / zb) / 2;
		OutReducer.mGear.rz= 2 * Rz / K2 * sin(Pi / double(zb)) / 2;
		for (int i = 0; i < Ng; i++)
		{
			OutReducer.mPinionList[i].Z = zg;
		}
	}
	template<typename T>
	double FCalTransRatio(T&& zg,//行星轮齿轮（外齿轮齿数）
		T&& zb,//中心轮齿数（内齿轮齿数）
		int ExportNotation)//输出标识，1-销轴输出；2-针壳输出(中心轮输出)
	{
		switch (ExportNotation)
		{
		case 1:
			return double(zg) / double(zg - zb);
		default:
			return double(zb) / double(zb - zg);
		}
	}


	
	////短幅系数
	//double CalRVBasicGeometry(double& rb, double& Rz) { return rb / Rz; }
	//template<typename T>
	//double CalRVBasicGeometry(T&& zb,double& ee/*偏心距*/, double& Rz) { return ee*zb / Rz; }
	////针轮节圆半径
	//double CalRVBasicGeometryByK1Rz(double& K1, double& Rz) { return K1*Rz; }
	//template<typename T>
	//double CalRVBasicGeometryByeeZb(T&& zb,double& ee) { return zb * ee; }
	void CalBasicGeometry();
	void CalBasicGeometry(int& zb,//中心轮齿数
		int& zg,//摆线行星轮齿数
		double& rz,//滚针半径
		double& Rz,//摆线轮理论齿廓的平均半径，分度圆半径
		double& ee, //偏心距
		CPinionCycloidGearTAN& OutReducer);

	double CalK1(int & zb, double & Rzf, double & ee);
	double CalK2(int & zb, double & rz, double & Rzf);
	//圆整偏心
	double ChooseRoundEccentricCoef(int& zb,double& Rzf, double& IOK1);//K1也要变动
	//圆整针齿套半径
	double ChooseRoundPinionToothCoverRadiusCoef(int& zb, double& Rzf, double& IOK2);
	void CalBasicGeometryByRzK1K2WithRound();
	void CalBasicGeometryByRzK1K2WithRound(int& zb,//中心轮齿数
		int& zg,//摆线行星轮齿数
		double& Rz,//针轮中心圆半径，分度圆半径
		double& K1,//短幅系数
		double& K2,//针径系数
		CPinionCycloidGearTAN& OutReducer);
#pragma endregion
#pragma region 强度计算
	void CalPinAndGearTorque(double InPinionTotalTorque);
	void DefineStressCalPara(double InPinionTotalTorque, double* InPinionModuleList, double InGearModule, double InKp);
	void DefineStressCalPara(double InPinionTotalTorque, double InPinionModule, double InGearModule, double InKp);
	double CalContForceMax(int& zg,//摆线轮齿数
		double& Rz,//针轮中心圆半径，分度圆半径？
		double InPinionTorque,//单个小轮传递力矩
		double& K1,//短幅系数
		double& Kp//均载系数
	);
	static double CalCoefS(double& PinThetab,//针轮相位
		double& K1,//短幅系数
		double& K2);//针径系数
	static double CalOneContForce(double& PinThetab,//针轮相位
		double& SS,//系数S
		double& FMax);//最大接触力
	static double CalCoefT(int& Inzb,//中心轮齿数，针轮齿数
		double& InPinThetab,//针轮相位
		double& InK1);
	//内摆线曲率半径，尔非摆线轮曲率半径，摆线轮曲率半径还需减去一个针轮半径的偏置量
	double CalCycloidCurveCurvatureRadius(double& InRz,
		double& InS,
		double& InT);
	//相对曲率计算
	static double CalRelativeCurvature(double& Inrz, double& InRho0);
	//当量弹性模量
	double CalEquivalentElasticModules(double& InE1, double& InE2);//弹性模量单位为N/mm^2，钢量级为10的5次方
	//基于赫兹公式的应力计算式
	static double CalNominalContStressBasic(double& InToothWidth, double& InRelativeCurvature, double& InEd, double& InFi);
	//一个位置的应力计算式
	double CalOneLocationNominalContStress(int& InZb, 
		double & InToothWidth, 
		double & Inrz, 
		double & InRzf, 
		double & InK1, 
		double & InK2,
		double & InPinThetab,
		double & InEd,
		double & InFMax);
	// 最大赫兹应力辅助计算式
	static double AuxiliaryForMinNegativeContStress(double* InPinThetab,
		void** InOtherCoef);
	// 最大接触应力计算式
	double FindMaxContStress(CPinionCycloidGearTAN& IOGearObj);
	//double FindMaxContStress(double InTotalPinionTorque,
	//	double* InPinionModuleList,
	//	double InGearModule,
	//	double InKp,
	//	CPinionCycloidGearTAN& IOGearObj);
	double FindMaxContStress(double InPinionToothTorque,//小轮轮齿承受的力矩，可正可负
		double* InPinionModuleList,//弹性模量
		double InGearModule,
		double InKp);
	double FindMaxContStress();
#pragma endregion 强度计算
#pragma region 受力计算
	//InCrankAxleCount,曲轴数目
	//InK_d,曲轴不平衡系数
	//InShaftAxisOffsetDistance,曲柄轴线相对于减速器中心偏移量，等于第一级渐开线齿轮的中心距
	//返回值为轴承径向力
	double CalBearingForce(int InCrankAxleCount,
		double InK_d,//曲轴载荷分配系数，只影响曲轴力和曲轴轴承力
		double InShaftAxisOffsetDistance);//曲轴中心距离减速器转动中心距离
	////曲轴力随着转角的变化，需先调用CalBearingForce函数
	//void CalShaftBearingForceByRotAng(
	//	double InRotAng, 
	//	double InAssembleAng);
	//InPinionIndex表明对应第几个摆线轮的相位,曲轴力随着转角的变化，需先调用CalBearingForce函数,
	void CalShaftBearingForceByRotAng(
		int InPinionIndex,
		double InRotAng,
		double InAssembleAng);
	//定义定位参数
	//void DefineLocationPara(double InDistBetweenAssembleEdgeToShaftLeft, int InAssembleFaceType);
	//void DefineLocationPara(double InDistBetweenAssembleEdgeToShaftLeft, int InAssembleFaceType);
	static void CalGearLeftAndRightLocation(double& InAssembleDist,int& InAssembleType,double& InToothWidth,double& OutLeft,double& OutRight);
	static double CalGearDesignToothWidth(double& InPinionAssembleDist, int& InPinionAssembleType, double& InPinionToothWidth, double& InGearAssembleDist, int& InGearAssembleType, double& InGearToothWidth);
#pragma endregion  受力计算
#pragma region 跨棒距计算和齿顶距计算
	// 摆线轮齿数， 针齿半径，针齿中心圆直径 (mm),偏心距 (mm), 量棒直径（0.002）(mm),跨棒距公差 (mm)，默认±0.005
	double CalcMValue(int Z, int r_z, double d_p, double e, double Rt, double tolerance);
	int CalcCrossToothNumber(int z);
	double calculatePhiA(double K1, double z_p);
	// 短幅外摆线齿廓点结构体（存储坐标）
	struct Point
	{
		double x;       // 直角坐标x
		double y;       // 直角坐标y
	};
	Point calculateToothProfile(double phi, double r_p, double r_pp, double delta_r_p, double delta_r_rp, double a, double z_p, double z_c);
	std::pair<double, double> calculateRLWithOptimization(double r_p, double r_pp, double delta_r_p, double delta_r_rp, double a, double z_p, double z_c, double target_di,double tolerance);
	//齿顶距计算
	double CalToothTipDistanceValue(int Z, double d_rp, double d_p, double e);
	//void computeDerivativesAnalytic(
	//	double phi,
	//	double r_p, double r_rp, double delta_r_p, double delta_r_rp,
	//	double a, double z_p, double z_c,
	//	double& dx_dphi, double& dy_dphi);
	//double bisection_root(const std::function<double(double)>& f, double a, double b, double tol, int maxIter);
	//struct VerticalTangentResult {
	//	bool found;
	//	double phi;
	//	Point pt;
	//	double dx;
	//	double dy;
	//};
	//VerticalTangentResult findNearestVerticalTangent(
	//	double phiA,
	//	double maxSearchDist,
	//	double step,
	//	double root_tol,
	//	double dy_eps,
	//	bool wrap2pi,
	//	double r_p, double r_rp, double delta_r_p, double delta_r_rp,
	//	double a, double z_p, double z_c);
#pragma endregion 跨棒距计算和齿顶距计算
#pragma region 摆线轮四个参数选择
	struct FourCycloidPara
	{
		double f_pt;//单个齿距偏差正负f_pt
		double F_p;//齿距累计偏差F_p
		double F_beta;//齿向偏差F_beta
		double F_r;//径向跳动F_r
	}mFourCycloidPara;
	void ObtainFourCycloidPara(int level, double d, double mn, double b);
	// 工具函数1：执行单值查询（封装重复逻辑）
	double querySingleValue(MYSQL* mysql, const std::string& tableName, int level, double d, double mn, double b);
	// 工具函数2：根据Fp值+同一d范围反查精度等级（核心新增逻辑）
	int reverseQueryLevel(MYSQL* mysql, double targetFp, double d, double mn);
#pragma endregion 摆线轮四个参数选择
#pragma region 参数推荐
	//InNp摆线轮数目
	void RecommondMainGeometryParaByContact(double InTargetStress, double InPinionTotalTorque, int InGearToothNum, int InNg = 2);
	//K2推荐函数，根据饶振纲书P456表18-4线性插值得到
	static double RecommandK2ByGearTooth(int InZb);
	static double AuxiliaryFunctionForOptimizeGearRadius(double* InRzf, void** InRVPairObj);
#pragma endregion 参数推荐
private:
	double CalCoefK_y();
	//inline void UpdatePinionZ(int)
	//{
	//	for (int i = 1; i < Ng; i++)
	//	{
	//		mPinionList[i].Z = mPinionList[0].Z;
	//	}
	//};
	//double Pi = 3.14159;
};

