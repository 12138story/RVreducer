# SOURCE ENCODING: gbk
#include "TANInvoluteGear.h"
#define _CiteGearPara \
int& z1 = mPinion.Z;\
int& z2 = mGear.Z; \
double& z_n1 = mPinion.zn; \
double& z_n2 = mGear.zn;\
double& d_a1 = mPinion.da;\
double& d_a2 = mGear.da;\
double& d_b1 = mPinion.db;\
double& d_b2 = mGear.db;\
double& d_1 = mPinion.d;\
double& d_2 = mGear.d;\
double& d_m1 = mPinion.dm;\
double& d_m2 = mGear.dm;\
double& d_Fa1 = mPinion.d_Fa;\
double& d_Fa2 = mGear.d_Fa;\
double& d_Nf1 = mPinion.d_Nf;\
double& d_Nf2 = mGear.d_Nf;\
double& d_Ff1 = mPinion.d_Ff;\
double& d_Ff2 = mGear.d_Ff;\
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

void CInvoluteGearTAN::CSingleInvoluteTAN::DefineSingleGearGeometry(double Inmn, double Inalpha_n, double Inbeta, double* InOtherSingleGearData,  CSingleInvoluteTAN & IOSingleGearObj)
{
	IOSingleGearObj.Z = static_cast<int>(InOtherSingleGearData[0]);//齿数
	IOSingleGearObj.mn = Inmn;//参考法向模数
	IOSingleGearObj.alpha_n = Inalpha_n;//法向压力角
	IOSingleGearObj.beta =Inbeta;//螺旋角
	IOSingleGearObj.SpiralIndex= static_cast<int>(InOtherSingleGearData[1]);//旋向标识，-1左旋，1右旋
	IOSingleGearObj.b_Design = InOtherSingleGearData[2];//设计齿宽
	IOSingleGearObj.h_ax = InOtherSingleGearData[3];//齿顶高系数
	IOSingleGearObj.x = InOtherSingleGearData[4];//高变位系数 
	IOSingleGearObj.cx = InOtherSingleGearData[5];//顶隙系数
	IOSingleGearObj.Rhf_fPx = InOtherSingleGearData[6];//齿根圆角系数
	IOSingleGearObj.s_pr = InOtherSingleGearData[7];//挖根量,mm
	IOSingleGearObj.C_a = InOtherSingleGearData[8];//齿顶修形量，um
	IOSingleGearObj.C_f = InOtherSingleGearData[9];//齿根修形量,um
	IOSingleGearObj.X_E = InOtherSingleGearData[10];//切向变位系数
	IOSingleGearObj.k = static_cast<int>(InOtherSingleGearData[11]);//跨齿距
	IOSingleGearObj.h_k = InOtherSingleGearData[12];//齿顶倒角量，mm
	IOSingleGearObj.D_i = InOtherSingleGearData[13];//轮幅外圆直径
	IOSingleGearObj.IOMeshIndex = Z / abs(Z);
}

void CInvoluteGearTAN::CSingleInvoluteTAN::CalSingleGearBasicGeometry()
{


	h_fx = h_ax + cx;
	h_aP = h_ax * mn;
	h_fP = h_fx * mn;
	h_a = h_aP + x * mn;
	//h_f = h_fP - x * mn;
	rho_fP = Rhf_fPx * mn;
	d = abs(Z) * mt;
	db = d * cos(alpha_t);

	//double Test = (Wk - (mn * cos(alpha_n) * (Pi * (k - 0.5) + Z * invalpha_t)));
	//X_E = (Wk - (mn * cos(alpha_n) * (Pi * (k - 0.5) + Z * invalpha_t))) / (2.0 * mn * sin(alpha_n));
	Wk = 2 * X_E* mn * sin(alpha_n) + (mn * cos(alpha_n) * (Pi * (k - 0.5) + Z * invalpha_t));
	//基于齿条加工的齿根圆
	dfE = d - 2.0 * IOMeshIndex * (h_fP - X_E * mn);
	h_f = h_fP - X_E * mn;
	//名义齿根圆
	df = d - 2.0 * IOMeshIndex * h_f;
	da = d + 2.0 * IOMeshIndex * h_a;
	d_Fa = da - 2 * IOMeshIndex * h_k;
	d_Ff = sqrt(pow(d*sin(alpha_t) - 2 * (h_fP - X_E * mn - rho_fP * (1 - sin(alpha_n))) / sin(alpha_t), 2) + pow(db, 2));	
	//机械设计手册第六版3卷15-15
	//h_FfP = h_fP - Rho_fP * (1.0 - sin(alpha_n));
	//d_Ff= d - 2.0 * IOMeshIndex*(h_FfP - x * mn);
	dm = (da + df) / 2;
	rb = db / 2;
	rm = dm / 2;
	ra = da / 2;
	rf = df / 2;
	alpha_Pn = alpha_n;
	h = (da - df) / 2;
	h_FaP0 = h_fP - rho_fP * (1 - sin(alpha_n));
	C_B = (1.0 + 0.5 * (1.2 - h_fP / mn)) * (1.0 - 0.02 * (20.0 - alpha_Pn/Degree));
}



void CInvoluteGearTAN::CSingleInvoluteTAN::DefineMaterial(double * InPhysicalData, double * InManufactureData)
{
	//DefineMaterial(InPhysicalData, InManufactureData, *this);
	// sigma_02和sigma_s均表达同一意思，也就是弹性极限应力
	Ex = InPhysicalData[0];
	Prxy = InPhysicalData[1];
	sigma_s = InPhysicalData[2];
	Density = InPhysicalData[3];
	sigma_Hlim = InPhysicalData[4];
	sigma_Flim = InPhysicalData[5];
	sigma_02 = InPhysicalData[6];
	sigma_B = InPhysicalData[7];
	MaterialType =(int)InManufactureData[0];
	MaterialQuality = (int)InManufactureData[1];
	CaseHardness = InManufactureData[2];
	CoreHardness = InManufactureData[3];
	IsShotPeen = (int)InManufactureData[4];
	LimitedPittingAllowable = (int)InManufactureData[5];
	

	//double sigma_Flim = 430.0; //试验齿轮的弯曲疲劳极限
}

void CInvoluteGearTAN::CSingleInvoluteTAN::DefineApplicationData(double * InData)
{
	IsReverseBending = static_cast<int>(InData[0]);
	IsFavorableContactPostion = static_cast<int>(InData[1]);
	HelixModificationType = static_cast<int>(InData[2]);
	IsProfileModification = static_cast<int>(InData[3]);
	GearBlankType = static_cast<int>(InData[4]);//1-实体齿轮
	S_Hmin = InData[5];
	S_Fmin = InData[6];
}

void CInvoluteGearTAN::CSingleInvoluteTAN::DefineProfileTolerence(int InTolerenceClass)
{
	double TempValue = pow(1.4142135623730951, InTolerenceClass - 5);
	f_pt = round((0.001*d+0.4*mn+5)*TempValue *2)/2.0;
	f_falpha = round(2*(0.55*mn+5)*TempValue)/2.0;
	f_Hbeta = round(2*(0.05*sqrt(d)+0.35*sqrt(b_Design)+4)*TempValue)/2.0;
}
void CInvoluteGearTAN::CSingleInvoluteTAN::DefineProfileTolerence(double* InInpuData,double* OutValueList)
{
	int TmpTolerenceClass = static_cast<int> (InInpuData[0]);
	double& Tmpmn = InInpuData[1];
	double& Tmpb_Design = InInpuData[2];
	double& TmpZ = InInpuData[3];//齿数
	double& Tmpbeta = InInpuData[4];//螺旋角
	double& Tempf_pt = OutValueList[0];
	double& Tempf_falpha = OutValueList[1];
	double& Tempf_Hbeta = OutValueList[2];
	double Tmpd = Tmpmn * TmpZ/cos(abs(Tmpbeta));
	double TempValue = pow(1.4142135623730951, TmpTolerenceClass - 5);
	Tempf_pt = round((0.001 * Tmpd + 0.4 * Tmpmn + 5) * TempValue * 2) / 2.0;
	Tempf_falpha = round(2 * (0.55 * Tmpmn + 5) * TempValue) / 2.0;
	Tempf_Hbeta = round(2 * (0.05 * sqrt(Tmpd) + 0.35 * sqrt(Tmpb_Design) + 4) * TempValue) / 2.0;
}

void CInvoluteGearTAN::CSingleInvoluteTAN::DefineProfileTolerence(double * InTolerenceList)
{
	ToleranceClass = static_cast<int>(InTolerenceList[0]);
	f_pt = InTolerenceList[1];
	f_falpha = InTolerenceList[2];
	f_Hbeta = InTolerenceList[3];
	R_aH = InTolerenceList[4];
	R_aF = InTolerenceList[5];
	R_zH = R_aH * 6;
}

void CInvoluteGearTAN::CSingleInvoluteTAN::CalDerivedTolerence()
{
	f_pb= f_pt * cos(alpha_t);
}

void CInvoluteGearTAN::CSingleInvoluteTAN::CalEquivalentMass()
{
	DidmRatio = D_i / dm;
	J_r = abs(Pi / 32. * Density*b_Design*(1 - pow(DidmRatio, 4))* pow(dm, 4));
	EquivalentMass = J_r / (b_Design*pow(rb, 2));
}

double CInvoluteGearTAN::CSingleInvoluteTAN::Finv(double InAlpha)
{
	return tan(InAlpha)-InAlpha;
}

double CInvoluteGearTAN::CSingleInvoluteTAN::CalMinProfileShift()
{
	//z_min = 2 * h_ax / pow(sin(alpha_t), 2);
	////double zv=Z/ pow(cos(beta), 3);
	//x_min = h_ax * (z_min - Z) / z_min;
	x_Emin = h_FaP0 / mn - (Z * pow(sin(alpha_t), 2)/cos(beta)) / 2;
	//x_min =  (2/tan(alpha_t) - zv*sin(alpha_t)) / 2;
	return x_Emin;
}

double CInvoluteGearTAN::CSingleInvoluteTAN::CalMaxProfileShift()
{
	double tmpSa = 0.25 * mt;
	void** pTempOtherCoefPtr = new void* [2];
	pTempOtherCoefPtr[0] = static_cast<void*> (this);
	pTempOtherCoefPtr[1] = static_cast<void*> (&tmpSa);
	double tmpX = 0.5;
	void* SolveFunPtr = static_cast<void*>(&AuxiliaryFunctionForMaxProfileShift);
	int OtherCoefCount = 2;
	x_EMax = TANFINDROOTBYITERATIONFORC(SolveFunPtr, &(tmpX), &OtherCoefCount, pTempOtherCoefPtr);
	delete[] pTempOtherCoefPtr;
	return x_EMax;
}

void CInvoluteGearTAN::CSingleInvoluteTAN::CalForce()
{
	F_t = 2000 * T / d/ N_m;
	F_r = F_t * alpha_n / cos(beta);
	F_a = TorqueDirect*SpiralIndex*F_t * tan(beta);
	F_n = F_t / (cos(alpha_n)*cos(beta));
}

