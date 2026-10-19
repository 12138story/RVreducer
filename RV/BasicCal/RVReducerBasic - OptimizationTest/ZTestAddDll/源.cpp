#include "Add.h"
#include <iostream>
void main()
{
	CAdd TestAdd;
	double TestData[3]{ 1,2,3 };
	std::cout << TestAdd.CalAdd(TestData, TestData + 2) << std::endl;
}
