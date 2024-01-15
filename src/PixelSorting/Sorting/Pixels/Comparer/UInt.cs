using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sorting.Pixels.Comparer
{
    public class ComparerUIntPixel24bit_soA_stR1 : IComparer<uint>
    {
        public int Compare(uint a, uint b)
        {
            return BitConverter.GetBytes(a)[0].CompareTo(BitConverter.GetBytes(b)[0]);
        }
    }
    public class ComparerUIntPixel24bit_soA_stR2 : IComparer<uint>
    {
        public int Compare(uint a, uint b)
        {
            return (a & 0xFF).CompareTo(b & 0xFF);
        }
    }
    public class ComparerUIntPixel24bit_soA_stR3 : IComparer<uint>
    {
        public int Compare(uint a, uint b)
        {
            return (int)(((a << 24) >> 8) - ((b << 24) >> 8));
        }
    }
    public class ComparerUIntPixel24bit_soA_stR4 : IComparer<uint>
    {
        public int Compare(uint a, uint b)
        {
            return (a << 24).CompareTo(b << 24);
        }
    }
    public class ComparerUIntPixel24bit_soA_stR5 : IComparer<uint>
    {
        public unsafe int Compare(uint a, uint b)
        {
            uint result = (((a << 24) >> 8) - ((b << 24) >> 8));
            return *(int*)&result;
        }
    }
}
