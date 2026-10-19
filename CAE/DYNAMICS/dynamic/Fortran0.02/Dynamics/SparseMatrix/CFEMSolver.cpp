#include "CFEMSolver.h"
CInnerElemTypeList& CFEMSolver::InnerElemTypeList= CFEMPreprocessor::InnerElemTypeList;
CFEMSolver::CFEMSolver()
{
	//InnerElemTypeList= CFEMSolver::InnerElemTypeList;
}

CFEMSolver::~CFEMSolver()
{
}


void CFEMSolver::CreatMatrixSpace(int InSolvedFreedomNum, CFEMSolver & IOFemBasicModel)
{
	IOFemBasicModel.StiffMatrix.CreateDiscreteCSRObj();
	IOFemBasicModel.StiffMatrix.pCSR_DISCRETE_FORMAT->Set(InSolvedFreedomNum, InSolvedFreedomNum);
}

void CFEMSolver::CreatMatrixSpace(int InSolveFreedomNum)
{
	CreatMatrixSpace(InSolveFreedomNum, *this);
}

//void CFEMSolver::MapNodeElemAndFreedom(TanNodeListType & InNodeList, TanElemListType & InElemList, TanElemTypeListType & InElemTypeList)
//{
//	size_t ElemCount = InElemList.size();
//	size_t NodeCountInOneElemCount = 0;
//	//size_t i = 0;
//	TanElemListType::iterator i = InElemList.begin();
//	//size_t i = InElemList.begin();
//	size_t j = 0;	
//	CNode* NodeAddress = 0;
//	int InnerElemTypeIndex = 0;
//	for (i = InElemList.begin();i != InElemList.end();i++)
//	{
//		NodeCountInOneElemCount = i->second->NodeNumList.size();
//		for (j=0;j< NodeCountInOneElemCount;j++)
//		{
//			InnerElemTypeIndex = i->second->ElemTypeNum.second->InnerTypeNum;
//			//NodeAddress = i->second->ElemNum.second->NodeAddressList[j];
//			NodeAddress = i->second->NodeAddressList[j];
//			NodeAddress->AssignFreedom(
//				InnerElemTypeList[InnerElemTypeIndex].FreedomNotationlist,
//				NodeAddress->FreedomList,
//				i->second->FreedomAddressList[j]
//			);
//			//InElemList[i]->ElemTypeNum
//		}
//	}
//}
//
//void CFEMSolver::MapNodeElemAndFreedom(CFEMPreprocessor & InPrepData)
//{
//	MapNodeElemAndFreedom(InPrepData.NodeList, InPrepData.ElemList, InPrepData.ElemTypeList);
//}

size_t CFEMSolver::CountNodeFreedom(TanNodeListType & InNodeList)
{
	size_t NodeFreedomCount = 0;
	for (TanNodeListType::iterator i= InNodeList.begin();i!=InNodeList.end();i++)		
	{
		NodeFreedomCount = NodeFreedomCount + (i->second->FreedomList.size());
	}
	return NodeFreedomCount;
}

