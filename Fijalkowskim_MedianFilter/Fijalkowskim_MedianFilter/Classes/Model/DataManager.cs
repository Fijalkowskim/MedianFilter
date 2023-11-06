using System.Runtime.InteropServices;
using System.Diagnostics;
using System;
using System.Threading;
using System.Drawing;
using System.Numerics;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Collections;

namespace Fijalkowskim_MedianFilter
{  
    public enum DllType { CPP, ASM}
    public class DataManager
    {
#if DEBUG
        [DllImport(@"D:\1 Studia\JA\MedianFilter\Fijalkowskim_MedianFilter\x64\Debug\JAAsm.dll")]
        static extern byte AsmMedianFilter(byte pixel);


        [DllImport(@"D:\1 Studia\JA\MedianFilter\Fijalkowskim_MedianFilter\x64\Debug\JACpp.dll")]
        static extern IntPtr CppMedianFiltering(IntPtr bitmap, int width, int height);

#else
 [DllImport(@"D:\1 Studia\JA\MedianFilter\Fijalkowskim_MedianFilter\x64\Release\JAAsm.dll")]
        static extern int MyProc1(int a, int b);

        [DllImport(@"D:\1 Studia\JA\MedianFilter\Fijalkowskim_MedianFilter\x64\Release\JACpp.dll")]
        static extern int CppFunc(int a, int b);
#endif

        public TimeSpan currentExecutionTime { get; private set; }
        public TimeSpan previousExecutionTime { get; private set; }
        public bool dataLoaded { get; private set; }
        Stopwatch stopwatch;
        public Bitmap bitmap { get; private set; }
        byte[,] expandedBitmapR, expandedBitmapG, expandedBitmapB;
        byte[] bitmap1DArray;
        int bitmapRGBSize;
        int colorChannelSize;
        int numberOfTasks;
        TaskData[] tasksData;
        int[] taskRanges;

        static readonly object arrayLock = new object();

        Controller controller;

        public DataManager(Controller controller)
        {
            this.controller = controller;
            currentExecutionTime = TimeSpan.Zero;
            previousExecutionTime = TimeSpan.Zero;
            stopwatch = new Stopwatch();
            bitmap = null;
            dataLoaded = false;
        }

