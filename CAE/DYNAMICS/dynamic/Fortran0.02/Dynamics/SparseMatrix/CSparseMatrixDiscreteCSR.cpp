#include "CSparseMatrixDiscreteCSR.h"

CSparseMatrixDiscreteCSR::CSparseMatrixDiscreteCSR()
{
}

//CSparseMatrixDiscreteCSR::CSparseMatrixDiscreteCSR(int InRowCount)
//{
//	Set(InRowCount,*this);
//}

CSparseMatrixDiscreteCSR::CSparseMatrixDiscreteCSR(int InRowCount, int InColumnCount)
{
	Set(InRowCount, InColumnCount, *this);
}

CSparseMatrixDiscreteCSR::~CSparseMatrixDiscreteCSR()
{
}

//void CSparseMatrixDiscreteCSR::Set(int InRowCount, CSparseMatrixDiscreteCSR& OutMatrix)
//{
//	std::vector<std::map<int, double>>(InRowCount, std::map<int, double>()).swap(OutMatrix);
//	OutMatrix.RowCount = InRowCount;
//}

//void CSparseMatrixDiscreteCSR::Set(int InRowCount)
//{
//	Set(InRowCount, *this);
//}

void CSparseMatrixDiscreteCSR::Set(int InRowCount, int InCloumnCount, CSparseMatrixDiscreteCSR & OutMatrix)
{
	OutMatrix.ColumnCount = InCloumnCount;
	std::vector<std::map<int, double>>(InRowCount, std::map<int, double>()).swap(OutMatrix);
	//Set(InRowCount, OutMatrix);
}

void CSparseMatrixDiscreteCSR::Set(int InRowCount, int InCloumnCount)
{
	Set(InRowCount, InCloumnCount, *this);
}

bool CSparseMatrixDiscreteCSR::CheckDiscreteCSRDataSpace(CSparseMatrixDiscreteCSR & InMatrix)
{
	if (& InMatrix)
	{
		if (InMatrix.size())
		{
			if (InMatrix[0].size())
			{
				return 1;
			}
			else
			{
				printf("%s\n", "离散CSR矩阵未定义列");
				return 0;
			}

		}
		else
		{
			printf("%s\n", "离散CSR矩阵未定义行");
			return 0;
		}
	}
	else
	{
		printf("%s\n", "离散CSR矩阵未定义对象");
		return 0;
	}

}

bool CSparseMatrixDiscreteCSR::CheckDiscreteCSRDataSpace()
{
	return CheckDiscreteCSRDataSpace(*this);
}

void CSparseMatrixDiscreteCSR::ObtainIndexAddressInSingleRow(int RowIndex, TanIntVector & ColumnIndexList, CSparseMatrixDiscreteCSR & InMatrix, std::vector<std::map<int, double>::iterator>& OutColumnIteratorAddressList)
{
	//寻找所求元素在稀疏矩阵的位置，若不存在则地址为0；
	int ColumnCount = (int)ColumnIndexList.size();
	std::vector<std::map<int, double>::iterator>(ColumnCount, InMatrix[RowIndex].end()).swap(OutColumnIteratorAddressList);
	std::map<int, double>::iterator ColumnIndexIter = InMatrix[RowIndex].begin();
	for (int i = 0; i < ColumnCount; i++)
	{
		ColumnIndexIter = InMatrix[RowIndex].find(ColumnIndexList[i]);
		if (ColumnIndexIter != InMatrix[RowIndex].end())
		{
			OutColumnIteratorAddressList[i] = ColumnIndexIter;
		}
	}
}

void CSparseMatrixDiscreteCSR::ObtainIndexAddressInSingleRow(int RowIndex, TanIntVector & ColumnIndexList, CSparseMatrixDiscreteCSR & InMatrix, TanDoublePointerVector & OutColumnIndexAddressList)
{
	if (InMatrix.CheckDiscreteCSRDataSpace())
	{

		//寻找所求元素在稀疏矩阵的位置，若不存在则地址为0；
		int ColumnCount = (int)ColumnIndexList.size();
		TanDoublePointerVector(ColumnCount, 0).swap(OutColumnIndexAddressList);
		std::map<int, double>::iterator ColumnIndexIter = InMatrix[RowIndex].begin();
		for (int i = 0; i < ColumnCount; i++)
		{
			ColumnIndexIter = InMatrix[RowIndex].find(ColumnIndexList[i]);
			if (ColumnIndexIter != InMatrix[RowIndex].end())
			{
				OutColumnIndexAddressList[i] = &ColumnIndexIter->second;
			}
		}
	}
	else
	{
		printf("%s\n", "提取离散CSR子矩阵地址时出现错误");
	}
}

void CSparseMatrixDiscreteCSR::ObtainIndexAddressInSingleRow(int RowIndex, TanIntVector * ColumnIndexList, CSparseMatrixDiscreteCSR & InMatrix, TanDoublePointerVector & OutColumnIndexAddressList)
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
void CSparseMatrixDiscreteCSR::ObtainIndexAddressInSingleRow(int RowIndex, TanIntVector & ColumnIndexList, TanDoublePointerVector & OutColumnIndexAddressList)
{
	ObtainIndexAddressInSingleRow(RowIndex, ColumnIndexList, *this, OutColumnIndexAddressList);
}
void CSparseMatrixDiscreteCSR::ObtainIndexAddressInSingleRow(int RowIndex,
	TanIntVector* ColumnIndexList,
	TanDoublePointerVector& OutColumnIndexAddressList)
{
	ObtainIndexAddressInSingleRow(RowIndex, ColumnIndexList, *this, OutColumnIndexAddressList);
}