void CFEMSolver::BuildNodeToGlobalMap(TanNodeListType & InNodeList, CBoundaryCondition& InBCList, TanDoubleVector & OutTotalFreedomValue, TanDoubleVector & OutTotalForceValue, TanIntVector & SolveMatrixIndexFromFull, TanIntVector & OutFixIndex)
{
	//size_t NodeCount = InNodeList.size();

	size_t FixedFreedomCount = 0;
	//集成边界条件
	for (CBoundaryCondition::iterator BCIter= InBCList.begin(); BCIter!= InBCList.end(); BCIter++)
	{
		for (std::map<int,double>::iterator BCIter2 = BCIter->second.begin(); BCIter2!= BCIter->second.end(); BCIter2++)
		{
			if (BCIter2->first < 1000)
			{
				if (InNodeList[BCIter->first]->FreedomList.count(BCIter2->first))
				{
					InNodeList[BCIter->first]->FreedomList[BCIter2->first].FreedomValue = &BCIter2->second;
					InNodeList[BCIter->first]->FreedomList[BCIter2->first].ToFullMatrixIndex = -1;
					InNodeList[BCIter->first]->IsAssignDirichletBoundaryCondition = 1;
					FixedFreedomCount++;
				}
			}
			else
			{
				if (InNodeList[BCIter->first]->FreedomList.count(BCIter2->first-1000))
				{
					InNodeList[BCIter->first]->FreedomList[BCIter2->first - 1000].FreedomValue = &BCIter2->second;
					InNodeList[BCIter->first]->FreedomList[BCIter2->first - 1000].ToFullMatrixIndex = -2;
				}

			}

		}
		//if (BCIter->second->FreedomIndex<1000)
		//{
		//	InNodeList[(*BCIter)->Num]->FreedomList[(*BCIter)->FreedomIndex].FreedomValue = &(*BCIter)->FreedomValue;
		//	InNodeList[(*BCIter)->Num]->FreedomList[(*BCIter)->FreedomIndex].ToFullMatrixIndex = -1;//标识为固定约束
		//	InNodeList[(*BCIter)->Num]->IsAssignDirichletBoundaryCondition = 1;
		//	FixedFreedomCount++;
		//}
		//else
		//{
		//	InNodeList[(*BCIter)->Num]->FreedomList[(*BCIter)->FreedomIndex - 1000].FreedomValue = &(*BCIter)->FreedomValue;
		//	InNodeList[(*BCIter)->Num]->FreedomList[(*BCIter)->FreedomIndex - 1000].ToFullMatrixIndex = -2;//标识为力约束
		//}
	}
	//生成总体自由和边界值数组
	size_t TotalNodeFreedom = CountNodeFreedom(InNodeList);
	TanDoubleVector(TotalNodeFreedom, 0).swap(OutTotalFreedomValue);
	TanDoubleVector(TotalNodeFreedom, 0).swap(OutTotalForceValue);
	TanIntVector(TotalNodeFreedom - FixedFreedomCount,0).swap(SolveMatrixIndexFromFull);
	TanIntVector(FixedFreedomCount, 0).swap(OutFixIndex);

	//TanIntVector(TotalNodeFreedom - FixedFreedomCount).swap(SolveMatrixIndexFromFull);

	//std::deque<int> TempSolveMatrixIndexFromFull;
	//std::deque<double> TempOutTotalRightEqu;
	//std::deque<double> TempOutTotalFreedomValue;
	//std::deque<int> TempOutFixIndex;
	int TotalIndex = 0;
	int FixedFreedomIndex = 0;
	int SolveMatrixIndex = 0;
	for (TanNodeListType::iterator NodeIter=InNodeList.begin();NodeIter!=InNodeList.end();NodeIter++)
	{
		for (std::map<int, CNode::tagNodeFreedom>::iterator NodeFreedomIter=NodeIter->second->FreedomList.begin(); 
			NodeFreedomIter!= NodeIter->second->FreedomList.end();
			NodeFreedomIter++)
		{			
			switch (NodeFreedomIter->second.ToFullMatrixIndex)
			{
			case 0://无边界条件
				NodeFreedomIter->second.ToFullMatrixIndex = TotalIndex;
				SolveMatrixIndexFromFull[SolveMatrixIndex] = TotalIndex;
				SolveMatrixIndex++;
				break;
			case -1://固定边界条件
				NodeFreedomIter->second.ToFullMatrixIndex = TotalIndex;
				OutFixIndex[FixedFreedomIndex] = TotalIndex;
				OutTotalFreedomValue[TotalIndex] = *(NodeFreedomIter->second.FreedomValue);
				FixedFreedomIndex++;
				break;
			case -2:
				NodeFreedomIter->second.ToFullMatrixIndex = TotalIndex;
				SolveMatrixIndexFromFull[SolveMatrixIndex] = TotalIndex;
				OutTotalForceValue[TotalIndex]= *(NodeFreedomIter->second.FreedomValue);
				SolveMatrixIndex++;
				break;
			}
			NodeFreedomIter->second.FreedomValue = &OutTotalForceValue[TotalIndex];
			TotalIndex++;
		}
	}



}

void CFEMSolver::BuildNodeToGlobalMap(TanNodeListType & InNodeList, CBoundaryCondition & InBCList)
{
	BuildNodeToGlobalMap(InNodeList, 
		InBCList, 
		TotalUValue, 
		TotalFValue,
		SolveMatrixIndexFromFull,
		FixedIndex);
}

void CFEMSolver::BuildNodeToGlobalMap(CFEMPreprocessor& IOPrepData)
{
	BuildNodeToGlobalMap(IOPrepData.NodeList, IOPrepData.BCCondition);
}

