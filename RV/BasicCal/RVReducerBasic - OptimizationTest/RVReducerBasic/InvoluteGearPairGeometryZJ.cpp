#include "TANInvoluteGear.h"
#include "InvoluteGearParameterAliases.h"

// ZJ 版齿轮副几何、啮合线、传动比及曲率计算；保留原版算法。

// 本模块内部参数别名：保持原有引用目标和展开顺序。
#define _CiteGearParaZJ \
int& z1 = mPinion.Z;\
int& z2 = mGear.Z; \
double& z_n1 = mPinion.zn; \
double& z_n2 = mGear.zn;\
double& d_a1 = mPinion.mSingleData.d_a;\
double& d_a2 = mGear.mSingleData.d_a;\
double& d_b1 = mPinion.db;\
double& d_b2 = mGear.db;\
double& d_1 = mPinion.d;\
double& d_2 = mGear.d;\
double& d_m1 = mPinion.dm;\
double& d_m2 = mGear.dm;\
double& d_Fa1 = mPinion.mSingleData.d_Fa;\
double& d_Fa2 = mGear.mSingleData.d_Fa;\
double& d_fE1 = mPinion.mSingleData.d_fe;\
double& d_fE2 = mGear.mSingleData.d_fe;\
double& d_Nf1 = mPinion.d_Nf;\
double& d_Nf2 = mGear.d_Nf;\
double& d_Ff1 = mPinion.mSingleData.d_Ff;\
double& d_Ff2 = mGear.mSingleData.d_Ff;\
double& d_Na1 = mPinion.d_Na;\
double& d_Na2 = mGear.d_Na;\
double& x1 = mPinion.x;\
double& x2 = mGear.x;\
double& h_fP1=mPinion.h_fP;\
double& h_fP2 = mGear.h_fP;\
double& q1=mPinion.DidmRatio;\
double& q2=mGear.DidmRatio;\
double& rho1=mPinion.Density;\
double& rho2=mGear.Density;\
double& d_w1 = mPinion.dw;\
double& d_w2 = mGear.dw;\
double& c_F1 = mPinion.c_F;\
double& c_F2 = mGear.c_F;\
double& g_f1 = mPinion.g_f; \
double& g_f2 = mGear.g_f; \
double& g_a1 = mPinion.g_a;\
double& g_a2 = mGear.g_a;\
double& rho_y1 = mPinion.rho_y;\
double& rho_y2 = mGear.rho_y;\
double& c1 = mPinion.c;\
double& c2 = mGear.c;\
double& zeta_f1 = mPinion.zeta_f;\
double& zeta_f2 = mGear.zeta_f;\
double& zeta1 = mPinion.zeta;\
double& zeta2 = mGear.zeta;\
double& K_ga1 = mPinion.K_ga;\
double& K_ga2 = mGear.K_ga;\
double& K_gf1 = mPinion.K_gf;\
double& K_gf2 = mGear.K_gf;\
double& v_ga1 = mPinion.v_ga;\
double& v_ga2 = mGear.v_ga;\
double& v_gf1 = mPinion.v_gf;\
double& v_gf2 = mGear.v_gf;\
double& phi_j1 = mPinion.phi_j;\
double& phi_j2 = mGear.phi_j;\
double& phi_gama1 = mPinion.phi_gama;\
double& phi_gama2 = mGear.phi_gama;\
double& phi_beta1 = mPinion.phi_beta;\
double& phi_beta2 = mGear.phi_beta;\
double& phi_alpha1 = mPinion.phi_alpha;\
double& phi_alpha2 = mGear.phi_alpha;\

