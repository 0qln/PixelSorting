﻿

global using Pixel32bit = int;


using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using Imaging;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Diagnostics.Runtime.Utilities;
using Sorting;
using Sorting.Pixels._24;
using Sorting.Pixels._32;
using Sorting.Pixels._8;
using Sorting.Pixels.Comparer;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using TestDataGenerator;



//for (double alpha = 0.0; alpha <= Math.PI; alpha += 0.1)
for (int x = 500; x <= 3500; x += 500)
    //int x = 3840;
{
    string str = x.ToString();
    //str = (str.Length < 3 ? str + ".0" : str)[..3];
    str = x.ToString();
    string SOURCE = $"../../../../../SampleImages/img_0/sample-image-SOURCE.bmp";
    string RESULT = $"../../../../../SampleImages/img_0/sample-image-RESULT-{str}.bmp";
    //string RESULT = $"../../../../../SampleImages/img_0/sample-image-RESULT.bmp";

#pragma warning disable CA1416 // Validate platform compatibility

    var bmp = Imaging.Utils.GetBitmap(SOURCE);
    var data = Imaging.Utils.ExposeData(bmp);
    var sorter = new Sorter<Pixel24bitExplicitStruct>(data.Scan0, data.Width, data.Height, data.Stride);
    sorter.SortCornerTriangleRightBottom(x, new PixelComparer.Ascending.GrayScale._24bitExplicitStruct());
    bmp.Save(RESULT);

#pragma warning restore CA1416 // Validate platform compatibility
}


//BenchmarkSwitcher.FromTypes([typeof(GenericPixelStructureBenchmark<,>)]).RunAllJoined();


public class SortBenchmark
{
    const string SOURCE = "../../../../../SampleImages/sample-image (1080p Full HD).bmp";


    public SortBenchmark()
    {
        data = Imaging.Utils.ExposeData(Imaging.Utils.GetBitmap(SOURCE));
        comparer = new();
        threshhold = new Pixel24bitExplicitStruct { B = 100, G = 100, R = 200 };
    }

    public BitmapData data;
    PixelComparer.Ascending.Red._24bitExplicitStruct comparer;
    static Pixel24bitExplicitStruct threshhold;


    [Benchmark]
    public void ThreshholdWithSpan()
    {
        var sorter = new Sorter<Pixel24bitExplicitStruct>(data.Scan0, data.Width, data.Height, data.Stride);
        //sorter.Sort(SortDirection.Horizontal, comparer, threshhold, true);
    }

    [Benchmark]
    public void ThreshholdWithoutSpan()
    {
        var sorter = new Sorter<Pixel24bitExplicitStruct>(data.Scan0, data.Width, data.Height, data.Stride);
        //sorter.Sort(SortDirection.Horizontal, comparer, threshhold, false);
    }

