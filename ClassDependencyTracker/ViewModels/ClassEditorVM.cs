using System.Collections.ObjectModel;

using ClassDependencyTracker.Models;
using ClassDependencyTracker.Utils.Extensions;

using CommunityToolkit.Mvvm.ComponentModel;

namespace ClassDependencyTracker.ViewModels;

public partial class ClassEditorVM : ObservableRecipient
{
    public ClassEditorVM()
    {

    }

    #region Properties

    [ObservableProperty]
    public ObservableCollection<ClassModel> _classes = null!;

    #endregion Properties
}
