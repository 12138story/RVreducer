#include "InterfaceForCSharp.h"
#include "BearingCalculators.h"
void CalTotalTorqueAndRPM(double *Indata, double *OutData)
{
	ConvertInputDataToRVSetTotalObj(Indata, gRVReducer);
	gRVReducer.CalTotalTransRatio();
	gRVReducer.CalGearPairTorque();
	OutData[0] = gRVReducer.mFirstPairMotorInputTorque;
	OutData[1] = gRVReducer.mFirstPairPinionRPM;
	OutData[2] = gRVReducer.mSecondPairPinionOutputTorque;
	OutData[3] = gRVReducer.mSecondPairPinionBearingRPM;
}

void CalInvGearTolerence(double *Indata, double *OutData)
{
	double TempData = Indata[4];
	Indata[4] = Indata[4] * Degree;
	CInvoluteGearTAN::CSingleInvoluteTAN::DefineProfileTolerence(Indata, OutData);
	Indata[4] = TempData;
}

// void CalInvGearStress(double* InGeometrydata,double* InMaterialAndManufactureData, double* InAppData, double* InTolerenceData, double* InLoadData,double* InAssembleData,double* InLocData,double* OutData)
//{
//
//	int i = 0;
//	double Esp = 1E-8;
//	int tmpColumnCount = 14;
//
//	gRVReducer.mFirstGearPair.DefineGearPairGeometry(&InGeometrydata[0], &InGeometrydata[tmpColumnCount], &InGeometrydata[2* tmpColumnCount]);
//	//0-物性参数，1-加工参数
//
//
//
//	tmpColumnCount = 8;
//	gRVReducer.mFirstGearPair.DefineGearPairMaterial(&InMaterialAndManufactureData[0],
//		&InMaterialAndManufactureData[1*tmpColumnCount],
//		&InMaterialAndManufactureData[2*tmpColumnCount],
//		&InMaterialAndManufactureData[3*tmpColumnCount]);
//
//
//
//
//	tmpColumnCount = 8;
//	gRVReducer.mFirstGearPair.DefineGearPairApplicationData(InAppData,
//		&InAppData[tmpColumnCount],
//		&InAppData[2* tmpColumnCount]);
//
//
//
//
//
//	tmpColumnCount = 6;
//	gRVReducer.mFirstGearPair.DefineGearPairTolerance(InTolerenceData, &InTolerenceData[tmpColumnCount]);
//
//	gRVReducer.mFirstGearPair.DefineLoadPara(InLoadData[0], InLoadData[1], InLoadData[2],0,0);
//
//	gRVReducer.mFirstGearPair.DefineGearPairAssembleData(InAssembleData);
//
//	//安装位置参数
//	tmpColumnCount = 2;
//	gRVReducer.mFirstGearPair.mPinion.DefineLocationPara(&InLocData[0]);
//	gRVReducer.mFirstGearPair.mGear.DefineLocationPara(&InLocData[tmpColumnCount]);
//
//
//	gRVReducer.mFirstGearPair.InterfaceCalGeometryAndStrength();
//	tmpColumnCount = 6;
//	OutData[0] = gRVReducer.mFirstGearPair.sigma_H0;
//
//	for (int i = 0; i < 2; i++)
//	{
//		OutData[i * tmpColumnCount + 0] = gRVReducer.mFirstGearPair.sigma_H0;
//		OutData[i* tmpColumnCount + 1] = gRVReducer.mFirstGearPair.mGearPtrList[i]->sigma_H;
//		OutData[i* tmpColumnCount + 2] = gRVReducer.mFirstGearPair.mGearPtrList[i]->sigma_HP;
//		OutData[i* tmpColumnCount + 3] = gRVReducer.mFirstGearPair.mGearPtrList[i]->sigma_F0;
//		OutData[i* tmpColumnCount + 4] = gRVReducer.mFirstGearPair.mGearPtrList[i]->sigma_F;
//		OutData[i* tmpColumnCount + 5] = gRVReducer.mFirstGearPair.mGearPtrList[i]->sigma_FP;
//	}
//
// }
void CalInvGearStress2(int *InDivideIndex, double *InInputData, double *OutData)
{
	ConvertInputDataToInvObj(InDivideIndex, InInputData, gRVReducer.mFirstGearPair);
	gRVReducer.mFirstGearPair.InterfaceCalGeometryAndStrength();
	int tmpColumnCount = InDivideIndex[1 + 4 * 7];
	OutData[0] = gRVReducer.mFirstGearPair.sigma_H0;
	double transferRadToDegree = 180 / PI;
	for (int i = 0; i < 2; i++)
	{
		OutData[i * tmpColumnCount + 0] = gRVReducer.mFirstGearPair.sigma_H0;
		OutData[i * tmpColumnCount + 1] = gRVReducer.mFirstGearPair.mGearPtrList[i]->sigma_H;
		OutData[i * tmpColumnCount + 2] = gRVReducer.mFirstGearPair.mGearPtrList[i]->sigma_HP;
		OutData[i * tmpColumnCount + 3] = gRVReducer.mFirstGearPair.mGearPtrList[i]->sigma_F0;
		OutData[i * tmpColumnCount + 4] = gRVReducer.mFirstGearPair.mGearPtrList[i]->sigma_F;
		OutData[i * tmpColumnCount + 5] = gRVReducer.mFirstGearPair.mGearPtrList[i]->sigma_FP;

		OutData[i * tmpColumnCount + 6] = gRVReducer.mFirstGearPair.i; // 按照字典中的顺序
		OutData[i * tmpColumnCount + 7] = gRVReducer.mFirstGearPair.u;
		OutData[i * tmpColumnCount + 8] = gRVReducer.mFirstGearPair.mGearPtrList[i]->mt;
		OutData[i * tmpColumnCount + 9] = gRVReducer.mFirstGearPair.mGearPtrList[i]->m_x;
		OutData[i * tmpColumnCount + 10] = gRVReducer.mFirstGearPair.alpha_t * transferRadToDegree;
		OutData[i * tmpColumnCount + 11] = gRVReducer.mFirstGearPair.alpha_wt * transferRadToDegree;
		OutData[i * tmpColumnCount + 12] = gRVReducer.mFirstGearPair.beta_b * transferRadToDegree;
		OutData[i * tmpColumnCount + 13] = gRVReducer.mFirstGearPair.a_w;
		OutData[i * tmpColumnCount + 14] = gRVReducer.mFirstGearPair.sigmax;
		OutData[i * tmpColumnCount + 15] = gRVReducer.mFirstGearPair.mGearPtrList[i]->x_Es;
		OutData[i * tmpColumnCount + 16] = gRVReducer.mFirstGearPair.mGearPtrList[i]->x_Ei;
		OutData[i * tmpColumnCount + 17] = gRVReducer.mFirstGearPair.mGearPtrList[i]->zn;
		OutData[i * tmpColumnCount + 18] = gRVReducer.mFirstGearPair.mGearPtrList[i]->h_aP;
		OutData[i * tmpColumnCount + 19] = gRVReducer.mFirstGearPair.mGearPtrList[i]->h_fx;
		OutData[i * tmpColumnCount + 20] = gRVReducer.mFirstGearPair.mGearPtrList[i]->h_fP;
		OutData[i * tmpColumnCount + 21] = gRVReducer.mFirstGearPair.mGearPtrList[i]->d;
		OutData[i * tmpColumnCount + 22] = gRVReducer.mFirstGearPair.mGearPtrList[i]->db;
		OutData[i * tmpColumnCount + 23] = gRVReducer.mFirstGearPair.mGearPtrList[i]->mSingleData.d_a;
		OutData[i * tmpColumnCount + 24] = gRVReducer.mFirstGearPair.mGearPtrList[i]->mESingleData.d_a;
		OutData[i * tmpColumnCount + 25] = gRVReducer.mFirstGearPair.mGearPtrList[i]->mISingleData.d_a;
		OutData[i * tmpColumnCount + 26] = gRVReducer.mFirstGearPair.mGearPtrList[i]->mSingleData.d_Fa;
		OutData[i * tmpColumnCount + 27] = gRVReducer.mFirstGearPair.mGearPtrList[i]->mESingleData.d_Fa;
		OutData[i * tmpColumnCount + 28] = gRVReducer.mFirstGearPair.mGearPtrList[i]->mISingleData.d_Fa;
		OutData[i * tmpColumnCount + 29] = gRVReducer.mFirstGearPair.mGearPtrList[i]->d_Na;
		OutData[i * tmpColumnCount + 30] = gRVReducer.mFirstGearPair.mGearPtrList[i]->dw;
		OutData[i * tmpColumnCount + 31] = gRVReducer.mFirstGearPair.mGearPtrList[i]->d_Nf;
		OutData[i * tmpColumnCount + 32] = gRVReducer.mFirstGearPair.mGearPtrList[i]->mSingleData.d_Ff;
		OutData[i * tmpColumnCount + 33] = gRVReducer.mFirstGearPair.mGearPtrList[i]->mESingleData.d_Ff;
		OutData[i * tmpColumnCount + 34] = gRVReducer.mFirstGearPair.mGearPtrList[i]->mISingleData.d_Ff;
		OutData[i * tmpColumnCount + 35] = gRVReducer.mFirstGearPair.mGearPtrList[i]->mSingleData.d_fe;
		OutData[i * tmpColumnCount + 36] = gRVReducer.mFirstGearPair.mGearPtrList[i]->mESingleData.d_fe;
		OutData[i * tmpColumnCount + 37] = gRVReducer.mFirstGearPair.mGearPtrList[i]->mISingleData.d_fe;
		OutData[i * tmpColumnCount + 38] = gRVReducer.mFirstGearPair.mGearPtrList[i]->c;
		OutData[i * tmpColumnCount + 39] = gRVReducer.mFirstGearPair.mGearPtrList[i]->c_F;
		OutData[i * tmpColumnCount + 40] = gRVReducer.mFirstGearPair.mGearPtrList[i]->mSingleData.h_a;
		OutData[i * tmpColumnCount + 41] = gRVReducer.mFirstGearPair.mGearPtrList[i]->mESingleData.h_a;
		OutData[i * tmpColumnCount + 42] = gRVReducer.mFirstGearPair.mGearPtrList[i]->mISingleData.h_a;
		OutData[i * tmpColumnCount + 43] = gRVReducer.mFirstGearPair.mGearPtrList[i]->mSingleData.h_f;
		OutData[i * tmpColumnCount + 44] = gRVReducer.mFirstGearPair.mGearPtrList[i]->mESingleData.h_f;
		OutData[i * tmpColumnCount + 45] = gRVReducer.mFirstGearPair.mGearPtrList[i]->mISingleData.h_f;
		OutData[i * tmpColumnCount + 46] = gRVReducer.mFirstGearPair.mGearPtrList[i]->h;
		OutData[i * tmpColumnCount + 47] = gRVReducer.mFirstGearPair.mGearPtrList[i]->p_t;
		OutData[i * tmpColumnCount + 48] = gRVReducer.mFirstGearPair.mGearPtrList[i]->p_n;
		OutData[i * tmpColumnCount + 49] = gRVReducer.mFirstGearPair.mGearPtrList[i]->p_bt;
		OutData[i * tmpColumnCount + 50] = gRVReducer.mFirstGearPair.mGearPtrList[i]->p_bn;
		OutData[i * tmpColumnCount + 51] = gRVReducer.mFirstGearPair.mGearPtrList[i]->p_et;
		OutData[i * tmpColumnCount + 52] = gRVReducer.mFirstGearPair.mGearPtrList[i]->p_en;
		OutData[i * tmpColumnCount + 53] = gRVReducer.mFirstGearPair.mGearPtrList[i]->p_z;
		OutData[i * tmpColumnCount + 54] = gRVReducer.mFirstGearPair.mGearPtrList[i]->p_x;
		OutData[i * tmpColumnCount + 55] = gRVReducer.mFirstGearPair.omega1;
		OutData[i * tmpColumnCount + 56] = gRVReducer.mFirstGearPair.v_n;
		OutData[i * tmpColumnCount + 57] = gRVReducer.mFirstGearPair.mGearPtrList[i]->v_ga;
		OutData[i * tmpColumnCount + 58] = gRVReducer.mFirstGearPair.mGearPtrList[i]->v_gf;
		OutData[i * tmpColumnCount + 59] = gRVReducer.mFirstGearPair.mGearPtrList[i]->zeta_f;
		OutData[i * tmpColumnCount + 60] = gRVReducer.mFirstGearPair.mGearPtrList[i]->K_ga;
		OutData[i * tmpColumnCount + 61] = gRVReducer.mFirstGearPair.mGearPtrList[i]->K_gf;
		OutData[i * tmpColumnCount + 62] = gRVReducer.mFirstGearPair.epsilon_alpha;
		OutData[i * tmpColumnCount + 63] = gRVReducer.mFirstGearPair.epsilon_beta;
		OutData[i * tmpColumnCount + 64] = gRVReducer.mFirstGearPair.epsilon_gamma;
		OutData[i * tmpColumnCount + 65] = gRVReducer.mFirstGearPair.g_alpha;
		OutData[i * tmpColumnCount + 66] = gRVReducer.mFirstGearPair.mGearPtrList[i]->phi_alpha * transferRadToDegree;
		OutData[i * tmpColumnCount + 67] = gRVReducer.mFirstGearPair.mGearPtrList[i]->phi_beta * transferRadToDegree;
		OutData[i * tmpColumnCount + 68] = gRVReducer.mFirstGearPair.mGearPtrList[i]->phi_gama * transferRadToDegree;
		OutData[i * tmpColumnCount + 69] = gRVReducer.mFirstGearPair.mGearPtrList[i]->phi_j * transferRadToDegree;
		OutData[i * tmpColumnCount + 70] = gRVReducer.mFirstGearPair.T2A;
		OutData[i * tmpColumnCount + 71] = gRVReducer.mFirstGearPair.T1B;
		OutData[i * tmpColumnCount + 72] = gRVReducer.mFirstGearPair.T1C;
		OutData[i * tmpColumnCount + 73] = gRVReducer.mFirstGearPair.T2C;
		OutData[i * tmpColumnCount + 74] = gRVReducer.mFirstGearPair.T2D;
		OutData[i * tmpColumnCount + 75] = gRVReducer.mFirstGearPair.T1E;
		OutData[i * tmpColumnCount + 76] = gRVReducer.mFirstGearPair.T1T2;
		OutData[i * tmpColumnCount + 77] = gRVReducer.mFirstGearPair.rho_A1;
		OutData[i * tmpColumnCount + 78] = gRVReducer.mFirstGearPair.rho_A2;
		OutData[i * tmpColumnCount + 79] = gRVReducer.mFirstGearPair.rho_B1;
		OutData[i * tmpColumnCount + 80] = gRVReducer.mFirstGearPair.rho_C1;
		OutData[i * tmpColumnCount + 81] = gRVReducer.mFirstGearPair.rho_C2;
		OutData[i * tmpColumnCount + 82] = gRVReducer.mFirstGearPair.rho_D2;
		OutData[i * tmpColumnCount + 83] = gRVReducer.mFirstGearPair.rho_E1;
		OutData[i * tmpColumnCount + 84] = gRVReducer.mFirstGearPair.rho_E2;
		OutData[i * tmpColumnCount + 85] = gRVReducer.mFirstGearPair.mGearPtrList[i]->dm;
		OutData[i * tmpColumnCount + 86] = gRVReducer.mFirstGearPair.mGearPtrList[i]->rb;
		OutData[i * tmpColumnCount + 87] = gRVReducer.mFirstGearPair.mGearPtrList[i]->rm;
		OutData[i * tmpColumnCount + 88] = gRVReducer.mFirstGearPair.mGearPtrList[i]->ra;
		OutData[i * tmpColumnCount + 89] = gRVReducer.mFirstGearPair.mGearPtrList[i]->rf;
		OutData[i * tmpColumnCount + 90] = gRVReducer.mFirstGearPair.mGearPtrList[i]->invalpha_t;
		OutData[i * tmpColumnCount + 91] = gRVReducer.mFirstGearPair.mGearPtrList[i]->tau * transferRadToDegree;
		OutData[i * tmpColumnCount + 92] = gRVReducer.mFirstGearPair.mGearPtrList[i]->Wk;
		OutData[i * tmpColumnCount + 93] = gRVReducer.mFirstGearPair.mGearPtrList[i]->s_n;
		OutData[i * tmpColumnCount + 94] = gRVReducer.mFirstGearPair.mGearPtrList[i]->g_a;
		OutData[i * tmpColumnCount + 95] = gRVReducer.mFirstGearPair.mGearPtrList[i]->g_f;
		OutData[i * tmpColumnCount + 96] = gRVReducer.mFirstGearPair.g_beta;
		OutData[i * tmpColumnCount + 97] = gRVReducer.mFirstGearPair.l_max;
		OutData[i * tmpColumnCount + 98] = gRVReducer.mFirstGearPair.j_wt;
		OutData[i * tmpColumnCount + 99] = gRVReducer.mFirstGearPair.j_t;
		OutData[i * tmpColumnCount + 100] = gRVReducer.mFirstGearPair.j_r;
		OutData[i * tmpColumnCount + 101] = gRVReducer.mFirstGearPair.C_a;
		OutData[i * tmpColumnCount + 102] = gRVReducer.mFirstGearPair.mGearPtrList[i]->rho_fP;
		OutData[i * tmpColumnCount + 103] = gRVReducer.mFirstGearPair.mGearPtrList[i]->h_FaP0;
		OutData[i * tmpColumnCount + 104] = gRVReducer.mFirstGearPair.mGearPtrList[i]->x_Emin;
		OutData[i * tmpColumnCount + 105] = gRVReducer.mFirstGearPair.h_w;
		OutData[i * tmpColumnCount + 106] = gRVReducer.mFirstGearPair.sigmax_E;
		OutData[i * tmpColumnCount + 107] = gRVReducer.mFirstGearPair.mGearPtrList[i]->s_t;
		OutData[i * tmpColumnCount + 108] = gRVReducer.mFirstGearPair.mGearPtrList[i]->psi * transferRadToDegree;
		OutData[i * tmpColumnCount + 109] = gRVReducer.mFirstGearPair.mGearPtrList[i]->psi_b * transferRadToDegree;
		OutData[i * tmpColumnCount + 110] = gRVReducer.mFirstGearPair.mGearPtrList[i]->e_t;
		OutData[i * tmpColumnCount + 111] = gRVReducer.mFirstGearPair.mGearPtrList[i]->eta * transferRadToDegree;
		OutData[i * tmpColumnCount + 112] = gRVReducer.mFirstGearPair.mGearPtrList[i]->eta_b * transferRadToDegree;
		OutData[i * tmpColumnCount + 113] = gRVReducer.mFirstGearPair.mGearPtrList[i]->e_n;
		OutData[i * tmpColumnCount + 114] = gRVReducer.mFirstGearPair.mGearPtrList[i]->s_ns;
		OutData[i * tmpColumnCount + 115] = gRVReducer.mFirstGearPair.mGearPtrList[i]->s_ni;
		OutData[i * tmpColumnCount + 116] = gRVReducer.mFirstGearPair.mGearPtrList[i]->T_sn;
		OutData[i * tmpColumnCount + 117] = gRVReducer.mFirstGearPair.mGearPtrList[i]->T_snv;
		OutData[i * tmpColumnCount + 118] = gRVReducer.mFirstGearPair.mGearPtrList[i]->x_EsV;
		OutData[i * tmpColumnCount + 119] = gRVReducer.mFirstGearPair.mGearPtrList[i]->x_EiV;
		OutData[i * tmpColumnCount + 120] = gRVReducer.mFirstGearPair.mGearPtrList[i]->d_v;
		OutData[i * tmpColumnCount + 121] = gRVReducer.mFirstGearPair.g_alphay;
		OutData[i * tmpColumnCount + 122] = gRVReducer.mFirstGearPair.v_g;
		OutData[i * tmpColumnCount + 123] = gRVReducer.mFirstGearPair.K_g;
		OutData[i * tmpColumnCount + 124] = gRVReducer.mFirstGearPair.mGearPtrList[i]->zeta;
		OutData[i * tmpColumnCount + 125] = gRVReducer.mFirstGearPair.mGearPtrList[i]->beta_y * transferRadToDegree;
		OutData[i * tmpColumnCount + 126] = gRVReducer.mFirstGearPair.mGearPtrList[i]->gamma_y * transferRadToDegree;
		OutData[i * tmpColumnCount + 127] = gRVReducer.mFirstGearPair.mGearPtrList[i]->alpha_yt * transferRadToDegree;
		OutData[i * tmpColumnCount + 128] = gRVReducer.mFirstGearPair.mGearPtrList[i]->alpha_yn * transferRadToDegree;
		OutData[i * tmpColumnCount + 129] = gRVReducer.mFirstGearPair.mGearPtrList[i]->xi_y * transferRadToDegree;
		OutData[i * tmpColumnCount + 130] = gRVReducer.mFirstGearPair.mGearPtrList[i]->rho_y;
		OutData[i * tmpColumnCount + 131] = gRVReducer.mFirstGearPair.mGearPtrList[i]->invalpha_yt;
		OutData[i * tmpColumnCount + 132] = gRVReducer.mFirstGearPair.mGearPtrList[i]->p_yt;
		OutData[i * tmpColumnCount + 133] = gRVReducer.mFirstGearPair.mGearPtrList[i]->p_yn;
		OutData[i * tmpColumnCount + 134] = gRVReducer.mFirstGearPair.mGearPtrList[i]->s_yt;
		OutData[i * tmpColumnCount + 135] = gRVReducer.mFirstGearPair.mGearPtrList[i]->psi_y * transferRadToDegree;
		OutData[i * tmpColumnCount + 136] = gRVReducer.mFirstGearPair.mGearPtrList[i]->e_yt;
		OutData[i * tmpColumnCount + 137] = gRVReducer.mFirstGearPair.mGearPtrList[i]->eta_y * transferRadToDegree;
		OutData[i * tmpColumnCount + 138] = gRVReducer.mFirstGearPair.mGearPtrList[i]->s_yn;
		OutData[i * tmpColumnCount + 139] = gRVReducer.mFirstGearPair.mGearPtrList[i]->e_yn;
		OutData[i * tmpColumnCount + 140] = gRVReducer.mFirstGearPair.mGearPtrList[i]->d_Nf;
		OutData[i * tmpColumnCount + 141] = gRVReducer.mFirstGearPair.mGearPtrList[i]->d_Na;
	}
}
// void CalInvGearStress2(InvGearObjInC* InGearObj, double* OutData)
//{
//	CalInvGearStress(InGearObj->GeometryData,
//		InGearObj->MateriaData,
//		InGearObj->AppData,
//		InGearObj->TolerenceData,
//		InGearObj->LoadData,
//		InGearObj->AssembleData,
//		InGearObj->LocData,
//		OutData);
// }

