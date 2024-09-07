using System.Numerics;

namespace Utils
{
    public static class Extensions
    {
        public static bool Equals(this decimal d, decimal other, decimal threshold)
        {
            return Math.Abs(d - other) < threshold;
        }

        public static bool Equals(this double d, double other, double threshold)
        {
            return Math.Abs(d - other) < threshold;
        }
    }
}
