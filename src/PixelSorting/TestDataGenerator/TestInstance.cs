namespace TestDataGenerator
{
    public record struct TestInstance(TestDataSize Properties, byte[] Unsorted, byte[] Sorted);
}
