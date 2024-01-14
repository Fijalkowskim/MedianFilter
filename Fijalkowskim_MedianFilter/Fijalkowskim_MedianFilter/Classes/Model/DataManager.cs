using System.Runtime.InteropServices;
using System.Diagnostics;
using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Drawing.Imaging;

// Mateusz Fijałkowski
// Median Filter v1 - 14.01.2024
// Silesian University of Technology 2023/24

namespace Fijalkowskim_MedianFilter
{  
    public enum DllType { CPP, ASM }
    //Most important class of the application for handling image filtering from c++ and assembly DLLs.
    public class DataManager
    {
        //Import proper DLLs 
#if DEBUG
        [DllImport(@"D:\.1Studia\JA\MedianFilter\Fijalkowskim_MedianFilter\x64\Debug\JAAsm.dll", CallingConvention = CallingConvention.Cdecl)]
        unsafe static extern void AsmMedianFilter(IntPtr bitmap, int bitmapWidth, int rows, int startRow, int bitmapHeight);

        [DllImport(@"D:\.1Studia\JA\MedianFilter\Fijalkowskim_MedianFilter\x64\Debug\JACpp.dll", CallingConvention = CallingConvention.Cdecl)]
        unsafe static extern void CppMedianFilter(IntPtr bitmap, int bitmapWidth, int rows, int startRow, int bitmapHeight);

#else
        [DllImport(@"D:\.1Studia\JA\MedianFilter\Fijalkowskim_MedianFilter\x64\Release\JAAsm.dll", CallingConvention = CallingConvention.Cdecl)]
          unsafe static extern void AsmMedianFilter(IntPtr bitmap, int bitmapWidth, int rows, int startRow, int bitmapHeight);

        [DllImport(@"D:\.1Studia\JA\MedianFilter\Fijalkowskim_MedianFilter\x64\Release\JACpp.dll", CallingConvention = CallingConvention.Cdecl)]
         unsafe static extern void CppMedianFilter(IntPtr bitmap, int bitmapWidth, int rows, int startRow, int bitmapHeight);
#endif
        
        //Bitmap data variables
        public Bitmap loadedBitmap { get; private set; }
        int bitmapWidth, bitmapHeight;
        //Tasks data variables
        int numberOfTasks;
        TaskData[] tasksData;
        //Control variables
        Controller controller;
        public bool applyingFilter { get; private set; }
        public bool dataLoaded { get; private set; }
        static readonly object _lock = new object();
        //Time measurement variables
        public long currentExecutionTime { get; private set; }
        public long previousExecutionTime { get; private set; }
        Stopwatch stopwatch;

