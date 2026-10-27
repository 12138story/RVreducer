#include "TANBearing.h"
#include "BearingCatalogRepository.h"

namespace
{
// 将数据访问层记录转换为现有公开数据结构，保持外部接口不变。
CBearingTAN::BearingData ToBearingData(const data_access::BearingCatalogRecord& record)
{
	CBearingTAN::BearingData bearing{};
	bearing.code = record.code;
	bearing.d = record.d;
	bearing.D = record.D;
	bearing.B = record.B;
	bearing.Cr = record.Cr;
	bearing.C0r = record.C0r;
	bearing.e = record.e;
	bearing.Y = record.Y;
	bearing.Yo = record.Yo;
	bearing.a = record.a;
	bearing.T = record.T;
	bearing.C = record.C;
	bearing.contactAngle = record.contactAngle;
	return bearing;
}
}
//	bearing1.fh_switch(L);
//	bearing1.fn_switch(n);
//bearing1.Para_switch(d, D);

CBearingTAN::CBearingTAN()
{
	InitialLifeCurve();
}
CBearingTAN::CBearingTAN(double n, int L, int d, int D) : n{ n }, L{ L }, d{ d }, D{ D }
{
	Para_switch(d, D);
	InitialLifeCurve();
}

CBearingTAN::CBearingTAN(int InBearingType, double* InBearingData)
{
	DefineRollingBearing(InBearingType, InBearingData);
	InitialLifeCurve();
}

CBearingTAN::CBearingTAN(double* InBearingData)
{
	DefineRollingBearing(int(InBearingData[0]), InBearingData + 1);
	InitialLifeCurve();
}

CBearingTAN::~CBearingTAN()
{

}


void CBearingTAN::EquivalentLoadP(double Fa, double Fr)
{
	switch (mBearingType) 
	{
		//深沟球轴承
		case DeepGrooveBallBearing:
			break;
		//滚针轴承
		case NeedleBearing:
			P = Fr;
			P0 = Fr;
			break;
		//圆锥滚子轴承
		case TaperedRollerBearing:
			if (Fa / Fr <= e)  //计算当量动载荷P
				P = Fr;
			else
			{
				P = (0.4 * Fr + Y * Fa);
			}
			//径向当量静载荷
			P0 = (0.5 * Fr + Yo * Fa);
			if (P0 < Fr)  //计算当量静载荷Po
			{
				P0 = Fr;
			}
			break;
		// 角接触球轴承 测试
		case AngularContactBallBearing:
			if (Fa / Fr <= e)
				P = Fr;
			else
				P = 0.44 * Fr + Y * Fa;

			P0 = 0.5 * Fr + 0.46 * Fa;
			if (P0 < Fr)
				P0 = Fr;
			break; 
	}


}

void CBearingTAN::CalFh()
{
	fh = fn * ft*Cr / (fm*fd*P);
}

double CBearingTAN::CalLifeByFh()
{	
	return mFh2LhCurve.CalInterpValueWithExtend(fh, mFh2LhCurve);
}

void CBearingTAN::Dynamic_Load(double InBearingLife,double InRPM)
{
	fh = fhSet(static_cast<int>(InBearingLife));
	fn = fnSet(InRPM);
	C = ((fh*fm*fd) / (fn*ft))*P;

}

void CBearingTAN::Static_Load()
{
	So = 2;
	Co = So*P0;	
}

void CBearingTAN::BearingBasicLifeL10(double Fa, double Fr)
{
	//计算出P
	EquivalentLoadP(Fa, Fr);
	//Dynamic_Load();
	Static_Load();
	CalAdditionalAxisForce(Fr);
	//圆锥滚子轴承
	L10 = (pow(10.0, 6.0) / (60.0 * n))*pow(Cr / P, epsilon);
	//深沟球轴承
//	L10 = (pow(10.0, 6.0) / (60.0 * n))*pow(Cr / P, 3.0);
}

//double CBearingTAN::Ex_Fd(double Fr)
//{
//	Fd = Fr / (2 * Y);
//	return Fd;
//}
double CBearingTAN::GetBearingRealLife(double InFa, double InFr)
{
	EquivalentLoadP(InFa, InFr);
	fnSet(n);
	CalFh();
	RealLife = CalLifeByFh();
	return RealLife;
}

