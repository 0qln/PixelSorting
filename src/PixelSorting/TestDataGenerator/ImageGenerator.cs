namespace TestDataGenerator
{
    public class ImageGenerator
    {
        //public delegate TPixel FromCardesian<TPixel>(int x, int y, int width, int height);

        //public delegate TPixel PixelValueTransform<TPixel>(TPixel pixel);

        //public static FromCardesian<TPixel> RadialCorner<TPixel>(int x, int y, int width, int height)
        //{
        //    double percentageX = Math.Abs(x - width) / (double)width;
        //    double percentageY = Math.Abs(y - height) / (double)height;

        //    double percentageCenter = Math.Clamp(Math.Sqrt(Math.Pow(percentageX, 2) + Math.Pow(percentageY, 2)), 0, 1);

        //    byte value =(byte)(255 * percentageCenter);

        //    return new TPixel(value, value, value);
        //}

        //public static FromCardesian RadialCenter = (int x, int y, int width, int height) =>
        //{
        //    int centerX = width / 2;
        //    int centerY = height / 2;

        //    double percentageX = Math.Abs(x - centerX) / (double)centerX;
        //    double percentageY = Math.Abs(y - centerY) / (double)centerY;

        //    double percentageCenter = Math.Clamp(Math.Sqrt(Math.Pow(percentageX, 2) + Math.Pow(percentageY, 2)), 0, 1);

        //    byte value = (byte)(255 * percentageCenter);

        //    return new Pixel(value, value, value);
        //};

        //public static PixelValueTransform Invert = (Pixel p) => new ((byte)(255 - p.R), (byte)(255 - p.G), (byte)(255 - p.B));

        //public static void Apply(Pixel[,] pixels, FromCardesian f)
        //{
        //    for (int x = 0; x < pixels.GetLength(0); x++)
        //    {
        //        for (int y = 0; y < pixels.GetLength(1); y++)
        //        {
        //            pixels[x, y] = f(x, y, pixels.GetLength(0), pixels.GetLength(1));
        //        }
        //    }
        //}

        //public static void Apply(Pixel[,] pixels, PixelValueTransform f)
        //{
        //    for (int x = 0; x < pixels.GetLength(0); x++)
        //    {
        //        for (int y = 0; y < pixels.GetLength(1); y++)
        //        {
        //            pixels[x, y] = f(pixels[x, y]);
        //        }
        //    }
        //}
    }
}
