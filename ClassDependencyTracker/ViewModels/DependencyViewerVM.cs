using System.Collections.ObjectModel;

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
        DependencyLevels = DispatcherUtils.CreateObservableCollection(Enumerable.Range(0, 10).Select(x => new DependencyLevel(x)));
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
    private ObservableCollection<DependencyLevel> _dependencyLevels = null!;

    #endregion Properties

    private void OnClassedUpdated(object recipient, ClassesUpdatedMsg message)
    {
        Refresh();
    }

    public void Refresh()
    {
        foreach (DependencyLevel level in DependencyLevels)
        {
            level.Classes.SafeClear();
        }

        foreach (ClassModel classModel in Classes)
        {
            int depth = classModel.Depth;
            for (int i = DependencyLevels.Count; i < depth; i++)
            {
                DependencyLevels.SafeAdd(new DependencyLevel(i));
            }
            DependencyLevels.ElementAt(depth).Classes.SafeAdd(classModel);
        }

        foreach (DependencyLevel level in DependencyLevels)
        {
            level.Refresh();
        }
    }
}
