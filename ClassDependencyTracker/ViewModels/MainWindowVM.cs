using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using Microsoft.Win32;

using ClassDependencyTracker.Models;
using ClassDependencyTracker.Properties;
using ClassDependencyTracker.Utils.Extensions;
using System.Diagnostics;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace ClassDependencyTracker.ViewModels;

public partial class MainWindowVM : ObservableObject, IDisposable
{
    public static MainWindowVM Instance { get; private set; } = null!;

    private const string _openFileDialogTitle = "Select Images To Convert";
    private const string _inputExtensionFilter = "Image files (*.bmp, *.jpg, *.png)|*.bmp;*.jpg;*.png|All files (*.*)|*.*";
    [GeneratedRegex(@"(\.bmp)|(\.jpg)|(\.png)")]
    private static partial Regex GetInputExtensionRegex ();
    public readonly Regex InputExtensionRegex = GetInputExtensionRegex();

    private const string _saveFileDialogTitle = "Save Video FileUtils";
    private const string _outputExtensionFilter = "Video FileUtils (*.mp4)|*.mp4|All files (*.*)|*.*";
    private const string _outputExtension = ".mp4";

    public MainWindowVM ()
    {
        Instance = this;
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }


    #region Properties

    private string? _outputFileName;
    public string OutputFileName
    {
        get => _outputFileName ??= "";
        set
        {
            if (SetProperty(ref _outputFileName, value))
            {
                Settings.Default.OutputFileName = value;
                Settings.Default.Save();
            }
        }
    }

    private string? _outputFilePath;
    public string OutputFilePath
    {
        get => _outputFilePath ??= "";
        set
        {
            if (SetProperty(ref _outputFilePath, value))
            {
                Settings.Default.OutputFilePath = value;
                Settings.Default.Save();
            }
        }
    }

    public ObservableCollection<ClassModel> Classes { get; } = DispatcherUtils.CreateObservableCollection<ClassModel>();

    [ObservableProperty]
    private IAppStatus? _status;

    #endregion Properties

    #region Commands

    [RelayCommand]
    public Task OnOpenFiles ()
    {
        return Task.Run(() =>
        {
            OpenFileDialog openFileDialog = new()
            {
                Title = _openFileDialogTitle,
                RestoreDirectory = true,
                //CheckFileExists = true,
                Multiselect = true,
                Filter = _inputExtensionFilter,
            };
            openFileDialog.ShowDialog();
            OpenFiles(openFileDialog.FileNames);
        });
    }

    #endregion Commands

    #region Public Methods

    #region Open

    public void OpenFiles (IEnumerable<string>? files)
    {
        files = files?.Where(f => InputExtensionRegex.IsMatch(f));
        if (files is null || !files.Any())
        {
            return;
        }

        int addCount = files.Count();
        //Status = new LoadingStatus(FileGrid.Count, FileGrid.Count + addCount, "Item", "Items");

        //Task.Run(() =>
        //{
        //    foreach (string file in files)
        //    {
        //        if (file.IsNullOrEmpty() || !InputExtensionRegex.IsMatch(file!))
        //        {
        //            continue;
        //        }

        //        if (FileGrid.OpenFile(file))
        //        {
        //            Status.Update(1);
        //        }
        //        else
        //        {
        //            Status = new TextStatus() { Status = $"Load Error for {file}", BGColor = Brushes.Red };
        //        }
        //    }
        //    Status.Hide();
        //    FileGrid.Refresh();
        //});
    }

    #endregion Open

    [RelayCommand]
    public Task Save ()
    {
        if (OutputFilePath.IsNullOrEmpty()) //No path, try save as instead
            return SaveAs();

        return Task.Run(() => SaveToFile(OutputFilePath));
    }

    [RelayCommand]
    public Task SaveAs ()
    {
        return Task.Run(() =>
        {
            SaveFileDialog fileDialog = new()
            {
                Title = _saveFileDialogTitle,
                FileName = OutputFileName,
                OverwritePrompt = true,
                AddExtension = true,
                DefaultExt = _outputExtension,
                RestoreDirectory = true,
                Filter = _outputExtensionFilter,
            };

            if (!Settings.Default.OutputDirectory.IsNullOrEmpty())
                fileDialog.InitialDirectory = Settings.Default.OutputDirectory;

            bool? result = fileDialog.ShowDialog();
            if (!result.HasValue || !result.Value) //Invalid selection
                return;

            OutputFilePath = fileDialog.FileName;

            if (OutputFilePath.IsNullOrEmpty()) //Invalid FilePath
                return;

            SaveToFile(OutputFilePath);
        });
    }

    private void SaveToFile(string filePath)
    {
        if (System.IO.File.Exists(OutputFilePath))
        {
            Trace.WriteLine($"FileUtils Exits at path {filePath}, deleting...");
            System.IO.File.Delete(OutputFilePath);
        }

        Trace.WriteLine("Do something to save");
    }

    #endregion Public Methods

    #region Private Methods

    // Private Methods

    #endregion Private Methods
}