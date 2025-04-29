using OMSI_RouteAdvisor.Map;
using OMSI_RouteAdvisor.Readers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using Brushes = System.Windows.Media.Brushes;
using Brush = System.Windows.Media.Brush;
using System.Windows.Shapes;
using System.Windows;
using System.Windows.Media.Imaging;

namespace OMSI_RouteAdvisor.Views.Misc
{
    /// <summary>
    /// A class responsible for drawing and changing all elements on the screen of MapWindow. Contains logic for Element Updating
    /// </summary>
    internal class MapRenderer
    {
        private readonly System.Windows.Controls.Image _mapBackground;
        private readonly Canvas _mapCanvas;
        private readonly Canvas _busStopsLayer;
        private readonly Canvas _directionArrow;
        private readonly RotateTransform _directionArrowRotation;
        private readonly Dictionary<int, Ellipse> _busStopPositions;
        private Ellipse _busPosition;
        private GameMemoryReader? _gameMemoryReader;
        private readonly MapData _mapData;

        private readonly Brush _active = Brushes.Red;
        private readonly Brush _passive = Brushes.Green;
        private readonly Brush _bus = Brushes.Blue;

        internal MapRenderer(System.Windows.Controls.Image mapBackground,
            Canvas mapCanvas,
            Canvas busStopsLayer,
            Canvas directionArrow,
            RotateTransform directionArrowRotation,
            MapData mapData)
        {
            _mapBackground = mapBackground;
            _mapCanvas = mapCanvas;
            _busStopsLayer = busStopsLayer;
            _directionArrow = directionArrow;
            _directionArrowRotation = directionArrowRotation;
            _busStopPositions = [];
            _busPosition = new();
            _mapData = mapData;
            _gameMemoryReader = null;

            LoadMapBmp(_mapData.BackgroundMapImg);
            DrawBusStops();
            DrawBusCircle();
        }

        /// <summary>
        /// Loads Current Map background image
        /// </summary>
        private void LoadMapBmp(BitmapSource bmpSource)
        {
            _mapBackground.Source = bmpSource;
        }

        /// <summary>
        /// Draw all bus stops during initialisation
        /// </summary>
        private void DrawBusStops()
        {
            foreach (KeyValuePair<int, BusStop> busStop in _mapData.BusStops)
            {
                Ellipse busStopCircle = new()
                {
                    Width = 15,
                    Height = 15,
                    Stroke = _passive,
                    StrokeThickness = 4,
                    Fill = Brushes.Transparent
                };

                Canvas.SetLeft(busStopCircle, busStop.Value.LocalX - 5); // Hardcode to insure more accurate display position
                Canvas.SetTop(busStopCircle, busStop.Value.LocalY - 5);

                _busStopsLayer.Children.Add(busStopCircle);
                _busStopPositions.Add(busStop.Key, busStopCircle);
            }
        }

        /// <summary>
        /// Draws Bus current position Circle during initialisation
        /// </summary>
        private void DrawBusCircle()
        {
            Ellipse busCircle = new()
            {
                Width = 10,
                Height = 10,
                Stroke = _bus,
                StrokeThickness = 2,
                Fill = _bus
            };

            Canvas.SetLeft(busCircle, 0); // Spawn bus circle out of bounds
            Canvas.SetTop(busCircle, 0);
            busCircle.Visibility = Visibility.Collapsed;

            _busStopsLayer.Children.Add(busCircle);
            _busPosition = busCircle;
        }

        /// <summary>
        /// Initialise GameMemoryReader to update map values
        /// </summary>
        /// <exception cref="Exception"></exception>
        public void InjectGame()
        {
            try
            {
                _gameMemoryReader = new();
            } catch
            {
                throw new Exception("Game is not running!");
            }
        }

        /// <summary>
        /// Updates Next Stop position
        /// </summary>
        public int UpdateNextStop()
        {
            int id = _gameMemoryReader!.GetNextBusStopId();
            if (id == 0 || _gameMemoryReader.PreviousBusStopId == id)
                return id;

            if (!_busStopPositions.TryGetValue(id, out var _))
                return id;

            _busStopPositions[id].Stroke = _active;
            _busStopPositions[id].Fill = _active;
            System.Windows.Controls.Panel.SetZIndex(_busStopPositions[id], 10);

            if (_busStopPositions.TryGetValue(_gameMemoryReader.PreviousBusStopId, out var busStop)) {
                busStop.Stroke = _passive;
                busStop.Fill = Brushes.Transparent;
                System.Windows.Controls.Panel.SetZIndex(_busStopPositions[_gameMemoryReader.PreviousBusStopId], 0);
            }

            _gameMemoryReader.PreviousBusStopId = id;
            return id;
        }

        /// <summary>
        /// Updates current Bus Position
        /// </summary>
        public (double busX, double busY) GetAndUpdateBusPosition()
        {
            double busX, busY;
            (busX, busY) = _gameMemoryReader!.GetCurrentBusPosition(_mapData);

            Canvas.SetLeft(_busPosition, busX - 5);
            Canvas.SetTop(_busPosition, busY - 5);

            return (busX, busY);
        }

        public void UpdateDirectionArrow(int nextBusStopId, double busX, double busY)
        {
            if (!_mapData.BusStops.TryGetValue(nextBusStopId, out var busStop)) {
                return;
            }

            double deltaX = busStop.LocalX - busX;
            double deltaY = busStop.LocalY - busY;
            double angle = Math.Atan2(deltaY, deltaX) * (180.0 / Math.PI);
            angle = angle + 90;

            _directionArrowRotation.Angle = angle;
        }

        /// <summary> 
        /// Disables the current bus stop highlighting 
        /// </summary>
        public void DisableBusStopHighlighting()
        {
            if (_busStopPositions.TryGetValue(_gameMemoryReader!.PreviousBusStopId, out var busStop)) {
                busStop.Stroke = _passive;
                busStop.Fill = Brushes.Transparent;
            }
        }

        /// <summary>
        /// Sets Bus circle element visible or invisible
        /// </summary>
        /// <param name="visible">Default true, element visible. False will set the element invisible</param>
        public void SetBusPositionVisible(bool visible = true)
        {
            if (!visible)
            {
                _busPosition.Visibility = Visibility.Collapsed;
                return;
            }
            
            _busPosition.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// Sets DirectionArrow Visibility
        /// </summary>
        /// <param name="visible">Default true, element visible. False will set the element invisible</param>
        public void SetDirectionArrowVisible(bool visible = true)
        {
            if (!visible)
            {
                _directionArrow.Visibility = Visibility.Hidden;
                return;
            }

            _directionArrow.Visibility = Visibility.Visible;
        }
    }
}
