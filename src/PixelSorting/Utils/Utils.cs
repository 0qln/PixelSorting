namespace Utils
{
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
            foreach (var item in source)
            {
                yield return item.ReinterpretCast<TSource, TResult>();
            }
        }

        public static int Lerp(int start, int end, int amount)
        {
            return -1;
        }

        public static double Lerp(int start, int end, double amount)
        {
            return start + (end - start) * amount;
        }

        public static int Round(int value, int threshhold)
        {
            return value - value % threshhold;
        }

        public static int Round(double value, int threshhold)
        {
            return Round((int)value, threshhold);
        }
    }
}