        public async Task LoadBitmap(Bitmap bitmap, int numberOfTasks)
        {        
            stopwatch.Restart();
            stopwatch.Start();

            bitmapRGBSize = bitmap.Width * bitmap.Height * 3;
            colorChannelSize = bitmap.Width * bitmap.Height;
            this.numberOfTasks = Math.Min(numberOfTasks, bitmapRGBSize);
            this.bitmap = bitmap;
            //CalculateExpandedBitmap();
            SetUpTasks();
            CalculateTaskData();
            LoadBitmapStripe();

            Bitmap bmp = BitmapFromTaskData(tasksData[0]);
            controller.mainMenu.SetFilteredBitmap(bmp);

           // bitmap1DArray = await Task.Run(()=> ArrayFromBitmap(bitmap));

            stopwatch.Stop();
            controller.mainMenu.SetExecutionTime(stopwatch.Elapsed.ToString(), "");

            dataLoaded = true;
        }
        void SetUpTasks()
        {
            tasksData = new TaskData[numberOfTasks];
            taskRanges = new int[numberOfTasks];
            int taskBitmapRange = colorChannelSize / numberOfTasks;
            int undvidedRange = colorChannelSize - numberOfTasks * taskBitmapRange;
            for (int i = 0; i < numberOfTasks; i++)
            {
                taskRanges[i] = taskBitmapRange;
                tasksData[i] = new TaskData();
                tasksData[i].bitmapStripeR = new List<byte>();
                tasksData[i].bitmapStripeG = new List<byte>();
                tasksData[i].bitmapStripeB = new List<byte>();
                if (undvidedRange != 0)
                {
                    taskRanges[i]++;
                    undvidedRange--;
                }
            }
            
        }
        void CalculateTaskData()
        {
            int currentIndex = 0;
            int endIndex;
            for (int i = 0; i < numberOfTasks; i++)
            {
                endIndex = currentIndex + taskRanges[i];

                int length = endIndex - currentIndex;

                int startX = currentIndex % bitmap.Width;
                int startY = currentIndex / bitmap.Width;
                tasksData[i].startPos = new Vector2(startX, startY);

                int count = 0;
                for (int y = startY; y < bitmap.Height; y++)
                {
                    for (int x = y == startY ? startX : 0; x < bitmap.Width; x++)
                    {
                        count++;

                        if (count == length)
                        {
                            tasksData[i].endPos = new Vector2(x, y);
                            y = bitmap.Height;
                            break;
                        }
                    }
                }
                currentIndex = endIndex;
            }
        }
        void LoadBitmapStripe()
        {
            int currThread = 0;
            Vector2 startPos, endPos;
            endPos = new Vector2(tasksData[currThread].endPos.X + 1 + 1, tasksData[currThread].endPos.Y + 1 + 1);

            for (int y = 0; y < bitmap.Height + 2; y++)
            {
                for (int x = 0; x < bitmap.Width + 2; x++)
                {
                    tasksData[currThread].bitmapStripeR.Add(expandedBitmapR[x, y]);
                    tasksData[currThread].bitmapStripeG.Add(expandedBitmapG[x, y]);
                    tasksData[currThread].bitmapStripeB.Add(expandedBitmapB[x, y]);
                    if (x == endPos.X && y == endPos.Y)
                    {
                        currThread++;
                        if (currThread == numberOfTasks)
                            return;
                        startPos = new Vector2(tasksData[currThread].startPos.X + 1 - 1, tasksData[currThread].startPos.Y + 1 - 1);
                        endPos = new Vector2(tasksData[currThread].endPos.X + 1 + 1, tasksData[currThread].endPos.Y + 1 + 1);
                        y = (int)startPos.Y;
                        x = (int)startPos.X;
                    }
                }
            }
            
        }
        Bitmap BitmapFromTaskData(TaskData taskData)
        {
            Bitmap bmp = new Bitmap(bitmap.Width, bitmap.Height);
            int idx = 0;
            for (int y = 0; y < bitmap.Height; y++)
            {
                for (int x = 0; x < bitmap.Width; x++)
                {
                    if(x >= taskData.startPos.X && x <= taskData.endPos.X && y >= taskData.startPos.Y && y <= taskData.endPos.Y)
                    {
                        bmp.SetPixel(x,y,Color.FromArgb(taskData.bitmapStripeR[idx], taskData.bitmapStripeG[idx], taskData.bitmapStripeB[idx]));
                        idx++;
                    }
                    else
                    {
                        bmp.SetPixel(x, y, Color.FromArgb(0, 0, 0));
                    }
                }
            }
            return bmp;
        }
        void CalculateExpandedBitmap()
        {
            expandedBitmapR = new byte[bitmap.Width + 2, bitmap.Height + 2];
            expandedBitmapG = new byte[bitmap.Width + 2, bitmap.Height + 2];
            expandedBitmapB = new byte[bitmap.Width + 2, bitmap.Height + 2];
            for (int y = 0; y < bitmap.Height + 2; y++)
            {
                for (int x = 0; x < bitmap.Width + 2; x++)
                {
                    if (x == 0 || y == 0 || x == bitmap.Width + 1 || y == bitmap.Height + 1)
                    {
                        expandedBitmapR[x, y] = 0;
                        expandedBitmapG[x, y] = 0;
                        expandedBitmapB[x, y] = 0;
                    }
                    else
                    {
                        expandedBitmapR[x, y] = bitmap.GetPixel(x - 1, y - 1).R;
                        expandedBitmapG[x, y] = bitmap.GetPixel(x - 1, y - 1).G;
                        expandedBitmapB[x, y] = bitmap.GetPixel(x - 1, y - 1).B;
                    }
                }
            }
        }
        public async Task LoadBitmapAsyncV3(Bitmap bitmap, int numberOfThreads, IProgress<ImageLoadingProgress> progress)
        {
            dataLoaded = false;
            stopwatch.Restart();
            stopwatch.Start();

            ImageLoadingProgress report = new ImageLoadingProgress();

            bitmapRGBSize = bitmap.Width * bitmap.Height * 3;
            colorChannelSize = bitmap.Width * bitmap.Height;

            this.numberOfTasks = numberOfThreads;
            int taskBitmapRange = colorChannelSize / numberOfThreads;

            bitmap1DArray = new byte[bitmapRGBSize];

            int currentIndex = 0;
            int endIndex;
            int pasteFromIndex = 0;
            for (int i = 0; i < numberOfThreads; i++)
            {
                endIndex = i == numberOfThreads - 1 ? colorChannelSize : currentIndex + taskBitmapRange;
                BitmapRGBArrayPart RGBArray = await Task.Run(() => ArrayFromBitmapAsync(bitmap, currentIndex, endIndex, pasteFromIndex, i));
                Array.Copy(RGBArray.arr, 0, bitmap1DArray, RGBArray.pasteIndex, RGBArray.arr.Length);
                currentIndex = endIndex;
                pasteFromIndex += taskBitmapRange * 3;

                report.percentageDone = (i + 1) * 100 / numberOfThreads;
                progress.Report(report);
            }
            stopwatch.Stop();
            controller.mainMenu.SetExecutionTime(stopwatch.Elapsed.ToString(), "");

            this.bitmap = bitmap;
            dataLoaded = true;

        }

