using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sorting;

namespace Main
{
    internal class PixelBenchmark
    {
        private SimpleComparablePixel[] _simplePixel = { new SimpleComparablePixel(0, 0, 0, 0), new SimpleComparablePixel(255, 255, 255, 255) };
        private ComparablePixel[] _pixel = { new ComparablePixel(0, 0, 0, 0), new ComparablePixel(255, 255, 255, 255) };

        public void SimplePixelCompare()
        {
            _simplePixel[0].CompareTo(_simplePixel[1]);
        }

        public void PixelCompare()
        {
            _pixel[0].CompareTo(_pixel[1]);
        }
    }
}
