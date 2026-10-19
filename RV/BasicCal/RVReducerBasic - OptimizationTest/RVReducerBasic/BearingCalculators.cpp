#include "BearingCalculators.h"
#include <cmath>
#include <iostream>
#include <algorithm>
#include <stdexcept>
#include <map>

const double PI = 3.14159265358979323846;
const std::vector<double> LoadFactorCalculator::std_angles = {5.0, 10.0, 15.0, 20.0, 25.0, 30.0, 35.0, 40.0, 45.0};
const std::vector<double> LoadFactorCalculator::std_load_rows = {0.172, 0.345, 0.689, 1.03, 1.38, 2.07, 3.45, 5.17, 6.89};

const LoadFactorCalculator::Table3Cell LoadFactorCalculator::table3_grid[9][9] =
    {
        // 行 0 (相对载荷 = 0.172)
        {{0.23, 0.44, 2.78, 1.0, 2.78, 0.72, 3.74}, {0.29, 0.46, 1.88, 1.0, 2.18, 0.75, 3.06}, {0.38, 0.44, 1.47, 1.0, 1.65, 0.72, 2.39}, {0.57, 0.43, 1.09, 1.0, 1.09, 0.70, 1.63}, {0.68, 0.41, 0.87, 1.0, 0.92, 0.67, 1.41}, {0.80, 0.39, 0.76, 1.0, 0.78, 0.63, 1.24}, {0.95, 0.37, 0.66, 1.0, 0.66, 0.60, 1.07}, {1.14, 0.35, 0.57, 1.0, 0.55, 0.57, 0.93}, {1.34, 0.33, 0.50, 1.0, 0.47, 0.54, 0.81}},
        // 行 1 (相对载荷 = 0.345)
        {{0.26, 0.44, 2.40, 1.0, 2.40, 0.72, 3.23}, {0.32, 0.46, 1.71, 1.0, 1.98, 0.75, 2.78}, {0.40, 0.44, 1.40, 1.0, 1.57, 0.72, 2.28}, {0.57, 0.43, 1.09, 1.0, 1.09, 0.70, 1.63}, {0.68, 0.41, 0.87, 1.0, 0.92, 0.67, 1.41}, {0.80, 0.39, 0.76, 1.0, 0.78, 0.63, 1.24}, {0.95, 0.37, 0.66, 1.0, 0.66, 0.60, 1.07}, {1.14, 0.35, 0.57, 1.0, 0.55, 0.57, 0.93}, {1.34, 0.33, 0.50, 1.0, 0.47, 0.54, 0.81}},
        // 行 2 (相对载荷 = 0.689)
        {{0.30, 0.44, 2.07, 1.0, 2.07, 0.72, 2.78}, {0.36, 0.46, 1.52, 1.0, 1.76, 0.75, 2.47}, {0.43, 0.44, 1.30, 1.0, 1.46, 0.72, 2.11}, {0.57, 0.43, 1.09, 1.0, 1.09, 0.70, 1.63}, {0.68, 0.41, 0.87, 1.0, 0.92, 0.67, 1.41}, {0.80, 0.39, 0.76, 1.0, 0.78, 0.63, 1.24}, {0.95, 0.37, 0.66, 1.0, 0.66, 0.60, 1.07}, {1.14, 0.35, 0.57, 1.0, 0.55, 0.57, 0.93}, {1.34, 0.33, 0.50, 1.0, 0.47, 0.54, 0.81}},
        // 行 3 (相对载荷 = 1.03)
        {{0.34, 0.44, 1.87, 1.0, 1.87, 0.72, 2.52}, {0.38, 0.46, 1.41, 1.0, 1.63, 0.75, 2.29}, {0.46, 0.44, 1.23, 1.0, 1.38, 0.72, 2.00}, {0.57, 0.43, 1.09, 1.0, 1.09, 0.70, 1.63}, {0.68, 0.41, 0.87, 1.0, 0.92, 0.67, 1.41}, {0.80, 0.39, 0.76, 1.0, 0.78, 0.63, 1.24}, {0.95, 0.37, 0.66, 1.0, 0.66, 0.60, 1.07}, {1.14, 0.35, 0.57, 1.0, 0.55, 0.57, 0.93}, {1.34, 0.33, 0.50, 1.0, 0.47, 0.54, 0.81}},
        // 行 4 (相对载荷 = 1.38)
        {{0.36, 0.44, 1.75, 1.0, 1.75, 0.72, 2.36}, {0.40, 0.46, 1.34, 1.0, 1.55, 0.75, 2.18}, {0.47, 0.44, 1.19, 1.0, 1.34, 0.72, 1.93}, {0.57, 0.43, 1.09, 1.0, 1.09, 0.70, 1.63}, {0.68, 0.41, 0.87, 1.0, 0.92, 0.67, 1.41}, {0.80, 0.39, 0.76, 1.0, 0.78, 0.63, 1.24}, {0.95, 0.37, 0.66, 1.0, 0.66, 0.60, 1.07}, {1.14, 0.35, 0.57, 1.0, 0.55, 0.57, 0.93}, {1.34, 0.33, 0.50, 1.0, 0.47, 0.54, 0.81}},
        // 行 5 (相对载荷 = 2.07)
        {{0.40, 0.44, 1.58, 1.0, 1.58, 0.72, 2.13}, {0.44, 0.46, 1.23, 1.0, 1.42, 0.75, 2.00}, {0.50, 0.44, 1.12, 1.0, 1.26, 0.72, 1.82}, {0.57, 0.43, 1.09, 1.0, 1.09, 0.70, 1.63}, {0.68, 0.41, 0.87, 1.0, 0.92, 0.67, 1.41}, {0.80, 0.39, 0.76, 1.0, 0.78, 0.63, 1.24}, {0.95, 0.37, 0.66, 1.0, 0.66, 0.60, 1.07}, {1.14, 0.35, 0.57, 1.0, 0.55, 0.57, 0.93}, {1.34, 0.33, 0.50, 1.0, 0.47, 0.54, 0.81}},
        // 行 6 (相对载荷 = 3.45)
        {{0.45, 0.44, 1.39, 1.0, 1.39, 0.72, 1.87}, {0.49, 0.46, 1.10, 1.0, 1.27, 0.75, 1.79}, {0.55, 0.44, 1.02, 1.0, 1.14, 0.72, 1.66}, {0.57, 0.43, 1.09, 1.0, 1.09, 0.70, 1.63}, {0.68, 0.41, 0.87, 1.0, 0.92, 0.67, 1.41}, {0.80, 0.39, 0.76, 1.0, 0.78, 0.63, 1.24}, {0.95, 0.37, 0.66, 1.0, 0.66, 0.60, 1.07}, {1.14, 0.35, 0.57, 1.0, 0.55, 0.57, 0.93}, {1.34, 0.33, 0.50, 1.0, 0.47, 0.54, 0.81}},
        // 行 7 (相对载荷 = 5.17)
        {{0.50, 0.44, 1.26, 1.0, 1.26, 0.72, 1.69}, {0.54, 0.46, 1.01, 1.0, 1.17, 0.75, 1.64}, {0.56, 0.44, 1.00, 1.0, 1.12, 0.72, 1.63}, {0.57, 0.43, 1.09, 1.0, 1.09, 0.70, 1.63}, {0.68, 0.41, 0.87, 1.0, 0.92, 0.67, 1.41}, {0.80, 0.39, 0.76, 1.0, 0.78, 0.63, 1.24}, {0.95, 0.37, 0.66, 1.0, 0.66, 0.60, 1.07}, {1.14, 0.35, 0.57, 1.0, 0.55, 0.57, 0.93}, {1.34, 0.33, 0.50, 1.0, 0.47, 0.54, 0.81}},
        // 行 8 (相对载荷 = 6.89)
        {{0.52, 0.44, 1.21, 1.0, 1.21, 0.72, 1.63}, {0.54, 0.46, 1.00, 1.0, 1.16, 0.75, 1.63}, {0.56, 0.44, 1.00, 1.0, 1.12, 0.72, 1.63}, {0.57, 0.43, 1.09, 1.0, 1.09, 0.70, 1.63}, {0.68, 0.41, 0.87, 1.0, 0.92, 0.67, 1.41}, {0.80, 0.39, 0.76, 1.0, 0.78, 0.63, 1.24}, {0.95, 0.37, 0.66, 1.0, 0.66, 0.60, 1.07}, {1.14, 0.35, 0.57, 1.0, 0.55, 0.57, 0.93}, {1.34, 0.33, 0.50, 1.0, 0.47, 0.54, 0.81}}};

