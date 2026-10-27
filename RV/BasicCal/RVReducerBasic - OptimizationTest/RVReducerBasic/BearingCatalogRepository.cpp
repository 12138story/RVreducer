#include "BearingCatalogRepository.h"
#include "PreDefine.h"
#include <iostream>
#include <mysql.h>
#include <string>

namespace
{
// 将领域层使用的轴承类型映射为受控的数据表名。
const char* ResolveTableName(int bearingType)
{
	switch (bearingType)
	{
	case DeepGrooveBallBearing:
		return "deepgrooveball";
	case NeedleBearing:
		return "needlebearing";
	case TaperedRollerBearing:
		return "taperedrollerbearingsinsinglerows";
	case AngularContactBallBearing:
		return "angularcontactballbearingsingle";
	default:
		return "bearing_default";
	}
}

// 安全读取数值字段，scale 用于将数据库中的 kN 转换为 N。
double ReadDouble(MYSQL_ROW row, unsigned int fieldCount, unsigned int index, double scale = 1.0)
{
	return index < fieldCount && row[index] ? std::stod(row[index]) * scale : 0.0;
}
}

namespace data_access
{
// 集中管理连接、查询、字段解析和资源释放，调用方只接收普通 C++ 数据。
std::vector<BearingCatalogRecord> BearingCatalogRepository::LoadAll(int bearingType)
{
	std::vector<BearingCatalogRecord> records;
	MYSQL* connection = mysql_init(nullptr);
	if (!connection)
	{
		std::cerr << "初始化轴承目录数据库连接失败。" << std::endl;
		return records;
	}

	if (!mysql_real_connect(connection, "localhost", "root", "jay", "bearings", 3729, nullptr, 0))
	{
		std::cerr << "连接轴承目录数据库失败："
			<< mysql_error(connection) << std::endl;
		mysql_close(connection);
		return records;
	}

	const std::string query = std::string("SELECT * FROM ") + ResolveTableName(bearingType);
	if (mysql_query(connection, query.c_str()) != 0)
	{
		std::cerr << "查询轴承目录失败：" << mysql_error(connection) << std::endl;
		mysql_close(connection);
		return records;
	}

	MYSQL_RES* result = mysql_store_result(connection);
	if (!result)
	{
		std::cerr << "读取轴承目录查询结果失败："
			<< mysql_error(connection) << std::endl;
		mysql_close(connection);
		return records;
	}

	const unsigned int fieldCount = mysql_num_fields(result);
	MYSQL_ROW row = nullptr;
	while ((row = mysql_fetch_row(result)) != nullptr)
	{
		BearingCatalogRecord record;
		record.code = fieldCount > 0 && row[0] ? row[0] : "";
		record.d = ReadDouble(row, fieldCount, 1);
		record.D = ReadDouble(row, fieldCount, 2);
		record.B = ReadDouble(row, fieldCount, 3);
		record.Cr = ReadDouble(row, fieldCount, 4, 1000.0);
		record.C0r = ReadDouble(row, fieldCount, 5, 1000.0);

		if (bearingType == TaperedRollerBearing)
		{
			record.e = ReadDouble(row, fieldCount, 6);
			record.Y = ReadDouble(row, fieldCount, 7);
			record.Yo = ReadDouble(row, fieldCount, 8);
			record.a = ReadDouble(row, fieldCount, 9);
			record.T = ReadDouble(row, fieldCount, 10);
			record.C = ReadDouble(row, fieldCount, 11);
		}
		else if (bearingType == AngularContactBallBearing)
		{
			record.a = ReadDouble(row, fieldCount, 6);
			record.contactAngle = 0.2618;
		}

		records.push_back(record);
	}

	mysql_free_result(result);
	mysql_close(connection);
	return records;
}
}
