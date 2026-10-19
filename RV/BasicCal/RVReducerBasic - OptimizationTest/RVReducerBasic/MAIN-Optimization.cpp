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
	CPinionCycloidGearTAN TestGear;
	//int Z = 29; 
	//int d_rp = 3;//针齿直径
	//double d_p = 42;
	//double e = 0.4;
	//double d_m = 3;
	//double b = 8;
	int Z = 39;//摆线轮齿数
	int d_rp = 7; //针齿直径
	double d_p = 177;// 针齿中心圆直径
	double e = 1.7;
	double d_m = 10; //量棒直径
	double b = 30;
	double mn = d_p / Z;
	int level = 5;
	double tolerance = 0.002;//量棒公差
	double M = TestGear.CalcMValue(Z, d_rp, d_p, e, d_m, tolerance);
	double DT = TestGear.CalToothTipDistanceValue(Z, d_rp, d_p, e);

	TestGear.ObtainFourCycloidPara(level, d_p, mn, b);

	CRVReducerTAN TestRVReducer;
	//完整推荐参数过程	
		//渐开线齿轮计算
	TestRVReducer.mFirstGearPair.DefineGearPairGeometryZJ(new double[31] { 17, -1, 100, 1, 0.1178, 0.4, 0.39, 0, 2, 0, 0, 2, 0, 0, 0, -0.0670, -0.0926, 0, 0, 0, 0, 0, 0, 1.4, 0.39, 0, 0, 0, 0, 0, 0},
		new double[31] { 103, 1, 100, 1, 0.0275, 0.4, 0.39, 0, 2, 0, 0, 13, 0, 0, 0, -0.0617, -0.0947, 0, 0, 0, 0, 0, 0, 1.4, 0.39, 0, 0, 0, 0, 0, 0},
		new double[9] { 8, 20 * Degree, 15.8 * Degree, 500, 1, 1, 0, 0, 360});
	TestRVReducer.mFirstGearPair.CalGearPairGeometryZJ();
	std::cout << "is ok!" << std::endl;
	//总体传动比分配
	TestRVReducer.mFixElemNotation = RVShellFixNotation;
	TestRVReducer.RecommandTransmissionRatio(RVShellFixNotation, 153);
	// RV-80E1S 推荐
	// 摆线轮推荐
	//TestRVReducer.CalTotalTransRatio(3.8,40);
	TestRVReducer.mOutputTorque = 784.0;
	TestRVReducer.mOutputRPM = 15.0;
	TestRVReducer.CalGearPairTorque();
	TestRVReducer.mSecondPairPinionTorqueForShaft;
	double TestRVTargetContactStress = 1000;
	TestRVReducer.mSecondGearPair.RecommondMainGeometryParaByContact(TestRVTargetContactStress,
		TestRVReducer.mSecondPairPinionTorqueForShaft, 
		TestRVReducer.mSecondGearPair.mGear.Z, 
		TestRVReducer.mSecondGearPair.Ng);
	//渐开线齿轮副推荐
	TestRVReducer.mDesignLife = 50000;
	TestRVReducer.mFirstGearPair.RecommondMainGeometryParaByContact(TestRVReducer.mFirstPairGearRatio, 
		1.2,//使用系数
		TestRVReducer.mFirstPairMotorInputTorque,
		TestRVReducer.mFirstPairPinionRPM,
		TestRVReducer.mDesignLife,
		1500,//材料许用极限接触应力
		ExternalMeshNotation,
		3);//行星轮数目

	std::cout << TestRVReducer.mFirstGearPair.u << std::endl;
	std::cout << TestRVReducer.mFirstGearPair.mPinion.sigma_F << std::endl;
	std::cout << TestRVReducer.mFirstGearPair.mGear.sigma_F << std::endl;
	std::cout << TestRVReducer.mFirstGearPair.mPinion.sigma_H << std::endl;
	std::cout << TestRVReducer.mFirstGearPair.mGear.sigma_H << std::endl;


	//TestRVReducer.RecommandTransmissionRatio(RVShellFixNotation, 100);
	//TestRVReducer.mFixElemNotation = 0;
	//TestRVReducer.mOutputTorque = 3136.0;
	//TestRVReducer.mOutputRPM = 15.0;
	//TestRVReducer.mFirstGearPair.mGear.Z = 82;
	//TestRVReducer.mFirstGearPair.mPinion.Z = 14;
	//TestRVReducer.mSecondGearPair.mGear.Z = 40;
	//TestRVReducer.mSecondGearPair.mPinionList[0].Z = 39;
	////Test.mPinion.Z = 29;
	//TestRVReducer.CalTotalTransRatio();
	//TestRVReducer.CalGearPairTorque();
	//double testTorque = abs(TestRVReducer.mFirstPairPinionInputTorque);
	//double testRpm = abs(TestRVReducer.mFirstPairPinionRPM);
	//double testU = TestRVReducer.mFirstGearPair.mGear.Z / double(TestRVReducer.mFirstGearPair.mPinion.Z);

	//std::vector<int> testVector;
	//NSTanBasiceFunction::GenerateArithmeticSequenceContainInputValueInRange(22, 2, 5, 0, 10, testVector);

	//TestRVReducer.mFixElemNotation = 0;
	//TestRVReducer.mOutputTorque = 8820.0;
	//TestRVReducer.mOutputRPM = 15.0;
	//TestRVReducer.mFirstGearPair.mGear.Z = 106;
	//TestRVReducer.mFirstGearPair.mPinion.Z = 23;
	//TestRVReducer.mSecondGearPair.mGear.Z = 58;
	//TestRVReducer.mSecondGearPair.mPinionList[0].Z = 57;
	////Test.mPinion.Z = 29;
	//TestRVReducer.CalTotalTransRatio();
	//TestRVReducer.CalGearPairTorque();
	//double testTorque = abs(TestRVReducer.mFirstPairPinionInputTorque);
	//double testRpm = abs(TestRVReducer.mFirstPairPinionRPM);
	//double testU = TestRVReducer.mFirstGearPair.mGear.Z / double(TestRVReducer.mFirstGearPair.mPinion.Z);
	//std::cout << NSTanBasiceFunction::TANGCD(9, 6) << std::endl;



	TestRVReducer.mFixElemNotation = 0;
	//TestRVReducer.mOutputTorque = -784.0;
	//TestRVReducer.mOutputRPM = 15.0;
	//TestRVReducer.mFirstGearPair.mGear.Z = 38;
	//TestRVReducer.mFirstGearPair.mPinion.Z = 10;
	//TestRVReducer.mSecondGearPair.mGear.Z = 40;
	//TestRVReducer.mSecondGearPair.mPinionList[0].Z = 39;
	//Test.mPinion.Z = 29;
	//TestRVReducer.CalTotalTransRatio();
	//TestRVReducer.CalGearPairTorque();
	
	//// RV-80E1S 推荐
	//TestRVReducer.CalTotalTransRatio(3.8,40);
	//TestRVReducer.mOutputTorque = 784.0;
	//TestRVReducer.mOutputRPM = 15.0;
	//TestRVReducer.CalGearPairTorque();
	//TestRVReducer.mSecondPairPinionTorqueForShaft;
	//TestRVReducer.mSecondGearPair.RecommondMainGeometryParaByContact(1000.0, TestRVReducer.mSecondPairPinionTorqueForShaft, 40);
	// //320H 推荐
	//TestRVReducer.CalTotalTransRatio(82.0/14, 40);
	//TestRVReducer.mOutputTorque = 3136;
	//TestRVReducer.mOutputRPM = 15.0;
	//TestRVReducer.CalGearPairTorque();
	//TestRVReducer.mSecondPairPinionTorqueForShaft;

	//TestRVReducer.mSecondGearPair.RecommondMainGeometryParaByContact(1000.0, TestRVReducer.mSecondPairPinionTorqueForShaft, 40);

	//// 900C 推荐
	//TestRVReducer.CalTotalTransRatio(106.0 / 23, 40);
	//TestRVReducer.mOutputTorque = 8820;
	//TestRVReducer.mOutputRPM = 15.0;
	//TestRVReducer.CalGearPairTorque();
	//TestRVReducer.mSecondPairPinionTorqueForShaft;
	//TestRVReducer.mSecondGearPair.RecommondMainGeometryParaByContact(1000.0, TestRVReducer.mSecondPairPinionTorqueForShaft, 40);
	//double ttt = NSMechanismFunction::ReturnPriorityNumberByLastByShortestDistance(1.35, PriorityNumberR10Type);
	//void** testPtr = new void* [3];
	//TestRVReducer.mSecondGearPair.mGear.Z = 40;
	////TestRVReducer.mSecondGearPair.mGear.rz = 3;
	//TestRVReducer.mSecondGearPair.mGear.AssembleFaceType = AssembleLeftBaseNotation;
	//for (int i = 0;i < TestRVReducer.mSecondGearPair.mPinionList.size();i++)
	//{
	//	TestRVReducer.mSecondGearPair.mPinionList[i].Z = TestRVReducer.mSecondGearPair.mGear.Z - 1;
	//	TestRVReducer.mSecondGearPair.mPinionList[i].AssembleFaceType = AssembleLeftBaseNotation;
	//	
	//}
	//TestRVReducer.mSecondGearPair.DefineStressCalPara(TestRVReducer.mSecondPairPinionTorqueForShaft, 2.06e8, 2.06e8, 1.0);




	//double TestRzf = 200.0 / 2;
	////TestRzf = 50;
	//testPtr[0] = static_cast<void*>(&TestRVReducer.mSecondGearPair);
	//testPtr[1] = static_cast<void*>(&TestRVReducer.mSecondPairPinionTorqueForShaft);
	//double tmpTargetStree = 200;
	//testPtr[2] = static_cast<void*>(&tmpTargetStree);


	//std::cout << TestRVReducer.mSecondGearPair.AuxiliaryFunctionForOptimizeGearRadius(&TestRzf, testPtr)<< std::endl;


	//void* TestSolveFunPtr = static_cast<void*>(&CPinionCycloidGearTAN::AuxiliaryFunctionForOptimizeGearRadius);
	//int testCoefCount = 3;
	//std::cout << TANFINDROOTBYITERATIONFORC(TestSolveFunPtr, &TestRzf, &testCoefCount, testPtr) << std::endl;;
	//TestRVReducer.mSecondGearPair;
	

	//int testzb = 40;
	//double testrz = 3;
	//double testRzf = 157.0 / 2;
	//std::cout << TestRVReducer.mSecondGearPair.CalK2(testzb, testrz, testRzf) << std::endl;

	//testzb = 40;
	//testrz = 4;
	//testRzf = 110;
	//std::cout << TestRVReducer.mSecondGearPair.CalK2(testzb, testrz, testRzf) << std::endl;

	//testzb = 58;
	//testrz = 6;
	//testRzf = 200;
	//std::cout << TestRVReducer.mSecondGearPair.CalK2(testzb, testrz, testRzf) << std::endl;

	int t = 0;
	//////渐开线齿轮计算
	//其它采用默认参数
	//TestRVReducer.mFirstGearPair.mPinion.Z = 10;
	//TestRVReducer.mFirstGearPair.mGear.Z = 38;
	//TestRVReducer.mFirstGearPair.alpha_n = 25 * Degree;
	//TestRVReducer.mFirstGearPair.mn = 1.75;
	//TestRVReducer.mFirstGearPair.mPinion.b_Design = 7;
	//TestRVReducer.mFirstGearPair.mGear.b_Design = 12.4;
	//TestRVReducer.mFirstGearPair.a_w = 42;
	//TestRVReducer.mFirstGearPair.n_P = 3;
	//TestRVReducer.mFirstGearPair.mPinion.Rhf_fPx = 0.25;
	//TestRVReducer.mFirstGearPair.mGear.Rhf_fPx = 0.25;
	////侧向变位系数，平衡弯曲应力。
	//TestRVReducer.mFirstGearPair.mPinion.X_E = 0.1795;
	//TestRVReducer.mFirstGearPair.mGear.X_E = -0.2555;
	//TestRVReducer.mFirstGearPair.DefineLoadPara(5.1242 * 3, -2295, 50000, 0, 0);


	//double* TestGEPinionData = new double[14] { 10, -1, 7, 1, 0, 0.25, 0.25, 0, 0, 0, 0.1795, 2, 0, 0 };
	//double* TestGEGearData = new double[14] { 38, 1, 12.4, 1, 0, 0.25, 0.25, 0, 0, 0, -0.2555, 4, 0, 0 };
	//double* TestGECommonData = new double[6] { 1.75, 25 * Degree, 0 * Degree, 42, 1, 3 };
	//TestRVReducer.mFirstGearPair.DefineGearPairGeometry(TestGEPinionData,
	//	TestGEGearData,
	//	TestGECommonData);
	
