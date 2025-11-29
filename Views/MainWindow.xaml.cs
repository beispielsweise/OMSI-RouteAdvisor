using OMSI_RouteAdvisor.Readers;
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
            ChangeStatusVisibility(true);

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
                    System.Windows.MessageBox.Show("Cannot load the map. Make sure to generate roadmap first. Wrong folder?",
                                           "Warning",
                                           MessageBoxButton.OK,
                                           MessageBoxImage.Error);
                }
                finally
                {
                    ChangeStatusVisibility(false);
                }
            }
        }
        private void ChooseMapFolderBtn2_Click(object sender, RoutedEventArgs e)
        {
            ChangeStatusVisibility(true);

            var dialogFolder = new System.Windows.Forms.FolderBrowserDialog();
            var result = dialogFolder.ShowDialog();

            if (result == System.Windows.Forms.DialogResult.OK)
            {
                string selectedMapPath = dialogFolder.SelectedPath;
               
                try
                {
                    MapImageReader.GenerateRoadmap(selectedMapPath);
                } catch
                {
                    System.Windows.MessageBox.Show("Wasn't able to generate roadmap. Wrong folder?",
                                           "Warning",
                                           MessageBoxButton.OK,
                                           MessageBoxImage.Error);
                }
                finally
                {
                    ChangeStatusVisibility(false);
                }
            }
        }

        private void ChangeStatusVisibility(bool isVisible)
        {
            if (isVisible)
            {
                StatusLabel.Visibility = Visibility.Visible;
                StatusLabel.UpdateLayout();
                return;
            }
            StatusLabel.Visibility = Visibility.Collapsed;
            StatusLabel.UpdateLayout();
        }
    }
}
