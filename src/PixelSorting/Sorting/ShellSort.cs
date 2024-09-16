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
        public int Pureness { get; set; }

        public IPixelComparer<TPixel> Comparer { get; set; } = comparer;

        public object Clone()
        {
            return new ShellSorter((IPixelComparer<TPixel>)Comparer.Clone())
            {
                Gaps = Gaps.ToArray(),
                Pureness = Pureness
            };
        }

        [Obsolete]
        public void Sort(PixelSpan2D span)
        {
            throw new NotImplementedException();
        }

        public void Sort(PixelSpan2DRun span)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Sort the array with a selected level of pureness.
    /// </summary>
    /// <param name="span"></param>
    /// <param name="comparer"></param>
    /// <param name="lo">Inlcusive</param>
    /// <param name="hi">Exclusive</param>
    /// <param name="pureness">
    /// An impure span is a not completely sorted span. Pureness increases overhead in 
    /// exponentially, where n is number of elements in the span. 
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