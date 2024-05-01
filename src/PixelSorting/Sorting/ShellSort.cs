using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sorting
{
    public partial class Sorter<TPixel>
        where TPixel : struct
    {
        private static readonly int[] SHELLSORT_GAPS = [701, 301, 132, 57, 23, 10, 4, 1];

        public static void ShellSort(PixelSpan2D span, IComparer<TPixel> comparer, int hi, int lo)
        {
            for (int gapIndex = 0; gapIndex < SHELLSORT_GAPS.Length; gapIndex++)
            {
                for (int gap = SHELLSORT_GAPS[gapIndex], i = gap + lo; i < hi; i++)
                {
                    TPixel temp = span[i];
                    int j = i;

                    while ((j >= gap) && (comparer.Compare(temp, span[j - gap]) < 0))
                    {
                        span[j] = span[j - gap];
                        j -= gap;
                    }

                    span[j] = temp;
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="span"></param>
        /// <param name="comparer"></param>
        public static void ShellSort(PixelSpan span, IComparer<TPixel> comparer)
        {
            for (int gapIndex = 0; gapIndex < SHELLSORT_GAPS.Length; gapIndex++)
            {
                for (int gap = SHELLSORT_GAPS[gapIndex], i = gap; i < span.ItemCount; i++)
                {
                    TPixel temp = span[i];
                    int j = i;

                    while ((j >= gap) && (comparer.Compare(temp, span[j - gap]) < 0))
                    {
                        span[j] = span[j - gap];
                        j -= gap;
                    }

                    span[j] = temp;
                }
            }
        }
    }
}
