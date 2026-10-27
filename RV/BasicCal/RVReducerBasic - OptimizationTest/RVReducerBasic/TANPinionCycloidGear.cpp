#include "TANPinionCycloidGear.h"
#include "CycloidToleranceRepository.h"
//CRVReducerTAN::FCalTransRatio
#define _CycloidGearPara \
int&zb = mGear.Z;\
int&zg = mPinionList[0].Z;\
double& ee = mOtherCoef.ee;\
double& rz = mGear.rz;\
double& Rz = mGear.Rzf;\
double& K1 = mOtherCoef.K1;\
double& K2 = mOtherCoef.K2;\
double& T_b = mGear.Torque;\

tagRVGearBasicTAN::tagRVGearBasicTAN()
{
	ShaftForceVector.Set(6);
}

void tagRVGearBasicTAN::DefineLocationPara(double InDistBetweenAssembleEdgeToShaftLeft, int InAssembleFaceType)
{
	AssembleDist = InDistBetweenAssembleEdgeToShaftLeft;
	AssembleFaceType = InAssembleFaceType;
}

void tagRVGearBasicTAN::DefineLocationPara(double* InData)
{
	AssembleDist = InData[0];
	AssembleFaceType = static_cast<int>(InData[1]);

}

double tagRVGearBasicTAN::CalGearRevoluteSpeed(double InPinionSpeed, double InRotatingArmSpeed, double GearToPinionToothRatio)
{
	return (InPinionSpeed + (-1 + GearToPinionToothRatio)*InRotatingArmSpeed)/ GearToPinionToothRatio;
}

double tagRVGearBasicTAN::CalPinionRevoluteSpeed(double InGearSpeed, double InRotatingArmSpeed, double GearToPinionToothRatio)
{
	return (GearToPinionToothRatio * (InGearSpeed - InRotatingArmSpeed) + InRotatingArmSpeed);
}


void CPinionCycloidGearTAN::CopyCycloidWidthConstraintPara(double* InData)
{	
	targetMinToothWidth = InData[0];
	targetMaxToothWidth = InData[1];
	EnableWidthConstraintCheckBox = InData[2];//默认没有启用候选框
}

CPinionCycloidGearTAN::CPinionCycloidGearTAN()
{
	Clear();

}

void CPinionCycloidGearTAN::Clear()
{
	std::vector<tagPinion>(2).swap(mPinionList);
	Ng = 2;
}

void CPinionCycloidGearTAN::CalBasicGeometry()
{
	_CycloidGearPara
	mGear.rm = ee * zb;//r'b
	mOtherCoef.K1 = mGear.rm / Rz;//K1
	//摆线轮参数计算
	mPinionList[0].rm = ee * zg;//r'g
	mPinionList[0].r = mOtherCoef.ee / mOtherCoef.K1;
	//mPinionList[0].R = Rz * zg / double(mPinionList[0].Z);
	mPinionList[0].R = Rz * zg / zb; //应该是zb
	mPinionList[0].ra = Rz + ee - rz;
	mPinionList[0].rf = Rz - ee - rz;
	mPinionList[0].h = 2 * ee;
	mOtherCoef.K2 = CalK2(zb, rz, Rz);
	mPinionList[0].rmg = Rz;
	
	for (int i = 1;i < Ng;i++)
	{
		mPinionList[i].rm = mPinionList[0].rm;
		mPinionList[i].r = mPinionList[0].r;
		mPinionList[i].R = mPinionList[0].R;
		mPinionList[i].ra = mPinionList[0].ra;
		mPinionList[i].rf = mPinionList[0].rf;
		mPinionList[i].h = mPinionList[0].h;
		mPinionList[i].rmg = mPinionList[0].rmg;

	}
	//计算齿宽
	mOtherCoef.BB = 0;
	for (int i = 0;i < Ng;i++)
	{
		mPinionList[i].BB = CalGearDesignToothWidth(mPinionList[i].AssembleDist, mPinionList[i].AssembleFaceType, mPinionList[i].ToothWidth,
			mGear.AssembleDist, mGear.AssembleFaceType, mGear.ToothWidth);
		mOtherCoef.BB = mOtherCoef.BB + mPinionList[i].BB;
	}
}

