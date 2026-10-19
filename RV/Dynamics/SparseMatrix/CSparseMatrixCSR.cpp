#include "CSparseMatrixCSR.h"

CSparseMatrixCSR::CSparseMatrixCSR()
{
}

CSparseMatrixCSR::~CSparseMatrixCSR()
{
}

void CSparseMatrixCSR::CreateCSRDataSpace(int InRowCount, int InColumnCount, int InNonZeroElemCount, CSparseMatrixCSR & OutCSRMatrix)
{
	DeleteCSRDataSpace(OutCSRMatrix);
	OutCSRMatrix.RowCount = InRowCount;
	OutCSRMatrix.ColumnCount = InColumnCount;
	OutCSRMatrix.Values = new double[InNonZeroElemCount]();
	OutCSRMatrix.ColumnIndex = new int[InNonZeroElemCount]();
	OutCSRMatrix.RowStartIndex = new int[InRowCount+1]();
	OutCSRMatrix.RowEndIndex = OutCSRMatrix.RowStartIndex + 1;
	OutCSRMatrix.IsCreatSpace = 1;
}

void CSparseMatrixCSR::CreateCSRDataSpace(int InRowCount, int InColumnCount, int InNonZeroElemCount)
{
	CreateCSRDataSpace(InRowCount, InColumnCount, InNonZeroElemCount, *this);
}

void CSparseMatrixCSR::DeleteCSRDataSpace(CSparseMatrixCSR & InCSRMatrix)
{
	if (&InCSRMatrix)
	{
		if (InCSRMatrix.IsCreatSpace)
		{
			delete[] InCSRMatrix.Values;
			delete[] InCSRMatrix.ColumnIndex;
			if ((InCSRMatrix.RowStartIndex+1)!=InCSRMatrix.RowEndIndex)//表明是CSR4格式
			{
				delete[] InCSRMatrix.RowEndIndex;
			}
			delete[] InCSRMatrix.RowStartIndex;
		}
		InCSRMatrix.Values = 0;
		InCSRMatrix.ColumnIndex = 0;
		InCSRMatrix.RowStartIndex = 0;
		InCSRMatrix.RowEndIndex = 0;
		InCSRMatrix.RowCount = 0;
		InCSRMatrix.ColumnCount = 0;
		InCSRMatrix.IsCreatSpace = 0;
	}
}

void CSparseMatrixCSR::DeleteCSRDataSpace()
{
	DeleteCSRDataSpace(*this);
}

bool CSparseMatrixCSR::CheckCSRDataSpace(CSparseMatrixCSR & InSparseMatrixCSR)
{
	if (&InSparseMatrixCSR)
	{
		if (InSparseMatrixCSR.Values)
		{
			return 1;
		}
		else
		{
			printf("%s\n", "CSR矩阵对象数组未定义");
			return 0;
		}
	}
	else
	{
		printf("%s\n", "CSR矩阵对象未定义");
		return 0;
	}
}

bool CSparseMatrixCSR::CheckCSRDataSpace()
{
	return CheckCSRDataSpace(*this);
}

bool CSparseMatrixCSR::IsCSR4Formate(CSparseMatrixCSR & InSparseMatrixCSR)
{
	if (InSparseMatrixCSR.RowEndIndex== (InSparseMatrixCSR.RowStartIndex + 1))
	{
		return 0;
	}
	else
	{
		return 1;
	}
}

bool CSparseMatrixCSR::IsCSR4Formate()
{
	return IsCSR4Formate(*this);
}

void CSparseMatrixCSR::CopyCSR(CSparseMatrixCSR & FromMatrix, CSparseMatrixCSR & ToMatrix)
{
	if (FromMatrix.CheckCSRDataSpace())
	{
		if (&FromMatrix != &ToMatrix)
		{
			int NonZeorElemCount = FromMatrix.RowEndIndex[FromMatrix.RowCount-1];
			ToMatrix.CreateCSRDataSpace(FromMatrix.RowCount,
				FromMatrix.ColumnCount,
				NonZeorElemCount);
			std::copy(FromMatrix.Values,
				FromMatrix.Values + NonZeorElemCount,
				ToMatrix.Values);
			std::copy(FromMatrix.ColumnIndex,
				FromMatrix.ColumnIndex + NonZeorElemCount,
				ToMatrix.ColumnIndex);
			if ((FromMatrix.RowStartIndex + 1) != FromMatrix.RowEndIndex)//表明是CSR4格式
			{
				std::copy(FromMatrix.RowStartIndex,
					FromMatrix.RowStartIndex + FromMatrix.RowCount,
					ToMatrix.RowStartIndex);
				ToMatrix.RowEndIndex = new int[FromMatrix.RowCount]();
				std::copy(FromMatrix.RowEndIndex,
					FromMatrix.RowEndIndex + FromMatrix.RowCount,
					ToMatrix.RowEndIndex);
			}
			else
			{
				std::copy(FromMatrix.RowStartIndex,
					FromMatrix.RowStartIndex + FromMatrix.RowCount + 1,
					ToMatrix.RowStartIndex);
				ToMatrix.RowEndIndex = ToMatrix.RowStartIndex + 1;
			}
		}
	}
	else
	{
		printf("%s\n", "拷贝CSR矩阵时出现错误");
	}
}

