# SOURCE ENCODING: gbk
#pragma once
#include "PreDefine.h"
#include "TANCubicSpline.h"
#include <iostream>
#include <mysql.h>
#include <vector>
#include <string>
#include <cmath>  
//所有轴承都按该序列进行定义
//轴承作用点按什么方式定义，0对应左端面，1对应安装面
# define BearingForcePointToLeftFace AssembleLeftBaseNotation
# define BearingForcePointToMiddle AssembleMiddleBaseNotation
# define BearingForcePointToRightFace AssembleRightBaseNotation
//轴承安装朝向，也是附加轴向力朝向标识
//从左指向右,正常安装方式
# define BearingExForceToRightNotation 1
//从右指向左
# define BearingExForceToLeftNotation -1

struct tagBearingBasic
{
	double Cr;//基本额定动载荷
	double C0r;//基本额定静载荷
	double e=0.0;//计算系数
};
namespace NSBearingStaticArrary
{
	extern double sLhListInBook [210];
	extern double sFhListInBook[210];
	extern double sFh2LhCoef[420]; 
	extern double sLh2FhCoef[420];	
}
class CBearingTAN
{
public:
	CBearingTAN() ;
	CBearingTAN(double n, int L, int d, int D);
	CBearingTAN(double* InBearingData);
	CBearingTAN(int InBearingType, double* InBearingData);
	~CBearingTAN();
	//tagBearingBasic* mDeepGrooveBall=0;

