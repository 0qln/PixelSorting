

using BenchmarkDotNet.Attributes;
using icecream;
using Microsoft.Diagnostics.Tracing.Parsers.Clr;
using Perfolizer.Mathematics.SignificanceTesting;
using System.Collections.Generic;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.InteropServices;
using System.Net;
using System.Numerics;

namespace SorterTesting;


public static class Extensions
{
    // Swap impl
    // ---
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Swap(this Span<Pixel_24bit> span, int i, int j)
    {
        Pixel_24bit t = span[i];
        span[i] = span[j];
        span[j] = t;
    }

    // Comparisons
    // ---
    public static bool ContentEqualsUntable<T>(this T[] array, T[] otherArray, IComparer<T> comparer)
    {
        ReadOnlySpan<T> span = (ReadOnlySpan<T>)array;
        ReadOnlySpan<T> other = (ReadOnlySpan<T>)otherArray;

        // If the spans differ in length, they're not equal.
        if (span.Length != other.Length)
        {
            return false.ic();
        }

        // Use the comparer to compare each element.
        for (int i = 0; i < span.Length; i++)
        {
            if (comparer.Compare(span[i], other[i]) != 0)
            {
                span.ToArray().ic();
                other.ToArray().ic();
                span[i].ic();
                other[i].ic();
                return false.ic();
            }
        }

        return true.ic();
    }

    public static bool ContentEqualsUnstable<T>(this T[][] array, T[][] otherArray, IComparer<T> comparer)
    {
        var span = (ReadOnlySpan<T[]>)array;
        var other = (ReadOnlySpan<T[]>)otherArray;

        // If the spans differ in length, they're not equal.
        if (span.Length != other.Length)
        {
            return false.ic();
        }

        // Use the comparer to compare each element.
        for (int i = 0; i < span.Length; ++i)
        {
            if (!span[i].ContentEqualsUntable(other[i], comparer))
                return false.ic();
        }

        return true.ic();
    }
}

public static class TestingData
{
    private static Random _rng = new Random();


    static TestingData()
    {
    }
    

    public static Pixel_24bit[] Get(int n)
    {
        var result = new Pixel_24bit[n];
        Array.Copy(Generate(n), result, n);
        return result;
    }

    public static IComparer<Pixel_24bit> GetComparer() => new Comparer24bit.Ascending.Red();

    /// <summary>
    /// Unit test the ISorter instance. 
    /// Unstable sorting is legal.
    /// </summary>
    /// <param name="sorter"></param>
    /// <returns></returns>
    public static bool IsValidSorter(ISorter sorter)
    {
        // Create a comparer instance
        var comparer = GetComparer();

        // Get copies of the unsorted data for the sorter and the solution
        Pixel_24bit[][] solutions = GetData(), toTestResults = GetData();
        solutions.ContentEqualsUnstable(toTestResults, comparer).ic();
        Pixel_24bit[][] GetData() => new Pixel_24bit[][]
        {
            Get(0),
            Get(1),
            Get(2),
            Get(3),
            Get(5),
            Get(100),
            //Get(1000),
            //Get(1024)
        };

        // Sort Solutions using a unstable sort
        Array.ForEach(solutions, x => Array.Sort(x, comparer));

        // Sort the sorting method of `sorter`
        try
        {
            Array.ForEach(toTestResults, x => sorter.Sort(x, comparer));
        }
        catch
        {
            Console.WriteLine("`sorter` threw an exception while unit testing: ");
            throw;
        }

        // compare results
        return solutions.ContentEqualsUnstable(toTestResults, comparer).ic();
    }


#pragma warning disable SYSLIB0011 // The generated data is can be trusted
    private static Pixel_24bit[] Generate(int n)
    {
        string filePath = $"data_{n}.dat";

        // If data for this n has already been generated, return it
        if (File.Exists(filePath).ic())
        {
            // Deserialize the Pixel_24bit array from the file and return it
            using (var fs = new FileStream(filePath, FileMode.Open))
            {
                return (Pixel_24bit[])new BinaryFormatter().Deserialize(fs);
            }
        }

        // Create new Data
        var newData = new Pixel_24bit[n];
        for (int i = 0; i < n; ++i)
        {
            var buffer = new byte[3];
            _rng.NextBytes(buffer);
            newData[i] = new Pixel_24bit(buffer[0], buffer[0], buffer[0]);
        }

        // Serialize the Pixel_24bit array and save it to a file
        using (var fs = new FileStream(filePath, FileMode.Create))
        {
            new BinaryFormatter().Serialize(fs, newData);
        }

        // Return the created data
        return newData;
    }
}
#pragma warning restore SYSLIB0011

public class SorterBenchmark
{
    [Benchmark]
    public void Heap50() => Sort50(new HeapSort());
    [Benchmark]
    public void Insertion50() => Sort50(new InsertionSort());



