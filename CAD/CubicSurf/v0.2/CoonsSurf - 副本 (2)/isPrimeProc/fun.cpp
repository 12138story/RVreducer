#include "fun.h"
//__declspec(dllexport) int isPrime(int n)
//{
//	int i;
//	for (i = 2; i*i <= n; i++)
//		if (n%i == 0) return 0;
//	return 1;
//}
__declspec(dllexport) int Add(int* A, int& B)
{
	A[1] = A[0];
	A[1] = A[1] + 5;
	return A[1] + B;
}