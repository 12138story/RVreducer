#pragma once
#include "CSparseMatrix.h"
//#include "CTanVector.h"
//#include "CNode.h"
//#include "CElem.h"
//#include "CInnerElemTypeList.h"
//#include "CBoundaryCondition.h"
//typedef std::map<int, CElem*> TanElemListType;
#include "CFEMPreprocessor.h"
//typedef std::map<int, CBoundaryCondition*> TanBCListType;
//typedef std::deque<CBoundaryCondition*> TanBCListType;

class CFEMSolver
{
public:
	CSparseMatrix StiffMatrix;
	CSparseMatrix MassMatrix;
	TanDoubleVector TotalUValue;
	TanDoubleVector TotalFValue;
	TanIntVector SolveMatrixIndexFromFull;
	TanIntVector FixedIndex;
	static CInnerElemTypeList& InnerElemTypeList;
	CFEMSolver();
	~CFEMSolver();
	static void CreatMatrixSpace(int InSolvedFreedomNum,CFEMSolver& IOFemBasicModel);
	void CreatMatrixSpace(int InSolveFreedomNum);

	//计算节点自由度
	static size_t  CountNodeFreedom(TanNodeListType& InNodeList);
	//构建整体到求解矩阵映射
	static void BuildNodeToGlobalMap(
		TanNodeListType& InNodeList,
		CBoundaryCondition& InBCList,
		TanDoubleVector& OutTotalFreedomValue,
		TanDoubleVector& OutTotalForceValue,
		TanIntVector& SolveMatrixIndexFromFull,
		TanIntVector& OutFixIndex
		);
	void BuildNodeToGlobalMap(
		TanNodeListType& InNodeList,
		CBoundaryCondition& InBCList);
	void BuildNodeToGlobalMap(CFEMPreprocessor& IOPrepData);

	//组装单个单元刚度
	static void AssembleSingleElemStiff(CElem*& InSingleElem,CSparseMatrix& IOStiffMatrix);
	//组装多个单元刚度
	static void  AssembleElemListStiff(	TanElemListType& InElemList,CSparseMatrix& IOStiffMatrix);

	//生成CSR格式稀疏求解矩阵
	static void GenStaticSolveEquCSR(
		CSparseMatrix& InStiffMatrix,
		TanDoubleVector& InTotalFreedom,
		TanDoubleVector& InTotalForce,
		TanIntVector& InSolveMatrixMapFromFull,
		TanIntVector& InFixedIndexList,
		CSparseMatrixCSR& OutCSRMatrix,
		TanDoubleVector& OutSolveEquRight);
	void GenStaticSolveEquCSR(CSparseMatrixCSR& OutCSRMatrix,
		TanDoubleVector& OutSolveEquRight);
	//静态分析
	void StaticAnalysis(CFEMPreprocessor& InFEMPrepModel);
	static void SolveLinearEquCSR(CSparseMatrixCSR& InCSRMatrix, TanDoubleVector& InTanEquRight, TanDoubleVector& OutSolveFreedomValueList);
	//根据前处理数据求解方程
	void SolveFEMModel(int InAnalysisType,CFEMPreprocessor & PrepModel);
};

