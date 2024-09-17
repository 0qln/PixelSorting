using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Sorting.Pixels._24;

/// <summary>
/// </summary>
[StructLayout(LayoutKind.Explicit)]
public struct Pixel24bitExplicitStruct
{
    [FieldOffset(0)] public byte B;
    [FieldOffset(1)] public byte G;
    [FieldOffset(2)] public byte R;


    /// <summary>
    /// Calculate the hue of this pixel.
    /// </summary>
    /// <returns></returns>
    public float GetHue()
    {
        if (R == G && G == B)
            return 0f;

        MinMaxRgb(out var min, out var max);

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