using PixelRetros.Benchmark;
using SortingLibrary;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;


// disable for everything
#pragma warning disable CA1416


static class Program
{
    const string IMG =          @"D:\Programmmieren\Projects\ImageSorterTesting\102038267_p0 (Custom).bmp";
    const string IMG_SORTED =   @"D:\Programmmieren\Projects\ImageSorterTesting\102038267_p0 (Custom)_Sorted.bmp";


    static void Main()
    {
        // init
        Bitmap bmp = new Bitmap(IMG);
        var sorter = new Sorter<Pixel_24bit>(GetBmpData(bmp));


        // benchmark
        //Benchmark.Run<SorterBenchmark>();


        // testing
        sorter.IntroTesting(new Comparer24bit.Descending.Blue());


        //save sorted image
        sorter.SortDirection = SortDirection.Vertical;
        sorter.StdSort(new Comparer24bit.Descending.Blue());
        bmp.UnlockBits(sorter.BitmapData);
        bmp.Save(IMG_SORTED);
    }


    class SorterBenchmark : Sorter<Pixel_24bit> 
    { 
        public SorterBenchmark() : base(GetBmpData(new Bitmap(IMG))) { } 
    }


    static BitmapData GetBmpData(Bitmap bmp)
    {
        Rectangle rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
        return bmp.LockBits(rect, ImageLockMode.ReadWrite, bmp.PixelFormat);
    }

}


