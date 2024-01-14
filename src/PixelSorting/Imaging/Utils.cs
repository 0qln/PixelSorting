using Sorting;
using System.Drawing;
using System.Drawing.Imaging;
using System.Reflection;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Imaging
{
    public class Utils
    {

#pragma warning disable CA1416 // Validate platform compatibility

        public static void Save(Pixel[,] pixels, string path)
        {
            Bitmap bmp = new Bitmap(pixels.GetLength(0), pixels.GetLength(1));

            for (int x = 0; x < pixels.GetLength(0); x++)
            {
                for (int y = 0; y < pixels.GetLength(1); y++)
                {
                    Pixel p = pixels[x, y];
                    bmp.SetPixel(x, y, Color.FromArgb(255, p.R, p.G, p.B));
                }
            }

            bmp.Save(path);
        }


        public static void Load(Pixel[,] pixels, string path)
        {
            Bitmap bmp = new Bitmap(path);

            for (int x = 0; x < pixels.GetLength(0); x++)
            {
                for (int y = 0; y < pixels.GetLength(1); y++)
                {
                    pixels[x, y].R = bmp.GetPixel(x, y).R;
                    pixels[x, y].G = bmp.GetPixel(x, y).G;
                    pixels[x, y].B = bmp.GetPixel(x, y).B;
                }
            }
        }
        
        
        public delegate TPixel PixelCreator<TPixel>(byte A, byte R, byte G, byte B);
        public delegate (byte A, byte R, byte G, byte B) PixelExtractor<TPixel>(TPixel pixel);


        public static void Save<TPixel>(TPixel[,] pixels, string path, PixelExtractor<TPixel> pixelExtractor)
        {
            Bitmap bmp = new Bitmap(pixels.GetLength(0), pixels.GetLength(1));

            for (int x = 0; x < pixels.GetLength(0); x++)
            {
                for (int y = 0; y < pixels.GetLength(1); y++)
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
            Bitmap bmp = new Bitmap(path);

            for (int x = 0; x < pixels.GetLength(0); x++)
            {
                for (int y = 0; y < pixels.GetLength(1); y++)
                {
                    var pixel = bmp.GetPixel(x, y);
                    pixels[x, y] = pixelCreator(pixel.A, pixel.R, pixel.G, pixel.B);
                }
            }
        }

        public static TPixel[,] Load<TPixel>(string path, PixelCreator<TPixel> pixelCreator)
            where TPixel : struct
        {
            Bitmap bmp = new Bitmap(path);
            TPixel[,] pixels = new TPixel[bmp.Width, bmp.Height];

            for (int x = 0; x < pixels.GetLength(0); x++)
            {
                for (int y = 0; y < pixels.GetLength(1); y++)
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

#pragma warning restore CA1416 // Validate platform compatibility

    }
}
