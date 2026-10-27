#include "TANPinionCycloidGear.h"

// 本文件集中实现摆线轮跨棒距、量棒匹配和齿顶距测量计算。
// 计算跨棒距，并把结果同步到两片摆线轮的测量参数中。
double CPinionCycloidGearTAN::CalcMValue(int Z, int d_rp, double r_p, double e, double d_m, double tolerance)
{
	double Rt_prime = d_m / 2.0; // 真实量棒直径除以2
	double delta_r_p = 0.0, delta_r_rp = 0.0;  // 明确初始化
	int z_b = Z + 1; 
	//double r_p = d_p / 2.0; // 针齿中心圆半径，对应字母Rz/Rzf
	double k1 = CalK1(z_b, r_p, e);
	double r_fg = r_p - e - d_rp / 2.0; //摆线轮齿根圆半径
	//double RL = CalcRL(r_p, r_z / 2.0, delta_r_p, delta_r_rp, e, z_b, Z, k1);// 量棒中心距摆线轮中心距离
	//double RL0 = r_fg + d_m / 2.0; // 量棒中心距摆线轮中心距离，这种计算方法只在针齿直径和量棒直径一样时有效
	std::pair<double, double> derivatives = calculateRLWithOptimization(
		r_p, d_rp / 2.0, delta_r_p, delta_r_rp, e, z_b, Z,
		Rt_prime, tolerance);
	double RL = derivatives.first;
	double di = derivatives.second;
	int n = CalcCrossToothNumber(Z); // 跨测间隔齿数
	double theta = 2 * PI * n / Z; // 两量棒中心夹角（弧度）
	double ptheta = PI - theta; //平角（弧度）
	double length1 = cos(ptheta)* RL;
	mPinionList[0].M = RL + length1 + di;
	mPinionList[1].M = mPinionList[0].M;
	//double centerDistance = 2 * RL * sin(theta / 2.0);
	//double M = centerDistance + d_m;
	return mPinionList[0].M; //能精确到小数点后一位。
}
// 迭代调整齿廓特征角，使计算量棒直径逼近目标值，并返回测量圆半径与实际量棒直径。
std::pair<double, double> CPinionCycloidGearTAN::calculateRLWithOptimization(double r_p, double r_rp, double delta_r_p, double delta_r_rp,double a, double z_p, double z_c, double Rt_prime,double tolerance)
{
	// 计算短幅系数K1
	double K1 = a * z_p / (r_p + delta_r_p);

	// 初始拐点 φ_A
	double phiA = calculatePhiA(K1, z_p);

	// 设置初始差分步长
	double delta_phi = 0.05;
	double delta_phi2 = delta_phi / 2.0;
	double KA = 0.0;
	double KA_prime = 0.0;
	// 计算拐点坐标
	Point A = calculateToothProfile(phiA, r_p, r_rp, delta_r_p, delta_r_rp, a, z_p, z_c);

	// 迭代优化法线斜率KA
	for (int i = 0; i < 1000; ++i) 
	{
		//差分计算导数
		Point A_delta_phi = calculateToothProfile(phiA + delta_phi, r_p, r_rp, delta_r_p, delta_r_rp, a, z_p, z_c);
		double delta_x = A_delta_phi.x - A.x;
		double delta_y = A_delta_phi.y - A.y;
		KA = -delta_x / delta_y;

		// 差分计算导数
		Point A_delta_phi2 = calculateToothProfile(phiA + delta_phi2, r_p, r_rp, delta_r_p, delta_r_rp, a, z_p, z_c);
		double delta_x2 = A_delta_phi2.x - A.x;
		double delta_y2 = A_delta_phi2.y - A.y;
		KA_prime = -delta_x2 / delta_y2;

		// 检查收敛性
		if (fabs(KA - KA_prime) < 0.0001)
		{
			KA = KA_prime;
			break;
		}
		else delta_phi2 /= 2.0;
	}

	// 计算测量圆半径RL
	double RL = -A.y / KA + A.x;
	double xB = RL;
	// 计算理论量棒半径Rt
	double Rt = sqrt((A.x - RL) * (A.x - RL) + A.y * A.y);

	// 迭代调整phiA，使量棒直径逼近目标值
	double step = 0.001; // 设置步长
	double RL_adj = 0;
	int j = 0;
	for (j = 0; j < 10000; ++j) {

		if (fabs(Rt - Rt_prime) > tolerance) 
		{
			if (Rt< Rt_prime) {
				phiA += step; // 增加phiA使量棒直径增加
			}
			else phiA -= step; // 减小phiA使量棒直径减小

			// 计算调整后的拐点坐标
			Point A_adj = calculateToothProfile(phiA, r_p, r_rp, delta_r_p, delta_r_rp, a, z_p, z_c);
			// 迭代优化法线斜率KA
			for (int i = 0; i < 1000; ++i)
			{
				//差分计算导数
				Point A_delta_phi = calculateToothProfile(phiA + delta_phi, r_p, r_rp, delta_r_p, delta_r_rp, a, z_p, z_c);
				double delta_x = A_delta_phi.x - A_adj.x;
				double delta_y = A_delta_phi.y - A_adj.y;
				KA = -delta_x / delta_y;

				// 差分计算导数
				Point A_delta_phi2 = calculateToothProfile(phiA + delta_phi2, r_p, r_rp, delta_r_p, delta_r_rp, a, z_p, z_c);
				double delta_x2 = A_delta_phi2.x - A_adj.x;
				double delta_y2 = A_delta_phi2.y - A_adj.y;
				KA_prime = -delta_x2 / delta_y2;

				// 检查收敛性
				if (fabs(KA - KA_prime) < 0.0001)
				{
					KA = KA_prime;
					break;
				}
				else delta_phi2 /= 2.0;
			}
			RL_adj = -A_adj.y / KA + A_adj.x;
			Rt = sqrt((A_adj.x - RL_adj) * (A_adj.x - RL_adj) + A_adj.y * A_adj.y);
			xB = RL_adj;
		}
		else
		{
			//Rt = Rt_prime;
			RL = xB;
			break;
		}
	}
	return std::make_pair(RL, 2 * Rt);
}
// 根据短幅系数和针齿数计算量棒接触搜索所需的初始特征角。
double CPinionCycloidGearTAN::calculatePhiA(double K1, double z_p) {
	double cos_phi = (1 + z_p * K1 * K1) / (K1 * (z_p + 1));
	return acos(cos_phi);
}
// 根据摆线齿廓方程计算指定转角处的二维坐标。
CPinionCycloidGearTAN::Point CPinionCycloidGearTAN::calculateToothProfile(double phi, double r_p, double r_rp, double delta_r_p, double delta_r_rp, double a, double z_p, double z_c) 
{
	double iH = z_p / z_c;
	double K1 = a * z_p / (r_p + delta_r_p);
	double F = 1 /sqrt(1 + K1 * K1 - 2 * K1 * cos(phi));

	double term1 = r_p - r_rp * F;
	double term2 = a - K1 * r_rp * F;                    

	double x = term1 * cos((1 - iH) * phi) - term2 * cos(iH * phi);
	double y = term1 * sin((1 - iH) * phi) + term2 * sin(iH * phi);

	return { x, y };
}
// 根据齿数确定两根量棒之间的跨测间隔齿数。
int CPinionCycloidGearTAN::CalcCrossToothNumber(int z)
{
	int n;
	if (z % 2 == 1) // 奇数齿
	{
		//n = (z + 1) / 2; // 间隔齿数，保证对称
		n = (z - 1) / 2 - 1; //实际需求
	}
	else // 偶数齿
	{
		n = z / 2;//暂时没有偶数齿的！暂定
		std::cout << "若需计算偶数齿数，请联系作者！" << std::endl;
	}
	return n;
}
// 根据齿顶圆几何关系计算齿顶距，并同步到两片摆线轮。
double CPinionCycloidGearTAN::CalToothTipDistanceValue(int Z, double d_rp, double Rz, double e)
{
	//通过齿顶圆半径来求
	double theta = 2 * PI / Z; // 两量棒中心夹角（弧度）
	double φ = theta / 2.0;
	double rz = d_rp / 2.0; //针齿半径
	//double Rz = d_p / 2.0; // 针齿中心圆半径，对应字母Rz/Rzf
	double r_ag = Rz + e - rz;
	double length1 = cos(φ) * r_ag;
	mPinionList[0].DT = length1 + r_ag;
	mPinionList[1].DT = mPinionList[0].DT;
	////通过曲率来求
	//double maxSearchDist = 2 * PI;
	//findNearestVerticalTangent(phiA, maxSearchDist, step, 1e-10, 1e-8, true, r_p, r_rp, delta_r_p, delta_r_rp, a, z_p, z_c);
	return mPinionList[0].DT;
}
