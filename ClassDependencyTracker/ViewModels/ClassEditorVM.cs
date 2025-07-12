using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Data;

using ClassDependencyTracker.Messages;
using ClassDependencyTracker.Models;
using ClassDependencyTracker.Utils.Extensions;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

namespace ClassDependencyTracker.ViewModels;

public partial class ClassEditorVM : ObservableRecipient
{
    public ClassEditorVM()
    {
        Classes = MainWindowVM.Instance.Classes;
        ClassesView = new ListCollectionView(Classes) { Filter = FilterClasses };
    }

    #region Properties

    [ObservableProperty]
    private ObservableCollection<ClassModel> _classes = null!;

    [ObservableProperty]
    private ListCollectionView _classesView = null!;

    #region Filters

    [ObservableProperty]
    private bool _showAdvanced;

    private const bool _defaultCaseSensitive = false;
    [ObservableProperty]
    private bool _caseSensitive = _defaultCaseSensitive;
    partial void OnCaseSensitiveChanged(bool value)
    {
        RefreshSearch();
    }

    [ObservableProperty]
    private StringFilterType _filterType = StringFilterType.Contains;
    partial void OnFilterTypeChanged(StringFilterType value)
    {
        RefreshSearch();
    }

    public StringFilterType[] FilterTypes { get; } = Enum.GetValues<StringFilterType>();

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsSearching))]
    private string _classNameFilter = "";
    partial void OnClassNameFilterChanged(string value)
    {
        RefreshSearch();
    }

    public bool IsSearching => !ClassNameFilter.IsNullOrEmpty();

    #endregion Filters

    #endregion Properties

    #region Commands

    [RelayCommand]
    public void AddClass()
    {
        ClassModel newClass = new ClassModel($"Class {Classes.Count}");
        Classes.Add(newClass);
        Messenger.Send(new ClassesUpdatedMsg(ClassUpdateType.Added));
    }

    [RelayCommand]
    public void DeleteClass(ClassModel @class)
    {
        string title = $"Delete Class {@class}?";
        string message = $"Do you wish to delete class {@class}?";
        MessageBoxResult result = MessageBox.Show(message, title, MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No);
        if (result != MessageBoxResult.Yes) return;

        Classes.SafeRemove(@class);
        Messenger.Send(new ClassesUpdatedMsg(ClassUpdateType.Removed));
    }

    [RelayCommand]
    public void ClearFilter()
    {
        ClassNameFilter = "";
    }

    #endregion Commands

    private void RefreshSearch()
    {
        ClassesView.Dispatcher.Invoke(ClassesView.Refresh);
    }

    private bool FilterClasses(object obj)
    {
        if (obj is not ClassModel model) return false;
        if (ClassNameFilter.IsNullOrEmpty()) return true;

        return StringExtensions.ApplyFilter(model.Name, ClassNameFilter, FilterType, CaseSensitive);
    }
}
