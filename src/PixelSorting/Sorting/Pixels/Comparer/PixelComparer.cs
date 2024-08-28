using Sorting.Pixels._24;
using Sorting.Pixels._32;
using Sorting.Pixels._8;

namespace Sorting.Pixels.Comparer;

/// <summary> A collection of hardcoded pixel comparers. </summary>
public abstract class PixelComparer
{
    /// <summary> In ascending sort order. </summary>
    public abstract class Ascending
    {
        /// <summary> Sort according to the alpha value. </summary>
        public class Alpha : 
            IComparer<Pixel32bitInt>, 
            IComparer<Pixel32bitUnion>
        {
            /// <summary> 32 bit pixel format. </summary>
            public int Compare(Pixel32bitInt a, Pixel32bitInt b) => a.UnshiftedA() - b.UnshiftedA();

            /// <summary> 32 bit pixel format. </summary>
            public int Compare(Pixel32bitUnion a, Pixel32bitUnion b) => a.A - b.A;
        }

        /// <summary> Sort according to the red value. </summary>
        public readonly struct RedStruct : IComparer<Pixel32bitUnion>
        {
            /// <summary> 32 bit pixel format. </summary>
            public int Compare(Pixel32bitUnion a, Pixel32bitUnion b) => a.R - b.R;
        }

        /// <summary> Sort according to the red value. </summary>
        public class Red :
            IComparer<Pixel32bitInt>,
            IComparer<Pixel32bitUnion>,
            IComparer<Pixel32bitExplicitStruct>,
            IComparer<Pixel32bitStruct>,
            IComparer<Pixel24bitStruct>,
            IComparer<Pixel24bitExplicitStruct>,
            IComparer<Pixel24bitRecord>
        {
            /// <summary> 32 bit pixel format. </summary>
            public int Compare(Pixel32bitInt a, Pixel32bitInt b) => a.UnshiftedR() - b.UnshiftedR();

            /// <summary> 32 bit pixel format. </summary>
            public int Compare(Pixel32bitUnion a, Pixel32bitUnion b) => a.R - b.R;

            /// <summary> 32 bit pixel format. </summary>
            public int Compare(Pixel32bitExplicitStruct a, Pixel32bitExplicitStruct b) => a.R - b.R;

            /// <summary> 32 bit pixel format. </summary>
            public int Compare(Pixel32bitStruct a, Pixel32bitStruct b) => a.R - b.R;

            /// <summary> 24 bit pixel format. </summary>
            public int Compare(Pixel24bitStruct a, Pixel24bitStruct b) => a.R - b.R;

            /// <summary> 24 bit pixel format. </summary>
            public int Compare(Pixel24bitExplicitStruct a, Pixel24bitExplicitStruct b) => a.R - b.R;

            /// <summary> 24 bit pixel format. </summary>
            public int Compare(Pixel24bitRecord a, Pixel24bitRecord b) => a.R - b.R;
        }

        /// <summary> Sort according to the green value. </summary>
        public class Green : 
            IComparer<Pixel32bitInt>, 
            IComparer<Pixel32bitUnion>
        {
            /// <summary> 32 bit pixel format. </summary>
            public int Compare(Pixel32bitInt a, Pixel32bitInt b) => a.UnshiftedG() - b.UnshiftedG();

            /// <summary> 32 bit pixel format. </summary>
            public int Compare(Pixel32bitUnion a, Pixel32bitUnion b) => a.G - b.G;
        }

        /// <summary> Sort according to the blue value. </summary>
        public class Blue : 
            IComparer<Pixel32bitInt>,
            IComparer<Pixel32bitUnion>,
            IComparer<Pixel24bitExplicitStruct>
        {
            /// <summary> 32 bit pixel format. </summary>
            public int Compare(Pixel32bitInt a, Pixel32bitInt b) => a.UnshiftedB() - b.UnshiftedB();

            /// <summary> 32 bit pixel format. </summary>
            public int Compare(Pixel32bitUnion a, Pixel32bitUnion b) => a.B - b.B;

            /// <summary> 24 bit pixel format. </summary>
            public int Compare(Pixel24bitExplicitStruct a, Pixel24bitExplicitStruct b) => a.B - b.B;
        }

        /// <summary> Sort according to the hue value. </summary>
        public class Hue : 
            IComparer<Pixel32bitUnion>, 
            IComparer<Pixel24bitExplicitStruct>
        {
            /// <summary> 32 bit pixel format. </summary>
            public int Compare(Pixel32bitUnion a, Pixel32bitUnion b)
            {
                // `result` is a valid comparison in floating point notation.
                var result = a.GetHue() - b.GetHue();
                // In order to cast to int, without data loss, scale and round the float.
                return (int)Math.Round(result * 1000, MidpointRounding.AwayFromZero);
            }

            /// <summary> 24 bit pixel format. </summary>
            public int Compare(Pixel24bitExplicitStruct a, Pixel24bitExplicitStruct b)
            {
                var result = a.GetHue() - b.GetHue();
                return (int)Math.Round(result * 1000, MidpointRounding.AwayFromZero);
            }
        }

        /// <summary> Sort according to the avg brightness value. </summary>
        public class GrayScale : 
            IComparer<Pixel32bitUnion>, 
            IComparer<Pixel24bitExplicitStruct>, 
            IComparer<Pixel8bit>
        {
            /// <summary> 32 bit pixel format. </summary>
            public int Compare(Pixel32bitUnion a, Pixel32bitUnion b)
            {
                // return a.GrayScale() - b.GrayScale();
                // We can safe the divisions by inlining the grayscale function.
                return (a.R + a.G + a.B) - (b.R + b.G + b.B);
            }

            /// <summary> 24 bit pixel format. </summary>
            public int Compare(Pixel24bitExplicitStruct a, Pixel24bitExplicitStruct b)
            {
                return (a.R + a.G + a.B) - (b.R + b.G + b.B);
            }

            /// <summary> 8 bit pixel format. </summary>
            public int Compare(Pixel8bit a, Pixel8bit b)
            {
                return a.Value - b.Value;
            }
        }
    }

    /// <summary> In descending sort order. </summary>
    public abstract class Descending
    {
        /// <summary> Sort according to the red value. </summary>
        public class Red : 
            IComparer<Pixel32bitInt>, 
            IComparer<Pixel24bitStruct>, 
            IComparer<Pixel24bitExplicitStruct>, 
            IComparer<Pixel32bitUnion>
        {
            /// <summary> 32 bit pixel format. </summary>
            public int Compare(Pixel32bitInt a, Pixel32bitInt b) => b.UnshiftedR() - a.UnshiftedR();

            /// <summary> 24 bit pixel format. </summary>
            public int Compare(Pixel24bitStruct a, Pixel24bitStruct b) => b.R - a.R;

            /// <summary> 24 bit pixel format. </summary>
            public int Compare(Pixel24bitExplicitStruct a, Pixel24bitExplicitStruct b) => b.R - a.R;

            /// <summary> 24 bit pixel format. </summary>
            public int Compare(Pixel32bitUnion a, Pixel32bitUnion b) => b.R - a.R;
        }
    }
}