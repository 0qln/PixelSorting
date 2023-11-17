using BenchmarkDotNet.Running;
using SorterTesting;
using icecream;


public class Program
{


    static void Main(string[] args)
    {
        //new InsertionSort().Sort(TestingData.Get(100), TestingData.GetComparer());

        
        //TestingData.IsValidSorter(new InsertionSort()).ic();
        TestingData.IsValidSorter(new TimSort()).ic();

        //BenchmarkRunner.Run<SorterBenchmark>();

    }



}