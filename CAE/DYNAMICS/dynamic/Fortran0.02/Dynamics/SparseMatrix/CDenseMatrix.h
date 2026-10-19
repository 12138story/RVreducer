#pragma once
#include "CommonDefine.h"
class CDenseMatrix
{
public:
	bool IsCreatSpace = 0;
	int RowCount = 0;
	int ColumnCount = 0;
	double* Values = 0;
	CDenseMatrix();
	
	~CDenseMatrix();
	//稠密矩阵的创建、析出与检查
	static void CreateDenseDataSpace(int InRowCount, int InColumnCount, CDenseMatrix& OutDenseMatrix);
	static void CreateDenseDataSpace(int InRowCount, int InColumnCount, double* InValues, CDenseMatrix& OutDenseMatrix);
	template <typename T1>
	static void CreateDenseDataSpace(int InRowCount, int InColumnCount, T1&& InValues, CDenseMatrix& OutDenseMatrix)
	{
		{
			CreateDenseDataSpace(InRowCount, InColumnCount, OutDenseMatrix);
			std::copy(InValues.begin(), InValues.end(), OutDenseMatrix.Values);
		}
	}
	void CreateDenseDataSpace(int InRowCount, int InColumnCount);
	void CreateDenseDataSpace(int InRowCount, int InColumnCount, double* InValues);
	static void DeleteDenseDataSpace(CDenseMatrix& InDenseMatrix);
	void DeleteDenseDataSpace();
	static bool CheckDenseDataSpace(CDenseMatrix& InDenseMatrix);
	bool CheckDenseDataSpace();
	template <typename T1>
	CDenseMatrix(int InRowCount, int InColumnCount, T1&& InValues)
	{
		CreateDenseDataSpace(InRowCount, InColumnCount, InValues, *this);
	}
	//提取某行某列地址
	static void ObtainIndexAddressInSingleRow(int RowIndex,
		TanIntVector& ColumnIndexList,
		CDenseMatrix& InMatrix,
		TanDoublePointerVector& OutColumnIndexAddressList);
	static void ObtainIndexAddressInSingleRow(int RowIndex,
		TanIntVector* ColumnIndexList,
		CDenseMatrix& InMatrix,
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
		CDenseMatrix& InMatrix,
		TanDoubleVector& OutColumnIndexListValue);
	static void ObtainIndexValueInSingleRow(int RowIndex,
		TanIntVector* ColumnIndexList,
		CDenseMatrix& InMatrix,
		TanDoubleVector& OutColumnIndexListValue);
	void ObtainIndexValueInSingleRow(int RowIndex,
		TanIntVector& ColumnIndexList,
		TanDoubleVector& OutColumnIndexListValue);
	void ObtainIndexValueInSingleRow(int RowIndex,
		TanIntVector* ColumnIndexList,
		TanDoubleVector& OutColumnIndexListValue);
};

