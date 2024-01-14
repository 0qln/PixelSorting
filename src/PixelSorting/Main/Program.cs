

global using Pixel32bit = int;


using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using Imaging;
using Sorting;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using TestDataGenerator;

//BenchmarkRunner.Run<PixelStructureBenchmark>();

//const string SOURCE = "../../../../../SampleImages/sample-image (1080p Full HD).bmp";
//const string RESULT = "../../../../../SampleImages/sample-image (1080p Full HD)_32bit.bmp";

//var bmp = Imaging.Utils.GetBitmap(SOURCE);
//var data = Imaging.Utils.ExposeData(bmp);
//var sorter = new Sorter<Pixel_24bit>(data.Scan0, data.Width, data.Height, data.Stride);

//List<Pixel_24bit[]> rows = new();
//for (int row = 0; row < data.Height; row++)
//{
//    rows.Add(sorter.GetRow(row).ToArray());
//}

////var newSorter = sorter.CastToPixelFormat<Pixel32bit>((a, b) => { });

//bmp.Save(RESULT);



BenchmarkRunner.Run<SpanBenchmark>();


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

    private ComparerIntPixel24bit_soA_stR4 comparer = new();
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
    [Params(/*0, 1, 2, 3, 4, 5, 6, 7,*/ 8)]
    public int dataIndex { get; set; }

    //[Params(50, 50, 426, 240, 640, 360, 854, 480, 1280, 720)]
    public int dataSize { get; set; }

    //static List<TestInstance<Pixel_24bit>> dataInstancesComparer = Generator.GenerateTestingData<Pixel_24bit>(Generator.GetDefaultTestingDataset(), new Comparer24bit_soA_stR(), 1).ToList();
    //static List<TestInstance<Pixel_24bit_Comparerable_soA_stR>> dataInstancesComparable = dataInstancesComparer.Select(x => x.CloneAs<Pixel_24bit_Comparerable_soA_stR>()).ToList();

    //[Benchmark]
    //public void IComparable()
    //{
    //    // Select data instance
    //    var dataInstance = dataInstancesComparer[dataIndex];

    //    // Run the sorting 
    //    Sorter<Pixel_24bit>.InsertionSort(dataInstance.Unsorted, new Comparer24bit_soA_stR(), dataInstance.Properties.Step, dataInstance.Properties.From, dataInstance.Properties.To);
    //}

    //[Benchmark]
    //public void IComparer()
    //{
    //    // Select data instance
    //    var dataInstance = dataInstancesComparable[dataIndex];

    //    // Run the sorting 
    //    Sorter<Pixel_24bit>.InsertionSort<Pixel_24bit_Comparerable_soA_stR>(dataInstance.Unsorted, dataInstance.Properties.Step, dataInstance.Properties.From, dataInstance.Properties.To);
    //}

    //[Benchmark]
    //public void ComparisonLambda()
    //{
    //    // Select data instance
    //    var dataInstance = dataInstancesComparer[dataIndex];

    //    // Run the sorting 
    //    Sorter<Pixel_24bit>.InsertionSort<Pixel_24bit>(dataInstance.Unsorted, (a, b) => a.R.CompareTo(b.R), dataInstance.Properties.Step, dataInstance.Properties.From, dataInstance.Properties.To);
    //}

    //[Benchmark]
    //public void ComparisonInstacneMethod()
    //{
    //    // Select data instance
    //    var dataInstance = dataInstancesComparer[dataIndex];

    //    // Run the sorting 
    //    Sorter.InsertionSort<Pixel_24bit>(dataInstance.Unsorted, comparison: new Comparer24bit_soA_stR().Compare, dataInstance.Properties.Step, dataInstance.Properties.From, dataInstance.Properties.To);
    //}

    //[Benchmark]
    //public void ComparisonStaticMethod()
    //{
    //    // Select data instance
    //    var dataInstance = dataInstancesComparer[dataIndex];

    //    // Run the sorting 
    //    Sorter.InsertionSort<Pixel_24bit>(dataInstance.Unsorted, comparison: StaticComparer24bit_soA_stR.StaticCompare, dataInstance.Properties.Step, dataInstance.Properties.From, dataInstance.Properties.To);
    //}

    //[Benchmark] 
    //public void InlineComparisonGeneric()
    //{
    //    // Select data instance
    //    var dataInstance = dataInstancesComparer[dataIndex];

    //    // Run the sorting 
    //    Sorter.InsertionSort_soA_stR<Pixel_24bit>(dataInstance.Unsorted, dataInstance.Properties.Step, dataInstance.Properties.From, dataInstance.Properties.To);
    //}

    //[Benchmark]
    //public void InlineComparison()
    //{
    //    // Select data instance
    //    var dataInstance = dataInstancesComparer[dataIndex];

    //    // Run the sorting 
    //    Sorter.InsertionSort_soA_stR(dataInstance.Unsorted, dataInstance.Properties.Step, dataInstance.Properties.From, dataInstance.Properties.To);
    //}
}

