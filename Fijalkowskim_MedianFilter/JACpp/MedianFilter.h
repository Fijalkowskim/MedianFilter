#pragma once
#include "pch.h"
#include <algorithm>
static unsigned char* FilterBitmapCahnnel(unsigned char* bitmap, int width, int height, int size);
extern "C" __declspec(dllexport) unsigned char* CppMedianFiltering(unsigned char* bitmap, int width, int height)
{
	int size = width * height * 3;
	int channelSize = width * height;
	unsigned char* rBitmap = new unsigned char[channelSize];
	unsigned char* gBitmap = new unsigned char[channelSize];
	unsigned char* bBitmap = new unsigned char[channelSize];
	int r = 0, g = 0, b = 0;
	for (int i = 0; i < size; i++)
	{
		if (i % 3 == 0)
		{
			rBitmap[r] = bitmap[i];
			r++;
		}
		else if (i % 3 == 1)
		{
			gBitmap[g] = bitmap[i];
			g++;
		}
		else
		{
			bBitmap[b] = bitmap[i];
			b++;
		}
	}
	unsigned char* NEWrBitmap = FilterBitmapCahnnel(rBitmap, width, height, channelSize);
	unsigned char* NEWgBitmap = FilterBitmapCahnnel(gBitmap, width, height, channelSize);
	unsigned char* NEWbBitmap = FilterBitmapCahnnel(bBitmap, width, height, channelSize);

	unsigned char* result = new unsigned char[size];

	r = 0;
	g = 0;
	b = 0;
	for (int i = 0; i < size; i++)
	{
		if (i% 3 == 0)
		{
			result[i] = NEWrBitmap[r];
			r++;
		}
		else if (i % 3 == 1)
		{
			result[i] = NEWgBitmap[g];

			g++;
		}
		else
		{
			result[i] = NEWbBitmap[b];

			b++;
		}
	}

	return result;
}

static unsigned char* FilterBitmapCahnnel(unsigned char* bitmap, int width, int height, int size)
{
	unsigned char* result = new unsigned char[size];
	int x = 0, y = 0;
	for (int i = 0; i < size; i++)
	{
		x = i % width;
		if (x == 0 && i != 0) y++;
		if (x == 0 || x == width - 1 || y == 0 || y == height - 1)continue;//Edge handle later
		int i1 = i - width - 1;
		int i2 = i - width;
		int i3 = i - width + 1;
		int i4 = i - 1;
		int i5 = i;
		int i6 = i + 1;
		int i7 = i + width - 1;
		int i8 = i + width;
		int i9 = i + width + 1;
		unsigned char arr[9];
		arr[0] = bitmap[i1];
		arr[1] = bitmap[i2];
		arr[2] = bitmap[i3];
		arr[3] = bitmap[i4];
		arr[4] = bitmap[i5];
		arr[5] = bitmap[i6];
		arr[6] = bitmap[i7];
		arr[7] = bitmap[i8];
		arr[8] = bitmap[i9];
		std::sort(std::begin(arr), std::end(arr));
		result[i] = arr[4];
	}
	return result;
}