// void CalCycloidGearStress(double* InputData, double* OutputData)
//{
//
//	//gRVReducer.mSecondGearPair.DefineBasicGeometryPara(zb, zg, rz, Rz, ee, PinionWidth, GearWidth);
//	//double& PinionTotalTorque= InputData[7];//摆线轮总力矩
//	//double& PinionModule= InputData[8];//摆线弹性模量
//	//double& GearModule = InputData[9];//针轮弹性模量
//	//int CycloidCount = static_cast<int>(InputData[10]);//摆线轮数目
//	//double& Kp = InputData[11];//使用系数
//	ConvertInputDataToCycloidObj(InputData, gRVReducer.mSecondGearPair);
//
//	OutputData[0] = gRVReducer.mSecondGearPair.FindMaxContStress();
//	//OutputData[0] = gRVReducer.mSecondGearPair.mMaxContStress;
//
//
//
// }

void CalCycloidGearStressByObj(int *InputIndexDividIndex, double *InputData, double *OutputData)
{
	ConvertInputDataToCycloidObj(InputIndexDividIndex, InputData, gRVReducer.mSecondGearPair);
	OutputData[0] = gRVReducer.mSecondGearPair.FindMaxContStress();
	OutputData[1] = gRVReducer.mSecondGearPair.mPinionList[0].h;
	OutputData[2] = gRVReducer.mSecondGearPair.mOtherCoef.K1;
	OutputData[3] = gRVReducer.mSecondGearPair.mPinionList[0].DT;
	OutputData[4] = gRVReducer.mSecondGearPair.mPinionList[0].M;
	OutputData[5] = gRVReducer.mSecondGearPair.mFourCycloidPara.f_pt;
	OutputData[6] = gRVReducer.mSecondGearPair.mFourCycloidPara.F_p;
	OutputData[7] = gRVReducer.mSecondGearPair.mFourCycloidPara.F_beta;
	OutputData[8] = gRVReducer.mSecondGearPair.mFourCycloidPara.F_r;
}

// void CalCrankShaftSystem(int* InputIntIndexDividIndex, int* InputDoubleIndexDividIndex, int* InputIntData, double* InputDoubleData, double* OutputData)
//{
//
//
//	//ConvertInputDataToShaftObj(InputIntIndexDividIndex, InputDoubleIndexDividIndex, InputIntData, InputDoubleData,gRVReducer.mCrankAxleSystem.mShaft);
//
//
//
//
//	//CalInvGearStress(double* InGeometrydata, double* InMaterialAndManufactureData, double* InAppData, double* InTolerenceData, double* InLoadData, double* InAssembleData, double* InLocData, double* OutData);
//	//CalCycloidGearStress(double* InputData, double* OutputData);
//
//	//需要输入安装相位
//	//gRVReducer.mFirstGearPair.CalForceVectorByRotAng(0);
// }

void CalCrankShaftSystem2(int *InputIntIndexDividIndex, int *InputIntData, double *InputDoubleData, double *OutputData)
{
	////总体
	// CalTotalTorqueAndRPM(InputDoubleData, OutputData);
	////渐开线
	// CalInvGearStress2(&InputIntData[InputIntIndexDividIndex[0 + 1 * 3]], &InputDoubleData[InputIntIndexDividIndex[1 + 1 * 3]], &OutputData[InputIntIndexDividIndex[2 + 1 * 3]]);
	////摆线轮转换
	// CalCycloidGearStressByObj(&InputIntData[InputIntIndexDividIndex[0 + 2 * 3]], &InputDoubleData[InputIntIndexDividIndex[1 + 2 * 3]], &OutputData[InputIntIndexDividIndex[2 + 2 * 3]]);
	////轴承从总体数据进行转换
	// int tmpBearingCount = InputIntData[InputIntIndexDividIndex[0 + 3 * 3]];
	// int tmpEachBearingInputDataCount = InputIntData[InputIntIndexDividIndex[0 + 3 * 3] + 1];
	// int tmpBearingInputDataStartIndex = InputIntIndexDividIndex[1 + 3 * 3];
	// std::vector<CBearingTAN>(tmpBearingCount).swap(gRVReducer.mBearingList);
	// for (int i = 0;i < tmpBearingCount;i++)
	//{
	//	ConvertInputDataToBearingObj(InputIntData[InputIntIndexDividIndex[0 + 3 * 3] + 5], &InputDoubleData[tmpBearingInputDataStartIndex + tmpEachBearingInputDataCount * i], gRVReducer.mBearingList[i]);
	// }
	//
	//
	//
	gRVReducer.mCrankAxleSystem.mShaft.ClearCalculatorData(); // 清理缓存 ++!
	gRVReducer.Clear();
	// 摆线轮转换
	ConvertInputDataToCycloidObj(&InputIntData[InputIntIndexDividIndex[0 + 2 * 3]], &InputDoubleData[InputIntIndexDividIndex[1 + 2 * 3]], gRVReducer.mSecondGearPair);
	////轴数据转换
	// ConvertInputDataToShaftObj(&InputIntData[InputIntIndexDividIndex[0 + 3 * 4]], &InputDoubleData[InputIntIndexDividIndex[1 + 3 * 4]], gRVReducer.mCrankAxleSystem.mShaft);
	// 总体数据转换
	ConvertInputDataToRVSetTotalObj(InputDoubleData, gRVReducer);
	gRVReducer.CalTotalTransRatio();
	gRVReducer.CalGearPairTorque();
	// 渐开线齿轮转换
	ConvertInputDataToInvObj(&InputIntData[InputIntIndexDividIndex[0 + 1 * 3]], &InputDoubleData[InputIntIndexDividIndex[1 + 1 * 3]], gRVReducer.mFirstGearPair);
	// 将渐开线齿轮载荷转换为减速器实际计算载荷
	gRVReducer.mFirstGearPair.DefineLoadPara(gRVReducer.mFirstPairMotorInputTorque, gRVReducer.mFirstPairPinionRPM, gRVReducer.mFirstGearPair.mPinion.Lh, 0, 0);
	gRVReducer.mFirstGearPair.InterfaceCalGeometryAndStrength();

	gRVReducer.mSecondGearPair.FindMaxContStress();
	// 轴系计算
	//  		//先要计算各齿轮副的力
	gRVReducer.mFirstGearPair.CalForceVectorByRotAng(0);
	gRVReducer.mSecondGearPair.CalBearingForce(3, 1.0, gRVReducer.mFirstGearPair.a_w); // 3为轴承孔数
	// 目前只定义了两个摆线轮
	gRVReducer.mSecondGearPair.CalShaftBearingForceByRotAng(0, 0, 0);
	gRVReducer.mSecondGearPair.CalShaftBearingForceByRotAng(1, 0, Pi);
	// gRVReducer.mSecondGearPair.CalBearingForce(3, 1.0, gRVReducer.mFirstGearPair.a_w);
	// gRVReducer.mSecondGearPair.CalShaftBearingForceByRotAng(0, 0);
	// gRVReducer.mThirdGearPair.CalBearingForce(3, 1.0, gRVReducer.mFirstGearPair.a_w);
	// gRVReducer.mThirdGearPair.CalShaftBearingForceByRotAng(0, Pi);
	// 对轴承进行转换
	// 轴承从总体数据进行转换
	int tmpBearingCount = InputIntData[InputIntIndexDividIndex[0 + 3 * 3]];
	int tmpEachBearingInputDataCount = InputIntData[InputIntIndexDividIndex[0 + 3 * 3] + 1];
	int tmpBearingInputDataStartIndex = InputIntIndexDividIndex[1 + 3 * 3];
	std::vector<CBearingTAN>(tmpBearingCount).swap(gRVReducer.mBearingList);
	for (int i = 0; i < tmpBearingCount; i++)
	{
		ConvertInputDataToBearingObj(InputIntData[InputIntIndexDividIndex[0 + 3 * 3] + 5], &InputDoubleData[tmpBearingInputDataStartIndex + tmpEachBearingInputDataCount * i], gRVReducer.mBearingList[i]);
	}
	// 构建部件的虚指针函数
	std::vector<void *> tmpAllComponentPtr;
	std::vector<int> tmpComponentTypeList;
	// 只插入需要的部分
	// 渐开线部分
	tmpAllComponentPtr.emplace_back(static_cast<void *>(&gRVReducer.mFirstGearPair.mGear)); // 1
	tmpComponentTypeList.emplace_back(InvGearNotation);
	// 摆线轮部分
	tmpAllComponentPtr.emplace_back(static_cast<void *>(&gRVReducer.mSecondGearPair.mPinionList[0])); // 2
	tmpComponentTypeList.emplace_back(CycloidPinionNotation);
	tmpAllComponentPtr.emplace_back(static_cast<void *>(&gRVReducer.mSecondGearPair.mPinionList[1])); // 4
	tmpComponentTypeList.emplace_back(CycloidPinionNotation);
	// 轴承部分
	for (size_t i = 0; i < gRVReducer.mBearingList.size(); i++)
	{
		if (gRVReducer.mBearingList[i].mBearingType == TaperedRollerBearing)
		{
			tmpAllComponentPtr.emplace_back(&gRVReducer.mBearingList[i]);
			tmpComponentTypeList.emplace_back(gRVReducer.mBearingList[i].mBearingType);
		}
	}
	// 轴上的构件组装
	for (int i = 0; i < tmpAllComponentPtr.size(); i++)
	{
		gRVReducer.mCrankAxleSystem.mComponentNumList.emplace_back(i);
	}
	gRVReducer.mCrankAxleSystem.MapComponentListByNum(gRVReducer.mCrankAxleSystem.mComponentNumList,
													  tmpComponentTypeList,
													  tmpAllComponentPtr);
	// 轴转换
	ConvertInputDataToShaftObj(&InputIntData[InputIntIndexDividIndex[0 + 4 * 3]], &InputDoubleData[InputIntIndexDividIndex[1 + 4 * 3]], gRVReducer.mCrankAxleSystem.mShaft);
	gRVReducer.mCrankAxleSystem.CalShaftSystem();
}