// 模块职责：按对应重载读取 ZJ 版齿轮副几何、驱动及间隙参数。
void CInvoluteGearTAN::DefineGearPairGeometryZJ(double* InPinionData, double* InGearData, double* InPairCommonData, CInvoluteGearTAN& IOGearPair)
{
	mn = InPairCommonData[0];
	alpha_n = InPairCommonData[1];
	beta = abs(InPairCommonData[2]);
	IOGearPair.a_w = InPairCommonData[3];
	mPinion.DefineSingleGearGeometryZJ(InPairCommonData[0], InPairCommonData[1], beta, InPinionData, IOGearPair.mPinion);
	mGear.DefineSingleGearGeometryZJ(InPairCommonData[0], InPairCommonData[1], beta, InGearData, IOGearPair.mGear);
	IOGearPair.GearPairType = static_cast<int>(InPairCommonData[4]);
	IOGearPair.n_P = static_cast<int>(InPairCommonData[5]);
	IOGearPair.DrivingGear = static_cast<int>(InPairCommonData[6]);
	IOGearPair.j_bn = InPairCommonData[7];
	IOGearPair.n0 = InPairCommonData[8];
	IOGearPair.mGear.N_m = IOGearPair.n_P;
	IOGearPair.mPinion.N_m = IOGearPair.n_P;
}

// 模块职责：按对应重载读取 ZJ 版齿轮副几何、驱动及间隙参数。
void CInvoluteGearTAN::DefineGearPairGeometryZJ(double* InPinionData, double* InGearData, double* InPairCommonData)
{
	DefineGearPairGeometryZJ(InPinionData, InGearData, InPairCommonData, *this);
}

