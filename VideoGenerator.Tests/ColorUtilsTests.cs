using System.Diagnostics;
using System.Drawing;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Media.Imaging;

using VideoGenerator.Utils.Extensions;

using NewColor = System.Windows.Media.Color;
using NewColors = System.Windows.Media.Colors;
using OldColor = System.Drawing.Color;

namespace VideoGenerator.Tests;

public static class AssertEx
{
    public static void AreEqual (ColorRecord expected, ColorRecord actual, string? message)
    {
        if (message.IsNullOrEmpty())
        {
            Assert.AreEqual(expected, actual);
        }
        else
        {
            Assert.AreEqual(expected, actual, message);
        }
    }

    public static void AreEqual (OldColor expected, NewColor actual, string? message = null)
    {
        AreEqual(expected.ToRecord(), actual.ToRecord(), message);
    }

    public static void AreEqual (NewColor expected, OldColor actual, string? message = null)
    {
        AreEqual(expected.ToRecord(), actual.ToRecord(), message);
    }

    public static void AreEqual (OldColor expected, OldColor actual, string? message = null)
    {
        AreEqual(expected.ToRecord(), actual.ToRecord(), message);
    }

    public static void AreEqual (NewColor expected, NewColor actual, string? message = null)
    {
        AreEqual(expected.ToRecord(), actual.ToRecord(), message);
    }

    public static void AreEqual (Bitmap expected, WriteableBitmap actual, bool compareTransparent = false, string? imageName = null)
    {
        Assert.AreEqual(expected.Width, actual.Width);
        Assert.AreEqual(expected.Height, actual.Height);
        for (int x = 0; x < actual.Width; x++)
        {
            for (int y = 0; y < actual.Height; y++)
            {
                var expectedPixel = expected.GetPixel(x, y).ToRecord();
                var actualPixel = actual.GetPixel(x, y).ToRecord();
                bool bothTransparent = expectedPixel.A == 0 && actualPixel.A == 0;
                if (compareTransparent || !bothTransparent)
                    AreEqual(expectedPixel, actualPixel, $"\nPixel ({x},{y}) in {imageName}");
            }
        }
    }
}

[TestClass]
public class ColorUtilsTests
{
    private static readonly (OldColor, NewColor)[] pairs =
    [
        (OldColor.Black, NewColors.Black),
        (OldColor.Red, NewColors.Red),
        (OldColor.Blue, NewColors.Blue),
        (OldColor.Green, NewColors.Green),
        (OldColor.Yellow, NewColors.Yellow),
        (OldColor.White, NewColors.White),
        (OldColor.Gray, NewColors.Gray),
        (OldColor.Orange, NewColors.Orange),
        (OldColor.Brown, NewColors.Brown),
        (OldColor.Pink, NewColors.Pink),
        (OldColor.Purple, NewColors.Purple),
    ];

    [TestMethod]
    public void ConvertForward ()
    {
        foreach (var colorPair in pairs)
        {
            var converted = colorPair.Item1.Convert();
            AssertEx.AreEqual(colorPair.Item2, converted);
        }
    }

    [TestMethod]
    public void ConvertBack ()
    {
        foreach (var colorPair in pairs)
        {
            var converted = colorPair.Item2.Convert();
            AssertEx.AreEqual(colorPair.Item1, converted);
        }
    }

    [TestMethod]
    public void DoubleConvertBidirectional ()
    {
        foreach (var colorPair in pairs)
        {
            var converted = colorPair.Item1.Convert().Convert();
            AssertEx.AreEqual(colorPair.Item1, converted);

            var converted2 = colorPair.Item2.Convert().Convert();
            AssertEx.AreEqual(colorPair.Item2, converted2);
        }
    }

    [DataTestMethod]
    [DataRow("./uv_test.png")]
    //[DataRow(@"C:\Users\TheMasonX/Pictures/Melody Sword and Shield Cutout Color.png")]
    public void ToWriteableBitmap_ComparePixels (string path)
    {
        TestImage(path, 0);
    }


    //[TestMethod]
    //public async Task ToWriteableBitmap_ComparePixels_UserPicturesAsync ()
    //{
    //    var files = Directory.GetFiles($"{Environment.GetEnvironmentVariable("UserProfile")}/Pictures/").Where(f => RegexEx.ImageRegex.IsMatch(f)).ToArray();
    //    for (int i = 0; i < files.Length; i++)
    //    {
    //        string filePath = files[i];
    //        Trace.WriteLine($"#{i} Starting: {filePath}");
    //        TestImage(filePath, i);
    //        Trace.WriteLine($"#{i} Finished: {filePath}");
    //        await Task.Delay(1);
    //    }
    //}

    public static void TestImage (string filePath, int index)
    {
        using Image bitmap = Image.FromFile(filePath);
        WriteableBitmap? writeableBitmap = bitmap.ToWriteableBitmap();
        Assert.IsNotNull(writeableBitmap);
        AssertEx.AreEqual((Bitmap)bitmap, writeableBitmap, false, $"#{index}: {filePath}");
    }
}