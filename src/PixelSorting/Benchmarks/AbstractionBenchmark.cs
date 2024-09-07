using BenchmarkDotNet.Attributes;
using Sorting;
using Sorting.Pixels._32;
using Sorting.Pixels.Comparer;

namespace Benchmarks;

public class AbstractionBenchmark
{
    public class Comparer() : BenchmarkBase(2)
    {
        [Benchmark]
        public unsafe void ComparerStruct()
        {
            var data = Data[0];
            var comparer = new PixelComparer.Ascending.RedStruct();
            var sorter = new Sorter32Bit((Pixel32bitUnion*)data.Scan0, data.Width, data.Height, data.Stride);
            // sorter.SortAngle(Math.PI / 2, sorter.GetAngleSorterInfo<IComparer<Pixel32bitUnion>>(Sorter32Bit.IntrospectiveSort, comparer));
        }

        [Benchmark]
        public unsafe void ComparerClass()
        {
            var data = Data[1];
            var comparer = new PixelComparer.Ascending.Red();
            var sorter = new Sorter32Bit((Pixel32bitUnion*)data.Scan0, data.Width, data.Height, data.Stride);
            // sorter.SortAngle(Math.PI / 2, sorter.GetAngleSorterInfo(Sorter32Bit.IntrospectiveSort, comparer));
        }
    }
 

}