void CFEMSolver::AssembleSingleElemStiff(CElem *& InSingleElem,  CSparseMatrix& IOStiffMatrix)
{
	CSparseMatrix ElemStiffMatrix;
	InSingleElem->GenStiffMatrix(InnerElemTypeList, ElemStiffMatrix);
	//生成单元到总体矩阵的映射
	std::vector<std::vector<CNode::tagNodeFreedom*>>::iterator ElemNodeIter = InSingleElem->FreedomAddressList.begin();
	std::vector<CNode::tagNodeFreedom*>::iterator NodeFreedomIter;
	TanIntVector AssembleMatrixIndexList(InnerElemTypeList[InSingleElem->ElemTypeNum.second->InnerTypeNum].FreedomCount, 0);
	size_t AssembleMatrixIndexIndex = 0;
	for (ElemNodeIter=InSingleElem->FreedomAddressList.begin(); ElemNodeIter!= InSingleElem->FreedomAddressList.end(); ElemNodeIter++)
	{
		for (NodeFreedomIter = ElemNodeIter->begin(); NodeFreedomIter != ElemNodeIter->end(); NodeFreedomIter++)
		{
			AssembleMatrixIndexList[AssembleMatrixIndexIndex] = (*NodeFreedomIter)->ToFullMatrixIndex;
			AssembleMatrixIndexIndex++;
		}
	}
	TanIntVector ElemMatrixIndexList(InnerElemTypeList[InSingleElem->ElemTypeNum.second->InnerTypeNum].FreedomCount, 0);
	std::iota(ElemMatrixIndexList.begin(), ElemMatrixIndexList.end(), 0);
	if (ElemStiffMatrix.pCSR_FORMAT)
	{
		IOStiffMatrix.AddSubMatrixByCSRDiscrete(
			AssembleMatrixIndexList, AssembleMatrixIndexList, *IOStiffMatrix.pCSR_DISCRETE_FORMAT,
			ElemMatrixIndexList, ElemMatrixIndexList, *ElemStiffMatrix.pCSR_FORMAT,
			AssembleMatrixIndexList, AssembleMatrixIndexList, *IOStiffMatrix.pCSR_DISCRETE_FORMAT
		);
	}
	else if (ElemStiffMatrix.pCSR_DISCRETE_FORMAT)
	{
		IOStiffMatrix.AddSubMatrixByCSRDiscrete(
			AssembleMatrixIndexList, AssembleMatrixIndexList, *IOStiffMatrix.pCSR_DISCRETE_FORMAT,
			ElemMatrixIndexList, ElemMatrixIndexList, *ElemStiffMatrix.pCSR_DISCRETE_FORMAT,
			AssembleMatrixIndexList, AssembleMatrixIndexList, *IOStiffMatrix.pCSR_DISCRETE_FORMAT
		);
	}
	else if (ElemStiffMatrix.pDENSE_FORMAT)
	{
		IOStiffMatrix.AddSubMatrixByCSRDiscrete(
			AssembleMatrixIndexList, AssembleMatrixIndexList, *IOStiffMatrix.pCSR_DISCRETE_FORMAT,
			ElemMatrixIndexList, ElemMatrixIndexList, *ElemStiffMatrix.pDENSE_FORMAT,
			AssembleMatrixIndexList, AssembleMatrixIndexList, *IOStiffMatrix.pCSR_DISCRETE_FORMAT
		);
	}
	//IOStiffMatrix.AddSubMatrixByCSRDiscrete()
}

void CFEMSolver::AssembleElemListStiff(TanElemListType & InElemList, CSparseMatrix & IOStiffMatrix)
{
	std::for_each(InElemList.begin(), InElemList.end(), [&IOStiffMatrix](TanElemListType::reference& ElemPair) {
		AssembleSingleElemStiff(ElemPair.second, IOStiffMatrix);
	});
}

void CFEMSolver::GenStaticSolveEquCSR(CSparseMatrix & InStiffMatrix, TanDoubleVector& InTotalFreedomValue, TanDoubleVector & InTotalForce, TanIntVector & InSolveMatrixMapFromFull, TanIntVector & InFixedIndexList, CSparseMatrixCSR & OutCSRMatrix, TanDoubleVector & OutSolveEquRight)
{
	CSparseMatrix::ObtainSubMatrixFromCSR(InSolveMatrixMapFromFull, InSolveMatrixMapFromFull, *InStiffMatrix.pCSR_DISCRETE_FORMAT, OutCSRMatrix);
	CSparseMatrix FixedMatrixElem;
	FixedMatrixElem.CreateCSRObj();
	CSparseMatrix::ObtainSubMatrixFromCSR(InSolveMatrixMapFromFull, InFixedIndexList, *InStiffMatrix.pCSR_DISCRETE_FORMAT, *FixedMatrixElem.pCSR_FORMAT);
	FixedMatrixElem.ConvertCSRToMKL();
	size_t FixedFreedomCount = InFixedIndexList.size();
	double* ToEquRightFixedFreedomValue = new double[FixedFreedomCount]();
	for (int i=0;i< FixedFreedomCount;i++)
	{
		ToEquRightFixedFreedomValue[i] = InTotalFreedomValue[InFixedIndexList[i]];
	}
	size_t SolveFreedomCount = InSolveMatrixMapFromFull.size();
	TanDoubleVector(SolveFreedomCount, 0).swap(OutSolveEquRight);
	double* EquRightPlusValueByFix = new double[SolveFreedomCount]();
	for (size_t i = 0; i < SolveFreedomCount; i++)
	{
		OutSolveEquRight[i]= InTotalForce[InSolveMatrixMapFromFull[i]];
	}
	
	FixedMatrixElem.pMKL_FORMAT->MultiVector(0, (int)SolveFreedomCount, ToEquRightFixedFreedomValue, EquRightPlusValueByFix);
	cblas_daxpy((int)SolveFreedomCount, -1.0, EquRightPlusValueByFix, 1, &OutSolveEquRight[0], 1);
	delete[] ToEquRightFixedFreedomValue;
	delete[] EquRightPlusValueByFix;
	//sparse_matrix_t A=
	//mkl_sparse_d_mv(SPARSE_OPERATION_NON_TRANSPOSE, 
	//	1.0,
	//	const sparse_matrix_t A, struct matrix_descr descr, const double *x, double beta, double *y)
}

