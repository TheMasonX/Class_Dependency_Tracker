using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using ClassDependencyTracker.Utils.Extensions;

using CommunityToolkit.Mvvm.ComponentModel;

namespace ClassDependencyTracker.Models;

public partial class ClassModel : ObservableObject
{
    internal ClassModel() : this("Unknown")
    {

    }

    public ClassModel(string name, IEnumerable<ClassModel>? requirements = null)
    {
        Name = name;
        Requirements = DispatcherUtils.CreateObservableCollection(requirements);
    }


    #region Properties

    [ObservableProperty]
    private string _name = "Unknown";

    [ObservableProperty]
    private Uri? _URL;

    [ObservableProperty]
    private ObservableCollection<ClassModel> _requirements = null!;

    #endregion Properties
}
