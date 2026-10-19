#pragma once
#include "PreDefine.h"
class CCubicSplineTAN
{
public:
	CCubicSplineTAN() = default;;
	~CCubicSplineTAN();
	int mPointDimension;
	int mPointCount;
	double* mpVarList;
	double* mpSurfCoef;
	int IndexCreateMethod = 0;//系数的创建方法，0-为非动态创建，1为动态创建,需要手动释放内存
	//生样条对象,注意VectorList的主序和fortran主序对应，需要为行主序，即每行为一个矢量
	void BuildSplineObj(int InPointDimension, int InPointCount, double* InVarList, double* InVectorList, CCubicSplineTAN& IOSplineObj);
	void BuildSplineObj(int InPointDimension, int InPointCount, double* InVarList, double* InVectorList);
	//假如只有1维，直接返回计算值
	double CalInterpValueWithExtend(double InValue, CCubicSplineTAN& InSplineObj);
	void CalInterpValueWithExtend(double InValue, CCubicSplineTAN& InSplineObj,double* OutVector);
	double CalInterpValueWithExtend(double InValue);
	void CalInterpValueWithExtend(double InValue,double* OutVector);
private:
	void DeleteDataSpace();

};

