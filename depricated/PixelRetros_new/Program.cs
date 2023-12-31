

using BenchmarkDotNet.Running;
using SortingLibrary;
using System.Drawing.Imaging;
using System.Drawing;

class Program
{
    static Bitmap _bmp;
    static BitmapData _bmpData;

    static Sorter<Pixel_24bit> GetNewSorter => new Sorter<Pixel_24bit>(_bmpData);

    class SorterBenchmark : Sorter<Pixel_24bit> { public SorterBenchmark() : base(_bmpData) { } }


    
    static void Main()
    {
        // init
        string bigImg = @"D:\Programmmieren\Projects\ImageSorterTesting\102038267_p0 (Custom).bmp";
        _bmp = new Bitmap(bigImg);
        Rectangle rect = new Rectangle(0, 0, _bmp.Width, _bmp.Height);
        _bmpData = _bmp.LockBits(rect, ImageLockMode.ReadWrite, _bmp.PixelFormat);


        // benchmark
        BenchmarkRunner.Run<SorterBenchmark>();
    }

}

