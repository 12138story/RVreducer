#include "BearingCalculators.h"
#include <cmath>
#include <stdexcept>
#include <algorithm>
#include <iostream>

const double DEG_TO_RAD = 3.14159265358979323846 / 180.0;

const std::vector<StaticFactorCalculator::F0Record> StaticFactorCalculator::table1_data =
    {
        {0.00, 14.7, 1.9, 61.6}, {0.01, 14.9, 2.0, 60.8}, {0.02, 15.1, 2.0, 59.9}, {0.03, 15.3, 2.1, 59.1}, {0.04, 15.5, 2.1, 58.3}, {0.05, 15.7, 2.1, 57.5}, {0.06, 15.9, 2.2, 56.7}, {0.07, 16.1, 2.2, 55.9}, {0.08, 16.3, 2.3, 55.1}, {0.09, 16.5, 2.3, 54.3}, {0.10, 16.4, 2.4, 53.5}, {0.11, 16.1, 2.4, 52.7}, {0.12, 15.9, 2.4, 51.9}, {0.13, 15.6, 2.5, 51.2}, {0.14, 15.4, 2.5, 50.4}, {0.15, 15.2, 2.6, 49.6}, {0.16, 14.9, 2.6, 48.8}, {0.17, 14.7, 2.7, 48.0}, {0.18, 14.4, 2.7, 47.3}, {0.19, 14.2, 2.8, 46.5}, {0.20, 14.0, 2.8, 45.7}, {0.21, 13.7, 2.8, 45.0}, {0.22, 13.5, 2.9, 44.2}, {0.23, 13.2, 2.9, 43.5}, {0.24, 13.0, 3.0, 42.7}, {0.25, 12.8, 3.0, 41.9}, {0.26, 12.5, 3.1, 41.2}, {0.27, 12.3, 3.1, 40.5}, {0.28, 12.1, 3.2, 39.7}, {0.29, 11.8, 3.2, 39.0}, {0.30, 11.6, 3.3, 38.2}, {0.31, 11.4, 3.3, 37.5}, {0.32, 11.2, 3.4, 36.8}, {0.33, 10.9, 3.4, 36.0}, {0.34, 10.7, 3.5, 35.3}, {0.35, 10.5, 3.5, 34.6}, {0.36, 10.3, 3.6, 34.6}, {0.37, 10.0, 3.6, 34.6}, {0.38, 9.8, 3.7, 34.6}, {0.39, 9.6, 3.8, 34.6}, {0.40, 9.4, 3.8, 34.6}};

double StaticFactorCalculator::LinearInterpolate(double x, double x0, double y0, double x1, double y1)
{
    if (x1 == x0)
        return y0;
    return y0 + (x - x0) * (y1 - y0) / (x1 - x0);
}

