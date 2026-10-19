#include "CDenseMatrix.h"

CDenseMatrix::CDenseMatrix()
{
}

CDenseMatrix::~CDenseMatrix()
{
	DeleteDenseDataSpace();
}
void CDenseMatrix::CreateDenseDataSpace(int InRowCount, int InColumnCount, CDenseMatrix & OutDenseMatrix)
{
	DeleteDenseDataSpace(OutDenseMatrix);
	OutDenseMatrix.ColumnCount = InColumnCount;
	OutDenseMatrix.RowCount = InRowCount;
	OutDenseMatrix.Values = new double[InRowCount*InColumnCount]();
	OutDenseMatrix.IsCreatSpace = 1;
}

void CDenseMatrix::CreateDenseDataSpace(int InRowCount, int InColumnCount, double * InValues, CDenseMatrix & OutDenseMatrix)
{
	CreateDenseDataSpace(InRowCount, InColumnCount, OutDenseMatrix);
	std::copy(InValues, InValues + InRowCount * InColumnCount, OutDenseMatrix.Values);
}

//void CDenseMatrix::CreateDenseDataSpace(int InRowCount, int InColumnCount, double * InValues, CDenseMatrix & OutDenseMatrix)
//{
//	CreateDenseDataSpace(InRowCount, InColumnCount, OutDenseMatrix);
//	std::copy(InValues, InValues + InRowCount * InColumnCount, OutDenseMatrix.Values);
//}


void CDenseMatrix::CreateDenseDataSpace(int InRowCount, int InColumnCount)
{
	DeleteDenseDataSpace();
	CreateDenseDataSpace(InRowCount, InColumnCount, *this);
}

void CDenseMatrix::CreateDenseDataSpace(int InRowCount, int InColumnCount, double * InValues)
{
	DeleteDenseDataSpace();
	CreateDenseDataSpace(InRowCount, InColumnCount, InValues, *this);
}

void CDenseMatrix::DeleteDenseDataSpace(CDenseMatrix & InDenseMatrix)
{
	if (&InDenseMatrix)
	{
		if (InDenseMatrix.IsCreatSpace)
		{
			delete[] InDenseMatrix.Values;
		}
		InDenseMatrix.ColumnCount = 0;
		InDenseMatrix.RowCount = 0;
		InDenseMatrix.Values = 0;
		InDenseMatrix.IsCreatSpace = 0;
	}
}

void CDenseMatrix::DeleteDenseDataSpace()
{
	DeleteDenseDataSpace(*this);
}

bool CDenseMatrix::CheckDenseDataSpace(CDenseMatrix & InDenseMatrix)
{
	if (&InDenseMatrix)
	{
		if (InDenseMatrix.Values)
		{
			return 1;
		}
		else
		{
			printf("%s\n", "稠密矩阵对象数组未定义");
			return 0;
		}
	}
	else
	{
		printf("%s\n", "稠密矩阵对象未定义");
		return 0;
	}

}

bool CDenseMatrix::CheckDenseDataSpace()
{
	return CheckDenseDataSpace(*this);
}

void CDenseMatrix::ObtainIndexAddressInSingleRow(int RowIndex, TanIntVector & ColumnIndexList, CDenseMatrix & InMatrix, TanDoublePointerVector & OutColumnIndexAddressList)
{
	TanDoublePointerVector(ColumnIndexList.size()).swap(OutColumnIndexAddressList);
	int InitialOffset = RowIndex * InMatrix.ColumnCount;
	for (size_t i = 0; i < ColumnIndexList.size(); i++)
	{
		OutColumnIndexAddressList[i] = &InMatrix.Values[ColumnIndexList[i] + InitialOffset];
	}	
}

void CDenseMatrix::ObtainIndexAddressInSingleRow(int RowIndex, TanIntVector * ColumnIndexList, CDenseMatrix & InMatrix, TanDoublePointerVector & OutColumnIndexAddressList)
{
	if (ColumnIndexList)
	{
		ObtainIndexAddressInSingleRow(RowIndex, *ColumnIndexList, InMatrix, OutColumnIndexAddressList);
	}
	else
	{
		TanIntVector* TempColumnIndexList = new TanIntVector(InMatrix.ColumnCount);
		std::iota(TempColumnIndexList->begin(), TempColumnIndexList->end(), 0);
		ObtainIndexAddressInSingleRow(RowIndex, *TempColumnIndexList, InMatrix, OutColumnIndexAddressList);
		delete TempColumnIndexList;
	}
}

void CDenseMatrix::ObtainIndexAddressInSingleRow(int RowIndex, TanIntVector & ColumnIndexList, TanDoublePointerVector & OutColumnIndexAddressList)
{
	ObtainIndexAddressInSingleRow(RowIndex, ColumnIndexList, *this, OutColumnIndexAddressList);
}

void CDenseMatrix::ObtainIndexAddressInSingleRow(int RowIndex, TanIntVector * ColumnIndexList, TanDoublePointerVector & OutColumnIndexAddressList)
{
	ObtainIndexAddressInSingleRow(RowIndex, ColumnIndexList, *this, OutColumnIndexAddressList);
}

void CDenseMatrix::ObtainIndexValueInSingleRow(int RowIndex, TanIntVector & ColumnIndexList, CDenseMatrix & InMatrix, TanDoubleVector & OutColumnIndexListValue)
{
	TanDoubleVector(ColumnIndexList.size()).swap(OutColumnIndexListValue);
	int InitialOffset = RowIndex * InMatrix.ColumnCount;
	for (size_t i = 0; i < ColumnIndexList.size(); i++)
	{
		OutColumnIndexListValue[i] = InMatrix.Values[ColumnIndexList[i] + InitialOffset];
	}
}

void CDenseMatrix::ObtainIndexValueInSingleRow(int RowIndex, TanIntVector * ColumnIndexList, CDenseMatrix & InMatrix, TanDoubleVector & OutColumnIndexListValue)
{
	if (ColumnIndexList)
	{
		ObtainIndexValueInSingleRow(RowIndex, *ColumnIndexList, InMatrix, OutColumnIndexListValue);
	}
	else
	{
		TanIntVector* TempColumnIndexList = new TanIntVector(InMatrix.ColumnCount);
		std::iota(TempColumnIndexList->begin(), TempColumnIndexList->end(), 0);
		ObtainIndexValueInSingleRow(RowIndex, *TempColumnIndexList, InMatrix, OutColumnIndexListValue);
		delete TempColumnIndexList;
	}
}

void CDenseMatrix::ObtainIndexValueInSingleRow(int RowIndex, TanIntVector & ColumnIndexList, TanDoubleVector & OutColumnIndexListValue)
{
	ObtainIndexValueInSingleRow(RowIndex, ColumnIndexList, *this, OutColumnIndexListValue);
}

void CDenseMatrix::ObtainIndexValueInSingleRow(int RowIndex, TanIntVector * ColumnIndexList, TanDoubleVector & OutColumnIndexListValue)
{
	ObtainIndexValueInSingleRow(RowIndex, ColumnIndexList, *this, OutColumnIndexListValue);
}

