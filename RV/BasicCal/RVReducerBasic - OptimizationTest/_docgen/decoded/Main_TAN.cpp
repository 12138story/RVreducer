# SOURCE ENCODING: gbk
#include "TANPinionCycloidGear.h"
#include "TANBearing.h"
#include "TANCubicSpline.h"
#include "TANInvoluteGear.h"
#include<fstream>
#include <vector>
#include "TANShaft.h"
#include "TANRVReducer.h"
#include "TANShaftSystem.h"
//注意，转角计算可能有误
extern "C" double _stdcall TestFunc(double* InVar, void** Other)
{
	double TestCoef;
	TestCoef = *(static_cast<double*>((Other[0])));
	return pow(*InVar, 2) + 0.5**InVar - 1.0;
}
int main ()
{
	//CPinionCycloidGearTAN Test;
	//Test.mGear.Omega = 1.;
	//Test.mPinion.Omega = 2.;
	//Test.mGear.Z = 30;
	//Test.mPinion.Z = 29;
	//Test.mGear.Rzf = 120;
	//Test.mOtherCoef.K1 = 0.65;
	//Test.mOtherCoef.K2 = 1.6;
	////Test.mGear.Omega = 1.;
	////Test.mPinion.Omega = 2.;
	////Test.mGear.Z = 26;
	////Test.mPinion.Z = 25;
	////Test.mGear.Rzf = 195;
	////Test.mOtherCoef.K1 = 0.8;
	////Test.mOtherCoef.K2 = 1.8;
	////Test.mOtherCoef.ee = Test.ChooseRoundEccentricCoef(Test.mGear.Z, Test.mGear.Rzf, Test.mOtherCoef.K1);
	////Test.mGear.rz = Test.ChooseRoundPinionToothCoverRadiusCoef(Test.mGear.Z, Test.mGear.Rzf, Test.mOtherCoef.K2);
	//Test.DefineBasicGeometryParaByK1K2(30, 29, 120, 0.65, 1.6, Test);
	//Test.CalBasicGeometryByRzK1K2WithRound();
	//std::cout << Test.FCalTransRatio(29,30,2)<<std::endl;
	////Test.CalBasicGeometry(30, 29, 8, 120, 2.6, Test);
	////Test.CalBasicGeometryByRzK1K2(30, 29, 120, 0.65, 1.6, Test);
	////Test.CalBasicGeometryByRzK1K2WithRound();
	//
	//double TestValuList[7] = {17.0,2.06e8,120.0,8.,0.75,1.5679256294882205,0.6484521072796934 };
	//void* TestVoidPtr[8];
	//TestVoidPtr[0] = static_cast<void*>(&Test.mGear.Z);
	//for (int i=0;i<7;i++)
	//{
	//	TestVoidPtr[1+i] = static_cast<void*>(&(TestValuList[0+i]));
	//}
	//double TestPinAng = 1.5;
	////std::cout << Test.AuxiliaryForMaxContStress(&TestPinAng, TestVoidPtr);
	//Test.mOtherCoef.BB = 17;
	//std::cout << Test.FindMaxContStress(769.3,
	//	2.06e8,
	//	2.06e8,
	//	2,
	//	1.1,
	//	Test,
	//	TestPinAng)<<std::endl;
	//std::cout << sin(3.1415926) << std::endl;
	//Test.CalBearingForce(3,1.2,1);
	////std::cout<< Test.CalCoefK_y()<< std::endl;
	//轴承测试
	//CBearingTAN TestBearing;
	//TestBearing.DefineTaperedRollerBearing(1380, 0.31, 1.9, 1.1, 85500, 118000);
	//TestBearing.EquivalentLoadP(1118.4, 4250);
	////TestBearing.fnSet(TestBearing.n);
	////TestBearing.CalFh();
	//TestBearing.GetBearingRealLife(1118.4, 4250);
	//std::cout << TestBearing.RealLife;
	//std::cout <<  TestBearing.fhSet(85000);
	////输出所有寿命因数值，以便后续进行插值
	//std::vector<double> LhData;
	//std::vector<double> fhData;	
	//LhData.push_back(100);
	//fhData.push_back(0.617);
	//double TempData = 0;
	//for (int i=105; i<100000;i=i+5)
	//{
	//	TempData=TestBearing.fhSet(i);
	//	if (abs(TempData- fhData.back())>0.000001)
	//	{
	//		LhData.push_back(i);
	//		fhData.push_back(TempData);
	//	}
	//}
	//LhData.back() = 100000;
	//fhData.back() = 4.90;
	//std::ofstream outfile;
	//outfile.open("D:\\WorkSpace\\softdevelop\\RV\\BasicCal\\滚子轴承寿命因数\\Lh.txt");
	//for (int i = 0; i < LhData.size(); i++)
	//{
	//	outfile << LhData[i] << ",";
	//}
	//outfile.close();
	//outfile.open("D:\\WorkSpace\\softdevelop\\RV\\BasicCal\\滚子轴承寿命因数\\fh.txt");
	//for (int i = 0; i < LhData.size(); i++)
	//{
	//	outfile << fhData[i] << ",";
	//}
	//outfile.close();
	////计算轴承系数
	//CCubicSplineTAN TestSpline;
	//TestSpline.BuildSplineObj(1, LhData.size(), &(LhData[0]), &(fhData[0]), TestSpline);
	//outfile.open("D:\\WorkSpace\\softdevelop\\RV\\BasicCal\\滚子轴承寿命因数\\Lh-fhCoef.txt");

	//for (int i = 0; i < 2*LhData.size(); i++)
	//{
	//	outfile << TestSpline.mpSurfCoef[i]<<",";
	//}
	//outfile.close();
	//TestSpline.BuildSplineObj(1, LhData.size(), &(fhData[0]), &(LhData[0]), TestSpline);
	//outfile.open("D:\\WorkSpace\\softdevelop\\RV\\BasicCal\\滚子轴承寿命因数\\fh-LhCoef.txt");
	//for (int i = 0; i < 2 * LhData.size(); i++)
	//{
	//	outfile << TestSpline.mpSurfCoef[i] << ",";
	//}
	//outfile.close();
	//渐开线测试
	//std::cout << "渐开线测试" << std::endl;
	//CInvoluteGearTAN TestInv;
	//TestInv.DefineGearPairGeometry(new double[14]{ 17,0,100,1,0.145,0.4,0.39,0,70,0,38.196,2,0,0 },
	//	new double[14] {103, 1, 100, 1, 0, 0.4, 0.39, 0, 70,0,307.943,13,0,0},
	//	new double[6] {8, 20*Degree, 15.8*Degree,500,1,1});
	//////int TestPointDimension = 1;
	//////int TestPointCount = LhData.size();
	//////std::vector<double>  SplineCoef(2 * TestPointCount);
	//////CALSPLINECOEF_TANINTER
	//////(&TestPointDimension, &TestPointCount, &(LhData[0]), &(fhData[0]), &(SplineCoef[0]));
	//////CCubicSplineTAN TestSpline;
	//////TestSpline.BuildSplineObj(TestPointDimension, TestPointCount, &(LhData[0]), &(fhData[0]), TestSpline);
	//////double TestValue = 280;
	//////double OutValue = 0;
	//////CALCUBICSPLINEINTERPVALUE_TANINTER(&TestValue, &TestPointDimension, &TestPointCount, &(LhData[0]), &(SplineCoef[0]), &OutValue);
	//////std::cout << std::endl;
	//////std::cout<<TestSpline.CalInterpValueWithExtend(100000, TestSpline);
	//////std::cout << Test.CalOneLocationNominalContStress(Test.mGear.Z,
	//////	17, Test.mGear.rf,
	//////	Test.mGear.Rzf, 
	//////	Test.mOtherCoef.K1,
	//////	Test.mOtherCoef.K2, 
	//////	TestValuList[1], 
	//////	TestValuList[3], double & InFMax)
	//////std::cout<<Test.CalOneContForce(300000, 3.1415926, Test.mOtherCoef.K1, Test.mOtherCoef.K2);
	//TestInv.CalGearPairGeometry();
	//TestInv.DefineLoadPara(9000, 360, 50000,0,0);
	//double* TestPHData = new double[6]{ 206000,0.3,500,7.83e-6,1500,430 };
	//int* TestMNData = new int[6]{ 10,1,60,30,0,0 };

	//TestInv.DefineGearPairMaterial(TestPHData, TestMNData, TestPHData, TestMNData);
	//double* TestAppData = new double[10]{ 1.0,0,0,0,1,0,0.0,0.0,0.0,320 };
	//TestInv.DefineGearPairApplicationData(TestAppData, TestAppData);
	//double* TestAcdataPinion = new double[5]{ 8.,10.,8.5,1.,3. };
	//double* TestAcdataGear = new double[5]{ 9.5,12.,9.5,1.,3.};
	//TestInv.DefineGearPairTolerance(TestAcdataPinion, TestAcdataGear);
	//TestInv.mPinion.CalDerivedTolerence();
	//TestInv.mGear.CalDerivedTolerence();
	//TestInv.CalBasicFactors();


	//TestInv.DefineGearPairGeometry(new double[14]{ 17,-1,100,1,0.14522,0.4,0.39,0,70,0,0.11779,2,0,0 },
	//	new double[14]{ 103, 1, 100, 1, 0, 0.4, 0.39, 0, 70,0,-0.02748,13,0,0 },
	//	new double[6]{ 8, 20 * Degree, 15.8*Degree,500,1,1 });
	////TestInv.CalGearPairGeometry();
	//TestInv.DefineLoadPara(9000, 360, 50000, 0, 0);
	//double* TestPHData = new double[8]{ 206000,0.3,500,7.83e-6,1500,500,500,1080};
	//int* TestMNData = new int[6]{ 10,1,60,30,0,0 };

	//TestInv.DefineGearPairMaterial(TestPHData, TestMNData, TestPHData, TestMNData);
	//double* TestAppData = new double[8]{ 0,0,0,1,1,1,1,1};
	//double* TestAppCommonData = new double[7]{ 1.0,1.0,0.0,320,1,1.0,1.0};
	//TestInv.DefineGearPairApplicationData(TestAppData, TestAppData, TestAppCommonData);
	//double* TestAcdataPinion = new double[6]{ 5,8.5,9.5,8.,1.,3. };
	//double* TestAcdataGear = new double[6]{ 5,9.0,9.5,9.0,1.,3. };
	//TestInv.DefineGearPairTolerance(TestAcdataPinion, TestAcdataGear);
	////TestInv.mPinion.CalDerivedTolerence();
	////TestInv.mGear.CalDerivedTolerence();
	////TestInv.CalBasicFactors();
	////TestInv.CalSurfaceDurability();
	////TestInv.CalToothBendingStrength();
	//TestInv.InterfaceCalGeometryAndStrength();

    //CSparseMatrixCSR::TestFunc();

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
	//渐开线齿轮计算
	double* TestGEPinionData = new double[14] { 10, -1, 12.4, 1, 0, 0.25, 0.25, 0, 0, 0, 0, 2, 0, 0 };
	double* TestGEGearData = new double[14] { 38, 1, 7, 1, 0, 0.25, 0.25, 0, 0, 0, 0, 4, 0, 0 };
	double* TestGECommonData = new double[6] { 1.75, 25 * Degree, 10 * Degree, 42, 1, 3 };
	TestRVReducer.mFirstGearPair.DefineGearPairGeometry(TestGEPinionData,
		TestGEGearData,
		TestGECommonData);
	//TestInv.CalGearPairGeometry();
	TestRVReducer.mFirstGearPair.DefineLoadPara(5.1241830065359473, -2295, 50000, 0, 0);
	TestRVReducer.mFirstGearPair.mGear.DefineLocationPara(new double[2] {9, 0});

	double* TestPHData = new double[8]{ 206000,0.3,500,7.83e-6,1500,500,500,1080 };
	double* TestMNData = new double[6]{ 10,1,60,30,0,0 };
	TestRVReducer.mFirstGearPair.DefineGearPairMaterial(TestPHData, TestMNData, TestPHData, TestMNData);
	double* TestAppData = new double[8]{ 0,1,1,0,3,1,1,1 };
	double* TestAppCommonData = new double[7]{ 1.25,1.0,0.0,320,1.0,1.0,1.0 };
	TestRVReducer.mFirstGearPair.DefineGearPairApplicationData(TestAppData, TestAppData, TestAppCommonData);
	double* TestAcdataPinion = new double[6]{ 5,5.5,6.0,5.5,1.,3. };
	double* TestAcdataGear = new double[6]{ 5,6.0,6.0,5.5,1.,3. };
	TestRVReducer.mFirstGearPair.DefineGearPairTolerance(TestAcdataPinion, TestAcdataGear);
	double* TestAssembleData = new double[6]{ 5,0,17,3.4,34 };
	TestRVReducer.mFirstGearPair.DefineGearPairAssembleData(TestAssembleData);
	TestRVReducer.mFirstGearPair.InterfaceCalGeometryAndStrength();
	std::cout << TestRVReducer.mFirstGearPair.mPinion.sigma_H << std::endl;
	std::cout << TestRVReducer.mFirstGearPair.mGear.sigma_H << std::endl;
	////摆线轮计算
	//CPinionCycloidGearTAN TestRVCycloid;
	//摆线轮1计算
	TestRVReducer.mSecondGearPair.DefineBasicGeometryPara(40, 39, 3, 157/2.0, 1.5, new double[2] {12, 12}, 24,2);
	TestRVReducer.mSecondGearPair.mPinionList[0].DefineLocationPara(new double[2] {29, 1});
	TestRVReducer.mSecondGearPair.mPinionList[1].DefineLocationPara(new double[2] {53, -1});
	TestRVReducer.mSecondGearPair.mGear.DefineLocationPara(new double[2] {29, 1});
	TestRVReducer.mSecondGearPair.CalBasicGeometry();
	TestRVReducer.mSecondGearPair.DefineStressCalPara(TestRVReducer.mSecondPairPinionTorqueForShaft, 2.06e8, 2.06e8, 1.0);
	TestRVReducer.mSecondGearPair.FindMaxContStress();
	////TestRVReducer.mSecondGearPair.mOtherCoef.BB = 12;
	////TestRVReducer.mSecondGearPair.mMaxContStress=TestRVReducer.mSecondGearPair.FindMaxContStress(TestRVReducer.mSecondPairPinionTorque,
	////	2.06e8,
	////	2.06e8,
	////	2,
	////	1.0,
	////	TestRVReducer.mSecondGearPair,
	////	TestRVReducer.mSecondGearPair.mPinAngInMaxConStress);
	//std::cout << TestRVReducer.mSecondGearPair.mMaxContStress << std::endl;
	////摆线轮2计算
	//TestRVReducer.mThirdGearPair.DefineBasicGeometryPara(40, 39, 3, 157 / 2.0, 1.5, new double[2] {12, 12}, 24, 2);
	//TestRVReducer.mThirdGearPair.mPinionList[0].DefineLocationPara(new double[2] {29, 1});
	//TestRVReducer.mSecondGearPair.mPinionList[1].DefineLocationPara(new double[2] {53, -1});
	//TestRVReducer.mThirdGearPair.mGear.DefineLocationPara(new double[2] {29, 1});
	//TestRVReducer.mThirdGearPair.CalBasicGeometry();
	//TestRVReducer.mThirdGearPair.DefineStressCalPara(TestRVReducer.mSecondPairPinionTorque, 2.06e8, 2.06e8, 1.0);
	//TestRVReducer.mThirdGearPair.FindMaxContStress();
	//轴系计算	
	//先要计算各齿轮副的力
	TestRVReducer.mFirstGearPair.CalForceVectorByRotAng(0);
	TestRVReducer.mSecondGearPair.CalBearingForce(3, 1.0, TestRVReducer.mFirstGearPair.a_w);
	TestRVReducer.mSecondGearPair.CalShaftBearingForceByRotAng(0, 0, 0);
	//TestRVReducer.mThirdGearPair.CalBearingForce(3, 1.0, TestRVReducer.mFirstGearPair.a_w);
	TestRVReducer.mSecondGearPair.CalShaftBearingForceByRotAng(1, 0, Pi);
	CShaftSystemTAN TestShaftSystem;
	TestRVReducer.mBearingList.emplace_back(CBearingTAN(TaperedRollerBearing, new double[6] {abs(TestRVReducer.mFirstPairCrankShaftBearingRPM), 14800, 13200,0.32,1.9,1.0}));
	TestRVReducer.mBearingList.emplace_back(CBearingTAN(TaperedRollerBearing, new double[6] {abs(TestRVReducer.mFirstPairCrankShaftBearingRPM), 14800, 13200, 0.32, 1.9, 1.0}));
	TestRVReducer.mBearingList[0].DefineLocationPara(new double[6] {-1, 28, 1, 8.2, 1,  11.75});//该定位尺寸等于28-(11.75-8.2)
	TestRVReducer.mBearingList[1].DefineLocationPara(new double[6] {-1, 54, 1, 8.2, -1,11.75});
	TestRVReducer.mFirstGearPair.mGear.DefineLocationPara(new double[2] {5.45, 0});
	std::vector<void*> TestAllComponent;
	std::vector<int> TestComponentType;
	TestAllComponent.emplace_back(static_cast<void*>(&TestRVReducer.mFirstGearPair.mGear));
	TestComponentType.emplace_back(InvGearNotation);
	TestAllComponent.emplace_back(static_cast<void*>(&TestRVReducer.mSecondGearPair.mPinionList[0]));
	TestComponentType.emplace_back(CycloidPinionNotation);
	TestAllComponent.emplace_back(static_cast<void*>(&TestRVReducer.mSecondGearPair.mPinionList[1]));
	TestComponentType.emplace_back(CycloidPinionNotation);
	TestAllComponent.emplace_back(static_cast<void*>(&TestRVReducer.mBearingList[0]));
	TestComponentType.emplace_back(TaperedRollerBearing);
	TestAllComponent.emplace_back(static_cast<void*>(&TestRVReducer.mBearingList[1]));
	TestComponentType.emplace_back(TaperedRollerBearing);
	for (int i = 0; i < TestAllComponent.size(); i++)
	{
		TestShaftSystem.mComponentNumList.emplace_back(i);
	}
	TestShaftSystem.MapComponentListByNum(TestShaftSystem.mComponentNumList, TestComponentType, TestAllComponent );
	int const TestNodeCount = 2;
	double TestNodeList[2]{ 0,69 };
	double TestShaftDirect[3]{ 0,0,1 };
	int TestSecType[1]{ 1 };
	double TestRC[1]{ 15};
	TestShaftSystem.mShaft.mEX = 2.1 * pow(10, 8);
	TestShaftSystem.mShaft.mPRXY = 0.3;
	TestShaftSystem.mShaft.DefineOrginGeometry(TestNodeCount, TestNodeList, TestShaftDirect, TestSecType, TestRC);
	TestShaftSystem.CalShaftSystem();
	//NSTanIOStream::PrintMatrixInConsole(42, 1, &TestShaftSystem.mShaft.mSolver.TotalUValue[0]);
	int t = 0;
	////TestShaftSystem.CalShaftSystemOnce();
	////TestShaftSystem.mShaft.TransformGeometyParaToOrderSet();
	//////TestShaftSystem.mShaft.InsertGeometyPara(TestNodeCount, TestNodeList, TestShaftDirect, TestSecType, TestRC);

	////TestShaftSystem.mShaft.InsertActionPointList(TestShaftSystem.mBCNodeList);
	////std::vector<CShaftTAN::tagBCNode> TempBCNodeList;
	////TempBCNodeList.emplace_back(CShaftTAN::tagBCNode());
	////std::set<double>::iterator TempIt = TestShaftSystem.mShaft.mNodeZCoorList.begin();
	////TempIt++;
	////TempBCNodeList[0].ZCoor = (TestShaftSystem.mShaft.mZCoorBegin + (*TempIt)) / 2;
	////TempBCNodeList[0].FreedomIndexList.emplace_back(3);
	////TempBCNodeList[0].FreedomValueList.emplace_back(0);
	////TestShaftSystem.mShaft.InsertActionPointList(TempBCNodeList);
	////TestShaftSystem.mShaft.ClearCalculatorData();
	////CShaftTAN TestShaft;
	////int const TestNodeCount = 3;
	////double TestNodeList[TestNodeCount]{ 0,0.2,0.5 };
	////double TestShaftDirect[3]{ 0,0,1 };
	////int TestSecType[2]{ 1,1 };
	//////double TestRC[4]{ 4.908738521234052*pow(10,-10), 4.908738521234052*pow(10,-10), 9.817477042468103*pow(10,-10), 0.00007853981633974483 };
	////double TestRC[2]{ 0.01,0.01 };
	////TestShaft.InsertGeometyPara(TestNodeCount, TestNodeList, TestShaftDirect, TestSecType, TestRC);
	////TestShaft.mEX = 2.1 * pow(10, 11);
	////TestShaft.mPRXY = 0.3;
	//////TestShaft.mNodeBCList[0] = { 0.0,std::vector<int>(6) = {1,2,3,4,5,6}, std::vector<double>(6,0) };
	//////TestShaft.mNodeBCList[2] = { 0.0,std::vector<int>(1) = {1001}, std::vector<double>(1,100) };
	////
	////std::vector<CShaftTAN::tagBCNode> TestBCList;
	////TestBCList.emplace_back(CShaftTAN::tagBCNode());
	////(std::vector<int>(6) = {1,2,3,4,5,6}).swap(TestBCList[0].FreedomIndexList);
	////std::vector<double>(6,0).swap(TestBCList[0].FreedomValueList);
	////TestBCList[0].ZCoor = 0;
	////TestShaft.InsertActionPointList(TestBCList);
	////std::vector<CShaftTAN::tagBCNode> TestBCList2;
	////TestBCList2.emplace_back(CShaftTAN::tagBCNode());
	////std::vector<int>(1, 1001).swap(TestBCList2[0].FreedomIndexList);
	////std::vector<double>(1, 100).swap(TestBCList2[0].FreedomValueList);
	////TestBCList2[0].ZCoor = 0.4;
	////TestShaft.InsertActionPointList(TestBCList2);
	////TestShaft.CalBeamStruct();
	////TestShaft.ClearCalculatorData();

	////TestShaftSystem.AddBearingToElemList(TestRVReducer.mBearingList[0]);
	////TestShaftSystem.AddBearingToElemList(TestRVReducer.mBearingList[3]);
	////TestShaftSystem.mComponentList.emplace_back(CShaftSystemTAN::tagShaftComponent(1, (void*)&TestRVReducer.mFirstGearPair.mGear));
	////TestShaftSystem.mComponentList.emplace_back(CShaftSystemTAN::tagShaftComponent(2, (void*)&TestRVReducer.mSecondGearPair.mPinion));
	////TestShaftSystem.mComponentList.emplace_back(CShaftSystemTAN::tagShaftComponent(2, (void*)&TestRVReducer.mThirdGearPair.mPinion));
	////TestShaftSystem.CalTotalExtendFa();

	////TestShaftSystem.PushBackInvGear(&(TestRVReducer.mFirstGearPair.mGear));
	////TestShaftSystem.PushBackCycloidPinion(&(TestRVReducer.mSecondGearPair.mPinion));
	////TestShaftSystem.PushBackCycloidPinion(&(TestRVReducer.mThirdGearPair.mPinion));
	////TestShaftSystem.PushBackBearing(&TestRVReducer.mBearingList[0]);
	////TestShaftSystem.PushBackBearing(&TestRVReducer.mBearingList[3]);
	////map测试
	//std::set<int> TestSet;
	//TestSet.insert(6);
	//TestSet.insert(5);
	//TestSet.insert(3);
	////TestSet.emplace_hint(3);
	//std::set<int>::iterator it1= TestSet.begin();
	//std::set<int>::iterator it2 = TestSet.end();
	//std::cout << std::distance(it1, it2) << std::endl;
	//std::pair<std::set<int>::iterator, bool> TestDex = TestSet.emplace(4);
	//std::cout << std::distance(it1, TestDex.first) << std::endl;
	//
	//std::list<int> TestList2;
	//
	//TestList2.emplace_back(1);
	//TestList2.emplace_back(2);
	//TestList2.emplace_back(3);
	//int Temp = 0;
	//std::list<int>::iterator TestIt2 = TestList2.begin();
	//TestIt2++;
	//TestList2.insert(TestIt2, 0);
	//std::deque<int> TestDeque;
	//TestDeque.emplace_back(1);
	//TestDeque.emplace_back(2);
	//TestDeque.emplace_back(3);
	//TestDeque.insert(TestDeque.begin() + 1, 0);
	//std::list<CTanVector> TestList;
	//TestList.emplace_back(CTanVector(4));
	//TestList.emplace_back(CTanVector(5));
	////std::cout << (it1 - it2) << std::endl;
	//(TestList.back())[0] = 1.0;
	////
	//// 
	////
	////std::cout << TestDex.first - it << std::endl;
	//std::map<double, int> TestMap;
	//std::map<double, int>::iterator iter;

	//TestMap[1.01] = 2;
	//TestMap[2.01] = 1;
	//iter = TestMap.begin();
	//std::cout << iter->second << std::endl;;
	////TestRVReducer.mBearingList[0].CalForceLocationPoint(new double[4] {28, 1, 11.75 - 8.2,  0});
	////TestRVReducer.mBearingList[1].CalForceLocationPoint(new double[3] {34.75, 2,  20.14});
	////TestRVReducer.mBearingList[2].CalForceLocationPoint(new double[3] {47.5, 2,  20.14});
	////TestRVReducer.mBearingList[3].CalForceLocationPoint(new double[4] {54, 1, 11.75 - 8.2,  1});
	////TestShaftSystem.mComponentList.emplace_back(1, static_cast<void*>(&TestRVReducer.mFirstGearPair.mGear), 103.0)



	////TestRVReducer.mBearingList.emplace_back(CBearingTAN())

	////TestShaftSystem.mComponentList.emplace()



	//TestShaftSystem.mComponentList.emplace_back(CShaftSystemTAN::tagShaftComponent(1, static_cast<void*>(&TestRVReducer.mFirstGearPair.mGear), 103.0));
	//std::cout << TestRVReducer.mThirdGearPair.mMaxContStress << std::endl;
	//TestRVReducer.mThirdGearPair.CalBearingForce(3,1.2, TestRVReducer.mFirstGearPair.a_w);
	////system("pause");
	//TestRVReducer.mFirstGearPair.mGear.F_r;
	//TestRVReducer.mFirstGearPair.mGear.F_t;

	//
	////CShaftSystemTAN::tagShaftComponent A = { 1, static_cast<void*>(&TestRVReducer.mFirstGearPair.mGear), 103.0 };








	////double TestNode[6]{ 0,0,0 ,0.0,0.0,0.4 };




	////double TestEXPRXY[2]{ 2.1 * pow(10, 11) ,0.3 };

	////int TestBCIndex[14]{ 0,1,0,2,0,3,0,4,0,5,0,6,1,1001 };
	////double TestBC[7]{ 0,0,0,0,0,0,100 };

	////TestShaft.CalBeamStruct(2, TestNode, TestEXPRXY, TestSecType, TestRC, 7, TestBCIndex, TestBC);


	//system("pause");
	//// 
	//// 
	////TestInv.mPinion.CalVirtualTooth(TestInv.Xi_alpha, TestInv.Xi_beta);

	//////润滑油粘度插值
	////std::vector<double> v40Data;
	////std::vector<double> v50Data;
	////double V40Data[]{ 10,15,22,32,46,68,100,150,220,320 };
	////double V50Data[]{ 7.5,10.6,15.0,21.0,30.0,43.0,61.0,89.0,125.0,180.0 };
	////double vf[]{ 0.0068,0.0131,0.023,0.040,0.067,0.107,0.158,0.227,0.295,0.370 };
	////double VData[10][2];
	////for (int i=0;i<10;i++)
	////{
	////	VData[i][0] = V50Data[i];
	////	VData[i][1] = vf[i];
	////}
	////CCubicSplineTAN OilSplinev40v50;
	////std::vector<double> TestVector(2);
	////OilSplinev40v50.BuildSplineObj(1, 10, V40Data, V50Data);
	//////OilSplinev40v50.CalInterpValueWithExtend(25, &(TestVector[0]));
	////std::ofstream outfile;
	////outfile.open("D:\\WorkSpace\\softdevelop\\RV\\BasicCal\\润滑油系数\\V40.txt");
	////for (int i = 0; i < 10; i++)
	////{
	////	outfile << V40Data[i] << ",";
	////}
	////outfile.close();
	////outfile.open("D:\\WorkSpace\\softdevelop\\RV\\BasicCal\\润滑油系数\\V40-V50.txt");
	////for (int i = 0; i < 20; i++)
	////{
	////	outfile << OilSplinev40v50.mpSurfCoef[i] << ",";
	////}
	////outfile.close();
	////CCubicSplineTAN OilSplinev40vf;
	////OilSplinev40vf.BuildSplineObj(1, 10, V40Data, vf);
	////outfile.open("D:\\WorkSpace\\softdevelop\\RV\\BasicCal\\润滑油系数\\V40-Vf.txt");
	////for (int i = 0; i < 20; i++)
	////{
	////	outfile << OilSplinev40vf.mpSurfCoef[i] << ",";
	////}
	////outfile.close();
	////std::cout << "润滑油" << std::endl;
	////std::cout<<TestInv.mOilV40ToV50Curve.CalInterpValueWithExtend(220) << std::endl;
	////std::cout << TestInv.mOilV40ToVfCurve.CalInterpValueWithExtend(220) << std::endl;

	return 0;
}