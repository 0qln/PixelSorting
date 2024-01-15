using System;
using System.Collections.Generic;
using System.Linq;
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
    }
}
