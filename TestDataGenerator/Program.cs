
internal class Program
{
    
    static (int Horizontal, int Vertical)[] CommonImageSizesL() => new[]
    {
        (50, 50),
        (426, 240),
        (640, 360),
        (854, 480),
        (1280, 720),
        (1920, 1080),
        (2560, 1440),
        (3840, 2160)
    };

    /// <summary>
    /// Run program to generate Testing Data
    /// </summary>
    /// <param name="args"></param>
    private static void Main(string[] args)
    {

        List<TestData> testingData = new();
        void AddVariances(int size)
        {
            testingData.Add(new TestData
            { Size = size, Step = 1, From = 0, To = size, });

            // Step alternation
            testingData.Add(new TestData
            { Size = size, Step = size / 10, From = 0, To = size, });

            // Window Alternation
            testingData.Add(new TestData
            { Size = size, Step = 1, From = size / 10, To = size - size / 10, });

            // Step and window alternation
            testingData.Add(new TestData
            { Size = size, Step = size / 10, From = size / 10, To = size - size / 10, });
        }

        foreach (var imgSize in CommonImageSizesL())
        {
            AddVariances(imgSize.Horizontal);
            AddVariances(imgSize.Vertical);
        }

        Random rng = new();

        if (!Directory.Exists("../../../Files/"))
             Directory.CreateDirectory("../../../Files/");
        
        foreach (var data in testingData)
        {
            byte[] unsorted = new byte[data.Size];
            rng.NextBytes(unsorted);
            byte[] sorted = unsorted.ToArray();
            InsertionSort<byte>(sorted, null, data.Step, data.From, data.To);

            var pathUnsorted = $"../../../Files/{data}_Unsorted.txt";
            var pathSorted = $"../../../Files/{data}_Sorted.txt";
            File.WriteAllLines(pathUnsorted, unsorted.Select(x => x.ToString()));
            File.WriteAllLines(pathSorted, sorted.Select(x => x.ToString()));
        }
    }

    /// <summary>
    /// This works, but slow.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="keys"></param>
    /// <param name="comparer"></param>
    /// <param name="step"></param>
    /// <param name="from">Inclusive</param>
    /// <param name="to">Exclusive</param>
    private static void InsertionSort<T>(Span<T> keys, IComparer<T>? comparer, int step, int from, int to)
    {
        // 
        comparer ??= Comparer<T>.Default;

        for (int i = from; i < to - step; i += step)
        {
            T t = keys[i + step];

            int j = i;
            while (j >= from && comparer.Compare(t, keys[j]) < 0)
            {
                keys[j + step] = keys[j];
                j -= step;
            }

            keys[j + step] = t;
        }
    }
}

record struct TestData(int Size, int Step, int From, int To);

