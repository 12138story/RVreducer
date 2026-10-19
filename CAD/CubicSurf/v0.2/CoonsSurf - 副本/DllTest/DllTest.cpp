// DllTest.cpp : 此文件包含 "main" 函数。程序执行将在此处开始并结束。
//

#include <iostream>
#include <vector>
#include <fstream>
#include <string>
template <typename T1>
int ImportDataFromTxt(std::string&& InImportPath, std::vector<T1>& OutDataList)
{
	std::ifstream data(InImportPath);
	T1 GearNodeTemp, PinNodeTemp;
	while (data >> GearNodeTemp)
		OutDataList.push_back(GearNodeTemp);//将数据压入堆栈。//
	data.close();
	return 0;
};
#pragma comment(lib,"InterpSurfTANDLL.lib")
extern "C" {void  _stdcall CALCUBICSURFCOEF_TANINTER(int*,int*,int*,double*, double*, double*,double*); }
int main()
{
	std::vector<double> pointData;
	std::vector<double> uData;
	std::vector<double> vData;
	std::string workDirection("D:\\WorkSpace\\softdevelop\\CAD\\CubicSurf\\v0.2\\CoonsSurf\\x64\\Debug\\");
	ImportDataFromTxt(workDirection + "Point.dat", pointData);
	ImportDataFromTxt(workDirection + "u.dat", uData);
	ImportDataFromTxt(workDirection + "v.dat", vData);
	int uCount = uData.size();
	int vCount = vData.size();
	int pointDimension = 3;
	std::vector<double> outCoef(pointDimension* uCount*vCount * 4, 0.0);
	CALCUBICSURFCOEF_TANINTER(&pointDimension, &uCount, &vCount, &uData[0], &vData[0], &pointData[0], &outCoef[0]);
    std::cout << "Hello World!\n";
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
