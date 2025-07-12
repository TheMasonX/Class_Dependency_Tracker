using System;
using System.Windows;
using System.Windows.Media.Animation;

namespace ClassDependencyTracker.Utils.Extensions;

internal class GridLengthAnimation : AnimationTimeline
{
    static GridLengthAnimation ()
    {
        FromProperty = DependencyProperty.Register("From", typeof(GridLength),
            typeof(GridLengthAnimation));

        ToProperty = DependencyProperty.Register("To", typeof(GridLength),
            typeof(GridLengthAnimation));

        UnitProperty = DependencyProperty.Register("Unit", typeof(GridUnitType),
            typeof(GridLengthAnimation));

        EasingFunctionProperty = DependencyProperty.Register("EasingFunction", typeof(IEasingFunction),
            typeof(GridLengthAnimation));
    }

    public override Type TargetPropertyType
    {
        get
        {
            return typeof(GridLength);
        }
    }

    protected override Freezable CreateInstanceCore ()
    {
        return new GridLengthAnimation();
    }

    public static readonly DependencyProperty FromProperty;
    public GridLength From
    {
        get
        {
            return (GridLength)GetValue(FromProperty);
        }
        set
        {
            SetValue(FromProperty, value);
        }
    }

    public static readonly DependencyProperty ToProperty;
    public GridLength To
    {
        get
        {
            return (GridLength)GetValue(ToProperty);
        }
        set
        {
            SetValue(ToProperty, value);
        }
    }

    public static readonly DependencyProperty UnitProperty;
    public GridUnitType Unit
    {
        get
        {
            return (GridUnitType)GetValue(UnitProperty);
        }
        set
        {
            SetValue(UnitProperty, value);
        }
    }

    public static readonly DependencyProperty EasingFunctionProperty;
    public IEasingFunction? EasingFunction
    {
        get
        {
            return (IEasingFunction?)GetValue(EasingFunctionProperty);
        }
        set
        {
            SetValue(EasingFunctionProperty, value);
        }
    }

    public override object GetCurrentValue (object defaultOriginValue,
        object defaultDestinationValue, AnimationClock animationClock)
    {
        double fromVal = ((GridLength)GetValue(FromProperty)).Value;
        double toVal = ((GridLength)GetValue(ToProperty)).Value;
        double percent = animationClock.CurrentProgress!.Value;

        GridUnitType unitVal = (GridUnitType)GetValue(UnitProperty);
        IEasingFunction? easingFunctionVal = (IEasingFunction)GetValue(EasingFunctionProperty);
        if (easingFunctionVal is not null)
        {
            percent = easingFunctionVal.Ease(percent);
        }

        return new GridLength(percent * (toVal - fromVal) + fromVal, unitVal);
    }
}