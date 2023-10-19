using System.Runtime.InteropServices;
using System.Diagnostics;
using System;
using System.Drawing;


namespace Fijalkowskim_MedianFilter
{  
    public enum DllType { CPP, ASM}
    public class DataManager
    {
#if DEBUG
        [DllImport(@"D:\1 Studia\JA\MedianFilter\Fijalkowskim_MedianFilter\x64\Debug\JAAsm.dll")]
        static extern int MyProc1(int a, int b);

        [DllImport(@"D:\1 Studia\JA\MedianFilter\Fijalkowskim_MedianFilter\x64\Debug\JACpp.dll")]
        static extern int CppFunc(int a, int b);
#else
 [DllImport(@"D:\1 Studia\JA\MedianFilter\Fijalkowskim_MedianFilter\x64\Release\JAAsm.dll")]
        static extern int MyProc1(int a, int b);

        [DllImport(@"D:\1 Studia\JA\MedianFilter\Fijalkowskim_MedianFilter\x64\Release\JACpp.dll")]
        static extern int CppFunc(int a, int b);
#endif

        public TimeSpan currentExecutionTime { get; private set; }
        public TimeSpan previousExecutionTime { get; private set; }
        Stopwatch stopwatch;
        public Bitmap loadedBitmap { get; set; }
        

        public DataManager()
        {
            currentExecutionTime = TimeSpan.Zero;
            previousExecutionTime = TimeSpan.Zero;
            stopwatch = new Stopwatch();
            loadedBitmap = null;
        }
        public Bitmap MedianFiltering(Bitmap bitmap)
        {
            Bitmap filteredBitmap = new Bitmap(bitmap.Width, bitmap.Height);
            for (int y = 0; y < bitmap.Height; y++)
            {
                for (int x = 0; x < bitmap.Width; x++)
                {
                    Color newColor = Color.FromArgb(0, bitmap.GetPixel(x, y).G, bitmap.GetPixel(x, y).B);
                    filteredBitmap.SetPixel(x, y, newColor);
                }
            }
            return filteredBitmap;
        }

        public int GetResult(int a, int b, DllType dllType)
        {          
            stopwatch.Reset();
            int result = 0;
            switch (dllType)
            {
                case DllType.CPP:
                    stopwatch.Start();
                    result = CppFunc(a, b);
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
