namespace TestDataGenerator
{
    public record struct TestInstance(TestDataSize Properties, byte[] Unsorted, byte[] Sorted);
    public record struct TestInstance<T>(TestDataSize Properties, T[] Unsorted, T[] Sorted);
}