    public void Sort50(ISorter sorter) => sorter.Sort(TestingData.Get(50), TestingData.GetComparer());
    public void Sort240pHorizontal(ISorter sorter) => sorter.Sort(TestingData.Get(426), TestingData.GetComparer()); // 240p horizontal resolution
    public void Sort240pVertical(ISorter sorter) => sorter.Sort(TestingData.Get(240), TestingData.GetComparer()); // 240p vertical resolution
    public void Sort360pHorizontal(ISorter sorter) => sorter.Sort(TestingData.Get(640), TestingData.GetComparer()); // 360p horizontal resolution
    public void Sort360pVertical(ISorter sorter) => sorter.Sort(TestingData.Get(360), TestingData.GetComparer()); // 360p vertical resolution
    public void Sort480pHorizontal(ISorter sorter) => sorter.Sort(TestingData.Get(854), TestingData.GetComparer()); // 480p horizontal resolution
    public void Sort480pVertical(ISorter sorter) => sorter.Sort(TestingData.Get(480), TestingData.GetComparer()); // 480p vertical resolution
    public void Sort720pHorizontal(ISorter sorter) => sorter.Sort(TestingData.Get(1280), TestingData.GetComparer()); // 720p horizontal resolution
    public void Sort720pVertical(ISorter sorter) => sorter.Sort(TestingData.Get(720), TestingData.GetComparer()); // 720p vertical resolution
    public void Sort1080pHorizontal(ISorter sorter) => sorter.Sort(TestingData.Get(1920), TestingData.GetComparer()); // 1080p horizontal resolution
    public void Sort1080pVertical(ISorter sorter) => sorter.Sort(TestingData.Get(1080), TestingData.GetComparer()); // 1080p vertical resolution
    public void Sort2KHorizontal(ISorter sorter) => sorter.Sort(TestingData.Get(2560), TestingData.GetComparer()); // 2K horizontal resolution
    public void Sort2KVertical(ISorter sorter) => sorter.Sort(TestingData.Get(1440), TestingData.GetComparer()); // 2K vertical resolution
    public void Sort4KHorizontal(ISorter sorter) => sorter.Sort(TestingData.Get(3840), TestingData.GetComparer()); // 4K horizontal resolution
    public void Sort4KVertical(ISorter sorter) => sorter.Sort(TestingData.Get(2160), TestingData.GetComparer()); // 4K vertical resolution
}


public interface ISorter
{
    void Sort(Span<Pixel_24bit> span, IComparer<Pixel_24bit> comparer); 
}

public class IntroSort : ISorter
{
    public void Sort(Span<Pixel_24bit> span, IComparer<Pixel_24bit> comparer)
    {

    }
}

public class TimSort : ISorter
{
    public void Sort(Span<Pixel_24bit> span, IComparer<Pixel_24bit> comparer)
    {
        TimExecutioner.Sort(span, comparer, 0, span.Length);
    }

    // https://github.com/openjdk/jdk/blob/master/src/java.base/share/classes/java/util/ComparableTimSort.java
    private class TimExecutioner
    {
        
        private const int MIN_RUN = 32;
        private const int INITIAL_TMP_STORAGE_LENGTH = 256;
        private const int MIN_GALLOP = 7;
        private int _minGallop = MIN_GALLOP;

        private Pixel_24bit[] _tmp;
        private int _tmpBase, _tmpLen;

        private int _stackSize = 0;
        private int[] _runBase, _runLen;

        private IComparer<Pixel_24bit> _comparer;


        public TimExecutioner(Span<Pixel_24bit> span, IComparer<Pixel_24bit> comparer, Pixel_24bit[]? tmp = null, int tmpBase = -1, int tmpLen = -1)
        {
            int len = span.Length;
            int tlen = (len < 2 * INITIAL_TMP_STORAGE_LENGTH) ? len >> 1 : INITIAL_TMP_STORAGE_LENGTH;

            _comparer = comparer;

            if (tmp is null || tmpLen < tlen || tmpBase + tmpLen > tmp.Length)
            {
                _tmp = new Pixel_24bit[tlen];
                _tmpBase = 0;
                _tmpLen = tlen;
            }
            else
            {
                _tmp = tmp;
                _tmpBase = tmpBase;
                _tmpLen = tmpLen;
            }

            int stackLen = (len < 120 ? 5 :
                            len < 1542 ? 10 :
                            len < 119151 ? 24 : 49);
            _runBase = new int[stackLen];
            _runLen = new int[stackLen];
        }

