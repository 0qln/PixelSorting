using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using Sorting;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using TestDataGenerator;
using static Utils.Utils;


BenchmarkRunner.Run<PixelStructureBenchmark>();

return;

var pixels = new Pixel_24bit[1000, 1000];
ImageGenerator.Apply(pixels, ImageGenerator.RadialCorner);
ImageGenerator.Apply(pixels, ImageGenerator.Invert);

Imaging.Utils.Safe(pixels, "radial.bmp");


public class ComparingBenchmark
{
    [Params(/*0, 1, 2, 3, 4, 5, 6, 7,*/ 8)]
    public int dataIndex { get; set; }

    //[Params(50, 50, 426, 240, 640, 360, 854, 480, 1280, 720)]
    public int dataSize { get; set; }

    static List<TestInstance<Pixel_24bit>> dataInstancesComparer = Generator.GenerateTestingData<Pixel_24bit>(Generator.GetDefaultTestingDataset(), new Comparer24bit_soA_stR(), 1).ToList();
    static List<TestInstance<Pixel_24bit_Comparerable_soA_stR>> dataInstancesComparable = dataInstancesComparer.Select(x => x.CloneAs<Pixel_24bit_Comparerable_soA_stR>()).ToList();

    [Benchmark]
    public void IComparable()
    {
        // Select data instance
        var dataInstance = dataInstancesComparer[dataIndex];

        // Run the sorting 
        Sorter.InsertionSort<Pixel_24bit>(dataInstance.Unsorted, new Comparer24bit_soA_stR(), dataInstance.Properties.Step, dataInstance.Properties.From, dataInstance.Properties.To);
    }

    [Benchmark]
    public void IComparer()
    {
        // Select data instance
        var dataInstance = dataInstancesComparable[dataIndex];

        // Run the sorting 
        Sorter.InsertionSort<Pixel_24bit_Comparerable_soA_stR>(dataInstance.Unsorted, dataInstance.Properties.Step, dataInstance.Properties.From, dataInstance.Properties.To);
    }

    [Benchmark]
    public void ComparisonLambda()
    {
        // Select data instance
        var dataInstance = dataInstancesComparer[dataIndex];

        // Run the sorting 
        Sorter.InsertionSort<Pixel_24bit>(dataInstance.Unsorted, (a, b) => a.R.CompareTo(b.R), dataInstance.Properties.Step, dataInstance.Properties.From, dataInstance.Properties.To);
    }

    [Benchmark]
    public void ComparisonInstacneMethod()
    {
        // Select data instance
        var dataInstance = dataInstancesComparer[dataIndex];

        // Run the sorting 
        Sorter.InsertionSort<Pixel_24bit>(dataInstance.Unsorted, comparison: new Comparer24bit_soA_stR().Compare, dataInstance.Properties.Step, dataInstance.Properties.From, dataInstance.Properties.To);
    }

    [Benchmark]
    public void ComparisonStaticMethod()
    {
        // Select data instance
        var dataInstance = dataInstancesComparer[dataIndex];

        // Run the sorting 
        Sorter.InsertionSort<Pixel_24bit>(dataInstance.Unsorted, comparison: StaticComparer24bit_soA_stR.StaticCompare, dataInstance.Properties.Step, dataInstance.Properties.From, dataInstance.Properties.To);
    }

    [Benchmark] 
    public void InlineComparisonGeneric()
    {
        // Select data instance
        var dataInstance = dataInstancesComparer[dataIndex];

        // Run the sorting 
        Sorter.InsertionSort_soA_stR<Pixel_24bit>(dataInstance.Unsorted, dataInstance.Properties.Step, dataInstance.Properties.From, dataInstance.Properties.To);
    }

    [Benchmark]
    public void InlineComparison()
    {
        // Select data instance
        var dataInstance = dataInstancesComparer[dataIndex];

        // Run the sorting 
        Sorter.InsertionSort_soA_stR(dataInstance.Unsorted, dataInstance.Properties.Step, dataInstance.Properties.From, dataInstance.Properties.To);
    }
}

public class PixelStructureBenchmark
{
    static List<TestInstance<_24bit>> data =
        //Generator.GenerateTestingData<_24bit>(Generator.GetDefaultTestingDataset(), new ComparerJust24bit_soA_stR(), 1).ToList();
        Generator.GenerateTestingData<_24bit>([new TestDataSize(Size:5000, Step: 1, From:0, 5000)], new ComparerJust24bit_soA_stR(), 420).ToList();
        

