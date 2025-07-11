using System;
using System.Windows.Controls;

using VideoGenerator.ViewModels;

namespace VideoGenerator.Views
{
    /// <summary>
    /// Interaction logic for FileGrid.xaml
    /// </summary>
    public partial class FileGrid : UserControl
    {
        public FileGrid ()
        {
            InitializeComponent();
        }

        private void filesGrid_UnloadingRow (object sender, DataGridRowEventArgs e)
        {
            if (e.Row.Item is IDisposable data)
            {
                data.Dispose();
            }

            if (DataContext is FileGridVM vm)
            {
                vm.RefreshCounts();
            }
        }
    }
}