void CPinionCycloidGearTAN::CalBasicGeometry(int& zb,//中心轮齿数
	int& zg,//摆线行星轮齿数
	double& rz,//滚针半径
	double& Rz,//摆线轮理论齿廓的平均半径，分度圆半径
	double& ee, //偏心距
	CPinionCycloidGearTAN & OutReducer)
{
	OutReducer.mGear.Z = zb;
	for (int i = 0; i < Ng; i++)
	{
		OutReducer.mPinionList[i].Z = zg;
	}
	OutReducer.mGear.rz = rz;
	OutReducer.mGear.Rzf = Rz;
	OutReducer.mOtherCoef.ee = ee;
	//OutReducer.mGear.Z = zb;
	//OutReducer.mPinion.Z = zg;
	//OutReducer.mOtherCoef.ee = ee;
	//OutReducer.mGear.rz = rz;
	////OutReducer.mPinion.rmg = Rz;
	//OutReducer.mGear.Rzf = Rz;
	OutReducer.CalBasicGeometry();
	//OutReducer.mGear.rm = ee * zb;//r'b
	//OutReducer.mOtherCoef.K1 = OutReducer.mGear.rm/Rz;//K1
	//OutReducer.mPinion.rm = ee * zg;//r'g
	//OutReducer.mPinion.r = OutReducer.mOtherCoef.ee / OutReducer.mOtherCoef.K1;
	//OutReducer.mPinion.R = Rz *zg/double(OutReducer.mPinion.Z);	
	//OutReducer.mPinion.ra = Rz + ee - rz;
	//OutReducer.mPinion.rf = Rz - ee - rz;
	//OutReducer.mPinion.h = 2 * ee;
	//OutReducer.mOtherCoef.K2 = Rz / rz * sin(pi / zb);
}
double CPinionCycloidGearTAN::CalK1(int & zb, double & Rzf, double & ee)
{
	return ee * zb / Rzf;
}
double CPinionCycloidGearTAN::CalK2(int & zb, double & rz, double & Rzf)
{
	return Rzf / rz * sin(Pi / zb);
}
double CPinionCycloidGearTAN::ChooseRoundEccentricCoef(int & zb, double & Rzf, double & IOK1)
{
	double ee = ceil(2 * IOK1 * Rzf / zb) / 2;
	IOK1 = CalK1(zb, Rzf, ee);
	return ee;
	
}

double CPinionCycloidGearTAN::ChooseRoundPinionToothCoverRadiusCoef(int & zb, double & Rzf, double & IOK2)
{
	double rz = ceil(2 * Rzf / IOK2 * sin(Pi / double(zb))) / 2;
	IOK2 = CalK2(zb, rz, Rzf);
	return rz;
}

void CPinionCycloidGearTAN::CalBasicGeometryByRzK1K2WithRound()
{
	CalBasicGeometryByRzK1K2WithRound(mGear.Z, mPinionList[0].Z, mGear.Rzf, mOtherCoef.K1, mOtherCoef.K2, *this);
	////double& K1 = mOtherCoef.K1;
	////double& K2 = mOtherCoef.K2;
	////int& zb = mGear.Z;
	////int& zg = mPinion.Z;	
	//////double& ee = mOtherCoef.ee;
	//////double& rz = mGear.rz;
	////double& Rz = mGear.Rzf;
	//_CycloidGearPara
	//mOtherCoef.ee = ChooseRoundEccentricCoef(mGear.Z, mGear.Rzf, mOtherCoef.K1);
	//mGear.rz = ChooseRoundPinionToothCoverRadiusCoef(mGear.Z, mGear.Rzf, mOtherCoef.K2);
	////double ee = ceil(2 * K1 * Rz / zb) / 2;
	//////double TempK1 = ee * zb / Rz;
	////double rz = ceil(2 * Rz / K2 * sin(pi / double(zb))) / 2;
	//CalBasicGeometry();
	////CalBasicGeometry(zb, zg, rz, Rz, ee, OutReducer);
}
void CPinionCycloidGearTAN::CalBasicGeometryByRzK1K2WithRound(int& zb,//中心轮齿数
	int& zg,//摆线行星轮齿数
	double& Rz,//针轮中心圆半径，分度圆半径
	double& K1,//短幅系数
	double& K2,//针径系数
	CPinionCycloidGearTAN& OutReducer)
{
	OutReducer.mOtherCoef.K1 = K1;
	OutReducer.mOtherCoef.K2 = K2;
	OutReducer.mGear.Rzf = Rz;
	OutReducer.mGear.Z = zb;
	for (int i = 0;i < Ng;i++)
	{
		OutReducer.mPinionList[i].Z = zg;
	}
	
	OutReducer.mOtherCoef.ee= ChooseRoundEccentricCoef(mGear.Z, mGear.Rzf, mOtherCoef.K1);
	OutReducer.mGear.rz = ChooseRoundPinionToothCoverRadiusCoef(mGear.Z, mGear.Rzf, mOtherCoef.K2);
	////double& ee = mOtherCoef.ee;
	//double& rz = mGear.rz;
	//double& Rz = mGear.Rzf;
	//double ee = ceil(2 * K1 * Rz / zb)/2;
	////double TempK1 = ee * zb / Rz;
	//double rz = ceil(2 * Rz / K2 * sin(pi / double(zb))) / 2;
	OutReducer.CalBasicGeometry();
	

}



