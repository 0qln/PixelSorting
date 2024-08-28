using Sorting.Pixels.KeySelector;

namespace Sorting;
// TODO?

public partial class Sorter<TPixel>
    where TPixel : struct
{
    public enum RadixOrder { MSD, LSD }

    public static void RadixSortBase10(PixelSpan2D span, IOrderedKeySelector<TPixel> selector, RadixOrder order)
    {
        if (order == RadixOrder.LSD)
        {
        }
        else
        {
        }
    }
}