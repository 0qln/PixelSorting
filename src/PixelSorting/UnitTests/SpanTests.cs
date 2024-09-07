using System.Numerics;
using Sorting.Pixels._32;

namespace UnitTests;

public class SpanTests
{
#if DEBUG
    [Theory]
    [InlineData(1, 1, 1, 1, 0, 1, 0, 0)]
    [InlineData(1, 1, 13, 10, 7, 10, 0, 0)]
    [InlineData(2, 2, 1, 1, 0, 1, 0, 0)]
    [InlineData(2, 2, 13, 10, 7, 10, 0, 0)]
    [InlineData(3, 3, 1, 1, 0, 1, 0, 0)]
    [InlineData(3, 3, 13, 10, 7, 10, 0, 0)]
    [InlineData(1920, 1080, 3, 10, 7, 10, 0, 0)]
    [InlineData(1080, 1920, 3, 10, 7, 10, 0, 0)]
    [InlineData(1920, 1080, 3, 10, 7, 10, 100, 0)]
    [InlineData(1080, 1920, 3, 10, 7, 10, 100, 0)]
    [InlineData(1920, 1080, 3, 10, 7, 10, 0, 100)]
    [InlineData(1080, 1920, 3, 10, 7, 10, 0, 100)]
    [InlineData(1920, 1080, 3, 10, 7, 10, 100, 100)]
    [InlineData(1080, 1920, 3, 10, 7, 10, 100, 100)]
    [InlineData(1920, 1080, 1, 1, 0, 1, 0, 0)]
    [InlineData(1080, 1920, 1, 1, 0, 1, 0, 0)]
    [InlineData(1920, 1080, 13, 10, 7, 10, 0, 0)]
    [InlineData(1080, 1920, 13, 10, 7, 10, 0, 0)]
    [InlineData(1920, 1080, 13, 10, 7, 10, 100, 0)]
    [InlineData(1080, 1920, 13, 10, 7, 10, 100, 0)]
    [InlineData(1920, 1080, 13, 10, 7, 10, 0, 100)]
    [InlineData(1080, 1920, 13, 10, 7, 10, 0, 100)]
    [InlineData(1920, 1080, 13, 10, 7, 10, 100, 100)]
    [InlineData(1080, 1920, 13, 10, 7, 10, 100, 100)]
    [InlineData(1920, 1080, -1, 1, 0, 1, 0, 0)]
    [InlineData(1080, 1920, -1, 1, 0, 1, 0, 0)]
    [InlineData(1920, 1080, -13, 10, 7, 10, 0, 0)]
    [InlineData(1080, 1920, -13, 10, 7, 10, 0, 0)]
    [InlineData(1920, 1080, -13, 10, 7, 10, 100, 0)]
    [InlineData(1080, 1920, -13, 10, 7, 10, 100, 0)]
    [InlineData(1920, 1080, -13, 10, 7, 10, 0, 100)]
    [InlineData(1080, 1920, -13, 10, 7, 10, 0, 100)]
    [InlineData(1920, 1080, -13, 10, 7, 10, 100, 100)]
    [InlineData(1080, 1920, -13, 10, 7, 10, 100, 100)]
    [InlineData(1920, 1080, 13, 10, -7, 10, 0, 0)]
    [InlineData(1080, 1920, 13, 10, -7, 10, 0, 0)]
    [InlineData(1920, 1080, 13, 10, -7, 10, 100, 0)]
    [InlineData(1080, 1920, 13, 10, -7, 10, 100, 0)]
    [InlineData(1920, 1080, 13, 10, -7, 10, 0, 100)]
    [InlineData(1080, 1920, 13, 10, -7, 10, 0, 100)]
    [InlineData(1920, 1080, 13, 10, -7, 10, 100, 100)]
    [InlineData(1080, 1920, 13, 10, -7, 10, 100, 100)]
    [InlineData(1920, 1080, -13, 10, -7, 10, 0, 0)]
    [InlineData(1080, 1920, -13, 10, -7, 10, 0, 0)]
    [InlineData(1920, 1080, -13, 10, -7, 10, 100, 0)]
    [InlineData(1080, 1920, -13, 10, -7, 10, 100, 0)]
    [InlineData(1920, 1080, -13, 10, -7, 10, 0, 100)]
    [InlineData(1080, 1920, -13, 10, -7, 10, 0, 100)]
    [InlineData(1920, 1080, -13, 10, -7, 10, 100, 100)]
    [InlineData(1080, 1920, -13, 10, -7, 10, 100, 100)]
    [InlineData(3840, 2160, 3, 10, 7, 10, 0, 0)]
    [InlineData(2160, 3840, 3, 10, 7, 10, 0, 0)]
    [InlineData(3840, 2160, 3, 10, 7, 10, 100, 0)]
    [InlineData(2160, 3840, 3, 10, 7, 10, 100, 0)]
    [InlineData(3840, 2160, 3, 10, 7, 10, 0, 100)]
    [InlineData(2160, 3840, 3, 10, 7, 10, 0, 100)]
    [InlineData(3840, 2160, 3, 10, 7, 10, 100, 100)]
    [InlineData(2160, 3840, 3, 10, 7, 10, 100, 100)]
    [InlineData(3840, 2160, 1, 1, 0, 1, 0, 0)]
    [InlineData(2160, 3840, 1, 1, 0, 1, 0, 0)]
    [InlineData(3840, 2160, 13, 10, 7, 10, 0, 0)]
    [InlineData(2160, 3840, 13, 10, 7, 10, 0, 0)]
    [InlineData(3840, 2160, 13, 10, 7, 10, 100, 0)]
    [InlineData(2160, 3840, 13, 10, 7, 10, 100, 0)]
    [InlineData(3840, 2160, 13, 10, 7, 10, 0, 100)]
    [InlineData(2160, 3840, 13, 10, 7, 10, 0, 100)]
    [InlineData(3840, 2160, 13, 10, 7, 10, 100, 100)]
    [InlineData(2160, 3840, 13, 10, 7, 10, 100, 100)]
    [InlineData(3840, 2160, -1, 1, 0, 1, 0, 0)]
    [InlineData(2160, 3840, -1, 1, 0, 1, 0, 0)]
    [InlineData(3840, 2160, -13, 10, 7, 10, 0, 0)]
    [InlineData(2160, 3840, -13, 10, 7, 10, 0, 0)]
    [InlineData(3840, 2160, -13, 10, 7, 10, 100, 0)]
    [InlineData(2160, 3840, -13, 10, 7, 10, 100, 0)]
    [InlineData(3840, 2160, -13, 10, 7, 10, 0, 100)]
    [InlineData(2160, 3840, -13, 10, 7, 10, 0, 100)]
    [InlineData(3840, 2160, -13, 10, 7, 10, 100, 100)]
    [InlineData(2160, 3840, -13, 10, 7, 10, 100, 100)]
    [InlineData(3840, 2160, 13, 10, -7, 10, 0, 0)]
    [InlineData(2160, 3840, 13, 10, -7, 10, 0, 0)]
    [InlineData(3840, 2160, 13, 10, -7, 10, 100, 0)]
    [InlineData(2160, 3840, 13, 10, -7, 10, 100, 0)]
    [InlineData(3840, 2160, 13, 10, -7, 10, 0, 100)]
    [InlineData(2160, 3840, 13, 10, -7, 10, 0, 100)]
    [InlineData(3840, 2160, 13, 10, -7, 10, 100, 100)]
    [InlineData(2160, 3840, 13, 10, -7, 10, 100, 100)]
    [InlineData(3840, 2160, -13, 10, -7, 10, 0, 0)]
    [InlineData(2160, 3840, -13, 10, -7, 10, 0, 0)]
    [InlineData(3840, 2160, -13, 10, -7, 10, 100, 0)]
    [InlineData(2160, 3840, -13, 10, -7, 10, 100, 0)]
    [InlineData(3840, 2160, -13, 10, -7, 10, 0, 100)]
    [InlineData(2160, 3840, -13, 10, -7, 10, 0, 100)]
    [InlineData(3840, 2160, -13, 10, -7, 10, 100, 100)]
    [InlineData(2160, 3840, -13, 10, -7, 10, 100, 100)]
    [InlineData(3840, 2060, 13, 10, 7, 10, 0, 0)]
    [InlineData(3840, 2060, 1, 1, 0, 1, 0, 0)]
    public void PixelSpan2D_FastEstimateItemCount(
        int maxU, int maxV,
        BigInteger numU, BigInteger denU, BigInteger numV, BigInteger denV,
        int offU, int offV)
    {
        var data = new Pixel32bitUnion[Math.Max(maxU * maxV, 1)];
        var span = new Sorter<Pixel32bitUnion>.PixelSpan2D(
            data, maxU, maxV,
            new Sorter<Pixel32bitUnion>.Fraction(numU, denU),
            new Sorter<Pixel32bitUnion>.Fraction(numV, denV),
            offU, offV);

        Assert.Equal(
            span.CalculateItemCount(),
            (int)span.EstimateItemCount());
    }
#endif

#if DEBUG
    [Theory]
    [InlineData(3840, 2060, 1, 1, 0, 1, 0, 0, 3840)]
    [InlineData(3840, 2060, 0, 1, 1, 1, 0, 0, 2060)]
    public void PixelSpan2D_CalculateItemCount(
        int maxU, int maxV,
        BigInteger numU, BigInteger denU, BigInteger numV, BigInteger denV,
        int offU, int offV,
        int expected)
    {
        var data = new Pixel32bitUnion[Math.Max(maxU * maxV, 1)];
        var span = new Sorter<Pixel32bitUnion>.PixelSpan2D(
            data, maxU, maxV,
            new Sorter<Pixel32bitUnion>.Fraction(numU, denU),
            new Sorter<Pixel32bitUnion>.Fraction(numV, denV),
            offU, offV);

        Assert.Equal(expected, span.CalculateItemCount());
    }
#endif

