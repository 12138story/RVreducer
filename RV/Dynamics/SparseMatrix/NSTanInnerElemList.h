#pragma once
#include <tuple>
#include "CMeshElem.h"
//extern std::tuple<CMeshElem> InnerElemList;
typedef std::tuple<
	CMeshElem//±àºÅ0£¬ÄöºÏµ¥Ôª
> TanInnerElemListType;//
namespace NSTanInnerElemList
{
	extern TanInnerElemListType InnerElemList;

}