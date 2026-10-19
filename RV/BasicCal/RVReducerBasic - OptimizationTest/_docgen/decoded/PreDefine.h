# SOURCE ENCODING: gbk
#pragma once
#include <math.h>
#include <vector>
#include <algorithm>
#include <iterator>
#include<iostream>
#include <stdlib.h>
#include <set>
#include <list>
#define InvGearNotation 1
#define CycloidPinionNotation 2
#define CycloidGearNotation 3
#define DeepGrooveBallBearing 1001
#define NeedleBearing 1002
#define TaperedRollerBearing 1003                            
#define AngularContactBallBearing 1004

#define Pi 3.1415926535897932384626433832795028841971693993751
#define Degree 0.017453292519943295769236907684886127134428718885417
#define DefaultTolerance 0.000001
#define max(a,b) (((a) > (b)) ? a : b)
#define min(a,b) (((a) < (b)) ? a : b)
#define AssembleLeftBaseNotation 1
#define AssembleMiddleBaseNotation 0
#define AssembleRightBaseNotation -1
double const PI{ 3.1415926535897932384626433832795028841971693993751 };
//#pragma comment(lib,"D:\\WorkSpace\\softdevelop\\MyNumeric\\Fortran\\MyNumeric\\x64\\Debug\\NumDll.lib")
//#pragma comment(lib,"D:\\WorkSpace\\softdevelop\\CAD\\CubicSurf\\v0.2\\CoonsSurf\\Lib\\InterpSurfTANDLL.lib")
#pragma comment(lib,"D:/WorkSpace/softdevelop/CAD/CubicSurf/v0.2/CoonsSurf/Lib/InterpSurfTANDLL.lib")
//#pragma comment(lib,"Lib/InterpSurfTANDLL.lib")
extern "C" {double  _stdcall TANFINDMINVALUEWITHOUTCONSTRAINFORC
(void* InFunction, int* InVarCount, double* IOVarList, int* InOtherFunCoefCount, void** InOtherFunCoefList); }
extern "C" {double  _stdcall TANFINDROOTBYBISECTIONFORC
(void* InFunction,  double* InInitialVar, int* InOtherFunCoefCount, void** InOtherFunCoefList); }
extern "C" {double  _stdcall TANFINDROOTBYITERATIONFORC
(void* InFunction, double* InInitialVar, int* InOtherFunCoefCount, void** InOtherFunCoefList); }
extern "C" {double  _stdcall CALSPLINECOEF_TANINTER
(int* InPointDimension, int* InPointCount, double* InVarList, double* InPointList, double* OutSurfCoefPointer); }
extern "C" {double  _stdcall CALCUBICSPLINEINTERPVALUE_TANINTER
(double* InVar, int* InPointDimension, int* InPointCount,double* InVarList, double* InSurfCoefPointer, double* OutVector); }
namespace NSTanBasiceFunction
{
	//呈现对数线性关系的函数插值
	double FLog10InterpByTwoValue(double InX0, double InX1, double InX2, double InY1, double InY2);
	//判断正负
	template <typename T1>
	int Fsign(T1&& InValue)
	{
		if (InValue<0)
		{
			return -1;
		}
		else if (InValue > 0)
		{
			return 1;
		}
		else
		{
			return 0;
		}
	}
	//找到set集的索引
	template <typename T1,typename T2>
	int FindSetElemIndex(T1&& InSearchValue, T2&& InSet)
	{
		auto TempIt = InSet.find(InSearchValue);
		return std::distance(InSet.begin(), TempIt);
	}
	//找数值离数组最近的索引
	template <typename T1, typename T2>
	int FindValueShortestIndexInDoubleList(T1 InSearchValue, int InSearchedListCount, T2* InSearchedList)
	{
		int tmpShortestIndex = 0;
		double tmpShortestDistance = abs(InSearchValue - InSearchedList[0]);
		double tmpShortestDistance2 = 0;
		for (int i = 1;i < InSearchedListCount;i++)
		{
			tmpShortestDistance2 = abs(InSearchValue - InSearchedList[i]);
			if (tmpShortestDistance > tmpShortestDistance2)
			{
				tmpShortestDistance = tmpShortestDistance2;
				tmpShortestIndex = i;
			}
		}
		return tmpShortestIndex;
	}
	//生成在最小最大值范围内且包含输入数值的等差数列
	void GenerateArithmeticSequenceContainInputValueInRange(int InContainValue, int InIncrValue,int InValueListCount, int InMinValue, int InMaxValue, std::vector<int>& OutVector);
	//计算最大公约数
	int TANGCD(int a, int b);
	//搜寻最接近InNum2和InNum1互质的数
	int ReturnShortedGCDNum(int InNum1, int InNum2, int SearchIncr,int InStartOffsetNum = 0);
	int ReturnShortedGCDNum(int InNum1, int InNum2);
}

