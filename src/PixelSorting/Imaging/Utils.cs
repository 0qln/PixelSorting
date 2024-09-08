using System.Drawing;
using System.Drawing.Imaging;
using Sorting;
using TestDataGenerator;

namespace Imaging;

public class Utils
{

#pragma warning disable CA1416 // Validate platform compatibility


    static int[] checks = new int[1920 * 1080];

    public static void VisualizeOverlap(double alpha)
    {
        var tanAlpha = Math.Tan(alpha);
        var imageHeight = 1080;
        var imageWidth = 1920;
        Array.Clear(checks, 0, checks.Length);

        void DoRun(double stepU, double stepV, double offU, double offV)
        {
            Sorter<int>.PixelSpan2D span = new(checks, imageWidth, imageHeight, stepU, stepV, offU, offV);
            for (int i = 0; i < span.ItemCount; i++)
            {
                span[i] = Math.Min(255, span[i] + 20);
            }

            // Save(checks, imageWidth, imageHeight, $"overlap\\{alpha}-{stepU}-{stepV}-{offU}-{offV}-overlap.bmp", pixel => (255, (byte)(pixel>>0), (byte)(pixel>>0), (byte)pixel));
        }

        void DoRunNew(double stepU, double stepV, int offU, int offV)
        {
            Sorter<int>.PixelSpan2DRun span = new(ref checks[0], imageWidth, imageHeight, stepU, stepV, offU, offV);
            for (int i = 0; i < span.ItemCount; i++)
            {
                span[i] = Math.Min(255, span[i] + 20);
            }
        }

        switch (alpha)
        {
            case 0:

                // top
                for (var i = 0; i < imageHeight; i++)
                    DoRunNew(1, 0, 0, i);

                break;

            case < Math.PI / 4:
                
                // left
                for (var i = 0; i < (int)(imageHeight * tanAlpha); i++)
                    DoRunNew(tanAlpha, 1, -i, 0);

                // top
                for (var i = 1; i < imageWidth; i++)
                    DoRunNew(tanAlpha, 1, i, 0);

                break;

            case Math.PI / 4:

                // left
                for (var i = 0; i < imageHeight; i++)
                    DoRunNew(1, 1, -i, 0);

                // top
                for (var i = 1; i < imageWidth; i++)
                    DoRunNew(1, 1, i, 0);

                break;

            case < Math.PI / 2:
                
                // left
                for (var i = 0; i < imageHeight; i++)
                    DoRunNew(1, 1 / tanAlpha, 0, i);
                
                // top
                for (var i = 1; i < (int)(imageWidth / tanAlpha); i++)
                    DoRunNew(1, 1 / tanAlpha, 0, -i);

                break;

            case Math.PI / 2:

                // left
                for (var i = 0; i < imageHeight; i++)
                    DoRunNew(1, 0, 0, i);

                break;

            case Math.PI / 2 + Math.PI / 4:

                // right
                for (var i = 0; i < imageHeight; i++)
                    DoRunNew(-1, 1, imageWidth - 1, i);

                // top
                for (var i = 1; i < (int)(imageWidth / -tanAlpha); i++)
                    DoRunNew(-1, 1, imageWidth - 1, -i);

                break;

            case < Math.PI / 2 + Math.PI / 4:

                // right
                for (var i = 0; i < imageHeight; i++)
                    DoRunNew(-1, 1 / -tanAlpha, imageWidth - 1, i);

                // top
                for (var i = 1; i < (int)(imageWidth / -tanAlpha); i++)
                    DoRunNew(-1, 1 / -tanAlpha, imageWidth - 1, -i);

                break;

            case < Math.PI:

                // right 
                for (var i = 0; i < (int)(imageHeight * -tanAlpha); i++)
                    DoRunNew(tanAlpha, 1, i + imageWidth - 1, 0);

                // top
                for (var i = 1; i < imageWidth; i++)
                    DoRunNew(tanAlpha, 1, -i + imageWidth - 1, 0);

                break;

            case Math.PI:

                // top
                for (var i = 0; i < imageWidth; i++)
                    DoRunNew(0, 1, i, 0);

                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(alpha));
        }

        Save(checks, imageWidth, imageHeight, $"overlap\\{alpha}-overlap-new.bmp", pixel => (255, (byte)(pixel>>0), (byte)(pixel>>0), (byte)pixel));

        // switch (alpha)
        // {
        //     case 0:
        //     {
        //         for (var i = 0; i < imageWidth; i++)
        //             DoRun(0, 1, i, 0);
        //         break;
        //     }
        //     case > 0 and < Math.PI / 2:
        //     {
        //                 Console.WriteLine(alpha);
        //                 Console.WriteLine(tanAlpha);
        //         Console.WriteLine("LEFT");
        //         // left
        //         // for (var i = 0; i < imageHeight - 1; i++)
        //         //     DoRun(1, 1 / tanAlpha, 0, i);
        //
        //         Console.WriteLine("TOP");
        //
        //         // top
        //         if (alpha > Math.PI / 4)
        //         {
        //             for (double i = 0; i < imageWidth; i += tanAlpha)
        //                 DoRun(1, 1 / tanAlpha, i, 0);
        //             //     var b = imageWidth / tanAlpha;
        //             // for (var i = 0; i < Math.Min(b, imageHeight) - 1; i++)
        //             //     DoRun(-1, -1 / tanAlpha, imageWidth - 1, i);
        //         }
        //         else
        //         {
        //             for (var i = 0; i < imageWidth; i++)
        //                 DoRun(tanAlpha, 1, i, 0);
        //         }
        //
        //         break;
        //     }
        //     case Math.PI / 2:
        //     {
        //         for (var i = 0; i < imageHeight; i++)
        //             DoRun(1, 0, 0, i);
        //         break;
        //     }
        //     case > Math.PI / 2 and < Math.PI:
        //     {
        //         // top
        //         for (var i = 0; i < imageWidth; i++)
        //         {
        //             DoRun(tanAlpha, 1, i, 0);
        //         }
        //
        //         // right
        //         if (alpha > Math.PI / 2 + Math.PI / 4)
        //         {
        //             for (var i = 0; i < imageHeight; i++)
        //                 DoRun(tanAlpha, 1, imageWidth - 1, i);
        //         }
        //         else
        //         {
        //             for (var i = 0; i < imageHeight; i++)
        //                 DoRun(-1, -1 / tanAlpha, imageWidth - 1, i);
        //         }
        //
        //         break;
        //     }
        //     case Math.PI:
        //     {
        //         for (var i = 0; i < imageWidth; i++)
        //             DoRun(0, 1, i, 0);
        //         break;
        //     }
        // }
    }
        
        
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

    public static void Save<TPixel>(TPixel[] pixels, int width, int height, string path, PixelExtractor<TPixel> pixelExtractor)
    {
        var bmp = new Bitmap(width, height);

        for (var x = 0; x < width; x++)
        {
            for (var y = 0; y < height; y++)
            {
                var p = pixelExtractor(pixels[y * width + x]);
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