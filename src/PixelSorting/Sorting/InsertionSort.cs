﻿using System.Diagnostics;
using Sorting.Pixels;

namespace Sorting;

public partial class Sorter<TPixel>
    where TPixel : struct
{
    public readonly struct InsertionSorter(IPixelComparer<TPixel> comparer) : ISorter
    {
        public object Clone()
        {
            return new InsertionSorter((IPixelComparer<TPixel>)comparer.Clone());
        }

        public void Sort(PixelSpan2D span)
        {
            InsertionSort(span, comparer);
        }

        public void Sort(PixelSpan2DRun span)
        {
            InsertionSort(span, comparer);
        }
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
        for (var i = from; i < to - step; i += step)
        {
            var t = span[i + step];

            var j = i;
            while (j >= from && comparer.Compare(t, span[j]) < 0)
            {
                span[j + step] = span[j];
                j -= step;
            }

            span[j + step] = t;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="span"></param>
    /// <param name="comparer"></param>
    public static void InsertionSort(PixelSpan span, IComparer<TPixel> comparer)
    {
        for (var i = 0; i < span.ItemCount - 1; ++i)
        {
            var t = span[i + 1];

            var j = i;
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
    public static void InsertionSort(PixelSpan2D span, IComparer<TPixel> comparer)
    {
        for (var i = 0u; i < span.ItemCount - 1; ++i)
        {
            var t = span[i + 1];

            var j = i;
            while (comparer.Compare(t, span[j]) < 0)
            {
                span[j + 1] = span[j];
                if (j == 0) break;
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
    public static void InsertionSort(PixelSpan2D span, IComparer<TPixel> comparer, uint lo, uint hi)
    {
        Debug.Assert(hi < span.ItemCount);

        for (var i = lo; i < hi; ++i)
        {
            var t = span[i + 1];

            var j = i;
            while (j >= lo && comparer.Compare(t, span[j]) < 0)
            {
                span[j + 1] = span[j];
                if (j == 0) break;
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
    public static void InsertionSort(PixelSpan2DRun span, IComparer<TPixel> comparer)
    {
        for (var i = 0u; i < span.ItemCount - 1; ++i)
        {
            var t = span[i + 1];

            var j = i;
            while (comparer.Compare(t, span[j]) < 0)
            {
                span[j + 1] = span[j];
                if (j == 0) break;
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
    public static void InsertionSort(PixelSpan2DRun span, IComparer<TPixel> comparer, uint lo, uint hi)
    {
        Debug.Assert(hi < span.ItemCount);

        for (var i = lo; i < hi; ++i)
        {
            var t = span[i + 1];

            var j = i;
            while (j >= lo && comparer.Compare(t, span[j]) < 0)
            {
                span[j + 1] = span[j];
                if (j == 0) break;
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
    public static void InsertionSort(UnsafePixelSpan2D span, IComparer<TPixel> comparer, uint lo, uint hi)
    {
        Debug.Assert(hi < span.ItemCount);

        for (var i = lo; i < hi; ++i)
        {
            var t = span[i + 1];

            var j = i;
            while (j >= lo && comparer.Compare(t, span[j]) < 0)
            {
                span[j + 1] = span[j];
                if (j == 0) break;
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

        for (var i = lo; i < hi; ++i)
        {
            var t = span[i + 1];

            var j = i;
            while (j >= lo && comparer.Compare(t, span[j]) < 0)
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
    public static void InsertionSort(FloatingPixelSpan span, IComparer<TPixel> comparer, int lo, int hi)
    {
        Debug.Assert(hi < span.ItemCount);

        for (var i = lo; i < hi; ++i)
        {
            var t = span[i + 1];

            var j = i;
            while (j >= lo && comparer.Compare(t, span[j]) < 0)
            {
                span[j + 1] = span[j];
                --j;
            }

            span[j + 1] = t;
        }
    }
}