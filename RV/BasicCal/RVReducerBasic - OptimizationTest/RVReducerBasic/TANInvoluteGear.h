#pragma once
#include "CTanVector.h"
#include "PreDefine.h"
#include "TANCubicSpline.h"
#include"TANMechanismFunction.h"

#define SteelSt 1//St正火低碳钢
#define SteelStCast 2//St[Cast]铸钢
#define SteelGTSperl 3//GTS[perl]黑心可锻铸铁（珠光体）
#define SteelGGGperl 4//GGG[perl]球墨铸铁(珠光体)
#define SteelGGGbai 5//GGG[bai]球墨铸铁(贝氏体)
#define SteelGGGferr 6//GGG[ferr]球墨铸铁(铁素体)
#define SteelGG 7//GG灰铸铁
#define SteelV 8//V整体淬硬锻钢 （碳钢，合金钢）整体淬火锻钢，淬透钢
#define SteelVCast 9//V整体淬硬铸钢 （碳钢，合金钢）整体淬火铸钢，淬透钢
#define SteelEh 10//Eh表面硬化铸钢
#define SteelIF 11//IF火焰或表面淬火铸或锻钢
#define SteelNTnitr 12//NTnitr 氮化钢
#define SteelNVnitr 13//NVnitr 渗氮硬化钢
#define SteelNVnitrcar 14//NVnitrcar 碳氮共渗钢
////add
////驱动齿轮
//#define Pinion 15//小齿轮驱动
//#define Gear 16//大齿轮驱动
////刀具类型
//#define RackCutter 17//齿条刀
//#define ShaperCutter 18//插齿刀
//
//#define NormalAngle 19 //不使用圆角
//#define RollAngle 20 //使用圆角
//1-一般外啮合，2-内啮合，太阳轮比行星轮，3-内啮合，行星轮比内齿圈,内齿圈固定，4-内啮合，行星轮比内齿圈,内齿圈转动
//1001-人字齿外啮合。。。。。。
#define ExternalMeshNotation 1
#define SunToPlanetMeshNotation 2
#define PlanetToFixedRingMeshNotation 3
#define PlanetToMovabledRingMeshNotation 4
#define DoubleHexlixMeshNotation 1001
//优化目标符号
#define ProfileShiftOptByBendStressNotation 0
#define ProfileShiftOptByContStressNotation 1
#define ToothNumOptNotationWithoutGCD 0//齿数分配时不考虑互质
#define ToothNumOptNotationWithGCD 1//齿数分配时考虑互质
//静态载荷时应力还未编写，准备直接进行载荷替换来进行计算。
namespace NSInvoluteStaticArrary
{
	extern double C1;
	extern double C2;
	extern double C3;
	extern double C4;
	extern double C5;
	extern double C6;
	extern double C7;
	extern double C8;
	extern double C9;
	extern double GearOil_v40[10];
	extern double GearOil_v50Coef[20];
	extern double GearOil_vfCoef[20];
	extern double GearStandardModuleFirst[28];//标准模数第一序列,有限序列
	extern double GearStandardModuleSecond[22];//标准模数第二序列,非优先
	extern double GearStandardModule[50];//所有标准模数值,大于等于1的所在模数索引在13，有37个元素
	//extern double c_thp;
}
class CInvoluteGearTAN
{
public:
	CInvoluteGearTAN();
	//外啮合时大轮齿数Z2为正，内啮合为负
	class CSingleInvoluteTAN
	{
	public:
#pragma region zhoujie add
		struct EISingleData
		{
			double d_a;
			double d_Fa;
			double d_fe;
			double d_Ff;
			double h_a;
			double h_f;
		};
		EISingleData mSingleData;
		EISingleData mESingleData;
		EISingleData mISingleData;
		double m_x;//轴向模数
		double p_z;//导程
		double d_y;//Y圆直径
		double beta_y;//Y圆螺旋角
		double gamma_y;//Y圆导程角
		double alpha_yt;//Y圆端面压力角
		double alpha_yn;//Y圆法向压力角
		double xi_y;//渐开线上Y点滚动角
		double rho_y;//渐开线曲率半径
		double invalpha_yt;//渐开线函数
		double tau;//齿距角
		double p_t;//端面齿距
		double p_n;//法向齿距
		double p_yt;//Y圆端面齿距
		double p_yn;//Y圆法向齿距
		double p_x;//轴向齿距
		double p_bt;//基圆端面齿距
		double p_et;//端面啮合齿距
		double p_en;//法向啮合齿距
		double d_v;//V圆直径
		double d_a;//齿顶圆直径
		double d_f;//齿根圆直径
		double s_yt;//Y圆端面齿厚
		double s_t;//分度圆端面齿厚
		double psi_y;//Y圆齿厚半角
		double psi;//分度圆齿厚半角
		double psi_b;//基圆齿厚半角
		double e_yt;//Y圆齿槽宽
		double e_t;//分度圆齿槽宽
		double eta_y;//Y圆齿槽宽半角
		double eta;//分度圆齿槽宽半角
		double eta_b;//基圆齿槽宽半角
		double s_yn;//Y圆法向齿厚
		double s_n;//分度圆法向齿厚
		double e_yn;//Y圆法向齿槽宽
		double e_n;//分度圆法向齿槽宽
		double h_aP0;// 刀具的齿顶高系数  需要给值！！
		int Cutter;//刀具类型：RackCutter 17 齿条刀  ShaperCutter 18 插齿刀
		int Angle;//是否使用圆角：NormalAngle 19 不使用圆角； RollAngle 20 使用圆角
		//插齿刀具参数 start
		double z0;//插齿刀齿数     需要给值！！
		double a0;//加工中心距
		double d_a0;//插齿刀具参数
		double d_b0;//基圆直径
		double d_Fa0;//齿顶形状直径
		double alpha_wt0;
		//插齿刀具参数 end
		double rho_aP0; //刀具的齿顶圆半径系数  需要给值！
		double c_F;
		double g_f;//齿条接触路径的长度
		double g_a;//齿条接触路径的长度
		double E_sns;//齿厚的偏差上限     需要给值！！
		double E_sni;//齿厚的偏差下限     需要给值！！
		double s_ns; //分度圆上的最大正常齿厚度
		double s_ni; //分度圆上的最小正常齿厚度
		double x_Es;//生成轮廓偏移系数
		double x_Ei;
		double q_max; //需要给值！！
		double q_min;//需要给值！！
		double x_EsV;
		double x_EiV;
		double T_sn, T_snv;//齿厚公差
		double k·mn;//齿轮齿顶高变化
		double Ada_upper; //齿顶圆容差
		double Ada_lower;
		double c; //理论顶隙
		double zeta_f;
		double zeta;
		double K_ga;
		double K_gf;
		double v_ga;
		double v_gf;
		double phi_j;
		double phi_gama;//总作用角
		double phi_beta;//纵向作用角
		double phi_alpha;//端面作用角
#pragma region zhoujie end
#pragma region 几何数据
		//几何计算基础数据
		int Z;
		double mn;//参考法向模数
		double alpha_n=20*Degree;//法向压力角
		double beta=0;//螺旋角
		int TorqueDirect = 1;//力矩旋向标识，每个齿轮由轮齿提供的力矩方向，1，指向右方，和z轴正向一致，-1指向左方，
		int SpiralIndex = -1;//旋向标识，-1左旋，1右旋
		double b_Design;//设计齿宽
		double b_eff;//计算弯曲应力时的有效齿宽,ISO6336-2019-3,p9的b解释
		double h_ax =1.0;//齿顶高系数
		double x =0;//高变位系数 
		double cx =0.25;//顶隙系数
		//可选输入
		double Rhf_fPx=0.38;//刀尖圆角系数
		double s_pr=0;//挖根量,mm
		double C_a=0;//齿顶修形量,um
		double C_f=0;//齿根修形量,um
		double Wk;//跨棒距,mm
		int k=2;//跨棒齿数
		double h_k=0;//齿顶倒角高度，暂时没用
		double D_i = 0;//轮缘内腔直径或内齿轮轮辐外圈，机械设计手册6版3卷15-147
		//其他几何数据
		double alpha_t;//端面压力角
		double alpha_Pn;
		double b_Work;//工作齿宽
		double h_fx;//齿根高系数
		double h_f;//齿根高
		double h_a;//齿顶高
		double h_FaP0;//齿条型刀具齿顶高，对应齿轮齿根
		double rho_fP;//刀尖圆角半径,mm		
		double d;//分度圆直径,mm
		double dm;//平均齿廓直径,mm
		double da;//齿顶圆高直径,mm
		double df;//齿根圆高直径,mm
		double dfE;//基于齿条加工的齿根圆
		double db;//基圆直径，mm
		double dw;//工作节圆直径
		double rm;
		double ra;//齿顶圆半径,mm
		double rf;//齿根圆半径,mm		
		double rb;
		double mt;//端面模数
		double h;//全齿高,mm
		double beta_b;//基圆螺旋角
		double X_E = 0;//切向变位系数？
		double invalpha_t;
		double h_fP;//基准齿条齿根高
		double h_aP;//基准齿条齿顶高
		//double h_FfP;//有效齿根高,未编
		double d_Ff;//渐开线起始齿根高
		double d_Fa;//成形齿高
		double d_Nf;
		double d_Na;//有效齿高
		int IOMeshIndex = 1;//内外啮合标识，1-外啮合，-1内啮合
		double DidmRatio;//内腔直径与平均直径比，机械设计手册6版3卷15-147
		double Rho;//接触点曲率半径
		//载荷数据
		double T;//力矩，Nm
		double F_t;
		double F_r;
		double F_a;
		double F_n;
		CTanVector ShaftForceVector;//根据转角定义力矢量
		double n;//转速，RPM
		double Lh;//设计寿命，小时
		double zn;//当量齿轮
		double C_B;
		//材料数据
		double Ex= 206000;//杨氏模量,N/mm^2
		double Prxy=0.3;//泊松比
		double sigma_s = 500;//屈服强度
		double sigma_Hlim = 1500.0;//试验齿轮的接触疲劳极限
		double sigma_Flim = 430.0; //试验齿轮的弯曲疲劳极限
		double sigma_B = 1080;//抗拉强度
		double sigma_02 = 500;// 弹性极限应力,残余变形达到0.2%的应力值，让sigma_02=sigma_s
		double Density = 7.83e-6;//密度kg/mm^3
		int MaterialType = SteelEh;//
		int MaterialQuality = 1;//用于计算Sigma_Flim及Sigma_Hlim//未编写这一部分，程序中实际没有用到
		int CaseHardness = 60;//表面硬度HRC
		int CoreHardness = 30;//芯部硬度HRC
		int IsShotPeen = 0;//是否喷丸
		int LimitedPittingAllowable = 0;//是否允许有限点蚀

