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
    #region Benchmarks
    /*
    | Method            | Datasize             | Mean       | Error    | StdDev   |
    |------------------ |--------------------- |-----------:|---------:|---------:|
    | InsertionSort     | TestD(...)080 } [59] |   742.7 us |  7.75 us |  6.87 us |
    | HeapSort          | TestD(...)080 } [59] |   478.3 us |  1.27 us |  1.19 us |
    | IntrospectiveSort | TestD(...)080 } [59] |   448.8 us |  2.19 us |  1.94 us |

    | InsertionSort     | TestD(...)972 } [60] |   527.0 us |  3.78 us |  3.35 us |
    | HeapSort          | TestD(...)972 } [60] |   344.3 us |  1.68 us |  1.57 us |
    | IntrospectiveSort | TestD(...)972 } [60] |   334.1 us |  2.10 us |  1.96 us |
    
    | InsertionSort     | TestD(...)080 } [61] |   104.7 us |  0.49 us |  0.44 us |
    | HeapSort          | TestD(...)080 } [61] |   104.9 us |  0.46 us |  0.43 us |
    | IntrospectiveSort | TestD(...)080 } [61] |   103.8 us |  0.78 us |  0.69 us |
    
    | InsertionSort     | TestD(...)972 } [62] |   105.1 us |  0.57 us |  0.53 us |
    | HeapSort          | TestD(...)972 } [62] |   104.1 us |  0.46 us |  0.43 us |
    | IntrospectiveSort | TestD(...)972 } [62] |   102.8 us |  0.48 us |  0.40 us |
    
    | InsertionSort     | TestD(...)440 } [59] | 1,246.9 us |  4.49 us |  4.20 us |
    | HeapSort          | TestD(...)440 } [59] |   758.5 us |  2.41 us |  2.14 us |
    | IntrospectiveSort | TestD(...)440 } [59] |   718.8 us |  4.32 us |  4.04 us |
    
    | InsertionSort     | TestD(...)296 } [61] |   840.7 us |  9.54 us |  7.97 us |
    | HeapSort          | TestD(...)296 } [61] |   554.9 us |  3.28 us |  2.91 us |
    | IntrospectiveSort | TestD(...)296 } [61] |   524.0 us |  2.33 us |  2.06 us |
    
    | InsertionSort     | TestD(...)440 } [61] |   143.2 us |  0.94 us |  0.88 us |
    | HeapSort          | TestD(...)440 } [61] |   141.8 us |  1.10 us |  1.03 us |
    | IntrospectiveSort | TestD(...)440 } [61] |   141.7 us |  0.81 us |  0.76 us |
    
    | InsertionSort     | TestD(...)296 } [63] |   143.1 us |  1.11 us |  0.99 us |
    | HeapSort          | TestD(...)296 } [63] |   142.4 us |  0.91 us |  0.85 us |
    | IntrospectiveSort | TestD(...)296 } [63] |   139.3 us |  1.13 us |  1.05 us |
    
    | InsertionSort     | TestD(...)920 } [59] | 2,113.9 us |  5.75 us |  5.38 us |
    | HeapSort          | TestD(...)920 } [59] | 1,233.0 us |  5.53 us |  4.61 us |
    | IntrospectiveSort | TestD(...)920 } [59] | 1,188.5 us |  7.75 us |  6.87 us |
    
    | InsertionSort     | TestD(...)728 } [61] | 1,437.3 us |  7.01 us |  6.56 us |
    | HeapSort          | TestD(...)728 } [61] |   926.8 us |  3.12 us |  2.92 us |
    | IntrospectiveSort | TestD(...)728 } [61] |   844.4 us |  2.21 us |  1.84 us |
    
    | InsertionSort     | TestD(...)920 } [61] |   189.3 us |  1.71 us |  1.60 us |
    | HeapSort          | TestD(...)920 } [61] |   189.0 us |  0.92 us |  0.86 us |
    | IntrospectiveSort | TestD(...)920 } [61] |   186.6 us |  1.01 us |  0.89 us |
    
    | InsertionSort     | TestD(...)728 } [63] |   198.1 us |  0.96 us |  0.90 us |
    | HeapSort          | TestD(...)728 } [63] |   192.5 us |  1.05 us |  0.98 us |
    | IntrospectiveSort | TestD(...)728 } [63] |   190.0 us |  1.13 us |  1.06 us |
    
    | InsertionSort     | TestD(...)160 } [59] | 2,677.5 us | 10.12 us |  9.47 us |
    | HeapSort          | TestD(...)160 } [59] | 1,510.3 us |  6.55 us |  5.80 us |
    | IntrospectiveSort | TestD(...)160 } [59] | 1,472.2 us |  6.87 us |  6.42 us |
    
    | InsertionSort     | TestD(...)944 } [61] | 1,766.3 us |  6.90 us |  6.12 us |
    | HeapSort          | TestD(...)944 } [61] | 1,057.6 us |  5.76 us |  5.38 us |
    | IntrospectiveSort | TestD(...)944 } [61] | 1,004.5 us |  4.88 us |  4.57 us |
    
    | InsertionSort     | TestD(...)160 } [61] |   215.0 us |  1.02 us |  0.90 us |
    | HeapSort          | TestD(...)160 } [61] |   212.3 us |  1.31 us |  1.22 us |
    | IntrospectiveSort | TestD(...)160 } [61] |   211.8 us |  0.72 us |  0.64 us |
    
    | InsertionSort     | TestD(...)944 } [63] |   211.8 us |  1.20 us |  1.12 us |
    | HeapSort          | TestD(...)944 } [63] |   212.3 us |  1.01 us |  0.95 us |
    | IntrospectiveSort | TestD(...)944 } [63] |   207.6 us |  1.36 us |  1.27 us |
    
    | InsertionSort     | TestD(...)560 } [59] | 3,800.6 us | 49.90 us | 46.68 us |
    | HeapSort          | TestD(...)560 } [59] | 2,082.6 us |  8.21 us |  6.41 us |
    | IntrospectiveSort | TestD(...)560 } [59] | 2,003.3 us |  7.25 us |  6.43 us |
    
    | InsertionSort     | TestD(...)304 } [61] | 2,526.7 us | 12.04 us | 10.68 us |
    | HeapSort          | TestD(...)304 } [61] | 1,459.1 us |  9.39 us |  7.84 us |
    | IntrospectiveSort | TestD(...)304 } [61] | 1,391.8 us |  6.29 us |  5.25 us |
    
    | InsertionSort     | TestD(...)560 } [61] |   255.4 us |  1.90 us |  1.69 us |
    | HeapSort          | TestD(...)560 } [61] |   255.4 us |  2.03 us |  1.90 us |
    | IntrospectiveSort | TestD(...)560 } [61] |   262.5 us |  1.64 us |  1.45 us |
    
    | InsertionSort     | TestD(...)304 } [63] |   267.5 us |  5.02 us |  5.98 us |
    | HeapSort          | TestD(...)304 } [63] |   255.9 us |  3.42 us |  3.03 us |
    | IntrospectiveSort | TestD(...)304 } [63] |   265.7 us |  2.01 us |  1.78 us |
    
    | InsertionSort     | TestD(...)840 } [59] | 8,138.7 us | 47.78 us | 39.90 us |
    | HeapSort          | TestD(...)840 } [59] | 4,319.6 us | 13.69 us | 12.14 us |
    | IntrospectiveSort | TestD(...)840 } [59] | 4,235.2 us | 15.36 us | 14.37 us |
    
    | InsertionSort     | TestD(...)456 } [61] | 5,379.2 us | 21.62 us | 19.16 us |
    | HeapSort          | TestD(...)456 } [61] | 2,970.9 us |  7.97 us |  7.07 us |
    | IntrospectiveSort | TestD(...)456 } [61] | 2,876.3 us |  6.75 us |  5.98 us |
    
    | InsertionSort     | TestD(...)840 } [61] |   389.9 us |  2.20 us |  2.05 us |
    | HeapSort          | TestD(...)840 } [61] |   383.4 us |  2.45 us |  2.29 us |
    | IntrospectiveSort | TestD(...)840 } [61] |   388.6 us |  0.92 us |  0.86 us |
    
    | InsertionSort     | TestD(...)456 } [63] |   391.3 us |  3.31 us |  2.94 us |
    | HeapSort          | TestD(...)456 } [63] |   386.8 us |  2.57 us |  2.28 us |
    | IntrospectiveSort | TestD(...)456 } [63] |   384.2 us |  2.54 us |  2.38 us |
    
     */
    #endregion

#pragma warning disable CS8500 // This takes the address of, gets the size of, or declares a pointer to a managed type
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
#pragma warning restore CS8500 // This takes the address of, gets the size of, or declares a pointer to a managed type
    }
}
