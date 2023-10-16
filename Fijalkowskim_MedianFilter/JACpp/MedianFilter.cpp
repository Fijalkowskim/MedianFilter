#include "pch.h"
#include "MedianFilter.h"

int MedianFilter::CppFunc(int a, int b)
{
    int pow = 1;
    for (int i = 0; i < b; i++) {
        pow = pow * a;
    }

    return pow;
}
