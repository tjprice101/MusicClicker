using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia;

namespace MusicClicker.Helpers
{
    // Small helper utilities for creating and configuring Image controls with
    // higher-quality bitmap interpolation and layout rounding to improve visual
    // appearance when images are scaled.
    public static class ImageHelpers
    {
        // Create an Image control configured for smooth scaling.
        public static Image CreateSmoothImage(Bitmap? source, double width, double height, string? tag = null, double opacity = 1.0, bool isEnabled = true)
        {
            var img = new Image
            {
                Source = source,
                Width = width,
                Height = height,
                Stretch = Stretch.UniformToFill,
                Margin = new Thickness(5),
                Tag = tag,
                Opacity = opacity,
                IsEnabled = isEnabled,
                UseLayoutRounding = true
            };

            // Set the bitmap interpolation mode to high quality where available.
            try
            {
                RenderOptions.SetBitmapInterpolationMode(img, BitmapInterpolationMode.HighQuality);
            }
            catch
            {
                // Some older Avalonia versions may not expose this API; silently ignore.
            }

            return img;
        }

        // Apply high-quality rendering settings to an existing Image control.
        public static void ApplyHighQuality(Image img)
        {
            if (img == null) return;
            img.UseLayoutRounding = true;
            try
            {
                RenderOptions.SetBitmapInterpolationMode(img, BitmapInterpolationMode.HighQuality);
            }
            catch
            {
                // ignore if API not present
            }
        }
    }
}
