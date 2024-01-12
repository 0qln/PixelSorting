namespace Utils
{
    public static class Utils
    {
        public static unsafe TResult ReinterpretCast<TSource, TResult>(this TSource source)
        {
            var sourceRef = __makeref(source);
            var dest = default(TResult);
            var destRef = __makeref(dest);
            *(IntPtr*)&destRef = *(IntPtr*)&sourceRef;
            return __refvalue(destRef, TResult);
        }

        public static IEnumerable<TResult> ReinterpretCast<TSource, TResult>(this IEnumerable<TSource> source)
        {
            foreach (var item in source)
            {
                yield return item.ReinterpretCast<TSource, TResult>();
            }
        }
    }
}