double CPinionCycloidGearTAN::CalCoefK_y()
{
	_CycloidGearPara
	double AngDev = 2 * Pi / zb;
	double& R = Rz;
	double theta = -AngDev / 2;
	double PositionVector_X;
	double PositionVector_Y;
	double VectorNormalValue;
	double fy = 0;
	double K_y=0;
	for (int i=0;i< zb/2.0;i++)
	{
		theta = theta+ AngDev;
		PositionVector_X = R * sin(theta);
		PositionVector_Y = mGear.rm - R * cos(theta);
		VectorNormalValue = sqrt(pow(PositionVector_X, 2) + pow(PositionVector_Y, 2));
		fy = acos(PositionVector_Y / VectorNormalValue);
		K_y = K_y + 2 * sin( 2*fy) / static_cast<double>(zb);
	}
	return K_y;
}
//计算跨棒距
// 查询四项摆线轮公差，并在完整成功后更新领域对象。
void CPinionCycloidGearTAN::ObtainFourCycloidPara(int level, double r, double mn, double b)
{
	data_access::CycloidToleranceRecord record;
	if (!data_access::CycloidToleranceRepository::TryLoad(level, 2.0 * r, mn, b, record))
	{
		return;
	}

	mFourCycloidPara.F_beta = record.F_beta;
	mFourCycloidPara.F_p = record.F_p;
	mFourCycloidPara.F_r = record.F_r;
	mFourCycloidPara.f_pt = record.f_pt;
}
//static double normalizePhi(double phi) {
//	const double TWO_PI = 2.0 * PI;
//	double r = fmod(phi, TWO_PI);
//	if (r < 0) r += TWO_PI;
//	return r;
//}
//void CPinionCycloidGearTAN::computeDerivativesAnalytic(
//	double phi,
//	double r_p, double r_rp, double delta_r_p, double delta_r_rp,
//	double a, double z_p, double z_c,
//	double& dx_dphi, double& dy_dphi)
//{
//	double iH = z_p / z_c;
//	double K1 = a * z_p / (r_p + delta_r_p);
//	double cosphi = cos(phi);
//	double sinphi = sin(phi);
//	double D = 1.0 + K1 * K1 - 2.0 * K1 * cosphi;
//	double D32 = pow(max(D, 1e-18), 1.5);
//
//	double F = 1.0 / sqrt(D);
//
//	double term1 = r_p - r_rp * F;
//	double term2 = a - K1 * r_rp * F;
//
//	// dF/dphi = -K1*sinφ / D^{3/2}
//	double dF_dphi = -K1 * sinphi / D32;
//	double dterm1 = -r_rp * dF_dphi;
//	double dterm2 = -K1 * r_rp * dF_dphi;
//
//	double alpha = (1.0 - iH);
//	double beta = iH;
//
//	dx_dphi = dterm1 * cos(alpha * phi)
//		+ term1 * (-alpha * sin(alpha * phi))
//		- dterm2 * cos(beta * phi)
//		+ term2 * (beta * sin(beta * phi));
//
//	dy_dphi = dterm1 * std::sin(alpha * phi)
//		+ term1 * (alpha * std::cos(alpha * phi))
//		+ dterm2 * std::sin(beta * phi)
//		+ term2 * (beta * std::cos(beta * phi));
//}
//double CPinionCycloidGearTAN::bisection_root(const std::function<double(double)>& f, double a, double b, double tol, int maxIter)
//{
//	double fa = f(a), fb = f(b);
//	if (!(std::isfinite(fa) && std::isfinite(fb) && fa * fb <= 0.0)) return std::numeric_limits<double>::quiet_NaN();
//	for (int iter = 0; iter < maxIter; ++iter) {
//		double m = 0.5 * (a + b);
//		double fm = f(m);
//		if (!std::isfinite(fm)) return std::numeric_limits<double>::quiet_NaN();
//		if (std::fabs(fm) < tol || (b - a) * 0.5 < tol) return m;
//		if (fa * fm <= 0.0) {
//			b = m; fb = fm;
//		}
//		else {
//			a = m; fa = fm;
//		}
//	}
//	return 0.5 * (a + b);
//}
//CPinionCycloidGearTAN::VerticalTangentResult CPinionCycloidGearTAN::findNearestVerticalTangent(
//	double phiA,
//	double maxSearchDist,
//	double step,
//	double root_tol,
//	double dy_eps,
//	bool wrap2pi,
//	double r_p, double r_rp, double delta_r_p, double delta_r_rp,
//	double a, double z_p, double z_c)
//{
//	VerticalTangentResult result;
//	result.found = false;
//	result.phi = 0.0;
//	result.pt = { 0.0, 0.0 };
//	result.dx = result.dy = 0.0;
//
//	auto f = [&](double phi)->double {
//		double dx, dy;
//		computeDerivativesAnalytic(phi, r_p, r_rp, delta_r_p, delta_r_rp, a, z_p, z_c, dx, dy);
//		return dx;
//		};
//
//	auto computeDy = [&](double phi)->double {
//		double dx, dy;
//		computeDerivativesAnalytic(phi, r_p, r_rp, delta_r_p, delta_r_rp, a, z_p, z_c, dx, dy);
//		return dy;
//		};
//
//	// check phiA itself first
//	double dxA = f(phiA);
//	double dyA = computeDy(phiA);
//	if (std::isfinite(dxA) && std::fabs(dxA) < root_tol && std::fabs(dyA) > dy_eps) {
//		CPinionCycloidGearTAN::Point P = calculateToothProfile(phiA, r_p, r_rp, delta_r_p, delta_r_rp, a, z_p, z_c);
//		result.found = true;
//		result.phi = phiA;
//		result.pt = P;
//		result.dx = dxA;
//		result.dy = dyA;
//		return result;
//	}
//
//	// expand both sides
//	double left_phi_prev = phiA;
//	double right_phi_prev = phiA;
//	double left_f_prev = dxA;
//	double right_f_prev = dxA;
//
//	int maxSteps = (int)std::ceil(maxSearchDist / step);
//	for (int i = 1; i <= maxSteps; ++i) {
//		// right side
//		double right_phi = phiA + i * step;
//		if (wrap2pi) right_phi = normalizePhi(right_phi);
//		double fr = f(right_phi);
//		if (std::isfinite(fr) && std::isfinite(right_f_prev)) {
//			if (fr == 0.0) {
//				double dyr = computeDy(right_phi);
//				if (std::fabs(dyr) > dy_eps) {
//					CPinionCycloidGearTAN::Point P = calculateToothProfile(right_phi, r_p, r_rp, delta_r_p, delta_r_rp, a, z_p, z_c);
//					result.found = true; result.phi = right_phi; result.pt = P; result.dx = fr; result.dy = dyr;
//					return result;
//				}
//			}
//			else if (right_f_prev * fr < 0.0) {
//				double a_br = right_phi_prev;
//				double b_br = right_phi;
//				auto flambda = [&](double phi)->double { return f(phi); };
//				double root = bisection_root(flambda, a_br, b_br, root_tol, 100);
//				if (std::isfinite(root)) {
//					double dyr = computeDy(root);
//					if (std::fabs(dyr) > dy_eps) {
//						CPinionCycloidGearTAN::Point P = calculateToothProfile(root, r_p, r_rp, delta_r_p, delta_r_rp, a, z_p, z_c);
//						result.found = true; result.phi = root; result.pt = P; result.dx = f(root); result.dy = dyr;
//						return result;
//					}
//				}
//			}
//		}
//		right_phi_prev = right_phi;
//		right_f_prev = fr;
//
//		// left side
//		double left_phi = phiA - i * step;
//		if (wrap2pi) left_phi = normalizePhi(left_phi);
//		double fl = f(left_phi);
//		if (std::isfinite(fl) && std::isfinite(left_f_prev)) {
//			if (fl == 0.0) {
//				double dyl = computeDy(left_phi);
//				if (std::fabs(dyl) > dy_eps) {
//					CPinionCycloidGearTAN::Point P = calculateToothProfile(left_phi, r_p, r_rp, delta_r_p, delta_r_rp, a, z_p, z_c);
//					result.found = true; result.phi = left_phi; result.pt = P; result.dx = fl; result.dy = dyl;
//					return result;
//				}
//			}
//			else if (left_f_prev * fl < 0.0) {
//				double a_br = left_phi;
//				double b_br = left_phi_prev;
//				auto flambda = [&](double phi)->double { return f(phi); };
//				double root = bisection_root(flambda, a_br, b_br, root_tol, 100);
//				if (std::isfinite(root)) {
//					double dyl = computeDy(root);
//					if (std::fabs(dyl) > dy_eps) {
//						CPinionCycloidGearTAN::Point P = calculateToothProfile(root, r_p, r_rp, delta_r_p, delta_r_rp, a, z_p, z_c);
//						result.found = true; result.phi = root; result.pt = P; result.dx = f(root); result.dy = dyl;
//						return result;
//					}
//				}
//			}
//		}
//		left_phi_prev = left_phi;
//		left_f_prev = fl;
//	}
//
//	// not found
//	return result;
//}
