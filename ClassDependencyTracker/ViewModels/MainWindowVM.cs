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
using ClassDependencyTracker.Models.DB;

namespace ClassDependencyTracker.ViewModels;

public partial class MainWindowVM : ObservableObject, IDisposable
{
    public static MainWindowVM Instance { get; private set; } = null!;

    private const string _openFileDialogTitle = "Select Class Dependency Database";
    [GeneratedRegex(FileUtils.DBExtension)]
    public static partial Regex FileExtensionRegex();

    private const string _saveFileDialogTitle = "Save Class Dependency Database";

    public MainWindowVM ()
    {
        Settings.Default.Upgrade();
        Instance = this;
        OpenWith();
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
                CheckFileExists = true,
                Multiselect = false,
                Filter = FileUtils.DBFilter,
            };
            openFileDialog.ShowDialog();
            OpenFile(openFileDialog.FileName);
        });
    }

    #endregion Commands

    #region Public Methods

    #region Open

    public void OpenWith()
    {
        string? file = Environment.GetCommandLineArgs().FirstOrDefault(FileExtensionRegex().IsMatch);
        if (file is not null && File.Exists(file))
            Task.Run(() => OpenFile(file));
        else if (Settings.Default.LoadLast && File.Exists(Settings.Default.LastInputFile))
            OpenFile(Settings.Default.LastInputFile);
    }

    public void OpenFile(string filename)
    {
        Settings.Default.LastInputFile = filename;
        Settings.Default.Save();

        ClassModel[] classes = DBUtils.LoadFromFile(filename);
        Classes.SafeClear();
        foreach (ClassModel cls in classes)
        {
            Classes.SafeAdd(cls);
        }
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
                DefaultExt = FileUtils.DBExtension,
                RestoreDirectory = true,
                Filter = FileUtils.DBFilter,
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
        DBUtils.CreateDB(filePath);
        DBUtils.SaveToFile(filePath, Classes.ToArray());
    }

    #endregion Public Methods

    #region Private Methods

    // Private Methods

    #endregion Private Methods
}