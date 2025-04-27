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

namespace OMSI_RouteAdvisor.Views
{
    /// <summary>
    /// Interaction logic for MapWindow.xaml
    /// </summary>
    public partial class MapWindow : Window
    {
        public MapData MapData { get; set; }
        private TranslateTransform MoveTransform { get; set; }
        private Brush Active { get; set; }
        private Brush Passive { get; set; }
        private Dictionary<int, Ellipse> BusStopPositions { get; set; }

        public MapWindow(string mapFolderPath)
        {
            InitializeComponent();

            MapData = new(mapFolderPath);
            MoveTransform = new(); 
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

        //
        private void SetupMapMovement()
        {
            // Set up the TranslateTransform for panning
            MapCanvas.RenderTransform = MoveTransform;
        }

        //
        public void CenterMapOnBus(double busPixelX, double busPixelY, double viewportWidth, double viewportHeight)
        {
            // Update map position to center bus
            MoveTransform.X = -busPixelX + (viewportWidth / 2);
            MoveTransform.Y = -busPixelY + (viewportHeight / 2);
        }

        // Move window on click and hold
        private void RootGrid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed)
                this.DragMove();
        }
    }
}
