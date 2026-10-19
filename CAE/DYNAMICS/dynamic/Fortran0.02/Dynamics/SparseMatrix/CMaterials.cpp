#include "CMaterials.h"
CMaterials::CMaterials()
{
}

CMaterials::~CMaterials()
{
}



double CMaterials::ObtainShearModulus(double InEx, double InProxy)
{
	return InEx/(2.0*(1.0+InProxy));
}

double CMaterials::ObtainShearModulus()
{
	Database["GX"]= Database["EX"] / (2.0*(1.0 + Database["PRXY"]));
	return Database["GX"];
}