void Testfunc()
{
	// CSparseMatrixCSR::TestFunc();
}

void CalGeometry(double *Indata, double *OutData)
{
	double TempData_alphan = Indata[63];
	double TempData_beta = Indata[64];
	Indata[63] = Indata[63] * Degree;
	Indata[64] = Indata[64] * Degree;
	gRVReducer.mFirstGearPair.DefineGearPairGeometryZJ(&Indata[0], &Indata[31], &Indata[62]);
	gRVReducer.mFirstGearPair.CalGearPairGeometryZJ();
	Indata[63] = TempData_alphan;
	Indata[64] = TempData_beta;

	OutData[0] = gRVReducer.mFirstGearPair.alpha_wt; // 端面啮合角
	OutData[1] = gRVReducer.mFirstGearPair.i;
	OutData[2] = gRVReducer.mFirstGearPair.u;
	OutData[3] = gRVReducer.mFirstGearPair.alpha_t;
	OutData[4] = gRVReducer.mFirstGearPair.a_w;
	OutData[5] = gRVReducer.mFirstGearPair.h_w;
	OutData[6] = gRVReducer.mFirstGearPair.sigmax;
	OutData[7] = gRVReducer.mFirstGearPair.sigmax_E;
	OutData[8] = gRVReducer.mFirstGearPair.g_alpha;
	OutData[9] = gRVReducer.mFirstGearPair.epsilon_alpha;
	OutData[10] = gRVReducer.mFirstGearPair.epsilon_beta;
	OutData[11] = gRVReducer.mFirstGearPair.g_beta;
	OutData[12] = gRVReducer.mFirstGearPair.epsilon_gamma;
	OutData[13] = gRVReducer.mFirstGearPair.l_max;
	OutData[14] = gRVReducer.mFirstGearPair.j_wt;
	OutData[15] = gRVReducer.mFirstGearPair.j_t;
	OutData[16] = gRVReducer.mFirstGearPair.j_r;
	OutData[17] = gRVReducer.mFirstGearPair.omega1;
	OutData[18] = gRVReducer.mFirstGearPair.v_n;
	OutData[19] = gRVReducer.mFirstGearPair.g_alphay;
	OutData[20] = gRVReducer.mFirstGearPair.v_g;
	OutData[21] = gRVReducer.mFirstGearPair.K_g;
	OutData[22] = gRVReducer.mFirstGearPair.rho_A1;
	OutData[23] = gRVReducer.mFirstGearPair.rho_E2;
	OutData[24] = gRVReducer.mFirstGearPair.rho_C1;
	OutData[25] = gRVReducer.mFirstGearPair.T1C;
	OutData[26] = gRVReducer.mFirstGearPair.rho_C2;
	OutData[27] = gRVReducer.mFirstGearPair.T2C;
	OutData[28] = gRVReducer.mFirstGearPair.rho_A2;
	OutData[29] = gRVReducer.mFirstGearPair.T2A;
	OutData[30] = gRVReducer.mFirstGearPair.rho_E1;
	OutData[31] = gRVReducer.mFirstGearPair.T1E;
	OutData[32] = gRVReducer.mFirstGearPair.rho_B1;
	OutData[33] = gRVReducer.mFirstGearPair.T1B;
	OutData[34] = gRVReducer.mFirstGearPair.rho_D2;
	OutData[35] = gRVReducer.mFirstGearPair.T2D;
	OutData[36] = gRVReducer.mFirstGearPair.T1T2;
	OutData[37] = gRVReducer.mFirstGearPair.g_beta;
	OutData[38] = gRVReducer.mFirstGearPair.C_a;

	int tempIndex = 39;	 // 在这之前数据的个数
	int mGearIndex = 96; // 下面for循环中的数据个数
	for (int i = 0; i < 2; i++)
	{
		// if (i == 0)
		//{
		//	gRVReducer.mFirstGearPair.mGear = gRVReducer.mFirstGearPair.mPinion;//!!!!这里貌似全给了！覆盖了
		// }
		if (i == 1)
		{
			gRVReducer.mFirstGearPair.mPinion = gRVReducer.mFirstGearPair.mGear;
		}
		OutData[i * mGearIndex + tempIndex] = gRVReducer.mFirstGearPair.mPinion.d;
		OutData[i * mGearIndex + tempIndex + 1] = gRVReducer.mFirstGearPair.mPinion.mt;
		OutData[i * mGearIndex + tempIndex + 2] = gRVReducer.mFirstGearPair.mPinion.m_x;
		OutData[i * mGearIndex + tempIndex + 3] = gRVReducer.mFirstGearPair.mPinion.p_z;
		OutData[i * mGearIndex + tempIndex + 4] = gRVReducer.mFirstGearPair.mPinion.beta_b;
		OutData[i * mGearIndex + tempIndex + 5] = gRVReducer.mFirstGearPair.mPinion.db;
		OutData[i * mGearIndex + tempIndex + 6] = gRVReducer.mFirstGearPair.mPinion.d_Na;
		OutData[i * mGearIndex + tempIndex + 7] = gRVReducer.mFirstGearPair.mPinion.beta_y;
		OutData[i * mGearIndex + tempIndex + 8] = gRVReducer.mFirstGearPair.mPinion.gamma_y;
		OutData[i * mGearIndex + tempIndex + 9] = gRVReducer.mFirstGearPair.mPinion.alpha_yt;
		OutData[i * mGearIndex + tempIndex + 10] = gRVReducer.mFirstGearPair.mPinion.d_Nf;
		OutData[i * mGearIndex + tempIndex + 11] = gRVReducer.mFirstGearPair.mPinion.alpha_yn;
		OutData[i * mGearIndex + tempIndex + 12] = gRVReducer.mFirstGearPair.mPinion.xi_y;
		OutData[i * mGearIndex + tempIndex + 13] = gRVReducer.mFirstGearPair.mPinion.rho_y;
		OutData[i * mGearIndex + tempIndex + 14] = gRVReducer.mFirstGearPair.mPinion.invalpha_yt;
		OutData[i * mGearIndex + tempIndex + 15] = gRVReducer.mFirstGearPair.mPinion.invalpha_t;
		OutData[i * mGearIndex + tempIndex + 16] = gRVReducer.mFirstGearPair.mPinion.tau;
		OutData[i * mGearIndex + tempIndex + 17] = gRVReducer.mFirstGearPair.mPinion.p_t;
		OutData[i * mGearIndex + tempIndex + 18] = gRVReducer.mFirstGearPair.mPinion.p_n;
		OutData[i * mGearIndex + tempIndex + 19] = gRVReducer.mFirstGearPair.mPinion.p_yt;
		OutData[i * mGearIndex + tempIndex + 20] = gRVReducer.mFirstGearPair.mPinion.p_yn;
		OutData[i * mGearIndex + tempIndex + 21] = gRVReducer.mFirstGearPair.mPinion.p_x;
		OutData[i * mGearIndex + tempIndex + 22] = gRVReducer.mFirstGearPair.mPinion.p_bt;
		OutData[i * mGearIndex + tempIndex + 23] = gRVReducer.mFirstGearPair.mPinion.p_bn;
		OutData[i * mGearIndex + tempIndex + 24] = gRVReducer.mFirstGearPair.mPinion.p_et;
		OutData[i * mGearIndex + tempIndex + 25] = gRVReducer.mFirstGearPair.mPinion.p_en;
		OutData[i * mGearIndex + tempIndex + 26] = gRVReducer.mFirstGearPair.mPinion.d_v;
		OutData[i * mGearIndex + tempIndex + 27] = gRVReducer.mFirstGearPair.mPinion.h_aP;
		OutData[i * mGearIndex + tempIndex + 28] = gRVReducer.mFirstGearPair.mPinion.h_fP;
		OutData[i * mGearIndex + tempIndex + 29] = gRVReducer.mFirstGearPair.mPinion.h;
		OutData[i * mGearIndex + tempIndex + 30] = gRVReducer.mFirstGearPair.mPinion.dw;
		OutData[i * mGearIndex + tempIndex + 31] = gRVReducer.mFirstGearPair.mPinion.s_yt;
		OutData[i * mGearIndex + tempIndex + 32] = gRVReducer.mFirstGearPair.mPinion.s_t;
		OutData[i * mGearIndex + tempIndex + 33] = gRVReducer.mFirstGearPair.mPinion.psi_y;
		OutData[i * mGearIndex + tempIndex + 34] = gRVReducer.mFirstGearPair.mPinion.psi;
		OutData[i * mGearIndex + tempIndex + 35] = gRVReducer.mFirstGearPair.mPinion.psi_b;
		OutData[i * mGearIndex + tempIndex + 36] = gRVReducer.mFirstGearPair.mPinion.e_yt;
		OutData[i * mGearIndex + tempIndex + 37] = gRVReducer.mFirstGearPair.mPinion.e_t;
		OutData[i * mGearIndex + tempIndex + 38] = gRVReducer.mFirstGearPair.mPinion.eta_y;
		OutData[i * mGearIndex + tempIndex + 39] = gRVReducer.mFirstGearPair.mPinion.eta;
		OutData[i * mGearIndex + tempIndex + 40] = gRVReducer.mFirstGearPair.mPinion.eta_b;
		OutData[i * mGearIndex + tempIndex + 41] = gRVReducer.mFirstGearPair.mPinion.s_yn;
		OutData[i * mGearIndex + tempIndex + 42] = gRVReducer.mFirstGearPair.mPinion.s_n;
		OutData[i * mGearIndex + tempIndex + 43] = gRVReducer.mFirstGearPair.mPinion.e_yn;
		OutData[i * mGearIndex + tempIndex + 44] = gRVReducer.mFirstGearPair.mPinion.e_n;
		OutData[i * mGearIndex + tempIndex + 45] = gRVReducer.mFirstGearPair.mPinion.s_ns;
		OutData[i * mGearIndex + tempIndex + 46] = gRVReducer.mFirstGearPair.mPinion.s_ni;
		OutData[i * mGearIndex + tempIndex + 47] = gRVReducer.mFirstGearPair.mPinion.x_Es;
		OutData[i * mGearIndex + tempIndex + 48] = gRVReducer.mFirstGearPair.mPinion.x_Ei;
		OutData[i * mGearIndex + tempIndex + 49] = gRVReducer.mFirstGearPair.mPinion.Wk;
		OutData[i * mGearIndex + tempIndex + 50] = gRVReducer.mFirstGearPair.mPinion.x_EsV;
		OutData[i * mGearIndex + tempIndex + 51] = gRVReducer.mFirstGearPair.mPinion.x_EiV;
		OutData[i * mGearIndex + tempIndex + 52] = gRVReducer.mFirstGearPair.mPinion.x_Emin;
		int tempIndexk = 6; // 下面for循环中数据个数
		for (int k = 0; k < 3; k++)
		{
			if (k == 1)
			{
				gRVReducer.mFirstGearPair.mPinion.mSingleData = gRVReducer.mFirstGearPair.mPinion.mESingleData;
			}
			else if (k == 2)
			{
				gRVReducer.mFirstGearPair.mPinion.mSingleData = gRVReducer.mFirstGearPair.mPinion.mISingleData;
			}
			OutData[i * mGearIndex + tempIndex + 53 + k * tempIndexk] = gRVReducer.mFirstGearPair.mPinion.mSingleData.d_a;
			OutData[i * mGearIndex + tempIndex + 54 + k * tempIndexk] = gRVReducer.mFirstGearPair.mPinion.mSingleData.d_Fa;
			OutData[i * mGearIndex + tempIndex + 55 + k * tempIndexk] = gRVReducer.mFirstGearPair.mPinion.mSingleData.d_fe;
			OutData[i * mGearIndex + tempIndex + 56 + k * tempIndexk] = gRVReducer.mFirstGearPair.mPinion.mSingleData.h_a;
			OutData[i * mGearIndex + tempIndex + 57 + k * tempIndexk] = gRVReducer.mFirstGearPair.mPinion.mSingleData.d_Ff;
			OutData[i * mGearIndex + tempIndex + 58 + k * tempIndexk] = gRVReducer.mFirstGearPair.mPinion.mSingleData.h_f;
		}
		OutData[i * mGearIndex + tempIndex + 71] = gRVReducer.mFirstGearPair.mPinion.g_f;
		OutData[i * mGearIndex + tempIndex + 72] = gRVReducer.mFirstGearPair.mPinion.g_a;
		OutData[i * mGearIndex + tempIndex + 73] = gRVReducer.mFirstGearPair.mPinion.zn;
		OutData[i * mGearIndex + tempIndex + 74] = gRVReducer.mFirstGearPair.mPinion.h_fx;
		OutData[i * mGearIndex + tempIndex + 75] = gRVReducer.mFirstGearPair.mPinion.dm;
		OutData[i * mGearIndex + tempIndex + 76] = gRVReducer.mFirstGearPair.mPinion.rb;
		OutData[i * mGearIndex + tempIndex + 77] = gRVReducer.mFirstGearPair.mPinion.rm;
		OutData[i * mGearIndex + tempIndex + 78] = gRVReducer.mFirstGearPair.mPinion.ra;
		OutData[i * mGearIndex + tempIndex + 79] = gRVReducer.mFirstGearPair.mPinion.rf;
		OutData[i * mGearIndex + tempIndex + 80] = gRVReducer.mFirstGearPair.mPinion.c_F;
		OutData[i * mGearIndex + tempIndex + 81] = gRVReducer.mFirstGearPair.mPinion.T_sn;
		OutData[i * mGearIndex + tempIndex + 82] = gRVReducer.mFirstGearPair.mPinion.T_snv;
		OutData[i * mGearIndex + tempIndex + 83] = gRVReducer.mFirstGearPair.mPinion.rho_fP;
		OutData[i * mGearIndex + tempIndex + 84] = gRVReducer.mFirstGearPair.mPinion.h_FaP0;
		OutData[i * mGearIndex + tempIndex + 85] = gRVReducer.mFirstGearPair.mPinion.c;
		OutData[i * mGearIndex + tempIndex + 86] = gRVReducer.mFirstGearPair.mPinion.phi_alpha;
		OutData[i * mGearIndex + tempIndex + 87] = gRVReducer.mFirstGearPair.mPinion.phi_beta;
		OutData[i * mGearIndex + tempIndex + 88] = gRVReducer.mFirstGearPair.mPinion.phi_gama;
		OutData[i * mGearIndex + tempIndex + 89] = gRVReducer.mFirstGearPair.mPinion.phi_j;
		OutData[i * mGearIndex + tempIndex + 90] = gRVReducer.mFirstGearPair.mPinion.v_gf;
		OutData[i * mGearIndex + tempIndex + 91] = gRVReducer.mFirstGearPair.mPinion.v_ga;
		OutData[i * mGearIndex + tempIndex + 92] = gRVReducer.mFirstGearPair.mPinion.K_gf;
		OutData[i * mGearIndex + tempIndex + 93] = gRVReducer.mFirstGearPair.mPinion.K_ga;
		OutData[i * mGearIndex + tempIndex + 94] = gRVReducer.mFirstGearPair.mPinion.zeta;
		OutData[i * mGearIndex + tempIndex + 95] = gRVReducer.mFirstGearPair.mPinion.zeta_f;
	}
}

