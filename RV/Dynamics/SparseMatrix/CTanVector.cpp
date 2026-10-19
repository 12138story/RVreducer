#include "CTanVector.h"



CTanVector::CTanVector(int InElemNum)
{
	Set(InElemNum);
}

CTanVector::CTanVector(int InElemNum, double * InVec)
{
	Set(InElemNum, InVec);
}

CTanVector::CTanVector(int InElemNum, double InValue)
{
	Set(InElemNum, InValue);
}

CTanVector::CTanVector(double * InVec)
{
	Set(InVec);
}


CTanVector::~CTanVector()
{
}
void CTanVector::Set(int InElemNum)
{
	std::vector<double>(InElemNum, 0).swap(*this);
}
void CTanVector::Set(int InElemNum, int AssignElemNum, double * InVec)
{
	Set(InElemNum);
	assign(InVec, InVec + AssignElemNum);
}
void CTanVector::Set(int InElemNum, double * InVec)
{
	Set(InElemNum);
	assign(InVec, InVec+InElemNum);
}
void CTanVector::Set(int InElemNum, double InValue)
{
	std::vector<double>(InElemNum, InValue).swap(*this);
}
void CTanVector::Set(double * InVec)
{
	assign(InVec, InVec + size());
}
void CTanVector::GetAbsoluteValue(CTanVector & InVec)
{
	std::for_each(InVec.begin(), InVec.end(), [](double& i1) {i1=abs(i1); });
}

void CTanVector::GetAbsoluteValue()
{
	GetAbsoluteValue(*this);
}

void CTanVector::GetMaxValueAndIndex(CTanVector & InVec, double * MaxValue, int * MaxValueIndex)
{
	auto MaxPoisition = max_element(InVec.begin(), InVec.end());
	if (MaxValue)
	{
		*MaxValue = *MaxPoisition;
	}
	if (MaxValueIndex)
	{
		*MaxValueIndex = (int)(MaxPoisition - InVec.begin());
	}
}

void CTanVector::GetMaxValueAndIndex(double * MaxValue, int * MaxValueIndex)
{
	GetMaxValueAndIndex(*this, MaxValue, MaxValueIndex);
}

void CTanVector::ScaleProduct(CTanVector & InVec, double InScale)
{
	cblas_dscal((int)InVec.size(), InScale, &InVec[0], 1);
}
void CTanVector::ScaleProduct(double InScale)
{
	ScaleProduct(*this, InScale);
}

double CTanVector::Sum(CTanVector & InVec,int StartIndex, int EndIndex)
{
	return cblas_dasum((int)(EndIndex- StartIndex+1), &InVec[StartIndex],1);
}

double CTanVector::Sum(int StartIndex, int EndIndex)
{
	return Sum(*this, StartIndex, EndIndex);
}

double CTanVector::Sum()
{
	return Sum(*this,0,(int)this->size()-1);
}

void CTanVector::Add(double InVec1Scal, CTanVector & InVec1, CTanVector & InVec2, CTanVector & OutVec)
{
	OutVec.Set((int)InVec2.size(), InVec2);
	cblas_daxpy((int)InVec2.size(), InVec1Scal, &InVec1[0], 1, &OutVec[0], 1);
}

void CTanVector::Add(double InVec1Scal, CTanVector & InVec1)
{
	cblas_daxpy((int)size(), InVec1Scal, &InVec1[0], 1, &(*this)[0], 1);
}

