using Sorting.Pixels;

namespace Sorting;

public partial class Sorter<TPixel>
    where TPixel : struct
{
    /// <summary>
    /// The shell sort algorithm.
    /// </summary>
    public class ShellSorter(IPixelComparer<TPixel> comparer) : ISorter
    {
        /// <summary>
        /// Ciura gap sequence.
        /// </summary>
        public static readonly ReadOnlyMemory<uint> DefaultGaps = (uint[])[701, 301, 132, 57, 23, 10, 4, 1];

        /// <summary>
        /// The gaps for the shell sort algorithm.
        /// Default is the Ciura gap sequence.
        /// </summary>
        public uint[] Gaps = [701, 301, 132, 57, 23, 10, 4, 1];

        /// <summary>
        /// The maximum pureness for the shell sort algorithm.
        /// </summary>
        public int ShellPurenessMax => Gaps.Length - 1;

        /// <summary>
        /// The pureness for the shell sort algorithm.
        /// </summary>
        public int? Pureness { get; set; }

        /// <summary>
        /// The comparer for the shell sort algorithm.
        /// </summary>
        public IPixelComparer<TPixel> Comparer { get; set; } = comparer;

        /// <summary>
        /// No pixels below this value will be sorted.
        /// If <see langword="null" />, there is no effect.
        /// </summary>
        public Threshold? Threshold { get; set; }


        /// <inheritdoc />
        public object Clone()
        {
            return new ShellSorter((IPixelComparer<TPixel>)Comparer.Clone())
            {
                Gaps = Gaps.ToArray(),
                Pureness = Pureness,
                Threshold = Threshold
            };
        }

        /// <inheritdoc />
        public void Sort(PixelSpan2DRun span)
        {
            if (!Pureness.HasValue)
            {
                if (!Threshold.HasValue)
                {
                    ShellSort(span, Comparer, Gaps);
                }
                else
                {
                    uint idx = 0;
                    while (span.NextRun(Threshold.Value.Comparer, Threshold.Value.Value, ref idx, out var run))
                        ShellSort(run, Comparer, Gaps);
                }
            }
            else
            {
                if (!Threshold.HasValue)
                {
                    ShellSort(span, Comparer, Pureness.Value, Gaps);
                }
                else
                {
                    uint idx = 0;
                    while (span.NextRun(Threshold.Value.Comparer, Threshold.Value.Value, ref idx, out var run))
                        ShellSort(run, Comparer, Pureness.Value, Gaps);
                }
            }
        }
    }

    /// <summary>
    /// Sort the array with a selected level of pureness.
    /// </summary>
    /// <param name="span"></param>
    /// <param name="comparer"></param>
    /// <param name="lo">Inclusive</param>
    /// <param name="hi">Exclusive</param>
    /// <param name="pureness">
    /// An impure span is a not completely sorted span.
    /// Pureness increases overhead exponentially. 
    /// </param>
    /// <param name="gaps"></param>
    /// <exception cref="ArgumentException"></exception>
    public static void ShellSort(PixelSpan2DRun span, IComparer<TPixel> comparer, uint lo, uint hi, int pureness, Span<uint> gaps)
    {
        if (pureness < 0 || pureness >= gaps.Length)
            throw new ArgumentException(nameof(pureness));
        
        for (uint gapIndex = 0; gapIndex <= pureness; gapIndex++)
        {
            for (uint gap = gaps[(int)gapIndex], i = gap + lo; i < hi; i++)
            {
                var temp = span[i];
                var j = i;
        
                while ((j >= gap) && (comparer.Compare(temp, span[j - gap]) < 0))
                {
                    span[j] = span[j - gap];
                    j -= gap;
                }
        
                span[j] = temp;
            }
        }
    }

    /// <summary>
    /// Sort the array with a selected level of pureness.
    /// </summary>
    /// <param name="span"></param>
    /// <param name="comparer"></param>
    /// <param name="pureness">
    /// An impure span is a not completely sorted span.
    /// Pureness increases overhead exponentially. 
    /// </param>
    /// <param name="gaps"></param>
    /// <exception cref="ArgumentException"></exception>
    public static void ShellSort(PixelSpan2DRun span, IComparer<TPixel> comparer, int pureness, Span<uint> gaps)
    {
        if (pureness < 0 || pureness >= gaps.Length)
            throw new ArgumentException(nameof(pureness));

        for (uint gapIndex = 0; gapIndex <= pureness; gapIndex++)
        {
            for (uint gap = gaps[(int)gapIndex], i = gap; i < span.ItemCount; i++)
            {
                var temp = span[i];
                var j = i;

                while ((j >= gap) && (comparer.Compare(temp, span[j - gap]) < 0))
                {
                    span[j] = span[j - gap];
                    j -= gap;
                }

                span[j] = temp;
            }
        }
    }

    /// <summary>
    /// Shell sort the pixel span.
    /// </summary>
    /// <param name="span"></param>
    /// <param name="comparer"></param>
    /// <param name="gaps"></param>
    public static void ShellSort(PixelSpan2DRun span, IComparer<TPixel> comparer, Span<uint> gaps)
    {
        for (int gapIndex = 0; gapIndex < gaps.Length; gapIndex++)
        {
            for (uint gap = gaps[gapIndex], i = gap; i < span.ItemCount; i++)
            {
                var temp = span[i];
                var j = i;

                while (j >= gap && comparer.Compare(temp, span[j - gap]) < 0)
                {
                    span[j] = span[j - gap];
                    j -= gap;
                }

                span[j] = temp;
            }
        }
    }

    /// <summary>
    /// Shell sort the pixel span.
    /// </summary>
    /// <param name="span"></param>
    /// <param name="comparer"></param>
    /// <param name="gaps"></param>
    public static void ShellSort(PixelSpan span, IComparer<TPixel> comparer, uint[] gaps)
    {
        for (var gapIndex = 0; gapIndex < gaps.Length; gapIndex++)
        {
            for (int gap = (int)gaps[gapIndex], i = gap; i < span.ItemCount; i++)
            {
                var temp = span[i];
                var j = i;

                while (j >= gap && comparer.Compare(temp, span[j - gap]) < 0)
                {
                    span[j] = span[j - gap];
                    j -= gap;
                }

                span[j] = temp;
            }
        }
    }
}