void ConvertInputDataToRVSetTotalObj(double *Indata, CRVReducerTAN &OutObj)
{
	OutObj.mFixElemNotation = static_cast<int>(Indata[0]);
	OutObj.mOutputTorque = Indata[1];
	OutObj.mOutputRPM = Indata[2];
	OutObj.mFirstGearPair.mPinion.Z = static_cast<int>(Indata[3]);
	OutObj.mFirstGearPair.mGear.Z = static_cast<int>(Indata[4]);
	OutObj.mSecondGearPair.mPinionList[0].Z = static_cast<int>(Indata[5]);
	OutObj.mSecondGearPair.mGear.Z = static_cast<int>(Indata[6]);
	// OutObj.mThirdGearPair.mPinionList[0].Z = OutObj.mSecondGearPair.mPinionList[0].Z;
	// OutObj.mThirdGearPair.mGear.Z = OutObj.mSecondGearPair.mGear.Z;
}

// 将摆线针轮数组指针转换为基本对象
void ConvertInputDataToCycloidObj(double *InputData, CPinionCycloidGearTAN &OutObj)
{
	double &zb = InputData[0];					// 中心轮，大轮齿数
	double &zg = InputData[1];					// 摆线行星轮齿数
	double &rz = InputData[2];					// 滚针半径
	double &Rz = InputData[3];					// 摆线轮理论齿廓的平均半径，分度圆半径,中心轮半径
	double &ee = InputData[4];					// 偏心距
	OutObj.Kp = InputData[5];					// 使用系数
	double &GearWidth = InputData[6];			// 针轮齿宽
	OutObj.mPinionTotalTorque = InputData[7];	// 摆线轮总力矩
	OutObj.mGear.EModule = InputData[8];		// 针轮弹性模量
	int TmpNg = static_cast<int>(InputData[9]); // 摆线轮数目
	int TmpLoopStart = 10;
	for (int i = 0; i < TmpNg; i++)
	{
		OutObj.mPinionList[i].ToothWidth = InputData[i + TmpLoopStart]; // 摆线轮齿宽
	}
	OutObj.DefineBasicGeometryPara(zb, zg, rz, Rz, ee, &InputData[TmpLoopStart], GearWidth, TmpNg);
	TmpLoopStart = TmpLoopStart + TmpNg;
	// for (int i = 0;i < TmpNg; i++)
	//{
	//	OutObj.mPinionList[i].EModule = InputData[i + TmpLoopStart];//摆线弹性模量
	// }
	OutObj.CalBasicGeometry();
	OutObj.DefineStressCalPara(OutObj.mPinionTotalTorque,
							   &InputData[TmpLoopStart],
							   OutObj.mGear.EModule,
							   OutObj.Kp);
}
void ConvertInputDataToCycloidObj(int *InputIndexDividIndex, double *InputData, CPinionCycloidGearTAN &OutObj)
{
	OutObj.mGear.Z = InputData[0];		 // 中心轮，大轮齿数
	OutObj.mGear.rz = InputData[1];		 // 滚针半径
	OutObj.mGear.Rzf = InputData[2];	 // 摆线轮理论齿廓的平均半径，分度圆半径,中心轮半径
	OutObj.mGear.EModule = InputData[3]; // 弹性模量
	OutObj.mGear.ToothWidth = InputData[4];
	OutObj.Kp = InputData[5];
	OutObj.mGear.DefineLocationPara(&InputData[6]);
	int tmpNg = InputIndexDividIndex[2];
	if (OutObj.Ng != tmpNg)
	{
		OutObj.Ng = tmpNg;
		std::vector<CPinionCycloidGearTAN::tagPinion>(OutObj.Ng).swap(OutObj.mPinionList);
	}
	int tmpPinionParaNum = InputIndexDividIndex[4];
	int tmpLoopStartIndex = InputIndexDividIndex[3];
	for (int i = 0; i < OutObj.Ng; i++)
	{
		OutObj.mPinionList[i].Z = InputData[tmpLoopStartIndex + 0];
		OutObj.mPinionList[i].ToothWidth = InputData[tmpLoopStartIndex + 1];
		OutObj.mPinionList[i].EModule = InputData[tmpLoopStartIndex + 2];
		OutObj.mOtherCoef.ee = InputData[tmpLoopStartIndex + 3];
		OutObj.Kp = InputData[tmpLoopStartIndex + 4];
		OutObj.mPinionTotalTorque = InputData[tmpLoopStartIndex + 5];
		OutObj.mPinionList[i].DefineLocationPara(&InputData[tmpLoopStartIndex + 6]);
		tmpLoopStartIndex = tmpLoopStartIndex + tmpPinionParaNum;
	}
	OutObj.CalBasicGeometry();
	OutObj.CalPinAndGearTorque(OutObj.mPinionTotalTorque);

	OutObj.CalcMValue(OutObj.mPinionList[0].Z, 2 * OutObj.mGear.rz, OutObj.mGear.Rzf, OutObj.mOtherCoef.ee, InputData[InputIndexDividIndex[3] + 8], InputData[InputIndexDividIndex[3] + 9]);
	OutObj.CalToothTipDistanceValue(OutObj.mPinionList[0].Z, 2 * OutObj.mGear.rz, OutObj.mGear.Rzf, OutObj.mOtherCoef.ee);
	OutObj.ObtainFourCycloidPara(InputData[InputIndexDividIndex[3] + 10], OutObj.mGear.Rzf, OutObj.mGear.Rzf * 2.0 / OutObj.mPinionList[0].Z, OutObj.mPinionList[0].ToothWidth);
}
void ConvertInputDataToInvObj(int *InputIndexDividIndex, double *InputData, CInvoluteGearTAN &OutObj)
{
	int i = 0;
	double Esp = 1E-8;
	// 分类数据接口开始索引
	int tmpStartIndex = 2;
	int tmpInitialIndexOffset = tmpStartIndex - 1;
	int tmpColumnCount = InputIndexDividIndex[tmpStartIndex];
	double *tmpGeometryData = &InputData[0];
	// 几何参数
	OutObj.DefineGearPairGeometryZJ(tmpGeometryData, &tmpGeometryData[tmpColumnCount], &tmpGeometryData[2 * tmpColumnCount]);
	// 0-物性参数，1-加工参数
	tmpColumnCount = InputIndexDividIndex[tmpStartIndex + 4 * 1];
	double *tmpMaterialAndManufactureData = &InputData[InputIndexDividIndex[tmpInitialIndexOffset + 4]];
	OutObj.DefineGearPairMaterial(&tmpMaterialAndManufactureData[0],
								  &tmpMaterialAndManufactureData[1 * tmpColumnCount],
								  &tmpMaterialAndManufactureData[2 * tmpColumnCount],
								  &tmpMaterialAndManufactureData[3 * tmpColumnCount]);

	tmpColumnCount = InputIndexDividIndex[tmpStartIndex + 4 * 2];
	double *tmpAppData = &InputData[InputIndexDividIndex[tmpInitialIndexOffset + 4 * 2]];
	OutObj.DefineGearPairApplicationData(tmpAppData,
										 &tmpAppData[tmpColumnCount],
										 &tmpAppData[2 * tmpColumnCount]);

	tmpColumnCount = InputIndexDividIndex[tmpStartIndex + 4 * 3];
	double *tmpTolerenceData = &InputData[InputIndexDividIndex[tmpInitialIndexOffset + 4 * 3]];
	OutObj.DefineGearPairTolerance(tmpTolerenceData, &tmpTolerenceData[tmpColumnCount]);

	tmpColumnCount = InputIndexDividIndex[tmpStartIndex + 4 * 4];
	double *tmpLoadData = &InputData[InputIndexDividIndex[tmpInitialIndexOffset + 4 * 4]];
	OutObj.DefineLoadPara(tmpLoadData[0], tmpLoadData[1], tmpLoadData[2], 0, 0);

	tmpColumnCount = InputIndexDividIndex[tmpStartIndex + 4 * 5];
	double *tmpAssembleData = &InputData[InputIndexDividIndex[tmpInitialIndexOffset + 4 * 5]];
	OutObj.DefineGearPairAssembleData(tmpAssembleData);

	// 安装位置参数
	tmpColumnCount = InputIndexDividIndex[tmpStartIndex + 4 * 6];
	double *tmpLocData = &InputData[InputIndexDividIndex[tmpInitialIndexOffset + 4 * 6]];
	OutObj.mPinion.DefineLocationPara(&tmpLocData[0]);
	OutObj.mGear.DefineLocationPara(&tmpLocData[tmpColumnCount]);
}
void ConvertInputDataToShaftObj(int *InputIntIndexDividIndex, int *InputDoubleIndexDividIndex, int *InputIntData, double *InputDoubleData, CShaftTAN &OutObj)
{
	OutObj.mCalSecTypeToDataCount = &InputIntData[InputIntIndexDividIndex[1]];
	OutObj.DefineOrginGeometry(InputIntData[InputIntIndexDividIndex[0]],
							   &InputDoubleData[InputDoubleIndexDividIndex[0]],
							   &InputDoubleData[InputDoubleIndexDividIndex[1]],
							   &InputIntData[InputIntIndexDividIndex[2]],
							   &InputDoubleData[InputDoubleIndexDividIndex[3]]);
	OutObj.mEX = InputDoubleData[InputIntIndexDividIndex[1] + 0];
	OutObj.mPRXY = InputDoubleData[InputIntIndexDividIndex[1] + 1];
}
void ConvertInputDataToShaftObj(int *InputIntData, double *InputDoubleData, CShaftTAN &OutObj)
{
	OutObj.mCalSecTypeToDataCount = &InputIntData[InputIntData[1]];
	OutObj.DefineOrginGeometry(InputIntData[0],
							   &InputDoubleData[InputIntData[3]],
							   &InputDoubleData[InputIntData[4]],
							   &InputIntData[InputIntData[2]],
							   &InputDoubleData[InputIntData[6]]);
	OutObj.mEX = InputDoubleData[InputIntData[5] + 0];
	OutObj.mPRXY = InputDoubleData[InputIntData[5] + 1];
}
void ConvertInputDataToBearingObj(int InLocationParaStartIndex, double *InputDoubleData, CBearingTAN &OutObj)
{
	// 定义结构参数
	OutObj.DefineRollingBearing(static_cast<int>(InputDoubleData[0]), &InputDoubleData[1]);
	// 定义定位尺寸
	OutObj.DefineLocationPara(&InputDoubleData[InLocationParaStartIndex]);
}
void NSTanExportSpace::ExportInvGearDataToOutputDataList(CInvoluteGearTAN::CSingleInvoluteTAN &InGear, double *OutData)
{
	OutData[0] = InGear.sigma_H;  // 接触应力
	OutData[1] = InGear.sigma_HP; // 接触应力
	OutData[2] = InGear.sigma_F0; // 名义弯曲应力
	OutData[3] = InGear.sigma_F;  // 计算接触应力
}
void RecommondPara(double *Indata, double *OutData)
{
	CRVReducerTAN TestRVReducer;
	// 总体传动比分配
	TestRVReducer.mFixElemNotation = Indata[0];
	TestRVReducer.RecommandTransmissionRatio(Indata[0], Indata[1]);
	// RV-80E1S 推荐
	// 摆线轮推荐
	TestRVReducer.mSecondGearPair.CopyCycloidWidthConstraintPara(&Indata[9]);
	// TestRVReducer.CalTotalTransRatio(3.8,40);
	TestRVReducer.mOutputTorque = Indata[2];
	TestRVReducer.mOutputRPM = Indata[3];
	TestRVReducer.CalGearPairTorque();
	TestRVReducer.mSecondPairPinionTorqueForShaft;
	double TestRVTargetContactStress = Indata[4];
	TestRVReducer.mSecondGearPair.RecommondMainGeometryParaByContact(TestRVTargetContactStress,
																	 TestRVReducer.mSecondPairPinionTorqueForShaft,
																	 TestRVReducer.mSecondGearPair.mGear.Z,
																	 TestRVReducer.mSecondGearPair.Ng);
	// 渐开线齿轮副推荐
	TestRVReducer.mDesignLife = Indata[5];
	TestRVReducer.mFirstGearPair.RecommondMainGeometryParaByContact(TestRVReducer.mFirstPairGearRatio,
																	Indata[6], // 使用系数
																	TestRVReducer.mFirstPairMotorInputTorque,
																	TestRVReducer.mFirstPairPinionRPM,
																	TestRVReducer.mDesignLife,
																	Indata[7], // 材料许用极限接触应力
																	ExternalMeshNotation,
																	Indata[8]); // 行星轮数目

	// 需要推荐的渐开线齿轮参数
	OutData[0] = TestRVReducer.mFirstGearPair.mn;
	OutData[1] = TestRVReducer.mFirstGearPair.alpha_n / Degree;
	OutData[2] = TestRVReducer.mFirstGearPair.beta / Degree;
	OutData[3] = TestRVReducer.mFirstGearPair.a_w;
	OutData[4] = TestRVReducer.mFirstGearPair.GearPairType; // 齿轮副类型：一般外啮合
	OutData[5] = TestRVReducer.mFirstGearPair.n_P;			// 行星轮个数
	OutData[6] = TestRVReducer.mFirstGearPair.DrivingGear;	// 驱动齿轮
	OutData[7] = TestRVReducer.mFirstGearPair.j_bn;			// 触点间隙
	OutData[8] = TestRVReducer.mFirstGearPair.n0;			// 主动轮转速
	int tmpUp = 9;
	int tmpDown = 31;
	for (int i = 0; i < 2; i++)
	{
		OutData[i * tmpDown + tmpUp + 0] = TestRVReducer.mFirstGearPair.mGearPtrList[i]->Z;
		OutData[i * tmpDown + tmpUp + 1] = TestRVReducer.mFirstGearPair.mGearPtrList[i]->SpiralIndex;
		OutData[i * tmpDown + tmpUp + 2] = TestRVReducer.mFirstGearPair.mGearPtrList[i]->b_Design;
		OutData[i * tmpDown + tmpUp + 3] = TestRVReducer.mFirstGearPair.mGearPtrList[i]->h_ax;
		OutData[i * tmpDown + tmpUp + 4] = TestRVReducer.mFirstGearPair.mGearPtrList[i]->x;
		OutData[i * tmpDown + tmpUp + 5] = TestRVReducer.mFirstGearPair.mGearPtrList[i]->cx;
		OutData[i * tmpDown + tmpUp + 6] = TestRVReducer.mFirstGearPair.mGearPtrList[i]->Rhf_fPx;
		OutData[i * tmpDown + tmpUp + 7] = TestRVReducer.mFirstGearPair.mGearPtrList[i]->s_pr;
		OutData[i * tmpDown + tmpUp + 8] = TestRVReducer.mFirstGearPair.mGearPtrList[i]->C_a;
		OutData[i * tmpDown + tmpUp + 9] = TestRVReducer.mFirstGearPair.mGearPtrList[i]->C_f;
		OutData[i * tmpDown + tmpUp + 10] = TestRVReducer.mFirstGearPair.mGearPtrList[i]->Angle; // 是否使用滚动角
		OutData[i * tmpDown + tmpUp + 11] = TestRVReducer.mFirstGearPair.mGearPtrList[i]->k;
		OutData[i * tmpDown + tmpUp + 12] = TestRVReducer.mFirstGearPair.mGearPtrList[i]->h_k;
		OutData[i * tmpDown + tmpUp + 13] = TestRVReducer.mFirstGearPair.mGearPtrList[i]->D_i;
		OutData[i * tmpDown + tmpUp + 14] = TestRVReducer.mFirstGearPair.mGearPtrList[i]->k·mn;
		OutData[i * tmpDown + tmpUp + 15] = TestRVReducer.mFirstGearPair.mGearPtrList[i]->E_sns;
		OutData[i * tmpDown + tmpUp + 16] = TestRVReducer.mFirstGearPair.mGearPtrList[i]->E_sni;
		OutData[i * tmpDown + tmpUp + 17] = TestRVReducer.mFirstGearPair.mGearPtrList[i]->Ada_upper;
		OutData[i * tmpDown + tmpUp + 18] = TestRVReducer.mFirstGearPair.mGearPtrList[i]->Ada_lower;
		OutData[i * tmpDown + tmpUp + 19] = TestRVReducer.mFirstGearPair.mGearPtrList[i]->q_max;
		OutData[i * tmpDown + tmpUp + 20] = TestRVReducer.mFirstGearPair.mGearPtrList[i]->q_min;
		OutData[i * tmpDown + tmpUp + 21] = TestRVReducer.mFirstGearPair.mGearPtrList[i]->d_y;
		OutData[i * tmpDown + tmpUp + 22] = TestRVReducer.mFirstGearPair.mGearPtrList[i]->Cutter;
		OutData[i * tmpDown + tmpUp + 23] = TestRVReducer.mFirstGearPair.mGearPtrList[i]->h_aP0;
		OutData[i * tmpDown + tmpUp + 24] = TestRVReducer.mFirstGearPair.mGearPtrList[i]->rho_aP0;
		OutData[i * tmpDown + tmpUp + 25] = TestRVReducer.mFirstGearPair.mGearPtrList[i]->z0;
		OutData[i * tmpDown + tmpUp + 26] = TestRVReducer.mFirstGearPair.mGearPtrList[i]->a0;
		OutData[i * tmpDown + tmpUp + 27] = TestRVReducer.mFirstGearPair.mGearPtrList[i]->d_a0;
		OutData[i * tmpDown + tmpUp + 28] = TestRVReducer.mFirstGearPair.mGearPtrList[i]->d_b0;
		OutData[i * tmpDown + tmpUp + 29] = TestRVReducer.mFirstGearPair.mGearPtrList[i]->d_Fa0;
		OutData[i * tmpDown + tmpUp + 30] = TestRVReducer.mFirstGearPair.mGearPtrList[i]->alpha_wt0;
	}
	// 需要推荐的摆线轮参数
	int tmpInvoluteGearParaCount = 71;
	// 针轮
	OutData[tmpInvoluteGearParaCount] = TestRVReducer.mSecondGearPair.mGear.Z;
	OutData[tmpInvoluteGearParaCount + 1] = TestRVReducer.mSecondGearPair.mGear.rz;			// 滚针半径
	OutData[tmpInvoluteGearParaCount + 2] = TestRVReducer.mSecondGearPair.mGear.ToothWidth; // 设计齿宽
	OutData[tmpInvoluteGearParaCount + 3] = TestRVReducer.mSecondGearPair.mGear.Rzf;		// 针轮分度圆半径Rzf
	OutData[tmpInvoluteGearParaCount + 4] = TestRVReducer.mSecondGearPair.mGear.EModule / 1000;
	OutData[tmpInvoluteGearParaCount + 5] = TestRVReducer.mSecondGearPair.Kp; // 使用系数
	tmpInvoluteGearParaCount = 77;
	int tmpFor = 6;
	// 两个摆线轮
	for (int i = 0; i < 2; i++)
	{
		OutData[tmpInvoluteGearParaCount + i * tmpFor + 0] = TestRVReducer.mSecondGearPair.mPinionList[i].Z;
		OutData[tmpInvoluteGearParaCount + i * tmpFor + 1] = TestRVReducer.mSecondGearPair.mOtherCoef.ee;				  // 偏心距
		OutData[tmpInvoluteGearParaCount + i * tmpFor + 2] = TestRVReducer.mSecondGearPair.mPinionList[i].ToothWidth;	  // 设计齿宽
		OutData[tmpInvoluteGearParaCount + i * tmpFor + 3] = TestRVReducer.mSecondGearPair.mPinionList[i].EModule / 1000; // 弹性模量
		OutData[tmpInvoluteGearParaCount + i * tmpFor + 4] = TestRVReducer.mSecondGearPair.Kp;							  // 使用系数
		OutData[tmpInvoluteGearParaCount + i * tmpFor + 5] = TestRVReducer.mSecondGearPair.mPinionTotalTorque;			  // 摆线轮传递总力矩，可能不是mPinionTotalTorque！！！
	}
	// 需要推荐的其他参数
	int tmpUpAll = tmpInvoluteGearParaCount + 2 * tmpFor;
	OutData[tmpUpAll + 0] = TestRVReducer.mFirstPairMotorInputTorque;
	OutData[tmpUpAll + 1] = TestRVReducer.mFirstPairPinionRPM;
}
int RecommondBearingLife(double *Indata, double *OutData, char **OutCodes, int maxCount, int maxLen)
{
	CBearingTAN TestBearing;
	int BearingType = static_cast<int>(Indata[0]);
	TestBearing.CalAndRecommedBearingLife(BearingType, &Indata[1]);

	int count = static_cast<int>(TestBearing.mBearingDataVec.size());
	if (count > maxCount)
		count = maxCount;

	int tmpDataCount = 0;
	if (BearingType == TaperedRollerBearing)
	{
		tmpDataCount = 9;
		for (int i = 0; i < count; i++)
		{
			strncpy_s(OutCodes[i], maxLen, TestBearing.mBearingDataVec[i].code.c_str(), _TRUNCATE);

			OutData[i * tmpDataCount + 0] = TestBearing.mBearingDataVec[i].d;
			OutData[i * tmpDataCount + 1] = TestBearing.mBearingDataVec[i].D;
			OutData[i * tmpDataCount + 2] = TestBearing.mBearingDataVec[i].B;
			OutData[i * tmpDataCount + 3] = TestBearing.mBearingDataVec[i].Cr;
			OutData[i * tmpDataCount + 4] = TestBearing.mBearingDataVec[i].C0r;
			OutData[i * tmpDataCount + 5] = TestBearing.mBearingDataVec[i].e;
			OutData[i * tmpDataCount + 6] = TestBearing.mBearingDataVec[i].Y;
			OutData[i * tmpDataCount + 7] = TestBearing.mBearingDataVec[i].Yo;
			OutData[i * tmpDataCount + 8] = TestBearing.mBearingDataVec[i].life;
		}
	}
	else
	{
		tmpDataCount = 6;
		for (int i = 0; i < count; i++)
		{
			strncpy_s(OutCodes[i], maxLen, TestBearing.mBearingDataVec[i].code.c_str(), _TRUNCATE);

			OutData[i * tmpDataCount + 0] = TestBearing.mBearingDataVec[i].d;
			OutData[i * tmpDataCount + 1] = TestBearing.mBearingDataVec[i].D;
			OutData[i * tmpDataCount + 2] = TestBearing.mBearingDataVec[i].B;
			OutData[i * tmpDataCount + 3] = TestBearing.mBearingDataVec[i].Cr;
			OutData[i * tmpDataCount + 4] = TestBearing.mBearingDataVec[i].C0r;
			OutData[i * tmpDataCount + 5] = TestBearing.mBearingDataVec[i].life;
		}
	}

	return count;
}
void GetForceAndForceLocation(int *CoordinateNumber, double *OutputDataX, double *OutputDataY)
{
	int tmpSize = gRVReducer.mCrankAxleSystem.mShaft.BendingNormalStress.size();
	CoordinateNumber[0] = tmpSize;
	int j = 0;
	for (int i = 0; i < tmpSize; i++)
	{
		OutputDataY[i] = gRVReducer.mCrankAxleSystem.mShaft.BendingNormalStress[i]; // 用于Y轴坐标
		OutputDataY[i + tmpSize] = gRVReducer.mCrankAxleSystem.mShaft.ShearStress[i];
	}
	for (auto value : gRVReducer.mCrankAxleSystem.mShaft.mNodeZCoorList)
	{
		OutputDataX[j] = value; // 用于X轴坐标
		j++;
		OutputDataX[j] = value;
		j++;
	}
}
void RecommondShaftPara(int *InputIntIndexDividIndex, int *InputIntData, double *InputDoubleData, double *OutputData, double *OutData)
{
	CBearingTAN TestBearing;
	std::vector<double> params = {};
	double tmpInLife = OutData[4]; // 输入的设计寿命
	////圆锥滚针轴承部分：
	// TestBearing.ObtainBearingDataVec(TaperedRollerBearing);
	auto taperedRollerBearingList = TestBearing.ObtainBearingDataVec(TaperedRollerBearing);
	// std::vector<double> standardDiameters = TestBearing.mDiameterList;//从数据库获取标准轴径表
	// if (standardDiameters.empty()|| TestBearing.mBearingDataVec.empty()) {
	//	std::cerr << "无法从数据库获取标准轴径表！" << std::endl;
	//	return;
	// }
	// 1.初始化轴的长度
	double Length1 = 0; // 1.大轮距左端面的最近距离  ------------如果渐开线齿轮的输入参数变化，182（10+170+2）,OutData[42]也需要变化
	if (InputDoubleData[183] == 1)
	{
		Length1 = InputDoubleData[182];
	}
	else if (InputDoubleData[183] == -1)
	{
		Length1 = InputDoubleData[182] - gRVReducer.mFirstGearPair.mGear.b_Design;
	}
	else
	{
		Length1 = InputDoubleData[182] - gRVReducer.mFirstGearPair.mGear.b_Design / 2;
	}

	// double Length2 = gRVReducer.mFirstGearPair.mGear.b_Design;//2.大轮设计齿宽
	double Length2 = InputDoubleData[InputIntIndexDividIndex[1 + 1 * 3] + 2];
	double Length3 = 4;	   // 3.大轮距圆锥滚子的最近距离
	double Length4 = 0;	   // 4.第一个圆锥滚子轴承的宽度
	double Length5 = 3.25; // 5.圆锥滚子轴承距摆线轮的最近距离
	// Length += gRVReducer.mSecondGearPair.mPinionList[0].ToothWidth;//6.第一片摆线轮的宽度
	double Length6 = InputDoubleData[InputIntIndexDividIndex[1 + 2 * 3] + InputIntData[InputIntIndexDividIndex[0 + 2 * 3] + 3] + 1]; // 6.第一片摆线轮的宽度
	double Length7 = 0;																												 // 7.两片摆线轮的间距
	// Length += gRVReducer.mSecondGearPair.mPinionList[1].ToothWidth;//8.第二片摆线轮的宽度
	double Length8 = InputDoubleData[InputIntIndexDividIndex[1 + 2 * 3] + InputIntData[InputIntIndexDividIndex[0 + 2 * 3] + 3] + 1 + InputIntData[InputIntIndexDividIndex[0 + 2 * 3] + 4]]; // 8.第二片摆线轮的宽度
	double Length9 = 5;																																										// 9.第二片摆线轮距第二个圆锥滚子轴承的距离
	double Length10 = 0;																																									// 10.第二个圆锥滚子轴承的宽度

	double TotalLength = Length1 + Length2 + Length3 + Length4 + Length5 + Length6 + Length7 + Length8 + Length9 + Length10; // 轴的总长度

	double allowedBending = (OutData[2] > 0 ? OutData[2] : 1e9);
	double allowedShear = (OutData[3] > 0 ? OutData[3] : 1e9);

	bool foundSolution = false;
	bool foundNeedleBearing = false;
	// 2.对数据库中的轴承开始遍历
	std::set<double> processedDiameters; // 用于去重
	for (const auto &b : taperedRollerBearingList)
	{
		if (processedDiameters.count(b.d))
			continue; // 已经处理过该内径的轴承，跳过
		processedDiameters.insert(b.d);
		double bearingInnerDiameter = b.d;
		double bearingWidthT = b.T;
		double bearingA = b.a; // 作用点距离左端面距离a
		// 3.装配，定位后进行应力计算，若不符合应力要求再从数据库中选择直径更大的轴承，反复计算，直至应力满足要求！得到最终的轴的直径和该装配的轴承！
		double tmpShaftLength = TotalLength + 2 * bearingWidthT;
		Length4 = bearingWidthT;
		Length10 = bearingWidthT;
		// 更新轴参数
		InputDoubleData[InputIntIndexDividIndex[1 + 4 * 3] + 1] = tmpShaftLength;		// 轴长度
		InputDoubleData[InputIntIndexDividIndex[1 + 4 * 3] + 7] = bearingInnerDiameter; // 轴承内径（轴直径）
		// 更新轴承基本参数
		InputDoubleData[InputIntIndexDividIndex[1 + 3 * 3] + 0] = TaperedRollerBearing; // 轴承类型
		InputDoubleData[InputIntIndexDividIndex[1 + 3 * 3] + 1] = gRVReducer.mFirstPairCrankShaftBearingRPM;
		InputDoubleData[InputIntIndexDividIndex[1 + 3 * 3] + 2] = b.Cr;
		InputDoubleData[InputIntIndexDividIndex[1 + 3 * 3] + 3] = b.C0r;
		InputDoubleData[InputIntIndexDividIndex[1 + 3 * 3] + 4] = b.e;
		InputDoubleData[InputIntIndexDividIndex[1 + 3 * 3] + 5] = b.Y;
		InputDoubleData[InputIntIndexDividIndex[1 + 3 * 3] + 6] = b.Yo;

		InputDoubleData[InputIntIndexDividIndex[1 + 3 * 3] + 3 * InputIntData[InputIntIndexDividIndex[0 + 3 * 3] + 1] + 0] = TaperedRollerBearing; // 轴承类型
		InputDoubleData[InputIntIndexDividIndex[1 + 3 * 3] + 3 * InputIntData[InputIntIndexDividIndex[0 + 3 * 3] + 1] + 1] = gRVReducer.mFirstPairCrankShaftBearingRPM;
		InputDoubleData[InputIntIndexDividIndex[1 + 3 * 3] + 3 * InputIntData[InputIntIndexDividIndex[0 + 3 * 3] + 1] + 2] = b.Cr;
		InputDoubleData[InputIntIndexDividIndex[1 + 3 * 3] + 3 * InputIntData[InputIntIndexDividIndex[0 + 3 * 3] + 1] + 3] = b.C0r;
		InputDoubleData[InputIntIndexDividIndex[1 + 3 * 3] + 3 * InputIntData[InputIntIndexDividIndex[0 + 3 * 3] + 1] + 4] = b.e;
		InputDoubleData[InputIntIndexDividIndex[1 + 3 * 3] + 3 * InputIntData[InputIntIndexDividIndex[0 + 3 * 3] + 1] + 5] = b.Y;
		InputDoubleData[InputIntIndexDividIndex[1 + 3 * 3] + 3 * InputIntData[InputIntIndexDividIndex[0 + 3 * 3] + 1] + 6] = b.Yo;
		// 更新轴承定位参数，主要是轴承作用点距离左端面距离a和轴承宽度(这里的宽度是使用的圆锥滚子轴承的宽度T)
		InputDoubleData[InputIntIndexDividIndex[1 + 3 * 3] + InputIntData[InputIntIndexDividIndex[0 + 3 * 3] + 5] + 0] = -1;																														   // 轴承定位面，安装面定位面类型，1-外圈侧壁,左侧，-1-内圈侧壁，右侧，0-作用点
		InputDoubleData[InputIntIndexDividIndex[1 + 3 * 3] + InputIntData[InputIntIndexDividIndex[0 + 3 * 3] + 5] + 1] = TestBearing.ObtainDistFromBearingLocatingSurfaceToReferenceSurface(-1, bearingA, bearingWidthT, Length1 + Length2 + Length3); // 偏离基准面的距离，需要由前面几个参数计算得出
		InputDoubleData[InputIntIndexDividIndex[1 + 3 * 3] + InputIntData[InputIntIndexDividIndex[0 + 3 * 3] + 5] + 2] = 1;																															   // 轴承作用点对应的轴承面类型
		InputDoubleData[InputIntIndexDividIndex[1 + 3 * 3] + InputIntData[InputIntIndexDividIndex[0 + 3 * 3] + 5] + 3] = bearingA;																													   // 轴承作用点距离距轴承作用点参考面距离
		InputDoubleData[InputIntIndexDividIndex[1 + 3 * 3] + InputIntData[InputIntIndexDividIndex[0 + 3 * 3] + 5] + 4] = 1;																															   // 轴承轴线方向/轴承朝向
		InputDoubleData[InputIntIndexDividIndex[1 + 3 * 3] + InputIntData[InputIntIndexDividIndex[0 + 3 * 3] + 5] + 5] = bearingWidthT;

		InputDoubleData[InputIntIndexDividIndex[1 + 3 * 3] + 3 * InputIntData[InputIntIndexDividIndex[0 + 3 * 3] + 1] + InputIntData[InputIntIndexDividIndex[0 + 3 * 3] + 5] + 0] = 1;
		InputDoubleData[InputIntIndexDividIndex[1 + 3 * 3] + 3 * InputIntData[InputIntIndexDividIndex[0 + 3 * 3] + 1] + InputIntData[InputIntIndexDividIndex[0 + 3 * 3] + 5] + 1] = TestBearing.ObtainDistFromBearingLocatingSurfaceToReferenceSurface(1, bearingA, bearingWidthT, tmpShaftLength - bearingWidthT);
		;
		InputDoubleData[InputIntIndexDividIndex[1 + 3 * 3] + 3 * InputIntData[InputIntIndexDividIndex[0 + 3 * 3] + 1] + InputIntData[InputIntIndexDividIndex[0 + 3 * 3] + 5] + 2] = 1;
		InputDoubleData[InputIntIndexDividIndex[1 + 3 * 3] + 3 * InputIntData[InputIntIndexDividIndex[0 + 3 * 3] + 1] + InputIntData[InputIntIndexDividIndex[0 + 3 * 3] + 5] + 3] = bearingA;
		InputDoubleData[InputIntIndexDividIndex[1 + 3 * 3] + 3 * InputIntData[InputIntIndexDividIndex[0 + 3 * 3] + 1] + InputIntData[InputIntIndexDividIndex[0 + 3 * 3] + 5] + 4] = -1;
		InputDoubleData[InputIntIndexDividIndex[1 + 3 * 3] + 3 * InputIntData[InputIntIndexDividIndex[0 + 3 * 3] + 1] + InputIntData[InputIntIndexDividIndex[0 + 3 * 3] + 5] + 5] = bearingWidthT;
		// 补充-更新针轮定位参数
		InputDoubleData[InputIntIndexDividIndex[1 + 2 * 3] + 6] = Length1 + Length2 + Length3 + Length4 + Length5;
		InputDoubleData[InputIntIndexDividIndex[1 + 2 * 3] + 7] = 1;
		// 补充-更新摆线轮定位参数
		InputDoubleData[InputIntIndexDividIndex[1 + 2 * 3] + InputIntData[InputIntIndexDividIndex[0 + 2 * 3] + 3] + 6] = Length1 + Length2 + Length3 + Length4 + Length5;
		InputDoubleData[InputIntIndexDividIndex[1 + 2 * 3] + InputIntData[InputIntIndexDividIndex[0 + 2 * 3] + 3] + 7] = 1;
		InputDoubleData[InputIntIndexDividIndex[1 + 2 * 3] + InputIntData[InputIntIndexDividIndex[0 + 2 * 3] + 3] + 6 + InputIntData[InputIntIndexDividIndex[0 + 2 * 3] + 4]] = Length1 + Length2 + Length3 + Length4 + Length5 + Length6 + Length7 + Length8;
		InputDoubleData[InputIntIndexDividIndex[1 + 2 * 3] + InputIntData[InputIntIndexDividIndex[0 + 2 * 3] + 3] + 7 + InputIntData[InputIntIndexDividIndex[0 + 2 * 3] + 4]] = -1;
		// 计算应力
		gRVReducer.mCrankAxleSystem.mShaft.ClearCalculatorData(); // 清理缓存
		CalCrankShaftSystem2(InputIntIndexDividIndex, InputIntData, InputDoubleData, OutputData);
		// gRVReducer.mCrankAxleSystem.mShaft.ClearCalculatorData(); //清理缓存
		double maxBend = 0.0, maxShear = 0.0;
		int tmpSize = gRVReducer.mCrankAxleSystem.mShaft.BendingNormalStress.size();
		for (int i = 0; i < tmpSize; i++)
		{
			maxBend = max(maxBend, std::abs(gRVReducer.mCrankAxleSystem.mShaft.BendingNormalStress[i]));
			maxShear = max(maxShear, std::abs(gRVReducer.mCrankAxleSystem.mShaft.ShearStress[i]));
		}
		// 应力校核
		if (maxBend > allowedBending || maxShear > allowedShear)
			continue; // 当前直径不够 → 试更大直径

		// 4.验证上述轴承是否满足寿命要求，若不满足，则更换下一个轴承，再从第二步开始执行
		// 进行轴承寿命验证
		// double tmpInLife = gRVReducer.mDesignLife;
		params = {gRVReducer.mFirstPairCrankShaftBearingRPM, b.Cr, b.C0r, b.e, b.Y, b.Yo};
		CBearingTAN testBearing(TaperedRollerBearing, params.data()); // n;cr;c0r;e;y;yo
		// double Fa1 = gRVReducer.mCrankAxleSystem.mShaft.mSolver.TotalFValue[6 * 2 + 2];
		// double Fr1 = sqrt(pow(gRVReducer.mCrankAxleSystem.mShaft.mSolver.TotalFValue[6 * 2 + 0], 2) + pow(gRVReducer.mCrankAxleSystem.mShaft.mSolver.TotalFValue[6 * 2 + 1], 2));
		// double Fa2 = gRVReducer.mCrankAxleSystem.mShaft.mSolver.TotalFValue[6 * 5 + 2];
		// double Fr2 = sqrt(pow(gRVReducer.mCrankAxleSystem.mShaft.mSolver.TotalFValue[6 * 5 + 0], 2) + pow(gRVReducer.mCrankAxleSystem.mShaft.mSolver.TotalFValue[6 * 5 + 1], 2));
		//  以第一个轴承为例，假设它位于节点idx1
		int idx1 = 2; // 轴承1所在节点
		CTanVector sumForce1(3);
		sumForce1[0] = sumForce1[1] = sumForce1[2] = 0;
		for (int j = 0; j <= idx1; ++j)
		{
			sumForce1[0] += gRVReducer.mCrankAxleSystem.mShaft.mSolver.TotalFValue[6 * j + 0];
			sumForce1[1] += gRVReducer.mCrankAxleSystem.mShaft.mSolver.TotalFValue[6 * j + 1];
			sumForce1[2] += gRVReducer.mCrankAxleSystem.mShaft.mSolver.TotalFValue[6 * j + 2];
		}
		double Fa1 = sumForce1[2];
		double Fr1 = sqrt(sumForce1[0] * sumForce1[0] + sumForce1[1] * sumForce1[1]);
		Fa1 = Fa1 / 1000;
		Fr1 = Fr1 / 1000;
		// 第二个轴承同理
		int idx2 = 5;
		CTanVector sumForce2(3);
		sumForce2[0] = sumForce2[1] = sumForce2[2] = 0;
		for (int j = 0; j <= idx2; ++j)
		{
			sumForce2[0] += gRVReducer.mCrankAxleSystem.mShaft.mSolver.TotalFValue[6 * j + 0];
			sumForce2[1] += gRVReducer.mCrankAxleSystem.mShaft.mSolver.TotalFValue[6 * j + 1];
			sumForce2[2] += gRVReducer.mCrankAxleSystem.mShaft.mSolver.TotalFValue[6 * j + 2];
		}
		double Fa2 = sumForce2[2];
		double Fr2 = sqrt(sumForce2[0] * sumForce2[0] + sumForce2[1] * sumForce2[1]);
		Fa2 = Fa2 / 1000;
		Fr2 = Fr2 / 1000;

		double tmpOutLife1 = testBearing.GetBearingRealLife(Fa1, Fr1); // 传入Fa，Fr调用函数得到轴承寿命
		double tmpOutLife2 = testBearing.GetBearingRealLife(Fa2, Fr2); // 传入Fa，Fr调用函数得到轴承寿命
		if (tmpOutLife1 >= tmpInLife && tmpOutLife2 >= tmpInLife)
		{
			// 通轴直径和长度参数
			OutData[0] = b.d;
			OutData[1] = tmpShaftLength;
			// 1.圆锥滚针轴承数据
			OutData[2] = InputDoubleData[InputIntIndexDividIndex[1 + 3 * 3] + InputIntData[InputIntIndexDividIndex[0 + 3 * 3] + 5] + 0]; // 轴承定位面
			OutData[3] = InputDoubleData[InputIntIndexDividIndex[1 + 3 * 3] + InputIntData[InputIntIndexDividIndex[0 + 3 * 3] + 5] + 1]; // 偏离基准面的距离
			OutData[4] = b.d;
			OutData[5] = b.D;
			OutData[6] = b.T;
			OutData[7] = b.B;
			OutData[8] = b.C;
			OutData[9] = InputDoubleData[InputIntIndexDividIndex[1 + 3 * 3] + InputIntData[InputIntIndexDividIndex[0 + 3 * 3] + 5] + 4]; // 轴承轴线方向/轴承朝向
			// 2.圆锥滚针轴承数据
			OutData[10] = InputDoubleData[InputIntIndexDividIndex[1 + 3 * 3] + 3 * InputIntData[InputIntIndexDividIndex[0 + 3 * 3] + 1] + InputIntData[InputIntIndexDividIndex[0 + 3 * 3] + 5] + 0]; // 轴承定位面
			OutData[11] = InputDoubleData[InputIntIndexDividIndex[1 + 3 * 3] + 3 * InputIntData[InputIntIndexDividIndex[0 + 3 * 3] + 1] + InputIntData[InputIntIndexDividIndex[0 + 3 * 3] + 5] + 1];
			OutData[12] = b.d;
			OutData[13] = b.D;
			OutData[14] = b.T;
			OutData[15] = b.B;
			OutData[16] = b.C;
			OutData[17] = InputDoubleData[InputIntIndexDividIndex[1 + 3 * 3] + 3 * InputIntData[InputIntIndexDividIndex[0 + 3 * 3] + 1] + InputIntData[InputIntIndexDividIndex[0 + 3 * 3] + 5] + 4];
			foundSolution = true;
			// 其他圆锥滚针轴承参数（用于UI显示）
			OutData[70] = b.Cr;
			OutData[71] = b.C0r;
			OutData[72] = b.e;
			OutData[73] = b.Y;
			OutData[74] = b.Yo;
			OutData[75] = b.a;
			break; // 找到合适组合，停止搜索
		}

		// 否则继续尝试下一个更大直径
	}
	// 如果未找到合适的组合
	if (!foundSolution)
	{
		OutData[0] = -1;
		OutData[1] = -1;
		return;
	}

	////滚针轴承部分：
	auto needlebearingList = TestBearing.ObtainBearingDataVec(NeedleBearing);
	for (const auto &b : needlebearingList)
	{
		if (b.d >= OutData[0] + 2 * gRVReducer.mSecondGearPair.mOtherCoef.ee) // 需要滚针轴承的内径大于推荐轴承的直径 + 2*偏心量 -> 接着调整曲轴的直径为符合所以要求的滚针轴承的内径！
		{
			params = {gRVReducer.mSecondPairPinionBearingRPM, b.Cr, b.C0r};
			CBearingTAN testBearing(NeedleBearing, params.data()); // n;cr;c0r
			double Fa1 = 0;
			double Fr1 = gRVReducer.mSecondGearPair.mPinionList[0].mMaxBearingF;
			double Fa2 = 0;
			double Fr2 = gRVReducer.mSecondGearPair.mPinionList[1].mMaxBearingF;
			double tmpOutLife1 = testBearing.GetBearingRealLife(Fa1, Fr1);								// 传入Fa，Fr调用函数得到轴承寿命
			double tmpOutLife2 = testBearing.GetBearingRealLife(Fa2, Fr2);								// 传入Fa，Fr调用函数得到轴承寿命
			if (tmpOutLife1 >= tmpInLife && tmpOutLife2 >= tmpInLife && b.B < Length6 && b.B < Length8) // 不仅要满足轴承寿命要求，还要满足滚针轴承宽度小于摆线轮宽度的要求
			{
				// 1.滚针轴承
				OutData[18] = 0;																																									// 中心基准面
				OutData[19] = Length1 + Length2 + Length3 + Length4 + Length5 + InputDoubleData[InputIntIndexDividIndex[1 + 2 * 3] + InputIntData[InputIntIndexDividIndex[0 + 2 * 3] + 3] + 1] / 2; // 偏离基准面距离
				OutData[20] = b.d;																																									// 滚针轴承内径FW || 曲轴直径
				OutData[21] = b.D;																																									// 滚针轴承外径EW
				OutData[22] = b.B;																																									// 滚针轴承宽度Bc
				// 2.滚针轴承
				OutData[23] = 0;
				OutData[24] = Length1 + Length2 + Length3 + Length4 + Length5 + Length6 + Length7 + InputDoubleData[InputIntIndexDividIndex[1 + 2 * 3] + InputIntData[InputIntIndexDividIndex[0 + 2 * 3] + 3] + 1 + InputIntData[InputIntIndexDividIndex[0 + 2 * 3] + 4]] / 2;
				OutData[25] = b.d;
				OutData[26] = b.D;
				OutData[27] = b.B;
				InputDoubleData[InputIntIndexDividIndex[1 + 3 * 3] + 1 * InputIntData[InputIntIndexDividIndex[0 + 3 * 3] + 1] + InputIntData[InputIntIndexDividIndex[0 + 3 * 3] + 5] + 5] = b.B;
				InputDoubleData[InputIntIndexDividIndex[1 + 3 * 3] + 2 * InputIntData[InputIntIndexDividIndex[0 + 3 * 3] + 1] + InputIntData[InputIntIndexDividIndex[0 + 3 * 3] + 5] + 5] = b.B;
				foundNeedleBearing = true;

				// 其他滚针轴承参数（用于UI显示）
				OutData[80] = b.Cr;
				OutData[81] = b.C0r;
				break; // 找到合适组合，停止搜索
			}
		}
	}
	if (!foundNeedleBearing)
	{
		OutData[0] = -2;
		OutData[1] = -2;
		return;
	}
	// 1.摆线轮参数
	OutData[28] = 2 * gRVReducer.mSecondGearPair.mPinionList[0].ra; // 摆线轮直径
	OutData[29] = InputDoubleData[InputIntIndexDividIndex[1 + 2 * 3] + InputIntData[InputIntIndexDividIndex[0 + 2 * 3] + 3] + 1];
	OutData[30] = Length1 + Length2 + Length3 + Length4 + Length5;
	OutData[31] = 0; // Y偏移
	OutData[32] = 1; // 左端面
	// 2.摆线轮参数
	OutData[33] = 2 * gRVReducer.mSecondGearPair.mPinionList[1].ra;
	OutData[34] = InputDoubleData[InputIntIndexDividIndex[1 + 2 * 3] + InputIntData[InputIntIndexDividIndex[0 + 2 * 3] + 3] + 1 + InputIntData[InputIntIndexDividIndex[0 + 2 * 3] + 4]];
	OutData[35] = Length1 + Length2 + Length3 + Length4 + Length5 + Length6 + Length7 + Length8;
	OutData[36] = 0;
	OutData[37] = -1;
	// 行星轮参数
	OutData[38] = gRVReducer.mFirstGearPair.mGear.da;
	OutData[39] = gRVReducer.mFirstGearPair.mGear.b_Design;
	OutData[40] = InputDoubleData[182]; // 定位面距离基准的距离
	OutData[41] = 0;
	OutData[42] = InputDoubleData[183];
	// 太阳轮参数
	OutData[45] = gRVReducer.mFirstGearPair.mPinion.da;
	OutData[46] = gRVReducer.mFirstGearPair.mPinion.b_Design;
	////曲轴部分轴段参数
	OutData[50] = Length1 + Length2 + Length3 + Length4 + Length5;			  // 曲轴1长度
	OutData[51] = Length6;													  // 曲轴2长度采用摆线轮1的宽度
	OutData[52] = Length7;													  // 曲轴3长度两片摆线轮过度阶段采用两片摆线轮的间距
	OutData[53] = Length8;													  // 曲轴4长度采用摆线轮2的宽度
	OutData[54] = Length9 + Length10;										  // 曲轴5长度
	OutData[55] = OutData[20];												  // 曲轴2直径
	OutData[56] = OutData[20] - 2 * gRVReducer.mSecondGearPair.mOtherCoef.ee; // 曲轴3直径
	OutData[57] = OutData[25];												  // 曲轴4直径
	OutData[60] = gRVReducer.mSecondGearPair.mOtherCoef.ee;					  // 偏心量

	// 补充参数：
	OutData[90] = 2 * gRVReducer.mSecondGearPair.mPinionList[0].rf; // 摆线轮齿根圆直径
	OutData[91] = 2 * gRVReducer.mSecondGearPair.mPinionList[1].rf;
}

