#include "CInnerElemTypeList.h"

CInnerElemTypeList::CInnerElemTypeList()
{
	std::vector<tagInnerElemPara>(4, tagInnerElemPara()).swap(*this);
	//啮合单元
	(*this)[0].NodeCount = 2;
	(*this)[0].FreedomCount = 12;
	//TanIntVector{ 0,1,2,3,4,5}.swap((*this)[0].FreedomNotationlist);
	TanIntVector{ 1,2,3,4,5,6 }.swap((*this)[0].FreedomNotationlist);
	//(*this)[0].pGenStiffMatrixDense = &NSTanInnerElem::MeshElemFun::GenStiffMatrixDense;
	(*this)[0].pGenStiffMatrix = &NSTanInnerElem::MeshElemFun::GenStiffMatrix;
	//(*this)[0].pGenStiffMatrixCSRDiscrte = &NSTanInnerElem::MeshElemFun::GenStiffMatrixCSRDiscrte;
	//欧拉梁单元
	(*this)[1].NodeCount = 2;
	(*this)[1].FreedomCount = 12;
	//TanIntVector{ 0,1,2,3,4,5}.swap((*this)[0].FreedomNotationlist);
	TanIntVector{ 1,2,3,4,5,6 }.swap((*this)[1].FreedomNotationlist);
	//(*this)[1].pGenStiffMatrixDense = &NSTanInnerElem::EulerBeamFun::GenStiffMatrixDense;
	(*this)[1].pGenStiffMatrix = &NSTanInnerElem::EulerBeamFun::GenStiffMatrix;
	
}

