#include "TANInvoluteGear.h"

// 单齿轮材料、公差、工况、定位及基础力学参数。

// 模块职责：读取单齿轮物性、材料强度和制造参数。
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

// 模块职责：读取单齿轮使用条件、修形选项及安全系数要求。
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

// 模块职责：按对应重载计算或读取单齿轮精度与公差参数。
void CInvoluteGearTAN::CSingleInvoluteTAN::DefineProfileTolerence(int InTolerenceClass)
{
	double TempValue = pow(1.4142135623730951, InTolerenceClass - 5);
	f_pt = round((0.001*d+0.4*mn+5)*TempValue *2)/2.0;
	f_falpha = round(2*(0.55*mn+5)*TempValue)/2.0;
	f_Hbeta = round(2*(0.05*sqrt(d)+0.35*sqrt(b_Design)+4)*TempValue)/2.0;
}

// 模块职责：按对应重载计算或读取单齿轮精度与公差参数。
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

// 模块职责：按对应重载计算或读取单齿轮精度与公差参数。
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

// 模块职责：由齿距偏差计算基节偏差。
void CInvoluteGearTAN::CSingleInvoluteTAN::CalDerivedTolerence()
{
	f_pb= f_pt * cos(alpha_t);
}

// 模块职责：按齿轮尺寸、密度及内孔比例计算等效质量。
void CInvoluteGearTAN::CSingleInvoluteTAN::CalEquivalentMass()
{
	DidmRatio = D_i / dm;
	J_r = abs(Pi / 32. * Density*b_Design*(1 - pow(DidmRatio, 4))* pow(dm, 4));
	EquivalentMass = J_r / (b_Design*pow(rb, 2));
}

// 模块职责：按转矩、转向及齿轮几何计算各方向分力。
void CInvoluteGearTAN::CSingleInvoluteTAN::CalForce()
{
	F_t = 2000 * T / d/ N_m;
	F_r = F_t * alpha_n / cos(beta);
	F_a = TorqueDirect*SpiralIndex*F_t * tan(beta);
	F_n = F_t / (cos(alpha_n)*cos(beta));
}

// 模块职责：按对应重载读取或设置单齿轮安装位置。
void CInvoluteGearTAN::CSingleInvoluteTAN::DefineLocationPara(double InDistBetweenAssembleEdgeToShaftLeft, int InAssembleFaceType)
{
	AssembleDist = InDistBetweenAssembleEdgeToShaftLeft;
	AssembleFaceType = InAssembleFaceType;
}

// 模块职责：按对应重载读取或设置单齿轮安装位置。
void CInvoluteGearTAN::CSingleInvoluteTAN::DefineLocationPara(double* InData)
{
	AssembleDist = InData[0];
	AssembleFaceType = static_cast<int>(InData[1]);
}
