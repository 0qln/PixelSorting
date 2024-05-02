using Sorting.Pixels.KeySelector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sorting
{
    // TODO?

    public partial class Sorter<TPixel>
        where TPixel : struct
    {
        public enum RadixOrder { MSD, LSD }

        public static void RadixSortBase10(PixelSpan2D span, IOrderedKeySelector<TPixel> selector, RadixOrder order)
        {
            if (order == RadixOrder.LSD)
            {
            }
            else
            {
            }
        }
    }
}
