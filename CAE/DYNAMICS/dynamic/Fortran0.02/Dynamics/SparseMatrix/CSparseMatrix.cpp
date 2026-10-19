#include "CSparseMatrix.h"

CSparseMatrix::CSparseMatrix()
{

}

CSparseMatrix::~CSparseMatrix()
{
	DeleteAllObj();

	//DeleteCSRDataSpace();
}

void CSparseMatrix::DeleteDenseObj()
{
	if (pDENSE_FORMAT)
	{
		pDENSE_FORMAT->DeleteDenseDataSpace();
	}
	pDENSE_FORMAT = 0;
}

void CSparseMatrix::DeleteCSRObj()
{
	if (pCSR_FORMAT)
	{
		pCSR_FORMAT->DeleteCSRDataSpace();
	}
	pCSR_FORMAT = 0;
}

void CSparseMatrix::DeleteMKLObj()
{
	if (pMKL_FORMAT)
	{
		pMKL_FORMAT->DeleteMKLDataSpace();
	}
	pMKL_FORMAT = 0;
}

void CSparseMatrix::DeleteDiscreteCSR()
{
	if (pCSR_DISCRETE_FORMAT)
	{
		delete pCSR_DISCRETE_FORMAT;
	}
	pCSR_DISCRETE_FORMAT = 0;
}

void CSparseMatrix::DeleteAllObj()
{
	DeleteDenseObj();
	DeleteCSRObj();
	DeleteMKLObj();
	DeleteDiscreteCSR();
}

void CSparseMatrix::CreateDenseObj()
{
	DeleteDenseObj();
	pDENSE_FORMAT = new CDenseMatrix;
}


void CSparseMatrix::CreateCSRObj()
{
	DeleteCSRObj();
	pCSR_FORMAT = new CSparseMatrixCSR;
}

void CSparseMatrix::CreateMKLObj()
{
	DeleteMKLObj();
	pMKL_FORMAT = new CSparseMatrixMKL;
}

void CSparseMatrix::CreateDiscreteCSRObj()
{
	DeleteDiscreteCSR();
	pCSR_DISCRETE_FORMAT = new CSparseMatrixDiscreteCSR;
}

void CSparseMatrix::CreateAllObj()
{
	CreateDenseObj();
	CreateCSRObj();
	CreateMKLObj();
	CreateDiscreteCSRObj();
}



void CSparseMatrix::ConvertDenseToCSR(CDenseMatrix & InDenseMatrix, CSparseMatrixCSR & OutCSRMatrix)
{
	bool IsOptValid = 0;
	if (InDenseMatrix.CheckDenseDataSpace())
	{
		OutCSRMatrix.DeleteCSRDataSpace();
		//根据输入参数确定行数和列数		
		OutCSRMatrix.RowCount = InDenseMatrix.RowCount;
		OutCSRMatrix.ColumnCount = InDenseMatrix.ColumnCount;
		//定义初始存储数组容器和计数参数
		int i = 0;
		int j = 0;
		int SingleRowElemCount = 0;
		std::vector <double> NonZeroElem;
		NonZeroElem.reserve(12 * InDenseMatrix.RowCount);
		std::vector <int> NonZeroColumnIndex;
		NonZeroColumnIndex.reserve(12 * InDenseMatrix.RowCount);
		//遍历数组，找出非0值		
		OutCSRMatrix.RowStartIndex = new int[InDenseMatrix.RowCount + 1]();
		int RowStartIndex = 0;
		int ElemIndex = 0;
		for (i = 0; i < InDenseMatrix.RowCount; i++)
		{
			OutCSRMatrix.RowStartIndex[i] = RowStartIndex;
			for (int j = 0; j < InDenseMatrix.ColumnCount; j++)
			{
				ElemIndex = j + i * InDenseMatrix.ColumnCount;
				if (InDenseMatrix.Values[ElemIndex] != 0.0)
				{
					RowStartIndex++;
					NonZeroElem.emplace_back(InDenseMatrix.Values[ElemIndex]);
					NonZeroColumnIndex.emplace_back(j);

				}
			}
		}
		OutCSRMatrix.RowStartIndex[InDenseMatrix.RowCount] = RowStartIndex;
		OutCSRMatrix.RowEndIndex = OutCSRMatrix.RowStartIndex + 1;
		OutCSRMatrix.Values = new double[RowStartIndex]();
		std::copy(NonZeroElem.begin(), NonZeroElem.end(), OutCSRMatrix.Values);
		OutCSRMatrix.ColumnIndex = new int[RowStartIndex]();
		std::copy(NonZeroColumnIndex.begin(), NonZeroColumnIndex.end(), OutCSRMatrix.ColumnIndex);
		OutCSRMatrix.IsCreatSpace = 1;
		IsOptValid = 1;
	}

	if (!IsOptValid)
	{
		printf("%s\n", "从稠密矩阵转换到CSR矩阵出错");
	}
}
void CSparseMatrix::ConvertDenseToCSR(CDenseMatrix & InDenseMatrix, CSparseMatrixCSR *& OutCSRMatrix)
{
	NSTanBasicObjOpt::CheckAndReturnObj(OutCSRMatrix);
	ConvertDenseToCSR(InDenseMatrix, *OutCSRMatrix);
}