double CInvoluteGearTAN::CSingleInvoluteTAN::Get_y_alpha(double In_f_pb,double InV)
{
	double y_alpha = 0.;
	if (MaterialType == 1 || MaterialType == 2 || MaterialType == 3 || MaterialType == 4 || MaterialType == 5 || MaterialType == 8 || MaterialType == 9)//条件1，1
	{
		if (InV <= 5)
		{
			y_alpha = 160.0 / sigma_Hlim * In_f_pb;//跑合量
		}
		if (5 < InV && InV <= 10)
		{
			double y_alpha_fpb_1 = 0.;
			if (In_f_pb > 80)
			{
				y_alpha_fpb_1 = 80.0;
			}
			y_alpha = 160.0 / sigma_Hlim * y_alpha_fpb_1;//更新跑合量
			if (y_alpha > 12800 / sigma_Hlim)
			{
				y_alpha = 12800.0 / sigma_Hlim;
			}
		}
		if (InV > 10)
		{
			double y_alpha_fpb_2 = 0.;
			if (In_f_pb > 40)
			{
				y_alpha_fpb_2 = 40.0;
			}
			y_alpha = 160.0 / sigma_Hlim * y_alpha_fpb_2;//更新跑合量
			if (y_alpha > 6400 / sigma_Hlim)
			{
				y_alpha = 6400.0 / sigma_Hlim;
			}
		}
	}
	else if (MaterialType == 6 || MaterialType == 7)//条件2
	{
		if (InV <= 5)
		{
			y_alpha = 0.275 * In_f_pb;
		}
		if (5 < InV && InV <= 10)
		{
			double y_alpha_fpb_3 = 0.;
			if (In_f_pb > 80)
			{
				y_alpha_fpb_3 = 80.0;
			}
			y_alpha = 0.275 * y_alpha_fpb_3;//更新跑合量
			if (y_alpha > 22)
			{
				y_alpha = 22.0;
			}
		}
		if (InV > 10)
		{
			double y_alpha_fpb_4 = 0.;
			if (In_f_pb > 40)
			{
				y_alpha_fpb_4 = 40.0;
			}
			y_alpha = 0.275 * y_alpha_fpb_4;
			if (y_alpha > 11)
			{
				y_alpha = 11.0;
			}
		}
	}
	else if (MaterialType == 10 || MaterialType == 11 || MaterialType == 12 || MaterialType == 13 || MaterialType == 14)//条件3
	{
		double y_alpha_fpb_5 = 0.;
		if (In_f_pb > 40)
		{
			y_alpha_fpb_5 = 40.0;
		}
		y_alpha = 0.075 * y_alpha_fpb_5;//更新跑合量
		if (y_alpha > 3)
		{
			y_alpha = 3.0;
		}
		y_alpha = 0.075 * In_f_pb;
	}

	return y_alpha;
}

double CInvoluteGearTAN::CSingleInvoluteTAN::Get_y_beta(int Material, double F_beta_x, double InV)
{	
	double y_beta = 0.;
	if (Material == SteelSt || Material == SteelStCast || Material == SteelV || Material == SteelVCast || Material == SteelGGGperl || Material == SteelGGGbai || Material == SteelGTSperl)//条件1
	{

		if (InV <= 5)
		{
			y_beta = 320.0 / sigma_Hlim * F_beta_x;
		}
		if (5 < InV && InV <= 10)
		{
			double F_beta_x1 = 0.;
			if (F_beta_x > 80)
			{
				F_beta_x1 = 80.0;
			}
			y_beta = 320.0 / sigma_Hlim * F_beta_x1;
			if (y_beta > 25600 / sigma_Hlim)
			{
				y_beta = 25600.0 / sigma_Hlim;
			}
		}
		if (InV > 10)
		{
			double F_beta_x2 = 0.;
			if (F_beta_x > 40)
			{
				F_beta_x2 = 40.0;
			}
			y_beta = 320.0 / sigma_Hlim * F_beta_x2;
			if (y_beta > 12800 / sigma_Hlim)
			{
				y_beta = 12800.0 / sigma_Hlim;
			}
		}
	}
	else if (Material == SteelGGGferr || Material == SteelGG)//条件2
	{
		if (InV <= 5)
		{
			y_beta = 0.55 * F_beta_x;
		}
		if (5 < InV && InV <= 10)
		{
			double F_beta_x3 = 0.;
			if (F_beta_x > 80)
			{
				F_beta_x3 = 80.0;
			}
			y_beta = 0.55 * F_beta_x3;
			if (y_beta > 45)
			{
				y_beta = 45.0;
			}
		}
		if (InV > 10)
		{
			double F_beta_x4 = 0.;
			if (F_beta_x > 40)
			{
				F_beta_x4 = 40.0;
			}
			y_beta = 0.55 * F_beta_x4;
			if (y_beta > 22)
			{
				y_beta = 22.0;
			}
		}
	}
	else if (Material == SteelEh || Material == SteelIF || Material == SteelNTnitr || Material == SteelNVnitr || Material == SteelNVnitrcar)//条件3
	{
		double F_beta_x4 = 0.;
		if (F_beta_x > 40)
		{
			F_beta_x4 = 40.0;
		}
		y_beta = 0.15 * F_beta_x4;
		if (y_beta > 6)
		{
			y_beta = 6.0;
		}
		y_beta = 0.15 * F_beta_x;
	}
	return y_beta;
}

double CInvoluteGearTAN::CSingleInvoluteTAN::get_x_beta(int Material)
{
	double X_beta = 0.;
	if (Material == SteelSt || Material == SteelStCast || Material == SteelV || Material == SteelVCast || Material == SteelGGGperl || Material == SteelGGGbai || Material == SteelGTSperl)//条件1
	{
		X_beta = 1 - 320.0 / sigma_Hlim;
	}
	else if ((Material == SteelGGGferr) || (Material == SteelGG))//条件2
	{
		X_beta = 0.45;
	}
	else if ((Material == SteelEh) || (Material == SteelIF) || (Material == SteelNTnitr) || (Material == SteelNVnitr) || (Material == SteelNVnitrcar))//条件3
	{
		X_beta = 0.85;
	}
	return X_beta;
}

double CInvoluteGearTAN::CSingleInvoluteTAN::Get_Rho(double InAlpha_wt)
{
	Rho = 0.5*db*tan(InAlpha_wt);
	return Rho;
}

void CInvoluteGearTAN::CSingleInvoluteTAN::Get_Z_NT()
{
	N_L=n * 60 * Lh;
	double Z_NTref1;
	double Z_NTref2;
	double N_Lref1;
	double N_Lref2;
	bool IsInterp = 0;
	if (MaterialType==SteelGG|| MaterialType == SteelGGGferr||MaterialType==SteelNTnitr|| MaterialType == SteelNVnitr)
	{
		Z_NTStatic = 1.3;
		if (N_L<=1e5)
		{
			Z_NT = 1.3;
		}
		else if (N_L > 1e10)
		{
			Z_NT = 0.85;
		}
		else if (N_L <= 2e6)
		{
			Z_NTref1 = 1.3;
			Z_NTref2 = 1.0;
			N_Lref1 = 1e5;
			N_Lref2 = 2e6;
			IsInterp = 1;
		}
		else
		{
			N_Lref1 = 2e6;
			N_Lref2 = 1e10;
			Z_NTref1 = 1.0;
			Z_NTref2 = 0.85;
			IsInterp = 1;
		}
	}
	else if (MaterialType == SteelNVnitrcar)
	{
		Z_NTStatic = 1.1;
		if (N_L <= 1e5)
		{
			Z_NT = 1.1;
		}
		else if (N_L > 1e10)
		{
			Z_NT = 0.85;
		}
		else if (N_L <= 2e6)
		{
			N_Lref1 = 1e5;
			N_Lref2 = 2e6;
			Z_NTref1 = 1.1;
			Z_NTref2 = 1.0;
			IsInterp = 1;
		}
		else
		{
			N_Lref1 = 2e6;
			N_Lref2 = 1e10;
			Z_NTref1 = 1.0;
			Z_NTref2 = 0.85;
			IsInterp = 1;
		}
	}
	else
	{
		if (LimitedPittingAllowable==0)//不允许有限点蚀
		{
			Z_NTStatic = 1.6;
			if (N_L <= 1e5)
			{
				Z_NT = 1.6;
			}
			else if (N_L > 1e10)
			{
				Z_NT = 0.85;
			}
			else if ( N_L <= 5e7 )
			{
				N_Lref1 = 1e5;
				N_Lref2 = 5e7;
				Z_NTref1 = 1.6;
				Z_NTref2 = 1.0;
				IsInterp = 1;
			}
			else
			{
				N_Lref1 = 5e7;
				N_Lref2 = 1e10;
				Z_NTref1 = 1.0;
				Z_NTref2 = 0.85;
				IsInterp = 1;
			}
		}
		else
		{
			Z_NTStatic = 1.6;
			if (N_L <= 6e5)
			{
				Z_NT = 1.6;
			}
			else if (N_L > 1e10)
			{
				Z_NT = 0.85;
			}
			else if (N_L <= 1e7)
			{
				N_Lref1 = 6e5;
				N_Lref2 = 1e7;
				Z_NTref1 = 1.6;
				Z_NTref2 = 1.3;
				IsInterp = 1;
			}
			else if (N_L <= 1e9)
			{
				N_Lref1 = 1e7;
				N_Lref2 = 1e9;
				Z_NTref1 = 1.3;
				Z_NTref2 = 1.0;
				IsInterp = 1;
			}
			else
			{
				N_Lref1 = 1e9;
				N_Lref2 = 1e10;
				Z_NTref1 = 1.0;
				Z_NTref2 = 0.85;
				IsInterp = 1;
			}
		}
	}
	if (IsInterp)
	{
		//double TempVar = (log(Z_NTref2) - log(Z_NTref1)) / (log(N_Lref2) - log(N_Lref1));
		//Z_NT = pow(N_L, TempVar) / pow(N_Lref1, TempVar);
		Z_NT = NSTanBasiceFunction::FLog10InterpByTwoValue(N_L, N_Lref1, N_Lref2, Z_NTref1, Z_NTref2);
	}

}

