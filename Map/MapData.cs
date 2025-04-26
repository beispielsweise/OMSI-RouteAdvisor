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
    public class MapData
    {
        public readonly string MapFolderPath = "";
        public readonly string BmpImagePath = "\\texture\\\\map\\\\whole.roadmap.bmp";

        public BitmapImage BgImg { get; set; }
        public Dictionary<int, Tile> Tiles { get; set; }
        public Dictionary<int, BusStop> BusStops { get; set; }
        public double TileSize { get; set; }
        public double MinGridX { get; set; }
        public double MinGridY { get; set; }
        public double MaxGridX { get; set; }
        public double MaxGridY { get; set; }
        public double WorldWidth { get; set; }
        public double WorldHeight { get; set; }
        public double ScaleFactor { get; set; }

        // Add incorrect folder check!!!
        public MapData(string mapFolderPath)
        {
            MapFolderPath = mapFolderPath;

            Tiles = new Dictionary<int, Tile>();
            BgImg = new BitmapImage(new Uri(this.MapFolderPath + BmpImagePath));
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
