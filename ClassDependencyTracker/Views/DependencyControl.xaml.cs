using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using ClassDependencyTracker.Models;
using ClassDependencyTracker.ViewModels;

namespace ClassDependencyTracker.Views;

/// <summary>
/// Interaction logic for DependencyControl.xaml
/// </summary>
public partial class DependencyControl : UserControl
{
    public DependencyControl()
    {
        InitializeComponent();
    }

    #region Dependency Properties

    public ClassModel Class
    {
        get { return (ClassModel)GetValue(ClassProperty); }
        set { SetValue(ClassProperty, value); }
    }

    // Using a DependencyProperty as the backing store for Class.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty ClassProperty =
        DependencyProperty.Register("Class", typeof(ClassModel), typeof(DependencyControl), new PropertyMetadata(OnClassPropertyChanged));

    public static void OnClassPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not DependencyControl uc)
        {
            Trace.WriteLine($"Couldn't get UserControl DependencyControl from type {d.GetType()}");
            return;
        }

        if (uc.DataContext is not DependencyControlVM vm)
        {
            Trace.WriteLine($"Couldn't get ViewModel DependencyControlVM for UserControl {uc}");
            return;
        }

        if (e.NewValue is not ClassModel classModel)
        {
            Trace.WriteLine($"Couldn't get Class for UserControl {uc}. NewValue is {e.NewValue}.");
            return;
        }

        vm.Class = classModel;
    }

    #endregion Dependency Properties
}
