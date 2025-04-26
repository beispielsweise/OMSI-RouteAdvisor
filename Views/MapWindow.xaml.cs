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
        public MapData mapData { get; set; }
        private TranslateTransform moveTransform = new TranslateTransform();
        private Brush Active { get; set; }
        private Brush Passive { get; set; }


        public MapWindow(string mapFolderPath)
        {
            InitializeComponent();

            mapData = new(mapFolderPath);
            Active = Brushes.Green;
            Passive = Brushes.Red;

            LoadMapBmp();
            DrawBusStops();

            // Empty for now
            SetupMapMovement();
        }

        private void LoadMapBmp()
        {
            MapBackground.Source = mapData.BgImg;
            this.Width = mapData.BgImg.Width;
            this.Height = mapData.BgImg.Height;
            MapBackground.Width = mapData.BgImg.Width;
            MapBackground.Height = mapData.BgImg.Height;
        }

        private void DrawBusStops()
        {
            foreach (KeyValuePair<int, BusStop> busStop in mapData.BusStops)
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

                Canvas.SetLeft(busStopCircle, busStop.Value.LocalX); 
                Canvas.SetTop(busStopCircle, busStop.Value.LocalY);

                BusStopsLayer.Children.Add(busStopCircle);
            }
        }
        private void SetupMapMovement()
        {
            // Set up the TranslateTransform for panning
            MapCanvas.RenderTransform = moveTransform;
        }

        public void CenterMapOnBus(double busPixelX, double busPixelY, double viewportWidth, double viewportHeight)
        {
            // Update map position to center bus
            moveTransform.X = -busPixelX + (viewportWidth / 2);
            moveTransform.Y = -busPixelY + (viewportHeight / 2);
        }

        private void RootGrid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed)
                this.DragMove();
        }
    }
}
