#include "NSTanIOStream.h"


void NSTanIOStream::PrintMatrixInConsole(int RowNum, int ColumnNum, double * InMatrix)
{
	int i = 0;
	int j = 0;
	for (i = 0; i < RowNum; i++)
	{
		for (j = 0; j < ColumnNum; j++)
		{
			std::cout << InMatrix[j + i * ColumnNum] << "\t";
		}
		std::cout << std::endl;
	}
}

void NSTanIOStream::PrintMatrixInConsole(int RowNum, int ColumnNum, double** InMatrix)
{
	int i = 0;
	int j = 0;
	for (i = 0; i < RowNum; i++)
	{
		for (j = 0; j < ColumnNum; j++)
		{
			std::cout << *((double*)InMatrix+j + i * ColumnNum) << "\t";
		}
		std::cout << std::endl;
	}
}

void NSTanIOStream::PrintMatrixInConsole(CDenseMatrix & InDenseMatrix)
{
	PrintMatrixInConsole(InDenseMatrix.RowCount, InDenseMatrix.ColumnCount, InDenseMatrix.Values);
}

void NSTanIOStream::PrintCSRSparseMatrixInConsole(CSparseMatrixCSR & InCSRMatrix)
{
	int i = 0;
	for (i = 0; i < InCSRMatrix.RowEndIndex[InCSRMatrix.RowCount-1]; i++)
	{
		std::cout << InCSRMatrix.Values[i] << "\t";
	}
	std::cout << "\n";
	for (i = 0; i < InCSRMatrix.RowEndIndex[InCSRMatrix.RowCount-1]; i++)
	{
		std::cout << InCSRMatrix.ColumnIndex[i] << "\t";
	}

	std::cout << "\n";
	for (i = 0; i < InCSRMatrix.RowCount; i++)
	{
		std::cout << InCSRMatrix.RowStartIndex[i] << "\t";
	}
	std::cout << "\n";
	for (i = 0; i < InCSRMatrix.RowCount; i++)
	{
		std::cout << InCSRMatrix.RowEndIndex[i] << "\t";
	}
	std::cout << "\n";
}
