using System.Runtime.InteropServices;

namespace UnitTests
{
    public class SortingTests
    {
        [Theory]
        [InlineData(1, 1, 1, 1)]
        public void InsertionSort(int Size, int Step, int From, int To)
        {
            var tests = Generator.GenerateTestingData<Pixel_24bit>([ new (Size, Step, From, To) ], new Comparer24bit_soA_stB(), 1);

            foreach (var test in tests)
            {
                var expected = test.Sorted;
                var result = test.Unsorted;
                Sorter.InsertionSort<Pixel_24bit>(result, default, Step, From, To);

                Assert.True(result.SequenceEqual(expected));
            }
        }
    }
}