        public static void Sort(Span<Pixel_24bit> span, IComparer<Pixel_24bit> comparer, int lo, int hi, Pixel_24bit[]? tmp = null, int tmpBase = -1, int tmpLen = -1)
        {
            Debug.Assert(span != null && lo >= 0 && lo <= hi && hi <= span.Length);

            int nRemaining = hi - lo;
            if (nRemaining < 2)
                return;  // Arrays of size 0 and 1 are always sorted

            // If array is small, do a "mini-TimSort" with no merges
            if (nRemaining < MIN_RUN)
            {
                int initRunLen = CountRunAndMakeAscending(span, comparer, lo, hi);
                BinarySort(span, comparer, lo, hi, lo + initRunLen);
                return;
            }

            // Main search
            TimExecutioner ts = new (span, comparer, tmp, tmpBase, tmpLen);
            int minRun = MinRunLength(nRemaining);
            do
            {
                // Identify next run
                int runLen = CountRunAndMakeAscending(span, comparer, lo, hi);

                // If run is short, extend to min(minRun, nRemaining)
                if (runLen < minRun)
                {
                    int force = nRemaining <= minRun ? nRemaining : minRun;
                    BinarySort(span, comparer, lo, lo + force, lo + runLen);
                    runLen = force;
                }

                // Push run onto pending-run stack, and maybe merge
                ts.PushRun(lo, runLen);
                ts.MergeCollapse(span, comparer);

                // Advance to find next run
                lo += runLen;
                nRemaining -= runLen;
            } 
            while (nRemaining != 0);


            // Merge all remaining runs to complete sort
            Debug.Assert(lo == hi);
            ts.MergeForceCollapse(span, comparer);
            Debug.Assert(ts._stackSize == 1);
        }

        private static void BinarySort(Span<Pixel_24bit> span, IComparer<Pixel_24bit> comparer, int lo, int hi, int start)
        {
            Debug.Assert(lo <= start && start <= hi);
            if (start == lo) start++;
            for (; start < hi; start++)
            {
                Pixel_24bit pivot = span[start];

                // Set left (and right) to the index where a[start] (pivot) belongs
                int left = lo;
                int right = start;
                Debug.Assert(left <= right);
                /*
                 * Invariants:
                 *   pivot >= all in [lo, left).
                 *   pivot <  all in [right, start).
                 */
                while (left < right)
                {
                    int mid = (left + right) >> 1;
                    if (comparer.Compare(pivot, span[mid]) < 0)
                        right = mid;
                    else
                        left = mid + 1;
                }
                Debug.Assert(left == right);

                /*
                 * The invariants still hold: pivot >= all in [lo, left) and
                 * pivot < all in [left, start), so pivot belongs at left.  Note
                 * that if there are elements equal to pivot, left points to the
                 * first slot after them -- that's why this sort is stable.
                 * Slide elements over to make room for pivot.
                 */
                int n = start - left;  // The number of elements to move
                                       // Switch is just an optimization for arraycopy in default case
                switch (n)
                {
                    case 2: 
                        span[left + 2] = span[left + 1];
                        goto case 1;

                    case 1:
                        span[left + 1] = span[left];
                        break;

                    default: 
                        Array.Copy(span.ToArray(), left, span.ToArray(), left + 1, n);
                        break;
                }
                span[left] = pivot;
            }
        }

        private static int CountRunAndMakeAscending(Span<Pixel_24bit> span, IComparer<Pixel_24bit> comparer, int lo, int hi)
        {
            Debug.Assert( lo<hi);
            int runHi = lo + 1;
            if (runHi == hi)
                return 1;

            // Find end of run, and reverse range if descending
            if (comparer.Compare(span[runHi++], span[lo]) < 0)
            {   
                // Descending
                while (runHi < hi && comparer.Compare(span[runHi], span[runHi - 1]) < 0)
                    runHi++;
                ReverseRange(span, lo, runHi);
            }
            else
            {   
                // Ascending
                while (runHi < hi && comparer.Compare(span[runHi], span[runHi - 1]) >= 0)
                    runHi++;
            }

            return runHi - lo;
        }

        private static void ReverseRange(Span<Pixel_24bit> span, int lo, int hi)
        {
            hi--;
            while (lo < hi)
            {
                Pixel_24bit t = span[lo];
                span[lo++] = span[hi];
                span[hi--] = t;
            }
        }

        private static int MinRunLength(int n)
        {
            Debug.Assert(n >= 0);
            int r = 0;  // Becomes 1 if any 1 bits are shifted off
            while (n >= MIN_RUN)
            {
                r |= (n & 1);
                n >>= 1;
            }
            return n + r;
        }

        private void PushRun(int runBase, int runLen)
        {
            _runBase[_stackSize] = runBase;
            _runLen[_stackSize] = runLen;
            _stackSize++;
        }

        private void MergeCollapse(Span<Pixel_24bit> span, IComparer<Pixel_24bit> comparer)
        {
            while (_stackSize > 1)
            {
                int n = _stackSize - 2;
                if (n > 0 && _runLen[n - 1] <= _runLen[n] + _runLen[n + 1] ||
                    n > 1 && _runLen[n - 2] <= _runLen[n] + _runLen[n - 1])
                {
                    if (_runLen[n - 1] < _runLen[n + 1])
                        n--;
                }
                else if (n < 0 || _runLen[n] > _runLen[n + 1])
                {
                    break; // Invariant is established
                }
                MergeAt(span, comparer, n);
            }
        }

