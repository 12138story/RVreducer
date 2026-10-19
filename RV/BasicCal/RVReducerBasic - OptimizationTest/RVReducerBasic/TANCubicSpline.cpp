#include "TANCubicSpline.h"

CCubicSplineTAN::~CCubicSplineTAN()
{
	DeleteDataSpace();
}

void CCubicSplineTAN::BuildSplineObj(int InPointDimension, int InPointCount, double * InVarList, double * InVectorList, CCubicSplineTAN & IOSplineObj)
{
	IOSplineObj.DeleteDataSpace();
	mPointDimension = InPointDimension;
	mPointCount = InPointCount;
	mpVarList = new double[InPointCount];
	mpSurfCoef = new double[InPointDimension*InPointCount*2];
	IndexCreateMethod = 1;
	std::copy(InVarList, InVarList + mPointCount, mpVarList);
	CALSPLINECOEF_TANINTER(&mPointDimension, &mPointCount, InVarList, InVectorList, mpSurfCoef);
	
}

void CCubicSplineTAN::BuildSplineObj(int InPointDimension, int InPointCount, double * InVarList, double * InVectorList)
{
	BuildSplineObj(InPointDimension, InPointCount, InVarList, InVectorList, *this);
}

double CCubicSplineTAN::CalInterpValueWithExtend(double InValue, CCubicSplineTAN & InSplineObj)
{
	double resultValue;
	CALCUBICSPLINEINTERPVALUE_TANINTER(&InValue, &mPointDimension, &mPointCount, mpVarList, mpSurfCoef, &resultValue);
	return resultValue;
}

void CCubicSplineTAN::CalInterpValueWithExtend(double InValue, CCubicSplineTAN & InSplineObj, double * OutVector)
{
	CALCUBICSPLINEINTERPVALUE_TANINTER(&InValue, &mPointDimension, &mPointCount, mpVarList, mpSurfCoef, OutVector);
}

double CCubicSplineTAN::CalInterpValueWithExtend(double InValue)
{
	return CalInterpValueWithExtend(InValue,*this);
}

void CCubicSplineTAN::CalInterpValueWithExtend(double InValue, double * OutVector)
{
	CalInterpValueWithExtend(InValue, *this, OutVector);
}

void CCubicSplineTAN::DeleteDataSpace()
{
	if (IndexCreateMethod == 1)
	{
		delete[] mpVarList;
		delete[] mpSurfCoef;
		IndexCreateMethod = 0;
	}
}
