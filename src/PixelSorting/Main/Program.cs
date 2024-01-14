

global using Pixel32bit = int;


using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using Imaging;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Sorting;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using TestDataGenerator;


//const string SOURCE = "../../../../../SampleImages/sample-image (1080p Full HD).bmp";
//const string RESULT = "../../../../../SampleImages/sample-image (1080p Full HD)_32bit.bmp";

//var bmp = Imaging.Utils.GetBitmap(SOURCE);
//var data = Imaging.Utils.ExposeData(bmp);
//var sorter = new Sorter<Pixel32bit>(data.Scan0, data.Width, data.Height, data.Stride);

//List<Pixel_24bit[]> rows = new();
//for (int row = 0; row < data.Height; row++)
//{
//    rows.Add(sorter.GetRow(row).ToArray());
//}

////var newSorter = sorter.CastToPixelFormat<Pixel32bit>((a, b) => { });

//bmp.Save(RESULT);


//var pixel = Pixel32bit_Util.FromARGB(10, 20, 30, 40);

//Console.WriteLine(pixel.ToPixelString());

//pixel = pixel & Pixel32bit_Util.AMask;

//Console.WriteLine(pixel.ToPixelString());


BenchmarkSwitcher.FromTypes([typeof(GenericPixelStructureBenchmark<,>)]).RunAllJoined();


public class SortBenchmark
{
    private readonly ComparerIntPixel_soA_stR_1 _comparer = new();

    public IEnumerable<TestDataSize> valuesFordatasizes => Generator.GetDefaultTestingDataset();

    [ParamsSource(nameof(valuesFordatasizes))]
    public TestDataSize Datasize;

    [Benchmark]
    public void InsertionSort()
    {
        var tests = Generator.GenerateTestingData<Pixel32bit>([Datasize], _comparer, 420).ToList();
        foreach (var test in tests)
        {
            Sorter<Pixel32bit>.InsertionSort(new Sorter<int>.PixelSpan(test.Unsorted, test.Properties.Step, test.Properties.From, test.Properties.To), _comparer);
        }
    }

    [Benchmark]
    public void HeapSort()
    {
        var tests = Generator.GenerateTestingData<Pixel32bit>([Datasize], _comparer, 420).ToList();
        foreach (var test in tests)
        {
            Sorter<Pixel32bit>.HeapSort(new Sorter<int>.PixelSpan(test.Unsorted, test.Properties.Step, test.Properties.From, test.Properties.To), _comparer);
        }
    }

    [Benchmark]
    public void IntrospectiveSort()
    {
        var tests = Generator.GenerateTestingData<Pixel32bit>([Datasize], _comparer, 420).ToList();
        foreach (var test in tests)
        {
            Sorter<Pixel32bit>.IntrospectiveSort(new Sorter<int>.PixelSpan(test.Unsorted, test.Properties.Step, test.Properties.From, test.Properties.To), _comparer);
        }
    }
}

public class SpanBenchmark
{
    /*
    | Method    | Size  | Step | Mean          | Error         | StdDev        |
    |---------- |------ |----- |--------------:|--------------:|--------------:|
    | PixelSpan | 100   | 1    |     15.600 us |     0.0885 us |     0.0828 us |
    | Span      | 100   | 1    |     15.037 us |     0.0678 us |     0.0566 us |
    | PixelSpan | 100   | 3    |      9.616 us |     0.0797 us |     0.0665 us |
    | Span      | 100   | 3    |      9.488 us |     0.0684 us |     0.0640 us |
    | PixelSpan | 100   | 10   |      8.992 us |     0.1037 us |     0.0970 us |
    | Span      | 100   | 10   |      8.779 us |     0.0893 us |     0.0835 us |
    | PixelSpan | 100   | 100  |      8.895 us |     0.0632 us |     0.0560 us |
    | Span      | 100   | 100  |      9.156 us |     0.1781 us |     0.3024 us |
    | PixelSpan | 1000  | 1    |    655.682 us |     5.2068 us |     4.6157 us |
    | Span      | 1000  | 1    |    642.451 us |     2.9418 us |     2.7518 us |
    | PixelSpan | 1000  | 3    |    169.032 us |     1.0326 us |     0.9154 us |
    | Span      | 1000  | 3    |    161.847 us |     1.3545 us |     1.2008 us |
    | PixelSpan | 1000  | 10   |    106.494 us |     1.5202 us |     1.4220 us |
    | Span      | 1000  | 10   |    107.210 us |     0.8853 us |     0.8281 us |
    | PixelSpan | 1000  | 100  |     99.164 us |     0.7178 us |     0.6363 us |
    | Span      | 1000  | 100  |     98.506 us |     0.3856 us |     0.3220 us |
    | PixelSpan | 10000 | 1    | 74,068.267 us | 1,431.7619 us | 2,186.4516 us |
    | Span      | 10000 | 1    | 70,040.456 us |   275.0532 us |   243.8275 us |
    | PixelSpan | 10000 | 3    |  7,185.526 us |    40.1015 us |    35.5489 us |
    | Span      | 10000 | 3    |  6,584.090 us |    20.3662 us |    18.0541 us |
    | PixelSpan | 10000 | 10   |  1,451.991 us |     8.4406 us |     7.8953 us |
    | Span      | 10000 | 10   |  1,391.708 us |    10.1714 us |     9.0167 us |
    | PixelSpan | 10000 | 100  |    948.901 us |     3.0210 us |     2.6780 us |
    | Span      | 10000 | 100  |    884.895 us |     4.0391 us |     3.7782 us |
     */
    [Params(10000, 1000, 100)]
    public int Size { get; set; }

