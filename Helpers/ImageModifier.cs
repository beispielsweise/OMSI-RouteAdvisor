using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using Color = System.Windows.Media.Color;
using System.Windows;
using System.Windows.Media;


namespace OMSI_RouteAdvisor.Helpers
{
    /// <summary>
    /// Modifies BitmapImage in various ways
    /// </summary>
    public class ImageModifier
    {
        // Pre-defined background image colors that needs to be filtered out
        private static readonly Color _greenTrees = Color.FromRgb(102, 189, 137);
        private static readonly Color _greyBG = Color.FromRgb(230, 230, 230);
        private static readonly Color _blackRR = Color.FromRgb(128, 128, 128);
        private static readonly Color _blueWater = Color.FromRgb(172, 196, 236);
        private static readonly Color _greyRoadOutline = Color.FromRgb(180, 181, 181);
        private static readonly int _tolerance = 10;

        /// <summary>
        /// Filters out unnecessary colors on the background map image, making it transparent
        /// </summary>
        /// <param name="source">Source image</param>
        /// <returns>Image without background noises</returns>
        public static BitmapSource MakeTransparent(BitmapSource source)
        {
            var formatConverted = new FormatConvertedBitmap(source, PixelFormats.Bgra32, null, 0);

            int stride = formatConverted.PixelWidth * 4;
            byte[] pixelData = new byte[stride * formatConverted.PixelHeight];
            formatConverted.CopyPixels(pixelData, stride, 0);

            for (int i = 0; i < pixelData.Length; i += 4)
            {
                byte b = pixelData[i];
                byte g = pixelData[i + 1];
                byte r = pixelData[i + 2];
                byte a = pixelData[i + 3];

                if (IsColorMatch(r, g, b, _greenTrees, _tolerance) ||
                    IsColorMatch(r, g, b, _greyBG, _tolerance) ||
                    IsColorMatch(r, g, b, _blackRR, _tolerance) ||
                    IsColorMatch(r, g, b, _blueWater, _tolerance) ||
                    IsColorMatch(r, g, b, _greyRoadOutline, _tolerance))
                {
                    pixelData[i + 3] = 0; // Set alpha to 0 (fully transparent)
                }
            }

            var writableBitmap = new WriteableBitmap(formatConverted);
            writableBitmap.WritePixels(
                new Int32Rect(0, 0, writableBitmap.PixelWidth, writableBitmap.PixelHeight),
                pixelData,
                stride,
                0
            );

            return writableBitmap;
        }

        // Helper function to check color match with tolerance
        private static bool IsColorMatch(byte r, byte g, byte b, Color targetColor, int tolerance)
        {
            return Math.Abs(r - targetColor.R) <= tolerance &&
                   Math.Abs(g - targetColor.G) <= tolerance &&
                   Math.Abs(b - targetColor.B) <= tolerance;
        }
    }
}
