using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sorting
{
    #region Benchmark results

    /* 
    | Method                   | dataIndex | Mean        | Error    | StdDev   |
    |------------------------- |---------- |------------:|---------:|---------:|
    | IComparable              | 7         |    21.06 ns | 0.174 ns | 0.163 ns |
    | IComparer                | 7         |    18.22 ns | 0.047 ns | 0.044 ns |
    | ComparisonLambda         | 7         |    18.15 ns | 0.042 ns | 0.038 ns |
    | ComparisonInstacneMethod | 7         |    26.74 ns | 0.323 ns | 0.270 ns |
    | ComparisonStaticMethod   | 7         |    39.83 ns | 0.040 ns | 0.036 ns |
    | IComparable              | 8         | 1,058.71 ns | 2.902 ns | 2.423 ns |
    | IComparer                | 8         |   765.31 ns | 1.124 ns | 0.939 ns |
    | ComparisonLambda         | 8         | 1,145.77 ns | 1.432 ns | 1.340 ns |
    | ComparisonInstacneMethod | 8         |   973.12 ns | 1.268 ns | 1.124 ns |
    | ComparisonStaticMethod   | 8         | 2,377.13 ns | 4.204 ns | 3.726 ns |

    | Method                   | dataIndex | Mean       | Error    | StdDev   |
    |------------------------- |---------- |-----------:|---------:|---------:|
    | IComparable              | 8         |   975.7 ns |  8.21 ns |  7.28 ns |
    | IComparer                | 8         |   767.2 ns |  3.73 ns |  3.31 ns |
    | ComparisonLambda         | 8         | 1,159.9 ns | 10.85 ns |  9.06 ns |
    | ComparisonInstacneMethod | 8         | 1,008.5 ns | 20.02 ns | 33.46 ns |
    | ComparisonStaticMethod   | 8         | 2,457.9 ns | 48.35 ns | 69.34 ns |
    | InlineComparisonGeneric  | 8         |   874.2 ns |  9.21 ns |  8.61 ns |
    | InlineComparison         | 8         |   882.4 ns | 16.72 ns | 16.42 ns |

    => IComparer's the way to go
    
    */

    #endregion



    public record struct Pixel_24bit(byte R, byte G, byte B);


    /// <summary>
    /// SortOder (so): Ascending
    /// SortType (st): Red
    /// </summary>
    public class Comparer24bit_soA_stR : IComparer<Pixel_24bit>
    {
        public int Compare(Pixel_24bit a, Pixel_24bit b)
        {
            return a.R.CompareTo(b.R);
        }
    }

    public class Comparer24bit_soA_stB : IComparer<Pixel_24bit>
    {
        public int Compare(Pixel_24bit a, Pixel_24bit b)
        {
            return a.B.CompareTo(b.B);
        }
    }

    #region Experimental

    public record struct Pixel_24bit_Comparerable_soA_stR(byte R, byte G, byte B) : IComparable<Pixel_24bit_Comparerable_soA_stR>
    {
        public int CompareTo(Pixel_24bit_Comparerable_soA_stR other)
        {
            return R.CompareTo(other.R);
        }
    }

    public class StaticComparer24bit_soA_stR
    {
        public static int StaticCompare(Pixel_24bit a, Pixel_24bit b)
        {
            return a.R.CompareTo(b.R);
        }
    }

    public struct SimpleComparablePixel(params byte[] data) : IComparable
    {
        public byte[] Data = data;

        public int CompareTo(object? obj)
        {
            return Data[0].CompareTo(((SimpleComparablePixel)obj!).Data[0]);
        }
    }

    public struct ComparablePixel(params byte[] data) : IComparable
    {
        public SortDirection SortDirection = SortDirection.Horizontal;
        public SortOrder SortOrder = SortOrder.Ascending;
        public SortType SortType = SortType.Red;

        public byte[] Data = data;
        public byte R => Data[0];


        public int CompareTo(object? obj)
        {
            ArgumentNullException.ThrowIfNull(obj);

            int ret = (int)SortDirection;
            ret *= SortType switch
            {
                SortType.Red => R.CompareTo(((ComparablePixel)obj).R),
                _ => throw new NotSupportedException(),
            };
            return ret;
        }
    }

    #endregion
}
