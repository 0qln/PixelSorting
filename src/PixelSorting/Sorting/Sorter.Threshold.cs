using Sorting.Pixels;

namespace Sorting;

public partial class Sorter<TPixel>
{
    /// <summary>
    /// The threshold to use.
    /// </summary>
    public readonly struct Threshold
    {
        /// <summary>
        /// The threshold value.
        /// </summary>
        public required TPixel Value { get; init; }

        /// <summary>
        /// The comparer to use.
        /// </summary>
        public required IPixelComparer<TPixel> Comparer { get; init; }
    }
}