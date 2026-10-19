#pragma once
#include "PreDefine.h"
//ISO17标准下R5-R40优先数
#define PriorityNumberR5Type 0
#define PriorityNumberR10Type 1
#define PriorityNumberR20Type 2
#define PriorityNumberR40Type 3
#define PriorityLastNumberStructType 10//结构尺寸优先数
#define PriorityLastNumberAsPoint5StructType 11//结构尺寸优先数,末尾按0.5圆整
namespace NSMechanismFunction
{
	//工程优先数
	extern double PriorityNumberListR5[5];
	extern double PriorityNumberListR10[10];
	extern double PriorityNumberListR20[20];
	extern double PriorityNumberListR40[40];
	extern int PriorityLastNumberListStruct[5];//最后一位优先尾数
	//通过最近距离求优先数
	double ReturnPriorityNumberByLastByShortestDistance(double InValue, int InPriorityNumberType);
	//通过向上取整求优先数
	double ReturnPriorityNumberByLastByRoundUp(double InValue, int InPriorityNumberType);
}