	double C, P;//计算额定动载荷及当量动载荷
	double fh, fn;//各种因数，机械设计手册第六版第二卷8-230,寿命因数与速度因数
	double fm = 1;//力矩载荷因数
	double ft = 1;//温度因数
	double fd = 1.5;//冲击载荷系数
	double Co;//基本额定静载荷计算值
	double So;//安全因数，表8-2-30，8-2-31 P8-235
	double P0;//当量静载荷计算值
	int  L;//额定寿命-单位小时，用于寿命因数fh选择和Cr推荐
	int d, D;
	double n, L10;
	double RealLife;//实际寿命
	double Fa_ex;//派生轴向力
	double Fa;//实际轴向力
	double Fr;//径向力
	double Y, Yo, e = 0.0;
	double Y1, Y2;//角接触球轴承计算系数
	double Cr, Cor;
	double epsilon;//轴承循环寿命指数，对于球轴承为3，对于滚子轴承为10/3
	//double X, Y;//当量动载荷计算系数
	//double X0, Y0;//当量静载荷计算系数
	int stype;
	int para_method;
	int mBearingType;
	double mForcePointToShaftLeftDistance;//轴承作用点距离端面距离
	int AssembleFaceType;
	double AssembleDist;
	double AssembleForcePointDistPara;//对于非对称轴承为，轴承作用点距安装面的距离。
	int AssebleOrientation;//安装方向
	double Width;//轴承宽度
	//double Fa_Additonal;//附加轴向力
#pragma region 深沟球轴承
	//定义深沟球轴承
	void DefineDeepGrooveBallBearing(double n,  double Cr, double C0r);
	//定义滚针轴承
	void DefineNeedleBearing(double n1, double Cr1, double C0r1);
	void DefineAngularContactBallBearing(double n1, double Cr1, double C0r1, double tmpe, double tmpY, double tmpY1, double tmpY2);
#pragma endregion 深沟球轴承
#pragma region 圆锥滚子轴承
	//定义圆锥滚子轴承
	void DefineTaperedRollerBearing(double n1, //额定转速
		double Cr1,//基本额定动载荷 
		double Cor1,//基本额定静载荷
		double e1, //计算系数
		double Y1, //计算系数
		double Yo1 //计算系数
	);//圆锥滚子轴承第二种参数定义模式
	//轴承统一定义模式
	void DefineRollingBearing(int InBearingType,double* InBearingData);
#pragma endregion 圆锥滚子轴承
#pragma region 通用计算式
	void EquivalentLoadP(double Fa, double Fr);//等效载荷,深沟球轴承的未编写
	void CalFh();//根据载荷和其他因数计算寿命因数;
	double CalLifeByFh();//根据寿命因数插值得到循环寿命;
	void Dynamic_Load(double InBearingLife, double InRPM);//额定动载荷计算
	void Static_Load();//额定静载荷计算
	void BearingBasicLifeL10(double Fa, double Fr);//轴承寿命计算
	void Para_switch(int InInnerDiameter, int InOuterDiameter);//额定动载荷参数选择，根据内径和外径选择轴承
	double fhSet(int L);//寿命因数fh选择
	double fnSet(double n);//速度因素fn选择
	//double fdSet();//冲击载荷因素设置
	//double Ex_Fd(double Fr);//派生轴向力
	double GetBearingRealLife(double InFa, double InFr);
#pragma endregion 通用计算式
	//与轴分析相关数据
	//void CalForceLocationPoint(double* InBearingLocationPara);
	//非对称轴承定位数据
	void DefineTaperedRollerLocationPara(int InAssembleFaceType,//安装面定位面类型，1-外圈侧壁,左侧，-1-内圈侧壁，右侧，0-作用点
		double InDistBetweenAssembleEdgeToShaftLeft,//轴承安装面距离轴左端距离
		int InBearingForcePointFaceType,//轴承作用点对应的轴承面类型，1-外圈侧壁,左侧，-1-内圈侧壁，右侧，0-作用点
		double InBearingForcePointDist,//轴承作用点距离距轴承作用点参考面距离	
		int InBearingAssembleOrient,//轴承轴线方向 0-轴承轴线方向与轴一致，接触角指向远离轴左侧方向，1-轴承轴线方向与轴相反，接触角指向靠近轴左侧方向);
		double InBearingWidth//轴承宽度
	);
	//对称轴承定位数据
	void DefineRadialBearingLocationPara(int InAssembleFaceType,//安装面定位面类型，1-左侧，-1-右侧，0-中间
		double InDistBetweenAssembleEdgeToShaftLeft,//轴承安装面距离轴左端距离 	
		int InBearingAssembleOrient,//轴承安装朝向，1-正向，-1反向			
		double InBearingWidth//轴承宽度
	);
	//标准轴承定位尺寸定义函数,将安装面与定位面匹配
	//InAssembleData[0]安装面定位面类型
	// InAssembleData[1] 轴承安装面距离轴左端距离
	// InAssembleData[2] 轴承作用点对应面类型
	// InAssembleData[3] 轴承作用点距离对应面距离或轴承宽度，对于向心轴承，为轴承宽度
	// InAssembleData[4] 轴承安装方向，确定附加轴向力和轴承朝向
	// InAssembleData[5] 轴承宽度
	void DefineLocationPara(double* InAssembleData);
	//计算派生轴向力
	double CalAdditionalAxisForce(double InFr);
	//计算实际轴向力
	//InTotalShaftAxisForce 轴的总轴向力
	//InBearForceBearingCount实际承受力的轴承数目
	double CalRealAxisForce(double InTotalShaftAxisForce, int InBearForceBearingCount);
	//判断轴承是否轴向约束，0-轴向不约束，1-轴向约束
	int JudgeBearingIsAxisConstrain(double InTotalAxisForce);

private:
	//非向心轴承，轴承作用点不在齿宽中部
	//void CalTaperedRollerBearingLocation(		
	//	double InDistBetweenAssembleEdgeToShaftLeft,//轴承安装面距离轴左端距离
	//	int InAssembleFaceType,//安装面定位面类型，0-外圈侧壁,左侧，1-内圈侧壁，右侧		 
	//	double InBearingForcePointDistk,//轴承作用点距离距安装定位面距离		
	//	int InBearingAxisDirectIndex//轴承轴线方向 0-轴承轴线方向与轴一致，接触角指向远离轴左侧方向，1-轴承轴线方向与轴相反，接触角指向靠近轴左侧方向
	//);
	////向心轴承，轴承作用点在齿宽中部
	//void CalRadialBearingLocation(
	//	double InDistBetweenAssembleEdgeToShaftLeft,//轴承安装面距离轴左端距离 
	//	int InAssembleFaceType,//安装面定位面类型，0-左侧，1-右侧，2-中间
	//	double InBearingWidth//轴承宽度
	//	
	//);
	//初始化计算曲线	
	CCubicSplineTAN mLh2FhCurve;
	CCubicSplineTAN mFh2LhCurve;
	void InitialLifeCurve();
	double mBallBearingCoef = 3.0;
	double mRollerbearingCoef = 1 / 3.0;
#pragma region 周杰 
public:
	void CalAndRecommedBearingLife(int InBearingType, double* InBearingData);
	double ObtainDistFromBearingLocatingSurfaceToReferenceSurface(int InBearingLocatingSurface, double InBearingA, double InBearingWidth, double tmpInLength);
	struct BearingData
	{
		std::string code;
		double d;
		double D;
		double B;
		double Cr;
		double C0r;
		double e;
		double Y;
		double Yo;
		double life;
		double a;
		double T;
		double C;
		double contactAngle;
	};
	std::vector<BearingData> ObtainBearingDataVec(int InBearingType);
	std::vector<BearingData> mBearingDataVec;
	std::vector<double> mDiameterList;

	//角接触球轴承计算系数表，机械设计手册第六版第二卷表8-2-81
	struct BearingLoadCoeff
	{
		double FaOverC0r; // Fa/C0r
		double e;
		double Y;
		double Y1;
		double Y2;
	};
	std::vector<BearingLoadCoeff> coeffTable = {
		{0.015, 0.38, 1.47, 1.65, 2.39},
		{0.029, 0.40, 1.40, 1.57, 2.28},
		{0.058, 0.43, 1.30, 1.46, 2.11},
		{0.087, 0.46, 1.23, 1.38, 2.00},
		{0.120, 0.47, 1.19, 1.34, 1.93},
		{0.170, 0.50, 1.12, 1.26, 1.82},
		{0.290, 0.55, 1.02, 1.14, 1.66},
		{0.440, 0.56, 1.00, 1.12, 1.63},
		{0.580, 0.56, 1.00, 1.12, 1.63}
	};
	bool GetLoadCoeff(double FaOverC0r, double& e, double& Y, double& Y1, double& Y2);
#pragma endregion
};