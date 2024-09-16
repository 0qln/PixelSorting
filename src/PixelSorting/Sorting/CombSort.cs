using Sorting.Pixels;

namespace Sorting;

public partial class Sorter<TPixel>
    where TPixel : struct
{
    /// <summary>
    /// The comb sort algorithm.
    /// </summary>
    /// <param name="comparer"></param>
    public class CombSorter(IPixelComparer<TPixel> comparer) : ISorter
    {
        /// <summary>
        /// The pureness with which the sorter sorts the span.
        /// Null means a perfect sort.
        /// </summary>
        public int? Pureness { get; set; }

        /// <summary>
        /// The comparer to use.
        /// </summary>
        public IPixelComparer<TPixel> Comparer { get; set; } = comparer;


        /// <inheritdoc />
        public object Clone()
        {
            return new CombSorter((IPixelComparer<TPixel>)Comparer.Clone())
            {
                Pureness = Pureness
            };
        }

        /// <inheritdoc />
        public void Sort(PixelSpan2DRun span)
        {
            if (!Pureness.HasValue)
            {
                CombSort(span, Comparer);
            }
            else
            {
                CombSort(span, Comparer, Pureness.Value);
            }
        }
    }

    /// <summary>
    /// The comb sort algorithm.
    /// </summary>
    /// <param name="span"></param>
    /// <param name="comparer"></param>
    public static void CombSort(PixelSpan span, IComparer<TPixel> comparer)
    {
        var gap = span.ItemCount;
        var shrink = 1.3f;
        var sorted = false;
        
        while (!sorted)
        {
            gap = (int)(gap / shrink);
            switch (gap)
            {
                case <= 1:
                    gap = 1;
                    sorted = true;
                    break;

                // We can make use of an optimization here, as there is
                // no request to keep the pureness low.
                case 9:
                case 10:
                    gap = 11;
                    break;
            }
        
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

    /// <summary>
    /// The comb sort algorithm.
    /// </summary>
    /// <param name="span"></param>
    /// <param name="comparer"></param>
    /// <param name="pureness"></param>
    public static void CombSort(PixelSpan span, IComparer<TPixel> comparer, int pureness)
    {
        var gap = span.ItemCount;
        var shrink = 1.3f;
        var sorted = false;
        
        for (var p = 0; p < pureness && !sorted; p++)
        {
            gap = (int)(gap / shrink);
            if (gap <= 1)
            {
                gap = 1;
                sorted = true;
            }
        
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


    /// <summary>
    /// The comb sort algorithm.
    /// </summary>
    /// <param name="span"></param>
    /// <param name="comparer"></param>
    public static void CombSort(PixelSpan2DRun span, IComparer<TPixel> comparer)
    {
        var gap = span.ItemCount;
        var shrink = 1.3f;
        var sorted = false;
        
        while (!sorted)
        {
            gap = (uint)(gap / shrink);
            switch (gap)
            {
                case <= 1:
                    gap = 1;
                    sorted = true;
                    break;

                // We can make use of an optimization here, as there is
                // no request to keep the pureness low.
                case 9:
                case 10:
                    gap = 11;
                    break;
            }
        
            for (uint i = 0; i + gap < span.ItemCount; i++)
            {
                if (comparer.Compare(span[i], span[i + gap]) > 0)
                {
                    Swap(span, i, i + gap);
                    sorted = false;
                }
            }
        }
    }

    /// <summary>
    /// The comb sort algorithm.
    /// </summary>
    /// <param name="span"></param>
    /// <param name="comparer"></param>
    /// <param name="pureness"></param>
    public static void CombSort(PixelSpan2DRun span, IComparer<TPixel> comparer, int pureness)
    {
        var gap = span.ItemCount;
        var shrink = 1.3f;
        var sorted = false;
        
        for (var p = 0; p < pureness && !sorted; p++)
        {
            gap = (uint)(gap / shrink);
            if (gap <= 1)
            {
                gap = 1;
                sorted = true;
            }
        
            for (uint i = 0; i + gap < span.ItemCount; i++)
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