void CBearingTAN::DefineTaperedRollerLocationPara(int InAssembleFaceType,//安装面定位面类型，1-外圈侧壁,左侧，-1-内圈侧壁，右侧，0-作用点
	double InDistBetweenAssembleEdgeToShaftLeft,//轴承安装面距离轴左端距离
	int InBearingForcePointFaceType,//轴承作用点对应的轴承面类型，1-外圈侧壁,左侧，-1-内圈侧壁，右侧，0-作用点
	double InBearingForcePointDist,//轴承作用点距离距轴承作用点参考面距离	
	int InBearingAssembleOrient,//轴承轴线方向 0-轴承轴线方向与轴一致，接触角指向远离轴左侧方向，1-轴承轴线方向与轴相反，接触角指向靠近轴左侧方向);)
	double InBearingWidth)//轴承宽度
{
	AssembleFaceType = InAssembleFaceType;
	AssebleOrientation = InBearingAssembleOrient;
	//确定安装定义面和轴承作用点是否定义面一致
	//如果一致
	if (AssembleFaceType == InBearingForcePointFaceType)
	{

		AssembleDist = InDistBetweenAssembleEdgeToShaftLeft;
		AssembleForcePointDistPara = InBearingForcePointDist;
	}
	//如果不一致,将安装参考面调整到作用点位置
	else
	{
		if (AssembleFaceType == AssembleMiddleBaseNotation)
		{
			AssembleDist = InDistBetweenAssembleEdgeToShaftLeft;
		}
		else if (InBearingForcePointFaceType == BearingForcePointToRightFace)
		{
			AssembleDist = InDistBetweenAssembleEdgeToShaftLeft + AssebleOrientation * (InBearingWidth - InBearingForcePointDist);
		}
		else
		{
			AssembleDist = InDistBetweenAssembleEdgeToShaftLeft - AssebleOrientation * (InBearingWidth - InBearingForcePointDist);
		}
		AssembleForcePointDistPara = 0.0;
		AssembleFaceType = AssembleMiddleBaseNotation;
	}
	Width = InBearingWidth;
}
void CBearingTAN::DefineRadialBearingLocationPara(int InAssembleFaceType,//安装面定位面类型，1-左侧，-1-右侧，0-中间
	double InDistBetweenAssembleEdgeToShaftLeft,//轴承安装面距离轴左端距离 		
	int InBearingAssembleOrient,//轴承安装朝向，1-正向，-1反向	
	double InBearingWidth)//轴承宽度
{
	AssembleFaceType = InAssembleFaceType;
	AssebleOrientation = InBearingAssembleOrient;
	if (AssembleFaceType == AssembleMiddleBaseNotation)
	{
		AssembleDist = 0.0;
		AssembleForcePointDistPara = 0.0;
	}
	else if (AssembleFaceType * AssebleOrientation == 1)
	{
		AssembleFaceType = AssembleLeftBaseNotation;
		AssebleOrientation = BearingExForceToRightNotation;
		AssembleForcePointDistPara = InBearingWidth / 2;
	}
	else
	{
		AssembleFaceType = AssembleRightBaseNotation;
		AssebleOrientation = BearingExForceToRightNotation;
		AssembleForcePointDistPara = InBearingWidth / 2;
	}
	AssembleDist = InDistBetweenAssembleEdgeToShaftLeft;
	Width = InBearingWidth;
}

