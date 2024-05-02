

global using Pixel32bit = int;


using BenchmarkDotNet.Attributes;
using Sorting;
using Sorting.Pixels._24;
using Sorting.Pixels._32;
using Sorting.Pixels.Comparer;
using System.Drawing.Imaging;
using TestDataGenerator;
using System.Reflection;
using BenchmarkDotNet.Running;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Sorting.Pixels.KeySelector;

#pragma warning disable CA1416 // Validate platform compatibility

for (double x = 0.0; x < Math.PI; x += 0.1)
//double x = Math.PI / 2;
{
    break;
    string str = x.ToString();
    str = (str.Length < 3 ? str + ".0" : str)[..3];
    string SOURCE = Path.GetFullPath(Path.Combine(
            Path.GetDirectoryName(Assembly.GetEntryAssembly()!.Location)!,
            $"../../../../../SampleImages/img_0/sample-image-1920x1080.bmp"));

    string RESULT = Path.GetFullPath(Path.Combine(
            Path.GetDirectoryName(Assembly.GetEntryAssembly()!.Location)!,
            $"../../../../../SampleImages/img_0/sample-image-RESULT-{str}.bmp"));

    var bmp = Imaging.Utils.GetBitmap(SOURCE);
    var data = Imaging.Utils.ExposeData(bmp);
    var sorter = new Sorter<Pixel32bitUnion>(data.Scan0, data.Width, data.Height, data.Stride);
    //sorter.AngleSort(x, sorter.FastSort(new PixelComparer.Ascending.Red._32bitUnion()));
    sorter.AngleSort(x, sorter.InsertionSorter(new PixelComparer.Descending.Red._32bitUnion()));
    bmp.Save(RESULT);

    Console.WriteLine("Finish iteration " + x);
}


//new SortBenchmark() { size = 1920 }.Insertion();


//return;

BenchmarkRunner.Run<SortBenchmark>();

//BenchmarkSwitcher.FromTypes([typeof(GenericPixelStructureBenchmark<,>)]).RunAllJoined();


public class SortBenchmark
{
    #region Exception Handling

    /* 
    
    Without bounds check:
    | Method | Mean     | Error    | StdDev   |
    |------- |---------:|---------:|---------:|
    | Unsafe | 16.38 ms | 0.327 ms | 0.306 ms |
    | Safe   | 19.91 ms | 0.240 ms | 0.224 ms |

    Without flooring before adding u and v (probably gives wrong results, should be tested):
    | Method | Mean     | Error    | StdDev   |
    |------- |---------:|---------:|---------:|
    | Unsafe | 14.99 ms | 0.065 ms | 0.058 ms |

    `with` keyword as initiator:
    | Method | Mean     | Error    | StdDev   |
    |------- |---------:|---------:|---------:|
    | Unsafe | 14.98 ms | 0.039 ms | 0.031 ms |
    
    Index precalculationg and caching:
    | Method | Mean     | Error    | StdDev   |
    |------- |---------:|---------:|---------:|
    | Unsafe | 11.24 ms | 0.097 ms | 0.086 ms |

    Inlined indexer span:
    | Method | Mean     | Error    | StdDev   |
    |------- |---------:|---------:|---------:|
    | Unsafe | 10.50 ms | 0.033 ms | 0.031 ms |

    Reintroduce bounds checks:
    | Method | Mean     | Error    | StdDev   |
    |------- |---------:|---------:|---------:|
    | Unsafe | 11.74 ms | 0.197 ms | 0.184 ms |

     */

    //[Benchmark]
    //public void Unsafe()
    //{
    //    source = Path.GetFullPath(Path.Combine(
    //            Path.GetDirectoryName(Assembly.GetEntryAssembly()!.Location)!,
    //            $"../../../../../../../../../SampleImages/img_0/benchmark_copy_1.bmp"));
    //    data = Imaging.Utils.ExposeData(Imaging.Utils.GetBitmap(source));
    //    comparer = new();
    //    var sorter = new Sorter<Pixel32bitUnion>(data.Scan0, data.Width, data.Height, data.Stride);
    //    sorter.SortUnsafe(Math.PI / 2, comparer);
    //}

    //[Benchmark]
    //public void Safe()
    //{
    //    source = Path.GetFullPath(Path.Combine(
    //            Path.GetDirectoryName(Assembly.GetEntryAssembly()!.Location)!,
    //            $"../../../../../../../../../SampleImages/img_0/benchmark_copy_2.bmp"));
    //    data = Imaging.Utils.ExposeData(Imaging.Utils.GetBitmap(source));
    //    comparer = new();
    //    var sorter = new Sorter<Pixel32bitUnion>(data.Scan0, data.Width, data.Height, data.Stride);
    //    sorter.Sort(Math.PI / 2, comparer);
    //}

