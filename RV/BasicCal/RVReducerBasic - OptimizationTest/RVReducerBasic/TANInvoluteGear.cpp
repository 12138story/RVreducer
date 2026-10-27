#include "TANInvoluteGear.h"
#include "InvoluteGearParameterAliases.h"

double NSInvoluteStaticArrary::C1 = 0.04723;
double NSInvoluteStaticArrary::C2 = 0.15551;
double NSInvoluteStaticArrary::C3 = 0.25791;
double NSInvoluteStaticArrary::C4 = -0.00635;
double NSInvoluteStaticArrary::C5 = -0.11654;
double NSInvoluteStaticArrary::C6 = -0.00193;
double NSInvoluteStaticArrary::C7 = -0.24188;
double NSInvoluteStaticArrary::C8 = 0.00529;
double NSInvoluteStaticArrary::C9 = 0.00182;
double NSInvoluteStaticArrary::GearOil_v40[10] = { 10,15,22,32,46,68,100,150,220,320 };
double NSInvoluteStaticArrary::GearOil_v50Coef[20] = { 7.5,0.609897,10.6,0.629699,15,0.613301,21,0.615416,30,0.635955,43,0.565632,61,0.564246,89,0.541157,125,0.50736,180,0.616679 };
double NSInvoluteStaticArrary::GearOil_vfCoef[20] = { 0.0068,0.00120649,0.0131,0.00131549,0.023,0.00153143,0.04,0.00184356,0.067,0.00193262,0.107,0.00171019,0.158,0.00150858,0.227,0.00120708,0.295,0.000804316,0.37,0.000782194 };
double NSInvoluteStaticArrary::GearStandardModuleFirst[28] = { 0.1,0.12,0.15,0.2,0.25,0.3,0.4,0.5,0.6,0.8,1,1.25,1.5,2,2.5,3,4,5,6,8,10,12,16,20,25,32,40,50 };
double NSInvoluteStaticArrary::GearStandardModuleSecond[22] = { 0.35,0.7,0.9,1.75,2.25,2.75,3.25,3.5,3.75,4.5,5.5,6.5,7,9,11,14,18,22,28,30,36,45 };
double NSInvoluteStaticArrary::GearStandardModule[50] = { 0.1,0.12,0.15,0.2,0.25,0.3,0.35,0.4,0.5,0.6,0.7,0.8,0.9,1,1.25,1.5,1.75,2,2.25,2.5,2.75,3,3.25,3.5,3.75,4,4.5,5,5.5,6,6.5,7,8,9,10,11,12,14,16,18,20,22,25,28,30,32,36,40,45,50 };

//double NSInvoluteStaticArrary::c_thp[9] = { 0.04723 ,0.15551,0.25791,-0.00635,-0.11654,-0.00193, };

CInvoluteGearTAN::CInvoluteGearTAN()
{
	mOilV40ToV50Curve.mPointCount = 10;
	mOilV40ToV50Curve.mPointDimension = 1;
	mOilV40ToV50Curve.mpVarList = &(NSInvoluteStaticArrary::GearOil_v40[0]);
	mOilV40ToV50Curve.mpSurfCoef = &(NSInvoluteStaticArrary::GearOil_v50Coef[0]);
	mOilV40ToVfCurve.mPointCount = 10;
	mOilV40ToVfCurve.mPointDimension = 1;
	mOilV40ToVfCurve.mpVarList = &(NSInvoluteStaticArrary::GearOil_v40[0]);
	mOilV40ToVfCurve.mpSurfCoef = &(NSInvoluteStaticArrary::GearOil_vfCoef[0]);
	mPinion.ShaftForceVector.Set(6);
	mGear.ShaftForceVector.Set(6);
	mGearPtrList.emplace_back(&mPinion);
	mGearPtrList.emplace_back(&mGear);
}

