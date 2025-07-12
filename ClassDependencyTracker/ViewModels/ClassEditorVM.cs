using System.Collections.ObjectModel;
using System.Windows;

using ClassDependencyTracker.Models;
using ClassDependencyTracker.Utils.Extensions;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace ClassDependencyTracker.ViewModels;

public partial class ClassEditorVM : ObservableRecipient
{
    public ClassEditorVM()
    {
        Classes = MainWindowVM.Instance.Classes;
    }

    #region Properties

    [ObservableProperty]
    private ClassModel? _selectedClass;

    [ObservableProperty]
    private ObservableCollection<ClassModel> _classes = null!;

    #endregion Properties

    #region Commands

    [RelayCommand]
    public void AddClass()
    {
        ClassModel newClass = new ClassModel($"Class {Classes.Count}");
        Classes.Add(newClass);
    }

    [RelayCommand]
    public void DeleteClass(ClassModel @class)
    {
        string title = $"Delete Class {@class}?";
        string message = $"Do you wish to delete class {@class}?";
        MessageBoxResult result = MessageBox.Show(message, title, MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No);
        if (result != MessageBoxResult.Yes) return;

        Classes.SafeRemove(@class);
    }

    #endregion Commands
}
