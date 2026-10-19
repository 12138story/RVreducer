#ifndef BEARING_CALCULATORS_H
#define BEARING_CALCULATORS_H

#include <string>
#include <vector>
#include <utility>





// ����ö���Լ��ݾɰ�ӿڣ����ڲ���������ȫ�ϳ���������
enum class BearingType { BallBearing, RollerBearing };

enum class BearingLayoutConfiguration
{
    Single,   // �������
    DB,       // ���Ա���Back-to-Back���ɶ����� / ��˫������ʽ���
    DF,       // ����棨Face-to-Face���ɶ����� / ��˫������ʽ���
    DT        // ������Tandem���ɶ�����
};

// ���������ж�̬������������ṹ�壬�����ϲ�ҵ����չ
struct MultiRowDynamicLifeResult
{
    double X = 1.0;
    double Y = 0.0;
    double P = 0.0;
    double L10r = 0.0;  // ���������������ת��
    double L10h = 0.0;  // �����������Сʱ��
    double a_iso = 1.0; // ϵͳ��������ϵ��
    double Lnm = 0.0;   // �����ۺ����������������ת��
};


enum class BearingSpecificType
{
    DeepGrooveBall,
    AngularContactBall,
    SelfAligningBall,
    RadialRoller,
    ThrustBall_90,
    ThrustBall_Angled,
    ThrustRoller_90,
    ThrustRoller_Angled
};

enum class ContaminationLevel
{
    ExtremeClean, HighClean, NormalClean, Slight, Typical, Severe, VerySevere
};

struct BearingParameters
{
    BearingSpecificType specificType = BearingSpecificType::DeepGrooveBall;
    // ������̬�������Զ�ʶ��Դ
    BearingLayoutConfiguration layout = BearingLayoutConfiguration::Single;
    int assemblyCount = 1; // �����������/������ (Single��1���ɶ���2��������3)
    double C = 0.0;       // ��������غ� (N)
    double C0 = 0.0;      // ��������غ� (N)
    double Cu = 0.0;      // ƣ���غɼ��� (N) 
    double Dpw = 0.0;     // ��н�Բֱ�� (mm) 
    double Dw = 0.0;      // �����幫��ֱ�� (mm) 
    double alpha = 0.0;   // ���ƽӴ��� (��) 
    double beta = 0.0;    // Բ׶������е�Բ׶�ǣ��㣩     
    int rows = 1;         // �������������� (i) 
    int Z = 0;            // ÿ�й��������� 
    double Lwe = 0.0;     // ������Ч���� (mm)
    double f0 = 0.0;      // �����ɼ���ϵ�� f0
    // ��Ч���ӳ��ȼ���ȫ�Զ��Ƶ�����Դ 
    double L_total = 0.0;    // �����ܳ��� (mm)
    double r = 0.0;  // ͨ��/������ӣ��Գ����˵��ǰ뾶 (mm)
    double rl = 0.0; // Բ׶����/�ǶԳƣ���˵��ǰ뾶 (mm)
    double rs = 0.0; // Բ׶����/�ǶԳƣ�С�˵��ǰ뾶 (mm)
    //  �ϵ�� bm ��������нṹ������ʶ
    double bm = 0.0;                // ��ʽ��д bm ͨ��
    // ��Ӧ GB/T 6391-2010 ��1����6����9 �� bm ���ڲ���ľ�����нṹ��
    bool hasFillingSlot = false;    // �Ƿ�Ϊ����װ��۵�����С� (���� bm = 1.1)
    bool isDrawnCupNeedle = false;  // �Ƿ�Ϊ����ѹ��Ȧ������С� (���� bm = 1.0)
    bool isSphericalRoller = false; // �Ƿ�Ϊ������/�������Ĺ�����С� 
    // KISSsoft ��д��дͨ��
    bool isCatalogOverride = false;
    double cat_e = 0.0;
    double cat_e0 = 0.0;
    double cat_X1 = 1.0;  double cat_Y1 = 0.0;
    double cat_X2 = 1.0;  double cat_Y2 = 0.0;
    double cat_X01 = 1.0; double cat_Y01 = 0.0;
    double cat_X02 = 1.0; double cat_Y02 = 0.0;
};