void CalBearingLife(double *Indata, double *OutData)
{
	CBearingTAN TestBearing;
	int BearingType = static_cast<int>(Indata[0]);
	// TestBearing.GetBearingRealLife(fa, fr);
	//  根据类型条件，遍历数据库进行寿命计算
	if (BearingType == DeepGrooveBallBearing || BearingType == NeedleBearing)
	{
		CBearingTAN testBearing(BearingType, new double[3]{Indata[1], Indata[4], Indata[5]});
		OutData[0] = testBearing.GetBearingRealLife(Indata[2], Indata[3]); // 传入Fa，Fr调用函数得到轴承寿命
	}
	else if (BearingType == TaperedRollerBearing)
	{
		CBearingTAN testBearing(BearingType, new double[6]{Indata[1], Indata[4], Indata[5], Indata[6], Indata[7], Indata[8]});
		OutData[0] = testBearing.GetBearingRealLife(Indata[2], Indata[3]); // 传入Fa，Fr调用函数得到轴承寿命
	}
	else if (BearingType == AngularContactBallBearing)
	{
		double FaOverC0r = Indata[2] / Indata[5];
		double e, Y, Y1, Y2;
		if (TestBearing.GetLoadCoeff(FaOverC0r, e, Y, Y1, Y2))
		{
			CBearingTAN testBearing(BearingType, new double[7]{Indata[1], Indata[4], Indata[5], e, Y, Y1, Y2});
			OutData[0] = testBearing.GetBearingRealLife(Indata[2], Indata[3]); // 传入Fa，Fr调用函数得到轴承寿命
		}
	}
	else
	{
		std::cerr << "这里对所有需要的轴承进行寿命计算。可新建一个数据表格，包含所需的所有轴承，并且统一表头" << std::endl;
	}
}

