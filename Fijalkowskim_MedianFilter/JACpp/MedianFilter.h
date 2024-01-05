#pragma once
#include "pch.h"
#include <algorithm>

extern "C" __declspec(dllexport) unsigned char* FilterBitmapStripe(unsigned char* stripe, int bitmapWidth, int rows)
{
	int resultSize = rows * bitmapWidth;
	unsigned char* result = new unsigned char[resultSize];
	int indexes[9];
	unsigned char fileredMaskR[9];
	unsigned char fileredMaskG[9];
	unsigned char fileredMaskB[9];
	
	int resultIndex = 0;
	int pixelIndex = bitmapWidth;
	for (int y = 0; y < rows; y++)
	{
		for (int x = 0; x < bitmapWidth; x += 3)
		{
			indexes[0] = pixelIndex - bitmapWidth - 3;
			indexes[1] = pixelIndex - bitmapWidth;
			indexes[2] = pixelIndex - bitmapWidth + 3;
			indexes[3] = pixelIndex - 3;
			indexes[4] = pixelIndex;
			indexes[5] = pixelIndex + 3;
			indexes[6] = pixelIndex + bitmapWidth - 3;
			indexes[7] = pixelIndex + bitmapWidth;
			indexes[8] = pixelIndex + bitmapWidth + 3;

			for (size_t i = 0; i < 9; i++)
			{   // 0 1 2
				// X - -
				// 3 4 5
				// X - -
				// 6 7 8
				// X - -
				bool leftEdge = x == 0 && (i == 0 || i == 3 || i == 6);
				// 0 1 2
				// - - X
				// 3 4 5
				// - - X
				// 6 7 8
				// - - X
				bool rightEdge = x == bitmapWidth - 3 && (i == 2 || i == 5 || i == 8);

				fileredMaskR[i] = leftEdge || rightEdge ? 0 : stripe[indexes[i]];
				fileredMaskG[i] = leftEdge || rightEdge ? 0 : stripe[indexes[i]+ 1];
				fileredMaskB[i] = leftEdge || rightEdge ? 0 : stripe[indexes[i] + 2];
			}
			std::sort(std::begin(fileredMaskR), std::end(fileredMaskR));
			std::sort(std::begin(fileredMaskG), std::end(fileredMaskG));
			std::sort(std::begin(fileredMaskB), std::end(fileredMaskB));
			result[resultIndex] =		fileredMaskR[4];
			result[resultIndex + 1] =	fileredMaskG[4];
			result[resultIndex + 2] =	fileredMaskB[4];

			pixelIndex+=3;
			resultIndex += 3;
		}
	}
	return result;
}


