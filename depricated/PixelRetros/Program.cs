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
    static (string Original, string Sorted) GetPaths(string folder, string name) => (
        Original: Path.Combine(folder, name), 
        Sorted: $"{Path.Combine(folder, Path.GetFileNameWithoutExtension(name))} _ Sorted{Path.GetExtension(name)}"
    );
    static (string Original, string Sorted) GetPaths(string path) => ( 
        Original: path,
        Sorted: $"{Path.Combine(Path.GetDirectoryName(path)!, Path.GetFileNameWithoutExtension(path))} _ Sorted{Path.GetExtension(path)}"
    );
    static List<(string Original, string Sorted)> Samples = new();

    static void Main(string[] args)
    {
        if (args.Length != 0) Samples.AddRange(Directory.GetFiles(args[0]).Select(s => GetPaths(s)));

        // Set Benchmark Meta Variables
        Benchmark.WarmupTimePerMethod = TimeSpan.FromSeconds(5);
        Benchmark.BenchmarkTimePerMethod = TimeSpan.FromSeconds(10);

        // Benchmark small and large sample images sizes
        Func<Bitmap, int> Size = x => x.Height * x.Width;
        var samplesData = Samples.Select(s => new Bitmap(s.Original));

        Console.WriteLine("Min");
        Benchmark.Run<Sorter<Pixel_24bit>>(GetBmpData(samplesData.MinBy(Size)!));

        Console.WriteLine("Max");
        Benchmark.Run<Sorter<Pixel_24bit>>(GetBmpData(samplesData.MaxBy(Size)!));


        // save sorted image
        //var bmp = new Bitmap(Sample.Original);
        //Sorter<Pixel_24bit> sorter;

        //sorter = new(GetBmpData(bmp));
        //sorter.HorizontalInner();
        //bmp.UnlockBits(sorter.BitmapData);
        //bmp.Save(Sample.Sorted);

        //sorter = new(GetBmpData(bmp));
        //sorter.HorizontalOuter();
        //bmp.UnlockBits(sorter.BitmapData);
        //bmp.Save(Sample.Sorted);
    }

    static BitmapData GetBmpData(Bitmap bmp)
    {
        Rectangle rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
        return bmp.LockBits(rect, ImageLockMode.ReadWrite, bmp.PixelFormat);
    }

}


