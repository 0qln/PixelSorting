using Sorting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace TestDataGenerator
{
    public static class Generator
    {

        public static (int Horizontal, int Vertical)[] CommonImageSizesL() => new[]
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

        static IEnumerable<TestDataSize> GetVariances(int size)
        {
            yield return (new TestDataSize { Size = size, Step = 1, From = 0, To = size, });

            // Step alternation
            yield return (new TestDataSize { Size = size, Step = size / 10, From = 0, To = size, });

            // Window Alternation
            yield return (new TestDataSize { Size = size, Step = 1, From = size / 10, To = size - size / 10, });

            // Step and window alternation
            yield return (new TestDataSize { Size = size, Step = size / 10, From = size / 10, To = size - size / 10, });
        }

        public static IEnumerable<TestDataSize> GetDefaultTestingDataset()
        {
            foreach (var imgSize in CommonImageSizesL())
            {
                foreach (var x in GetVariances(imgSize.Horizontal)) yield return x;
                foreach (var x in GetVariances(imgSize.Vertical)) yield return x;
            }
        }

        public static IEnumerable<TestInstance> GenerateTestingData(IEnumerable<TestDataSize> testingDataSizes, int? seed = null)
        {
            Random rng = seed is null ? new() : new((int)seed);

            foreach (var datasize in testingDataSizes)
            {
                byte[] unsorted = new byte[datasize.Size];
                rng.NextBytes(unsorted);
                byte[] sorted = unsorted.ToArray();
                Sorter.InsertionSort<byte>(sorted, comparer:null, datasize.Step, datasize.From, datasize.To);

                yield return new(datasize, unsorted, sorted);
            }
        }
        public static IEnumerable<TestInstance<TStruct>> GenerateTestingData<TStruct>(IEnumerable<TestDataSize> testingDataSizes, IComparer<TStruct> comparer, int? seed = null)
            where TStruct :struct
        {
            Random rng = seed is null ? new() : new((int)seed);

            foreach (var datasize in testingDataSizes)
            {
                byte[] bytes = new byte[datasize.Size * Marshal.SizeOf<TStruct>()];
                rng.NextBytes(bytes);
                TStruct[] unsorted = ByteArrayToStructArray<TStruct>(bytes);
                TStruct[] sorted = unsorted.ToArray();
                Sorter.InsertionSort(sorted, comparer, datasize.Step, datasize.From, datasize.To);

                yield return new(datasize, unsorted, sorted);
            }
        }

        public static TStruct ByteArrayToStruct<TStruct>(byte[] byteArray) where TStruct : struct
        {
            if (byteArray == null || byteArray.Length == 0)
            {
                throw new ArgumentException("Byte array is null or empty.");
            }

            if (Marshal.SizeOf(typeof(TStruct)) > byteArray.Length)
            {
                throw new ArgumentException("Byte array is smaller than the size of the struct.");
            }

            // Pin the managed memory while copying out the data, then unpin it
            GCHandle handle = GCHandle.Alloc(byteArray, GCHandleType.Pinned);
            TStruct theStruct = (TStruct)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(TStruct))!;
            handle.Free();

            return theStruct;
        }
        public static TStruct[] ByteArrayToStructArray<TStruct>(byte[] byteArray) where TStruct : struct
        {
            if (byteArray == null || byteArray.Length == 0)
            {
                throw new ArgumentException("Byte array is null or empty.");
            }

            int structSize = Marshal.SizeOf(typeof(TStruct));

            if (byteArray.Length % structSize != 0)
            {
                throw new ArgumentException("Byte array length is not a multiple of the struct size.");
            }

            int structCount = byteArray.Length / structSize;
            TStruct[] structArray = new TStruct[structCount];

            for (int i = 0; i < structCount; i++)
            {
                byte[] structBytes = new byte[structSize];
                Array.Copy(byteArray, i * structSize, structBytes, 0, structSize);
                structArray[i] = ByteArrayToStruct<TStruct>(structBytes);
            }

            return structArray;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="testingData"></param>
        /// <param name="folder">Absolute folder path</param>
        public static void SaveToDisc(IEnumerable<TestInstance> testingData, string? folder = null)
        {
            folder ??= "../../../Files";

            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }

            foreach (var instance in testingData)
            {
                var pathUnsorted = Path.Combine(folder, instance.Properties.ToString() + "_Unsorted.txt");
                var pathSorted = Path.Combine(folder, instance.Properties.ToString() + "_Sorted.txt");
                File.WriteAllLines(pathUnsorted, instance.Unsorted.Select(x => x.ToString()));
                File.WriteAllLines(pathSorted, instance.Sorted.Select(x => x.ToString()));
            }
        }
    }
}
