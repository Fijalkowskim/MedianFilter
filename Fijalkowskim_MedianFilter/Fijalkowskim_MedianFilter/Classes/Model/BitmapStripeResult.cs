using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fijalkowskim_MedianFilter
{
    public class BitmapStripeResult
    {
        public byte[] resultArrayR;
        public byte[] resultArrayG;
        public byte[] resultArrayB;
        public int startRow;
        public int rows;

        public BitmapStripeResult(byte[] resultArrayR, byte[] resultArrayG, byte[] resultArrayB, int startRow, int rows)
        {
            this.resultArrayR = resultArrayR;
            this.resultArrayG = resultArrayG;
            this.resultArrayB = resultArrayB;
            this.startRow = startRow;
            this.rows = rows;
        }
    }
}