        private void MergeForceCollapse(Span<Pixel_24bit> span, IComparer<Pixel_24bit> comparer)
        {
            while (_stackSize > 1)
            {
                int n = _stackSize - 2;
                if (n > 0 && _runLen[n - 1] < _runLen[n + 1])
                    n--;
                MergeAt(span, comparer, n);
            }
        }

        private void MergeAt(Span<Pixel_24bit> span, IComparer<Pixel_24bit> comparer, int i)
        {
            Debug.Assert(_stackSize >= 2);
            Debug.Assert(i >= 0);
            Debug.Assert(i == _stackSize - 2 || i == _stackSize - 3);

            int base1 = _runBase[i];
            int len1 = _runLen[i];
            int base2 = _runBase[i + 1];
            int len2 = _runLen[i + 1];
            Debug.Assert(len1 > 0 && len2 > 0);
            Debug.Assert(base1 +len1 == base2);


            _runLen[i] = len1 + len2;
            if (i == _stackSize - 3)
            {
                _runBase[i + 1] = _runBase[i + 2];
                _runLen[i + 1] = _runLen[i + 2];
            }
            _stackSize--;

            /*
             * Find where the first element of run2 goes in run1. Prior elements
             * in run1 can be ignored (because they're already in place).
             */
            int k = GallopRight(span[base2], span, comparer, base1, len1, 0);
            Debug.Assert(k >= 0);
            base1 += k;
            len1 -= k;
            if (len1 == 0)
                return;

            /*
             * Find where the last element of run1 goes in run2. Subsequent elements
             * in run2 can be ignored (because they're already in place).
             */
            len2 = GallopLeft(span[base1 + len1 - 1], span, comparer, base2, len2, len2 - 1);
            Debug.Assert(len2 >= 0);
            if (len2 == 0)
                return;

            // Merge remaining runs, using tmp array with min(len1, len2) elements
            if (len1 <= len2)
                mergeLo(span, comparer, base1, len1, base2, len2);
            else
                mergeHi(span, comparer, base1, len1, base2, len2);
        }

        private static int GallopLeft(Pixel_24bit key, Span<Pixel_24bit> span, IComparer<Pixel_24bit> comparer, int first /*=base*/, int len, int hint)
        {
            Debug.Assert(len > 0 && hint >= 0 && hint < len);

            int lastOfs = 0;
            int ofs = 1;
            if (comparer.Compare(key, span[first + hint]) > 0)
            {
                // Gallop right until a[base+hint+lastOfs] < key <= a[base+hint+ofs]
                int maxOfs = len - hint;
                while (ofs < maxOfs && comparer.Compare(key, span[first + hint + ofs]) > 0)
                {
                    lastOfs = ofs;
                    ofs = (ofs << 1) + 1;
                    if (ofs <= 0)   // int overflow
                        ofs = maxOfs;
                }
                if (ofs > maxOfs)
                    ofs = maxOfs;

                // Make offsets relative to base
                lastOfs += hint;
                ofs += hint;
            }
            else
            { // key <= a[base + hint]
              // Gallop left until a[base+hint-ofs] < key <= a[base+hint-lastOfs]
                int maxOfs = hint + 1;
                while (ofs < maxOfs && comparer.Compare(key, span[first + hint - ofs]) <= 0)
                {
                    lastOfs = ofs;
                    ofs = (ofs << 1) + 1;
                    if (ofs <= 0)   // int overflow
                        ofs = maxOfs;
                }
                if (ofs > maxOfs)
                    ofs = maxOfs;

                // Make offsets relative to base
                int tmp = lastOfs;
                lastOfs = hint - ofs;
                ofs = hint - tmp;
            }
            Debug.Assert(- 1 <= lastOfs && lastOfs < ofs && ofs <= len);

            /*
             * Now a[base+lastOfs] < key <= a[base+ofs], so key belongs somewhere
             * to the right of lastOfs but no farther right than ofs.  Do a binary
             * search, with invariant a[base + lastOfs - 1] < key <= a[base + ofs].
             */
            lastOfs++;
            while (lastOfs < ofs)
            {
                int m = lastOfs + ((ofs - lastOfs) >> 1);

                if (comparer.Compare(key, span[first + m]) > 0)
                    lastOfs = m + 1;  // a[base + m] < key
                else
                    ofs = m;          // key <= a[base + m]
            }
            Debug.Assert(lastOfs == ofs);    // so a[base + ofs - 1] < key <= a[base + ofs]
            return ofs;
        }

