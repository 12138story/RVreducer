#pragma once
namespace NSTanBasicObjOpt
{
	//验证指针对象是否存在，不存在则建立新的对象
	template <typename T1>
	void CheckAndReturnObj(T1*& IOObjPointer)
	{
		if (!IOObjPointer)
		{
			IOObjPointer = new T1;
		}
	}
	//验证指针对象是否存在，存在则删除并建立新的对象
	template <typename T1>
	void CreateAndReturnObj(T1*& IOObjPointer)
	{
		if (IOObjPointer)
		{
			delete IOObjPointer;
		}
		IOObjPointer = new T1;
	}
	//验证指针对象是否存在，存在则删除对象
	void DeleteObj(double* IOObjPointer);

	template <typename T1>
	void DeleteObj(T1*& IOObjPointer)
	{
		if (IOObjPointer)
		{
			delete IOObjPointer;
		}
		IOObjPointer = 0;
	}
	//验证容器指针对象是否存在，存在则删除对象
	template <typename T1>
	void DeleteListObj(T1& InMapObjList)
	{
		for_each(InMapObjList.begin(), InMapObjList.end(), [](typename T1::reference& OneObj) {delete OneObj.second; });
		InMapObjList.clear();
	}
	//查找map容器指针对象
	//template <typename T1,typename T2,typename T3>
	//void FindIterInMapList(T1& InMapObjList,T2&& InKey,T3& OutIter)
	//{

	//	for_each(InMapObjList.begin(), InMapObjList.end(), [](typename T1::reference& OneObj) {delete OneObj.second; });
	//	InMapObjList.clear();
	//}
}