void CInvoluteGearTAN::DefineLoadPara(double InTorque, double InRPM, double InDesignLife, bool InTorqueIndex, bool InRPMIndex)
{
	if (InTorqueIndex)
	{
		mGear.T = abs(InTorque);
		mGear.TorqueDirect = NSTanBasiceFunction::Fsign(InTorque);
		mPinion.T = mPinion.Z / static_cast<double>(mGear.Z)*InTorque;
		mPinion.TorqueDirect = (mGear.IOMeshIndex)*mGear.TorqueDirect;
	}
	else
	{
		mPinion.T = abs(InTorque);
		mPinion.TorqueDirect =- NSTanBasiceFunction::Fsign(InTorque);
		mGear.T = mGear.Z / static_cast<double>(mPinion.Z)*InTorque;
		mGear.TorqueDirect = (mGear.IOMeshIndex)*mPinion.TorqueDirect;
	}
	if (InRPMIndex)
	{
		mGear.n = abs(InRPM);
		mPinion.n = mGear.Z / static_cast<double>(mPinion.Z)*mGear.n;
	}
	else
	{
		mPinion.n = abs(InRPM);
		mGear.n = mPinion.Z / static_cast<double>(mGear.Z)*mPinion.n;
	}
	mPinion.Lh = InDesignLife;
	mGear.Lh = InDesignLife;
}

void CInvoluteGearTAN::DefineLoadPara(double InTorque, double InRPM, double InDesignLife, bool InTorqueIndex, bool InRPMIndex, double InBigToSmallGearCountRatio)
{
	DefineLoadPara(InTorque, InRPM, InDesignLife, InTorqueIndex, InRPMIndex);
	mGear.Lh = InBigToSmallGearCountRatio * mPinion.Lh;
}

void CInvoluteGearTAN::DefineGearPairMaterial(double * InPinionPhysicalData, double * InPinionManufactureData, double * InGearPhysicalData, double * InGearManufactureData)
{
	mPinion.DefineMaterial(InPinionPhysicalData, InPinionManufactureData);
	mGear.DefineMaterial(InPinionPhysicalData, InPinionManufactureData);
}

void CInvoluteGearTAN::DefineGearPairApplicationData(double * InPinionData, double * InGearData, double* InCommonData)
{
	K_A = InCommonData[0];
	K_gama = InCommonData[1];	
	BearingSpanOffset = InCommonData[2];//BearingSpanOffset
	//HelixModificationType = static_cast<int>(InPinionData[4]);
	v_40 = InCommonData[3];
	HelixModificationQuality = static_cast<int>(InCommonData[4]);
	S_Hmin = InCommonData[5];
	S_Fmin = InCommonData[6];
	mPinion.DefineApplicationData(InPinionData);
	mGear.DefineApplicationData(InGearData);
}

void CInvoluteGearTAN::DefineGearPairTolerance(double * InPinionData, double * InGearData)
{
	mPinion.DefineProfileTolerence(InPinionData);
	mGear.DefineProfileTolerence(InGearData);
}

void CInvoluteGearTAN::DefineGearPairAssembleData(double * InData)
{
	AssembleType=static_cast<int>(InData[0]);//安装类型
	IsShaftStiffening = static_cast<int>(InData[1]);//是否为刚性轴，d1/d_sh<1.15为非刚性轴，键连接齿轮也非刚性
	d_sh = InData[2];//小轮轴弯曲变形当量直径，对于圆轴，就是轴直径
	PinionMeanPointToSpanMeanPointDistance = InData[3];//小轮齿宽中点至轴承跨距中点距离
	BearingSpanLength = InData[4];//小轮齿宽中点至轴承跨距中点距离
}