void CInvoluteGearTAN::CSingleInvoluteTAN::Get_YF(double InXi_alpha, double InXi_beta,double InXi_alpha_n)
{
	EE= Pi * mn / 4.0 - h_fP * tan(alpha_n ) + s_pr / cos(alpha_n ) - (1.0 - sin(alpha_n )) * rho_fP / cos(alpha_n );
	GG= rho_fP / mn - h_fP / mn + x;
	HH = 2.0 / zn * (Pi / 2.0 - EE / mn) - Pi / 3.0;
	double* pTemptheta = new double[2]{ -Pi / 2.0 + 0.00001, Pi / 2.0 - 0.00001 };
	void* SolveFunPtr = static_cast<void*>(&AuxiliaryFunctionForthetaInYF);
	int OtherCoefCount = 3;
	void** pTempOtherCoefPtr = new void*[3];
	pTempOtherCoefPtr[0] = static_cast<void*> (&GG);
	pTempOtherCoefPtr[1] = static_cast<void*> (&HH);
	pTempOtherCoefPtr[2] = static_cast<void*> (&zn);
	//std::cout << AuxiliaryFunctionForthetaInYF(&pTemptheta[0], pTempOtherCoefPtr) << std::endl;
	//std::cout << AuxiliaryFunctionForthetaInYF(&pTemptheta[1], pTempOtherCoefPtr) << std::endl;
	theta = TANFINDROOTBYBISECTIONFORC(SolveFunPtr, pTemptheta, &OtherCoefCount, pTempOtherCoefPtr);
	//CalVirtualTooth(0,0);
	delete[] pTemptheta;
	delete[] pTempOtherCoefPtr;
	double& Xi_alphan = InXi_alpha_n;
	d_n = zn * mn;
	p_bn = Pi * mn * cos(alpha_n);
	d_bn = d_n * cos(alpha_n);
	//需删除
	//da = 159.66;
	/*da = 872.35;*/
	d_an = d_n + da - d;
	d_en = 2.0 * IOMeshIndex * sqrt(pow(sqrt(d_an * d_an / 4.0 - d_bn * d_bn / 4) - PI * d * cos(beta) * cos(alpha_n) * (Xi_alphan - 1.0) / abs(Z), 2) + d_bn * d_bn / 4.0);
	alpha_en = acos(d_bn / d_en);
	double invalpha_en = Finv(alpha_en);
	gama_e = ((0.5 * PI + 2.0 * x * tan(alpha_n)) / zn + Finv(alpha_n) - invalpha_en);
	alpha_Fen = alpha_en - gama_e;
	s_Fn = mn * (zn * sin(PI / 3 - theta) + sqrt(3) * (GG / cos(theta) - rho_fP / mn));
	rho_F= mn * (rho_fP / mn + 2.0 * GG * GG / cos(theta) / (zn * (cos(theta) * cos(theta)) - 2.0 * GG));
	h_Fe = mn * (0.5 * ((cos(gama_e) - sin(gama_e) * tan(alpha_Fen)) * d_en / mn - zn * cos(PI / 3.0 - theta) - (GG / cos(theta) - rho_fP / mn)));
	if (abs(InXi_beta)<DefaultTolerance)
	{
		if (Xi_alphan<2.0)
		{
			f_Xi = 1.0;
		}
		else
		{
			f_Xi = 0.7;
		}
	}
	else if (InXi_beta>=1.0)
	{
		f_Xi = pow(Xi_alphan, -0.5);
	}
	else
	{
		if (Xi_alphan < 2.0)
		{
			f_Xi =pow(1.0- InXi_beta+ InXi_beta/ Xi_alphan,0.5);
		}
		else
		{
			f_Xi = pow((1.0 - InXi_beta)/2 + InXi_beta / Xi_alphan, 0.5);
		}
	}
	Y_F = 6.0 * h_Fe / mn * cos(alpha_Fen) / pow((s_Fn / mn),2)/ cos(alpha_n)*f_Xi;
}

void CInvoluteGearTAN::CSingleInvoluteTAN::Get_YS()
{
	q_s = s_Fn / 2 / rho_F;
	double LL = s_Fn / h_Fe;
	double RelofY_S = 1.0 / (1.21 + 2.3 / LL);
	if (q_s>=1.0&&q_s<8)
	{
		Y_S= (1.2 + 0.13 * LL) * pow(q_s, RelofY_S / 1.0);
	}
	else
	{
		Y_S = (1.2 + 0.13 * LL) * pow(q_s, RelofY_S / 1.0);
		std::cout << "Y_S应用超范围" << std::endl;
	}
	
}

void CInvoluteGearTAN::CSingleInvoluteTAN::Get_Ybeta(double InXi_beta)
{
	double TempXi_beta = min(InXi_beta, 1.0);
	double TempBeta=min(beta, 30*Degree);
	Y_beta = (1 - TempXi_beta * TempBeta / (120.0*Degree))/pow(cos(TempBeta),3);
}

void CInvoluteGearTAN::CSingleInvoluteTAN::Get_YB()
{
	s_R = abs((D_i - df) / 2);
	Iss_RTooSmall = 0;
	if (IOMeshIndex==1)//外啮合
	{
		double s_Rht = s_R / h;
		if (s_Rht>=1.2)
		{
			Y_B = 1.0;
		}
		else if (s_Rht>0.5)
		{
			Y_B = 1.6*log(2.242/ s_Rht);
		}
		else
		{
			Y_B = 1.6*log(2.242 / s_Rht);
			Iss_RTooSmall = 1;
		}
	}
	else//内啮合
	{
		double s_Rmn = s_R / mn;
		if (s_Rmn >= 3.5)
		{
			Y_B = 1.0;
		}
		else if (s_Rmn > 1.75)
		{
			Y_B = 1.6*log(8.324 / s_Rmn);
		}
		else
		{
			Y_B = 1.6*log(8.324 / s_Rmn);
			Iss_RTooSmall = 1;
		}
	}

	
}

void CInvoluteGearTAN::CSingleInvoluteTAN::Get_YDT(double InXi_alpha_n)
{
	if (ToleranceClass>4)
	{
		Y_DT = 1.0;
	}
	else
	{
		if (InXi_alpha_n<=2.05)
		{
			Y_DT = 1.0;
		}
		else if (InXi_alpha_n<=2.5)
		{
			Y_DT = -0.666*InXi_alpha_n + 2.366;
		}
		else
		{
			Y_DT = 0.7;
		}
	}
}
/*1.耐久性极限下的相对齿根圆角敏感系数Y_deltarelT
  2.静强度的相对齿根圆角敏感系数Y_deltarelT*/
void CInvoluteGearTAN::CSingleInvoluteTAN::Get_YdeltarefT()
{
	//持久寿命Y_deltarelT
	double Chi_Px = 0.2;
	double Chi_x = Chi_Px * (1.0 + 2.0 * q_s);
	double Chi_Tx= Chi_Px * (1.0 + 2.0 * 2.5);
	double Rho_p;
	if (MaterialType==SteelGG)
	{
		if (sigma_B <= 150.0)
		{
			Rho_p = 0.3124;
		}
		else
		{
			Rho_p = 0.3095;
		}		
	}
	else if (MaterialType == SteelGGGferr)
	{
		Rho_p = 0.3095;
	}
	else if (MaterialType== SteelNTnitr|| MaterialType == SteelNVnitr|| MaterialType == SteelNVnitrcar)
	{
		Rho_p = 0.1005;
	}
	else if (MaterialType== SteelSt|| MaterialType == SteelStCast)
	{
		if (sigma_s<=300)
		{
			Rho_p = 0.0833;
		}
		else
		{
			Rho_p = 0.0445;
		}
	}
	else if (MaterialType == SteelV|| MaterialType == SteelVCast|| MaterialType == SteelGTSperl|| MaterialType == SteelGGGperl|| MaterialType == SteelGGGbai)
	{
		if (sigma_s <= 500)
		{
			Rho_p = 0.0281;
		}
		else if (sigma_s <= 600)
		{
			Rho_p = 0.0194;
		}
		else if (sigma_s <= 800)
		{
			Rho_p = 0.0064;
		}
		else
		{
			Rho_p = 0.0014;
		}
	}
	else
	{
		Rho_p = 0.0030;
	}
	Y_deltarelT =(1 + sqrt(Rho_p * Chi_x)) / (1 + sqrt(Rho_p * Chi_Tx));
	double TempSqrt;
	if (MaterialType== SteelSt|| MaterialType == SteelStCast)
	{
		TempSqrt = pow(200.0 / sigma_s, 0.25);
		Y_deltarelTStatic= (1.0 + 0.93 * (Y_S - 1.0) * TempSqrt) / (1 + 0.93 * TempSqrt);
	}
	else if (MaterialType == SteelV || MaterialType == SteelVCast || MaterialType == SteelGGGperl || MaterialType == SteelGGGbai)
	{
		TempSqrt = pow(300.0 / sigma_s, 0.25);
		Y_deltarelTStatic = (1.0 + 0.82 * (Y_S - 1.0) * TempSqrt) / (1 + 0.82 * TempSqrt);
	}
	else if (MaterialType == SteelEh || MaterialType == SteelIF )
	{
		Y_deltarelTStatic = 0.44 * Y_S + 0.12;
	}
	else if (MaterialType == SteelNTnitr || MaterialType == SteelNVnitr|| MaterialType == SteelNVnitrcar)
	{
		Y_deltarelTStatic = 0.20 * Y_S + 0.60;
	}
	else if (MaterialType == SteelGTSperl)
	{
		Y_deltarelTStatic = 0.075 * Y_S + 0.85;
	}
	else
	{
		Y_deltarelTStatic = 1.0;
	}


}

void CInvoluteGearTAN::CSingleInvoluteTAN::Get_YRrefT()
{
	R_zF = 6.0 * R_aF;
	if (MaterialType == SteelSt || MaterialType == SteelStCast)
	{
		if (R_zF <= 1.0)
		{
			Y_RrefT = 1.07;
		}
		else
		{
			Y_RrefT = 5.306 - 4.203 * pow(R_zF + 1, 0.01);
		}
	}
	else if (MaterialType == SteelGG || MaterialType == SteelGGGferr || MaterialType == SteelNTnitr || MaterialType == SteelNVnitr || MaterialType == SteelNVnitrcar)
	{
		if (R_zF <= 1.0)
		{
			Y_RrefT = 1.025;
		}
		else
		{
			Y_RrefT = 4.229 - 3.259 * pow(R_zF + 1, 0.005);
		}		
	}
	else
	{
		if (R_zF <= 1.0)
		{
			Y_RrefT = 1.12;
		}
		else
		{
			Y_RrefT = 1.674 - 0.529 * pow(R_zF + 1, 0.1);
		}
		
	}
	Y_RrefTStatic = 1.0;
}

void CInvoluteGearTAN::CSingleInvoluteTAN::Get_YX()
{
	if (MaterialType == SteelEh || MaterialType == SteelIF || MaterialType == SteelNTnitr || MaterialType == SteelNVnitr || MaterialType == SteelNVnitrcar)
	{
		if (mn <= 5)
		{
			Y_X = 1.0;
		}
		else if (mn <= 25)
		{
			Y_X = 1.05 - 0.01 * mn;
		}
		else
		{
			Y_X = 0.8;
		}
	}
	else if (MaterialType == SteelGG || MaterialType == SteelGGGferr)
	{
		if (mn <= 5)
		{
			Y_X = 1.0;
		}
		else if (mn <= 25)
		{
			Y_X = 1.75 - 0.015 * mn;
		}
		else
		{
			Y_X = 0.7;
		}
	}
	else
	{
		if (mn <= 5)
		{
			Y_X = 1.0;
		}
		else if (mn <= 25)
		{
			Y_X = 1.03 - 0.006 * mn;
		}
		else
		{
			Y_X = 0.85;
		}
	}
	Y_XStatic = 1.0;
}

