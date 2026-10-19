#pragma once
#include "CommonDefine.h"
#include "CSparseMatrixCSR.h"
#include "NSTanBasicObjOpt.h"
class CSparseMatrixMKL
{
public:
	CSparseMatrixMKL();
	~CSparseMatrixMKL();
	bool IsCreatHandleSpace = 0;
	sparse_matrix_t* MKLSparseHandle = 0;
	CSparseMatrixCSR* FromCSRMatrix = 0;

	//MKLIternal格式矩阵的创建、析出与检查
	static void DeleteMKLDataSpace(CSparseMatrixMKL& InMKLMatrix);
	void DeleteMKLDataSpace();
	//未声明是否创建句柄指向空间
	static void CreateMKLDataSpace(CSparseMatrixMKL& InMKLMatrix);
	void CreateMKLDataSpace();
	static bool CheckMKLDataSpace(CSparseMatrixMKL& InSparseMatrixMKL);
	bool CheckMKLDataSpace();
	//MKL矩阵拷贝
	static void CopyMKL(CSparseMatrixMKL& FromMatrix, CSparseMatrixMKL& ToMatrix);
	//基于MKL的稀疏矩阵运算
	//加法
	static void AddMatrix(
		int InAMatrixOptType, //0-不转置，1-转置，2-共轭转置(目前用不上，因为没有使用复数)
		double InAMatrixCoef,		
		CSparseMatrixMKL& InAMatrix,
		CSparseMatrixMKL& InBMatrix,
		CSparseMatrixMKL& OutCMatrix);
	void AddMatrix(
		int InAMatrixOptType,
		double InAMatrixCoef, 
		CSparseMatrixMKL& InAMatrix,
		CSparseMatrixMKL& InBMatrix);
	void AddMatrix(
		int InAMatrixOptType,
		double InAMatrixCoef, 
		CSparseMatrixMKL& InAMatrix);
	//乘法
	static void MultiMatrix(
		int InAMatrixOptType, //0-不转置，1-转置，2-共轭转置(目前用不上，因为没有使用复数)
		CSparseMatrixMKL& InAMatrix,
		CSparseMatrixMKL& InBMatrix,
		CSparseMatrixMKL& OutCMatrix);
	void MultiMatrix(int InAMatrixOptType, CSparseMatrixMKL& InAMatrix, CSparseMatrixMKL& InBMatrix);
	void MultiMatrix(int InAMatrixOptType, CSparseMatrixMKL& InBMatrix);
	//乘以矢量
	static void MultiVector(
		int InAMatrixOptType, //0-不转置，1-转置，2-共轭转置(目前用不上，因为没有使用复数)
		int RowCount,
		CSparseMatrixMKL& InAMatrix,
		double* InVector,
		double* OutVector);
	void MultiVector(
		int InAMatrixOptType, //0-不转置，1-转置，2-共轭转置(目前用不上，因为没有使用复数)
		int RowCount,
		double* InVector,
		double* OutVector);
	//求解线性方程
	//求解方法，0LU分解方法
	static void SolveLinearEquation(int InSolveMethod,int InUnkownCount, CSparseMatrixMKL& InSolveMatrix,double*& InEquRight,double*& SolvedValueList);
	////提取子矩阵
	//void ObtainSubMatrix(
	//	TanIntVector& InAMatrixRowIndexList,
	//	TanIntVector& InAMatrixColumnIndexList, 
	//	CSparseMatrixMKL& InAMatrix, 
	//	CSparseMatrixMKL& OutBMatrix);
};

