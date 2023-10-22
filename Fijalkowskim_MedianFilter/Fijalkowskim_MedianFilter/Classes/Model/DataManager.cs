using System.Runtime.InteropServices;
using System.Diagnostics;
using System;
using System.Drawing;
using System.IO;


namespace Fijalkowskim_MedianFilter
{  
    public enum DllType { CPP, ASM}
    public class DataManager
    {
#if DEBUG
        [DllImport(@"D:\1 Studia\JA\MedianFilter\Fijalkowskim_MedianFilter\x64\Debug\JAAsm.dll")]
        static extern int MyProc1(int a, int b);

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
        Stopwatch stopwatch;
        public Bitmap loadedBitmap { get; private set; }
        byte[] loadedBitmapArray;
        int bitmapSize;
        int numberOfThreads;

        public DataManager()
        {
            currentExecutionTime = TimeSpan.Zero;
            previousExecutionTime = TimeSpan.Zero;
            stopwatch = new Stopwatch();
            loadedBitmap = null;
        }
        public void LoadBitmap(Bitmap bitmap, int numberOfThreads)
        {
            bitmapSize = bitmap.Width * bitmap.Height * 3;
            this.numberOfThreads = numberOfThreads;
            loadedBitmap = bitmap;
            loadedBitmapArray = ArrayFromBitmap(bitmap);
        }
       

        public Bitmap UseMedianFilter(DllType dllType)
        {
            if (loadedBitmap == null || loadedBitmapArray == null) return null;
            Bitmap result = new Bitmap(loadedBitmap.Width, loadedBitmap.Height);
            stopwatch.Reset();
            switch (dllType)
            {
                case DllType.CPP:
                    IntPtr unmanagedPointer = Marshal.AllocHGlobal(bitmapSize);
                    Marshal.Copy(loadedBitmapArray, 0, unmanagedPointer, bitmapSize);
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
                    //result =  MyProc1(a, b);
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
