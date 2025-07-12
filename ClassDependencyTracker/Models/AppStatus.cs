using System;
using System.Windows.Media;

using CommunityToolkit.Mvvm.ComponentModel;

namespace ClassDependencyTracker.Models;

public interface IAppStatus
{
    string Status { get; }
    bool Notify { get; }
    Brush TextColor { get; }
    Brush BGColor { get; }
    float ProgressPercent { get; set; }
    bool ShowProgress { get; }

    bool Update(object? data);
    void Hide();
}

public partial class TextStatus : ObservableObject, IAppStatus
{
    public TextStatus(string message = "")
    {
        Status = message;
        Notify = true;
        Task.Run(() =>
        {
            Task.Delay(HideAfter).Wait();
            Hide();
        });
    }

    public static TextStatus Success(string message)
    {
        return new TextStatus(message)
        {
            BGColor = Brushes.LightGreen,
        };
    }

    public static TextStatus Error(string message)
    {
        Console.Beep();
        return new TextStatus(message)
        {
            BGColor = Brushes.OrangeRed,
        };
    }

    #region Properties

    public TimeSpan HideAfter { get; set; } = TimeSpan.FromSeconds(2);

    private string? _status;
    public string Status
    {
        get => _status ??= "";
        set => SetProperty(ref _status, value);
    }

    [ObservableProperty]
    private bool _notify;

    [ObservableProperty]
    private bool _showProgress;

    [ObservableProperty]
    private float _progressPercent;

    [ObservableProperty]
    private Brush _textColor = Brushes.Black;

    [ObservableProperty]
    private Brush _BGColor = Brushes.WhiteSmoke;

    #endregion Properties

    public void Hide()
    {
        Notify = false;
    }

    public bool Update(object? data)
    {
        return false;
    }
}


public partial class LoadingStatus : ObservableObject, IAppStatus, IDisposable
{
    private readonly string _itemLabel = "Item";
    private readonly string _itemLabelPlural = "Items";
    private int _currentCount = 0;
    private readonly object _lock = new();

    public LoadingStatus(int startCount, int totalCount, string itemLabel, string itemLabelPlural)
    {
        lock (_lock)
        {
            _currentCount = StartCount = startCount;
            TotalCount = totalCount;
            _itemLabel = itemLabel;
            _itemLabelPlural = itemLabelPlural;
            ProgressPercent = 0;
            ShowProgress = true;
            Notify = true;
        }
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }


    #region Properties

    [ObservableProperty]
    private int _startCount;

    [ObservableProperty]
    private int _totalCount;

    private string? _status;
    public string Status
    {
        get => _status ??= "";
        set => SetProperty(ref _status, value);
    }

    [ObservableProperty]
    private bool _notify;

    [ObservableProperty]
    private bool _showProgress;

    [ObservableProperty]
    private float _progressPercent;

    [ObservableProperty]
    private Brush _textColor = Brushes.Black;

    [ObservableProperty]
    private Brush _BGColor = Brushes.Transparent;

    #endregion Properties

    #region Commands

    //Commands

    #endregion Commands

    #region Public Methods

    public bool Update(object? data)
    {
        int loadingCount = TotalCount - StartCount;
        if (data is not int value || loadingCount <= 0)
        {
            Status = "Invalid Status";
            TextColor = Brushes.Black;
            BGColor = Brushes.Red;
            return false;
        }
        lock (_lock)
        {
            _currentCount += value;
            ProgressPercent = (_currentCount - StartCount) / (float)loadingCount;
            if (ProgressPercent >= 1.0)
            {
                Status = "Complete";
                BGColor = Brushes.Green;
            }
            else
            {
                Status = $"{ProgressPercent:P1} Complete : Loaded {_currentCount - StartCount}/{loadingCount} {(_currentCount > 1 ? _itemLabelPlural : _itemLabel)}";
            }
        }

        TextColor = Brushes.Black;
        return true;
    }

    public void Hide()
    {
        Notify = false;
    }

    #endregion Public Methods

    #region Private Methods

    //Private Methods

    #endregion Private Methods
}