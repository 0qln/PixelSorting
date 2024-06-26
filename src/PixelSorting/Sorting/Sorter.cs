﻿using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Diagnostics;
using Sorting.Pixels.KeySelector;

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

    // TODO: this can be a readonly ref struct.

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


        /// <summary>
        /// Initialize a Sorter.
        /// </summary>
        /// <param name="byteDataBegin">The adress of the first byte of the image data.</param>
        /// <param name="height">The height of the image in pixels.</param>
        /// <param name="width">The width of the image in pixels.</param>
        /// <param name="stride">The width of the image in bytes.</param>
        /// <exception cref="ArgumentException"></exception>
        public Sorter(ref TPixel byteDataBegin, int width, int height, int stride)
            : this ((nint)Unsafe.AsPointer(ref byteDataBegin), width, height, stride)
        {
        }


        public unsafe bool NextRowPixelSpan(int y, IComparer<TPixel> comparer, TPixel threshhold, out PixelSpan span, ref int iteratorX, Span<TPixel> pixels)
        {
            // Remember where we started.
            int begin = iteratorX;

            // Get current lo.
            int lo = y * _imageWidth;

            // Go to next span start.
            while (iteratorX < _imageWidth && comparer.Compare(pixels[lo + iteratorX], threshhold) < 0)
            {
                iteratorX++;
            }

            // Get next span width.
            while (iteratorX < _imageWidth && comparer.Compare(pixels[lo + iteratorX], threshhold) >= 0)
            {
                iteratorX++;
            }

            // If we are at the end of the row: reset iterator and return false.
            // This will also handle the first loop if it gets to the end.
            if (iteratorX >= _imageWidth)
            {
                iteratorX = 0;
                span = default;
                return false;
            }

            // Assign span and return true.
            int hi = lo + iteratorX;
            span = new PixelSpan(pixels, 1, lo + begin, hi);
            return true;
        }


        public unsafe bool NextColPixelSpan(int x, IComparer<TPixel> comparer, TPixel threshhold, out PixelSpan span, ref int iteratorY, Span<TPixel> pixels)
        {
            // Remember where we started
            int begin = iteratorY;

            // Go to next span start.
            while (iteratorY < _pixelCount && comparer.Compare(pixels[x + iteratorY], threshhold) < 0)
            {
                iteratorY += _imageWidth;
            }

            // Get next span width.
            while (iteratorY < _pixelCount && comparer.Compare(pixels[x + iteratorY], threshhold) >= 0)
            {
                iteratorY += _imageWidth;
            }

            // If we are at the end of the row: reset iterator and return false.
            // This will also handle the first loop if it gets to the end.
            if (iteratorY >= _pixelCount)
            {
                iteratorY = 0;
                span = default;
                return false;
            }

            // Assign span and return true.
            span = new PixelSpan(pixels, _imageWidth, x + begin, x + iteratorY);
            return true;
        }


        public PixelSpan GetRowPixelSpan(int y)
        {
            int lo = y * _imageWidth;
            return new PixelSpan(_pixels, 1, lo, lo + _imageWidth);
        }


        public PixelSpan GetColPixelSpan(int x)
        {
            return new PixelSpan(_pixels, _imageWidth, x, _pixelCount);
        }


        public void Sort(SortDirection sortDirection, IComparer<TPixel> comparer)
        {
            switch (sortDirection)
            {
                case SortDirection.Horizontal:
                    for (int row = 0; row < _imageHeight; row++)
                    {
                        IntrospectiveSort(GetRowPixelSpan(row), comparer);
                    }
                    break;

                case SortDirection.Vertical:
                    for (int col = 0; col < _imageWidth; col++)
                    {
                        IntrospectiveSort(GetColPixelSpan(col), comparer);
                    }
                    break;
            }
        }


        public delegate void AngleSorter(double ustep, double vstep, int uoff, int voff, nint[] indeces);


        /// <summary>
        /// Sort the image along the angle <paramref name="alpha"/>, where: 
        ///     alpha(0) ~ Vertical, 
        ///     alpha(PI / 2) ~ Horizontal, 
        ///     alpha(PI) ~ Vertical. 
        /// </summary>
        /// <param name="alpha">Angle in Radians, element of [ 0 ; PI ]</param>
        /// <param name="sorter">The sort function.</param>
        public void AngleSort(double alpha, AngleSorter sorter)
        {
            // the diagonal of the pixel-rect.
            double c = Math.Sqrt(Math.Pow(_imageWidth, 2) + Math.Pow(_imageHeight, 2));

            // the base length of the triangle formed by alpha + pixel-rect height.
            double baseA(double angle) => c * Math.Sin(angle);

            // the angle of the diagonal of the pixel-rect. 
            double theta = Math.Asin(_imageWidth / c);
            Debug.Assert(baseA(theta) == _imageWidth);

            // Local function to shorten syntax.
            nint[] indeces = new nint[(int)Math.Ceiling(c) + 1]; // longest diagonal will be the tangent

            // store the result of tan alpha, becuase it is used often.
            double tanAlpha = Math.Tan(alpha);

            if (alpha == 0)
            {
                for (int i = 0; i < _imageWidth; i++)
                    sorter(0, 1, i, 0, indeces);
            }

            else if (alpha > 0 && alpha < Math.PI / 2)
            {
                // top
                if (alpha > Math.PI / 4)
                {
                    for (int i = 0; i < _imageWidth; i++)
                        sorter(1, 1 / tanAlpha, i, 0, indeces);
                }
                else
                {
                    for (int i = 0; i < _imageWidth; i++)
                        sorter(tanAlpha, 1, i, 0, indeces);
                }

                // left
                for (int i = 0; i < _imageHeight; i++)
                    sorter(1, 1 / tanAlpha, 0, i, indeces);
            }

            else if (alpha == Math.PI / 2)
            {
                for (int i = 0; i < _imageHeight; i++)
                    sorter(1, 0, 0, i, indeces);
            }

            else if (alpha > Math.PI / 2 && alpha < Math.PI)
            {
                // top
                for (int i = 0; i < _imageWidth; i++)
                {
                    sorter(tanAlpha, 1, i, 0, indeces);
                }

                // right
                if (alpha > Math.PI / 2 + Math.PI / 4)
                {
                    for (int i = 0; i < _imageHeight; i++)
                        sorter(tanAlpha, 1, _imageWidth - 1, i, indeces);
                }
                else
                {
                    for (int i = 0; i < _imageHeight; i++)
                        sorter(-1, -1 / tanAlpha, _imageWidth - 1, i, indeces);
                }
            }

            else if (alpha == Math.PI)
            {
                for (int i = 0; i < _imageWidth; i++)
                    sorter(0, 1, i, 0, indeces);
            }
        }


        /// <summary>
        /// Create a comb sorter.
        /// </summary>
        /// <param name="comparer"></param>
        /// <param name="pureness"></param>
        /// <returns></returns>
        public AngleSorter CombSort(IComparer<TPixel> comparer, int pureness)
        {
            void Sort(double ustep, double vstep, int uoff, int voff, nint[] indeces)
            {
                var span = new PixelSpan2D(_pixels, indeces, _imageWidth, _imageHeight, ustep, vstep, uoff, voff);
                CombSort(span, comparer, pureness);
            }

            return Sort;
        }


        /// <summary>
        /// Sort the image using the specified <paramref name="comparer"/> rotated clockwise towards the angle <paramref name="alpha"/>, where: 
        ///     alpha(0) ~ Vertical, 
        ///     alpha(PI / 2) ~ Horizontal, 
        ///     alpha(PI) ~ Vertical. 
        /// </summary>
        /// <param name="alpha">Angle in Radians, element of [ 0 ; PI ]</param>
        /// <param name="comparer">The comparer that is used to compare the pixels</param>
        public AngleSorter ShellSort(IComparer<TPixel> comparer, int pureness)
        {
            void Sort(double ustep, double vstep, int uoff, int voff, nint[] indeces)
            {
                var span = new PixelSpan2D(_pixels, indeces, _imageWidth, _imageHeight, ustep, vstep, uoff, voff);
                ShellSort(span, comparer, 0, span.ItemCount, pureness);
            }

            return Sort;
        }


        /// <summary>
        /// Sort the image using the specified <paramref name="comparer"/> rotated clockwise towards the angle <paramref name="alpha"/>, where: 
        ///     alpha(0) ~ Vertical, 
        ///     alpha(PI / 2) ~ Horizontal, 
        ///     alpha(PI) ~ Vertical. 
        /// </summary>
        /// <param name="alpha">Angle in Radians, element of [ 0 ; PI ]</param>
        /// <param name="comparer">The comparer that is used to compare the pixels</param>
        public AngleSorter FastSort(IComparer<TPixel> comparer)
        {
            void Sort(double ustep, double vstep, int uoff, int voff, nint[] indeces)
            {
                var span = new PixelSpan2D(_pixels, indeces, _imageWidth, _imageHeight, ustep, vstep, uoff, voff);
                IntrospectiveSort(span, comparer);
            }

            return Sort;
        }


        public AngleSorter PigeonSorter(IOrderedKeySelector<TPixel> selector)
        {
            void Sort(double ustep, double vstep, int uoff, int voff, nint[] indeces)
            {
                var span = new PixelSpan2D(_pixels, indeces, _imageWidth, _imageHeight, ustep, vstep, uoff, voff);
                PigeonholeSort(span, selector);
            }

            return Sort;
        }


        public AngleSorter InsertionSorter(IComparer<TPixel> comparer)
        {
            void Sort(double ustep, double vstep, int uoff, int voff, nint[] indeces)
            {
                var span = new PixelSpan2D(_pixels, indeces, _imageWidth, _imageHeight, ustep, vstep, uoff, voff);
                InsertionSort(span, comparer);
            }

            return Sort;
        }


        // Used for testing.
        internal void SortUnsafe(double alpha, IComparer<TPixel> comparer)
        {
            // the diagonal of the pixel-rect.
            double c = Math.Sqrt(Math.Pow(_imageWidth, 2) + Math.Pow(_imageHeight, 2));

            // Local function to shorten syntax.
            nint[] indeces = new nint[(int)Math.Ceiling(c) + 1]; // longest diagonal will be the tangent
            void Sort(double ustep, double vstep, int uoff, int voff)
                => IntrospectiveSort(
                        new UnsafePixelSpan2D(_pixels, indeces, _imageWidth, _imageHeight, ustep, vstep, uoff, voff),
                        comparer);

            // store the result of tan alpha, becuase it is used often.
            double tanAlpha = Math.Tan(alpha);

            if (alpha == 0)
            {
                for (int i = 0; i < _imageWidth; i++) Sort(0, 1, i, 0);
            }

            else if (alpha > 0 && alpha < Math.PI / 2)
            {
                // top
                if (alpha > Math.PI / 4)
                {
                    for (int i = 0; i < _imageWidth; i++) Sort(1, 1 / tanAlpha, i, 0);
                }
                else
                {
                    for (int i = 0; i < _imageWidth; i++) Sort(tanAlpha, 1, i, 0);
                }

                // left
                for (int i = 0; i < _imageHeight; i++) Sort(1, 1 / tanAlpha, 0, i);
            }

            else if (alpha == Math.PI / 2)
            {
                for (int i = 0; i < _imageHeight; i++) Sort(1, 0, 0, i);
            }

            else if (alpha > Math.PI / 2 && alpha < Math.PI)
            {
                // top
                for (int i = 0; i < _imageWidth; i++) Sort(tanAlpha, 1, i, 0);

                // right
                if (alpha > Math.PI / 2 + Math.PI / 4)
                {
                    for (int i = 0; i < _imageHeight; i++) Sort(tanAlpha, 1, _imageWidth - 1, i);
                }
                else
                {
                    for (int i = 0; i < _imageHeight; i++) Sort(-1, -1 / tanAlpha, _imageWidth - 1, i);
                }
            }

            else if (alpha == Math.PI)
            {
                for (int i = 0; i < _imageWidth; i++) Sort(0, 1, i, 0);
            }
        }


        public void Sort(SortDirection sortDirection, IComparer<TPixel> comparer, TPixel threshhold)
        {
            switch (sortDirection)
            {
                case SortDirection.Horizontal:
                    for (int row = 0; row < _imageHeight; row++)
                    {
                        // Iterator to remember where the last threshhold window was.
                        int iteratorX = 0;

                        // Create a span around the pixels.
                        Span<TPixel> pixels = new(_pixels, _pixelCount);

                        while (NextRowPixelSpan(row, comparer, threshhold, out PixelSpan span, ref iteratorX, pixels))
                        {
                            IntrospectiveSort(span, comparer);
                        }
                    }
                    break;

                case SortDirection.Vertical:
                    for (int col = 0; col < _imageWidth; col++)
                    {
                        // Iterator to remember where the last threshhold window was.
                        int iteratorY = 0;

                        // Create a span around the pixels.
                        Span<TPixel> pixels = new(_pixels, _pixelCount);

                        while (NextColPixelSpan(col, comparer, threshhold, out PixelSpan span, ref iteratorY, pixels))
                        {
                            IntrospectiveSort(span, comparer);
                        }
                    }
                    break;
            }
        }


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

    }
#pragma warning restore CS8500 // This takes the address of, gets the size of, or declares a pointer to a managed type

}
