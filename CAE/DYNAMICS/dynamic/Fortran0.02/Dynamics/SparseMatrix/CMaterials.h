#pragma once
#include "CommonDefine.h"
class CMaterials
{
public:
	int Num=0;
	//double Ex=0;
	//double Proxy=0;
	//double Gx=0;
	std::map<std::string, double> Database;
	CMaterials();
	~CMaterials();
	static double ObtainShearModulus(double InEx, double InProxy);
	double ObtainShearModulus();
	//Ä£°åÇø
	//template <typename T1, typename T2, typename T3>
	//void AssignData(T1&& InNum, T2&& InMatNotation, T3&& InMatValue)
	//{
	//	Num = InNum;
	//	data["EX"] = InMatNotation;
	//	data["PRXY"] = InMatValue;
	//	data["GX"] = ObtainShearModulus(InMatNotation, InMatValue);
	//}
	template <typename T1, typename T2, typename T3>
	void AssignData(T1&& InNum, T2&& InMatNotation, T3&& InMatValue)
	{
		Num = InNum;
		Database[InMatNotation] = InMatValue;
	}
	template <typename T1, typename T2, typename T3>
	CMaterials(T1&& InNum, T2&& InMatNotation, T3&& InMatValue)
	{
		AssignData(InNum, InMatNotation, InMatValue);
	}


};

