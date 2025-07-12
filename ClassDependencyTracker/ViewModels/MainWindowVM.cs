using System.IO;
using System.Text.RegularExpressions;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using Microsoft.Win32;

using ClassDependencyTracker.Models;
using ClassDependencyTracker.Properties;
using ClassDependencyTracker.Utils.Extensions;
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
        string? filePath = Environment.GetCommandLineArgs().FirstOrDefault(FileExtensionRegex().IsMatch);
        if (filePath is not null && File.Exists(filePath))
            Task.Run(() => OpenFile(filePath));
        else if (Settings.Default.LoadLast && File.Exists(Settings.Default.LastInputFile))
            OpenFile(Settings.Default.LastInputFile);
    }

    public void OpenFile(string filePath)
    {
        Settings.Default.LastInputFile = filePath;
        Settings.Default.Save();

        Status = new TextStatus($"Opening Database file: {filePath}");
        ClassModel[] classes = DBUtils.LoadFromFile(filePath);
        Classes.SafeClear();
        foreach (ClassModel cls in classes)
        {
            Classes.SafeAdd(cls);
        }
        if (classes.Length > 0)
            Status = TextStatus.Success($"Loaded {classes.Length} classes from database file: {filePath}");
        else
            TextStatus.Error($"Didn't load any classes from database file: {filePath}. See log for details.");
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
        Status = new TextStatus($"Saving to the database file {filePath}");
        DBUtils.CreateDB(filePath);
        ClassModel[] classes = Classes.ToArray();
        bool success = DBUtils.TrySaveToFile(filePath, classes);
        if (success)
            Status = TextStatus.Success($"Saved {classes.Length} classes to the database file: {filePath}");
        else
            Status = TextStatus.Error($"Error saving to the database file: {filePath}. See log for details.");
    }

    #endregion Public Methods

    #region Private Methods

    // Private Methods

    #endregion Private Methods
}