#include "TANInvoluteGear.h"
#include "InvoluteGearParameterAliases.h"

// 渐开线齿轮动载及载荷分布系数。

// 模块职责：按转速、等效质量与齿轮误差计算动载系数 K_v。
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

// 模块职责：考虑轴变形与齿向误差，计算齿向载荷分布系数。
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

// 模块职责：考虑齿距与齿廓误差，计算齿间载荷分配系数。
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

// 模块职责：按支承布置和轴刚度计算影响载荷分布的轴变形量。
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
