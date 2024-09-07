using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sorting.Pixels
{
    public interface IPixelComparer<in T> : IComparer<T>, ICloneable
    {
    }
}