//	double* TestGEPinionData = new double[14] { 16, -1, 14, 1, 0, 0.25, 0.38, 0, 0, 0, 0, 2, 0, 0 };
//	double* TestGEGearData = new double[14] { 43, 1, 14.5, 1, 0, 0.25, 0.38, 0, 0, 0, 0, 4, 0, 0 };
//	double* TestGECommonData = new double[6] { 1.5, 25 * Degree, 0 * Degree, 45, 1, 1 };
//	TestRVReducer.mFirstGearPair.DefineGearPairGeometry(TestGEPinionData,
//		TestGEGearData,
//		TestGECommonData);
//	////TestRVReducer.mFirstGearPair.mPinion.x = 0.5;
//	TestRVReducer.mFirstGearPair.CalGearPairGeometry();
//	//std::cout << TestRVReducer.mFirstGearPair.mPinion.CalMinProfileShift() << std::endl;
////std::cout << TestRVReducer.mFirstGearPair.mGear.CalMinProfileShift() << std::endl;
//	std::cout << TestRVReducer.mFirstGearPair.mPinion.CalMaxProfileShift() << std::endl;
//	std::cout << TestRVReducer.mFirstGearPair.mGear.CalMaxProfileShift() << std::endl;
	////显示根据中心距的变位系数是多少


	//CInvoluteGearTAN::CSingleInvoluteTAN::GetAlphaByInvFunctionValue(0.0149044);

	//TestRVReducer.mFirstGearPair.mGear.DefineLocationPara(new double[2] {0, 0});

	//double* TestPHData = new double[8] { 206000, 0.3, 500, 7.83e-6, 1500, 430, 500, 1080 };
	//double* TestMNData = new double[6] { 10, 1, 60, 30, 0, 0 };
	//TestRVReducer.mFirstGearPair.DefineGearPairMaterial(TestPHData, TestMNData, TestPHData, TestMNData);
	//double* TestAppData = new double[7] { 0, 1, 1, 0, 1, 1, 1 };
	//double* TestAppCommonData = new double[7] { 1.25, 1.0, 0.0, 320, 1.0, 1.0, 1.0 };
	//TestRVReducer.mFirstGearPair.DefineGearPairApplicationData(TestAppData, TestAppData, TestAppCommonData);
	//double* TestAcdataPinion = new double[6] { 5, 5.5, 6.0, 5.5, 1., 3. };
	//double* TestAcdataGear = new double[6] { 5, 6.0, 6.0, 5.5, 1., 3. };
	//TestRVReducer.mFirstGearPair.DefineGearPairTolerance(TestAcdataPinion, TestAcdataGear);
	//double* TestAssembleData = new double[5] { 5, 0, 17, 3.4, 34 };
	//TestRVReducer.mFirstGearPair.DefineGearPairAssembleData(TestAssembleData);
	

	//TestRVReducer.mFirstGearPair.InterfaceCalGeometryAndStrength();
	//std::cout << TestRVReducer.mFirstGearPair.CalProfileShiftSumByCenterDistance() << std::endl;
	//std::cout << TestRVReducer.mFirstGearPair.mPinion.CalMinProfileShift() << std::endl;
	//TestRVReducer.mFirstGearPair.OptimizeProfileShiftToEqualLife();
	


	//TestRVReducer.mFirstGearPair.CalGearPairGeometry();
	//TestRVReducer.mFirstGearPair.mPinion.x = TestRVReducer.mFirstGearPair.mPinion.CalMinProfileShift();
	//TestRVReducer.mFirstGearPair.mGear.x = -TestRVReducer.mFirstGearPair.mPinion.x;
	//TestRVReducer.mFirstGearPair.InterfaceCalGeometryAndStrength();




	//TestRVReducer.mFirstGearPair.RecommondMainGeometryParaByContact(double(38)/10, 1.2, 5.1242,2250,50000, 1500, 1, 3);

	//TestRVReducer.mFirstGearPair.RecommondMainGeometryParaByContact(testU, 1.2, testTorque, 1500, 1);




	TestRVReducer.mFirstGearPair.mn = 3;
	TestRVReducer.mFirstGearPair.InterfaceCalGeometryAndStrength();
	std::cout << TestRVReducer.mFirstGearPair.mPinion.sigma_H << std::endl;
	std::cout << TestRVReducer.mFirstGearPair.mGear.sigma_H << std::endl;
	return 0;
}