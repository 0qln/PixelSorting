namespace Sorting;

public partial class Sorter<TPixel>
{
    public interface ISorter : ICloneable
    {
        public void Sort(PixelSpan2D span);
        public void Sort(PixelSpan2DRun span);
    }
}