    #endregion


    #region Directional

    /* 
    
    | Method            | Mean     | Error    | StdDev   |
    |------------------ |---------:|---------:|---------:|
    | ConstantDirection | 10.55 ms | 0.073 ms | 0.065 ms |
    | DynamicDirection  | 19.22 ms | 0.085 ms | 0.071 ms |

    holy overhead!

     */

    ////[Benchmark]
    //public void ConstantDirection()
    //{
    //    source = Path.GetFullPath(Path.Combine(
    //            Path.GetDirectoryName(Assembly.GetEntryAssembly()!.Location)!,
    //            $"../../../../../../../../../SampleImages/img_0/benchmark_copy_1.bmp"));
    //    data = Imaging.Utils.ExposeData(Imaging.Utils.GetBitmap(source));
    //    comparer = new();
    //    var sorter = new Sorter<Pixel32bitUnion>(data.Scan0, data.Width, data.Height, data.Stride);
    //    sorter.Sort(SortDirection.Horizontal, comparer);
    //}

    ////[Benchmark]
    //public void DynamicDirection()
    //{
    //    source = Path.GetFullPath(Path.Combine(
    //            Path.GetDirectoryName(Assembly.GetEntryAssembly()!.Location)!,
    //            $"../../../../../../../../../SampleImages/img_0/benchmark_copy_2.bmp"));
    //    data = Imaging.Utils.ExposeData(Imaging.Utils.GetBitmap(source));
    //    comparer = new();
    //    var sorter = new Sorter<Pixel32bitUnion>(data.Scan0, data.Width, data.Height, data.Stride);
    //    sorter.Sort(Math.PI / 2, comparer);
    //}

    #endregion


    #region Sorting method

    /*
    
    | Method    | size | Mean        | Error     | StdDev    |
    |---------- |----- |------------:|----------:|----------:|
    
    | Intro     | 1280 |    76.50 us |  1.331 us |  1.245 us |
    | Pigeon    | 1280 |    28.49 us |  0.258 us |  0.201 us |
    | Heap      | 1280 |   118.96 us |  2.305 us |  2.654 us |
    | Insertion | 1280 |   711.55 us | 11.506 us | 10.200 us |
    | Comb      | 1280 |   116.72 us |  2.284 us |  2.805 us |
    | Shell     | 1280 |   118.41 us |  2.363 us |  5.430 us |

    | Intro     | 1920 |   116.44 us |  2.296 us |  2.457 us |
    | Pigeon    | 1920 |    39.29 us |  0.660 us |  1.066 us |
    | Heap      | 1920 |   173.64 us |  0.475 us |  0.371 us |
    | Insertion | 1920 | 1,107.09 us |  9.524 us |  8.442 us |
    | Comb      | 1920 |   162.30 us |  0.321 us |  0.284 us |
    | Shell     | 1920 |   173.76 us |  1.498 us |  1.328 us |
    
    | Intro     | 2560 |   153.64 us |  0.870 us |  0.727 us |
    | Pigeon    | 2560 |    47.42 us |  0.307 us |  0.257 us |
    | Heap      | 2560 |   239.18 us |  2.938 us |  2.748 us |
    | Insertion | 2560 | 2,752.26 us | 51.128 us | 66.481 us |
    | Comb      | 2560 |   224.01 us |  4.347 us |  5.652 us |
    | Shell     | 2560 |   231.80 us |  0.993 us |  0.829 us |
    
    | Intro     | 3840 |   244.66 us |  3.099 us |  2.747 us |
    | Pigeon    | 3840 |    70.12 us |  0.665 us |  0.589 us |
    | Heap      | 3840 |   383.10 us |  3.288 us |  2.567 us |
    | Insertion | 3840 | 4,364.99 us | 84.025 us | 96.763 us |
    | Comb      | 3840 |   345.23 us |  2.139 us |  1.896 us |
    | Shell     | 3840 |   358.01 us |  4.541 us |  4.248 us |
    
     */

    public IEnumerable<int> TestInstancesSource => Generator.CommonImageSizesL().Select(x => x.Horizontal).Skip(4);

    [ParamsSource(nameof(TestInstancesSource))]
    public int size;

    private IOrderedKeySelector<Pixel32bitUnion> selector = new OrderedKeySelector.Descending.Red._32bitUnion();
    private IComparer<Pixel32bitUnion> comparer = new PixelComparer.Descending.Red._32bitUnion();

    private nint[] indeces;

    private Random rng = new Random(857943);


