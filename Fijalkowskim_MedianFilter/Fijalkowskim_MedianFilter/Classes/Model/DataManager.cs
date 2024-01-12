using System.Runtime.InteropServices;
using System.Diagnostics;
using System;
using System.Threading;
using System.Drawing;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Drawing.Imaging;

namespace Fijalkowskim_MedianFilter
{  
    public enum DllType { CPP, ASM}
    public class DataManager
    {
#if DEBUG
        [DllImport(@"D:\.1Studia\JA\MedianFilter\Fijalkowskim_MedianFilter\x64\Debug\JAAsm.dll")]
        unsafe static extern void AsmMedianFilter(IntPtr stripe, int bitmapWidth, int rows, int startRow, int bitmapHeight);

        [DllImport(@"D:\.1Studia\JA\MedianFilter\Fijalkowskim_MedianFilter\x64\Debug\JACpp.dll", CallingConvention = CallingConvention.StdCall)]
        unsafe static extern void CppMedianFilter(IntPtr stripe, int bitmapWidth, int rows, int startRow, int bitmapHeight);

#else
        [DllImport(@"D:\.1Studia\JA\MedianFilter\Fijalkowskim_MedianFilter\x64\Release\JAAsm.dll")]
        unsafe static extern void AsmMedianFilter(byte* stripe, int bitmapWidth, int rows);

        [DllImport(@"D:\.1Studia\JA\MedianFilter\Fijalkowskim_MedianFilter\x64\Release\JACpp.dll", CallingConvention = CallingConvention.StdCall)]
        unsafe static extern void CppMedianFilter(byte* stripe, int bitmapWidth, int rows);
#endif
        Controller controller;
        //Bitmap
        public Bitmap loadedBitmap { get; private set; }
        int bitmapWidth, bitmapHeight;
        //Tasks
        int numberOfTasks;
        TaskData[] tasksData;      
        //Controll variables
        public bool applyingFilter { get; private set; }
        public bool dataLoaded { get; private set; }
        static readonly object _lock = new object();
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
            bitmapHeight = bitmap.Height;
            bitmapWidth = bitmap.Width * 3;
            dataLoaded = true;       
        }
        public async Task<Bitmap> UseMedianFilter(DllType dllType, int numberOfTasks, IProgress<ImageLoadingProgress> progress)
        {
            if (applyingFilter || loadedBitmap == null) return null;
            applyingFilter = true;
            Bitmap result = new Bitmap(loadedBitmap);
            //Setting tasks
            SetUpTasks(numberOfTasks);
            //Variables
            List<Task> tasks = new List<Task>();
            ImageLoadingProgress report = new ImageLoadingProgress();

            //SaveBitmapToFile(loadedBitmap, "OriginalBitmap.txt");
            Rectangle rect = new Rectangle(0, 0, result.Width, result.Height);
            BitmapData bmpData = result.LockBits(rect,ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            IntPtr ptr = bmpData.Scan0;
            //Timer
            stopwatch.Reset();
            stopwatch.Start();

            switch (dllType)
            {

                //C++
                case DllType.CPP:
                    //Main filtering tasks
                    for (int i = 0; i < numberOfTasks; i++)
                    {
                        TaskData taskData = tasksData[i];

                        tasks.Add(Task.Run(() =>
                        {
                            //int noTasks = numberOfTasks;

                            CppMedianFilter(ptr, bitmapWidth, taskData.rows, taskData.startRow, bitmapHeight);
                            /*lock (_lock)
                            {
                                report.percentageDone += 100 / noTasks;
                                progress.Report(report);
                            }
                            return res;*/

                        }));
                    }
                    break;
                //Assembly
                case DllType.ASM:
                    for (int i = 0; i < numberOfTasks; i++)
                    {
                        TaskData taskData = tasksData[i];

                        tasks.Add(Task.Run(() =>
                        {
                                AsmMedianFilter(ptr, bitmapWidth, taskData.rows, taskData.startRow, bitmapHeight);
                        }));
                    }
                    break;
            }
            await Task.WhenAll(tasks);
            result.UnlockBits(bmpData);    
            stopwatch.Stop();
            HandleTimer();
            report.percentageDone = 100;
            progress.Report(report);
            applyingFilter = false;
            return result;          
        }

        void SetUpTasks(int numberOfTasks)
        {
            this.numberOfTasks = Math.Min(numberOfTasks, bitmapHeight);

            tasksData = new TaskData[numberOfTasks];
            int taskBitmapRange = bitmapHeight / numberOfTasks;
            int undvidedRange = bitmapHeight - numberOfTasks * taskBitmapRange;
            int startRow = 0;

            for (int i = 0; i < numberOfTasks; i++)
            {
                tasksData[i] = new TaskData();
                tasksData[i].rows = taskBitmapRange;
                tasksData[i].startRow = startRow;

                if (undvidedRange != 0)
                {
                    tasksData[i].rows++;
                    undvidedRange--;
                }
                startRow += tasksData[i].rows;
            }
            
        }
        void HandleTimer()
        {
            if (currentExecutionTime != -1)
                previousExecutionTime = currentExecutionTime;
            currentExecutionTime = stopwatch.ElapsedMilliseconds;
            controller.mainMenu.SetExecutionTime(currentExecutionTime.ToString(), previousExecutionTime < 0 ? "" : previousExecutionTime.ToString());
        }
    }
}
