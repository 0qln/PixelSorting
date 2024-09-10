using Sorting.Pixels.KeySelector;

namespace Sorting;

public partial class Sorter<TPixel>
    where TPixel : struct
{
    /// <summary>
    /// The pigeonhole sort algorithm.
    /// Scales very well with large input sizes.
    /// </summary>
    public class PigeonholeSorter : ISorter
    {
        private readonly IOrderedKeySelector<TPixel> _selector;
        private readonly List<TPixel>[] _auxiliary;

        /// <summary>
        /// Constructs a new instance of the <see cref="PigeonholeSorter"/> class.
        /// </summary>
        /// <param name="selector"></param>
        public PigeonholeSorter(IOrderedKeySelector<TPixel> selector)
        {
            _selector = selector;
            _auxiliary = new List<TPixel>[_selector.GetCardinality()];
            for (var hole = 0; hole < _auxiliary.Length; hole++)
                _auxiliary[hole] = new List<TPixel>();
        }

        public void Sort(PixelSpan2DRun span)
        {
            var expectedDistribution = span.ItemCount / _selector.GetCardinality();

            for (var hole = 0; hole < _auxiliary.Length; hole++)
            {
                _auxiliary[hole].EnsureCapacity((int)expectedDistribution);
            }

            for (uint item = 0; item < span.ItemCount; item++)
            {
                var pixel = span[item];
                _auxiliary[_selector.GetKey(pixel)].Add(pixel);
            }

            uint i = 0;
            for (var key = 0; key < _auxiliary.Length; key++)
            {
                for (var item = 0; item < _auxiliary[key].Count; item++)
                {
                    span[i++] = _auxiliary[key][item];
                }
            }

            for (var hole = 0; hole < _auxiliary.Length; hole++)
            {
                _auxiliary[hole].Clear();
            }
        }

        [Obsolete]
        public void Sort(PixelSpan2D span)
        {
            var expectedDistribution = span.ItemCount / _selector.GetCardinality();

            for (var hole = 0; hole < _auxiliary.Length; hole++)
            {
                _auxiliary[hole].EnsureCapacity((int)expectedDistribution);
            }

            for (uint item = 0; item < span.ItemCount; item++)
            {
                var pixel = span[item];
                _auxiliary[_selector.GetKey(pixel)].Add(pixel);
            }

            uint i = 0;
            for (var key = 0; key < _auxiliary.Length; key++)
            {
                for (var item = 0; item < _auxiliary[key].Count; item++)
                {
                    span[i++] = _auxiliary[key][item];
                }
            }

            for (var hole = 0; hole < _auxiliary.Length; hole++)
            {
                _auxiliary[hole].Clear();
            }
        }

        public object Clone()
        {
            return new PigeonholeSorter((IOrderedKeySelector<TPixel>)_selector.Clone());
        }
    }
}