public class PixelStructureBenchmark
{
    static List<TestInstance<_24bit>> data =
        //Generator.GenerateTestingData<_24bit>(Generator.GetDefaultTestingDataset(), new ComparerJust24bit_soA_stR(), 1).ToList();
        Generator.GenerateTestingData<_24bit>([new TestDataSize(Size:5000, Step: 1, From:0, 5000)], new ComparerJust24bit_soA_stR(), 420).ToList();
        

    static List<TestInstance<Pixel>> dataInstances1 = data.Select(x => TestInstance<Pixel>.CastFrom(x, _24bit.ToPixel_24bit)).ToList();
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
    static List<TestInstance<Pixel32bit>> dataInstances10 = data.Select(ti =>
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


    //[Benchmark]
    //public void Pixel_24bit()
    //{
    //    var instance = dataInstances1.MaxBy(x => x.Properties.Size);
    //    Sorter<Pixel_24bit>.InsertionSort(instance.Unsorted, new Comparer24bit_soA_stR(), instance.Properties.Step, instance.Properties.From, instance.Properties.To);
    //    Debug.Assert(instance.Unsorted.SequenceEqual(instance.Sorted));
    //}

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

    //[Benchmark]
    //public void InsertionSort_24bitAsInt1()
    //{
    //    var instance = dataInstances10.MaxBy(x => x.Properties.Size);
    //    Sorter.InsertionSort_24bitAsInt(instance.Unsorted, new ComparerIntPixel24bit_soA_stR1(), instance.Properties.Step, instance.Properties.From, instance.Properties.To);
    //    Debug.Assert(instance.Unsorted.SequenceEqual(instance.Sorted));
    //}

    //[Benchmark]
    //public void InsertionSort_24bitAsInt2()
    //{
    //    var instance = dataInstances10.MaxBy(x => x.Properties.Size);
    //    Sorter.InsertionSort_24bitAsInt(instance.Unsorted, new ComparerIntPixel24bit_soA_stR2(), instance.Properties.Step, instance.Properties.From, instance.Properties.To);
    //    Debug.Assert(instance.Unsorted.SequenceEqual(instance.Sorted));
    //}

    //[Benchmark]
    //public void InsertionSort_24bitAsInt_Anded()
    //{
    //    var instance = dataInstances10.MaxBy(x => x.Properties.Size);
    //    Sorter.InsertionSort_24bitAsInt_Anded(instance.Unsorted, new ComparerIntPixel24bit_soA_stR4(), instance.Properties.Step, instance.Properties.From, instance.Properties.To);
    //    Debug.Assert(instance.Unsorted.SequenceEqual(instance.Sorted));
    //}

    //[Benchmark]
    //public void InsertionSort_24bitAsUInt2()
    //{
    //    var instance = dataInstances11.MaxBy(x => x.Properties.Size);
    //    Sorter.InsertionSort_24bitAsUInt(instance.Unsorted, new ComparerUIntPixel24bit_soA_stR2(), instance.Properties.Step, instance.Properties.From, instance.Properties.To);
    //    Debug.Assert(instance.Unsorted.SequenceEqual(instance.Sorted));
    //}

    //[Benchmark]
    //public void InsertionSort_24bitAsUInt3()
    //{
    //    var instance = dataInstances11.MaxBy(x => x.Properties.Size);
    //    Sorter.InsertionSort_24bitAsUInt(instance.Unsorted, new ComparerUIntPixel24bit_soA_stR3(), instance.Properties.Step, instance.Properties.From, instance.Properties.To);
    //    Debug.Assert(instance.Unsorted.SequenceEqual(instance.Sorted));
    //}

    //[Benchmark]
    //public void InsertionSort_24bitAsUInt4()
    //{
    //    var instance = dataInstances11.MaxBy(x => x.Properties.Size);
    //    Sorter.InsertionSort_24bitAsUInt(instance.Unsorted, new ComparerUIntPixel24bit_soA_stR4(), instance.Properties.Step, instance.Properties.From, instance.Properties.To);
    //    Debug.Assert(instance.Unsorted.SequenceEqual(instance.Sorted));
    //}

    //[Benchmark]
    //public void InsertionSort_24bitAsUInt5()
    //{
    //    var instance = dataInstances11.MaxBy(x => x.Properties.Size);
    //    Sorter.InsertionSort_24bitAsUInt(instance.Unsorted, new ComparerUIntPixel24bit_soA_stR5(), instance.Properties.Step, instance.Properties.From, instance.Properties.To);
    //    Debug.Assert(instance.Unsorted.SequenceEqual(instance.Sorted));
    //}
}