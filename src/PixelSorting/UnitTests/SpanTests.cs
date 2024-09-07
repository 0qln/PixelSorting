﻿using System.Numerics;
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
        var indices = new nint[maxU + maxV + 1];
        var span = new Sorter<Pixel32bitUnion>.PixelSpan2D(
            data, indices, maxU, maxV, 
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
        var indices = new nint[maxU + maxV + 1];
        var span = new Sorter<Pixel32bitUnion>.PixelSpan2D(
            data, indices, maxU, maxV, 
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
        var indices = new nint[3840 + 2060];
        var span = new Sorter<Pixel32bitUnion>.PixelSpan2D(
            data, indices, 3840, 2060, 1.1, 1.6, 0, 0);

        for (uint i = 0; i < span.ItemCount; i++)
            Assert.Equal(span.MapIndex(i), span.LookupIndex(i));
    }
}