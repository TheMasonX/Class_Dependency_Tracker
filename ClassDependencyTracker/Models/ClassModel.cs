using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Security.Claims;
using System.Windows;
using System.Windows.Input;

using ClassDependencyTracker.Messages;
using ClassDependencyTracker.Models.DB;
using ClassDependencyTracker.Utils.Classes;
using ClassDependencyTracker.Utils.Extensions;
using ClassDependencyTracker.ViewModels;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

using Serilog;

namespace ClassDependencyTracker.Models;

public partial class ClassModel : ObservableRecipient, IDisposable
{
    private const int _maxDepth = 50;
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

    #region Semester

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(Spring), nameof(Summer), nameof(Fall))]
    private Semester _semester;

    public bool Spring
    {
        get => Semester.HasFlag(Semester.Spring);
        set => SetSemesterFlag(Semester.Spring, value);
    }

    public bool Summer
    {
        get => Semester.HasFlag(Semester.Summer);
        set => SetSemesterFlag(Semester.Summer, value);
    }

    public bool Fall
    {
        get => Semester.HasFlag(Semester.Fall);
        set => SetSemesterFlag(Semester.Fall, value);
    }

    private void SetSemesterFlag(Semester semester, bool value)
    {
        if (value)
            Semester |= semester;
        else
            Semester &= ~semester;
    }

    #endregion Semester

    [ObservableProperty]
    private ObservableCollection<DependencyModel> _requirements = null!;
    partial void OnRequirementsChanged(ObservableCollection<DependencyModel> value)
    {
        Revalidate();
    }

    [ObservableProperty]
    private bool _isValid;

    [ObservableProperty]
    private bool _isExpanded = false;

    private int _depth = 0;
    public int Depth
    {
        get
        {
            if (_depth > _maxDepth)
            {
                Log.Logger.Error("Calculating Depth for class {ClassName} surpassed max depth of {MaxDepth}. Cause is probably a cycle.", Name, _maxDepth);
                return _depth;
            }
            return _depth = !AnyRequirements ? 0 : Requirements.Max(x => x.RequiredClass.Depth) + 1;
        }
    }

    public bool AnyRequirements => Requirements.Count > 0;
    public bool CanAdd => Requirements.Count < (AllClasses.Count - 1);

    public static ObservableCollection<ClassModel> AllClasses => MainWindowVM.Instance.Classes;

    #endregion Properties

    #region Commands

    private ICommand? _deleteClassCommand;
    public ICommand DeleteClassCommand => _deleteClassCommand ??= new RelayCommand(() => MainWindowVM.Instance.DeleteClassCommand.Execute(this));

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
        Messenger.Send(new ClassesUpdatedMsg(UpdateType.None, UpdateType.Added));
    }

    public void DeleteRequirementSilent(DependencyModel classModel)
    {
        Requirements.SafeRemove(classModel);
        Refresh();
        Messenger.Send(new ClassesUpdatedMsg(UpdateType.None, UpdateType.Removed));
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
            Semester = dbModel.Semester,
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
            Semester = Semester,
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
        RemoveInvalidRequirements();
        Refresh();
    }

    private void RemoveInvalidRequirements()
    {
        DependencyModel[] currentRequirements = Requirements.ToArray();
        List<DependencyModel> invalidRequirements = [];
        for (int i = 0; i < currentRequirements.Length; i++)
        {
            var model = currentRequirements[i];
            if (model.RequiredClass is null || model.RequiredClass == this || currentRequirements[0..i].Contains(model))
                invalidRequirements.Add(model);
        }

        //var invalidRequirements = Requirements.Where(x => x.RequiredClass is null || x.RequiredClass == this).ToArray();
        foreach (var requirement in invalidRequirements)
        {
            DeleteRequirementSilent(requirement);
        }
    }

    private bool IsValidRequirement(ClassModel classModel)
    {
        return (classModel != this) && !Requirements.Any(x => x.RequiredClass == classModel);
    }

    #endregion Validity

    private void OnClassedUpdated(object recipient, ClassesUpdatedMsg message)
    {
        Revalidate();
    }

    public override string ToString()
    {
        string[] reqNames = Requirements.Select(x => x.RequiredClass.Name).ToArray();

        string formattedRequirements = reqNames.Length switch
        {
            0 => "",
            > 3 => $" | {reqNames.Length} Requirements",
            _ => $" | Requirements: [{string.Join(", ", reqNames)}]",
        };

        return $"Class: {Name} | Depth: {Depth}{formattedRequirements}";
    }
}
