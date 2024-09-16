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


/// <summary>
/// This is the class used to sort an image.
/// It operates on the raw in-memory bytes of the image.
/// This class supports parallelization internally.
/// </summary>
/// <typeparam name="TPixel"></typeparam>
public unsafe partial class Sorter<TPixel>
    where TPixel : struct
{
    // Using closures and delegates at this stage would be nice, but they 
    // only use SHO, as the closure object is generated at compile time.
    /// <summary>
    /// Represents the information needed to sort an image along a specific angle.
    /// </summary>
    public readonly struct AngleSorterInfo : ICloneable
    {
        /// <summary>
        /// Gets or initializes the sorter to be used for sorting the image along the specified angle.
        /// </summary>
        public ISorter Sorter { get; init; }

        /// <summary>
        /// Gets or initializes a pointer to the pixels of the image.
        /// </summary>
        internal TPixel* Pixels { get; init; }

        /// <summary>
        /// Gets or initializes the width of the image.
        /// </summary>
        public int ImageWidth { get; init; }

        /// <summary>
        /// Gets or initializes the height of the image.
        /// </summary>
        public int ImageHeight { get; init; }

        /// <summary>
        /// Sorts the image along the specified angle using the specified step sizes and offsets.
        /// </summary>
        /// <param name="uStep">The step size in the u direction.</param>
        /// <param name="vStep">The step size in the v direction.</param>
        /// <param name="uOff">The offset in the u direction.</param>
        /// <param name="vOff">The offset in the v direction.</param>
        /// <param name="inverseIndexing">A flag indicating whether to use inverse indexing.</param>
        /// <returns>The sorted <see cref="AngleSorterInfo"/> object.</returns>
        public AngleSorterInfo Sort(double uStep, double vStep, int uOff, int vOff, bool inverseIndexing)
        {
            Sorter.Sort(new PixelSpan2DRun(Pixels, ImageWidth, ImageHeight, uStep, vStep, uOff, vOff, inverseIndexing));
            return this;
        }

        /// <summary>
        /// Creates a clone of the <see cref="AngleSorterInfo"/> object.
        /// </summary>
        /// <returns>A clone of the <see cref="AngleSorterInfo"/> object.</returns>
        object ICloneable.Clone() => Clone();

        /// <summary>
        /// Creates a clone of the <see cref="AngleSorterInfo"/> object.
        /// </summary>
        /// <returns>A clone of the <see cref="AngleSorterInfo"/> object.</returns>
        public AngleSorterInfo Clone() => this with { Sorter = (ISorter)Sorter.Clone() };
    }

    /// <summary>
    /// Get the AngleSorterInfo for the specified sorter.
    /// </summary>
    /// <param name="sorter"></param>
    /// <returns></returns>
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
        MaxDegreeOfParallelism = 4
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

    /// <summary>
    /// Sort the image along the specified <paramref name="sortDirection"/> using the specified <paramref name="comparer"/>.
    /// </summary>
    /// <param name="sortDirection"></param>
    /// <param name="comparer"></param>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    [Obsolete("Needs to be reviewed.")]
    public void Sort(SortDirection sortDirection, IComparer<TPixel> comparer)
    {
        switch (sortDirection)
        {
            case SortDirection.Horizontal:
                for (var row = 0; row < _imageHeight; row++)
                    IntrospectiveSort(GetRowPixelSpan(row), comparer);

                break;

            case SortDirection.Vertical:
                for (var col = 0; col < _imageWidth; col++)
                    IntrospectiveSort(GetColPixelSpan(col), comparer);

                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(sortDirection), sortDirection, null);
        }

        return;

        PixelSpan GetRowPixelSpan(int y)
        {
            var lo = y * _imageWidth;
            return new PixelSpan(_pixels, 1, lo, lo + _imageWidth);
        }


        PixelSpan GetColPixelSpan(int x)
        {
            return new PixelSpan(_pixels, _imageWidth, x, _pixelCount);
        }
    }

    /// <summary>
    /// This function is mainly used as a utility function for testing.
    /// </summary>
    /// <param name="alpha"></param>
    /// <param name="width"></param>
    /// <param name="height"></param>
    /// <param name="action"></param>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public static void DoRun(double alpha, int width, int height, Action<double, double, int, int, bool> action)
    {
        var tanAlpha = Math.Tan(alpha);

        switch (alpha)
        {
            case 0:

                // top
                for (var i = 0; i < width; i++)
                    action(0, 1, i, 0, false);

                break;

            case < Math.PI / 4:

                // left
                for (var i = 0; i < (int)(height * tanAlpha) + 1; i++)
                    action(tanAlpha, 1, -i, 0, false);

                // top
                for (var i = 1; i < width; i++)
                    action(tanAlpha, 1, i, 0, false);

                break;

            case Math.PI / 4:

                // left
                for (var i = 0; i < height; i++)
                    action(1, 1, -i, 0, false);

                // top
                for (var i = 1; i < width; i++)
                    action(1, 1, i, 0, false);

                break;

            case < Math.PI / 2:

                // left
                for (var i = 0; i < height; i++)
                    action(1, 1 / tanAlpha, 0, i, false);

                // top
                for (var i = 1; i < (int)(width / tanAlpha) + 1; i++)
                    action(1, 1 / tanAlpha, 0, -i, false);

                break;

            case Math.PI / 2:

                // left
                for (var i = 0; i < height; i++)
                    action(1, 0, 0, i, false);

                break;

            case < Math.PI / 2 + Math.PI / 4:

                // right
                for (var i = 0; i < height; i++)
                    action(-1, 1 / -tanAlpha, width - 1, i, true);

                // top
                for (var i = 1; i < (int)(width / -tanAlpha) + 2; i++)
                    action(-1, 1 / -tanAlpha, width - 1, -i, true);

                break;

            case Math.PI / 2 + Math.PI / 4:

                // right
                for (var i = 0; i < height; i++)
                    action(-1, 1, width - 1, i, true);

                // top
                for (var i = 1; i < (int)(width / -tanAlpha) + 1; i++)
                    action(-1, 1, width - 1, -i, true);

                break;

            case < Math.PI:

                // right 
                for (var i = 0; i < (int)(height * -tanAlpha) + 1; i++)
                    action(tanAlpha, 1, i + width - 1, 0, true);

                // top
                for (var i = 1; i < width; i++)
                    action(tanAlpha, 1, -i + width - 1, 0, true);

                break;

            case Math.PI:

                // top
                for (var i = 0; i < width; i++)
                    action(0, 1, i, 0, false);

                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(alpha));
        }
    }

    /// <summary>
    /// Sort the image along the angle <paramref name="alpha"/>,
    /// where: <br/>
    ///     alpha(0)      ~ Vertical,   <br/>
    ///     alpha(PI / 2) ~ Horizontal, <br/>
    ///     alpha(PI)     ~ Vertical.   <br/>
    /// This function will execute single threaded.
    /// </summary>
    /// <param name="alpha">Angle in Radians, element of range [ 0 ; PI ]</param>
    /// <param name="sorterInfo">The information for sorting, which includes: the sorting method, the sorting comparer, etc.</param>
    public void SortAngle(double alpha, AngleSorterInfo sorterInfo)
    {
        // TODO: SmallObjectHeap bottleneck due to closure object allocation?
        DoRun(alpha, _imageWidth, _imageHeight,
            (ustep, vstep, uoff, voff, invert) => { sorterInfo.Sort(ustep, vstep, uoff, voff, invert); });
    }

    /// <summary>
    /// Sort the image along the angle <paramref name="alpha"/>,
    /// where: <br/>
    ///     alpha(0)      ~ Vertical,   <br/>
    ///     alpha(PI / 2) ~ Horizontal, <br/>
    ///     alpha(PI)     ~ Vertical.   <br/>
    /// This function will execute multithreaded, using the specified parallelization
    /// strategy and options specified in <see cref="ParallelOpts"/>.
    /// </summary>
    /// <param name="alpha">Angle in Radians, element of range [ 0 ; PI ]</param>
    /// <param name="sorterInfo">The information for sorting, which includes: the sorting method, the sorting comparer, etc.</param>
    /// <exception cref="ArgumentOutOfRangeException">For alpha values out of bounds.</exception>
    public void SortAngleAsync(double alpha, AngleSorterInfo sorterInfo)
    {
        var tanAlpha = Math.Tan(alpha);
        int height = _imageHeight, width = _imageWidth;

        switch (alpha)
        {
            case 0:
                Parallel.For(0, width, ParallelOpts, sorterInfo.Clone,
                    (i, _, info) => info.Sort(0, 1, i, 0, false), Noop);
                break;

            case < Math.PI / 4:
                Parallel.For(0, (int)(height * tanAlpha) + 1, ParallelOpts, sorterInfo.Clone,
                    (i, _, info) => info.Sort(tanAlpha, 1, -i, 0, false), Noop);
                Parallel.For(1, width, ParallelOpts, sorterInfo.Clone,
                    (i, _, info) => info.Sort(tanAlpha, 1, i, 0, false), Noop);
                break;

            case Math.PI / 4:
                Parallel.For(0, height, ParallelOpts, sorterInfo.Clone,
                    (i, _, info) => info.Sort(1, 1, -i, 0, false), Noop);
                Parallel.For(1, width, ParallelOpts, sorterInfo.Clone,
                    (i, _, info) => info.Sort(1, 1, i, 0, false), Noop);
                break;

            case < Math.PI / 2:
                Parallel.For(0, height, ParallelOpts, sorterInfo.Clone,
                    (i, _, info) => info.Sort(1, 1 / tanAlpha, 0, i, false), Noop);
                Parallel.For(1, (int)(width / tanAlpha) + 1, ParallelOpts, sorterInfo.Clone,
                    (i, _, info) => info.Sort(1, 1 / tanAlpha, 0, -i, false), Noop);
                break;

            case Math.PI / 2:
                Parallel.For(0, height, ParallelOpts, sorterInfo.Clone,
                    (i, _, info) => info.Sort(1, 0, 0, i, false), Noop);
                break;

            case < Math.PI / 2 + Math.PI / 4:
                Parallel.For(0, height, ParallelOpts, sorterInfo.Clone,
                    (i, _, info) => info.Sort(-1, 1 / -tanAlpha, width - 1, i, true), Noop);
                Parallel.For(1, (int)(width / -tanAlpha) + 2, ParallelOpts, sorterInfo.Clone,
                    (i, _, info) => info.Sort(-1, 1 / -tanAlpha, width - 1, -i, true), Noop);
                break;

            case Math.PI / 2 + Math.PI / 4:
                Parallel.For(0, height, ParallelOpts, sorterInfo.Clone,
                    (i, _, info) => info.Sort(-1, 1, width - 1, i, true), Noop);
                Parallel.For(1, (int)(width / -tanAlpha) + 1, ParallelOpts, sorterInfo.Clone,
                    (i, _, info) => info.Sort(-1, 1, width - 1, -i, true), Noop);
                break;

            case < Math.PI:
                Parallel.For(0, (int)(height * -tanAlpha) + 1, ParallelOpts, sorterInfo.Clone,
                    (i, _, info) => info.Sort(tanAlpha, 1, i + width - 1, 0, true), Noop);
                Parallel.For(1, width, ParallelOpts, sorterInfo.Clone,
                    (i, _, info) => info.Sort(tanAlpha, 1, -i + width - 1, 0, true), Noop);
                break;

            case Math.PI:
                Parallel.For(0, width, ParallelOpts, sorterInfo.Clone,
                    (i, _, info) => info.Sort(0, 1, i, 0, true), Noop);
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(alpha));
        }

        return;

        static void Noop(AngleSorterInfo sorterInfo)
        {
        }
    }

    /// <summary>
    /// Sorts the image in the specified <paramref name="sortDirection"/> using the specified <paramref name="comparer"/>. <br/>
    /// The <paramref name="threshold"/> is used as a minimum threshold for the comparer.
    /// </summary>
    /// <param name="sortDirection"></param>
    /// <param name="comparer"></param>
    /// <param name="threshold"></param>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    [Obsolete("Needs to be reviewed.")]
    public void Sort(SortDirection sortDirection, IComparer<TPixel> comparer, TPixel threshold)
    {
        switch (sortDirection)
        {
            case SortDirection.Horizontal:
                for (var row = 0; row < _imageHeight; row++)
                {
                    var iteratorX = 0;
                    var pixels = new Span<TPixel>(_pixels, _pixelCount);

                    while (NextRowPixelSpan(row, comparer, out var span, ref iteratorX, pixels))
                        IntrospectiveSort(span, comparer);
                }

                break;

            case SortDirection.Vertical:
                for (var col = 0; col < _imageWidth; col++)
                {
                    var iteratorY = 0;
                    var pixels = new Span<TPixel>(_pixels, _pixelCount);

                    while (NextColPixelSpan(col, comparer, out var span, ref iteratorY, pixels))
                        IntrospectiveSort(span, comparer);
                }

                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(sortDirection), sortDirection, null);
        }

        return;

        bool NextRowPixelSpan(
            int y, IComparer<TPixel> comparer, out PixelSpan span, ref int iteratorX, Span<TPixel> pixels)
        {
            // Remember where we started.
            var begin = iteratorX;

            // Get current lo.
            var lo = y * _imageWidth;

            // Go to next span start.
            while (iteratorX < _imageWidth && comparer.Compare(pixels[lo + iteratorX], threshold) < 0)
            {
                iteratorX++;
            }

            // Get next span width.
            while (iteratorX < _imageWidth && comparer.Compare(pixels[lo + iteratorX], threshold) >= 0)
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

        bool NextColPixelSpan(
            int x, IComparer<TPixel> comparer, out PixelSpan span, ref int iteratorY, Span<TPixel> pixels)
        {
            // Remember where we started
            var begin = iteratorY;

            // Go to next span start.
            while (iteratorY < _pixelCount && comparer.Compare(pixels[x + iteratorY], threshold) < 0)
            {
                iteratorY += _imageWidth;
            }

            // Get next span width.
            while (iteratorY < _pixelCount && comparer.Compare(pixels[x + iteratorY], threshold) >= 0)
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
    }
}