void CSparseMatrixCSR::ObtainIndexAddressInSingleRow(int RowIndex,
	TanIntVector& ColumnIndexList,
	CSparseMatrixCSR& InMatrix,
	TanDoublePointerVector& OutColumnIndexAddressList)
{
	if (InMatrix.CheckCSRDataSpace())
	{
		double* IndexAddress = 0;
		std::map<int, double*>  InMatrixRowNonZeroElemIndex;
		int i = 0;
		//int NonElemCount = InMatrix.RowEndIndex[i] - InMatrix.RowStartIndex[i];
		//int ColumnIndex = 0;
		for (i = InMatrix.RowStartIndex[RowIndex]; i < InMatrix.RowEndIndex[RowIndex]; i++)
		{
			InMatrixRowNonZeroElemIndex[InMatrix.ColumnIndex[i]] = &InMatrix.Values[i];
		}
		//寻找所求元素在稀疏矩阵的位置，若不存在则地址为0；
		int ColumnCount = (int)ColumnIndexList.size();
		TanDoublePointerVector(ColumnCount, 0).swap(OutColumnIndexAddressList);
		std::map<int, double*>::iterator ColumnIndexIter = InMatrixRowNonZeroElemIndex.begin();
		for (i = 0; i < ColumnCount; i++)
		{
			ColumnIndexIter = InMatrixRowNonZeroElemIndex.find(ColumnIndexList[i]);
			if (ColumnIndexIter != InMatrixRowNonZeroElemIndex.end())
			{
				OutColumnIndexAddressList[i] = ColumnIndexIter->second;
			}
		}
	}
	else
	{
		printf("%s\n", "提取CSR子矩阵地址时出现错误");
	}
}

void CSparseMatrixCSR::ObtainIndexAddressInSingleRow(int RowIndex, TanIntVector * ColumnIndexList, CSparseMatrixCSR & InMatrix, TanDoublePointerVector & OutColumnIndexAddressList)
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

void CSparseMatrixCSR::ObtainIndexAddressInSingleRow(int RowIndex, TanIntVector & ColumnIndexList, TanDoublePointerVector & OutColumnIndexAddressList)
{
	ObtainIndexAddressInSingleRow(RowIndex, ColumnIndexList, *this, OutColumnIndexAddressList);
}

void CSparseMatrixCSR::ObtainIndexAddressInSingleRow(int RowIndex, TanIntVector * ColumnIndexList, TanDoublePointerVector & OutColumnIndexAddressList)
{
	ObtainIndexAddressInSingleRow(RowIndex, ColumnIndexList, *this, OutColumnIndexAddressList);
}

void CSparseMatrixCSR::ObtainIndexValueInSingleRow(int RowIndex,
	TanIntVector& ColumnIndexList,
	CSparseMatrixCSR& InMatrix,
	TanDoubleVector& OutColumnIndexValueList)
{
	if (InMatrix.CheckCSRDataSpace())
	{
		double* IndexAddress = 0;
		std::map<int, double>  InMatrixRowNonZeroElemValue;
		int i = 0;
		//int NonElemCount = InMatrix.RowEndIndex[i] - InMatrix.RowStartIndex[i];
		//int ColumnIndex = 0;
		for (i = InMatrix.RowStartIndex[RowIndex]; i < InMatrix.RowEndIndex[RowIndex]; i++)
		{
			InMatrixRowNonZeroElemValue[InMatrix.ColumnIndex[i]] = InMatrix.Values[i];
		}
		//寻找所求矩阵元素的值，若不存在则地址为0；	
		int ColumnCount = (int)ColumnIndexList.size();
		//TanpDoubleVector OutColumnIndexAddressList(ColumnCount, 0);
		TanDoubleVector(ColumnCount, 0).swap(OutColumnIndexValueList);
		std::map<int, double>::iterator ColumnIndexIter = InMatrixRowNonZeroElemValue.begin();
		for (i = 0; i < ColumnCount; i++)
		{
			ColumnIndexIter = InMatrixRowNonZeroElemValue.find(ColumnIndexList[i]);
			if (ColumnIndexIter != InMatrixRowNonZeroElemValue.end())
			{
				OutColumnIndexValueList[i] = ColumnIndexIter->second;
			}
		}
	}
	else
	{
		printf("%s\n", "提取CSR子矩阵值时出现错误");
	}


}

void CSparseMatrixCSR::ObtainIndexValueInSingleRow(int RowIndex, TanIntVector * ColumnIndexList, CSparseMatrixCSR & InMatrix, TanDoubleVector & OutColumnIndexListValue)
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