        private static int GallopRight(Pixel_24bit key, Span<Pixel_24bit> span, IComparer<Pixel_24bit> comparer, int first /*=base*/, int len, int hint)
        {
            Debug.Assert(len > 0 && hint >= 0 && hint < len);

            int ofs = 1;
            int lastOfs = 0;
            if (comparer.Compare(key, span[first + hint]) < 0)
            {
                // Gallop left until a[b+hint - ofs] <= key < a[b+hint - lastOfs]
                int maxOfs = hint + 1;
                while (ofs < maxOfs && comparer.Compare(key, span[first + hint - ofs]) < 0)
                {
                    lastOfs = ofs;
                    ofs = (ofs << 1) + 1;
                    if (ofs <= 0)   // int overflow
                        ofs = maxOfs;
                }
                if (ofs > maxOfs)
                    ofs = maxOfs;

                // Make offsets relative to b
                int tmp = lastOfs;
                lastOfs = hint - ofs;
                ofs = hint - tmp;
            }
            else
            { // a[b + hint] <= key
              // Gallop right until a[b+hint + lastOfs] <= key < a[b+hint + ofs]
                int maxOfs = len - hint;
                while (ofs < maxOfs && comparer.Compare(key, span[first + hint + ofs]) >= 0)
                {
                    lastOfs = ofs;
                    ofs = (ofs << 1) + 1;
                    if (ofs <= 0)   // int overflow
                        ofs = maxOfs;
                }
                if (ofs > maxOfs)
                    ofs = maxOfs;

                // Make offsets relative to b
                lastOfs += hint;
                ofs += hint;
            }
            Debug.Assert(- 1 <= lastOfs && lastOfs < ofs && ofs <= len);

            /*
             * Now a[b + lastOfs] <= key < a[b + ofs], so key belongs somewhere to
             * the right of lastOfs but no farther right than ofs.  Do a binary
             * search, with invariant a[b + lastOfs - 1] <= key < a[b + ofs].
             */
            lastOfs++;
            while (lastOfs < ofs)
            {
                int m = lastOfs + ((ofs - lastOfs) >> 1);

                if (comparer.Compare(key, span[first + m]) < 0)
                    ofs = m;          // key < a[b + m]
                else
                    lastOfs = m + 1;  // a[b + m] <= key
            }
            Debug.Assert(lastOfs == ofs);    // so a[b + ofs - 1] <= key < a[b + ofs]
            return ofs;
        }

        private void mergeLo(Span<Pixel_24bit> span, IComparer<Pixel_24bit> comparer, int base1, int len1, int base2, int len2)
        {
            Debug.Assert(len1 > 0 && len2 > 0 && base1 + len1 == base2);

            // Copy first run into temp array
            Pixel_24bit[] tmp = EnsureCapacity(span, len1);

            int cursor1 = _tmpBase; // Indexes into tmp array
            int cursor2 = base2;   // Indexes int a
            int dest = base1;      // Indexes int a
            Array.Copy(span.ToArray(), base1, tmp, cursor1, len1);

            // Move first element of second run and deal with degenerate cases
            span[dest++] = span[cursor2++];
            if (--len2 == 0)
            {
                Array.Copy(tmp, cursor1, span.ToArray(), dest, len1);
                return;
            }
            if (len1 == 1)
            {
                Array.Copy(span.ToArray(), cursor2, span.ToArray(), dest, len2);
                span[dest + len2] = tmp[cursor1]; // Last elt of run 1 to end of merge
                return;
            }

            int minGallop = _minGallop;  // Use local variable for performance
            
            while (true)
            {
                int count1 = 0; // Number of times in a row that first run won
                int count2 = 0; // Number of times in a row that second run won

                /*
                 * Do the straightforward thing until (if ever) one run starts
                 * winning consistently.
                 */
                do
                {
                    Debug.Assert(len1 > 1 && len2 > 0);
                    if (comparer.Compare(span[cursor2], tmp[cursor1]) < 0)
                    {
                        span[dest++] = span[cursor2++];
                        count2++;
                        count1 = 0;
                        if (--len2 == 0)
                            goto outerEnd;
                    }
                    else
                    {
                        span[dest++] = tmp[cursor1++];
                        count1++;
                        count2 = 0;
                        if (--len1 == 1)
                            goto outerEnd;
                    }
                } 
                while ((count1 | count2) < minGallop);

                /*
                 * One run is winning so consistently that galloping may be a
                 * huge win. So try that, and continue galloping until (if ever)
                 * neither run appears to be winning consistently anymore.
                 */
                do
                {
                    Debug.Assert(len1 > 1 && len2 > 0);
                    count1 = GallopRight(span[cursor2], tmp, comparer, cursor1, len1, 0);
                    if (count1 != 0)
                    {
                        Array.Copy(tmp, cursor1, span.ToArray(), dest, count1);
                        dest += count1;
                        cursor1 += count1;
                        len1 -= count1;
                        if (len1 <= 1)  // len1 == 1 || len1 == 0
                            goto outerEnd;
                    }
                    span[dest++] = span[cursor2++];
                    if (--len2 == 0)
                        goto outerEnd;

                    count2 = GallopLeft(tmp[cursor1], span, comparer, cursor2, len2, 0);
                    if (count2 != 0)
                    {
                        Array.Copy(span.ToArray(), cursor2, span.ToArray(), dest, count2);
                        dest += count2;
                        cursor2 += count2;
                        len2 -= count2;
                        if (len2 == 0)
                            goto outerEnd;
                    }
                    span[dest++] = tmp[cursor1++];
                    if (--len1 == 1)
                        goto outerEnd;
                    minGallop--;
                } while (count1 >= MIN_GALLOP | count2 >= MIN_GALLOP);
                if (minGallop < 0)
                    minGallop = 0;
                minGallop += 2;  // Penalize for leaving gallop mode
            }

        outerEnd:

            _minGallop = minGallop < 1 ? 1 : minGallop;  // Write back to field

            if (len1 == 1)
            {
                Debug.Assert(len2 > 0);
                Array.Copy(span.ToArray(), cursor2, span.ToArray(), dest, len2);
                span[dest + len2] = tmp[cursor1]; //  Last elt of run 1 to end of merge
            }
            else if (len1 == 0)
            {
                throw new ArgumentException("Comparison method violates its general contract!");
            }
            else
            {
                Debug.Assert(len2 == 0);
                Debug.Assert(len1 > 1);
                Array.Copy(tmp, cursor1, span.ToArray(), dest, len1);
            }
        }

