#include "NSTanBasicObjOpt.h"

void NSTanBasicObjOpt::DeleteObj(double * IOObjPointer)
{
	{
		if (IOObjPointer)
		{
			delete[] IOObjPointer;
		}
		IOObjPointer = 0;
	}
}
