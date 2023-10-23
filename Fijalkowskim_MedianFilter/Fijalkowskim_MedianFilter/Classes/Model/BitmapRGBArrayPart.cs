using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fijalkowskim_MedianFilter
{
    public class BitmapRGBArrayPart
    {
        public byte[] arr;
        public int pasteIndex;

        public BitmapRGBArrayPart(byte[] arr, int pasteIndex)
        {
            this.arr = arr;
            this.pasteIndex = pasteIndex;
        }
    }
}
