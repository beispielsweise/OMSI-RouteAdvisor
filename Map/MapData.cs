using OMSI_RouteAdvisor.Helpers;
using OMSI_RouteAdvisor.Readers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace OMSI_RouteAdvisor.Map
{
    /// <summary>
    /// Contains general data of the currently opened map
    /// </summary>
    public class MapData
    {
        public readonly string MapFolderPath = "";
        private readonly string _bmpImagePath = "\\texture\\\\map\\\\whole.roadmap.bmp";
        public BitmapSource BackgroundMapImg { get; set; }
        public Dictionary<int, Tile> Tiles { get; set; }
        public Dictionary<int, BusStop> BusStops { get; set; }
        public double TileSize { get; set; }
        public double MinGridX { get; set; }
        public double MinGridY { get; set; }
        public double MaxGridX { get; set; }
        public double MaxGridY { get; set; }
        public double WorldWidth { get; set; }
        public double WorldHeight { get; set; }
        public double ScaleFactor { get; set; } // Difference between Game World width and Local Image width

        public MapData(string mapFolderPath)
        {
            MapFolderPath = mapFolderPath;

            Tiles = new Dictionary<int, Tile>();

            BitmapSource backgroundMapImg;
            try
            {
                backgroundMapImg = new BitmapImage(new Uri(this.MapFolderPath + _bmpImagePath));
            } catch
            {
                throw new Exception("No wholemap image");
            }
            backgroundMapImg = ImageModifier.MakeTransparent(backgroundMapImg);
            BackgroundMapImg = backgroundMapImg;

            BusStops = new Dictionary<int, BusStop>();

            MapDataReader.ScanGlobalTiles(this);
            MapDataReader.CheckTileSize(this);

            (double minX, double minY, double maxX, double maxY) = CoordinatesConverter.GetGridMinMaxValues(this);
            MinGridX = minX;
            MinGridY = minY;
            MaxGridX = maxX;
            MaxGridY = maxY;
            (double worldWidth, double worldHeight) = CoordinatesConverter.GetWorldSize(this);
            WorldWidth = worldWidth;
            WorldHeight = worldHeight;
            double scaleFactor = CoordinatesConverter.GetScaleFactor(this);
            ScaleFactor = scaleFactor;
            CoordinatesConverter.BusStopsToLocal(this);
        }
    }
}
