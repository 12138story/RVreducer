#include "TANInvoluteGear.h"
#include "InvoluteGearParameterAliases.h"

// 普通版齿轮副几何、啮合线及中心距与变位换算。

// 模块职责：按对应重载读取普通版齿轮副公共参数与两轮几何参数。
void CInvoluteGearTAN::DefineGearPairGeometry(double * InPinionData, double * InGearData, double * InPairCommonData, CInvoluteGearTAN & IOGearPair)
{
	mn = InPairCommonData[0];
	alpha_n = InPairCommonData[1];
	beta = abs(InPairCommonData[2]);
	IOGearPair.a_w = InPairCommonData[3];
	mPinion.DefineSingleGearGeometry(InPairCommonData[0], InPairCommonData[1], beta, InPinionData, IOGearPair.mPinion);
	mGear.DefineSingleGearGeometry(InPairCommonData[0], InPairCommonData[1], beta, InGearData, IOGearPair.mGear);
	IOGearPair.GearPairType = static_cast<int>(InPairCommonData[4]);
	IOGearPair.n_P = static_cast<int>(InPairCommonData[5]);
	IOGearPair.mGear.N_m = IOGearPair.n_P;
	IOGearPair.mPinion.N_m = IOGearPair.n_P;
}

// 模块职责：按对应重载读取普通版齿轮副公共参数与两轮几何参数。
void CInvoluteGearTAN::DefineGearPairGeometry(double * InPinionData, double * InGearData, double * InPairCommonData)
{
	DefineGearPairGeometry(InPinionData, InGearData, InPairCommonData, *this);
}

// 模块职责：统一两轮几何参数并计算普通版公共几何量。
void CInvoluteGearTAN::CalGearPairCommonGeomtryPara()
{
	//统一参数
	mPinion.beta = beta;
	mGear.beta = beta;
	mPinion.N_m = n_P;
	mGear.N_m = n_P;
	mGear.SpiralIndex = -mPinion.SpiralIndex;
	mPinion.alpha_n = alpha_n;
	mGear.alpha_n = alpha_n;
	mPinion.mn = mn;
	mGear.mn = mn;
	u = mGear.Z / static_cast<double>(mPinion.Z);
	mt = mn / cos(beta);
	alpha_t = atan(tan(alpha_n) / cos(beta));
	invalpha_t = CInvoluteGearTAN::CSingleInvoluteTAN::Finv(alpha_t);
	beta_b = atan(tan(beta) * cos(alpha_t));
	mPinion.mt = mt;
	mGear.mt = mt;
	mPinion.alpha_t = alpha_t;
	mGear.alpha_t = alpha_t;
	mPinion.invalpha_t = invalpha_t;
	mGear.invalpha_t = invalpha_t;
	mPinion.beta_b = beta_b;
	mGear.beta_b = beta_b;
	pn = Pi * mn;
	pt = Pi * mt;
	pbt = pt * cos(alpha_t);
	pet = pbt;
	C_a = (mPinion.C_a + mGear.C_a) / 2;
	////中心距离计算
	//a_w = mt * (mGear.Z + mPinion.Z) / 2 + (mGear.x + mPinion.x) * mt;
}

// 模块职责：按普通版有效啮合边界计算啮合线长度与重合度。
void CInvoluteGearTAN::CalContactPathLength()
{
	_CiteGearPara
	//double& d_a1 = mPinion.da;
	//double& d_a2 = mGear.da;
	//double& d_b1 = mPinion.db;
	//double& d_b2 = mGear.db;
	//double& d_m1 = mPinion.dm;
	//double& d_m2 = mGear.dm;
	//double& d_Fa1 = mPinion.d_Fa;
	//double& d_Fa2 = mGear.d_Fa;
	//double& d_Nf1 = mPinion.d_Nf;
	//double& d_Nf2 = mGear.d_Nf;
	//double& d_Na1 = mPinion.d_Na;
	//double& d_Na2 = mGear.d_Na;
	//int& z1 = mPinion.Z;
	//int& z2 = mGear.Z;
	//由齿顶形状直径决定有效齿廓起始点，如果由齿根形状决定则有另外一套方法
	//d_Na1 = d_Fa1;
	//d_Na2 = d_Fa2;
	d_Nf1 = sqrt(pow(2 * a_w * sin(alpha_wt) - mGear.IOMeshIndex * sqrt(pow(d_Fa2, 2) - pow(d_b2, 2)), 2) + pow(d_b1, 2));
	d_Nf2 = mGear.IOMeshIndex * sqrt(pow(2 * a_w * sin(alpha_wt) - sqrt(pow(d_Fa1, 2) - pow(d_b1, 2)), 2) + pow(d_b2, 2));
	//if (d_Nf1 < mPinion.d_Ff)
	//{
	//	d_Nf1 = mPinion.d_Ff;
	//}
	//if (d_Nf2 < mGear.d_Ff)
	//{
	//	d_Nf2 = mGear.d_Ff;
	//}
	if (abs(d_Nf1 - d_Ff1) < DefaultTolerance)
	{
		d_Na2 = sqrt(pow(2 * a_w*sin(alpha_wt) - sqrt(pow(d_Ff1, 2) - pow(d_b1, 2)), 2) + pow(d_b2, 2));
	}
	else
	{
		d_Na2 = d_Fa2;
	}
	if (abs(d_Nf2 - d_Ff2) < DefaultTolerance)
	{
		d_Na1 = sqrt(pow(2 * a_w*sin(alpha_wt) - mGear.IOMeshIndex*sqrt(pow(d_Ff2, 2) - pow(d_b2, 2)), 2) + pow(d_b1, 2));
	}
	else
	{
		d_Na1 = d_Fa1;
	}

	g_alpha = (sqrt(pow(d_Na1, 2) - pow(d_b1, 2)) + mGear.IOMeshIndex * (sqrt(pow(d_Na2, 2) - pow(d_b2, 2)) - 2 * a_w * sin(alpha_wt))) / 2 ;
	////需要删除的部分
	//g_alpha = 37.84464;
	//重合度
	Xi_alpha = g_alpha / pet;
	Xi_beta = b*sin(beta) / (mn*Pi);
	Xi_Gama = Xi_alpha + Xi_beta;
}