        private void mergeHi(Span<Pixel_24bit> span, IComparer<Pixel_24bit> comparer, int base1, int len1, int base2, int len2)
        {
            Debug.Assert(len1 > 0 && len2 > 0 && base1 + len1 == base2);

            // Copy first run into temp array
            Pixel_24bit[] tmp = EnsureCapacity(span, len1);
            int tmpBase = _tmpBase;
            Array.Copy(span.ToArray(), base2, tmp, tmpBase, len2);

            int cursor1 = base1 + len1 - 1;  // Indexes into a
            int cursor2 = tmpBase + len2 - 1; // Indexes into tmp array
            int dest = base2 + len2 - 1;     // Indexes into a

            // Move last element of first run and deal with degenerate cases
            span[dest--] = span[cursor1--];
            if (--len1 == 0)
            {
                Array.Copy(tmp, tmpBase, span.ToArray(), dest - (len2 - 1), len2);
                return;
            }
            if (len2 == 1)
            {
                dest -= len1;
                cursor1 -= len1;
                Array.Copy(span.ToArray(), cursor1 + 1, span.ToArray(), dest + 1, len1);
                span[dest] = tmp[cursor2];
                return;
            }

            int minGallop = _minGallop;  // Use local variable for performance
        
            while (true)
            {
                int count1 = 0; // Number of times in a row that first run won
                int count2 = 0; // Number of times in a row that second run won

                /*
                 * Do the straightforward thing until (if ever) one run
                 * appears to win consistently.
                 */
                do
                {
                    Debug.Assert(len1 > 0 && len2 > 1);
                    if (comparer.Compare(tmp[cursor2], span[cursor1]) < 0)
                    {
                        span[dest--] = span[cursor1--];
                        count1++;
                        count2 = 0;
                        if (--len1 == 0)
                            goto outer;
                    }
                    else
                    {
                        span[dest--] = tmp[cursor2--];
                        count2++;
                        count1 = 0;
                        if (--len2 == 1)
                            goto outer;
                    }
                } 
                while ((count1 | count2) < minGallop);

                /*
                 * One run is winning so consistently that galloping may be a
                 * huge win. So try that, and continue galloping until (if ever)
                 * neither run appears to be winning consistently anymore.
                 */
                do
                {
                    Debug.Assert(len1 > 0 && len2 > 1);
                    count1 = len1 - GallopRight(tmp[cursor2], span, comparer, base1, len1, len1 - 1);
                    if (count1 != 0)
                    {
                        dest -= count1;
                        cursor1 -= count1;
                        len1 -= count1;
                        Array.Copy(span.ToArray(), cursor1 + 1, span.ToArray(), dest + 1, count1);
                        if (len1 == 0)
                            goto outer;
                    }
                    span[dest--] = tmp[cursor2--];
                    if (--len2 == 1)
                        goto outer;

                    count2 = len2 - GallopLeft(span[cursor1], tmp, comparer, tmpBase, len2, len2 - 1);
                    if (count2 != 0)
                    {
                        dest -= count2;
                        cursor2 -= count2;
                        len2 -= count2;
                        Array.Copy(tmp, cursor2 + 1, span.ToArray(), dest + 1, count2);
                        if (len2 <= 1)
                            goto outer; // len2 == 1 || len2 == 0
                    }
                    span[dest--] = span[cursor1--];
                    if (--len1 == 0)
                        goto outer;
                    minGallop--;
                } 
                while (count1 >= MIN_GALLOP | count2 >= MIN_GALLOP);
                if (minGallop < 0)
                    minGallop = 0;
                minGallop += 2;  // Penalize for leaving gallop mode
            }
        outer:
            _minGallop = minGallop < 1 ? 1 : minGallop;  // Write back to field

            if (len2 == 1)
            {
                Debug.Assert(len1 > 0);
                dest -= len1;
                cursor1 -= len1;
                Array.Copy(span.ToArray(), cursor1 + 1, span.ToArray(), dest + 1, len1);
                span[dest] = tmp[cursor2];  // Move first elt of run2 to front of merge
            }
            else if (len2 == 0)
            {
                throw new ArgumentException("Comparison method violates its general contract!");
            }
            else
            {
                Debug.Assert(len1 == 0);
                Debug.Assert(len2 > 0);
                Array.Copy(tmp, tmpBase, span.ToArray(), dest - (len2 - 1), len2);
            }
        }