    static List<TestInstance<Pixel_24bit>> dataInstances1 = data.Select(x => TestInstance<Pixel_24bit>.CastFrom(x, _24bit.ToPixel_24bit)).ToList();
    static List<TestInstance<RawPixel_24bit>> dataInstances2 = data.Select(x => TestInstance<RawPixel_24bit>.CastFrom(x, _24bit.ToRawPixel_24bit)).ToList();
    static List<TestInstance<ArrayPixel2_24bit>> dataInstances5 = data.Select(x => TestInstance<ArrayPixel2_24bit>.CastFrom(x, _24bit.ToArrayPixel2_24bit)).ToList();
    static List<TestInstance<ArrayPixel2ro_24bit>> dataInstances6 = data.Select(x => TestInstance<ArrayPixel2ro_24bit>.CastFrom(x, _24bit.ToArrayPixel2ro_24bit)).ToList();
    static List<TestInstance<FlatPixel_24bit>> dataInstances7 = data.Select(x => TestInstance<FlatPixel_24bit>.CastFrom(x, _24bit.ToFlatPixel_24bit)).ToList();
    static List<TestInstance<byte>> dataInstances8 = data.Select(ti =>
    {
        TestInstance<byte> ret = new TestInstance<byte>
        {
            Properties = ti.Properties,
            Unsorted = ti.Unsorted.SelectMany<_24bit, byte>(pixel => [pixel.R, pixel.G, pixel.B]).ToArray(),
            Sorted = ti.Sorted.SelectMany<_24bit, byte>(pixel => [pixel.R, pixel.G, pixel.B]).ToArray(),
        };
        return ret;
    }).ToList();
    static List<TestInstance<byte>> dataInstances9 = data.Select(ti =>
    {
        TestInstance<byte> ret = new TestInstance<byte>
        {
            Properties = ti.Properties,
            Unsorted = ti.Unsorted.SelectMany<_24bit, byte>(pixel => [pixel.R, pixel.G, pixel.B]).ToArray(),
            Sorted = ti.Sorted.SelectMany<_24bit, byte>(pixel => [pixel.R, pixel.G, pixel.B]).ToArray(),
        };
        return ret;
    }).ToList();
    static List<TestInstance<int>> dataInstances10 = data.Select(ti =>
    {
        TestInstance<int> ret = new TestInstance<int>
        {
            Properties = ti.Properties,
            Unsorted = ti.Unsorted.Select<_24bit, int>(pixel => BitConverter.ToInt32([pixel.R, pixel.G, pixel.B, 0])).ToArray(),
            Sorted = ti.Sorted.Select<_24bit, int>(pixel => BitConverter.ToInt32([pixel.R, pixel.G, pixel.B, 0])).ToArray(),
        };
        return ret;
    }).ToList();
    static List<TestInstance<uint>> dataInstances11 = data.Select(ti =>
    {
        TestInstance<uint> ret = new TestInstance<uint>
        {
            Properties = ti.Properties,
            Unsorted = ti.Unsorted.Select<_24bit, uint>(pixel => BitConverter.ToUInt32([pixel.R, pixel.G, pixel.B, 0])).ToArray(),
            Sorted = ti.Sorted.Select<_24bit, uint>(pixel => BitConverter.ToUInt32([pixel.R, pixel.G, pixel.B, 0])).ToArray(),
        };
        return ret;
    }).ToList();


    [Benchmark]
    public void Pixel_24bit()
    {
        var instance = dataInstances1.MaxBy(x => x.Properties.Size);
        Sorter.InsertionSort(instance.Unsorted, new Comparer24bit_soA_stR(), instance.Properties.Step, instance.Properties.From, instance.Properties.To);
        Debug.Assert(instance.Unsorted.SequenceEqual(instance.Sorted));
    }

    //[Benchmark]
    //public void RawPixel_24bit()
    //{
    //    var instance = dataInstances2.MaxBy(x => x.Properties.Size);
    //    Sorter.InsertionSort(instance.Unsorted, new ComparerRaw24bit_soA_stR(), instance.Properties.Step, instance.Properties.From, instance.Properties.To);
    //    Debug.Assert(instance.Unsorted.SequenceEqual(instance.Sorted));
    //}

    //[Benchmark]
    //public void ArrayPixel2_24bit()
    //{
    //    var instance = dataInstances5.MaxBy(x => x.Properties.Size);
    //    Sorter.InsertionSort(instance.Unsorted, new ComparerArrayPixel224bit_soA_stR(), instance.Properties.Step, instance.Properties.From, instance.Properties.To);
    //    Debug.Assert(instance.Unsorted.SequenceEqual(instance.Sorted));
    //}