		//动力学数据
		double EquivalentMass;//当量质量kg/mm，机械设计手册6版3卷15-147
		double J_r;//转动惯量
		//使用数据
		double K_A;
		double K_gama;
		int IsReverseBending = 0;
		int IsFavorableContactPostion = 1;
		//螺旋线修形类型，1)//不修形，2)//仅中心鼓形修形，3)//仅中心鼓形修形，4)//仅螺旋线正，5)//螺旋线修正加中心鼓形修形 6)//齿端修薄
		int HelixModificationType = 1;
		int IsProfileModification = 0;
		int N_m = 1;//啮合接触齿数
		int GearBlankType = 1;//1-实体齿轮
		//double BearingSpanOffset;//BearingSpanOffset
		//double f_sh;//等效错位量
		//double f_ma;//啮合错位量
		//double v_40;//润滑油粘度
		//精度数据
		int ToleranceClass = 5;
		double f_pt = 5.5;//端面节线偏差,um
		double f_falpha = 6.0;//齿廓偏差,um
		double f_Hbeta = 5.5;//螺旋线偏差,um
		double R_aH = 1.0;//表面粗糙度,um
		double R_aF = 3.0;//齿根粗糙度,um
		double f_pb;//基圆节线偏差,um
		double R_zH = 6.0;
		double R_zF;
		//无根切最小齿数和变位系数
		//double z_min = 0;
		double x_Emin = 0;
		double x_EMax = 1;
#pragma endregion 几何数据
		//应力数据
		double sigma_H;//接触应力
		double sigma_HP;//许用接触应力
		double sigma_HPStatic;//许用接触应力
		double S_Hmin = 1.0;//最小接触疲劳安全系数
		double S_Fmin = 1.0;//最小弯曲疲劳安全系数
		double S_H;//接触疲劳安全系数
		double S_HStatic;//接触疲劳安全系数
		double N_L;//载荷循环次数
		double Z_NT;
		double Z_NTStatic;
		double Z_W;
		double Z_WStatic;
		//弯曲应力数据
		double EE;
		double GG;
		double HH;
		double theta;
		double Y_F;		
		double Y_S;//未编写挖根的影响部分
		double Y_beta;
		double Y_B;
		double Y_DT;
		double sigma_F0;
		double sigma_F;
		double Y_ST;
		double Y_deltarelT;//齿根圆角敏感系数，其中有限寿命部分，ISO标准中没有，但是手册上有，未编写
		double Y_deltarelTStatic;
		double Y_RrefT;//齿根表面状况系数，其中有限寿命部分，ISO标准中没有，但是手册上有，未编写
		double Y_RrefTStatic;
		double Y_X;//尺寸系数
		double Y_XStatic;
		double Y_NT;//寿命系数
		double Y_NTStatic;
		double sigma_FP;
		double sigma_FPStatic;
		double S_F;//弯曲安全系数
		double S_FStatic;
		//double Y_Sa;//机械设计手册中的系数，未编写
		//当量齿轮数据
		double d_n;
		double p_bn;//基圆法向齿距
		double d_bn;
		double d_an;
		double d_en;
		double alpha_en;
		double gama_e;
		double alpha_Fen;
		double s_Fn;
		double rho_F;
		double h_Fe;
		double f_Xi;
		double s_R;//外啮合齿轮s_R>0.5ht，内啮合s_R>1.75mn
		int Iss_RTooSmall;//1,s_R太小，0s_R合适
		//安装参数
		int AssembleFaceType=0;//安装面定位面类型，1-外圈侧壁,左侧，-1-内圈侧壁，右侧	0-中点
		double AssembleDist=0;//安装面距离轴左侧距离
		//定义几何数据
		//InOtherSingleGearData[0]=Z;//齿数
		//InOtherSingleGearData[1]=SpiralIndex;//小轮旋向标识，-1左旋，1右旋
		//InOtherSingleGearData[2]=b_Design;//设计齿宽
		//InOtherSingleGearData[3]=h_ax;//齿顶高系数
		//InOtherSingleGearData[4]=x;//高变位系数 
		//InOtherSingleGearData[5]=cx;//顶隙系数
		//InOtherSingleGearData[6]=Rhf_fPx;
		//InOtherSingleGearData[7]=s_pr;
		//InOtherSingleGearData[8]=C_a;
		//InOtherSingleGearData[9]=C_f;
		////InOtherSingleGearData[10]=Wk;
		//InOtherSingleGearData[10]=X_E;
		//InOtherSingleGearData[11]=k;
		//InOtherSingleGearData[12]=h_k;
		//InOtherSingleGearData[13]=D_i;
		void DefineSingleGearGeometry(double Inmn, double Inalpha_n, double Inbeta, double* InOtherSingleGearData, CSingleInvoluteTAN& IOSingleGearObj);
		//单齿几何计算，目前只对标ISO标准完成计算
		void CalSingleGearBasicGeometry();
		//add
		void DefineSingleGearGeometryZJ(double Inmn, double Inalpha_n, double Inbeta, double* InOtherSingleGearData, CSingleInvoluteTAN& IOSingleGearObj);
		void CalSingleGearBasicGeometryZJ();
		double CalGearParad_FE(double InX_E);
		double CalGearParad_Ff(double InX_E);
		void CalEISingleData(double InX_E, double Ada, EISingleData& OutSingleData);
		//InPhysicalData[0]=Ex;//杨氏模量,N/mm^2
		//InPhysicalData[1] = Prxy;//泊松比
		//InPhysicalData[2] = sigma_s;//屈服强度
		//InPhysicalData[3] = Density;//密度kg/mm^3
		//sigma_Hlim = InPhysicalData[4];//试验齿轮的接触疲劳极限Mpa
		//sigma_Flim = InPhysicalData[5];//试验齿轮的完全疲劳极限Mpa
		//sigma_02 = InPhysicalData[6];
		//sigma_B = InPhysicalData[7];
		//InManufactureData[0]=MaterialType;
		//InManufactureData[1] = MaterialQuality;
		//InManufactureData[2] = CaseHardness;//表面硬度HRC
		//InManufactureData[3] = CoreHardness;//芯部硬度HRC
		//InManufactureData[4] = IsShotPeen;//是否喷丸
		//InManufactureData[5] = LimitedPittingAllowable;//是否允许有限点蚀	