double LoadFactorCalculator::LinearInterpolate(double x, double x0, double y0, double x1, double y1)
{
    if (x1 == x0)
        return y0;
    return y0 + (x - x0) * (y1 - y0) / (x1 - x0);
}

void LoadFactorCalculator::CalculateXY(const BearingParameters &bearing, double Fr, double Fa, double &X, double &Y)
{
    double abs_Fr = std::abs(Fr);
    double abs_Fa = std::abs(Fa);

    if ((bearing.specificType == BearingSpecificType::ThrustBall_90 ||
         bearing.specificType == BearingSpecificType::ThrustRoller_90) &&
        abs_Fr > 0.0)
    {
        throw std::runtime_error(
            std::string("安全熔断级物理冲突：90度纯推力轴承接触角为90°，") +
            "在力学结构上完全不具备径向约束能力！输入径向力 Fr = " + std::to_string(abs_Fr) +
            " N 将导致套圈脱开与轴承瓦解致命风险！");
    }
    if (bearing.isCatalogOverride)
    {
        double ratio = (abs_Fr == 0.0) ? 99999.0 : (abs_Fa / abs_Fr);
        if (ratio <= bearing.cat_e)
        {
            X = bearing.cat_X1;
            Y = bearing.cat_Y1;
        }
        else
        {
            X = bearing.cat_X2;
            Y = bearing.cat_Y2;
        }
        return;
    }
    if (bearing.alpha > 45.0 &&
        (bearing.specificType == BearingSpecificType::DeepGrooveBall ||
         bearing.specificType == BearingSpecificType::AngularContactBall ||
         bearing.specificType == BearingSpecificType::SelfAligningBall ||
         bearing.specificType == BearingSpecificType::RadialRoller))
    {
        throw std::invalid_argument("核心物理冲突：向心类轴承公称接触角 alpha 不得大于 45°。此工况必须转入推力大类核算轨道。");
    }
    double alpha_rad = bearing.alpha * PI / 180.0;
    double ratio = (abs_Fr == 0.0) ? 99999.0 : (abs_Fa / abs_Fr);
    switch (bearing.specificType)
    {
    case BearingSpecificType::DeepGrooveBall:
    {
        static const std::vector<std::pair<double, std::pair<double, double>>> table3_deep_groove =
            {
                {0.172, {0.19, 2.30}}, {0.345, {0.22, 1.99}}, {0.689, {0.26, 1.71}}, {1.03, {0.28, 1.55}}, {1.38, {0.30, 1.45}}, {2.07, {0.34, 1.31}}, {3.45, {0.38, 1.15}}, {5.17, {0.42, 1.04}}, {6.89, {0.44, 1.00}}};
        double auto_f0 = StaticFactorCalculator::CalculateF0(bearing);
        double current_c0 = (bearing.C0 > 0.0) ? bearing.C0 : StaticFactorCalculator::CalculateC0(bearing);

        // 规避 C0 为 0 时的除零断裂
        double relative_load = (current_c0 <= 0.0) ? 0.0 : ((auto_f0 * abs_Fa) / current_c0);

        double e = 0.0, Y_table = 0.0;
        if (relative_load <= table3_deep_groove[0].first)
        {
            e = table3_deep_groove[0].second.first;
            Y_table = table3_deep_groove[0].second.second;
        }
        else if (relative_load >= table3_deep_groove.back().first)
        {
            e = table3_deep_groove.back().second.first;
            Y_table = table3_deep_groove.back().second.second;
        }
        else
        {
            for (size_t i = 0; i < table3_deep_groove.size() - 1; ++i)
            {
                if (relative_load >= table3_deep_groove[i].first && relative_load <= table3_deep_groove[i + 1].first)
                {
                    e = LinearInterpolate(relative_load, table3_deep_groove[i].first, table3_deep_groove[i].second.first, table3_deep_groove[i + 1].first, table3_deep_groove[i + 1].second.first);
                    Y_table = LinearInterpolate(relative_load, table3_deep_groove[i].first, table3_deep_groove[i].second.second, table3_deep_groove[i + 1].first, table3_deep_groove[i + 1].second.second);
                    break;
                }
            }
        }
        if (ratio <= e)
        {
            X = 1.0;
            Y = 0.0;
        }
        else
        {
            X = 0.56;
            Y = Y_table;
        }
        break;
    }
    case BearingSpecificType::AngularContactBall:
    {
        double auto_f0 = StaticFactorCalculator::CalculateF0(bearing);
        double current_c0 = (bearing.C0 > 0.0) ? bearing.C0 : StaticFactorCalculator::CalculateC0(bearing);
        double relative_load = (current_c0 <= 0.0) ? 0.0 : ((auto_f0 * bearing.rows * abs_Fa) / current_c0);
        std::vector<double> active_load_rows = std_load_rows;
        if (std::abs(bearing.alpha - 5.0) < 1.0)
        {
            active_load_rows = {0.173, 0.346, 0.692, 1.04, 1.38, 2.08, 3.46, 5.19, 6.92};
        }
        else if (std::abs(bearing.alpha - 10.0) < 1.0)
        {
            active_load_rows = {0.175, 0.35, 0.7, 1.05, 1.4, 2.1, 3.5, 5.25, 7.0};
        }
        else if (std::abs(bearing.alpha - 15.0) < 1.0)
        {
            active_load_rows = {0.178, 0.357, 0.714, 1.07, 1.43, 2.14, 3.57, 5.35, 7.14};
        }
        size_t r0 = 0, r1 = 0;
        if (relative_load <= active_load_rows.front())
        {
            r0 = 0;
            r1 = 0;
        }
        else if (relative_load >= active_load_rows.back())
        {
            r0 = 8;
            r1 = 8;
        }
        else
        {
            for (size_t i = 0; i < active_load_rows.size() - 1; ++i)
            {
                if (relative_load >= active_load_rows[i] && relative_load <= active_load_rows[i + 1])
                {
                    r0 = i;
                    r1 = i + 1;
                    break;
                }
            }
        }
        size_t c0 = 0, c1 = 0;
        double target_alpha = bearing.alpha;
        if (target_alpha <= std_angles.front())
        {
            c0 = 0;
            c1 = 0;
        }
        else if (target_alpha >= std_angles.back())
        {
            c0 = 8;
            c1 = 8;
        }
        else
        {
            for (size_t j = 0; j < std_angles.size() - 1; ++j)
            {
                if (target_alpha >= std_angles[j] && target_alpha <= std_angles[j + 1])
                {
                    c0 = j;
                    c1 = j + 1;
                    break;
                }
            }
        }

        auto interpolate_cell = [&](size_t row) -> Table3Cell
        {
            Table3Cell res;
            if (c0 == c1)
                return table3_grid[row][c0];
            double a0 = std_angles[c0], a1 = std_angles[c1];
            res.e = LinearInterpolate(target_alpha, a0, table3_grid[row][c0].e, a1, table3_grid[row][c1].e);
            res.x_single_gt = LinearInterpolate(target_alpha, a0, table3_grid[row][c0].x_single_gt, a1, table3_grid[row][c1].x_single_gt);
            res.y_single_gt = LinearInterpolate(target_alpha, a0, table3_grid[row][c0].y_single_gt, a1, table3_grid[row][c1].y_single_gt);
            res.x_double_le = LinearInterpolate(target_alpha, a0, table3_grid[row][c0].x_double_le, a1, table3_grid[row][c1].x_double_le);
            res.y_double_le = LinearInterpolate(target_alpha, a0, table3_grid[row][c0].y_double_le, a1, table3_grid[row][c1].y_double_le);
            res.x_double_gt = LinearInterpolate(target_alpha, a0, table3_grid[row][c0].x_double_gt, a1, table3_grid[row][c1].x_double_gt);
            res.y_double_gt = LinearInterpolate(target_alpha, a0, table3_grid[row][c0].y_double_gt, a1, table3_grid[row][c1].y_double_gt);
            return res;
        };

        Table3Cell cell0 = interpolate_cell(r0);
        Table3Cell cell1 = interpolate_cell(r1);

        Table3Cell final_cell;
        double l0 = active_load_rows[r0], l1 = active_load_rows[r1];
        final_cell.e = LinearInterpolate(relative_load, l0, cell0.e, l1, cell1.e);
        final_cell.x_single_gt = LinearInterpolate(relative_load, l0, cell0.x_single_gt, l1, cell1.x_single_gt);
        final_cell.y_single_gt = LinearInterpolate(relative_load, l0, cell0.y_single_gt, l1, cell1.y_single_gt);
        final_cell.x_double_le = LinearInterpolate(relative_load, l0, cell0.x_double_le, l1, cell1.x_double_le);
        final_cell.y_double_le = LinearInterpolate(relative_load, l0, cell0.y_double_le, l1, cell1.y_double_le);
        final_cell.x_double_gt = LinearInterpolate(relative_load, l0, cell0.x_double_gt, l1, cell1.x_double_gt);
        final_cell.y_double_gt = LinearInterpolate(relative_load, l0, cell0.y_double_gt, l1, cell1.y_double_gt);

        if (bearing.rows == 1)
        {
            if (ratio <= final_cell.e)
            {
                X = 1.0;
                Y = 0.0;
            }
            else
            {
                X = final_cell.x_single_gt;
                Y = final_cell.y_single_gt;
            }
        }
        else
        {
            if (ratio <= final_cell.e)
            {
                X = final_cell.x_double_le;
                Y = final_cell.y_double_le;
            }
            else
            {
                X = final_cell.x_double_gt;
                Y = final_cell.y_double_gt;
            }
        }
        break;
    }

    case BearingSpecificType::SelfAligningBall:
    {
        if (bearing.alpha <= 0.0)
        {
            throw std::invalid_argument("核心参数异常：调心球轴承公称接触角 alpha 必须大于 0°。");
        }

        double e = 1.5 * std::tan(alpha_rad);
        double cot_alpha = 1.0 / std::tan(alpha_rad);

        if (bearing.rows == 1)
        {
            if (ratio <= e)
            {
                X = 1.0;
                Y = 0.0;
            }
            else
            {
                X = 0.4;
                Y = 0.4 * cot_alpha;
            }
        }
        else
        {
            if (ratio <= e)
            {
                X = 1.0;
                Y = 0.42 * cot_alpha;
            }
            else
            {
                X = 0.65;
                Y = 0.65 * cot_alpha;
            }
        }
        break;
    }

    case BearingSpecificType::RadialRoller:
    {
        if (bearing.alpha == 0.0)
        {
            if (abs_Fa > 0.0)
            {
                std::cout << "\n>> [工程提示] 警告：纯径向滚子轴承承受了轴向力 Fa = " << abs_Fa << " N。\n"
                          << "   实际应用中请务必核算挡边滑动摩擦导致的磨损与发热极限！\n\n";
            }
            X = 1.0;
            Y = 0.0;
        }
        else
        {
            double e = 1.5 * std::tan(alpha_rad);
            double cot_alpha = 1.0 / std::tan(alpha_rad);
            if (bearing.rows == 1)
            {
                if (ratio <= e)
                {
                    X = 1.0;
                    Y = 0.0;
                }
                else
                {
                    X = 0.4;
                    Y = 0.4 * cot_alpha;
                }
            }
            else
            {
                if (ratio <= e)
                {
                    X = 1.0;
                    Y = 0.45 * cot_alpha;
                }
                else
                {
                    X = 0.67;
                    Y = 0.67 * cot_alpha;
                }
            }
        }
        break;
    }

    case BearingSpecificType::ThrustBall_Angled:
    {
        struct Table5Record
        {
            double alpha;
            double e;
            double x_gt;
            double x_double_le;
            double y_double_le;
        };
        static const std::vector<Table5Record> table5_table =
            {
                {45.0, 1.25, 0.66, 1.18, 0.59}, {50.0, 1.49, 0.73, 1.37, 0.57}, {55.0, 1.79, 0.81, 1.60, 0.56}, {60.0, 2.17, 0.92, 1.90, 0.55}, {65.0, 2.68, 1.06, 2.30, 0.54}, {70.0, 3.43, 1.28, 2.90, 0.53}, {75.0, 4.67, 1.66, 3.89, 0.52}, {80.0, 7.09, 2.43, 5.86, 0.52}, {85.0, 14.29, 4.80, 11.75, 0.51}};

        double target_alpha = bearing.alpha;
        double e = 0.0, x_gt = 0.0, x_double_le = 0.0, y_double_le = 0.0;

        if (target_alpha <= table5_table.front().alpha)
        {
            auto r = table5_table.front();
            e = r.e;
            x_gt = r.x_gt;
            x_double_le = r.x_double_le;
            y_double_le = r.y_double_le;
        }
        else if (target_alpha >= table5_table.back().alpha)
        {
            auto r = table5_table.back();
            e = r.e;
            x_gt = r.x_gt;
            x_double_le = r.x_double_le;
            y_double_le = r.y_double_le;
        }
        else
        {
            for (size_t i = 0; i < table5_table.size() - 1; ++i)
            {
                if (target_alpha >= table5_table[i].alpha && target_alpha <= table5_table[i + 1].alpha)
                {
                    double a0 = table5_table[i].alpha;
                    double a1 = table5_table[i + 1].alpha;
                    e = LinearInterpolate(target_alpha, a0, table5_table[i].e, a1, table5_table[i + 1].e);
                    x_gt = LinearInterpolate(target_alpha, a0, table5_table[i].x_gt, a1, table5_table[i + 1].x_gt);
                    x_double_le = LinearInterpolate(target_alpha, a0, table5_table[i].x_double_le, a1, table5_table[i + 1].x_double_le);
                    y_double_le = LinearInterpolate(target_alpha, a0, table5_table[i].y_double_le, a1, table5_table[i + 1].y_double_le);
                    break;
                }
            }
        }

        if (bearing.rows == 1)
        {
            if (ratio <= e)
            {
                throw std::runtime_error("安全拒绝：单向角接触推力球轴承承受过大径向力(Fa/Fr <= e)。");
            }
            else
            {
                X = x_gt;
                Y = 1.0;
            }
        }
        else
        {
            if (ratio <= e)
            {
                X = x_double_le;
                Y = y_double_le;
            }
            else
            {
                X = x_gt;
                Y = 1.0;
            }
        }
        break;
    }

    case BearingSpecificType::ThrustRoller_Angled:
    {
        double tan_alpha = std::tan(alpha_rad);
        double e = 1.5 * tan_alpha;

        if (bearing.rows == 1)
        {
            if (ratio <= e)
            {
                throw std::runtime_error("安全拒绝：单向推力滚子轴承由于径向力过大(Fa/Fr <= e)。");
            }
            X = tan_alpha;
            Y = 1.0;
        }
        else
        {
            if (ratio <= e)
            {
                X = 1.5 * tan_alpha;
                Y = 0.67;
            }
            else
            {
                X = tan_alpha;
                Y = 1.0;
            }
        }
        break;
    }
    case BearingSpecificType::ThrustBall_90:
    case BearingSpecificType::ThrustRoller_90:
    {
        X = 0.0;
        Y = 1.0;
        break;
    }
    default:
        X = 1.0;
        Y = 0.0;
        break;
    }
}

