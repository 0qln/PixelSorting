using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sorting
{
    public partial class Sorter<TPixel>
        where TPixel : struct
    {
        public static void HeapSort(PixelSpan keys, IComparer<TPixel> comparer)
        {
            HeapSort(keys, comparer, 0, keys.ItemCount - 1);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="keys"></param>
        /// <param name="comparer"></param>
        /// <param name="lo">Inclusive</param>
        /// <param name="hi">Inclusive</param>
        public static void HeapSort(PixelSpan keys, IComparer<TPixel> comparer, int lo, int hi)
        {
            int n = hi - lo + 1;
            for (int i = n / 2; i >= 1; i--)
            {
                DownHeap(keys, comparer, i, n, lo);
            }
            for (int i = n; i > 1; i--)
            {
                Swap(keys, lo, lo + i - 1);
                DownHeap(keys, comparer, 1, i - 1, lo);
            }
        }

        private static void DownHeap(PixelSpan keys, IComparer<TPixel> comparer, int i, int n, int lo)
        {
            TPixel d = keys[lo + i - 1];
            int child;
            while (i <= n / 2)
            {
                child = 2 * i;
                if (child < n && comparer.Compare(keys[lo + child - 1], keys[lo + child]) < 0)
                {
                    child++;
                }
                if (!(comparer.Compare(d, keys[lo + child - 1]) < 0))
                    break;
                keys[lo + i - 1] = keys[lo + child - 1];
                i = child;
            }
            keys[lo + i - 1] = d;
        }

    }
}