CInnerElemTypeList::~CInnerElemTypeList()
{
}
#pragma region 啮合单元
void NSTanInnerElem::MeshElemFun::GenStandardStiffMatrix(std::vector<CNode*>& NodeAddressList, CMaterials *& InMat, CRealConstant *& InRealConst, double *& OutStiffMatrixValues)
{
		double Rx1 = InRealConst->ParaList[0] - NodeAddressList[0]->NodeCoor[0];
	double Ry1 = InRealConst->ParaList[1] - NodeAddressList[0]->NodeCoor[1];
	double Rz1 = InRealConst->ParaList[2] - NodeAddressList[0]->NodeCoor[2];
	double Rx2 = InRealConst->ParaList[0] - NodeAddressList[1]->NodeCoor[0];
	double Ry2 = InRealConst->ParaList[1] - NodeAddressList[1]->NodeCoor[1];
	double Rz2 = InRealConst->ParaList[2] - NodeAddressList[1]->NodeCoor[2];
	CTanVector Normal(3, &(InRealConst->ParaList[3]));
	double NormPow = pow(Normal[0], 2) + pow(Normal[1], 2) + pow(Normal[2], 2);
	Normal.ScaleProduct(sqrt(NormPow));
	double& nx = Normal[0];
	double& ny = Normal[1];
	double& nz = Normal[2];
	OutStiffMatrixValues[0] = pow(nx, 2);
	OutStiffMatrixValues[1] = nx * ny;
	OutStiffMatrixValues[2] = nx * nz;
	OutStiffMatrixValues[3] = nx * (nz*Ry1 - ny * Rz1);
	OutStiffMatrixValues[4] = nx * (-(nz*Rx1) + nx * Rz1);
	OutStiffMatrixValues[5] = nx * (ny*Rx1 - nx * Ry1);
	OutStiffMatrixValues[6] = -pow(nx, 2);
	OutStiffMatrixValues[7] = -(nx*ny);
	OutStiffMatrixValues[8] = -(nx*nz);
	OutStiffMatrixValues[9] = nx * (-(nz*Ry2) + ny * Rz2);
	OutStiffMatrixValues[10] = nx * (nz*Rx2 - nx * Rz2);
	OutStiffMatrixValues[11] = nx * (-(ny*Rx2) + nx * Ry2);
	OutStiffMatrixValues[12] = nx * ny;
	OutStiffMatrixValues[13] = pow(ny, 2);
	OutStiffMatrixValues[14] = ny * nz;
	OutStiffMatrixValues[15] = ny * (nz*Ry1 - ny * Rz1);
	OutStiffMatrixValues[16] = ny * (-(nz*Rx1) + nx * Rz1);
	OutStiffMatrixValues[17] = ny * (ny*Rx1 - nx * Ry1);
	OutStiffMatrixValues[18] = -(nx*ny);
	OutStiffMatrixValues[19] = -pow(ny, 2);
	OutStiffMatrixValues[20] = -(ny*nz);
	OutStiffMatrixValues[21] = ny * (-(nz*Ry2) + ny * Rz2);
	OutStiffMatrixValues[22] = ny * (nz*Rx2 - nx * Rz2);
	OutStiffMatrixValues[23] = ny * (-(ny*Rx2) + nx * Ry2);
	OutStiffMatrixValues[24] = nx * nz;
	OutStiffMatrixValues[25] = ny * nz;
	OutStiffMatrixValues[26] = pow(nz, 2);
	OutStiffMatrixValues[27] = nz * (nz*Ry1 - ny * Rz1);
	OutStiffMatrixValues[28] = nz * (-(nz*Rx1) + nx * Rz1);
	OutStiffMatrixValues[29] = nz * (ny*Rx1 - nx * Ry1);
	OutStiffMatrixValues[30] = -(nx*nz);
	OutStiffMatrixValues[31] = -(ny*nz);
	OutStiffMatrixValues[32] = -pow(nz, 2);
	OutStiffMatrixValues[33] = nz * (-(nz*Ry2) + ny * Rz2);
	OutStiffMatrixValues[34] = nz * (nz*Rx2 - nx * Rz2);
	OutStiffMatrixValues[35] = nz * (-(ny*Rx2) + nx * Ry2);
	OutStiffMatrixValues[36] = nx * (nz*Ry1 - ny * Rz1);
	OutStiffMatrixValues[37] = ny * (nz*Ry1 - ny * Rz1);
	OutStiffMatrixValues[38] = nz * (nz*Ry1 - ny * Rz1);
	OutStiffMatrixValues[39] = pow(nz*Ry1 - ny * Rz1, 2);
	OutStiffMatrixValues[40] = -((nz*Rx1 - nx * Rz1)*(nz*Ry1 - ny * Rz1));
	OutStiffMatrixValues[41] = (-(ny*Rx1) + nx * Ry1)*(-(nz*Ry1) + ny * Rz1);
	OutStiffMatrixValues[42] = nx * (-(nz*Ry1) + ny * Rz1);
	OutStiffMatrixValues[43] = ny * (-(nz*Ry1) + ny * Rz1);
	OutStiffMatrixValues[44] = nz * (-(nz*Ry1) + ny * Rz1);
	OutStiffMatrixValues[45] = -((nz*Ry1 - ny * Rz1)*(nz*Ry2 - ny * Rz2));
	OutStiffMatrixValues[46] = (nz*Ry1 - ny * Rz1)*(nz*Rx2 - nx * Rz2);
	OutStiffMatrixValues[47] = (ny*Rx2 - nx * Ry2)*(-(nz*Ry1) + ny * Rz1);
	OutStiffMatrixValues[48] = nx * (-(nz*Rx1) + nx * Rz1);
	OutStiffMatrixValues[49] = ny * (-(nz*Rx1) + nx * Rz1);
	OutStiffMatrixValues[50] = nz * (-(nz*Rx1) + nx * Rz1);
	OutStiffMatrixValues[51] = -((nz*Rx1 - nx * Rz1)*(nz*Ry1 - ny * Rz1));
	OutStiffMatrixValues[52] = pow(nz*Rx1 - nx * Rz1, 2);
	OutStiffMatrixValues[53] = -((ny*Rx1 - nx * Ry1)*(nz*Rx1 - nx * Rz1));
	OutStiffMatrixValues[54] = nx * (nz*Rx1 - nx * Rz1);
	OutStiffMatrixValues[55] = ny * (nz*Rx1 - nx * Rz1);
	OutStiffMatrixValues[56] = nz * (nz*Rx1 - nx * Rz1);
	OutStiffMatrixValues[57] = (nz*Rx1 - nx * Rz1)*(nz*Ry2 - ny * Rz2);
	OutStiffMatrixValues[58] = -((nz*Rx1 - nx * Rz1)*(nz*Rx2 - nx * Rz2));
	OutStiffMatrixValues[59] = (-(ny*Rx2) + nx * Ry2)*(-(nz*Rx1) + nx * Rz1);
	OutStiffMatrixValues[60] = nx * (ny*Rx1 - nx * Ry1);
	OutStiffMatrixValues[61] = ny * (ny*Rx1 - nx * Ry1);
	OutStiffMatrixValues[62] = nz * (ny*Rx1 - nx * Ry1);
	OutStiffMatrixValues[63] = (-(ny*Rx1) + nx * Ry1)*(-(nz*Ry1) + ny * Rz1);
	OutStiffMatrixValues[64] = -((ny*Rx1 - nx * Ry1)*(nz*Rx1 - nx * Rz1));
	OutStiffMatrixValues[65] = pow(ny*Rx1 - nx * Ry1, 2);
	OutStiffMatrixValues[66] = nx * (-(ny*Rx1) + nx * Ry1);
	OutStiffMatrixValues[67] = ny * (-(ny*Rx1) + nx * Ry1);
	OutStiffMatrixValues[68] = nz * (-(ny*Rx1) + nx * Ry1);
	OutStiffMatrixValues[69] = (ny*Rx1 - nx * Ry1)*(-(nz*Ry2) + ny * Rz2);
	OutStiffMatrixValues[70] = (-(ny*Rx1) + nx * Ry1)*(-(nz*Rx2) + nx * Rz2);
	OutStiffMatrixValues[71] = -((ny*Rx1 - nx * Ry1)*(ny*Rx2 - nx * Ry2));
	OutStiffMatrixValues[72] = -pow(nx, 2);
	OutStiffMatrixValues[73] = -(nx*ny);
	OutStiffMatrixValues[74] = -(nx*nz);
	OutStiffMatrixValues[75] = nx * (-(nz*Ry1) + ny * Rz1);
	OutStiffMatrixValues[76] = nx * (nz*Rx1 - nx * Rz1);
	OutStiffMatrixValues[77] = nx * (-(ny*Rx1) + nx * Ry1);
	OutStiffMatrixValues[78] = pow(nx, 2);
	OutStiffMatrixValues[79] = nx * ny;
	OutStiffMatrixValues[80] = nx * nz;
	OutStiffMatrixValues[81] = nx * (nz*Ry2 - ny * Rz2);
	OutStiffMatrixValues[82] = nx * (-(nz*Rx2) + nx * Rz2);
	OutStiffMatrixValues[83] = nx * (ny*Rx2 - nx * Ry2);
	OutStiffMatrixValues[84] = -(nx*ny);
	OutStiffMatrixValues[85] = -pow(ny, 2);
	OutStiffMatrixValues[86] = -(ny*nz);
	OutStiffMatrixValues[87] = ny * (-(nz*Ry1) + ny * Rz1);
	OutStiffMatrixValues[88] = ny * (nz*Rx1 - nx * Rz1);
	OutStiffMatrixValues[89] = ny * (-(ny*Rx1) + nx * Ry1);
	OutStiffMatrixValues[90] = nx * ny;
	OutStiffMatrixValues[91] = pow(ny, 2);
	OutStiffMatrixValues[92] = ny * nz;
	OutStiffMatrixValues[93] = ny * (nz*Ry2 - ny * Rz2);
	OutStiffMatrixValues[94] = ny * (-(nz*Rx2) + nx * Rz2);
	OutStiffMatrixValues[95] = ny * (ny*Rx2 - nx * Ry2);
	OutStiffMatrixValues[96] = -(nx*nz);
	OutStiffMatrixValues[97] = -(ny*nz);
	OutStiffMatrixValues[98] = -pow(nz, 2);
	OutStiffMatrixValues[99] = nz * (-(nz*Ry1) + ny * Rz1);
	OutStiffMatrixValues[100] = nz * (nz*Rx1 - nx * Rz1);
	OutStiffMatrixValues[101] = nz * (-(ny*Rx1) + nx * Ry1);
	OutStiffMatrixValues[102] = nx * nz;
	OutStiffMatrixValues[103] = ny * nz;
	OutStiffMatrixValues[104] = pow(nz, 2);
	OutStiffMatrixValues[105] = nz * (nz*Ry2 - ny * Rz2);
	OutStiffMatrixValues[106] = nz * (-(nz*Rx2) + nx * Rz2);
	OutStiffMatrixValues[107] = nz * (ny*Rx2 - nx * Ry2);
	OutStiffMatrixValues[108] = nx * (-(nz*Ry2) + ny * Rz2);
	OutStiffMatrixValues[109] = ny * (-(nz*Ry2) + ny * Rz2);
	OutStiffMatrixValues[110] = nz * (-(nz*Ry2) + ny * Rz2);
	OutStiffMatrixValues[111] = -((nz*Ry1 - ny * Rz1)*(nz*Ry2 - ny * Rz2));
	OutStiffMatrixValues[112] = (nz*Rx1 - nx * Rz1)*(nz*Ry2 - ny * Rz2);
	OutStiffMatrixValues[113] = (ny*Rx1 - nx * Ry1)*(-(nz*Ry2) + ny * Rz2);
	OutStiffMatrixValues[114] = nx * (nz*Ry2 - ny * Rz2);
	OutStiffMatrixValues[115] = ny * (nz*Ry2 - ny * Rz2);
	OutStiffMatrixValues[116] = nz * (nz*Ry2 - ny * Rz2);
	OutStiffMatrixValues[117] = pow(nz*Ry2 - ny * Rz2, 2);
	OutStiffMatrixValues[118] = -((nz*Rx2 - nx * Rz2)*(nz*Ry2 - ny * Rz2));
	OutStiffMatrixValues[119] = (-(ny*Rx2) + nx * Ry2)*(-(nz*Ry2) + ny * Rz2);
	OutStiffMatrixValues[120] = nx * (nz*Rx2 - nx * Rz2);
	OutStiffMatrixValues[121] = ny * (nz*Rx2 - nx * Rz2);
	OutStiffMatrixValues[122] = nz * (nz*Rx2 - nx * Rz2);
	OutStiffMatrixValues[123] = (nz*Ry1 - ny * Rz1)*(nz*Rx2 - nx * Rz2);
	OutStiffMatrixValues[124] = -((nz*Rx1 - nx * Rz1)*(nz*Rx2 - nx * Rz2));
	OutStiffMatrixValues[125] = (-(ny*Rx1) + nx * Ry1)*(-(nz*Rx2) + nx * Rz2);
	OutStiffMatrixValues[126] = nx * (-(nz*Rx2) + nx * Rz2);
	OutStiffMatrixValues[127] = ny * (-(nz*Rx2) + nx * Rz2);
	OutStiffMatrixValues[128] = nz * (-(nz*Rx2) + nx * Rz2);
	OutStiffMatrixValues[129] = -((nz*Rx2 - nx * Rz2)*(nz*Ry2 - ny * Rz2));
	OutStiffMatrixValues[130] = pow(nz*Rx2 - nx * Rz2, 2);
	OutStiffMatrixValues[131] = -((ny*Rx2 - nx * Ry2)*(nz*Rx2 - nx * Rz2));
	OutStiffMatrixValues[132] = nx * (-(ny*Rx2) + nx * Ry2);
	OutStiffMatrixValues[133] = ny * (-(ny*Rx2) + nx * Ry2);
	OutStiffMatrixValues[134] = nz * (-(ny*Rx2) + nx * Ry2);
	OutStiffMatrixValues[135] = (ny*Rx2 - nx * Ry2)*(-(nz*Ry1) + ny * Rz1);
	OutStiffMatrixValues[136] = (-(ny*Rx2) + nx * Ry2)*(-(nz*Rx1) + nx * Rz1);
	OutStiffMatrixValues[137] = -((ny*Rx1 - nx * Ry1)*(ny*Rx2 - nx * Ry2));
	OutStiffMatrixValues[138] = nx * (ny*Rx2 - nx * Ry2);
	OutStiffMatrixValues[139] = ny * (ny*Rx2 - nx * Ry2);
	OutStiffMatrixValues[140] = nz * (ny*Rx2 - nx * Ry2);
	OutStiffMatrixValues[141] = (-(ny*Rx2) + nx * Ry2)*(-(nz*Ry2) + ny * Rz2);
	OutStiffMatrixValues[142] = -((ny*Rx2 - nx * Ry2)*(nz*Rx2 - nx * Rz2));
	OutStiffMatrixValues[143] = pow(ny*Rx2 - nx * Ry2, 2);
	cblas_dscal(144, InRealConst->ParaList[6] / NormPow, OutStiffMatrixValues, 1);
}
void NSTanInnerElem::MeshElemFun::GenStiffMatrixDense(std::vector<CNode*>& NodeAddressList, CMaterials *& InMat, CRealConstant *& InRealConst, CDenseMatrix & OutStiffMatrix)
{
	OutStiffMatrix.CreateDenseDataSpace(12, 12);
	GenStandardStiffMatrix(NodeAddressList, InMat, InRealConst, OutStiffMatrix.Values);
}