void CSparseMatrix::ConvertDenseToCSR(CDenseMatrix & InDenseMatrix)
{
	ConvertDenseToCSR(InDenseMatrix, pCSR_FORMAT);
}
void CSparseMatrix::ConvertDenseToCSR()
{
	ConvertDenseToCSR(*pDENSE_FORMAT);
}

void CSparseMatrix::ConvertCSRToDense(CSparseMatrixCSR& InCSRMatrix, CDenseMatrix& OutDenseMatrix)
{
	bool IsOptValid = 0;
	if (InCSRMatrix.CheckCSRDataSpace())
	{
		OutDenseMatrix.CreateDenseDataSpace(InCSRMatrix.RowCount, InCSRMatrix.ColumnCount, OutDenseMatrix);
		int i = 0;
		int j = 0;
		int ColIndex = 0;
		for (i = 0; i < InCSRMatrix.RowCount; i++)
		{
			for (j = InCSRMatrix.RowStartIndex[i]; j < InCSRMatrix.RowEndIndex[i]; j++)
			{
				ColIndex = InCSRMatrix.ColumnIndex[j];
				OutDenseMatrix.Values[ColIndex + i * InCSRMatrix.ColumnCount] = InCSRMatrix.Values[j];
			}
		}
		IsOptValid = 1;
	}
	if (!IsOptValid)
	{
		printf("%s\n", "从CSR转换到稠密矩阵出错");
	}
}

void CSparseMatrix::ConvertCSRToDense(CSparseMatrixCSR & InCSRMatrix, CDenseMatrix *& OutDenseMatrix)
{

	NSTanBasicObjOpt::CheckAndReturnObj(OutDenseMatrix);
	ConvertCSRToDense(InCSRMatrix, *OutDenseMatrix);
}

void CSparseMatrix::ConvertCSRToDense(CSparseMatrixCSR & InCSRMatrix)
{
	ConvertCSRToDense(InCSRMatrix,pDENSE_FORMAT);

}

void CSparseMatrix::ConvertCSRToDense()
{
	ConvertCSRToDense(*pCSR_FORMAT);
}
void CSparseMatrix::ConvertCSRToMKL(CSparseMatrixCSR & InSparseMatrixCSR, CSparseMatrixMKL & OutSparseMatrixMKL)
{
	bool IsOptValid = 0;
	if (InSparseMatrixCSR.CheckCSRDataSpace())
	{
		OutSparseMatrixMKL.CreateMKLDataSpace();
		mkl_sparse_d_create_csr(
			OutSparseMatrixMKL.MKLSparseHandle,
			SPARSE_INDEX_BASE_ZERO,
			InSparseMatrixCSR.RowCount,  // number of rows
			InSparseMatrixCSR.ColumnCount,  // number of cols
			InSparseMatrixCSR.RowStartIndex,
			InSparseMatrixCSR.RowEndIndex,
			InSparseMatrixCSR.ColumnIndex,
			InSparseMatrixCSR.Values);
		OutSparseMatrixMKL.IsCreatHandleSpace = 1;
		OutSparseMatrixMKL.FromCSRMatrix = &InSparseMatrixCSR;
		IsOptValid = 1;
	}
	if (!IsOptValid)
	{
		printf("%s\n", "从CSR矩阵转换到MKL矩阵时出现错误");
	}
}

