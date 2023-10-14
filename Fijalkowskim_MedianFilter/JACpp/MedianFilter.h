#pragma once
class MedianFilter
{
public: 
	int Add(int a, int b);
};

extern "C" __declspec(dllexport) int Add(int a, int b)
{
	MedianFilter m;
	return m.Add(a, b);
}