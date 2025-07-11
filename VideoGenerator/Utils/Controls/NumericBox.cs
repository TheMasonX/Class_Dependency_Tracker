using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;

using Serilog;

namespace VideoGenerator.Utils.Controls;

public partial class NumericBox : TextBox
{
    public NumericBox () : base()
    {
        PreviewTextInput += NumericBox_PreviewTextInput;
        //RegisterMouseEvents();
    }

    //private void RegisterMouseEvents ()
    //{
    //    MouseLeftButtonDown += NumericBox_MouseLeftButtonDown;
    //    MouseLeftButtonUp += (a, b) => clicked = false;
    //    LostFocus += (a, b) => clicked = dragging = false;
    //    MouseMove += NumericBox_MouseMove;
    //}

    //private readonly double minDragDistance = SystemParameters.MinimumHorizontalDragDistance * SystemParameters.MinimumHorizontalDragDistance + SystemParameters.MinimumVerticalDragDistance * SystemParameters.MinimumVerticalDragDistance;
    //private Point mouseClickPosition;
    //private bool clicked = false;
    //private bool dragging = false;

    //private void NumericBox_MouseLeftButtonDown (object sender, System.Windows.Input.MouseButtonEventArgs e)
    //{
    //    if (sender is not UIElement element)
    //    {
    //        return;
    //    }

    //    clicked = true;
    //    mouseClickPosition = e.MouseDevice.GetPosition(element);
    //}

    //private void NumericBox_MouseMove (object sender, System.Windows.Input.MouseEventArgs e)
    //{
    //    if (sender is not UIElement element)
    //    {
    //        return;
    //    }

    //    Point pos = e.MouseDevice.GetPosition(element);
    //    Vector offset = mouseClickPosition - pos;
    //    double offsetMagnitude = offset.Length - minDragDistance;
    //    if (offsetMagnitude <= 0)
    //    {
    //        return;
    //    }

    //    Log.Debug("{Magnitude}", offsetMagnitude);
    //}

    [GeneratedRegex(@"[\d]{1,}")]
    private static partial Regex GetIntRegex ();
    public static readonly Regex IntRegex = GetIntRegex();
    public static Regex FloatRegex => new($"[\\d,\\{DecimalSeparator}]{{1,}}");
    public static Regex FloatSeparatorRegex => new($".*[{DecimalSeparator}]{{1,}}.*");
    public static string DecimalSeparator => CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;

    private void NumericBox_PreviewTextInput (object sender, System.Windows.Input.TextCompositionEventArgs e)
    {
        Regex match = (IsFloat && !FloatSeparatorRegex.IsMatch(Text)) ? FloatRegex : IntRegex;
        e.Handled = !match.IsMatch(e.Text);
    }

    public bool IsFloat
    {
        get => (bool)GetValue(IsFloatProperty);
        set => SetValue(IsFloatProperty, value);
    }

    public static readonly DependencyProperty IsFloatProperty = DependencyProperty.Register("IsFloat", typeof(bool), typeof(NumericBox), new PropertyMetadata(true));
}
