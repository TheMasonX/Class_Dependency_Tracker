using System;

using CommunityToolkit.Mvvm.ComponentModel;

using VideoGenerator.Utils.Video;

using OldColor = System.Drawing.Color;
using NewColor = System.Windows.Media.Color;
using NewColors = System.Windows.Media.Colors;
using ImageFilter = System.Action<OpenCvSharp.Mat, OpenCvSharp.Mat>;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using VideoGenerator.Views;
using System.Linq;
using VideoGenerator.Utils.Extensions;
using System.ComponentModel;
using OpenCvSharp;

namespace VideoGenerator.ViewModels;

public interface IImageTool : INotifyPropertyChanged
{
    string Name { get; }
    string? Description { get; }
    string IconPath { get; }
    ImageFilter? Filter { get; }
    UserControl View { get; }
    string? KeyTip { get; }
}

public class ImageFilterVM : ObservableObject, IImageTool, IDisposable
{
    public ImageFilterVM()
    {

    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }


    #region Properties

    private string? _name;
    public string Name
    {
        get => _name ??= "ImageFilters";
        private set => SetProperty(ref _name, value);
    }

    private string? _description;
    public string? Description
    {
        get => _description;
        private set => SetProperty(ref _description, value);
    }

    private string? _iconPath;
    public string IconPath
    {
        get => _iconPath ??= "";
        private set => SetProperty(ref _iconPath, value);
    }

    private string? _keyTip;
    public string? KeyTip
    {
        get => _keyTip ??= Name.FirstOrDefault().ToString();
        private set => SetProperty(ref _keyTip, value);
    }
    
    private NewColor? _minColor;
    public NewColor MinColor
    {
        get => _minColor ??= NewColors.Black;
        set => SetProperty(ref _minColor, value);
    }

    private NewColor? _maxColor;
    public NewColor MaxColor
    {
        get => _maxColor ??= NewColors.Black;
        set => SetProperty(ref _maxColor, value);
    }

    public ImageFilter? Filter => ApplyFilter;

    private UserControl? _view;
    public UserControl View
    {
        get => _view ??= GetView();
        private set => SetProperty(ref _view, value);
    }

    private ObservableCollection<VideoFilter>? _filterTypes;
    public ObservableCollection<VideoFilter>? FilterTypes
    {
        get => _filterTypes ??= new(Enum.GetValues<VideoFilter>());
        private set => SetProperty(ref _filterTypes, value);
    }

    private VideoFilter _selectedFilterType = VideoFilter.None;
    public VideoFilter SelectedFilterType
    {
        get => _selectedFilterType;
        private set => SetProperty(ref _selectedFilterType, value);
    }

    #endregion Properties

    #region Commands

    //Commands

    #endregion Commands

    #region Public Methods

    //Public Methods

    #endregion Public Methods

    #region Private Methods

    private void ApplyFilter(Mat a, Mat b)
    {
        ImageFilter[] filters = [(a,b) => VideoUtils.MaskColors(a, b, MinColor.Convert(), MaxColor.Convert())];
        VideoUtils.RemoveMasks(a, b, filters);
    }

    private ImageFilterControl GetView ()
    {
        ImageFilterControl uc = new()
        {
            DataContext = this,
        };
        return uc;
    }

    #endregion Private Methods
}