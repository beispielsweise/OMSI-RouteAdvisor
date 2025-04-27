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

namespace OMSI_RouteAdvisor.Views
{
    /// <summary>
    /// Interaction logic for MapWindow.xaml
    /// </summary>
    public partial class MapWindow : Window
    {
        public MapData MapData { get; set; }
        private TranslateTransform MoveTransform { get; set; }
        private ScaleTransform ScaleTransform { get; set; }
        private Brush Active { get; set; }
        private Brush Passive { get; set; }
        private Dictionary<int, Ellipse> BusStopPositions { get; set; }
        private bool _isWindowFixed = false;

        public MapWindow(string mapFolderPath)
        {
            InitializeComponent();

            MapData = new(mapFolderPath);
            Active = Brushes.Green;
            Passive = Brushes.Red;
            BusStopPositions = new Dictionary<int, Ellipse>();

            LoadMapBmp();
            DrawBusStops();
            
            // Empty for now
            SetupMapMovement();
        }

        /// <summary>
        /// Loads Current Map background image
        /// </summary>
        private void LoadMapBmp()
        {
            MapBackground.Source = MapData.BackgroundMapImg;
            this.Width = MapData.BackgroundMapImg.Width;
            this.Height = MapData.BackgroundMapImg.Height;
            MapBackground.Width = MapData.BackgroundMapImg.Width;
            MapBackground.Height = MapData.BackgroundMapImg.Height;
        }

        /// <summary>
        /// Draw all bus stops during initialisation
        /// </summary>
        private void DrawBusStops()
        {
            foreach (KeyValuePair<int, BusStop> busStop in MapData.BusStops)
            {
                // Example: drawing one bus stop manually
                Ellipse busStopCircle = new()
                {
                    Width = 10,
                    Height = 10,
                    Stroke = Active,       
                    StrokeThickness = 2,
                    Fill = Brushes.Transparent                   
                };

                Canvas.SetLeft(busStopCircle, busStop.Value.LocalX - 5); // Hardcode to insure more accurate display position
                Canvas.SetTop(busStopCircle, busStop.Value.LocalY - 5);

                BusStopsLayer.Children.Add(busStopCircle);
                BusStopPositions.Add(busStop.Key, busStopCircle);
            }
        }

        /// <summary>
        /// Initialises Map movement variables
        /// </summary>
        private void SetupMapMovement()
        {
            ScaleTransform = new ScaleTransform(1.0, 1.0);
            MoveTransform = new TranslateTransform();

            TransformGroup group = new TransformGroup();
            group.Children.Add(ScaleTransform);
            group.Children.Add(MoveTransform);

            MapCanvas.RenderTransform = group;
        }

        /// <summary>
        /// Changes the zoom of the map with a slider
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MapZoomSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (ScaleTransform != null)
            {
                ScaleTransform.ScaleX = MapZoomSlider.Value;
                ScaleTransform.ScaleY = MapZoomSlider.Value;
            }
        }

        /// <summary>
        /// Changes the window to be unresizible unmovable and topmost with a checkbox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FixWindowPositionButton_Click(object sender, RoutedEventArgs e)
        {
            _isWindowFixed = !_isWindowFixed;

            if (_isWindowFixed)
            {
                this.ResizeMode = ResizeMode.NoResize;
                ChangeWindowZ.MakeTopmost(this);
            } else
            {
                this.ResizeMode= ResizeMode.CanResize;
                ChangeWindowZ.RevertTopmost(this);
            }
        }

        /// <summary>
        /// Closes the Map and returns to the <c>MainWindow</c>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CloseMapButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }

        /// <summary>
        /// CentersMap on a given coordinate
        /// </summary>
        private void CenterMap(double x, double y)
        {
            double centerX = (MapBackground.Width * ScaleTransform.ScaleX) / 2.0;
            double centerY = (MapBackground.Height * ScaleTransform.ScaleY) / 2.0;

            MoveTransform.X = -(centerX - (this.ActualWidth / 2.0));
            MoveTransform.Y = -(centerY - ((this.ActualHeight - FixWindowPositionButton.ActualHeight) / 2.0));
        }

        /// <summary>
        /// Allows the window to be moved on a click and hold action
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RootGrid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed && !_isWindowFixed)
                this.DragMove();
        }
    }
}
