using System;
using System.Collections.ObjectModel;
using System.Windows.Data;

using ClassDependencyTracker.Models;
using ClassDependencyTracker.ViewModels;

using CommunityToolkit.Mvvm.ComponentModel;

namespace ClassDependencyTracker.Utils.Classes;

public partial class DependencyModel : ObservableObject
{
    [ObservableProperty]
    private ClassModel _sourceClass;

    [ObservableProperty]
    private ClassModel? _requiredClass;

    public DependencyModel(ClassModel source, ClassModel requirement)
    {
        _sourceClass = source;
        _requiredClass = requirement;
        NonSourceClasses = new ListCollectionView(AllClasses) { Filter = FilterOutSource };
    }

    public static ObservableCollection<ClassModel> AllClasses => MainWindowVM.Instance.Classes;
    public ListCollectionView NonSourceClasses { get; }

    private bool FilterOutSource(object obj)
    {
        return (obj is ClassModel model) && model != SourceClass;
    }

    public override string ToString()
    {
        return $"{SourceClass} -> {RequiredClass}";
    }

    public static bool operator ==(DependencyModel l, DependencyModel r) => Equals(l, r);
    public static bool operator !=(DependencyModel l, DependencyModel r) => !Equals(l, r);

    public override bool Equals(object? obj)
    {
        return obj is DependencyModel other && other.RequiredClass == RequiredClass && other.SourceClass == SourceClass;
    }

    public override int GetHashCode()
    {
        return (SourceClass.GetHashCode() * 23) ^ RequiredClass?.GetHashCode() ?? 0;
    }
}
