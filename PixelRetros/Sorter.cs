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

    public unsafe void HeapTesting(IComparer<TPixel> comparer)
    {
        int n = 10;
        int idx = 0;

        void* ptr = (_bmpData.Scan0).ToPointer();
        int bytes = _bmpData.Height * _bmpData.Width;
        Span<TPixel> span = new Span<TPixel>(ptr, bytes);


        void Reset(Span<TPixel> span)
        {
            span = new Span<TPixel>(ptr, bytes);
            for (byte i = 0; i < n - 1; i++)
            {
                span[i] = (TPixel)(object)new Pixel_24bit(i, i, i);
            }
        }
        void Print(Span<TPixel> span)
        {
            for (int i = 0; i < n; i++)
            {
                Console.WriteLine(span[i]);
            }
            Console.WriteLine();
        }

        Reset(span);
        Print(span);


        Reset(span);
        HeapSort_OG(span.Slice(0, n), comparer);
        Print(span);


        Reset(span);
        HeapSort(span, comparer, 1, 0, n);
        Print(span);

        //Reset(span);
        //HeapSort(span, comparer, 1, 1, n);
        //Print(span);

    }


    private delegate int IndexSelector(int i);

    [Benchmark]
    public unsafe void BenchmarkSort()
    {
        StdSortComparer((IComparer<TPixel>)new Comparer24bit.Ascending.Blue());
    }
    [Benchmark]
    public unsafe void BenchmarkSortInlined()
    {
        StdSortComparer((IComparer<TPixel>)new Comparer24bit.Ascending.BlueInlined());
    }


    public unsafe void StdSortComparer(IComparer<TPixel> comparer)
    {
        void* ptr = (_bmpData.Scan0).ToPointer();
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
            InsertionSort(span, comparer, step, from(i), to(i));
        }
    }

    #region Introspective Sort
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
                throw new NotImplementedException();
                //HeapSort(keys.Slice(0, partitionSize), comparer);
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


    #region Heap Sort
    #region Original
    private static void HeapSort_OG(Span<TPixel> span, IComparer<TPixel> comparer)
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


    private static void HeapSort(Span<TPixel> span, IComparer<TPixel> comparer, int step, int from, int to)
    {
        int n = to;
        for (int i = n >> 1; i >= 1; i--)
        {
            DownHeap(
                span,
                i, n, 0,
                comparer,
                step: step, from: from, to: to);
        }

        for (int i = n; i > 1; i--)
        {
            Swap(span, 0, i - 1);

            DownHeap(
                span: span,
                i: 1, n: i - 1, lo: 0,
                comparer: comparer,
                step: step, from: from, to: to);
        }
    }
    private static void DownHeap(Span<TPixel> span, int i, int n, int lo, IComparer<TPixel> comparer, int step, int from, int to)
    {
        TPixel d = span[lo + i - 1];

        while (i <= n / 2)
        {
            int child = 2 * i;
            if (child < n && comparer.Compare(span[lo + child - 1], span[lo + child]) < 0)
            {
                child += 1;
            }

            if (comparer.Compare(d, span[lo + child - 1]) >= 0)
            {
                break;
            }

            span[lo + i - 1] = span[lo + child - 1];

            i = child;
        }

        span[lo + i - 1] = d;
    }

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
        for (int i = from; i < to - step; i += step)
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
