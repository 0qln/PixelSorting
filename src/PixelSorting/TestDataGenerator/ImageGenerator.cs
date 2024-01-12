using Sorting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestDataGenerator
{
    public class ImageGenerator
    {
        public delegate Pixel_24bit CardesianToPixel(int x, int y, int width, int height);

        public delegate Pixel_24bit PixelToPixel(Pixel_24bit pixel);

        public static CardesianToPixel Radial = (int x, int y, int width, int height) =>
        {
            int centerX = width / 2;
            int centerY = height / 2;

            double percentageY = Math.Abs(y - centerY) / (double)centerY;
            double percentageX = Math.Abs(x - centerX) / (double)centerX;

            double percentageCenter = Math.Clamp(Math.Sqrt(Math.Pow(percentageX, 2) + Math.Pow(percentageY, 2)), 0, 1);

            byte value =(byte)(255 * percentageCenter);

            return new Pixel_24bit(value, value, value);
        };

        public static PixelToPixel Invert = (Pixel_24bit p) => new ((byte)(255 - p.R), (byte)(255 - p.G), (byte)(255 - p.B));

        public static void Apply(Pixel_24bit[,] pixels, CardesianToPixel f)
        {
            for (int x = 0; x < pixels.GetLength(0); x++)
            {
                for (int y = 0; y < pixels.GetLength(1); y++)
                {
                    pixels[x, y] = f(x, y, pixels.GetLength(0), pixels.GetLength(1));
                }
            }
        }

        public static void Apply(Pixel_24bit[,] pixels, PixelToPixel f)
        {
            for (int x = 0; x < pixels.GetLength(0); x++)
            {
                for (int y = 0; y < pixels.GetLength(1); y++)
                {
                    pixels[x, y] = f(pixels[x, y]);
                }
            }
        }
    }
}
