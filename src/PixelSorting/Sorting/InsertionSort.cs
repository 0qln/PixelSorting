using System;
using System.Collections;
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
        public unsafe void InsertionSort(SortDirection direction)
        {
            //if (SortDirection == SortDirection.Horizontal)
            //{
            //    for (int y = 0; y < _imageHeight; y++)
            //    {
            //        InsertionSort<TPixel>()
            //    }
            //}
        }

        /// <summary></summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="span"></param>
        /// <param name="comparer"></param>
        /// <param name="step"></param>
        /// <param name="from">Inclusive</param>
        /// <param name="to">Exclusive</param>
        public static void InsertionSort(Span<TPixel> span, IComparer<TPixel> comparer, int step, int from, int to)
        {
            for (int i = from; i < to - step; i += step)
            {
                TPixel t = span[i + step];

                int j = i;
                while (j >= from && comparer.Compare(t, span[j]) < 0)
                {
                    span[j + step] = span[j];
                    j -= step;
                }

                span[j + step] = t;
            }
        }


        public static void InsertionSort(PixelSpan span, IComparer<TPixel> comparer)
        {
            for (int i = 0; i < span.ItemCount - 1; ++i)
            {
                TPixel t = span[i + 1];

                int j = i;
                while (j >= 0 && comparer.Compare(t, span[j]) < 0)
                {
                    span[j + 1] = span[j];
                    --j;
                }

                span[j + 1] = t;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="span"></param>
        /// <param name="comparer"></param>
        /// <param name="lo">Inclusive</param>
        /// <param name="hi">Inclusive</param>
        public static void InsertionSort(PixelSpan span, IComparer<TPixel> comparer, int lo, int hi)
        {
            Debug.Assert(hi < span.ItemCount);

            for (int i = lo; i < hi; ++i)
            {
                TPixel t = span[i + 1];

                int j = i;
                while (j >= lo && comparer.Compare(t, span[j]) < 0)
                {
                    span[j + 1] = span[j];
                    --j;
                }

                span[j + 1] = t;
            }
        }
    }
}
