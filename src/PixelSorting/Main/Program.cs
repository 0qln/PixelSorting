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
    sorter.SortAngle(angle, sorter.PigeonSorter(new OrderedKeySelector.Descending.Red()));
    bmp.Save(result);

    return str;
}

void Rotate(int times)
{
    var i = 1;
    for (var x = 0.0; x <= Math.PI; x += Math.PI / times)
    {
        var watch = Stopwatch.StartNew();

        var angle = RunThrough(x, 30);

        watch.Stop();

        Console.WriteLine($"[{i++,4}] Loaded, sorted, and saved 'sample-image-1920x1080.bmp' with {angle} Radians in {watch.ElapsedMilliseconds} Milliseconds");
    }
}

Rotate(1024);

//return;

//BenchmarkRunner.Run<SortBenchmark>();

// BenchmarkSwitcher.FromTypes([typeof(GenericPixelStructureBenchmark<,>)]).RunAllJoined();


#pragma warning restore CA1416 // Validate platform compatibility
