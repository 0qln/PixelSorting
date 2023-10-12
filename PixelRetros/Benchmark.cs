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



        public static TimeSpan WarmupTime { get; set; } = TimeSpan.FromSeconds(10);
        public static TimeSpan BenchmarkTime { get; set; } = TimeSpan.FromSeconds(20);


        public static IEnumerable<MethodInfo> GetMethods<T>()
        {
            return typeof(T).GetMethods().Where(x => x.GetCustomAttribute<BenchmarkAttribute>() != null);
        }

        public static void Run<T>()
        {
            var results = new Dictionary<MethodInfo, List<long>>();
            foreach (var method in GetMethods<T>())
            {
                results.Add(method, new List<long>());
            }

            Console.WriteLine("Warming up...");
            var warmupTimer = Stopwatch.StartNew();
            while (warmupTimer.ElapsedTicks <= WarmupTime.Ticks)
            {
                foreach (var method in results.Keys)
                {
                    T instance = Activator.CreateInstance<T>();

                    Stopwatch sw = Stopwatch.StartNew();
                    method.Invoke(instance, null);
                    sw.Stop();
                }
            }
            Console.WriteLine("Finished warm up.");

            Console.WriteLine("Starting benchmark...");
            var benchmarkTimer = Stopwatch.StartNew();
            while (benchmarkTimer.ElapsedTicks <= BenchmarkTime.Ticks)
            {
                foreach (var method in results.Keys)
                {
                    T instance = Activator.CreateInstance<T>();

                    Stopwatch sw = Stopwatch.StartNew();
                    method.Invoke(instance, null);
                    sw.Stop();

                    results[method].Add(sw.ElapsedTicks);
                }
            }
            Console.WriteLine("Finished benchmark.");

            foreach (var method in results.Keys)
            {
                Console.WriteLine("\n" + method.Name);
                foreach (var time in results[method])
                {
                    Console.WriteLine($"{TimeSpan.FromTicks(time)}");
                }
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
