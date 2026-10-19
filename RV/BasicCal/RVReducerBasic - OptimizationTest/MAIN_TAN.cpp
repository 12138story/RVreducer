#include "TANPinionCycloidGear.h"
#include "TANBearing.h"
#include "TANCubicSpline.h"
#include "TANInvoluteGear.h"
#include<fstream>
#include <vector>
#include "TANShaft.h"
#include "TANRVReducer.h"
#include "TANShaftSystem.h"
int main()
{
	CRVReducerTAN TestRVReducer;
	TestRVReducer.mFixElemNotation = 0;
	TestRVReducer.mOutputTorque = -784.0;
	TestRVReducer.mOutputRPM = 15.0;
	TestRVReducer.mFirstGearPair.mGear.Z = 38;
	TestRVReducer.mFirstGearPair.mPinion.Z = 10;
	TestRVReducer.mSecondGearPair.mGear.Z = 40;
	TestRVReducer.mSecondGearPair.mPinionList[0].Z = 39;
	//Test.mPinion.Z = 29;
	TestRVReducer.CalTotalTransRatio();
	TestRVReducer.CalGearPairTorque();
	//½¥¿ªÏß³ÝÂÖ¼ÆËã
	double* TestGEPinionData = new double[14] { 10, -1, 12.4, 1, 0, 0.25, 0.25, 0, 0, 0, 0, 2, 0, 0 };
	double* TestGEGearData = new double[14] { 38, 1, 7, 1, 0, 0.25, 0.25, 0, 0, 0, 0, 4, 0, 0 };
	double* TestGECommonData = new double[6] { 1.75, 25 * Degree, 10 * Degree, 42, 1, 3 };
	TestRVReducer.mFirstGearPair.DefineGearPairGeometry(TestGEPinionData,
		TestGEGearData,
		TestGECommonData);
	//TestInv.CalGearPairGeometry();
	TestRVReducer.mFirstGearPair.DefineLoadPara(5.1241830065359473, -2295, 50000, 0, 0);
	TestRVReducer.mFirstGearPair.mGear.DefineLocationPara(new double[2] {0, 0});

	double* TestPHData = new double[8] { 206000, 0.3, 500, 7.83e-6, 1500, 500, 500, 1080 };
	double* TestMNData = new double[6] { 10, 1, 60, 30, 0, 0 };
	TestRVReducer.mFirstGearPair.DefineGearPairMaterial(TestPHData, TestMNData, TestPHData, TestMNData);
	double* TestAppData = new double[7] { 0, 1, 1, 0, 1, 1, 1 };
	double* TestAppCommonData = new double[7] { 1.25, 1.0, 0.0, 320, 1.0, 1.0, 1.0 };
	TestRVReducer.mFirstGearPair.DefineGearPairApplicationData(TestAppData, TestAppData, TestAppCommonData);
	double* TestAcdataPinion = new double[6] { 5, 5.5, 6.0, 5.5, 1., 3. };
	double* TestAcdataGear = new double[6] { 5, 6.0, 6.0, 5.5, 1., 3. };
	TestRVReducer.mFirstGearPair.DefineGearPairTolerance(TestAcdataPinion, TestAcdataGear);
	double* TestAssembleData = new double[5] { 5, 0, 17, 3.4, 34 };
	TestRVReducer.mFirstGearPair.DefineGearPairAssembleData(TestAssembleData);
	TestRVReducer.mFirstGearPair.InterfaceCalGeometryAndStrength();
	std::cout << TestRVReducer.mFirstGearPair.mPinion.sigma_H << std::endl;
	std::cout << TestRVReducer.mFirstGearPair.mGear.sigma_H << std::endl;
	TestRVReducer.mFirstGearPair.mn = 3;
	TestRVReducer.mFirstGearPair.InterfaceCalGeometryAndStrength();
	std::cout << TestRVReducer.mFirstGearPair.mPinion.sigma_H << std::endl;
	std::cout << TestRVReducer.mFirstGearPair.mGear.sigma_H << std::endl;
	return 0;
}