#include "TANInvoluteGear.h"

// 单齿轮几何与变位求解；普通版与 ZJ 版算法分别保留。

// 模块职责：按原有普通版输入布局定义单齿轮几何参数。
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

// 模块职责：按普通版公式计算单齿轮基本几何参数。
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

// 模块职责：计算指定角度的渐开线函数值。
double CInvoluteGearTAN::CSingleInvoluteTAN::Finv(double InAlpha)
{
	return tan(InAlpha)-InAlpha;
}

// 模块职责：按当前齿轮几何计算最小变位系数。
double CInvoluteGearTAN::CSingleInvoluteTAN::CalMinProfileShift()
{
	//z_min = 2 * h_ax / pow(sin(alpha_t), 2);
	////double zv=Z/ pow(cos(beta), 3);
	//x_min = h_ax * (z_min - Z) / z_min;
	x_Emin = h_FaP0 / mn - (Z * pow(sin(alpha_t), 2)/cos(beta)) / 2;
	//x_min =  (2/tan(alpha_t) - zv*sin(alpha_t)) / 2;
	return x_Emin;
}

// 模块职责：调用原有求根算法计算最大变位系数。
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

// 模块职责：按啮合角计算齿廓曲率相关参数。
double CInvoluteGearTAN::CSingleInvoluteTAN::Get_Rho(double InAlpha_wt)
{
	Rho = 0.5*db*tan(InAlpha_wt);
	return Rho;
}

// 模块职责：计算齿顶厚度偏差，供最大变位系数求根使用。
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

// 模块职责：计算渐开线函数值偏差，供反求角度使用。
double CInvoluteGearTAN::CSingleInvoluteTAN::AuxiliaryFunctionForalphaByInvFunctionValue(double* InAlpha, void** InInvAlphaValue)
{
	double tmpInvValue = *static_cast<double*> (InInvAlphaValue[0]);
	return Finv(*InAlpha)- tmpInvValue;
}

// 模块职责：调用原有迭代算法，由渐开线函数值反求角度。
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

// 模块职责：按原有 ZJ 版输入布局定义单齿轮几何参数。
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

// 模块职责：按原有 ZJ 版公式计算单齿轮基本几何参数。
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

// 模块职责：按给定加工变位与齿顶尺寸计算单齿轮派生几何数据。
void CInvoluteGearTAN::CSingleInvoluteTAN::CalEISingleData(double InX_E, double Ada, EISingleData& OutSingleData)
{
	OutSingleData.d_a = d + 2.0 * (x * mn + h_aP + k·mn) * IOMeshIndex + Ada;
	OutSingleData.d_Fa = OutSingleData.d_a - 2 * IOMeshIndex * h_k;
	OutSingleData.d_fe = CalGearParad_FE(InX_E);
	OutSingleData.h_a = abs(OutSingleData.d_a - d) / 2;//h_aP + InX_E * mn + k_factor * mn;后面个公式错误的原因在InX_E，这里就只是x，有多个值是因为d_a多值！
	OutSingleData.d_Ff = CalGearParad_Ff(InX_E);
	OutSingleData.h_f = h_fP - InX_E * mn;
}

// 模块职责：按原有刀具及加工条件计算齿根相关直径。
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

// 模块职责：按原有刀具及加工条件计算齿根渐开线起始直径。
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
