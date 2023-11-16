using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SorterTesting
{
    public class Comparer24bit
    {
        public class Descending
        {
            public class Red : IComparer<Pixel_24bit>
            {
                public int Compare(Pixel_24bit a, Pixel_24bit b)
                {
                    return b.R.CompareTo(a.R);
                }
            }

            public class Blue : IComparer<Pixel_24bit>
            {
                public int Compare(Pixel_24bit a, Pixel_24bit b)
                {
                    return b.B.CompareTo(a.B);
                }
            }
        }

        public class Ascending
        {
            public class Red : IComparer<Pixel_24bit>
            {
                public int Compare(Pixel_24bit a, Pixel_24bit b)
                {
                    return a.R.CompareTo(b.R);
                }
            }

            public class Blue : IComparer<Pixel_24bit>
            {
                public int Compare(Pixel_24bit a, Pixel_24bit b)
                {
                    return a.B.CompareTo(b.B);
                }
            }

        }
    }
}
