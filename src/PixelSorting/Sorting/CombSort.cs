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
        public static void CombSort(PixelSpan2D span, IComparer<TPixel> comparer)
        {
            int gap = span.ItemCount;
            float shrink = 1.3f;
            bool sorted = false;

            while (!sorted)
            {
                // Update the gap value for a next comb
                gap = (int)(gap / shrink);
                if (gap <= 1)
                {
                    gap = 1;
                    sorted = true;
                }
                //else if (gap == 9 || gap == 10)
                //{
                //    gap = 11;
                //}

                for (int i = 0; i + gap < span.ItemCount; i++)
                {
                    if (comparer.Compare(span[i], span[i + gap]) > 0)
                    {
                        Swap(span, i, i + gap);
                        sorted = false;
                    }
                }
            }
        }

        public static void CombSort(PixelSpan2D span, IComparer<TPixel> comparer, int pureness)
        {
            int gap = span.ItemCount;
            float shrink = 1.3f;
            bool sorted = false;

            for (int p = 0; p < pureness && !sorted; p++)
            {
                // Update the gap value for a next comb
                gap = (int)(gap / shrink);
                if (gap <= 1)
                {
                    gap = 1;
                    sorted = true;
                }
                //else if (gap == 9 || gap == 10)
                //{
                //    gap = 11;
                //}

                for (int i = 0; i + gap < span.ItemCount; i++)
                {
                    if (comparer.Compare(span[i], span[i + gap]) > 0)
                    {
                        Swap(span, i, i + gap);
                        sorted = false;
                    }
                }
            }
        }

        public static void CombSort(PixelSpan span, IComparer<TPixel> comparer)
        {
            int gap = span.ItemCount;
            float shrink = 1.3f;
            int maxIterations = 1000;
            bool sorted = false;

            for (int pureness = 0; pureness < maxIterations && !sorted; pureness++)
            {
                // Update the gap value for a next comb
                gap = (int)(gap / shrink);
                if (gap <= 1)
                {
                    gap = 1;
                    sorted = true;
                }
                //else if (gap == 9 || gap == 10)
                //{
                //    gap = 11;
                //}

                for (int i = 0; i + gap < span.ItemCount; i++)
                {
                    if (comparer.Compare(span[i], span[i + gap]) > 0)
                    {
                        Swap(span, i, i + gap);
                        sorted = false;
                    }
                }
            }
        }
    }
}
