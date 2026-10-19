#pragma once
#include "CommonDefine.h"
class CSparseMatrixDiscreteCSR:
	public std::vector<std::map<int, double>>
{
public:
	CSparseMatrixDiscreteCSR();
	//CSparseMatrixDiscreteCSR(int InRowCount);
	CSparseMatrixDiscreteCSR(int InRowCount, int InColumnCount);
	~CSparseMatrixDiscreteCSR();
	//std::vector<std::map<int, double>> Data;
	int ColumnCount = 0;
	int RowCount = 0;
	//static void Set(int InRowCount, CSparseMatrixDiscreteCSR& OutMatrix);
	//void Set(int InRowCount);
	static void Set(int InRowCount, int InCloumnCount, CSparseMatrixDiscreteCSR& OutMatrix);
	void Set(int InRowCount, int InCloumnCount);
	static bool CheckDiscreteCSRDataSpace(CSparseMatrixDiscreteCSR& InMatrix);
	bool CheckDiscreteCSRDataSpace();
	//提取某行某列迭代器
	static void ObtainIndexAddressInSingleRow(int RowIndex,
		TanIntVector& ColumnIndexList,
		CSparseMatrixDiscreteCSR& InMatrix,
		std::vector<std::map<int, double>::iterator>& OutColumnIteratorAddressList);

	//提取某行某列地址
	static void ObtainIndexAddressInSingleRow(int RowIndex,
		TanIntVector& ColumnIndexList,
		CSparseMatrixDiscreteCSR& InMatrix,
		TanDoublePointerVector& OutColumnIndexAddressList);
	static void ObtainIndexAddressInSingleRow(int RowIndex,
		TanIntVector* ColumnIndexList,
		CSparseMatrixDiscreteCSR& InMatrix,
		TanDoublePointerVector& OutColumnIndexAddressList);
	void ObtainIndexAddressInSingleRow(int RowIndex,
		TanIntVector& ColumnIndexList,
		TanDoublePointerVector& OutColumnIndexAddressList);
	void ObtainIndexAddressInSingleRow(int RowIndex,
		TanIntVector* ColumnIndexList,
		TanDoublePointerVector& OutColumnIndexAddressList);
	

	//提取某行的列值
	static void ObtainIndexValueInSingleRow(int RowIndex,
		TanIntVector& ColumnIndexList,
		CSparseMatrixDiscreteCSR& InMatrix,
		TanDoubleVector& OutColumnIndexListValue);
	static void ObtainIndexValueInSingleRow(int RowIndex,
		TanIntVector* ColumnIndexList,
		CSparseMatrixDiscreteCSR& InMatrix,
		TanDoubleVector& OutColumnIndexListValue);
	void ObtainIndexValueInSingleRow(int RowIndex,
		TanIntVector& ColumnIndexList,
		TanDoubleVector& OutColumnIndexListValue);
	void ObtainIndexValueInSingleRow(int RowIndex,
		TanIntVector* ColumnIndexList,
		TanDoubleVector& OutColumnIndexListValue);
	//提取某行的列值,输出map形式
	static void ObtainIndexValueInSingleRow(int RowIndex,
		TanIntVector& ColumnIndexList,
		CSparseMatrixDiscreteCSR& InMatrix,
		TanIntpDoubleMap& OutColumnIndexValuePointerList);
	void ObtainIndexValueInSingleRow(int RowIndex,
		TanIntVector& ColumnIndexList,
		TanIntpDoubleMap& OutColumnIndexValuePointerList);
	//赋予某行的列值
	static void AssignIndexValueInSingleRow(int RowIndex,
		TanIntVector& ColumnIndexList,
		TanDoubleVector & InColumnIndexListValue,
		CSparseMatrixDiscreteCSR & OutMatrix);
	static void AssignIndexValueInSingleRow(int RowIndex,
		TanIntVector & ColumnIndexList,
		std::vector<std::map<int, double>::iterator>& ColumnIndexIteratorList,
		TanDoubleVector & InColumnIndexListValue,
		CSparseMatrixDiscreteCSR & OutMatrix);
};

