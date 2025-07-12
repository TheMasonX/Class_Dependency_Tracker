using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Data;
using System.Windows.Input;

using ClassDependencyTracker.Utils.Extensions;
using ClassDependencyTracker.ViewModels;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace ClassDependencyTracker.Models;

public partial class ClassModel : ObservableObject
{
    private static int _classIndex = 0;

    public ClassModel() : this($"Class {_classIndex++}")
    {

    }

    public ClassModel(string name, IEnumerable<ClassModel>? requirements = null)
    {
        Name = name;
        Requirements = DispatcherUtils.CreateObservableCollection(requirements);
        AllOtherClasses = new ListCollectionView(AllClasses)
        {
            Filter = FilterOutSelf,
        };
    }

    #region Properties

    [ObservableProperty]
    private bool _isValid;

    [ObservableProperty]
    private string _name = "Unknown";

    [ObservableProperty]
    private string? _URL;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CanRemove))]
    private ClassModel? _selectedClass;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CanRemove))]
    private ObservableCollection<ClassModel> _requirements = null!;

    public bool CanRemove => Requirements.Count > 0 && SelectedClass is not null;

    public static ObservableCollection<ClassModel> AllClasses => MainWindowVM.Instance.Classes;
    public ListCollectionView AllOtherClasses { get; }

    #endregion Properties

    #region Commands

    [ObservableProperty]
    private ICommand? _deleteCommand;

    [RelayCommand]
    public void AddRequirement()
    {
        //TODO: Dialog to pick a class
        ClassModel? requirement = AllClasses.FirstOrDefault();
        if (requirement is not null)
            Requirements.Add(requirement);

        OnPropertyChanged(nameof(CanRemove));
    }

    [RelayCommand]
    public void DeleteRequirement(ClassModel classModel)
    {
        Requirements.SafeRemove(classModel);
        OnPropertyChanged(nameof(CanRemove));
    }

    #endregion Commands

    private bool FilterOutSelf(object obj)
    {
        if (obj is not ClassModel otherClass) return false;

        return otherClass != this;
    }

    public override string ToString() => Name;
}
