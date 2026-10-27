#pragma once

#include <string>
#include <vector>

namespace data_access
{
// 轴承目录的纯数据结构，用于隔离领域模型与 MySQL 类型。
struct BearingCatalogRecord
{
	std::string code;
	double d = 0.0;
	double D = 0.0;
	double B = 0.0;
	double Cr = 0.0;
	double C0r = 0.0;
	double e = 0.0;
	double Y = 0.0;
	double Yo = 0.0;
	double a = 0.0;
	double T = 0.0;
	double C = 0.0;
	double contactAngle = 0.0;
};

class BearingCatalogRepository
{
public:
	// 按轴承类型读取完整目录，并统一完成数据库字段和单位转换。
	static std::vector<BearingCatalogRecord> LoadAll(int bearingType);
};
}
