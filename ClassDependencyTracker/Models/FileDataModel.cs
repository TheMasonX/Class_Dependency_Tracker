using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;

using CommunityToolkit.Mvvm.ComponentModel;

using Serilog;

using ClassDependencyTracker.Utils.Extensions;

namespace ClassDependencyTracker.Models;

public interface IFileData<T>
{
    string FileExtensions { get; }
    string FileFilter { get; }

    string FilePath { get; }
    string Name { get; }
    FileInfo? Info { get; }
    ulong Size { get; }
    string SizeFormatted { get; }
    T? Data { get; }
    T? GetData ();
    bool SaveData (string outputPath);
}

public abstract partial class FileDataModel<T> : ObservableObject, IFileData<T>, IDisposable
{
    protected FileDataModel(FileInfo fileInfo)
    {
        Info = new FileInfo(FilePath);
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

    [ObservableProperty]
    private FileInfo? _info;
    partial void OnInfoChanged(FileInfo? value)
    {
        if (value is null)
            return;

        FilePath = value.FullName;
        Name = value.Name;
        Size = (ulong)value.Length;
    }

    [ObservableProperty]
    private ulong _size;

    public string SizeFormatted => FileUtils.FormatFileSize(Size);

    public virtual string FileExtensions => "";
    public virtual string FileFilter => FileUtils.AllFilesFilter;

    #endregion Properties

    #region Public Methods

    public abstract T? GetData ();
    public abstract bool SaveData (string outputPath);

    #endregion Public Methods

    #region Private Methods

    protected abstract void LoadData ();
    

    #endregion Private Methods
}

public class SQLData : FileDataModel<string>
{
    private string _data = "";

    public SQLData(string filePath) : base(new FileInfo(filePath)) { }

    public override void Dispose ()
    {
        GC.SuppressFinalize(this);
    }

    #region Properties

    #endregion Properties

    #region Public Methods

    public override string? GetData()
    {
        if (_data is null)
        {
            LoadData();
        }

        return _data;
    }

    public override bool SaveData(string outputPath)
    {
        if (_data is null || outputPath.IsNullOrEmpty())
        {
            return false;
        }

        try
        {
            File.WriteAllText(outputPath, _data);
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
        try
        {
            _data = File.ReadAllText(Info!.FullName);
        }
        catch (Exception ex)
        {
            Trace.WriteLine($"Loading {Name} from {FilePath} threw error: {ex}");
        }
    }

    #endregion Private Methods
}