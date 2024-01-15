using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Sorting.Pixels._24
{
    [StructLayout(LayoutKind.Sequential)]
    public struct Pixel24bitStruct(byte r, byte g, byte b)
    {
        public byte B = r;
        public byte G = g;
        public byte R = b;
    }
}
