#pragma once
#include "pch.h"
#include <algorithm>

extern "C" __declspec(dllexport) void CppMedianFilter(unsigned char* stripe, int bitmapWidth, int rows, int startRow, int bitmapHeight)
{
	unsigned char fileredMaskR[9], fileredMaskG[9], fileredMaskB[9];
	bool leftEdge, rightEdge, topEdge, bottomEdge = false;
	const short int indexes[9] = { -bitmapWidth - 3, -bitmapWidth,-bitmapWidth + 3, -3, 0, +3, bitmapWidth - 3, bitmapWidth, bitmapWidth + 3 };
	
	unsigned int pixelIndex = startRow * bitmapWidth;
	for (unsigned short int y = startRow; y < startRow + rows; y++)
	{
		for (unsigned short int x = 0; x < bitmapWidth; x += 3)
		{
			leftEdge = x == 0;
			rightEdge = x == bitmapWidth - 3;
			topEdge = y == 0;
			bottomEdge = y == bitmapHeight - 1;

			fileredMaskR[0] = leftEdge || topEdge ? 0 :		stripe[pixelIndex + indexes[0]];
			fileredMaskR[1] = topEdge ? 0 :					stripe[pixelIndex + indexes[1]];
			fileredMaskR[2] = rightEdge || topEdge ? 0 :	stripe[pixelIndex + indexes[2]];
			fileredMaskR[3] = leftEdge ? 0 :				stripe[pixelIndex + indexes[3]];
			fileredMaskR[4] =								stripe[pixelIndex + indexes[4]];
			fileredMaskR[5] = rightEdge ? 0 :				stripe[pixelIndex + indexes[5]];
			fileredMaskR[6] = leftEdge || bottomEdge ? 0 :	stripe[pixelIndex + indexes[6]];
			fileredMaskR[7] = bottomEdge ? 0 :				stripe[pixelIndex + indexes[7]];
			fileredMaskR[8] = rightEdge || bottomEdge ? 0 : stripe[pixelIndex + indexes[8]];

			fileredMaskG[0] = leftEdge || topEdge ? 0 :		stripe[pixelIndex + indexes[0] + 1];
			fileredMaskG[1] = topEdge ? 0 :					stripe[pixelIndex + indexes[1] + 1];
			fileredMaskG[2] = rightEdge || topEdge ? 0 :	stripe[pixelIndex + indexes[2] + 1];
			fileredMaskG[3] = leftEdge ? 0 :				stripe[pixelIndex + indexes[3] + 1];
			fileredMaskG[4] =								stripe[pixelIndex + indexes[4] + 1];
			fileredMaskG[5] = rightEdge ? 0 :				stripe[pixelIndex + indexes[5] + 1];
			fileredMaskG[6] = leftEdge || bottomEdge ? 0 :	stripe[pixelIndex + indexes[6] + 1];
			fileredMaskG[7] = bottomEdge ? 0 :				stripe[pixelIndex + indexes[7] + 1];
			fileredMaskG[8] = rightEdge || bottomEdge ? 0 : stripe[pixelIndex + indexes[8] + 1];

			fileredMaskB[0] = leftEdge || topEdge ? 0 :		stripe[pixelIndex + indexes[0] + 2];
			fileredMaskB[1] = topEdge ? 0 :					stripe[pixelIndex + indexes[1] + 2];
			fileredMaskB[2] = rightEdge || topEdge ? 0 :	stripe[pixelIndex + indexes[2] + 2];
			fileredMaskB[3] = leftEdge ? 0 :				stripe[pixelIndex + indexes[3] + 2];
			fileredMaskB[4] =								stripe[pixelIndex + indexes[4] + 2];
			fileredMaskB[5] = rightEdge ? 0 :				stripe[pixelIndex + indexes[5] + 2];
			fileredMaskB[6] = leftEdge || bottomEdge ? 0 :	stripe[pixelIndex + indexes[6] + 2];
			fileredMaskB[7] = bottomEdge ? 0 :				stripe[pixelIndex + indexes[7] + 2];
			fileredMaskB[8] = rightEdge || bottomEdge ? 0 : stripe[pixelIndex + indexes[8] + 2];
		
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