		void DefineMaterial(double* InPhysicalData, double* InManufactureData);
		//IsReverseBending = static_cast<int>(InData[0]);
		//IsFavorableContactPostion = static_cast<int>(InData[1]);
		//HelixModificationType = static_cast<int>(InData[2]);
		//IsProfileModification = static_cast<int>(InData[3]);
		//N_m = static_cast<int>(InData[4]);//啮合接触齿数
		//GearBlankType = static_cast<int>(InData[5]);//1-实体齿轮
		//S_Hmin = InData[6];//接触疲劳设计最小安全系数
		//S_Fmin = InData[7];//弯曲疲劳设计最小安全系数
		void DefineApplicationData(double* InData);
		//精度计算，分两种方法，根据精度等级计算和根据输入值
		void DefineProfileTolerence(int InTolerenceClass);
		//TmpTolerenceClass = static_cast<int> (InInpuData[0]);
		//double& Tmpmn = InInpuData[1];
		//double& Tmpb_Design = InInpuData[2];
		//double& Tmpd = InInpuData[3];
		//double& Tempf_pt = OutValueList[0];
		//double& Tempf_falpha = OutValueList[1];
		//double& Tempf_Hbeta = OutValueList[2];
		static void DefineProfileTolerence(double* InInpuData, double* OutValueList);
		//f_pt = InTolerenceList[0];
		//f_alpha = InTolerenceList[1];
		//f_Hbeta = InTolerenceList[2];
		//R_aH = InTolerenceList[3];
		//R_aF = InTolerenceList[4];


