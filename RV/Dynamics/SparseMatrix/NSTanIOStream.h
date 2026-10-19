#pragma once
#include "CommonDefine.h"
#include "CSparseMatrixCSR.h"
#include "CDenseMatrix.h"
namespace NSTanIOStream
{
	template <typename T1>
	void PrintVectorInConsole(T1&& InVector)
	{
		for (int i=0;i< InVector.size();i++)
		{
			std::cout << InVector[i] << "\t";
		}
		std::cout << "\n";
	}
	void PrintMatrixInConsole(int RowNum, int ColumnNum, double* InMatrix);
	void PrintMatrixInConsole(int RowNum, int ColumnNum, double** InMatrix);
	void PrintMatrixInConsole(CDenseMatrix&  InDenseMatrix);
	void PrintCSRSparseMatrixInConsole(CSparseMatrixCSR&  InCSRMatrix);	
};

