using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;

namespace ClassDependencyTracker.Utils.Extensions;

public static class DispatcherUtils
{
    public static ObservableCollection<T> CreateObservableCollection<T>(IEnumerable<T>? collection = null)
    {
        return Application.Current?.Dispatcher?.Invoke(() =>
        {
            return new ObservableCollection<T>(collection ?? []);
        })!;
    }
}
