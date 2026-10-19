#pragma once
#include "CommonDefine.h"
#include "CDenseMatrix.h"
#include "CSparseMatrixCSR.h"
#include "CSparseMatrixMKL.h"
#include "NSTanBasicObjOpt.h"
#include "CSparseMatrixDiscreteCSR.h"
#include "mkl.h"

class CSparseMatrix
{

public:

	CDenseMatrix* pDENSE_FORMAT = 0;
	CSparseMatrixCSR* pCSR_FORMAT = 0;
	CSparseMatrixMKL* pMKL_FORMAT = 0;
	CSparseMatrixDiscreteCSR* pCSR_DISCRETE_FORMAT = 0;

	CSparseMatrix();
	~CSparseMatrix();
	//格式对象的创建、删除与检查
	void DeleteDenseObj();
	void DeleteCSRObj();
	void DeleteMKLObj();
	void DeleteDiscreteCSR();
	void DeleteAllObj();
	void CreateDenseObj();
	void CreateCSRObj();
	void CreateMKLObj();
	void CreateDiscreteCSRObj();
	void CreateAllObj();


	//稠密矩阵转化为CSR稀疏矩阵
	static void ConvertDenseToCSR(CDenseMatrix& InDenseMatrix, CSparseMatrixCSR & OutCSRMatrix);
	static void ConvertDenseToCSR(CDenseMatrix& InDenseMatrix, CSparseMatrixCSR *& OutCSRMatrix);
	void ConvertDenseToCSR(CDenseMatrix& InDenseMatrix);
	void ConvertDenseToCSR();
	static void ConvertCSRToDense(CSparseMatrixCSR& InCSRMatrix,CDenseMatrix& OutDenseMatrix);
	static void ConvertCSRToDense(CSparseMatrixCSR& InCSRMatrix, CDenseMatrix*& OutDenseMatrix);
	void ConvertCSRToDense(CSparseMatrixCSR& InCSRMatrix);
	void ConvertCSRToDense();
	//CSR稀疏矩阵与MKL矩阵间的转换
	static void ConvertCSRToMKL(CSparseMatrixCSR& InSparseMatrixCSR, CSparseMatrixMKL& OutSparseMatrixMKL);
	static void ConvertCSRToMKL(CSparseMatrixCSR& InSparseMatrixCSR, CSparseMatrixMKL*& OutSparseMatrixMKL);
	void ConvertCSRToMKL(CSparseMatrixCSR& InSparseMatrixCSR);
	void ConvertCSRToMKL();
	//MKL转换到CSR通过指针移动
	static void  ConvertMKLToCSR(CSparseMatrixMKL& InSparseMatrixMKL, CSparseMatrixCSR& OutSparseMatrixCSR);
	static void ConvertMKLToCSR(CSparseMatrixMKL& InSparseMatrixMKL, CSparseMatrixCSR*& OutSparseMatrixCSR);
	void  ConvertMKLToCSR(CSparseMatrixMKL& InSparseMatrixMKL);
	void  ConvertMKLToCSR();

	//MKL转换到CSR通过深拷贝
	static void  ConvertMKLToCSRByCopy(CSparseMatrixMKL& InSparseMatrixMKL, CSparseMatrixCSR& OutSparseMatrixCSR);
	void ConvertMKLToCSRByCopy(CSparseMatrixMKL& InSparseMatrixMKL);
	void ConvertMKLToCSRByCopy();
	//CSR稀疏矩阵转换到离散CSR矩阵
	static void ConvertCSRToCSRDiscrete(CSparseMatrixCSR& InSparseMatrixCSR,CSparseMatrixDiscreteCSR& OutSparseMatrixCSRDiscrete);
	static void ConvertCSRToCSRDiscrete(CSparseMatrixCSR& InSparseMatrixCSR, CSparseMatrixDiscreteCSR*& OutSparseMatrixCSRDiscrete);
	void ConvertCSRToCSRDiscrete(CSparseMatrixCSR& InSparseMatrixCSR);
	void ConvertCSRToCSRDiscrete();

