#include "PreDefine.h"


double NSTanBasiceFunction::FLog10InterpByTwoValue(double InX0, double InX1, double InX2, double InY1, double InY2)
{
	double TempExp;
	TempExp = (log10(InY2) - log10(InY1)) / (log10(InX2) - log10(InX1));
	return pow(InX0 / InX1, TempExp)* InY1;
}

void NSTanBasiceFunction::GenerateArithmeticSequenceContainInputValueInRange(int InContainValue, int InIncrValue, int InValueListCount, int InMinValue, int InMaxValue, std::vector<int>& OutVector)
{
	std::vector<int>(InValueListCount).swap(OutVector);
	int tmpBaseValue = 0;
	if (InContainValue < InMinValue)
	{
		tmpBaseValue = InMinValue;
	}
	else if (InContainValue > InMaxValue)
	{
		tmpBaseValue = InMaxValue;
	}
	else
	{
		tmpBaseValue = InContainValue;
	}

	int tmpPreviousElemCount = (tmpBaseValue - InMinValue) / InIncrValue;
	int tmpLastElemCount = (InMaxValue - tmpBaseValue) / InIncrValue;
	int tmpEachSideElemCount = (InValueListCount - 1) / 2;
	if (tmpPreviousElemCount < tmpEachSideElemCount)
	{
		int tmpStartValue = tmpBaseValue - tmpPreviousElemCount * InIncrValue;
		for (int i = 0; i < InValueListCount; i++)
		{
			OutVector[i] = tmpStartValue + InIncrValue * i;
		}
	}
	else if (tmpLastElemCount < tmpEachSideElemCount)
	{
		int tmpEndValue = tmpBaseValue + tmpLastElemCount * InIncrValue;
		for (int i = 0; i < InValueListCount; i++)
		{
			OutVector[InValueListCount - i - 1] = tmpEndValue - InIncrValue * i;
		}
	}
	else
	{
		int tmpStartValue = tmpBaseValue - tmpEachSideElemCount * InIncrValue;
		for (int i = 0; i < InValueListCount; i++)
		{
			OutVector[i] = tmpStartValue + InIncrValue * i;
		}
	}
}

int NSTanBasiceFunction::TANGCD(int a, int b)
{
	{
		while (b != 0) {
			int temp = b;
			b = a % b;
			a = temp;
		}
		return a;
	}
}

int NSTanBasiceFunction::ReturnShortedGCDNum(int InNum1,int InNum2, int SearchIncr, int InStartOffsetNum)
{
	int tmpNum1 = InNum2 + InStartOffsetNum;
	int tmpNum2 = InNum2 - InStartOffsetNum;
	for (int i = 0;i < 1000;i++)
	{
		if (TANGCD(InNum1, tmpNum1) == 1)
		{
			break;
		}
		tmpNum1 = tmpNum1 + SearchIncr;
	}
	for (int i = 0;i < 1000;i++)
	{		
		if (TANGCD(InNum1, tmpNum2 ) == 1)
		{
			break;
		}
		tmpNum2 = tmpNum2 - SearchIncr;
	}
	if ((InNum2 - tmpNum2) < (tmpNum1 - InNum2))
	{
		return tmpNum2;
	}
	else
	{
		return tmpNum1;
	}
}

int NSTanBasiceFunction::ReturnShortedGCDNum(int InNum1, int InNum2)
{
	if (InNum1 % 2 == 0)//为偶数，只需要在奇数集合中搜寻
	{
		if (InNum2 % 2 == 0)
		{
			return ReturnShortedGCDNum(InNum1, InNum2, 2, 1);
		}
		else
		{
			return ReturnShortedGCDNum(InNum1, InNum2, 2, 0);
		}
	}
	else
	{
		return ReturnShortedGCDNum(InNum1, InNum2, 1, 0);
	}
}


