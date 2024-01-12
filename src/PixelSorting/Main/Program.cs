using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using Sorting;
using System.Runtime.InteropServices;
using TestDataGenerator;
using static Utils.Utils;

BenchmarkRunner.Run<Benchmark>();

public class Benchmark
{
    [Params(/*0, 1, 2, 3, 4, 5,*/ 6, 7, 8)]
    public int dataIndex { get; set; }

    //[Params(50, 50, 426, 240, 640, 360, 854, 480, 1280, 720)]
    public int dataSize { get; set; }

    static List<TestInstance<Pixel_24bit>> dataInstancesComparer = Generator.GenerateTestingData<Pixel_24bit>(Generator.GetDefaultTestingDataset(), new Comparer24bit_soA_stB(), 1).ToList();
    static List<TestInstance<Pixel_24bit_Comparerable_soA_stR>> dataInstancesComparable = dataInstancesComparer.Select(x => x.CloneAs<Pixel_24bit_Comparerable_soA_stR>()).ToList();

    [Benchmark]
    public void IComparable()
    {
        // Select data instance
        var dataInstance = dataInstancesComparer[dataIndex];

        // Run the sorting 
        Sorter.InsertionSort<Pixel_24bit>(dataInstance.Unsorted, new Comparer24bit_soA_stB(), dataInstance.Properties.Step, dataInstance.Properties.From, dataInstance.Properties.To);
    }

    [Benchmark]
    public void IComparer()
    {
        // Select data instance
        var dataInstance = dataInstancesComparable[dataIndex];

        // Run the sorting 
        Sorter.InsertionSort<Pixel_24bit_Comparerable_soA_stR>(dataInstance.Unsorted, dataInstance.Properties.Step, dataInstance.Properties.From, dataInstance.Properties.To);
    }
}

