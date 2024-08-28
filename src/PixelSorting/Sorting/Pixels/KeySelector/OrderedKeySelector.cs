using Sorting.Pixels._32;

namespace Sorting.Pixels.KeySelector;

public interface IOrderedKeySelector<in TPixel>
    where TPixel : struct
{
    public int GetKey(TPixel value);

    // for example a red channel selector has a cardinality of 256, because it only
    // looks at the red property, which only has 256 possible values.
    public int GetCardinality();
}

public abstract class OrderedKeySelector
{
    public abstract class Ascending
    {
        public class Red : IOrderedKeySelector<Pixel32bitUnion>
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

    public abstract class Descending
    {
        public class Red : IOrderedKeySelector<Pixel32bitUnion>
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