void CSparseMatrix::ConvertCSRToMKL(CSparseMatrixCSR & InSparseMatrixCSR, CSparseMatrixMKL *& OutSparseMatrixMKL)
{
	NSTanBasicObjOpt::CheckAndReturnObj(OutSparseMatrixMKL);
	ConvertCSRToMKL(InSparseMatrixCSR, *OutSparseMatrixMKL);
}

void CSparseMatrix::ConvertCSRToMKL(CSparseMatrixCSR & InSparseMatrixCSR)
{
	ConvertCSRToMKL(InSparseMatrixCSR, pMKL_FORMAT);
}

void CSparseMatrix::ConvertCSRToMKL()
{
	ConvertCSRToMKL(*pCSR_FORMAT);
}
void CSparseMatrix::ConvertMKLToCSR(CSparseMatrixMKL & InSparseMatrixMKL, CSparseMatrixCSR & OutSparseMatrixCSR)
{
	if (InSparseMatrixMKL.CheckMKLDataSpace())
	{
		if (InSparseMatrixMKL.FromCSRMatrix != (&OutSparseMatrixCSR))
		{
			OutSparseMatrixCSR.DeleteCSRDataSpace();
			sparse_index_base_t IndexBase = SPARSE_INDEX_BASE_ZERO;
			//注意MKL的导出函数只是简单传递指针，因此OutSparseMatrixCSR.IsCreatSpace设置为0
			mkl_sparse_d_export_csr(*InSparseMatrixMKL.MKLSparseHandle,
				&IndexBase,
				&OutSparseMatrixCSR.RowCount,
				&OutSparseMatrixCSR.ColumnCount,
				&OutSparseMatrixCSR.RowStartIndex,
				&OutSparseMatrixCSR.RowEndIndex,
				&OutSparseMatrixCSR.ColumnIndex,
				&OutSparseMatrixCSR.Values);
		}
	}
	else
	{
		printf("%s\n", "从MKL浅转换为CSR形式时出现错误");
	}
}

void CSparseMatrix::ConvertMKLToCSR(CSparseMatrixMKL & InSparseMatrixMKL, CSparseMatrixCSR *& OutSparseMatrixCSR)
{
	NSTanBasicObjOpt::CheckAndReturnObj(OutSparseMatrixCSR);
	ConvertMKLToCSR(InSparseMatrixMKL, *OutSparseMatrixCSR);
}

void CSparseMatrix::ConvertMKLToCSR(CSparseMatrixMKL & InSparseMatrixMKL)
{
	ConvertMKLToCSR(InSparseMatrixMKL,pCSR_FORMAT);
}

void CSparseMatrix::ConvertMKLToCSR()
{
	ConvertMKLToCSR(*pMKL_FORMAT);
}

void CSparseMatrix::ConvertMKLToCSRByCopy(CSparseMatrixMKL& InSparseMatrixMKL, CSparseMatrixCSR & OutSparseMatrixCSR)
{
	if (InSparseMatrixMKL.CheckMKLDataSpace())
	{
		if (InSparseMatrixMKL.FromCSRMatrix != (&OutSparseMatrixCSR))
		{
			CSparseMatrixCSR* TempCSRMatrix=new CSparseMatrixCSR;
			ConvertMKLToCSR(InSparseMatrixMKL, TempCSRMatrix);
			OutSparseMatrixCSR.CopyCSR(*TempCSRMatrix, OutSparseMatrixCSR);
			delete TempCSRMatrix;
		}

	}
	else
	{
		printf("%s\n", "从MKL深转换为CSR形式时出现错误");
	}	
}

void CSparseMatrix::ConvertMKLToCSRByCopy(CSparseMatrixMKL & InSparseMatrixMKL)
{
	NSTanBasicObjOpt::CheckAndReturnObj(pCSR_FORMAT);
	ConvertMKLToCSRByCopy(InSparseMatrixMKL, *pCSR_FORMAT);
}