    private Sorter<Pixel32bitUnion>.PixelSpan2D PrepareInput()
    {
        var test = Enumerable.Range(0, size).Select(x => new Pixel32bitUnion { Int = rng.Next() }).ToArray();
        indeces = new nint[size];
        return new Sorter<Pixel32bitUnion>.PixelSpan2D(test, indeces, size, 1, 1, 0, 0, 0);
    }

    [Benchmark] public void Pigeon() => Sorter<Pixel32bitUnion>.PigeonholeSort(PrepareInput(), selector);

    //[Benchmark] public void Intro() => Sorter<Pixel32bitUnion>.IntrospectiveSort(PrepareInput(), comparer);
    //[Benchmark] public void Pigeon() => Sorter<Pixel32bitUnion>.PigeonholeSort(PrepareInput(), selector);
    //[Benchmark] public void Heap() => Sorter<Pixel32bitUnion>.HeapSort(PrepareInput(), comparer);
    //[Benchmark] public void Insertion() => Sorter<Pixel32bitUnion>.InsertionSort(PrepareInput(), comparer);
    //[Benchmark] public void Comb() => Sorter<Pixel32bitUnion>.CombSort(PrepareInput(), comparer);
    //[Benchmark] public void Shell() => Sorter<Pixel32bitUnion>.ShellSort(PrepareInput(), comparer);

    #endregion
}

public class SpanBenchmark
{
    #region Initiation

    /*
     
    Begin:
    | Method      | Mean           | Error       | StdDev      |
    |------------ |---------------:|------------:|------------:|
    | PixelSpan   |       367.5 ns |     0.92 ns |     0.86 ns |
    | PixelSpan2D | 2,172,930.9 ns | 9,920.29 ns | 9,279.45 ns |

    Holy overhead ! :o


    FastEstimateItemCount:
    | Method      | Mean       | Error    | StdDev   |
    |------------ |-----------:|---------:|---------:|
    | PixelSpan   |   367.3 ns |  6.45 ns |  5.72 ns |
    | PixelSpan2D | 3,642.1 ns | 49.75 ns | 46.53 ns |
    | Method      | Mean       | Error    | StdDev   |

    8915347 Bug fixes later:
    |------------ |-----------:|---------:|---------:|
    | PixelSpan   |   372.6 ns |  5.22 ns |  4.88 ns |
    | PixelSpan2D | 4,742.8 ns | 38.04 ns | 35.58 ns |

     */

    int Width = 1920, Height = 1080;
    Pixel32bitUnion[] Data;

    public SpanBenchmark()
    {
        Data = new Pixel32bitUnion[Width * Height];
    }

    //[Benchmark]
    //public void PixelSpan()
    //{
    //    for (int i = 0; i < Height; i++)
    //    {
    //        int lo = i * Width;
    //        new Sorter<Pixel32bitUnion>.PixelSpan(Data, 1, lo, lo + Width);
    //    }
    //}

    //[Benchmark]
    //public void PixelSpan2D()
    //{
    //    for (int i = 0; i < Height; i++)
    //    {
    //        new Sorter<Pixel32bitUnion>.PixelSpan2D(Data, Width, Height, 1, 0, 0, i);
    //    }
    //}

    #endregion


    #region Insertion sort runtime
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
    //[Params(10000, 1000, 100)]
    //public int Size { get; set; }

    //[Params(100, 10, 1, 3)]
    //public int Step { get; set; }

    //private PixelComparer.Ascending.Red._32bit comparer = new();
    //List<TestInstance<Pixel32bit>> data = new();


    //[Benchmark]
    //public void PixelSpan()
    //{
    //    data = Generator.GenerateTestingData<Pixel32bit>([new TestDataSize(Size, Step, 0, Size)], comparer, 420).ToList();
    //    var instance = data.First();
    //    Sorter<Pixel32bit>.InsertionSort(new Sorter<int>.PixelSpan(instance.Unsorted, instance.Properties.Step, instance.Properties.From, instance.Properties.To), comparer);
    //}

    //[Benchmark]
    //public void Span()
    //{
    //    data = Generator.GenerateTestingData<Pixel32bit>([new TestDataSize(Size, Step, 0, Size)], comparer, 420).ToList();
    //    var instance = data.First();
    //    Sorter<Pixel32bit>.InsertionSort(new Span<int>(instance.Unsorted), comparer, instance.Properties.Step, instance.Properties.From, instance.Properties.To);
    //}
    #endregion
}

public class ComparingBenchmark
{
    [ParamsSource(nameof(comparers))]
    public IComparer<Pixel32bit>? comparer;
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
        if (comparer is null) return;

        var tests = Generator.GenerateTestingData<Pixel32bit>([new TestDataSize { Size = 10000, From = 0, Step = 1, To = 10000 }], comparer, 420).ToList();
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

#pragma warning restore CA1416 // Validate platform compatibility
