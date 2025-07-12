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

namespace ClassDependencyTracker.Utils.Controls;

/// <summary>
/// Interaction logic for WatermarkCheckbox.xaml
/// </summary>
public partial class WatermarkCheckbox : UserControl
{
    public bool ShowIcon { get; set; }

    public WatermarkCheckbox()
    {
        InitializeComponent();
    }

    public Image Icon
    {
        get { return (Image)GetValue(IconProperty); }
        set { SetValue(IconProperty, value); }
    }

    // Using a DependencyProperty as the backing store for Icon.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty IconProperty =
        DependencyProperty.Register("Icon", typeof(Image), typeof(WatermarkCheckbox), new PropertyMetadata(null));

    public string Text
    {
        get { return (string)GetValue(TextProperty); }
        set { SetValue(TextProperty, value); }
    }

    // Using a DependencyProperty as the backing store for Text.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty TextProperty =
        DependencyProperty.Register("Text", typeof(string), typeof(WatermarkCheckbox), new PropertyMetadata(OnTextChanged));

    public static void OnTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (e.NewValue is not string stringVal)
        {
            Trace.WriteLine($"OnTextChanged failed: {e.NewValue} was not a string.");
            return;
        }

        if (d is not WatermarkCheckbox uc)
        {
            Trace.WriteLine($"OnTextChanged failed: {d} was not a WatermarkCheckbox.");
            return;
        }

        uc.ShowIcon = string.IsNullOrEmpty(stringVal);
    }
}
