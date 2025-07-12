using System.Collections.ObjectModel;

using ClassDependencyTracker.Models;
using ClassDependencyTracker.Utils.Extensions;

using CommunityToolkit.Mvvm.ComponentModel;

namespace ClassDependencyTracker.ViewModels;

public partial class DependencyControlVM : ObservableRecipient
{
    public DependencyControlVM()
    {

    }

    #region Properties

    [ObservableProperty]
    private ClassModel _class = null!;

    #endregion Properties
}
