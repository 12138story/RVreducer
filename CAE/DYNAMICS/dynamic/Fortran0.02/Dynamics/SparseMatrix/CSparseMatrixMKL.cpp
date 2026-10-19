#include "CSparseMatrixMKL.h"

CSparseMatrixMKL::CSparseMatrixMKL()
{
}

CSparseMatrixMKL::~CSparseMatrixMKL()
{
	DeleteMKLDataSpace();
}
void CSparseMatrixMKL::DeleteMKLDataSpace(CSparseMatrixMKL& InMKLMatrix)
{
	if (&InMKLMatrix)
	{
		if (&(InMKLMatrix.MKLSparseHandle))
		{
			if (InMKLMatrix.IsCreatHandleSpace)
			{
				mkl_sparse_destroy(*InMKLMatrix.MKLSparseHandle);
				InMKLMatrix.IsCreatHandleSpace = 0;
			}
			delete InMKLMatrix.MKLSparseHandle;
		}
		InMKLMatrix.MKLSparseHandle = 0;
		InMKLMatrix.FromCSRMatrix = 0;
	}

}

void CSparseMatrixMKL::DeleteMKLDataSpace()
{
	DeleteMKLDataSpace(*this);
}


void CSparseMatrixMKL::CreateMKLDataSpace(CSparseMatrixMKL& InMKLMatrix)
{
	DeleteMKLDataSpace(InMKLMatrix);
	InMKLMatrix.MKLSparseHandle = new sparse_matrix_t;
}


void CSparseMatrixMKL::CreateMKLDataSpace()
{
	CreateMKLDataSpace(*this);
}


bool CSparseMatrixMKL::CheckMKLDataSpace(CSparseMatrixMKL & InSparseMatrixMKL)
{

	if ((&InSparseMatrixMKL))
	{
		if (InSparseMatrixMKL.FromCSRMatrix)
		{
			if (!(InSparseMatrixMKL.FromCSRMatrix->Values))
			{
				printf("%s\n", "MKL矩阵对象中原始CSR矩阵被删除");
				return 0;
			}

		}
		if (InSparseMatrixMKL.MKLSparseHandle)
		{
			return 1;
		}
		else
		{
			printf("%s\n", "MKL矩阵对象中句柄未定义");
			return 0;
		}

	}
	else
	{
		printf("%s\n", "MKL矩阵对象未定义");
		return 0;
	}

}

bool CSparseMatrixMKL::CheckMKLDataSpace()
{
	return CheckMKLDataSpace(*this);
}

void CSparseMatrixMKL::CopyMKL(CSparseMatrixMKL & FromMatrix, CSparseMatrixMKL & ToMatrix)
{
	if (FromMatrix.CheckMKLDataSpace())
	{
		ToMatrix.CreateMKLDataSpace();
		matrix_descr MatrixStructType{ SPARSE_MATRIX_TYPE_GENERAL,SPARSE_FILL_MODE_LOWER,SPARSE_DIAG_NON_UNIT };
		mkl_sparse_copy(*FromMatrix.MKLSparseHandle, MatrixStructType, ToMatrix.MKLSparseHandle);
		ToMatrix.IsCreatHandleSpace = 1;
	}

}


