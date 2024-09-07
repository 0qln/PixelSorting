using BenchmarkDotNet.Running;
using Benchmarks;

BenchmarkRunner.Run<SpanBenchmark.Runtime>();
// BenchmarkSwitcher.FromTypes([typeof(GenericPixelStructureBenchmark<,>)]).RunAllJoined();

