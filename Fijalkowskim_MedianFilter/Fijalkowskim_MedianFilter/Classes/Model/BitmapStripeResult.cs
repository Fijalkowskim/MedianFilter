using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fijalkowskim_MedianFilter
{
    public class BitmapStripeResult
    {
        public byte[] resultArray;
        public int startRow;
        public int rows;

        public BitmapStripeResult(byte[] resultArray, int startRow, int rows)
        {
            this.resultArray = resultArray;
            this.startRow = startRow;
            this.rows = rows;
        }
    }
}