	//离散CSR矩阵转换到CSR稀疏矩阵
	static void ConvertCSRDiscreteToCSR(CSparseMatrixDiscreteCSR& InSparseMatrixCSRDiscrete,CSparseMatrixCSR& OutSparseMatrixCSR);
	static void ConvertCSRDiscreteToCSR(CSparseMatrixDiscreteCSR& InSparseMatrixCSRDiscrete, CSparseMatrixCSR*& OutSparseMatrixCSR);
	void ConvertCSRDiscreteToCSR(CSparseMatrixDiscreteCSR& InSparseMatrixCSRDiscrete);
	void ConvertCSRDiscreteToCSR();
	//加法
	static void AddMatrixByMKL(CSparseMatrix& InAMatrix, CSparseMatrix& InBMatrix, CSparseMatrix& OutCMatrix);
	void AddMatrixByMKL(CSparseMatrix& InAMatrix, CSparseMatrix& InBMatrix);
	void AddMatrixByMKL(CSparseMatrix& InAMatrix);

	//乘法
	//static void MultiMatrixByMKL(tagSparseMatrixMKL& InAMatrix, tagSparseMatrixMKL& InBMatrix, tagSparseMatrixMKL& OutCMatrix);
	static void MultiMatrixByMKL(CSparseMatrix& InAMatrix, CSparseMatrix& InBMatrix, CSparseMatrix& OutCMatrix);
	void MultiMatrixByMKL(CSparseMatrix& InAMatrix, CSparseMatrix& InBMatrix);
	void MultiMatrixByMKL(CSparseMatrix& InBMatrix);
private:
	//判断TanIntVector指针是否存在，不存在这生成等比数列,若创建返回1，未创建返回0;
	static bool CheckMatrixIndexVector(TanIntVector*& InMatrixColumnIndexVector,int& MatrixColumnCount, TanIntVector*& OutMatrixColumnIndexVector);
public:
	//其它辅助函数
	//子矩阵相加，无检查
	template<typename T1, typename T2>
	static void AddSubMatrixByCSRDiscrete(
		TanIntVector& InAMatrixRowIndexList,
		TanIntVector& InAMatrixColumnIndexList,
		T1& InAMatrix,
		TanIntVector& InBMatrixRowIndexList,
		TanIntVector& InBMatrixColumnIndexList,
		T2& InBMatrix,
		TanIntVector& OutCMatrixRowIndexList,
		TanIntVector& OutCMatrixColumnIndexList,
		CSparseMatrixDiscreteCSR& OutCMatrix)
	{
		std::vector<TanDoubleVector> AMatrixValue(InAMatrixRowIndexList.size(), TanDoubleVector());
		std::vector<TanDoubleVector> BMatrixValue(InBMatrixRowIndexList.size(), TanDoubleVector());
		//OutCMatrix.Set((int)OutCMatrixRowIndexList.size(), (int)OutCMatrixColumnIndexList.size());
		for (int i = 0; i < InAMatrixRowIndexList.size(); i++)
		{
			InAMatrix.ObtainIndexValueInSingleRow(InAMatrixRowIndexList[i], InAMatrixColumnIndexList, AMatrixValue[i]);
			InBMatrix.ObtainIndexValueInSingleRow(InBMatrixRowIndexList[i], InBMatrixColumnIndexList, BMatrixValue[i]);
			cblas_daxpy((int)(InAMatrixColumnIndexList.size()), 1.0, &(AMatrixValue[i][0]), 1, &(BMatrixValue[i][0]), 1);
		}
		std::vector<std::map<int, double>::iterator> CMatrixAddress;
		for (int i = 0; i < InAMatrixRowIndexList.size(); i++)
		{
			OutCMatrix.ObtainIndexAddressInSingleRow(OutCMatrixRowIndexList[i], OutCMatrixColumnIndexList, OutCMatrix, CMatrixAddress);
			//考虑并行化
			OutCMatrix.AssignIndexValueInSingleRow(OutCMatrixRowIndexList[i], OutCMatrixColumnIndexList, CMatrixAddress, BMatrixValue[i], OutCMatrix);
		}
	}