		void DefineProfileTolerence(double* InTolerenceList);
		//计算导出精度数据
		void CalDerivedTolerence();
		void CalEquivalentMass();
		static double Finv(double InAlpha);
		//不根切的最小变位系数
		double CalMinProfileShift();
		//齿顶不变尖的最大变位系数
		double CalMaxProfileShift();
		//力计算
		void CalForce();
		//In_f_pb是齿轮副的偏差，为小齿轮和大齿轮偏差中的较大值
		//InV节线位置线速度
		//该函数仅检查一次
		double Get_y_alpha(double In_f_pb, double InV);
		//该函数仅检查一次
		double Get_y_beta(int Material, double F_beta_x, double InV);
		double get_x_beta(int Material);
		double Get_Rho(double InAlpha_wt);
		void Get_Z_NT();
		//外啮合的齿廓系数
		void Get_YF(double InXi_alpha, double InXi_beta,double InXi_alpha_n);//内啮合程序未编写
		void Get_YS();//存在挖根量tg时的程序未编写
		void Get_Ybeta(double InXi_beta);//存在挖根量tg时的程序未编写
		void Get_YB();
		void Get_YDT(double InXi_alpha_n);
		void Get_YdeltarefT();
		void Get_YRrefT();
		void Get_YX();
		void Get_YNT();
		//单齿弯曲应力计算
		void CalBendingStress(double InUnitiF,double InTotalK,double InXi_alpha, double InXi_beta, double InXi_alphan);
		void CalSafetyCoef();
		//void CalVirtualTooth(double InXi_alpha,double InXi_beta);
		//该函数目前只与轴的受力分解有关，与齿轮工作齿宽计算无关
		void DefineLocationPara(double InDistBetweenAssembleEdgeToShaftLeft, int InAssembleFaceType);
		void DefineLocationPara(double* InData);
		//根据渐开线函数反求对应角
		static double GetAlphaByInvFunctionValue(double InInvValue);
		private:
			double q_s;
			//齿顶不变尖辅助函数
			static double AuxiliaryFunctionForMaxProfileShift(double* InX, void** InGearObj);
			static double AuxiliaryFunctionForthetaInYF(double* InTheta, void**InEGH);			
			static double AuxiliaryFunctionForalphaByInvFunctionValue(double* InAlpha, void** InInvAlphaValue);
		
	};
	CSingleInvoluteTAN mPinion;
	CSingleInvoluteTAN mGear;
	//齿轮副类型;
	//1-一般外啮合，2-内啮合，太阳轮比行星轮，3-内啮合，行星轮比内齿圈,内齿圈固定，4-内啮合，行星轮比内齿圈,内齿圈转动
	//1001-人字齿外啮合。。。。。。
	int GearPairType = ExternalMeshNotation;
	double u;//齿数比
	int Tooth_Sum;//齿数和
	double mn;
	double alpha_n;
	double beta = 0;
	double a_x;//未变位时中心距
	double a_w = 0;//中心距离,mm
	double C_a;//齿顶修形量,um
	double mt;
	double b;//工作齿宽
	double alpha_t;//端面理论压力角
	double alpha_wt;//端面工作压力角
	double beta_b;
	double invalpha_t;
	double pn;//齿距
	double pt;//端面节距
	double pbt;
	double pet;
	double g_alpha;
	double Xi_alpha;
	double Xi_beta;
	double Xi_Gama;
	double Rho_red;//相对曲率半径
	int n_P = 1;//行星轮个数
	double x_Sum = 0;//变位系数和
	double y_t = 0;//中心距变动系数，机械设计手册第三卷P15-48,表15-1-25
	double y_n = 0;//中心距变动系数，机械设计手册第三卷P15-48,表15-1-25
#pragma region zhoujie add
	double i;
	int DrivingGear;//驱动齿轮:Pinion 15 小齿轮驱动;Gear 16大齿轮驱动
	double h_w;//
	double j_bn;// 触点间隙  需要给参数！
	double sigmax; //齿廓偏移系数之和
	double sigmax_E; //非零背差的齿廓偏移系数之和
	double T1C, rho_C1, T2C, rho_C2, T2A, rho_A2, T1E, rho_E1, T1B, rho_B1, T2D, rho_D2, T1T2;//齿面曲率半径
	double epsilon_alpha;//端面重合度
	double epsilon_beta;//纵向重合度
	double g_beta;//纵向重合弧长度
	double epsilon_gamma;//总重合度
	double l_max;//
	double j_wt;// 节圆处的周向间隙
	double j_t;// 分度圆处的圆周间隙
	double j_r;
	double n0;//主动轮转速
	double omega1;
	double v_n;
	double v_g;
	double g_alphay;
	double K_g;
	double rho_A1, rho_E2;
#pragma region zhoujie end
	//载荷数据
	double F_t=0;//切向载荷
	double F_r;//径向载荷
	double F_a;//轴向载荷
	double F_n;//法向载荷
	double F_m;//名义切向载荷
	double F_tH;//端平面内起决定作用的切向载荷
	//接触应力系数
	double Z_epsilon;
	double q_p;
	double c_thp;
	double C_M;
	double C_R;
	double alpha_Pn;
	double C_B;//基准齿廓系数
	double c_p;//单齿刚度
	double c_gamaalpha;//啮合刚度
	double c_gama_beta;//啮合刚度
	//动力学因数
	double m_red;//
	double n_E1;
	double N;
	double N_S;
	double Velocity;//节线线速度
	//使用系数
	double K_A = 1.25;
	double K_gama = 1.0;
	double K_v;
	double K_Hbeta;
	double K_Fbeta;
	double K_Halpha;
	double K_Falpha;
	double BearingSpanOffset = 0;
	//接触应力
	//名义接触应力部分
	double Z_H;	
	double Z_B;//小轮单对齿啮合系数
	double Z_D;//大轮单对齿啮合系数
	double Z_E;
	double Z_Xi;
	double Z_beta;	
	double sigma_H0;
	//许用接触应力部分
	double Z_L;//润滑系数
	double Z_LStatic;//润滑系数
	double Z_v;//速度系数
	double Z_vStatic;//速度系数
	double Z_R;//粗糙度系数
	double Z_RStatic;//粗糙度系数
	double Z_W;//表面硬化系数
	double Z_WStatic;
	double Z_X;//尺寸系数，IOS标准直接等于1
	int HardenPairType;//硬化配对类型
	//修形质量,针对斜齿轮,ISO标准6336-2-2019-Table3，1基于3维接触分析进行修形，最大接触区居中，应力分布基本均匀，2-基于加工人员经验进行的齿廓适当修形，3-未修形
	//相当于接触区控制，Favourable Contact Position参数 ISO6336-30:2022
	int HelixModificationQuality = 1;
	
