#pragma once
#include "CommonDefine.h"
#include "NSTanBasicObjOpt.h"
class CSparseMatrixCSR
{
public:
	bool IsCreatSpace = 0;
	//bool IsCSR4Formate = 0;
	int RowCount = 0;
	int ColumnCount = 0;
	double* Values = 0;
	int* ColumnIndex = 0;
	int* RowStartIndex = 0;
	int* RowEndIndex = 0;
	CSparseMatrixCSR();
	~CSparseMatrixCSR();
	//CSR格式矩阵的创建、析出与检查
	static void CreateCSRDataSpace(int InRowCount, int InColumnCount, int InNonZeroElemCount, CSparseMatrixCSR& OutCSRMatrix);//默认创建方法是CSR3格式
	void CreateCSRDataSpace(int InRowCount, int InColumnCount, int InNonZeroElemCount);
	static void DeleteCSRDataSpace(CSparseMatrixCSR& InCSRMatrix);
	void DeleteCSRDataSpace();
	static bool CheckCSRDataSpace(CSparseMatrixCSR& InSparseMatrixCSR);
	bool CheckCSRDataSpace();
	static bool IsCSR4Formate(CSparseMatrixCSR& InSparseMatrixCSR);
	bool IsCSR4Formate();
	//深拷贝CSR格式矩阵
	static void CopyCSR(CSparseMatrixCSR& FromMatrix, CSparseMatrixCSR& ToMatrix);
	//提取某行某列地址
	static void ObtainIndexAddressInSingleRow(int RowIndex,
		TanIntVector& ColumnIndexList,
		CSparseMatrixCSR& InMatrix,
		TanDoublePointerVector& OutColumnIndexAddressList);
	static void ObtainIndexAddressInSingleRow(int RowIndex,
		TanIntVector* ColumnIndexList,
		CSparseMatrixCSR& InMatrix,
		TanDoublePointerVector& OutColumnIndexAddressList);
	void ObtainIndexAddressInSingleRow(int RowIndex,
		TanIntVector& ColumnIndexList,
		TanDoublePointerVector& OutColumnIndexAddressList);
	void ObtainIndexAddressInSingleRow(int RowIndex,
		TanIntVector* ColumnIndexList,
		TanDoublePointerVector& OutColumnIndexAddressList);
	//提取某行某列值
	static void ObtainIndexValueInSingleRow(int RowIndex,
		TanIntVector& ColumnIndexList,
		CSparseMatrixCSR& InMatrix,
		TanDoubleVector& OutColumnIndexListValue);
	static void ObtainIndexValueInSingleRow(int RowIndex,
		TanIntVector* ColumnIndexList,
		CSparseMatrixCSR& InMatrix,
		TanDoubleVector& OutColumnIndexListValue);
	void ObtainIndexValueInSingleRow(int RowIndex,
		TanIntVector& ColumnIndexList,
		TanDoubleVector& OutColumnIndexListValue);
	void ObtainIndexValueInSingleRow(int RowIndex,
		TanIntVector* ColumnIndexList,
		TanDoubleVector& OutColumnIndexListValue);
	//提取某行某列输出形式为map
	static void ObtainIndexValueInSingleRow(int RowIndex,
		TanIntVector& ColumnIndexList,
		CSparseMatrixCSR& InMatrix,
		TanIntpDoubleMap& OutColumnIndexValuePointerList);
	void ObtainIndexValueInSingleRow(int RowIndex,
		TanIntVector& ColumnIndexList,
		TanIntpDoubleMap& OutColumnIndexValuePointerList);
	//求解线性方程,基于CSR3格式
	static void SolveLinearEquByParallelDSS(CSparseMatrixCSR& InCSRMatrix,double* InEquRight,double* OutSolution);
	void SolveLinearEquByParallelDSS(double* InEquRight, double* OutSolution);
};

