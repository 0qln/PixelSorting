using BenchmarkDotNet.Attributes;
using Sorting;
using Sorting.Pixels._32;
using Sorting.Pixels.Comparer;
using Sorting.Pixels.KeySelector;
using TestDataGenerator;

namespace Benchmarks;

public class SortBenchmark 
{
    public class ExceptionHandling() : BenchmarkBase(2)
    {
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

        // [Benchmark]
        // public unsafe void Unsafe()
        // {
        //     var data = Data[0];
        //     var sorter = new Sorter32Bit((Pixel32bitUnion*)data.Scan0, data.Width, data.Height, data.Stride);
        //     sorter.SortUnsafe(Math.PI / 2, Comparer);
        // }
        //
        // [Benchmark]
        // public unsafe void Safe()
        // {
        //     var data = Data[1];
        //     var sorter = new Sorter32Bit((Pixel32bitUnion*)data.Scan0, data.Width, data.Height, data.Stride);
        //     sorter.SortSafe(Math.PI / 2, Comparer);
        // }

    }


    public class Directional() : BenchmarkBase(2)
    {

        /*

        | Method            | Mean     | Error    | StdDev   |
        |------------------ |---------:|---------:|---------:|
        | ConstantDirection | 10.55 ms | 0.073 ms | 0.065 ms |
        | DynamicDirection  | 19.22 ms | 0.085 ms | 0.071 ms |

        holy overhead!

        */

        [Benchmark]
        public unsafe void ConstantDirection()
        {
            var data = Data[0];
            var sorter = new Sorter32Bit((Pixel32bitUnion*)data.Scan0, data.Width, data.Height, data.Stride);
            sorter.Sort(SortDirection.Horizontal, Comparer);
        }

        [Benchmark]
        public unsafe void DynamicDirection()
        {
            var data = Data[1];
            var sorter = new Sorter32Bit((Pixel32bitUnion*)data.Scan0, data.Width, data.Height, data.Stride);
            // sorter.SortAngleAsync(Math.PI / 2, sorter.GetAngleSorterInfo(Sorter32Bit.IntrospectiveSort, Comparer));
        }

    }


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


    // Comb sort with 'rule of 11':
    | Method        | size | Mean      | Error    | StdDev   |
    |-------------- |----- |----------:|---------:|---------:|
    | Comb          | 1280 | 106.94 us | 0.341 us | 0.285 us |
    | OptimizedComb | 1280 |  94.42 us | 0.316 us | 0.264 us |
    | Comb          | 1920 | 156.79 us | 0.485 us | 0.454 us |
    | OptimizedComb | 1920 | 145.33 us | 1.745 us | 1.632 us |
    | Comb          | 2560 | 214.06 us | 1.163 us | 0.908 us |
    | OptimizedComb | 2560 | 195.01 us | 0.669 us | 0.558 us |
    | Comb          | 3840 | 338.51 us | 1.742 us | 1.455 us |
    | OptimizedComb | 3840 | 320.40 us | 3.060 us | 2.555 us |

     */

    public IEnumerable<int> TestInstancesSource => Generator.CommonImageSizes().Select(x => x.Horizontal).Skip(4);

    [ParamsSource(nameof(TestInstancesSource))]
    public int size;

    private IKeySelector<Pixel32bitUnion> selector = new OrderedKeySelector.Descending.Red();
    private IComparer<Pixel32bitUnion> comparer = new PixelComparer.Descending.Red();

    private Random rng = new Random(857943);


    private Sorter<Pixel32bitUnion>.PixelSpan2D PrepareInput()
    {
        var test = Enumerable.Range(0, size).Select(x => new Pixel32bitUnion { Int = rng.Next() }).ToArray();
        return new Sorter<Pixel32bitUnion>.PixelSpan2D(test, size, 1, 1, 0, 0, 0);
    }

    // [Benchmark] public void Intro() => Sorter<Pixel32bitUnion>.IntrospectiveSort(PrepareInput(), comparer);
    // [Benchmark] public void Pigeon() => new Sorter<Pixel32bitUnion>.PigeonholeSorter(selector).Sort(PrepareInput());
    // [Benchmark] public void Heap() => Sorter<Pixel32bitUnion>.HeapSort(PrepareInput(), comparer);
    // [Benchmark] public void Insertion() => Sorter<Pixel32bitUnion>.InsertionSort(PrepareInput(), comparer);
    // [Benchmark] public void Comb() => Sorter<Pixel32bitUnion>.CombSort(PrepareInput(), comparer);
    // [Benchmark] public void Shell() => Sorter<Pixel32bitUnion>.ShellSort(PrepareInput(), comparer);

    #endregion
}