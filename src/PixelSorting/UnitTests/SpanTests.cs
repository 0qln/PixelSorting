using Sorting.Pixels._32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTests
{
    public class SpanTests
    {

        [Theory]

        [InlineData(1920, 1080, 1, 0, 0, 0)]
        [InlineData(1080, 1920, 1, 0, 0, 0)]

        [InlineData(1920, 1080, 1.3, 0.7, 0, 0)]
        [InlineData(1080, 1920, 1.3, 0.7, 0, 0)]

        [InlineData(1920, 1080, 1.3, 0.7, 100, 0)]
        [InlineData(1080, 1920, 1.3, 0.7, 100, 0)]

        [InlineData(1920, 1080, 1.3, 0.7, 0, 100)]
        [InlineData(1080, 1920, 1.3, 0.7, 0, 100)]
        
        [InlineData(1920, 1080, 1.3, 0.7, 100, 100)]
        [InlineData(1080, 1920, 1.3, 0.7, 100, 100)]


        [InlineData(1920, 1080, -1, 0, 0, 0)]
        [InlineData(1080, 1920, -1, 0, 0, 0)]

        [InlineData(1920, 1080, -1.3, 0.7, 0, 0)]
        [InlineData(1080, 1920, -1.3, 0.7, 0, 0)]

        [InlineData(1920, 1080, -1.3, 0.7, 100, 0)]
        [InlineData(1080, 1920, -1.3, 0.7, 100, 0)]

        [InlineData(1920, 1080, -1.3, 0.7, 0, 100)]
        [InlineData(1080, 1920, -1.3, 0.7, 0, 100)]

        [InlineData(1920, 1080, -1.3, 0.7, 100, 100)]
        [InlineData(1080, 1920, -1.3, 0.7, 100, 100)]


        [InlineData(1920, 1080, 1.3, -0.7, 0, 0)]
        [InlineData(1080, 1920, 1.3, -0.7, 0, 0)]

        [InlineData(1920, 1080, 1.3, -0.7, 100, 0)]
        [InlineData(1080, 1920, 1.3, -0.7, 100, 0)]

        [InlineData(1920, 1080, 1.3, -0.7, 0, 100)]
        [InlineData(1080, 1920, 1.3, -0.7, 0, 100)]

        [InlineData(1920, 1080, 1.3, -0.7, 100, 100)]
        [InlineData(1080, 1920, 1.3, -0.7, 100, 100)]


        [InlineData(1920, 1080, -1.3, -0.7, 0, 0)]
        [InlineData(1080, 1920, -1.3, -0.7, 0, 0)]

        [InlineData(1920, 1080, -1.3, -0.7, 100, 0)]
        [InlineData(1080, 1920, -1.3, -0.7, 100, 0)]

        [InlineData(1920, 1080, -1.3, -0.7, 0, 100)]
        [InlineData(1080, 1920, -1.3, -0.7, 0, 100)]

        [InlineData(1920, 1080, -1.3, -0.7, 100, 100)]
        [InlineData(1080, 1920, -1.3, -0.7, 100, 100)]

        public void PixelSpan2D_FastEstimateItemCount(
            int maxU, int maxV, double stepU, double stepV, int offU, int offV)
        {
            var data = new Pixel32bitUnion[maxU * maxV];
            var indeces = new nint[maxU + maxV];
            var span = new Sorter<Pixel32bitUnion>.PixelSpan2D(
                data, indeces, maxU, maxV, stepU, stepV, offU, offV);

            Assert.Equal(span.EstimateItemCount(), span.FastEstimateItemCount());
        }

    }
}
