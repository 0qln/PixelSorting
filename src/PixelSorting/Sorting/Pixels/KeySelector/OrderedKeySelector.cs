using Sorting.Pixels._32;

namespace Sorting.Pixels.KeySelector;

/// <summary>
/// A key selector
/// </summary>
/// <typeparam name="TPixel"></typeparam>
public interface IKeySelector<in TPixel> : ICloneable
    where TPixel : struct
{
    /// <summary>
    /// Returns the key for the given value.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public int GetKey(TPixel value);

    /// <summary>
    /// Returns the cardinality of the key. <br/>
    /// For example a red channel selector has a cardinality of 256, because it only
    /// looks at the R property, which only has 256 possible values.
    /// </summary>
    /// <returns></returns>
    public int GetCardinality();
}

/// <summary>
/// A key selector.
/// </summary>
public abstract class OrderedKeySelector
{
    /// <summary>
    /// Order in ascending order.
    /// </summary>
    public abstract class Ascending
    {
        /// <summary>
        /// Red key
        /// </summary>
        public class Red : IKeySelector<Pixel32bitUnion>
        {
            /// <inheritdoc />
            public int GetCardinality() => 256;

            /// <inheritdoc />
            public int GetKey(Pixel32bitUnion value) => value.R;

            /// <inheritdoc />
            public object Clone() => new Red();
        }
    }

    /// <summary>
    /// Order in descending order.
    /// </summary>
    public abstract class Descending
    {
        /// <summary>
        /// Red key
        /// </summary>
        public class Red : IKeySelector<Pixel32bitUnion>
        {
            /// <inheritdoc />
            public int GetCardinality() => 256;

            /// <inheritdoc />
            public int GetKey(Pixel32bitUnion value) => 255 - value.R;

            /// <inheritdoc />
            public object Clone() => new Red();
        }
    }
}