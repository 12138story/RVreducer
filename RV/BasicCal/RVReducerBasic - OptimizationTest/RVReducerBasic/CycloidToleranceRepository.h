#pragma once

namespace data_access
{
// 一次公差查询所需的四项摆线轮精度参数。
struct CycloidToleranceRecord
{
	double f_pt = -1.0;
	double F_p = -1.0;
	double F_beta = -1.0;
	double F_r = -1.0;
};

class CycloidToleranceRepository
{
public:
	// 查询并组合四项公差；参数或数据库结果无效时返回 false。
	static bool TryLoad(int level, double pitchDiameter, double module, double width,
		CycloidToleranceRecord& record);
};
}
