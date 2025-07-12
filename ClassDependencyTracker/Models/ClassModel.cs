using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Windows.Input;

using ClassDependencyTracker.Messages;
using ClassDependencyTracker.Models.DB;
using ClassDependencyTracker.Utils.Classes;
using ClassDependencyTracker.Utils.Extensions;
using ClassDependencyTracker.ViewModels;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

namespace ClassDependencyTracker.Models;

public partial class ClassModel : ObservableRecipient, IDisposable
{
    private static int _classIndex = 0;

    public ClassModel() : this($"Class {_classIndex++}")
    {

    }

    public ClassModel(string name, IEnumerable<ClassModel>? requirements = null)
    {
        Name = name;
        var deps = requirements?.Select(x => new DependencyModel(this, x)) ?? [];
        Requirements = DispatcherUtils.CreateObservableCollection(deps);
        Messenger.Register<ClassesUpdatedMsg>(this, OnClassedUpdated);
    }

    private void OnClassedUpdated(object recipient, ClassesUpdatedMsg message)
    {
        Revalidate();
    }

    public void Dispose()
    {
        Messenger.UnregisterAll(this);
        GC.SuppressFinalize(this);
    }

    #region Properties

    [ObservableProperty]
    private int? _ID;

    [ObservableProperty]
    private string _name = "Unknown";

    [ObservableProperty]
    private string? _URL;

    [ObservableProperty]
    private int _credits;

    [ObservableProperty]
    private ObservableCollection<DependencyModel> _requirements = null!;
    partial void OnRequirementsChanged(ObservableCollection<DependencyModel> value)
    {
        Revalidate();
    }

    [ObservableProperty]
    private bool _isValid;

    public bool AnyRequirements => Requirements.Count > 0;
    public bool CanAdd => Requirements.Count < (AllClasses.Count - 1);

    public static ObservableCollection<ClassModel> AllClasses => MainWindowVM.Instance.Classes;

    #endregion Properties

    #region Commands

    [ObservableProperty]
    private ICommand? _deleteCommand;

    [RelayCommand]
    public void AddRequirement()
    {
        //TODO: Dialog to pick a class
        ClassModel? requirement = AllClasses.FirstOrDefault(IsValidRequirement);
        if (requirement is not null)
        {
            AddRequirement(requirement, false);
        }
    }

    public void AddRequirement(ClassModel newRequirement, bool testValidity = true)
    {
        if (testValidity && !IsValidRequirement(newRequirement))
            return;

        DependencyModel dep = new DependencyModel(this, newRequirement);
        Requirements.Add(dep);
        Refresh();
    }

    [RelayCommand]
    public void DeleteRequirement(DependencyModel classModel)
    {
        Requirements.SafeRemove(classModel);
        Refresh();
    }

    #endregion Commands

    #region DB Model

    public static ClassModel ParseDBModel(DBClassModel dbModel)
    {
        return new ClassModel
        {
            ID = dbModel.ID,
            Name = dbModel.Name,
            URL = dbModel.URL,
            Credits = dbModel.Credits,
        };
    }

    public DBClassModel ToDBModel()
    {
        return new DBClassModel
        {
            ID = ID,
            Name = Name,
            URL = URL,
            Credits = Credits,
        };
    }

    #endregion DB Model

    #region Validity

    public void Refresh()
    {
        OnPropertyChanged(nameof(Requirements));
        OnPropertyChanged(nameof(AnyRequirements));
        OnPropertyChanged(nameof(CanAdd));
    }

    public void Revalidate()
    {
        Refresh();
        RemoveInvalidRequirements();
    }

    #endregion Validity

    private void RemoveInvalidRequirements()
    {
        var invalidRequirements = Requirements.Where(x => x.RequiredClass is null || x.SourceClass == this).ToArray();
        foreach (var requirement in invalidRequirements)
        {
            DeleteRequirement(requirement);
        }
    }

    private bool IsValidRequirement(ClassModel classModel)
    {
        return (classModel != this) && !Requirements.Any(x => x.RequiredClass == classModel);
    }

    public override string ToString() => Name;
}
