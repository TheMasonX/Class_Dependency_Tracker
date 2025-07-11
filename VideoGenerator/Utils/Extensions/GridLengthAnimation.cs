using System;
using System.Windows;
using System.Windows.Media.Animation;

namespace VideoGenerator.Utils.Extensions;

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

    protected override System.Windows.Freezable CreateInstanceCore ()
    {
        return new GridLengthAnimation();
    }

    public static readonly DependencyProperty FromProperty;
    public GridLength From
    {
        get
        {
            return (GridLength)GetValue(GridLengthAnimation.FromProperty);
        }
        set
        {
            SetValue(GridLengthAnimation.FromProperty, value);
        }
    }

    public static readonly DependencyProperty ToProperty;
    public GridLength To
    {
        get
        {
            return (GridLength)GetValue(GridLengthAnimation.ToProperty);
        }
        set
        {
            SetValue(GridLengthAnimation.ToProperty, value);
        }
    }

    public static readonly DependencyProperty UnitProperty;
    public GridUnitType Unit
    {
        get
        {
            return (GridUnitType)GetValue(GridLengthAnimation.UnitProperty);
        }
        set
        {
            SetValue(GridLengthAnimation.UnitProperty, value);
        }
    }

    public static readonly DependencyProperty EasingFunctionProperty;
    public IEasingFunction? EasingFunction
    {
        get
        {
            return (IEasingFunction?)GetValue(GridLengthAnimation.EasingFunctionProperty);
        }
        set
        {
            SetValue(GridLengthAnimation.EasingFunctionProperty, value);
        }
    }

    public override object GetCurrentValue (object defaultOriginValue,
        object defaultDestinationValue, AnimationClock animationClock)
    {
        double fromVal = ((GridLength)GetValue(GridLengthAnimation.FromProperty)).Value;
        double toVal = ((GridLength)GetValue(GridLengthAnimation.ToProperty)).Value;
        double percent = animationClock.CurrentProgress!.Value;

        GridUnitType unitVal = (GridUnitType)GetValue(GridLengthAnimation.UnitProperty);
        IEasingFunction? easingFunctionVal = (IEasingFunction)GetValue(GridLengthAnimation.EasingFunctionProperty);
        if (easingFunctionVal is not null)
        {
            percent = easingFunctionVal.Ease(percent);
        }

        return new GridLength(percent * (toVal - fromVal) + fromVal, unitVal);
    }
}