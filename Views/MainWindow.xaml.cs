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

namespace OMSI_RouteAdvisor.Views
{
    /// <summary>
    /// App starting page. User can choose the map folder he will play with
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Function calls FolderBrowserDialog and created "MapWindow". 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ChooseMapFolderBtn_Click(object sender, RoutedEventArgs e)
        {
            var dialogFolder = new System.Windows.Forms.FolderBrowserDialog();
            var result = dialogFolder.ShowDialog();

            if (result == System.Windows.Forms.DialogResult.OK)
            {
                string selectedMapPath = dialogFolder.SelectedPath;

                try
                {
                    MapWindow mapWindow = new MapWindow(selectedMapPath);
                    mapWindow.Show();
                    this.Close();
                } catch
                {
                    System.Windows.MessageBox.Show("Make sure to generate roadmap first. Choose a correct folder!",
                                           "Warning",
                                           MessageBoxButton.OK,
                                           MessageBoxImage.Error);
                }
            }
        }
    }
}