void RecommondMainBearing(double *Indata, double *OutData)
{
	int tmpBearingType = TaperedRollerBearing;
	if (Indata[7] == 1)
		tmpBearingType = AngularContactBallBearing;
	double allowedBending = Indata[9];
	double allowedShear = Indata[10];
	bool foundSolution = false;
	gRVReducer.mFirstPairCrankShaftBearingRPM = Indata[8]; // 假设转速为1000rpm

	CBearingTAN TestBearing;
	double tmpBearingLife = Indata[11]; // 输入的设计寿命

	auto taperedRollerBearingList = TestBearing.ObtainBearingDataVec(tmpBearingType);

	std::set<double> processedDiameters; // 用于去重

	double bearingInnerDiameter = 0;
	double tmpLength = 0;
	double tmp_e = 0;
	double tmp_Y = 0;
	double tmp_Yo = 0;
	double tmp_C = 0;
	double tmp_a = 0;
	double tmp_contactAngle = 0;

	for (const auto &b : taperedRollerBearingList)
	{
		if (processedDiameters.count(b.d))
			continue; // 已经处理过该内径的轴承，跳过
		processedDiameters.insert(b.d);

		bearingInnerDiameter = b.d;
		tmpLength = b.B;
		tmp_a = b.a;

		if (tmpBearingType == TaperedRollerBearing) // 圆锥滚子轴承特殊处理
		{
			tmpLength = b.T;
			tmp_e = b.e;
			tmp_Y = b.Y;
			tmp_Yo = b.Yo;
			tmp_C = b.C;
		}
		if (tmpBearingType == AngularContactBallBearing)
		{
			tmp_contactAngle = b.contactAngle;
		}

		double distFromBearingLocatingSurfaceToReferenceSurface1 = TestBearing.ObtainDistFromBearingLocatingSurfaceToReferenceSurface(-1, tmp_a, tmpLength, 0); // 暂时采取0作为偏离基准面的距离，忽略齿轮的影响
		double distFromBearingLocatingSurfaceToReferenceSurface2 = TestBearing.ObtainDistFromBearingLocatingSurfaceToReferenceSurface(1, tmp_a, tmpLength, 2 * tmpLength);
		double *params = new double[19]{abs(gRVReducer.mFirstPairCrankShaftBearingRPM), b.Cr, b.C0r, tmp_e, tmp_Y, tmp_Yo,
										-1, distFromBearingLocatingSurfaceToReferenceSurface1, 1, tmp_a, 1, tmpLength,
										-1, distFromBearingLocatingSurfaceToReferenceSurface2, 1, tmp_a, -1, tmpLength,
										b.d};

		AssemblyGearAndMainBearing(gRVReducer.mCrankAxleSystem, &Indata[0], &params[0], &OutData[0]);
		// 应力校核
		if (OutData[0] > allowedBending || OutData[1] > allowedShear)
			continue;

		// 进行轴承寿命验证
		int idx1 = 1; // 轴承1所在节点
		int idx2 = 3; // 轴承2所在节点
		double targetForceValue = Indata[6];
		double epsilon = 1e-5;
		std::vector<double> extractedCoors;
		for (auto it = gRVReducer.mCrankAxleSystem.mBearingLocateCoorToPtrList.begin(); it != gRVReducer.mCrankAxleSystem.mBearingLocateCoorToPtrList.end(); ++it)
		{
			extractedCoors.push_back(it->first);
		}
		if (targetForceValue <= extractedCoors[0]) // 轴承1的作用力点是第三个点（总共5个）
		{
			idx1 = 2;
			idx2 = 3;
		}
		else if (targetForceValue >= extractedCoors[1])
		{
			idx2 = 2;
		}

		CTanVector sumForce1(3);
		sumForce1[0] = sumForce1[1] = sumForce1[2] = 0;
		for (int j = 0; j <= idx1; ++j)
		{
			sumForce1[0] += gRVReducer.mCrankAxleSystem.mShaft.mSolver.TotalFValue[6 * j + 0];
			sumForce1[1] += gRVReducer.mCrankAxleSystem.mShaft.mSolver.TotalFValue[6 * j + 1];
			sumForce1[2] += gRVReducer.mCrankAxleSystem.mShaft.mSolver.TotalFValue[6 * j + 2];
		}
		double Fa1 = sumForce1[2];
		double Fr1 = sqrt(sumForce1[0] * sumForce1[0] + sumForce1[1] * sumForce1[1]);
		Fa1 = Fa1 / 1000;
		Fr1 = Fr1 / 1000;

		CTanVector sumForce2(3);
		sumForce2[0] = sumForce2[1] = sumForce2[2] = 0;
		for (int j = 0; j <= idx2; ++j)
		{
			sumForce2[0] += gRVReducer.mCrankAxleSystem.mShaft.mSolver.TotalFValue[6 * j + 0];
			sumForce2[1] += gRVReducer.mCrankAxleSystem.mShaft.mSolver.TotalFValue[6 * j + 1];
			sumForce2[2] += gRVReducer.mCrankAxleSystem.mShaft.mSolver.TotalFValue[6 * j + 2];
		}
		double Fa2 = sumForce2[2];
		double Fr2 = sqrt(sumForce2[0] * sumForce2[0] + sumForce2[1] * sumForce2[1]);
		Fa2 = Fa2 / 1000;
		Fr2 = Fr2 / 1000;

		double tmpOutLife1 = gRVReducer.mBearingList[0].GetBearingRealLife(Fa1, Fr1); // 传入Fa，Fr调用函数得到轴承寿命
		double tmpOutLife2 = gRVReducer.mBearingList[1].GetBearingRealLife(Fa2, Fr2); // 传入Fa，Fr调用函数得到轴承寿命

		if (tmpOutLife1 >= tmpBearingLife && tmpOutLife2 >= tmpBearingLife)
		{
			// 通轴直径和长度参数
			OutData[0] = b.d;
			OutData[1] = 3 * tmpLength; // 暂时采取轴承的3 倍作为轴的长度
			// 1.圆锥滚针轴承数据
			OutData[2] = -1;												// 轴承定位面
			OutData[3] = distFromBearingLocatingSurfaceToReferenceSurface1; // 偏离基准面的距离
			OutData[4] = 1;													// 轴承轴线方向/轴承朝向
			// 2.圆锥滚针轴承数据
			OutData[5] = 1; // 轴承定位面
			OutData[6] = distFromBearingLocatingSurfaceToReferenceSurface2;
			OutData[7] = -1;
			foundSolution = true;
			// 其他圆锥滚针轴承参数（用于UI显示）
			OutData[8] = b.d;
			OutData[9] = b.D;
			OutData[10] = tmpLength;
			OutData[11] = b.B;
			OutData[12] = tmp_C;
			OutData[13] = b.Cr;
			OutData[14] = b.C0r;
			OutData[15] = tmp_e;
			OutData[16] = tmp_Y;
			OutData[17] = tmp_Yo;
			OutData[18] = tmp_a;
			// 角接触球轴承特殊参数（用于UI显示）
			if (tmpBearingType == AngularContactBallBearing)
			{
				OutData[11] = tmp_contactAngle;
			}
			OutData[20] = tmpBearingType;
			break; // 找到合适组合，停止搜索
		}
	}
	// 如果未找到合适的组合
	if (!foundSolution)
	{
		OutData[0] = -1;
		OutData[1] = -1;
		return;
	}
}
void AssemblyGearAndMainBearing(CShaftSystemTAN &TestShaftSystem, double *InGearData, double *InBearingData, double *OutData)
{
	gRVReducer.mCrankAxleSystem.mShaft.ClearCalculatorData(); // 清理缓存 ++!
	gRVReducer.Clear();
	int tmpBearingType = TaperedRollerBearing;
	if (InGearData[7] == 1)
		tmpBearingType = AngularContactBallBearing; // 判断是否为角接触球轴承

	gRVReducer.mFirstGearPair.CalForceVectorByRotAng(0);
	for (int i = 0; i < gRVReducer.mFirstGearPair.mGear.ShaftForceVector.size(); i++)
	{
		gRVReducer.mFirstGearPair.mGear.ShaftForceVector[i] = InGearData[i];
	}

	gRVReducer.mBearingList.emplace_back(CBearingTAN(tmpBearingType, new double[6]{InBearingData[0], InBearingData[1], InBearingData[2], InBearingData[3], InBearingData[4], InBearingData[5]}));
	gRVReducer.mBearingList.emplace_back(CBearingTAN(tmpBearingType, new double[6]{InBearingData[0], InBearingData[1], InBearingData[2], InBearingData[3], InBearingData[4], InBearingData[5]}));
	gRVReducer.mBearingList[0].DefineLocationPara(new double[6]{InBearingData[6], InBearingData[7], InBearingData[8], InBearingData[9], InBearingData[10], InBearingData[11]}); // 该定位尺寸等于28-(11.75-8.2)
	gRVReducer.mBearingList[1].DefineLocationPara(new double[6]{InBearingData[12], InBearingData[13], InBearingData[14], InBearingData[15], InBearingData[16], InBearingData[17]});
	// gRVReducer.mBearingList[1].DefineLocationPara(new double[6] {-1, 54, 1, 8.2, -1, 11.75});
	gRVReducer.mFirstGearPair.mGear.DefineLocationPara(new double[2]{InGearData[6], 0});
	std::vector<void *> TestAllComponent;
	std::vector<int> TestComponentType;
	TestAllComponent.emplace_back(static_cast<void *>(&gRVReducer.mFirstGearPair.mGear));
	TestComponentType.emplace_back(InvGearNotation);
	TestAllComponent.emplace_back(static_cast<void *>(&gRVReducer.mBearingList[0]));
	TestComponentType.emplace_back(tmpBearingType);
	TestAllComponent.emplace_back(static_cast<void *>(&gRVReducer.mBearingList[1]));
	TestComponentType.emplace_back(tmpBearingType);
	for (int i = 0; i < TestAllComponent.size(); i++)
	{
		TestShaftSystem.mComponentNumList.emplace_back(i);
	}
	TestShaftSystem.MapComponentListByNum(TestShaftSystem.mComponentNumList, TestComponentType, TestAllComponent); // 角接触球轴承的装配暂时不确定是正确。

	int const TestNodeCount = 2;
	double TestNodeEnd = InBearingData[17] * 3;
	if (InGearData[6] > TestNodeEnd)
		TestNodeEnd = InGearData[6] + InBearingData[17]; // 默认采用3倍轴承宽度作为轴长，如果齿轮位置超过这个长度，则以齿轮位置加轴承宽度作为轴长
	double TestNodeList[2]{0, TestNodeEnd};
	double TestShaftDirect[3]{0, 0, 1};
	int TestSecType[1]{1};
	double TestRC[1]{InBearingData[18] / 2.0};
	TestShaftSystem.mShaft.mEX = 2.1 * pow(10, 8);
	TestShaftSystem.mShaft.mPRXY = 0.3;
	TestShaftSystem.mShaft.DefineOrginGeometry(TestNodeCount, TestNodeList, TestShaftDirect, TestSecType, TestRC);
	TestShaftSystem.IsMainBearing = true;
	TestShaftSystem.CalShaftSystem();

	double maxBend = 0.0, maxShear = 0.0;
	int tmpSize = TestShaftSystem.mShaft.BendingNormalStress.size();

	for (int i = 0; i < tmpSize; i++)
	{
		maxBend = max(maxBend, std::abs(TestShaftSystem.mShaft.BendingNormalStress[i]));
		maxShear = max(maxShear, std::abs(TestShaftSystem.mShaft.ShearStress[i]));
	}
	OutData[0] = maxBend;
	OutData[1] = maxShear;
}

