global using Pixel32bit = int;

using Sorting;
using System.Reflection;
using Sorting.Pixels.KeySelector;
using System.Diagnostics;
using System.Drawing;
using Sorting.Pixels._32;
using Sorting.Pixels.Comparer;

#pragma warning disable CA1416 // Validate platform compatibility

unsafe string RunThrough(double angle, int outPrecision = 6)
{
    var str = angle.ToString();
    str = (str.Contains('.') ? str : str + '.').PadRight(outPrecision, '0');
    str = str[..outPrecision];

    var source = Path.GetFullPath(Path.Combine(
            Path.GetDirectoryName(Assembly.GetEntryAssembly()!.Location)!,
            $"../../../../../SampleImages/img_0/sample-image-1920x1080.bmp"));

    var result = Path.GetFullPath(Path.Combine(
            Path.GetDirectoryName(Assembly.GetEntryAssembly()!.Location)!,
            $"../../../../../SampleImages/img_0/sample-image-RESULT-{str}.bmp"));

    var bmp = Imaging.Utils.GetBitmap(source);
    var data = Imaging.Utils.ExposeData(bmp);
    var sorter = new Sorter32Bit((Pixel32bitUnion*)data.Scan0, data.Width, data.Height, data.Stride);
    sorter.SortAngleAsync(angle, sorter.GetAngleSorterInfo(new Sorter32Bit.PigeonholeSorter(new OrderedKeySelector.Descending.Red())));
    bmp.Save(result);

    return str;
}

unsafe void Rotate(int times)
{
    string sampleImage = "sample-image-1920x1080.bmp";

    Console.WriteLine($"Rotating image: {sampleImage}");

    var source = Path.GetFullPath(Path.Combine(
        Path.GetDirectoryName(Assembly.GetEntryAssembly()!.Location)!,
        $"../../../../../SampleImages/img_0/{sampleImage}"));

    var sourceBmp = Imaging.Utils.GetBitmap(source);

    var i = 0;
    for (var x = 0.0; x <= Math.PI; x += Math.PI / times)
    {
        var str = x.ToString();
        str = (str.Contains('.') ? str : str + '.').PadRight(30, '0');
        str = str[..30];
        
        Console.Write($"[{i++,4}] ");
        Console.Write($"[{str} Rad] ");

        var watch = Stopwatch.StartNew();

        using var bmp = new Bitmap(sourceBmp);
        var data = Imaging.Utils.ExposeData(bmp);
        Console.Write($"Loaded after {watch.ElapsedMilliseconds}ms, ");

        var sorter = new Sorter32Bit((Pixel32bitUnion*)data.Scan0, data.Width, data.Height, data.Stride);
        // sorter.SortAngle(x, sorter.GetAngleSorterInfo(new Sorter32Bit.PigeonholeSorter(new OrderedKeySelector.Ascending.Red())));
        sorter.SortAngleAsync(x, sorter.GetAngleSorterInfo(new Sorter32Bit.IntrospectiveSorter(new PixelComparer.Ascending.Hue())));
        Console.Write($"Sorted after {watch.ElapsedMilliseconds}ms, ");

        var result = Path.GetFullPath(Path.Combine(
            Path.GetDirectoryName(Assembly.GetEntryAssembly()!.Location)!,
            $"../../../../../SampleImages/img_0/sample-image-RESULT-{str}.bmp"));
        bmp.Save(result);
        Console.Write($"Saved after {watch.ElapsedMilliseconds}ms ");

        Console.WriteLine();

        watch.Stop();
    }
}

unsafe void RotateVisualizeOverlap(int times)
{
    var i = 0;
    for (var x = 0.0; x <= Math.PI; x += Math.PI / times)
    {
        var str = x.ToString();
        str = (str.Contains('.') ? str : str + '.').PadRight(30, '0');
        str = str[..30];
        
        Console.Write($"[{i++,4}] ");
        Console.Write($"[{str} Rad] ");
        Console.WriteLine();
        Imaging.Utils.VisualizeOverlap(x);
    }
}

void RotateRangeVisualizeOverlap(int times, double begin, double end)
{
    var i = 0;
    for (var x = begin; x <= end; x += Math.PI / times)
    {
        var str = x.ToString();
        str = (str.Contains('.') ? str : str + '.').PadRight(30, '0');
        str = str[..30];
        
        Console.Write($"[{i++,4}] ");
        Console.Write($"[{str} Rad] ");
        Console.WriteLine();
        Imaging.Utils.VisualizeOverlap(x);
    }
} 

Rotate(24);
// RotateRangeVisualizeOverlap(100, Math.PI / 4, Math.PI / 2);
// RotateVisualizeOverlap(24);
// Imaging.Utils.VisualizeOverlap(Math.PI / 25);

return;
var top = 24;
foreach (var i in Enumerable.Range(1, top))
{
    var alpha = Math.PI / top * i;
    Imaging.Utils.VisualizeOverlap(alpha);
}

#pragma warning restore CA1416 // Validate platform compatibility