void CSparseMatrixDiscreteCSR::ObtainIndexValueInSingleRow(int RowIndex, TanIntVector & ColumnIndexList, CSparseMatrixDiscreteCSR & InMatrix, TanDoubleVector & OutColumnIndexListValue)
{
	//寻找所求元素在稀疏矩阵的位置，若不存在则地址为0；
	int ColumnCount = (int)ColumnIndexList.size();
	TanDoubleVector(ColumnCount, 0).swap(OutColumnIndexListValue);
	std::map<int, double>::iterator ColumnIndexIter = InMatrix[RowIndex].begin();
	for (int i = 0; i < ColumnCount; i++)
	{
		ColumnIndexIter = InMatrix[RowIndex].find(ColumnIndexList[i]);
		if (ColumnIndexIter != InMatrix[RowIndex].end())
		{
			OutColumnIndexListValue[i] = ColumnIndexIter->second;
		}
	}
}

void CSparseMatrixDiscreteCSR::ObtainIndexValueInSingleRow(int RowIndex, TanIntVector * ColumnIndexList, CSparseMatrixDiscreteCSR & InMatrix, TanDoubleVector & OutColumnIndexListValue)
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
void CSparseMatrixDiscreteCSR::ObtainIndexValueInSingleRow(int RowIndex, TanIntVector & ColumnIndexList, TanDoubleVector & OutColumnIndexListValue)
{
	ObtainIndexValueInSingleRow(RowIndex, ColumnIndexList, *this, OutColumnIndexListValue);
}
void CSparseMatrixDiscreteCSR::ObtainIndexValueInSingleRow(int RowIndex,
	TanIntVector* ColumnIndexList,
	TanDoubleVector& OutColumnIndexListValue)
{
	ObtainIndexValueInSingleRow(RowIndex, ColumnIndexList, *this, OutColumnIndexListValue);
}

void CSparseMatrixDiscreteCSR::ObtainIndexValueInSingleRow(int RowIndex, TanIntVector & ColumnIndexList, CSparseMatrixDiscreteCSR & InMatrix, TanIntpDoubleMap& OutColumnIndexValuePointerList)
{
	//寻找所求元素在稀疏矩阵的位置，若不存在则地址为0；
	int ColumnCount = (int)ColumnIndexList.size();
	OutColumnIndexValuePointerList.clear();
	std::map<int, double>::iterator ColumnIndexIter = InMatrix[RowIndex].begin();
	for (int i = 0; i < ColumnCount; i++)
	{
		ColumnIndexIter = InMatrix[RowIndex].find(ColumnIndexList[i]);
		if (ColumnIndexIter != InMatrix[RowIndex].end())
		{
			OutColumnIndexValuePointerList[i] = &(ColumnIndexIter->second);
		}
	}
}

void CSparseMatrixDiscreteCSR::ObtainIndexValueInSingleRow(int RowIndex, TanIntVector & ColumnIndexList, TanIntpDoubleMap& OutColumnIndexValuePointerList)
{
	ObtainIndexValueInSingleRow(RowIndex, ColumnIndexList, *this, OutColumnIndexValuePointerList);
}

void CSparseMatrixDiscreteCSR::AssignIndexValueInSingleRow(int RowIndex, TanIntVector & ColumnIndexList,  TanDoubleVector & InColumnIndexListValue, CSparseMatrixDiscreteCSR & OutMatrix)
{
	TanDoublePointerVector CMatrixAddress;
	ObtainIndexAddressInSingleRow(RowIndex, ColumnIndexList, OutMatrix, CMatrixAddress);
	for (int i = 0; i < ColumnIndexList.size(); i++)
	{
		if (InColumnIndexListValue[i])
		{
			if (CMatrixAddress[i])
			{
				*CMatrixAddress[i] = InColumnIndexListValue[i];
			}
			else
			{
				OutMatrix[RowIndex][ColumnIndexList[i]] = InColumnIndexListValue[i];
			}
		}
		else
		{
			if (CMatrixAddress[i])
			{
				OutMatrix[RowIndex].erase(OutMatrix[RowIndex].find(ColumnIndexList[i]));
			}
		}
	}
}

void CSparseMatrixDiscreteCSR::AssignIndexValueInSingleRow(int RowIndex, TanIntVector & ColumnIndexList, std::vector<std::map<int, double>::iterator>& ColumnIndexIteratorList, TanDoubleVector & InColumnIndexListValue, CSparseMatrixDiscreteCSR & OutMatrix)
{
	for (int i = 0; i < ColumnIndexIteratorList.size(); i++)
	{
		if (InColumnIndexListValue[i])
		{
			if (ColumnIndexIteratorList[i] != OutMatrix[RowIndex].end())
			{
				ColumnIndexIteratorList[i]->second = InColumnIndexListValue[i];
			}
			else
			{
				OutMatrix[RowIndex][ColumnIndexList[i]] = InColumnIndexListValue[i];
			}
		}
		else
		{
			if (ColumnIndexIteratorList[i] != OutMatrix[RowIndex].end())
			{
				OutMatrix[RowIndex].erase(ColumnIndexIteratorList[i]);
			}
		}
	}

}
