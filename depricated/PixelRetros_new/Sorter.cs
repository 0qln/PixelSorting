#pragma warning disable CA1416 // Validate platform compatibility


using BenchmarkDotNet.Attributes;
using System;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Cryptography;

namespace SortingLibrary;

public class Sorter<TPixel>
{
    // This is the threshold where Introspective sort switches to Insertion sort.
    // Empirically, 16 seems to speed up most cases without slowing down others, at least for integers.
    // Large value types may benefit from a smaller number.
    public const int IntrosortSizeThreshold = 16;


    public SortType SortType { get; set; } = SortType.Red;
    public SortDirection SortDirection { get; set; } = SortDirection.Horizontal;
    public SortOrder SortOrder { get; set; } = SortOrder.Ascending;
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




    public Span<IComparable> BuildSpanFromPixels() => BuildSpanFromPixels(0);

    private Span<IComparable> BuildSpanFromPixels(int scanIdx)
    {
        if (SortDirection == SortDirection.Horizontal)
        {
            return new Span<IComparable>(_pixels?.Chunk(_bmpData.Width).ElementAt(scanIdx));
        }
        else
        {
            throw new NotImplementedException();
        }
    }


    private unsafe delegate TPixel* PixelSelector(int index, Span<TPixel> range);
    private unsafe delegate int IndexSelector(int index);


    [Benchmark]
    public unsafe void StdSortComparer()
    {
        //void* ptr = (_bmpData.Scan0).ToPointer();
        //int bytes = 100 * _bytesPerPixel;
        //var span = new Span<TPixel>(ptr, bytes);

        //// test if this is being inlined
        //IndexSelector selector = index =>
        //{
        //    return 1 + index;
        //};
        //IndexSelector stepper = index =>
        //{
        //    return index;
        //};

        ////foreach (var p in span) Console.WriteLine(p);

        //Console.WriteLine("Start Sorting");
        //InsertionSort(span, (IComparer<TPixel>)new Comparer24bit_soA_stB(), selector, stepper);
        //Console.WriteLine("Stop Sorting");

        //foreach (var p in span) Console.WriteLine(p);



        if (SortDirection == SortDirection.Horizontal)
        {
            for (int row = 0; row < _bmpData.Height-1; row++)
            {
                int bytes = _bmpData.Stride;
                void* ptr = (_bmpData.Scan0 + bytes * row).ToPointer();
                var span = new Span<TPixel>(ptr, bytes);

                InsertionSort(span, (IComparer<TPixel>)new Comparer24bit_soA_stR());
                //IntrospectiveSort(span, (IComparer<TPixel>)new Comparer24bit_soA_stR());
            }
        }
        else
        {
            void* ptr = (_bmpData.Scan0).ToPointer();
            int bytes = _bmpData.Height * _bmpData.Stride;
            var span = new Span<TPixel>(ptr, bytes);


            for (int column = 0; column < _bmpData.Width; column++)
            {
                int step = _bmpData.Width;
                int from = 0 + column;
                int to = _bmpData.Height * _bmpData.Width + column;

                InsertionSort(span, (IComparer<TPixel>)new Comparer24bit_soA_stR(), step, from, to);
            }
        }
    }


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

                throw new NotImplementedException();
                //InsertionSort(keys.Slice(0, partitionSize), comparer);
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

    private static void HeapSort(Span<TPixel> keys, IComparer<TPixel> comparer)
    {
        int n = keys.Length;
        for (int i = n >> 1; i >= 1; i--)
        {
            DownHeap(keys, i, n, 0, comparer);
        }

        for (int i = n; i > 1; i--)
        {
            Swap(keys, 0, i - 1);
            DownHeap(keys, 1, i - 1, 0, comparer);
        }
    }

    private static void DownHeap(Span<TPixel> keys, int i, int n, int lo, IComparer<TPixel> comparer)
    {
        TPixel d = keys[lo + i - 1];

        while (i <= n >> 1)
        {
            int child = 2 * i;
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
}