void CSparseMatrixMKL::AddMatrix(
	int InAMatrixOptType,
	double InAMatrixCoef, 
	CSparseMatrixMKL & InAMatrix,
	CSparseMatrixMKL & InBMatrix,
	CSparseMatrixMKL & OutCMatrix)
{
	bool IsOptValid = 0;
	if ((InAMatrix.CheckMKLDataSpace()) &&
		(InBMatrix.CheckMKLDataSpace()) &&
		(&OutCMatrix))
	{
		sparse_operation_t MatrixOpt;
		switch (InAMatrixOptType)
		{
		case 0:
			MatrixOpt = SPARSE_OPERATION_NON_TRANSPOSE;
			break;
		case 1:
			MatrixOpt = SPARSE_OPERATION_TRANSPOSE;
			break;
		case 2:
			MatrixOpt = SPARSE_OPERATION_CONJUGATE_TRANSPOSE;
			break;
		}
		if (InAMatrix.MKLSparseHandle == OutCMatrix.MKLSparseHandle || InBMatrix.MKLSparseHandle == OutCMatrix.MKLSparseHandle)
		{
			CSparseMatrixMKL TempReusltMKLMatrix;
			TempReusltMKLMatrix.CreateMKLDataSpace();
			mkl_sparse_d_add(
				MatrixOpt,
				*InAMatrix.MKLSparseHandle,
				InAMatrixCoef,
				*InBMatrix.MKLSparseHandle,
				TempReusltMKLMatrix.MKLSparseHandle);
			OutCMatrix.CreateMKLDataSpace();
			*OutCMatrix.MKLSparseHandle = *TempReusltMKLMatrix.MKLSparseHandle;
			OutCMatrix.IsCreatHandleSpace = 1;
		}
		else
		{
			OutCMatrix.CreateMKLDataSpace();
			mkl_sparse_d_add(
				MatrixOpt,
				*InAMatrix.MKLSparseHandle,
				1.0,
				*InBMatrix.MKLSparseHandle,
				OutCMatrix.MKLSparseHandle);
			OutCMatrix.IsCreatHandleSpace = 1;
		}
		IsOptValid = 1;

	}
	if (!IsOptValid)
	{
		printf("%s\n", "MKL矩阵相加时存在错误");
	}

}



void CSparseMatrixMKL::AddMatrix(
	int InAMatrixOptType,
	double InAMatrixCoef, 
	CSparseMatrixMKL & InAMatrix, 
	CSparseMatrixMKL & InBMatrix)
{
	AddMatrix(InAMatrixOptType, InAMatrixCoef,InAMatrix, InBMatrix, *this);

}

void CSparseMatrixMKL::AddMatrix(int InAMatrixOptType,	double InAMatrixCoef, CSparseMatrixMKL & InAMatrix)
{
	AddMatrix(InAMatrixOptType, InAMatrixCoef,InAMatrix, *this);
}

void CSparseMatrixMKL::MultiMatrix(int InAMatrixOptType,CSparseMatrixMKL & InAMatrix, CSparseMatrixMKL & InBMatrix, CSparseMatrixMKL & OutCMatrix)
{
	bool IsOptValid = 0;
	sparse_status_t MKLStatus;
	if ((InAMatrix.CheckMKLDataSpace()) &&
		(InBMatrix.CheckMKLDataSpace()) && 
		(& OutCMatrix))
	{
		sparse_operation_t MatrixOpt;
		switch (InAMatrixOptType)
		{
		case 0:
			MatrixOpt = SPARSE_OPERATION_NON_TRANSPOSE;
			break;
		case 1:
			MatrixOpt = SPARSE_OPERATION_TRANSPOSE;
			break;
		case 2:
			MatrixOpt = SPARSE_OPERATION_CONJUGATE_TRANSPOSE;
			break;
		}
		if (InAMatrix.MKLSparseHandle == OutCMatrix.MKLSparseHandle || InBMatrix.MKLSparseHandle == OutCMatrix.MKLSparseHandle)
		{
			CSparseMatrixMKL TempReusltMKLMatrix;
			TempReusltMKLMatrix.CreateMKLDataSpace();
			
			MKLStatus=mkl_sparse_spmm(
				MatrixOpt,
				*InAMatrix.MKLSparseHandle,
				*InBMatrix.MKLSparseHandle,
				TempReusltMKLMatrix.MKLSparseHandle);
			OutCMatrix.CreateMKLDataSpace();
			*OutCMatrix.MKLSparseHandle = *TempReusltMKLMatrix.MKLSparseHandle;
			OutCMatrix.IsCreatHandleSpace = 1;
		}
		else
		{
			OutCMatrix.CreateMKLDataSpace();
			MKLStatus=mkl_sparse_spmm(
				MatrixOpt,
				*InAMatrix.MKLSparseHandle,
				*InBMatrix.MKLSparseHandle,
				OutCMatrix.MKLSparseHandle);
			OutCMatrix.IsCreatHandleSpace = 1;
		}
		IsOptValid = 1;

	}
	if ((!IsOptValid))
	{
		printf("%s\n", "MKL矩阵相乘时存在错误");
	}
}