struct OperatingConditions
{
    double Fr = 0.0;
    double Fa = 0.0;
    double n = 0.0;
    double v = 0.0;
    ContaminationLevel contamLevel = ContaminationLevel::NormalClean;
    // ��Ӧ 9.3.3.3.2: ��Ĥ���� Lambda (������ֵ > 0.0��������ʹ�ù�ʽ 30)
    double lambda_film = 0.0;
    // ��Ӧ 9.3.3.3.4: �Ƿ��о�֤ʵ�ļ�ѹ (EP) ���Ӽ�
    bool hasEPAdditive = false;
};

struct EnvironmentParameters
{
    double reliability = 90.0;
};
// �������ṹ��
struct UnifiedBearingReport
{
    // ��̬����� (GB/T 4662)
    double C0 = 0.0;
    double f0 = 0.0;
    double P0 = 0.0;
    double S0 = 0.0;

    // ��̬����� (GB/T 6391)
    double C = 0.0;
    double X = 1.0;
    double Y = 0.0;
    double P = 0.0;
    double L10r = 0.0;
    double L10h = 0.0;

    // ����������
    double kappa = 0.0;
    double ec = 0.0;
    double a1 = 1.0;
    double a_iso = 1.0;
    double Lnm = 0.0;
};
class BearingFactorService
{
public:
    static double GetReliabilityFactor(double reliabilityPercentage);
};

class ViscosityCalculator
{
public:
    static double CalculateReferenceViscosity(double n, double Dpw);
    static double CalculateViscosityRatio(double v, double v1);
};

class ContaminationFactorCalculator
{
public:
    //��¼ A ��������ʽ����
    static double Getec(ContaminationLevel level, double Dpw, double kappa);
 
};

class LoadFactorCalculator
{
private:
    static const std::vector<double> std_load_rows;
    static const std::vector<double> std_angles;

    struct Table3Cell
    {
        double e;
        double x_single_gt; double y_single_gt;
        double x_double_le; double y_double_le;
        double x_double_gt; double y_double_gt;
    };
    static const Table3Cell table3_grid[9][9];
    static double LinearInterpolate(double x, double x0, double y0, double x1, double y1);
public:
    static void CalculateXY(const BearingParameters& bearing, double Fr, double Fa, double& X, double& Y);
};

class AisoCalculator
{
public:
    static double CalculateAiso(const BearingParameters& bearing, double kappa, double ec, double Cu, double P);
};

class BearingLifeCalculator
{
public:
    static double CalculateEquivalentDynamicLoad(double X, double Fr, double Y, double Fa);
    static double CalculateBasicRatingLifeL10(const BearingParameters& bearing, double P);
    static double CalculateModifiedRatingLifeLnm(double L10, double a1, double a_iso);
    // ���ж�̬��������������
    static MultiRowDynamicLifeResult CalculateMultiRowDynamicLife
    (
        const BearingParameters& bearing,
        BearingLayoutConfiguration layout,
        int assemblyCount, // ��������/���� N (����ɶ��� 2�������� 3)
        const OperatingConditions& cond,
        const EnvironmentParameters& env
    );
    // �Զ�ʶ���Զ�������ˮ���������ȫ�Զ���ҵ�������
    static UnifiedBearingReport ExecuteUnifiedPipeline
    (
        BearingParameters& bearing,
        const OperatingConditions& cond,
        const EnvironmentParameters& env
    );

    static void PrintReport(const BearingParameters& bearing, const OperatingConditions& cond, const UnifiedBearingReport& report);
};


class StaticFactorCalculator
{
private:
    struct F0Record
    {
        double ratio;
        double radial_and_angular;
        double self_aligning;
        double thrust_ball;
    };
    static const std::vector<F0Record> table1_data;
    static double LinearInterpolate(double x, double x0, double y0, double x1, double y1);
public:
    // ȫ�Զ�����������������
    static void ResolveGeometry(BearingParameters& bearing);
    static double CalculateF0(const BearingParameters& bearing);
    static double CalculateC0(const BearingParameters& bearing);
    static double CalculateP0(const BearingParameters& bearing, double Fr, double Fa);
    static double CalculateS0(const BearingParameters& bearing, double Fr, double Fa);

};
class DynamicLoadRatingCalculator
{
public:
    // ������д��ࡢ�Ƕ���Ӵ����Σ�ȫ�Զ�����ISO��׼����غ�(Cr��Ca)
    static double CalculateDynamicLoadRating(const BearingParameters& bearing);
    int CalUnifiedBearingLife(double* InData, double* OutData);

};

#endif