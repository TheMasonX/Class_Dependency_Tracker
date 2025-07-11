using System.Drawing;

using VideoGenerator.Models;
using VideoGenerator.ViewModels;

namespace VideoGenerator.Tests;

[TestClass]
public class FileModelTests
{
    [DataTestMethod]
    [DataRow(@".\uv_test.png", 4096, 4096, DisplayName = "UV_Test")]
    public void ImageModelTest (string imagePath, int width, int height)
    {
        using ImageData imageData = new(imagePath);
        Image? image = imageData.GetData();
        FileInfo fileInfo = new(imagePath);

        Assert.IsNotNull(image);
        Assert.AreEqual(fileInfo.Name, imageData.Name);
        Assert.AreEqual(fileInfo.FullName, imageData.Info!.FullName);
        Assert.AreEqual(width, image.Width);
        Assert.AreEqual(height, image.Height);
    }

    //[DataTestMethod]
    //[DataRow(@".\uv_test.png", 4096, 4096, DisplayName = "ImageEditorTest")]
    //public void ImageEditorTest (string imagePath, int width, int height)
    //{
    //    ImageEditorVM editorVM = new();
    //    editorVM.OpenImage(new(imagePath));
    //    Assert.AreEqual(width, editorVM.Bitmap!.PixelWidth);
    //    Assert.AreEqual(height, editorVM.Bitmap!.PixelHeight);
    //}
}