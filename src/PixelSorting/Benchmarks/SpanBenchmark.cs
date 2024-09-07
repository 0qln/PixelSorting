using BenchmarkDotNet.Attributes;
using Sorting;
using Sorting.Pixels._32;
using Sorting.Pixels.Comparer;
using TestDataGenerator;

namespace Benchmarks;

public class SpanBenchmark
{
    public class Initiation
    {
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

        private const int Width = 1920, Height = 1080;

        private readonly Pixel32bitUnion[] _data = new Pixel32bitUnion[Width * Height];
        private readonly nint[] _indices = new nint[(int)Math.Ceiling(Math.Sqrt(Math.Pow(Width, 2) + Math.Pow(Height, 2))) + 1];


        [Benchmark]
        public void SpanInit()
        {
            for (var i = 0; i < Height; i++)
            {
                var lo = i * Width;
                _ = new Span<Pixel32bitUnion>(_data, lo, Width);
            }
        }
        
        // [Benchmark]
        // public void PixelSpanInit()
        // {
        //     for (var i = 0; i < Height; i++)
        //     {
        //         var lo = i * Width;
        //         _ = new Sorter<Pixel32bitUnion>.PixelSpan(_data, 1, lo, lo + Width);
        //     }
        // }

        [Benchmark]
        public void PixelSpan2DInit()
        {
            for (var i = 0; i < Height; i++)
            {
                _ = new Sorter<Pixel32bitUnion>.PixelSpan2D(ref _data[0], Width, Height, 1, 0, 0, i);
            }
        }

//         [Benchmark]
//         public void PixelSpan2DInitLegacy0()
//         {
//             for (var i = 0; i < Height; i++)
//             {
// #pragma warning disable CS0618 // Type or member is obsolete
//                 _ = new Sorter<Pixel32bitUnion>.PixelSpan2D(ref _data[0], _indices, Width, Height, 1, 0, 0, i, LEGACY: true);
// #pragma warning restore CS0618 // Type or member is obsolete
//             }
//         }
    }


    // TODO
    public class Runtime
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
        // [Params(10000, 1000, 100)] public int Size { get; set; }
        //
        // [Params(100, 10, 1, 3)] public int Step { get; set; }
        //
        // private readonly List<TestInstance<Pixel32bitUnion>> _data = [];
        //
        //
        // [Benchmark]
        // public void PixelSpan()
        // {
        //     throw new NotImplementedException();
        //
        //     _data = Generator
        //         .GenerateTestingData<Pixel32bitUnion>([new TestDataSize(Size, Step, 0, Size)], Comparer, 420).ToList();
        //     var instance = _data.First();
        //     Sorter<Pixel32bitUnion>.InsertionSort(
        //         new Sorter<int>.PixelSpan(instance.Unsorted, instance.Properties.Step, instance.Properties.From,
        //             instance.Properties.To), _comparer);
        // }
        //
        // [Benchmark]
        // public void Span()
        // {
        //     throw new NotImplementedException();
        //
        //     _data = Generator
        //         .GenerateTestingData<Pixel32bitUnion>([new TestDataSize(Size, Step, 0, Size)], _comparer, 420).ToList();
        //     var instance = _data.First();
        //     // Sorter<Pixel32bitUnion>.InsertionSort(new Span<int>(instance.Unsorted), _comparer, instance.Properties.Step, instance.Properties.From, instance.Properties.To);
        // }

        private const int Width = 1920, Height = 1080;

        private readonly Pixel32bitUnion[] _data = new Pixel32bitUnion[Width * Height];

        // While testing, unnecessary code was removed from the constructor of span, this the measurements are fair.
        // Both perform about equal

        [Benchmark]
        public void PixelSpan2DAccessCompute()
        {
            var span = new Sorter<Pixel32bitUnion>.PixelSpan2D(ref _data[0], Width, Height, 1, 0, 0, 0);

            for (uint i = 0; i < span.ItemCount; i++)
                span.MapIndex(i);
        } 

        [Benchmark]
        public void PixelSpan2DAccessLookup()
        {
            var span = new Sorter<Pixel32bitUnion>.PixelSpan2D(ref _data[0], Width, Height, 1, 0, 0, 0);

            for (uint i = 0; i < span.ItemCount; i++)
#pragma warning disable CS0618 // Type or member is obsolete
                span.LookupIndex(i);
#pragma warning restore CS0618 // Type or member is obsolete
        } 
    }
}