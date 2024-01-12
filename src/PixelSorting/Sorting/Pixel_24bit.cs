using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sorting
{
    // Benchmark results [IComparable vs. IComparer]:
    /*
    | Method      | dataIndex | Mean        | Error     | StdDev   |
    |------------ |---------- |------------:|----------:|---------:|
    | IComparable | 6         |   106.15 ns |  2.121 ns | 4.610 ns |
    | IComparer   | 6         |    79.82 ns |  1.551 ns | 1.961 ns |
    | IComparable | 7         |    23.47 ns |  0.214 ns | 0.190 ns |
    | IComparer   | 7         |    18.22 ns |  0.040 ns | 0.036 ns |
    | IComparable | 8         | 1,160.54 ns |  2.204 ns | 1.954 ns |
    | IComparer   | 8         |   776.26 ns | 11.067 ns | 9.241 ns |
    */

    public record struct Pixel_24bit(byte R, byte G, byte B);


    // (so = SortOrder)
    // (st = SortType)
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


    public record struct Pixel_24bit_Comparerable_soA_stR(byte R, byte G, byte B) : IComparable<Pixel_24bit_Comparerable_soA_stR>
    {
        public int CompareTo(Pixel_24bit_Comparerable_soA_stR other)
        {
            return R.CompareTo(other.R);
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
}