// 模块职责：按 ZJ 版流程计算公共几何、间隙、滑动与啮合参数。
void CInvoluteGearTAN::CalGearPairCommonGeomtryParaZJ()
{
	_CiteGearParaZJ;
	beta_b = mGear.beta_b;
	u = z2 / static_cast<double>(z1);
	//传动比计算
	CalGearTransmissionRatio();
	alpha_t = atan(tan(alpha_n) / cos(beta));
	alpha_wt = acos(abs(z1 + z2) * (mn * cos(alpha_t) / (2.0 * a_w * cos(beta))));//端面啮合角
	d_w1 = d_b1 / cos(alpha_wt);
	d_w2 = d_b2 / cos(alpha_wt); //d_w由于需要alpha_wt，而alpha_wt需要连两个齿数，故写在这里！
	//a_w = (d_w2 + d_w1 * mGear.IOMeshIndex) / 2;
	h_w = (d_a1 + d_a2 * mGear.IOMeshIndex) / 2 - a_w * mGear.IOMeshIndex;
	c1 = mGear.IOMeshIndex * (a_w - d_fE2 / 2) - d_a1 / 2;
	c2 = mGear.IOMeshIndex * (a_w - d_a2 / 2) - d_fE1 / 2;
	sigmax = x1 + x2;
	sigmax_E = sigmax - j_bn / (2 * mn * sin(alpha_n));

	mt = mn / cos(beta);
	pn = Pi * mn;
	pt = Pi * mt;
	pbt = pt * cos(alpha_t);
	pet = pbt;
	C_a = (mPinion.C_a + mGear.C_a) / 2;
	//接触线长度
	CalContactPathLengthZJ();
	c_F1 = (d_Nf1 - d_Ff1) * mGear.IOMeshIndex / 2;
	c_F2 = (d_Nf2 - d_Ff2) * mGear.IOMeshIndex / 2;
	//g_alpha = (sqrt(pow(d_Na1, 2) - pow(d_b1, 2)) + (sqrt(pow(d_Na2, 2) - pow(d_b2, 2)) - 2 * a_w * sin(alpha_wt)) * mGear.IOMeshIndex) / 2;
	g_f1 = (sqrt(pow(d_Na2, 2) - pow(d_b2, 2)) - d_b2 * tan(alpha_wt)) * mGear.IOMeshIndex / 2;
	g_a2 = g_f1;
	g_a1 = (sqrt(pow(d_Na1, 2) - pow(d_b1, 2)) - d_b1 * tan(alpha_wt)) * mGear.IOMeshIndex / 2;
	g_f2 = g_a1;
	//齿面曲率半径
	CalSurfaceCurvatureRadius();
	phi_alpha1 = 2 * g_alpha / d_b1;
	phi_alpha2 = 2 * g_alpha / d_b2;
	epsilon_alpha = phi_alpha2 / mGear.tau;
	phi_beta1 = 2 * min(mPinion.b_Design, mGear.b_Design) * sin(beta) / (mn * z1);
	phi_beta2 = 2 * min(mPinion.b_Design, mGear.b_Design) * sin(beta) / (mn * z2);
	epsilon_beta = phi_beta2 / mGear.tau;
	g_beta = min(mPinion.b_Design, mGear.b_Design) * tan(beta);
	phi_gama1 = phi_alpha1 + phi_beta1;
	phi_gama2 = phi_alpha2 + phi_beta2;
	epsilon_gamma = epsilon_alpha + epsilon_beta;
	double l_max1 = g_alpha / sin(beta_b);
	double l_max2 = min(mPinion.b_Design, mGear.b_Design) / cos(beta_b);
	l_max = min(l_max1, l_max2);
	phi_j1 = 2 * j_bn / (mn * z1 * cos(alpha_n));
	phi_j2 = 2 * j_bn / (mn * abs(z2) * cos(alpha_n));
	j_wt = j_bn / (cos(alpha_wt) * cos(beta_b));
	j_t = j_bn / (cos(beta) * cos(alpha_n));
	j_r = j_wt / (2 * tan(alpha_wt));
	omega1 = 2 * PI * n0 / 60000;
	v_n = omega1 * d_b1 / 2;
	//v_g = omega1 * (rho_y2 / u - rho_y1); // +-怎么确定？
	g_alphay = fabs(rho_C1 - rho_y1);
	v_g = fabs(omega1 * g_alphay * (1 + 1 / u));
	v_gf1 = fabs(omega1 * g_f1 * (1 + 1 / u));
	v_gf2 = fabs(omega1 * g_f2 * (1 + 1 / u));
	v_ga1 = fabs(omega1 * g_a1 * (1 + 1 / u));
	v_ga2 = fabs(omega1 * g_a2 * (1 + 1 / u));
	K_gf1 = -2 * g_f1 * (1 + 1 / u) / d_w1;
	K_gf2 = -2 * g_f2 * (1 + 1 / u) / d_w1;
	K_ga1 = 2 * g_a1 * (1 + 1 / u) / d_w1;
	K_ga2 = 2 * g_a2 * (1 + 1 / u) / d_w1;
	K_g = 2 * g_alphay * (1 + 1 / u) / d_w1;
	zeta1 = 1 - rho_y2 / (u * rho_y1);
	zeta2 = 1 - u * rho_y1 / rho_y2;
	rho_A1 = T1T2 - rho_A2;
	rho_E2 = T1T2 - rho_E1;
	zeta_f1 = 1 - rho_A2 / (u * rho_A1);
	zeta_f2 = 1 - u * rho_E1 / rho_E2;

	mPinion.g_f = g_f1;
	mGear.g_f = g_f2;
	mPinion.g_a = g_a1;
	mGear.g_a = g_a2;
	mPinion.dw = d_w1;
	mGear.dw = d_w2;
	mPinion.d_Na =d_Na1;
	mGear.d_Na = d_Na2;
	mPinion.d_Nf = d_Nf1;
	mGear.d_Nf = d_Nf2;
	mPinion.c_F = c_F1;
	mGear.c_F = c_F2;
	mPinion.c = c1;
	mGear.c = c2;
	mPinion.phi_alpha = phi_alpha1;
	mGear.phi_alpha = phi_alpha2;
	mPinion.phi_beta = phi_beta1;
	mGear.phi_beta = phi_beta2;
	mPinion.phi_gama = phi_gama1;
	mGear.phi_gama = phi_gama2;
	mPinion.phi_j = phi_j1;
	mGear.phi_j = phi_j2;
	mPinion.v_gf = v_gf1;
	mGear.v_gf = v_gf2;
	mPinion.v_ga = v_ga1;
	mGear.v_ga = v_ga2;
	mPinion.K_gf = K_gf1;
	mGear.K_gf = K_gf2;
	mPinion.K_ga = K_ga1;
	mGear.K_ga = K_ga2;
	mPinion.zeta = zeta1;
	mGear.zeta = zeta2;
	mPinion.zeta_f = zeta_f1;
	mGear.zeta_f = zeta_f2;


}