double StaticFactorCalculator::CalculateF0(const BearingParameters &bearing)
{
    if (bearing.f0 > 0.0)
        return bearing.f0;
    // 【核心自动分轨】识别到滚子轴承系列时，直接斩断逻辑返回 0.0，不实施任何 f0 查表演算
    if (bearing.specificType == BearingSpecificType::RadialRoller ||
        bearing.specificType == BearingSpecificType::ThrustRoller_90 ||
        bearing.specificType == BearingSpecificType::ThrustRoller_Angled)
    {
        return 0.0;
    }
    double alpha_rad = bearing.alpha * DEG_TO_RAD;
    double current_ratio = (bearing.Dpw <= 0.0) ? 0.0 : ((bearing.Dw * std::cos(alpha_rad)) / bearing.Dpw);
    double min_ratio = table1_data.front().ratio;
    double max_ratio = table1_data.back().ratio;
    current_ratio = std::max(min_ratio, std::min(current_ratio, max_ratio));
    bool is_thrust_track = (bearing.alpha > 45.0) || (bearing.specificType == BearingSpecificType::ThrustBall_90);
    double f0_val = 0.0;
    if (current_ratio <= min_ratio)
    {
        if (bearing.specificType == BearingSpecificType::SelfAligningBall)
            f0_val = table1_data.front().self_aligning;
        else if (is_thrust_track)
            f0_val = table1_data.front().thrust_ball;
        else
            f0_val = table1_data.front().radial_and_angular;
    }
    else if (current_ratio >= max_ratio)
    {
        if (bearing.specificType == BearingSpecificType::SelfAligningBall)
            f0_val = table1_data.back().self_aligning;
        else if (is_thrust_track)
            f0_val = table1_data.back().thrust_ball;
        else
            f0_val = table1_data.back().radial_and_angular;
    }
    else
    {
        for (size_t i = 0; i < table1_data.size() - 1; ++i)
        {
            if (current_ratio >= table1_data[i].ratio && current_ratio <= table1_data[i + 1].ratio)
            {
                double x0 = table1_data[i].ratio;
                double x1 = table1_data[i + 1].ratio;
                double y0 = 0.0, y1 = 0.0;

                if (bearing.specificType == BearingSpecificType::SelfAligningBall)
                {
                    y0 = table1_data[i].self_aligning;
                    y1 = table1_data[i + 1].self_aligning;
                }
                else if (is_thrust_track)
                {
                    y0 = table1_data[i].thrust_ball;
                    y1 = table1_data[i + 1].thrust_ball;
                }
                else
                {
                    y0 = table1_data[i].radial_and_angular;
                    y1 = table1_data[i + 1].radial_and_angular;
                }
                f0_val = LinearInterpolate(current_ratio, x0, y0, x1, y1);
                break;
            }
        }
    }
    return f0_val;
}

double StaticFactorCalculator::CalculateC0(const BearingParameters &bearing)
{
    double alpha_rad = bearing.alpha * DEG_TO_RAD;
    bool is_thrust_track =
        (bearing.alpha > 45.0) ||
        (bearing.specificType == BearingSpecificType::ThrustBall_90) ||
        (bearing.specificType == BearingSpecificType::ThrustBall_Angled) ||
        (bearing.specificType == BearingSpecificType::ThrustRoller_90) ||
        (bearing.specificType == BearingSpecificType::ThrustRoller_Angled);
    bool is_roller =
        (bearing.specificType == BearingSpecificType::RadialRoller ||
         bearing.specificType == BearingSpecificType::ThrustRoller_90 ||
         bearing.specificType == BearingSpecificType::ThrustRoller_Angled);
    if (!is_roller)
    {
        double f0 = CalculateF0(bearing);
        if (is_thrust_track)
        {
            return f0 * bearing.rows * bearing.Z * std::pow(bearing.Dw, 2) * std::sin(alpha_rad);
        }
        else
        {
            return f0 * bearing.rows * bearing.Z * std::pow(bearing.Dw, 2) * std::cos(alpha_rad);
        }
    }
    else
    {
        double current_ratio = (bearing.Dpw <= 0.0) ? 0.0 : ((bearing.Dw * std::cos(alpha_rad)) / bearing.Dpw);

        if (is_thrust_track)
        {
            return 220.0 * (1.0 - current_ratio) * bearing.rows * bearing.Z * bearing.Lwe * bearing.Dw * std::sin(alpha_rad);
        }
        else
        {
            return 44.0 * (1.0 - current_ratio) * bearing.rows * bearing.Z * bearing.Lwe * bearing.Dw * std::cos(alpha_rad);
        }
    }
}

