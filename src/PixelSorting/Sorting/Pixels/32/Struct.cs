using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

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
