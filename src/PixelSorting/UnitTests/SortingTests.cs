using System.Runtime.InteropServices;

namespace UnitTests
{
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
    }
}