    [Benchmark]
    public void NoThreshhold()
    {
        var sorter = new Sorter<Pixel24bitExplicitStruct>(data.Scan0, data.Width, data.Height, data.Stride);
        sorter.Sort(SortDirection.Horizontal, comparer);
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

    private PixelComparer.Ascending.Red._32bit comparer = new();
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
        //new PixelComparer_soA_stR_32bit(),
        //new ComparerIntPixel_soA_stR_1(), 
        //new ComparerIntPixel_soA_stR_2(),
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


//[GenericTypeArguments(typeof(Pixel32bit), typeof(PixelComparer.Ascending.Red._32bit))]
//[GenericTypeArguments(typeof(Pixel32bitUnion), typeof(PixelComparer.Ascending.Red._32bitUnion))]
//[GenericTypeArguments(typeof(Pixel32bitStruct), typeof(PixelComparer.Ascending.Red._32bitStruct))]
//[GenericTypeArguments(typeof(Pixel32bitExplicitStruct), typeof(PixelComparer.Ascending.Red._32bitExplicitStruct))]
/* Benchmarks
| Type                                                           | Method | SIZE                 | Mean        | Error     | StdDev    |
|--------------------------------------------------------------- |------- |--------------------- |------------:|----------:|----------:|
| GenericPixelStructureBenchmark<Int32, _32bit>                  | Pixel  | TestD(...)000 } [59] |    13.25 us |  0.100 us |  0.093 us |
| GenericPixelStructureBenchmark<Pixel24bitRecord, _24bitStruct> | Pixel  | TestD(...)000 } [59] |    13.99 us |  0.039 us |  0.035 us |
| GenericPixelStructureBenchmark<Pixel24bitStruct, _24bit>       | Pixel  | TestD(...)000 } [59] |    14.10 us |  0.049 us |  0.041 us |

| GenericPixelStructureBenchmark<Int32, _32bit>                  | Pixel  | TestD(...)000 } [61] |   199.22 us |  1.019 us |  0.954 us |
| GenericPixelStructureBenchmark<Pixel24bitRecord, _24bitStruct> | Pixel  | TestD(...)000 } [61] |   243.98 us |  0.226 us |  0.200 us |
| GenericPixelStructureBenchmark<Pixel24bitStruct, _24bit>       | Pixel  | TestD(...)000 } [61] |   243.87 us |  0.516 us |  0.483 us |

| GenericPixelStructureBenchmark<Int32, _32bit>                  | Pixel  | TestD(...)000 } [63] | 2,401.08 us | 42.840 us | 54.178 us |
| GenericPixelStructureBenchmark<Pixel24bitRecord, _24bitStruct> | Pixel  | TestD(...)000 } [63] | 2,837.27 us |  9.344 us |  7.803 us |
| GenericPixelStructureBenchmark<Pixel24bitStruct, _24bit>       | Pixel  | TestD(...)000 } [63] | 2,707.20 us | 34.741 us | 32.496 us |


| Type                                                                           | Method | SIZE                 | Mean        | Error     | StdDev    |
|------------------------------------------------------------------------------- |------- |--------------------- |------------:|----------:|----------:|
| GenericPixelStructureBenchmark<Int32, _32bit>                                  | Pixel  | TestD(...)000 } [59] |    13.37 us |  0.235 us |  0.271 us |
| GenericPixelStructureBenchmark<Pixel32bitExplicitStruct, _32bitExplicitStruct> | Pixel  | TestD(...)000 } [59] |    18.23 us |  0.064 us |  0.060 us |
| GenericPixelStructureBenchmark<Pixel32bitStruct, _32bitStruct>                 | Pixel  | TestD(...)000 } [59] |    18.26 us |  0.063 us |  0.059 us |
| GenericPixelStructureBenchmark<Pixel32bitUnion, _32bitUnion>                   | Pixel  | TestD(...)000 } [59] |    14.20 us |  0.282 us |  0.431 us |
| GenericPixelStructureBenchmark<Int32, _32bit>                                  | Pixel  | TestD(...)000 } [61] |   210.65 us |  1.039 us |  0.868 us |
| GenericPixelStructureBenchmark<Pixel32bitExplicitStruct, _32bitExplicitStruct> | Pixel  | TestD(...)000 } [61] |   209.87 us |  0.800 us |  0.668 us |
| GenericPixelStructureBenchmark<Pixel32bitStruct, _32bitStruct>                 | Pixel  | TestD(...)000 } [61] |   210.34 us |  0.706 us |  0.660 us |
| GenericPixelStructureBenchmark<Pixel32bitUnion, _32bitUnion>                   | Pixel  | TestD(...)000 } [61] |   187.86 us |  0.813 us |  0.679 us |
| GenericPixelStructureBenchmark<Int32, _32bit>                                  | Pixel  | TestD(...)000 } [63] | 2,313.41 us | 10.993 us |  9.179 us |
| GenericPixelStructureBenchmark<Pixel32bitExplicitStruct, _32bitExplicitStruct> | Pixel  | TestD(...)000 } [63] | 2,284.46 us | 21.973 us | 19.479 us |
| GenericPixelStructureBenchmark<Pixel32bitStruct, _32bitStruct>                 | Pixel  | TestD(...)000 } [63] | 2,249.94 us | 20.948 us | 17.493 us |
| GenericPixelStructureBenchmark<Pixel32bitUnion, _32bitUnion>                   | Pixel  | TestD(...)000 } [63] | 2,180.54 us | 22.532 us | 21.077 us |
|------------------------------------------------------------------------------- |------- |--------------------- |------------:|----------:|----------:|
| GenericPixelStructureBenchmark<Int32, _32bit>                                  | Pixel  | TestD(...)000 } [59] |    13.43 us |  0.163 us |  0.136 us |
| GenericPixelStructureBenchmark<Pixel32bitExplicitStruct, _32bitExplicitStruct> | Pixel  | TestD(...)000 } [59] |    15.88 us |  0.075 us |  0.070 us |
| GenericPixelStructureBenchmark<Pixel32bitStruct, _32bitStruct>                 | Pixel  | TestD(...)000 } [59] |    16.02 us |  0.132 us |  0.117 us |
| GenericPixelStructureBenchmark<Pixel32bitUnion, _32bitUnion>                   | Pixel  | TestD(...)000 } [59] |    13.77 us |  0.091 us |  0.076 us |
| GenericPixelStructureBenchmark<Int32, _32bit>                                  | Pixel  | TestD(...)000 } [61] |   218.08 us |  3.859 us |  3.610 us |
| GenericPixelStructureBenchmark<Pixel32bitExplicitStruct, _32bitExplicitStruct> | Pixel  | TestD(...)000 } [61] |   223.13 us |  0.734 us |  0.686 us |
| GenericPixelStructureBenchmark<Pixel32bitStruct, _32bitStruct>                 | Pixel  | TestD(...)000 } [61] |   211.53 us |  0.846 us |  0.707 us |
| GenericPixelStructureBenchmark<Pixel32bitUnion, _32bitUnion>                   | Pixel  | TestD(...)000 } [61] |   187.66 us |  0.828 us |  0.774 us |
| GenericPixelStructureBenchmark<Int32, _32bit>                                  | Pixel  | TestD(...)000 } [63] | 2,239.87 us | 34.779 us | 32.533 us |
| GenericPixelStructureBenchmark<Pixel32bitExplicitStruct, _32bitExplicitStruct> | Pixel  | TestD(...)000 } [63] | 2,206.87 us | 10.678 us |  9.466 us |
| GenericPixelStructureBenchmark<Pixel32bitStruct, _32bitStruct>                 | Pixel  | TestD(...)000 } [63] | 2,210.49 us |  9.911 us |  8.276 us |
| GenericPixelStructureBenchmark<Pixel32bitUnion, _32bitUnion>                   | Pixel  | TestD(...)000 } [63] | 2,153.10 us |  8.551 us |  7.140 us |

 */


[GenericTypeArguments(typeof(Pixel24bitStruct), typeof(PixelComparer.Ascending.Red._24bitStruct))]
[GenericTypeArguments(typeof(Pixel24bitRecord), typeof(PixelComparer.Ascending.Red._24bitRecord))]
[GenericTypeArguments(typeof(Pixel24bitExplicitStruct), typeof(PixelComparer.Ascending.Red._24bitExplicitStruct))]
/* Benchmarks
all about equal...
 */


public class GenericPixelStructureBenchmark<TPixel, TComparer>
    where TPixel : struct
    where TComparer : IComparer<TPixel>, new()
{
    public IEnumerable<TestDataSize> TestInstancesSource => Generator.GetRealisticTestingDataset();

    [ParamsSource(nameof(TestInstancesSource))]
    public TestDataSize SIZE;

    private Dictionary<TestDataSize, TestInstance<TPixel>> testInstances = new();

    private IComparer<TPixel> comparer = new TComparer();


    // Precompute the all testing data s.d. it does not interferce with the benchmark.
    public GenericPixelStructureBenchmark()
    {
        // Take the extra step to cast the same data into different sizes. Otherwise, bytes might get
        // shifted into places they werent in a pixel of different size.
        var defaulttests = Generator.GenerateTestingData<Pixel24bitStruct>(TestInstancesSource, new PixelComparer.Ascending.Red._24bitStruct(), 69).ToList();
        foreach (var test in defaulttests) testInstances.Add(test.Properties, CastTo<TPixel>(test));
    }


    [Benchmark]
    public void Pixel()
    {
        var test = testInstances[SIZE];
        Sorter<TPixel>.IntrospectiveSort(new Sorter<TPixel>.PixelSpan(test.Unsorted, test.Properties.Step, test.Properties.From, test.Properties.To), comparer);
    }

    private static TestInstance<TResultPixel> CastTo<TResultPixel>(TestInstance<Pixel24bitStruct> instance24bit)
    {
        Dictionary<Type, Func<Pixel24bitStruct, object>> pixelConverter = new()
        {
            { typeof(Pixel24bitStruct), x24 => x24 },
            { typeof(Pixel24bitRecord), x24 => new Pixel24bitRecord(x24.R, x24.G, x24.B) },
            { typeof(Pixel24bitExplicitStruct), x24 => new Pixel24bitExplicitStruct { R = x24.R, G = x24.G, B = x24.B } },

            { typeof(Pixel32bit), x24 => Pixel32bit_Util.From24bit(x24) },
            { typeof(Pixel32bitUnion), x24 => new Pixel32bitUnion { A = 255, R = x24.R, G = x24.G, B = x24.B } },
            { typeof(Pixel32bitStruct), x24 => new Pixel32bitStruct{ A = 255, R = x24.R, G = x24.G, B = x24.B } },
            { typeof(Pixel32bitExplicitStruct), x24 => new Pixel32bitExplicitStruct{ A = 255, R = x24.R, G = x24.G, B = x24.B } }
        };

        return new TestInstance<TResultPixel>
        {
            Properties = instance24bit.Properties,
            Unsorted = instance24bit.Unsorted.Select(pixelConverter[typeof(TResultPixel)]).Cast<TResultPixel>().ToArray(),
            Sorted = instance24bit.Sorted.Select(pixelConverter[typeof(TResultPixel)]).Cast<TResultPixel>().ToArray()
        };
    }
}