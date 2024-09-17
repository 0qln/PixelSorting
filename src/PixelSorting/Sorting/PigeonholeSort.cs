using Sorting.Pixels.KeySelector;

namespace Sorting;

public partial class Sorter<TPixel>
    where TPixel : struct
{
    /// <summary>
    /// The pigeonhole sort algorithm.
    /// Scales very well with large input sizes.
    /// Can cause memory issues with very large input sizes. (> 4k images)
    /// </summary>
    public class PigeonholeSorter : ISorter
    {
        /// <summary>
        /// The key selector.
        /// </summary>
        public IKeySelector<TPixel> Selector { get; }

        /// <summary>
        /// No pixels below this value will be sorted.
        /// If <see langword="null" />, there is no effect.
        /// </summary>
        public Threshold? Threshold { get; set; }

        private readonly List<TPixel>[] _auxiliary;

        /// <summary>
        /// Constructs a new instance of the <see cref="PigeonholeSorter"/> class.
        /// </summary>
        /// <param name="selector"></param>
        public PigeonholeSorter(IKeySelector<TPixel> selector)
        {
            Selector = selector;
            _auxiliary = new List<TPixel>[Selector.GetCardinality()];
            for (var hole = 0; hole < _auxiliary.Length; hole++)
                // We have no span input yet. So we can't make any guesses 
                // about the distribution of the input.
                _auxiliary[hole] = [];
        }

        /// <inheritdoc />
        public void Sort(PixelSpan2DRun span)
        {
            if (Threshold.HasValue)
            {
                // TODO: maybe the auxiliary allocations and capacity checks can be optimized.
                uint idx = 0;
                while (span.NextRun(Threshold.Value.Comparer, Threshold.Value.Value, ref idx, out var run))
                    _Sort(run);
            }
            else
            {
                _Sort(span);
            }
        }

        private void _Sort(PixelSpan2DRun span)
        {
            var expectedDistribution = span.ItemCount / Selector.GetCardinality();

            for (var hole = 0; hole < _auxiliary.Length; hole++)
            {
                _auxiliary[hole].EnsureCapacity((int)expectedDistribution);
            }

            for (uint item = 0; item < span.ItemCount; item++)
            {
                var pixel = span[item];
                _auxiliary[Selector.GetKey(pixel)].Add(pixel);
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

        /// <inheritdoc />
        public object Clone()
        {
            return new PigeonholeSorter((IKeySelector<TPixel>)Selector.Clone());
        }
    }
}