void CTanVector::GenOrthogonalVector(CTanVector & InVec, CTanVector & OutVec)
{
	double MaxValue;
	int MaxIndex;
	CTanVector TempVector;
	int ElemNum = (int)InVec.size();
	int EndIndex = ElemNum - 1;
	//TempVector.Set(ElemNum, InVec);
	//TempVector.GetAbsoluteValue();
	//TempVector.GetMaxValueAndIndex(&MaxValue, &MaxIndex);
	MaxIndex=(int)cblas_idamax((int)ElemNum, &InVec[0], 1);
	MaxValue = InVec[MaxIndex];
	TempVector.Set(ElemNum, InVec);
	TempVector.ScaleProduct(1.0/MaxValue);
	double OriginMaxPosValue = 0;
	if (MaxIndex==0)
	{
		OriginMaxPosValue = -TempVector.Sum(1, EndIndex);
	}
	else if (MaxIndex== EndIndex)
	{
		OriginMaxPosValue = -TempVector.Sum(0, EndIndex-1);
	}
	else
	{
		OriginMaxPosValue= -TempVector.Sum(0, MaxIndex - 1)- TempVector.Sum(MaxIndex + 1,EndIndex);
	}
	double OutValueDenominator= sqrt((double)(ElemNum - 1) + pow(OriginMaxPosValue, 2));
	double OtherValue = 1.0 / OutValueDenominator;
	OutVec.Set(ElemNum, OtherValue);
	OutVec[MaxIndex] = OriginMaxPosValue / OutValueDenominator;
}

void CTanVector::GenOrthogonalVector(CTanVector & OutVec)
{
	GenOrthogonalVector(*this, OutVec);
}

double CTanVector::DotProduct(CTanVector & InVec1, CTanVector & InVec2)
{
	return cblas_ddot((int)InVec1.size(), &InVec1[0],1, &InVec2[0],1);
}

double CTanVector::DotProduct(CTanVector & InVec)
{
	return DotProduct(*this, InVec);
}

double CTanVector::ObtainNorm2(CTanVector & InVec)
{
	return cblas_dnrm2((int)InVec.size(),&InVec[0],1);
}

double CTanVector::ObtainNorm2()
{
	return ObtainNorm2(*this);
}

void CTanVector::CrossProduct(CTanVector & InVec1, CTanVector & InVec2, CTanVector & OutVec)
{
	OutVec.Set((int)InVec1.size(), 0.0);
	OutVec[0] = InVec1[1] * InVec2[2] - InVec1[2] * InVec2[1];
	OutVec[1] = InVec1[2] * InVec2[0] - InVec1[0] * InVec2[2];
	OutVec[2] = InVec1[0] * InVec2[1] - InVec1[1] * InVec2[0];
	
}

void CTanVector::CrossProduct(CTanVector & InVec, CTanVector & OutVec)
{
	CrossProduct(*this, InVec, OutVec);
}

CTanVector CTanVector::CrossProduct(CTanVector & InVec)
{
	CTanVector OutVector;
	CrossProduct(*this, InVec, OutVector);
	return OutVector;
}

double CTanVector::Normalize(CTanVector & InVec)
{
	double Norm = InVec.ObtainNorm2();
	std::for_each(InVec.begin(), InVec.end(), [&Norm](double& i1) {i1 = i1/ Norm; });
	return Norm;
}

double CTanVector::Normalize()
{
	return Normalize(*this);

}

void CTanVector::GenLocalCoor(CTanVector & InVec, CTanVector & OutXCoor, CTanVector & OutYCoor, CTanVector & OutZCoor)
{
	OutZCoor = InVec;
	OutZCoor.Normalize();
	InVec.GenOrthogonalVector(InVec, OutXCoor);
	OutZCoor.CrossProduct(OutXCoor, OutYCoor);
}

void CTanVector::GenCoorTransFromLocalToGlobal(CTanVector & InVec, double * Out1DTransMatrix)
{
	CTanVector TempVec[3];
	GenLocalCoor(InVec, TempVec[0], TempVec[1], TempVec[2]);
	cblas_dcopy(3, &TempVec[0][0], 1, &Out1DTransMatrix[0], 1);
	cblas_dcopy(3, &TempVec[1][0], 1, &Out1DTransMatrix[3], 1);
	cblas_dcopy(3, &TempVec[2][0], 1, &Out1DTransMatrix[6], 1);
}

