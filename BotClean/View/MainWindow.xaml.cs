using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using BotClean.ViewModel;

namespace BotClean.View
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private MainViewModel _viewModel;
        private MouseEventArgs _mouseEventArgs;

        public MainWindow()
        {
            InitializeComponent();
            _viewModel = new MainViewModel(); ;
            DataContext = _viewModel;
        }

        private void DataGrid_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            DependencyObject dep = (DependencyObject)e.OriginalSource;

            var counter = 0;
            while ((dep != null) && !(dep is DataGridCell) && !(dep is DataGridColumnHeader))
            {
                dep = VisualTreeHelper.GetParent(dep);
                counter++;
                if(counter > 500) break;
            }

            if (dep == null)
                return;

            if (dep is DataGridCell)
            {
                DataGridCell cell = dep as DataGridCell;
                var cellTextBlock = (TextBlock)(cell.Content);
                if (cellTextBlock == null) return;
                var cellText = cellTextBlock.Text;
                if (!cellText.Contains("http://")) return;
                System.Diagnostics.Process.Start(cellText);
            }
        }

        private void OpenContextMenu(object sender, MouseEventArgs mouseEventArgs)
        {
            _mouseEventArgs = mouseEventArgs;
            var cm = FindResource("ContextMenuItems") as ContextMenu;
            if (cm == null) return;
            cm.PlacementTarget = sender as Button;
            cm.IsOpen = true;
        }

        private void Delete(object sender, RoutedEventArgs e)
        {
            if(_mouseEventArgs == null) return;

            DependencyObject dep = (DependencyObject)_mouseEventArgs.OriginalSource;

            var counter = 0;
            while ((dep != null) && !(dep is DataGridCell) && !(dep is DataGridColumnHeader))
            {
                dep = VisualTreeHelper.GetParent(dep);
                counter++;
                if (counter > 500) break;
            }

            if (dep == null)
                return;

            if (dep is DataGridCell)
            {
                var parent = VisualTreeHelper.GetParent(dep) as DataGridCellsPanel;
                var children = parent.Children;
                DataGridCell cell = children[3] as DataGridCell;
                var cellTextBlock = (TextBlock)(cell.Content);
                if (cellTextBlock == null) return;
                var cellText = cellTextBlock.Text;
                if (!cellText.Contains("http://")) return;

                foreach (var post in _viewModel.Posts)
                {
                    if (cellText != post.Name && cellText != post.NameLink && cellText != post.PostLink && cellText != post.PostText) continue;
                    _viewModel.Posts.Remove(post);
                    break;
                }
            }
        }
    }
}