double StaticFactorCalculator::CalculateP0(const BearingParameters &bearing, double Fr, double Fa)
{
    double abs_Fr = std::abs(Fr);
    double abs_Fa = std::abs(Fa);
    if (bearing.isCatalogOverride)
    {
        double ratio = (abs_Fr == 0.0) ? 99999.0 : (abs_Fa / abs_Fr);
        double X0 = 1.0;
        double Y0 = 0.0;

        if (ratio <= bearing.cat_e0)
        {
            X0 = bearing.cat_X01;
            Y0 = bearing.cat_Y01;
        }
        else
        {
            X0 = bearing.cat_X02;
            Y0 = bearing.cat_Y02;
        }
        double P0 = X0 * abs_Fr + Y0 * abs_Fa;
        if (bearing.rows == 1 && P0 < abs_Fr)
            P0 = abs_Fr;
        return P0;
    }
    double P0 = 0.0;
    double alpha_rad = bearing.alpha * DEG_TO_RAD;
    switch (bearing.specificType)
    {
    case BearingSpecificType::DeepGrooveBall:
    {
        P0 = 0.6 * abs_Fr + 0.5 * abs_Fa;
        if (P0 < abs_Fr)
            P0 = abs_Fr;
        break;
    }
    case BearingSpecificType::AngularContactBall:
    {
        double X0 = 0.5;
        double Y0 = 0.5;

        if (bearing.rows == 1)
        {
            X0 = 0.5;
            if (bearing.alpha <= 15.0)
                Y0 = 0.46;
            else if (bearing.alpha >= 40.0)
                Y0 = 0.26;
            else
                Y0 = 0.46 + (bearing.alpha - 15.0) * (0.26 - 0.46) / (40.0 - 15.0);

            P0 = X0 * abs_Fr + Y0 * abs_Fa;
            if (P0 < abs_Fr)
                P0 = abs_Fr;
        }
        else
        {
            X0 = 1.0;
            if (bearing.alpha <= 15.0)
                Y0 = 0.92;
            else if (bearing.alpha >= 40.0)
                Y0 = 0.52;
            else
                Y0 = 0.92 + (bearing.alpha - 15.0) * (0.52 - 0.92) / (40.0 - 15.0);

            P0 = X0 * abs_Fr + Y0 * abs_Fa;
        }
        break;
    }
    case BearingSpecificType::SelfAligningBall:
    {
        if (bearing.alpha == 0.0)
        {
            P0 = abs_Fr;
        }
        else
        {
            double cot_alpha = 1.0 / std::tan(alpha_rad);
            P0 = abs_Fr + 0.22 * cot_alpha * abs_Fa;
        }
        break;
    }
    case BearingSpecificType::RadialRoller:
    {
        if (bearing.alpha == 0.0)
        {
            P0 = abs_Fr;
        }
        else
        {
            double cot_alpha = 1.0 / std::tan(alpha_rad);
            if (bearing.rows == 1)
            {
                P0 = 0.5 * abs_Fr + 0.22 * cot_alpha * abs_Fa;
                if (P0 < abs_Fr)
                    P0 = abs_Fr;
            }
            else
            {
                P0 = abs_Fr + 0.44 * cot_alpha * abs_Fa;
            }
        }
        break;
    }
    case BearingSpecificType::ThrustBall_90:
    case BearingSpecificType::ThrustRoller_90:
        P0 = abs_Fa;
        break;

    case BearingSpecificType::ThrustBall_Angled:
    case BearingSpecificType::ThrustRoller_Angled:
        P0 = abs_Fa + 2.3 * abs_Fr * std::tan(alpha_rad);
        break;

    default:
        P0 = abs_Fr;
        break;
    }
    return P0;
}

double StaticFactorCalculator::CalculateS0(const BearingParameters &bearing, double Fr, double Fa)
{
    double current_c0 = bearing.C0;
    if (current_c0 <= 0.0)
    {
        current_c0 = CalculateC0(bearing);
    }
    double P0 = CalculateP0(bearing, Fr, Fa);
    // 0负荷极限下安全切轨防御
    if (P0 <= 0.0)
        return 99999.0;
    return current_c0 / P0;
}

