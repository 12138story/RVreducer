#include "TANInvoluteGear.h"
#include "InvoluteGearParameterAliases.h"

// 渐开线齿轮接触强度影响系数。

// 模块职责：计算小轮与大轮的单对齿啮合接触系数。
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

// 模块职责：计算接触强度使用的重合度系数。
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

// 模块职责：计算润滑油、速度和齿面粗糙度对接触强度的影响系数。
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

// 模块职责：计算齿面工作硬化对接触强度的影响系数。
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
