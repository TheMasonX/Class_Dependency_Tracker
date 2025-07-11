using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using Serilog;

using VideoGenerator.Models;
using VideoGenerator.Utils.Extensions;

using OpenCvSharp;
using OpenCvSharp.Extensions;
using OpenCvSharp.WpfExtensions;

using Image = System.Drawing.Image;

namespace VideoGenerator.ViewModels;

public class ImageEditorVM : ObservableObject, IDisposable
{
    public ImageEditorVM ()
    {
        Trace.WriteLine($"{ImageTools.Count} ImageTools Available!");
    }


    public void Dispose ()
    {
        GC.SuppressFinalize(this);
    }


    #region Properties

    private ImageData? _image;
    public ImageData? Image
    {
        get => _image;
        private set
        {
            if (SetProperty(ref _image, value))
            {
                CreateNewBitmap(value?.GetData());
            }
        }
    }

    private WriteableBitmap? _bitmap;
    public WriteableBitmap? Bitmap
    {
        get => _bitmap;
        set
        {
            if (SetProperty(ref _bitmap, value) && value is not null)
            {
                ResetControls();
            }
        }
    }

    private ObservableCollection<IImageTool>? _imageTools;
    public ObservableCollection<IImageTool> ImageTools
    {
        get => _imageTools ??= GetTools();
        set => SetProperty(ref _imageTools, value);
    }

    private ObservableCollection<UserControl>? _imageToolViews;
    public ObservableCollection<UserControl>? ImageToolViews
    {
        get => _imageToolViews;
        set => SetProperty(ref _imageToolViews, value);
    }

    private readonly double _minZoom = 0.01;
    public double MinZoom => _minZoom;
    private readonly double _maxZoom = 5;
    public double MaxZoom => _maxZoom;

    private double _zoom = 1;
    public double Zoom
    {
        get => _zoom;
        set => SetProperty(ref _zoom, value);
    }

    private double _centerX = .5;
    public double CenterX
    {
        get => _centerX;
        set => SetProperty(ref _centerX, value);
    }

    private double _centerY = .5;
    public double CenterY
    {
        get => _centerY;
        set => SetProperty(ref _centerY, value);
    }

    private double _actualWidth = 0;
    public double ActualWidth
    {
        get => _actualWidth;
        set => SetProperty(ref _actualWidth, value);
    }

    private double _actualHeight = 0;
    public double ActualHeight
    {
        get => _actualHeight;
        set => SetProperty(ref _actualHeight, value);
    }

    #endregion Properties

    #region Commands

    private RelayCommand<RoutedEventArgs>? _mouseWheelCommand;
    public ICommand MouseWheelCommand => _mouseWheelCommand ??= new RelayCommand<RoutedEventArgs>(OnMouseWheel);

    public void OnMouseWheel (RoutedEventArgs? e)
    {
        if (e is not MouseWheelEventArgs args)
        {
            return;
        }

        var delta = args.Delta;
        var oldZoom = Zoom;
        Zoom = Math.Clamp(Zoom + delta * .001, MinZoom, MaxZoom);

        Log.Debug("Delta {Delta}, Zoom went from {OldZoom} to {NewZoom}", delta, oldZoom, Zoom);
    }

    #endregion Commands

    #region Public Methods

    public bool OpenImage (ImageData? image)
    {
        if (image is null)
        {
            return false;
        }

        Image = image;
        return true;
    }

    public void ResetControls ()
    {
        if (Bitmap is null)
        {
            return;
        }

        CenterX = CenterY = .5;
        Zoom = 1;
    }

    #endregion Public Methods

    #region Private Methods

    private ObservableCollection<IImageTool> GetTools()
    {
        var type = typeof(IImageTool);
        var tools = AppDomain.CurrentDomain.GetAssemblies().AsParallel()
            .SelectMany(s => s.GetTypes())                                      //Flatten the types from each assemly
            .Where(p => type.IsAssignableFrom(p) && p.IsClass)                  //Get only classes matching the interface
            .Select(t => t.GetConstructor([])?.Invoke([]) as IImageTool)        //Create the constructors
            .Where(t => t is not null).ToArray();                               //Filter out the null constructs and force enumeration

        foreach(var tool in tools)
        {
            if (tool is null) continue;
            tool.PropertyChanged += Tool_PropertyChanged;
        }
        var toolViews = tools.Select(t => t?.View).Where(v => v is not null).ToArray();

        var toolViewVMPairs = Application.Current.Dispatcher.Invoke(() =>       //Have to create ObservableCollections on the MainThread
        {
            return (Views: new ObservableCollection<UserControl>(toolViews!),
                    ViewModels: new ObservableCollection<IImageTool>(tools!));
        });
        ImageToolViews = toolViewVMPairs.Views;
        return toolViewVMPairs.ViewModels;
    }

    private void Tool_PropertyChanged (object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (sender is not IImageTool tool || tool.Filter is null) return;
        if (Image?.GetData() is not Bitmap bitmap) return;

        using Mat dst = new();
        tool.Filter(bitmap.ToMat(), dst);
        Bitmap = new(dst.ToBitmapSource());
    }

    private void CreateNewBitmap (Image? image)
    {
        if (image is not Bitmap bitmap || image.Width <= 0 || image.Height <= 0) return;

        //Bitmap = image.ToWriteableBitmap();
        Bitmap = new(bitmap.ToMat().ToBitmapSource());
    }

    #endregion Private Methods
}