        public BitmapRGBArrayPart ArrayFromBitmapAsync(Bitmap bitmap, int startIndex, int endIndex, int pasteFromIndex, int taskNo)
        {
            if (startIndex >= endIndex) return null;

            int length = endIndex - startIndex;
            byte[] arr = new byte[length * 3];
            int startX = startIndex % bitmap.Width;
            int startY = startIndex / bitmap.Width;

            int pixel = 0;
            int count = 0;
            for (int y = startY; y < bitmap.Height; y++)
            {
                for (int x = startX; x < bitmap.Width; x++)
                {
                    arr[pixel] = bitmap.GetPixel(x, y).R;
                    arr[pixel + 1] = bitmap.GetPixel(x, y).G;
                    arr[pixel + 2] = bitmap.GetPixel(x, y).B;
                    pixel += 3;
                    count++;

                    if (count == length)
                    {
                        return new BitmapRGBArrayPart(arr, pasteFromIndex);
                    }
                }
            }
            return new BitmapRGBArrayPart(arr, pasteFromIndex);

        }

        public async Task<Bitmap> UseMedianFilter(DllType dllType,IProgress<ImageLoadingProgress> progress)
        {
           

            if (bitmap == null || bitmap1DArray == null) return null;
            Bitmap result = new Bitmap(bitmap.Width, bitmap.Height);
            stopwatch.Reset();
            ImageLoadingProgress report = new ImageLoadingProgress();
            switch (dllType)
            {
                case DllType.CPP:
                    IntPtr unmanagedPointer = Marshal.AllocHGlobal(bitmapRGBSize);
                    Marshal.Copy(bitmap1DArray, 0, unmanagedPointer, bitmapRGBSize);
                    IntPtr resultPtr = IntPtr.Zero;
                    byte[] resultArray = new byte[bitmap1DArray.Length];

                    stopwatch.Start();
                    resultPtr = CppMedianFiltering(unmanagedPointer, bitmap.Width, bitmap.Height);
                    stopwatch.Stop();
                    Marshal.Copy(resultPtr, resultArray, 0, resultArray.Length);

                    Marshal.FreeHGlobal(unmanagedPointer);
   
                    result = BitmapFromArray(resultArray, bitmap.Width, bitmap.Height);
                    break;
                case DllType.ASM:
                    stopwatch.Start();
                    byte[] resultArr = new byte[bitmapRGBSize];
                    //AsmMedianFilter(loadedBitmapArray, RGBbitmapArraySize);
                    for (int i = 0; i < bitmapRGBSize; i++)
                    {
                        await Task.Run(() =>
                        {
                            resultArr[i] = AsmMedianFilter(bitmap1DArray[i]);
                        });
                        report.percentageDone = (i + 1) * 100 / bitmapRGBSize;
                        progress.Report(report);
                    }

                    //AsmMedianFilter(loadedBitmapArray, RGBbitmapArraySize);
                    result = BitmapFromArray(resultArr, bitmap.Width, bitmap.Height);
                    stopwatch.Stop();
                    break;
            }
            if (currentExecutionTime != TimeSpan.Zero)
                previousExecutionTime = currentExecutionTime;
            currentExecutionTime = stopwatch.Elapsed;

            return result;
        }
        #region Conversion methods
        public byte[] PointerToArray(IntPtr ptr, int size)
        {
            byte[] arr = new byte[size];
            Marshal.Copy(ptr, arr, 0, size);
            return arr;
        }
        public byte[] ArrayFromBitmap(Bitmap bitmap)
        {
            byte[] arr = new byte[bitmap.Width * bitmap.Height * 3];
            int pixel = 0;
            for (int y = 0; y < bitmap.Height; y++)
            {
                for (int x = 0; x < bitmap.Width; x++)
                {
                    arr[pixel] = bitmap.GetPixel(x, y).R;
                    arr[pixel + 1] = bitmap.GetPixel(x, y).G;
                    arr[pixel + 2] = bitmap.GetPixel(x, y).B;
                    pixel += 3;
                }
            }
            return arr;
        }
       
        public Bitmap BitmapFromArray(byte[] arr, int width, int height)
        {
            Bitmap bitmap = new Bitmap(width, height);
            int pixel = 0;
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    bitmap.SetPixel(x, y, Color.FromArgb(arr[pixel], arr[pixel + 1], arr[pixel + 2]));
                    pixel += 3;
                }
            }
            return bitmap;
        }
        #endregion
    }
}