	//int HelixModificationType;
	//int AssebleType;
	double f_sh;//等效错位量,应当通过计算求得,人字齿的未编
	double f_ma;//啮合错位量,应当通过计算求得
	double F_beta_y;
	double v_40 = 320;//润滑油粘度
	double v_50 = -2000;//润滑油粘度 50度测试
	//许用应力数据
	double sigma_Hlim;
	double S_Hmin = 1.0;
	double S_Fmin = 1.0;
	//弯曲应力部分
	double Xi_alphan;
	//double sigma_Flim;
	//安装参数；
	int AssembleType = 5;//安装类型,两轮悬臂
	int IsShaftStiffening = 0;//是否为刚性轴，d1/d_sh<1.15为非刚性轴，键连接齿轮也非刚性
	//int GearPairCount;//分流系数，有多少齿轮
	double k_p = -1;//结构系数
	double d_sh = 17;//小轮轴弯曲变形当量直径，对于圆轴，就是轴直径
	double PinionMeanPointToSpanMeanPointDistance = 0;//小轮齿宽中点至轴承跨距中点距离
	double BearingSpanLength = 34;//轴承跨距
	std::vector<CSingleInvoluteTAN*> mGearPtrList;


#pragma region 几何计算
	//单齿数据和DefineSingleGearGeometry中定义数列一致
	//InPairCommonData[0]=mn;//参考法向模数
	//InPairCommonData[1]=alpha_n;//法向压力角
	//InPairCommonData[2]=beta;//螺旋角
	//InPairCommonData[3]=a_w;//中心距
	//InPairCommonData[4]=GearPairType;//齿轮副类型
	//InPairCommonData[5]=n_P;//行星轮数目
	void DefineGearPairGeometry(double* InPinionData, double* InGearData, double* InPairCommonData, CInvoluteGearTAN& IOGearPair);
	void DefineGearPairGeometry(double* InPinionData, double* InGearData, double* InPairCommonData);
	void CalGearPairCommonGeomtryPara();
	//接触线参数计算
	void CalContactPathLength();
	//基本参数计算，不涉及中心距和变位系数的协调计算
	void CalGearPairGeometry();
	//根据变位系数计算中心距
	double CalCenterDistanceByProfileShift();
	//根据中心距计算需要的总变位系数
	double CalProfileShiftSumByCenterDistance();
	//add
	void DefineGearPairGeometryZJ(double* InPinionData, double* InGearData, double* InPairCommonData, CInvoluteGearTAN& IOGearPair);
	void DefineGearPairGeometryZJ(double* InPinionData, double* InGearData, double* InPairCommonData);
	void CalGearPairCommonGeomtryParaZJ();
	void CalGearPairGeometryZJ();
	void CalContactPathLengthZJ();
	void CalGearTransmissionRatio();
	void CalSurfaceCurvatureRadius();

#pragma endregion 几何计算
#pragma region 强度计算
#pragma region 系数计算
	//InTorqueIndex与InRPMIndex为表明载荷是大轮还是小轮的标识符，0为小轮，1为大轮
	void DefineLoadPara(double InTorque,//小轮输入力矩或大轮减速器输出力矩，可正可负
		double InRPM,
		double InDesignLife,
		bool InTorqueIndex,
		bool InRPMIndex);
	void DefineLoadPara(double InTorque,//小轮输入力矩或大轮减速器输出力矩，可正可负
		double InRPM,
		double InDesignLife,//以主动轮为基准的循环寿命
		bool InTorqueIndex,
		bool InRPMIndex,
		double InBigToSmallGearCountRatio);//被动轮个数比主动轮个数的比值
	void DefineGearPairMaterial(double* InPinionPhysicalData, double* InPinionManufactureData, double* InGearPhysicalData, double* InGearManufactureData);
	//K_A = InCommonData[0];
	//K_gama = InCommonData[1];
	//BearingSpanOffset = InCommonData[2];//BearingSpanOffset
	//v_40 = InCommonData[3];
	//HelixModificationQuality = static_cast<int>(InCommonData[4]);
	//S_Hmin = InCommonData[5];
	//S_Fmin = InCommonData[6];
	void DefineGearPairApplicationData(double* InPinionData, double* InGearData,double* InCommonData);
	void DefineGearPairTolerance( double* InPinionData, double* InGearData);
	//AssembleType = static_cast<int>(InData[0]);//安装类型
	//IsShaftStiffening = static_cast<int>(InData[1]);//是否为刚性轴，d1/d_sh<1.15为非刚性轴，键连接齿轮也非刚性
	//GearPairCount = static_cast<int>(InData[2]);//分流系数，有多少齿轮
	//d_sh = InData[3];//小轮轴弯曲变形当量直径，对于圆轴，就是轴直径
	//PinionMeanPointToSpanMeanPointDistance = InData[4];//小轮齿宽中点至轴承跨距中点距离
	//BearingSpanLength = InData[5];//小轮齿宽中点至轴承跨距中点距离
	void DefineGearPairAssembleData(double* InData);
	void CalBasicFactors();//基本系数计算
#pragma endregion 系数计算
#pragma region 接触强度，点蚀计算
	void CalSurfaceDurability();
#pragma endregion 接触强度计算
#pragma region 弯曲强度，点蚀计算
	void CalToothBendingStrength();
#pragma endregion
#pragma endregion 强度计算
#pragma region 参数推荐
public:
	//参数推荐时中间参数，机械设计手册第3卷8.3，15-142
	double fy_d_test = 0.4;//根据P15-143表15-1-86，悬臂布置得到
	double fy_a_test = 0;//齿宽系数
	double A_a_test = 483;//螺旋角为0时中心距修正系数
	double A_d_test = 766;//螺旋角为0时小轮直径修正系数
	double sigma_HP_test = 0;
	double K_Sum_test = 0;//估计的荷系数
	double a_w_min_test=0;//推荐的最小中心距
	double d_1_min_test = 0;//推荐的小轮最小直径
	//调整变位系数以便两齿轮等寿命
public:
	//int OptTargetNotation 0-以弯曲应力为基准优化，1-以接触应力为基准优化
	void OptimizeProfileShiftToEqualLife(int OptTargetNotation);
	//InMeshSign=1 如果为外啮合,=-1为内啮合
	void RecommondMainGeometryParaByContact(double InU,double InK,double InT1, double InPinionRPM, double InDesignLife ,double InsigmaHLim,int InMeshSign, int InN_p=1);
	//InMn法向模数，InA_w中心距，InN_p行星轮数
	void SetGeometryParaByMnAndDefaultPara(double InMn,double InA_w, int InN_p);
	//根据传动比和，初定中心距，模数确定齿数
	void OptimizeGearTooth(double InTranRatio, double InA_w, double InMn, int InToothNumOptNotation = ToothNumOptNotationWithoutGCD);
private:
	static double AuxiliaryFunctionForOptimizeProfileShiftByBending(double* InPinionx, void** InGearObj);
	static double AuxiliaryFunctionForOptimizeProfileShiftByContact(double* InPinionx, void** InGearObj);
#pragma endregion 参数推荐
#pragma region 优化时中间参数


#pragma endregion 优化时中间参数
	//接口函数，计算逻辑
public:
	void InterfaceCalGeometryAndStrength();
	//系统力计算
	void CalForceVectorByRotAng(double InAssembleAng);//基于当前安装相位角得到小轮和大力适量，安装相位角为连心在XY平面逆时针转动为正；
private:
	//几何相关系数
	double invalpha_wt = 0;

	//动载相关因数
	double Cv1, Cv2, Cv3, Cv4, Cv5, Cv6, Cv7;
	double f_pb, f_falpha, y_alpha,y_p, y_f;
	double f_pb_eff, f_falpha_eff;
	double Bp, Bf,Bk;
	//double f_sh, f_ma;
	void CalDynamicFactorKV();
	//基于ISO标准B法
	void CalFaceLoadFactorsKHbetaKFbeta();
	//基于ISO标准B法
	void CalFaceLoadFactorsKHalphaKFalpha();
//	void CalGearForceByDriveTorque()
	//接触应力相关因数
	void CalZBAndZD();
	void CalZXi();
	void CalZLZVZR();
	void CalZW();
	//小轮结构尺寸系数
	void Calf_sh();
	//void CalZX();//尺寸系数，机械设计手册6版3卷15-165上有计算公式，但未编写
	//void CalZNTByMethodB();
	double f_ZCa;

	

public:

	CCubicSplineTAN mOilV40ToV50Curve;
	CCubicSplineTAN mOilV40ToVfCurve;

};

