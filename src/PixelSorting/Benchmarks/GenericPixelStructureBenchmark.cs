using BenchmarkDotNet.Attributes;
using Sorting;
using Sorting.Pixels._24;
using Sorting.Pixels._32;
using Sorting.Pixels.Comparer;
using TestDataGenerator;

namespace Benchmarks;

public class GenericPixelStructureBenchmark() : BenchmarkBase(1)
{
    [Benchmark]
    public unsafe void Pixel32bitUnion()
    {
        var data = Data[0];
        var sorter = new Sorter<Pixel32bitUnion>((Pixel32bitUnion*)data.Scan0, data.Width, data.Height, data.Stride);
        sorter.Sort(SortDirection.Horizontal, Comparer);
    }

    [Benchmark]
    public unsafe void Pixel32bitExplicitStruct()
    {
        var data = Data[1];
        var sorter = new Sorter<Pixel32bitExplicitStruct>((Pixel32bitExplicitStruct*)data.Scan0, data.Width, data.Height, data.Stride);
        sorter.Sort(SortDirection.Horizontal, Comparer);
    }

    [Benchmark]
    public unsafe void Pixel32bitStruct()
    {
        var data = Data[2];
        var sorter = new Sorter<Pixel32bitStruct>((Pixel32bitStruct*)data.Scan0, data.Width, data.Height, data.Stride);
        sorter.Sort(SortDirection.Horizontal, Comparer);
    }

    [Benchmark]
    public unsafe void Pixel32bitInt()
    {
        var data = Data[3];
        var sorter = new Sorter<int>((int*)data.Scan0, data.Width, data.Height, data.Stride);
        sorter.Sort(SortDirection.Horizontal, Comparer);
    }
}