void CSparseMatrix::ConvertMKLToCSRByCopy()
{
	ConvertMKLToCSRByCopy(*pMKL_FORMAT);
}

void CSparseMatrix::ConvertCSRToCSRDiscrete(CSparseMatrixCSR & InSparseMatrixCSR, CSparseMatrixDiscreteCSR & OutSparseMatrixCSRDiscrete)
{
	if (InSparseMatrixCSR.CheckCSRDataSpace())
	{
		OutSparseMatrixCSRDiscrete.Set(InSparseMatrixCSR.RowCount, InSparseMatrixCSR.ColumnCount);
		for (int i = 0; i < InSparseMatrixCSR.RowCount; i++)
		{
			for (int j = InSparseMatrixCSR.RowStartIndex[i]; j < InSparseMatrixCSR.RowEndIndex[i]; j++)
			{
				OutSparseMatrixCSRDiscrete[i][InSparseMatrixCSR.ColumnIndex[j]] = InSparseMatrixCSR.Values[j];
			}
		}
	}
	else
	{
		printf("%s\n", "CSR矩阵转换到离散CSR矩阵出错");
	}
}

void CSparseMatrix::ConvertCSRToCSRDiscrete(CSparseMatrixCSR & InSparseMatrixCSR, CSparseMatrixDiscreteCSR *& OutSparseMatrixCSRDiscrete)
{
	NSTanBasicObjOpt::CheckAndReturnObj(OutSparseMatrixCSRDiscrete);
	ConvertCSRToCSRDiscrete(InSparseMatrixCSR, *OutSparseMatrixCSRDiscrete);
}

void CSparseMatrix::ConvertCSRToCSRDiscrete(CSparseMatrixCSR & InSparseMatrixCSR)
{
	ConvertCSRToCSRDiscrete(InSparseMatrixCSR, pCSR_DISCRETE_FORMAT);
}

void CSparseMatrix::ConvertCSRToCSRDiscrete()
{
	ConvertCSRToCSRDiscrete(*pCSR_FORMAT);
}

void CSparseMatrix::ConvertCSRDiscreteToCSR(CSparseMatrixDiscreteCSR & InSparseMatrixCSRDiscrete, CSparseMatrixCSR & OutSparseMatrixCSR)
{
	if (InSparseMatrixCSRDiscrete.CheckDiscreteCSRDataSpace())
	{
		int RowCount = 0;
		RowCount = (int)(InSparseMatrixCSRDiscrete.size());
		TanIntVector EachRowElemCount(RowCount, 0);
		int i = 0;
		std::generate(EachRowElemCount.begin(), EachRowElemCount.end(), [&] { return (int)(InSparseMatrixCSRDiscrete[i++].size()); });
		int TotalNonZeroElemCount = std::accumulate(EachRowElemCount.begin(), EachRowElemCount.end(), 0);
		TanIntVector EachRowElemEnd(RowCount, 0);
		//i = 1;
		std::partial_sum(EachRowElemCount.begin(), EachRowElemCount.end(), EachRowElemEnd.begin());
		//std::generate(EachRowElemEnd.begin(), EachRowElemEnd.end(), [&] {return std::accumulate(EachRowElemCount.begin(), EachRowElemCount.begin() + i++, 0); });
		//i = 0;
		OutSparseMatrixCSR.CreateCSRDataSpace(RowCount, InSparseMatrixCSRDiscrete.ColumnCount, EachRowElemEnd[RowCount - 1]);
		std::copy(EachRowElemEnd.begin(), EachRowElemEnd.end(), OutSparseMatrixCSR.RowEndIndex);
		//std::copy(&(InSparseMatrixCSRDiscrete.Data[0].begin()->second),&(InSparseMatrixCSRDiscrete.Data[0].end()->second), OutSparseMatrixCSR.Values);
		//std::copy(InSparseMatrixCSRDiscrete.Data.begin(), InSparseMatrixCSRDiscrete.Data.end(), OutSparseMatrixCSR.Values+ EachRowElemEnd[i]);
		//int RowStartIndex = 0;
		//可以考虑并行化进行优化
		for (i = 0; i < RowCount; i++)
		{
			auto IndexIter = InSparseMatrixCSRDiscrete[i].begin();
			for (int j = 0; j < EachRowElemCount[i]; j++)
			{
				OutSparseMatrixCSR.ColumnIndex[j + OutSparseMatrixCSR.RowStartIndex[i]] = IndexIter->first;
				OutSparseMatrixCSR.Values[j + OutSparseMatrixCSR.RowStartIndex[i]] = IndexIter->second;
				IndexIter++;
			}
		}
	}
	else
	{
		printf("%s\n", "离散CSR矩阵转换到CSR矩阵出错");
	}
	//OutSparseMatrixCSR.RowCount = InSparseMatrixCSRDiscrete.Data.size();
	//OutSparseMatrixCSR.ColumnCount = InSparseMatrixCSRDiscrete.CloumnCount;

	//InSparseMatrixCSR.CreateCSRDataSpace()
}

