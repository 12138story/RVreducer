#pragma once
#include "PreDefine.h"
#include "CNode.h"
#include "CElem.h"
#include "CElemType.h"
#include "CMaterials.h"
#include "CRealConstant.h"
#include "CBoundaryCondition.h"
#include "CInnerElemTypeList.h"
#include "CFEMSolver.h"

class CShaftTAN
{
public:
	CShaftTAN();
	~CShaftTAN();
	struct tagBCNode
	{
		int NodeNum=0;
		double ZCoor=0;
		std::vector<int> FreedomIndexList;
		std::vector<double> FreedomValueList;
	};
	//初始化函数
	void Initialization();
	//存储截面类型和数据长度的数组
	// 初始值化值如下
	//Insectype=0,代表为输入极为实际常数，数据量4
	//Insectype=1, 输入为圆，数据量1。
	//Insectype=2, 输入为偏心圆，数据量4。
	static int mDefaultSecTypeToDataCount[3];
	//计算时后截面类型与数据量对应数列，默认为指向mSecTypeToDataCount;
	int* mCalSecTypeToDataCount;
	CFEMPreprocessor mProcessor;
	CFEMSolver mSolver;
	
	////材料属性,弹性模量，泊松比d
	double mEX;
	double mPRXY;
	int mNodeCount;
	////边界条件
	//int mBCCount;
	//int* mpBCNodeList;
	//int* mpBCNodeFreedomIndex;
	//double* mpBCNodeData;
	//int DeleteDatadSpace();
	//int CreatDataSpace(int InNodeCount, int InBCCount);
	//std::map<double> mNodeList;
	//std::vector
	std::vector<double> BendingNormalStress;
	std::vector<double> ShearStress;
	std::vector<double> mOriginGeoemtryNode;
	std::vector<int> mOriginSecRealConstType;
	std::vector<CTanVector> mOriginSecRealConst;
	CTanVector mShaftDirect;
	std::set<double> mNodeZCoorList;
	double mZCoorBegin;
	double mZCoorEnd;

	//单元末尾节点与单元实常数的映射
	std::map<double,CTanVector> mRealConstList;
	//std::vector<tagBCNode> mNodeObjList;
	// //开始只插入作用点坐标，最后再组装
	std::map<double, tagBCNode*> mNodeBCListZCoorToBCObjPtr;
	
	std::map<int, tagBCNode*> mNodeBCListNumToBCObjPtr;
	//经过该函数后所有截面都会变成4参量的SecType=0的标准计算力学截面。
	void DefineOrginGeometry(int InNodeCount, double* InNodeZCoorList, double* InShaftDirect, int* InSecTypeList, double* InSecTypeDataList);	
	void InsertGeometyPara(int InNodeCount, double* InNodeZCoorList,double* InShaftDirect, int* InSecTypeList, double* InSecTypeDataList);
	void TransformGeometyParaToOrderSet();

	void InsertActionPointList(std::vector<tagBCNode>& InBCList);
	//清空计算数据，包括mNodeZCoorList，mRealConstList，mNodeBCListZCoorToBCObjPtr，和，mNodeBCListNumToBCObjPtr
	void ClearCalculatorData();
	void DefineNodeList(int InNodeCount,double* InNodeCoor);
	//void GenElemList();
	//void DefineSecList(int* SecTypeList,  double* InSecTypeDataList);
	//返回值为指针位置偏移量；Insectype==0,代表为输入极为实际常数;Insectype==1, 输入为圆。
	//输出值为欧拉梁实常数,0-惯性矩，1-惯性矩，2-极惯性矩，3-面积,
	int GenRealConstBySecType(int InSecType, double* InSecTypeData,double* OutRealConst);
	void DefineMat(double InEx, double InPRXY);
	//InBCFreedomIndex为2倍InBCCount，每2个数值构成节点号和xy值
	void CalBeamStruct(int InNodeCount, double * InNodeCoorList, double * InMatEXPRXY, int* InSecTypeList, double* InSecTypeDataList, int InBCCount, int* InBCFreedomIndex, double * InBCValueList);
	void CalBeamStruct();
	void GetForceAndForceLocation();
	//void GenRealConstBySecType();
	
private:
	//用于临时存储计算得到的实常数，并插入到前处理器中。
	CTanVector mpSecRealConst;
	//需要输出的节点结果数据类型，用于数据存储空间生成，目前只用了位移
	int mResultTypeCount=1;
};

