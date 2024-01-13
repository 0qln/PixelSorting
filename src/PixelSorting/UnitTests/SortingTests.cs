using System.Linq;
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
        public void UInt_red(uint a, uint b)
        {
            var result = new ComparerUIntPixel24bit_soA_stR3().Compare(a, b);
            var expected = To24bit(a).R.CompareTo(To24bit(b).R);

            if      (expected == 0) Assert.True(result == 0);
            else if (expected > 0) Assert.True(result > 0);
            else if (expected < 0) Assert.True(result < 0);
        }

        [Theory]
        [InlineData(68608, 0)]
        [InlineData(68608, 68609)]
        [InlineData(1245439, 68710)]
        [InlineData(68610, 68618)]
        public void Int_red(int a, int b)
        {
            var result = new ComparerIntPixel24bit_soA_stR2().Compare(a, b);
            var expected = To24bit(a).R.CompareTo(To24bit(b).R);

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
                    var result = new ComparerIntPixel24bit_soA_stR2().Compare(a, b);
                    var expected = To24bit(a).R.CompareTo(To24bit(b).R);

                    if (expected == 0) Assert.True(result == 0);
                    else if (expected > 0) Assert.True(result > 0);
                    else if (expected < 0) Assert.True(result < 0);
                }
            }
        }

        [Fact]
        public void UInt_red_BULK()
        {
            for (uint a = 0xF; a <= 0x00FFFFFF; a <<= 0x1)
            {
                for (uint b = 0xF; b <= 0x00FFFFFF; b <<= 0x1)
                {
                    var result = new ComparerUIntPixel24bit_soA_stR4().Compare(a, b);
                    var expected = To24bit(a).R.CompareTo(To24bit(b).R);

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
        [InlineData(100, 1, 1, 1)]
        [InlineData(1000, 1, 1, 1)]
        public void InsertionSort(int Size, int Step, int From, int To)
        {
            var tests = Generator.GenerateTestingData<Pixel_24bit>([new(Size, Step, From, To)], new Comparer24bit_soA_stR(), 1);

            foreach (var test in tests)
            {
                // Sort
                Sorter.InsertionSort<Pixel_24bit>(test.Unsorted, comparer: default, Step, From, To);

                // Assert
                Assert.True(test.Unsorted.SequenceEqual(test.Sorted));
            }
        }

        [Fact]
        public void InsertionSort_24bit()
        {
            var pixeltests = Generator.GenerateTestingData<Pixel_24bit>([new(1000, 1, 0, 1000)], new Comparer24bit_soA_stR(), 1);
            var bytetest = pixeltests.Select(test =>
            {
                TestInstance<byte> ret = new TestInstance<byte>
                {
                    Properties = test.Properties,
                    Unsorted = test.Unsorted.SelectMany<Pixel_24bit, byte>(pixel => [pixel.R, pixel.G, pixel.B]).ToArray(),
                    Sorted = test.Sorted.SelectMany<Pixel_24bit, byte>(pixel => [pixel.R, pixel.G, pixel.B]).ToArray(),
                };
                return ret;
            }).ToList();

            foreach (var test in bytetest)
            {
                // The sorting method to test: 
                Sorter.InsertionSort_24bit(test.Unsorted, new ComparerByByte24bit_soA(), 0, test.Properties.Step, test.Properties.From, test.Properties.To);

                // Assert result
                Assert.True(test.Sorted.SequenceEqual(test.Unsorted));
            }
        }

        [Fact]
        public void InsertionSort_24bitAsInt()
        {
            var pixeltests = Generator.GenerateTestingData<Pixel_24bit>([new(1000, 1, 0, 1000)], new Comparer24bit_soA_stR(), 1);
            var bytetest = pixeltests.Select(test =>
            {
                TestInstance<int> ret = new TestInstance<int>
                {
                    Properties = test.Properties,
                    Unsorted = test.Unsorted.Select<Pixel_24bit, int>(pixel => BitConverter.ToInt32([pixel.R, pixel.G, pixel.B, 0])).ToArray(),
                    Sorted = test.Sorted.Select<Pixel_24bit, int>(pixel => BitConverter.ToInt32([pixel.R, pixel.G, pixel.B, 0])).ToArray(),
                };
                return ret;
            }).ToList();

            foreach (var test in bytetest)
            {
                // The sorting method to test: 
                Sorter.InsertionSort_24bitAsInt(test.Unsorted, new ComparerIntPixel24bit_soA_stR4(), test.Properties.Step, test.Properties.From, test.Properties.To);

                // Assert result
                Assert.True(test.Sorted.SequenceEqual(test.Unsorted));
            }
        }

        [Fact]
        public void InsertionSort_24bitAsUInt()
        {
            var pixeltests = Generator.GenerateTestingData<Pixel_24bit>([new(1000, 1, 0, 1000)], new Comparer24bit_soA_stR(), 1);
            var bytetest = pixeltests.Select(test =>
            {
                TestInstance<uint> ret = new TestInstance<uint>
                {
                    Properties = test.Properties,
                    Unsorted = test.Unsorted.Select<Pixel_24bit, uint>(pixel => BitConverter.ToUInt32([pixel.R, pixel.G, pixel.B, 0])).ToArray(),
                    Sorted = test.Sorted.Select<Pixel_24bit, uint>(pixel => BitConverter.ToUInt32([pixel.R, pixel.G, pixel.B, 0])).ToArray(),
                };
                return ret;
            }).ToList();

            foreach (var test in bytetest)
            {
                // The sorting method to test: 
                Sorter.InsertionSort_24bitAsUInt(test.Unsorted, new ComparerUIntPixel24bit_soA_stR1(), test.Properties.Step, test.Properties.From, test.Properties.To);

                // Assert result
                Assert.True(test.Sorted.SequenceEqual(test.Unsorted));
            }
        }
    }
}