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
        // TODO: Increase customizability => pick another squence with more gaps.

        // Ciura gap sequence
        private static readonly int[] SHELLSORT_GAPS = [701, 301, 132, 57, 23, 10, 4, 1];
        public static readonly int ShellPurenessMax = SHELLSORT_GAPS.Length - 1;


        /// <summary>
        /// Sort the array with a selected level of pureness.
        /// </summary>
        /// <param name="span"></param>
        /// <param name="comparer"></param>
        /// <param name="lo">Inlcusive</param>
        /// <param name="hi">Exclusive</param>
        /// <param name="pureness">
        /// An impure span is a not completeley sorted span. Pureness increases overhead in 
        /// O(n^2) fashion, where n is number of elements in the span. 
        /// </param>
        /// <exception cref="ArgumentException"></exception>
        public static void ShellSort(PixelSpan2D span, IComparer<TPixel> comparer, int lo, int hi, int pureness)
        {
            if (pureness < 0 || pureness >= SHELLSORT_GAPS.Length)
            {
                throw new ArgumentException(nameof(pureness));
            }

            for (int gapIndex = 0; gapIndex <= pureness; gapIndex++)
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
