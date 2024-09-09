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
            IPixelComparer<Pixel32bitInt>, 
            IPixelComparer<Pixel32bitUnion>
        {
            /// <summary> 32 bit pixel format. </summary>
            public int Compare(Pixel32bitInt a, Pixel32bitInt b) => a.UnshiftedA() - b.UnshiftedA();

            /// <summary> 32 bit pixel format. </summary>
            public int Compare(Pixel32bitUnion a, Pixel32bitUnion b) => a.A - b.A;

            public object Clone() => new Alpha();
        }

        /// <summary> Sort according to the red value. </summary>
        public class Red :
            IPixelComparer<Pixel32bitInt>,
            IPixelComparer<Pixel32bitUnion>,
            IPixelComparer<Pixel32bitExplicitStruct>,
            IPixelComparer<Pixel32bitStruct>,
            IPixelComparer<Pixel24bitStruct>,
            IPixelComparer<Pixel24bitExplicitStruct>,
            IPixelComparer<Pixel24bitRecord>
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

            public object Clone() => new Red();
        }

        /// <summary> Sort according to the green value. </summary>
        public class Green : 
            IPixelComparer<Pixel32bitInt>, 
            IPixelComparer<Pixel32bitUnion>
        {
            /// <summary> 32 bit pixel format. </summary>
            public int Compare(Pixel32bitInt a, Pixel32bitInt b) => a.UnshiftedG() - b.UnshiftedG();

            /// <summary> 32 bit pixel format. </summary>
            public int Compare(Pixel32bitUnion a, Pixel32bitUnion b) => a.G - b.G;

            public object Clone() => new Green();
        }

        /// <summary> Sort according to the blue value. </summary>
        public class Blue : 
            IPixelComparer<Pixel32bitInt>,
            IPixelComparer<Pixel32bitUnion>,
            IPixelComparer<Pixel24bitExplicitStruct>
        {
            /// <summary> 32 bit pixel format. </summary>
            public int Compare(Pixel32bitInt a, Pixel32bitInt b) => a.UnshiftedB() - b.UnshiftedB();

            /// <summary> 32 bit pixel format. </summary>
            public int Compare(Pixel32bitUnion a, Pixel32bitUnion b) => a.B - b.B;

            /// <summary> 24 bit pixel format. </summary>
            public int Compare(Pixel24bitExplicitStruct a, Pixel24bitExplicitStruct b) => a.B - b.B;

            public object Clone() => new Blue();
        }

        /// <summary> Sort according to the hue value. </summary>
        public class Hue : 
            IPixelComparer<Pixel32bitUnion>, 
            IPixelComparer<Pixel24bitExplicitStruct>
        {
            /// <summary> 32 bit pixel format. </summary>
            public int Compare(Pixel32bitUnion a, Pixel32bitUnion b)
            {
                // a valid comparison in floating point notation.
                // return (int)Math.Round(a.Hue) - (int)Math.Round(b.Hue);
                return a.Hue.CompareTo(b.Hue);

                // In order to cast to int, without data loss, scale and round the float.
                // return (int)Math.Round(result * 1000, MidpointRounding.AwayFromZero);
            }

            /// <summary> 24 bit pixel format. </summary>
            public int Compare(Pixel24bitExplicitStruct a, Pixel24bitExplicitStruct b)
            {
                var result = a.GetHue() - b.GetHue();
                return (int)Math.Round(result * 1000, MidpointRounding.AwayFromZero);
            }

            public object Clone() => new Hue();
        }

        /// <summary> Sort according to the avg brightness value. </summary>
        public class GrayScale : 
            IPixelComparer<Pixel32bitUnion>, 
            IPixelComparer<Pixel24bitExplicitStruct>, 
            IPixelComparer<Pixel8bit>
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

            public object Clone() => new GrayScale();
        }
    }

    /// <summary> In descending sort order. </summary>
    public abstract class Descending
    {
        /// <summary> Sort according to the red value. </summary>
        public class Red : 
            IPixelComparer<Pixel32bitInt>, 
            IPixelComparer<Pixel24bitStruct>, 
            IPixelComparer<Pixel24bitExplicitStruct>, 
            IPixelComparer<Pixel32bitUnion>
        {
            /// <summary> 32 bit pixel format. </summary>
            public int Compare(Pixel32bitInt a, Pixel32bitInt b) => b.UnshiftedR() - a.UnshiftedR();

            /// <summary> 24 bit pixel format. </summary>
            public int Compare(Pixel24bitStruct a, Pixel24bitStruct b) => b.R - a.R;

            /// <summary> 24 bit pixel format. </summary>
            public int Compare(Pixel24bitExplicitStruct a, Pixel24bitExplicitStruct b) => b.R - a.R;

            /// <summary> 24 bit pixel format. </summary>
            public int Compare(Pixel32bitUnion a, Pixel32bitUnion b) => b.R - a.R;

            public object Clone() => new Red();
        }
    }
}