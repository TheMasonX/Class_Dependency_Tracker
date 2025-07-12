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

using CommunityToolkit.Mvvm.Input;

namespace ClassDependencyTracker.Views;

/// <summary>
/// Interaction logic for ClassControl.xaml
/// </summary>
public partial class ClassControl : UserControl
{
    public ClassControl()
    {
        InitializeComponent();
    }

    #region Dependency Properties

    public IRelayCommand<ClassModel> DeleteCommand
    {
        get { return (IRelayCommand<ClassModel>)GetValue(DeleteCommandProperty); }
        set { SetValue(DeleteCommandProperty, value); }
    }

    // Using a DependencyProperty as the backing store for DeleteCommand.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty DeleteCommandProperty =
        DependencyProperty.Register("DeleteCommand", typeof(IRelayCommand<ClassModel>), typeof(ClassControl), new PropertyMetadata(OnDeleteCommandPropertyChanged));

    public static void OnDeleteCommandPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not ClassControl uc)
        {
            Trace.WriteLine($"Couldn't get UserControl ClassControl from type {d.GetType()}");
            return;
        }

        if (uc.DataContext is not ClassModel vm)
        {
            Trace.WriteLine($"Couldn't get ViewModel ClassControlVM for UserControl {uc}");
            return;
        }

        if (e.NewValue is not IRelayCommand<ClassModel> command)
        {
            Trace.WriteLine($"Couldn't get DeleteCommand for UserControl {uc}. NewValue is {e.NewValue}.");
            return;
        }

        vm.DeleteCommand = new RelayCommand(() => command.Execute(vm));
    }

    #endregion Dependency Properties
}
