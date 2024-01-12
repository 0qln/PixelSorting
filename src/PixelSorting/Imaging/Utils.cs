using Sorting;
using System.Drawing;

namespace Imaging
{
    public class Utils
    {
#pragma warning disable CA1416 // Validate platform compatibility
        public static void Safe(Pixel_24bit[,] pixels, string path)
        {
            Bitmap bmp = new Bitmap(pixels.GetLength(0), pixels.GetLength(1));

            for (int x = 0; x < pixels.GetLength(0); x++)
            {
                for (int y = 0; y < pixels.GetLength(1); y++)
                {
                    Pixel_24bit p = pixels[x, y];
                    bmp.SetPixel(x, y, Color.FromArgb(255, p.R, p.G, p.B));
                }
            }

            bmp.Save(path);
        }
#pragma warning restore CA1416 // Validate platform compatibility
    }
}
