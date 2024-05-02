using Sorting.Pixels.KeySelector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sorting
{

    public partial class Sorter<TPixel>
        where TPixel : struct
    {
        public static void PigeonholeSort(PixelSpan2D span, IOrderedKeySelector<TPixel> selector)
        {
            List<TPixel>[] auxilary = new List<TPixel>[selector.GetCardinality()];
            int expectedDistribution = span.ItemCount / selector.GetCardinality();
            for (int hole = 0; hole < auxilary.Length; hole++)
            {
                auxilary[hole] = new List<TPixel>(expectedDistribution);
            }

            for (int item = 0; item < span.ItemCount; item++)
            {
                TPixel pixel = span[item];
                auxilary[selector.GetKey(pixel)].Add(pixel);
            }

            int i = 0;
            for (int key = 0; key < auxilary.Length; key++)
            {
                for (int item = 0; item < auxilary[key].Count; item++)
                {
                    span[i++] = auxilary[key][item];
                }
            }
        }
    }
}