void CInvoluteGearTAN::CSingleInvoluteTAN::Get_YNT()
{
	double Y_NTRef1;
	double Y_NTRef2;
	double N_LRef1;
	double N_LRef2;
	bool IsInterp=0;
	IsInterp = 0;
	if (MaterialType==SteelNVnitrcar)
	{
		if (N_L<=1e3)
		{
			Y_NT = 1.1;
		}
		else if (N_L>=1e10)
		{
			Y_NT = 0.85;
		}
		else if (N_L<=3e6)
		{
			IsInterp = 1;
			Y_NTRef1 = 1.1;
			Y_NTRef2 = 1.0;
			N_LRef1 = 1e3;
			N_LRef2 = 3e6;
		}
		else
		{
			IsInterp = 1;
			Y_NTRef1 = 1.0;
			Y_NTRef2 = 0.85;
			N_LRef1 = 3e6;
			N_LRef2 = 1e10;
		}
		Y_NTStatic = 1.1;
	}
	else if (MaterialType == SteelGG|| MaterialType == SteelGGGferr|| MaterialType == SteelNTnitr|| MaterialType == SteelNVnitr)
	{
		if (N_L <= 1e3)
		{
			Y_NT = 1.6;
		}
		else if (N_L >= 1e10)
		{
			Y_NT = 0.85;
		}
		else if (N_L <= 3e6)
		{
			IsInterp = 1;
			Y_NTRef1 = 1.6;
			Y_NTRef2 = 1.0;
			N_LRef1 = 1e3;
			N_LRef2 = 3e6;
		}
		else
		{
			IsInterp = 1;
			Y_NTRef1 = 1.0;
			Y_NTRef2 = 0.85;
			N_LRef1 = 3e6;
			N_LRef2 = 1e10;
		}
		Y_NTStatic = 1.6;
	}
	else if (MaterialType == SteelEh || MaterialType == SteelIF)
	{
		if (N_L <= 1e3)
		{
			Y_NT = 2.5;
		}
		else if (N_L >= 1e10)
		{
			Y_NT = 0.85;
		}
		else if (N_L <= 3e6)
		{
			IsInterp = 1;
			Y_NTRef1 = 2.5;
			Y_NTRef2 = 1.0;
			N_LRef1 = 1e3;
			N_LRef2 = 3e6;
		}
		else
		{
			IsInterp = 1;
			Y_NTRef1 = 1.0;
			Y_NTRef2 = 0.85;
			N_LRef1 = 3e6;
			N_LRef2 = 1e10;
		}
		Y_NTStatic = 2.5;
	}
	else
	{
		if (N_L <= 1e4)
		{
			Y_NT = 2.5;
		}
		else if (N_L >= 1e10)
		{
			Y_NT = 0.85;
		}
		else if (N_L <= 3e6)
		{
			IsInterp = 1;
			Y_NTRef1 = 2.5;
			Y_NTRef2 = 1.0;
			N_LRef1 = 1e3;
			N_LRef2 = 3e6;
		}
		else
		{
			IsInterp = 1;
			Y_NTRef1 = 1.0;
			Y_NTRef2 = 0.85;
			N_LRef1 = 3e6;
			N_LRef2 = 1e10;
		}
		Y_NTStatic = 2.5;
	}
	if (IsInterp)
	{
		Y_NT = NSTanBasiceFunction::FLog10InterpByTwoValue(N_L, N_LRef1, N_LRef2, Y_NTRef1, Y_NTRef2);
	}
}

void CInvoluteGearTAN::CSingleInvoluteTAN::CalBendingStress(double InUnitiF, double InTotalK, double InXi_alpha, double InXi_beta, double InXi_alphan)
{
	Get_YF(InXi_alpha, InXi_beta, InXi_alphan);
	Get_YS();
	Get_YB();
	Get_Ybeta(InXi_beta);
	Get_YDT(InXi_alphan);
	//齿形系数与转速和载荷无关
	sigma_F0 = InUnitiF * Y_F * Y_S * Y_beta * Y_B * Y_DT  / mn;
	sigma_F = InTotalK * sigma_F0;
	CalSafetyCoef();
}

void CInvoluteGearTAN::CSingleInvoluteTAN::CalSafetyCoef()
{
	Y_ST = 2.0;
	Get_YdeltarefT();
	Get_YRrefT();
	Get_YX();
	Get_YNT();
	sigma_FP = sigma_Flim * Y_ST*Y_NT * Y_deltarelT * Y_RrefT* Y_X / S_Fmin;
	sigma_FPStatic = sigma_Flim * Y_ST*Y_NTStatic * Y_deltarelTStatic * Y_RrefTStatic* Y_XStatic / S_Fmin;
	S_F = sigma_FP * S_Fmin / sigma_F;
	S_FStatic = sigma_FPStatic / sigma_F;
}

void CInvoluteGearTAN::CSingleInvoluteTAN::DefineLocationPara(double InDistBetweenAssembleEdgeToShaftLeft, int InAssembleFaceType)
{
	AssembleDist = InDistBetweenAssembleEdgeToShaftLeft;
	AssembleFaceType = InAssembleFaceType;
}

void CInvoluteGearTAN::CSingleInvoluteTAN::DefineLocationPara(double* InData)
{
	AssembleDist = InData[0];
	AssembleFaceType = static_cast<int>(InData[1]);
}



double CInvoluteGearTAN::CSingleInvoluteTAN::AuxiliaryFunctionForMaxProfileShift(double* InX, void** InGearObj)
{
	CSingleInvoluteTAN* tmpGearObj = static_cast<CSingleInvoluteTAN*> (InGearObj[0]);
	double& tmpsa = *static_cast<double*> (InGearObj[1]);
	double& tmpX = *InX;
	double tmpda = tmpGearObj->d + 2 * (tmpGearObj->h_ax + tmpX) * tmpGearObj->mn;
	double tmpalpha_a = acos(tmpGearObj->db / tmpda);
	int& tmpZ = tmpGearObj->Z;
	double test = tmpda * (PI / (2 * tmpZ) + 2 * tmpX * tan(tmpGearObj->alpha_t) / tmpZ + Finv(tmpGearObj->alpha_t) - Finv(tmpalpha_a)) - tmpsa;
	return tmpda * (PI / (2 * tmpZ) + 2 * tmpX * tan(tmpGearObj->alpha_t) / tmpZ + Finv(tmpGearObj->alpha_t) - Finv(tmpalpha_a)) - tmpsa;
}

double CInvoluteGearTAN::CSingleInvoluteTAN::AuxiliaryFunctionForthetaInYF(double * InTheta, void ** InGHzn)
{
	double GG = *static_cast<double*> (InGHzn[0]);
	double HH = *static_cast<double*> (InGHzn[1]);
	double Zn = *static_cast<double*> (InGHzn[2]);	
	return 2 * GG / Zn * tan(*InTheta) - HH - (*InTheta);
}

double CInvoluteGearTAN::CSingleInvoluteTAN::AuxiliaryFunctionForalphaByInvFunctionValue(double* InAlpha, void** InInvAlphaValue)
{
	double tmpInvValue = *static_cast<double*> (InInvAlphaValue[0]);
	return Finv(*InAlpha)- tmpInvValue;
}

double CInvoluteGearTAN::CSingleInvoluteTAN::GetAlphaByInvFunctionValue(double InInvValue)
{
	void* SolveFunPtr = static_cast<void*>(&AuxiliaryFunctionForalphaByInvFunctionValue);
	int OtherCoefCount = 1;
	void** pTempOtherCoefPtr = new void* [1];
	pTempOtherCoefPtr[0] = static_cast<void*> (&InInvValue);
	//double* ptmpAlpha = new double[2] {0, 60 * Degree};
	//double ResultAlpha = TANFINDROOTBYBISECTIONFORC(SolveFunPtr, ptmpAlpha, &OtherCoefCount, pTempOtherCoefPtr);
	//delete[] ptmpAlpha;
	double ResultAlpha = 20 * Degree;
	ResultAlpha = TANFINDROOTBYITERATIONFORC(SolveFunPtr, &ResultAlpha, &OtherCoefCount, pTempOtherCoefPtr);
	

	delete[] pTempOtherCoefPtr;
	
	return ResultAlpha;
}

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

void CInvoluteGearTAN::DefineGearPairGeometry(double * InPinionData, double * InGearData, double * InPairCommonData)
{
	DefineGearPairGeometry(InPinionData, InGearData, InPairCommonData, *this);
}

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

