using System.Windows;

namespace ClassDependencyTracker.Utils.Extensions;

public static class DialogUtils
{
    public static bool ConfirmationDialog(string title, string message)
    {
        MessageBoxResult result = MessageBox.Show(message, title, MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No);
        return result == MessageBoxResult.Yes;
    }
}
