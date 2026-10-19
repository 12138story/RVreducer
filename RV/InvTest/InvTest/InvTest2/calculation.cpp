#include<iomanip>
#include "data.h"
double gety_alpha(int Material, double f_pb, double v)
{
	double y_alpha = 0.;
	if (Material == 1 || Material == 2 || Material == 3 || Material == 4 || Material == 5 || Material == 8 || Material == 9)//条件1
	{
		if (v <= 5)
		{
			y_alpha = 160.0 / sigma_Hlim * f_pb;//跑合量
		}
		if (5 < v && v <= 10)
		{
			double y_alpha_fpb_1 = 0.;
			if (f_pb > 80)
			{
				y_alpha_fpb_1 = 80.0;
			}
			y_alpha = 160.0 / sigma_Hlim * y_alpha_fpb_1;//更新跑合量
			if (y_alpha > 12800 / sigma_Hlim)
			{
				y_alpha = 12800.0 / sigma_Hlim;
			}
		}
		if (v > 10)
		{
			double y_alpha_fpb_2 = 0.;
			if (f_pb > 40)
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
	else if (Material == 6 || Material == 7)//条件2
	{
		if (v <= 5)
		{
			y_alpha = 0.275 * f_pb;
		}
		if (5 < v && v <= 10)
		{
			double y_alpha_fpb_3 = 0.;
			if (f_pb > 80)
			{
				y_alpha_fpb_3 = 80.0;
			}
			y_alpha = 0.275 * y_alpha_fpb_3;//更新跑合量
			if (y_alpha > 22)
			{
				y_alpha = 22.0;
			}
		}
		if (v > 10)
		{
			double y_alpha_fpb_4 = 0.;
			if (f_pb > 40)
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
	else if (Material == 10 || Material == 11 || Material == 12 || Material == 13 || Material == 14)//条件3
	{
		double y_alpha_fpb_5 = 0.;
		if (f_pb > 40)
		{
			y_alpha_fpb_5 = 40.0;
		}
		y_alpha = 0.075 * y_alpha_fpb_5;//更新跑合量
		if (y_alpha > 3)
		{
			y_alpha = 3.0;
		}
		y_alpha = 0.075 * f_pb;
	}
	return y_alpha;
}
double gety_beta(int Material, double F_beta_x, double v)
{
	double y_beta = 0.;
	if (Material == 1 || Material == 2 || Material == 3 || Material == 4 || Material == 5 || Material == 8 || Material == 9)//条件1
	{
		if (v <= 5)
		{
			y_beta = 320.0 / sigma_Hlim * F_beta_x;
		}
		if (5 < v && v <= 10)
		{
			double F_beta_x1 = 0.;
			if (F_beta_x1 > 80)
			{
				F_beta_x1 = 80.0;
			}
			y_beta = 320.0 / sigma_Hlim * F_beta_x1;
			if (y_beta > 25600 / sigma_Hlim)
			{
				y_beta = 25600.0 / sigma_Hlim;
			}
		}
		if (v > 10)
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
	else if (Material == 6 || Material == 7)//条件2
	{
		if (v <= 5)
		{
			y_beta = 0.55 * F_beta_x;
		}
		if (5 < v && v <= 10)
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
		if (v > 10)
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
	else if (Material == 10 || Material == 11 || Material == 12 || Material == 13 || Material == 14)//条件3
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
double getX_beta(int Material)
{
	double X_beta = 0.;
	if (Material == 1 || Material == 2 || Material == 3 || Material == 4 || Material == 5 || Material == 8 || Material == 9)//条件1
	{
		X_beta = 1 - 320.0 / sigma_Hlim;
	}
	else if (Material == 6 || Material == 7)//条件2
	{
		X_beta = 0.45;
	}
	else if (Material == 10 || Material == 11 || Material == 12 || Material == 13 || Material == 14)//条件3
	{
		X_beta = 0.85;
	}
	return X_beta;
}
double getY_deltarelT(double q_s, double rho_1, double Y_S)
{
	/*1.耐久性极限下的相对齿根圆角敏感系数Y_deltarelT
	  2.静强度的相对齿根圆角敏感系数Y_deltarelT*/
	int select_con2 = 1;
	double XP_1 = 1.0 / 5.0;
	double X_1 = XP_1 * (1.0 + 2.0 * q_s);
	double q_sT = 2.5;
	double XT_1 = XP_1 * (1.0 + 2.0 * q_sT);
	double Y_deltarelT = 0.;
	switch (select_con2)
	{
	case 1:
		Y_deltarelT = (1 + sqrt(rho_1 * X_1)) / (1 + sqrt(rho_1 * XT_1));
		break;
	case 2:
		/*a.对于有较好屈服点的St
		  b.对于具有平稳增加的延伸曲线，以0.2%弹性极限应力作为条件的St、V和GGG(perl.bai.)
		  c.对于以产生初始裂纹时的应力为极限应力的渗碳处理的Eh、IF(root)
		  d.对于以产生初始裂纹时的应力为极限应力的NT、NV
		  e.对以产生初始裂纹时应力为极限应力的GTS
		  f.对于取断裂极限为极限应力的GG、GGG(ferr.) */
		string select_2_1;
		if (select_2_1 == "a")
		{
			double sigma_s = 800.0;
			Y_deltarelT = 1.0 + 0.93 * (Y_S - 1.0) * pow(200.0 / sigma_s, 1.0 / 4.0) / (1 + 0.93 * pow(200.0 / sigma_s, 1.0 / 4.0));
		}
		else if (select_2_1 == "b")
		{
			double sigma_0_2 = 700.0;
			Y_deltarelT = 1.0 + 0.82 * (Y_S - 1.0) * pow(300.0 / sigma_0_2, 1.0 / 4.0) / (1 + 0.82 * pow(300.0 / sigma_0_2, 1.0 / 4.0));
		}
		else if (select_2_1 == "c")
		{
			Y_deltarelT = 0.44 * Y_S + 0.12;
		}
		else if (select_2_1 == "d")
		{
			Y_deltarelT = 0.20 * Y_S + 0.60;
		}
		else if (select_2_1 == "e")
		{
			Y_deltarelT = 0.075 * Y_S + 0.85;
		}
		else if (select_2_1 == "f")
		{
			Y_deltarelT = 1.0;
		}
		else
		{
			Y_deltarelT = 0.;//输入错误
		}

		break;
	}
	return Y_deltarelT;
}
double getY_RrelT(double R_aF)
{
	double R_zF = 6.0 * R_aF;
	double Y_RrelT = 0.;
	/*a.耐久性极限的Y_RrelT
	  b.静强度的Y_RrelT*/
	string select_con3 = "a";
	if (select_con3 == "a")
	{
		/*1.对于V、GGG*(perl.,bai.)、Eh、IF(root)和GTS(perl.)
		  2.对于St
		  3.对于GG、GGG(ferr.)和NT、NV*/
		int select_Rz = 1;
		switch (select_Rz)
		{
		case 1:
			if (R_zF < 1)
			{
				Y_RrelT = 1.12;
			}
			if (1 <= R_zF && R_zF <= 40)
			{
				Y_RrelT = 1.674 - 0.529 * pow(R_zF + 1, 0.1 / 1.0);
			}
			break;
		case 2:
			if (R_zF < 1)
			{
				Y_RrelT = 1.07;
			}
			if (1 <= R_zF && R_zF <= 40)
			{
				Y_RrelT = 5.306 - 4.203 * pow(R_zF + 1, 0.01 / 1.0);
			}
			break;
		case 3:
			if (R_zF < 1)
			{
				Y_RrelT = 1.025;
			}
			if (1 <= R_zF && R_zF <= 40)
			{
				Y_RrelT = 4.229 - 3.259 * pow(R_zF + 1, 0.005 / 1.0);
			}
			break;
		}
	}
	else if (select_con3 == "b")
	{
		Y_RrelT = 1.0;
	}
	return Y_RrelT;
}
double getY_X(int material, double m_n)
{
	double Y_X = 1.0;//初始化
	/*1.耐久性极限下的尺寸系数Y_X
	  2.静强度下的尺寸系数Y_x*/
	int select_con4 = 1;
	switch (select_con4)
	{
	case 1:
		if (material == 1 || material == 2 || material == 3 || material == 4 || material == 5 || material == 8 || material == 9)
		{
			if (m_n <= 5)
			{
				Y_X = 1.0;
			}
			else if (5 < m_n && m_n < 30)
			{
				Y_X = 1.03 - 0.006 * m_n;
			}
			else
			{
				Y_X = 0.85;
			}
		}
		else if (material == 10 || material == 11 || material == 12 || material == 13 || material == 14)
		{
			if (m_n <= 5)
			{
				Y_X = 1.0;
			}
			else if (5 < m_n && m_n < 30)
			{
				Y_X = 1.05 - 0.01 * m_n;
			}
			else
			{
				Y_X = 0.8;
			}
		}
		else if (material == 6 || material == 7)
		{
			if (m_n <= 5)
			{
				Y_X = 1.0;
			}
			else if (5 < m_n && m_n < 30)
			{
				Y_X = 1.075 - 0.0015 * m_n;
			}
			else
			{
				Y_X = 0.7;
			}
		}
		break;
	case 2:
		Y_X = 1.0;
		break;
	}
	return Y_X;
}
double* Gear_ISO6336()
{
#pragma region 材料 
	switch (SelofMaterial_2)//小轮的材料选择
	{
	case 1:
		Material_1 = "St1";//材料：正火低碳钢/铸钢  类型：锻造正火低碳钢
		break;
	case 2:
		Material_1 = "St_cast1"; //材料：正火低碳钢 / 铸钢  类型：铸钢
		break;
	case 3:
		Material_1 = "GTS_perl1";//材料：铸铁材料  类型：黑心可锻铸铁（珠光体）
		break;
	case 4:
		Material_1 = "GGG_perl1";//材料：铸铁材料  类型：球墨铸铁（珠光体）
		break;
	case 5:
		Material_1 = "GGG_bai1";//材料：铸铁材料  类型：球墨铸铁（贝氏体）
		break;
	case 6:
		Material_1 = "GGG_ferr1";//材料：铸铁材料  类型：球墨铸铁（铁素体）
		break;
	case 7:
		Material_1 = "GG1";//材料：铸铁材料  类型：灰口铸铁
		break;
	case 8:
		Material_1 = "V1";//材料：整体硬化锻钢  类型：碳钢，合金钢
		break;
	case 9:
		Material_1 = "V_cast1";//材料：整体硬化铸钢  类型：碳钢，合金钢
		break;
	case 10:
		Material_1 = "Eh1";//材料：渗碳淬火锻钢
		break;
	case 11:
		Material_1 = "IF1";//材料：火焰或感应淬火锻或铸钢
		break;
	case 12:
		Material_1 = "NT_nitr1";//材料：渗氮锻钢/渗氮钢/整体硬化钢，渗氮  类型：渗氮钢
		break;
	case 13:
		Material_1 = "NV_nitr1";//材料：渗氮锻钢/渗氮钢/整体硬化钢，渗氮  类型：整体硬化钢
		break;
	case 14:
		Material_1 = "NV_nitrocar1";//材料：渗碳共渗锻钢  类型：整体硬化钢
		break;
	default:
		break;
	}

	switch (SelofMaterial_2)//大轮的材料选择
	{
	case 1:
		Material_2 = "St2";//材料：正火低碳钢/铸钢  类型：锻造正火低碳钢
		break;
	case 2:
		Material_2 = "St_cast2"; //材料：正火低碳钢 / 铸钢  类型：铸钢
		break;
	case 3:
		Material_2 = "GTS_perl2";//材料：铸铁材料  类型：黑心可锻铸铁（珠光体）
		break;
	case 4:
		Material_2 = "GGG_perl2";//材料：铸铁材料  类型：球墨铸铁（珠光体）
		break;
	case 5:
		Material_2 = "GGG_bai2";//材料：铸铁材料  类型：球墨铸铁（贝氏体）
		break;
	case 6:
		Material_2 = "GGG_ferr2";//材料：铸铁材料  类型：球墨铸铁（铁素体）
		break;
	case 7:
		Material_2 = "GG2";//材料：铸铁材料  类型：灰口铸铁
		break;
	case 8:
		Material_2 = "V2";//材料：整体硬化锻钢  类型：碳钢，合金钢
		break;
	case 9:
		Material_2 = "V_cast2";//材料：整体硬化铸钢  类型：碳钢，合金钢
		break;
	case 10:
		Material_2 = "Eh2";//材料：渗碳淬火锻钢
		break;
	case 11:
		Material_2 = "IF2";//材料：火焰或感应淬火锻或铸钢
		break;
	case 12:
		Material_2 = "NT_nitr2";//材料：渗氮锻钢/渗氮钢/整体硬化钢，渗氮  类型：渗氮钢
		break;
	case 13:
		Material_2 = "NV_nitr2";//材料：渗氮锻钢/渗氮钢/整体硬化钢，渗氮  类型：整体硬化钢
		break;
	case 14:
		Material_2 = "NV_nitrocar2";//材料：渗碳共渗锻钢  类型：整体硬化钢
		break;
	default:
		break;
	}
#pragma endregion

#pragma region 端面模数
	m_n = m;//法向模数
	if (GearType == 1)//直齿的情况
	{
		m_t = m_n;//端面模数
	}
	if (GearType == 2)//斜齿的情况
	{
		m_t = m_n / cos(beta * PI / 180.0);//端面模数
	}
#pragma endregion

#pragma region 齿轮参数
	b = min(b1, b2);//工作齿宽
	Q1 = Q, Q2 = Q;//ISO精度等级
	u = z2 / z1;//齿数比
	d1 = m_t * z1;
	d2 = m_t * z2;//分度圆直径
	h_fP1 = 1.25 * m_n, h_fP2 = 1.25 * m_n;//渐开线圆柱齿轮基本齿条齿根高
	rho_fP1 = 0.30 * m_n, rho_fP2 = 0.30 * m_n;//渐开线圆柱齿轮基本齿条的齿根圆角半径
	alpha_t = atan(tan(alpha_n * PI / 180.0) / cos(beta * PI / 180.0)) * 180.0 / PI;//端面压力角
	alpha_wt = acos(abs(z1 + z2) * (m_n * cos(alpha_t * PI / 180.0) / (2.0 * a_w * cos(beta * PI / 180.0)))) * 180.0 / PI;//节圆压力角
	invalpha_n = tan(alpha_n * PI / 180.0) - alpha_n * PI / 180.0;
	invalpha_t = tan(alpha_t * PI / 180.0) - alpha_t * PI / 180.0;
	X_E1 = (W_k1 - (m_n * cos(alpha_n * PI / 180.0) * (PI * (k1 - 0.5) + z1 * invalpha_t))) / (2.0 * m_n * sin(alpha_n * PI / 180.0));
	X_E2 = (W_k2 - (m_n * cos(alpha_n * PI / 180.0) * (PI * (k2 - 0.5) + z2 * invalpha_t))) / (2.0 * m_n * sin(alpha_n * PI / 180.0));
	d_f1 = d1 - 2.0 * (h_fP1 - (x1 * m_n));
	d_f2 = d2 - 2.0 * (h_fP2 - (x2 * m_n));//齿根圆直径
	d_b1 = d1 * cos(alpha_t * PI / 180.0);
	d_b2 = d2 * cos(alpha_t * PI / 180.0);//基圆直径
	d_Nf1 = 2.0 * sqrt(pow(sqrt(d_a1 * d_a1 / 4.0 - d_b1 * d_b1 / 4.0) - g_alpha, 2.0) + d_b1 * d_b1 / 4.0);
	d_Nf2 = 2.0 * sqrt(pow(sqrt(d_a2 * d_a2 / 4.0 - d_b2 * d_b2 / 4.0) - g_alpha, 2.0) + d_b2 * d_b2 / 4.0);//有效齿根圆直径
	beta_b =  asin(sin(beta * PI / 180.0) * cos(alpha_n * PI / 180.0)) * 180.0 / PI;//基圆螺旋角
	d_w1 = d_b1 / cos(alpha_wt * PI / 180.0);
	d_w2 = d_b2 / cos(alpha_t * PI / 180.0);//工作螺距直径
	d_m1 = (d_a1 + d_f1) / 2.0;//平均直径
	d_m2 = (d_a2 + d_f2) / 2.0;
	p_n = PI * m_n;
	p_t = PI * m_t;
	p_bt = p_t * cos(alpha_t * PI / 180.0);//端面基圆齿距
	p_et = p_bt;
	epsilon_alpha = g_alpha / p_et;//端面重合度
	epsilon_beta = b * sin(beta * PI / 180.0) / m_n / PI;//轴向重合度
	epsilon_gama = epsilon_alpha + epsilon_beta;
	f_pb = max(f_pb1, f_pb2);
	f_falpha = max(f_falpha1, f_falpha2);
	v = d1 * PI * n1 / 60000.0;//分度圆切向速度
	v_w = v;
	F_t = 2000.0 * T1 / d1;//啮合齿轮副分度圆上端面切向载荷
	N_L1 = n1 * 60.0 * Lh;//小齿轮载荷循环次数
	N_L2 = N_L1 / u;//大齿轮载荷循环系数
	R_zH1 = 4.8;//小齿轮的峰—粗糙度
	R_zH2 = 4.8;//大齿轮的峰—粗糙度
	R_zH = (R_zH1 + R_zH2) / 2.0;//齿轮副的平均峰—谷粗糙度
	p1 = 0.5 * d_b1 * tan(alpha_wt * PI / 180.0);
	p2 = 0.5 * d_b2 * tan(alpha_wt * PI / 180.0);
	p_red = p1 * p2 / (p1 + p2);//综合曲率半径
	HB1 = Case_h1 * 10.0;
	HB2 = Case_h1 * 10.0;
#pragma endregion

#pragma region 节点区域系数Z_H和齿面接触强度弹性系数Z_E和重合度系数Z_ε和螺旋角系数Z_β
	Z_H = sqrt(2.0 * cos(beta_b * PI / 180.0) * cos(alpha_wt * PI / 180.0) / cos(alpha_t * PI / 180.0) / cos(alpha_t * PI / 180.0) / sin(alpha_wt * PI / 180.0));
	double ResofZ_E = 1.0 / (PI * ((1 - v1 * v1) / E1 + (1 - v2 * v2) / E2));
	Z_E = pow(ResofZ_E, 1.0 / 2.0);
	switch (GearType)
	{
	case 1:
		Z_epsilon = pow((4.0 - epsilon_alpha) / 3.0, 1.0 / 2.0);
		break;
	case 2:
		if (epsilon_beta < 1)
		{
			Z_epsilon = pow((4.0 - epsilon_alpha) * (1.0 - epsilon_beta) / 3.0 + epsilon_beta / epsilon_alpha, 1.0 / 2.0);
		}
		else
		{
			Z_epsilon = pow(1 / epsilon_alpha, 1.0 / 2.0);
		}
		break;
	}
	Z_beta = sqrt(1 / cos(beta * PI / 180.0));
#pragma endregion

#pragma region  寿命系数Z_NT
#pragma region 参考寿命系数

	/*cout << "情况1：所选材料为St,V,GGG,GTS,Eh,IF,允许有限点蚀" << endl;
	cout << "情况2：所选材料为St,V,GGG,GTS,Eh,IF" << endl;
	cout << "情况3：所选材料为GG,GGG,NT,NV" << endl;
	cout << "情况4：所选材料为NV" << endl;*/
	switch (SelofZ_NT1)
	{
	case 1:
		if (N_L1 <= 6 * pow(10, 5))//静强度
		{
			Z_NT1 = 1.6;
		}
		if (6 * pow(10, 5) < N_L1 && N_L1 <= pow(10, 7))
		{
			Z_NT1 = 1.3;
		}
		if (pow(10, 7) < N_L1 && N_L1 <= pow(10, 9))
		{
			Z_NT1 = 1.0;
		}
		if (pow(10, 9) < N_L1 && N_L1 <= pow(10, 10))
		{
			Z_NT1 = 0.975;
		}
		break;
	case 2:
		if (N_L1 <= pow(10, 5))//静强度
		{
			Z_NT1 = 1.6;
		}
		if (pow(10, 5) < N_L1 && N_L1 <= 5 * pow(10, 7))
		{
			Z_NT1 = 1.0;
		}
		if (5 * pow(10, 7) < N_L1 && N_L1 <= pow(10, 8))
		{
			Z_NT1 = 1.0;
		}
		if (pow(10, 8) < N_L1 && N_L1 <= pow(10, 10))
		{
			Z_NT1 = 0.975;
		}
		break;
	case 3:
		if (N_L1 <= pow(10, 5))//静强度
		{
			Z_NT1 = 1.3;
		}
		if (pow(10, 5) < N_L1 && N_L1 <= 2 * pow(10, 6))
		{
			Z_NT1 = 1.0;
		}
		if (2 * pow(10, 6) < N_L1 && N_L1 <= pow(10, 10))
		{
			Z_NT1 = 0.975;
		}
		break;
	case 4:
		if (N_L1 <= pow(10, 5))//静强度
		{
			Z_NT1 = 1.1;
		}
		if (pow(10, 5) < N_L1 && N_L1 <= 2 * pow(10, 6))
		{
			Z_NT1 = 1.0;
		}
		if (2 * pow(10, 6) < N_L1 && N_L1 <= pow(10, 10))
		{
			Z_NT1 = 0.975;
		}
		break;
	}
	switch (SelofZ_NT2)
	{
	case 1:
		if (N_L2 <= 6 * pow(10, 5))//静强度
		{
			Z_NT2 = 1.6;
		}
		if (6 * pow(10, 5) < N_L2 && N_L2 <= pow(10, 7))
		{
			Z_NT2 = 1.3;
		}
		if (pow(10, 7) < N_L2 && N_L2 <= pow(10, 9))
		{
			Z_NT2 = 1.0;
		}
		if (pow(10, 9) < N_L2 && N_L2 <= pow(10, 10))
		{
			Z_NT2 = 0.96176;
		}
		break;
	case 2:
		if (N_L2 <= pow(10, 5))//静强度
		{
			Z_NT2 = 1.6;
		}
		if (pow(10, 5) < N_L2 && N_L2 <= 5 * pow(10, 7))
		{
			Z_NT2 = 1.0;
		}
		if (5 * pow(10, 7) < N_L2 && N_L2 <= pow(10, 8))
		{
			Z_NT2 = 1.0;
		}
		if (pow(10, 8) < N_L2 && N_L2 <= pow(10, 10))
		{
			Z_NT2 = 0.96176;
		}
		break;
	case 3:
		if (N_L2 <= pow(10, 5))//静强度
		{
			Z_NT2 = 1.3;
		}
		if (pow(10, 5) < N_L2 && N_L2 <= 2 * pow(10, 6))
		{
			Z_NT2 = 1.0;
		}
		if (2 * pow(10, 6) < N_L2 && N_L2 <= pow(10, 10))
		{
			Z_NT2 = 0.96176;
		}
		break;
	case 4:
		if (N_L2 <= pow(10, 5))//静强度
		{
			Z_NT2 = 1.1;
		}
		if (pow(10, 5) < N_L2 && N_L2 <= 2 * pow(10, 6))
		{
			Z_NT2 = 1.0;
		}
		if (2 * pow(10, 6) < N_L2 && N_L2 <= pow(10, 10))
		{
			Z_NT2 = 0.96176;
		}
		break;
	}
#pragma endregion

#pragma region 图解值
	Z_NT1 = 0.975;
	Z_NT2 = 1.014;
#pragma endregion
#pragma endregion

#pragma region 润滑剂系数Z_L和速度系数Z_v和粗糙度系数Z_R和尺寸系数Z_X和工作硬化系数Z_W
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
	Z_L = C_ZL + 4.0 * (1.0 - C_ZL) / pow(1.2 + 134.0 / v_40, 2.0);
	C_Zv = C_ZL + 0.02;
	Z_v = C_Zv + 2.0 * (1.0 - C_Zv) / pow(0.8 + (32.0 / v_w), 1.0 / 2.0);
	R_zH10 = R_zH * pow(10.0 / p_red, 1.0 / 3.0);//齿轮副的表面平均峰—谷粗糙度
	if (850.0 <= sigma_Hlim && sigma_Hlim <= 1200.0)
	{
		C_ZR = 0.32 - 0.0002 * sigma_Hlim;
	}
	else if (sigma_Hlim < 850.0)
	{
		C_ZR = 0.15;
	}
	else
	{
		C_ZR = 0.08;
	}
	Z_R = pow(3.0 / R_zH10, C_ZR);
	Z_X = 1.0;
	if (u > 20.0)
	{
		u = 20.0;
	}
	double A = 0.00898 * HB1 / HB2 - 0.00829;
	if (1.2 <= HB1 || HB2 <= 1.7)
	{
		Z_W = 1.0 + A * (u - 1.0);//工作硬化系数（耐久性极限和高周疲劳）
	}
	if ((HB1 / HB2) < 1.2)
	{
		Z_W = 1.0;
	}
	if ((HB1 / HB2) > 1.7)
	{
		Z_W = 1.0 + 0.00698 * (u - 1.0);
	}
#pragma endregion

#pragma region  啮合系数Z_B和Z_D
	/* 1.斜齿轮齿对中经齿廓和螺旋线修形使得载荷分配合理，并且在接触应力均布，最大接触应力靠近节线
	   2.斜齿轮齿对根据制造商的经验进行了适当的齿面修行
	   3.斜齿轮齿无修形处理*/
	if (Selof_ZCa == 1)
	{
		f_ZCa = 1.0;
	}
	else if (Selof_ZCa == 2)
	{
		f_ZCa = 1.07;
	}
	else if (Selof_ZCa == 3)
	{
		f_ZCa = 1.2;
	}
	double ResofM1 = (sqrt((d_a1 * d_a1 / d_b1 / d_b1) - 1.0) - 2.0 * PI / z1) * (sqrt((d_a2 * d_a2 / d_b2 / d_b2) - 1.0) - (epsilon_alpha - 1.0) * 2.0 * PI / z2);
	M1 = tan(alpha_wt * PI / 180.0) / sqrt(ResofM1);//小齿轮模数M1
	double ResofM2 = (sqrt((d_a2 * d_a2 / d_b2 / d_b2) - 1.0) - 2.0 * PI / z2) * (sqrt((d_a1 * d_a1 / d_b1 / d_b1) - 1.0) - (epsilon_alpha - 1.0) * 2.0 * PI / z1);
	M2 = tan(alpha_wt * PI / 180.0 / ResofM2);	//大齿轮模数M2
	switch (GearType)
	{
	case 1: 
		if (M1 <= 1)
		{
			Z_B = 1;
		}
		else
		{
			Z_B = M1;
		}
		if (M2 <= 1)
		{
			Z_D = 1;
		}
		else
		{
			Z_D = M2;
		}
		break;
	case 2:
		if (epsilon_alpha > 1 && epsilon_beta >= 1)
		{
			Z_B = sqrt(f_ZCa);
			Z_D = Z_B;
		}
		else if (epsilon_alpha > 1 && epsilon_beta < 1)
		{
			if (M1 <= 1)
			{
				Z_B = 1 * epsilon_beta * (sqrt(f_ZCa) - 1);
			}
			else
			{
				Z_B = M1 + epsilon_beta * (sqrt(f_ZCa) - M1);
			}
			if (M2 <= 1)
			{
				Z_D = 1 + epsilon_beta * (sqrt(f_ZCa) - 1);
			}
			else
			{
				Z_D = M2 + epsilon_beta * (sqrt(f_ZCa) - M2);
			}
		}
		break;
	}
#pragma endregion

#pragma region 动载系数K_v
	C_B1 = (1.0 + 0.5 * (1.2 - h_fP1 / m_n)) * (1.0 - 0.02 * (20.0 - alpha_pn));//小轮基本齿条系数
	C_B2 = (1.0 + 0.5 * (1.2 - h_fP2 / m_n)) * (1.0 - 0.02 * (20.0 - alpha_pn));//大轮基本齿条系数
	C_B = 0.5 * (C_B1 + C_B2);
	//小齿轮当量齿数
	z_n1 = z1 / cos(beta_b * PI / 180.0) / cos(beta_b * PI / 180.0) / cos(beta * PI / 180.0);
	//大齿轮当量齿数
	z_n2 = z2 / cos(beta_b * PI / 180.0) / cos(beta_b * PI / 180.0) / cos(beta * PI / 180.0);
	//一对轮齿柔度的最小值
	q_1 = C1 + C2 / z_n1 + C3 / z_n2 + C4 * x1 + C5 * x1 / z_n1 + C6 * x2 + C7 * x2 / z_n2 + C8 * x1 * x1 + C9 * x2 * x2;
	cth_1 = 1.0 / q_1;//理论单对齿刚度
	if (F_t * K_A / b < 100.0)
	{
		c_1 = cth_1 * C_M * C_R * C_B * cos(beta * PI / 180.0) * pow(F_t * K_A / b / 100.0, 0.25);
	}
	else
	{
		c_1 = cth_1 * C_M * C_R * C_B * cos(beta * PI / 180.0);//单对齿啮合刚度
	}
	//啮合刚度c_γα
	if (GearType == 1)//直齿的情况
	{
		if (epsilon_alpha >= 1.2)
		{
			c_gama_alpha = c_1 * (0.75 * epsilon_alpha + 0.25);
		}
		if (epsilon_alpha < 1.2)
		{
			c_gama_alpha = 0.9 * c_1 * (0.75 * epsilon_alpha + 0.25);
		}
	}
	if (GearType == 2)//斜齿的情况
	{
		if ((epsilon_alpha >= 1.2) && (beta <= 30))
		{
			c_gama_alpha = c_1 * (0.75 * epsilon_alpha + 0.25);
		}
		if (epsilon_alpha < 1.2)
		{
			c_gama_alpha = 0.9 * c_1 * (0.75 * epsilon_alpha + 0.25);
		}
	}
	//啮合刚度c_γβ
	c_gama_beta = 0.85 * c_gama_alpha;
	m_red = PI / 8.0 * (d_m1 * d_m1 / d_b1 / d_b1) * d_m1 * d_m1 / (1.0 / (1.0 - q1 * q1 * q1 * q1) / rho1 + 1.0 / (1.0 - q2 * q2 * q2 * q2) / rho2 / u / u);
	n_E1 = 30000.0 / (PI * z1) * sqrt(c_gama_alpha / m_red);//共振转速
	N = round(n1 / n_E1 * 1000.0) / 1000.0;
	//在主共振区内的临界转速比N_S
	if (F_t * K_A / b < 100)
	{
		N_S = 0.5 + 0.35 * sqrt(F_t * K_A / (100.0 * b));
	}
	if (F_t * K_A / b >= 100)
	{
		N_S = 0.85;
	}
	if (1 < epsilon_gama && epsilon_gama <= 2)
	{
		Cv1 = 0.32;//考虑齿距偏差影响的系数并假定为常数
		Cv2 = 0.34;//考虑齿形偏差影响的系数
		Cv3 = 0.23;//考虑啮合刚度周期变化影响的系数
		Cv4 = 0.90;//考虑由啮合刚度周期性变化引起的齿轮副扭转共振的影响系数
		Cv5 = 0.47;
		Cv6 = 0.47;
	}
	if (epsilon_gama > 2)
	{
		Cv1 = 0.32;//考虑齿距偏差影响的系数并假定为常数
		Cv2 = 0.57 / (epsilon_gama - 0.3);//考虑齿形偏差影响的系数
		Cv3 = 0.096 / (epsilon_gama - 1.56);//考虑啮合刚度周期变化影响的系数
		Cv4 = (0.57 - 0.05 * epsilon_gama) / (epsilon_gama - 1.44);
		Cv5 = 0.47;
		Cv6 = 0.12 / (epsilon_gama - 1.74);
	}
	if (1 < epsilon_gama && epsilon_gama <= 1.5)
	{
		Cv7 = 0.75;//考虑因啮合刚度变化，在高的恒定转速运行时轮齿弯曲变形产生的载荷分量的影响系数
	}
	if (1.5 < epsilon_gama && epsilon_gama <= 2.5)
	{
		Cv7 = 0.125 * sin(PI * (epsilon_gama - 2.0)) + 0.875;
	}
	if (epsilon_gama > 2.5)
	{
		Cv7 = 1.0;
	}
	if (SelofMaterial_1 == SelofMaterial_2)//材料相同
	{
		y_p = gety_alpha(SelofMaterial_1, f_pb, v);
	}
	else//材料不同
	{
		double y_alpha1 = gety_alpha(SelofMaterial_1, f_pb, v);
		double y_alpha2 = gety_alpha(SelofMaterial_1, f_pb, v);
		y_p = (y_alpha1 + y_alpha2) / 2.0;
	}
	
	if (SelofMaterial_1 == SelofMaterial_2)//材料相同
	{
		y_f = gety_alpha(SelofMaterial_1, f_falpha, v);
	}
	else//材料不同
	{
		double y_alpha1 = gety_alpha(SelofMaterial_1, f_falpha, v);
		double y_alpha2 = gety_alpha(SelofMaterial_1, f_falpha, v);
		y_f = (y_alpha1 + y_alpha2) / 2.0;
	}
	f_pb_eff = f_pb - y_p;
	f_falpha_eff = f_falpha - y_f;
	Bp = c_1 * f_pb_eff / (K_A * F_t / b);//Bp,Bf,Bk是考虑轮齿偏差及修形对动载荷的影响的无量纲参数
	Bf = c_1 * f_falpha_eff / (K_A * F_t / b);
	Bk = abs(1 - c_1 * C_a / (K_A * F_t / b));
	K = (Cv1 * Bp) + (Cv2 * Bf) + (Cv3 * Bk);
	if (N <= N_S)//亚临界区,大多数工业齿轮在此区间内工作
	{
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
		K_v = Cv5 * Bp + Cv6 * Bf + Cv7 + ((Cv1 * Bp + Cv2 * Bf + Cv4 * Bk + 1) - (Cv5 * Bp + Cv6 * Bf + Cv7)) / 0.35 * (1.5 - N);
	}
	K_v = ceil(K_v * 1000.0) / 1000.0;
#pragma endregion

#pragma region 接触强度计算时的螺旋线载荷分配系数K_Hβ和弯曲强度计算时的螺旋线载荷分配系数K_Fβ
	if (Beta_Shape == 1)//不修形
	{
		B1 = 1.0;
		B2 = 1.0;
	}
	if (Beta_Shape == 2)//仅中心鼓形修形
	{
		B1 = 1.0;
		B2 = 0.5;
	}
	if (Beta_Shape == 3)//仅螺旋线修形
	{
		B1 = pow(0.1, e);
		B2 = 1.0;
	}
	if (Beta_Shape == 4)//螺旋线修形加中心鼓形修形
	{
		B1 = pow(0.1, e);
		B2 = 0.5;
	}
	if (Beta_Shape == 5)//齿端修薄
	{
		B1 = 0.7;
		B2 = 0.7;
	}
	F_m = F_t * K_A * K_v;
	//由小齿轮和大齿轮的轴变形引起的当量螺旋线偏啮分量f_sh和由制造偏差引起的螺旋线偏啮分量f_ma
	f_sh = F_m / b * 0.023 * (abs(B_x + K_x * L * s1 / d1 / d1 * pow(d1 / d_sh, 4.0) - 0.3) + 0.3) * pow(b / d1, 2.0);
	f_ma = sqrt(f_Hbeta1 * f_Hbeta1 + f_Hbeta2 * f_Hbeta2);
	F_beta_x = 1.33 * B1 * f_sh + B2 * f_ma;//初始当量螺旋线偏啮量
	if (Material_1 == Material_2)//材料相同
	{
		y_beta = gety_beta(SelofMaterial_1, F_beta_x, v);
		X_beta = getX_beta(SelofMaterial_1);
	}
	else//材料不同
	{
		double y_beta1 = gety_beta(SelofMaterial_1, F_beta_x, v);
		double X_beta1 = getX_beta(SelofMaterial_1);
		double y_beta2 = gety_beta(SelofMaterial_2, F_beta_x, v);
		double X_beta2 = getX_beta(SelofMaterial_2);
		y_beta = (y_beta1 + y_beta2) / 2.0;
		X_beta = (X_beta1 + X_beta2) / 2.0;
	}
	F_beta_y = F_beta_x - y_beta;//有效当量螺旋线偏啮量
	if ((F_beta_y * c_gama_beta / (2.0 * F_m / b)) >= 1.0)
	{
		K_Hbeta = sqrt(2.0 * F_beta_y * c_gama_beta / (F_m / b));
	}
	if ((F_beta_y * c_gama_beta / (2.0 * F_m / b)) < 1.0)
	{
		K_Hbeta = 1.0 + F_beta_y * c_gama_beta / (2.0 * F_m / b);
	}
	double h1 = (d_a1 - d_f1) / 2.0;
	double h2 = (d_a2 - d_f2) / 2.0;
	b_h = min((b1 / h1), (b2 / h2));
	if (b_h < 3)//极限状况
	{
		b_h = 3.0;
	}
	N_F = b_h * b_h / (1.0 + b_h + b_h * b_h);
	K_Fbeta = pow(K_Hbeta, N_F);
#pragma endregion 

#pragma region 接触强度计算时的齿间载荷分配系数K_Hα和弯曲强度计算时的齿间载荷分配系数K_Fα
	F_tH = F_t * K_A * K_v * K_Hbeta;//端平面内起决定作用的切向载荷
	if (epsilon_gama <= 2)
	{
		K_Falpha = epsilon_gama / 2.0 + (0.9 + 0.4 * c_gama_alpha * (f_pb - y_alpha) / (F_tH / b));
		K_Halpha = K_Falpha;
	}
	if (epsilon_gama > 2)
	{
		K_Falpha = 0.9 + 0.4 * sqrt(2.0 * (epsilon_gama - 1.0) / epsilon_gama) * c_gama_alpha * (f_pb - y_alpha) / (F_tH / b);
		K_Halpha = K_Falpha;
	}
	if (K_Halpha > (epsilon_gama / (epsilon_alpha * Z_epsilon * Z_epsilon)))//K_Hα的限制条件
	{
		K_Halpha = epsilon_gama / (epsilon_alpha * Z_epsilon * Z_epsilon);
	}
	if (K_Halpha < 1.0)//K_Hα的限制条件
	{
		K_Halpha = 1.0;
	}
	if (K_Falpha > (epsilon_gama / (0.25 * epsilon_alpha + 0.75)))//K_Fα的限制条件
	{
		K_Falpha = epsilon_gama / (0.25 * epsilon_alpha + 0.75);
	}
	if (K_Falpha < 1.0)//K_Fα的限制条件
	{
		K_Falpha = 1.0;
	}
#pragma endregion 

#pragma region 接触强度
	//节点处计算齿面接触应力的基本值
	sigma_H0 = Z_H * Z_E * Z_epsilon * Z_beta * sqrt(F_t * (u + 1) / (d1 * b * u));
	//接触应力σH
	sigma_H1 = Z_B * sigma_H0 * sqrt(K_A * K_gama * K_v * K_Hbeta * K_Halpha);
	sigma_H2 = Z_D * sigma_H0 * sqrt(K_A * K_gama * K_v * K_Hbeta * K_Halpha);
	//有限寿命下的许用接触应力σHP
	sigma_HP1 = sigma_Hlim * Z_NT1 / S_Hmin * Z_L * Z_v * Z_R * Z_W * Z_X;
	sigma_HP2 = sigma_Hlim * 1.014 / S_Hmin * Z_L * Z_v * Z_R * Z_W * Z_X;
	//安全系数S_H
	S_H1 = sigma_HP1 * S_Hmin / sigma_H2;
	S_H2 = sigma_HP2 * S_Hmin / sigma_H2;
#pragma endregion

#pragma region 载荷分布影响系数f_ε
	epsilon_alphan = epsilon_alpha / cos(beta_b * PI / 180.0) / cos(beta_b * PI / 180.0);
	if (epsilon_beta == 0)
	{
		if (epsilon_alphan < 2.0)
		{
			f_epsilon = 1.0;
		}
		else
		{
			f_epsilon = 0.7;
		}
	}
	if (0 < epsilon_beta && epsilon_beta < 1.0)
	{
		if (epsilon_alphan < 2.0)
		{
			f_epsilon = sqrt(1.0 - epsilon_beta + epsilon_beta / epsilon_alphan);
		}
		else
		{
			f_epsilon = sqrt((1.0 - epsilon_beta) / 2.0 + epsilon_beta / epsilon_alphan);
		}
	}
	else
	{
		f_epsilon = pow(epsilon_alphan, -0.5);
	}
#pragma endregion

#pragma region 齿廓系数Y_F
	//小齿轮当量直齿轮参数
	d_n1 = z_n1 * m_n;	
	p_bn1 = PI * m_n * cos(alpha_n * PI / 180.0);
	d_bn1 = d_n1 * cos(alpha_n * PI / 180.0);
	d_an1 = d_n1 + d_a1 - d1;
	d_en1 = 2.0 * (z1 / abs(z1)) * (sqrt(pow(sqrt(d_an1 * d_an1 / 4.0 - d_bn1 * d_bn1 / 4.0) - PI * d1 * cos(beta * PI / 180.0) * cos(alpha_n * PI / 180.0) * (epsilon_alphan - 1.0) / abs(z1), 2.0) + d_bn1 * d_bn1 / 4.0));
	//小齿轮载荷作用角α_en1
	alpha_en1 = acos(d_bn1 / d_en1) * 180.0 / PI;
	invalpha_en1 = tan(alpha_en1 * PI / 180.0) - alpha_en1 * PI / 180.0;
	//小齿轮当量直齿轮辅助值γ_e1
	gama_e1 = ((0.5 * PI + 2.0 * x1 * tan(alpha_n * PI / 180.0)) / z_n1 + invalpha_n - invalpha_en1) * 180.0 / PI;
	//小齿轮齿廓系数压力角α_Fen1
	alpha_Fen1 = alpha_en1 - gama_e1;
	rho_fPv1 = rho_fP1;
	double E1 = PI * m_n / 4.0 - h_fP1 * tan(alpha_n * PI / 180.0) + s_pr1 / cos(alpha_n * PI / 180.0) - (1.0 - sin(alpha_n * PI / 180.0)) * rho_fP1 / cos(alpha_n * PI / 180.0);
	double G1 = rho_fPv1 / m_n - h_fP1 / m_n + x1;
	double H1 = 2.0 / z_n1 * (PI / 2.0 - E1 / m_n) - PI / 3.0;
	double theta1 = PI / 6.0;
	double i1 = 1;
	while (1)
	{
		theta1 = 2.0 * G1 * tan(theta1) / z_n1 - H1;
		i1++;
		if (i1 > 7)
		{
			break;
		}
	}
	s_Fn1 = m_n * (z_n1 * sin(PI / 3.0 - theta1) + sqrt(3.0) * (G1 / cos(theta1) - rho_fPv1 / m_n));
	rho_F1 = m_n * (rho_fPv1 / m_n + 2.0 * G1 * G1 / cos(theta1) / (z_n1 * (cos(theta1) * cos(theta1)) - 2.0 * G1));
	//小齿轮齿根应力计算中载荷作用于单对齿啮合区外界点时的弯曲力臂h_Fe1
	h_Fe1 = m_n * (0.5 * ((cos(gama_e1 * PI / 180.0) - sin(gama_e1 * PI / 180.0) * tan(alpha_Fen1 * PI / 180.0)) * d_en1 / m_n - z_n1 * cos(PI / 3.0 - theta1) - (G1 / cos(theta1) - rho_fPv1 / m_n)));
	//小齿轮齿廓系数Y_F1
	Y_F1 = 6.0 * h_Fe1 / m_n * cos(alpha_Fen1 * PI / 180.0) / (s_Fn1 / m_n) / (s_Fn1 / m_n) / cos(alpha_n * PI / 180.0);
	//大齿轮当量直齿轮参数	
	d_n2 = z_n2 * m_n;
	p_bn2 = PI * m_n * cos(alpha_n * PI / 180.0);
	d_bn2 = d_n2 * cos(alpha_n * PI / 180.0);
	d_an2 = d_n2 + d_a2 - d2;
	//大齿轮载荷作用角α_en2
	d_en2 = 2.0 * (z2 / abs(z2)) * (sqrt(pow(sqrt(d_an2 * d_an2 / 4.0 - d_bn2 * d_bn2 / 4.0) - PI * d2 * cos(beta * PI / 180.0) * cos(alpha_n * PI / 180.0) * (epsilon_alphan - 1.0) / abs(z2), 2.0) + d_bn2 * d_bn2 / 4.0));
	alpha_en2 = acos(d_bn2 / d_en2) * 180.0 / PI;
	invalpha_en2 = tan(alpha_en2 * PI / 180.0) - alpha_en2 * PI / 180.0;
	//大齿轮当量直齿轮辅助值γ_e2
	gama_e2 = ((0.5 * PI + 2.0 * x2 * tan(alpha_n * PI / 180.0)) / z_n2 + invalpha_n - invalpha_en2) * 180.0 / PI;
	//大齿轮齿廓系数压力角α_Fen2
	alpha_Fen2 = alpha_en2 - gama_e2;
	rho_fPv2 = rho_fP2;
	double E2 = PI * m_n / 4.0 - h_fP2 * tan(alpha_n * PI / 180.0) + s_pr2 / cos(alpha_n * PI / 180.0) - (1.0 - sin(alpha_n * PI / 180.0)) * rho_fP2 / cos(alpha_n * PI / 180.0);
	double G2 = rho_fPv2 / m_n - h_fP2 / m_n + x2;
	double H2 = 2.0 / z_n2 * (PI / 2.0 - E2 / m_n) - PI / 3.0;
	double theta2 = PI / 6.0;
	double i2 = 1.0;
	while (1)
	{
		theta2 = 2.0 * G2 * tan(theta2) / z_n2 - H2;
		i2++;
		if (i2 > 5.0)
		{
			break;
		}
	}
	s_Fn2 = m_n * (z_n2 * sin(PI / 3.0 - theta2) + sqrt(3) * (G2 / cos(theta2) - rho_fPv2 / m_n));
	rho_F2 = m_n * (rho_fPv2 / m_n + 2.0 * G2 * G2 / cos(theta2) / (z_n2 * (cos(theta2) * cos(theta2)) - 2.0 * G2));
	//大齿轮齿根应力计算中载荷作用于单对齿啮合区外界点时的弯曲力臂h_Fe2
	h_Fe2 = m_n * (0.5 * ((cos(gama_e2 * PI / 180.0) - sin(gama_e2 * PI / 180.0) * tan(alpha_Fen2 * PI / 180.0)) * d_en2 / m_n - z_n2 * cos(PI / 3.0 - theta2) - (G2 / cos(theta2) - rho_fPv2 / m_n)));
	//大齿轮齿廓系数Y_F2
	Y_F2 = 6.0 * h_Fe2 / m_n * cos(alpha_Fen2 * PI / 180.0) / (s_Fn2 / m_n) / (s_Fn2 / m_n) / cos(alpha_n * PI / 180.0);
#pragma endregion

#pragma region  应力修正系数Y_S
	q_s1 = s_Fn1 / 2.0 / rho_F1;
	q_s2 = s_Fn2 / 2.0 / rho_F2;
	L1 = s_Fn1 / h_Fe1;
	L2 = s_Fn2 / h_Fe2;
	if (1.0 <= q_s1 && q_s1 < 8.0	)
	{
		double RelofY_S1 = 1.0 / (1.21 + 2.3 / L1);
		Y_S1 = (1.2 + 0.13 * L1) * pow(q_s1, RelofY_S1);
	}
	else
	{
		Y_S1 = 0.;//计算错误
	}
	if (1.0 <= q_s2 && q_s2 < 8.0)
	{
		double RelofY_S2 = 1.0 / (1.21 + 2.3 / L2);
		Y_S2 = (1.2 + 0.13 * L2) * pow(q_s2, RelofY_S2);
	}
	else
	{
		Y_S2 = 0.;//计算错误
	}
#pragma endregion

#pragma region 螺旋角系数Y_β和轮缘厚度系数Y_B和齿高系数Y_DT
	//螺旋角系数Y_β
	double epsilon_beta1 = 0.;
	double beta1 = 0.;
	if (epsilon_beta > 1.0)
	{
		epsilon_beta1 = 1.0;
	}
	else
	{
		epsilon_beta1 = epsilon_beta;
	}
	if (beta > 30.0)
	{
		beta1 = 30.0;
	}
	else
	{
		beta1 = beta;
	}
	Y_beta = (1.0 - epsilon_beta1 * beta1 / 120.0);
	//轮缘厚度系数Y_B
	Y_B1 = 1.0;
	Y_B2 = 1.0;
	//齿高系数Y_DT
	if (epsilon_alphan <= 2.05 || (Q > 4.0 && epsilon_alphan > 2.05))
	{
		Y_DT = 1.0;
	}
	if (2.05 < epsilon_alphan && epsilon_alphan <= 0.5 && Q <= 4.0)
	{
		Y_DT = -0.666 * epsilon_alphan + 2.366;
	}
	if (epsilon_alphan > 2.5 && Q <= 4.0)
	{
		Y_DT = 0.7;
	}
#pragma endregion

#pragma region 弯曲强度寿命系数Y_NT和与标准试验齿轮尺寸相关的应力修正系数Y_ST
	/*cout << "情况1：所选材料为St,V,GGG,GTS" << endl;
  cout << "情况2：所选材料为Eh,IF" << endl;
  cout << "情况3：所选材料为GG,GGG,NT,NV" << endl;
  cout << "情况4：所选材料为NV" << endl;*/
	int SelofY_ST1 = 2;
	int SelofY_ST2 = 2;
	switch (SelofY_ST1)
	{
	case 1:
		if (N_L1 <= pow(10, 4))//静强度
		{
			Y_NT1 = 2.5;
		}
		if (pow(10, 4) < N_L1 && N_L1 <= 3 * pow(10, 6))
		{
			Y_NT1 = 1.0;
		}
		if (3 * pow(10, 6) < N_L1 && N_L1 <= pow(10, 10))
		{
			Y_NT1 = 0.930;
		}
		break;
	case 2:
		if (N_L1 <= pow(10, 3))//静强度
		{
			Y_NT1 = 2.5;
		}
		if (pow(10, 3) < N_L1 && N_L1 <= 3 * pow(10, 6))
		{
			Y_NT1 = 1.0;
		}
		if (3 * pow(10, 6) < N_L1 && N_L1 <= pow(10, 10))
		{
			Y_NT1 = 0.930;
		}
		break;
	case 3:
		if (N_L1 <= pow(10, 3))//静强度
		{
			Y_NT1 = 1.6;
		}
		if (pow(10, 3) < N_L1 && N_L1 <= 3 * pow(10, 6))
		{
			Y_NT1 = 1.0;
		}
		if (3 * pow(10, 6) < N_L1 && N_L1 <= pow(10, 10))
		{
			Y_NT1 = 0.930;
		}
		break;
	case 4:
		if (N_L1 <= pow(10, 3))//静强度
		{
			Y_NT1 = 1.1;
		}
		if (pow(10, 3) < N_L1 && N_L1 <= 3 * pow(10, 6))
		{
			Y_NT1 = 1.0;
		}
		if (3 * pow(10, 6) < N_L1 && N_L1 <= pow(10, 10))
		{
			Y_NT1 = 0.930;
		}
		break;
	}
	switch (SelofY_ST2)
	{
	case 1:
		if (N_L2 <= pow(10, 4))//静强度
		{
			Y_NT2 = 2.5;
		}
		if (pow(10, 4) < N_L2 && N_L2 <= 3 * pow(10, 6))
		{
			Y_NT2 = 1.0;
		}
		if (3 * pow(10, 6) < N_L2 && N_L2 <= pow(10, 10))
		{
			Y_NT2 = 0.949;
		}
		break;
	case 2:
		if (N_L2 <= pow(10, 3))//静强度
		{
			Y_NT2 = 2.5;
		}
		if (pow(10, 3) < N_L2 && N_L2 <= 3 * pow(10, 6))
		{
			Y_NT2 = 1.0;
		}
		if (3 * pow(10, 6) < N_L2 && N_L2 <= pow(10, 10))
		{
			Y_NT2 = 0.949;
		}
		break;
	case 3:
		if (N_L2 <= pow(10, 3))//静强度
		{
			Y_NT2 = 1.6;
		}
		if (pow(10, 3) < N_L2 && N_L2 <= 3 * pow(10, 6))
		{
			Y_NT2 = 1.0;
		}
		if (3 * pow(10, 6) < N_L2 && N_L2 <= pow(10, 10))
		{
			Y_NT2 = 0.949;
		}
		break;
	case 4:
		if (N_L2 <= pow(10, 3))//静强度
		{
			Y_NT2 = 1.1;
		}
		if (pow(10, 3) < N_L2 && N_L2 <= 3 * pow(10, 6))
		{
			Y_NT2 = 1.0;
		}
		if (3 * pow(10, 6) < N_L2 && N_L2 <= pow(10, 10))
		{
			Y_NT2 = 0.949;
		}
		break;
	}
	//与标准试验齿轮尺寸相关的应力修正系数Y_ST
	Y_ST1 = 2.0;
	Y_ST2 = 2.0;
#pragma endregion

#pragma region 相对齿根圆角敏感系数Y_δrelT和相对齿根表面状况系数Y_RrelT和尺寸系数Y_X
	/*1.GG,σ_B=150N/mm^2
	 2.GG、GGG(ferr.),σ_B=300N/mm^2
	 3.NT、NV,适于所有硬化处理
	 4.St,σ_S=300N/mm^2
	 5.St,σ_S=400N/mm^2
	 6.V、GTS、GGG(perl.bai.),σ_S=500N/mm^2
	 7.V、GTS、GGG(perl.bai.),σ_S=600N/mm^2
	 8.V、GTS、GGG(perl.bai.),σ_S=800N/mm^2
	 9.V、GTS、GGG(perl.bai.),σ_S=1000N/mm^2
	 10.Eh、IF(root),适于所有硬化处理*/
	 //滑移厚度ρ'
	int select_con1 = 10;
	switch (select_con1)
	{
	case 1:
		rho1_1 = 0.3124;
		break;
	case 2:
		rho1_1 = 0.3095;
		break;
	case 3:
		rho1_1 = 0.1005;
		break;
	case 4:
		rho1_1 = 0.0833;
		break;
	case 5:
		rho1_1 = 0.0445;
		break;
	case 6:
		rho1_1 = 0.0281;
		break;
	case 7:
		rho1_1 = 0.0194;
		break;
	case 8:
		rho1_1 = 0.0064;
		break;
	case 9:
		rho1_1 = 0.0014;
		break;
	case 10:
		rho1_1 = 0.0030;
		break;
	}
	rho2_1 = rho1_1;
	//相对齿根圆角敏感系数Y_δrelT
	Y_deltarelT1 = getY_deltarelT(q_s1, rho1_1, Y_S1);
	Y_deltarelT2 = getY_deltarelT(q_s2, rho2_1, Y_S2);
	//相对齿根表面状况系数Y_RrelT
	Y_RrelT1 = getY_RrelT(R_aF1);;
	Y_RrelT2 = getY_RrelT(R_aF2);
	//尺寸系数Y_X
	Y_X1 = getY_X(SelofMaterial_1, m_n);
	Y_X2 = getY_X(SelofMaterial_2, m_n);
#pragma endregion

#pragma region 弯曲强度
	//齿根弯曲应力基本值σ_F0
	sigma_F01 = F_t * Y_F1 * Y_S1 * Y_beta * Y_B1 * Y_DT / b1 / m_n;
	sigma_F02 = F_t * Y_F2 * Y_S2 * Y_beta * Y_B2 * Y_DT / b1 / m_n;
	//齿根弯曲应力σ_F
	sigma_F1 = sigma_F01 * K_A * K_gama * K_v * K_Fbeta * K_Falpha;
	sigma_F2 = sigma_F02 * K_A * K_gama * K_v * K_Fbeta * K_Falpha;
	//许用齿根弯曲应力极限σ_FP
	sigma_FP1 = sigma_Flim * Y_ST1 * Y_NT1 * Y_deltarelT1 * Y_RrelT1 * Y_X1 / S_Fmin;
	sigma_FP2 = sigma_Flim * Y_ST2 * Y_NT2 * Y_deltarelT2 * Y_RrelT2 * Y_X2 / S_Fmin;
	//弯曲强度安全系数
	S_F1 = sigma_FP1 * S_Fmin / sigma_F1;
	S_F2 = sigma_FP2 * S_Fmin / sigma_F2;
#pragma endregion

	static double output[] = {
		a_w,z1,z2,m,u,alpha_n,beta,b1,b2,x1,x2,d1,d2,
			 K_A, K_v, K_Hbeta, K_Fbeta, K_Halpha, K_Falpha, c_gama_alpha,c_gama_beta,
			 F_t,Z_H,Z_E,Z_epsilon,Z_beta,Z_B,Z_D,sigma_H0,sigma_H1,sigma_H2,sigma_HP1,sigma_HP2,Z_NT1,Z_NT2,Z_L,Z_v,Z_R,Z_W,Z_X,S_H1,S_H2,
			 Y_F1, Y_F2, Y_S1, Y_S2,Y_beta,Y_B1,Y_B2,Y_DT,sigma_F01,sigma_F02,sigma_F1,sigma_F2,sigma_FP1,sigma_FP2,Y_NT1,Y_NT2,Y_deltarelT1,Y_deltarelT2,Y_RrelT1,Y_RrelT2,Y_X1,Y_X2,S_F1,S_F2
	};
	return output;
}
int main()
{
	Gear_ISO6336();
	cout << "齿数比u=" << fixed << setprecision(5) << u << endl;
	cout << "*********************************************************" << endl;
	cout << "分度圆上端面切向载荷F_t=" << F_t << "N" << endl;
	cout << "齿宽b=" << b << "mm" << endl;
	cout << "小齿轮分度圆直径d1=" << d1 << "mm" << endl;
	cout << "节点区域系数Z_H=" << Z_H << endl;
	cout << "弹性系数Z_E=" << Z_E << endl;
	cout << "重合度系数Z_ε=" << Z_epsilon << endl;
	cout << "螺旋角系数Z_β=" << Z_beta << endl;
	cout << "小齿轮单对啮合系数Z_B=" << Z_B << endl;
	cout << "大齿轮单对啮合系数Z_D=" << Z_D << endl;
	cout << "小齿轮寿命系数Z_NT1=" << Z_NT1 << endl;
	cout << "大齿轮寿命系数Z_NT2=" << Z_NT2 << endl;
	cout << "润滑剂系数Z_L=" << Z_L << endl;
	cout << "粗糙度系数Z_R=" << Z_R << endl;
	cout << "速度系数Z_v=" << Z_v << endl;
	cout << "齿面工作硬化系数系数Z_W=" << Z_W << endl;
	cout << "尺寸系数Z_X=" << Z_X << endl;
	cout << "试验齿轮的接触疲劳极限σ_Hlim=" << sigma_Hlim << "N/mm^2" << endl;
	cout << "接触疲劳最小安全系数S_Hmin=" << S_Hmin << endl;
	cout << "*********************************************************" << endl;
	cout << "使用系数K_A=" << K_A << endl;
	cout << "均载系数K_γ=" << K_gama << endl;
	cout << "动载系数K_v=" << K_v << endl;
	cout << "接触强度计算时的螺旋线载荷分配系数K_Hβ=" << K_Hbeta << endl;
	cout << "弯曲强度计算时的螺旋线载荷分配系数K_Fβ=" << K_Fbeta << endl;
	cout << "齿间载荷分配系数K_Hα=" << K_Halpha << endl;
	cout << "齿间载荷分配系数K_Fα=" << K_Falpha << endl;
	cout << "*********************************************************" << endl;
	cout << "齿面接触应力的基本值σH0=" << sigma_H0 << "N/mm^2" << endl;
	cout << "小齿轮的接触应力σH1=" << sigma_H1 << "N/mm^2" << endl;
	cout << "大齿轮的接触应力σH2=" << sigma_H2 << "N/mm^2" << endl;
	cout << "小齿轮许用接触应力σHP1=" << sigma_HP1 << "N/mm^2" << endl;
	cout << "大齿轮许用接触应力σHP2=" << sigma_HP2 << "N/mm^2" << endl;
	cout << "小齿轮安全系数S_H1=" << S_H1 << endl;
	cout << "大齿轮安全系数S_H2=" << S_H2 << endl;
	cout << "*********************************************************" << endl;
	cout << "分度圆上端面切向载荷F_t=" << F_t << "N" << endl;
	cout << "小齿轮齿宽b1=" << b1 << "mm" << endl;
	cout << "法向模数m_n=" << m_n << "mm" << endl;
	cout << "小齿轮齿廓系数Y_F1=" << Y_F1 << endl;
	cout << "大齿轮齿廓系数Y_F2=" << Y_F2 << endl;
	cout << "小齿轮应力修正系数Y_S1=" << Y_S1 << endl;
	cout << "大齿轮应力修正系数Y_S2=" << Y_S2 << endl;
	cout << "螺旋角系数Y_β=" << Y_beta << endl;
	cout << "小齿轮轮缘厚度系数Y_B1=" << Y_B1 << endl;
	cout << "大齿轮轮缘厚度系数Y_B2=" << Y_B2 << endl;
	cout << "齿高系数Y_DT=" << Y_DT << endl;
	cout << "小齿轮与标准试验齿轮尺寸有关的应力修正系数Y_ST1=" << Y_ST1 << endl;
	cout << "大齿轮与标准试验齿轮尺寸有关的应力修正系数Y_ST2=" << Y_ST2 << endl;
	cout << "小齿轮齿根弯曲强度的寿命系数Y_NT1=" << Y_NT1 << endl;
	cout << "大齿轮齿根弯曲强度的寿命系数Y_NT2=" << Y_NT2 << endl;
	cout << "小齿轮缺口敏感系数Y_δrelT1=" << Y_deltarelT1 << endl;
	cout << "大齿轮缺口敏感系数Y_δrelT2=" << Y_deltarelT2 << endl;
	cout << "小齿轮相对表面状况系数Y_RrelT1=" << Y_RrelT1 << endl;
	cout << "大齿轮相对表面状况系数Y_RrelT2=" << Y_RrelT2 << endl;
	cout << "小齿轮尺寸系数Y_X1=" << Y_X1 << endl;
	cout << "大齿轮尺寸系数Y_X2=" << Y_X2 << endl;
	cout << "试验齿轮的弯曲疲劳极限σ_Flim=" << sigma_Flim << "N/mm^2" << endl;
	cout << "齿根应力所需的最小安全系数S_Fmin=" << S_Fmin << endl;
	cout << "*********************************************************" << endl;
	cout << "小齿轮齿根弯曲应力基本值σ_F01=" << sigma_F01 << "N/mm^2" << endl;
	cout << "大齿轮齿根弯曲应力基本值σ_F02=" << sigma_F02 << "N/mm^2" << endl;
	cout << "小齿轮齿根弯曲应力σ_F1=" << sigma_F1 << "N/mm^2" << endl;
	cout << "大齿轮齿根弯曲应力σ_F2=" << sigma_F2 << "N/mm^2" << endl;
	cout << "小齿轮许用齿根弯曲应力极限σ_FP1=" << sigma_FP1 << "N/mm^2" << endl;
	cout << "大齿轮许用齿根弯曲应力极限σ_FP2=" << sigma_FP2 << "N/mm^2" << endl;
	cout << "小齿轮弯曲强度安全系数S_F1=" << S_F1 << endl;
	cout << "大齿轮弯曲强度安全系数S_F2=" << S_F2 << endl;
}
