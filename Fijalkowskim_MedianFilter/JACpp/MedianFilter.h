// Mateusz Fija³kowski
// Median Filter v1 - 14.01.2024
// Silesian University of Technology 2023/24
#pragma once
#include "pch.h"
#include <algorithm>

//Method applying median filtering directly onto given bitmap pointer
// 
//Median filtering - The filtering algorithm involves determining the median (middle value) of the values of neighboring pixels in a mask of a selected size (in my case the mask is 3x3).
//After calculating the median, its value is saved to the currently processed pixel. If a pixel is on an edge, its neighbors are assumed to have a value of 0.
//R, G and B channels are filtered separately.
// 
//bitmap - Pointer to original bitmap
//bitmapWidth - Width of bitmap taking into account rgb channels (pixels width * 3)
//rows - Rows to filter (from task data)
//startRow - Row of bitmap to start filtering (from task data)

extern "C" __declspec(dllexport) void CppMedianFilter(unsigned char* bitmap, int bitmapWidth, int rows, int startRow, int bitmapHeight)
{
	//9 element arays of neighbour pixels for calculating median
	unsigned char fileredMaskR[9], fileredMaskG[9], fileredMaskB[9];
	//Boolean values for edge detection
	bool leftEdge, rightEdge, topEdge, bottomEdge = false;
	//Indexes of 9 neighbour pixels at 3x3 mask
	// 0 1 2
	// 3 4 5
	// 6 7 8
	const short int indexes[9] = { -bitmapWidth - 3, -bitmapWidth,-bitmapWidth + 3, -3, 0, +3, bitmapWidth - 3, bitmapWidth, bitmapWidth + 3 };
	//Initialize starting pixel index
	unsigned int pixelIndex = startRow * bitmapWidth;
	//Loops going over all rgb channels values
	for (unsigned short int y = startRow; y < startRow + rows; y++)
	{
		for (unsigned short int x = 0; x < bitmapWidth; x += 3)
		{
			//Edge detection
			leftEdge = x == 0;
			rightEdge = x == bitmapWidth - 3;
			topEdge = y == 0;
			bottomEdge = y == bitmapHeight - 1;
			//Set 3x3 masks for each color channel. If current pixel is on the edge set 0 instead of not existing neighbours.
			//Red channel
			fileredMaskR[0] = leftEdge || topEdge ? 0 :		bitmap[pixelIndex + indexes[0]]; //Top left
			fileredMaskR[1] = topEdge ? 0 :					bitmap[pixelIndex + indexes[1]]; //Top center
			fileredMaskR[2] = rightEdge || topEdge ? 0 :	bitmap[pixelIndex + indexes[2]]; //Top right
			fileredMaskR[3] = leftEdge ? 0 :				bitmap[pixelIndex + indexes[3]]; //Middle left
			fileredMaskR[4] =								bitmap[pixelIndex + indexes[4]]; //Middle center
			fileredMaskR[5] = rightEdge ? 0 :				bitmap[pixelIndex + indexes[5]]; //Middle right
			fileredMaskR[6] = leftEdge || bottomEdge ? 0 :	bitmap[pixelIndex + indexes[6]]; //Bottom left
			fileredMaskR[7] = bottomEdge ? 0 :				bitmap[pixelIndex + indexes[7]]; //Bottom center
			fileredMaskR[8] = rightEdge || bottomEdge ? 0 : bitmap[pixelIndex + indexes[8]]; //Bottom right
			//Green channel
			fileredMaskG[0] = leftEdge || topEdge ? 0 :		bitmap[pixelIndex + indexes[0] + 1]; //Top left
			fileredMaskG[1] = topEdge ? 0 :					bitmap[pixelIndex + indexes[1] + 1]; //Top center
			fileredMaskG[2] = rightEdge || topEdge ? 0 :	bitmap[pixelIndex + indexes[2] + 1]; //Top right
			fileredMaskG[3] = leftEdge ? 0 :				bitmap[pixelIndex + indexes[3] + 1]; //Middle left
			fileredMaskG[4] =								bitmap[pixelIndex + indexes[4] + 1]; //Middle center
			fileredMaskG[5] = rightEdge ? 0 :				bitmap[pixelIndex + indexes[5] + 1]; //Middle right
			fileredMaskG[6] = leftEdge || bottomEdge ? 0 :	bitmap[pixelIndex + indexes[6] + 1]; //Bottom left
			fileredMaskG[7] = bottomEdge ? 0 :				bitmap[pixelIndex + indexes[7] + 1]; //Bottom center
			fileredMaskG[8] = rightEdge || bottomEdge ? 0 : bitmap[pixelIndex + indexes[8] + 1]; //Bottom right
			//Blue channel
			fileredMaskB[0] = leftEdge || topEdge ? 0 :		bitmap[pixelIndex + indexes[0] + 2]; //Top left
			fileredMaskB[1] = topEdge ? 0 :					bitmap[pixelIndex + indexes[1] + 2]; //Top center
			fileredMaskB[2] = rightEdge || topEdge ? 0 :	bitmap[pixelIndex + indexes[2] + 2]; //Top right
			fileredMaskB[3] = leftEdge ? 0 :				bitmap[pixelIndex + indexes[3] + 2]; //Middle left
			fileredMaskB[4] =								bitmap[pixelIndex + indexes[4] + 2]; //Middle center
			fileredMaskB[5] = rightEdge ? 0 :				bitmap[pixelIndex + indexes[5] + 2]; //Middle right
			fileredMaskB[6] = leftEdge || bottomEdge ? 0 :	bitmap[pixelIndex + indexes[6] + 2]; //Bottom left
			fileredMaskB[7] = bottomEdge ? 0 :				bitmap[pixelIndex + indexes[7] + 2]; //Bottom center
			fileredMaskB[8] = rightEdge || bottomEdge ? 0 : bitmap[pixelIndex + indexes[8] + 2]; //Bottom right
			//Sort all arrays 
			std::sort(std::begin(fileredMaskR), std::end(fileredMaskR));
			std::sort(std::begin(fileredMaskG), std::end(fileredMaskG));
			std::sort(std::begin(fileredMaskB), std::end(fileredMaskB));
			//Set median (4th element) as new pixel value
			bitmap[pixelIndex] = fileredMaskR[4];
			bitmap[pixelIndex+1] = fileredMaskG[4];
			bitmap[pixelIndex+2] = fileredMaskB[4];
			//Increment pixelIndex by 3 (R,G,B)
			pixelIndex+=3;
		}
	}
}


