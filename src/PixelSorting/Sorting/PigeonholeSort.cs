using Sorting.Pixels.KeySelector;

namespace Sorting;

public partial class Sorter<TPixel>
    where TPixel : struct
{
    public class PigeonholeSorter : ISorter
    {
        private readonly IOrderedKeySelector<TPixel> _selector;
        private readonly List<TPixel>[] _auxilary;

        public PigeonholeSorter(IOrderedKeySelector<TPixel> selector)
        {
            _selector = selector;
            _auxilary = new List<TPixel>[_selector.GetCardinality()];
            for (var hole = 0; hole < _auxilary.Length; hole++)
                _auxilary[hole] = new List<TPixel>();
        }

        public void Sort(PixelSpan2DRun span)
        {
            var expectedDistribution = span.ItemCount / _selector.GetCardinality();

            for (var hole = 0; hole < _auxilary.Length; hole++)
            {
                _auxilary[hole].EnsureCapacity((int)expectedDistribution);
            }

            for (uint item = 0; item < span.ItemCount; item++)
            {
                var pixel = span[item];
                _auxilary[_selector.GetKey(pixel)].Add(pixel);
            }

            uint i = 0;
            for (var key = 0; key < _auxilary.Length; key++)
            {
                for (var item = 0; item < _auxilary[key].Count; item++)
                {
                    span[i++] = _auxilary[key][item];
                }
            }

            for (var hole = 0; hole < _auxilary.Length; hole++)
            {
                _auxilary[hole].Clear();
            }
        }

        public void Sort(PixelSpan2D span)
        {
            var expectedDistribution = span.ItemCount / _selector.GetCardinality();

            for (var hole = 0; hole < _auxilary.Length; hole++)
            {
                _auxilary[hole].EnsureCapacity((int)expectedDistribution);
            }

            for (uint item = 0; item < span.ItemCount; item++)
            {
                var pixel = span[item];
                _auxilary[_selector.GetKey(pixel)].Add(pixel);
            }

            uint i = 0;
            for (var key = 0; key < _auxilary.Length; key++)
            {
                for (var item = 0; item < _auxilary[key].Count; item++)
                {
                    span[i++] = _auxilary[key][item];
                }
            }

            for (var hole = 0; hole < _auxilary.Length; hole++)
            {
                _auxilary[hole].Clear();
            }
        }

        public object Clone()
        {
            return new PigeonholeSorter((IOrderedKeySelector<TPixel>)_selector.Clone());
        }
    }
}