namespace Sorting;

public partial class Sorter<TPixel>
    where TPixel : struct
{
    public static void HeapSort(PixelSpan keys, IComparer<TPixel> comparer)
    {
        HeapSort(keys, comparer, 0, keys.ItemCount - 1);
    }

    [Obsolete]
    public static void HeapSort(PixelSpan2D keys, IComparer<TPixel> comparer)
    {
        HeapSort(keys, comparer, 0, keys.ItemCount - 1);
    }

#if EXPERIMENTAL
    public static void HeapSort(UnsafePixelSpan2D keys, IComparer<TPixel> comparer)
    {
        HeapSort(keys, comparer, 0, (uint)keys.ItemCount - 1);
    }
#endif


    /// <summary>
    /// 
    /// </summary>
    /// <param name="keys"></param>
    /// <param name="comparer"></param>
    /// <param name="lo">Inclusive</param>
    /// <param name="hi">Inclusive</param>
    public static void HeapSort(PixelSpan2DRun keys, IComparer<TPixel> comparer, uint lo, uint hi)
    {
        var n = hi - lo + 1;
        for (var i = n / 2; i >= 1; i--)
        {
            DownHeap(keys, comparer, i, n, lo);
        }
        for (var i = n; i > 1; i--)
        {
            Swap(keys, lo, lo + i - 1);
            DownHeap(keys, comparer, 1, i - 1, lo);
        }
    }



    /// <summary>
    /// 
    /// </summary>
    /// <param name="keys"></param>
    /// <param name="comparer"></param>
    /// <param name="lo">Inclusive</param>
    /// <param name="hi">Inclusive</param>
    [Obsolete]
    public static void HeapSort(PixelSpan2D keys, IComparer<TPixel> comparer, uint lo, uint hi)
    {
        var n = hi - lo + 1;
        for (var i = n / 2; i >= 1; i--)
        {
            DownHeap(keys, comparer, i, n, lo);
        }
        for (var i = n; i > 1; i--)
        {
            Swap(keys, lo, lo + i - 1);
            DownHeap(keys, comparer, 1, i - 1, lo);
        }
    }


#if EXPERIMENTAL
    /// <summary>
    /// 
    /// </summary>
    /// <param name="keys"></param>
    /// <param name="comparer"></param>
    /// <param name="lo">Inclusive</param>
    /// <param name="hi">Inclusive</param>
    public static void HeapSort(UnsafePixelSpan2D keys, IComparer<TPixel> comparer, uint lo, uint hi)
    {
        var n = hi - lo + 1;
        for (var i = n / 2; i >= 1; i--)
        {
            DownHeap(keys, comparer, i, n, lo);
        }
        for (var i = n; i > 1; i--)
        {
            Swap(keys, lo, lo + i - 1);
            DownHeap(keys, comparer, 1, i - 1, lo);
        }
    }
#endif

    /// <summary>
    /// 
    /// </summary>
    /// <param name="keys"></param>
    /// <param name="comparer"></param>
    /// <param name="lo">Inclusive</param>
    /// <param name="hi">Inclusive</param>
    public static void HeapSort(PixelSpan keys, IComparer<TPixel> comparer, int lo, int hi)
    {
        var n = hi - lo + 1;
        for (var i = n / 2; i >= 1; i--)
        {
            DownHeap(keys, comparer, i, n, lo);
        }
        for (var i = n; i > 1; i--)
        {
            Swap(keys, lo, lo + i - 1);
            DownHeap(keys, comparer, 1, i - 1, lo);
        }
    }

    private static void DownHeap(PixelSpan2DRun keys, IComparer<TPixel> comparer, uint i, uint n, uint lo)
    {
        var d = keys[lo + i - 1];
        uint child;
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

#if EXPERIMENTAL
    private static void DownHeap(UnsafePixelSpan2D keys, IComparer<TPixel> comparer, uint i, uint n, uint lo)
    {
        var d = keys[lo + i - 1];
        uint child;
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
#endif

    [Obsolete]
    private static void DownHeap(PixelSpan2D keys, IComparer<TPixel> comparer, uint i, uint n, uint lo)
    {
        var d = keys[lo + i - 1];
        uint child;
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

    private static void DownHeap(PixelSpan keys, IComparer<TPixel> comparer, int i, int n, int lo)
    {
        var d = keys[lo + i - 1];
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