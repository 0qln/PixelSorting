using System;
using System.Collections;
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

        /// <summary></summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="span"></param>
        /// <param name="comparer"></param>
        /// <param name="step"></param>
        /// <param name="from">Inclusive</param>
        /// <param name="to">Exclusive</param>
        public static unsafe void InsertionSort_24bit(Span<byte> span, IComparer<byte> comparer, int comparebyte, int step, int from, int to)
        {
            from *= 3;
            step *= 3;
            to *= 3;

            for (int i = from; i < to - step; i += step)
            {
                byte t0 = span[i + step + 0];
                byte t1 = span[i + step + 1];
                byte t2 = span[i + step + 2];

                int j = i;
                while (j >= from && comparer.Compare(t0, span[j + comparebyte]) < 0)
                {
                    span[j + step + 0] = span[j + 0];
                    span[j + step + 1] = span[j + 1];
                    span[j + step + 2] = span[j + 2];
                    j -= step;
                }

                span[j + step + 0] = t0;
                span[j + step + 1] = t1;
                span[j + step + 2] = t2;
            }
        }

        /// <summary>Can handle up to 32bit color-formats</summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="span"></param>
        /// <param name="comparer"></param>
        /// <param name="step"></param>
        /// <param name="from">Inclusive</param>
        /// <param name="to">Exclusive</param>
        public static unsafe void InsertionSort(Span<int> span, IComparer<int> comparer, int step, int from, int to)
        {
            for (int i = from; i < to - step; i += step)
            {
                int t = span[i + step];

                int j = i;
                while (j >= from && comparer.Compare(t, span[j]) < 0)
                {
                    span[j + step] = span[j];
                    j -= step;
                }

                span[j + step] = t;
            }
        }

        /// <summary></summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="span"></param>
        /// <param name="comparer"></param>
        /// <param name="step"></param>
        /// <param name="from">Inclusive</param>
        /// <param name="to">Exclusive</param>
        public static unsafe void InsertionSort_24bitAsInt(Span<int> span, IComparer<int> comparer, int step, int from, int to)
        {
            for (int i = from; i < to - step; i += step)
            {
                int t = span[i + step];

                int j = i;
                while (j >= from && comparer.Compare(t, span[j]) < 0)
                {
                    span[j + step] = span[j];
                    j -= step;
                }

                span[j + step] = t;
            }
        }

        /// <summary></summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="span"></param>
        /// <param name="comparer"></param>
        /// <param name="step"></param>
        /// <param name="from">Inclusive</param>
        /// <param name="to">Exclusive</param>
        public static unsafe void InsertionSort_24bitAsInt_Anded(Span<int> span, IComparer<int> comparer, int step, int from, int to)
        {
            for (int i = from; i < to - step; i += step)
            {
                int t = span[i + step];
                int tAnded = t & 0xFF;

                int j = i;
                while (j >= from && comparer.Compare(tAnded, span[j] & 0xFF) < 0)
                {
                    span[j + step] = span[j];
                    j -= step;
                }

                span[j + step] = t;
            }
        }

        /// <summary></summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="span"></param>
        /// <param name="comparer"></param>
        /// <param name="step"></param>
        /// <param name="from">Inclusive</param>
        /// <param name="to">Exclusive</param>
        public static unsafe void InsertionSort_24bitAsUInt(Span<uint> span, IComparer<uint> comparer, int step, int from, int to)
        {
            for (int i = from; i < to - step; i += step)
            {
                uint t = span[i + step];

                int j = i;
                while (j >= from && comparer.Compare(t, span[j]) < 0)
                {
                    span[j + step] = span[j];
                    j -= step;
                }

                span[j + step] = t;
            }
        }

        /// <summary></summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="span"></param>
        /// <param name="comparer"></param>
        /// <param name="step"></param>
        /// <param name="from">Inclusive</param>
        /// <param name="to">Exclusive</param>
        public static void InsertionSort<T>(Span<T> span, Comparison<T> comparison, int step, int from, int to)
        {
            for (int i = from; i < to - step; i += step)
            {
                T t = span[i + step];

                int j = i;
                while (j >= from && comparison(t, span[j]) < 0)
                {
                    span[j + step] = span[j];
                    j -= step;
                }

                span[j + step] = t;
            }
        }

        /// <summary></summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="span"></param>
        /// <param name="comparer"></param>
        /// <param name="step"></param>
        /// <param name="from">Inclusive</param>
        /// <param name="to">Exclusive</param>
        public static unsafe void InsertionSort_soA_stR<T>(Span<T> span, int step, int from, int to)
        {
            fixed (T* begin = &span[0])
            {
                Span<Pixel_24bit> pSpan = new Span<Pixel_24bit>(begin, span.Length);

                for (int i = from; i < to - step; i += step)
                {
                    Pixel_24bit t = pSpan[i + step];

                    int j = i;
                    while (j >= from && t.R - pSpan[j].R < 0)
                    {
                        pSpan[j + step] = pSpan[j];
                        j -= step;
                    }

                    pSpan[j + step] = t;
                }
            }
        }

        /// <summary></summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="span"></param>
        /// <param name="comparer"></param>
        /// <param name="step"></param>
        /// <param name="from">Inclusive</param>
        /// <param name="to">Exclusive</param>
        public static unsafe void InsertionSort_soA_stR(Span<Pixel_24bit> span, int step, int from, int to)
        {
            for (int i = from; i < to - step; i += step)
            {
                Pixel_24bit t = span[i + step];

                int j = i;
                while (j >= from && t.R - span[j].R < 0)
                {
                    span[j + step] = span[j];
                    j -= step;
                }

                span[j + step] = t;
            }
        }

        /// <summary></summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="span"></param>
        /// <param name="comparer"></param>
        /// <param name="step"></param>
        /// <param name="from">Inclusive</param>
        /// <param name="to">Exclusive</param>
        public static void InsertionSort<T>(Span<T> span, int step, int from, int to)
            where T : IComparable<T>
        {
            for (int i = from; i < to - step; i += step)
            {
                T t = span[i + step];

                int j = i;
                while (j >= from && t.CompareTo(span[j]) < 0)
                {
                    span[j + step] = span[j];
                    j -= step;
                }

                span[j + step] = t;
            }
        }
    }
}
