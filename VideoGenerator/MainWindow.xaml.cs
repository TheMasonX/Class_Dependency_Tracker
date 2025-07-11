using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;

using VideoGenerator.ViewModels;

namespace VideoGenerator;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow ()
    {
        InitializeComponent();
    }

    private void Window_Drop (object sender, DragEventArgs e)
    {
        if (DataContext is not MainWindowVM vm)
        {
            return;
        }

        var files = GetFiles(e, vm.InputExtensionRegex);

        vm.OpenFiles(files);
    }

    private void Window_DragEnter (object sender, DragEventArgs e)
    {
        if (DataContext is not MainWindowVM vm)
        {
            return;
        }

        GetFiles(e, vm.InputExtensionRegex);
    }

    private void Window_DragOver (object sender, DragEventArgs e)
    {
        if (DataContext is not MainWindowVM vm)
        {
            return;
        }

        GetFiles(e, vm.InputExtensionRegex);
    }

    private static IQueryable<string>? GetFiles (DragEventArgs e, Regex regex)
    {
        if (!e.Data.GetDataPresent(DataFormats.FileDrop))
        {
            e.Effects = DragDropEffects.None;
            e.Handled = true;
            return null;
        }
        var files = ((string[])e.Data.GetData(DataFormats.FileDrop)).Where(f => regex.IsMatch(f)).AsQueryable();
        if (!files.Any())
        {
            e.Effects = DragDropEffects.None;
            e.Handled = true;
            return null;
        }

        e.Effects = DragDropEffects.Copy;
        e.Handled = true;
        return files;
    }
}