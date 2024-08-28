using System.Drawing;
using System.Drawing.Imaging;
using TestDataGenerator;

namespace Imaging;

public class Utils
{

#pragma warning disable CA1416 // Validate platform compatibility

        
        
    public delegate TPixel PixelCreator<TPixel>(byte A, byte R, byte G, byte B);
    public delegate (byte A, byte R, byte G, byte B) PixelExtractor<TPixel>(TPixel pixel);


    public static void Save<TPixel>(TPixel[,] pixels, string path, PixelExtractor<TPixel> pixelExtractor)
    {
        var bmp = new Bitmap(pixels.GetLength(0), pixels.GetLength(1));

        for (var x = 0; x < pixels.GetLength(0); x++)
        {
            for (var y = 0; y < pixels.GetLength(1); y++)
            {
                var p = pixelExtractor(pixels[x, y]);
                bmp.SetPixel(x, y, Color.FromArgb(p.A, p.R, p.G, p.B));
            }
        }

        bmp.Save(path);
    }
    public static void Load<TPixel>(TPixel[,] pixels, string path, PixelCreator<TPixel> pixelCreator)
        where TPixel : struct
    {
        var bmp = new Bitmap(path);

        for (var x = 0; x < pixels.GetLength(0); x++)
        {
            for (var y = 0; y < pixels.GetLength(1); y++)
            {
                var pixel = bmp.GetPixel(x, y);
                pixels[x, y] = pixelCreator(pixel.A, pixel.R, pixel.G, pixel.B);
            }
        }
    }

    public static TPixel[,] Load<TPixel>(string path, PixelCreator<TPixel> pixelCreator)
        where TPixel : struct
    {
        var bmp = new Bitmap(path);
        var pixels = new TPixel[bmp.Width, bmp.Height];

        for (var x = 0; x < pixels.GetLength(0); x++)
        {
            for (var y = 0; y < pixels.GetLength(1); y++)
            {
                var pixel = bmp.GetPixel(x, y);
                pixels[x, y] = pixelCreator(pixel.A, pixel.R, pixel.G, pixel.B);
            }
        }

        return pixels;
    }

    public static Bitmap GetBitmap(string path) => new(path);

    public static BitmapData ExposeData(Bitmap bmp)
    {
        Rectangle rect = new (0, 0, bmp.Width, bmp.Height);
        return bmp.LockBits(rect, ImageLockMode.ReadWrite, bmp.PixelFormat);
    }


    public static void SaveResizings(string SOURCE, Func<(int, int), string> targetName)
    {
        var sourceBmp = GetBitmap(SOURCE);

        foreach (var size in Generator.CommonImageSizes())
        {
            var resizeBmp = new Bitmap(sourceBmp, new Size(size.Horizontal, size.Vertical));
            resizeBmp.Save(targetName(size));
        }
    }

#pragma warning restore CA1416 // Validate platform compatibility

}