namespace Sorting;

public partial class Sorter<TPixel>
{
    /// <summary>
    /// Implementers of this interface don't necessarily need
    /// to be thread-safe or support parallel sorting.
    /// </summary>
    public interface ISorter : ICloneable
    {
        /// <summary>
        /// Sort the span.
        /// </summary>
        /// <param name="span"></param>
        public void Sort(PixelSpan2DRun span);
    }
}