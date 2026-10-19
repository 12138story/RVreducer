#pragma once
#include <vector>
#include "CNode.h"
//#include "CElemType.h"
#include "CMaterials.h"
#include "CRealConstant.h"
#include "CSparseMatrix.h"
#include "NSTanIOStream.h"
struct tagInnerElemPara
{	
	int ElemTypeNotation = 0;
	int NodeCount = 0;
	int FreedomCount = 0;
	TanIntVector FreedomNotationlist;
	//void(*pGenStiffMatrixDense)(std::vector<CNode*>&, CMaterials*&, CRealConstant*&, CDenseMatrix&) = 0;
	void(*pGenStiffMatrix)(std::vector<CNode*>&, CMaterials*&, CRealConstant*&, CSparseMatrix&) = 0;
	//void(*pGenStiffMatrixCSRDiscrte)(std::vector<CNode*>&, CMaterials*&, CRealConstant*&, CSparseMatrixDiscreteCSR&) = 0;
	//void* GenStiffMatrixCSRDiscrte = 0;
	void* ReturnFreedomListFun = 0;
};
class CInnerElemTypeList:
	public std::vector<tagInnerElemPara>
{
public:
	CInnerElemTypeList();
	~CInnerElemTypeList();
};
namespace NSTanInnerElem
{
	namespace MeshElemFun
	{
		static void GenStandardStiffMatrix(std::vector<CNode*>& NodeAddressList, CMaterials*& InMat, CRealConstant*& InRealConst, double*& OutStiffMatrixValues);
		static void GenStiffMatrixDense(std::vector<CNode*>& NodeAddressList, CMaterials*& InMat, CRealConstant*& InRealConst, CDenseMatrix& OutStiffMatrix);
		static void GenStiffMatrixCSR(std::vector<CNode*>& NodeAddressList, CMaterials*& InMat, CRealConstant*& InRealConst, CSparseMatrixCSR& OutStiffMatrix);
		static void GenStiffMatrixCSRDiscrte(std::vector<CNode*>& NodeAddressList, CMaterials*& InMat, CRealConstant*& InRealConst, CSparseMatrixDiscreteCSR& OutStiffMatrix);
		static void GenStiffMatrix(std::vector<CNode*>& NodeAddressList, CMaterials*& InMat, CRealConstant*& InRealConst, CSparseMatrix& OutStiffMatrix);
	}
	namespace EulerBeamFun
	{
		static void GenStardardStiffMatrixDense(double& length, double& Emodulus, double& Gmodulus, double& Ix, double& Iy, double& Ipz, double& Area, double*& OutStiffMatrix);
		static void GenStardardStiffMatrixCSR(double& length, double& Emodulus, double& Gmodulus, double& Ix, double& Iy, double& Ipz, double& Area, CSparseMatrixCSR& OutStiffMatrix);
		static void GenStardardStiffMatrixCSRDiscrte(double& length, double& Emodulus, double& Gmodulus, double& Ix, double& Iy, double& Ipz, double& Area, CSparseMatrixDiscreteCSR& OutStiffMatrix);
		//static void GenStiffMatrixDense(std::vector<CNode*>& NodeAddressList, CMaterials*& InMat, CRealConstant*& InRealConst, CDenseMatrix& OutStiffMatrix);
		static void TransformElemSysMatrixToGlobalSysMatrix(double*& InCoorTransFromGlobalToLocal, CSparseMatrix& InMatrix, CSparseMatrix& OutMatrix);
		//static void TransformElemSysMatrixToGlobalSysMatrix(CTanVector& InUnitElemAxial, CSparseMatrix& InMatrix, CSparseMatrix& OutMatrix);
		//仅包含判断轴线是否具有差异性的坐标变换的全局刚度生成函数，未考虑截面惯性矩对应坐标轴的差异，也即截面各向不同的情况
		void GenStiffMatrix(
			std::vector<CNode*>& NodeAddressList, 
			CMaterials*& InMat, 
			CRealConstant*& InRealConst, //Ix,Iy,Iz（轴向）,面积
			CSparseMatrix& OutStiffMatrix);

	}
}
