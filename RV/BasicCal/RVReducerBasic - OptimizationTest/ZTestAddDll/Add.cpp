#include "Add.h"

double CAdd::CalAdd(double* InData, double* OutData)
{
    OutData[0] = InData[0] + InData[1];
    return OutData[0];
}