void NSTanInnerElem::MeshElemFun::GenStiffMatrixCSR(std::vector<CNode*>& NodeAddressList, CMaterials *& InMat, CRealConstant *& InRealConst, CSparseMatrixCSR & OutStiffMatrix)
{
	OutStiffMatrix.DeleteCSRDataSpace();
	OutStiffMatrix.Values = new double[144];
	OutStiffMatrix.RowStartIndex = new int[13]{ 0, 12, 24, 36, 48, 60, 72, 84, 96, 108, 120, 132, 144 };
	OutStiffMatrix.RowEndIndex = OutStiffMatrix.RowStartIndex + 1;
	OutStiffMatrix.RowCount = 12;
	OutStiffMatrix.ColumnIndex = new int[144]{ 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11 };
	OutStiffMatrix.ColumnCount = 12;
	GenStandardStiffMatrix(NodeAddressList, InMat, InRealConst, OutStiffMatrix.Values);	
	
	OutStiffMatrix.IsCreatSpace = 1;
}

void NSTanInnerElem::MeshElemFun::GenStiffMatrixCSRDiscrte(std::vector<CNode*>& NodeAddressList, CMaterials *& InMat, CRealConstant *& InRealConst, CSparseMatrixDiscreteCSR & OutStiffMatrix)
{
	double Rx1 = InRealConst->ParaList[0] - NodeAddressList[0]->NodeCoor[0];
	double Ry1 = InRealConst->ParaList[1] - NodeAddressList[0]->NodeCoor[1];
	double Rz1 = InRealConst->ParaList[2] - NodeAddressList[0]->NodeCoor[2];
	double Rx2 = InRealConst->ParaList[0] - NodeAddressList[1]->NodeCoor[0];
	double Ry2 = InRealConst->ParaList[1] - NodeAddressList[1]->NodeCoor[1];
	double Rz2 = InRealConst->ParaList[2] - NodeAddressList[1]->NodeCoor[2];
	CTanVector Normal(3, &(InRealConst->ParaList[3]));
	double NormPow = pow(Normal[0], 2) + pow(Normal[1], 2) + pow(Normal[2], 2);
	Normal.ScaleProduct(sqrt(NormPow));
	double& nx = Normal[0];
	double& ny = Normal[1];
	double& nz = Normal[2];
	OutStiffMatrix.Set(12, 12);
	OutStiffMatrix[0][0] = pow(nx, 2);
	OutStiffMatrix[0][1] = nx * ny;
	OutStiffMatrix[0][2] = nx * nz;
	OutStiffMatrix[0][3] = nx * (nz*Ry1 - ny * Rz1);
	OutStiffMatrix[0][4] = nx * (-(nz*Rx1) + nx * Rz1);
	OutStiffMatrix[0][5] = nx * (ny*Rx1 - nx * Ry1);
	OutStiffMatrix[0][6] = -pow(nx, 2);
	OutStiffMatrix[0][7] = -(nx*ny);
	OutStiffMatrix[0][8] = -(nx*nz);
	OutStiffMatrix[0][9] = nx * (-(nz*Ry2) + ny * Rz2);
	OutStiffMatrix[0][10] = nx * (nz*Rx2 - nx * Rz2);
	OutStiffMatrix[0][11] = nx * (-(ny*Rx2) + nx * Ry2);
	OutStiffMatrix[1][0] = nx * ny;
	OutStiffMatrix[1][1] = pow(ny, 2);
	OutStiffMatrix[1][2] = ny * nz;
	OutStiffMatrix[1][3] = ny * (nz*Ry1 - ny * Rz1);
	OutStiffMatrix[1][4] = ny * (-(nz*Rx1) + nx * Rz1);
	OutStiffMatrix[1][5] = ny * (ny*Rx1 - nx * Ry1);
	OutStiffMatrix[1][6] = -(nx*ny);
	OutStiffMatrix[1][7] = -pow(ny, 2);
	OutStiffMatrix[1][8] = -(ny*nz);
	OutStiffMatrix[1][9] = ny * (-(nz*Ry2) + ny * Rz2);
	OutStiffMatrix[1][10] = ny * (nz*Rx2 - nx * Rz2);
	OutStiffMatrix[1][11] = ny * (-(ny*Rx2) + nx * Ry2);
	OutStiffMatrix[2][0] = nx * nz;
	OutStiffMatrix[2][1] = ny * nz;
	OutStiffMatrix[2][2] = pow(nz, 2);
	OutStiffMatrix[2][3] = nz * (nz*Ry1 - ny * Rz1);
	OutStiffMatrix[2][4] = nz * (-(nz*Rx1) + nx * Rz1);
	OutStiffMatrix[2][5] = nz * (ny*Rx1 - nx * Ry1);
	OutStiffMatrix[2][6] = -(nx*nz);
	OutStiffMatrix[2][7] = -(ny*nz);
	OutStiffMatrix[2][8] = -pow(nz, 2);
	OutStiffMatrix[2][9] = nz * (-(nz*Ry2) + ny * Rz2);
	OutStiffMatrix[2][10] = nz * (nz*Rx2 - nx * Rz2);
	OutStiffMatrix[2][11] = nz * (-(ny*Rx2) + nx * Ry2);
	OutStiffMatrix[3][0] = nx * (nz*Ry1 - ny * Rz1);
	OutStiffMatrix[3][1] = ny * (nz*Ry1 - ny * Rz1);
	OutStiffMatrix[3][2] = nz * (nz*Ry1 - ny * Rz1);
	OutStiffMatrix[3][3] = pow(nz*Ry1 - ny * Rz1, 2);
	OutStiffMatrix[3][4] = -((nz*Rx1 - nx * Rz1)*(nz*Ry1 - ny * Rz1));
	OutStiffMatrix[3][5] = (-(ny*Rx1) + nx * Ry1)*(-(nz*Ry1) + ny * Rz1);
	OutStiffMatrix[3][6] = nx * (-(nz*Ry1) + ny * Rz1);
	OutStiffMatrix[3][7] = ny * (-(nz*Ry1) + ny * Rz1);
	OutStiffMatrix[3][8] = nz * (-(nz*Ry1) + ny * Rz1);
	OutStiffMatrix[3][9] = -((nz*Ry1 - ny * Rz1)*(nz*Ry2 - ny * Rz2));
	OutStiffMatrix[3][10] = (nz*Ry1 - ny * Rz1)*(nz*Rx2 - nx * Rz2);
	OutStiffMatrix[3][11] = (ny*Rx2 - nx * Ry2)*(-(nz*Ry1) + ny * Rz1);
	OutStiffMatrix[4][0] = nx * (-(nz*Rx1) + nx * Rz1);
	OutStiffMatrix[4][1] = ny * (-(nz*Rx1) + nx * Rz1);
	OutStiffMatrix[4][2] = nz * (-(nz*Rx1) + nx * Rz1);
	OutStiffMatrix[4][3] = -((nz*Rx1 - nx * Rz1)*(nz*Ry1 - ny * Rz1));
	OutStiffMatrix[4][4] = pow(nz*Rx1 - nx * Rz1, 2);
	OutStiffMatrix[4][5] = -((ny*Rx1 - nx * Ry1)*(nz*Rx1 - nx * Rz1));
	OutStiffMatrix[4][6] = nx * (nz*Rx1 - nx * Rz1);
	OutStiffMatrix[4][7] = ny * (nz*Rx1 - nx * Rz1);
	OutStiffMatrix[4][8] = nz * (nz*Rx1 - nx * Rz1);
	OutStiffMatrix[4][9] = (nz*Rx1 - nx * Rz1)*(nz*Ry2 - ny * Rz2);
	OutStiffMatrix[4][10] = -((nz*Rx1 - nx * Rz1)*(nz*Rx2 - nx * Rz2));
	OutStiffMatrix[4][11] = (-(ny*Rx2) + nx * Ry2)*(-(nz*Rx1) + nx * Rz1);
	OutStiffMatrix[5][0] = nx * (ny*Rx1 - nx * Ry1);
	OutStiffMatrix[5][1] = ny * (ny*Rx1 - nx * Ry1);
	OutStiffMatrix[5][2] = nz * (ny*Rx1 - nx * Ry1);
	OutStiffMatrix[5][3] = (-(ny*Rx1) + nx * Ry1)*(-(nz*Ry1) + ny * Rz1);
	OutStiffMatrix[5][4] = -((ny*Rx1 - nx * Ry1)*(nz*Rx1 - nx * Rz1));
	OutStiffMatrix[5][5] = pow(ny*Rx1 - nx * Ry1, 2);
	OutStiffMatrix[5][6] = nx * (-(ny*Rx1) + nx * Ry1);
	OutStiffMatrix[5][7] = ny * (-(ny*Rx1) + nx * Ry1);
	OutStiffMatrix[5][8] = nz * (-(ny*Rx1) + nx * Ry1);
	OutStiffMatrix[5][9] = (ny*Rx1 - nx * Ry1)*(-(nz*Ry2) + ny * Rz2);
	OutStiffMatrix[5][10] = (-(ny*Rx1) + nx * Ry1)*(-(nz*Rx2) + nx * Rz2);
	OutStiffMatrix[5][11] = -((ny*Rx1 - nx * Ry1)*(ny*Rx2 - nx * Ry2));
	OutStiffMatrix[6][0] = -pow(nx, 2);
	OutStiffMatrix[6][1] = -(nx*ny);
	OutStiffMatrix[6][2] = -(nx*nz);
	OutStiffMatrix[6][3] = nx * (-(nz*Ry1) + ny * Rz1);
	OutStiffMatrix[6][4] = nx * (nz*Rx1 - nx * Rz1);
	OutStiffMatrix[6][5] = nx * (-(ny*Rx1) + nx * Ry1);
	OutStiffMatrix[6][6] = pow(nx, 2);
	OutStiffMatrix[6][7] = nx * ny;
	OutStiffMatrix[6][8] = nx * nz;
	OutStiffMatrix[6][9] = nx * (nz*Ry2 - ny * Rz2);
	OutStiffMatrix[6][10] = nx * (-(nz*Rx2) + nx * Rz2);
	OutStiffMatrix[6][11] = nx * (ny*Rx2 - nx * Ry2);
	OutStiffMatrix[7][0] = -(nx*ny);
	OutStiffMatrix[7][1] = -pow(ny, 2);
	OutStiffMatrix[7][2] = -(ny*nz);
	OutStiffMatrix[7][3] = ny * (-(nz*Ry1) + ny * Rz1);
	OutStiffMatrix[7][4] = ny * (nz*Rx1 - nx * Rz1);
	OutStiffMatrix[7][5] = ny * (-(ny*Rx1) + nx * Ry1);
	OutStiffMatrix[7][6] = nx * ny;
	OutStiffMatrix[7][7] = pow(ny, 2);
	OutStiffMatrix[7][8] = ny * nz;
	OutStiffMatrix[7][9] = ny * (nz*Ry2 - ny * Rz2);
	OutStiffMatrix[7][10] = ny * (-(nz*Rx2) + nx * Rz2);
	OutStiffMatrix[7][11] = ny * (ny*Rx2 - nx * Ry2);
	OutStiffMatrix[8][0] = -(nx*nz);
	OutStiffMatrix[8][1] = -(ny*nz);
	OutStiffMatrix[8][2] = -pow(nz, 2);
	OutStiffMatrix[8][3] = nz * (-(nz*Ry1) + ny * Rz1);
	OutStiffMatrix[8][4] = nz * (nz*Rx1 - nx * Rz1);
	OutStiffMatrix[8][5] = nz * (-(ny*Rx1) + nx * Ry1);
	OutStiffMatrix[8][6] = nx * nz;
	OutStiffMatrix[8][7] = ny * nz;
	OutStiffMatrix[8][8] = pow(nz, 2);
	OutStiffMatrix[8][9] = nz * (nz*Ry2 - ny * Rz2);
	OutStiffMatrix[8][10] = nz * (-(nz*Rx2) + nx * Rz2);
	OutStiffMatrix[8][11] = nz * (ny*Rx2 - nx * Ry2);
	OutStiffMatrix[9][0] = nx * (-(nz*Ry2) + ny * Rz2);
	OutStiffMatrix[9][1] = ny * (-(nz*Ry2) + ny * Rz2);
	OutStiffMatrix[9][2] = nz * (-(nz*Ry2) + ny * Rz2);
	OutStiffMatrix[9][3] = -((nz*Ry1 - ny * Rz1)*(nz*Ry2 - ny * Rz2));
	OutStiffMatrix[9][4] = (nz*Rx1 - nx * Rz1)*(nz*Ry2 - ny * Rz2);
	OutStiffMatrix[9][5] = (ny*Rx1 - nx * Ry1)*(-(nz*Ry2) + ny * Rz2);
	OutStiffMatrix[9][6] = nx * (nz*Ry2 - ny * Rz2);
	OutStiffMatrix[9][7] = ny * (nz*Ry2 - ny * Rz2);
	OutStiffMatrix[9][8] = nz * (nz*Ry2 - ny * Rz2);
	OutStiffMatrix[9][9] = pow(nz*Ry2 - ny * Rz2, 2);
	OutStiffMatrix[9][10] = -((nz*Rx2 - nx * Rz2)*(nz*Ry2 - ny * Rz2));
	OutStiffMatrix[9][11] = (-(ny*Rx2) + nx * Ry2)*(-(nz*Ry2) + ny * Rz2);
	OutStiffMatrix[10][0] = nx * (nz*Rx2 - nx * Rz2);
	OutStiffMatrix[10][1] = ny * (nz*Rx2 - nx * Rz2);
	OutStiffMatrix[10][2] = nz * (nz*Rx2 - nx * Rz2);
	OutStiffMatrix[10][3] = (nz*Ry1 - ny * Rz1)*(nz*Rx2 - nx * Rz2);
	OutStiffMatrix[10][4] = -((nz*Rx1 - nx * Rz1)*(nz*Rx2 - nx * Rz2));
	OutStiffMatrix[10][5] = (-(ny*Rx1) + nx * Ry1)*(-(nz*Rx2) + nx * Rz2);
	OutStiffMatrix[10][6] = nx * (-(nz*Rx2) + nx * Rz2);
	OutStiffMatrix[10][7] = ny * (-(nz*Rx2) + nx * Rz2);
	OutStiffMatrix[10][8] = nz * (-(nz*Rx2) + nx * Rz2);
	OutStiffMatrix[10][9] = -((nz*Rx2 - nx * Rz2)*(nz*Ry2 - ny * Rz2));
	OutStiffMatrix[10][10] = pow(nz*Rx2 - nx * Rz2, 2);
	OutStiffMatrix[10][11] = -((ny*Rx2 - nx * Ry2)*(nz*Rx2 - nx * Rz2));
	OutStiffMatrix[11][0] = nx * (-(ny*Rx2) + nx * Ry2);
	OutStiffMatrix[11][1] = ny * (-(ny*Rx2) + nx * Ry2);
	OutStiffMatrix[11][2] = nz * (-(ny*Rx2) + nx * Ry2);
	OutStiffMatrix[11][3] = (ny*Rx2 - nx * Ry2)*(-(nz*Ry1) + ny * Rz1);
	OutStiffMatrix[11][4] = (-(ny*Rx2) + nx * Ry2)*(-(nz*Rx1) + nx * Rz1);
	OutStiffMatrix[11][5] = -((ny*Rx1 - nx * Ry1)*(ny*Rx2 - nx * Ry2));
	OutStiffMatrix[11][6] = nx * (ny*Rx2 - nx * Ry2);
	OutStiffMatrix[11][7] = ny * (ny*Rx2 - nx * Ry2);
	OutStiffMatrix[11][8] = nz * (ny*Rx2 - nx * Ry2);
	OutStiffMatrix[11][9] = (-(ny*Rx2) + nx * Ry2)*(-(nz*Ry2) + ny * Rz2);
	OutStiffMatrix[11][10] = -((ny*Rx2 - nx * Ry2)*(nz*Rx2 - nx * Rz2));
	OutStiffMatrix[11][11] = pow(ny*Rx2 - nx * Ry2, 2);
	double MatrixCoef = InRealConst->ParaList[6] / NormPow;
	//std::for_each(OutStiffMatrix.begin(), OutStiffMatrix.end(), [&OutStiffMatrix, &MatrixCoef](CSparseMatrixDiscreteCSR::reference& RowObj) {
	//	std::for_each(RowObj.begin(), RowObj.end(), [&RowObj, &MatrixCoef](std::map<int, double>::reference& MapObj) {MapObj.second = MatrixCoef * MapObj.second; });
	//});
	//for (size_t i=0;i< OutStiffMatrix.size();i++)
	//{
	//	for (size_t j=0;j< OutStiffMatrix[0].size();j++)
	//	{
	//		OutStiffMatrix[i][j] = OutStiffMatrix[i][j] * MatrixCoef;
	//	}
	//}
	std::for_each(OutStiffMatrix.begin(), OutStiffMatrix.end(), [&OutStiffMatrix, &MatrixCoef](CSparseMatrixDiscreteCSR::reference& RowObj) {
		std::for_each(RowObj.begin(), RowObj.end(), [&RowObj, &MatrixCoef](std::map<int, double>::reference& MapObj) {MapObj.second = MatrixCoef * MapObj.second; });
	});
}
void NSTanInnerElem::MeshElemFun::GenStiffMatrix(std::vector<CNode*>& NodeAddressList, CMaterials *& InMat, CRealConstant *& InRealConst, CSparseMatrix & OutStiffMatrix)
{
	//OutStiffMatrix.DeleteAllObj();
	//OutStiffMatrix.CreateDiscreteCSRObj();
	//GenStiffMatrixCSRDiscrte(NodeAddressList, InMat, InRealConst, *OutStiffMatrix.pCSR_DISCRETE_FORMAT);
	OutStiffMatrix.DeleteAllObj();
	OutStiffMatrix.CreateDenseObj();
	GenStiffMatrixDense(NodeAddressList, InMat, InRealConst, *OutStiffMatrix.pDENSE_FORMAT);
}
#pragma endregion 啮合单元

