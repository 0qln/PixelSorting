using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Sorting
{
    public partial class Sorter
    {
        public SortType SortType { get; set; } = SortType.Red;
        public SortDirection SortDirection { get; set; } = SortDirection.Horizontal;
        public SortOrder SortOrder { get; set; } = SortOrder.Ascending;

        private readonly int _imageWidth, _imageHeight, _imageStride, _bytesPerPixel;
        private byte[] _imageData;
        private ComparablePixel[] _pixels;
        private SimpleComparablePixel[] _simplePixels;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="pixelData">Expected to be 3 bytes per pixel in RGB format.</param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public Sorter(byte[] pixelData, int width, int height, int stride)
        {
            _imageStride = stride;
            _imageHeight = height;
            _imageWidth = width;
            _imageData = pixelData;
            _bytesPerPixel = stride / width;

            if (_bytesPerPixel != 3) throw new NotImplementedException();

            _pixels = _imageData.Chunk(_bytesPerPixel).Select(x => new ComparablePixel(x)).ToArray();
            _simplePixels = _imageData.Chunk(_bytesPerPixel).Select(x => new SimpleComparablePixel(x)).ToArray();
        }


        private Span<ComparablePixel> BuildSpanFromPixels(int scanIdx)
        {
            if (SortDirection == SortDirection.Horizontal)
            {
                return new Span<ComparablePixel>(_pixels.Chunk(_imageWidth).ElementAt(scanIdx));
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        private Span<ComparablePixel> BuildSpanFromBytes(int scanIdx)
        {
            byte[] scan;
            if (SortDirection == SortDirection.Horizontal)
            {
                scan = _imageData.Chunk(_imageStride).ElementAt(scanIdx);
            }
            else
            {
                throw new NotImplementedException();
            }

            return new Span<ComparablePixel>(
                scan
                .Chunk(_bytesPerPixel)
                .Select(x => new ComparablePixel(x))
                .ToArray()
            );
        }

        private Span<SimpleComparablePixel> BuildSimpleSpanFromPixels(int scanIdx)
        {
            if (SortDirection == SortDirection.Horizontal)
            {
                return new Span<SimpleComparablePixel>(_simplePixels.Chunk(_imageWidth).ElementAt(scanIdx));
            }
            else
            {
                throw new NotImplementedException();
            }
        }
        private Span<SimpleComparablePixel> BuildSimpleSpanFromBytes(int scanIdx)
        {
            byte[] scan;
            if (SortDirection == SortDirection.Horizontal)
            {
                scan = _imageData.Chunk(_imageStride).ElementAt(scanIdx);
            }
            else
            {
                throw new NotImplementedException();
            }

            return new Span<SimpleComparablePixel>(
                scan
                .Chunk(_bytesPerPixel)
                .Select(x => new SimpleComparablePixel(x))
                .ToArray()
            );
        }


        public void StdSortBytes()
        {
            if (SortOrder == SortOrder.Ascending)
            {
                for (int scan = 0; scan < (SortDirection == SortDirection.Horizontal ? _imageWidth : _imageHeight); scan++)
                {
                    BuildSpanFromBytes(scan).Sort();
                }
            }
        }

        public void StdSortPixels()
        {
            if (SortOrder == SortOrder.Ascending)
            {
                for (int scan = 0; scan < (SortDirection == SortDirection.Horizontal ? _imageWidth : _imageHeight); scan++)
                {
                    BuildSpanFromPixels(scan).Sort();
                }
            }
        }

        public void SimpleStdSortBytes()
        {
            if (SortOrder == SortOrder.Ascending)
            {
                for (int scan = 0; scan < (SortDirection == SortDirection.Horizontal ? _imageWidth : _imageHeight); scan++)
                {
                    BuildSimpleSpanFromBytes(scan).Sort();
                }
            }
        }

        public void SimpleStdSortPixels()
        {
            if (SortOrder == SortOrder.Ascending)
            {
                for (int scan = 0; scan < (SortDirection == SortDirection.Horizontal ? _imageWidth : _imageHeight); scan++)
                {
                    BuildSimpleSpanFromPixels(scan).Sort();
                }
            }
        }


        private unsafe static void SwapIfGreaterWithValues<TPixel>(Span<TPixel> keys, IComparer<TPixel> comparer, int i, int j)
        {
            if (comparer.Compare(keys[i], keys[j]) > 0)
            {
                Swap(keys, i, j);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void Swap<TPixel>(Span<TPixel> keys, int i, int j)
        {
            TPixel k = keys[i];
            keys[i] = keys[j];
            keys[j] = k;
        }
    }
}