// 前向声明
static void ParseBasicLifeParameters(const double *InData, BearingParameters &bearing, OperatingConditions &cond);
static void ParseModifiedLifeParameters(const double *InData, BearingParameters &bearing, OperatingConditions &cond, EnvironmentParameters &env);
static void PackOutputData(const UnifiedBearingReport &report, const BearingParameters &bearing, const OperatingConditions &cond, double *OutData);

int CalCustomBearingLife(double *InData, double *OutData)
{
	try
	{
		BearingParameters bearing;
		OperatingConditions cond;
		EnvironmentParameters env;

		// 1. 结构化解耦解析
		ParseBasicLifeParameters(InData, bearing, cond);		 // 解析基础寿命/静载必需参数 (0..30)
		ParseModifiedLifeParameters(InData, bearing, cond, env); // 解析 ISO 修正寿命专有参数 (31..36)

		// 2. 调用核心计算
		UnifiedBearingReport report = BearingLifeCalculator::ExecuteUnifiedPipeline(bearing, cond, env);

		// 3. 打包输出 20 位结果
		PackOutputData(report, bearing, cond, OutData);

		return 0;
	}
	catch (const std::exception &e)
	{
		std::cerr << "[CalBearingLife 异常] " << e.what() << std::endl;
		OutData[0] = -1.0;
		return -1;
	}
	catch (...)
	{
		OutData[0] = -2.0;
		return -2;
	}
}
static void ParseBasicLifeParameters(const double *InData, BearingParameters &bearing, OperatingConditions &cond)
{
	// A1. 类型与形态
	int typeCode = static_cast<int>(InData[0]); // 轴承类型判断，输入值不同进行计算的规则不同
	// 深沟球InData[0]=0.0
	// 角接触InData[0]=1.0
	// 调心球InData[0]=2.0
	// 向心滚子大类InData[0]=3.0，
	// 向心滚子大类InData[0]=3.0，同样适用常规圆锥滚子轴承（需打开isTapered开关，修改InData[13]=1）
	// 向心滚子大类InData[0]=3.0，默认机制套圈滚针轴承（冲压滚针轴承请打开isDrawnCupNeedle计算开关InData[13]=2）
	// 向心滚子大类InData[0]=3.0，调心滚子轴承（需要打开isSphericalRoller 计算开关InData[13]=3）
	// 90度推力球InData[0]=4.0
	// 非90推力球InData[0]=5.0
	// 90度推力圆柱滚子轴承，推力滚针轴承InData[0]=6.0
	// 非90度推力滚子、非90度的止推调心球轴承（需要打开isSphericalRoller 计算开关，即修改InData[13]=3）InData[0]=7.0
	bearing.specificType = static_cast<BearingSpecificType>(typeCode);
	bearing.layout = static_cast<BearingLayoutConfiguration>(static_cast<int>(InData[1])); // 布置方式需令InData[1]=0
	bearing.assemblyCount = static_cast<int>(InData[2]);								   // 排列方式令InData[2]=1
	// A2. 基础几何与额定载荷
	bearing.C = InData[3];						// 额定动载荷
	bearing.C0 = InData[4];						// 额定静载荷
	bearing.Dpw = InData[5];					// 节圆直径
	bearing.Dw = InData[6];						// 滚动体直径
	bearing.alpha = InData[7];					// 接触角
	bearing.rows = static_cast<int>(InData[8]); // 列数默认InData[8]=1
	bearing.Z = static_cast<int>(InData[9]);	// 滚动个数
	bearing.bm = InData[10];					// 动载荷修正系数
	// 初始化内部开关为默认安全状态
	bearing.hasFillingSlot = false;
	bearing.isDrawnCupNeedle = false;
	bearing.isSphericalRoller = false;

	// A3. 解析特有几何与自动控制开关 (InData[11..16])
	switch (bearing.specificType)
	{
	case BearingSpecificType::DeepGrooveBall:
		bearing.hasFillingSlot = (InData[11] > 0.5); // 是否有装填槽
		break;

	case BearingSpecificType::RadialRoller:
	{
		bearing.L_total = InData[11];				// 滚子总长
		bearing.r = InData[12];						// 普通滚子倒角
		int subType = static_cast<int>(InData[13]); // 此处赋值值为1进行圆锥滚子轴承计算，2为冲压滚针计算，3为调心滚子计算

		if (subType == 1 || InData[14] > 0.0) // 圆锥滚子轴承
		{
			bearing.beta = InData[14]; // 圆锥角
			bearing.rl = InData[15];   // 大端倒角，圆锥滚子特有
			bearing.rs = InData[16];   // 小端倒角，圆锥滚子特有
		}
		else if (subType == 2) // 冲压外圈滚针轴承
		{
			bearing.isDrawnCupNeedle = true;
		}
		else if (subType == 3) // 向心调心滚子轴承
		{
			bearing.isSphericalRoller = true;
		}
		break;
	}

	case BearingSpecificType::ThrustRoller_90: // 90度推力滚子轴承，推力滚针轴承
		bearing.L_total = InData[11];
		bearing.r = InData[12];
		break;

	case BearingSpecificType::ThrustRoller_Angled: // 非90度推力滚子、非90度的止推调心球轴承（需要打开isSphericalRoller 计算开关，即修改InData[13]=3）
	{
		bearing.L_total = InData[11];
		bearing.r = InData[12];
		if (InData[13] > 0.5) // 止推调心标志
		{
			bearing.isSphericalRoller = true;
		}
		break;
	}

	default:
		break;
	}

	// 安全清洗：防止球轴承残留滚子几何参数
	bool isRoller = (bearing.specificType == BearingSpecificType::RadialRoller ||
					 bearing.specificType == BearingSpecificType::ThrustRoller_90 ||
					 bearing.specificType == BearingSpecificType::ThrustRoller_Angled);
	if (!isRoller)
	{
		bearing.L_total = 0.0;
		bearing.beta = 0.0;
		bearing.r = 0.0;
		bearing.rl = 0.0;
		bearing.rs = 0.0;
		bearing.isDrawnCupNeedle = false;
		bearing.isSphericalRoller = false;
	}
	else
	{
		bearing.hasFillingSlot = false;
	}

	// A4. Catalog 覆写通道 (InData[17..27])
	bearing.isCatalogOverride = (InData[17] > 0.5);
	bearing.cat_e = InData[18];
	bearing.cat_e0 = InData[19];
	bearing.cat_X1 = InData[20];
	bearing.cat_Y1 = InData[21];
	bearing.cat_X2 = InData[22];
	bearing.cat_Y2 = InData[23];
	bearing.cat_X01 = InData[24];
	bearing.cat_Y01 = InData[25];
	bearing.cat_X02 = InData[26];
	bearing.cat_Y02 = InData[27];

	// A5. 基础工况外载 (InData[28..30])
	cond.Fr = InData[28];
	cond.Fa = InData[29];
	cond.n = InData[30];
}

