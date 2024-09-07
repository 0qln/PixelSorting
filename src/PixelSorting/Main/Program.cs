global using Pixel32bit = int;

using Sorting;
using System.Reflection;
using Sorting.Pixels.KeySelector;
using System.Diagnostics;
using Sorting.Pixels._32;

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
    sorter.SortAngle(angle, sorter.GetAngleSorterInfo(new Sorter32Bit.PigeonholeSorter(new OrderedKeySelector.Descending.Red())));
    bmp.Save(result);

    return str;
}

unsafe void Rotate(int times)
{
    var i = 1;
    for (var x = 0.0; x <= Math.PI; x += Math.PI / times)
    {
        Console.Write($"[{i++,4}] ");

        var watch = Stopwatch.StartNew();

        var str = x.ToString();
        str = (str.Contains('.') ? str : str + '.').PadRight(30, '0');
        str = str[..30];

        var source = Path.GetFullPath(Path.Combine(
            Path.GetDirectoryName(Assembly.GetEntryAssembly()!.Location)!,
            $"../../../../../SampleImages/img_0/sample-image-1920x1080.bmp"));

        var result = Path.GetFullPath(Path.Combine(
            Path.GetDirectoryName(Assembly.GetEntryAssembly()!.Location)!,
            $"../../../../../SampleImages/img_0/sample-image-RESULT-{str}.bmp"));

        var bmp = Imaging.Utils.GetBitmap(source);
        var data = Imaging.Utils.ExposeData(bmp);
        Console.Write($"Loaded in {watch.ElapsedMilliseconds}ms, ");
        var sorter = new Sorter32Bit((Pixel32bitUnion*)data.Scan0, data.Width, data.Height, data.Stride);
        sorter.SortAngle(x, sorter.GetAngleSorterInfo(new Sorter32Bit.PigeonholeSorter(new OrderedKeySelector.Descending.Red())));
        Console.Write($"Sorted in {watch.ElapsedMilliseconds}ms, ");
        bmp.Save(result);
        Console.Write($"Saved in {watch.ElapsedMilliseconds}ms ");
        var angle = str;

        watch.Stop();

        Console.WriteLine($"'sample-image-1920x1080.bmp' with {angle} Radians");
    }
}

Rotate(1024);


#pragma warning restore CA1416 // Validate platform compatibility