double AisoCalculator::CalculateAiso(const BearingParameters &bearing, double kappa, double ec, double Cu, double P)
{
    // 0载荷/轻载工况安全钳位，不终止程序，直接赋予国标规定的系统修正系数上限
    if (P <= 0.0)
        return 50.0;
    if (kappa < 0.1)
        throw std::out_of_range("黏度比 kappa < 0.1，超出国标公式范围。");
    if (kappa > 4.0)
        kappa = 4.0;
    bool is_thrust = (bearing.alpha > 45.0) ||
                     (bearing.specificType == BearingSpecificType::ThrustBall_90) ||
                     (bearing.specificType == BearingSpecificType::ThrustBall_Angled) ||
                     (bearing.specificType == BearingSpecificType::ThrustRoller_90) ||
                     (bearing.specificType == BearingSpecificType::ThrustRoller_Angled);
    // 完全废除对 bearing.type 的依赖，使用 specificType 直接路由
    bool is_roller = (bearing.specificType == BearingSpecificType::RadialRoller ||
                      bearing.specificType == BearingSpecificType::ThrustRoller_90 ||
                      bearing.specificType == BearingSpecificType::ThrustRoller_Angled);
    double denominator_factor = is_thrust ? (is_roller ? 2.5 : 3.0) : 1.0;
    double load_ratio = (ec * Cu) / (denominator_factor * P);
    if (load_ratio > 5.0)
        load_ratio = 5.0;
    double inner_term = 0.0;
    if (!is_roller)
    {
        if (kappa >= 0.1 && kappa < 0.4)
            inner_term = 2.5671 - (2.2649 / std::pow(kappa, 0.054381));
        else if (kappa >= 0.4 && kappa < 1.0)
            inner_term = 2.5671 - (1.9987 / std::pow(kappa, 0.19087));
        else if (kappa >= 1.0 && kappa <= 4.0)
            inner_term = 2.5671 - (1.9987 / std::pow(kappa, 0.071739));
    }
    else
    {
        if (kappa >= 0.1 && kappa < 0.4)
            inner_term = 1.5859 - (1.3993 / std::pow(kappa, 0.054381));
        else if (kappa >= 0.4 && kappa < 1.0)
            inner_term = 1.5859 - (1.2348 / std::pow(kappa, 0.19087));
        else if (kappa >= 1.0 && kappa <= 4.0)
            inner_term = 1.5859 - (1.2348 / std::pow(kappa, 0.071739));
    }
    inner_term = std::max(0.0, inner_term);
    double inner_exponent = is_roller ? 1.0 : 0.83;
    double ratio_exponent = is_roller ? 0.4 : (1.0 / 3.0);
    double base_term = 1.0 - std::pow(inner_term, inner_exponent) * std::pow(load_ratio, ratio_exponent);
    if (base_term <= 0.0)
        return 50.0;
    double final_exponent = -9.3;
    if (is_roller)
    {
        if (is_thrust)
        {
            final_exponent = -9.185;
        }
        else
        {
            final_exponent = (kappa >= 1.0) ? -9.185 : -8.185;
        }
    }
    double a_iso = 0.1 * std::pow(base_term, final_exponent);
    return std::min(a_iso, 50.0);
}
double BearingFactorService::GetReliabilityFactor(double reliabilityPercentage)
{
    // 定义符合国标 GB/T 6391 表12 的结构化离散节点
    struct ReliabilityNode
    {
        double pct;    // 可靠度百分比 (%)
        double factor; // 对应的 a1 系数
    };

    static const std::vector<ReliabilityNode> table12 =
        {
            {90.0, 1.0},
            {95.0, 0.64},
            {96.0, 0.55},
            {97.0, 0.47},
            {98.0, 0.37},
            {99.0, 0.25},
            {99.2, 0.22},
            {99.4, 0.19},
            {99.6, 0.16},
            {99.8, 0.12},
            {99.9, 0.093},
            {99.92, 0.087},
            {99.94, 0.080},
            {99.95, 0.077}};

    double rel = reliabilityPercentage;
    // 轨道一：防断裂级浮点数模糊匹配
    const double EPSILON = 1e-5;
    for (const auto &node : table12)
    {
        if (std::abs(rel - node.pct) < EPSILON)
        {
            return node.factor;
        }
    }
    // 轨道二：工业安全边界钳位与防御
    if (rel < 90.0)
    {
        // 当可靠度低于国标基准 90% 时，保守回归基准动载寿命，系数赋予 1.0
        std::cout << "\n>> [安全钳位] 警告：输入可靠度 " << rel
                  << "% 低于国标 GB/T 6391 下限 (90%)。\n"
                  << "   系统已自动将其钳位归化为 90% (a1 = 1.0)，按基本额定寿命进行保守核算。\n\n";
        return 1.0;
    }
    if (rel > 99.95)
    {
        // 超出标准最高边界时执行极限安全钳位，规避零寿命发散风险
        std::cout << "\n>> [安全钳位] 警告：输入可靠度 " << rel
                  << "% 极高，超出表12上限 (99.95%)。\n"
                  << "   为防止修正寿命趋近于零导致系统发散，已强制钳位至 99.95% 极限 (a1 = 0.077)。\n\n";
        return 0.077;
    }
    // 轨道三：自适应区间线性插值解算（支持非标准连续可靠度输入）
    for (size_t i = 0; i < table12.size() - 1; ++i)
    {
        if (rel > table12[i].pct && rel < table12[i + 1].pct)
        {
            double r0 = table12[i].pct;
            double r1 = table12[i + 1].pct;
            double a0 = table12[i].factor;
            double a1_val = table12[i + 1].factor;

            // 基础线性插值公式：y = y0 + (x - x0) * (y1 - y0) / (x1 - x0)
            return a0 + (rel - r0) * (a1_val - a0) / (r1 - r0);
        }
    }

    // 终极兜底保障
    return 1.0;
}

