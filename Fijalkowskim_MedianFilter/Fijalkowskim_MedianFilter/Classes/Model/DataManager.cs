using System.Runtime.InteropServices;
using System.Diagnostics;
using System;

namespace Fijalkowskim_MedianFilter
{  
    public enum DllType { CPP, ASM}
    public class DataManager
    {
        [DllImport(@"D:\1 Studia\JA\Sem5\Fijalkowskim_MedianFilter\x64\Debug\JAAsm.dll")]
        static extern int MyProc1(int a, int b);

        [DllImport(@"D:\1 Studia\JA\Sem5\Fijalkowskim_MedianFilter\x64\Debug\JACpp.dll")]
        static extern int Add(int a, int b);

        public TimeSpan currentExecutionTime { get; private set; }
        public TimeSpan previousExecutionTime { get; private set; }
        Stopwatch stopwatch;

        public DataManager()
        {
            currentExecutionTime = TimeSpan.Zero;
            previousExecutionTime = TimeSpan.Zero;
            stopwatch = new Stopwatch();
        }

        public int GetResult(int a, int b, DllType dllType)
        {          
            int result = 0;
            switch (dllType)
            {
                case DllType.CPP:
                    stopwatch.Start();
                    result =  Add(a, b);
                    stopwatch.Stop();
                    break;
                case DllType.ASM:
                    stopwatch.Start();
                    result =  MyProc1(a, b);
                    stopwatch.Stop();
                    break;
            }
            if (currentExecutionTime != TimeSpan.Zero)
                previousExecutionTime = currentExecutionTime;
            currentExecutionTime = stopwatch.Elapsed;
            return result;
        }
    }
}
