namespace Sorting;

public partial class Sorter<TPixel>
    where TPixel : struct
{
    public static void CombSort(PixelSpan2D span, IComparer<TPixel> comparer)
    {
        // var gap = span.ItemCount;
        // var shrink = 1.3f;
        // var sorted = false;
        //
        // while (!sorted)
        // {
        //     // Update the gap value for a next comb
        //     gap = (uint)(gap / shrink);
        //     if (gap <= 1)
        //     {
        //         gap = 1;
        //         sorted = true;
        //     }
        //     // We can use an optimization here, as there is no cusom request
        //     // to keep the pureness low.
        //     else if (gap == 9 || gap == 10)
        //     {
        //         gap = 11;
        //     }
        //
        //     for (var i = 0; i + gap < span.ItemCount; i++)
        //     {
        //         if (comparer.Compare(span[i], span[i + gap]) > 0)
        //         {
        //             Swap(span, i, i + gap);
        //             sorted = false;
        //         }
        //     }
        // }
    }

    public static void CombSort(PixelSpan2D span, IComparer<TPixel> comparer, int pureness)
    {
        // var gap = span.ItemCount;
        // var shrink = 1.3f;
        // var sorted = false;
        //
        // for (var p = 0; p < pureness && !sorted; p++)
        // {
        //     // Update the gap value for a next comb
        //     gap = (int)(gap / shrink);
        //     if (gap <= 1)
        //     {
        //         gap = 1;
        //         sorted = true;
        //     }
        //
        //     for (var i = 0; i + gap < span.ItemCount; i++)
        //     {
        //         if (comparer.Compare(span[i], span[i + gap]) > 0)
        //         {
        //             Swap(span, i, i + gap);
        //             sorted = false;
        //         }
        //     }
        // }
    }

    public static void CombSort(PixelSpan span, IComparer<TPixel> comparer)
    {
        var gap = span.ItemCount;
        var shrink = 1.3f;
        var maxIterations = 1000;
        var sorted = false;

        for (var pureness = 0; pureness < maxIterations && !sorted; pureness++)
        {
            // Update the gap value for a next comb
            gap = (int)(gap / shrink);
            if (gap <= 1)
            {
                gap = 1;
                sorted = true;
            }

            for (var i = 0; i + gap < span.ItemCount; i++)
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