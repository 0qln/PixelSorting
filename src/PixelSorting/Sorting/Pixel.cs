


global using Pixel32bit = int;


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
    /// <summary>
    /// ARGB pixel format
    /// </summary>
    public static class Pixel32bit_Util
    {
        public const int AShift = 24;
        public const int RShift = 16;
        public const int GShift = 8;
        public const int BShift = 0;
        public const int AMask = unchecked(0xFF << AShift);
        public const int RMask = unchecked(0xFF << RShift);
        public const int GMask = unchecked(0xFF << GShift);
        public const int BMask = unchecked(0xFF << BShift);

        public static Pixel32bit FromARGB(int a, int r, int g, int b)
        {
            return
                unchecked(
                a << AShift |
                r << RShift |
                g << GShift |
                b << BShift
                );
        }

        public static Pixel32bit From24bit(Pixel24bitStruct pixel24Bit)
        {
            return (int)
                unchecked((uint)(
                255 << AShift |
                pixel24Bit.R << RShift |
                pixel24Bit.G << GShift |
                pixel24Bit.B << BShift
            ));
        }

        public static string ToPixelString(this Pixel32bit pixel)
        {
            const int PAD = 4;
            var a = pixel.A().ToString().PadLeft(PAD);
            var r = pixel.R().ToString().PadLeft(PAD);
            var g = pixel.G().ToString().PadLeft(PAD);
            var b = pixel.B().ToString().PadLeft(PAD);
            return $"{{ {a},{r},{g},{b} }}";
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte A(this Pixel32bit pixel) => unchecked((byte)(pixel >> AShift));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte R(this Pixel32bit pixel) => unchecked((byte)(pixel >> RShift));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte G(this Pixel32bit pixel) => unchecked((byte)(pixel >> GShift));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte B(this Pixel32bit pixel) => unchecked((byte)(pixel >> BShift));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int UnshiftedA(this Pixel32bit pixel) => unchecked((pixel & AMask));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int UnshiftedR(this Pixel32bit pixel) => unchecked((pixel & RMask));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int UnshiftedG(this Pixel32bit pixel) => unchecked((pixel & GMask));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int UnshiftedB(this Pixel32bit pixel) => unchecked((pixel & BMask));
    }


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
    
    
    | Method            | comparer             | Mean     | Error    | StdDev   |
    |------------------ |--------------------- |---------:|---------:|---------:|
    | IntrospectiveSort | Sorti(...)A_stR [32] | 23.90 ms | 0.181 ms | 0.151 ms |
    | IntrospectiveSort | Sorti(...)stR_1 [34] | 26.20 ms | 0.045 ms | 0.040 ms |
    | IntrospectiveSort | Sorti(...)stR_2 [34] | 23.46 ms | 0.109 ms | 0.091 ms |
    
    => Use unshifted values for comparisons.

     */

    #endregion



    public record struct Pixel24bitRecord(byte R, byte G, byte B);

    public struct Pixel24bitStruct(byte r, byte g, byte b)
    {
        public byte
            R = r,
            G = g,
            B = b;
    }

    /// <summary>
    /// Trying to get the best from both worlds, speed of handling as `Int32`, and quick
    /// pixel property (e.g. r,g,b) access from using as single byte.
    /// Benchmarks show this to be slightly superior.
    /// </summary>
    [StructLayout(LayoutKind.Explicit)]
    public struct Pixel32bitUnion
    {
        [FieldOffset(0)] public int Int;
        [FieldOffset(0)] public byte B;
        [FieldOffset(1)] public byte G;
        [FieldOffset(2)] public byte R;
        [FieldOffset(3)] public byte A;
    }

    /// <summary>
    /// </summary>
    [StructLayout(LayoutKind.Explicit)]
    public struct Pixel32bitExplicitStruct
    {
        [FieldOffset(0)] public byte B;
        [FieldOffset(1)] public byte G;
        [FieldOffset(2)] public byte R;
        [FieldOffset(3)] public byte A;
    }

    /// <summary>
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct Pixel32bitStruct
    {
        public byte B;
        public byte G;
        public byte R;
        public byte A;
    }

    #region Experimental Pixel Structures

    /// <summary>
    /// Currently unusable, due to the `ref` keyword, which is neccessary, but prevents
    /// the structure from being used as a type argument.
    /// </summary>
    public readonly unsafe ref struct Pixel24bitSpan
    {
        private readonly ref byte _reference;


        public unsafe Pixel24bitSpan(ref byte reference)
        {
            _reference = ref reference;
        }


        public unsafe ref byte R
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return ref _reference;
            }
        }

        public unsafe ref byte G
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return ref Unsafe.Add(ref _reference, 1);
            }
        }

        public unsafe ref byte B
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return ref Unsafe.Add(ref _reference, 2);
            }
        }
    }
    #endregion

    #region Comparers

    /// <summary> A collection of differenct hardcoded pixel comparers. </summary>
    public class PixelComparer
    {
        /// <summary> In ascending sort order. </summary>
        public class Ascending
        {
            /// <summary> Sort according to the alpha value. </summary>
            public class Alpha
            {
                /// <summary> 32 bit pixel format. </summary>
                public class _32bit : IComparer<Pixel32bit>
                {
                    public int Compare(Pixel32bit a, Pixel32bit b) => a.UnshiftedA() - b.UnshiftedA();
                }
            }

            public class Red
            {
                /// <summary> 32 bit pixel format. </summary>
                public class _32bit : IComparer<Pixel32bit>
                {
                    public int Compare(Pixel32bit a, Pixel32bit b) => a.UnshiftedR() - b.UnshiftedR();
                }

                /// <summary> 32 bit pixel format. </summary>
                public class _32bitUnion : IComparer<Pixel32bitUnion>
                {
                    public int Compare(Pixel32bitUnion a, Pixel32bitUnion b) => a.R - b.R;
                }

                /// <summary> 32 bit pixel format. </summary>
                public class _32bitExplicitStruct : IComparer<Pixel32bitExplicitStruct>
                { 
                    public int Compare(Pixel32bitExplicitStruct a, Pixel32bitExplicitStruct b) => a.R - b.R;
                }

                /// <summary> 32 bit pixel format. </summary>
                public class _32bitStruct : IComparer<Pixel32bitStruct>
                {
                    public int Compare(Pixel32bitStruct a, Pixel32bitStruct b) => a.R - b.R;
                }

                /// <summary> 24 bit pixel format. </summary>
                public class _24bitStruct : IComparer<Pixel24bitStruct>
                {
                    public int Compare(Pixel24bitStruct a, Pixel24bitStruct b) => a.R - b.R;
                }

                /// <summary> 24 bit pixel format. </summary>
                public class _24bitRecord : IComparer<Pixel24bitRecord>
                {
                    public int Compare(Pixel24bitRecord a, Pixel24bitRecord b) => a.R - b.R;
                }

                ///// <summary> 24 bit pixel format. </summary>
                //public class _24bitSpan : IComparer<Pixel24bitSpan>
                //{
                //    public int Compare(Pixel24bitSpan a, Pixel24bitSpan b) => a.R - b.R;
                //}
            }

            public class Green
            {
                /// <summary> 32 bit pixel format. </summary>
                public class _32bit : IComparer<Pixel32bit>
                {
                    public int Compare(Pixel32bit a, Pixel32bit b) => a.UnshiftedG() - b.UnshiftedG();
                }
            }

            public class Blue
            {
                /// <summary> 32 bit pixel format. </summary>
                public class _32bit : IComparer<Pixel32bit>
                {
                    public int Compare(Pixel32bit a, Pixel32bit b) => a.UnshiftedB() - b.UnshiftedB();
                }
            }
        }

        /// <summary> In descending sort order. </summary>
        public class Descending
        {
            public class Red
            {
                /// <summary> 32 bit pixel format. </summary>
                public class _32bit : IComparer<Pixel32bit>
                {
                    public int Compare(Pixel32bit a, Pixel32bit b) => b.UnshiftedR() - a.UnshiftedR();
                }
                /// <summary> 24 bit pixel format. </summary>
                public class _24bit : IComparer<Pixel24bitStruct>
                {
                    public int Compare(Pixel24bitStruct a, Pixel24bitStruct b) => b.R - a.R;
                }
            }
        }
    }


    #region UInt
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
    #endregion


    #endregion
}