    [Fact]
    public void PixelSpan2D_Indexing()
    {
        var data = new Pixel32bitUnion[3840 * 2060];
        var span = new Sorter<Pixel32bitUnion>.PixelSpan2D(
            data, 3840, 2060, 1.1, 1.6, 0, 0);

        for (uint i = 0; i < span.ItemCount; i++)
            Assert.Equal(span.MapIndex(i), span.LookupIndex(i));
    }

    /// <summary>
    /// Asserts that each angle of pixels does not overlap with any other when iterating.
    /// </summary>
    [Theory]
    [InlineData(0)]
    [InlineData(0.0314159265358979340000000000)]
    [InlineData(1.4451326206513060000000000000)]
    [InlineData(Math.PI)]
    public void Test_AtomicIndexing(double alpha)
    {
        var tanAlpha = Math.Tan(alpha);
        var imageHeight = 1080;
        var imageWidth = 1920;
        var checks = new bool[imageHeight * imageWidth];

        void AssertRun(double stepU, double stepV, int offU, int offV)
        {
            Sorter<bool>.PixelSpan2D span = new(checks, imageWidth, imageHeight, stepU, stepV, offU, offV);
            for (int i = 0; i < span.ItemCount; i++)
            {
                Assert.False(span[i]);
                span[i] = true;
            }
        }

        switch (alpha)
        {
            case 0:
            {
                for (var i = 0; i < imageWidth; i++)
                    AssertRun(0, 1, i, 0);
                break;
            }
            case > 0 and < Math.PI / 2:
            {
                // left
                for (var i = 0; i < imageHeight; i++)
                    AssertRun(1, 1 / tanAlpha, 0, i);

                // top
                if (alpha > Math.PI / 4)
                {
                    for (var i = 0; i < imageWidth; i++)
                        AssertRun(1, 1 / tanAlpha, i, 0);
                }
                else
                {
                    for (var i = 0; i < imageWidth; i++)
                        AssertRun(tanAlpha, 1, i, 0);
                }

                break;
            }
            case Math.PI / 2:
            {
                for (var i = 0; i < imageHeight; i++)
                    AssertRun(1, 0, 0, i);
                break;
            }
            case > Math.PI / 2 and < Math.PI:
            {
                // top
                for (var i = 0; i < imageWidth; i++)
                {
                    AssertRun(tanAlpha, 1, i, 0);
                }

                // right
                if (alpha > Math.PI / 2 + Math.PI / 4)
                {
                    for (var i = 0; i < imageHeight; i++)
                        AssertRun(tanAlpha, 1, imageWidth - 1, i);
                }
                else
                {
                    for (var i = 0; i < imageHeight; i++)
                        AssertRun(-1, -1 / tanAlpha, imageWidth - 1, i);
                }

                break;
            }
            case Math.PI:
            {
                for (var i = 0; i < imageWidth; i++)
                    AssertRun(0, 1, i, 0);
                break;
            }
        }
    }
}