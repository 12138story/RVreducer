#pragma once
#include "PreDefine.h"
#include "TANBearing.h"
#include "TANShaft.h"
#include "TANInvoluteGear.h"
#include "TANPinionCycloidGear.h"
#include <map>
class CShaftSystemTAN
{
public:
	//轴系
	CShaftSystemTAN();
	struct tagShaftComponent
	{
		//轴上构件类型标识符1-1000
		// 1-1000，齿轮，1-渐开线齿轮，2-摆线针轮中摆线轮，3-摆线针轮中的针轮
		// 1001-2000 轴承,1001-深沟球轴承，1101- 圆柱滚子轴承，1102-滚针轴承，1103-圆锥滚子轴承 
		int ComponentType;
		void* ComponentPtr;//构件指针
		double ComponentForcePointCoor=0;//构件作用点坐标
		int ComponentNodeNum;//构件节点编号
		tagShaftComponent(int a, void* b, double c) :ComponentType(a), ComponentPtr(b), ComponentForcePointCoor(c) {};
		tagShaftComponent(int a, void* b) :ComponentType(a), ComponentPtr(b) {};

	};

	//std::vector<int> mGearTypeList;
	//std::vector<void*> mGearPtrList;
	//std::vector<CBearingTAN*> mBearingPtrList;
	std::vector<int> mComponentNumList;
	std::vector<tagShaftComponent> mComponentList;
	std::vector<CShaftTAN::tagBCNode> mBCNodeList;
	std::map<double, CBearingTAN*> mBearingLocateCoorToPtrList;
	std::vector<int> mFixFreedomIndexCount;//轴系已经约束的自由度标识计数器，0-表示未约束，1-表示已经约束，且约束了多少次
	//std::map<double, CShaftTAN::tagBCNode> mBCNodeList;//带边界条件的节点列表
	CShaftTAN mShaft;
	double mSupplementBCNodeZCoor;
	void Clear();

	
	double mTotalExtendFa=0;//构件的总轴向力
	double mTotalFa;//构件的总轴向力
	int mTotalFixBearingCount;//总的端面固定轴承数
	void AddBearingToElemList(CBearingTAN& InBearing);
	void CalTotalExtendFa();
	double CalElemForceLocation(int InElemType, double* InElemDataList);

	void MapComponentListByNum(std::vector<int>& InComponentNumList, std::vector<int>& InAllComponentTypeList, std::vector<void*>& InAllComponentList);
	void PushBackInvGear(CInvoluteGearTAN::CSingleInvoluteTAN* InInvGearPtr);
	void PushBackCycloidPinion(CPinionCycloidGearTAN::tagPinion* InCycloidPinionPtr);

	//注意，轴承力根据固定方向平分
	void PushBackBearing(CBearingTAN* InBearingPtr);
	//补充约束函数
	//统计有多少位移约束
	void CountFixedFreedomList(std::vector<CShaftTAN::tagBCNode>& InExistBCNode, std::vector<int>& OutFixFreedomIndexCount);
	//如果约束不够，就在最后面补充约束
	bool SupplementFixedFreedomToBCNode(double InSupplementNodeZCoor, std::vector<int>& InFixFreedomIndexCountList, std::vector<CShaftTAN::tagBCNode>& OutSupplementBCNode);
	void CalShaftSystemFirst();//计算一次轴承系统，获取径向力
	void CalShaftSystem();
	//std::map<int,void*>
	bool IsMainBearing = false;
private:
	//计算力作用于非对称构件的作用点坐标
	double CalUnsymmetryElemForceLocation(
		double InDistBetweenAssembleEdgeToShaftLeft,//安装面距离轴左端距离
		int InAssembleFaceType,//安装面定位面类型，1-左侧，-1-右侧		 
		double InElemForcePointDistk,//轴承作用点距离距安装定位面距离		
		int InElemAxisDirectIndex//正向安装方向 1-作用点指向与轴正方向一致，接触角指向远离轴左侧方向，-1-反向安装，作用点指向与轴正方向相反，接触角指向靠近轴左侧方向
	);
	//计算力作用在对称构件中心点处的构件作用点坐标
	
	double CalSymmetryElemForceLocation(
		double InDistBetweenAssembleEdgeToShaftLeft,//安装面距离轴左端距离 
		int InAssembleFaceType,//安装面定位面类型，1-左侧，-1-右侧，0-中间
		double InElemWidth//轴承宽度
	);
};

