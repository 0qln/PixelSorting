using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sorting.Pixels
{
    /// <summary>
    /// Pixel comparer.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IPixelComparer<in T> : IComparer<T>, ICloneable
        where T : struct
    {
    }
}
