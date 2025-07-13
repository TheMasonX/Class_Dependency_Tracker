using System.Collections.ObjectModel;
using System.Windows.Media;

using ClassDependencyTracker.Utils.Extensions;

using CommunityToolkit.Mvvm.ComponentModel;

namespace ClassDependencyTracker.Models;

public partial class DependencyLevel : ObservableObject
{
    //https://learn.microsoft.com/en-us/dotnet/api/system.windows.media.colors?view=windowsdesktop-9.0
    public static Brush[] Backgrounds =
    [
        Brushes.Snow,
        Brushes.WhiteSmoke,
    ];

    public DependencyLevel(int level)
    {
        Level = level;
        Name = $"Level {level + 1}";
        Label = Name;
        Background = Backgrounds[Level % Backgrounds.Length];
    }

    public int Level { get; }
    public string Name { get; }
    public Brush Background { get; }

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
