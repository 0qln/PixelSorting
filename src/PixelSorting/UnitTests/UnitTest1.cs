namespace UnitTests
{
    public class UnitTest1
    {
        [Theory]
        [InlineData(1, 1, 1, 1)]
        public void TestSorting(int Size, int Step, int From, int To)
        {
            var tests = Generator.GenerateTestingData([ new (Size, Step, From, To) ]);

            foreach (var test in tests)
            {
                // TEST
            }
        }
    }
}