    //[Benchmark]
    //public void ArrayPixel2ro_24bit()
    //{
    //    var instance = dataInstances6.MaxBy(x => x.Properties.Size);
    //    Sorter.InsertionSort(instance.Unsorted, new ComparerArrayPixel2ro24bit_soA_stR(), instance.Properties.Step, instance.Properties.From, instance.Properties.To);
    //    Debug.Assert(instance.Unsorted.SequenceEqual(instance.Sorted));
    //}

    //[Benchmark] 
    //public void FlatPixel_24bit()
    //{
    //    var instance = dataInstances7.MaxBy(x => x.Properties.Size);
    //    Sorter.InsertionSort(instance.Unsorted, new ComparerFlatPixel24bit_soA_stR(), instance.Properties.Step, instance.Properties.From, instance.Properties.To);
    //    Debug.Assert(instance.Unsorted.SequenceEqual(instance.Sorted));
    //}

    //[Benchmark]
    //public void Span_24bit()
    //{
    //    var instance = dataInstances8.MaxBy(x => x.Properties.Size);
    //    Sorter.InsertionSort_24bit(instance.Unsorted, new ComparerByByte24bit_soA(), 0, instance.Properties.Step, instance.Properties.From, instance.Properties.To);
    //    Debug.Assert(instance.Unsorted.SequenceEqual(instance.Sorted));
    //}

    [Benchmark]
    public void InsertionSort_24bitAsInt1()
    {
        var instance = dataInstances10.MaxBy(x => x.Properties.Size);
        Sorter.InsertionSort_24bitAsInt(instance.Unsorted, new ComparerIntPixel24bit_soA_stR1(), instance.Properties.Step, instance.Properties.From, instance.Properties.To);
        Debug.Assert(instance.Unsorted.SequenceEqual(instance.Sorted));
    }

    [Benchmark]
    public void InsertionSort_24bitAsInt2()
    {
        var instance = dataInstances10.MaxBy(x => x.Properties.Size);
        Sorter.InsertionSort_24bitAsInt(instance.Unsorted, new ComparerIntPixel24bit_soA_stR2(), instance.Properties.Step, instance.Properties.From, instance.Properties.To);
        Debug.Assert(instance.Unsorted.SequenceEqual(instance.Sorted));
    }

    [Benchmark]
    public void InsertionSort_24bitAsInt_Anded()
    {
        var instance = dataInstances10.MaxBy(x => x.Properties.Size);
        Sorter.InsertionSort_24bitAsInt_Anded(instance.Unsorted, new ComparerIntPixel24bit_soA_stR4(), instance.Properties.Step, instance.Properties.From, instance.Properties.To);
        Debug.Assert(instance.Unsorted.SequenceEqual(instance.Sorted));
    }

    [Benchmark]
    public void InsertionSort_24bitAsUInt2()
    {
        var instance = dataInstances11.MaxBy(x => x.Properties.Size);
        Sorter.InsertionSort_24bitAsUInt(instance.Unsorted, new ComparerUIntPixel24bit_soA_stR2(), instance.Properties.Step, instance.Properties.From, instance.Properties.To);
        Debug.Assert(instance.Unsorted.SequenceEqual(instance.Sorted));
    }

    [Benchmark]
    public void InsertionSort_24bitAsUInt3()
    {
        var instance = dataInstances11.MaxBy(x => x.Properties.Size);
        Sorter.InsertionSort_24bitAsUInt(instance.Unsorted, new ComparerUIntPixel24bit_soA_stR3(), instance.Properties.Step, instance.Properties.From, instance.Properties.To);
        Debug.Assert(instance.Unsorted.SequenceEqual(instance.Sorted));
    }

    [Benchmark]
    public void InsertionSort_24bitAsUInt4()
    {
        var instance = dataInstances11.MaxBy(x => x.Properties.Size);
        Sorter.InsertionSort_24bitAsUInt(instance.Unsorted, new ComparerUIntPixel24bit_soA_stR4(), instance.Properties.Step, instance.Properties.From, instance.Properties.To);
        Debug.Assert(instance.Unsorted.SequenceEqual(instance.Sorted));
    }

    [Benchmark]
    public void InsertionSort_24bitAsUInt5()
    {
        var instance = dataInstances11.MaxBy(x => x.Properties.Size);
        Sorter.InsertionSort_24bitAsUInt(instance.Unsorted, new ComparerUIntPixel24bit_soA_stR5(), instance.Properties.Step, instance.Properties.From, instance.Properties.To);
        Debug.Assert(instance.Unsorted.SequenceEqual(instance.Sorted));
    }
}