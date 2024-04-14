using System.Runtime.InteropServices;

namespace Sorting.Pixels._32
{
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
}
