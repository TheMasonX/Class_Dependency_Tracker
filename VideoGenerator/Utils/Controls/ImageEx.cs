using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using Serilog;

namespace VideoGenerator.Utils.Controls;

public class ImageEx : Image
{
    public ImageEx () : base()
    {
        SizeChanged += ImageEx_SizeChanged;
        MouseDown += OnMouseDown;
        MouseUp += OnMouseUp;
        MouseMove += OnMouseMove;
        LostKeyboardFocus += (a, b) => ReleaseClick();
        LostFocus += (a, b) => ReleaseClick();
    }

    #region ======================================== Mouse Drag ========================================

    [Flags]
    private enum DragState
    {
        None = 0b00,
        Clicked = 0b01,
        Dragging = 0b10,
    }

    private DragState _dragState;
    private Point _mouseClickPosition;
    private static double _minDragDistance = Math.Sqrt(SystemParameters.MinimumHorizontalDragDistance * SystemParameters.MinimumHorizontalDragDistance + SystemParameters.MinimumVerticalDragDistance * SystemParameters.MinimumVerticalDragDistance);

    private void OnMouseDown (object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        if (e.ChangedButton != MouseDragButton)
        {
            return;
        }

        if (this.CaptureMouse())
        {
            _dragState = DragState.Clicked;
            _mouseClickPosition = e.GetPosition(this);
        }
    }

    private void OnMouseUp (object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        if (e.ChangedButton != MouseDragButton)
        {
            return;
        }

        ReleaseClick();
    }

    private void OnMouseMove (object sender, System.Windows.Input.MouseEventArgs e)
    {
        if (!_dragState.HasFlag(DragState.Clicked))
        {
            return; //Not even clicked
        }

        Point currentPos = e.GetPosition(this);
        Vector offset = _mouseClickPosition - currentPos;
        double offsetMagnitude = offset.Length;
        if (offsetMagnitude < _minDragDistance)
        {
            _dragState &= ~DragState.Dragging;
            return;
        }

        Log.Information("Dragging, State is {DragState} and offset is {Offset}, Parent is {Parent}", _dragState, offset, Parent);
        _dragState |= DragState.Dragging;
    }

    private void ReleaseClick ()
    {
        _dragState = DragState.None;
        this.ReleaseMouseCapture();
    }

    public MouseButton MouseDragButton
    {
        get => (MouseButton)GetValue(MouseDragButtonProperty);
        set => SetValue(MouseDragButtonProperty, value);
    }

    public static readonly DependencyProperty MouseDragButtonProperty = DependencyProperty.Register("MouseDragButton", typeof(MouseButton), typeof(ImageEx), new PropertyMetadata(MouseButton.Left));

    #endregion ======================================== Mouse Drag ========================================

    #region ======================================== Image Size ========================================

    private void ImageEx_SizeChanged (object sender, SizeChangedEventArgs e)
    {
        if (e.WidthChanged)
        {
            ActualWidthValue = e.NewSize.Width;
        }

        if (e.HeightChanged)
        {
            ActualHeightValue = e.NewSize.Height;
        }

        Log.Information("Parent is {Parent}", Parent);
    }

    public double ActualWidthValue
    {
        get => (double)GetValue(ActualWidthValueProperty);
        set => SetValue(ActualWidthValueProperty, value);
    }

    public static readonly DependencyProperty ActualWidthValueProperty = DependencyProperty.Register("ActualWidthValue", typeof(double), typeof(ImageEx), new PropertyMetadata(0.0));

    public double ActualHeightValue
    {
        get => (double)GetValue(ActualHeightValueProperty);
        set => SetValue(ActualHeightValueProperty, value);
    }

    public static readonly DependencyProperty ActualHeightValueProperty = DependencyProperty.Register("ActualHeightValue", typeof(double), typeof(ImageEx), new PropertyMetadata(0.0));

    #endregion ======================================== Image Size ========================================
}
