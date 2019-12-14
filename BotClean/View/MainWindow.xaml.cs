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
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainViewModel();
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
    }
}