double CInvoluteGearTAN::CalProfileShiftSumByCenterDistance()
{
	y_t = (a_w - a_x) / mt;
	y_n = (a_w - a_x) / mn;
	alpha_wt = acos(a_x / a_w * cos(alpha_t));
	invalpha_wt = CSingleInvoluteTAN::Finv(alpha_wt);
	x_Sum = (invalpha_wt - invalpha_t) / tan(alpha_n) / 2 * Tooth_Sum;
	return x_Sum;
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

void CInvoluteGearTAN::OptimizeProfileShiftToEqualLife(int OptTargetNotation)
{
	//总和变位系数
	CalProfileShiftSumByCenterDistance();
	//mPinion.x = mPinion.CalMinProfileShift();//小轮的最小高变位系数
	//mGear.CalMinProfileShift();
	//mGear.x = tmpX_Sum - mPinion.x;
	int OtherCoefCount = 1;
	void** pTempOtherCoefPtr = new void* [1];
	pTempOtherCoefPtr[0] = static_cast<void*> (this);
	void* SolveFunPtr = 0;
	double* tmpXRange = new double[4];//前两个用于X的二分求解，后两个存储X极值对应的求解函数值，以便判断是否异号，异号则需要迭代求解，否则直接取较小的函数值作为结果
	tmpXRange[0] = mPinion.CalMinProfileShift();
	tmpXRange[1] = mPinion.CalMaxProfileShift();
	bool IsNeedIter = 0;//是否需要迭代
	if (OptTargetNotation == ProfileShiftOptByBendStressNotation)
	{	tmpXRange[2] = AuxiliaryFunctionForOptimizeProfileShiftByBending(&(tmpXRange[0]), pTempOtherCoefPtr);
		tmpXRange[3] = AuxiliaryFunctionForOptimizeProfileShiftByBending(&(tmpXRange[1]), pTempOtherCoefPtr);
		if (tmpXRange[2] * tmpXRange[3] > 0)
		{			
			if (abs(tmpXRange[2]) < abs(tmpXRange[3]))
			{
				mPinion.x = tmpXRange[0];
			}
			else
			{
				mPinion.x = tmpXRange[1];
			}
		}
		else//表明需要迭代求解
		{
			IsNeedIter = 1;
			SolveFunPtr = static_cast<void*>(&AuxiliaryFunctionForOptimizeProfileShiftByBending);
		}
	}
	else
	{		
		tmpXRange[2] = AuxiliaryFunctionForOptimizeProfileShiftByContact(&(tmpXRange[0]), pTempOtherCoefPtr);
		tmpXRange[3] = AuxiliaryFunctionForOptimizeProfileShiftByContact(&(tmpXRange[1]), pTempOtherCoefPtr);
		if (tmpXRange[2] * tmpXRange[3] > 0)
		{
			if (abs(tmpXRange[2]) < abs(tmpXRange[3]))
			{
				mPinion.x = tmpXRange[0];
			}
			else
			{
				mPinion.x = tmpXRange[1];
			}
		}
		else//表明需要迭代求解
		{
			IsNeedIter = 1;
			SolveFunPtr = static_cast<void*>(&AuxiliaryFunctionForOptimizeProfileShiftByContact);
		}
	}
	if (IsNeedIter)
	{
		mPinion.x = TANFINDROOTBYBISECTIONFORC(SolveFunPtr, tmpXRange, &OtherCoefCount, pTempOtherCoefPtr);
	}
	mGear.x = x_Sum - mPinion.x;
	//double* tmpXlist = new double[20];
	//std::vector < double > tmpResult(20);
	//for (int i = 0;i < 20;i++)
	//{
	//	tmpXlist[i] = 0.05 * i;
	//	tmpResult[i]=AuxiliaryFunctionForOptimizeProfileShift(&(tmpXlist[i]), pTempOtherCoefPtr);

	//}

	delete[] pTempOtherCoefPtr;
	delete[] tmpXRange;

}
void CInvoluteGearTAN::RecommondMainGeometryParaByContact(double InU, double InK, double InT1, double InPinionRPM, double InDesignLife, double InsigmaHLim, int InMeshSign, int InN_p)
{
	double tmpUSum1 = 0;
	//u = InU;
	tmpUSum1 = InU + InMeshSign;

	fy_a_test = fy_d_test / (0.5 * tmpUSum1);
	sigma_Hlim = InsigmaHLim;
	sigma_HP_test = sigma_Hlim * 0.9;
	K_Sum_test = InK;
	//初定中心距离
	a_w_min_test = A_a_test * tmpUSum1 * pow(InK * InT1 / (fy_a_test * InU * pow(sigma_HP_test, 2)), 1 / 3.0);
	d_1_min_test = pow(2, 1 / 3.0) * A_d_test * a_w_min_test / (tmpUSum1 * A_a_test) ;	
	double tmpa_w = NSMechanismFunction::ReturnPriorityNumberByLastByRoundUp(a_w_min_test, PriorityLastNumberStructType);
	//自定义推荐逻辑,主要为确定合适的mn
	double tmpmn_Test = 0.03 * tmpa_w;//推荐模数
	double* tmpMnList = &(NSInvoluteStaticArrary::GearStandardModule[14]);//小于1的模数不考虑
	int tmpMnCount = 36;
	int tmpMnBaseIndex = NSTanBasiceFunction::FindValueShortestIndexInDoubleList(tmpmn_Test, tmpMnCount, tmpMnList);	
	//生成推荐的mn索引序列
	int tmpSearMnCount = 3;//总共搜寻多少个mn,需要为奇数
	std::vector<int> tmpTestMnIndexVectorList;
	NSTanBasiceFunction::GenerateArithmeticSequenceContainInputValueInRange(tmpMnBaseIndex, 1, tmpSearMnCount, 0, tmpMnCount-1, tmpTestMnIndexVectorList);	
	//CTanVector tmpTestMnVectorList(tmpSearMnCount);
	CTanVector tmpResult(tmpSearMnCount);
	double tmpOptS = 0;//优化的安全系数
	double tmpOptMn = 0;
	double tmpCurrentS = 0;//当前安全系数
	double tmpOptPinionX = 0;
	double tmpOptGearX = 0;
	double tmpOptXSum = 0;
	//std::vector<double> tmpTestMnVectorList(5);
	//std::vector<double> tmpTestMnVectorList(5);
	for (int i = 0; i < tmpSearMnCount; i++)
	{
		OptimizeGearTooth(InU, tmpa_w, tmpMnList[tmpMnBaseIndex + i]);
		SetGeometryParaByMnAndDefaultPara(tmpMnList[tmpMnBaseIndex + i], tmpa_w, InN_p);
		DefineLoadPara(InT1, InPinionRPM, InDesignLife, 0, 0, InN_p);
		//以接触应力为基准进行判断
		OptimizeProfileShiftToEqualLife(ProfileShiftOptByContStressNotation);
		tmpCurrentS = min(mPinion.S_H, mGear.S_H);
		std::cout << mPinion.sigma_H << std::endl;
		std::cout << mGear.sigma_H << std::endl;
		//搜寻解，使安全系数最大
		if (tmpCurrentS > tmpOptS)
		{
			tmpOptS = tmpCurrentS;
			tmpOptMn = tmpMnList[tmpMnBaseIndex + i];
			tmpOptPinionX = mPinion.x;
			tmpOptGearX = mGear.x;
			tmpOptXSum = x_Sum;
		}

		//tmpTestMnVectorList[i] = tmpMnList[tmpTestMnIndexVectorList[i]];
	}
	SetGeometryParaByMnAndDefaultPara(tmpOptMn, tmpa_w, InN_p);
	mPinion.x = tmpOptPinionX;
	mGear.x = tmpOptGearX;
	x_Sum = tmpOptXSum;
	//mn = tmpMnList[tmpMnBaseIndex+2];


	//mPinion.Z = round(2 * tmpa_w / (mn * (1 + u)));
	//mGear.Z = round(u * mPinion.Z);
	//if (mPinion.Z < 15)
	//{
	//	alpha_n = 25 * Degree;
	//}
	//else
	//{
	//	alpha_n = 20 * Degree;
	//}
	//mPinion.b_Design = fy_a_test * tmpa_w;
	//mGear.b_Design = fy_a_test * tmpa_w;
	//a_w = tmpa_w;
	//mPinion.X_E = 0.1795;
	//mGear.X_E = -0.2555;
	//mGear.b_Design = 12.4;
	//n_P = 3;
	//mPinion.Z = 10;
	//mGear.Z = 38;
	//alpha_n = 25 * Degree;
	//mn = 1.75;
	//mPinion.b_Design = 7;
	//mGear.b_Design = 12.4;
	//a_w = 42;
	//n_P = 3;
	//mPinion.Rhf_fPx = 0.25;
	//mGear.Rhf_fPx = 0.25;
	//mPinion.X_E = 0.1795;
	//mGear.X_E = -0.2555;
	//n_P = 3;
	//mPinion.N_m = n_P;
	//std::cout << mPinion.x << std::endl;
	

	
	InterfaceCalGeometryAndStrength();
	//std::cout << mPinion.x << std::endl;
	//std::cout << mGear.x << std::endl;
	//std::cout << mPinion.sigma_F << std::endl;
	//std::cout << mGear.sigma_F << std::endl;
	//std::cout << mPinion.S_F << std::endl;
	//std::cout << mGear.S_F << std::endl;
	//std::cout << "mPinion.S_H" << std::endl;
	//std::cout << mPinion.S_H << std::endl;
	//std::cout << mGear.S_H << std::endl;


	//std::cout << mPinion.sigma_H << std::endl;
	//std::cout << mGear.sigma_H << std::endl;


	//double tmpmn = tmpMnList[tmpMnBaseIndex];
	//int tmpz1 = round(2 * tmpa_w / (tmpmn * (1 + u)));
	//int tmpz2 = round(u * tmpz1);
	//u = double(tmpz2)/ tmpz1;
	//double tmpb = fy_a_test * tmpa_w;
	
}
void CInvoluteGearTAN::SetGeometryParaByMnAndDefaultPara(double InMn, double InA_w, int InN_p)
{
	mn = InMn;
	a_w = InA_w;
	mPinion.Z = round(2 * InA_w / (mn * (1 + u)));
	mGear.Z = round(u * mPinion.Z);
	if (mPinion.Z < 15)
	{
		alpha_n = 25 * Degree;
	}
	else
	{
		alpha_n = 20 * Degree;
	}
	mPinion.b_Design = fy_a_test * InA_w;
	mGear.b_Design = fy_a_test * InA_w;
	a_w = InA_w;
	n_P = InN_p;
	CalGearPairGeometryZJ();
	//OptimizeProfileShiftToEqualLife();
	//mPinion.x = mPinion.CalMinProfileShift();
	//mGear.x = x_Sum - mPinion.x;

}
void CInvoluteGearTAN::OptimizeGearTooth(double InTranRatio, double InA_w, double InMn, int InToothNumOptNotation)
{
	//齿数的理论公式
	double tmpZ1 = 2 * InA_w / (InMn * (1 + InTranRatio));
	//double tmpZ2 = tmpZ1 * InTranRatio;
	mPinion.Z = round(tmpZ1);
	mGear.Z = round(InTranRatio * tmpZ1);
	if (InToothNumOptNotation == ToothNumOptNotationWithGCD)
	{
		//找互质数
		mGear.Z = NSTanBasiceFunction::ReturnShortedGCDNum(mPinion.Z, mGear.Z);
	}
	u = double(mGear.Z) / mPinion.Z;
	//mGear.Z = InTranRatio*
}
double CInvoluteGearTAN::AuxiliaryFunctionForOptimizeProfileShiftByBending(double* InPinionx, void** InGearObj)
{
	CInvoluteGearTAN* tmpGearPairObj = static_cast<CInvoluteGearTAN*> (InGearObj[0]);
	tmpGearPairObj->mPinion.x = *InPinionx;
	tmpGearPairObj->mGear.x = tmpGearPairObj->x_Sum - tmpGearPairObj->mPinion.x;
	tmpGearPairObj->CalGearPairGeometry();
	tmpGearPairObj->InterfaceCalGeometryAndStrength();
	//return tmpGearPairObj->mPinion.sigma_F- tmpGearPairObj->mGear.sigma_F;
	return tmpGearPairObj->mPinion.S_F - tmpGearPairObj->mGear.S_F;
}

double CInvoluteGearTAN::AuxiliaryFunctionForOptimizeProfileShiftByContact(double* InPinionx, void** InGearObj)
{
	CInvoluteGearTAN* tmpGearPairObj = static_cast<CInvoluteGearTAN*> (InGearObj[0]);
	tmpGearPairObj->mPinion.x = *InPinionx;
	tmpGearPairObj->mGear.x = tmpGearPairObj->x_Sum - tmpGearPairObj->mPinion.x;
	tmpGearPairObj->CalGearPairGeometry();
	tmpGearPairObj->InterfaceCalGeometryAndStrength();
	return tmpGearPairObj->mPinion.S_H - tmpGearPairObj->mGear.S_H;;
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

void CInvoluteGearTAN::CalDynamicFactorKV()
{
	_CiteGearPara
	Velocity = d_1 * Pi*mPinion.n / 6e4;
	mPinion.CalEquivalentMass();
	mGear.CalEquivalentMass();
	double& m1 = mPinion.EquivalentMass;
	double& m2 = mGear.EquivalentMass;
	if (GearPairType == 1)
	 {
		m_red = m1 * m2 / (m1 + m2);
		//下面是外啮合的第二种算法
		//m_red = Pi / 8.0 * (d_m1 * d_m1 / d_b1 / d_b1) * d_m1 * d_m1 / (1.0 / (1.0 - q1 * q1 * q1 * q1) / rho1 + 1.0 / (1.0 - q2 * q2 * q2 * q2) / rho2 / u / u);
	}
	else if (GearPairType == 2)
	{
		m_red = m1 * m2 / (m1 + n_P * m2);
	}
	else if (GearPairType == 3)
	{
		m_red = Pi / 8 * pow(d_m1, 4) / pow(d_b1, 2)*(1 - pow(q1, 4))*rho1;
	}
	else
	{
		m_red = m1 * m2 / (m1 + m2);
	}
	n_E1 = 30000.0 / Pi / z1 * sqrt(c_gamaalpha / m_red);//共振转速
	N = mPinion.n / n_E1;
	//在主共振区内的临界转速比N_S
	//在主共振区内的临界转速比N_S
	if (F_t * K_A / b < 100)
	{
		N_S = 0.5 + 0.35 * sqrt(F_t * K_A / (100.0 * b));
	}
	else
	{
		N_S = 0.85;
	}
	if (Xi_Gama > 2)
	{
	Cv1 = 0.32;//考虑齿距偏差影响的系数并假定为常数
	Cv2 = 0.57 / (Xi_Gama - 0.3);//考虑齿形偏差影响的系数
	Cv3 = 0.096 / (Xi_Gama - 1.56);//考虑啮合刚度周期变化影响的系数
	Cv4 = (0.57 - 0.05 * Xi_Gama) / (Xi_Gama - 1.44);
	Cv5 = 0.47;
	Cv6 = 0.12 / (Xi_Gama - 1.74);
	}
	else// if ((1 < Xi_Gama) && (Xi_Gama <= 2));小心重合度不为1
	{
		Cv1 = 0.32;//考虑齿距偏差影响的系数并假定为常数
		Cv2 = 0.34;//考虑齿形偏差影响的系数
		Cv3 = 0.23;//考虑啮合刚度周期变化影响的系数
		Cv4 = 0.90;//考虑由啮合刚度周期性变化引起的齿轮副扭转共振的影响系数
		Cv5 = 0.47;
		Cv6 = 0.47;
	}
	if (Xi_Gama > 2.5)
	{
		Cv7 = 1.0;
	}
	else if ((1.5 < Xi_Gama) && (Xi_Gama <= 2.5))
	{
		Cv7 = 0.125 * sin(Pi * (Xi_Gama - 2.0)) + 0.875;
	}
	else //if ((1 < Xi_Gama) && (Xi_Gama <= 1.5))
	{
		Cv7 = 0.75;//考虑因啮合刚度变化，在高的恒定转速运行时轮齿弯曲变形产生的载荷分量的影响系数
	}
	f_pb = max(mPinion.f_pb, mGear.f_pb);
	f_falpha = max(mPinion.f_falpha, mGear.f_falpha);
	if (mPinion.MaterialType==mGear.MaterialType)
	{
		y_alpha = mPinion.Get_y_alpha(f_pb,Velocity);
	}
	else
	{
		double y_alpha1 = mPinion.Get_y_alpha(f_pb, Velocity);
		double y_alpha2 = mGear.Get_y_alpha(f_pb, Velocity);
		y_alpha = (y_alpha1 + y_alpha2) / 2.0;
	}
	//Method A
	y_p = y_alpha;
	//存疑
	y_f = 0.075 * f_falpha;
	f_pb_eff = f_pb - y_p;
	f_falpha_eff = f_falpha - y_f;
	Bp = c_p * f_pb_eff / (K_A * F_t / b);//Bp,Bf,Bk是考虑轮齿偏差及修形对动载荷的影响的无量纲参数
	Bf = c_p * f_falpha_eff / (K_A * F_t / b);
	Bk = (1 - c_p * C_a / (K_A * F_t / b));
	
	if (N <= N_S)//亚临界区,大多数工业齿轮在此区间内工作
	{
		double K = Cv1 * Bp + Cv2 * Bf + Cv3 * Bk;
		K_v = N * K + 1;
	}
	if (N_S < N && N <= 1.15)//主共振区
	{
		K_v = Cv1 * Bp + Cv2 * Bf + Cv4 * Bk + 1;
	}
	if (N >= 1.5)//超临界区
	{
		K_v = Cv5 * Bp + Cv6 * Bf + Cv7;
	}
	if (1.15 < N && N < 1.5)//过渡区
	{
		K_v = Cv5 * Bp + Cv6 * Bf + Cv7 + (Cv1 * Bp + Cv2 * Bf + Cv4 * Bk + 1 - (Cv5 * Bp + Cv6 * Bf + Cv7)) / 0.35 * (1.5 - N);
	}

}

void CInvoluteGearTAN::CalFaceLoadFactorsKHbetaKFbeta()
{
	_CiteGearPara
	F_m = F_t * K_A*K_gama*K_v;
	//double ll = 34;
	//double s = 3.4;
	//double TempValue = k_p * ll*s / pow(d_1, 2)*pow(d_1 / d_sh, 4);
	////double TempValue = 0;
	////对于直齿轮与人字齿
	//double TempB = b;
	//double B_x = 1;
	//f_sh = F_m / b * 0.023 * (abs(B_x + TempValue - 0.3) + 0.3) * pow(TempB / d_1, 2);
	Calf_sh();
	f_ma = sqrt(pow(mPinion.f_Hbeta, 2) + pow(mGear.f_Hbeta, 2));
	//f_ma = 10.26;
	double B1, B2;
	//if ()
	//{
	//}
	//螺旋线修形
	int& IsFavorableContactPostion = mPinion.IsFavorableContactPostion;
	int& HelixModificationType = mPinion.HelixModificationType;
	if ((HelixModificationType == 1) || (HelixModificationType == 0))//不修形
	{
		B1 = 1.0;
		B2 = 1.0;
	}
	else if (HelixModificationType == 2)//仅中心鼓形修形
	{
		B1 = 1.0;
		B2 = 0.5;
	}
	else if (HelixModificationType == 3)//仅中心鼓形修形
	{
		B1 = 0.5;
		B2 = 0.5;
	}
	else if (HelixModificationType == 4)//仅螺旋线正
	{
		B1 = 0.1;
		B2 = 1.0;
	}
	else if (HelixModificationType == 5)//螺旋线修正加中心鼓形修形
	{
		B1 = 0.1;
		B2 = 0.5;
	}
	else if (HelixModificationType == 6)//齿端修薄
	{
		B1 = 0.7;
		B2 = 0.7;
	}
	else
	{
		B1 = 1.0;
		B2 = 1.0;
	}
	double F_beta_x;
	//IsFavorableContactPostion = 1;
	if (IsFavorableContactPostion==0)//未保证接触区良好
	{
		F_beta_x =  1.33 * B1 * f_sh + B2 * f_ma;
	}
	else//接触区良好
	{
		double F_beta_x_min = 0.005*F_m / b;
		//有疑问,用的两轮偏差均值
		double F_beta_x_min2 = 0.5*(mGear.f_Hbeta + mPinion.f_Hbeta);
		//double F_beta_x_min = 0.005*F_m / b;
		if (F_beta_x_min < F_beta_x_min2)
		{
			F_beta_x_min = F_beta_x_min2;
		}
		//double F_beta_x_min = max(0.005*F_m / b, 0.5*f_Hbeta);
		//double f_Hbeta5 = round(2*(4 + 0.35*sqrt(b)) + 0.05*(sqrt(d_1) + sqrt(d_2)))/2.0;
		double f_Hbeta5 = round(2 * (4 + 0.35*sqrt(b)) + 0.05*(2*sqrt(d_1))) / 2.0;
		F_beta_x = abs(1.33*B1*f_sh - f_Hbeta5);
		//if (F_beta_x < F_beta_x_min)
		//{
		//	F_beta_x = F_beta_x_min;
		//}
	}
	//F_beta_x = 5.29;
	double y_beta;
	double x_beta;
	if (mPinion.MaterialType==mGear.MaterialType)
	{
		y_beta = mPinion.Get_y_beta(mPinion.MaterialType, F_beta_x, Velocity);
		x_beta = mPinion.get_x_beta(mPinion.MaterialType);
	}
	else
	{
		y_beta = (mPinion.Get_y_beta(mPinion.MaterialType, F_beta_x, Velocity)+
			mGear.Get_y_beta(mPinion.MaterialType, F_beta_x, Velocity))/2;
		x_beta = (mPinion.get_x_beta(mPinion.MaterialType)+ mGear.get_x_beta(mPinion.MaterialType))/2;
	}
	F_beta_y = F_beta_x - y_beta;//有效当量螺旋线偏啮量
	//K_Hbeta ISO 6336-1第50页，还有b_cal
	if ((F_beta_y * c_gama_beta / (2 * F_m / b)) >= 1)
	{
		K_Hbeta = sqrt(2 * F_beta_y * c_gama_beta / (F_m / b));
	}
	else 
	{
		K_Hbeta = 1 + F_beta_y * c_gama_beta / (2 * F_m / b);
	}
	double b_h = min((mPinion.b_Design / mPinion.h), (mGear.b_Design / mGear.h));
	if (b_h < 3)//极限状况
	{
		b_h = 3;
	}
	double N_F = b_h * b_h / (1 + b_h + b_h * b_h);
	K_Fbeta = pow(K_Hbeta, N_F);
	//double F_beta_x = 1.33 * B1 * f_sh + B2 * f_ma;
}

void CInvoluteGearTAN::CalFaceLoadFactorsKHalphaKFalpha()
{
	F_tH = F_t * K_A * K_v * K_gama * K_Hbeta;//端平面内起决定作用的切向载荷
	double Tempy_alpha = 0;
	Tempy_alpha = 0.075*(max(f_pb, f_falpha));
	double TempCoef = 0;
	//mPinion.IsProfileModification = 0;
	//Tempy_alpha
	if (mPinion.IsProfileModification == 0)//是否进行齿廓修形以补偿变形,0未补偿
	{
		TempCoef = f_pb - Tempy_alpha;
	}	
	else
	{
		TempCoef = 0.5*(max(f_pb, f_falpha) - Tempy_alpha);
	}
	if (Xi_Gama <= 2)
	{
		K_Falpha = Xi_Gama / 2 * (0.9 + 0.4 * c_gamaalpha * TempCoef / (F_tH / b));
	}
	else
	{
		K_Falpha = 0.9 + 0.4 * sqrt(2.0 * (Xi_Gama - 1.0) / Xi_Gama) * c_gamaalpha * TempCoef / (F_tH / b);
	}
	K_Halpha = K_Falpha;
	if (K_Halpha > (Xi_Gama / (Xi_alpha * Z_epsilon * Z_epsilon)))//K_Hα的限制条件
	{
		K_Halpha = Xi_Gama / (Xi_alpha * Z_epsilon * Z_epsilon);
	}
	if (K_Halpha < 1.0)//K_Hα的限制条件
	{
		K_Halpha = 1.0;
	}
	if (K_Falpha > (Xi_Gama / (0.25 * Xi_alpha + 0.75)))//K_Fα的限制条件
	{
		K_Falpha = Xi_Gama / (0.25 * Xi_alpha + 0.75);
	}
	if (K_Falpha < 1.0)//K_Fα的限制条件
	{
		K_Falpha = 1.0;
	}
}

void CInvoluteGearTAN::CalZBAndZD()
{
	_CiteGearPara

		if (HelixModificationQuality == 1)
		{
			f_ZCa = 1.0;
		}
		else if (HelixModificationQuality == 2)
		{
			f_ZCa = 1.07;
		}
		else
		{
			f_ZCa = 1.2;
		}
	if (mGear.IOMeshIndex>0)//外啮合齿轮
	{
		double DNa1B1 = sqrt(pow(d_Na1, 2) / pow(d_b1, 2) - 1);
		double DNa2B2 = sqrt(pow(d_Na2, 2) / pow(d_b2, 2) - 1);
		double M1 = tan(alpha_wt) / sqrt((DNa1B1 - 2 * Pi / z1)*(DNa2B2 - (Xi_alpha - 1) * 2 * Pi / z2));
		double M2 = tan(alpha_wt) / sqrt((DNa2B2 - 2 * Pi / z2)*(DNa1B1 - (Xi_alpha - 1) * 2 * Pi / z1));
		if (abs(beta)<DefaultTolerance)//直齿轮
		{
			Z_B = max(M1, 1.0);
			Z_D = max(M2, 1.0);
		}
		else //斜齿轮
		{
			//只有Xi_alpha >1的情况，当Xi_alpha >2时 计算个偏保守
			if (Xi_beta >= 1.0)
			{
				Z_B = sqrt(f_ZCa);
				Z_D = Z_B;
			}
			else//Xi_beta < 1.0
			{
				if (M1 <= 1.0)
				{
					Z_B = 1 + Xi_beta * (sqrt(f_ZCa) - 1);
				}
				else
				{
					Z_B = M1 + Xi_beta * (sqrt(f_ZCa) - M1);
				}
				if (M2 <= 1.0)
				{
					Z_D = 1 + Xi_beta * (sqrt(f_ZCa) - 1);;
				}
				else
				{
					Z_D = M2 + Xi_beta * (sqrt(f_ZCa) - M2);
				}
			}

		}
	}//内啮合
	else
	{
		Z_B = 1;
		Z_D = 1;
	}
}

void CInvoluteGearTAN::CalZXi()
{
	if (abs(beta)< DefaultTolerance)
	{
		Z_Xi = sqrt((4.0 - Xi_alpha) / 3);
	}
	else
	{
		if (Xi_beta<1.0)
		{
			Z_Xi = sqrt((4.0 - Xi_alpha) / 3 * (1.0 - Xi_beta) + Xi_beta / Xi_alpha);
		}
		else
		{
			Z_Xi = sqrt(1.0 / Xi_alpha);
		}
	}

}

void CInvoluteGearTAN::CalZLZVZR()
{
	//ZL计算
	double C_ZL;
	if (850.0 < sigma_Hlim && sigma_Hlim < 1200.0)
	{
		C_ZL = sigma_Hlim / 437.5 + 0.6357;
	}
	else if (sigma_Hlim < 850.0)
	{
		C_ZL = 0.83;
	}
	else
	{
		C_ZL = 0.91;
	}
	//double v_50 = mOilV40ToV50Curve.CalInterpValueWithExtend(v_40);
	if (v_50 <-1000.0)//v50未赋值
	{
		Z_L = C_ZL + 4.0 * (1.0 - C_ZL) / pow((1.2 + 134.0 / v_40), 2);
	}
	else
	{
		Z_L = C_ZL + 4.0 * (1.0 - C_ZL) / pow((1.2 + 80.0 / v_50), 2);
	}
	//double TempSigma_Hlim;
	//TempSigma_Hlim = max(850.0, sigma_Hlim);
	//TempSigma_Hlim = min(1200.0, TempSigma_Hlim);
	double C_Zv = C_ZL + 0.02;
	Z_v= C_Zv + 2.0 * (1.0 - C_Zv) / sqrt(0.8 + (32.0 / Velocity));
	double R_zH = (mPinion.R_zH + mGear.R_zH) / 2;
	double Rho1= mPinion.Get_Rho(alpha_wt);
	double Rho2 = mGear.Get_Rho(alpha_wt);
	Rho_red = Rho1 * Rho2 / (Rho1 + Rho2);
	double R_zH10 = R_zH * pow(10 / Rho_red, 1.0/3.0);
	double C_ZR;
	if (sigma_Hlim<850)
	{
		C_ZR = 0.15;
	}
	else if(sigma_Hlim > 1200)
	{
		C_ZR = 0.08;
	}
	else
	{
		C_ZR = 0.32 - 0.0002 * sigma_Hlim;
	}
	Z_R = pow(3.0 / (R_zH10), C_ZR / 1.0);
	Z_LStatic = 1.0; Z_vStatic = 1.0; Z_RStatic = 1.0;
}

void CInvoluteGearTAN::CalZW()
{
	//未验证-未校核
	mPinion.Z_W = 1.0;
	mGear.Z_W = 1.0;
	mPinion.Z_WStatic = 1.0;
	mGear.Z_WStatic = 1.0;
	double TempHB = min(mPinion.CaseHardness, mGear.CaseHardness);//目前是洛式硬度
	TempHB = (TempHB - 32.25) / 0.098;
	bool ISGearThroughHarden = (mGear.MaterialType == SteelV || mGear.MaterialType == SteelVCast || mGear.MaterialType == SteelNVnitr || mGear.MaterialType == SteelNVnitrcar);//大轮是否完全硬化
	if (mPinion.MaterialType==SteelEh|| mPinion.MaterialType == SteelIF|| mPinion.MaterialType == SteelNTnitr)//小轮表面硬化,钢的类型需进一步考察
	{
		if (ISGearThroughHarden)//大轮整体硬化
		{
			double R_zHForZW = mPinion.R_zH*pow(10.0 / Rho_red, 0.33)*pow(mPinion.R_zH / mGear.R_zH, 0.66) / pow(v_40*Velocity / 1500.0, 0.33);
			if (R_zHForZW > 16)
			{
				R_zHForZW = 16;
			}
			else if (R_zHForZW < 3)
			{
				R_zHForZW = 3;
			}

			if ((TempHB <= 470) && (TempHB >= 130))
			{
				Z_W = (1.2 - (TempHB - 130) / 1700.0)*pow(3.0 / R_zHForZW, 0.15);
				Z_WStatic = 1.05 - (TempHB - 130) / 6800.0;
			}
			else if (TempHB > 470)
			{
				Z_W = pow(3.0 / R_zHForZW, 0.15);
				Z_WStatic = 1.0;
			}
			else
			{
				Z_W = 1.2*pow(3.0 / R_zHForZW, 0.15);
				Z_WStatic = 1.05;
			}
			mPinion.Z_W = Z_W;
			mGear.Z_W = Z_W;
			mPinion.Z_WStatic = Z_WStatic;
			mGear.Z_WStatic = Z_WStatic;
		}
		else if (mGear.MaterialType == SteelGGGperl || mGear.MaterialType == SteelGGGbai || mGear.MaterialType == SteelGGGferr || mGear.MaterialType == SteelGTSperl)//此刻ZW仅用于大轮,大轮为球墨铸铁
		{
			if (TempHB <162)
			{
				Z_W = 1.2;
			}
			else if (TempHB > 344)
			{
				Z_W = 1.0;
			}
			else
			{
				Z_W = 1.2 - (1.87*TempHB - 303.6) / 1700.0;
			}
			Z_WStatic = 1.0;
			mGear.Z_W = Z_W;
		}
	else if ((mPinion.MaterialType == SteelV || mPinion.MaterialType == SteelVCast || mPinion.MaterialType == SteelNVnitr || mPinion.MaterialType == SteelNVnitrcar)&& ISGearThroughHarden)//小轮完全硬化,大轮也完全硬化)
	{
		double TempHB1= (mPinion.CaseHardness - 32.25) / 0.098;
		double TempHB2 = (mGear.CaseHardness - 32.25) / 0.098;
		double HB1HB2 = TempHB2 / TempHB1;
		if (HB1HB2>1.7)
		{
			Z_W = 1.0 + 0.00698*(u - 1.0);
		}
		else if (HB1HB2<1.2)
		{
			Z_W = 1.0;
		}
		else
		{
			double TempA = 0.00898*HB1HB2 - 0.00829;
			Z_W = 1.0 + TempA * (u - 1.0);
		}
		Z_WStatic = 1.0;
		mGear.Z_W = Z_W;
	}

	}


}

void CInvoluteGearTAN::Calf_sh()
{
	_CiteGearPara
	
	if (d_1 / d_sh < 1.15)
	{
		IsShaftStiffening = 0;
	}
	else
	{
		IsShaftStiffening = 1;
	}
	if (IsShaftStiffening==0)
	{
		switch (AssembleType)
		{
		case 1:
			k_p = 0.8;
			break;
		case 2:
			k_p = -0.8;
			break;
		case 3:
			k_p = 1.33;
			break;
		case 4:
			k_p = -0.6;
			break;
		case 5:
			k_p = -1.0;
			break;
		default:
			k_p = -1.0;
			break;
		}
	}
	else
	{
		switch (AssembleType)
		{
		case 1:
			k_p = 0.48;
			break;
		case 2:
			k_p = -0.48;
			break;
		case 3:
			k_p = 1.33;
			break;
		case 4:
			k_p = -0.36;
			break;
		case 5:
			k_p = -0.6;
			break;
		default:
			k_p = -0.6;
			break;
		}
	}
	double TempValue = k_p * PinionMeanPointToSpanMeanPointDistance*BearingSpanLength / pow(d_1, 2)*pow(d_1 / d_sh, 4);
	//double TempValue = 0;
	double TempB=0;
	double Tempk;
	double B_x;
	//对于直齿轮与人字齿
	if (GearPairType<500)
	{
		TempB = b;
		Tempk = 100 / static_cast<double>(n_P);
		B_x = 1 + 2 * (100 - Tempk) / Tempk;
	}
	else
	{
		TempB = b/2;
		Tempk = 100 / static_cast<double>(n_P);
		B_x = 0.5 + (200 - Tempk) / Tempk;
	}
	f_sh = F_m / b * 0.023 * (abs(B_x + TempValue - 0.3) + 0.3) * pow(TempB / d_1, 2);
}

#pragma region zhoujie add
void CInvoluteGearTAN::CSingleInvoluteTAN::DefineSingleGearGeometryZJ(double Inmn, double Inalpha_n, double Inbeta, double* InOtherSingleGearData, CSingleInvoluteTAN& IOSingleGearObj)
{
	IOSingleGearObj.Z = static_cast<int>(InOtherSingleGearData[0]);//齿数
	IOSingleGearObj.mn = Inmn;//参考法向模数
	IOSingleGearObj.alpha_n = Inalpha_n;//法向压力角
	IOSingleGearObj.beta = Inbeta;//螺旋角
	IOSingleGearObj.SpiralIndex = static_cast<int>(InOtherSingleGearData[1]);//旋向标识，-1左旋，1右旋
	IOSingleGearObj.b_Design = InOtherSingleGearData[2];//设计齿宽
	IOSingleGearObj.h_ax = InOtherSingleGearData[3];//齿顶高系数
	IOSingleGearObj.x = InOtherSingleGearData[4];//齿廓变位系数 
	IOSingleGearObj.cx = InOtherSingleGearData[5];//顶隙系数cx + h_ax = h_fx
	IOSingleGearObj.Rhf_fPx = InOtherSingleGearData[6];//齿根圆角(半径)系数
	IOSingleGearObj.s_pr = InOtherSingleGearData[7];//挖根量,mm-
	IOSingleGearObj.C_a = InOtherSingleGearData[8];//齿顶修形量，um-
	IOSingleGearObj.C_f = InOtherSingleGearData[9];//齿根修形量,um-
	IOSingleGearObj.Angle = InOtherSingleGearData[10];//是否使用滚动角          
	IOSingleGearObj.k = static_cast<int>(InOtherSingleGearData[11]);//跨齿距-跨齿数
	IOSingleGearObj.h_k = InOtherSingleGearData[12];//齿顶倒角量，mm-
	IOSingleGearObj.D_i = InOtherSingleGearData[13];//轮幅外圆直径-
	IOSingleGearObj.IOMeshIndex = Z / abs(Z);
	IOSingleGearObj.k·mn = InOtherSingleGearData[14];//齿轮齿顶高变化代替后续计算涉及的k_factor*mn
	IOSingleGearObj.E_sns = InOtherSingleGearData[15];//齿厚的偏差上限 
	IOSingleGearObj.E_sni = InOtherSingleGearData[16];//齿厚的偏差下限
	IOSingleGearObj.Ada_upper = InOtherSingleGearData[17];//对应kisssoft齿顶圆容差上限
	IOSingleGearObj.Ada_lower = InOtherSingleGearData[18];//对应kisssoft齿顶圆容差下限
	IOSingleGearObj.q_max = InOtherSingleGearData[19];//机械加工余量q_max
	IOSingleGearObj.q_min = InOtherSingleGearData[20];//机械加工余量q_min
	IOSingleGearObj.d_y = InOtherSingleGearData[21];//任意直径
	IOSingleGearObj.Cutter = InOtherSingleGearData[22];//刀具类型 0齿条刀  1 插齿刀
	IOSingleGearObj.h_aP0 = InOtherSingleGearData[23];//齿条刀的齿顶高系数
	IOSingleGearObj.rho_aP0 = InOtherSingleGearData[24];//齿条刀的齿顶圆半径系数
	IOSingleGearObj.z0 = InOtherSingleGearData[25];//插齿刀具参数-插齿刀齿数
	IOSingleGearObj.a0 = InOtherSingleGearData[26];//插齿刀具参数-插齿刀加工中心距
	IOSingleGearObj.d_a0 = InOtherSingleGearData[27];//插齿刀具参数-插齿刀具齿顶直径
	IOSingleGearObj.d_b0 = InOtherSingleGearData[28];//插齿刀具参数-插齿刀基圆直径
	IOSingleGearObj.d_Fa0 = InOtherSingleGearData[29];//插齿刀具参数-插齿刀齿顶形状直径
	IOSingleGearObj.alpha_wt0 = InOtherSingleGearData[30];//插齿刀具参数-插齿刀具参数alpha_wt0
}

void CInvoluteGearTAN::CSingleInvoluteTAN::CalSingleGearBasicGeometryZJ()
{///mn;beta;Z;alpha_n;x;k
	d = abs(Z) * mn / cos(beta);
	mt = mn / cos(beta);
	m_x = mt / tan(beta);
	p_z = abs(Z) * mn * Pi / sin(beta);
	beta_b = asin(sin(beta) * cos(alpha_n));
	zn = Z / (pow(cos(beta_b),2) * cos(beta));
	db = abs(Z) * mn * cos(alpha_n) / cos(beta_b);
	//d_y = db * cos(alpha_n) * tan(beta_b) / sin(beta_b);
	beta_y = atan(tan(beta) * d_y / d);
	gamma_y = Pi / 2 - beta_y;
	alpha_yt = acos(db / d_y);
	alpha_t = acos(db / d);
	alpha_yn = atan(tan(alpha_yt) * cos(beta_y));
	xi_y = tan(alpha_yt);
	rho_y = IOMeshIndex * sqrt(pow(d_y, 2) - pow(db, 2)) / 2;
	invalpha_yt = CInvoluteGearTAN::CSingleInvoluteTAN::Finv(alpha_yt);
	invalpha_t = CInvoluteGearTAN::CSingleInvoluteTAN::Finv(alpha_t);
	tau = 2 * Pi / abs(Z);
	p_t = d * tau / 2;
	p_n = Pi * mn;
	p_yt = d_y * p_t / d;
	p_yn = p_yt * cos(beta_y);
	//p_x = p_yn / sin(beta_y);
	p_x = p_z / abs(Z);
	p_bt = db * p_t / d;
	p_bn = p_bt * cos(beta_b);
	p_et = p_bt;
	p_en = p_bn;
	d_v = d + 2 * x * mn * IOMeshIndex;
	h_aP = h_ax * mn;
	h_fx = h_ax + cx;
	h_fP = h_fx * mn;
	//h = h_aP + k_factor * mn + h_fP;//h = abs(mSingleData.d_a - d_f) / 2;另一种计算方法
	//d_f = d - 2 * IOMeshIndex * (h_fP - x * mn);
	s_yt = d_y * ((Pi + 4 * x * tan(alpha_n)) / (2 * abs(Z)) + (invalpha_t - invalpha_yt) * IOMeshIndex);
	s_t = mn * (Pi / 2 + 2 * x * tan(alpha_n)) / cos(beta);
	psi_y = s_yt / d_y;
	psi = (Pi + 4 * x * tan(alpha_n)) / (2 * abs(Z));
	psi_b = psi + invalpha_t * IOMeshIndex;
	e_yt = d_y * ((Pi - 4 * x * tan(alpha_n)) / (2 * abs(Z)) - (invalpha_t - invalpha_yt) * IOMeshIndex);
	e_t = mn * (Pi / 2 - 2 * x * tan(alpha_n)) / cos(beta);
	eta_y = e_yt / d_y;
	eta = (Pi - 4 * x * tan(alpha_n)) / (2 * abs(Z));
	eta_b = eta - invalpha_t * IOMeshIndex;
	s_yn = s_yt * cos(beta_y);
	s_n = s_t * cos(beta);//s_n = mn * (Pi / 2 + 2 * x * tan(alpha_n));另一种计算方法
	e_yn = e_yt * cos(beta_y);
	e_n = mn * (Pi / 2 - 2 * x * tan(alpha_n));
	s_ns = s_n + E_sns;
	s_ni = s_n + E_sni;
	x_Es = x + E_sns / (2 * mn * tan(alpha_n));
	x_Ei = x + E_sni / (2 * mn * tan(alpha_n));
	T_sn = E_sni - E_sns;//依据BS ISO 21771:2007的Figure 37
	T_snv = 2 * (q_max - q_min) / cos(alpha_n) - T_sn; //T_snv不知道是啥！这里依据BS ISO 21771:2007的公式122
	x_EsV = x_Ei + q_max / (mn * sin(alpha_n));
	x_EiV = x_Es + q_min / (mn * sin(alpha_n));
	CalEISingleData(x, 0, mSingleData);     //这里计算的是一部分理论值，下面两个是实际值
	CalEISingleData(x_Es, Ada_upper, mESingleData);
	CalEISingleData(x_Ei, Ada_lower, mISingleData);

	d_Fa = mSingleData.d_Fa;
	d_Ff = mSingleData.d_Ff;
	dfE = mSingleData.d_fe; 
	h_f = mSingleData.h_f;
	h_a = mSingleData.h_a;
	df = d - 2.0 * IOMeshIndex * h_f;
	da = d + 2.0 * IOMeshIndex * h_a;
	rho_fP = Rhf_fPx * mn;
	h_FaP0 = h_fP - rho_fP * (1 - sin(alpha_n));
	x_Emin = h_FaP0 / mn - Z * (1 - cos(2 * alpha_t)) / (4 * cos(beta));
	Wk = 2 * x * mn * sin(alpha_n) + (mn * cos(alpha_n) * (Pi * (k - 0.5) + Z * invalpha_t));
	dm = (da + df) / 2;
	rb = db / 2;
	rm = dm / 2;
	ra = da / 2;
	rf = df / 2;
	alpha_Pn = alpha_n;
	h = abs(da - df) / 2;	
	C_B = (1.0 + 0.5 * (1.2 - h_fP / mn)) * (1.0 - 0.02 * (20.0 - alpha_Pn / Degree));
}

void CInvoluteGearTAN::CSingleInvoluteTAN::CalEISingleData(double InX_E, double Ada, EISingleData& OutSingleData)
{
	OutSingleData.d_a = d + 2.0 * (x * mn + h_aP + k·mn) * IOMeshIndex + Ada;
	OutSingleData.d_Fa = OutSingleData.d_a - 2 * IOMeshIndex * h_k;
	OutSingleData.d_fe = CalGearParad_FE(InX_E);
	OutSingleData.h_a = abs(OutSingleData.d_a - d) / 2;//h_aP + InX_E * mn + k_factor * mn;后面个公式错误的原因在InX_E，这里就只是x，有多个值是因为d_a多值！
	OutSingleData.d_Ff = CalGearParad_Ff(InX_E);
	OutSingleData.h_f = h_fP - InX_E * mn;
}

double CInvoluteGearTAN::CSingleInvoluteTAN::CalGearParad_FE(double InX_E)
{
	double d_fe;
	if (Cutter == 0)
	{
		//d_FE有两个值
		 d_fe = d + 2 * InX_E * mn - 2 * h_aP0 * mn; //此处的X_E为对应kissssoft上的输出Generating Profile shift coefficient[xE.e/i]；对应标准上的展成变位系数，分别对应x_Es和x_Ei。且这里的d_fE对应也有两个值！

	}
	else if (Cutter == 1)
	{
		d_fe = 2 * a0 - d_a0 * IOMeshIndex;
	}
	return d_fe;
}

double CInvoluteGearTAN::CSingleInvoluteTAN::CalGearParad_Ff(double InX_E)
{
	double dFf;
	if (Angle == 0)
	{
		if (Cutter == 0)
		{
			dFf = sqrt(pow(d * sin(alpha_t) - 2 * (h_aP0 * mn - InX_E * mn - rho_aP0 * mn * (1 - sin(alpha_n))) / sin(alpha_t), 2) + pow(db, 2));
		}
		else if (Cutter == 1)
		{
			dFf = sqrt(pow(2 * a0 * sin(alpha_wt0) - sqrt(pow(d_Fa0, 2) - pow(d_b0, 2)) * IOMeshIndex, 2)) + pow(db, 2);
		}
	}
	else if (Angle == 1)
	{
		double xi_t;
		double alpha_Ff;
		if (Cutter == 0)
		{
			xi_t = tan(alpha_t);
			alpha_Ff = atan(xi_t - 4 * (h_aP0 * mn - rho_aP0 * mn * (1 - sin(alpha_t)) / mn - InX_E) * cos(beta) / (Z * sin(2 * alpha_t)));
			//d_Ff = sqrt(pow(d * sin(alpha_t) - 2 * (h_aP0 * mn - InX_E * mn - rho_aP0 * mn * (1 - sin(alpha_n))) / sin(alpha_t), 2) + pow(db, 2)); //X_E使用的平均值
		}
		else if (Cutter == 1)
		{
			double xi_wt0 = 0;
			alpha_Ff = atan(z0 * (xi_wt0 - tan(acos(d_b0 / d_Fa0))) / fabs(Z) + xi_wt0);
			//d_Ff = sqrt(pow(2 * a0 * sin(alpha_wt0) - sqrt(pow(d_Fa0, 2) - pow(d_b0, 2)) * IOMeshIndex, 2)) + pow(db, 2);
		}
		dFf = db / cos(alpha_Ff);
	}
	return dFf;
}

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

void CInvoluteGearTAN::DefineGearPairGeometryZJ(double* InPinionData, double* InGearData, double* InPairCommonData)
{
	DefineGearPairGeometryZJ(InPinionData, InGearData, InPairCommonData, *this);
}

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

#pragma region zhoujie end

//void CInvoluteGearTAN::CalZNTByMethodB()
//{
//	mPinion.Get_Z_NT();
//	mGear.Get_Z_NT();
//}