// 模块职责：按 ZJ 版流程计算工作齿宽、单齿轮及齿轮副几何。
void CInvoluteGearTAN::CalGearPairGeometryZJ()
{
	_CiteGearParaZJ
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

	mPinion.CalSingleGearBasicGeometryZJ();
	mGear.CalSingleGearBasicGeometryZJ();

	CalGearPairCommonGeomtryParaZJ();//在后，里面会用到单个的参数

}

// 模块职责：按 ZJ 版有效齿廓边界计算啮合线长度与重合度。
void CInvoluteGearTAN::CalContactPathLengthZJ()
{
	_CiteGearParaZJ
	d_Nf1 = sqrt(pow(2 * a_w * sin(alpha_wt) - mGear.IOMeshIndex * sqrt(pow(d_Fa2, 2) - pow(d_b2, 2)), 2) + pow(d_b1, 2));
	d_Nf2 = mGear.IOMeshIndex * sqrt(pow(2 * a_w * sin(alpha_wt) - sqrt(pow(d_Fa1, 2) - pow(d_b1, 2)), 2) + pow(d_b2, 2));

	if (d_Nf1 < d_Ff1)
	{
		d_Nf1 = d_Ff1;
	}
	if (d_Nf2 < d_Ff2)
	{
		d_Nf2 = d_Ff2;
	}

	if (abs(d_Nf1 - d_Ff1) < DefaultTolerance)
	{
		d_Na2 = sqrt(pow(2 * a_w * sin(alpha_wt) - sqrt(pow(d_Ff1, 2) - pow(d_b1, 2)), 2) + pow(d_b2, 2));
	}
	else
	{
		d_Na2 = d_Fa2;
	}
	if (abs(d_Nf2 - d_Ff2) < DefaultTolerance)
	{
		d_Na1 = sqrt(pow(2 * a_w * sin(alpha_wt) - mGear.IOMeshIndex * sqrt(pow(d_Ff2, 2) - pow(d_b2, 2)), 2) + pow(d_b1, 2));
	}
	else
	{
		d_Na1 = d_Fa1;
	}

	g_alpha = (sqrt(pow(d_Na1, 2) - pow(d_b1, 2)) + (sqrt(pow(d_Na2, 2) - pow(d_b2, 2)) - 2 * a_w * sin(alpha_wt)) * mGear.IOMeshIndex) / 2;
	////需要删除的部分
	//g_alpha = 37.84464;
	//重合度
	Xi_alpha = g_alpha / pet;
	Xi_beta = b*sin(beta) / (mn*Pi);
	Xi_Gama = Xi_alpha + Xi_beta;
}

// 模块职责：按驱动齿轮选项计算带方向的齿数传动比。
void CInvoluteGearTAN::CalGearTransmissionRatio()
{
	if (DrivingGear == 0)
	{
		i = -(mGear.Z / static_cast<double>(mPinion.Z));
	}
	else if (DrivingGear == 1)
	{
		i = -(mPinion.Z / static_cast<double>(mGear.Z));
	}
}

// 模块职责：计算关键啮合点的齿廓曲率半径及沿啮合线的距离。
void CInvoluteGearTAN::CalSurfaceCurvatureRadius()
{
	_CiteGearParaZJ;
	rho_C1 = d_b1 * tan(alpha_wt) / 2;
	T1C = rho_C1;
	rho_C2 = d_b2 * tan(alpha_wt) * mGear.IOMeshIndex / 2;
	T2C = rho_C2;
	rho_A2 = sqrt(pow(d_Na2, 2) - pow(d_b2, 2)) * mGear.IOMeshIndex / 2;
	T2A = rho_A2;
	rho_E1 = sqrt(pow(d_Na1, 2) - pow(d_b1, 2)) / 2;
	T1E = rho_E1;
	rho_B1 = rho_E1 - mGear.p_et; //pet随便用一个齿轮的来计算即可
	T1B = rho_B1;
	rho_D2 = rho_A2 - mGear.p_et;
	T2D = rho_D2;
	T1T2 = rho_C1 + rho_C2;
}

#undef _CiteGearParaZJ
