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
        double _zoom = 0;

        private Brush Active { get; set; }
        private Brush Passive { get; set; }
        private Brush Bus { get; set; }
        private Dictionary<int, Ellipse> BusStopPositions { get; set; }
        private Ellipse BusPosition { get; set; }
        private bool _isWindowFixed = false;
        private bool _isGameInjected = false;
        private GameMemoryReader? GameMemoryReader { get; set; }

        public MapWindow(string mapFolderPath)
        {
            InitializeComponent();

            MapData = new(mapFolderPath);
            Active = Brushes.Red;
            Passive = Brushes.Green;
            Bus = Brushes.Blue;
            BusStopPositions = new Dictionary<int, Ellipse>();

            LoadMapBmp();
            DrawBusStops();
            
            MoveTransform = new TranslateTransform();
            ScaleTransform = new ScaleTransform();
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
                Ellipse busStopCircle = new()
                {
                    Width = 10,
                    Height = 10,
                    Stroke = Passive,       
                    StrokeThickness = 2,
                    Fill = Brushes.Transparent                   
                };

                Canvas.SetLeft(busStopCircle, busStop.Value.LocalX - 5); // Hardcode to insure more accurate display position
                Canvas.SetTop(busStopCircle, busStop.Value.LocalY - 5);

                BusStopsLayer.Children.Add(busStopCircle);
                BusStopPositions.Add(busStop.Key, busStopCircle);
            }

            Ellipse busCircle = new()
            {
                Width = 8,
                Height = 8,
                Stroke = Bus,
                StrokeThickness = 2,
                Fill = Bus 
            };

            Canvas.SetLeft(busCircle, 10000); // Spawn bus circle out of bounds
            Canvas.SetTop(busCircle, 10000);

            BusStopsLayer.Children.Add(busCircle);
            BusPosition = busCircle;
        }

        /// <summary>
        /// Initialises Map movement variables
        /// </summary>
        private void SetupMapMovement()
        {
            ScaleTransform = new ScaleTransform(1.0, 1.0);
            _zoom = ScaleTransform.ScaleX;
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
                _zoom = ScaleTransform.ScaleX;
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
                ChangeWindowZ.MakeTopmost(this);
            } else
            {
                this.ResizeMode= ResizeMode.CanResize;
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
                    GameMemoryReader = new GameMemoryReader();
                } catch
                {
                    System.Windows.MessageBox.Show("Please open the game before injecting.\nIf you still see the error, open an issue on github please",
                        "Warning",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
                    _isGameInjected = false;
                    InjectGameCheckbox.IsChecked = false;
                    return;
                }

                StartUpdateLoop();
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
        private void RootGrid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
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
                UpdateBusPosition();
                UpdateNextStop();

                await Task.Delay(1200);
            }
        }

        /// <summary>
        /// Updates Next Stop position
        /// </summary>
        private void UpdateNextStop()
        {
            int id = GameMemoryReader.GetNextBusStopId();
            if (id == 0 || GameMemoryReader.PreviousBusStopId == id)
                return;

            BusStopPositions[id].Stroke = Active;
            BusStopPositions[id].Fill = Active;

            try
            {
                BusStopPositions[GameMemoryReader.PreviousBusStopId].Stroke = Passive;
                BusStopPositions[GameMemoryReader.PreviousBusStopId].Fill = Brushes.Transparent;
            }
            catch { }

            GameMemoryReader.PreviousBusStopId = id;
        }

        /// <summary>
        /// Updates current Bus Position
        /// </summary>
        private void UpdateBusPosition()
        {
            double busX, busY;
            (busX, busY) = GameMemoryReader.GetCurrentBusPosition(MapData);

            Canvas.SetLeft(BusPosition, busX - 5);
            Canvas.SetTop(BusPosition, busY - 5);

            CenterMap(busX, busY);
        }

        /// <summary>
        /// CentersMap on a given coordinate
        /// </summary>
        private void CenterMap(double x, double y)
        {
            MoveTransform.X = (this.ActualWidth / 2) - (x * _zoom);
            MoveTransform.Y = (this.ActualHeight / 2) - (y * _zoom);
        }
    }
}
