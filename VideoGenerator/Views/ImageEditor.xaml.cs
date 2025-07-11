using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using VideoGenerator.Models;
using VideoGenerator.ViewModels;

namespace VideoGenerator.Views;

/// <summary>
/// Interaction logic for ImageEditor.xaml
/// </summary>
public partial class ImageEditor : UserControl
{
    public ImageEditor ()
    {
        InitializeComponent();
    }

    public ImageData SelectedItem
    {
        get => (ImageData)GetValue(SelectedItemProperty);
        set => SetValue(SelectedItemProperty, value);
    }

    public static readonly DependencyProperty SelectedItemProperty = DependencyProperty.Register("SelectedItem", typeof(ImageData), typeof(ImageEditor), new PropertyMetadata(null, OnSelectedItemChanged));

    private static void OnSelectedItemChanged (DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not ImageEditor editor)
        {
            Debug.Fail($"d {d} is {d.GetType()}");
            return;
        }

        if (editor.DataContext is not ImageEditorVM vm)
        {
            Debug.Fail($"editor.DataContext {editor.DataContext} is {editor.DataContext.GetType()}");
            return;
        }

        vm.OpenImage(e.NewValue as ImageData);
    }

    private void NumericBox_AccessKeyPressed (object sender, AccessKeyPressedEventArgs e)
    {

    }

    private void NumericBox_KeyTipAccessed (object sender, KeyTipAccessedEventArgs e)
    {
        e.Handled = true;
        Focus();
    }
}
