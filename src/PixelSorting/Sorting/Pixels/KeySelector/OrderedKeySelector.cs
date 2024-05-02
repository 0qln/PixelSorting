using Sorting.Pixels._32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sorting.Pixels.KeySelector
{
    public interface IOrderedKeySelector<TPixel>
        where TPixel : struct
    {
        public int GetKey(TPixel value);

        // for example a red channel selector has a cardinality of 256, because it only
        // looks at the red property, which only has 256 possible values.
        public int GetCardinality();
    }

    public abstract partial class OrderedKeySelector
    {
        public abstract partial class Ascending
        {
            public abstract class Red
            {
                public class _32bitUnion : IOrderedKeySelector<Pixel32bitUnion>
                {
                    public int GetCardinality()
                    {
                        return 256;
                    }

                    public int GetKey(Pixel32bitUnion value)
                    {
                        return value.R;
                    }
                }
            }
        }

        public abstract partial class Descending
        {
            public abstract class Red
            {
                public class _32bitUnion : IOrderedKeySelector<Pixel32bitUnion>
                {
                    public int GetCardinality()
                    {
                        return 256;
                    }

                    public int GetKey(Pixel32bitUnion value)
                    {
                        return 255 - value.R;
                    }
                }
            }
        }
    }
}
