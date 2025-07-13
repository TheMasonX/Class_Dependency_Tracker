using System.Collections.ObjectModel;
using System.Windows.Data;

using ClassDependencyTracker.Messages;
using ClassDependencyTracker.Models;
using ClassDependencyTracker.Utils.Extensions;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

namespace ClassDependencyTracker.ViewModels;

public partial class DependencyViewerVM : ObservableRecipient, IDisposable
{
    public DependencyViewerVM()
    {
        IEnumerable<DependencyLevel> seedLevels = Enumerable.Range(0, 10).Select(x => new DependencyLevel(x));
        Levels = DispatcherUtils.CreateObservableCollection(seedLevels);
        LevelsView = Levels.CreateListCollectionView(FilterLevels);

        Messenger.Register<ClassesUpdatedMsg>(this, OnClassedUpdated);
    }

    public void Dispose()
    {
        Messenger.UnregisterAll(this);
        GC.SuppressFinalize(this);
    }

    #region Properties

    public IRelayCommand AddClassCommand => MainWindowVM.Instance.AddClassCommand;
    public IRelayCommand<ClassModel> DeleteClassCommand => MainWindowVM.Instance.DeleteClassCommand;
    public ObservableCollection<ClassModel> Classes => MainWindowVM.Instance.Classes;

    [ObservableProperty]
    private ObservableCollection<DependencyLevel> _levels = null!;

    [ObservableProperty]
    private ListCollectionView _levelsView = null!;

    #endregion Properties

    private bool FilterLevels(object obj)
    {
        if (obj is not DependencyLevel level) //Impossible not to pass, but it's bookkeeping
            return false;

        return level.Classes.Count > 0;
    }

    private void OnClassedUpdated(object recipient, ClassesUpdatedMsg message)
    {
        Refresh();
    }

    public void Refresh()
    {
        foreach (DependencyLevel level in Levels)
        {
            level.Classes.SafeClear();
        }

        foreach (ClassModel classModel in Classes)
        {
            int depth = classModel.Depth;
            for (int i = Levels.Count; i < depth; i++)
            {
                Levels.SafeAdd(new DependencyLevel(i));
            }
            Levels.ElementAt(depth).Classes.SafeAdd(classModel);
        }

        foreach (DependencyLevel level in Levels)
        {
            level.Refresh();
        }
        LevelsView.SafeRefresh();
    }
}
