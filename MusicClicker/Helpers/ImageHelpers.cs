/*
 * File: Helpers/ImageHelpers.cs
 * Summary: Utility helpers for image loading and creating smooth Image controls.
 * Purpose: Provides a bitmap cache to avoid repeated decoding and helpers to create Image controls with high-quality rendering.
 */

using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia;
using System;
using System.Collections.Concurrent;
using System.IO;
using System.Threading.Tasks;
using Avalonia.Platform;

namespace MusicClicker.Helpers
{
    // Small helper utilities for creating and configuring Image controls with
    // higher-quality bitmap interpolation and layout rounding to improve visual
    // appearance when images are scaled.
    public static class ImageHelpers
    {
        // Simple in-memory cache to avoid repeatedly decoding the same bitmap assets.
        // Key is the asset URI string (optionally with a decode width suffix).
        private static readonly ConcurrentDictionary<string, Bitmap> _bitmapCache = new();
        
        // Directory on disk to cache raw asset files (copied from avares:// streams).
        private static readonly string _diskCacheDir = Path.Combine(Environment.CurrentDirectory, "Save", "BitmapCache");
        // Maximum disk cache size in bytes (200 MB)
        private const long _maxDiskCacheBytes = 200 * 1024 * 1024;

        /// <summary>
        /// Load a bitmap from an application asset URI and cache it.
        /// Use `avares://` style URIs used across the project.
        /// </summary>
        public static Bitmap? GetBitmap(string assetUri, int decodeWidth = 0)
        {
            if (string.IsNullOrEmpty(assetUri))
                return null;

            // Create a cache key that includes requested decode width for future scaled variants
            string key = decodeWidth > 0 ? assetUri + "|w=" + decodeWidth : assetUri;

            if (_bitmapCache.TryGetValue(key, out var cached))
                return cached;

            try
            {
                // Ensure disk cache directory exists
                try { Directory.CreateDirectory(_diskCacheDir); } catch { }

                // Use a filesystem cache to avoid repeatedly opening asset streams from the app package.
                string cacheFile = Path.Combine(_diskCacheDir, ComputeHash(key) + Path.GetExtension(assetUri).Replace("?", ""));

                if (File.Exists(cacheFile))
                {
                    try
                    {
                        var bmpFromFile = new Bitmap(cacheFile);
                        _bitmapCache[key] = bmpFromFile;
                        return bmpFromFile;
                    }
                    catch
                    {
                        // If loading from file fails, remove it and fall back to asset stream
                        try { File.Delete(cacheFile); } catch { }
                    }
                }

                var uri = new System.Uri(assetUri);
                using var stream = Avalonia.Platform.AssetLoader.Open(uri);
                if (stream != null)
                {
                    // Copy asset stream to disk cache for faster subsequent loads
                    try
                    {
                        using var outFs = File.OpenWrite(cacheFile);
                        stream.Position = 0;
                        stream.CopyTo(outFs);
                    }
                    catch
                    {
                        // ignore disk cache write errors and continue to decode from stream
                    }

                    // Decode bitmap from the (possibly newly written) cache file if present,
                    // otherwise decode directly from the original asset stream.
                    Bitmap bmp;
                    if (File.Exists(cacheFile))
                        bmp = new Bitmap(cacheFile);
                    else
                    {
                        stream.Position = 0;
                        bmp = new Bitmap(stream);
                    }

                    _bitmapCache[key] = bmp;

                    // Maintain cache size under limit in background
                    _ = Task.Run(() => EnsureDiskCacheLimit());

                    return bmp;
                }
            }
            catch { }

            return null;
        }

        // Compute a stable short hash for use as a filename in disk cache
        private static string ComputeHash(string input)
        {
            try
            {
                using var sha = System.Security.Cryptography.SHA256.Create();
                var data = System.Text.Encoding.UTF8.GetBytes(input);
                var hash = sha.ComputeHash(data);
                return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
            }
            catch
            {
                // fallback: sanitize input
                return string.Join("_", input.Split(Path.GetInvalidFileNameChars()));
            }
        }

        // Ensure disk cache size stays under the configured limit by removing oldest files.
        private static void EnsureDiskCacheLimit()
        {
            try
            {
                var dir = new DirectoryInfo(_diskCacheDir);
                if (!dir.Exists) return;

                var files = dir.GetFiles();
                long total = 0;
                foreach (var f in files) total += f.Length;

                if (total <= _maxDiskCacheBytes) return;

                // Order by LastWriteTime ascending (oldest first) and delete until under limit
                Array.Sort(files, (a, b) => a.LastWriteTimeUtc.CompareTo(b.LastWriteTimeUtc));
                foreach (var f in files)
                {
                    try
                    {
                        long before = total;
                        total -= f.Length;
                        f.Delete();
                        if (total <= _maxDiskCacheBytes) break;
                    }
                    catch { }
                }
            }
            catch { }
        }
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