        //Contructor initializes data
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
        //Method for loading bitmap data
        public void LoadBitmap(Bitmap bitmap)
        {
            this.loadedBitmap = bitmap; 
            bitmapHeight = bitmap.Height;
            bitmapWidth = bitmap.Width * 3;
            dataLoaded = true;       
        }
        //Main method for applying median filtering called from mainMenu class.
        public async Task<Bitmap> UseMedianFilter(DllType dllType, int numberOfTasks, IProgress<ImageLoadingProgress> progress)
        {
            if (applyingFilter || loadedBitmap == null) return null;
            applyingFilter = true;
            //Result is clone of loaded bitmap. All filtering is done directly onto this. Result is then returned and set in mainMenu for preview.
            Bitmap result = new Bitmap(loadedBitmap);
            //Method for setting tasks
            SetUpTasks(numberOfTasks);
            //List of tasks applying filter
            List<Task> tasks = new List<Task>();
            ImageLoadingProgress report = new ImageLoadingProgress();
            //Locking bitmap and getting pointer to the first element.
            Rectangle rect = new Rectangle(0, 0, result.Width, result.Height);
            BitmapData bmpData = result.LockBits(rect,ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            IntPtr ptr = bmpData.Scan0;
            //Restarting timer for measuring filtering time
            stopwatch.Reset();
            stopwatch.Start();
            //Depending on dllType selected by user c++ or assembly filter is chosen. Filtering is then applied on separate tasks simultaneously
            switch (dllType)
            {
                //C++
                case DllType.CPP:
                    for (int i = 0; i < numberOfTasks; i++)
                    {
                        TaskData taskData = tasksData[i];

                        tasks.Add(Task.Run(() =>
                        {
                            CppMedianFilter(ptr, bitmapWidth, taskData.rows, taskData.startRow, bitmapHeight);
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
            //After all tasks are finished unlock bitmap, report progress, stop the timer and return result to mainMenu
            await Task.WhenAll(tasks);
            result.UnlockBits(bmpData);    
            stopwatch.Stop();
            HandleTimer();
            report.percentageDone = 100;
            progress.Report(report);
            applyingFilter = false;
            return result;          
        }
        //Method for setting tasks data based on number of tasks selected by user.
        //Each task is responsible for calculated number of rows of bitmap.
        //If rows cannot be distributed evenly throughout tasks additional rows are added to initial tasks one at the time.
        void SetUpTasks(int numberOfTasks)
        {
            //If number of tasks chosen by user is greater then bitmap height, clamp it.
            this.numberOfTasks = Math.Min(numberOfTasks, bitmapHeight);
            tasksData = new TaskData[numberOfTasks];
            //Calculate how many rows should be handled by each task
            int taskBitmapRange = bitmapHeight / numberOfTasks;
            //Undivided range is number of rows that cannot be distributed evenly
            int undvidedRange = bitmapHeight - numberOfTasks * taskBitmapRange;
            int startRow = 0;

            for (int i = 0; i < numberOfTasks; i++)
            {
                //Add data for each task
                tasksData[i] = new TaskData();
                tasksData[i].rows = taskBitmapRange;
                tasksData[i].startRow = startRow;
                //If there are some undivided rows, add them to current task
                if (undvidedRange != 0)
                {
                    tasksData[i].rows++;
                    undvidedRange--;
                }
                startRow += tasksData[i].rows;
            }
            
        }
        //Method for displaying execution time of filtering methods
        void HandleTimer()
        {
            if (currentExecutionTime != -1)
                previousExecutionTime = currentExecutionTime;
            currentExecutionTime = stopwatch.ElapsedMilliseconds;
            controller.mainMenu.SetExecutionTime(currentExecutionTime.ToString(), previousExecutionTime < 0 ? "" : previousExecutionTime.ToString());
        }
        //Helper method for simulating filtering and printing execution times to file
        public async Task<long> SimulateFiltering(DllType dllType, int numberOfTasks)
        {
            if (applyingFilter || loadedBitmap == null) return -1;
            applyingFilter = true;
            //Result is clone of loaded bitmap. All filtering is done directly onto this. Result is then returned and set in mainMenu for preview.
            Bitmap result = new Bitmap(loadedBitmap);
            //Method for setting tasks
            SetUpTasks(numberOfTasks);
            //List of tasks applying filter
            List<Task> tasks = new List<Task>();
            //Locking bitmap and getting pointer to the first element.
            Rectangle rect = new Rectangle(0, 0, result.Width, result.Height);
            BitmapData bmpData = result.LockBits(rect, ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            IntPtr ptr = bmpData.Scan0;
            //Restarting timer for measuring filtering time
            stopwatch.Reset();
            stopwatch.Start();
            //Depending on dllType selected by user c++ or assembly filter is chosen. Filtering is then applied on separate tasks simultaneously
            switch (dllType)
            {
                //C++
                case DllType.CPP:
                    for (int i = 0; i < numberOfTasks; i++)
                    {
                        TaskData taskData = tasksData[i];

                        tasks.Add(Task.Run(() =>
                        {
                            CppMedianFilter(ptr, bitmapWidth, taskData.rows, taskData.startRow, bitmapHeight);
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
            //After all tasks are finished unlock bitmap, report progress, stop the timer and return result to mainMenu
            await Task.WhenAll(tasks);
            result.UnlockBits(bmpData);
            stopwatch.Stop();
            applyingFilter = false;
            return stopwatch.ElapsedMilliseconds;
        }
    }
}
