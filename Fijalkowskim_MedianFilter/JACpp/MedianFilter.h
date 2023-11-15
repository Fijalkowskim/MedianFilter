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
		int indexes[9];
		indexes[0] = i - width - 1;
		indexes[1] = i - width;
		indexes[2] = i - width + 1;
		indexes[3] = i - 1;
		indexes[4] = i;
		indexes[5] = i + 1;
		indexes[6] = i + width - 1;
		indexes[7] = i + width;
		indexes[8] = i + width + 1;
		unsigned char fileredMask[9];
		for (size_t i = 0; i < 9; i++)
		{
			fileredMask[i] = bitmap[indexes[i]];
		}
		std::sort(std::begin(fileredMask), std::end(fileredMask));
		result[i] = fileredMask[4];
	}
	return result;
}
extern "C" __declspec(dllexport) unsigned char* FilterBitmapStripe(unsigned char* stripe, int bitmapWidth, int rows, int startRow)
{
	unsigned char* result = new unsigned char[rows * bitmapWidth];
	int indexes[9];
	unsigned char fileredMask[9];
	int resultIndex = 0;
	int stripeIndex = bitmapWidth;
	for (int y = 0; y < rows; y++)
	{
		for (int x = 0; x < bitmapWidth; x++)
		{
			indexes[0] = stripeIndex - bitmapWidth - 1;
			indexes[1] = stripeIndex - bitmapWidth;
			indexes[2] = stripeIndex - bitmapWidth + 1;
			indexes[3] = stripeIndex - 1;
			indexes[4] = stripeIndex;
			indexes[5] = stripeIndex + 1;
			indexes[6] = stripeIndex + bitmapWidth - 1;
			indexes[7] = stripeIndex + bitmapWidth;
			indexes[8] = stripeIndex + bitmapWidth + 1;

			for (size_t i = 0; i < 9; i++)
			{
				bool leftEdge = x == 0 && (i == 0 || i == 3 || i == 6);
				bool rightEdge = x == bitmapWidth - 1 && (i == 2 || i == 5 || i == 8);

				fileredMask[i] = leftEdge || rightEdge ? 0 : stripe[indexes[i]];
			}
			std::sort(std::begin(fileredMask), std::end(fileredMask));
			result[resultIndex] = fileredMask[4];

			stripeIndex++;
			resultIndex++;
		}
	}

	return result;
}
