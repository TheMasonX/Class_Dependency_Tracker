using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

using NewColor = System.Windows.Media.Color;
using NewPixelFormat = System.Windows.Media.PixelFormat;
using NewPixelFormats = System.Windows.Media.PixelFormats;
using OldColor = System.Drawing.Color;
using OldPixelFormat = System.Drawing.Imaging.PixelFormat;

namespace VideoGenerator.Utils.Extensions;

public record ColorRecord (int R, int G, int B, int A);

public static class ColorUtils
{
    public static ColorRecord ToRecord (this OldColor color)
    {
        return new ColorRecord(color.R, color.G, color.B, color.A);
    }

    public static ColorRecord ToRecord (this NewColor color)
    {
        return new ColorRecord(color.R, color.G, color.B, color.A);
    }

    public static NewColor Convert (this OldColor color)
    {
        NewColor ret = new()
        {
            R = color.R,
            G = color.G,
            B = color.B,
            A = color.A,
        };
        return ret;
    }

    public static OldColor Convert (this NewColor color)
    {
        OldColor ret = OldColor.FromArgb(color.A, color.R, color.G, color.B);
        return ret;
    }

    public static NewPixelFormat Convert (this OldPixelFormat format)
    {
        return format switch
        {
            OldPixelFormat.Format8bppIndexed => NewPixelFormats.Indexed8,
            OldPixelFormat.Format24bppRgb => NewPixelFormats.Rgb24,
            OldPixelFormat.Format32bppArgb => NewPixelFormats.Bgra32,
            _ => NewPixelFormats.Default,
        };
    }

    public static OldPixelFormat Convert (this NewPixelFormat format)
    {
        if (format == NewPixelFormats.Rgb24)
        {
            return OldPixelFormat.Format24bppRgb;
        }

        if (format == NewPixelFormats.Indexed8)
        {
            return OldPixelFormat.Format8bppIndexed;
        }

        return OldPixelFormat.DontCare;
    }

    public static BitmapPalette? Convert (this ColorPalette? palette)
    {
        var colors = palette?.Entries?.Select(c => c.Convert())?.ToList();
        if (colors is null || colors.Count <= 0 || colors.Count > 256)
        {
            return null;
        }

        return new BitmapPalette(colors);
    }

    public static WriteableBitmap? ToWriteableBitmap (this Image? image)
    {
        if (image is null || image.Width <= 0 || image.Height <= 0)
        {
            return null;
        }

        if (image is not Bitmap bitmap)
        {
            Debug.Fail($"Image {image} was not bitmap");
            return null;
        }

        BitmapSource bitmapSource = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(bitmap.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
        return new WriteableBitmap(bitmapSource);
    }
}