void CSparseMatrixCSR::ObtainIndexValueInSingleRow(int RowIndex, TanIntVector & ColumnIndexList, TanDoubleVector & OutColumnIndexListValue)
{
	ObtainIndexValueInSingleRow(RowIndex, ColumnIndexList, *this, OutColumnIndexListValue);
}

void CSparseMatrixCSR::ObtainIndexValueInSingleRow(int RowIndex, TanIntVector * ColumnIndexList, TanDoubleVector & OutColumnIndexListValue)
{
	ObtainIndexValueInSingleRow(RowIndex, ColumnIndexList, *this, OutColumnIndexListValue);
}

void CSparseMatrixCSR::ObtainIndexValueInSingleRow(int RowIndex, TanIntVector & ColumnIndexList, CSparseMatrixCSR & InMatrix, TanIntpDoubleMap& OutColumnIndexValuePointerList)
{
	double* IndexAddress = 0;
	std::map<int, double*>  InMatrixRowNonZeroElemValue;
	int i = 0;
	//int NonElemCount = InMatrix.RowEndIndex[i] - InMatrix.RowStartIndex[i];
	//int ColumnIndex = 0;
	for (i = InMatrix.RowStartIndex[RowIndex]; i < InMatrix.RowEndIndex[RowIndex]; i++)
	{
		InMatrixRowNonZeroElemValue[InMatrix.ColumnIndex[i]] = &InMatrix.Values[i];
	}

	
	//寻找所求矩阵元素的值，若不存在则地址为0；	
	int ColumnCount = (int)ColumnIndexList.size();
	//TanpDoubleVector OutColumnIndexAddressList(ColumnCount, 0);
	OutColumnIndexValuePointerList.clear();
	std::map<int, double*>::iterator ColumnIndexIter = InMatrixRowNonZeroElemValue.begin();
	for (i = 0; i < ColumnCount; i++)
	{
		ColumnIndexIter = InMatrixRowNonZeroElemValue.find(ColumnIndexList[i]);
		if (ColumnIndexIter != InMatrixRowNonZeroElemValue.end())
		{
			OutColumnIndexValuePointerList[i] = ColumnIndexIter->second;
		}
	}
}

void CSparseMatrixCSR::ObtainIndexValueInSingleRow(int RowIndex, TanIntVector & ColumnIndexList, TanIntpDoubleMap& OutColumnIndexValuePointerList)
{
	ObtainIndexValueInSingleRow(RowIndex, ColumnIndexList, *this, OutColumnIndexValuePointerList);
}

void CSparseMatrixCSR::SolveLinearEquByParallelDSS(CSparseMatrixCSR & InCSRMatrix, double * InEquRight, double * OutSolution)
{
	MKL_INT& n = InCSRMatrix.RowCount;
	double*& a = InCSRMatrix.Values;
	int*& ia = InCSRMatrix.RowStartIndex;
	int*& ja = InCSRMatrix.ColumnIndex;
	double*&b = InEquRight;
	double*& x = OutSolution;

	MKL_INT mtype = 11;       /* Real unsymmetric matrix */
	MKL_INT nrhs = 1;     /* Number of right hand sides. */
	/* Internal solver memory pointer pt, */
	/* 32-bit: int pt[64]; 64-bit: long int pt[64] */
	/* or void *pt[64] should be OK on both architectures */
	void* pt[64];
	//void** pt = new void* [64];
	/* Pardiso control parameters. */
	MKL_INT iparm[64];
	MKL_INT maxfct, mnum, phase, error, msglvl;
	/* Auxiliary variables. */
	MKL_INT i;
	double ddum;          /* Double dummy */
	MKL_INT idum;         /* Integer dummy. */
/* -------------------------------------------------------------------- */
/* .. Setup Pardiso control parameters. */
/* -------------------------------------------------------------------- */
	for (i = 0; i < 64; i++)
	{
		iparm[i] = 0;
	}
	iparm[0] = 1;         /* No solver default */
	iparm[1] = 2;         /* Fill-in reordering from METIS */
	iparm[3] = 0;         /* No iterative-direct algorithm */
	iparm[4] = 0;         /* No user fill-in reducing permutation */
	iparm[5] = 0;         /* Write solution into x */
	iparm[6] = 0;         /* Not in use */
	iparm[7] = 2;         /* Max numbers of iterative refinement steps */
	iparm[8] = 0;         /* Not in use */
	iparm[9] = 13;        /* Perturb the pivot elements with 1E-13 */
	iparm[10] = 1;        /* Use nonsymmetric permutation and scaling MPS */
	iparm[11] = 0;        /* Conjugate transposed/transpose solve */
	iparm[12] = 1;        /* Maximum weighted matching algorithm is switched-on (default for non-symmetric) */
	iparm[13] = 0;        /* Output: Number of perturbed pivots */
	iparm[14] = 0;        /* Not in use */
	iparm[15] = 0;        /* Not in use */
	iparm[16] = 0;        /* Not in use */
	iparm[17] = -1;       /* Output: Number of nonzeros in the factor LU */
	iparm[18] = -1;       /* Output: Mflops for LU factorization */
	iparm[19] = 0;        /* Output: Numbers of CG Iterations */
	iparm[34] = 1;
	maxfct = 1;           /* Maximum number of numerical factorizations. */
	mnum = 1;         /* Which factorization to use. */
	msglvl = 0;           /* Print statistical information  */
	error = 0;            /* Initialize error flag */
/* -------------------------------------------------------------------- */
/* .. Initialize the internal solver memory pointer. This is only */
/* necessary for the FIRST call of the PARDISO solver. */
/* -------------------------------------------------------------------- */
	for (i = 0; i < 64; i++)
	{
		pt[i] = 0;
	}
	/* -------------------------------------------------------------------- */
	/* .. Reordering and Symbolic Factorization. This step also allocates */
	/* all memory that is necessary for the factorization. */
	/* -------------------------------------------------------------------- */
	phase = 11;
	PARDISO(pt, &maxfct, &mnum, &mtype, &phase,
		&n, a, ia, ja, &idum, &nrhs, iparm, &msglvl, &ddum, &ddum, &error);
	/* -------------------------------------------------------------------- */
	/* .. Numerical factorization. */
	/* -------------------------------------------------------------------- */
	phase = 22;
	PARDISO(pt, &maxfct, &mnum, &mtype, &phase,
		&n, a, ia, ja, &idum, &nrhs, iparm, &msglvl, &ddum, &ddum, &error);

	/* -------------------------------------------------------------------- */
	/* .. Back substitution and iterative refinement. */
	/* -------------------------------------------------------------------- */
	phase = 33;
	PARDISO(pt, &maxfct, &mnum, &mtype, &phase,
		&n, a, ia, ja, &idum, &nrhs, iparm, &msglvl, b, x, &error);
	phase = -1;           /* Release internal memory. */
	PARDISO(pt, &maxfct, &mnum, &mtype, &phase,
		&n, &ddum, ia, ja, &idum, &nrhs,
		iparm, &msglvl, &ddum, &ddum, &error);
}

