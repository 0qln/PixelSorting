using System.Diagnostics;
using Sorting.Pixels;

namespace Sorting;

public partial class Sorter<TPixel>
    where TPixel : struct
{
    /// <summary>
    /// The insertion sort algorithm.
    /// </summary>
    /// <param name="comparer">
    /// The comparer to use.
    /// </param>
    public class InsertionSorter(IPixelComparer<TPixel> comparer) : ISorter 
    {
        /// <summary>
        /// The comparer to use.
        /// </summary>
        public IPixelComparer<TPixel> Comparer { get; set; } = comparer;

        /// <summary>
        /// No pixels below this value will be sorted.
        /// If <see langword="null"/>, there is no effect.
        /// </summary>
        public Threshold? Threshold { get; set; }


        /// <inheritdoc />
        public object Clone()
        {
            return new InsertionSorter((IPixelComparer<TPixel>)Comparer.Clone())
            {
                Threshold = Threshold
            };
        }

        /// <inheritdoc />
        public void Sort(PixelSpan2DRun span)
        {
            if (Threshold.HasValue)
            {
                uint idx = 0;
                while (span.NextRun(Threshold.Value.Comparer, Threshold.Value.Value, ref idx, out var run))
                    InsertionSort(run, Comparer);
            }
            else
            {
                InsertionSort(span, Comparer);
            }
        }
    }

    /// <summary></summary>
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
    [Obsolete]
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
    [Obsolete]
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

#if EXPERIMENTAL
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
#endif

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
}