	template<typename T1, typename T2>
	static void AddSubMatrixByCSRDiscrete(
		TanIntVector* InAMatrixRowIndexList,
		TanIntVector* InAMatrixColumnIndexList,
		T1& InAMatrix,
		TanIntVector* InBMatrixRowIndexList,
		TanIntVector* InBMatrixColumnIndexList,
		T2& InBMatrix,
		TanIntVector* OutCMatrixRowIndexList,
		TanIntVector* OutCMatrixColumnIndexList,
		CSparseMatrixDiscreteCSR& OutCMatrix)
	{
		TanIntVector* TempARowIndexList;
		TanIntVector* TempAColumnIndexList;
		TanIntVector* TempBRowIndexList;
		TanIntVector* TempBColumnIndexList;
		TanIntVector* TempCRowIndexList;
		TanIntVector* TempCColumnIndexList;
		bool IsCreateARowList = CheckMatrixIndexVector(InAMatrixRowIndexList, InAMatrix.RowCount, TempARowIndexList);
		bool IsCreateAColumnList = CheckMatrixIndexVector(InAMatrixColumnIndexList, InAMatrix.ColumnCount, TempAColumnIndexList);
		bool IsCreateBRowList = CheckMatrixIndexVector(InBMatrixRowIndexList, InBMatrix.RowCount, TempBRowIndexList);
		bool IsCreateBColumnList = CheckMatrixIndexVector(InBMatrixColumnIndexList, InBMatrix.ColumnCount, TempBColumnIndexList);
		bool IsCreateCRowList = CheckMatrixIndexVector(OutCMatrixRowIndexList, OutCMatrix.RowCount, TempCRowIndexList);
		bool IsCreateCColumnList = CheckMatrixIndexVector(OutCMatrixColumnIndexList, OutCMatrix.ColumnCount, TempCColumnIndexList);
		AddSubMatrixByCSRDiscrete(*TempARowIndexList, *TempAColumnIndexList, InAMatrix,
			*TempBRowIndexList, *TempBColumnIndexList, InBMatrix,
			*TempCRowIndexList, *TempCColumnIndexList, OutCMatrix);
		if (IsCreateAColumnList)
		{
			delete TempAColumnIndexList;
		}
		if (IsCreateBColumnList)
		{
			delete TempBColumnIndexList;
		}
		if (IsCreateCColumnList)
		{
			delete TempCColumnIndexList;
		}

		if (IsCreateARowList)
		{
			delete TempARowIndexList;
		}
		if (IsCreateBRowList)
		{
			delete TempBRowIndexList;
		}
		if (IsCreateCRowList)
		{
			delete TempCRowIndexList;
		}
	}
	//提取CSR格式子矩阵到CSR格式
	template<typename T1>
	static void ObtainSubMatrixFromCSR(
		TanIntVector& InSubMatrixRowIndexList,
		TanIntVector& InSubMatrixColumnIndexList,
		T1& InMatrix,
		CSparseMatrixCSR& OutMatrix)
	{
		int SubMatrixRowCount = (int)InSubMatrixRowIndexList.size();
		int SubMatrixColumnCount = (int)InSubMatrixColumnIndexList.size();
		std::vector<TanIntpDoubleMap> AssignValueAddressTable(SubMatrixRowCount,TanIntpDoubleMap());
		TanIntVector RowElemCount(SubMatrixRowCount, 0);
		TanIntVector EachRowElemEnd(SubMatrixRowCount, 0);
		
		for (int i=0;i< SubMatrixRowCount;i++)
		{
			InMatrix.ObtainIndexValueInSingleRow(InSubMatrixRowIndexList[i], InSubMatrixColumnIndexList, AssignValueAddressTable[i]);
			RowElemCount[i] = (int)AssignValueAddressTable[i].size();
		}
		std::partial_sum(RowElemCount.begin(), RowElemCount.end(), EachRowElemEnd.begin());
		int TotalElemCount = EachRowElemEnd[SubMatrixRowCount-1];
		OutMatrix.CreateCSRDataSpace(SubMatrixRowCount, SubMatrixColumnCount, TotalElemCount);
		std::copy(EachRowElemEnd.begin(), EachRowElemEnd.end() - 1, OutMatrix.RowStartIndex + 1);
		std::copy(EachRowElemEnd.begin(), EachRowElemEnd.end(), OutMatrix.RowEndIndex);
		for (int i = 0; i < SubMatrixRowCount; i++)
		{
			auto IndexIter = AssignValueAddressTable[i].begin();
			for (int j = 0; j < RowElemCount[i]; j++)
			{
				OutMatrix.ColumnIndex[j + OutMatrix.RowStartIndex[i]] = IndexIter->first;
				OutMatrix.Values[j + OutMatrix.RowStartIndex[i]] = *IndexIter->second;
				IndexIter++;
			}
		}
	}
	//提取CSR格式子矩阵到CSR格式
	template<typename T1>
	static void ObtainSubMatrixFromCSR(
		TanIntVector* InSubMatrixRowIndexList,
		TanIntVector* InSubMatrixColumnIndexList,
		T1& InMatrix,
		CSparseMatrixCSR& OutMatrix)
	{
		TanIntVector* TempSubMatrixRowIndexList;
		TanIntVector* TempSubMatrixColumnIndexList;
		bool IsCreateSubMatrixRowList = CheckMatrixIndexVector(InSubMatrixRowIndexList, InMatrix.RowCount, TempSubMatrixRowIndexList);
		bool IsCreateSubColumnList = CheckMatrixIndexVector(InSubMatrixColumnIndexList, InMatrix.ColumnCount, TempSubMatrixColumnIndexList);
		ObtainSubMatrixFromCSR(*TempSubMatrixRowIndexList, *TempSubMatrixColumnIndexList, InMatrix, OutMatrix);
		if (IsCreateSubMatrixRowList)
		{
			delete TempSubMatrixRowIndexList;
		}
		if (IsCreateSubColumnList)
		{
			delete TempSubMatrixColumnIndexList;
		}
	}
};
//废码
		//子矩阵单行相加，无检查

	//template<typename T1,typename T2>
	//static void AddSingleRowMatrixByCSRDiscrete(
	//	int InAMatrixRowIndex,
	//	TanIntVector& InAMatrixColumnIndexList,
	//	T1& InAMatrix,
	//	int InBMatrixRowIndex,
	//	TanIntVector& InBMatrixColumnIndexList,
	//	T2& InBMatrix,
	//	int OutCMatrixRowIndex,
	//	TanIntVector& OutCMatrixColumnIndexList,
	//	CSparseMatrixDiscreteCSR& OutCMatrix)
	//{
	//	//TanDoubleVector AMatrixValue;
	//	//TanDoubleVector BMatrixValue;
	//	TanDoubleVector AMatrixValue;
	//	TanDoubleVector BMatrixValue;
	//	InAMatrix.ObtainIndexValueInSingleRow(InAMatrixRowIndex, InAMatrixColumnIndexList, AMatrixValue);
	//	InBMatrix.ObtainIndexValueInSingleRow(InBMatrixRowIndex, InAMatrixColumnIndexList, BMatrixValue);
	//	cblas_daxpy((int)OutCMatrixColumnIndexList.size(), 1.0, &(AMatrixValue[0]), 1, &(BMatrixValue[0]), 1);
	//	std::vector<std::map<int, double>::iterator> CMatrixAddress;
	//	OutCMatrix.ObtainIndexAddressInSingleRow(OutCMatrixRowIndex, OutCMatrixColumnIndexList, OutCMatrix, CMatrixAddress);
	//	//考虑并行化
	//	OutCMatrix.AssignIndexValueInSingleRow(OutCMatrixRowIndex, OutCMatrixColumnIndexList, CMatrixAddress, BMatrixValue, OutCMatrix);
	//}
	//template<typename T1, typename T2>
	//static void AddSingleRowMatrixByCSRDiscrete(int InAMatrixRowIndex,
	//	TanIntVector* InAMatrixColumnIndexList,
	//	T1& InAMatrix,
	//	int InBMatrixRowIndex,
	//	TanIntVector* InBMatrixColumnIndexList,
	//	T2& InBMatrix,
	//	int OutCMatrixRowIndex,
	//	TanIntVector* OutCMatrixColumnIndexList,
	//	CSparseMatrixDiscreteCSR& OutCMatrix)
	//{
	//	TanIntVector* TempAColumnIndexList;
	//	TanIntVector* TempBColumnIndexList;
	//	TanIntVector* TempCColumnIndexList;
	//	bool IsCreateAColumnList = CheckMatrixIndexVector(InAMatrixColumnIndexList, InAMatrix.ColumnCount, TempAColumnIndexList);
	//	bool IsCreateBColumnList = CheckMatrixIndexVector(InBMatrixColumnIndexList, InBMatrix.ColumnCount, TempBColumnIndexList);
	//	bool IsCreateCColumnList = CheckMatrixIndexVector(OutCMatrixColumnIndexList, OutCMatrix.ColumnCount, TempCColumnIndexList);
	//	AddSingleRowMatrixByCSRDiscrete(InAMatrixRowIndex, *TempAColumnIndexList, InAMatrix,
	//		InBMatrixRowIndex, *TempBColumnIndexList, InBMatrix,
	//		OutCMatrixRowIndex, *TempCColumnIndexList, OutCMatrix);
	//	if (IsCreateAColumnList)
	//	{
	//		delete TempAColumnIndexList;
	//	}
	//	if (IsCreateBColumnList)
	//	{
	//		delete TempBColumnIndexList;
	//	}
	//	if (IsCreateCColumnList)
	//	{
	//		delete TempCColumnIndexList;
	//	}
	//}