void NSTanInnerElem::EulerBeamFun::GenStardardStiffMatrixDense(double & length, double & Emodulus, double & Gmodulus, double & Ix, double & Iy, double & Ipz, double & Area, double *& OutStiffMatrix)
{
	OutStiffMatrix[0] = (12 * Emodulus*Iy) / pow(length, 3);
	OutStiffMatrix[1] = 0;
	OutStiffMatrix[2] = 0;
	OutStiffMatrix[3] = 0;
	OutStiffMatrix[4] = (6 * Emodulus*Iy) / pow(length, 2);
	OutStiffMatrix[5] = 0;
	OutStiffMatrix[6] = (-12 * Emodulus*Iy) / pow(length, 3);
	OutStiffMatrix[7] = 0;
	OutStiffMatrix[8] = 0;
	OutStiffMatrix[9] = 0;
	OutStiffMatrix[10] = (6 * Emodulus*Iy) / pow(length, 2);
	OutStiffMatrix[11] = 0;
	OutStiffMatrix[12] = 0;
	OutStiffMatrix[13] = (12 * Emodulus*Ix) / pow(length, 3);
	OutStiffMatrix[14] = 0;
	OutStiffMatrix[15] = (-6 * Emodulus*Ix) / pow(length, 2);
	OutStiffMatrix[16] = 0;
	OutStiffMatrix[17] = 0;
	OutStiffMatrix[18] = 0;
	OutStiffMatrix[19] = (-12 * Emodulus*Ix) / pow(length, 3);
	OutStiffMatrix[20] = 0;
	OutStiffMatrix[21] = (-6 * Emodulus*Ix) / pow(length, 2);
	OutStiffMatrix[22] = 0;
	OutStiffMatrix[23] = 0;
	OutStiffMatrix[24] = 0;
	OutStiffMatrix[25] = 0;
	OutStiffMatrix[26] = (Area*Emodulus) / length;
	OutStiffMatrix[27] = 0;
	OutStiffMatrix[28] = 0;
	OutStiffMatrix[29] = 0;
	OutStiffMatrix[30] = 0;
	OutStiffMatrix[31] = 0;
	OutStiffMatrix[32] = -((Area*Emodulus) / length);
	OutStiffMatrix[33] = 0;
	OutStiffMatrix[34] = 0;
	OutStiffMatrix[35] = 0;
	OutStiffMatrix[36] = 0;
	OutStiffMatrix[37] = (-6 * Emodulus*Ix) / pow(length, 2);
	OutStiffMatrix[38] = 0;
	OutStiffMatrix[39] = (4 * Emodulus*Ix) / length;
	OutStiffMatrix[40] = 0;
	OutStiffMatrix[41] = 0;
	OutStiffMatrix[42] = 0;
	OutStiffMatrix[43] = (6 * Emodulus*Ix) / pow(length, 2);
	OutStiffMatrix[44] = 0;
	OutStiffMatrix[45] = (2 * Emodulus*Ix) / length;
	OutStiffMatrix[46] = 0;
	OutStiffMatrix[47] = 0;
	OutStiffMatrix[48] = (6 * Emodulus*Iy) / pow(length, 2);
	OutStiffMatrix[49] = 0;
	OutStiffMatrix[50] = 0;
	OutStiffMatrix[51] = 0;
	OutStiffMatrix[52] = (4 * Emodulus*Iy) / length;
	OutStiffMatrix[53] = 0;
	OutStiffMatrix[54] = (-6 * Emodulus*Iy) / pow(length, 2);
	OutStiffMatrix[55] = 0;
	OutStiffMatrix[56] = 0;
	OutStiffMatrix[57] = 0;
	OutStiffMatrix[58] = (2 * Emodulus*Iy) / length;
	OutStiffMatrix[59] = 0;
	OutStiffMatrix[60] = 0;
	OutStiffMatrix[61] = 0;
	OutStiffMatrix[62] = 0;
	OutStiffMatrix[63] = 0;
	OutStiffMatrix[64] = 0;
	OutStiffMatrix[65] = (Gmodulus*Ipz) / length;
	OutStiffMatrix[66] = 0;
	OutStiffMatrix[67] = 0;
	OutStiffMatrix[68] = 0;
	OutStiffMatrix[69] = 0;
	OutStiffMatrix[70] = 0;
	OutStiffMatrix[71] = -((Gmodulus*Ipz) / length);
	OutStiffMatrix[72] = (-12 * Emodulus*Iy) / pow(length, 3);
	OutStiffMatrix[73] = 0;
	OutStiffMatrix[74] = 0;
	OutStiffMatrix[75] = 0;
	OutStiffMatrix[76] = (-6 * Emodulus*Iy) / pow(length, 2);
	OutStiffMatrix[77] = 0;
	OutStiffMatrix[78] = (12 * Emodulus*Iy) / pow(length, 3);
	OutStiffMatrix[79] = 0;
	OutStiffMatrix[80] = 0;
	OutStiffMatrix[81] = 0;
	OutStiffMatrix[82] = (-6 * Emodulus*Iy) / pow(length, 2);
	OutStiffMatrix[83] = 0;
	OutStiffMatrix[84] = 0;
	OutStiffMatrix[85] = (-12 * Emodulus*Ix) / pow(length, 3);
	OutStiffMatrix[86] = 0;
	OutStiffMatrix[87] = (6 * Emodulus*Ix) / pow(length, 2);
	OutStiffMatrix[88] = 0;
	OutStiffMatrix[89] = 0;
	OutStiffMatrix[90] = 0;
	OutStiffMatrix[91] = (12 * Emodulus*Ix) / pow(length, 3);
	OutStiffMatrix[92] = 0;
	OutStiffMatrix[93] = (6 * Emodulus*Ix) / pow(length, 2);
	OutStiffMatrix[94] = 0;
	OutStiffMatrix[95] = 0;
	OutStiffMatrix[96] = 0;
	OutStiffMatrix[97] = 0;
	OutStiffMatrix[98] = -((Area*Emodulus) / length);
	OutStiffMatrix[99] = 0;
	OutStiffMatrix[100] = 0;
	OutStiffMatrix[101] = 0;
	OutStiffMatrix[102] = 0;
	OutStiffMatrix[103] = 0;
	OutStiffMatrix[104] = (Area*Emodulus) / length;
	OutStiffMatrix[105] = 0;
	OutStiffMatrix[106] = 0;
	OutStiffMatrix[107] = 0;
	OutStiffMatrix[108] = 0;
	OutStiffMatrix[109] = (-6 * Emodulus*Ix) / pow(length, 2);
	OutStiffMatrix[110] = 0;
	OutStiffMatrix[111] = (2 * Emodulus*Ix) / length;
	OutStiffMatrix[112] = 0;
	OutStiffMatrix[113] = 0;
	OutStiffMatrix[114] = 0;
	OutStiffMatrix[115] = (6 * Emodulus*Ix) / pow(length, 2);
	OutStiffMatrix[116] = 0;
	OutStiffMatrix[117] = (4 * Emodulus*Ix) / length;
	OutStiffMatrix[118] = 0;
	OutStiffMatrix[119] = 0;
	OutStiffMatrix[120] = (6 * Emodulus*Iy) / pow(length, 2);
	OutStiffMatrix[121] = 0;
	OutStiffMatrix[122] = 0;
	OutStiffMatrix[123] = 0;
	OutStiffMatrix[124] = (2 * Emodulus*Iy) / length;
	OutStiffMatrix[125] = 0;
	OutStiffMatrix[126] = (-6 * Emodulus*Iy) / pow(length, 2);
	OutStiffMatrix[127] = 0;
	OutStiffMatrix[128] = 0;
	OutStiffMatrix[129] = 0;
	OutStiffMatrix[130] = (4 * Emodulus*Iy) / length;
	OutStiffMatrix[131] = 0;
	OutStiffMatrix[132] = 0;
	OutStiffMatrix[133] = 0;
	OutStiffMatrix[134] = 0;
	OutStiffMatrix[135] = 0;
	OutStiffMatrix[136] = 0;
	OutStiffMatrix[137] = -((Gmodulus*Ipz) / length);
	OutStiffMatrix[138] = 0;
	OutStiffMatrix[139] = 0;
	OutStiffMatrix[140] = 0;
	OutStiffMatrix[141] = 0;
	OutStiffMatrix[142] = 0;
	OutStiffMatrix[143] = (Gmodulus*Ipz) / length;
}

