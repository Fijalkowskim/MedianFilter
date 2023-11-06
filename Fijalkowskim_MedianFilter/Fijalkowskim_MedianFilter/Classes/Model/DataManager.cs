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
        [DllImport(@"D:\1 Studia\JA\MedianFilter\Fijalkowskim_MedianFilter\x64\Debug\JACpp.dll")]
        static extern IntPtr FilterBitmapStripe(IntPtr stripe, int bitmapWidth, int rows, int startRow);

#else
 [DllImport(@"D:\1 Studia\JA\MedianFilter\Fijalkowskim_MedianFilter\x64\Release\JAAsm.dll")]
        static extern int MyProc1(int a, int b);

        [DllImport(@"D:\1 Studia\JA\MedianFilter\Fijalkowskim_MedianFilter\x64\Release\JACpp.dll")]
        static extern int CppFunc(int a, int b);
#endif

        public long currentExecutionTime { get; private set; }
        public long previousExecutionTime { get; private set; }
        public bool dataLoaded { get; private set; }
        Stopwatch stopwatch;
        public Bitmap loadedBitmap { get; private set; }
        int bitmapWidth, bitmapHeight;
        int bitmapRGBSize;
        int colorChannelSize;
        int numberOfTasks;
        TaskData[] tasksData;
        private CancellationTokenSource cancellationTokenSource;
        static readonly object arrayLock = new object();
        public bool applyingFilter { get; private set; }

        Controller controller;

        public DataManager(Controller controller)
        {
            this.controller = controller;
            currentExecutionTime = -1;
            previousExecutionTime = -1;
            stopwatch = new Stopwatch();
            loadedBitmap = null;
            dataLoaded = false;
            applyingFilter = false;
        }

        public void LoadBitmap(Bitmap bitmap)
        {        
            bitmapRGBSize = bitmap.Width * bitmap.Height * 3;
            colorChannelSize = bitmap.Width * bitmap.Height;
            this.loadedBitmap = bitmap;
            bitmapHeight = bitmap.Height;
            bitmapWidth = bitmap.Width;
            ImageLoadingProgress report = new ImageLoadingProgress();
            dataLoaded = true;
            
        }
        BitmapStripeResult ApplyMedianFilterCpp(ref Bitmap bmp, TaskData taskData, int width, int height)
        {
            int stripeLength = taskData.bitmapStripeR.Count;

            IntPtr ptrR = Marshal.AllocHGlobal(stripeLength);
            IntPtr ptrG = Marshal.AllocHGlobal(stripeLength);
            IntPtr ptrB = Marshal.AllocHGlobal(stripeLength);
            Marshal.Copy(taskData.bitmapStripeR.ToArray(), 0, ptrR, stripeLength);
            Marshal.Copy(taskData.bitmapStripeG.ToArray(), 0, ptrG, stripeLength);
            Marshal.Copy(taskData.bitmapStripeB.ToArray(), 0, ptrB, stripeLength);
            IntPtr resultRptr = IntPtr.Zero;
            IntPtr resultGptr = IntPtr.Zero;
            IntPtr resultBptr = IntPtr.Zero;

            byte[] resultArrayR = new byte[stripeLength];
            byte[] resultArrayG = new byte[stripeLength];
            byte[] resultArrayB = new byte[stripeLength];

            resultRptr = FilterBitmapStripe(ptrR, width, taskData.rows, taskData.startRow);
            resultGptr = FilterBitmapStripe(ptrG, width, taskData.rows, taskData.startRow);
            resultBptr = FilterBitmapStripe(ptrB, width, taskData.rows, taskData.startRow);
            try
            {
                Marshal.Copy(resultRptr, resultArrayR, 0, resultArrayR.Length);
                Marshal.Copy(resultGptr, resultArrayG, 0, resultArrayG.Length);
                Marshal.Copy(resultBptr, resultArrayB, 0, resultArrayB.Length);
            }
            catch(Exception e)
            {

            }
            

            Marshal.FreeHGlobal(ptrR);
            Marshal.FreeHGlobal(ptrG);
            Marshal.FreeHGlobal(ptrB);

            return new BitmapStripeResult(resultArrayR, resultArrayG, resultArrayB, taskData.startRow, taskData.rows);
            //controller.mainMenu.SetFilteredBitmap(bmp);
            
        }       
        void SetBitmapStripe(ref Bitmap bmp, byte[] R, byte[] G, byte[] B, int startRow, int rows)
        {
            int idx = 0;
            for (int y = startRow; y < startRow + rows; y++)
            {
                for (int x = 0; x < bitmapWidth; x++)
                {
                    lock (arrayLock)
                    {
                        bmp.SetPixel(x, y, Color.FromArgb(R[idx], G[idx], B[idx]));
                    }
                    idx++;
                }
            }
        }
        void SetUpTasks(int numberOfTasks)
        {
            this.numberOfTasks = Math.Min(numberOfTasks, bitmapHeight);

            tasksData = new TaskData[numberOfTasks];
            int taskBitmapRange = bitmapHeight / numberOfTasks;
            int undvidedRange = bitmapHeight - numberOfTasks * taskBitmapRange;
            for (int i = 0; i < numberOfTasks; i++)
            {
                tasksData[i] = new TaskData();
                tasksData[i].rows = taskBitmapRange;
                tasksData[i].bitmapStripeR = new List<byte>();
                tasksData[i].bitmapStripeG = new List<byte>();
                tasksData[i].bitmapStripeB = new List<byte>();
                if (undvidedRange != 0)
                {
                    tasksData[i].rows++;
                    undvidedRange--;
                }
            }
            
        }
        void CalculateTaskData()
        {
            int startY = 0;
            int rows = 0;
            for (int i = 0; i < numberOfTasks; i++)
            {
                rows = tasksData[i].rows;
                tasksData[i].startRow = startY;
                for (int y = startY - 1; y < startY + rows + 1; y++)
                {
                    if (y < 0 || y >= bitmapHeight)
                    {
                        for (int x = 0; x < bitmapWidth; x++)
                        {
                            tasksData[i].bitmapStripeR.Add(0);
                            tasksData[i].bitmapStripeG.Add(0);
                            tasksData[i].bitmapStripeB.Add(0);
                        }
                    }
                    else
                    {
                        for (int x = 0; x < bitmapWidth; x++)
                        {
                            tasksData[i].bitmapStripeR.Add(loadedBitmap.GetPixel(x,y).R);
                            tasksData[i].bitmapStripeG.Add(loadedBitmap.GetPixel(x,y).G);
                            tasksData[i].bitmapStripeB.Add(loadedBitmap.GetPixel(x,y).B);
                        }
                    }
                }
                startY += rows;
            }
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

        public async Task<Bitmap> UseMedianFilter(DllType dllType, int numberOfTasks, IProgress<ImageLoadingProgress> progress)
        {
            if (applyingFilter || loadedBitmap == null) return null;
            applyingFilter = true;
            if (cancellationTokenSource != null)
            {
                cancellationTokenSource.Cancel();
                cancellationTokenSource.Dispose();
            }
            cancellationTokenSource = new CancellationTokenSource();
            SetUpTasks(numberOfTasks);
            CalculateTaskData();

            List<Task< BitmapStripeResult >> tasks = new List<Task< BitmapStripeResult >>();
            Bitmap result = new Bitmap(bitmapWidth, bitmapHeight);
            ImageLoadingProgress report = new ImageLoadingProgress();
            stopwatch.Reset();
            stopwatch.Start();
            switch (dllType)
            {
                case DllType.CPP:
                    BitmapStripeResult[] tasksResults = new BitmapStripeResult[numberOfTasks];
                    for (int i = 0; i < numberOfTasks; i++)
                    {
                        int taskIndex = i;
                        tasks.Add(Task.Run(() => {
                            return ApplyMedianFilterCpp(ref result, tasksData[taskIndex], bitmapWidth, bitmapHeight);
                            
                        },cancellationTokenSource.Token));
                    }
                    await Task.WhenAll(tasks);
                    for (int i = 0; i < tasks.Count; i++)
                    {
                        report.percentageDone = (i + 1) * 100 / bitmapRGBSize;
                        progress.Report(report);
                        if (result == null)
                            continue;
                        tasksResults[i] = tasks[i].Result;

                        SetBitmapStripe(ref result, tasksResults[i].resultArrayR, tasksResults[i].resultArrayG, tasksResults[i].resultArrayB,
                            tasksResults[i].startRow, tasksResults[i].rows);
                    }
                   


                    break;
                case DllType.ASM:
                    byte[] resultArr = new byte[bitmapRGBSize];
                    //AsmMedianFilter(loadedBitmapArray, RGBbitmapArraySize);
                    for (int i = 0; i < bitmapRGBSize; i++)
                    {
                        await Task.Run(() =>
                        {
                            //resultArr[i] = AsmMedianFilter(bitmap1DArray[i]);
                        });
                        report.percentageDone = (i + 1) * 100 / bitmapRGBSize;
                        progress.Report(report);
                    }

                    //AsmMedianFilter(loadedBitmapArray, RGBbitmapArraySize);
                    result = BitmapFromArray(resultArr, bitmapWidth, bitmapHeight);
                    
                    break;
            }
            stopwatch.Stop();
            if (currentExecutionTime != -1)
                previousExecutionTime = currentExecutionTime;
            currentExecutionTime = stopwatch.ElapsedMilliseconds;
            controller.mainMenu.SetExecutionTime(currentExecutionTime.ToString(), previousExecutionTime < 0 ? "" : previousExecutionTime.ToString());
            applyingFilter = false;
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