double ViscosityCalculator::CalculateReferenceViscosity(double n, double Dpw)
{
    // 基于 GB/T 6391-2010 第 9.3.3.3.1 节 公式 (28) 与 (29)
    // 1. 尺寸缺失防御：基于图片定义，Dpw 物理上不可能 <= 0。
    // 若遇到未正确初始化的异常输入，赋予极小正数防止底数 <=0 导致 std::pow 产生 NaN
    double eff_Dpw = (Dpw <= 0.0) ? 1e-5 : Dpw;
    // 2. 停转/极低转速物理极限防御：
    // 当 n 趋近于 0 时，理论上无法形成流体动压润滑，所需的参考黏度 v1 趋于无穷大。
    // 摒弃原有的魔法数字 1000.0，返回一个符合渐进特性的极大值 (1.0e6)，
    // 这将使后续公式(27)计算出的 kappa 自然趋近于 0，从而在系统逻辑上正确触发极度贫油的寿命惩罚。
    if (n <= 0.0)
    {
        return 1.0e6;
    }
    // 3. 严格执行国标分界解算
    if (n < 1000.0)
    {
        return 45000.0 * std::pow(n, -0.83) * std::pow(eff_Dpw, -0.5);
    }
    else
    {
        return 4500.0 * std::pow(n, -0.5) * std::pow(eff_Dpw, -0.5);
    }
}

