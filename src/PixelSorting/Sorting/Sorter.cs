using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Diagnostics;
using Sorting.Pixels._32;
using Sorting.Pixels._8;
using Sorting.Pixels.KeySelector;

namespace Sorting;

public unsafe class Sorter32Bit(Pixel32bitUnion* byteDataBegin, int width, int height, int stride)
    : Sorter<Pixel32bitUnion>(byteDataBegin, width, height, stride);

public unsafe class Sorter8Bit(Pixel8bit* byteDataBegin, int width, int height, int stride)
    : Sorter<Pixel8bit>(byteDataBegin, width, height, stride);

// TODO: more overloads

public unsafe partial class Sorter<TPixel>
    where TPixel : struct
{
    // Using closures and delegates at this stage would be nice, but they 
    // only use SHO, as the closure object is generated at compile time.
    public readonly struct AngleSorterInfo : ICloneable
    {
        public ISorter Sorter { get; init; }

        public TPixel* Pixels { get; init; }

        public int ImageWidth { get; init; }

        public int ImageHeight { get; init; }


        public AngleSorterInfo Sort(double uStep, double vStep, int uOff, int vOff)
        {
            Sorter.Sort(new PixelSpan2D(Pixels, ImageWidth, ImageHeight, uStep, vStep, uOff, vOff));
            return this;
        }

        object ICloneable.Clone() => Clone();

        public AngleSorterInfo Clone() => this with { Sorter = (ISorter)Sorter.Clone() };
    }

    public AngleSorterInfo GetAngleSorterInfo(ISorter sorter)
    {
        return new AngleSorterInfo
        {
            Sorter = sorter,
            Pixels = _pixels,
            ImageWidth = _imageWidth,
            ImageHeight = _imageHeight
        };
    }


    private readonly int _imageWidth, _imageHeight;

    // Safety: C# forces the data to be pinned before taking the address of it.
    private readonly TPixel* _pixels;
    private readonly int _pixelCount;

    /// <summary>
    /// The parallelization options.
    /// </summary>
    public ParallelOptions ParallelOpts { get; set; } = new()
    {
        MaxDegreeOfParallelism = -1
    };


    /// <summary>
    /// Initialize a Sorter.
    /// </summary>
    /// <param name="byteDataBegin">The address of the first byte of the image data.</param>
    /// <param name="height">The height of the image in pixels.</param>
    /// <param name="width">The width of the image in pixels.</param>
    /// <param name="stride">The width of the image in bytes.</param>
    /// <exception cref="ArgumentException"></exception>
    public Sorter(TPixel* byteDataBegin, int width, int height, int stride)
    {
        if (stride / width != Marshal.SizeOf<TPixel>())
        {
            throw new ArgumentException(
                $"{nameof(stride)} to {nameof(width)} ratio does not match " +
                $"the struct size of {nameof(TPixel)}.");
        }

        _imageHeight = height;
        _imageWidth = width;
        _pixels = byteDataBegin;
        _pixelCount = height * width;
    }


    public bool NextRowPixelSpan(int y, IComparer<TPixel> comparer, TPixel threshhold, out PixelSpan span,
        ref int iteratorX, Span<TPixel> pixels)
    {
        // Remember where we started.
        var begin = iteratorX;

        // Get current lo.
        var lo = y * _imageWidth;

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
        var hi = lo + iteratorX;
        span = new PixelSpan(pixels, 1, lo + begin, hi);
        return true;
    }

    public bool NextColPixelSpan(int x, IComparer<TPixel> comparer, TPixel threshhold, out PixelSpan span,
        ref int iteratorY, Span<TPixel> pixels)
    {
        // Remember where we started
        var begin = iteratorY;

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
        var lo = y * _imageWidth;
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
                for (var row = 0; row < _imageHeight; row++)
                {
                    IntrospectiveSort(GetRowPixelSpan(row), comparer);
                }

                break;

            case SortDirection.Vertical:
                for (var col = 0; col < _imageWidth; col++)
                {
                    IntrospectiveSort(GetColPixelSpan(col), comparer);
                }

                break;
        }
    }

    /// <summary>
    /// Sort the image along the angle <paramref name="alpha"/>, where: 
    ///     alpha(0) ~ Vertical, 
    ///     alpha(PI / 2) ~ Horizontal, 
    ///     alpha(PI) ~ Vertical. 
    /// </summary>
    /// <param name="alpha">Angle in Radians, element of [ 0 ; PI ]</param>
    /// <param name="sorterInfo">The sort function.</param>
    public void SortAngle(double alpha, AngleSorterInfo sorterInfo)
    {
#if DEBUG
        // the diagonal of the pixel-rect.
        var c = Math.Sqrt(Math.Pow(_imageWidth, 2) + Math.Pow(_imageHeight, 2));

        // the base length of the triangle formed by alpha + pixel-rect height.
        double BaseA(double angle) => c * Math.Sin(angle);

        // the angle of the diagonal of the pixel-rect. 
        var theta = Math.Asin(_imageWidth / c);

        Debug.Assert(Math.Abs(BaseA(theta) - _imageWidth) < 0.000001);
#endif

        var tanAlpha = Math.Tan(alpha);

        switch (alpha)
        {
            case 0:
            {
                Parallel.For( 0, _imageWidth, ParallelOpts, sorterInfo.Clone, (i, _, info) => info.Sort(0, 1, i, 0), Noop);
                break;
            }
            case > 0 and < Math.PI / 2:
            {
                // left
                Parallel.For( 0, _imageHeight, ParallelOpts, sorterInfo.Clone, (i, _, info) => info.Sort(1, 1 / tanAlpha, 0, i), Noop);

                // top
                if (alpha > Math.PI / 4)
                {
                    Parallel.For( 0, _imageWidth, ParallelOpts, sorterInfo.Clone, (i, _, info) => info.Sort(1, 1 / tanAlpha, i, 0), Noop);
                }
                else
                {
                    Parallel.For( 0, _imageWidth, ParallelOpts, sorterInfo.Clone, (i, _, info) => info.Sort(tanAlpha, 1, i, 0), Noop);
                }

                break;
            }
            case Math.PI / 2:
            {
                Parallel.For( 0, _imageHeight, ParallelOpts, sorterInfo.Clone, (i, _, info) => info.Sort(1, 0, 0, i), Noop);
                break;
            }
            case > Math.PI / 2 and < Math.PI:
            {
                // top
                Parallel.For( 0, _imageWidth, ParallelOpts, sorterInfo.Clone, (i, _, info) => info.Sort(tanAlpha, 1, i, 0), Noop);

                // right
                if (alpha > Math.PI / 2 + Math.PI / 4)
                {
                    Parallel.For( 0, _imageHeight, ParallelOpts, sorterInfo.Clone, (i, _, info) => info.Sort(tanAlpha, 1, _imageWidth - 1, i), Noop);
                }
                else
                {
                    Parallel.For( 0, _imageHeight, ParallelOpts, sorterInfo.Clone, (i, _, info) => info.Sort(-1, -1 / tanAlpha, _imageWidth - 1, i), Noop);
                }

                break;
            }
            case Math.PI:
            {
                Parallel.For( 0, _imageWidth, sorterInfo.Clone, (i, _, info) => info.Sort(0, 1, i, 0), Noop);
                break;
            }
        }

        return;

        static void Noop(AngleSorterInfo sorterInfo) { }
    }


    // Used for testing.
    public void SortUnsafe(double alpha, IComparer<TPixel> comparer)
    {
        // the diagonal of the pixel-rect.
        var c = Math.Sqrt(Math.Pow(_imageWidth, 2) + Math.Pow(_imageHeight, 2));

        // Local function to shorten syntax.
        var indeces = new nint[(int)Math.Ceiling(c) + 1]; // longest diagonal will be the tangent

        void Sort(double ustep, double vstep, int uoff, int voff)
            => IntrospectiveSort(
                new UnsafePixelSpan2D(_pixels, indeces, _imageWidth, _imageHeight, ustep, vstep, uoff, voff),
                comparer);

        // store the result of tan alpha, becuase it is used often.
        var tanAlpha = Math.Tan(alpha);

        if (alpha == 0)
        {
            for (var i = 0; i < _imageWidth; i++) Sort(0, 1, i, 0);
        }

        else if (alpha > 0 && alpha < Math.PI / 2)
        {
            // top
            if (alpha > Math.PI / 4)
            {
                for (var i = 0; i < _imageWidth; i++) Sort(1, 1 / tanAlpha, i, 0);
            }
            else
            {
                for (var i = 0; i < _imageWidth; i++) Sort(tanAlpha, 1, i, 0);
            }

            // left
            for (var i = 0; i < _imageHeight; i++) Sort(1, 1 / tanAlpha, 0, i);
        }

        else if (alpha == Math.PI / 2)
        {
            for (var i = 0; i < _imageHeight; i++) Sort(1, 0, 0, i);
        }

        else if (alpha > Math.PI / 2 && alpha < Math.PI)
        {
            // top
            for (var i = 0; i < _imageWidth; i++) Sort(tanAlpha, 1, i, 0);

            // right
            if (alpha > Math.PI / 2 + Math.PI / 4)
            {
                for (var i = 0; i < _imageHeight; i++) Sort(tanAlpha, 1, _imageWidth - 1, i);
            }
            else
            {
                for (var i = 0; i < _imageHeight; i++) Sort(-1, -1 / tanAlpha, _imageWidth - 1, i);
            }
        }

        else if (alpha == Math.PI)
        {
            for (var i = 0; i < _imageWidth; i++) Sort(0, 1, i, 0);
        }
    }


    public void SortSafe(double alpha, IComparer<TPixel> comparer)
    {
        // the diagonal of the pixel-rect.
        var c = Math.Sqrt(Math.Pow(_imageWidth, 2) + Math.Pow(_imageHeight, 2));

        // Local function to shorten syntax.
        var indeces = new nint[(int)Math.Ceiling(c) + 1]; // longest diagonal will be the tangent

        void Sort(double ustep, double vstep, int uoff, int voff)
            => IntrospectiveSort(
                new PixelSpan2D(_pixels, _imageWidth, _imageHeight, ustep, vstep, uoff, voff),
                comparer);

        // store the result of tan alpha, becuase it is used often.
        var tanAlpha = Math.Tan(alpha);

        if (alpha == 0)
        {
            for (var i = 0; i < _imageWidth; i++) Sort(0, 1, i, 0);
        }

        else if (alpha > 0 && alpha < Math.PI / 2)
        {
            // top
            if (alpha > Math.PI / 4)
            {
                for (var i = 0; i < _imageWidth; i++) Sort(1, 1 / tanAlpha, i, 0);
            }
            else
            {
                for (var i = 0; i < _imageWidth; i++) Sort(tanAlpha, 1, i, 0);
            }

            // left
            for (var i = 0; i < _imageHeight; i++) Sort(1, 1 / tanAlpha, 0, i);
        }

        else if (alpha == Math.PI / 2)
        {
            for (var i = 0; i < _imageHeight; i++) Sort(1, 0, 0, i);
        }

        else if (alpha > Math.PI / 2 && alpha < Math.PI)
        {
            // top
            for (var i = 0; i < _imageWidth; i++) Sort(tanAlpha, 1, i, 0);

            // right
            if (alpha > Math.PI / 2 + Math.PI / 4)
            {
                for (var i = 0; i < _imageHeight; i++) Sort(tanAlpha, 1, _imageWidth - 1, i);
            }
            else
            {
                for (var i = 0; i < _imageHeight; i++) Sort(-1, -1 / tanAlpha, _imageWidth - 1, i);
            }
        }

        else if (alpha == Math.PI)
        {
            for (var i = 0; i < _imageWidth; i++) Sort(0, 1, i, 0);
        }
    }


    public void Sort(SortDirection sortDirection, IComparer<TPixel> comparer, TPixel threshhold)
    {
        switch (sortDirection)
        {
            case SortDirection.Horizontal:
                for (var row = 0; row < _imageHeight; row++)
                {
                    // Iterator to remember where the last threshhold window was.
                    var iteratorX = 0;

                    // Create a span around the pixels.
                    Span<TPixel> pixels = new(_pixels, _pixelCount);

                    while (NextRowPixelSpan(row, comparer, threshhold, out var span, ref iteratorX, pixels))
                    {
                        IntrospectiveSort(span, comparer);
                    }
                }

                break;

            case SortDirection.Vertical:
                for (var col = 0; col < _imageWidth; col++)
                {
                    // Iterator to remember where the last threshhold window was.
                    var iteratorY = 0;

                    // Create a span around the pixels.
                    Span<TPixel> pixels = new(_pixels, _pixelCount);

                    while (NextColPixelSpan(col, comparer, threshhold, out var span, ref iteratorY, pixels))
                    {
                        IntrospectiveSort(span, comparer);
                    }
                }

                break;
        }
    }
}