void CInvoluteGearTAN::CalBasicFactors()
{
	_CiteGearPara
	Z_epsilon = sqrt(1 / Xi_alpha);
	F_t = 2000 * mPinion.T / mPinion.d/ mPinion.N_m;
	//F_r = F_t * alpha_n / cos(beta);
	//F_a = F_t * tan(beta);
	//F_n = abs(F_t) / (cos(alpha_n)*cos(beta));
	mPinion.CalForce();
	mGear.CalForce();
	z_n1 = z1 /  (pow(cos(beta_b),2)* cos(beta));
	z_n2 = u * z_n1;
	using namespace NSInvoluteStaticArrary;
	q_p = C1 + C2 / z_n1 + C3 / z_n2 + C4 * x1 + C5 * x1 / z_n1 + C6 * x2 + C7 * x2 / z_n2 + C8 * x1 * x1 + C9 * x2 * x2;
	c_thp = 1 / q_p;
	C_M = 0.8;
	C_R = 1.0;
	alpha_Pn = alpha_n;
	//mPinion.C_B = (1.0 + 0.5 * (1.2 - h_fP1 / mn)) * (1.0 - 0.02 * (20.0*Degree- alpha_Pn));
	//mGear.C_B= (1.0 + 0.5 * (1.2 - h_fP1 / mn)) * (1.0 - 0.02 * (20.0*Degree - alpha_Pn));
	C_B = (mPinion.C_B + mGear.C_B) / 2;
	c_p = c_thp * C_M*C_R*C_B*cos(beta);
	double TempValue = K_A * F_t / b / 100;
	if (TempValue<1.0)
	{
		c_p = c_p * pow(TempValue, 0.25);
	}
	//c_p = 9.749;
	if ((Xi_alpha >= 1.2) && (beta < 30 * Degree))
	{
		c_gamaalpha = c_p * (0.75 * Xi_alpha + 0.25);
	}
	else
	{
		c_gamaalpha = 0.9 * c_p * (0.75 * Xi_alpha + 0.25);
	}
	c_gama_beta = 0.85*c_gamaalpha;
	CalDynamicFactorKV();
	CalFaceLoadFactorsKHbetaKFbeta();
	CalFaceLoadFactorsKHalphaKFalpha();
	//m_red = PI / 8.0 * (d_m1 * d_m1 / d_b1 / d_b1) * d_m1 * d_m1 / (1.0 / (1.0 - q1 * q1 * q1 * q1) / rho1 + 1.0 / (1.0 - q2 * q2 * q2 * q2) / rho2 / u / u);
}

void CInvoluteGearTAN::CalSurfaceDurability()
{
	_CiteGearPara
	Z_H = sqrt(2.0 * cos(beta_b) * cos(alpha_wt) / cos(alpha_t) / cos(alpha_t) / sin(alpha_wt));
	CalZBAndZD();
	//弹性系数
	Z_E = sqrt(1.0 / (Pi*((1.0 - pow(mPinion.Prxy, 2)) / mPinion.Ex + (1.0 - pow(mGear.Prxy, 2)) / mGear.Ex)));
	//接触比系数
	CalZXi();
	//螺旋角系数
	Z_beta = sqrt(1 / cos(beta));
	//名义接触应力，名义接触应力与转速无关
	sigma_H0 = Z_H * Z_E*Z_Xi*Z_beta*sqrt(F_t / (d_1*b)*(u + 1.0) / u);
	//K_v与K_K_Hbeta会受到转速影响
	//K_Hbeta = 1.968;
	//K_Halpha = 1.117;
	double Sigma0PlusK = sigma_H0 * sqrt(K_A*K_gama*K_v*K_Hbeta*K_Halpha);
	//计算接触应力,在转速0-10000RPM只变化100MPa;
	mPinion.sigma_H = Z_B * Sigma0PlusK;
	mGear.sigma_H = Z_D * Sigma0PlusK;
	//许用接触应力
	sigma_Hlim = min(mPinion.sigma_Hlim, mGear.sigma_Hlim);
	CalZLZVZR();
	CalZW();
	//尺寸系数，IOS标准直接等于1
	Z_X = 1;
	mPinion.Get_Z_NT();
	mGear.Get_Z_NT();
	double TempVar = Z_L * Z_v * Z_R * Z_X / S_Hmin;
	mPinion.sigma_HP = mPinion.sigma_Hlim*mPinion.Z_NT*mPinion.Z_W*TempVar;
	mGear.sigma_HP = mGear.sigma_Hlim*mGear.Z_NT*mGear.Z_W*TempVar;
	mPinion.S_H = mPinion.sigma_HP * S_Hmin / mPinion.sigma_H;
	mGear.S_H = mGear.sigma_HP * S_Hmin / mGear.sigma_H;
	//静态接触强度-没校核
	TempVar = Z_LStatic * Z_vStatic * Z_RStatic * Z_X / S_Hmin;
	mPinion.sigma_HPStatic = mPinion.sigma_Hlim*mPinion.Z_NTStatic*mPinion.Z_WStatic*TempVar;
	mGear.sigma_HPStatic = mGear.sigma_Hlim*mGear.Z_NTStatic*mGear.Z_WStatic*TempVar;
	mPinion.S_HStatic = mPinion.sigma_HPStatic * S_Hmin / mPinion.sigma_H;
	mGear.S_HStatic = mGear.sigma_HPStatic * S_Hmin / mGear.sigma_H;
}

