using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Data;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using Serilog;

using VideoGenerator.Models;
using VideoGenerator.Utils.Extensions;

namespace VideoGenerator.ViewModels;

public partial class FileGridVM : ObservableObject, IDisposable
{
    private readonly object _lock = new();

    public FileGridVM ()
    {

    }

    public void Dispose ()
    {
        if (_imageFiles is not null)
        {
            foreach (var file in _imageFiles)
            {
                file.Dispose();
            }

            _imageFiles.Clear();
        }
        _imageFilesView = null;
        GC.SuppressFinalize(this);
    }



    #region Properties

    private List<ImageData>? _imageFiles;
    public List<ImageData> ImageFiles
    {
        get => _imageFiles ??= [];
        set => SetProperty(ref _imageFiles, value);
    }

    private ImageData? _selectedImageFile;
    public ImageData? SelectedImageFile
    {
        get => _selectedImageFile;
        set => SetProperty(ref _selectedImageFile, value);
    }

    private ListCollectionView? _imageFilesView;
    public ListCollectionView ImageFilesView
    {
        get => _imageFilesView ??= CreateFileListView(ImageFiles);
        //get => _imageFilesView ??= new(ImageFiles);
        set => SetProperty(ref _imageFilesView, value);
    }

    private string? _fileNameFilter;
    public string FileNameFilter
    {
        get => _fileNameFilter ??= "";
        set
        {
            if (SetProperty(ref _fileNameFilter, value) && EnableFileNameFilter && ImageFilesView.CanFilter)
            {
                Refresh();
            }
        }
    }

    private bool _enableFileNameFilter;
    public bool EnableFileNameFilter
    {
        get => _enableFileNameFilter;
        set
        {
            if (SetProperty(ref _enableFileNameFilter, value))
            {
                Refresh();
            }
        }
    }

    public bool EnableFileFilter => EnableFileNameFilter; // In case we need more filter options than just the name

    public int Count => ImageFiles?.Count ?? 0;
    public int VisibleCount => ImageFilesView?.Count ?? 0;

    private bool _isFilterOpen;
    public bool IsFilterOpen
    {
        get => _isFilterOpen;
        set => SetProperty(ref _isFilterOpen, value);
    }

    #endregion Properties

    #region Commands

    [RelayCommand]
    public void ToggleFilenameFilter ()
    {
        EnableFileNameFilter = !EnableFileNameFilter;
    }

    #endregion Commands

    #region Public Methods

    public bool OpenFile (string? file)
    {
        if (file.IsNullOrEmpty())
        {
            return false;
        }

        var sw = Stopwatch.StartNew();
        ImageData data = new(file!);
        sw.Stop();
        Log.Information("Opened {File} in {Elapsed}ms", file, sw.ElapsedMilliseconds);
        lock (_lock)
        {
            ImageFiles.Add(data);
        }
        RefreshCounts();
        return true;
    }

    public void Refresh ()
    {
        //ImageFilesView.Refresh();
        ImageFilesView.Dispatcher.Invoke(ImageFilesView.Refresh);
        OnPropertyChanged(nameof(EnableFileFilter));
        OnPropertyChanged(nameof(VisibleCount));
    }

    public void RefreshCounts ()
    {
        OnPropertyChanged(nameof(Count));
        OnPropertyChanged(nameof(VisibleCount));
    }

    #endregion Public Methods

    #region Private Methods

    private ListCollectionView CreateFileListView<T> (List<T> files)
    {
        if (Application.Current is null)
        {
            return new ListCollectionView(files); //In case this is being run outside the application like during benchmarking
        }

        return Application.Current.Dispatcher.Invoke(() =>
        {
            var view = new ListCollectionView(files) { Filter = FilterImageFileNames };
            BindingOperations.EnableCollectionSynchronization(view, _lock);
            return view;
        });
    }

    private bool FilterImageFileNames (object file)
    {
        if (!EnableFileNameFilter || FileNameFilter.IsNullOrEmpty())
        {
            return true;
        }

        if (file is not ImageData imageData)
        {
            return false;
        }

        return imageData.Name.Contains(FileNameFilter, StringComparison.CurrentCultureIgnoreCase);
    }

    #endregion Private Methods
}