// 2.B修正寿命专有区块 (InData[31..36])可以不用管
static void ParseModifiedLifeParameters(const double *InData, BearingParameters &bearing, OperatingConditions &cond, EnvironmentParameters &env)
{
	bearing.Cu = InData[31];
	cond.v = InData[32];
	cond.contamLevel = static_cast<ContaminationLevel>(static_cast<int>(InData[33]));
	cond.lambda_film = InData[34];
	cond.hasEPAdditive = (InData[35] > 0.5);
	env.reliability = InData[36];
}

// 3. 组装输出数据 (固定 20 位 OutData)
static void PackOutputData(const UnifiedBearingReport &report,
						   const BearingParameters &bearing,
						   const OperatingConditions &cond,
						   double *OutData)
{
	OutData[0] = 1.0;			// 成功识别码
	OutData[1] = report.C0;		// 基本额定静载荷 C0 (N)
	OutData[2] = report.f0;		// 静负荷计算系数 f0
	OutData[3] = report.P0;		// 当量静载荷 P0 (N)
	OutData[4] = report.S0;		// 静态安全系数 S0
	OutData[5] = report.C;		// 基本额定动载荷 C (N)
	OutData[6] = report.X;		// 动载荷分配系数 X
	OutData[7] = report.Y;		// 动载荷分配系数 Y
	OutData[8] = report.P;		// 当量动载荷 P (N)
	OutData[9] = report.L10r;	// 基本额定寿命 L10r (10^6 转)
	OutData[10] = report.L10h;	// 基本额定寿命 L10h (小时)
	OutData[11] = report.kappa; // 黏度比 kappa
	OutData[12] = report.ec;	// 工况污染系数 ec
	OutData[13] = report.a1;	// 可靠度修正系数 a1
	OutData[14] = report.a_iso; // 系统修正系数 a_iso
	OutData[15] = report.Lnm;	// 综合修正额定寿命 Lnm (10^6 转)
	OutData[16] = bearing.Lwe;	// 滚子有效接触长度 Lwe (mm)
	OutData[17] = cond.Fa;		// 轴向外载荷 Fa (N)
	OutData[18] = cond.Fr;		// 径向外载荷 Fr (N)
	OutData[19] = cond.n;		// 运行转速 n (r/min)
}