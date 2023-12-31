using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sorting
{
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
