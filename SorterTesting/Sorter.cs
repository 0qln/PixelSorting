

using BenchmarkDotNet.Attributes;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;

namespace SorterTesting;


public static class Extensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Swap(this Span<Pixel_24bit> span, int i, int j)
    {
        Pixel_24bit t = span[i];
        span[i] = span[j];
        span[j] = t;
    }
}

public class SorterBenchmark
{
    static Sorter _1000 = new Sorter(1000);
    static Sorter _10000 = new Sorter(10000);
    static Sorter _100000 = new Sorter(100000);
    static Sorter _1000000 = new Sorter(1000000);


    [Benchmark]
    public void Tim1000()
    {
        _1000.TimSort();
    }
    [Benchmark]
    public void Tim10000()
    {
        _10000.TimSort();
    }
    [Benchmark]
    public void Tim100000()
    {
        _100000.TimSort();
    }
    [Benchmark]
    public void Tim1000000()
    {
        _1000000.TimSort();
    }


    [Benchmark]
    public void Heap1000()
    {
        _1000.HeapSort();
    }
    [Benchmark]
    public void Heap10000()
    {
        _10000.HeapSort();
    }
    [Benchmark]
    public void Heap100000()
    {
        _100000.HeapSort();
    }
    [Benchmark]
    public void Heap1000000()
    {
        _1000000.HeapSort();
    }
}

public class Sorter
{
    private static Random _rng = new Random();

    private readonly IComparer<Pixel_24bit> _comparer;
    private readonly ReadOnlyCollection<Pixel_24bit> _data;
    private readonly int _count;

    public Pixel_24bit[] Data => _data.ToArray();


    public Sorter(int n)
    {
        _count = n;
        _data = new ReadOnlyCollection<Pixel_24bit>(TestData(n));
        _comparer = new Comparer24bit.Ascending.Red();
    }


    private Pixel_24bit[] TestData(int n)
    {
        var data = new Pixel_24bit[n];
        for (int i = 0; i < n; i++)
            data[i] = Pixel_24bit.FromHash(_rng.Next());
        return data;
    }

    private void PrintData()
    {
        foreach (var data in _data)
        {
            Console.WriteLine(data.ToString());
        }
        Console.WriteLine();
    }
    private void PrintData(Pixel_24bit[] data)
    {
        foreach (var e in data)
        {
            Console.WriteLine(e.ToString());
        }
        Console.WriteLine();
    }


    [Benchmark]
    public void HeapSort()
    {
        SorterTesting.HeapSort.Sort(Data, _comparer);
    }

    [Benchmark]
    public void TimSort()
    {
        SorterTesting.TimSort.Sort(Data, _comparer, _count);
    }



    public void Testing()
    {
        var data = Data;
        PrintData(data);
        SorterTesting.TimSort.Sort(data, _comparer, _count);
        PrintData(data);
    }
}

public static class TimSort
{
    const int RUN = 32;

    private static void InsertionSort(Span<Pixel_24bit> span, IComparer<Pixel_24bit> comparer, int left, int right)
    {
        for (int i = left + 1; i <= right; i++)
        {
            var temp = span[i];
            int j = i - 1;
            while (j >= left && comparer.Compare(span[j], temp) > 0)
            {
                span[j + 1] = span[j];
                j--;
            }
            span[j + 1] = temp;
        }
    }


    private static void Merge(Span<Pixel_24bit> span, IComparer<Pixel_24bit> comparer, int l, int m, int r)
    {
        int i;
        // original array is broken in two parts 
        // left and right array 
        int len1 = m - l + 1, len2 = r - m;
        var left = new Pixel_24bit[len1];
        var right = new Pixel_24bit[len2];
        for (i = 0; i < len1; i++)
            left[i] = span[l + i];
        for (i = 0; i < len2; i++)
            right[i] = span[m + 1 + i];

        i = 0;
        int j = 0;
        int k = l;

        // after comparing, we merge those two array 
        // in larger sub array 
        while (i < len1 && j < len2)
        {
            if (comparer.Compare(left[i], right[j]) <= 0)
            {
                span[k] = left[i];
                i++;
            }
            else
            {
                span[k] = right[j];
                j++;
            }
            k++;
        }

        // copy remaining elements of left, if any 
        while (i < len1)
        {
            span[k] = left[i];
            k++;
            i++;
        }

        // copy remaining element of right, if any 
        while (j < len2)
        {
            span[k] = right[j];
            k++;
            j++;
        }
    }

    public static void Sort(Span<Pixel_24bit> span, IComparer<Pixel_24bit> comparer, int n)
    {

        // Sort individual subarrays of size RUN 
        for (int i = 0; i < n; i += RUN)
            InsertionSort(span, comparer, i, Math.Min((i + 31), (n - 1)));

        // start merging from size RUN (or 32). It will merge 
        // to form size 64, then 128, 256 and so on .... 
        for (int size = RUN; size < n; size = 2 * size)
        {
            // pick starting point of left sub array. We 
            // are going to merge arr[left..left+size-1] 
            // and arr[left+size, left+2*size-1] 
            // After every merge, we increase left by 2*size 
            for (int left = 0; left < n; left += 2 * size)
            {
                // find ending point of left sub array 
                // mid+1 is starting point of right sub array 
                int mid = left + size - 1;
                int right = Math.Min((left + 2 * size - 1), (n - 1));

                // merge sub array arr[left.....mid] & 
                // arr[mid+1....right] 
                Merge(span, comparer, left, mid, right);
            }
        }
    }
}

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
            if (!s.Contains(span[i].Hash()))
                s.Add(span[i].Hash());
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
            ++pixelCounts[span[i].Hash() % n /*ass ._.*/];

        int p = 0;
        for (int i = 0; i < n; ++i)
            for (int j = 0; j < pixelCounts[i]; ++j)
                span[p++] = Pixel_24bit.FromHash(pixelCounts[i]);
    }


}

public static class HeapSort
{
    public static void Sort(Span<Pixel_24bit> span, IComparer<Pixel_24bit> comparer)
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

public static class InsertionSort
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

    public static void Sort(Span<Pixel_24bit> span, IComparer<Pixel_24bit> comparer)
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



public record struct Pixel_24bit(byte R, byte G, byte B)
{
    public static Pixel_24bit FromHash(int hash)
    {
        return new Pixel_24bit((byte)hash, (byte)(hash >> 8), (byte)(hash >> 16));
    }

    public int Hash()
    {
        return R | (G << 8) | (B << 16);
    }
}
