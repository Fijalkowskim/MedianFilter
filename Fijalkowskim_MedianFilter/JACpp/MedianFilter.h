#pragma once
class MedianFilter
{
public: 
	int CppFunc(int a, int b);
};

extern "C" __declspec(dllexport) int CppFunc(int a, int b)
{
	MedianFilter m;
	return m.CppFunc(a, b);
}