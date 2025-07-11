using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;

using CommunityToolkit.Mvvm.ComponentModel;

using Serilog;

using VideoGenerator.Utils.Extensions;

namespace VideoGenerator.Models;

public interface IFileData<T>
{
    string FilePath { get; }
    string Name { get; }
    FileInfo? Info { get; }
    ulong Size { get; }
    string SizeFormatted { get; }
    T? Data { get; }
    T? GetData ();
    bool SaveData (string outputPath);
}

public abstract class FileDataModel<T> : ObservableObject, IFileData<T>, IDisposable
{
    public FileDataModel (string filePath)
    {
        if (filePath.IsNullOrEmpty())
        {
            return;
        }

        //_data = Image.FromFile(filePath);

        FilePath = filePath;
        Info = new(FilePath);
        Name = Info.Name;
        Size = (ulong)(Info?.Length ?? 0);
        LoadData();
    }

    public abstract void Dispose ();


    #region Properties

    public T? Data => GetData();

    private string? _filePath;
    public string FilePath
    {
        get => _filePath ??= "";
        private set => SetProperty(ref _filePath, value);
    }

    private string? _fileName;
    public string Name
    {
        get => _fileName ?? "";
        private set => SetProperty(ref _fileName, value);
    }

    private FileInfo? _fileInfo;
    public FileInfo? Info
    {
        get => _fileInfo;
        private set => SetProperty(ref _fileInfo, value);
    }

    private ulong _size;
    public ulong Size
    {
        get => _size;
        private set => SetProperty(ref _size, value);
    }

    public string SizeFormatted => FormatFileSize();

    #endregion Properties

    #region Public Methods

    public abstract T? GetData ();
    public abstract bool SaveData (string outputPath);

    #endregion Public Methods

    #region Private Methods

    protected abstract void LoadData ();

    private string FormatFileSize ()
    {
        const double kb = 1024;
        const double mb = 1024 * 1024;
        const double gb = 1024 * 1024 * 1024;

        if (Size < kb)
        {
            return $"{Size:N0}B";
        }

        if (Size < mb)
        {
            return $"{(Size / kb):N1}KB";
        }

        if (Size < gb)
        {
            return $"{(Size / mb):N1}MB";
        }

        return $"{(Size / gb):N1}GB";
    }

    #endregion Private Methods
}

public class ImageData : FileDataModel<Image>
{
    private Image? _data;

    public ImageData (string filePath) : base(filePath) { }

    public override void Dispose ()
    {
        _data?.Dispose();
        _data = null;
        GC.SuppressFinalize(this);
    }

    #region Properties

    public int PixelCount => (Data?.Size.Width * Data?.Size.Height) ?? 0;

    #endregion Properties

    #region Public Methods

    public override Image? GetData ()
    {
        if (_data is null)
        {
            LoadData();
        }

        return _data;
    }

    public override bool SaveData (string outputPath)
    {
        if (_data is null || outputPath.IsNullOrEmpty())
        {
            return false;
        }

        try
        {
            _data.Save(outputPath);
            Log.Information("Saved {Name} to {Path}", Name, outputPath);
            return true;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error Saving {Name} to {Path}", Name, outputPath);
            return false;
        }
    }

    #endregion Public Methods

    #region Private Methods

    protected override void LoadData ()
    {
        if (FilePath.IsNullOrEmpty())
        {
            return;
        }

        try
        {
            _data = Image.FromFile(FilePath);
        }
        catch (Exception ex)
        {
            Trace.WriteLine($"Loading {Name} from {FilePath} threw error: {ex}");
        }
    }

    #endregion Private Methods
}

//public class VideoDataModel : FileDataModel<FFMpegCore.FFM>
//{s
//    private FFMpegCore.FFMpeg? _data;

//    public VideoDataModel (string filePath) : base(filePath) { }

//    public override void Dispose ()
//    {
//        _data?();
//        _data = null;
//        GC.SuppressFinalize(this);
//    }

//    #region Public Methods

//    public override Image? GetData ()
//    {
//        return _data;
//    }

//    public override bool SaveData (string outputPath)
//    {
//        if (_data is null || outputPath.IsNullOrEmpty()) return false;

//        try
//        {
//            _data.Save(outputPath);
//            return true;
//        }
//        catch (Exception ex)
//        {
//            Trace.WriteLine($"Saving {Name} to {outputPath} threw error: {ex}");
//            return false;
//        }
//    }

//    #endregion Public Methods

//    #region Private Methods

//    protected override void LoadData ()
//    {
//        if (Info is null) return;

//        try
//        {
//            _data = FFMpeg.
//        }
//        catch (Exception ex)
//        {
//            Trace.WriteLine($"Loading {Name} from {Path} threw error: {ex}");
//        }
//    }

//    #endregion Private Methods
//}