using Sorting.Pixels.KeySelector;

namespace Sorting;

public partial class Sorter<TPixel>
    where TPixel : struct
{
    public static void PigeonholeSort(PixelSpan2D span, IOrderedKeySelector<TPixel> selector)
    {
        // List<TPixel>[] auxilary = new List<TPixel>[selector.GetCardinality()];
        // var expectedDistribution = span.ItemCount / selector.GetCardinality();
        // for (var hole = 0; hole < auxilary.Length; hole++)
        // {
        //     auxilary[hole] = new List<TPixel>(expectedDistribution);
        // }
        //
        // for (var item = 0; item < span.ItemCount; item++)
        // {
        //     var pixel = span[item];
        //     auxilary[selector.GetKey(pixel)].Add(pixel);
        // }
        //
        // var i = 0;
        // for (var key = 0; key < auxilary.Length; key++)
        // {
        //     for (var item = 0; item < auxilary[key].Count; item++)
        //     {
        //         span[i++] = auxilary[key][item];
        //     }
        // }
    }
}