// SparseMatrix.cpp : 此文件包含 "main" 函数。程序执行将在此处开始并结束。
//
//#define M 5
//#define NRHS 2
//#define NNZ 13
//#include "mkl_spblas.h"
#include "NSTanIOStream.h"
#include "CSparseMatrix.h"
#include "CTanVector.h"
#include "CNode.h"
#include "CElem.h"
#include "CElemType.h"
#include "CMaterials.h"
#include "CRealConstant.h"
#include "CBoundaryCondition.h"
#include "CInnerElemTypeList.h"
#include "CFEMSolver.h"
#include "CFEMPreprocessor.h"

//#include "NSTanInnerElemList.h"
//#include "mkl.h"
//#include "CDenseMatrix.h"
//#include "CSparseMatrixMKL.h"
//#include <forward_list>
//#include <list>
//void testf(std::pair<int,double>&& t)
//{
//	std::cout << t.first << std::endl;
//}

void testf(std::map<int, double>::reference t)
{
	std::cout << t.second << std::endl;
}
void testf3(std::vector<double>::reference t)
{
	std::cout << t << std::endl;
}
void testf2(std::map<int, double>::reference& t)
{
	t.second = t.first;
}
//void testf2(std::pair<int, double>&& t)
//{
//	t.second = 3;
//}
int main()
{


//	CSparseMatrix TestSparseSolve;
//	TestSparseSolve.CreateDenseObj();
//	double a1[5][5]{
//		{1.0,	-1.0,	0,	-3.0,   0},
//	{-2.0,	5.0,	0,		0,		0},
//	{0,		0,		4.0,	6.0,	4.0},
//	{-4.0,	0,		2.0,	7.0,    0},
//	{0,		8.0,    0,      0,		-5.0}
//	};
//	//TestSparseSolve.pDENSE_FORMAT->CreateDenseDataSpace(5, 5, (double*)a);
//	CDenseMatrix TestDenseMatrix(5, 5, (double*)a1);
//	TestSparseSolve.ConvertDenseToCSR(TestDenseMatrix);
//	TestSparseSolve.ConvertCSRToDense();
//	TestSparseSolve.ConvertCSRToMKL();
//	CSparseMatrix TestSparseSolve2;
//	TestSparseSolve2.ConvertMKLToCSR(*TestSparseSolve.pMKL_FORMAT);
//	TestSparseSolve2.ConvertCSRToDense();
//	double x[5];
//	double* b = new double[5]{ 1,1,1,1,1 };
//
//
//TestSparseSolve.pCSR_FORMAT->SolveLinearEquByParallelDSS(*TestSparseSolve.pCSR_FORMAT, b, x);
	//NSTanIOStream::PrintCSRSparseMatrixInConsole(*TestSparseSolve2.pCSR_FORMAT);
	//NSTanIOStream::PrintMatrixInConsole(*TestSparseSolve2.pDENSE_FORMAT);
	//CFEMSolver TestFemBasic;
	//std::map<int, CNode*> testmap;
	//CFEMPreprocessor TestPrep;
	//TestPrep.InsertNode(1, 0, 0, 0);
	//TestPrep.InsertNode(2, 0.4, 0, 0);
	//TestPrep.InsertElemType(1, 0);
	//TestPrep.InsertRealConst(4, 7, new double[7]{ 0.4, 0.3, 0.5, 1.0 / 2.0, sqrt(3.0) / 2.0, 0, 5 * pow(10,4) });
	//TestPrep.InsertRealConst(5, 4, new double[4]{ 4.908738521234052*pow(10,-10), 4.908738521234052*pow(10,-10), 9.817477042468103*pow(10,-10), 0.00007853981633974483 });
	//TestPrep.InsertElem(1, 1, 2, 4, 2, new int[2]{ 1,2 });
	//TestPrep.InsertMat(1, 2.1*pow(10, 11), 0.3);
	//for (int i = 1; i < 6 + 1; i++)
	//{
	//	TestPrep.InsertBoundaryCondition(1, i, 0);
	//	TestPrep.InsertBoundaryCondition(2, i, 0);
	//}
	//TestPrep.InsertBoundaryCondition(1, 1, 1);
	//TestPrep.InsertBoundaryCondition(2, 1004, 100);





	//TestPrep.DeleteNode(0);



	//testmap[1]= new CNode(1, 0, 0, 0);
	//auto ttt= new CNode(1, 1, 2, 3);
	//CFEMPreprocessor TestPrep;
	//TestPrep.InsertNode(1, 0, 1, 0);
	//CNode* ttt1= TestPrep.pPREP_PARA->NodeList[1];
	//TestPrep.InsertNode(1, 0, 2, 0);
	////TestPrep.DeleteDataSpace();
	////std::map<int, CElem*> TestElemList;
	//int* TestNodeNum1 = new int[2]{ 1,2 };
	//TestPrep.InsertElem(1, 1, 2, 4, 2, TestNodeNum1);
	//int* TestNodeNum2 = new int[2]{ 2,3 };
	//TestPrep.InsertElem(1, 1, 2, 4, 2, TestNodeNum2);
	//int tx = 1;
	//TestPrep.DeleteNode(tx);

	//delete ttt; 
	//TanNodeListType TestNodeList;
	//TestFemBasic.pPREP_PARA->NodeList[1] = new CNode(1, 0, 0, 0);
	//TestFemBasic.pPREP_PARA->NodeList[2] = new CNode(2, 1, 0, 0);
	////TanElemTypeListType TestElemTypeList;
	//TestFemBasic.pPREP_PARA->ElemTypeList[1] = new CElemType(1, 0);

	////TanRealConsListType TestRealConstList;
	//double* TestRealConstV = new double[7]{ 0.4, 0.3, 0.5, 1.0 / 2.0, sqrt(3.0) / 2.0, 0, 5 * pow(10,4) };
	//TestFemBasic.pPREP_PARA->RealConsList[4] = new CRealConstant(4, 7, TestRealConstV);
	////TanMatListType TestMat;
	//TestFemBasic.pPREP_PARA->MatList[1] = new CMaterials(1, 2.1*pow(10, 11), 0.3);
	////std::map<int, CElem*> TestElemList;
	//int* TestNodeNum = new int[2]{ 1,2 };
	//TestFemBasic.pPREP_PARA->ElemList[1] = new CElem(1, 1, 2, 4, 2, TestNodeNum);

	////CBoundaryCondition TestBoundary;
	////int i = 0;
	//for (int i = 1; i < 6 + 1; i++)
	//{
	//	TestFemBasic.pPREP_PARA->BCCondition.PushBoundaryValue(1, i, 0);
	//	TestFemBasic.pPREP_PARA->BCCondition.PushBoundaryValue(2, i, 0);
	//}
	//TestFemBasic.pPREP_PARA->BCCondition.PushBoundaryValue(1, 1, 1);
	////TestFemBasic.pPREP_PARA->BCCondition.EraseBoundaryValue(1, 4);
	//TestFemBasic.pPREP_PARA->BCCondition.PushBoundaryValue(2, 1004, 100);
	////给所有单元赋予参数
	//for (auto iter = TestFemBasic.pPREP_PARA->ElemList.begin(); iter != TestFemBasic.pPREP_PARA->ElemList.end(); iter++)
	//{
	//	(*iter->second).AssignParaIndex(TestFemBasic.pPREP_PARA->NodeList, TestFemBasic.pPREP_PARA->ElemTypeList, TestFemBasic.pPREP_PARA->MatList, TestFemBasic.pPREP_PARA->RealConsList, TestFemBasic.InnerElemTypeList);
	//}

	////std::map<int, double> testxxx1;
	////testxxx1[0] = 0.5;
	////testxxx1[2] = 1.5;

	//////auto ttt = testxxx1.begin();
	//////ttt->first;
	////std::for_each(testxxx1.begin(), testxxx1.end(), testf2);
	////std::for_each(testxxx1.begin(), testxxx1.end(), testf);
	////std::vector <double > testxxx1{ 1,0,3 };
	////std::for_each(testxxx1.begin(), testxxx1.end(), [&](std::vector<double>::reference& t) {t = &t - &testxxx1[0]; });
	////std::for_each(testxxx1.begin(), testxxx1.end(), testf3);
	//CSparseMatrixDiscreteCSR TestDiscreteMatrix;
	////TestMeshElem1.GenStiffMatrix(TestElemList[1]->NodeAddressList, 0, TestElemList[1]->RealConsNum.second, TestDiscreteMatrix);
	////int* ttttt = new int { 0 };

	////TT1->GenStiffMatrix(TestElemList[1]->NodeAddressList, 0, TestElemList[1]->RealConsNum.second, TestDiscreteMatrix);
	//CInnerElemTypeList TestInnerElemList;
	//TestFemBasic.pSOLVE_PARA->StiffMatrix.CreateDiscreteCSR();
	//(*TestInnerElemList[0].pGenStiffMatrixCSRDiscrte)(
	//	TestFemBasic.pPREP_PARA->ElemList[1]->NodeAddressList,
	//	TestFemBasic.pPREP_PARA->ElemList[1]->MatNum.second,
	//	TestFemBasic.pPREP_PARA->ElemList[1]->RealConsNum.second,
	//	*TestFemBasic.pSOLVE_PARA->StiffMatrix.pCSR_DISCRETE_FORMAT
	//	/*TestDiscreteMatrix*/);


	// 
	// 
	// 之前的测试函数
	// 
	//CFEMSolver TestFemBasic;

	//CFEMPreprocessor TestPrep;

	//TestPrep.InsertNode(100, 0, 0, 0);
	//TestPrep.InsertNode(101, 1, 0, 0);
	//TestPrep.InsertNode(1000, -10, 0, 0);
	//TestPrep.InsertNode(1001, 13, 5, 0);
	//TestPrep.InsertElemType(1, 1);//欧拉梁单元
	//TestPrep.InsertElemType(200, 0);//啮合单元
	//TestPrep.InsertRealConst(1, 4, new double[4]{ 4.908738521234052*pow(10,-10), 4.908738521234052*pow(10,-10), 9.817477042468103*pow(10,-10), 0.00007853981633974483 });//欧拉梁单元实常数
	//TestPrep.InsertRealConst(200, 7, new double[7]{ 0.4, 0.3, 0.5, 1.0 / 2.0, sqrt(3.0) / 2.0, 0, 5 * pow(10,4) });//啮合单元实常数
	//TestPrep.InsertMat(1, (std::string)"EX",2.1*pow(10, 11));
	//TestPrep.InsertMat(1, (std::string)"PRXY", 0.3);
	//TestPrep.InsertElem(2001, 1, 1, 1, 2, new int[2]{ 1000,100 });
	//TestPrep.InsertElem(2002, 1, 1, 1, 2, new int[2]{ 1001,101});
	//int* testdat = new int[2]{ 100,101 };
	//TestPrep.InsertElem(500000, 200, 1, 200, 2, testdat);
	////std::cout << &(TestPrep.ElemList[500000]->NodeNumList[0]);
	//for (int i = 1; i < 6 + 1; i++)
	//{
	//	TestPrep.InsertBoundaryCondition(1000, i, 0);
	//	TestPrep.InsertBoundaryCondition(1001, i, 0);
	//}
	//TestPrep.InsertBoundaryCondition(1000, 1, 1);
	//TestPrep.InsertBoundaryCondition(1001, 1006, 100);

	//TestFemBasic.SolveFEMModel(0,TestPrep);
	//for (int i = 1; i < 6 + 1; i++)
	//{
	//	std::cout << i<<"\t" <<TestFemBasic.TotalFValue[TestPrep.NodeList[1000]->FreedomList[i].ToFullMatrixIndex] << std::endl;
	//}
	//for (int i = 1; i < 6 + 1; i++)
	//{
	//	std::cout << i << "\t" << TestFemBasic.TotalFValue[TestPrep.NodeList[1001]->FreedomList[i].ToFullMatrixIndex] << std::endl;
	//}

	//for (auto i = TestPrep.NodeList.begin(); i != TestPrep.NodeList.end(); i++)
	//{
	//	std::cout << i->first << "\t" << TestFemBasic.TotalUValue[i->second->FreedomList[1].ToFullMatrixIndex] << std::endl;
	//}



	//RV计算测试

	CFEMSolver TestFemBasic;

	CFEMPreprocessor TestPrep;

	TestPrep.InsertNode(100, 0, 0, 0);
	TestPrep.InsertNode(1000, 0, 0, 1);
	//TestPrep.InsertNode(1000, -10, 0, 0);
	//TestPrep.InsertNode(1001, 13, 5, 0);
	TestPrep.InsertElemType(1, 1);//欧拉梁单元
	//TestPrep.InsertElemType(200, 0);//啮合单元
	TestPrep.InsertRealConst(1, 4, new double[4] { 4.908738521234052 * pow(10, -10), 4.908738521234052 * pow(10, -10), 9.817477042468103 * pow(10, -10), 0.00007853981633974483 });//欧拉梁单元实常数
	//TestPrep.InsertRealConst(200, 7, new double[7] { 0.4, 0.3, 0.5, 1.0 / 2.0, sqrt(3.0) / 2.0, 0, 5 * pow(10, 4) });//啮合单元实常数
	TestPrep.InsertMat(1, (std::string)"EX", 2.1 * pow(10, 11));
	TestPrep.InsertMat(1, (std::string)"PRXY", 0.3);
	//TestPrep.InsertElem(2001, 1, 1, 1, 2, new int[2] { 1000, 100 });
	//TestPrep.InsertElem(2002, 1, 1, 1, 2, new int[2] { 1001, 101});
	//int* testdat = new int[2] { 100, 101 };
	TestPrep.InsertElem(2001, 1, 1, 1, 2, new int[2] { 1000, 100 });
	//TestPrep.InsertElem(500000, 200, 1, 200, 2, testdat);
	//std::cout << &(TestPrep.ElemList[500000]->NodeNumList[0]);
	for (int i = 1; i < 6 + 1; i++)
	{
		TestPrep.InsertBoundaryCondition(100, i, 0);
		//TestPrep.InsertBoundaryCondition(1001, i, 0);
	}
	//TestPrep.InsertBoundaryCondition(1000, 1, 1);
	TestPrep.InsertBoundaryCondition(1000, 1006, 100);

	TestFemBasic.SolveFEMModel(0, TestPrep);
	for (int i = 1; i < 6 + 1; i++)
	{
		std::cout << i << "\t" << TestFemBasic.TotalFValue[TestPrep.NodeList[100]->FreedomList[i].ToFullMatrixIndex] << std::endl;
	}
	for (int i = 1; i < 6 + 1; i++)
	{
		std::cout << i << "\t" << TestFemBasic.TotalFValue[TestPrep.NodeList[1000]->FreedomList[i].ToFullMatrixIndex] << std::endl;
	}
	for (int i = 1; i < 6 + 1; i++)
	{
		std::cout << i << "\t" << TestFemBasic.TotalUValue[TestPrep.NodeList[1000]->FreedomList[i].ToFullMatrixIndex] << std::endl;
	}

	//for (auto i = TestPrep.NodeList.begin(); i != TestPrep.NodeList.end(); i++)
	//{
	//	std::cout << i->first << "\t" << TestFemBasic.TotalUValue[i->second->FreedomList[1].ToFullMatrixIndex] << std::endl;
	//}


	int t = 0;





//	//CSparseMatrix EulerBeamMatrix;
//	//TestPrep.ConstructPrepDataMap();
//	//NSTanInnerElem::EulerBeamFun::GenStiffMatrix(TestPrep.ElemList[1]->NodeAddressList,
//	//	TestPrep.MatList[1],
//	//	TestPrep.RealConsList[5],
//	//	EulerBeamMatrix);
//	//EulerBeamMatrix.ConvertCSRToDense();
//	//NSTanIOStream::PrintMatrixInConsole(*EulerBeamMatrix.pDENSE_FORMAT);
//	//	//StaticAnalysis(TestPrep);
//
//	//TestPrep.ConstructPrepDataMap();
//	////TestFemBasic.MapNodeElemAndFreedom(TestPrep.NodeList, TestPrep.ElemList, TestPrep.ElemTypeList);
//	//TestFemBasic.BuildNodeToGlobalMap(TestPrep);
//	//TestFemBasic.CreatMatrixSpace(TestFemBasic.TotalFreedomValue.size());
//	//TestFemBasic.AssembleElemListStiff(TestPrep.ElemList, TestFemBasic.StiffMatrix);
//	//CSparseMatrix TestSolveMatrix;
//	//TanDoubleVector TestSolveEquRight;
//	//TestSolveMatrix.CreateCSRObj();
//	//TestFemBasic.GenStaticSolveEquCSR(*TestSolveMatrix.pCSR_FORMAT, TestSolveEquRight);
//	//TanDoubleVector TestResult(TestSolveMatrix.pCSR_FORMAT->RowCount,0);
//	//TestSolveMatrix.ConvertCSRToDense();
//	//NSTanIOStream::PrintMatrixInConsole(*TestSolveMatrix.pDENSE_FORMAT);
//	//TestSolveMatrix.pCSR_FORMAT->SolveLinearEquByParallelDSS(&TestSolveEquRight[0], &TestResult[0]);
//	//TestSolveMatrix.ConvertCSRToCSRDiscrete();
//
//
//
//
//	//std::get<0>(CElemType::InnerElemTypeList).GenStiffMatrix(TestElemList[1]->NodeAddressList, 0, TestElemList[1]->RealConsNum.second, TestDiscreteMatrix);
//	double* DTestv1 = new double[4]{ 1,1,0,0 };
//	double* DTestv2 = new double[4]{ 1.0,1.0,1,0 };
//	CTanVector Testv1(4, DTestv1);
//	CTanVector Testv2(4, DTestv2);
//	CTanVector Testv3;
//	CTanVector Testv4;
//	Testv1.GenOrthogonalVector(Testv1, Testv3);
//	Testv4.CrossProduct(Testv1, Testv2, Testv3);
//	CTanVector Testv5 = Testv1.CrossProduct(Testv2);
//	Testv2.Normalize(Testv2);
//	std::vector<CTanVector> T1(3);
//	Testv1.GenLocalCoor(Testv1, T1[0], T1[1], T1[2]);
//	//double* TestMatrix = new double[9]{ 0 };
//	//std::vector<double> TestMatrix(9);
//	double* TestMatrix = new double[9]{ 0 };
//	Testv1.GenCoorTransFromLocalToGlobal(TestMatrix);
//	std::cout << Testv1.ObtainNorm2();
//
//
//
//
//	double TestMatrixAdd1[5][5]{ { 1,-1,0,-3,0},
//								{-2,5,0,0,0},
//								{0,0,4,6,4},
//								{-4,0,2,7,0},
//								{0,8,0,0,-5} };
//	CSparseMatrix ADDMatrix1;
//	ADDMatrix1.CreateDenseObj();
//	ADDMatrix1.pDENSE_FORMAT->CreateDenseDataSpace(5, 5, (double*)TestMatrixAdd1);
//	ADDMatrix1.ConvertDenseToCSR();
//	ADDMatrix1.ConvertCSRToCSRDiscrete();
//	ADDMatrix1.ConvertCSRDiscreteToCSR();
//	ADDMatrix1.ConvertCSRToMKL();
//	ADDMatrix1.ConvertMKLToCSR();
//	//ADDMatrix1.DeleteCSRObj();
//	//ADDMatrix1.ConvertMKLToCSR();
//	//子矩阵测试
//	CSparseMatrix SubMatrix;
//	TanIntVector SubMatrixIndex{ 1,3,2 };
//	SubMatrix.CreateAllObj();
//	SubMatrix.ObtainSubMatrixFromCSR(SubMatrixIndex, SubMatrixIndex, *ADDMatrix1.pCSR_DISCRETE_FORMAT, *SubMatrix.pCSR_FORMAT);
//	SubMatrix.ConvertCSRToDense();
//	//std::cout << "提取子矩阵测试" << std::endl;
//	//NSTanIOStream::PrintMatrixInConsole(*ADDMatrix1.pDENSE_FORMAT);
//	std::cout << "提取子矩阵测试" << std::endl;
//	NSTanIOStream::PrintMatrixInConsole(*SubMatrix.pDENSE_FORMAT);
//	std::cout << "测试稀疏矩阵" << std::endl;
//	TanDoubleVector testXVector{0.1,5,7,3,2};
//	TanDoubleVector testYVector(5, 0);
//	ADDMatrix1.pMKL_FORMAT->MultiVector(0, 5, *ADDMatrix1.pMKL_FORMAT, &testXVector[0], &testXVector[0]);
//	//std::vector<std::map<int, double>::iterator> test1;
//	//TanIntVector TestColumIndex{ 1,0,2 };
//	//ADDMatrix1.pCSR_DISCRETE_FORMAT->ObtainIndexAddressInSingleRow(0, TestColumIndex, *ADDMatrix1.pCSR_DISCRETE_FORMAT, test1);
//	//NSTanIOStream::PrintVectorInConsole(test1);
////	for (int i=0; i< test1.size();i++)
////{
////	if (test1[i]!= (*ADDMatrix1.pCSR_DISCRETE_FORMAT)[0].end())
////	{
////		std::cout << (test1[i])->second << std::endl;
////	}
////	else
////	{
////		std::cout << 0<< std::endl;
////	}
////}
//	//NSTanIOStream::PrintCSRSparseMatrixInConsole(*ADDMatrix1.pCSR_FORMAT);
//	//TanDoubleVector EachRowElemCount(5, 0);
//
//	//std::map<int, double> &TestMap = (*ADDMatrix1.pCSR_DISCRETE_FORMAT)[0];
//	//auto tx = TestMap.begin();
//	////std
//	////int i = 0;
//	////std::generate(EachRowElemCount.begin(), EachRowElemCount.end(), [&TestMatrixAdd1,&i] { return TestMatrixAdd1[0][i++]; });
//	////double x = std::accumulate(EachRowElemCount.begin(), EachRowElemCount.begin()+4, 0);
//	//std::for_each(TestMap.begin(), TestMap.end(), [](std::pair<int, double>&& a) {std::cout << a.first << std::endl; });
//	//CSparseMatrixDiscreteCSR* Test1=new CSparseMatrixDiscreteCSR(4);
//	//Test1->CheckDiscreteCSRDataSpace(*Test1);
//	//TanIntVector TestColumIndex{1,0,3};
//	//TanDoubleVector TestColumValue;
//	//TanDoublePointerVector TestColumAddress;
//	////ADDMatrix1.pCSR_FORMAT->ObtainIndexValueInSingleRow(0, TestColumIndex, *ADDMatrix1.pCSR_FORMAT, TestColumValue);
//	////ADDMatrix1.pCSR_FORMAT->ObtainIndexAddressInSingleRow(0, TestColumIndex, *ADDMatrix1.pCSR_FORMAT, TestColumAddress);
//	//ADDMatrix1.pCSR_DISCRETE_FORMAT->ObtainIndexValueInSingleRow(0, 0, *ADDMatrix1.pCSR_DISCRETE_FORMAT, TestColumValue);
//	//ADDMatrix1.pCSR_DISCRETE_FORMAT->ObtainIndexAddressInSingleRow(0, 0, *ADDMatrix1.pCSR_DISCRETE_FORMAT, TestColumAddress);
//	//std::cout << "子矩阵" << std::endl;
//	//NSTanIOStream::PrintVectorInConsole(TestColumValue);
//	//std::cout << "子矩阵地址" << std::endl;
//	//NSTanIOStream::PrintVectorInConsole(TestColumAddress);
//	//for (int i=0; i< TestColumAddress.size();i++)
//	//{
//	//	if (TestColumAddress[i])
//	//	{
//	//		std::cout << *TestColumAddress[i] << std::endl;
//	//	}
//	//	else
//	//	{
//	//		std::cout << 0<< std::endl;
//	//	}
//	//}
//	//
//	//ADDMatrix1.ConvertCSRToMKL();
//	//mkl_sparse_d_set_value(*ADDMatrix1.pMKL_FORMAT->MKLSparseHandle, 0, 2, 5);
//	//ADDMatrix1.ConvertCSRToDense();
//	//int* ttt = &TestColumIndex[0];
//	//std::cout << ttt[0] << std::endl;
//	////std::cout << *(TestColumAddress[1]) << std::endl;
//	//NSTanIOStream::PrintMatrixInConsole(*ADDMatrix1.pDENSE_FORMAT);
//
//	//std::cout << "加法测试" << std::endl;
//
//	
//	double TestMatrixAdd2[5][5]{ { -5,-7,2,1,4},
//						{1,5,0,0,0},
//						{2,0,4,6,3},
//						{7,8,3,1,0},
//						{1,-8,0,0,7} };
//	CSparseMatrix ADDMatrix2;
//	ADDMatrix2.CreateDenseObj();
//	ADDMatrix2.pDENSE_FORMAT->CreateDenseDataSpace(5, 5, (double*)TestMatrixAdd2);
//	ADDMatrix2.ConvertDenseToCSR();
//	ADDMatrix2.ConvertCSRToMKL();
//	CSparseMatrix ADDMatrix4;
//	std::cout << "加法测试" << std::endl;
//	ADDMatrix4.AddMatrixByMKL(ADDMatrix1, ADDMatrix2);
//	ADDMatrix4.ConvertMKLToCSR();
//	ADDMatrix4.ConvertCSRToDense();
//	CSparseMatrix CopyMatrix;
//	CopyMatrix.CreateCSRObj();
//	CopyMatrix.pCSR_FORMAT->CopyCSR(*ADDMatrix4.pCSR_FORMAT, *CopyMatrix.pCSR_FORMAT);
//	CopyMatrix.ConvertCSRToDense();
//	NSTanIOStream::PrintMatrixInConsole(*CopyMatrix.pDENSE_FORMAT);
//	NSTanIOStream::PrintMatrixInConsole(*ADDMatrix4.pDENSE_FORMAT);
//	//ADDMatrix4.DeleteMKLObj();
//	//ADDMatrix4.pCSR_FORMAT->IsCreatSpace=1;
//	//ADDMatrix4.DeleteCSRObj();
//		TanIntVector AddMatrix1RowIndex{ 1,3,2,4,0 };
//	TanIntVector AddMatrix2RowIndex{ 4,1,0,2,3 };
//	TanIntVector AddMatrix3RowIndex{ 4,2,1,0,3 };
//	//	CSparseMatrix::AddSubMatrixByCSRDiscrete(
//	//	AddMatrix1RowIndex, AddMatrix1RowIndex, *ADDMatrix1.pCSR_FORMAT,
//	//	AddMatrix2RowIndex, AddMatrix2RowIndex, *ADDMatrix2.pCSR_FORMAT,
//	//	AddMatrix3RowIndex, AddMatrix3RowIndex, *ADDMatrix1.pCSR_DISCRETE_FORMAT
//	//);
//		CSparseMatrix::AddSubMatrixByCSRDiscrete(
//			&AddMatrix1RowIndex, 0, *ADDMatrix1.pCSR_FORMAT,
//			&AddMatrix2RowIndex, 0, *ADDMatrix2.pCSR_FORMAT,
//			0, 0, *ADDMatrix1.pCSR_DISCRETE_FORMAT
//		);
//		std::cout << "子矩阵测试" << std::endl;
//		ADDMatrix1.ConvertCSRDiscreteToCSR();
//		ADDMatrix1.ConvertCSRToDense();
//		ADDMatrix1.ConvertCSRToMKL();
//		std::cout << "子矩阵测试" << std::endl;
//		NSTanIOStream::PrintMatrixInConsole(*ADDMatrix1.pDENSE_FORMAT);
////	//TanDoublePointerVector AMatrixAdress;
////	//ADDMatrix1.pCSR_FORMAT->ObtainIndexAddressInSingleRow(0, 0, AMatrixAdress);
////	TanIntVector Matrix1;
////	TanIntVector AddMatrix1RowIndex{ 1,3,2,4,0 };
////	TanIntVector AddMatrix2RowIndex{ 4,1,0,2,3 };
////	TanIntVector AddMatrix3RowIndex{ 4,2,1,0,3 };
////	TanDoubleVector TestDerive;
////	ADDMatrix2.ConvertCSRToDense();
////	NSTanIOStream::PrintMatrixInConsole(*ADDMatrix1.pDENSE_FORMAT);
////	ADDMatrix2.pCSR_FORMAT->ObtainIndexValueInSingleRow(AddMatrix2RowIndex[0], &AddMatrix2RowIndex, TestDerive);
////	ADDMatrix2.ConvertCSRToMKL();
////	//std::vector<std::map<int, double>::iterator> test1;
//////TanIntVector TestColumIndex{ 1,0,2 };
//////ADDMatrix1.pCSR_DISCRETE_FORMAT->ObtainIndexAddressInSingleRow(0, TestColumIndex, *ADDMatrix1.pCSR_DISCRETE_FORMAT, test1);
////
////	CSparseMatrix::AddSubMatrixByCSRDiscrete(
////		AddMatrix1RowIndex, &AddMatrix1RowIndex, *ADDMatrix1.pCSR_FORMAT,
////		AddMatrix2RowIndex, &AddMatrix2RowIndex, *ADDMatrix2.pCSR_FORMAT,
////		AddMatrix3RowIndex, &AddMatrix3RowIndex, *ADDMatrix1.pCSR_DISCRETE_FORMAT
////	);
////	ADDMatrix1.ConvertCSRDiscreteToCSR();
////	ADDMatrix1.ConvertCSRToDense();
////	std::cout << "子矩阵测试" << std::endl;
////	NSTanIOStream::PrintMatrixInConsole(*ADDMatrix1.pDENSE_FORMAT);
////	//NSTanIOStream::PrintVectorInConsole((*ADDMatrix1.pCSR_DISCRETE_FORMAT)[0]);
////	CSparseMatrix ADDMatrixResult;
////	ADDMatrixResult.AddMatrixByMKL(ADDMatrix1, ADDMatrix2);
////	ADDMatrixResult.AddMatrixByMKL(ADDMatrix1);
////	ADDMatrixResult.ConvertMKLToCSR();
////	ADDMatrixResult.ConvertMKLToCSRByCopy();
////	ADDMatrixResult.ConvertCSRToDense();
////	//NSTanIOStream::PrintCSRSparseMatrixInConsole(*ADDMatrixResult.pCSR_FORMAT);
////	//NSTanIOStream::PrintMatrixInConsole(*ADDMatrixResult.pDENSE_FORMAT);	
//	printf("%s\n", "矩阵相乘");
//	CSparseMatrix MultiMatrixResult;
//	MultiMatrixResult.MultiMatrixByMKL(ADDMatrix1, ADDMatrix2);
//	MultiMatrixResult.ConvertMKLToCSR();
//	MultiMatrixResult.ConvertCSRToDense();
//	NSTanIOStream::PrintMatrixInConsole(*MultiMatrixResult.pDENSE_FORMAT);
//
//	printf("%s\n", "矩阵相乘2");
//	MultiMatrixResult.MultiMatrixByMKL(ADDMatrix2, ADDMatrix1);
//	MultiMatrixResult.ConvertMKLToCSR();
//	MultiMatrixResult.ConvertCSRToDense();	
//	NSTanIOStream::PrintMatrixInConsole(*MultiMatrixResult.pDENSE_FORMAT);
//	printf("%s\n", "矩阵相乘3");
//	ADDMatrix2.ConvertCSRToMKL();
//	MultiMatrixResult.MultiMatrixByMKL(ADDMatrix2);
//	//MultiMatrixResult.pMKL_FORMAT->MultiMatrix(1,*ADDMatrix2.pMKL_FORMAT);
//
//	MultiMatrixResult.ConvertMKLToCSR();	
//	CSparseMatrixCSR* TestCSR=0;
//	//NSTanIOStream::PrintCSRSparseMatrixInConsole(*MultiMatrixResult.pCSR_FORMAT);
//	
//	MultiMatrixResult.ConvertMKLToCSR();
//	MultiMatrixResult.pCSR_FORMAT->Values[6] = 5;
//	MultiMatrixResult.ConvertMKLToCSR(*MultiMatrixResult.pMKL_FORMAT, TestCSR);
//	MultiMatrixResult.ConvertCSRToDense(*TestCSR);
//	//NSTanIOStream::PrintMatrixInConsole(*MultiMatrixResult.pDENSE_FORMAT);
//
//	
//	int t = 0;
//	t = 1;
}

// 运行程序: Ctrl + F5 或调试 >“开始执行(不调试)”菜单
// 调试程序: F5 或调试 >“开始调试”菜单

// 入门使用技巧: 
//   1. 使用解决方案资源管理器窗口添加/管理文件
//   2. 使用团队资源管理器窗口连接到源代码管理
//   3. 使用输出窗口查看生成输出和其他消息
//   4. 使用错误列表窗口查看错误
//   5. 转到“项目”>“添加新项”以创建新的代码文件，或转到“项目”>“添加现有项”以将现有代码文件添加到项目
//   6. 将来，若要再次打开此项目，请转到“文件”>“打开”>“项目”并选择 .sln 文件
