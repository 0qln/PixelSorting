using System.Runtime.CompilerServices;
using Sorting.Pixels;

namespace Sorting;

public partial class Sorter<TPixel>
    where TPixel : struct
{
    public class IntrospectiveSorter(IPixelComparer<TPixel> comparer) : ISorter
    {
        public object Clone()
        {
            return new IntrospectiveSorter((IPixelComparer<TPixel>)comparer.Clone());
        }

        public void Sort(PixelSpan2D span)
        {
            IntrospectiveSort(span, comparer);
        }
    }

    // This is the threshold where Introspective sort switches to Insertion sort.
    // Empirically, 16 seems to speed up most cases without slowing down others, at least for integers.
    // Large value types may benefit from a smaller number.
    public const int IntrosortSizeThreshold = 16;
    private static uint FloorLog2(uint n)
    {
        var result = 0u;
        while (n >= 1)
        {
            ++result;
            n = n / 2;
        }
        return result;
    }

    private static int FloorLog2(int n)
    {
        var result = 0;
        while (n >= 1)
        {
            ++result;
            n = n / 2;
        }
        return result;
    }

    public static void IntrospectiveSort(PixelSpan keys, IComparer<TPixel> comparer)
    {
        if (keys.ItemCount <= 1) return;
        IntroSort(keys, comparer, 0, keys.ItemCount - 1, 2 * FloorLog2(keys.ItemCount));
    }

    public static void IntrospectiveSort(FloatingPixelSpan keys, IComparer<TPixel> comparer)
    {
        if (keys.ItemCount <= 1) return;
        IntroSort(keys, comparer, 0, keys.ItemCount - 1, 2 * FloorLog2(keys.ItemCount));
    }

    public static void IntrospectiveSort(PixelSpan2D keys, IComparer<TPixel> comparer)
    {
        if (keys.ItemCount <= 1) return;
        IntroSort(keys, comparer, 0, keys.ItemCount - 1, 2 * FloorLog2(keys.ItemCount));
    }

    public static void IntrospectiveSort(UnsafePixelSpan2D keys, IComparer<TPixel> comparer)
    {
        if (keys.ItemCount <= 1) return;
        IntroSort(keys, comparer, 0, keys.ItemCount - 1, 2 * FloorLog2(keys.ItemCount));
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="keys"></param>
    /// <param name="comparer"></param>
    /// <param name="lo">Inclusive</param>
    /// <param name="hi">Inclusive</param>
    /// <param name="depthLimit"></param>
    // IntroSort is recursive; block it from being inlined into itself as
    // this is currenly not profitable.
    [MethodImpl(MethodImplOptions.NoInlining)]
    private static void IntroSort(PixelSpan2D keys, IComparer<TPixel> comparer, uint lo, uint hi, uint depthLimit)
    {
        while (hi > lo)
        {
            var partitionSize = hi - lo + 1;
            if (partitionSize <= IntrosortSizeThreshold)
            {
                if (partitionSize == 1)
                {
                    return;
                }

                if (partitionSize == 2)
                {
                    SwapIfGreater(keys, comparer, lo, hi);
                    return;
                }

                if (partitionSize == 3)
                {
                    SwapIfGreater(keys, comparer, lo, hi - 1);
                    SwapIfGreater(keys, comparer, lo, hi);
                    SwapIfGreater(keys, comparer, hi - 1, hi);
                    return;
                }

                InsertionSort(keys, comparer, lo, hi);
                return;
            }

            if (depthLimit == 0)
            {
                HeapSort(keys, comparer, lo, hi); 
                return;
            }
            depthLimit--;

            var p = PickPivotAndPartition(keys, comparer, lo, hi);
            IntroSort(keys, comparer, p + 1, hi, depthLimit);
            hi = p - 1;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="keys"></param>
    /// <param name="comparer"></param>
    /// <param name="lo">Inclusive</param>
    /// <param name="hi">Inclusive</param>
    /// <param name="depthLimit"></param>
    // IntroSort is recursive; block it from being inlined into itself as
    // this is currenly not profitable.
    [MethodImpl(MethodImplOptions.NoInlining)]
    private static void IntroSort(UnsafePixelSpan2D keys, IComparer<TPixel> comparer, uint lo, uint hi, uint depthLimit)
    {
        while (hi > lo)
        {
            var partitionSize = hi - lo + 1;
            if (partitionSize <= IntrosortSizeThreshold)
            {
                if (partitionSize == 1)
                {
                    return;
                }

                if (partitionSize == 2)
                {
                    SwapIfGreater(keys, comparer, lo, hi);
                    return;
                }

                if (partitionSize == 3)
                {
                    SwapIfGreater(keys, comparer, lo, hi - 1);
                    SwapIfGreater(keys, comparer, lo, hi);
                    SwapIfGreater(keys, comparer, hi - 1, hi);
                    return;
                }

                InsertionSort(keys, comparer, lo, hi);
                return;
            }

            if (depthLimit == 0)
            {
                HeapSort(keys, comparer, lo, hi);
                return;
            }
            depthLimit--;

            var p = PickPivotAndPartition(keys, comparer, lo, hi);
            IntroSort(keys, comparer, p + 1, hi, depthLimit);
            hi = p - 1;
        }
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="keys"></param>
    /// <param name="comparer"></param>
    /// <param name="lo">Inclusive</param>
    /// <param name="hi">Inclusive</param>
    /// <param name="depthLimit"></param>
    // IntroSort is recursive; block it from being inlined into itself as
    // this is currenly not profitable.
    [MethodImpl(MethodImplOptions.NoInlining)]
    private static void IntroSort(PixelSpan keys, IComparer<TPixel> comparer, int lo, int hi, int depthLimit)
    {
        while (hi > lo)
        {
            var partitionSize = hi - lo + 1;
            if (partitionSize <= IntrosortSizeThreshold)
            {
                if (partitionSize == 1)
                {
                    return;
                }

                if (partitionSize == 2)
                {
                    SwapIfGreater(keys, comparer, lo, hi);
                    return;
                }

                if (partitionSize == 3)
                {
                    SwapIfGreater(keys, comparer, lo, hi - 1);
                    SwapIfGreater(keys, comparer, lo, hi);
                    SwapIfGreater(keys, comparer, hi - 1, hi);
                    return;
                }

                InsertionSort(keys, comparer, lo, hi);
                return;
            }

            if (depthLimit == 0)
            {
                HeapSort(keys, comparer, lo, hi); 
                return;
            }
            depthLimit--;

            var p = PickPivotAndPartition(keys, comparer, lo, hi);
            IntroSort(keys, comparer, p + 1, hi, depthLimit);
            hi = p - 1;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="keys"></param>
    /// <param name="comparer"></param>
    /// <param name="lo">Inclusive</param>
    /// <param name="hi">Inclusive</param>
    /// <param name="depthLimit"></param>
    // IntroSort is recursive; block it from being inlined into itself as
    // this is currenly not profitable.
    [MethodImpl(MethodImplOptions.NoInlining)]
    private static void IntroSort(FloatingPixelSpan keys, IComparer<TPixel> comparer, int lo, int hi, int depthLimit)
    {
        while (hi > lo)
        {
            var partitionSize = hi - lo + 1;
            if (partitionSize <= IntrosortSizeThreshold)
            {
                if (partitionSize == 1)
                {
                    return;
                }

                if (partitionSize == 2)
                {
                    SwapIfGreater(keys, comparer, lo, hi);
                    return;
                }

                if (partitionSize == 3)
                {
                    SwapIfGreater(keys, comparer, lo, hi - 1);
                    SwapIfGreater(keys, comparer, lo, hi);
                    SwapIfGreater(keys, comparer, hi - 1, hi);
                    return;
                }

                InsertionSort(keys, comparer, lo, hi);
                return;
            }

            if (depthLimit == 0)
            {
                HeapSort(keys, comparer, lo, hi);
                return;
            }
            depthLimit--;

            var p = PickPivotAndPartition(keys, comparer, lo, hi);
            IntroSort(keys, comparer, p + 1, hi, depthLimit);
            hi = p - 1;
        }
    }

    private static uint PickPivotAndPartition(UnsafePixelSpan2D keys, IComparer<TPixel> comparer, uint lo, uint hi)
    {
        // Compute median-of-three.  But also partition them, since we've done the comparison.
        var middle = lo + ((hi - lo) / 2);

        // Sort lo, mid and hi appropriately, then pick mid as the pivot.
        SwapIfGreater(keys, comparer, lo, middle);  // swap the low with the mid point
        SwapIfGreater(keys, comparer, lo, hi);   // swap the low with the high
        SwapIfGreater(keys, comparer, middle, hi); // swap the middle with the high

        var pivot = keys[middle];
        Swap(keys, middle, hi - 1);
        var left = lo;  // We already partitioned lo and hi and put the pivot in hi - 1.  And we pre-increment & decrement below.
        var right = hi - 1;  // We already partitioned lo and hi and put the pivot in hi - 1.  And we pre-increment & decrement below.

        while (left < right)
        {
            while (comparer.Compare(keys[++left], pivot) < 0) ;
            while (comparer.Compare(pivot, keys[--right]) < 0) ;

            if (left >= right)
                break;

            Swap(keys, left, right);
        }

        // Put pivot in the right location.
        Swap(keys, left, (hi - 1));
        return left;
    }

    private static uint PickPivotAndPartition(PixelSpan2D keys, IComparer<TPixel> comparer, uint lo, uint hi)
    {
        // Compute median-of-three.  But also partition them, since we've done the comparison.
        var middle = lo + (hi - lo) / 2;

        // Sort lo, mid and hi appropriately, then pick mid as the pivot.
        SwapIfGreater(keys, comparer, lo, middle);  // swap the low with the mid point
        SwapIfGreater(keys, comparer, lo, hi);   // swap the low with the high
        SwapIfGreater(keys, comparer, middle, hi); // swap the middle with the high

        var pivot = keys[middle];
        Swap(keys, middle, hi - 1);
        var left = lo;  // We already partitioned lo and hi and put the pivot in hi - 1.  And we pre-increment & decrement below.
        var right = hi - 1;  // We already partitioned lo and hi and put the pivot in hi - 1.  And we pre-increment & decrement below.

        while (left < right)
        {
            while (comparer.Compare(keys[++left], pivot) < 0) ;
            while (comparer.Compare(pivot, keys[--right]) < 0) ;

            if (left >= right)
                break;

            Swap(keys, left, right);
        }

        // Put pivot in the right location.
        Swap(keys, left, hi - 1);
        return left;
    }

    private static int PickPivotAndPartition(PixelSpan keys, IComparer<TPixel> comparer, int lo, int hi)
    {
        // Compute median-of-three.  But also partition them, since we've done the comparison.
        var middle = lo + ((hi - lo) / 2);

        // Sort lo, mid and hi appropriately, then pick mid as the pivot.
        SwapIfGreater(keys, comparer, lo, middle);  // swap the low with the mid point
        SwapIfGreater(keys, comparer, lo, hi);   // swap the low with the high
        SwapIfGreater(keys, comparer, middle, hi); // swap the middle with the high

        var pivot = keys[middle];
        Swap(keys, middle, hi - 1);
        int left = lo, right = hi - 1;  // We already partitioned lo and hi and put the pivot in hi - 1.  And we pre-increment & decrement below.

        while (left < right)
        {
            while (comparer.Compare(keys[++left], pivot) < 0) ;
            while (comparer.Compare(pivot, keys[--right]) < 0) ;

            if (left >= right)
                break;

            Swap(keys, left, right);
        }

        // Put pivot in the right location.
        Swap(keys, left, (hi - 1));
        return left;
    }

    private static int PickPivotAndPartition(FloatingPixelSpan keys, IComparer<TPixel> comparer, int lo, int hi)
    {
        // Compute median-of-three.  But also partition them, since we've done the comparison.
        var middle = lo + ((hi - lo) / 2);

        // Sort lo, mid and hi appropriately, then pick mid as the pivot.
        SwapIfGreater(keys, comparer, lo, middle);  // swap the low with the mid point
        SwapIfGreater(keys, comparer, lo, hi);   // swap the low with the high
        SwapIfGreater(keys, comparer, middle, hi); // swap the middle with the high

        var pivot = keys[middle];
        Swap(keys, middle, hi - 1);
        int left = lo, right = hi - 1;  // We already partitioned lo and hi and put the pivot in hi - 1.  And we pre-increment & decrement below.

        while (left < right)
        {
            while (comparer.Compare(keys[++left], pivot) < 0) ;
            while (comparer.Compare(pivot, keys[--right]) < 0) ;

            if (left >= right)
                break;

            Swap(keys, left, right);
        }

        // Put pivot in the right location.
        Swap(keys, left, hi - 1);
        return left;
    }

    private static void Swap(PixelSpan2D keys, uint i, uint j)
    {
        (keys[i], keys[j]) = (keys[j], keys[i]);
    }

    private static void Swap(UnsafePixelSpan2D keys, uint i, uint j)
    {
        (keys[i], keys[j]) = (keys[j], keys[i]);
    }

    private static void Swap(PixelSpan keys, int i, int j)
    {
        (keys[i], keys[j]) = (keys[j], keys[i]);
    }

    private static void Swap(FloatingPixelSpan keys, int i, int j)
    {
        (keys[i], keys[j]) = (keys[j], keys[i]);
    }

    private static void SwapIfGreater(PixelSpan2D keys, IComparer<TPixel> comparer, uint i, uint j)
    {
        if (i != j)
        {
            if (comparer.Compare(keys[i], keys[j]) > 0)
            {
                (keys[i], keys[j]) = (keys[j], keys[i]);
            }
        }
    }

    private static void SwapIfGreater(UnsafePixelSpan2D keys, IComparer<TPixel> comparer, uint i, uint j)
    {
        if (i != j)
        {
            if (comparer.Compare(keys[i], keys[j]) > 0)
            {
                (keys[i], keys[j]) = (keys[j], keys[i]);
            }
        }
    }

    private static void SwapIfGreater(PixelSpan keys, IComparer<TPixel> comparer, int i, int j)
    {
        if (i != j)
        {
            if (comparer.Compare(keys[i], keys[j]) > 0)
            {
                (keys[i], keys[j]) = (keys[j], keys[i]);
            }
        }
    }

    private static void SwapIfGreater(FloatingPixelSpan keys, IComparer<TPixel> comparer, int i, int j)
    {
        if (i != j)
        {
            if (comparer.Compare(keys[i], keys[j]) > 0)
            {
                (keys[i], keys[j]) = (keys[j], keys[i]);
            }
        }
    }
}