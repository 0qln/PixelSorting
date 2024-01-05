using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using Sorting;
using System.Runtime.InteropServices;
using TestDataGenerator;


//BenchmarkRunner.Run<Benchmark>();

public class Benchmark
{
    [Params(0, 1, 2, 3, 4, 5, 6, 7, 8)]
    public int dataIndex { get; set; }

    [Params(50, 50, 426, 240, 640, 360, 854, 480, 1280, 720)]
    public int dataSize { get; set; }

    static List<TestInstance<Pixel_24bit>> dataInstances = Generator.GenerateTestingData<Pixel_24bit>(
        Generator.GetDefaultTestingDataset(), new Comparer24bit_soA_stB(), Marshal.SizeOf(typeof(Pixel_24bit)), 1).ToList();

    [Benchmark]
    public void T1()
    {
        var dataInstance = dataInstances.First(x => x.Unsorted.Length == dataSize);
        Sorter.InsertionSort<Pixel_24bit>(dataInstance.Unsorted, null, dataInstance.Properties.Step, dataInstance.Properties.From, dataInstance.Properties.To);
    }
}

