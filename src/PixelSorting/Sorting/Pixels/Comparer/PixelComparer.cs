﻿using Sorting.Pixels._24;
using Sorting.Pixels._32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sorting.Pixels.Comparer
{
    /// <summary> A collection of differenct hardcoded pixel comparers. </summary>
    public abstract partial class PixelComparer
    {
        /// <summary> In ascending sort order. </summary>
        public abstract partial class Ascending
        {
            /// <summary> Sort according to the alpha value. </summary>
            public abstract partial class Alpha
            {
                /// <summary> 32 bit pixel format. </summary>
                public class _32bit : IComparer<Pixel32bitInt>
                {
                    public int Compare(Pixel32bitInt a, Pixel32bitInt b) => a.UnshiftedA() - b.UnshiftedA();
                }

                /// <summary> 32 bit pixel format. </summary>
                public class _32bitUnion : IComparer<Pixel32bitUnion>
                {
                    public int Compare(Pixel32bitUnion a, Pixel32bitUnion b) => a.A - b.A;
                }
            }

            /// <summary> Sort according to the red value. </summary>
            public abstract class Red
            {
                /// <summary> 32 bit pixel format. </summary>
                public class _32bit : IComparer<Pixel32bitInt>
                {
                    public int Compare(Pixel32bitInt a, Pixel32bitInt b) => a.UnshiftedR() - b.UnshiftedR();
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
                public class _24bitExplicitStruct : IComparer<Pixel24bitExplicitStruct>
                {
                    public int Compare(Pixel24bitExplicitStruct a, Pixel24bitExplicitStruct b) => a.R - b.R;
                }

                /// <summary> 24 bit pixel format. </summary>
                public class _24bitRecord : IComparer<Pixel24bitRecord>
                {
                    public int Compare(Pixel24bitRecord a, Pixel24bitRecord b) => a.R - b.R;
                }
            }

            /// <summary> Sort according to the green value. </summary>
            public abstract class Green
            {
                /// <summary> 32 bit pixel format. </summary>
                public class _32bit : IComparer<Pixel32bitInt>
                {
                    public int Compare(Pixel32bitInt a, Pixel32bitInt b) => a.UnshiftedG() - b.UnshiftedG();
                }

                /// <summary> 32 bit pixel format. </summary>
                public class _32bitUnion : IComparer<Pixel32bitUnion>
                {
                    public int Compare(Pixel32bitUnion a, Pixel32bitUnion b) => a.G - b.G;
                }
            }

            /// <summary> Sort according to the blue value. </summary>
            public abstract class Blue
            {
                /// <summary> 32 bit pixel format. </summary>
                public class _32bit : IComparer<Pixel32bitInt>
                {
                    public int Compare(Pixel32bitInt a, Pixel32bitInt b) => a.UnshiftedB() - b.UnshiftedB();
                }

                /// <summary> 32 bit pixel format. </summary>
                public class _32bitUnion : IComparer<Pixel32bitUnion>
                {
                    public int Compare(Pixel32bitUnion a, Pixel32bitUnion b) => a.B - b.B;
                }

                public class _24bitExplicitStruct : IComparer<Pixel24bitExplicitStruct>
                {
                    public int Compare(Pixel24bitExplicitStruct a, Pixel24bitExplicitStruct b) => a.B - b.B;
                }
            }

            /// <summary> Sort according to the hue value. </summary>
            public abstract class Hue
            {
                /// <summary> 32 bit pixel format. </summary>
                public class _32bitUnion : IComparer<Pixel32bitUnion>
                {
                    public int Compare(Pixel32bitUnion a, Pixel32bitUnion b) 
                    {
                        float result = a.GetHue() - b.GetHue();
                        return (int)Math.Round(result * 1000, MidpointRounding.AwayFromZero);
                    }
                }

                /// <summary> 24 bit pixel format. </summary>
                public class _24bitExplicitStruct : IComparer<Pixel24bitExplicitStruct>
                {
                    public int Compare(Pixel24bitExplicitStruct a, Pixel24bitExplicitStruct b)
                    {
                        float result = a.GetHue() - b.GetHue();
                        return (int)Math.Round(result * 1000, MidpointRounding.AwayFromZero);
                    }
                }
            }
        }

        /// <summary> In descending sort order. </summary>
        public abstract class Descending
        {
            /// <summary> Sort according to the red value. </summary>
            public abstract class Red
            {
                /// <summary> 32 bit pixel format. </summary>
                public class _32bit : IComparer<Pixel32bitInt>
                {
                    public int Compare(Pixel32bitInt a, Pixel32bitInt b) => b.UnshiftedR() - a.UnshiftedR();
                }

                /// <summary> 24 bit pixel format. </summary>
                public class _24bit : IComparer<Pixel24bitStruct>
                {
                    public int Compare(Pixel24bitStruct a, Pixel24bitStruct b) => b.R - a.R;
                }

                /// <summary> 24 bit pixel format. </summary>
                public class _32bitUnion : IComparer<Pixel32bitUnion>
                {
                    public int Compare(Pixel32bitUnion a, Pixel32bitUnion b) => b.R - a.R;
                }
            }
        }
    }
}