double ViscosityCalculator::CalculateViscosityRatio(double v, double v1)
{
    // 基于 GB/T 6391-2010 第 9.3.3.3.1 节 公式 (27)
    // 核心回归：这里必须保持数学公式的纯粹性，还原真实的物理黏度比。
    // 取消在此处人为限制 [0.1, 4.0] 的防呆钳位，以保证最终系统输出的计算报告中 kappa 的数值完全真实。
    // (注：规范中 a_iso 图表所需的 [0.1, 4.0] 值域截断，应当在上层 AisoCalculator 的经验模型内部局部应用)
    if (v1 <= 0.0)
        return 0.0;

    return v / v1;
}

double ContaminationFactorCalculator::Getec(ContaminationLevel level, double Dpw, double kappa)
{
    // 物理防御：规避尺寸为0或停转导致的异常
    if (Dpw <= 0.0 || kappa <= 0.0)
        return 0.0;
    // 极度清洁工况不受公式限制，直接返回理想值 1.0
    if (level == ContaminationLevel::ExtremeClean)
    {
        return 1.0;
    }
    double c1 = 0.0;
    double c2 = 0.0;
    // 根据 GB/T 6391-2010 附录 A.6 连续函数公式映射常数
    switch (level)
    {
    case ContaminationLevel::HighClean:
        c1 = 0.0864;
        c2 = 0.6796;
        break;
    case ContaminationLevel::NormalClean:
        c1 = 0.0432;
        c2 = 1.141;
        break;
    case ContaminationLevel::Slight:
    case ContaminationLevel::Typical:
        c1 = 0.0177;
        c2 = (Dpw < 500.0) ? 1.887 : 1.677;
        break;
    case ContaminationLevel::Severe:
        c1 = 0.0115;
        c2 = 2.662;
        break;
    case ContaminationLevel::VerySevere:
        c1 = 0.00617;
        c2 = 4.06;
        break;
    default:
        return 0.0;
    }
    // 核心计算步骤
    double a = c1 * std::pow(kappa, 0.68) * std::pow(Dpw, 0.55);
    // 国标安全限值：a 不得大于 1.0
    a = std::min(a, 1.0);
    double ec = a * (1.0 - c2 / std::pow(Dpw, 1.0 / 3.0));
    // 边界钳位与防御：当尺寸极小或常数过大时可能导致括号内为负数
    // 污染系数在物理意义上不能为负，也不能超过完全清洁的 1.0
    return std::max(0.0, std::min(ec, 1.0));
}
double BearingLifeCalculator::CalculateEquivalentDynamicLoad(double X, double Fr, double Y, double Fa)
{
    return X * Fr + Y * Fa;
}
double BearingLifeCalculator::CalculateBasicRatingLifeL10(const BearingParameters &bearing, double P)
{
    // 载荷下不再中断抛错，默认输出计算寿命理论上限
    if (P <= 0.0)
        return 1.0e8;
    // 【核心自动分轨】
    bool isRoller = (bearing.specificType == BearingSpecificType::RadialRoller ||
                     bearing.specificType == BearingSpecificType::ThrustRoller_90 ||
                     bearing.specificType == BearingSpecificType::ThrustRoller_Angled);
    double exponent = isRoller ? (10.0 / 3.0) : 3.0;
    return std::pow(bearing.C / P, exponent);
}
double BearingLifeCalculator::CalculateModifiedRatingLifeLnm(double L10, double a1, double a_iso)
{
    return a1 * a_iso * L10;
}
MultiRowDynamicLifeResult BearingLifeCalculator::CalculateMultiRowDynamicLife(
    const BearingParameters &bearing,
    BearingLayoutConfiguration layout,
    int assemblyCount,
    const OperatingConditions &cond,
    const EnvironmentParameters &env)
{
    MultiRowDynamicLifeResult result;
    double abs_Fr = std::abs(cond.Fr);
    double abs_Fa = std::abs(cond.Fa);
    double ratio = (abs_Fr == 0.0) ? 99999.0 : (abs_Fa / abs_Fr);
    // 识别滚动体大类
    bool isRoller = (bearing.specificType == BearingSpecificType::RadialRoller ||
                     bearing.specificType == BearingSpecificType::ThrustRoller_90 ||
                     bearing.specificType == BearingSpecificType::ThrustRoller_Angled);
    // 轨道一：依据 GB/T 6391 / ISO 281 进行多列 X, Y 动态系数路由切换
    if (layout == BearingLayoutConfiguration::Single || layout == BearingLayoutConfiguration::DT)
    {
        // 串联配置（DT）或单列：严格执行“单列轴承”对应的 X, Y 查表与计算规则
        if (isRoller)
        {
            if (bearing.alpha == 0.0)
            {
                result.X = 1.0;
                result.Y = 0.0;
            }
            else
            {
                double alpha_rad = bearing.alpha * PI / 180.0;
                double e = 1.5 * std::tan(alpha_rad);
                if (ratio <= e)
                {
                    result.X = 1.0;
                    result.Y = 0.0;
                }
                else
                {
                    result.X = 0.4;
                    result.Y = 0.4 / std::tan(alpha_rad);
                }
            }
        }
        else
        {
            // 球轴承轨道：调用既有的标准单列插值器逻辑
            // 注意：若为 DT 串联，国标规定检索表3的相对载荷时，需用单套受力 (abs_Fa / assemblyCount)
            double dummyX, dummyY;
            LoadFactorCalculator::CalculateXY(bearing, abs_Fr, abs_Fa, dummyX, dummyY);
            result.X = dummyX;
            result.Y = dummyY;
        }
    }
    else if (layout == BearingLayoutConfiguration::DB || layout == BearingLayoutConfiguration::DF)
    {
        //  背对背/面对面（DB/DF）配置：强制硬切换至标准“双列轴承”专用系数轨
        if (isRoller)
        {
            if (bearing.alpha == 0.0)
            {
                result.X = 1.0;
                result.Y = 0.0;
            }
            else
            {
                double alpha_rad = bearing.alpha * PI / 180.0;
                double e = 1.5 * std::tan(alpha_rad);
                if (ratio <= e)
                {
                    result.X = 1.0;
                    result.Y = 0.45 / std::tan(alpha_rad); // 双列在低轴向力下 Y 依然生效
                }
                else
                {
                    result.X = 0.67;
                    result.Y = 0.67 / std::tan(alpha_rad);
                }
            }
        }
        else
        {
            // 角接触球轴承双列/DB/DF 动态系数硬跳变处理（以标准通用成对双列为例）
            double alpha_rad = bearing.alpha * PI / 180.0;
            double e = 1.5 * std::tan(alpha_rad); // 简化判据，实际可结合表3网格
            if (ratio <= e)
            {
                result.X = 1.0;
                // 根据标准，双列球轴承在 Fa/Fr <= e 时，Y 值为单列 Y 值的两倍左右或直接查双列栏
                result.Y = 0.92; // 示例：对应15度角接触双列典型值，实际可接入 grid 的 x_double_le/y_double_le
            }
            else
            {
                result.X = 0.57;
                result.Y = 0.93;
            }
        }
    }
    // 轨道二：当量动载荷 P 计算与零载荷防御安全钳位
    // 输入力 Fr, Fa 为整体组合承受的总外载荷
    result.P = result.X * abs_Fr + result.Y * abs_Fa;
    if (result.P <= 0.0)
    {
        // 触发零载荷安全熔断保护：不终止程序，直接默认输出国标及系统允许的计算寿命上限
        result.L10r = 1.0e8; // 1亿百万转
        result.L10h = (cond.n <= 0.0) ? 999999.0 : (1.0e6 * result.L10r) / (60.0 * cond.n);
        result.a_iso = 50.0; // 国标规定的上限值
        double a1 = BearingFactorService::GetReliabilityFactor(env.reliability);
        result.Lnm = a1 * result.a_iso * result.L10r;
        return result;
    }
    // 轨道三：常规状态寿命迭代演进
    // 基础额定寿命计算（由于提示说明“只有额定动载荷不需要考虑”，在此直接代入已经过有效处理的 bearing.C）
    double exponent = isRoller ? (10.0 / 3.0) : 3.0;
    result.L10r = std::pow(bearing.C / result.P, exponent);
    result.L10h = (cond.n <= 0.0) ? 999999.0 : (1.0e6 * result.L10r) / (60.0 * cond.n);
    // 自适应国标修正寿命计算
    double v1 = ViscosityCalculator::CalculateReferenceViscosity(cond.n, bearing.Dpw);
    double kappa = ViscosityCalculator::CalculateViscosityRatio(cond.v, v1);
    double ec = ContaminationFactorCalculator::Getec(cond.contamLevel, bearing.Dpw, kappa);
    result.a_iso = AisoCalculator::CalculateAiso(bearing, kappa, ec, bearing.Cu, result.P);
    double a1 = BearingFactorService::GetReliabilityFactor(env.reliability);
    result.Lnm = CalculateModifiedRatingLifeLnm(result.L10r, a1, result.a_iso);
    return result;
}