    [Params(100, 10, 1, 3)]
    public int Step { get; set; }

    private ComparerIntPixel_soA_stR_1 comparer = new();
    List<TestInstance<Pixel32bit>> data;

    public SpanBenchmark()
    {
    }


    [Benchmark]
    public void PixelSpan()
    {
        data = Generator.GenerateTestingData<Pixel32bit>([new TestDataSize(Size, Step, 0, Size)], comparer, 420).ToList();
        var instance = data.First();
        Sorter<Pixel32bit>.InsertionSort(new Sorter<int>.PixelSpan(instance.Unsorted, instance.Properties.Step, instance.Properties.From, instance.Properties.To), comparer);
    }

    [Benchmark]
    public void Span()
    {
        data = Generator.GenerateTestingData<Pixel32bit>([new TestDataSize(Size, Step, 0, Size)], comparer, 420).ToList();
        var instance = data.First();
        Sorter<Pixel32bit>.InsertionSort(new Span<int>(instance.Unsorted), comparer, instance.Properties.Step, instance.Properties.From, instance.Properties.To);
    }
}

public class ComparingBenchmark
{
    [ParamsSource(nameof(comparers))]
    public IComparer<Pixel32bit> comparer;
    public IEnumerable<IComparer<Pixel32bit>> comparers => [ 
        new PixelComparer_soA_stR_32bit(),
        new ComparerIntPixel_soA_stR_1(), 
        new ComparerIntPixel_soA_stR_2(),
        new PixelComparer.Ascending.Red._32bit(),
    ];

    //[ParamsSource(nameof(valuesFordatasizes))]
    //public TestDataSize Datasize;
    //public IEnumerable<TestDataSize> valuesFordatasizes => Generator.GetDefaultTestingDataset();

    [Benchmark]
    public void IntrospectiveSort()
    {
        var tests = Generator.GenerateTestingData<Pixel32bit>([new TestDataSize { Size=10000, From=0, Step=1, To=10000 }], comparer, 420).ToList();
        foreach (var test in tests)
        {
            Sorter<Pixel32bit>.IntrospectiveSort(new Sorter<int>.PixelSpan(test.Unsorted, test.Properties.Step, test.Properties.From, test.Properties.To), comparer);
        }
    }

}


[GenericTypeArguments(typeof(Pixel32bit), typeof(PixelComparer.Ascending.Red._32bit))]
[GenericTypeArguments(typeof(Pixel24bitStruct), typeof(PixelComparer.Ascending.Red._24bit))]
[GenericTypeArguments(typeof(Pixel24bitRecord), typeof(PixelComparer.Ascending.Red._24bitStruct))]
/*
| Type                                                           | Method | Mean     | Error    | StdDev   |
|--------------------------------------------------------------- |------- |---------:|---------:|---------:|
| GenericPixelStructureBenchmark<Pixel32bit, _32bit>             | Pixel  | 676.1 us |  6.39 us |  5.98 us |
| GenericPixelStructureBenchmark<Pixel24bitRecord, _24bitStruct> | Pixel  | 797.3 us | 15.83 us | 23.69 us |
| GenericPixelStructureBenchmark<Pixel24bitStruct, _24bit>       | Pixel  | 721.5 us | 10.57 us |  9.37 us |
*/
public class GenericPixelStructureBenchmark<TPixel, TComparer>
    where TPixel : struct
    where TComparer : IComparer<TPixel>, new()
{
    private IEnumerable<TestInstance<TPixel>> testInstances;
    private IComparer<TPixel> comparer = new TComparer();


    // Precompute the testing data s.d. it does not interferce with the benchmark.
    public GenericPixelStructureBenchmark()
    {
        // Take the extra step to cast the same data into different sizes. Otherwise, bytes might get
        // shifted into places they werent in a pixel of different size.
        var defaulttests = Generator.GenerateTestingData<Pixel24bitStruct>([new TestDataSize { Size = 10000, From = 0, Step = 1, To = 10000 }], new PixelComparer.Ascending.Red._24bit(), 420).ToList();
        testInstances = defaulttests.Select(CastTo<TPixel>);
    }


    [Benchmark]
    public void Pixel()
    {
        foreach (var test in testInstances)
        {
            Sorter<TPixel>.IntrospectiveSort(new Sorter<TPixel>.PixelSpan(test.Unsorted, test.Properties.Step, test.Properties.From, test.Properties.To), comparer);
        }
    }

    private static TestInstance<TResultPixel> CastTo<TResultPixel>(TestInstance<Pixel24bitStruct> instance24bit)
    {
        Dictionary<Type, Func<Pixel24bitStruct, object>> pixelConverter = new()
        {
            { typeof(Pixel32bit), x24 => Pixel32bit_Util.From24bit(x24) },
            { typeof(Pixel24bitStruct), x24 => x24 },
            { typeof(Pixel24bitRecord), x24 => new Pixel24bitRecord(x24.R, x24.G, x24.B) }
        };

        return new TestInstance<TResultPixel>
        {
            Properties = instance24bit.Properties,
            Unsorted = instance24bit.Unsorted.Select(pixelConverter[typeof(TResultPixel)]).Cast<TResultPixel>().ToArray(),
            Sorted = instance24bit.Sorted.Select(pixelConverter[typeof(TResultPixel)]).Cast<TResultPixel>().ToArray()
        };
    }
}