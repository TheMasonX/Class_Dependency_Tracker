using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Media;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using Microsoft.Win32;

using VideoGenerator.Models;
using VideoGenerator.Properties;
using VideoGenerator.Utils.Extensions;

namespace VideoGenerator.ViewModels;

public partial class MainWindowVM : ObservableObject, IDisposable
{
    private const string _openFileDialogTitle = "Select Images To Convert";
    private const string _inputExtensionFilter = "Image files (*.bmp, *.jpg, *.png)|*.bmp;*.jpg;*.png|All files (*.*)|*.*";
    [GeneratedRegex(@"(\.bmp)|(\.jpg)|(\.png)")]
    private static partial Regex GetInputExtensionRegex ();
    public readonly Regex InputExtensionRegex = GetInputExtensionRegex();

    private const string _saveFileDialogTitle = "Save Video File";
    private const string _outputExtensionFilter = "Video File (*.mp4)|*.mp4|All files (*.*)|*.*";
    private const string _outputExtension = ".mp4";

    //public MainWindowVM ()
    public MainWindowVM ()
    {
        //OpenFiles(new string[] { @"C:\Users\TheMasonX\Pictures\uv_test.png" });
        Test();
    }

    private void Test ()
    {
        Task.Run(() =>
        {
            Task.Delay(1000).Wait();
            var photos = new DirectoryInfo(@"C:\Users\TheMasonX\Pictures\");
            var files = photos.GetFiles().Select(f => f.FullName).AsQueryable();
            OpenFiles(files);
        });
    }

    public void Dispose()
    {
        _fileGrid?.Dispose();
        _imageEditor?.Dispose();
        GC.SuppressFinalize(this);
    }


    #region Properties

    private string? _outputFileName;
    public string OutputFileName
    {
        get => _outputFileName ??= "";
        set
        {
            if (SetProperty(ref _outputFileName, value))
            {
                Settings.Default.OutputFileName = value;
                Settings.Default.Save();
            }
        }
    }

    private string? _outputFilePath;
    public string OutputFilePath
    {
        get => _outputFilePath ??= "";
        set
        {
            if (SetProperty(ref _outputFilePath, value))
            {
                Settings.Default.OutputFilePath = value;
                Settings.Default.Save();
            }
        }
    }

    private FileGridVM? _fileGrid;
    public FileGridVM FileGrid
    {
        get => _fileGrid ??= new FileGridVM();
        set => SetProperty(ref _fileGrid, value);
    }

    private ImageEditorVM? _imageEditor;
    public ImageEditorVM ImageEditor
    {
        get => _imageEditor ??= new ImageEditorVM();
        set => SetProperty(ref _imageEditor, value);
    }

    private IAppStatus? _status;
    public IAppStatus? Status
    {
        get => _status;
        set => SetProperty(ref _status, value);
    }

    #endregion Properties

    #region Commands

    [RelayCommand]
    public Task OnOpenFiles ()
    {
        return Task.Run(() =>
        {
            OpenFileDialog openFileDialog = new()
            {
                Title = _openFileDialogTitle,
                RestoreDirectory = true,
                //CheckFileExists = true,
                Multiselect = true,
                Filter = _inputExtensionFilter,
            };
            openFileDialog.ShowDialog();
            OpenFiles(openFileDialog.FileNames.AsQueryable());
        });
    }

    [RelayCommand]
    public Task OnSave ()
    {
        return Task.Run(() =>
        {
            Save();
        });
    }

    [RelayCommand]
    public Task OnSaveAs ()
    {
        return Task.Run(() =>
        {
            SaveAs();
        });
    }

    #endregion Commands

    #region Public Methods

    #region Open

    public void OpenFiles (IQueryable<string>? files)
    {
        files = files?.Where(f => InputExtensionRegex.IsMatch(f));
        if (files is null || !files.Any())
        {
            return;
        }

        int addCount = files.Count();
        Status = new LoadingStatus(FileGrid.Count, FileGrid.Count + addCount, "Item", "Items");

        Task.Run(() =>
        {
            foreach (string file in files)
            {
                if (file.IsNullOrEmpty() || !InputExtensionRegex.IsMatch(file!))
                {
                    continue;
                }

                if (FileGrid.OpenFile(file))
                {
                    Status.Update(1);
                }
                else
                {
                    Status = new TextStatus() { Status = $"Load Error for {file}", BGColor = Brushes.Red };
                }
            }
            Status.Hide();
            FileGrid.Refresh();
        });
    }

    #endregion Open

    public Task Save ()
    {
        if (OutputFilePath.IsNullOrEmpty()) //No path, try save as instead
        {
            return SaveAs();
        }

        return Task.Run(() =>
        {
            if (File.Exists(OutputFilePath))
            {
                File.Delete(OutputFilePath);
            }

            //using (StreamWriter sw = File.CreateText(OutputFilePath))
            //{
            //    sw.Write(OutputText);
            //    sw.Flush();
            //    sw.Close();
            //}
        });
    }

    public Task SaveAs ()
    {
        return Task.Run(() =>
        {
            SaveFileDialog fileDialog = new()
            {
                Title = _saveFileDialogTitle,
                FileName = OutputFileName,
                OverwritePrompt = true,
                AddExtension = true,
                DefaultExt = _outputExtension,
                RestoreDirectory = true,
                Filter = _outputExtensionFilter,
            };
            if (!Settings.Default.OutputDirectory.IsNullOrEmpty())
            {
                fileDialog.InitialDirectory = Settings.Default.OutputDirectory;
            }

            bool? result = fileDialog.ShowDialog();
            if (!result.HasValue || !result.Value)
            {
                return; //Invalid selection
            }

            OutputFilePath = fileDialog.FileName;

            if (!OutputFilePath.IsNullOrEmpty())
            {
                Save(); //Valid FilePath
            }
        });
    }

    #endregion Public Methods

    #region Private Methods

    // Private Methods

    #endregion Private Methods
}