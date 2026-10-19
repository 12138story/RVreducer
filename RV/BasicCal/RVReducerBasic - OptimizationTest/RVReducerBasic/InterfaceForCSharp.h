#pragma once
#include "TANRVReducer.h"
CRVReducerTAN gRVReducer;
struct InvGearObjInC
{
    double* GeometryData;
    double* MateriaData;
    double* AppData;
    double* TolerenceData;
    double* LoadData;
    double* AssembleData;
    double* LocData;
};
extern "C" __declspec(dllexport) void CalTotalTorqueAndRPM(double* Indata, double* OutData);
extern "C" __declspec(dllexport) void CalInvGearTolerence(double* Indata, double* OutData);
//extern "C" __declspec(dllexport) void CalInvGearStress(double* InGeometrydata, double* InMaterialAndManufactureData, double* InAppData, double* InTolerenceData, double* InLoadData, double* InAssembleData, double* InLocData,double* OutData);
extern "C" __declspec(dllexport) void CalInvGearStress2(int* InDivideIndex, double* InInputData, double* OutData);
//extern "C" __declspec(dllexport) void CalInvGearStress2(InvGearObjInC* InObj, double* OutData);
//extern "C" __declspec(dllexport) void CalCycloidGearStress(double* InputData, double* OutputData);
extern "C" __declspec(dllexport) void CalCycloidGearStressByObj(int* InputIndexDividIndex,double* InputData, double* OutputData);
//曲轴计算
//extern "C" __declspec(dllexport) void CalCrankShaftSystem(int* InputIntIndexDividIndex, int* InputDoubleIndexDividIndex, int* InputIntData, double* InputDoubleData,double* OutputData);
extern "C" __declspec(dllexport) void CalCrankShaftSystem2(int* InputIntIndexDividIndex, int* InputIntData, double* InputDoubleData, double* OutputData);
extern "C" __declspec(dllexport) void Testfunc();
extern "C" __declspec(dllexport) void CalGeometry(double* Indata, double* OutData);
extern "C" __declspec(dllexport) void RecommondPara(double* Indata, double* OutData);
extern "C" __declspec(dllexport) void GetForceAndForceLocation(int* CoordinateNumber, double* OutputDataX, double* OutputDataY);
extern "C" __declspec(dllexport) int RecommondBearingLife(double* Indata, double* OutData, char** OutCodes, int maxCount, int maxLen);
extern "C" __declspec(dllexport) void RecommondShaftPara(int* InputIntIndexDividIndex, int* InputIntData, double* InputDoubleData, double* OutputData, double* OutData);
extern "C" __declspec(dllexport) void CalBearingLife(double* Indata, double* OutData);
extern "C" __declspec(dllexport) void RecommondMainBearing(double* Indata, double* OutData);
extern "C" __declspec(dllexport) int CalCustomBearingLife(double* InData, double* OutData);
void ConvertInputDataToRVSetTotalObj(double* Indata, CRVReducerTAN& OutObj);
void ConvertInputDataToCycloidObj(double* InputData, CPinionCycloidGearTAN& OutObj);
void ConvertInputDataToCycloidObj(int* InputIndexDividIndex,double* InputData, CPinionCycloidGearTAN& OutObj);
void ConvertInputDataToInvObj(int* InputIndexDividIndex, double* InputData, CInvoluteGearTAN& OutObj);
void ConvertInputDataToShaftObj(int* InputIntIndexDividIndex,int* InputDoubleIndexDividIndex,int* InputIntData, double* InputDoubleData, CShaftTAN& OutObj);
void ConvertInputDataToShaftObj(int* InputIntData, double* InputDoubleData, CShaftTAN& OutObj);
void ConvertInputDataToBearingObj(int InLocationParaStartIndex, double* InputDoubleData, CBearingTAN& OutObj);
void AssemblyGearAndMainBearing(CShaftSystemTAN& TestShaftSystem, double* Indata, double* InBearingData, double* OutData);
namespace NSTanExportSpace
{

	void ExportInvGearDataToOutputDataList(CInvoluteGearTAN::CSingleInvoluteTAN& InGear, double* OutData);
}