        private Pixel_24bit[] EnsureCapacity(Span<Pixel_24bit> span, int minCapacity)
        {
            if (_tmpLen < minCapacity)
            {
                // Compute smallest power of 2 > minCapacity
                int newSize = -1 >> BitOperations.LeadingZeroCount((uint)minCapacity);
                newSize++;

                if (newSize < 0) // Not bloody likely!
                    newSize = minCapacity;
                else
                    newSize = Math.Min(newSize, span.Length >> 1);

                Pixel_24bit[] newArray = new Pixel_24bit[newSize];
                _tmp = newArray;
                _tmpLen = newSize;
                _tmpBase = 0;
            }
            return _tmp;
        }
    }
}

/*
public static class PaletteSort
{

    /// <summary>
    /// don't use this :-)
    /// </summary>
    /// <param name="span"></param>
    public static void Sort(Span<Pixel_24bit> span)
    {
        HashSet<int> s = new HashSet<int>();
        for (int i = 0; i < span.Length; i++)
        {
            if (!s.Contains(span[i].Hash))
                s.Add(span[i].Hash);
        }
        UnsafeSort(span, s.Count);
    }

    /// <summary>
    /// don't use this :-)
    /// </summary>
    /// <param name="span"></param>
    /// <param name="n">Number of distinct pixels in the span</param>
    public static void UnsafeSort(Span<Pixel_24bit> span, int n)
    {
        int[] pixelCounts = new int[n];
        for (int i = 0; i < span.Length; ++i)
            ++pixelCounts[span[i].Hash % n]; // *ass* ._.

        int p = 0;
        for (int i = 0; i < n; ++i)
            for (int j = 0; j < pixelCounts[i]; ++j)
                span[p++] = new Pixel_24bit(pixelCounts[i]);
    }
}
*/
public class HeapSort : ISorter
{
    public void Sort(Span<Pixel_24bit> span, IComparer<Pixel_24bit> comparer)
    {
        int n = span.Length;
        
        // Build heap
        for (int i = n / 2; i >= 0; --i)
        {
            Heapify(span, comparer, n, i);
        }

        // Heap Sort
        for (int i = n - 1; i >= 0; --i)
        {
            span.Swap(0, i);

            // Restore Heap
            Heapify(span, comparer, i, 0);
        }
    }

    public static void Heapify(Span<Pixel_24bit> span, IComparer<Pixel_24bit> comparer, int n, int i)
    {
        int largest = i;
        int l = 2 * i + 1;
        int r = 2 * i + 1;


        if (l < n && comparer.Compare(span[l], span[largest]) > 0)
            largest = l;
        
        if (r < n && comparer.Compare(span[r], span[largest]) > 0)
            largest = r;


        if (largest != i)
        {
            span.Swap(largest, i);

            Heapify(span, comparer, n, largest);
        }
    }
}