//void CInvoluteGearTAN::CalSingleGearGeometry(CInvoluteGearTAN& IOGearPair)
//{
//	using IOGearPair;
//	IOGearPair.
//}

// 模块职责：按普通版流程计算有效齿宽、单齿轮与齿轮副几何。
void CInvoluteGearTAN::CalGearPairGeometry()
{
	_CiteGearPara
	//工作齿宽
	if (mPinion.b_Design<mGear.b_Design)
	{
		mPinion.b_Work = mPinion.b_Design;
		mGear.b_Work = mPinion.b_Work;
		b = mPinion.b_Design;
	}
	else
	{
		mPinion.b_Work = mGear.b_Design;
		mGear.b_Work = mGear.b_Design;
		b = mGear.b_Design;
	}
	CalGearPairCommonGeomtryPara();
	mPinion.CalSingleGearBasicGeometry();
	mGear.CalSingleGearBasicGeometry();
	a_x = (mGear.d + mGear.IOMeshIndex * mPinion.d) / 2;
	Tooth_Sum = mGear.Z + mGear.IOMeshIndex * mPinion.Z;
	//x_Sum = (mGear.x + mGear.IOMeshIndex * mPinion.x);
	//mPinion.x = CalProfileShiftSumByCenterDistance();
	//mGear.x = 0;
	//
	alpha_wt = acos(abs(z1 + z2) * (mn * cos(alpha_t) / (2.0 * a_w * cos(beta))));//节圆压力角
	////该函数还计算了invalpha_wt，alpha_wt，y_t，y_n
	//CalCenterDistanceByProfileShift();
	mPinion.dw = mPinion.db / cos(alpha_wt);
	mGear.dw = mGear.db / cos(alpha_wt);

	//接触线长度
	CalContactPathLength();

	//CalCenterDistanceByProfileShift();


}

// 模块职责：根据当前变位系数和反求啮合角计算中心距。
double CInvoluteGearTAN::CalCenterDistanceByProfileShift()
{
	//std::cout << 2 * (mGear.x + mGear.IOMeshIndex * mPinion.x) * tan(alpha_n) / tmpZSum<<std::endl;
	//std::cout << CInvoluteGearTAN::CSingleInvoluteTAN::Finv(invalpha_t) << std::endl;

	invalpha_wt = 2 * x_Sum * tan(alpha_n) / Tooth_Sum + CSingleInvoluteTAN::Finv(alpha_t);
	alpha_wt = CSingleInvoluteTAN::GetAlphaByInvFunctionValue(invalpha_wt);
	y_t = Tooth_Sum * (cos(alpha_t) / cos(alpha_wt) - 1) / 2;
	y_n = y_t / cos(beta);
	a_w = a_x + y_t * mt;
	return a_w;
}

// 模块职责：根据当前中心距反算总变位系数。
double CInvoluteGearTAN::CalProfileShiftSumByCenterDistance()
{
	y_t = (a_w - a_x) / mt;
	y_n = (a_w - a_x) / mn;
	alpha_wt = acos(a_x / a_w * cos(alpha_t));
	invalpha_wt = CSingleInvoluteTAN::Finv(alpha_wt);
	x_Sum = (invalpha_wt - invalpha_t) / tan(alpha_n) / 2 * Tooth_Sum;
	return x_Sum;
}
