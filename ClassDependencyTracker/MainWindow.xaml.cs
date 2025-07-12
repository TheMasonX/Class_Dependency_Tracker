using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;

using ClassDependencyTracker.Utils.Extensions;
using ClassDependencyTracker.ViewModels;

namespace ClassDependencyTracker;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private void Window_Drop(object sender, DragEventArgs e)
    {
        if (DataContext is not MainWindowVM vm)
            return;

        string? file = GetFile(e);
        if (file.IsNullOrEmpty())
            return;

        vm.OpenFile(file);
    }

    private void Window_DragEnter(object sender, DragEventArgs e)
    {
        if (DataContext is not MainWindowVM vm)
            return;

        GetFile(e);
    }

    private void Window_DragOver(object sender, DragEventArgs e)
    {
        if (DataContext is not MainWindowVM vm)
            return;

        GetFile(e);
    }

    private static string? GetFile(DragEventArgs e)
    {
        e.Handled = true;

        if (!e.Data.GetDataPresent(DataFormats.FileDrop))
        {
            e.Effects = DragDropEffects.None;
            return null;
        }

        string[] fileDropFiles = (string[])e.Data.GetData(DataFormats.FileDrop);
        string? file = fileDropFiles.FirstOrDefault(MainWindowVM.FileExtensionRegex().IsMatch);
        if (!file.IsNullOrEmpty())
        {
            e.Effects = DragDropEffects.None;
            return null;
        }

        e.Effects = DragDropEffects.Copy;
        return file;
    }
}