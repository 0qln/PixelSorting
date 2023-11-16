#pragma warning disable CA1416 // Validate platform compatibility

using PixelRetros.Benchmark;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Cryptography;

namespace SortingLibrary;

public class Sorter<TPixel>
    where TPixel : struct
{
    // This is the threshold where Introspective sort switches to Insertion sort.
    // Empirically, 16 seems to speed up most cases without slowing down others, at least for integers.
    // Large value types may benefit from a smaller number.
    public const int IntrosortSizeThreshold = 16;


    public SortDirection SortDirection { get; set; } = SortDirection.Horizontal;
    public BitmapData BitmapData => _bmpData;

    private readonly int _bytesPerPixel;
    private IntPtr _imageDataPtr;
    private readonly BitmapData _bmpData;
    private readonly int _bytes;
    private IComparable[]? _pixels;


    public Sorter(BitmapData bitmapData)
    {
        _bmpData = bitmapData;
        _imageDataPtr = _bmpData.Scan0;
        _bytesPerPixel = bitmapData.Stride / bitmapData.Width;
        _bytes = Math.Abs(_bmpData.Stride) * _bmpData.Height;

        if (_bytesPerPixel != 3) throw new NotImplementedException();
    }


    public unsafe void HorizontalInner()
    {
        int bytes = _bmpData.Stride * _bmpData.Height;
        void* ptr = (void*)_bmpData.Scan0;
        var span = new Span<TPixel>(ptr, bytes);

        for (int row = 0; row < _bmpData.Height - 1; row++)
        {
            InsertionSort(
                keys: span,
                comparer: (IComparer<TPixel>)new Comparer24bit.Ascending.Blue(),
                step: 1,
                from: _bmpData.Width * row,
                to: (row + 1) * _bmpData.Width
            );
        }
    }

    public unsafe void IntroTesting(IComparer<TPixel> comparer)
    {
        int length = 50;
        Span<TPixel> span;

        int step = 3;


        void Print(Span<TPixel> span)
        {
            for (int i = 0; i < length; i++)
                Console.WriteLine((span[i] + "  " + ((Pixel_24bit)(object)span[i]).R % step).PadLeft(50));

            Console.WriteLine();
        }

        span = new Span<TPixel>((void*)_bmpData.Scan0, length);
        for (byte i = 0; i < length; i++) span[i] = (TPixel)(object)new Pixel_24bit(i, i, i);
        Print(span);


        span = new Span<TPixel>((void*)_bmpData.Scan0, length);
        for (byte i = 0; i < length; i++) span[i] = (TPixel)(object)new Pixel_24bit(i, i, i);

        IntrospectiveSort(span, comparer, step, 10, 30);

        Print(span);
    }
    public unsafe void HeapTesting(IComparer<TPixel> comparer)
    {
        int length = 60;
        Span<TPixel> span;

        int step = 7;
        int from = 10, to = 30;


        void Print(Span<TPixel> span)
        {
            for (int i = 0; i < length; i++)
                Console.WriteLine((span[i] + "  " + ((Pixel_24bit)(object)span[i]).R % step).PadLeft(50));

            Console.WriteLine();
        }

        span = new Span<TPixel>((void*)_bmpData.Scan0, length);
        for (byte i = 0; i < length; i++) span[i] = (TPixel)(object)new Pixel_24bit(i, i, i);
        Print(span);


        span = new Span<TPixel>((void*)_bmpData.Scan0, length);
        for (byte i = 0; i < length; i++) span[i] = (TPixel)(object)new Pixel_24bit(i, i, i);

        HeapSort_F(span, comparer, step);
        Print(span);


        span = new Span<TPixel>((void*)_bmpData.Scan0, length);
        for (byte i = 0; i < length; i++) span[i] = (TPixel)(object)new Pixel_24bit(i, i, i);

        InsertionSort(span, comparer, step);
        Print(span);
    }


    private delegate int IndexSelector(int i);


    [Benchmark]
    public unsafe void BenchmarkSort()
    {
        StdSort((IComparer<TPixel>)new Comparer24bit.Ascending.Blue());
    }


    public unsafe void StdSort(IComparer<TPixel> comparer)
    {
        void* ptr = _bmpData.Scan0.ToPointer();
        int bytes = _bmpData.Height * _bmpData.Width;
        var span = new Span<TPixel>(ptr, bytes);

        int step;
        IndexSelector from, to;

        if (SortDirection == SortDirection.Horizontal)
        {
            step = 1;
            from = row => row * _bmpData.Width;
            to = row => (row+1) * _bmpData.Width;            
        }
        else // vertical
        {
            step = _bmpData.Width;
            from = column => column;
            to = column => column + _bmpData.Height * _bmpData.Width;
        }

        for (int i = 0; i < (SortDirection == SortDirection.Horizontal ? _bmpData.Height : _bmpData.Width); i++)
        {
            IntrospectiveSort(span, comparer, step, from(i), to(i));
        }
    }

    #region Introspective Sort
    #region Normal
    private void IntrospectiveSort(Span<TPixel> keys, IComparer<TPixel> comparer)
    {
        if (keys.Length > 1)
        {
            IntroSort(keys, 2 * (BitOperations.Log2((uint)keys.Length) + 1), comparer);
        }
    }

    private static void IntroSort(Span<TPixel> keys, int depthLimit, IComparer<TPixel> comparer)
    {
        int hi = keys.Length - 1;
        while (hi > 0)
        {
            int partitionSize = hi + 1;

            if (partitionSize <= IntrosortSizeThreshold)
            {
                Debug.Assert(partitionSize >= 2);

                if (partitionSize == 2)
                {
                    SwapIfGreaterWithValues(keys, comparer, 0, hi);
                    return;
                }

                if (partitionSize == 3)
                {
                    SwapIfGreaterWithValues(keys, comparer, 0, hi - 1);
                    SwapIfGreaterWithValues(keys, comparer, 0, hi);
                    SwapIfGreaterWithValues(keys, comparer, hi - 1, hi);
                    return;
                }

                InsertionSort(keys.Slice(0, partitionSize), comparer);
                return;
            }

            if (depthLimit == 0)
            {
                HeapSort(keys.Slice(0, partitionSize), comparer);
                return;
            }
            depthLimit--;

            int p = PickPivotAndPartition(keys.Slice(0, partitionSize), comparer);

            // Note we've already partitioned around the pivot and do not have to move the pivot again.
            IntroSort(keys[(p + 1)..partitionSize], depthLimit, comparer);
            hi = p - 1;
        }
    }

    private static int PickPivotAndPartition(Span<TPixel> keys, IComparer<TPixel> comparer)
    {
        int hi = keys.Length - 1;

        // Compute median-of-three.  But also partition them, since we've done the comparison.
        int middle = hi >> 1;

        // Sort lo, mid and hi appropriately, then pick mid as the pivot.
        SwapIfGreaterWithValues(keys, comparer, 0, middle);  // swap the low with the mid point
        SwapIfGreaterWithValues(keys, comparer, 0, hi);      // swap the low with the high
        SwapIfGreaterWithValues(keys, comparer, middle, hi); // swap the middle with the high

        TPixel pivot = keys[middle];
        Swap(keys, middle, hi - 1);
        int left = 0, right = hi - 1;  // We already partitioned lo and hi and put the pivot in hi - 1.  And we pre-increment & decrement below.

        while (left < right)
        {
            while (comparer.Compare(keys[++left], pivot) < 0) ;
            while (comparer.Compare(pivot, keys[--right]) < 0) ;

            if (left >= right)
                break;

            Swap(keys, left, right);
        }

        // Put pivot in the right location.
        if (left != hi - 1)
        {
            Swap(keys, left, hi - 1);
        }
        return left;
    }
    #endregion


    #region Step, from, and to parameters
    private void IntrospectiveSort(Span<TPixel> keys, IComparer<TPixel> comparer, int step, int from, int to)
    {
        var n = (uint)(to - from / step);
        var depthLimit = 2 * (BitOperations.Log2(n) + 1);

        if (n > 1)
        {
            IntroSort(keys, depthLimit, comparer, step, from, to);
        }
    }

    private static void IntroSort(Span<TPixel> keys, int depthLimit, IComparer<TPixel> comparer, int step, int from, int to)
    {
        int d = to % step + step;
        int hi = to - d;


        while (hi > from)
        {
            int partitionSize = hi + d;

            if (partitionSize < IntrosortSizeThreshold * step)
            {
                if (hi == 2 * step)
                {
                    SwapIfGreaterWithValues(keys, comparer, from, hi);
                    return;
                }

                if (hi == 3 * step)
                {
                    SwapIfGreaterWithValues(keys, comparer, from, hi - step);
                    SwapIfGreaterWithValues(keys, comparer, from, hi);
                    SwapIfGreaterWithValues(keys, comparer, hi - step, hi);
                    return;
                }

                InsertionSort(keys.Slice(from, partitionSize - from), comparer, step);
                return;
            }

            //if (depthLimit == 0)
            //{
            //    HeapSort_F(keys.Slice(from, partitionSize - from), comparer, step);
            //    return;
            //}
            //depthLimit--;

            int p = PickPivotAndPartition(keys.Slice(from, partitionSize - from), comparer, step);

            // Note we've already partitioned around the pivot and do not have to move the pivot again.
            IntroSort(keys[(p + step)..partitionSize], depthLimit, comparer, step);
            hi = p - step;
        }
    }

    private static int PickPivotAndPartition(Span<TPixel> keys, IComparer<TPixel> comparer, int step, int from, int to)
    {
        int d = to % step + step;
        int hi = to - d;
        int lo = from;

        // Compute median-of-three.  But also partition them, since we've done the comparison.
        // This must be a multiple of `step`.
        // TODO: Might find a more efficient way to unforce this later.
        int middle = ((hi - lo) >> 1) / step * step + lo;

        // Sort lo, mid and hi appropriately, then pick mid as the pivot.
        SwapIfGreaterWithValues(keys, comparer, lo, middle);  // swap the low with the mid point
        SwapIfGreaterWithValues(keys, comparer, lo, hi);      // swap the low with the high
        SwapIfGreaterWithValues(keys, comparer, middle, hi); // swap the middle with the high

        TPixel pivot = keys[middle];
        Swap(keys, middle, hi - step);
        int left = 0, right = hi - step;  // We already partitioned lo and hi and put the pivot in hi - step.  And we pre-increment & decrement below.

        while (left < right)
        {
            while (comparer.Compare(keys[(left += step)], pivot) < 0) ;
            while (comparer.Compare(pivot, keys[(right -= step)]) < 0) ;

            if (left >= right)
                break;

            Swap(keys, left, right);
        }

        // Put pivot in the right location.
        if (left != hi - step)
        {
            Swap(keys, left, hi - step);
        }
        return left;
    }
    #endregion


    #region Steppedieどゅ
    private void IntrospectiveSort(Span<TPixel> keys, IComparer<TPixel> comparer, int step)
    {
        int n = keys.Length / step;

        if (n > 1)
        {
            IntroSort(keys, 2 * (BitOperations.Log2((uint)n) + 1), comparer, step);
        }
    }

    private static void IntroSort(Span<TPixel> keys, int depthLimit, IComparer<TPixel> comparer, int step)
    {
        int d = keys.Length % step + step;
        int hi = keys.Length - d;


        while (hi > 0)
        {
            int partitionSize = hi + d;

            if (hi <= IntrosortSizeThreshold * step)
            {
                if (hi == 2 * step)
                {
                    SwapIfGreaterWithValues(keys, comparer, 0, hi);
                    return;
                }

                if (hi == 3 * step)
                {
                    SwapIfGreaterWithValues(keys, comparer, 0, hi - step);
                    SwapIfGreaterWithValues(keys, comparer, 0, hi);
                    SwapIfGreaterWithValues(keys, comparer, hi - step, hi);
                    return;
                }

                InsertionSort(keys.Slice(0, partitionSize), comparer, step);
                return;
            }

            //if (depthLimit == 0)
            //{
            //    // We don't have a good heap sort method for this yet :(
            //    //HeapSort(keys.Slice(0, partitionSize), comparer, step); 
            //    InsertionSort(keys.Slice(0, partitionSize), comparer, step);

            //    return;
            //}
            //depthLimit--;

            int p = PickPivotAndPartition(keys.Slice(0, partitionSize), comparer, step);

            // Note we've already partitioned around the pivot and do not have to move the pivot again.
            IntroSort(keys[(p + step)..partitionSize], depthLimit, comparer, step);
            hi = p - step;
        }
    }

    private static int PickPivotAndPartition(Span<TPixel> keys, IComparer<TPixel> comparer, int step)
    {
        int d = keys.Length % step + step;
        int hi = keys.Length - d;

        // Compute median-of-three.  But also partition them, since we've done the comparison.
        // This must be a multiple of `step`.
        // TODO: Might find a more efficient way to unforce this later.
        int middle = (hi >> 1) / step * step; //compiler might remove this? careful

        // Sort lo, mid and hi appropriately, then pick mid as the pivot.
        SwapIfGreaterWithValues(keys, comparer, 0, middle);  // swap the low with the mid point
        SwapIfGreaterWithValues(keys, comparer, 0, hi);      // swap the low with the high
        SwapIfGreaterWithValues(keys, comparer, middle, hi); // swap the middle with the high

        TPixel pivot = keys[middle];
        Swap(keys, middle, hi - step);
        int left = 0, right = hi - step;  // We already partitioned lo and hi and put the pivot in hi - step.  And we pre-increment & decrement below.

        while (left < right)
        {
            while (comparer.Compare(keys[left += step], pivot) < 0) ;
            while (comparer.Compare(pivot, keys[right -= step]) < 0) ;

            if (left >= right)
                break;

            Swap(keys, left, right);
        }

        // Put pivot in the right location.
        if (left != hi - step)
        {
            Swap(keys, left, hi - step);
        }
        return left;
    }
    #endregion

    #region From To
    private void IntrospectiveSort(Span<TPixel> keys, IComparer<TPixel> comparer, int from, int to)
    {
        var n = (uint)(to - from);
        var depthLimit = 2 * (BitOperations.Log2(n) + 1);

        if (n == keys.Length)
        {
            IntroSort(keys, depthLimit, comparer);
        }
        else if (n > 1U)
        {
            IntroSort(keys, depthLimit, comparer, from, to);
        }
    }

    private static void IntroSort(Span<TPixel> keys, int depthLimit, IComparer<TPixel> comparer, int from, int to)
    {
        int hi = to - 1;

        while (hi > from)
        {
            int partitionSize = hi + 1;

            if (partitionSize <= IntrosortSizeThreshold)
            {
                Debug.Assert(partitionSize >= 2);

                if (partitionSize == 2)
                {
                    SwapIfGreaterWithValues(keys, comparer, from, hi);
                    return;
                }

                if (partitionSize == 3)
                {
                    SwapIfGreaterWithValues(keys, comparer, from, hi - 1);
                    SwapIfGreaterWithValues(keys, comparer, from, hi);
                    SwapIfGreaterWithValues(keys, comparer, hi - 1, hi);
                    return;
                }

                InsertionSort(keys.Slice(from, partitionSize-from), comparer);
                return;
            }

            if (depthLimit == 0)
            {
                HeapSort(keys.Slice(from, partitionSize-from), comparer);
                return;
            }

            depthLimit--;

            int p = PickPivotAndPartition(keys.Slice(from, partitionSize-from), comparer, from, to);

            // Note we've already partitioned around the pivot and do not have to move the pivot again.
            IntroSort(keys[(p + 1)..partitionSize], depthLimit, comparer, from, to);
            hi = p - 1;
        }
    }

    private static int PickPivotAndPartition(Span<TPixel> keys, IComparer<TPixel> comparer, int from, int to)
    {
        int hi = to - 1;
        int lo = from;

        // Compute median-of-three.  But also partition them, since we've done the comparison.
        int middle = ((hi-lo) >> 1) + lo; // might be inaccurate

        // Sort lo, mid and hi appropriately, then pick mid as the pivot.
        SwapIfGreaterWithValues(keys, comparer, lo, middle);  // swap the low with the mid point
        SwapIfGreaterWithValues(keys, comparer, lo, hi);      // swap the low with the high
        SwapIfGreaterWithValues(keys, comparer, middle, hi); // swap the middle with the high

        TPixel pivot = keys[middle];
        Swap(keys, middle, hi - 1);
        int left = 0, right = hi - 1;  // We already partitioned lo and hi and put the pivot in hi - 1.  And we pre-increment & decrement below.

        while (left < right)
        {
            while (comparer.Compare(keys[++left], pivot) < 0) ;
            while (comparer.Compare(pivot, keys[--right]) < 0) ;

            if (left >= right)
                break;

            Swap(keys, left, right);
        }

        // Put pivot in the right location.
        if (left != hi - 1)
        {
            Swap(keys, left, hi - 1);
        }

        return left;
    }
    #endregion

    #endregion


    #region Heap Sort
    #region Original
    private static void HeapSort(Span<TPixel> span, IComparer<TPixel> comparer)
    {
        int n = span.Length;

        for (int i = n >> 1; i >= 1; i--)
        {
            DownHeap_OG(span, i, n, 0, comparer);
        }

        for (int i = n; i > 1; i--)
        {
            Swap(span, 0, i - 1);
            DownHeap_OG(span, 1, i - 1, 0, comparer);
        }
    }
    private static void DownHeap_OG(Span<TPixel> span, int i, int n, int lo, IComparer<TPixel> comparer)
    {
        TPixel d = span[lo + i - 1];

        while (i <= n >> 1)
        {
            int child = 2 * i;
            if (child < n && comparer.Compare(span[lo + child - 1], span[lo + child]) < 0)
            {
                child++;
            }

            if (!(comparer.Compare(d, span[lo + child - 1]) < 0))
            {
                break;
            }

            span[lo + i - 1] = span[lo + child - 1];
            i = child;
        }

        span[lo + i - 1] = d;
    }
    #endregion

    #region From To
    private static void HeapSort(Span<TPixel> span, IComparer<TPixel> comparer, int from, int to)
    {
        // build max heap
        for (int i = to >> 1; i >= from + 1; i --)
        {
            DownHeap(span, i, to - from, from, comparer);
        }

        // build sorted span from heap
        for (int i = to; i > from + 1; i --)
        {
            Swap(span, from, i - 1);
            DownHeap(span, 1, i - 1 - from, from, comparer);
        }
    }
    private static void DownHeap(Span<TPixel> span, int i, int to, int from, IComparer<TPixel> comparer)
    {
        TPixel d = span[from + i - 1];

        while (i <= to >> 1)
        {
            int child = i << 1;

            if (child < to && comparer.Compare(span[from + child - 1], span[from + child]) < 0)
            {
                child += 1;
            }

            if (comparer.Compare(d, span[from + child - 1]) >= 0)
            {
                break;
            }

            span[from + i - 1] = span[from + child - 1];

            i = child;
        }

        span[from + i - 1] = d;
    }
    #endregion

    #region From To with Fake Step
    private static void HeapSort(Span<TPixel> span, IComparer<TPixel> comparer, int step, int from, int to)
    {
        int i, j;

        // Create a copy of the original span with every 'step'-th element
        TPixel[] sortedElements = new TPixel[(to-from - 1) / step + 1];

        for (i = from, j = 0; j < sortedElements.Length; i += step, j++)
        {
            sortedElements[j] = span[i];
        }

        // Sort the copied span
        HeapSort(sortedElements, comparer);

        // Merge the sorted copied span back into the original span
        for (i = from, j = 0; j < sortedElements.Length; i += step, j++)
        {
            span[i] = sortedElements[j];
        }
    }
    #endregion

    #region Fake Step
    private static void HeapSort_F(Span<TPixel> span, IComparer<TPixel> comparer, int step)
    {
        int n = span.Length;
        int j, i;

        // Create a copy of the original span with every 'step'-th element
        TPixel[] sortedElements = new TPixel[(n-1) / step + 1];

        for (i = 0, j = 0; j < sortedElements.Length; i += step, j++)
        {
            sortedElements[j] = span[i];
        }

        // Sort the copied span
        HeapSort(sortedElements, comparer);

        // Merge the sorted copied span back into the original span
        for (i = 0, j = 0; j < sortedElements.Length; i += step, j++)
        {
            span[i] = sortedElements[j];
        }
    }
    #endregion

    #region Step (Does not work)
    private static void HeapSort(Span<TPixel> span, IComparer<TPixel> comparer, int step)
    {
        int n = span.Length;

        for (int i = n >> step; i >= step; i -= step)
        {
            DownHeap(span, i, n, 0, comparer, step);
        }

        for (int i = n; i > step; i -= step)
        {
            Swap(span, 0, i - step);
            DownHeap(span, step, i - step, 0, comparer, step);
        }
    }
    private static void DownHeap(Span<TPixel> span, int i, int n, int lo, IComparer<TPixel> comparer, int step)
    {
        TPixel d = span[lo + i - step];

        while (i <= n >> step)
        {
            int child = i << step;
            if (child < n && comparer.Compare(span[lo + child - step], span[lo + child]) < 0)
            {
                child += step;
            }

            if (!(comparer.Compare(d, span[lo + child - step]) < 0))
            {
                break;
            }

            span[lo + i - step] = span[lo + child - step];
            i = child;
        }

        span[lo + i - step] = d;
    }
    #endregion
    #endregion


    #region Insertion Sort

    /// <summary>
    /// 
    /// </summary>
    /// <param name="keys"></param>
    /// <param name="comparer"></param>
    /// <param name="stepper"></param>
    /// <param name="from">inclusive</param>
    /// <param name="to">exclusive</param>
    private static void InsertionSort(Span<TPixel> keys, IComparer<TPixel> comparer, int step, int from, int to)
    {
        for (int i = from; i < (to - step); i += step)
        {
            TPixel t = keys[i + step];

            int j = i;
            while (j >= from && comparer.Compare(t, keys[j]) < 0)
            {
                keys[j + step] = keys[j];
                j -= step;
            }

            keys[j + step] = t;
        }
    }

    private static void InsertionSort(Span<TPixel> keys, IComparer<TPixel> comparer, int step)
    {
        if (keys.Length > 500) Console.WriteLine("InsertionSort over 500");

        for (int i = 0; i < (keys.Length - step); i += step)
        {
            TPixel t = keys[i + step];

            int j = i;
            while (j >= 0 && comparer.Compare(t, keys[j]) < 0)
            {
                keys[j + step] = keys[j];
                j -= step;
            }

            keys[j + step] = t;
        }
    }

    private static void InsertionSort(Span<TPixel> keys, IComparer<TPixel> comparer)
    {
        for (int i = 0; i < keys.Length - 1; i += 1)
        {
            TPixel t = keys[i + 1];

            int j = i;
            while (j >= 0 && comparer.Compare(t, keys[j]) < 0)
            {
                keys[j + 1] = keys[j];
                j -= 1;
            }

            keys[j + 1] = t;
        }
    }
    #endregion


    #region Utilities
    private unsafe static void SwapIfGreaterWithValues(Span<TPixel> keys, IComparer<TPixel> comparer, int i, int j)
    {
        if (comparer.Compare(keys[i], keys[j]) > 0)
        {
            Swap(keys, i, j);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void Swap(Span<TPixel> keys, int i, int j)
    {
        TPixel k = keys[i];
        keys[i] = keys[j];
        keys[j] = k;
    }
    #endregion
}

