#include "TANMechanismFunction.h"
double NSMechanismFunction::PriorityNumberListR5[5] = { 1.00, 1.60, 2.50, 4.00, 6.30 };
double NSMechanismFunction::PriorityNumberListR10[10] = { 1.00, 1.25, 1.60, 2.00, 2.50, 3.15, 4.00, 5.00, 6.30, 8.00 };
double NSMechanismFunction::PriorityNumberListR20[20] = { 1.00, 1.12, 1.25, 1.40, 1.60, 1.80, 2.00, 2.24, 2.50, 2.80, 3.15, 3.55, 4.00, 4.50, 5.00, 5.60, 6.30, 7.10, 8.00, 9.00 };
double NSMechanismFunction::PriorityNumberListR40[40] = { 1.00, 1.06, 1.12, 1.18, 1.25, 1.32, 1.40, 1.50, 1.60, 1.70,1.80, 1.90, 2.00, 2.12, 2.24, 2.36, 2.50, 2.65, 2.80, 3.00,
			3.15, 3.35, 3.55, 3.75, 4.00, 4.25, 4.50, 4.75, 5.00, 5.30,5.60, 6.00, 6.30, 6.70, 7.10, 7.50, 8.00, 8.50, 9.00, 9.50 };
int NSMechanismFunction::PriorityLastNumberListStruct[5] = { 0, 2, 5, 6, 8 };
double NSMechanismFunction::ReturnPriorityNumberByLastByShortestDistance(double InValue, int InPriorityNumberType)
{
	int tmpValue = round(InValue);
	int tmpLastNum = tmpValue % 10;
	int tmpIndex=0;
	double tmpLastValue = 0;
	double tmpPreviousValue = 0;
	double tmpRemainderValue = 0;
	if (InPriorityNumberType == PriorityLastNumberStructType)
	{
		tmpIndex = NSTanBasiceFunction::FindValueShortestIndexInDoubleList(tmpLastNum, 5, PriorityLastNumberListStruct);
		double resultValue = (tmpValue / 10) * 10 + PriorityLastNumberListStruct[tmpIndex];
		return resultValue;
	}
	else if (InPriorityNumberType == PriorityNumberR10Type)
	{
		tmpPreviousValue = static_cast<double>(int(InValue / 10) * 10);
		tmpRemainderValue = InValue - tmpPreviousValue;
		tmpIndex = NSTanBasiceFunction::FindValueShortestIndexInDoubleList(tmpRemainderValue,10, PriorityNumberListR10);
		return tmpPreviousValue + PriorityNumberListR10[tmpIndex];
	}
	else
	{
		return InValue;
	}

}

double NSMechanismFunction::ReturnPriorityNumberByLastByRoundUp(double InValue, int InPriorityNumberType)
{
	if (InPriorityNumberType == PriorityLastNumberStructType)
	{
		int tmpValue = round(InValue);
		int tmpLastNum = tmpValue % 10;
		int tmpIndex = 0;
		for (int i = 0;i < 5;i++)
		{
			if (tmpLastNum - PriorityLastNumberListStruct[i] < 0)
			{
				tmpIndex = i;
				break;
			}
		}
		double resultValue = (tmpValue / 10) * 10 + PriorityLastNumberListStruct[tmpIndex];
		return resultValue;
	}
	else if (InPriorityNumberType == PriorityLastNumberAsPoint5StructType)
	{
		int tmpValue = ceil(InValue * 2);
		return double(tmpValue) / 2;
	}
	else
	{
		return InValue;
	}
}