void CSparseMatrix::ConvertCSRDiscreteToCSR(CSparseMatrixDiscreteCSR & InSparseMatrixCSRDiscrete, CSparseMatrixCSR *& InSparseMatrixCSR)
{
	NSTanBasicObjOpt::CheckAndReturnObj(InSparseMatrixCSR);
	ConvertCSRDiscreteToCSR(InSparseMatrixCSRDiscrete, *InSparseMatrixCSR);
}

void CSparseMatrix::ConvertCSRDiscreteToCSR(CSparseMatrixDiscreteCSR & InSparseMatrixCSRDiscrete)
{
	ConvertCSRDiscreteToCSR(InSparseMatrixCSRDiscrete, pCSR_FORMAT);
}

void CSparseMatrix::ConvertCSRDiscreteToCSR()
{
	ConvertCSRDiscreteToCSR(*pCSR_DISCRETE_FORMAT);
}

void CSparseMatrix::AddMatrixByMKL(CSparseMatrix & InAMatrix, CSparseMatrix & InBMatrix, CSparseMatrix & OutCMatrix)
{
	NSTanBasicObjOpt::CheckAndReturnObj(OutCMatrix.pMKL_FORMAT);
	OutCMatrix.pMKL_FORMAT->AddMatrix(0,1.0,*InAMatrix.pMKL_FORMAT, *InBMatrix.pMKL_FORMAT);
}

void CSparseMatrix::AddMatrixByMKL(CSparseMatrix & InAMatrix, CSparseMatrix & InBMatrix)
{
	AddMatrixByMKL(InAMatrix, InBMatrix, *this);
}

void CSparseMatrix::AddMatrixByMKL(CSparseMatrix & InAMatrix)
{
	AddMatrixByMKL(InAMatrix, *this);
}

void CSparseMatrix::MultiMatrixByMKL(CSparseMatrix & InAMatrix, CSparseMatrix & InBMatrix, CSparseMatrix & OutCMatrix)
{
	NSTanBasicObjOpt::CheckAndReturnObj(OutCMatrix.pMKL_FORMAT);
	OutCMatrix.pMKL_FORMAT->MultiMatrix(0,*InAMatrix.pMKL_FORMAT, *InBMatrix.pMKL_FORMAT, *OutCMatrix.pMKL_FORMAT);
}

void CSparseMatrix::MultiMatrixByMKL(CSparseMatrix & InAMatrix, CSparseMatrix & InBMatrix)
{
	MultiMatrixByMKL(InAMatrix, InBMatrix, *this);
}

void CSparseMatrix::MultiMatrixByMKL(CSparseMatrix & InBMatrix)
{
	MultiMatrixByMKL(*this, InBMatrix, *this);
}

bool CSparseMatrix::CheckMatrixIndexVector(TanIntVector *& InMatrixColumnIndexVector, int & MatrixColumnCount, TanIntVector *& OutMatrixColumnIndexVector)
{
	bool IsCreateAColumnList = 0;
	if (InMatrixColumnIndexVector)
	{
		OutMatrixColumnIndexVector = InMatrixColumnIndexVector;
	}
	else
	{
		OutMatrixColumnIndexVector = new TanIntVector(MatrixColumnCount);
		std::iota(OutMatrixColumnIndexVector->begin(), OutMatrixColumnIndexVector->end(), 0);
		IsCreateAColumnList = 1;
	}
	return IsCreateAColumnList;
}
