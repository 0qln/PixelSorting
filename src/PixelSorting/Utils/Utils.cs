using System.Reflection;

namespace Utils;

public static class Utils
{
#pragma warning disable CS8500
    public static unsafe TResult ReinterpretCast<TSource, TResult>(this TSource source)
    {
        var sourceRef = __makeref(source);
        var dest = default(TResult);
        var destRef = __makeref(dest);
        *(IntPtr*)&destRef = *(IntPtr*)&sourceRef;
        return __refvalue(destRef, TResult);
    }
#pragma warning restore CS8500

    public static IEnumerable<TResult> ReinterpretCast<TSource, TResult>(this IEnumerable<TSource> source)
    {
        return source.Select(item => item.ReinterpretCast<TSource, TResult>());
    }

    public static double Lerp(int start, int end, double amount) => start + (end - start) * amount;

    public static int Round(int value, int threshhold) => value - value % threshhold;

    public static int Round(double value, int threshhold) => Round((int)value, threshhold);

    public static string GetBenchmarkCopy(int i)
    {
        var result = GetSampleImagePath($"/img_0/benchmark_copy_{i}.bmp");

        if (!File.Exists(result))
            File.Copy(GetSampleImagePath("/img_0/benchmark_copy_0.bmp"), result);

        return result;
    }

    public static string GetSampleImagePath(string relativePath) =>
        Path.GetFullPath(Path.Combine(
            Path.GetDirectoryName(Assembly.GetEntryAssembly()!.Location)!,
            Path.Combine("../../../../../../../../../SampleImages", relativePath)));
}