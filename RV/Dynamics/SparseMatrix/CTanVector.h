#pragma once
#include "CommonDefine.h"
class CTanVector :
	public std::vector<double>
{
public:
	CTanVector():std::vector < double>() {}
	CTanVector(int InElemNum);
	CTanVector(int InElemNum, double* InVec);
	CTanVector(int InElemNum, double InValue);
	CTanVector(double* InVec);

	~CTanVector();
	void Set(int InElemNum);
	void Set(int InElemNum, int AssignElemNum, double* InVec);
	void Set(int InElemNum, double* InVec);
	void Set(int InElemNum, double InValue);
	void Set(double* InVec);
	template <typename T1>
	void Set(int InElemNum, T1&& InVec)
	{
		CTanVector TempVector(InElemNum);
		TempVector.assign(InVec.begin(), InVec.end());
		TempVector.swap(*this);
	}
	//全部变为绝对值
	static void GetAbsoluteValue(CTanVector& InVec);
	void GetAbsoluteValue();
	//获取最大值和最大值位置
	static void GetMaxValueAndIndex(CTanVector& InVec, double* OutMaxValue, int* OutMaxValueIndex);
	void GetMaxValueAndIndex(double* MaxValue, int* MaxValueIndex);
	//数乘
	static void ScaleProduct(CTanVector& InVec, double InScale);
	void ScaleProduct(double InScale);
	//向量元素求和
	static double Sum(CTanVector& InVec, int StartIndex, int EndIndex);
	double Sum(int StartIndex, int EndIndex);
	double Sum();
	//向量求加
	static void Add(double InVec1Scal,CTanVector& InVec1, CTanVector& InVec2, CTanVector& OutVec);
	void Add(double InVec1Scal, CTanVector& InVec1);
	//生成一个向量的正交向量
	static void GenOrthogonalVector(CTanVector& InVec, CTanVector& OutVec);
	void GenOrthogonalVector(CTanVector& OutVec);	
	//点积
	static double DotProduct(CTanVector& InVec1, CTanVector& InVec2);
	double DotProduct(CTanVector& InVec);
	//2范数
	static double ObtainNorm2(CTanVector& InVec);	
	double ObtainNorm2();
	//叉积，只计算前3维
	static void CrossProduct(CTanVector& InVec1, CTanVector& InVec2, CTanVector& OutVec);
	void CrossProduct(CTanVector& InVec, CTanVector& OutVec);
	CTanVector CrossProduct(CTanVector& InVec);
	//单位化, 并返回向量长度
	static double Normalize(CTanVector& InVec);
	double Normalize();
	//根据一个向量生成局部坐标系矢量
	static void GenLocalCoor(CTanVector& InVec, CTanVector& OutXCoor, CTanVector& OutYCoor, CTanVector& OutZCoor);
	template <typename T1>
	void GenLocalCoor(T1&& OutVecList)
	{
		GenLocalCoor(*this, OutVecList[0], OutVecList[1], OutVecList[2]);
	}
	//根据一个向量生成坐标变换矩一维数列
	//从局部坐标系到全局坐标系
	static void GenCoorTransFromLocalToGlobal(CTanVector& InVec, double* Out1DTransMatrix);
	template <typename T1>
	static void GenCoorTransFromLocalToGlobal(CTanVector& InVec, T1&& Out1DTransMatrix)
	{
		CTanVector TempVec[3];
		GenLocalCoor(InVec, TempVec[0], TempVec[1], TempVec[2]);
		for (int i=0;i<3;i++)
		{
			Out1DTransMatrix[i] = TempVec[i][0];
			Out1DTransMatrix[i + 3] = TempVec[i][1];
			Out1DTransMatrix[i + 6] = TempVec[i][2];
		}
	}
	template <typename T1>
	void GenCoorTransFromLocalToGlobal(T1&& Out1DTransMatrix)
	{
		GenCoorTransFromLocalToGlobal(*this, Out1DTransMatrix);
	}
	//从全局坐标系到局部坐标系
	template <typename T1>
	static void GenCoorTransFromGlobalToLocal(CTanVector& InVec, T1&& Out1DTransMatrix)
	{
		CTanVector TempVec[3];
		GenLocalCoor(InVec, TempVec[0], TempVec[1], TempVec[2]);
		for (int i = 0; i < 3; i++)
		{
			Out1DTransMatrix[i] = TempVec[0][i];
			Out1DTransMatrix[i + 3] = TempVec[1][i];
			Out1DTransMatrix[i + 6] = TempVec[2][i];
		}
	}
	template <typename T1>
	void  GenCoorTransFromGlobalToLocal(T1&& Out1DTransMatrix)
	{
		GenCoorTransFromGlobalToLocal(*this, Out1DTransMatrix);
	}
};