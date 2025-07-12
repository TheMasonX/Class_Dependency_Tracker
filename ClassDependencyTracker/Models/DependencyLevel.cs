using System.Collections.ObjectModel;

using ClassDependencyTracker.Utils.Extensions;

using CommunityToolkit.Mvvm.ComponentModel;

namespace ClassDependencyTracker.Models;

public partial class DependencyLevel : ObservableObject
{
    public DependencyLevel(int level)
    {
        Level = level;
        Name = $"Level {level}";
        Label = Name;
    }

    public int Level { get; }
    public string Name { get; }

    [ObservableProperty]
    private string _label;

    public bool Visible => Classes.Count > 0;

    [ObservableProperty]
    private bool _isExpanded = true;

    [ObservableProperty]
    private ObservableCollection<ClassModel> _classes = DispatcherUtils.CreateObservableCollection<ClassModel>();
    partial void OnClassesChanged(ObservableCollection<ClassModel> value)
    {
        Refresh();
    }


    public void SetClasses(IEnumerable<ClassModel> classes)
    {
        Classes.SafeClear();
        foreach (var c in classes)
        {
            Classes.SafeAdd(c);
        }

        Refresh();
    }

    public void Refresh()
    {
        OnPropertyChanged(nameof(Visible));
        int count = Classes.Count;
        Label = $"{Name} - {count} {(count == 1 ? "Class" : "Classes")}";
    }
}
