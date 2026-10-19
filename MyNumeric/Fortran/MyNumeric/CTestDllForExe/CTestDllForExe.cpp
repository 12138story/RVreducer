// CTestDllForExe.cpp : 此文件包含 "main" 函数。程序执行将在此处开始并结束。
//
#define Pi 3.1415926535897932384626433832795028841971693993751
#include <iostream>
#pragma comment(lib,"..\\x64\\Debug\\NumDll.lib")
extern "C" {double  _stdcall TANCALVECTORNORM(int*, double*); }
extern "C" {double  _stdcall TESTFUNPTR(void*,int*,double*); }
extern "C" {double  _stdcall TANFINDMINVALUEWITHOUTCONSTRAINFORC(void*,int*,double*,int*,void*); }
extern "C" {double  _stdcall TANFINDROOTBYBISECTIONFORC
(void* InFunction, double* InInitialVar, int* InOtherFunCoefCount, void** InOtherFunCoefList); }
double TestFunc()
{
	std::cout << "HHHHHHHHHHHHHHHHH" << std::endl;
	return 1.0;
}
extern "C" double _stdcall TESTOPTFUNC(double* InX, void** CoefList)
{
	double* TestCoefPtr = static_cast<double*>(CoefList[0]);
	double TempValue= 10.0*(InX[0])*(InX[0]) +5.0*InX[0] + TestCoefPtr[0];
	//std::cout << InX[0] << std::endl;
	//std::cout << TempValue<< std::endl;
	return TempValue;
}
//extern "C" double _stdcall TestFunc(double* InVar, void** Other)
//{
//	double TestCoef;
//	TestCoef = *(static_cast<double*>((Other[0])));
//	return pow(*InVar, 2) + 0.5*TestCoef + 1.0;
//}


double TestFunc1(double* InVar, void** Other)
{
	double TestCoef;
	TestCoef = *(static_cast<double*>((Other[0])));
	//double tempvalue=pow(*InVar, 2) + 0.5**InVar - 1.0;
	double GG = -0.89221;
	double HH = -0.88131;
	double Zn = 18.905122999927293;
	double tempvalue = 2 * GG / Zn * tan(*InVar) - HH - (*InVar);
	//std::cout << TestCoef << std::endl;
	std::cout << *InVar << std::endl;
	std::cout << tempvalue << std::endl;
	return tempvalue;
}
int main()
{
	int a = 3;
	double TestVector[3] = { 1.0,2.0,3.0 };
	std::cout << TANCALVECTORNORM(&a, TestVector)<<std::endl;
	void* TESTFUN = static_cast<void*>(&TESTOPTFUNC);
	int VarCount = 1;
	int CoefCount = 1;
	double TestMinValue = 3.0;
	void* CoefList[1];

	CoefList[0]= static_cast<void*>(&TestMinValue);
	double TestX[1] = { 1.0 };
	TestX[0] = 300.0;
	//std::cout << TESTFUNPTR(TESTFUN, &VarCount, TestX);
	std::cout << "求最小值" << std::endl;
	std::cout << TANFINDMINVALUEWITHOUTCONSTRAINFORC(TESTFUN, &VarCount, TestX, &CoefCount, CoefList);
	std::cout << "求最小值" << std::endl;
	double TestSolve[2]{ -Pi/2+0.00001,Pi / 2 - 0.00001 };
	int testCoefCout = 1;
	void* TestCoef[1];
	double CoefPar = 0.5;
	TestCoef[0] = static_cast<void*>(&CoefPar);
	void* TESTFUN1 = static_cast<void*>(&TestFunc1);
	std::cout << "求最根" << std::endl;
	std::cout << TANFINDROOTBYBISECTIONFORC(TESTFUN1, TestSolve, &testCoefCout, TestCoef);
	//TESTFUNPTR(TestFun);
	return 0;
}


