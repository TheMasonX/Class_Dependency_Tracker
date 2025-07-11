using System;
using System.Windows.Media;

using CommunityToolkit.Mvvm.ComponentModel;

namespace VideoGenerator.Models
{
    public interface IAppStatus
    {
        string Status { get; }
        bool Notify { get; }
        Brush TextColor { get; }
        Brush BGColor { get; }
        bool Update (object data);
        void Hide ();
    }

    public class TextStatus : ObservableObject, IAppStatus
    {

        #region Properties

        private string? _status;
        public string Status
        {
            get => _status ??= "";
            set => SetProperty(ref _status, value);
        }

        private bool _notify;
        public bool Notify
        {
            get => _notify;
            private set => SetProperty(ref _notify, value);
        }

        private Brush _textColor = Brushes.Black;
        public Brush TextColor
        {
            get => _textColor;
            set => SetProperty(ref _textColor, value);
        }

        private Brush _bgColor = Brushes.Transparent;
        public Brush BGColor
        {
            get => _bgColor;
            set => SetProperty(ref _bgColor, value);
        }

        public void Hide ()
        {
            Notify = false;
        }

        public bool Update (object data)
        {
            return false;
        }

        #endregion Properties
    }


    public class LoadingStatus : ObservableObject, IAppStatus, IDisposable
    {
        private readonly string _itemLabel = "Item";
        private readonly string _itemLabelPlural = "Items";
        private int _currentCount = 0;
        private readonly object _lock = new();

        public LoadingStatus (int startCount, int totalCount, string itemLabel, string itemLabelPlural)
        {
            lock (_lock)
            {
                _currentCount = StartCount = startCount;
                TotalCount = totalCount;
                _itemLabel = itemLabel;
                _itemLabelPlural = itemLabelPlural;
                ProgressPercent = 0;
                Notify = true;
            }
        }

        public void Dispose ()
        {
            GC.SuppressFinalize(this);
        }


        #region Properties

        private int _startCount;
        public int StartCount
        {
            get => _startCount;
            set => SetProperty(ref _startCount, value);
        }

        private int _totalCount;
        public int TotalCount
        {
            get => _totalCount;
            set => SetProperty(ref _totalCount, value);
        }

        private string? _status;
        public string Status
        {
            get => _status ??= "";
            set => SetProperty(ref _status, value);
        }

        private bool _notify;
        public bool Notify
        {
            get => _notify;
            set => SetProperty(ref _notify, value);
        }

        private float _progressPercent;
        public float ProgressPercent
        {
            get => _progressPercent;
            set => SetProperty(ref _progressPercent, value);
        }

        private Brush _textColor = Brushes.Black;
        public Brush TextColor
        {
            get => _textColor;
            set => SetProperty(ref _textColor, value);
        }

        private Brush _bgColor = Brushes.Transparent;
        public Brush BGColor
        {
            get => _bgColor;
            set => SetProperty(ref _bgColor, value);
        }

        #endregion Properties

        #region Commands

        //Commands

        #endregion Commands

        #region Public Methods

        public bool Update (object data)
        {
            int loadingCount = _totalCount - _startCount;
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
                ProgressPercent = (_currentCount - _startCount) / (float)loadingCount;
                if (ProgressPercent >= 1.0)
                {
                    Status = "Complete";
                    BGColor = Brushes.Green;
                }
                else
                {
                    Status = $"{ProgressPercent:P1} Complete : Loaded {_currentCount - _startCount}/{loadingCount} {(_currentCount > 1 ? _itemLabelPlural : _itemLabel)}";
                }
            }

            TextColor = Brushes.Black;
            return true;
        }

        public void Hide ()
        {
            Notify = false;
        }

        #endregion Public Methods

        #region Private Methods

        //Private Methods

        #endregion Private Methods
    }
}