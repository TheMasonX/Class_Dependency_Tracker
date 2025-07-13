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
}
