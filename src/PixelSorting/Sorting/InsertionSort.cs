using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sorting
{
    public partial class Sorter
    {
        /// <summary></summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="span"></param>
        /// <param name="comparer"></param>
        /// <param name="step"></param>
        /// <param name="from">Inclusive</param>
        /// <param name="to">Exclusive</param>
        public static void InsertionSort<T>(Span<T> span, IComparer<T>? comparer, int step, int from, int to)
        {
            // 
            comparer ??= Comparer<T>.Default;

            for (int i = from; i < to - step; i += step)
            {
                T t = span[i + step];

                int j = i;
                while (j >= from && comparer.Compare(t, span[j]) < 0)
                {
                    span[j + step] = span[j];
                    j -= step;
                }

                span[j + step] = t;
            }
        }
    }
}
