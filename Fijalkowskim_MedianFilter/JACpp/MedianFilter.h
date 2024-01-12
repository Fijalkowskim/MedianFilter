#pragma once
#include "pch.h"
#include <algorithm>

extern "C" __declspec(dllexport) void CppMedianFilter(unsigned char* stripe, int bitmapWidth, int rows, int startRow, int bitmapHeight)
{
	int indexes[9];
	unsigned char fileredMaskR[9];
	unsigned char fileredMaskG[9];
	unsigned char fileredMaskB[9];
	
	int pixelIndex = startRow * bitmapWidth;
	for (int y = startRow; y < startRow + rows; y++)
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
			{   //  i =
				// 0 1 2
				// 3 4 5
				// 6 7 8
				bool leftEdge = x == 0 && (i == 0 || i == 3 || i == 6);
				bool rightEdge = x == bitmapWidth - 3 && (i == 2 || i == 5 || i == 8);
				bool topBottomEdge = (i < 3 && y == 0) || (i > 5 && y == bitmapHeight - 1);
				if (topBottomEdge) {
					indexes[0] = indexes[0];
				}

				fileredMaskR[i] = leftEdge || rightEdge || topBottomEdge ? 0 : stripe[indexes[i]];
				fileredMaskG[i] = leftEdge || rightEdge || topBottomEdge ? 0 : stripe[indexes[i]+ 1];
				fileredMaskB[i] = leftEdge || rightEdge || topBottomEdge ? 0 : stripe[indexes[i] + 2];
			}
			std::sort(std::begin(fileredMaskR), std::end(fileredMaskR));
			std::sort(std::begin(fileredMaskG), std::end(fileredMaskG));
			std::sort(std::begin(fileredMaskB), std::end(fileredMaskB));
			stripe[pixelIndex] = fileredMaskR[4];
			stripe[pixelIndex+1] = fileredMaskG[4];
			stripe[pixelIndex+2] = fileredMaskB[4];

			pixelIndex+=3;
		}
	}
}