void CBearingTAN::DefineLocationPara(double* InAssembleData)
{

	if (mBearingType == DeepGrooveBallBearing ||
		mBearingType == NeedleBearing)//如果是对称结构
	{
		DefineRadialBearingLocationPara(
			static_cast<int>(InAssembleData[0]),
			InAssembleData[1],
			static_cast<int>(InAssembleData[4]),
			InAssembleData[5]);
	}
	else
	{
		DefineTaperedRollerLocationPara(static_cast<int>(InAssembleData[0]),
			InAssembleData[1],
			static_cast<int>(InAssembleData[2]),
			InAssembleData[3],
			static_cast<int>(InAssembleData[4]),
			InAssembleData[5]);
	}
}
double CBearingTAN::CalAdditionalAxisForce(double InFr)
{
	if (mBearingType==TaperedRollerBearing)
	{
		Fa_ex = InFr / (2 * Y)*AssebleOrientation;
	}
	else
	{
		Fa_ex = 0;
	}
	return Fa_ex;
}
double CBearingTAN::CalRealAxisForce(double InTotalShaftAxisForce, int InBearForceBearingCount)
{
	if (mBearingType==DeepGrooveBallBearing)
	{
		Fa = -InTotalShaftAxisForce/ InBearForceBearingCount;
	}
	//表明带有附加轴向力
	else if (abs(Fa_ex)>DefaultTolerance)
	{
		if (NSTanBasiceFunction::Fsign(InTotalShaftAxisForce) != NSTanBasiceFunction::Fsign(Fa_ex))//总体运动趋势方向与轴承力方向相反，轴承被压紧
		{
			Fa = Fa_ex -InTotalShaftAxisForce / InBearForceBearingCount;
		}
		else
		{
			Fa = Fa_ex;
		}
	}
	else
	{
		Fa = 0.0;
	}

	return Fa;
}
int CBearingTAN::JudgeBearingIsAxisConstrain(double InTotalAxisForce)
{
	if (mBearingType == DeepGrooveBallBearing)
	{
		return 1;
	}
	else if (mBearingType == NeedleBearing)
	{
		return 0;
	}
	else if (mBearingType == TaperedRollerBearing|| mBearingType == AngularContactBallBearing)
	{
		if (NSTanBasiceFunction::Fsign(InTotalAxisForce) != NSTanBasiceFunction::Fsign(Fa_ex))
		{
			if (abs(InTotalAxisForce)>DefaultTolerance)
			{
				return 1;
			}
			else
			{
				return 0;
			}
			
		}
		else
		{
			return 0;
		}
	}
}

void CBearingTAN::InitialLifeCurve()
{
	mLh2FhCurve.mPointCount = 210;
	mLh2FhCurve.mPointDimension = 1;
	mLh2FhCurve.mpVarList = &(NSBearingStaticArrary::sLhListInBook[0]);
	mLh2FhCurve.mpSurfCoef = &(NSBearingStaticArrary::sLh2FhCoef[0]);
	mFh2LhCurve.mPointCount = 210;
	mFh2LhCurve.mPointDimension = 1;
	mFh2LhCurve.mpVarList = &(NSBearingStaticArrary::sFhListInBook[0]);
	mFh2LhCurve.mpSurfCoef = &(NSBearingStaticArrary::sFh2LhCoef[0]);
}

void CBearingTAN::DefineDeepGrooveBallBearing(double InRPM,  double InCr, double InC0r)
{
	n = InRPM;
	Cr = InCr;
	Cor = InC0r;
	epsilon = mBallBearingCoef;
	mBearingType = DeepGrooveBallBearing;
	
}
void CBearingTAN::DefineNeedleBearing(double Inn1, double InCr1, double InC0r1)
{
	n = Inn1;
	Cr = InCr1;
	Cor = InC0r1;
	epsilon = mRollerbearingCoef;
	mBearingType = NeedleBearing;
}
void CBearingTAN::DefineAngularContactBallBearing(double n1, double Cr1, double C0r1, double tmpe, double tmpY, double tmpY1, double tmpY2)
{
	n = n1; Cr = Cr1; Cor = C0r1; epsilon = mBallBearingCoef; e = tmpe;  Y = tmpY; Y1 = tmpY1; Y2 = tmpY2;
	mBearingType = AngularContactBallBearing;
}
////圆锥滚子轴承实现1
//void CBearingTAN::bevel_ex1(double n1, int L1, int d1, int D1)
//{
//	n = n1; L = L1; d = d1; InOuterDiameter = D1;
//	
//}
//圆锥滚子轴承实现2
void CBearingTAN::DefineTaperedRollerBearing(double n1, double Cr1, double Cor1, double e1, double Y1, double Yo1 )
{
	n = n1; e = e1; Y = Y1; Cr = Cr1; Yo = Yo1; Cor = Cor1; epsilon = mRollerbearingCoef;
	mBearingType = TaperedRollerBearing;
}

