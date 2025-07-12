using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
/// Interaction logic for ClassEditor.xaml
/// </summary>
public partial class ClassEditor : UserControl
{
    public ClassEditor()
    {
        InitializeComponent();
    }

    #region Dependency Properties

    //public ObservableCollection<ClassModel> Classes
    //{
    //    get { return (ObservableCollection<ClassModel>)GetValue(ClassesProperty); }
    //    set { SetValue(ClassesProperty, value); }
    //}

    //// Using a DependencyProperty as the backing store for Classes.  This enables animation, styling, binding, etc...
    //public static readonly DependencyProperty ClassesProperty =
    //    DependencyProperty.Register("Classes", typeof(ObservableCollection<ClassModel>), typeof(ClassEditor), new PropertyMetadata(OnClassesPropertyChanged));

    //public static void OnClassesPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    //{
    //    if (d is not ClassEditor uc)
    //    {
    //        Trace.WriteLine($"Couldn't get UserControl ClassEditor from type {d.GetType()}");
    //        return;
    //    }

    //    if (uc.DataContext is not ClassEditorVM vm)
    //    {
    //        Trace.WriteLine($"Couldn't get ViewModel ClassEditorVM for UserControl {uc}");
    //        return;
    //    }

    //    if (e.NewValue is not ObservableCollection<ClassModel> classes)
    //    {
    //        Trace.WriteLine($"Couldn't get Classes for UserControl {uc}. NewValue is {e.NewValue}.");
    //        return;
    //    }

    //    vm.Classes = classes;
    //}

    #endregion Dependency Properties
}
