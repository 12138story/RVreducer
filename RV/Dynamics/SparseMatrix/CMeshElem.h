#pragma once
#include "CommonDefine.h"
#include "CNode.h"
#include "CMaterials.h"
#include "CRealConstant.h"
#include "CSparseMatrix.h"
class CMeshElem
{
public:
	int ElemTypeNotation = 10;
	int NodeCount = 2;
	int FreedomCount = 12;
	TanIntVector FreedomNotation;
	CMeshElem();
	~CMeshElem();
	static void GenStiffMatrixDense(std::vector<CNode*>& NodeAddressList, CMaterials* InMat, CRealConstant* InRealConst, CDenseMatrix& OutStiffMatrix);
	static void GenStiffMatrixCSRDiscrte(std::vector<CNode*>& NodeAddressList, CMaterials* InMat, CRealConstant* InRealConst,CSparseMatrixDiscreteCSR& OutStiffMatrix);
	void ReturnFreedomList(TanIntVector& OutFreedomNotation);
};