using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows;

using Microsoft.VisualBasic;

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

    public static bool SafeInvoke(Action action, [CallerArgumentExpression(nameof(action))] string actionName = "")
    {
        var dispatcher = Application.Current?.Dispatcher;
        if (dispatcher is null)
        {
            Trace.WriteLine($"Application.Current.Dispatcher is null, can't invoke {actionName}");
            return false;
        }

        dispatcher.Invoke(action);
        return true;
    }

    public static bool SafeInvoke(Func<bool> func, [CallerArgumentExpression(nameof(func))] string funcName = "")
    {
        bool res = false;
        void action() => res = func.Invoke();
        if (!SafeInvoke(action, funcName))
            return false;

        return res;
    }


    public static bool SafeAdd<T>(this ObservableCollection<T> collection, T model) =>  SafeInvoke(() => collection.Add(model));

    public static bool SafeRemove<T>(this ObservableCollection<T> collection, T model) =>  SafeInvoke(() => collection.Remove(model));
}
