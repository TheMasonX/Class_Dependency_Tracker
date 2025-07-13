using System.Collections.ObjectModel;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;

using ClassDependencyTracker.Messages;
using ClassDependencyTracker.Models;
using ClassDependencyTracker.Models.DB;
using ClassDependencyTracker.Properties;
using ClassDependencyTracker.Utils.Extensions;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

using Microsoft.Win32;

namespace ClassDependencyTracker.ViewModels;

public partial class MainWindowVM : ObservableRecipient, IDisposable
{
    public static MainWindowVM Instance { get; private set; } = null!;

    [GeneratedRegex(FileUtils.DBExtension)]
    public static partial Regex FileExtensionRegex();
    private const string _openFileDialogTitle = "Select Class Dependency Database";
    private const string _saveFileDialogTitle = "Save Class Dependency Database";

    public MainWindowVM ()
    {
        Settings.Default.Upgrade();
        Instance = this;
        OpenWith();
    }

    public void Dispose()
    {
        Messenger.UnregisterAll(this);
        GC.SuppressFinalize(this);
    }


    #region Properties

    private string _outputFileName = Settings.Default.OutputFileName;
    public string OutputFileName
    {
        get => _outputFileName;
        set
        {
            if (SetProperty(ref _outputFileName, value))
            {
                Settings.Default.OutputFileName = value;
                Settings.Default.Save();
            }
        }
    }

    private string _outputFilePath = Settings.Default.OutputFilePath;
    public string OutputFilePath
    {
        get => _outputFilePath;
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

    //TODO: A separate class should handle all of the class/dependency state
    #region Global Button Handlers

    [RelayCommand]
    public void AddClass()
    {
        ClassModel newClass = new ClassModel($"Class {Classes.Count + 1}");
        Classes.Add(newClass);
        Messenger.Send(new ClassesUpdatedMsg(UpdateType.Added, UpdateType.None));
    }

    [RelayCommand]
    public void DeleteClass(ClassModel classModel)
    {
        string title = $"Delete Class {classModel.Name}?";
        string message = $"Do you wish to delete {classModel.Name}?";
        bool result = DialogUtils.ConfirmationDialog(title, message);
        if (!result) //User cancelled
            return;

        RemoveFromRequirements(classModel);
        Classes.SafeRemove(classModel);
        Messenger.Send(new ClassesUpdatedMsg(UpdateType.Removed, UpdateType.None));
    }

    [RelayCommand]
    public void DeleteRequirement(DependencyModel requirement)
    {
        string requiredName = requirement.RequiredClass.Name;
        string title = $"Delete Requirement for {requiredName}?";
        string message = $"Do you wish to delete the requirement for class {requiredName}?";
        bool result = DialogUtils.ConfirmationDialog(title, message);
        if (!result) //User cancelled
            return;

        requirement.SourceClass.DeleteRequirementSilent(requirement);
    }

    private void RemoveFromRequirements(ClassModel classModel)
    {
        var invalidatedRequirements = Classes.SelectMany(x => x.Requirements)
                    .Where(x => x.RequiredClass == classModel)
                    .ToArray();
        foreach (var invalid in invalidatedRequirements)
        {
            invalid.SourceClass.DeleteRequirementSilent(invalid);
        }
    }

    #endregion Global Button Handlers

    #region Open

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
            bool? res = openFileDialog.ShowDialog();
            if (res != true) //User cancelled
                return;

            OpenFile(openFileDialog.FileName);
        });
    }

    public void OpenWith()
    {
        string? filePath = Environment.GetCommandLineArgs().FirstOrDefault(FileExtensionRegex().IsMatch);
        if (File.Exists(filePath)) //Valid file in the command args
            Task.Run(() => OpenFile(filePath));
        else if (Settings.Default.LoadLast && File.Exists(Settings.Default.LastInputFile)) //Fallback of trying to open the last file, if that's enabled
            Task.Run(() => OpenFile(Settings.Default.LastInputFile));
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
        Messenger.Send(new ClassesUpdatedMsg(UpdateType.Replaced, UpdateType.Replaced));

        if (classes.Length > 0)
            Status = TextStatus.Success($"Loaded {classes.Length} classes from database file: {filePath}");
        else
            TextStatus.Error($"Didn't load any classes from database file: {filePath}. See log for details.");
    }

    #endregion Open

    #region Save

    private bool CanSave => Classes.Count > 0;

    [RelayCommand(CanExecute=nameof(CanSave))]
    public Task Save()
    {
        if (OutputFilePath.IsNullOrEmpty()) //No path, try save as instead
            return SaveAs();

        return Task.Run(() => SaveToFile(OutputFilePath));
    }

    [RelayCommand(CanExecute=nameof(CanSave))]
    public Task SaveAs()
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
            if (result != true) //User cancelled
                return;

            OutputFilePath = fileDialog.FileName;

            if (OutputFilePath.IsNullOrEmpty()) //Invalid FilePath
                return;

            SaveToFile(OutputFilePath);
        });
    }

    private void SaveToFile(string filePath)
    {
        if (!CanSave)
            return;

        ClassModel[] classes = Classes.ToArray();
        Status = new TextStatus($"Saving to the database file {filePath}");
        DBUtils.CreateDB(filePath);
        bool success = DBUtils.TrySaveToFile(filePath, classes);
        if (success)
            Status = TextStatus.Success($"Saved {classes.Length} classes to the database file: {filePath}");
        else
            Status = TextStatus.Error($"Error saving to the database file: {filePath}. See log for details.");
    }

    #endregion Save

    #endregion Commands
}