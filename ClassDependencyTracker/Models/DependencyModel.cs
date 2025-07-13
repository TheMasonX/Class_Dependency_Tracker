using System.Collections.ObjectModel;
using System.Windows.Data;

using ClassDependencyTracker.Messages;
using ClassDependencyTracker.Models.DB;
using ClassDependencyTracker.Utils.Extensions;
using ClassDependencyTracker.ViewModels;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;


namespace ClassDependencyTracker.Models;

public partial class DependencyModel : ObservableObject
{
    public DependencyModel(ClassModel source, ClassModel requirement)
    {
        SourceClass = source;
        RequiredClass = requirement;
        NonSourceClasses = AllClasses.CreateListCollectionView(FilterOutSource);
    }

    #region Properties


    public ClassModel SourceClass { get; }

    [ObservableProperty]
    private ClassModel _requiredClass = null!;
    partial void OnRequiredClassChanged(ClassModel value)
    {
        WeakReferenceMessenger.Default.Send(new ClassesUpdatedMsg(UpdateType.None, UpdateType.Replaced));
    }

    public static ObservableCollection<ClassModel> AllClasses => MainWindowVM.Instance.Classes;
    public ListCollectionView NonSourceClasses { get; private set; } = null!;

    public IRelayCommand<DependencyModel> DeleteRequirementCommand => MainWindowVM.Instance.DeleteRequirementCommand;

    #endregion Properties

    private bool FilterOutSource(object obj)
    {
        if (obj is not ClassModel model)
            return false;
        
        //Ask the source if this is a valid class, but don't check for duplicates since that would disclude the current class
        return SourceClass.IsValidRequirement(model, false);
    }

    public static DependencyModel? ParseDBModel(DBDependencyModel dbModel, IEnumerable<ClassModel> classes)
    {
        ClassModel? sourceClass = classes.FirstOrDefault(x => x.ID == dbModel.SourceID);
        if (sourceClass is null)
        {
            Log.Logger.Error("No source class ClassModel found for {DBDependencyModel}", dbModel);
            return null;
        }

        ClassModel? requiredClass = classes.FirstOrDefault(x => x.ID == dbModel.RequiredID);
        if (requiredClass is null)
        {
            Log.Logger.Error("No required class ClassModel found for {DBDependencyModel}", dbModel);
            return null;
        }

        return new DependencyModel(sourceClass, requiredClass);
    }

    public DBDependencyModel? ToDBModel(IEnumerable<DBClassModel> classes)
    {
        DBClassModel? sourceClass = classes.FirstOrDefault(x => x.Name == SourceClass.Name);
        if (sourceClass is null)
        {
            Log.Logger.Error("No source class DBClassModel found for {DependencyModel} matching {SourceClass}", this, SourceClass);
            return null;
        }
        else if (!sourceClass.ID.HasValue)
        {
            Log.Logger.Error("Source {SourceClassDB} found for {DependencyModel} matching {SourceClass}, but ID is null", sourceClass, this, SourceClass);
            return null;
        }

        DBClassModel? requiredClass = classes.FirstOrDefault(x => x.Name == RequiredClass?.Name);
        if (requiredClass is null)
        {
            Log.Logger.Error("No required class DBClassModel found for {DependencyModel} matching {RequiredClass}", this, RequiredClass);
            return null;
        }
        else if (!requiredClass.ID.HasValue)
        {
            Log.Logger.Error("Required {RequiredClassDB} found for {DependencyModel} matching {RequiredClass}, but ID is null", requiredClass, this, RequiredClass);
            return null;
        }

        return new DBDependencyModel
        {
            SourceID = sourceClass.ID.Value,
            RequiredID = requiredClass.ID.Value,
        };
    }

    #region Overrides

    public override string ToString()
    {
        return $"'{SourceClass.Name}' Requires '{RequiredClass.Name}'";
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

    #endregion
}