void CSparseMatrixMKL::MultiMatrix(int InAMatrixOptType, CSparseMatrixMKL & InAMatrix, CSparseMatrixMKL & InBMatrix)
{
	//NSTanBasicObjOpt::CheckAndReturnObj(this->)
	MultiMatrix(InAMatrixOptType,InAMatrix, InBMatrix, *this);
}

void CSparseMatrixMKL::MultiMatrix(int InAMatrixOptType, CSparseMatrixMKL & InBMatrix)
{
	MultiMatrix(InAMatrixOptType,*this, InBMatrix, *this);
}

void CSparseMatrixMKL::MultiVector(int InAMatrixOptType, int RowCount, CSparseMatrixMKL & InAMatrix, double * InVector, double * OutVector)
{
	sparse_operation_t MatrixOpt;
	switch (InAMatrixOptType)
	{
	case 0:
		MatrixOpt = SPARSE_OPERATION_NON_TRANSPOSE;
		break;
	case 1:
		MatrixOpt = SPARSE_OPERATION_TRANSPOSE;
		break;
	case 2:
		MatrixOpt = SPARSE_OPERATION_CONJUGATE_TRANSPOSE;
		break;
	}
	matrix_descr MatrixStructType{ SPARSE_MATRIX_TYPE_GENERAL,SPARSE_FILL_MODE_LOWER,SPARSE_DIAG_NON_UNIT };
	if (InVector== OutVector)
	{
		double* TempVector = new double[RowCount]();
		cblas_dcopy(RowCount, InVector, 1, TempVector, 1);
		mkl_sparse_d_mv(MatrixOpt, 1.0, *InAMatrix.MKLSparseHandle, MatrixStructType, TempVector, 0.0, OutVector);
		delete[] TempVector;
	}
	else
	{
		mkl_sparse_d_mv(MatrixOpt, 1.0, *InAMatrix.MKLSparseHandle, MatrixStructType, InVector, 0.0, OutVector);
	}
	
	//if (InVector == OutVector)
	//{
	//	
	//	
	//	
	//	CSparseMatrixMKL TempReusltMKLMatrix;
	//	TempReusltMKLMatrix.CreateMKLDataSpace();

	//	MKLStatus = mkl_sparse_spmm(
	//		MatrixOpt,
	//		*InAMatrix.MKLSparseHandle,
	//		*InBMatrix.MKLSparseHandle,
	//		TempReusltMKLMatrix.MKLSparseHandle);
	//	OutCMatrix.CreateMKLDataSpace();
	//	*OutCMatrix.MKLSparseHandle = *TempReusltMKLMatrix.MKLSparseHandle;
	//	OutCMatrix.IsCreatHandleSpace = 1;
	//}
}

void CSparseMatrixMKL::MultiVector(int InAMatrixOptType, int RowCount, double * InVector, double * OutVector)
{
	MultiVector(InAMatrixOptType, RowCount, *this, InVector, OutVector);
}

void CSparseMatrixMKL::SolveLinearEquation(int InSolveMethod, int InUnkownCount, CSparseMatrixMKL & InSolveMatrix, double *& InEquRight, double *& SolvedValueList)
{
	//switch (InSolveMethod)
	//{
	//case 0:
	//	cluster_sparse_solver()
	//	break;
	//default:
	//	break;
	//}
	//phase = 11;
	//PARDISO(pt, &maxfct, &mnum, &mtype, &phase,
	//	&n, a, ia, ja, &idum, &nrhs, iparm, &msglvl, &ddum, &ddum, &error);
}






