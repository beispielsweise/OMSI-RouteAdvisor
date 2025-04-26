using OMSI_RouteAdvisor.Map;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OMSI_RouteAdvisor.Readers
{
    class MapDataReader
    {
        public static void ScanGlobalTiles(MapData mapData)
        {
            string[] lines = File.ReadAllLines(mapData.MapFolderPath+ "\\global.cfg");

            for (int i = 0; i < lines.Length; i++)
            {
                if (lines[i].Trim().Equals("[map]", StringComparison.OrdinalIgnoreCase))
                {
                    int gridX = int.Parse(lines[i + 1].Trim());
                    int gridY = int.Parse(lines[i + 2].Trim());
                    Tile tile = new Tile(gridX, gridY);
                    mapData.Tiles.Add(i, tile);

                    GetTileBusStops(mapData, tile.GetTileFilename(), i);
                }
            }
        }

        private static void GetTileBusStops(MapData mapData, string tileFilename, int parentTileId)
        {
            string[] lines;
            try
            {
                lines = File.ReadAllLines(mapData.MapFolderPath + "\\" + tileFilename);
            }
            catch
            {
                new Exception("No such tile with name " + tileFilename + " exists...");
                return;
            }

            for (int i = 0; i < lines.Length; i++)
            {
                if (lines[i].Trim().Equals("[object]", StringComparison.OrdinalIgnoreCase))
                {
                    string objectType = lines[i + 2].Trim();
                    if (objectType.EndsWith("bus_stop.sco", StringComparison.OrdinalIgnoreCase))
                    {
                        string isTrainStationStr = lines[i + 14].Trim();
                        if (isTrainStationStr.Equals(""))
                            return;

                        mapData.BusStops.Add(int.Parse(lines[i + 3]), new BusStop(int.Parse(lines[i + 3]),
                        parentTileId,
                        double.Parse(lines[i + 4], CultureInfo.InvariantCulture),
                        double.Parse(lines[i + 5], CultureInfo.InvariantCulture),
                        lines[i + 11])
                        );
                    }
                }
            }
        }

        public static void CheckTileSize(MapData mapData)
        {
            string[] lines = File.ReadAllLines(mapData.MapFolderPath + "\\global.cfg");

            bool isUsingRWC = false;
            for (int i = 0; i < lines.Length; i++)
            {
                if (lines[i].Trim().Equals("[worldcoordinates]", StringComparison.OrdinalIgnoreCase))
                {
                    isUsingRWC = true;
                    break;
                }
            }

            if (isUsingRWC)
            {
                double tileSize = CountTileSizeFromRWC();
                mapData.TileSize = tileSize;
            } else
            {
                mapData.TileSize = 300;
            }
        }

        private static double CountTileSizeFromRWC()
        {
            const double EarthRadius = 6378137.0; // in meters (WGS84)
            const double EarthCircum = 2 * Math.PI * EarthRadius; // Earth's circumference
            const int REAL_TILE_COUNT = 65536; // number of tiles across the world
            const double TILE_DEG = 360.0 / REAL_TILE_COUNT; // degrees of longitude per tile

            double inputLatitude = 52.5; // Example: Berlin-Spandau
            double latRad = inputLatitude * Math.PI / 180.0;

            // Meters per degree of longitude at given latitude
            double metersPerDegLon = (EarthCircum / 360.0) * Math.Cos(latRad);
            // Width/height of one tile in meters
            double tileSizeMeters = metersPerDegLon * TILE_DEG;
            return tileSizeMeters;
        }
    }
}
