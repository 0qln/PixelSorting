using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Sorting.Pixels._32
{
    /// <summary>
    /// Trying to get the best from both worlds, speed of handling as `Int32`, and quick
    /// pixel property (e.g. r,g,b) access from using as single byte.
    /// Benchmarks show this seems to be slightly superior.
    /// </summary>
    [StructLayout(LayoutKind.Explicit)]
    public struct Pixel32bitUnion
    {
        [FieldOffset(0)] public int Int;
        [FieldOffset(0)] public byte B;
        [FieldOffset(1)] public byte G;
        [FieldOffset(2)] public byte R;
        [FieldOffset(3)] public byte A;
        
        public int GrayScale()
        {
            return (R + G + B) / 3;
        }

        /// <summary>
        /// Calculate the hue of this pixel.
        /// </summary>
        /// <returns></returns>
        public float GetHue()
        {
            if (R == G && G == B)
                return 0f;

            MinMaxRgb(out int min, out int max);

            float delta = max - min;
            float hue;

            if (R == max)
                hue = (G - B) / delta;
            else if (G == max)
                hue = (B - R) / delta + 2f;
            else
                hue = (R - G) / delta + 4f;

            hue *= 60f;
            if (hue < 0f)
                hue += 360f;

            return hue;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void MinMaxRgb(out int min, out int max)
        {
            if (R > G)
            {
                max = R;
                min = G;
            }
            else
            {
                max = G;
                min = R;
            }
            if (B > max)
            {
                max = B;
            }
            else if (B < min)
            {
                min = B;
            }
        }
    }
}