//void CSparseMatrixCSR::SolveLinearEquByParallelDSS(CSparseMatrixCSR& InCSRMatrix, double* InEquRight, double* OutSolution)
//{
//	/* Matrix data. */
//	MKL_INT n = 5;
//	MKL_INT ia[6] = { 1, 4, 6, 9, 12, 14 };
//	MKL_INT ja[13] =
//	{ 1, 2,    4,
//	  1, 2,
//			3, 4, 5,
//	  1,    3, 4,
//		 2,       5
//	};
//	double a[13] =
//	{ 1.0,-1.0,     -3.0,
//	 -2.0, 5.0,
//				4.0, 6.0, 4.0,
//	 -4.0,      2.0, 7.0,
//		   8.0,          -5.0
//	};
//	MKL_INT mtype = 11;       /* Real unsymmetric matrix */
//	// Descriptor of main sparse matrix properties
//	struct matrix_descr descrA;
//	// Structure with sparse matrix stored in CSR format
//	sparse_matrix_t       csrA;
//	sparse_operation_t    transA;
//	/* RHS and solution vectors. */
//	double b[5], x[5], bs[5], res, res0;
//	MKL_INT nrhs = 1;     /* Number of right hand sides. */
//	/* Internal solver memory pointer pt, */
//	/* 32-bit: int pt[64]; 64-bit: long int pt[64] */
//	/* or void *pt[64] should be OK on both architectures */
//	void* pt[64];
//	/* Pardiso control parameters. */
//	MKL_INT iparm[64];
//	MKL_INT maxfct, mnum, phase, error, msglvl;
//	/* Auxiliary variables. */
//	MKL_INT i, j;
//	double ddum;          /* Double dummy */
//	MKL_INT idum;         /* Integer dummy. */
//	/* -------------------------------------------------------------------- */
//	/* .. Setup Pardiso control parameters. */
//	/* -------------------------------------------------------------------- */
//	for (i = 0; i < 64; i++)
//	{
//		iparm[i] = 0;
//	}
//	iparm[0] = 1;         /* No solver default */
//	iparm[1] = 2;         /* Fill-in reordering from METIS */
//	iparm[3] = 0;         /* No iterative-direct algorithm */
//	iparm[4] = 0;         /* No user fill-in reducing permutation */
//	iparm[5] = 0;         /* Write solution into x */
//	iparm[6] = 0;         /* Not in use */
//	iparm[7] = 2;         /* Max numbers of iterative refinement steps */
//	iparm[8] = 0;         /* Not in use */
//	iparm[9] = 13;        /* Perturb the pivot elements with 1E-13 */
//	iparm[10] = 1;        /* Use nonsymmetric permutation and scaling MPS */
//	iparm[11] = 0;        /* Conjugate transposed/transpose solve */
//	iparm[12] = 1;        /* Maximum weighted matching algorithm is switched-on (default for non-symmetric) */
//	iparm[13] = 0;        /* Output: Number of perturbed pivots */
//	iparm[14] = 0;        /* Not in use */
//	iparm[15] = 0;        /* Not in use */
//	iparm[16] = 0;        /* Not in use */
//	iparm[17] = -1;       /* Output: Number of nonzeros in the factor LU */
//	iparm[18] = -1;       /* Output: Mflops for LU factorization */
//	iparm[19] = 0;        /* Output: Numbers of CG Iterations */
//	maxfct = 1;           /* Maximum number of numerical factorizations. */
//	mnum = 1;         /* Which factorization to use. */
//	msglvl = 0;           /* Print statistical information  */
//	error = 0;            /* Initialize error flag */
//	/* -------------------------------------------------------------------- */
//	/* .. Initialize the internal solver memory pointer. This is only */
//	/* necessary for the FIRST call of the PARDISO solver. */
//	/* -------------------------------------------------------------------- */
//	for (i = 0; i < 64; i++)
//	{
//		pt[i] = 0;
//	}
//	/* -------------------------------------------------------------------- */
//	/* .. Reordering and Symbolic Factorization. This step also allocates */
//	/* all memory that is necessary for the factorization. */
//	/* -------------------------------------------------------------------- */
//	phase = 11;
//	PARDISO(pt, &maxfct, &mnum, &mtype, &phase,
//		&n, a, ia, ja, &idum, &nrhs, iparm, &msglvl, &ddum, &ddum, &error);
//	//if (error != 0)
//	//{
//	//	printf("\nERROR during symbolic factorization: " IFORMAT, error);
//	//	exit(1);
//	//}
//	//printf("\nReordering completed ... ");
//	//printf("\nNumber of nonzeros in factors = " IFORMAT, iparm[17]);
//	//printf("\nNumber of factorization MFLOPS = " IFORMAT, iparm[18]);
//	/* -------------------------------------------------------------------- */
//	/* .. Numerical factorization. */
//	/* -------------------------------------------------------------------- */
//	phase = 22;
//	PARDISO(pt, &maxfct, &mnum, &mtype, &phase,
//		&n, a, ia, ja, &idum, &nrhs, iparm, &msglvl, &ddum, &ddum, &error);
//	//if (error != 0)
//	//{
//	//	printf("\nERROR during numerical factorization: " IFORMAT, error);
//	//	exit(2);
//	//}
//	printf("\nFactorization completed ... ");
//	/* -------------------------------------------------------------------- */
//	/* .. Back substitution and iterative refinement. */
//	/* -------------------------------------------------------------------- */
//	phase = 33;
//
//	descrA.type = SPARSE_MATRIX_TYPE_GENERAL;
//	descrA.mode = SPARSE_FILL_MODE_UPPER;
//	descrA.diag = SPARSE_DIAG_NON_UNIT;
//	mkl_sparse_d_create_csr(&csrA, SPARSE_INDEX_BASE_ONE, n, n, ia, ia + 1, ja, a);
//
//	/* Set right hand side to one. */
//	for (i = 0; i < n; i++)
//	{
//		b[i] = 1;
//	}
//	//  Loop over 3 solving steps: Ax=b, AHx=b and ATx=b
//	for (i = 0; i < 3; i++)
//	{
//		iparm[11] = i;        /* Conjugate transposed/transpose solve */
//		if (i == 0)
//			transA = SPARSE_OPERATION_NON_TRANSPOSE;
//		else if (i == 1)
//			transA = SPARSE_OPERATION_CONJUGATE_TRANSPOSE;
//		else
//			transA = SPARSE_OPERATION_TRANSPOSE;
//
//		//printf("\n\nSolving system with iparm[11] = " IFORMAT " ...\n", iparm[11]);
//		PARDISO(pt, &maxfct, &mnum, &mtype, &phase,
//			&n, a, ia, ja, &idum, &nrhs, iparm, &msglvl, b, x, &error);
//		//if (error != 0)
//		//{
//		//	printf("\nERROR during solution: " IFORMAT, error);
//		//	exit(3);
//		//}
//
//		//printf("\nThe solution of the system is: ");
//		//for (j = 0; j < n; j++)
//		//{
//		//	printf("\n x [" IFORMAT "] = % f", j, x[j]);
//		//}
//		//printf("\n");
//		// Compute residual
//		mkl_sparse_d_mv(transA, 1.0, csrA, descrA, x, 0.0, bs);
//		res = 0.0;
//		res0 = 0.0;
//		for (j = 1; j <= n; j++)
//		{
//			res += (bs[j - 1] - b[j - 1]) * (bs[j - 1] - b[j - 1]);
//			res0 += b[j - 1] * b[j - 1];
//		}
//		res = sqrt(res) / sqrt(res0);
//		printf("\nRelative residual = %e", res);
//		// Check residual
//		if (res > 1e-10)
//		{
//			printf("Error: residual is too high!\n");
//			exit(10 + (int)i);
//		}
//	}
//	mkl_sparse_destroy(csrA);
//
//	/* -------------------------------------------------------------------- */
//	/* .. Termination and release of memory. */
//	/* -------------------------------------------------------------------- */
//	phase = -1;           /* Release internal memory. */
//	PARDISO(pt, &maxfct, &mnum, &mtype, &phase,
//		&n, &ddum, ia, ja, &idum, &nrhs,
//		iparm, &msglvl, &ddum, &ddum, &error);
//
//
//	//MKL_INT& n = InCSRMatrix.RowCount;
//	//double*& a = InCSRMatrix.Values;
//	////InCSRMatrix.RowStartIndex = 0;
//	////InCSRMatrix.ColumnIndex = 0;
//	//int*& ia = InCSRMatrix.RowStartIndex;
//	//int*& ja = InCSRMatrix.ColumnIndex;
//	//double*& b = InEquRight;
//	//double*& x = OutSolution;
//	//// 定义 PARDISO 控制参数
//	//MKL_INT mtype = 11;       // 实数非对称矩阵（具体类型参考文档）
//	//MKL_INT nrhs = 1;         // 右端项数量
//	//void* pt[64];             // PARDISO 内部数据结构指针
//	//MKL_INT iparm[64];        // 求解器参数
//	//MKL_INT maxfct = 1, mnum = 1, phase, error = 0;
//	//double dparm[64];
//	//MKL_INT msglvl = 1;       // 输出日志级别（0=静默，1=基本信息）
//	//
//	//// 初始化参数
//	//pardisoinit(pt, &mtype, iparm);
//	//iparm[0] = 1;             // 使用自定义参数（0=默认）
//	//iparm[1] = 2;             // 填充缩减算法（推荐值）
//	//iparm[7] = 2;             // 迭代次数限制
//	//iparm[9] = 8;             // 扰动参数（针对非对称矩阵）
//	//iparm[34] = 0;            // 0-based 索引
//	//// 阶段1：符号分析（Symbolic Factorization）
//	//phase = 11;
//	//pardiso(pt, &maxfct, &mnum, &mtype, &phase, &n, a, ia, ja,
//	//	NULL, &nrhs, iparm, &msglvl, NULL, NULL, &error);
//	//if (error != 0) 
//	//{
//	//	printf("符号分析错误: %d\n", error);
//	//	exit(1);
//	//}
//	//MKL_INT mtype = 11;       /* Real unsymmetric matrix */
//	//MKL_INT nrhs = 1;     /* Number of right hand sides. */
//	///* Internal solver memory pointer pt, */
//	///* 32-bit: int pt[64]; 64-bit: long int pt[64] */
//	///* or void *pt[64] should be OK on both architectures */
//	////void* pt[64] = new void* [64];
//	//void** pt = new void* [64];
//	///* Pardiso control parameters. */
//	//MKL_INT iparm[64];
//	//MKL_INT maxfct, mnum, phase, error, msglvl;
//	///* Auxiliary variables. */
//	//MKL_INT i;
//	//double ddum;          /* Double dummy */
//	//MKL_INT idum;         /* Integer dummy. */
//	///* -------------------------------------------------------------------- */
//	///* .. Setup Pardiso control parameters. */
//	///* -------------------------------------------------------------------- */
//	//for (i = 0; i < 64; i++)
//	//{
//	//	iparm[i] = 0;
//	//}
//	//iparm[0] = 1;         /* No solver default */
//	//iparm[1] = 2;         /* Fill-in reordering from METIS */
//	//iparm[3] = 0;         /* No iterative-direct algorithm */
//	//iparm[4] = 0;         /* No user fill-in reducing permutation */
//	//iparm[5] = 0;         /* Write solution into x */
//	//iparm[6] = 0;         /* Not in use */
//	//iparm[7] = 2;         /* Max numbers of iterative refinement steps */
//	//iparm[8] = 0;         /* Not in use */
//	//iparm[9] = 13;        /* Perturb the pivot elements with 1E-13 */
//	//iparm[10] = 1;        /* Use nonsymmetric permutation and scaling MPS */
//	//iparm[11] = 0;        /* Conjugate transposed/transpose solve */
//	//iparm[12] = 1;        /* Maximum weighted matching algorithm is switched-on (default for non-symmetric) */
//	//iparm[13] = 0;        /* Output: Number of perturbed pivots */
//	//iparm[14] = 0;        /* Not in use */
//	//iparm[15] = 0;        /* Not in use */
//	//iparm[16] = 0;        /* Not in use */
//	//iparm[17] = -1;       /* Output: Number of nonzeros in the factor LU */
//	//iparm[18] = -1;       /* Output: Mflops for LU factorization */
//	//iparm[19] = 0;        /* Output: Numbers of CG Iterations */
//	//iparm[34] = 1;
//	//maxfct = 1;           /* Maximum number of numerical factorizations. */
//	//mnum = 1;         /* Which factorization to use. */
//	//msglvl = 0;           /* Print statistical information  */
//	//error = 0;            /* Initialize error flag */
//	/* -------------------------------------------------------------------- */
//	/* .. Initialize the internal solver memory pointer. This is only */
//	/* necessary for the FIRST call of the PARDISO solver. */
//	/* -------------------------------------------------------------------- */
//	//for (i = 0; i < 64; i++)
//	//{
//	//	pt[i] = 0;
//	//}
//	///* -------------------------------------------------------------------- */
//	///* .. Reordering and Symbolic Factorization. This step also allocates */
//	///* all memory that is necessary for the factorization. */
//	///* -------------------------------------------------------------------- */
//	//phase = 11;
//	//PARDISO(pt, &maxfct, &mnum, &mtype, &phase,
//	//	&n, a, ia, ja, &idum, &nrhs, iparm, &msglvl, &ddum, &ddum, &error);
//	///* -------------------------------------------------------------------- */
//	///* .. Numerical factorization. */
//	///* -------------------------------------------------------------------- */
//	//phase = 22;
//	//PARDISO(pt, &maxfct, &mnum, &mtype, &phase,
//	//	&n, a, ia, ja, &idum, &nrhs, iparm, &msglvl, &ddum, &ddum, &error);
//
//	///* -------------------------------------------------------------------- */
//	///* .. Back substitution and iterative refinement. */
//	///* -------------------------------------------------------------------- */
//	//phase = 33;
//	//PARDISO(pt, &maxfct, &mnum, &mtype, &phase,
//	//	&n, a, ia, ja, &idum, &nrhs, iparm, &msglvl, b, x, &error);
//	//phase = -1;           /* Release internal memory. */
//	//PARDISO(pt, &maxfct, &mnum, &mtype, &phase,
//	//	&n, &ddum, ia, ja, &idum, &nrhs,
//	//	iparm, &msglvl, &ddum, &ddum, &error);
//	//delete[] pt;
//}
void CSparseMatrixCSR::SolveLinearEquByParallelDSS(double * InEquRight, double * OutSolution)
{
	SolveLinearEquByParallelDSS(*this, InEquRight, OutSolution);
}

