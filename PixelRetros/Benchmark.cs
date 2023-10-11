using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace PixelRetros.Benchmark
{
    public static class Benchmark
    {
        private record class BenchmarkResult(long Avarage, long StandardDeviation);



        public static TimeSpan WarmupTimePerMethod { get; set; } = TimeSpan.FromSeconds(5);
        public static TimeSpan BenchmarkTimePerMethod { get; set; } = TimeSpan.FromSeconds(8);


        public static void Run<T>()
        {
            var methods = typeof(T).GetMethods().Where(x => x.GetCustomAttribute<BenchmarkAttribute>() != null);
            var results = new Dictionary<MethodInfo, List<long>>();

            foreach (var method in methods)
            {

                Console.WriteLine("\n" + method.Name);

                Stopwatch metTime;
                List<long> allElapsedTicks = new();


                Console.WriteLine("Warming up...");
                metTime = Stopwatch.StartNew();
                while (metTime.ElapsedTicks <= WarmupTimePerMethod.Ticks)
                {
                    T instance = Activator.CreateInstance<T>();
                    Stopwatch sw = Stopwatch.StartNew();
                    method.Invoke(instance, null);
                    sw.Stop();
                }
                metTime.Stop();
                Console.WriteLine("Finished warm up.");


                Console.WriteLine("Starting benchmark...");
                metTime = Stopwatch.StartNew();
                while (metTime.ElapsedTicks <= BenchmarkTimePerMethod.Ticks)
                {
                    T instance = Activator.CreateInstance<T>();

                    Stopwatch sw = Stopwatch.StartNew();
                    method.Invoke(instance, null);
                    sw.Stop();

                    allElapsedTicks.Add(sw.ElapsedTicks);
                    Console.WriteLine($"{TimeSpan.FromTicks(sw.ElapsedTicks)}");
                }
                metTime.Stop();


                results.Add(method, allElapsedTicks);
            }


            foreach (var result in results)
            {
                Console.WriteLine("\n" + result.Key.Name);
                Console.WriteLine($"Standard Deviation: {TimeSpan.FromTicks(StdDev(result.Value))}");
                Console.WriteLine($"Average:            {TimeSpan.FromTicks(Average(result.Value))}");
            }
        }

        public static long Average(List<long> values) => values.Sum() / values.Count;
        public static long StdDev(List<long> values)
        {
            var avg = values.Sum() / (values.Count - 1);
            return (long)(values.Sum(x => Math.Pow(x - avg, 2)) / avg);
        }
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class BenchmarkAttribute : Attribute
    {
    }
}
