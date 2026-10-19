#pragma once
#include <vector>
#include <iostream>
#include <deque>
#include <numeric>
#include <map>
#include <algorithm> 
#include "mkl_spblas.h"
#include "mkl.h"
#include <string>
typedef std::vector<int> TanIntVector;
typedef std::vector<double*> TanDoublePointerVector;
typedef std::vector<double> TanDoubleVector;
typedef std::map<int, double*> TanIntpDoubleMap;