void CSparseMatrixCSR::TestFunc()
{
	/* Matrix data. */
	MKL_INT n = 5;
	MKL_INT ia[6] = { 1, 4, 6, 9, 12, 14 };
	MKL_INT ja[13] =
	{ 1, 2,    4,
	  1, 2,
			3, 4, 5,
	  1,    3, 4,
		 2,       5
	};
	double a[13] =
	{ 1.0,-1.0,     -3.0,
	 -2.0, 5.0,
				4.0, 6.0, 4.0,
	 -4.0,      2.0, 7.0,
		   8.0,          -5.0
	};
	MKL_INT mtype = 11;       /* Real unsymmetric matrix */
	// Descriptor of main sparse matrix properties
	struct matrix_descr descrA;
	// Structure with sparse matrix stored in CSR format
	sparse_matrix_t       csrA;
	sparse_operation_t    transA;
	/* RHS and solution vectors. */
	double b[5], x[5], bs[5], res, res0;
	MKL_INT nrhs = 1;     /* Number of right hand sides. */
	/* Internal solver memory pointer pt, */
	/* 32-bit: int pt[64]; 64-bit: long int pt[64] */
	/* or void *pt[64] should be OK on both architectures */
	void* pt[64];
	/* Pardiso control parameters. */
	MKL_INT iparm[64];
	MKL_INT maxfct, mnum, phase, error, msglvl;
	/* Auxiliary variables. */
	MKL_INT i, j;
	double ddum;          /* Double dummy */
	MKL_INT idum;         /* Integer dummy. */
	/* -------------------------------------------------------------------- */
	/* .. Setup Pardiso control parameters. */
	/* -------------------------------------------------------------------- */
	for (i = 0; i < 64; i++)
	{
		iparm[i] = 0;
	}
	iparm[0] = 1;         /* No solver default */
	iparm[1] = 2;         /* Fill-in reordering from METIS */
	iparm[3] = 0;         /* No iterative-direct algorithm */
	iparm[4] = 0;         /* No user fill-in reducing permutation */
	iparm[5] = 0;         /* Write solution into x */
	iparm[6] = 0;         /* Not in use */
	iparm[7] = 2;         /* Max numbers of iterative refinement steps */
	iparm[8] = 0;         /* Not in use */
	iparm[9] = 13;        /* Perturb the pivot elements with 1E-13 */
	iparm[10] = 1;        /* Use nonsymmetric permutation and scaling MPS */
	iparm[11] = 0;        /* Conjugate transposed/transpose solve */
	iparm[12] = 1;        /* Maximum weighted matching algorithm is switched-on (default for non-symmetric) */
	iparm[13] = 0;        /* Output: Number of perturbed pivots */
	iparm[14] = 0;        /* Not in use */
	iparm[15] = 0;        /* Not in use */
	iparm[16] = 0;        /* Not in use */
	iparm[17] = -1;       /* Output: Number of nonzeros in the factor LU */
	iparm[18] = -1;       /* Output: Mflops for LU factorization */
	iparm[19] = 0;        /* Output: Numbers of CG Iterations */
	maxfct = 1;           /* Maximum number of numerical factorizations. */
	mnum = 1;         /* Which factorization to use. */
	msglvl = 0;           /* Print statistical information  */
	error = 0;            /* Initialize error flag */
	/* -------------------------------------------------------------------- */
	/* .. Initialize the internal solver memory pointer. This is only */
	/* necessary for the FIRST call of the PARDISO solver. */
	/* -------------------------------------------------------------------- */
	for (i = 0; i < 64; i++)
	{
		pt[i] = 0;
	}
	/* -------------------------------------------------------------------- */
	/* .. Reordering and Symbolic Factorization. This step also allocates */
	/* all memory that is necessary for the factorization. */
	/* -------------------------------------------------------------------- */
	phase = 11;
	PARDISO(pt, &maxfct, &mnum, &mtype, &phase,
		&n, a, ia, ja, &idum, &nrhs, iparm, &msglvl, &ddum, &ddum, &error);
	//if (error != 0)
	//{
	//	printf("\nERROR during symbolic factorization: " IFORMAT, error);
	//	exit(1);
	//}
	//printf("\nReordering completed ... ");
	//printf("\nNumber of nonzeros in factors = " IFORMAT, iparm[17]);
	//printf("\nNumber of factorization MFLOPS = " IFORMAT, iparm[18]);
	/* -------------------------------------------------------------------- */
	/* .. Numerical factorization. */
	/* -------------------------------------------------------------------- */
	phase = 22;
	PARDISO(pt, &maxfct, &mnum, &mtype, &phase,
		&n, a, ia, ja, &idum, &nrhs, iparm, &msglvl, &ddum, &ddum, &error);
	//if (error != 0)
	//{
	//	printf("\nERROR during numerical factorization: " IFORMAT, error);
	//	exit(2);
	//}
	printf("\nFactorization completed ... ");
	/* -------------------------------------------------------------------- */
	/* .. Back substitution and iterative refinement. */
	/* -------------------------------------------------------------------- */
	phase = 33;

	descrA.type = SPARSE_MATRIX_TYPE_GENERAL;
	descrA.mode = SPARSE_FILL_MODE_UPPER;
	descrA.diag = SPARSE_DIAG_NON_UNIT;
	mkl_sparse_d_create_csr(&csrA, SPARSE_INDEX_BASE_ONE, n, n, ia, ia + 1, ja, a);

	/* Set right hand side to one. */
	for (i = 0; i < n; i++)
	{
		b[i] = 1;
	}
	//  Loop over 3 solving steps: Ax=b, AHx=b and ATx=b
	for (i = 0; i < 3; i++)
	{
		iparm[11] = i;        /* Conjugate transposed/transpose solve */
		if (i == 0)
			transA = SPARSE_OPERATION_NON_TRANSPOSE;
		else if (i == 1)
			transA = SPARSE_OPERATION_CONJUGATE_TRANSPOSE;
		else
			transA = SPARSE_OPERATION_TRANSPOSE;

		//printf("\n\nSolving system with iparm[11] = " IFORMAT " ...\n", iparm[11]);
		PARDISO(pt, &maxfct, &mnum, &mtype, &phase,
			&n, a, ia, ja, &idum, &nrhs, iparm, &msglvl, b, x, &error);
		//if (error != 0)
		//{
		//	printf("\nERROR during solution: " IFORMAT, error);
		//	exit(3);
		//}

		//printf("\nThe solution of the system is: ");
		//for (j = 0; j < n; j++)
		//{
		//	printf("\n x [" IFORMAT "] = % f", j, x[j]);
		//}
		//printf("\n");
		// Compute residual
		mkl_sparse_d_mv(transA, 1.0, csrA, descrA, x, 0.0, bs);
		res = 0.0;
		res0 = 0.0;
		for (j = 1; j <= n; j++)
		{
			res += (bs[j - 1] - b[j - 1]) * (bs[j - 1] - b[j - 1]);
			res0 += b[j - 1] * b[j - 1];
		}
		res = sqrt(res) / sqrt(res0);
		printf("\nRelative residual = %e", res);
		// Check residual
		if (res > 1e-10)
		{
			printf("Error: residual is too high!\n");
			exit(10 + (int)i);
		}
	}
	mkl_sparse_destroy(csrA);

	/* -------------------------------------------------------------------- */
	/* .. Termination and release of memory. */
	/* -------------------------------------------------------------------- */
	phase = -1;           /* Release internal memory. */
	PARDISO(pt, &maxfct, &mnum, &mtype, &phase,
		&n, &ddum, ia, ja, &idum, &nrhs,
		iparm, &msglvl, &ddum, &ddum, &error);


}


