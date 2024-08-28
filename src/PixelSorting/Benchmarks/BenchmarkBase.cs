using System.Drawing.Imaging;
using Sorting.Pixels.Comparer;
using static Utils.Utils;
using static Imaging.Utils;

namespace Benchmarks;

public abstract class BenchmarkBase
{
    protected readonly BitmapData[] Data;
    protected readonly PixelComparer.Ascending.Red Comparer = new();


    protected BenchmarkBase(int benchmarks)
    {
        Data = new BitmapData[benchmarks];

        for (var i = 0; i < benchmarks; i++)
        {
            var source = GetBenchmarkCopy(i);
            using var bmp = GetBitmap(source);
            var data = ExposeData(bmp);
            Data[i] = data;
        }
    }

}