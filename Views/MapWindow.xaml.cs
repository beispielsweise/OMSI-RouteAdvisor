using OMSI_RouteAdvisor.Map;
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
using Brushes = System.Windows.Media.Brushes;
using Brush = System.Windows.Media.Brush;
using OMSI_RouteAdvisor.Views.Misc;
using OMSI_RouteAdvisor.Readers;
using System.Diagnostics.Eventing.Reader;
using System.Windows.Ink;
using Rectangle = System.Windows.Shapes.Rectangle;

namespace OMSI_RouteAdvisor.Views
{
    /// <summary>
    /// Interaction logic for MapWindow.xaml
    /// </summary>
    public partial class MapWindow : Window
    {
        private MapData _mapData;
        private TranslateTransform _moveTransform;
        private ScaleTransform _scaleTransform;
        double _zoom = 0;
        private bool _isWindowFixed = false;
        private bool _isGameInjected = false;
        private MapRenderer _mapRenderer;

        public MapWindow(string mapFolderPath)
        {
            InitializeComponent();

            try
            {
                _mapData = new(mapFolderPath);
            }
            catch 
            {
                throw new Exception("Generate roadmap please! Check if the map folder is correct!");   
            }

            _moveTransform = new TranslateTransform();
            _scaleTransform = new ScaleTransform();
            SetupMapMovement();

            this.Width = 800;
            this.Height = 800;

            _mapRenderer = new(this.MapBackground, this.MapCanvas, this.BusStopsLayer, _mapData);
        }

        /// <summary>
        /// Initialises Map movement variables
        /// </summary>
        private void SetupMapMovement()
        {
            _scaleTransform = new ScaleTransform(1.0, 1.0);
            _zoom = _scaleTransform.ScaleX;
            _moveTransform = new TranslateTransform();

            TransformGroup group = new TransformGroup();
            group.Children.Add(_scaleTransform);
            group.Children.Add(_moveTransform);

            MapCanvas.RenderTransform = group;
        }

        /// <summary>
        /// Changes the zoom of the map with a slider
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MapZoomSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (_scaleTransform != null)
            {
                _scaleTransform.ScaleX = MapZoomSlider.Value;
                _scaleTransform.ScaleY = MapZoomSlider.Value;
                _zoom = _scaleTransform.ScaleX;
            }
        }

        /// <summary>
        /// Changes the window to be unresizible unmovable and topmost with a checkbox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FixWindowPositionCheckbox_Click(object sender, RoutedEventArgs e)
        {
            _isWindowFixed = !_isWindowFixed;

            if (_isWindowFixed)
            {
                this.ResizeMode = ResizeMode.NoResize;
                WindowBorder.BorderThickness = new Thickness(0);
                ChangeWindowZ.MakeTopmost(this);
            } else
            {
                this.ResizeMode= ResizeMode.CanResizeWithGrip;
                WindowBorder.BorderThickness = new Thickness(20);
                ChangeWindowZ.RevertTopmost(this);
            }
        }
        
        /// <summary>
        /// Changes the inject state, which activates reading from Game memory
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void InjectGameCheckbox_Click(object sender, RoutedEventArgs e)
        {
            _isGameInjected = !_isGameInjected;

            if (_isGameInjected)
            {
                try
                {
                    _mapRenderer.InjectGame();    
                }
                catch
                {
                    System.Windows.MessageBox.Show("Please open the game before injecting.\nIf you still see the error, open an issue on github please",
                        "Warning",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
                    _isGameInjected = false;
                    _mapRenderer.SetBusPositionVisible(false);
                    InjectGameCheckbox.IsChecked = false;
                    return;
                }

                _mapRenderer.SetBusPositionVisible();
                StartUpdateLoop();
            } else
            {
                _mapRenderer.DisableBusStopHighlighting();
                _mapRenderer.SetBusPositionVisible(false);
            }
                
        }

        /// <summary>
        /// Closes the Map and returns to the <c>MainWindow</c>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CloseMapButton_Click(object sender, RoutedEventArgs e)
        {
            _isGameInjected = false;

            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }

        /// <summary>
        /// Allows the window to be moved on a click and hold action
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WindowBorder_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed && !_isWindowFixed)
                this.DragMove();
        }

        /// <summary>
        /// Starts Update loop to read the Game Memory
        /// </summary>
        private async void StartUpdateLoop()
        {
            while (_isGameInjected)
            {
                _mapRenderer.UpdateNextStop();
                double busX, busY;
                (busX, busY) = _mapRenderer.GetAndUpdateBusPosition();
                CenterMap(busX, busY);

                await Task.Delay(1000);
            }
        }

        /// <summary>
        /// CentersMap on a given coordinate
        /// </summary>
        private void CenterMap(double x, double y)
        {
            _moveTransform.X = (this.ActualWidth / 2) - (x * _zoom);
            _moveTransform.Y = (this.ActualHeight / 2) - (y * _zoom);
        }
    }
}