void NSTanInnerElem::EulerBeamFun::GenStardardStiffMatrixCSR(double & length, double & Emodulus, double & Gmodulus, double & Ix, double & Iy, double & Ipz, double & Area, CSparseMatrixCSR& OutStiffMatrix)
{
	//OutStiffMatrix.CreateCSRDataSpace(12,12,40);
	OutStiffMatrix.DeleteCSRDataSpace();
	OutStiffMatrix.Values = new double[40];
	OutStiffMatrix.RowStartIndex = new int[13]{ 0, 4, 8, 10, 14, 18, 20, 24, 28, 30, 34, 38, 40 };
	OutStiffMatrix.RowEndIndex = OutStiffMatrix.RowStartIndex + 1;
	OutStiffMatrix.RowCount = 12;
	OutStiffMatrix.ColumnIndex = new int[40]{ 0, 4, 6, 10, 1, 3, 7, 9, 2, 8, 1, 3, 7, 9, 0, 4, 6, 10, 5, 11, 0, 4, 6, 10, 1, 3, 7, 9, 2, 8, 1, 3, 7, 9, 0, 4, 6, 10, 5, 11 };
	OutStiffMatrix.ColumnCount = 12;
	double EmodulusMultiIy = Emodulus * Iy;
	double EmodulusMultiIx = Emodulus * Ix;
	double AreaMulitEmodulus = Emodulus *Area;
	double GmodulusMultiIpz = Gmodulus * Ipz;
	double LengthPower3Denominator = 1.0/pow(length,3);
	double LengthPower2Denominator = 1.0 / pow(length, 2);
	double LengthPower1Denominator = 1.0 / length;
	OutStiffMatrix.Values[0] = (12 * EmodulusMultiIy) * LengthPower3Denominator;
	OutStiffMatrix.Values[1] = (6 * EmodulusMultiIy) * LengthPower2Denominator;
	OutStiffMatrix.Values[2] = (-12 * EmodulusMultiIy) * LengthPower3Denominator;
	OutStiffMatrix.Values[3] = (6 * EmodulusMultiIy) * LengthPower2Denominator;
	OutStiffMatrix.Values[4] = (12 * EmodulusMultiIx) * LengthPower3Denominator;
	OutStiffMatrix.Values[5] = (-6 * EmodulusMultiIx) * LengthPower2Denominator;
	OutStiffMatrix.Values[6] = (-12 * EmodulusMultiIx) * LengthPower3Denominator;
	OutStiffMatrix.Values[7] = (-6 * EmodulusMultiIx) * LengthPower2Denominator;
	OutStiffMatrix.Values[8] = (AreaMulitEmodulus)* LengthPower1Denominator;
	OutStiffMatrix.Values[9] = -((AreaMulitEmodulus)* LengthPower1Denominator);
	OutStiffMatrix.Values[10] = (-6 * EmodulusMultiIx) * LengthPower2Denominator;
	OutStiffMatrix.Values[11] = (4 * EmodulusMultiIx) * LengthPower1Denominator;
	OutStiffMatrix.Values[12] = (6 * EmodulusMultiIx) * LengthPower2Denominator;
	OutStiffMatrix.Values[13] = (2 * EmodulusMultiIx) * LengthPower1Denominator;
	OutStiffMatrix.Values[14] = (6 * EmodulusMultiIy) * LengthPower2Denominator;
	OutStiffMatrix.Values[15] = (4 * EmodulusMultiIy) * LengthPower1Denominator;
	OutStiffMatrix.Values[16] = (-6 * EmodulusMultiIy) * LengthPower2Denominator;
	OutStiffMatrix.Values[17] = (2 * EmodulusMultiIy) * LengthPower1Denominator;
	OutStiffMatrix.Values[18] = (GmodulusMultiIpz)* LengthPower1Denominator;
	OutStiffMatrix.Values[19] = -((GmodulusMultiIpz)* LengthPower1Denominator);
	OutStiffMatrix.Values[20] = (-12 * EmodulusMultiIy) * LengthPower3Denominator;
	OutStiffMatrix.Values[21] = (-6 * EmodulusMultiIy) * LengthPower2Denominator;
	OutStiffMatrix.Values[22] = (12 * EmodulusMultiIy) * LengthPower3Denominator;
	OutStiffMatrix.Values[23] = (-6 * EmodulusMultiIy) * LengthPower2Denominator;
	OutStiffMatrix.Values[24] = (-12 * EmodulusMultiIx) * LengthPower3Denominator;
	OutStiffMatrix.Values[25] = (6 * EmodulusMultiIx) * LengthPower2Denominator;
	OutStiffMatrix.Values[26] = (12 * EmodulusMultiIx) * LengthPower3Denominator;
	OutStiffMatrix.Values[27] = (6 * EmodulusMultiIx) * LengthPower2Denominator;
	OutStiffMatrix.Values[28] = -((AreaMulitEmodulus)* LengthPower1Denominator);
	OutStiffMatrix.Values[29] = (AreaMulitEmodulus)* LengthPower1Denominator;
	OutStiffMatrix.Values[30] = (-6 * EmodulusMultiIx) * LengthPower2Denominator;
	OutStiffMatrix.Values[31] = (2 * EmodulusMultiIx) * LengthPower1Denominator;
	OutStiffMatrix.Values[32] = (6 * EmodulusMultiIx) * LengthPower2Denominator;
	OutStiffMatrix.Values[33] = (4 * EmodulusMultiIx) * LengthPower1Denominator;
	OutStiffMatrix.Values[34] = (6 * EmodulusMultiIy) * LengthPower2Denominator;
	OutStiffMatrix.Values[35] = (2 * EmodulusMultiIy) * LengthPower1Denominator;
	OutStiffMatrix.Values[36] = (-6 * EmodulusMultiIy) * LengthPower2Denominator;
	OutStiffMatrix.Values[37] = (4 * EmodulusMultiIy) * LengthPower1Denominator;
	OutStiffMatrix.Values[38] = -((GmodulusMultiIpz)* LengthPower1Denominator);
	OutStiffMatrix.Values[39] = (GmodulusMultiIpz)* LengthPower1Denominator;
	OutStiffMatrix.IsCreatSpace = 1;
}
#pragma region 欧拉梁单元
void NSTanInnerElem::EulerBeamFun::GenStardardStiffMatrixCSRDiscrte(double & length, double & Emodulus, double & Gmodulus, double & Ix, double & Iy, double & Ipz, double & Area, CSparseMatrixDiscreteCSR & OutStiffMatrix)
{
	OutStiffMatrix[0][0] = (12 * Emodulus*Iy) / pow(length, 3);
	OutStiffMatrix[0][4] = (6 * Emodulus*Iy) / pow(length, 2);
	OutStiffMatrix[0][6] = (-12 * Emodulus*Iy) / pow(length, 3);
	OutStiffMatrix[0][10] = (6 * Emodulus*Iy) / pow(length, 2);
	OutStiffMatrix[1][1] = (12 * Emodulus*Ix) / pow(length, 3);
	OutStiffMatrix[1][3] = (-6 * Emodulus*Ix) / pow(length, 2);
	OutStiffMatrix[1][7] = (-12 * Emodulus*Ix) / pow(length, 3);
	OutStiffMatrix[1][9] = (-6 * Emodulus*Ix) / pow(length, 2);
	OutStiffMatrix[2][2] = (Area*Emodulus) / length;
	OutStiffMatrix[2][8] = -((Area*Emodulus) / length);
	OutStiffMatrix[3][1] = (-6 * Emodulus*Ix) / pow(length, 2);
	OutStiffMatrix[3][3] = (4 * Emodulus*Ix) / length;
	OutStiffMatrix[3][7] = (6 * Emodulus*Ix) / pow(length, 2);
	OutStiffMatrix[3][9] = (2 * Emodulus*Ix) / length;
	OutStiffMatrix[4][0] = (6 * Emodulus*Iy) / pow(length, 2);
	OutStiffMatrix[4][4] = (4 * Emodulus*Iy) / length;
	OutStiffMatrix[4][6] = (-6 * Emodulus*Iy) / pow(length, 2);
	OutStiffMatrix[4][10] = (2 * Emodulus*Iy) / length;
	OutStiffMatrix[5][5] = (Gmodulus*Ipz) / length;
	OutStiffMatrix[5][11] = -((Gmodulus*Ipz) / length);
	OutStiffMatrix[6][0] = (-12 * Emodulus*Iy) / pow(length, 3);
	OutStiffMatrix[6][4] = (-6 * Emodulus*Iy) / pow(length, 2);
	OutStiffMatrix[6][6] = (12 * Emodulus*Iy) / pow(length, 3);
	OutStiffMatrix[6][10] = (-6 * Emodulus*Iy) / pow(length, 2);
	OutStiffMatrix[7][1] = (-12 * Emodulus*Ix) / pow(length, 3);
	OutStiffMatrix[7][3] = (6 * Emodulus*Ix) / pow(length, 2);
	OutStiffMatrix[7][7] = (12 * Emodulus*Ix) / pow(length, 3);
	OutStiffMatrix[7][9] = (6 * Emodulus*Ix) / pow(length, 2);
	OutStiffMatrix[8][2] = -((Area*Emodulus) / length);
	OutStiffMatrix[8][8] = (Area*Emodulus) / length;
	OutStiffMatrix[9][1] = (-6 * Emodulus*Ix) / pow(length, 2);
	OutStiffMatrix[9][3] = (2 * Emodulus*Ix) / length;
	OutStiffMatrix[9][7] = (6 * Emodulus*Ix) / pow(length, 2);
	OutStiffMatrix[9][9] = (4 * Emodulus*Ix) / length;
	OutStiffMatrix[10][0] = (6 * Emodulus*Iy) / pow(length, 2);
	OutStiffMatrix[10][4] = (2 * Emodulus*Iy) / length;
	OutStiffMatrix[10][6] = (-6 * Emodulus*Iy) / pow(length, 2);
	OutStiffMatrix[10][10] = (4 * Emodulus*Iy) / length;
	OutStiffMatrix[11][5] = -((Gmodulus*Ipz) / length);
	OutStiffMatrix[11][11] = (Gmodulus*Ipz) / length;
}

