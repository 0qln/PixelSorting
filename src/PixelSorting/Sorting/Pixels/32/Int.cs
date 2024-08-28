global using Pixel32bitInt = int;

using Sorting.Pixels._24;
using System.Runtime.CompilerServices;


namespace Sorting.Pixels._32;

/// <summary>
/// ARGB pixel format
/// </summary>
public static class Pixel32bitExtensions
{
    public const int AShift = 24;
    public const int RShift = 16;
    public const int GShift = 8;
    public const int BShift = 0;
    public const int AMask = unchecked(0xFF << AShift);
    public const int RMask = unchecked(0xFF << RShift);
    public const int GMask = unchecked(0xFF << GShift);
    public const int BMask = unchecked(0xFF << BShift);

    public static Pixel32bitInt FromARGB(int a, int r, int g, int b)
    {
        return unchecked(
            a << AShift |
            r << RShift |
            g << GShift |
            b << BShift
        );
    }

    public static Pixel32bitInt From24bit(Pixel24bitStruct pixel24Bit)
    {
        return (int)unchecked((uint)(
            255 << AShift |
            pixel24Bit.R << RShift |
            pixel24Bit.G << GShift |
            pixel24Bit.B << BShift
        ));
    }

    public static string ToPixelString(this Pixel32bitInt pixel)
    {
        const int pad = 4;
        var a = pixel.A().ToString().PadLeft(pad);
        var r = pixel.R().ToString().PadLeft(pad);
        var g = pixel.G().ToString().PadLeft(pad);
        var b = pixel.B().ToString().PadLeft(pad);
        return $"{{ {a},{r},{g},{b} }}";
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static byte A(this Pixel32bitInt pixel) => unchecked((byte)(pixel >> AShift));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static byte R(this Pixel32bitInt pixel) => unchecked((byte)(pixel >> RShift));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static byte G(this Pixel32bitInt pixel) => unchecked((byte)(pixel >> GShift));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static byte B(this Pixel32bitInt pixel) => unchecked((byte)(pixel >> BShift));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int UnshiftedA(this Pixel32bitInt pixel) => unchecked((pixel & AMask));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int UnshiftedR(this Pixel32bitInt pixel) => unchecked((pixel & RMask));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int UnshiftedG(this Pixel32bitInt pixel) => unchecked((pixel & GMask));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int UnshiftedB(this Pixel32bitInt pixel) => unchecked((pixel & BMask));
}