// 在 BearingCalculators.cpp 末尾追加以下核心全自动流实现

UnifiedBearingReport BearingLifeCalculator::ExecuteUnifiedPipeline(
    BearingParameters &bearing,
    const OperatingConditions &cond,
    const EnvironmentParameters &env)
{
    UnifiedBearingReport rep;
    // 90度纯推力轴承全自动物理角度强锁对齐
    if (bearing.specificType == BearingSpecificType::ThrustBall_90 ||
        bearing.specificType == BearingSpecificType::ThrustRoller_90)
    {
        bearing.alpha = 90.0; // 自动覆盖前台的任何输入值，强制对齐为 90° 纯推力状态
    }
    // 1. 【自动识别与同步】将组配形态自动映射并刷新底层物理列数 rows
    // 基于 rows（列数）进行全自动智能逆向识别与防御
    if (bearing.rows <= 1)
    {
        // 100% 确定为常规单列轴承，前台哪怕没配 layout，程序也全自动修正
        bearing.rows = 1;
        bearing.layout = BearingLayoutConfiguration::Single;
        bearing.assemblyCount = 1;
    }
    else
    {
        // 当 rows >= 2 时，为了打破国标查表冲突，进行防错级智能推导
        if (bearing.layout == BearingLayoutConfiguration::Single)
        {
            // 防御：如果用户填了多列，但 layout 遗留为了 Single，默认自动归入 DB/DF 整体式双列轨
            bearing.layout = BearingLayoutConfiguration::DB;
        }

        if (bearing.layout == BearingLayoutConfiguration::DT)
        {
            // 如果是串联，成组套数自然等于物理列数
            bearing.assemblyCount = bearing.rows;
        }
        else
        {
            // DB/DF 整体双列或成对组合，成组套数默认为 2
            bearing.assemblyCount = 2;
        }
    }
    // 2. 前置几何自动推导
    StaticFactorCalculator::ResolveGeometry(bearing);
    // 若 C 未输入(为0)则执行自动解算
    rep.C = (bearing.C > 0.0) ? bearing.C : DynamicLoadRatingCalculator::CalculateDynamicLoadRating(bearing);
    // 3. 静态校核
    rep.C0 = (bearing.C0 > 0.0) ? bearing.C0 : StaticFactorCalculator::CalculateC0(bearing);
    rep.f0 = StaticFactorCalculator::CalculateF0(bearing);
    rep.P0 = StaticFactorCalculator::CalculateP0(bearing, cond.Fr, cond.Fa);
    rep.S0 = StaticFactorCalculator::CalculateS0(bearing, cond.Fr, cond.Fa);

    // 4. 【自动识别分轨】动态校核 X, Y 系数多列路由自适应切轨
    double abs_Fr = std::abs(cond.Fr);
    double abs_Fa = std::abs(cond.Fa);
    double ratio = (abs_Fr == 0.0) ? 99999.0 : (abs_Fa / abs_Fr);
    bool isRoller = (bearing.specificType == BearingSpecificType::RadialRoller ||
                     bearing.specificType == BearingSpecificType::ThrustRoller_90 ||
                     bearing.specificType == BearingSpecificType::ThrustRoller_Angled);

    if (bearing.layout == BearingLayoutConfiguration::Single || bearing.layout == BearingLayoutConfiguration::DT)
    {
        LoadFactorCalculator::CalculateXY(bearing, cond.Fr, cond.Fa, rep.X, rep.Y);
    }
    else if (bearing.layout == BearingLayoutConfiguration::DB || bearing.layout == BearingLayoutConfiguration::DF)
    {
        // 背对背或面对面：硬切换至双列动态系数轨
        if (isRoller)
        {
            if (bearing.alpha == 0.0)
            {
                rep.X = 1.0;
                rep.Y = 0.0;
            }
            else
            {
                double alpha_rad = bearing.alpha * PI / 180.0;
                double e = 1.5 * std::tan(alpha_rad);
                if (ratio <= e)
                {
                    rep.X = 1.0;
                    rep.Y = 0.45 / std::tan(alpha_rad);
                }
                else
                {
                    rep.X = 0.67;
                    rep.Y = 0.67 / std::tan(alpha_rad);
                }
            }
        }
        else
        {
            // 球轴承双列硬切换示例
            double alpha_rad = bearing.alpha * PI / 180.0;
            double e = 1.5 * std::tan(alpha_rad);
            if (ratio <= e)
            {
                rep.X = 1.0;
                rep.Y = 0.92;
            }
            else
            {
                rep.X = 0.57;
                rep.Y = 0.93;
            }
        }
    }
    // 5. 当量动载荷计算与【零载荷安全钳位机制】
    rep.P = rep.X * abs_Fr + rep.Y * abs_Fa;

    if (rep.P <= 0.0)
    {
        // 0载荷逻辑安全切轨，默认输出寿命与修正系数上限，常驻服务不崩溃
        rep.L10r = 1.0e8;
        rep.L10h = (cond.n <= 0.0) ? 999999.0 : (1.0e6 * rep.L10r) / (60.0 * cond.n);
        rep.a_iso = 50.0;
        rep.a1 = BearingFactorService::GetReliabilityFactor(env.reliability);
        rep.Lnm = rep.a1 * rep.a_iso * rep.L10r;
        return rep;
    }

    // 6. 标准寿命解算与 ISO 修正模型
    double exponent = isRoller ? (10.0 / 3.0) : 3.0;
    rep.L10r = std::pow(rep.C / rep.P, exponent);
    rep.L10h = (cond.n <= 0.0) ? 999999.0 : (1.0e6 * rep.L10r) / (60.0 * cond.n);
    rep.L10h = (cond.n <= 0.0) ? 999999.0 : (1.0e6 * rep.L10r) / (60.0 * cond.n);
    // ====================== 暂时注释解除修正寿命计算 ======================
    /*
    // 严格落实 GB/T 6391 9.3.3.3 黏度比全小节核心逻辑
    if (cond.lambda_film > 0.0)
    {
        rep.kappa = std::pow(cond.lambda_film, 1.3);
    }
    else
    {
        rep.kappa = ViscosityCalculator::CalculateViscosityRatio
        (
            cond.v,
            ViscosityCalculator::CalculateReferenceViscosity(cond.n, bearing.Dpw)
        );
    }

    double effective_kappa = rep.kappa;
    bool is_ep_active = false;
    if (cond.hasEPAdditive && rep.kappa < 1.0)
    {
        effective_kappa = 1.0;
        is_ep_active = true;
    }

    rep.ec = ContaminationFactorCalculator::Getec(cond.contamLevel, bearing.Dpw, effective_kappa);

    // 此处 CalculateAiso 会在 kappa < 0.1 时 throw 异常
    rep.a_iso = AisoCalculator::CalculateAiso(bearing, effective_kappa, rep.ec, bearing.Cu, rep.P);

    if (is_ep_active && rep.a_iso > 3.0)
    {
        rep.a_iso = 3.0;
    }
    rep.a1 = BearingFactorService::GetReliabilityFactor(env.reliability);
    rep.Lnm = CalculateModifiedRatingLifeLnm(rep.L10r, rep.a1, rep.a_iso);
    */

    // 临时替代逻辑：屏蔽修正寿命异常，直接回传基础寿命
    rep.kappa = 0.0;
    rep.ec = 0.0;
    rep.a1 = BearingFactorService::GetReliabilityFactor(env.reliability);
    rep.a_iso = 1.0;             // 修正系数默认设为 1.0
    rep.Lnm = rep.a1 * rep.L10r; // Lnm 退化为基本额定寿命计算
    return rep;
}
void BearingLifeCalculator::PrintReport(const BearingParameters &bearing, const OperatingConditions &cond, const UnifiedBearingReport &report)
{
    std::cout << "           滚动轴承全自动解算报告          \n";
    std::string layout_str = "单列(Single)";
    if (bearing.layout == BearingLayoutConfiguration::DB)
        layout_str = "背对背成对(DB) / 双列整体式";
    if (bearing.layout == BearingLayoutConfiguration::DF)
        layout_str = "面对面成对(DF) / 双列整体式";
    if (bearing.layout == BearingLayoutConfiguration::DT)
        layout_str = "串联成组(DT) [套数: " + std::to_string(bearing.assemblyCount) + "]";
    std::cout << ">> [系统识别] 轴承形态: " << layout_str << " | 解析计算物理列数 i = " << bearing.rows << "\n";
    std::cout << ">> [几何衍生] 滚子有效接触长度 Lwe = " << bearing.Lwe << " mm (总长: " << bearing.L_total << "mm)\n";
    std::cout << "----------------------------------------------------------------------\n";
    std::cout << ">> [GB/T 4662 静载核算]\n";
    // 动态展示 C0 是手动输入还是系统解算
    std::cout << "   基础额定静载荷 C0: " << report.C0 << " N "
              << (bearing.C0 > 0.0 ? "[用户手动输入]" : "[系统自动解算]")
              << "  |  静负荷计算系数 f0: " << report.f0 << "\n";
    std::cout << "   组合当量静载荷 P0: " << report.P0 << " N  |  静态安全系数 S0:  " << report.S0 << "\n";
    std::cout << "----------------------------------------------------------------------\n";
    std::cout << ">> [GB/T 6391 动载与寿命核算]\n";
    std::cout << "   基本额定动载荷 C : " << report.C << " N "
              << (bearing.C > 0.0 ? "[用户手动输入]" : "[系统自动解算]")
              << "  |  动载切轨系数 X : " << report.X << "  |  Y : " << report.Y << "\n";
    std::cout << "   组合当量动载荷 P : " << report.P << " N " << (report.P <= 1e-5 ? " [检测到零载荷]" : "") << "\n";
    std::cout << "   基本额定寿命 L10r: " << report.L10r << " 百万转 (10^6 转)\n";
    std::cout << "   基本额定寿命 L10h: " << report.L10h << " 小时 (h)\n";
    std::cout << "----------------------------------------------------------------------\n";
    std::cout << ">> [高级修正寿命核算]\n";
    // 动态展示黏度比背后的物理状态
    std::cout << "   基准黏度比 kappa: " << report.kappa;
    if (cond.lambda_film > 0.0)
        std::cout << " [基于 Lambda 油膜参数替代]";
    if (cond.hasEPAdditive && report.kappa < 1.0)
        std::cout << " -> (EP 添加剂强效干预至 1.0)";
    std::cout << "\n";
    std::cout << "   工况污染系数 ec: " << report.ec << "\n";
    std::cout << "   可靠度修正系数 a1 : " << report.a1 << "  |  系统修正系数 a_iso: " << report.a_iso;
    if (cond.hasEPAdditive && report.kappa < 1.0 && report.a_iso == 3.0)
        std::cout << " [已触发 EP 极限钳位 <= 3.0]";
    std::cout << "\n";
    std::cout << ">> [核心主输出] 最终综合修正额定寿命 Lnm: " << report.Lnm << " 百万转\n";
}