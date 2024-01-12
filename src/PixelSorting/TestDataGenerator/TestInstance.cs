using Utils;

namespace TestDataGenerator
{
    public record struct TestInstance(TestDataSize Properties, byte[] Unsorted, byte[] Sorted);
    public record struct TestInstance<T>(TestDataSize Properties, T[] Unsorted, T[] Sorted)
    {


        public TestInstance<T> Clone_()
        {
            return new TestInstance<T> { Properties = Properties, Unsorted = (T[])Unsorted.Clone(), Sorted = (T[])Sorted.Clone() };
        }

        public TestInstance<TResult> CloneAs<TResult>()
        {
            return Clone_().ReinterpretCast<TestInstance<T>, TestInstance<TResult>>();
        }
    }
}