// 在 BearingStaticFactors.cpp 中添加该函数的具体实现
void StaticFactorCalculator::ResolveGeometry(BearingParameters &bearing)
{
    // 1. 自动识别是否属于滚子轴承
    bool is_roller = (bearing.specificType == BearingSpecificType::RadialRoller ||
                      bearing.specificType == BearingSpecificType::ThrustRoller_90 ||
                      bearing.specificType == BearingSpecificType::ThrustRoller_Angled);
    // 2. 如果是滚子轴承，执行有效长度修正逻辑
    if (is_roller && bearing.L_total > 0.0)
    {
        double l_eff = bearing.L_total;
        double beta_rad = bearing.beta * DEG_TO_RAD;
        // 判断倒角类型并进行 ISO 标准扣除
        if (bearing.r > 0.0)
        {
            // 适用于对称倒角：总长扣除两端倒角
            l_eff = bearing.L_total - 2.0 * bearing.r;
        }
        else if (bearing.rl > 0.0 || bearing.rs > 0.0)
        {
            // 适用于圆锥滚子等非对称倒角
            l_eff = (bearing.L_total - (bearing.rl + bearing.rs)) / (cos(beta_rad / 2));
        }
        // 3. 防御性边界保护
        if (l_eff > 0.0)
        {
            // 将修正后的有效接触长度赋给 Lwe，不要覆盖 L_total
            bearing.Lwe = l_eff;
        }
        else
        {
            bearing.Lwe = bearing.L_total; // 兜底
        }
        std::cout << "[几何自动衍生] 检测到滚子轴承，有效接触长度(Lwe)已修正为: "
                  << bearing.Lwe << " mm" << std::endl;
    }
}
// 动态额定动载荷
double DynamicLoadRatingCalculator::CalculateDynamicLoadRating(const BearingParameters &bearing)
{
    // 1. 基础参数安全性校验边界
    if (bearing.Dw <= 0.0 || bearing.Z <= 0 || bearing.rows <= 0)
    {
        return 0.0;
    }
    double alpha_rad = bearing.alpha * 3.14159265358979323846 / 180.0;
    double cos_alpha = std::cos(alpha_rad);
    double sin_alpha = std::sin(alpha_rad);
    // 2. 物理智能分轨路由判定
    bool isRoller = (bearing.specificType == BearingSpecificType::RadialRoller ||
                     bearing.specificType == BearingSpecificType::ThrustRoller_90 ||
                     bearing.specificType == BearingSpecificType::ThrustRoller_Angled);

    bool isThrust = (bearing.alpha > 45.0) ||
                    (bearing.specificType == BearingSpecificType::ThrustBall_90) ||
                    (bearing.specificType == BearingSpecificType::ThrustBall_Angled) ||
                    (bearing.specificType == BearingSpecificType::ThrustRoller_90) ||
                    (bearing.specificType == BearingSpecificType::ThrustRoller_Angled);

    // 自动投影计算辅助几何参数 gamma
    double gamma = (std::abs(bearing.alpha - 90.0) < 1e-6) ? (bearing.Dw / bearing.Dpw) : (bearing.Dw * cos_alpha / bearing.Dpw);
    double bm = bearing.bm;
    if (bm <= 0.0)
    {
        if (!isRoller)
        {
            // 球轴承大类 (依据表1、表4)
            if (bearing.hasFillingSlot)
            {
                bm = 1.1; // 表1：有装填槽的轴承
            }
            else
            {
                bm = 1.3; // 表1、表4：常规向心及推力球轴承
            }
        }
        else
        {
            if (!isThrust)
            {
                // 向心滚子大类 (依据表6)
                if (bearing.isDrawnCupNeedle)
                {
                    bm = 1.0; // 表6：冲压外圈滚针轴承
                }
                else if (bearing.isSphericalRoller)
                {
                    bm = 1.15; // 表6：调心滚子轴承
                }
                else
                {
                    bm = 1.1; // 表6：圆柱滚子轴承、圆锥滚子轴承和机制套圈滚针轴承
                }
            }
            else
            {
                // --- 推力滚子大类 (依据表9) ---
                if (bearing.isSphericalRoller)
                {
                    bm = 1.15; // 表9：推力调心滚子轴承
                }
                else if (bearing.beta > 0.0 || bearing.rl > 0.0 || bearing.rs > 0.0 || std::abs(bearing.alpha - 90.0) > 1e-6)
                {
                    bm = 1.1; // 表9：推力圆锥滚子轴承
                }
                else
                {
                    bm = 1.0; // 表9：推力圆柱滚子轴承和推力滚针轴承
                }
            }
        }
    }
    // 分支 A：滚子轴承动载荷计算轨道 (依据 ISO 1281-1:2021)
    if (isRoller)
    {
        double lambda_nu = !isThrust ? 0.83 : 0.73;

        if (!isThrust)
        {
            // 向心滚子轴承计算基本额定径向动载荷 Cr (Section 6.5)
            double term_gamma = (1.0 - gamma) / (1.0 + gamma);
            double bracket = 1.0 + std::pow(1.04 * std::pow(term_gamma, 143.0 / 108.0), 9.0 / 2.0);

            double f_c = 551.13373 * 0.377 * lambda_nu * ((std::pow(gamma, 2.0 / 9.0) * std::pow(1.0 - gamma, 29.0 / 27.0)) / std::pow(1.0 + gamma, 1.0 / 4.0)) * std::pow(bracket, -2.0 / 9.0);

            double term_L = bearing.rows * bearing.Lwe * pow(cos_alpha, 1);
            return bm * f_c * std::pow(term_L, 7.0 / 9.0) * std::pow(bearing.Z, 3.0 / 4.0) * std::pow(bearing.Dw, 29.0 / 27.0);
        }
        else
        {
            // 推力滚子轴承计算基本额定轴向动载荷 Ca (Section 6.6 & 6.7)
            double eta = 1.0 - 0.15 * sin_alpha;

            if (std::abs(bearing.alpha - 90.0) < 1e-6)
            {
                // alpha = 90° 纯推力滚子轴承
                double gamma_90 = bearing.Dw / bearing.Dpw;
                double f_c = 472.45388 * lambda_nu * eta * std::pow(gamma_90, 2.0 / 9.0);
                double term_L = bearing.rows * bearing.Lwe;
                return bm * f_c * std::pow(term_L, 7.0 / 9.0) * std::pow(bearing.Z, 3.0 / 4.0) * std::pow(bearing.Dw, 29.0 / 27.0);
            }
            else
            {
                // alpha != 90° 角接触推力滚子轴承
                double term_gamma = (1.0 - gamma) / (1.0 + gamma);
                double bracket = 1.0 + std::pow(std::pow(term_gamma, 143.0 / 108.0), 9.0 / 2.0);

                double f_c = 551.13373 * lambda_nu * eta * (std::pow(gamma, 2.0 / 9.0) * std::pow(1.0 - gamma, 29.0 / 27.0)) / std::pow(1.0 + gamma, 1.0 / 4.0) * std::pow(bracket, -2.0 / 9.0);

                double term_L = bearing.rows * bearing.Lwe * cos_alpha;
                return bm * f_c * std::pow(term_L, 7.0 / 9.0) * std::tan(alpha_rad) * std::pow(bearing.Z, 3.0 / 4.0) * std::pow(bearing.Dw, 29.0 / 27.0);
            }
        }
    }
    // 分支 B：球轴承动载荷计算轨道 (依据 ISO 1281-1:2021)
    else
    {
        double ri = 0.52 * bearing.Dw;
        double re = 0.53 * bearing.Dw;
        if (bearing.specificType == BearingSpecificType::ThrustBall_90 || bearing.specificType == BearingSpecificType::ThrustBall_Angled)
        {
            ri = 0.535 * bearing.Dw;
            re = 0.535 * bearing.Dw;
        }
        else if (bearing.specificType == BearingSpecificType::SelfAligningBall)
        {
            ri = 0.53 * bearing.Dw;
            re = 0.5 * (1.0 + 1.0 / gamma) * bearing.Dw;
        }
        double term_ri = (2.0 * ri) / (2.0 * ri - bearing.Dw);
        double term_gamma_part = (std::pow(gamma, 0.3) * std::pow(1.0 - gamma, 1.39)) / std::pow(1.0 + gamma, 1.0 / 3.0);

        if (!isThrust)
        {
            // 向心球轴承 Cr 计算轨道
            double lambda = 0.95;
            if (bearing.specificType == BearingSpecificType::DeepGrooveBall && bearing.rows > 1)
                lambda = 0.90;
            else if (bearing.specificType == BearingSpecificType::SelfAligningBall)
                lambda = 1.0;
            double inner_term = 1.04 * std::pow((1.0 - gamma) / (1.0 + gamma), 1.72);
            // pow((params.ri / params.re) * (2 * params.re - params.Dw) / (2 * params.ri - params.Dw), 0.41)在Romax的双轨制下ri与re均为0.52Dw，导致该项恒为1，简化后得到1.04的常数项
            double bracket = 1.0 + std::pow(inner_term, 10.0 / 3.0);
            double f_c = 98.0665 * 0.41 * lambda * std::pow(term_ri, 0.41) * term_gamma_part * std::pow(bracket, -3.0 / 10.0);
            double size_factor = (bearing.Dw <= 25.4) ? std::pow(bearing.Dw, 1.8) : (3.647 * std::pow(bearing.Dw, 1.4));
            return bm * f_c * std::pow(bearing.rows * cos_alpha, 0.7) * std::pow(bearing.Z, 2.0 / 3.0) * size_factor;
        }
        else
        {
            // 推力球轴承 Ca 计算轨道
            double lambda = 0.90;
            double eta = 1.0 - sin_alpha / 3.0;
            double inner_term_non90 = std::pow((ri / re) * (2.0 * re - bearing.Dw) / (2.0 * ri - bearing.Dw), 0.41) * std::pow((1.0 - gamma) / (1.0 + gamma), 1.72);
            double inner_term_90 = std::pow((ri / re) * (2.0 * re - bearing.Dw) / (2.0 * ri - bearing.Dw), 0.41);
            double Ca_single = 0.0;
            double size_factor = (bearing.Dw <= 25.4) ? std::pow(bearing.Dw, 1.8) : (3.647 * std::pow(bearing.Dw, 1.4));
            if (std::abs(bearing.alpha - 90.0) < 1e-6)
            {
                double bracket_90 = 1.0 + std::pow(inner_term_90, 10.0 / 3.0);
                double f_c = 98.0665 * lambda * eta * std::pow(term_ri, 0.41) * std::pow(gamma, 0.3) * std::pow(bracket_90, -3.0 / 10.0);
                Ca_single = bm * f_c * std::pow(bearing.Z, 2.0 / 3.0) * size_factor;
            }
            else
            {
                double bracket_non90 = 1.0 + std::pow(inner_term_non90, 10.0 / 3.0);
                double f_c = 98.0665 * lambda * eta * std::pow(term_ri, 0.41) * term_gamma_part * std::pow(bracket_non90, -3.0 / 10.0);
                Ca_single = bm * f_c * std::pow(cos_alpha, 0.7) * std::tan(alpha_rad) * std::pow(bearing.Z, 2.0 / 3.0) * size_factor;
            }

            // 多列合成缩放安全钳位
            if (bearing.rows == 1)
            {
                return Ca_single;
            }
            else
            {
                const double exponent = 0.7;
                double sum_term = bearing.rows * std::pow(Ca_single, -exponent);
                return std::pow(sum_term, -3.0 / exponent);
            }
        }
    }
}
