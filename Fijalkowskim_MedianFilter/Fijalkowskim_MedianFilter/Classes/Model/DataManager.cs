using System.Runtime.InteropServices;
using System.Diagnostics;
using System;
using System.Threading;
using System.Drawing;
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
        public Bitmap loadedBitmap { get; private set; }
        byte[] loadedBitmapArray;
        int RGBbitmapArraySize;
        int bitmapSize;
        int numberOfThreads;

        static readonly object arrayLock = new object();
        private readonly SemaphoreSlim semaphore = new SemaphoreSlim(1);


        Controller controller;

        public DataManager(Controller controller)
        {
            this.controller = controller;
            currentExecutionTime = TimeSpan.Zero;
            previousExecutionTime = TimeSpan.Zero;
            stopwatch = new Stopwatch();
            loadedBitmap = null;
            dataLoaded = false;
        }

        public async Task LoadBitmap(Bitmap bitmap, int numberOfThreads)
        {
            stopwatch.Restart();
            stopwatch.Start();

            RGBbitmapArraySize = bitmap.Width * bitmap.Height * 3;
            this.numberOfThreads = numberOfThreads;
            loadedBitmapArray = await Task.Run(()=> ArrayFromBitmap(bitmap));

            stopwatch.Stop();
            controller.mainMenu.SetExecutionTime(stopwatch.Elapsed.ToString(), "");

            loadedBitmap = bitmap;
            dataLoaded = true;
        }
        public async Task LoadBitmapAsyncV3(Bitmap bitmap, int numberOfThreads, IProgress<ImageLoadingProgress> progress)
        {
            dataLoaded = false;
            stopwatch.Restart();
            stopwatch.Start();

            ImageLoadingProgress report = new ImageLoadingProgress();

            RGBbitmapArraySize = bitmap.Width * bitmap.Height * 3;
            bitmapSize = bitmap.Width * bitmap.Height;

            this.numberOfThreads = numberOfThreads;
            int taskBitmapRange = bitmapSize / numberOfThreads;

            loadedBitmapArray = new byte[RGBbitmapArraySize];

            int currentIndex = 0;
            int endIndex;
            int pasteFromIndex = 0;
            for (int i = 0; i < numberOfThreads; i++)
            {
                endIndex = i == numberOfThreads - 1 ? bitmapSize : currentIndex + taskBitmapRange;
                BitmapRGBArrayPart RGBArray = await Task.Run(() => ArrayFromBitmapAsync(bitmap, currentIndex, endIndex, pasteFromIndex, i));
                Array.Copy(RGBArray.arr, 0, loadedBitmapArray, RGBArray.pasteIndex, RGBArray.arr.Length);
                currentIndex = endIndex;
                pasteFromIndex += taskBitmapRange * 3;

                report.percentageDone = (i + 1) * 100 / numberOfThreads;
                progress.Report(report);
            }
            stopwatch.Stop();
            controller.mainMenu.SetExecutionTime(stopwatch.Elapsed.ToString(), "");

            loadedBitmap = bitmap;
            dataLoaded = true;

        }
        public async Task LoadBitmapAsync(Bitmap bitmap, int numberOfThreads, IProgress<ImageLoadingProgress> progress)
        {  
            dataLoaded = false;
            stopwatch.Restart();
            stopwatch.Start();

            ImageLoadingProgress report = new ImageLoadingProgress();

            RGBbitmapArraySize = bitmap.Width * bitmap.Height * 3;
            bitmapSize = bitmap.Width * bitmap.Height;

            this.numberOfThreads = numberOfThreads;
            int taskBitmapRange = bitmapSize / numberOfThreads;

            loadedBitmapArray = new byte[RGBbitmapArraySize];
            List<Task<BitmapRGBArrayPart>> tasks = new List<Task<BitmapRGBArrayPart>>();
            List<BitmapRGBArrayPart> testArr = new List<BitmapRGBArrayPart>();
            
            int currentIndex = 0;
            int endIndex;
            int pasteFromIndex = 0;
            for (int i = 0; i < numberOfThreads; i++)
            {
                endIndex = i == numberOfThreads - 1 ? bitmapSize: currentIndex + taskBitmapRange;
                tasks.Add(Task.Run(() => ArrayFromBitmapAsync(bitmap, currentIndex, endIndex, pasteFromIndex, i)));
                currentIndex = endIndex;
                pasteFromIndex += taskBitmapRange * 3;

                report.percentageDone = (i + 1)*100 / numberOfThreads;
                progress.Report(report);
            }
            var results = await Task.WhenAll(tasks);
            foreach (var item in results)
            {
                if (item == null)continue;

                //Array.Copy(item.arr, 0, loadedBitmapArray, item.pasteIndex, item.arr.Length);
                await semaphore.WaitAsync();
                try
                {
                    Array.Copy(item.arr, 0, loadedBitmapArray, item.pasteIndex, item.arr.Length);
                }
                finally
                {
                    semaphore.Release();
                }
            }

            stopwatch.Stop();
            controller.mainMenu.SetExecutionTime(stopwatch.Elapsed.ToString(), "");

            loadedBitmap = bitmap;
            dataLoaded = true;

        }
        public async Task LoadBitmapAsyncV2(Bitmap bitmap, int numberOfThreads, IProgress<ImageLoadingProgress> progress)
        {
            dataLoaded = false;
            stopwatch.Restart();
            stopwatch.Start();
            ImageLoadingProgress report = new ImageLoadingProgress();

            RGBbitmapArraySize = bitmap.Width * bitmap.Height * 3;
            bitmapSize = bitmap.Width * bitmap.Height;

            this.numberOfThreads = numberOfThreads;
            int taskBitmapRange = bitmapSize / numberOfThreads;

            loadedBitmapArray = new byte[RGBbitmapArraySize];
            List<Task<BitmapRGBArrayPart>> tasks = new List<Task<BitmapRGBArrayPart>>();
            List<BitmapRGBArrayPart> testArr = new List<BitmapRGBArrayPart>();

            int currentIndex = 0;
            int endIndex;
            int pasteFromIndex = 0;
            await Task.Run(() =>
            {
                Parallel.For(0, numberOfThreads, i =>
                 {
                     endIndex = i == numberOfThreads - 1 ? bitmapSize : currentIndex + taskBitmapRange;
                     BitmapRGBArrayPart arr = ArrayFromBitmapAsync(bitmap, currentIndex, endIndex, pasteFromIndex, i);
                     Array.Copy(arr.arr, 0, loadedBitmapArray, arr.pasteIndex, arr.arr.Length);
                     currentIndex = endIndex;
                     pasteFromIndex += taskBitmapRange * 3;

                     report.percentageDone = (i + 1) * 100 / numberOfThreads;
                     progress.Report(report);
                 });
            });

            stopwatch.Stop();
            controller.mainMenu.SetExecutionTime(stopwatch.Elapsed.ToString(), "");

            loadedBitmap = bitmap;
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
           

            if (loadedBitmap == null || loadedBitmapArray == null) return null;
            Bitmap result = new Bitmap(loadedBitmap.Width, loadedBitmap.Height);
            stopwatch.Reset();
            ImageLoadingProgress report = new ImageLoadingProgress();
            switch (dllType)
            {
                case DllType.CPP:
                    IntPtr unmanagedPointer = Marshal.AllocHGlobal(RGBbitmapArraySize);
                    Marshal.Copy(loadedBitmapArray, 0, unmanagedPointer, RGBbitmapArraySize);
                    IntPtr resultPtr = IntPtr.Zero;
                    byte[] resultArray = new byte[loadedBitmapArray.Length];

                    stopwatch.Start();
                    resultPtr = CppMedianFiltering(unmanagedPointer, loadedBitmap.Width, loadedBitmap.Height);
                    stopwatch.Stop();
                    Marshal.Copy(resultPtr, resultArray, 0, resultArray.Length);

                    Marshal.FreeHGlobal(unmanagedPointer);
   
                    result = BitmapFromArray(resultArray, loadedBitmap.Width, loadedBitmap.Height);
                    break;
                case DllType.ASM:
                    stopwatch.Start();
                    byte[] resultArr = new byte[RGBbitmapArraySize];
                    //AsmMedianFilter(loadedBitmapArray, RGBbitmapArraySize);
                    for (int i = 0; i < RGBbitmapArraySize; i++)
                    {
                        await Task.Run(() =>
                        {
                            resultArr[i] = AsmMedianFilter(loadedBitmapArray[i]);
                        });
                        report.percentageDone = (i + 1) * 100 / RGBbitmapArraySize;
                        progress.Report(report);
                    }

                    //AsmMedianFilter(loadedBitmapArray, RGBbitmapArraySize);
                    result = BitmapFromArray(resultArr, loadedBitmap.Width, loadedBitmap.Height);
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
