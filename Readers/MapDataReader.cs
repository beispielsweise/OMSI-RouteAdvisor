using OMSI_RouteAdvisor.Map;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace OMSI_RouteAdvisor.Readers
{
    class MapDataReader
    {
        /// <summary>
        /// Scans global.cfg for all map tiles that are in use
        /// </summary>
        /// <param name="mapData"></param>
        public static void ScanGlobalTiles(MapData mapData)
        {
            string[] lines; 
            try
            {
                lines = File.ReadAllLines(mapData.MapFolderPath + "\\global.cfg");
            }
            catch { return; }

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

        /// <summary>
        /// Scans for all bus stops that are stored in the tile .map file
        /// </summary>
        /// <param name="mapData"></param>
        /// <param name="tileFilename"></param>
        /// <param name="parentTileId"></param>
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

        /// <summary>
        /// Checks if the Map uses RWC (real world coordinates) and decides what the size of a tile should be
        /// Default: 300 by 300
        /// </summary>
        /// <param name="mapData"></param>
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

            // Barebones way of asking for latitude. I don't like it, but this stays for now
            // TODO: Prettify
            if (isUsingRWC)
            {
                double latitude = 52.2;
                bool validInput = false;

                while (!validInput)
                {
                    try
                    {
                        string input = Microsoft.VisualBasic.Interaction.InputBox("Enter a latitude value:", "Input Needed", "0.0");
                        latitude = double.Parse(input);
                        validInput = true;
                    }
                    catch
                    {
                        System.Windows.MessageBox.Show(
                            "Invalid input. Please enter a valid number.",
                            "Warning",
                            MessageBoxButton.OK,
                            MessageBoxImage.Error
                        );
                    }
                }

                double tileSize = CountTileSizeFromRWC(latitude);
                mapData.TileSize = tileSize;
            } else
            {
                mapData.TileSize = 300;
            }
        }

        /// <summary>
        /// Counts Map tile Size in RWC based on map Latitude
        /// </summary>
        /// <param name="mapLatitude">Current map latitude</param>
        /// <returns>New tile size</returns>
        private static double CountTileSizeFromRWC(double mapLatitude)
        {
            const double EarthRadius = 6378137.0; // in meters (WGS84)
            const double EarthCircum = 2 * Math.PI * EarthRadius; // Earth's circumference
            const int REAL_TILE_COUNT = 65536; // number of tiles across the world
            const double TILE_DEG = 360.0 / REAL_TILE_COUNT; // degrees of longitude per tile

            double latRad = mapLatitude * Math.PI / 180.0;

            // Meters per degree of longitude at given latitude
            double metersPerDegLon = (EarthCircum / 360.0) * Math.Cos(latRad);
            // Width/height of one tile in meters
            double tileSizeMeters = metersPerDegLon * TILE_DEG;
            return tileSizeMeters;
        }
    }
}
