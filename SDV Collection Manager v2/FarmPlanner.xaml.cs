using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Managers;
using WpfAutoGrid;

namespace SDV_Collection_Manager_v2
{
    public partial class FarmPlanner : Window
    {
        public FarmPlanner()
        {
            InitializeComponent();
            buildLayoutOverlay();
        }
        private void buildLayoutOverlay()
        {
            int columns = layoutOverlay.ColumnCount;
            int rows = layoutOverlay.RowCount;
            BitmapImage source = new BitmapImage();
            source.BeginInit();
            source.UriSource = new Uri(@"pack://application:,,,/SDV Collection Manager v2;component/images/calendar/Filler.png");
            source.EndInit();
            for (int c = 0; c < columns; c++)
            {
                for (int r = 0; r < rows; r++)
                {
                    layoutOverlay.Children.Add(new Image { Source = source });
                }
            }
        }
        private void layoutOverlay_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.OriginalSource != null)
            {
                if (e.OriginalSource is Image)
                {
                    if (FarmManager.SelectedTerrain != null)
                    {
                        Image image = e.OriginalSource as Image;
                        BitmapImage source = new BitmapImage();
                        source.BeginInit();
                        source.UriSource = new Uri(@"pack://application:,,,/SDV Collection Manager v2;component/" + FarmManager.SelectedTerrain.Image);
                        source.EndInit();
                        image.Source = source;
                    }
                    else
                    {
                        MessageBox.Show("Please select an object first!", "No object selected", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            OptionsMenu.saveConfig();
            e.Cancel = true;
            Visibility = Visibility.Hidden;
        }
    }
}
