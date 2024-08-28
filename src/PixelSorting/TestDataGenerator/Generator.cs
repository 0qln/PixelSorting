using Sorting;
using System.Runtime.InteropServices;

namespace TestDataGenerator;

public static class Generator
{
    public static (int Horizontal, int Vertical)[] CommonImageSizes() =>
    [
        (50, 50),
        (426, 240),
        (640, 360),
        (854, 480),
        (1280, 720),
        (1920, 1080),
        (2560, 1440),
        (3840, 2160)
    ];

    public static IEnumerable<TestDataSize> GetVariances(int size)
    {
        // Default
        yield return new TestDataSize { Size = size, Step = 1, From = 0, To = size, };

        // Step alternation
        yield return new TestDataSize { Size = size, Step = size / 10, From = 0, To = size, };

        // Window Alternation
        yield return new TestDataSize { Size = size, Step = 1, From = size / 10, To = size - size / 10, };

        // Step and window alternation
        yield return new TestDataSize { Size = size, Step = size / 10, From = size / 10, To = size - size / 10, };
    }

    public static IEnumerable<TestDataSize> GetRealisticTestingDataset()
    {
        foreach (var (horizontal, vertical) in CommonImageSizes())
        {
            // Row
            yield return new TestDataSize
                { Size = horizontal, Step = 1, From = 0, To = horizontal };

            // Col
            yield return new TestDataSize
                { Size = horizontal * vertical, Step = horizontal, From = 0, To = horizontal * vertical };
        }
    }

    public static IEnumerable<TestDataSize> GetDefaultTestingDataset()
    {
        foreach (var (horizontal, vertical) in CommonImageSizes())
        {
            foreach (var x in GetVariances(horizontal)) yield return x;
            foreach (var x in GetVariances(vertical)) yield return x;
        }
    }

    public static IEnumerable<TestInstance<TStruct>> GenerateTestingData<TStruct>(
        IEnumerable<TestDataSize> testingDataSizes,
        IComparer<TStruct> comparer,
        int? seed = null)
        where TStruct : struct
    {
        Random rng = seed is null ? new() : new((int)seed);

        foreach (var datasize in testingDataSizes)
        {
            var bytes = new byte[datasize.Size * Marshal.SizeOf<TStruct>()];
            rng.NextBytes(bytes);
            var unsorted = ByteArrayToStructArray<TStruct>(bytes);
            var sorted = unsorted.ToArray();
            Sorter<TStruct>.InsertionSort(sorted, comparer, datasize.Step, datasize.From, datasize.To);

            yield return new(datasize, unsorted, sorted);
        }
    }

    public static TStruct ByteArrayToStruct<TStruct>(byte[] byteArray) where TStruct : struct
    {
        ArgumentNullException.ThrowIfNull(byteArray);

        if (Marshal.SizeOf(typeof(TStruct)) > byteArray.Length)
        {
            throw new ArgumentException("Byte array is smaller than the size of the struct.");
        }

        // Pin the managed memory while copying out the data, then unpin it
        var handle = GCHandle.Alloc(byteArray, GCHandleType.Pinned);
        var theStruct = (TStruct)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(TStruct))!;
        handle.Free();

        return theStruct;
    }

    public static TStruct[] ByteArrayToStructArray<TStruct>(byte[] byteArray) where TStruct : struct
    {
        if (byteArray == null || byteArray.Length == 0)
        {
            throw new ArgumentException("Byte array is null or empty.");
        }

        var structSize = Marshal.SizeOf(typeof(TStruct));

        if (byteArray.Length % structSize != 0)
        {
            throw new ArgumentException("Byte array length is not a multiple of the struct size.");
        }

        var structCount = byteArray.Length / structSize;
        var structArray = new TStruct[structCount];

        for (var i = 0; i < structCount; i++)
        {
            var structBytes = new byte[structSize];
            Array.Copy(byteArray, i * structSize, structBytes, 0, structSize);
            structArray[i] = ByteArrayToStruct<TStruct>(structBytes);
        }

        return structArray;
    }

    public static void SaveToDisc(IEnumerable<TestInstance> testingData, string? folder = null)
    {
        folder ??= "../../../Files";

        if (!Directory.Exists(folder))
        {
            Directory.CreateDirectory(folder);
        }

        foreach (var instance in testingData)
        {
            var pathUnsorted = Path.Combine(folder, instance.Properties + "_Unsorted.txt");
            var pathSorted = Path.Combine(folder, instance.Properties + "_Sorted.txt");
            File.WriteAllLines(pathUnsorted, instance.Unsorted.Select(x => x.ToString()));
            File.WriteAllLines(pathSorted, instance.Sorted.Select(x => x.ToString()));
        }
    }
}