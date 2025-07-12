using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace ClassDependencyTracker.Utils.Converters;

public class BooleanConverter<T>(T trueValue, T falseValue) : IValueConverter
{
    public T True { get; set; } = trueValue;
    public T False { get; set; } = falseValue;

    public virtual object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        bool isTrue = (value is bool boolVal && boolVal);
        T result = isTrue ? True : False;
        return result!;
    }

    public virtual object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value is T typedValue && EqualityComparer<T>.Default.Equals(typedValue, True);
    }
}

public class InvertedBoolToVisibilityConverter() : BooleanConverter<Visibility>(Visibility.Hidden, Visibility.Visible)
{ }

public class InvertedBoolConverter() : BooleanConverter<bool>(false, true)
{ }

public class BoolToBrushConverter() : BooleanConverter<Brush>(Brushes.Green, Brushes.Red)
{ }

public class BoolToGridLengthConverter() : BooleanConverter<GridLength>(GridLength.Auto, new GridLength(0))
{

}