void CInvoluteGearTAN::CalToothBendingStrength()
{
	Xi_alphan = Xi_alpha / pow(cos(beta_b), 2);
	//计算有效齿宽
	//如果两轮齿宽不一致,根据ISO6336-2019-3,p9的b解释，“最多”可以在齿宽的齿轮两侧各加1个法向模数宽度值作为有效齿宽，即加2*mn
	if (mPinion.b_Design< mGear.b_Design)
	{
		mPinion.b_eff = mPinion.b_Design;
		mGear.b_eff = min(mPinion.b_eff + 2 * mn, mGear.b_Design);
	}
	else
	{
		mGear.b_eff = mGear.b_Design;
		mPinion.b_eff = min(mGear.b_eff + 2 * mn, mPinion.b_Design);
		
	}
	double UnitForce = F_t / mPinion.b_eff;
	//double UnitForce = F_t / b;
	double TotalK = K_A * K_gama*K_v*K_Fbeta*K_Falpha;
	mPinion.CalBendingStress(UnitForce, TotalK, Xi_alpha, Xi_beta, Xi_alphan);
	UnitForce=F_t / mGear.b_eff;
	mGear.CalBendingStress(UnitForce, TotalK, Xi_alpha, Xi_beta, Xi_alphan);

}

void CInvoluteGearTAN::InterfaceCalGeometryAndStrength()
{
	CalGearPairGeometryZJ();
	//mPinion.DefineProfileTolerence(mPinion.ToleranceClass);
	//mGear.DefineProfileTolerence(mGear.ToleranceClass);
	mPinion.CalDerivedTolerence();
	mGear.CalDerivedTolerence();
	CalBasicFactors();
	CalSurfaceDurability();
	CalToothBendingStrength();
}

void CInvoluteGearTAN::CalForceVectorByRotAng(double InAssembleAng)
{
	double& Fr = mGear.F_r;
	double& Ft = mGear.F_t;
	//double Fa = mGear.F_a;
	mGear.ShaftForceVector[0] =  -mGear.TorqueDirect * (Fr * cos(InAssembleAng) - Ft * sin(InAssembleAng));
	mGear.ShaftForceVector[1] =  -mGear.TorqueDirect * (Ft * cos(InAssembleAng) + Fr * sin(InAssembleAng));
	mGear.ShaftForceVector[2] = mGear.F_a;
	CTanVector TempForceDist(3);
	CTanVector TempTorque(3);
	TempForceDist[0] = -mGear.d / 2*cos(InAssembleAng);
	TempForceDist[1] = -mGear.d / 2*sin(InAssembleAng);
	TempForceDist[2] = 0;
	TempForceDist.CrossProduct( mGear.ShaftForceVector,TempTorque);
	std::copy(TempTorque.begin(), TempTorque.end(), &(mGear.ShaftForceVector[3]));
	mPinion.ShaftForceVector[0] = - mGear.ShaftForceVector[0];
	mPinion.ShaftForceVector[1] = - mGear.ShaftForceVector[1];
	mPinion.ShaftForceVector[2] = - mGear.ShaftForceVector[2];
	TempForceDist[0] = mPinion.d / 2 * cos(InAssembleAng);
	TempForceDist[1] = mPinion.d / 2 * sin(InAssembleAng);
	TempForceDist[2] = 0;
	TempForceDist.CrossProduct(mPinion.ShaftForceVector, TempTorque);
	std::copy(TempTorque.begin(), TempTorque.end(), &(mPinion.ShaftForceVector[3]));
}


//void CInvoluteGearTAN::CalZNTByMethodB()
//{
//	mPinion.Get_Z_NT();
//	mGear.Get_Z_NT();
//}


