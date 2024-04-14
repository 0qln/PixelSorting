using System.Runtime.InteropServices;

namespace Sorting.Pixels._32
{
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
}
