// SparseMatrix.cpp : 此文件包含 "main" 函数。程序执行将在此处开始并结束。
//
#define M 5
#define NRHS 2
#define NNZ 13
#include "mkl_spblas.h"
#include "NSTanIOStream.h"
#include "CSparseMatrix.h"
//#include <forward_list>
//#include <list>
int main()
{
	double* TestMatrix2 = new double[25]{ 1,-1,0,-3,0,
										-2,5,0,0,0,
										0,0,4,6,4,
										-4,0,2,7,0,
										0,8,0,0,-5 };

	NSTanIOStream::PrintMatrixInConsole(5, 5, (double*)TestMatrix2);







	//矩阵转换

	//std::list<int> fl;

	//for (int i=0;i<5;i++)
	//{
	//	fl.push_back(i);
	//}
	//int f2[5]{ 0 };
	//auto start = fl.begin();
	//auto end = fl.end();
	//auto  start = fl.begin();
	//auto  end = fl.end();
	//std::copy(fl.begin(), fl.end(), f2);

	//for (int i = 0; i < 5; i++)
	//{
	//	std::cout << f2[i];
	//}

	//for (const auto &elem : fl)
	//{
	//	std::cout << elem;
	//}
	CSparseMatrix ATestSparse;
	ATestSparse.ConvertDenseToCSR3(5, 5, TestMatrix2);

	for (int i = 0; i < 13; i++)
	{
		std::cout << ATestSparse.pCSR3_FORMAT->Values[i] << "\t";
	}
	std::cout << "\n";
	for (int i = 0; i < 13; i++)
	{
		std::cout << ATestSparse.pCSR3_FORMAT->ColumnIndex[i] << "\t";
	}

	std::cout << "\n";
	for (int i = 0; i < 6; i++)
	{
		std::cout << ATestSparse.pCSR3_FORMAT->RowIndex[i]<<"\t";
	}
    std::cout << "Hello World!\n";


	CSparseMatrix::tagSparseMatrixMKL ATestMKLSparse;

	ATestSparse.ConvertCSR3ToMKL(*ATestSparse.pCSR3_FORMAT,ATestMKLSparse);

/*
	ATestSparse.DeleteMKLDataSpace(ATestMKLSparse);*/
	CSparseMatrix ATestSparse2;
	ATestSparse2.ConvertCSR3ToMKL(*ATestSparse.pCSR3_FORMAT);
	ATestSparse2.CreatCSR3DataSpace();
	ATestSparse2.ConvertMKLToCSR3(ATestMKLSparse, *ATestSparse2.pCSR3_FORMAT);
	//ATestSparse2.DeleteCSR3DataSpace(ATestSparse2.CSR3_FORMAT);


	std::cout << ATestSparse2.pCSR3_FORMAT->RowCount << std::endl;
	std::cout << ATestSparse2.pCSR3_FORMAT->ColumnCount << std::endl;
	for (int i = 0; i < 13; i++)
	{
		std::cout << ATestSparse2.pCSR3_FORMAT->Values[i] << "\t";
	}
	std::cout << "\n";
	for (int i = 0; i < 13; i++)
	{
		std::cout << ATestSparse2.pCSR3_FORMAT->ColumnIndex[i] << "\t";
	}

	std::cout << "\n";
	for (int i = 0; i < 6; i++)
	{
		std::cout << ATestSparse2.pCSR3_FORMAT->RowIndex[i] << "\t";
	}
	std::cout << "Hello World!\n";
	ATestSparse2.ConvertMKLToCSR3ByCopy();
	ATestSparse2.ConvertCSR3ToMKL();
	ATestSparse2.DeleteMKLDataSpace(*ATestSparse2.pMKL_FORMAT);
	CSparseMatrix::tagSparseMatrixCSR3 ATestCSR3Sparse;

	ATestSparse2.ConvertCSR3ToMKL(ATestCSR3Sparse);
	ATestSparse2.ConvertCSR3ToMKL(*ATestSparse.pCSR3_FORMAT);
	std::cout << "加法测试" << std::endl;
	double TestMatrixAdd1[5][5]{ { 1,-1,0,-3,0},
								{-2,5,0,0,0},
								{0,0,4,6,4},
								{-4,0,2,7,0},
								{0,8,0,0,-5} };
	CSparseMatrix ADDMatrix1;
	ADDMatrix1.ConvertDenseToCSR3(5, 5, (double*)TestMatrixAdd1);
	ADDMatrix1.ConvertCSR3ToMKL();
	std::cout << "ADDMatrix1" << std::endl;
	for (int i = 0; i < 13; i++)
	{
		std::cout << ADDMatrix1.pCSR3_FORMAT->Values[i] << "\t";
	}
	std::cout << "\n";
	for (int i = 0; i < 13; i++)
	{
		std::cout << ADDMatrix1.pCSR3_FORMAT->ColumnIndex[i] << "\t";
	}

	std::cout << "\n";
	for (int i = 0; i < 6; i++)
	{
		std::cout << ADDMatrix1.pCSR3_FORMAT->RowIndex[i] << "\t";
	}
	std::cout <<  std::endl;
	double TestMatrixAdd2[5][5]{ { -5,-7,2,1,4},
							{1,5,0,0,0},
							{2,0,4,6,3},
							{7,8,3,1,0},
							{1,-8,0,0,7} };
	CSparseMatrix ADDMatrix2;
	ADDMatrix2.ConvertDenseToCSR3(5, 5, (double*)TestMatrixAdd2);
	ADDMatrix2.ConvertCSR3ToMKL();
	std::cout << "ADDMatrix2" << std::endl;
	for (int i = 0; i < 13; i++)
	{
		std::cout << ADDMatrix2.pCSR3_FORMAT->Values[i] << "\t";
	}
	std::cout << "\n";
	for (int i = 0; i < 13; i++)
	{
		std::cout << ADDMatrix2.pCSR3_FORMAT->ColumnIndex[i] << "\t";
	}

	std::cout << "\n";
	for (int i = 0; i < 6; i++)
	{
		std::cout << ADDMatrix2.pCSR3_FORMAT->RowIndex[i] << "\t";
	}
	std::cout << "相加" << std::endl;

	CSparseMatrix ADDReultMatrix;
	CSparseMatrix ADDReultMatrix2;
	ADDReultMatrix2.CreatMKLDataSpace();
	ADDReultMatrix2.AddMatrixByMKL(
		*ADDMatrix1.pMKL_FORMAT,
		*ADDMatrix2.pMKL_FORMAT,
		*ADDReultMatrix2.pMKL_FORMAT
		);
	ADDReultMatrix2.AddMatrixByMKL(*ADDMatrix1.pMKL_FORMAT);
	ADDReultMatrix.CreatMKLDataSpace();
	ADDReultMatrix.CopyMKLToMKL(*ADDReultMatrix2.pMKL_FORMAT, *ADDReultMatrix.pMKL_FORMAT);
	ADDReultMatrix.ConvertMKLToCSR3();
	for (int i = 0; i < ADDReultMatrix.pCSR3_FORMAT->RowIndex[ADDReultMatrix.pCSR3_FORMAT->RowCount]; i++)
	{
		std::cout << ADDReultMatrix.pCSR3_FORMAT->Values[i] << "\t";
	}
	std::cout << "\n";
	for (int i = 0; i < ADDReultMatrix.pCSR3_FORMAT->RowIndex[ADDReultMatrix.pCSR3_FORMAT->RowCount]; i++)
	{
		std::cout << ADDReultMatrix.pCSR3_FORMAT->ColumnIndex[i] << "\t";
	}

	std::cout << "\n";
	for (int i = 0; i < ADDReultMatrix.pCSR3_FORMAT->RowCount; i++)
	{
		std::cout << ADDReultMatrix.pCSR3_FORMAT->RowIndex[i] << "\t";
	}

	//double* TestMatrixAdd2 = new double[25]{ 1,-1,0,-3,0,
	//							-2,5,0,0,0,
	//							0,0,4,6,4,
	//							-4,0,2,7,0,
	//							0,8,0,0,-5 };
	//double* TestMatrixAdd2 = new double[25]{ -1,-7,2,1,4,
	//						1,5,0,0,0,
	//						2,0,4,6,3,
	//						7,8,3,1,0,
	//						1,-8,0,0,7 };






	int t = 0;
	t = 1;
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
