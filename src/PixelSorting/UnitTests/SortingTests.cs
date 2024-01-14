using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace UnitTests
{
    public class ComparisonTests
    {
        static uint ToUInt(_24bit p) => BitConverter.ToUInt32([p.R, p.G, p.B, 0]);
        static _24bit To24bit(uint x) => new(BitConverter.GetBytes(x)[0], BitConverter.GetBytes(x)[1], BitConverter.GetBytes(x)[2]);
        static _24bit To24bit(int x) => new(BitConverter.GetBytes(x)[0], BitConverter.GetBytes(x)[1], BitConverter.GetBytes(x)[2]);


        [Theory]
        [InlineData(68608, 0)]
        [InlineData(68608, 68609)]
        [InlineData(1245439, 68710)]
        [InlineData(68610, 68618)]
        public void Int_red(int a, int b)
        {
            var result = new ComparerIntPixel_soA_stR_2().Compare(a, b);
            var expected = new ComparerIntPixel_soA_stR_1().Compare(a, b);

            if (expected == 0) Assert.True(result == 0);
            else if (expected > 0) Assert.True(result > 0);
            else if (expected < 0) Assert.True(result < 0);
        }

        [Fact]
        public void Int_red_BULK()
        {
            for (int a = 0xF; a <= 0x00FFFFFF; a <<= 0x1)
            {
                for (int b = 0xF; b <= 0x00FFFFFF; b <<= 0x1)
                {
                    var result = new ComparerIntPixel_soA_stR().Compare(a, b);
                    var expected = new ComparerIntPixel_soA_stR_1().Compare(a, b);

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
            var comparer = new ComparerIntPixel_soA_stR_1();

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
        public void HeapSort_PixelSpan(int size, int step, int from, int to)
        {
            var comparer = new ComparerIntPixel_soA_stR_1();

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
            var comparer = new ComparerIntPixel_soA_stR_1();

            var tests = Generator.GenerateTestingData<int>([new(size, step, from, to)], comparer, 69);

            foreach (var test in tests)
            {
                Sorter<int>.IntrospectiveSort(new Sorter<int>.PixelSpan(test.Unsorted, test.Properties.Step, test.Properties.From, test.Properties.To), comparer);
                Assert.True(test.Sorted.SequenceEqual(test.Unsorted, EqualityComparer<int>.Create((a, b) => comparer.Compare(a, b) == 0)));
            }
        }
    }
}