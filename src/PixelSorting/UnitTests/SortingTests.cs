using Sorting.Pixels._24;
using Sorting.Pixels._32;
using Sorting.Pixels.Comparer;

namespace UnitTests
{
    using Pixel32bit = int;
    public class ComparisonTests
    {
        static uint ToUInt(Pixel24bitStruct p) => BitConverter.ToUInt32([p.R, p.G, p.B, 0]);
        static Pixel32bitUnion ToUnionInt(Pixel32bit p) => new Pixel32bitUnion { Int = p };
        static Pixel24bitStruct To24bit(uint x) => new(BitConverter.GetBytes(x)[0], BitConverter.GetBytes(x)[1], BitConverter.GetBytes(x)[2]);
        static Pixel24bitStruct To24bit(int x) => new(BitConverter.GetBytes(x)[0], BitConverter.GetBytes(x)[1], BitConverter.GetBytes(x)[2]);


        [Fact]
        public void Int_red_BULK()
        {
            for (int a = 0xF; a != 0x0F000000; a <<= 0x1)
            {
                for (int b = 0xF; b != 0x0F000000; b <<= 0x1)
                {
                    var result = new PixelComparer.Ascending.Red._32bit().Compare(a, b);
                    var expected = new PixelComparer.Ascending.Red._32bitUnion().Compare(ToUnionInt(a), ToUnionInt(b));
                    
                    if (expected == 0) Assert.True(result == 0);
                    else if (expected > 0) Assert.True(result > 0);
                    else if (expected < 0) Assert.True(result < 0);
                }
            }
        }
    }

    public class SortingTests
    {
        [Theory]
        [InlineData(5, 1, 0, 5)]
        [InlineData(100, 1, 0, 100)]
        [InlineData(100, 2, 0, 100)]
        [InlineData(100, 3, 0, 100)]
        [InlineData(100, 1, 30, 100)]
        [InlineData(100, 2, 0, 70)]
        [InlineData(100, 3, 30, 70)]
        public void InsertionSort_PixelSpan(int size, int step, int from, int to)
        {
            var comparer = new PixelComparer.Ascending.Red._32bit();

            var tests = Generator.GenerateTestingData<int>([new (size, step, from, to)], comparer, 69);

            foreach (var test in tests)
            {
                Sorter<int>.InsertionSort(new Sorter<int>.PixelSpan(test.Unsorted, test.Properties.Step, test.Properties.From, test.Properties.To), comparer);
                Assert.True(test.Sorted.SequenceEqual(test.Unsorted, EqualityComparer<int>.Create((a, b) => comparer.Compare(a, b) == 0)));
            }
        }


        [Theory]
        [InlineData(5, 1, 0, 5)]
        [InlineData(100, 1, 0, 100)]
        [InlineData(100, 2, 0, 100)]
        [InlineData(100, 3, 0, 100)]
        [InlineData(100, 1, 30, 100)]
        [InlineData(100, 2, 0, 70)]
        [InlineData(100, 3, 30, 70)]
        public void ShellSort_PixelSpan(int size, int step, int from, int to)
        {
            var comparer = new PixelComparer.Ascending.Red._32bit();

            var tests = Generator.GenerateTestingData<int>([new(size, step, from, to)], comparer, 69);

            foreach (var test in tests)
            {
                Sorter<int>.ShellSort(new Sorter<int>.PixelSpan(test.Unsorted, test.Properties.Step, test.Properties.From, test.Properties.To), comparer);
                Assert.True(test.Sorted.SequenceEqual(test.Unsorted, EqualityComparer<int>.Create((a, b) => comparer.Compare(a, b) == 0)));
            }
        }


        [Theory]
        [InlineData(5, 1, 0, 5)]
        [InlineData(100, 1, 0, 100)]
        [InlineData(100, 2, 0, 100)]
        [InlineData(100, 3, 0, 100)]
        [InlineData(100, 1, 30, 100)]
        [InlineData(100, 2, 0, 70)]
        [InlineData(100, 3, 30, 70)]
        public void HeapSort_PixelSpan(int size, int step, int from, int to)
        {
            var comparer = new PixelComparer.Ascending.Red._32bit();

            var tests = Generator.GenerateTestingData<int>([new(size, step, from, to)], comparer, 69);

            foreach (var test in tests)
            {
                Sorter<int>.HeapSort(new Sorter<int>.PixelSpan(test.Unsorted, test.Properties.Step, test.Properties.From, test.Properties.To), comparer);
                Assert.True(test.Sorted.SequenceEqual(test.Unsorted, EqualityComparer<int>.Create((a, b) => comparer.Compare(a, b) == 0)));
            }
        }



        [Theory]
        [InlineData(5, 1, 0, 5)]

        [InlineData(100, 1, 0, 100)]
        [InlineData(100, 2, 0, 100)]
        [InlineData(100, 3, 0, 100)]
        [InlineData(100, 1, 30, 100)]
        [InlineData(100, 2, 0, 70)]
        [InlineData(100, 3, 30, 70)]
        public void IntroSort_PixelSpan(int size, int step, int from, int to)
        {
            var comparer = new PixelComparer.Ascending.Red._32bit();

            var tests = Generator.GenerateTestingData<int>([new(size, step, from, to)], comparer, 69);

            foreach (var test in tests)
            {
                Sorter<int>.IntrospectiveSort(new Sorter<int>.PixelSpan(test.Unsorted, test.Properties.Step, test.Properties.From, test.Properties.To), comparer);
                Assert.True(test.Sorted.SequenceEqual(test.Unsorted, EqualityComparer<int>.Create((a, b) => comparer.Compare(a, b) == 0)));
            }
        }
    }
}
