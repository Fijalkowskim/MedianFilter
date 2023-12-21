# Median Filter Project
![image](https://github.com/Fijalkowskim/MedianFilter/assets/91847461/866a3804-7bee-4182-9e89-998d80ade8e1)

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