void CFEMSolver::GenStaticSolveEquCSR(CSparseMatrixCSR& OutCSRMatrix,
	TanDoubleVector& OutSolveEquRight)
{
	GenStaticSolveEquCSR(StiffMatrix,
		TotalUValue,
		TotalFValue,
		SolveMatrixIndexFromFull,
		FixedIndex,
		OutCSRMatrix,
		OutSolveEquRight);
}

void CFEMSolver::StaticAnalysis(CFEMPreprocessor & InFEMPrepModel)
{
	InFEMPrepModel.ConstructPrepDataMap();
	BuildNodeToGlobalMap(InFEMPrepModel);
	CreatMatrixSpace((int)TotalUValue.size());
	AssembleElemListStiff(InFEMPrepModel.ElemList, StiffMatrix);
	CSparseMatrix* SolveMatrix=new CSparseMatrix;
	TanDoubleVector* SolveEquRight=new TanDoubleVector;
	SolveMatrix->CreateCSRObj();
	GenStaticSolveEquCSR(*SolveMatrix->pCSR_FORMAT, *SolveEquRight);
	double* SolveFreedomValueList = new double[SolveEquRight->size()]();
	SolveMatrix->pCSR_FORMAT->SolveLinearEquByParallelDSS(&SolveEquRight->operator[](0), SolveFreedomValueList);
	for (size_t i=0; i<SolveMatrixIndexFromFull.size();i++)
	{
		TotalUValue[SolveMatrixIndexFromFull[i]] = SolveFreedomValueList[i];
	}
	//获取其它固定节点支反力
	CSparseMatrix FixedMatrixElem;
	FixedMatrixElem.CreateCSRObj();
	size_t FixedFreedomCount = FixedIndex.size();
	if (FixedFreedomCount)
	{
		CSparseMatrix::ObtainSubMatrixFromCSR(&FixedIndex, 0, *StiffMatrix.pCSR_DISCRETE_FORMAT, *FixedMatrixElem.pCSR_FORMAT);
	}
	else
	{
		StiffMatrix.ConvertCSRDiscreteToCSR(*StiffMatrix.pCSR_DISCRETE_FORMAT, *FixedMatrixElem.pCSR_FORMAT);
	}
	double* OtherFValue = new double[FixedFreedomCount]();
	FixedMatrixElem.ConvertCSRToMKL();
	FixedMatrixElem.pMKL_FORMAT->MultiVector(0, (int)FixedFreedomCount, &TotalUValue[0], OtherFValue);
	for (size_t i = 0; i < FixedFreedomCount; i++)
	{
		TotalFValue[FixedIndex[i]] = OtherFValue[i];
	}

	delete SolveMatrix;
	delete SolveEquRight;
	delete[] SolveFreedomValueList;
	delete[] OtherFValue;
}

void CFEMSolver::SolveLinearEquCSR(CSparseMatrixCSR & InCSRMatrix, TanDoubleVector & TanEquRight, TanDoubleVector & SolveFreedomValueList)
{
	InCSRMatrix.SolveLinearEquByParallelDSS(&TanEquRight[0], &SolveFreedomValueList[0]);

}

void CFEMSolver::SolveFEMModel(int InAnalysisType, CFEMPreprocessor & PrepModel)
{
	PrepModel.ConstructPrepDataMap();
	switch (InAnalysisType)
	{
	case 0:
		StaticAnalysis(PrepModel);
		break;
	default:
		break;
	}
}
