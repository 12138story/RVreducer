#include "TANInvoluteGear.h"

// 单齿轮齿根弯曲强度、安全系数及相关影响系数。

// 模块职责：按材料、速度和误差计算齿廓跑合影响量。
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

// 模块职责：按材料、速度和误差计算齿向跑合影响量。
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

// 模块职责：按材料类别取得齿向跑合计算系数。
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

// 模块职责：计算接触强度寿命系数及其静态取值。
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

// 模块职责：求解齿根几何并计算齿形系数。
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

// 模块职责：按齿根应力集中关系计算应力修正系数。
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

// 模块职责：按螺旋角及重合度计算弯曲强度螺旋角系数。
void CInvoluteGearTAN::CSingleInvoluteTAN::Get_Ybeta(double InXi_beta)
{
	double TempXi_beta = min(InXi_beta, 1.0);
	double TempBeta=min(beta, 30*Degree);
	Y_beta = (1 - TempXi_beta * TempBeta / (120.0*Degree))/pow(cos(TempBeta),3);
}

// 模块职责：按轮缘厚度与齿高关系计算轮缘厚度系数。
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

// 模块职责：按原有规则计算深齿系数。
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

// 模块职责：计算相对齿根圆角敏感系数及静态取值。
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

// 模块职责：计算相对齿根表面状况系数及静态取值。
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

// 模块职责：计算弯曲强度尺寸系数及静态取值。
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

// 模块职责：计算弯曲强度寿命系数及静态取值。
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

// 模块职责：组合齿根影响系数并计算弯曲应力与安全系数。
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

// 模块职责：组合材料、寿命与表面系数，计算许用弯曲应力和安全系数。
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

// 模块职责：计算齿形系数所需角度方程的残差，供原有求根算法使用。
double CInvoluteGearTAN::CSingleInvoluteTAN::AuxiliaryFunctionForthetaInYF(double * InTheta, void ** InGHzn)
{
	double GG = *static_cast<double*> (InGHzn[0]);
	double HH = *static_cast<double*> (InGHzn[1]);
	double Zn = *static_cast<double*> (InGHzn[2]);
	return 2 * GG / Zn * tan(*InTheta) - HH - (*InTheta);
}