void CBearingTAN::DefineRollingBearing(int InBearingType, double* InBearingData)
{
	switch (InBearingType)
	{
	case (DeepGrooveBallBearing):
		DefineDeepGrooveBallBearing(InBearingData[0], InBearingData[1], InBearingData[2]);
		break;
	case (NeedleBearing):
		DefineNeedleBearing(InBearingData[0], InBearingData[1], InBearingData[2]);
		break;
	case (TaperedRollerBearing):
		DefineTaperedRollerBearing(InBearingData[0], //额定转速
			InBearingData[1], //基本额定动载荷  Cr1
			InBearingData[2], //基本额定静载荷 Cor1
			InBearingData[3], //计算系数  e1
			InBearingData[4], //计算系数 Y1
			InBearingData[5] //计算系数 Yo1
		);
		break;
	case(AngularContactBallBearing):
		DefineAngularContactBallBearing(InBearingData[0], InBearingData[1], InBearingData[2], InBearingData[3], InBearingData[4], InBearingData[5], InBearingData[6]);
		break;
	}
		
}
//（轴承类型，[n,fa,fr,life])
// 从目录读取候选轴承，并按原有寿命计算规则生成推荐列表。
void CBearingTAN::CalAndRecommedBearingLife(int InBearingType, double* InBearingData)
{
	const auto records = data_access::BearingCatalogRepository::LoadAll(InBearingType);
	std::vector<BearingData> bearingList;
	bearingList.reserve(records.size());
	for (const auto& record : records)
	{
		bearingList.push_back(ToBearingData(record));
	}
	std::vector<BearingData> overLifeList;//存放超寿命轴承数据

	// 根据类型条件，遍历数据库进行寿命计算 
	if (InBearingType == DeepGrooveBallBearing || InBearingType == NeedleBearing)
	{
		for (int i = 0; i < bearingList.size(); i++)
		{
			CBearingTAN testBearing(InBearingType, new double[3] {InBearingData[0], bearingList[i].Cr, bearingList[i].C0r});
			bearingList[i].life = testBearing.GetBearingRealLife(InBearingData[1], InBearingData[2]); //传入Fa，Fr调用函数得到轴承寿命
			if (bearingList[i].life > InBearingData[3])
			{
				overLifeList.push_back(bearingList[i]);
			}
		}
	}
	else if (InBearingType == TaperedRollerBearing)
	{
		for (int i = 0; i < bearingList.size(); i++)
		{
			CBearingTAN testBearing(InBearingType, new double[6] {InBearingData[0], bearingList[i].Cr, bearingList[i].C0r, bearingList[i].e, bearingList[i].Y, bearingList[i].Yo});
			bearingList[i].life = testBearing.GetBearingRealLife(InBearingData[1], InBearingData[2]); //传入Fa，Fr调用函数得到轴承寿命
			if (bearingList[i].life > InBearingData[3])
			{
				overLifeList.push_back(bearingList[i]);
			}
		}
	}
	else if (InBearingType == AngularContactBallBearing)
	{
		for (int i = 0; i < bearingList.size(); i++)
		{
			double FaOverC0r = InBearingData[1] / bearingList[i].C0r;
			double e, Y, Y1, Y2;
			CBearingTAN TestBearing;
			if (TestBearing.GetLoadCoeff(FaOverC0r, e, Y, Y1, Y2))
			{
				CBearingTAN testBearing(InBearingType, new double[7] {InBearingData[0], bearingList[i].Cr, bearingList[i].C0r, e, Y, Y1, Y2});
				bearingList[i].life = testBearing.GetBearingRealLife(InBearingData[1], InBearingData[2]); //传入Fa，Fr调用函数得到轴承寿命
				if (bearingList[i].life > InBearingData[3])
				{
					overLifeList.push_back(bearingList[i]);
				}
			}
		}
	}
	else
	{
		std::cerr << "这里对所有需要的轴承进行寿命计算。可新建一个数据表格，包含所需的所有轴承，并且统一表头" << std::endl;
	}

	//std::vector<BearingData> topClosestList;//存放寿命最接近目标寿命的轴承数据
	if (overLifeList.size() == 0)
	{
		std::cout << "未发现超寿命轴承" << std::endl;
		return;
	}

	else
	{
		// 按与 InBearingData[3] 的寿命差进行排序
		std::sort(overLifeList.begin(), overLifeList.end(),
			[targetLife = InBearingData[3]](const BearingData& a, const BearingData& b) {
				//return std::abs(a.life - targetLife) < std::abs(b.life - targetLife);
				if (a.d == b.d)
					return a.B < b.B;  // 同直径，选宽度小的
				return a.d < b.d;
			});

		// 拷贝前十个（如果不足10个就拷贝全部）
		int count = (overLifeList.size() < 2000) ? overLifeList.size() : 2000;
		mBearingDataVec.assign(overLifeList.begin(), overLifeList.begin() + count);

		// 打印确认
		std::cout << "最接近目标寿命的前 " << count << " 个轴承：" << std::endl;
		for (const auto& b : mBearingDataVec) {
			std::cout << ", Name: " << b.code
				<< ", Life: " << b.life << ", Cr: " << b.Cr << ", C0r: " << b.C0r << std::endl;
		}

	}

}
// 读取并排序供界面选择的轴承目录数据。
std::vector<CBearingTAN::BearingData> CBearingTAN::ObtainBearingDataVec(int InBearingType)
{
	std::vector<BearingData> resultBearingData;
	if (InBearingType != NeedleBearing && InBearingType != TaperedRollerBearing
		&& InBearingType != AngularContactBallBearing)
	{
		return resultBearingData;
	}

	const auto records = data_access::BearingCatalogRepository::LoadAll(InBearingType);
	resultBearingData.reserve(records.size());
	for (const auto& record : records)
	{
		resultBearingData.push_back(ToBearingData(record));
	}

	std::sort(resultBearingData.begin(), resultBearingData.end(),
		[InBearingType](const BearingData& lhs, const BearingData& rhs) {
			if (lhs.d == rhs.d)
			{
				return InBearingType == TaperedRollerBearing ? lhs.T < rhs.T : lhs.B < rhs.B;
			}
			return lhs.d < rhs.d;
		});
	return resultBearingData;
}
double CBearingTAN::ObtainDistFromBearingLocatingSurfaceToReferenceSurface(int InBearingLocatingSurface, double InBearingA,  //InBearingA:轴承作用点距离距轴承作用点参考面距离
	double InBearingWidth,
	double tmpInLength) 	//tmpInLength:轴承左端面距离基准的距离
{
	double dist = 0.0;
	if (InBearingLocatingSurface == -1) //内圈侧壁，右侧（以轴承右侧为参考面）
	{
		dist = tmpInLength + InBearingWidth;
	}
	else if (InBearingLocatingSurface == 1)//外圈侧壁,左侧
	{
		dist = tmpInLength;
	}
	else if (InBearingLocatingSurface == 0)//0-作用点，先当作作用点，而不是中间位置@
	{
		dist = tmpInLength + InBearingA;
	}
	
	return dist;
}
bool CBearingTAN::GetLoadCoeff(double FaOverC0r,
	double& e, double& Y,
	double& Y1, double& Y2)
{
	// 边界处理：小于最小值，取第一个点
	if (FaOverC0r <= coeffTable.front().FaOverC0r)
	{
		const auto& data = coeffTable.front();
		e = data.e;
		Y = data.Y;
		Y1 = data.Y1;
		Y2 = data.Y2;
		return true;
	}
	// 大于最大值，取最后一个点
	if (FaOverC0r >= coeffTable.back().FaOverC0r)
	{
		const auto& data = coeffTable.back();
		e = data.e;
		Y = data.Y;
		Y1 = data.Y1;
		Y2 = data.Y2;
		return true;
	}

	// 查找区间：找到第一个大于FaOverC0r的点
	auto it = std::upper_bound(coeffTable.begin(), coeffTable.end(), FaOverC0r,
		[](double val, const BearingLoadCoeff& elem) {
			return val < elem.FaOverC0r;
		});

	// 前后两个点
	const auto& data1 = *(it - 1);
	const auto& data2 = *it;

	// 计算插值比例
	double t = (FaOverC0r - data1.FaOverC0r) /
		(data2.FaOverC0r - data1.FaOverC0r);

	// 线性插值计算所有系数
	e = data1.e + t * (data2.e - data1.e);
	Y = data1.Y + t * (data2.Y - data1.Y);
	Y1 = data1.Y1 + t * (data2.Y1 - data1.Y1);
	Y2 = data1.Y2 + t * (data2.Y2 - data1.Y2);

	return true;
}
//圆锥滚子轴承通用实现
//void CBearingTAN::bevel_ex(int para_method, double para[] )
//{
//	if (para_method == 0)
//	{
//		n = para[0]; 
//		L = round(para[1]);
//		d = round(para[2]);
//		InOuterDiameter = round(para[3]);
//		Para_switch();
//	}
//	if (para_method == 1)
//	{
//		n = para[0];
//		L = round(para[1]);
//		e = para[2]; 
//		Y = para[3];
//		Cr = para[4];
//		Yo = para[5];
//		Cor = para[6];
//	}
//}