void NSTanInnerElem::EulerBeamFun::TransformElemSysMatrixToGlobalSysMatrix(double*& InCoorTransFromGlobalToLocal, CSparseMatrix & InMatrix, CSparseMatrix & OutMatrix)
{
	//CTanVector TempJudgeVector(InUnitElemAxial);
	double const Epsilon = 0.000001;
	InMatrix.ConvertCSRToMKL();
	CSparseMatrix InitialTransMatrix;
	CSparseMatrix TransMatrix;
	OutMatrix.CreateMKLObj();
	InitialTransMatrix.CreateDenseObj();
	InitialTransMatrix.pDENSE_FORMAT->RowCount = 3;
	InitialTransMatrix.pDENSE_FORMAT->ColumnCount = 3;
	InitialTransMatrix.pDENSE_FORMAT->Values = InCoorTransFromGlobalToLocal;
	//InitialTransMatrix.pDENSE_FORMAT->CreateDenseDataSpace(3, 3);
	//InUnitElemAxial.GenCoorTransFromGlobalToLocal(InitialTransMatrix.pDENSE_FORMAT->Values);
	InitialTransMatrix.ConvertDenseToCSR();
	TransMatrix.CreateCSRObj();
	int SubBlockNonZeroElemCount = InitialTransMatrix.pCSR_FORMAT->RowStartIndex[3];
	TransMatrix.pCSR_FORMAT->CreateCSRDataSpace(12, 12, 4 * SubBlockNonZeroElemCount);
	std::copy(InitialTransMatrix.pCSR_FORMAT->ColumnIndex,
		InitialTransMatrix.pCSR_FORMAT->ColumnIndex + SubBlockNonZeroElemCount,
		TransMatrix.pCSR_FORMAT->ColumnIndex);
	std::copy(InitialTransMatrix.pCSR_FORMAT->Values,
		InitialTransMatrix.pCSR_FORMAT->Values + SubBlockNonZeroElemCount,
		TransMatrix.pCSR_FORMAT->Values);
	std::copy(InitialTransMatrix.pCSR_FORMAT->RowEndIndex,
		InitialTransMatrix.pCSR_FORMAT->RowEndIndex + 3,
		TransMatrix.pCSR_FORMAT->RowEndIndex);
	for (int i = 1; i < 4; i++)
	{
		std::copy(InitialTransMatrix.pCSR_FORMAT->Values,
			InitialTransMatrix.pCSR_FORMAT->Values + SubBlockNonZeroElemCount,
			TransMatrix.pCSR_FORMAT->Values + i * SubBlockNonZeroElemCount);
		for (int j = 0; j < 3; j++)
		{
			TransMatrix.pCSR_FORMAT->RowEndIndex[3 * i + j] = InitialTransMatrix.pCSR_FORMAT->RowEndIndex[j] + i * SubBlockNonZeroElemCount;
		}
		for (int j = 0; j < SubBlockNonZeroElemCount; j++)
		{
			TransMatrix.pCSR_FORMAT->ColumnIndex[SubBlockNonZeroElemCount * i + j] = InitialTransMatrix.pCSR_FORMAT->ColumnIndex[j] + i * 3;
		}
		/*TransMatrix.pCSR_FORMAT->RowEndIndex[SubBlockNonZeroElemCount*i]*/
	}
	TransMatrix.ConvertCSRToMKL();
	OutMatrix.pMKL_FORMAT->MultiMatrix(0, *InMatrix.pMKL_FORMAT, *TransMatrix.pMKL_FORMAT);
	OutMatrix.pMKL_FORMAT->MultiMatrix(1, *TransMatrix.pMKL_FORMAT, *OutMatrix.pMKL_FORMAT);
}

