#include "CycloidToleranceRepository.h"

#include <cmath>
#include <cstdlib>
#include <iostream>
#include <mysql.h>
#include <string>

namespace
{
// 根据表的尺寸区间查询指定精度等级的单项公差值。
double QuerySingleValue(MYSQL* connection, const std::string& tableName, int level,
	double d, double mn, double b)
{
	const std::string gradeField = "`" + std::to_string(level) + "`";
	std::string sql;
	if (tableName == "f_pt" || tableName == "f_p" || tableName == "f_r")
	{
		sql = "SELECT " + gradeField + " FROM " + tableName
			+ " WHERE d_min < " + std::to_string(d) + " AND d_max >= " + std::to_string(d)
			+ " AND m_min < " + std::to_string(mn) + " AND m_max >= " + std::to_string(mn);
	}
	else if (tableName == "f_beta")
	{
		sql = "SELECT " + gradeField + " FROM " + tableName
			+ " WHERE d_min < " + std::to_string(d) + " AND d_max >= " + std::to_string(d)
			+ " AND b_min < " + std::to_string(b) + " AND b_max >= " + std::to_string(b);
	}
	else
	{
		std::cerr << "未知的摆线轮公差数据表：" << tableName << std::endl;
		return -1.0;
	}

	if (mysql_query(connection, sql.c_str()) != 0)
	{
		std::cerr << "查询摆线轮公差表失败（表：" << tableName << "）："
			<< mysql_error(connection) << std::endl;
		return -1.0;
	}

	MYSQL_RES* result = mysql_store_result(connection);
	if (!result)
	{
		std::cerr << "读取摆线轮公差查询结果失败（表：" << tableName << "）："
			<< mysql_error(connection) << std::endl;
		return -1.0;
	}

	double value = -1.0;
	MYSQL_ROW row = mysql_fetch_row(result);
	if (row && row[0])
	{
		value = std::atof(row[0]);
	}
	else
	{
		std::cerr << "未找到匹配的摆线轮公差数据（表：" << tableName << "）。" << std::endl;
	}

	mysql_free_result(result);
	return value;
}

// 用交换后的 Fp 值反查对应精度等级，保持原有计算流程不变。
int ReverseQueryLevel(MYSQL* connection, double targetFp, double d, double mn)
{
	if (targetFp < 0.0)
	{
		return -1;
	}

	const std::string sql =
		"SELECT `0`, `1`, `2`, `3`, `4`, `5`, `6`, `7`, `8`, `9`, `10`, `11`, `12` FROM f_p "
		"WHERE d_min < " + std::to_string(d) + " AND d_max >= " + std::to_string(d);
	if (mysql_query(connection, sql.c_str()) != 0)
	{
		std::cerr << "反查摆线轮公差精度等级失败："
			<< mysql_error(connection) << std::endl;
		return -1;
	}

	MYSQL_RES* result = mysql_store_result(connection);
	if (!result)
	{
		std::cerr << "读取摆线轮公差等级反查结果失败："
			<< mysql_error(connection) << std::endl;
		return -1;
	}

	int matchedLevel = -1;
	MYSQL_ROW row = nullptr;
	while (matchedLevel == -1 && (row = mysql_fetch_row(result)) != nullptr)
	{
		for (int level = 0; level <= 12; ++level)
		{
			if (row[level] && std::fabs(std::atof(row[level]) - targetFp) < 1e-6)
			{
				matchedLevel = level;
				break;
			}
		}
	}

	mysql_free_result(result);
	return matchedLevel;
}
}

namespace data_access
{
// 完成一次完整查询，并确保失败时不向领域对象写入不完整数据。
bool CycloidToleranceRepository::TryLoad(int level, double pitchDiameter, double module,
	double width, CycloidToleranceRecord& record)
{
	if (level < 0 || level > 12 || pitchDiameter <= 0.0 || module <= 0.0 || width <= 0.0)
	{
		std::cerr << "摆线轮公差查询参数无效。" << std::endl;
		return false;
	}

	MYSQL* connection = mysql_init(nullptr);
	if (!connection)
	{
		std::cerr << "初始化摆线轮公差数据库连接失败。" << std::endl;
		return false;
	}

	if (!mysql_real_connect(connection, "localhost", "root", "jay", "bearings", 3729, nullptr, 0))
	{
		std::cerr << "连接摆线轮公差数据库失败："
			<< mysql_error(connection) << std::endl;
		mysql_close(connection);
		return false;
	}
	mysql_set_character_set(connection, "utf8");

	CycloidToleranceRecord loaded;
	loaded.F_p = QuerySingleValue(connection, "f_p", level, pitchDiameter, module, width);
	loaded.F_r = QuerySingleValue(connection, "f_r", level, pitchDiameter, module, width);

	const double originalFp = loaded.F_p;
	loaded.F_p = loaded.F_r;
	loaded.F_r = originalFp;

	const int adjustedLevel = ReverseQueryLevel(connection, loaded.F_p, pitchDiameter, module);
	if (adjustedLevel >= 0)
	{
		loaded.f_pt = QuerySingleValue(connection, "f_pt", adjustedLevel,
			pitchDiameter, module, width);
		loaded.F_beta = QuerySingleValue(connection, "f_beta", adjustedLevel,
			pitchDiameter, module, width);
	}

	mysql_close(connection);
	if (adjustedLevel < 0 || loaded.f_pt < 0.0 || loaded.F_p < 0.0
		|| loaded.F_beta < 0.0 || loaded.F_r < 0.0)
	{
		std::cerr << "未能获得完整的摆线轮公差数据。" << std::endl;
		return false;
	}

	record = loaded;
	return true;
}
}
