using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;
using System.Text;
using System.Threading.Tasks;

namespace Sorting
{
    public unsafe partial class Sorter<TPixel>
        where TPixel : struct
    {
        private readonly int _imageWidth, _imageHeight, _imageStride, _bytesPerPixel;

        // Both sould point to the same address in memory.
        private readonly ulong _byteCount;
        private readonly byte* _bytes;
        private readonly int _pixelCount;
        private readonly TPixel* _pixels;


        public delegate void Sort(SortDirection direction, SortOrder order, SortType type);

        internal delegate void SortInternal(Span<TPixel> keys, IComparer<TPixel> comparer, int step, int from, int to);


        /// <summary>
        /// Initialize a Sorter.
        /// </summary>
        /// <param name="byteDataBegin">The adress of the first byte of the image data.</param>
        /// <param name="height">The height of the image in pixels.</param>
        /// <param name="width">The width of the image in pixels.</param>
        /// <param name="stride">The width of the image in bytes.</param>
        /// <exception cref="ArgumentException"></exception>
        public Sorter(nint byteDataBegin, int width, int height, int stride)
        {
            _bytesPerPixel = stride / width;
            if (_bytesPerPixel != Marshal.SizeOf<TPixel>())
            {
                throw new ArgumentException("Given `stride` to `width` ratio does not match the struct size of `TPixel`.");
            }
            _imageStride = stride;
            _imageHeight = height;
            _imageWidth = width;
            _bytes = (byte*)byteDataBegin;
            _pixels = (TPixel*)byteDataBegin;
            _byteCount = (ulong)height * (ulong)stride;
            _pixelCount = height * width;
        }


        public unsafe Span<TPixel> GetRow(int y)
        {
            return new Span<TPixel>(_pixels + y * _imageWidth, _imageWidth);
        }

        // TODO: make custom Span<T> ??


        public unsafe Sorter<TResult> CastToPixelFormat<TResult>(Func<TPixel, TResult> pixelConverter)
            where TResult : struct
        {
            // Allocate new data
            int newSize = Marshal.SizeOf<TResult>();
            byte[] newBytes = new byte[_pixelCount * newSize];

            // Populate new data
            Span<TResult> newPixels = new Span<TResult>(&newBytes, _pixelCount);
            for (int i = 0; i < _pixelCount; i++)
            {
                newPixels[i] = pixelConverter(_pixels[i]);
            }

            // Create new sorter with copy of this pixeldata
            var result = new Sorter<TResult>((IntPtr)(&newBytes), _imageWidth, _imageHeight, newSize * _imageWidth);

            // Return the result
            return result;
        }


        /// <summary>
        /// Custom `Span<typeparamref name="TPixel"/>` implementation.
        /// </summary>
        /// <typeparam name="TPixel"></typeparam>
        public readonly ref struct PixelSpan
        {
            /// <summary>A byref or a native ptr.</summary>
            internal readonly ref TPixel _reference;
            /// <summary>The number of elements this Span operates on.</summary>
            private readonly int _items;
            /// <summary>The number that controls how many where the next elmenet is when indexing.</summary>
            private readonly int _step;


            public PixelSpan(TPixel[] reference, int step, int lo, int hi)
            {
                int size = hi - lo;
                _items = size / step + (size % step == 0 ? 0 : 1);
                _reference = ref reference[lo];
                _step = step;
            }


            public ref TPixel this[int index]
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get
                {
                    if ((uint)index >= (uint)_items)
                        throw new IndexOutOfRangeException();
                    return ref Unsafe.Add(ref _reference, (nint)(uint)(index * _step));
                }
            }

            public int ItemCount
            {
                get => _items;
            }

            public int Step
            {
                get => _step;
            }
        }
    }
}