void NSTanInnerElem::EulerBeamFun::GenStiffMatrix(std::vector<CNode*>& NodeAddressList, CMaterials *& InMat, CRealConstant *& InRealConst, CSparseMatrix & OutStiffMatrix)
{
	CTanVector ElemAxisVector(3, &NodeAddressList[1]->NodeCoor[0]);
	cblas_daxpy(3,-1.0, &NodeAddressList[0]->NodeCoor[0],1, &ElemAxisVector[0],1);
	double ElemLength = ElemAxisVector.Normalize();
	double & Ix = InRealConst->ParaList[0];
	double & Iy = InRealConst->ParaList[1];
	double & Ipz = InRealConst->ParaList[2];
	double & Area = InRealConst->ParaList[3];	
	double Epsilon = 0.00001;
	InMat->ObtainShearModulus();
	double& EX = InMat->Database["EX"];
	double& GX = InMat->Database["GX"];
	if (pow(ElemAxisVector[0], 2) + pow(ElemAxisVector[1], 2) + pow(ElemAxisVector[2] - 1.0, 2) > pow(Epsilon, 2))
	{
		double* TransCoorFromGlobalToLocal = new double[9]();
		ElemAxisVector.GenCoorTransFromGlobalToLocal(TransCoorFromGlobalToLocal);
		CSparseMatrix ElemStiffMatrix;
		ElemStiffMatrix.CreateCSRObj();
		GenStardardStiffMatrixCSR(ElemLength, EX, GX, Ix, Iy, Ipz, Area, *ElemStiffMatrix.pCSR_FORMAT);
		TransformElemSysMatrixToGlobalSysMatrix(TransCoorFromGlobalToLocal, ElemStiffMatrix, OutStiffMatrix);
		OutStiffMatrix.ConvertMKLToCSR();
		delete [] TransCoorFromGlobalToLocal;
	}
	else
	{
		OutStiffMatrix.CreateCSRObj();
		GenStardardStiffMatrixCSR(ElemLength, EX, GX, Ix, Iy, Ipz, Area, *OutStiffMatrix.pCSR_FORMAT);
	}
}
#pragma endregion 欧拉梁单元