public class InsertionSort : ISorter
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="keys"></param>
    /// <param name="comparer"></param>
    /// <param name="step"></param>
    /// <param name="left">Inclusive</param>
    /// <param name="right">Exclusive</param>
    public static void Sort(Span<Pixel_24bit> keys, IComparer<Pixel_24bit> comparer, int step, int left, int right)
    {
        for (int i = left; i < (right - step); i += step)
        {
            Pixel_24bit t = keys[i + step];

            int j = i;
            while (j >= left && comparer.Compare(t, keys[j]) < 0)
            {
                keys[j + step] = keys[j];
                j -= step;
            }

            keys[j + step] = t;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="keys"></param>
    /// <param name="comparer"></param>
    /// <param name="step"></param>
    /// <param name="left">Inclusive</param>
    /// <param name="right">Exclusive</param>
    public static void Sort(Span<Pixel_24bit> keys, IComparer<Pixel_24bit> comparer, int left, int right)
    {
        for (int i = left; i < (right - 1); i++)
        {
            Pixel_24bit t = keys[i + 1];

            int j = i;
            while (j >= left && comparer.Compare(t, keys[j]) < 0)
            {
                keys[j + 1] = keys[j];
                j--;
            }

            keys[j + 1] = t;
        }
    }

    public static void Sort(Span<Pixel_24bit> keys, IComparer<Pixel_24bit> comparer, int step)
    {
        for (int i = 0; i < (keys.Length - step); i += step)
        {
            Pixel_24bit t = keys[i + step];

            int j = i;
            while (j >= 0 && comparer.Compare(t, keys[j]) < 0)
            {
                keys[j + step] = keys[j];
                j -= step;
            }

            keys[j + step] = t;
        }
    }

    public void Sort(Span<Pixel_24bit> span, IComparer<Pixel_24bit> comparer)
    {
        for (int i = 0; i < span.Length - 1; i += 1)
        {
            Pixel_24bit t = span[i + 1];

            int j = i;
            while (j >= 0 && comparer.Compare(t, span[j]) < 0)
            {
                span[j + 1] = span[j];
                j -= 1;
            }

            span[j + 1] = t;
        }
    }
}

public class BlockSort : ISorter
{
    public void Sort(Span<Pixel_24bit> span, IComparer<Pixel_24bit> comparer)
    {
        throw new NotImplementedException();
    }
}

public static class FloydRivest
{

    /// <summary>
    /// Rearranges the elements between left and right, such that for some value k, the kth element in the 
    /// list will contain the (k − left + 1)th smallest value, with the ith element being less than or equal 
    /// to the kth for all left ≤ i ≤ k and the jth element being larger or equal to for k ≤ j ≤ right.
    /// </summary>
    /// <param name="span"></param>
    /// <param name="comparer"></param>
    /// <param name="left">Inclusive</param>
    /// <param name="right">Inclusive</param>
    /// <param name="k">left ≤ k ≤ right</param>
    public static void Select(Span<Pixel_24bit> span, IComparer<Pixel_24bit> comparer, int left, int right, int k)
    {
        // left is the left index for the interval
        // right is the right index for the interval
        // k is the desired index value, where array[k] is the (k+1)th smallest element when left = 0
        while (right > left)
        {
            // Use select recursively to sample a smaller set of size s
            // the arbitrary constants 600 and 0.5 are used in the original
            // version to minimize execution time.
            if (right - left > 600)
            {
                int n = right - left + 1;
                int _i = k - left + 1;
                int z = (int)Math.Log(n);
                int s = (int)(Math.Exp(2 * z / 3) / 2);
                int sd = (int)(Math.Sqrt(z * s * (n - s) / n) * Math.Sign(_i - n / 2));
                int newLeft = Math.Max(left, k - _i * s/n + sd);
                int newRight = Math.Min(right, k + (n - _i) * s / n + sd);
                Select(span, comparer, newLeft, newRight, k);
            }

            // partition the elements between left and right around t
            Pixel_24bit t = span[k];
            int i = left;
            int j = right;
            
            span.Swap(left, k);
            if (comparer.Compare(span[right],  t) > 0)
            {
                span.Swap(right, left);
            }
            while (i < j)
            {
                span.Swap(i, j);
                i++;
                j--;
                while (comparer.Compare(span[i], t) < 0)                
                    i++;
                
                while (comparer.Compare(span[j], t) > 0)                
                    j--;
                
                if (comparer.Compare(span[left], t) == 0)
                {
                    span.Swap(left, j);
                }
                else
                {
                    j++;
                    span.Swap(left, k);
                }
                // Adjust left and right towards the boundaries of the subset
                // containing the (k − left + 1)th smallest element.
                if (!(comparer.Compare(span[j], span[k]) > 0))                
                    left = j + 1;
                
                if (!(comparer.Compare(span[k], span[j]) > 0))                
                    right = j - 1;
                
            }
        }
    }
}



[Serializable]
public struct Pixel_24bit
{
    private byte _r, _g, _b;
    public readonly byte R => _r;
    public readonly byte G => _g;
    public readonly byte B => _b;


    public Pixel_24bit(byte r, byte g, byte b)
    {
        (_r, _g, _b) = (r, g, b);
    }
    public Pixel_24bit(byte[] rgb)
    {
        (_r, _g, _b) = (rgb[0], rgb[1], rgb[2]);
    }


    // Serialization
    private Pixel_24bit(SerializationInfo info, StreamingContext context)
    {
        _r = info.GetByte("Red");
        _g = info.GetByte("Green");
        _b = info.GetByte("Blue");
    }
    public void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        info.AddValue("Red", _r);
        info.AddValue("Green", _g);
        info.AddValue("Blue", _b);
    }

    public bool Equals(Pixel_24bit other)
    {
        return _r == other._r && _g == other._g && _b == other._b;
    }
}