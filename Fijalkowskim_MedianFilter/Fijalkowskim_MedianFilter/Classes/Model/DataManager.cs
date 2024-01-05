using System.Runtime.InteropServices;
using System.Diagnostics;
using System;
using System.Threading;
using System.Drawing;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Fijalkowskim_MedianFilter
{  
    public enum DllType { CPP, ASM}
    public class DataManager
    {
#if DEBUG
        [DllImport(@"D:\.1Studia\JA\MedianFilter\Fijalkowskim_MedianFilter\x64\Debug\JAAsm.dll")]
        unsafe static extern void AsmMedianFilter(byte* stripe, int bitmapWidth, int rows);

        [DllImport(@"D:\.1Studia\JA\MedianFilter\Fijalkowskim_MedianFilter\x64\Debug\JACpp.dll", CallingConvention = CallingConvention.StdCall)]
        static extern IntPtr FilterBitmapStripe(IntPtr stripe, int bitmapWidth, int rows);

#else
 [DllImport(@"D:\.1 Studia\JA\MedianFilter\Fijalkowskim_MedianFilter\x64\Release\JAAsm.dll")]
        unsafe static extern void AsmMedianFilter(byte* pixels, int width, int height);

        [DllImport(@"D:\.1 Studia\JA\MedianFilter\Fijalkowskim_MedianFilter\x64\Release\JACpp.dll")]
        static extern IntPtr FilterBitmapStripe(IntPtr stripe, int bitmapWidth, int rows);
#endif
        Controller controller;
        //Bitmap
        public Bitmap loadedBitmap { get; private set; }
        int bitmapWidth, bitmapHeight, bitmapSize;
        //Tasks
        int numberOfTasks;
        TaskData[] tasksData;      
        //Controll variables
        public bool applyingFilter { get; private set; }
        public bool dataLoaded { get; private set; }
        static readonly object arrayLock = new object();
        private CancellationTokenSource cancellationTokenSource;
        //Stopwatch
        public long currentExecutionTime { get; private set; }
        public long previousExecutionTime { get; private set; }
        Stopwatch stopwatch;
        //---------

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
            this.loadedBitmap = bitmap;
            bitmapSize = bitmap.Width * bitmap.Height;    
            bitmapHeight = bitmap.Height;
            bitmapWidth = bitmap.Width;
            dataLoaded = true;       
        }
        public async Task<Bitmap> UseMedianFilter(DllType dllType, int numberOfTasks, IProgress<ImageLoadingProgress> progress)
        {
            if (applyingFilter || loadedBitmap == null) return null;
            applyingFilter = true;

            //Cannellation token
            StopProcess();
            cancellationTokenSource = new CancellationTokenSource();

            //Setting tasks
            SetUpTasks(numberOfTasks);
            CalculateTaskData();

            //Variables
            List<Task<BitmapStripeResult>> tasks = new List<Task<BitmapStripeResult>>();
            Bitmap result = new Bitmap(bitmapWidth, bitmapHeight);
            ImageLoadingProgress report = new ImageLoadingProgress();

            //Timer
            stopwatch.Reset();
            stopwatch.Start();

            BitmapStripeResult[] tasksResults = new BitmapStripeResult[numberOfTasks];

            SaveBitmapToFile(loadedBitmap, "OriginalBitmap.txt");

            switch (dllType)
            {

                //C++
                case DllType.CPP:                 
                    //Main filtering tasks
                    for (int i = 0; i < numberOfTasks; i++)
                    {
                        int taskIndex = i;
                        tasks.Add(Task.Run(() => {
                         
                            BitmapStripeResult res = ApplyCppMedianFilter(tasksData[taskIndex], bitmapWidth * 3);
                            report.percentageDone = (taskIndex + 1) * 100 / numberOfTasks;
                            progress.Report(report);
                            return res;

                        }, cancellationTokenSource.Token));
                    }
                    break;
                    //Assembly
                case DllType.ASM:
                    for (int i = 0; i < numberOfTasks; i++)
                    {
                        int taskIndex = i;
                        tasks.Add(Task.Run(() => {

                            BitmapStripeResult res = ApplyAsmMedianFilter(tasksData[taskIndex], bitmapWidth * 3, taskIndex);
                            report.percentageDone = (taskIndex + 1) * 100 / numberOfTasks;
                            progress.Report(report);
                            return res;

                        }, cancellationTokenSource.Token));
                    }
                    break;
            }
            await Task.WhenAll(tasks);

            report.percentageDone = 100;
            progress.Report(report);

            //Combine stripes
            for (int i = 0; i < tasks.Count; i++)
            {
                if (result == null) continue;
                AddStripeToBitmap(ref result, tasks[i].Result.resultArray, tasks[i].Result.startRow, tasks[i].Result.rows);
            }
            SaveBitmapToFile(result, "FilteredBitmap.txt");
            stopwatch.Stop();
            HandleTimer();
            applyingFilter = false;
            return result;
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
                tasksData[i].bitmapStripe = new List<byte>();
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
                            tasksData[i].bitmapStripe.Add(0);
                            tasksData[i].bitmapStripe.Add(0);
                            tasksData[i].bitmapStripe.Add(0);
                        }
                    }
                    else
                    {
                        for (int x = 0; x < bitmapWidth; x++)
                        {
                            tasksData[i].bitmapStripe.Add(loadedBitmap.GetPixel(x, y).R);
                            tasksData[i].bitmapStripe.Add(loadedBitmap.GetPixel(x, y).G);
                            tasksData[i].bitmapStripe.Add(loadedBitmap.GetPixel(x, y).B);
                        }
                    }
                }
                startY += rows;
            }
        }
        BitmapStripeResult ApplyCppMedianFilter(TaskData taskData, int stripeWidth)
        {
            int stripeLength = taskData.bitmapStripe.Count;
            int resultSize = stripeWidth * taskData.rows;

            IntPtr ptr = Marshal.AllocHGlobal(stripeLength);
 

            Marshal.Copy(taskData.bitmapStripe.ToArray(), 0, ptr, stripeLength);
            IntPtr resultPtr = IntPtr.Zero;

            byte[] resultArray = null;

            resultPtr = FilterBitmapStripe(ptr, stripeWidth, taskData.rows);
            if (resultPtr != IntPtr.Zero)
            {
                resultArray = new byte[resultSize];
                try
                {
                    Marshal.Copy(resultPtr, resultArray, 0, resultSize);
                }           
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
            Marshal.FreeHGlobal(ptr);
            return new BitmapStripeResult(resultArray, taskData.startRow, taskData.rows);
        }
        BitmapStripeResult ApplyAsmMedianFilter(TaskData taskData, int stripeWidth, int threadNumber)
        {
            int stripeLength = taskData.bitmapStripe.Count;
            int resultSize = stripeWidth * taskData.rows;
            byte[] filteredStripe = new byte[stripeLength];
            byte[] resultArray = new byte[resultSize];

            unsafe
            {
                fixed (byte* bytePtr = taskData.bitmapStripe.ToArray())
                {
                    AsmMedianFilter(bytePtr, stripeWidth, taskData.rows);
                    if (bytePtr != null)
                    {
                        try
                        {
                            Marshal.Copy((IntPtr)bytePtr, filteredStripe, 0, stripeLength);
                            int pixelIndex = stripeWidth;
                            for (int i = 0; i < resultSize; i++)
                            {
                                resultArray[i] = filteredStripe[pixelIndex];
                                pixelIndex++;
                            }
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);
                        }
                    }
                }
            }      

            return new BitmapStripeResult(resultArray, taskData.startRow, taskData.rows);
        }
        void AddStripeToBitmap(ref Bitmap bmp, byte[] arr, int startRow, int rows)
        {
            if (arr == null) return;
            int idx = 0;
            for (int y = startRow; y < startRow + rows; y++)
            {
                for (int x = 0; x < bitmapWidth; x++)
                {
                    lock (arrayLock)
                    {
                        bmp.SetPixel(x, y, Color.FromArgb(arr[idx], arr[idx+1], arr[idx+2]));
                    }
                    idx+=3;
                }
            }
        }
        void HandleTimer()
        {
            if (currentExecutionTime != -1)
                previousExecutionTime = currentExecutionTime;
            currentExecutionTime = stopwatch.ElapsedMilliseconds;
            controller.mainMenu.SetExecutionTime(currentExecutionTime.ToString(), previousExecutionTime < 0 ? "" : previousExecutionTime.ToString());
        }
        public bool StopProcess()
        {
            if (cancellationTokenSource != null)
            {
                cancellationTokenSource.Cancel();
                cancellationTokenSource.Dispose();
                cancellationTokenSource = null;
                return true;
            }
            return false;
        }
        #region File saving helpers
        void SaveBitmapToFile(Bitmap bitmap, String filename)
        {
            if (bitmap == null)
                return;

            int width = bitmap.Width;
            int height = bitmap.Height;

            using (System.IO.StreamWriter file = new System.IO.StreamWriter(filename))
            {
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        Color pixelColor = bitmap.GetPixel(x, y);
                        file.Write($"({pixelColor.R} {pixelColor.G} {pixelColor.B})");
                    }
                    file.WriteLine();
                }
            }
        }
        #endregion
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
