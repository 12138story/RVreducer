#ifndef ISO_6336H1_H
#define ISO_6336H1_H

#include<iostream>
#include<math.h>
#define PI 3.14159265
#define e 2.71828
#define max(a,b) (((a) > (b)) ? a : b)
#define min(a,b) (((a) < (b)) ? a : b)
using namespace std;


//输入参数
int GearType = 2;//齿形类型选择：1.直齿轮；2.斜齿轮；3.锥齿轮
int SelofMaterial_1 = 10;
int SelofMaterial_2 = SelofMaterial_1;//材料选择
int Selof_ZCa = 1;//f_ZCa类型选择
int Beta_Shape = 1;//修形类型选择
double z1 = 17.0, z2 = 103.0;//齿数
double alpha_n = 20.0;//法向压力角
double beta = 15.8;//螺旋角
double m = 8.0;
double b1 = 100.0, b2 = 100.0;//齿轮齿宽
double a_w = 500.0;//中心距
double x1 = 0.145, x2 = 0.;//变位系数
double W_k1 = 38.196, W_k2 = 307.943;//跨度测量
double k1 = 2.0, k2 = 13.0;//跨越的齿数
double d_a1 = 159.66, d_a2 = 872.35;//齿顶圆直径
double s_pr1 = 0., s_pr2 = 0.;
double q1 = 0., q2 = 0.;//精加工的材料余量
double C_a = 70.0;//齿端修正，单位μm
double Q = 5.0;//ISO精度等级
double f_pt1 = 8.0, f_pt2 = 9.5;//单个齿距偏差，单位um
double f_falpha1 = 10.0, f_falpha2 = 12.0;//齿轮齿廓形状偏差	
double f_Hbeta1 = 8.5, f_Hbeta2 = 9.5;//齿轮螺旋线倾斜偏差
double R_aH1 = 1.0, R_aH2 = 1.0;//表面粗糙度
double R_aF1 = 3.0, R_aF2 = 3.0;//圆角粗糙度
double Case_h1 = 60.0, Case_h2 = 60.0;//表面硬度（HRC）
double Core_h1 = 30.0, Core_h2 = 30.0; //内部硬度（HRC）
double E1 = 206000.0, E2 = 206000.0;//弹性模量
double v1 = 0.3, v2 = 0.3;//泊松比
double sigma_S1 = 500.0, sigma_S2 = 500.0;//屈服/极限应力
double sigma_Hlim = 1500.0;//试验齿轮的接触疲劳极限
double sigma_Flim = 500.0; //试验齿轮的弯曲疲劳极限
double S_Hmin = 1.0;//接触疲劳最小安全系数
double S_Fmin = 1.0;//齿根应力所需的最小安全系数
double N_M1 = 1.0, N_M2 = 1.0;//网格接触数量
double s1 = 0.;//轴承跨度偏移
double f_sh_1 = 0.;//由小齿轮和大齿轮的轴变形引起的当量螺旋线偏齿分量
double f_ma_1 = 0.;//由制造偏差引起的螺旋线偏齿分量
double v_40 = 320.0; //在40℃处的名义粘度,单位cSt
double T1 = 9000.0;//扭矩
double n1 = 360.0;//转速
double Lh = 50000.0;//寿命
double g_alpha = 37.84464;//接触线长度
double C_M = 0.8;//理论修正系数
double C_R = 1.0;//轮体结构系数
double C1 = 0.04723, C2 = 0.15551, C3 = 0.25791, C4 = -0.00635, C5 = -0.11654, C6 = -0.00193, C7 = -0.24188, C8 = 0.00529, C9 = 0.00182;
double rho1 = 0.00000783, rho2 = 0.00000783;//密度

//求解参数
double Q1, Q2;//精度等级
double m_n;//法向模数
double m_t;//端面模数
double u;//齿数比
string Material_1, Material_2;
double b;//工作齿宽
double h_fP1, h_fP2;//渐开线圆柱齿轮基本齿条齿根高
double rho_fP1, rho_fP2;// 渐开线圆柱齿轮基本齿条的齿根圆角半径
double d1, d2, d_f1, d_f2, d_b1, d_b2, d_m1, d_m2, d_w1, d_w2, d_Nf1, d_Nf2, d_n1, d_n2, d_bn1, d_bn2, d_an1, d_an2, d_en1, d_en2;
double alpha_t, alpha_wt, alpha_en1, alpha_en2, alpha_Fen1, alpha_Fen2, gama_e1, gama_e2;
double invalpha_n, invalpha_t, invalpha_en1, invalpha_en2;
double X_E1,X_E2;
double beta_b ;//基圆螺旋角
double p_n, p_t, p_bt, p_et, p_bn1, p_bn2;
double epsilon_alpha, epsilon_beta, epsilon_gama,epsilon_alphan;
double f_pb1, f_pb2;
double v ;//分度圆切向速度
double v_w;
double F_t, F_m, F_tH;
double N_L1, N_L2;//载荷循环系数
double R_zH1, R_zH2;//峰—谷粗糙度
double R_zH;//齿轮副的平均峰—谷粗糙度
double p1, p2;
double p_red ;//综合曲率半径
double HB1 , HB2 ;
double alpha_pn = alpha_n;//圆柱齿轮基本齿条法向压力角
double C_B1 ;//小轮基本齿条系数
double C_B2 ;//大轮基本齿条系数
double C_B ;
double z_n1, z_n2;
double L1, L2;
double q_1, q_s1, q_s2;
double y_alpha, y_p, y_f, y_beta;
double X_beta;
double M1, M2;
double R_zH10;
double C_ZL, C_Zv, C_ZR;
double Cv1,Cv2, Cv3, Cv4, Cv5, Cv6, Cv7;
double c_1, c_gama_alpha,c_gama_beta, cth_1;
double Z_H, Z_E, Z_epsilon, Z_beta, Z_NT1, Z_NT2, Z_L, Z_R, Z_v, Z_X, Z_W, Z_B, Z_D;
double B1, B2, Bp, Bf, Bk;
double K, K_v, K_Hbeta, K_Fbeta, K_Falpha, K_Halpha, K_A = 1.0, K_gama = 1.0;
double sigma_H0, sigma_H1, sigma_H2, sigma_HP1, sigma_HP2, sigma_F01, sigma_F02, sigma_F1, sigma_F2, sigma_FP1, sigma_FP2;
double Y_F1, Y_F2, Y_S1, Y_S2, Y_beta, Y_DT, Y_B1, Y_B2, Y_NT1, Y_NT2, Y_ST1, Y_ST2, Y_deltarelT1, Y_deltarelT2, Y_RrelT1, Y_RrelT2, Y_X1, Y_X2;
double S_H1, S_H2, S_F1, S_F2;
double f_ZCa,f_epsilon, f_pb_eff, f_falpha_eff, f_pb, f_falpha, f_sh, f_ma;
double F_beta_x, F_beta_y;
double N_S, N, N_F;
double m_red;
double n_E1;
double b_h;
double s_Fn1, s_Fn2, h_Fe1, h_Fe2;
double rho_fPv1, rho_fPv2, rho_F1, rho_F2,rho1_1, rho2_1;
#endif