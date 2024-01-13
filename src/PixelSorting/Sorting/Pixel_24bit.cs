using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
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

    /*

    | Method                         | Mean      | Error     | StdDev    |
    |------------------------------- |----------:|----------:|----------:|
    | Pixel_24bit                    |  9.330 us | 0.1713 us | 0.1603 us |
    | InsertionSort_24bitAsInt1      | 42.401 us | 0.8365 us | 0.9959 us |
    | InsertionSort_24bitAsInt2      |  7.909 us | 0.0217 us | 0.0170 us |
    | InsertionSort_24bitAsInt_Anded |  7.970 us | 0.0956 us | 0.0847 us |
    | InsertionSort_24bitAsUInt2     |  9.057 us | 0.0584 us | 0.0547 us |
    | InsertionSort_24bitAsUInt3     | 10.144 us | 0.0179 us | 0.0150 us |
    | InsertionSort_24bitAsUInt4     |  7.985 us | 0.1243 us | 0.1163 us |
    | InsertionSort_24bitAsUInt5     | 10.645 us | 0.2067 us | 0.3510 us |

    => For Number based comparers, choose either UInt method 4, or Int method 2
    => Inlining the comparisons does not imrove performance, even when caching is possible through it (see `InsertionSort_24bitAsInt_Anded`)

     */

    #endregion

    #region Depricated pixel formats
    public struct _24bit(byte r, byte g, byte b)
    {
        public override string ToString()
        {
            return $"{{{R}, {G}, {B}}}";
        }

        public byte R = r, G = g, B = b;

        public static Pixel_24bit ToPixel_24bit(_24bit input)
        {
            return new Pixel_24bit(input.R, input.G, input.B);
        }

        public static RawPixel_24bit ToRawPixel_24bit(_24bit input)
        {
            return new RawPixel_24bit(input.R, input.G, input.B);
        }

        public static ArrayPixel2ro_24bit ToArrayPixel2ro_24bit(_24bit input)
        {
            return new ArrayPixel2ro_24bit([input.R, input.G, input.B]);
        }

        public static ArrayPixel2_24bit ToArrayPixel2_24bit(_24bit input)
        {
            return new ArrayPixel2_24bit([input.R, input.G, input.B]);
        }

        public static FlatPixel_24bit ToFlatPixel_24bit(_24bit input)
        {
            return new FlatPixel_24bit(input.R, input.G, input.B);
        }


        //public static unsafe Memory<byte> ToByteArrayAsWrapper(_24bit input)
        //{
        //    return new Memory<byte>(&input._r, 3);
        //}

        //public static unsafe SpanPixel_24bit ToSpanPixel_24bit_24bit(_24bit input)
        //{
        //    return new SpanPixel_24bit(&input._r);
        //}
    }

    public record struct Pixel_24bit(byte R, byte G, byte B);

    public struct RawPixel_24bit(byte r, byte g, byte b)
    {
        public byte
            R = r,
            G = g,
            B = b;
    }

    public struct ArrayPixel2ro_24bit(byte[] rgb)
    {
        public readonly byte R = rgb[0];
        public readonly byte G = rgb[1];
        public readonly byte B = rgb[2];
    }

    public struct ArrayPixel2_24bit(byte[] rgb)
    {
        public byte R = rgb[0];
        public byte G = rgb[1];
        public byte B = rgb[2];
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct FlatPixel_24bit(byte r, byte g, byte b)
    {
        public byte
            R = r,
            G = g,
            B = b;
    }

    //public readonly unsafe ref struct SpanPixel_24bit
    //{
    //    private const int _length = 3;
    //    internal readonly byte* _reference;

    //    public unsafe SpanPixel_24bit(byte* reference)
    //    {
    //        _reference = reference;
    //    }

    //    public unsafe ref byte R
    //    {
    //        get
    //        {
    //            return ref _reference[0];
    //        }
    //    }

    //    public unsafe ref byte G
    //    {
    //        get
    //        {
    //            return ref _reference[1];
    //        }
    //    }

    //    public unsafe ref byte B
    //    {
    //        get
    //        {
    //            return ref _reference[2];
    //        }
    //    }
    //}
    #endregion

    #region Comparers
    /// <summary>SortOder (so): Ascending | SortType (st): Red</summary>
    public class ComparerIntPixel_soA_stR : IComparer<int>
    {
        public int Compare(int a, int b)
        {
            return (a & 0xFF) - (b & 0xFF);
        }
    }
    /// <summary>SortOder (so): Ascending | SortType (st): Green</summary>
    public class ComparerIntPixel_soA_stG : IComparer<int>
    {
        public int Compare(int a, int b)
        {
            return (a & 0xFF00) - (b & 0xFF00);
        }
    }
    /// <summary>SortOder (so): Ascending | SortType (st): Blue</summary>
    public class ComparerIntPixel_soA_stB : IComparer<int>
    {
        public int Compare(int a, int b)
        {
            return (a & 0xFF0000) - (b & 0xFF0000);
        }
    }


    // Others...

    public class Comparer24bit_soA_stR : IComparer<Pixel_24bit>
    {
        public int Compare(Pixel_24bit a, Pixel_24bit b)
        {
            return a.R.CompareTo(b.R);
        }
    }
    public class ComparerJust24bit_soA_stR : IComparer<_24bit>
    {
        public int Compare(_24bit a, _24bit b)
        {
            return a.R.CompareTo(b.R);
        }
    }
    public class ComparerRaw24bit_soA_stR : IComparer<RawPixel_24bit>
    {
        public int Compare(RawPixel_24bit a, RawPixel_24bit b)
        {
            return a.R.CompareTo(b.R);
        }
    }
    public class ComparerArrayPixel224bit_soA_stR : IComparer<ArrayPixel2_24bit>
    {
        public int Compare(ArrayPixel2_24bit a, ArrayPixel2_24bit b)
        {
            return a.R.CompareTo(b.R);
        }
    }
    public class ComparerArrayPixel2ro24bit_soA_stR : IComparer<ArrayPixel2ro_24bit>
    {
        public int Compare(ArrayPixel2ro_24bit a, ArrayPixel2ro_24bit b)
        {
            return a.R.CompareTo(b.R);
        }
    }
    public class ComparerFlatPixel24bit_soA_stR : IComparer<FlatPixel_24bit>
    {
        public int Compare(FlatPixel_24bit a, FlatPixel_24bit b)
        {
            return a.R.CompareTo(b.R);
        }
    }
    public unsafe class ComparerByReference24bit_soA_stR : IComparer<nint>
    {
        public int Compare(nint aptr, nint bptr)
        {
            var a = Unsafe.AsRef<byte>((byte*)aptr);
            var b = Unsafe.AsRef<byte>((byte*)aptr);
            return a - b;
        }
    }
    public unsafe class ComparerByByte24bit_soA : IComparer<byte>
    {
        public int Compare(byte a, byte b)
        {
            return a.CompareTo(b);
        }
    }
    public class ComparerIntPixel24bit_soA_stR1 : IComparer<int>
    {
        public int Compare(int a, int b)
        {
            return BitConverter.GetBytes(a)[0].CompareTo(BitConverter.GetBytes(b)[0]);
        }
    }
    public class ComparerIntPixel24bit_soA_stR2 : IComparer<int>
    {
        public int Compare(int a, int b)
        {
            return (a & 0xFF) - (b & 0xFF);
        }
    }
    public class ComparerIntPixel24bit_soA_stR3 : IComparer<int>
    {
        public int Compare(int a, int b)
        {
            return (a << 24) - (b << 24);
        }
    }
    public class ComparerIntPixel24bit_soA_stR4 : IComparer<int>
    {
        public int Compare(int a, int b)
        {
            return (a) - (b);
        }
    }
    public class ComparerUIntPixel24bit_soA_stR1 : IComparer<uint>
    {
        public int Compare(uint a, uint b)
        {
            return BitConverter.GetBytes(a)[0].CompareTo(BitConverter.GetBytes(b)[0]);
        }
    }
    public class ComparerUIntPixel24bit_soA_stR2 : IComparer<uint>
    {
        public int Compare(uint a, uint b)
        {
            return (a & 0xFF).CompareTo(b & 0xFF);
        }
    }
    public class ComparerUIntPixel24bit_soA_stR3 : IComparer<uint>
    {
        public int Compare(uint a, uint b)
        {
            return (int)(((a << 24) >> 8) - ((b << 24) >> 8));
        }
    }
    public class ComparerUIntPixel24bit_soA_stR4 : IComparer<uint>
    {
        public int Compare(uint a, uint b)
        {
            return (a << 24).CompareTo(b << 24);
        }
    }
    public class ComparerUIntPixel24bit_soA_stR5 : IComparer<uint>
    {
        public unsafe int Compare(uint a, uint b)
        {
            uint result = (((a << 24) >> 8) - ((b << 24) >> 8));
            return *(int*)&result;
        }
    }

    public class Comparer24bit_soA_stB : IComparer<Pixel_24bit>
    {
        public int Compare(Pixel_24bit a, Pixel_24bit b)
        {
            return a.B.CompareTo(b.B);
        }
    }
    #endregion

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
