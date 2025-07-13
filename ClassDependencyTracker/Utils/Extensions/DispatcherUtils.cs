using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Data;


namespace ClassDependencyTracker.Utils.Extensions;

public static class DispatcherUtils
{
    public static ObservableCollection<T> CreateObservableCollection<T>(IEnumerable<T>? collection = null, bool invokeIfNoDispatcher = true)
    {
        ObservableCollection<T> result = null!;
        SafeInvoke(() => result = new ObservableCollection<T>(collection ?? []), invokeIfNoDispatcher);
        return result;
    }

    public static ListCollectionView CreateListCollectionView<T>(this ObservableCollection<T> collection, Predicate<object> filter, bool invokeIfNoDispatcher = true)
    {
        ListCollectionView result = null!;
        SafeInvoke(() => result = new ListCollectionView(collection) { Filter = filter }, invokeIfNoDispatcher);
        return result;
    }

    public static bool SafeInvoke(Action action, bool invokeIfNoDispatcher = true, [CallerArgumentExpression(nameof(action))] string actionName = "")
    {
        var dispatcher = Application.Current?.Dispatcher;
        if (dispatcher is null)
        {
            if (invokeIfNoDispatcher)
            {
                Log.Logger.Warning("Application.Current.Dispatcher is null, but we'll try invoking anyways {Action}", actionName);
                action.Invoke();
                return true;
            }
            Log.Logger.Warning("Application.Current.Dispatcher is null, can't invoke {Action}", actionName);
            return false;
        }

        dispatcher.Invoke(action);
        return true;
    }

    public static bool SafeInvoke(Func<bool> func, bool invokeIfNoDispatcher = true, [CallerArgumentExpression(nameof(func))] string funcName = "")
    {
        bool res = false;
        void action() => res = func.Invoke();
        if (!SafeInvoke(action, invokeIfNoDispatcher, funcName))
            return false;

        return res;
    }


    public static bool SafeAdd<T>(this ObservableCollection<T> collection, T model) =>  SafeInvoke(() => collection.Add(model));
    public static bool SafeRemove<T>(this ObservableCollection<T> collection, T model) =>  SafeInvoke(() => collection.Remove(model));
    public static bool SafeClear<T>(this ObservableCollection<T> collection) =>  SafeInvoke(collection.Clear);


    public static void SafeRefresh(this ListCollectionView view) =>  view.Dispatcher?.Invoke(view.Refresh);
}
