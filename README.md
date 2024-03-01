# Median Filter Project
![image](https://github.com/Fijalkowskim/MedianFilter/assets/91847461/be8d74d2-293e-4887-a857-a1b5dc3c7e9c)
![image](https://github.com/Fijalkowskim/MedianFilter/assets/91847461/e28f1606-246f-47b7-91c7-ffa9ee8c3c7c)
![image](https://github.com/Fijalkowskim/MedianFilter/assets/91847461/c41ba4b9-647a-4403-b900-34ca10d2035b)
![image](https://github.com/Fijalkowskim/MedianFilter/assets/91847461/fdb2291e-a7c9-415e-aa0f-033a973e4c26)

## Introduction
This project implements a median filter for image processing using C# (Windows Forms Application). The filter is applied to an image, and the implementation utilizes two external dynamic-link libraries (DLLs) written in C++ and x64 assembly. The project allows configuring the number of threads used for parallel processing.

## Table of Contents
- [Prerequisites](#prerequisites)
- [Project Structure](#project-structure)
- [DLLs](#dlls)
- [Usage](#usage)

## Prerequisites
Ensure you have the following installed before running the project:
- Visual Studio (for C# development)
- C++ compiler (for compiling C++ DLL)
- Assembler (for assembling x64 assembly code)

## Project Structure
The project consists of the following components:

### C# Code (Fijalkowskim_MedianFilter)
- `DataManager`: Manages the image data, filtering, and threading.
- `DllType`: Enum specifying the type of DLL to use (CPP or ASM).
- `Controller`: Connects data managment with user interface.
- Other helper classes and methods.

### C++ DLL (`JACpp.dll`)
- Provides a C++ implementation of the median filter.

### Assembly x64 DLL (`JAAsm.dll`)
- Provides an x64 assembly implementation of the median filter.

## DLLs
The project uses two DLLs written in C++ and x64 assembly to perform the median filter. The DLLs should be compiled separately using the provided source code.

## Usage
1. Open the solution in Visual Studio.
2. Build the solution to compile the C# code.
3. Compile the C++ DLL (`JACpp.dll`) and the x64 assembly DLL (`JAAsm.dll`) separately.
4. Ensure that the DLLs are in the appropriate debug/release folders.
5. Run the application.

