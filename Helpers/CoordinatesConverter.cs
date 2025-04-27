using OMSI_RouteAdvisor.Map;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OMSI_RouteAdvisor.Helpers
{
    /// <summary>
    /// A class that helps convert in-game coordinates
    /// </summary>
    class CoordinatesConverter
    {
        /// <summary>
        /// Returns four Minimum and Maximum Grid Tile numbers for X and Y Coordinate
        /// </summary>
        /// <param name="mapData"></param>
        /// <returns>A tuple of 4 double values</returns>
        public static (double minX, double minY, double maxX, double maxY) GetGridMinMaxValues(MapData mapData)
        {
            double minX = mapData.Tiles.Min(kvp => kvp.Value.GridX);
            double minY = mapData.Tiles.Min(kvp => kvp.Value.GridY);
            double maxX = mapData.Tiles.Max(kvp => kvp.Value.GridX);
            double maxY = mapData.Tiles.Max(kvp => kvp.Value.GridY);

            return (minX, minY, maxX, maxY);
        }

        /// <summary>
        /// Returns Game-Map width and height
        /// </summary>
        /// <param name="mapData"></param>
        /// <returns>A tuple of 2 double values</returns>
        public static (double worldWidth, double worldHeight) GetWorldSize(MapData mapData) 
        {
            double worldWidth = (mapData.MaxGridX - mapData.MinGridX + 1) * mapData.TileSize;
            double worldHeight = (mapData.MaxGridY - mapData.MinGridY + 1) * mapData.TileSize;

            return (worldWidth, worldHeight);
        }

        /// <summary>
        /// Returns a factor of difference between Bmp image coordinates and Game-Map coordinates
        /// </summary>
        /// <param name="mapData"></param>
        /// <returns></returns>
        public static double GetScaleFactor(MapData mapData)
        {
            return mapData.WorldWidth / mapData.BackgroundMapImg.Width;
        }

        /// <summary>
        /// Converts tile coordinates each BusStop object to local image coordinates
        /// </summary>
        /// <param name="mapData"></param>
        public static void BusStopsToLocal(MapData mapData)
        {
            foreach (KeyValuePair<int, BusStop> busStop in mapData.BusStops)
            {
                Tile parentTile = mapData.Tiles[busStop.Value.ParentTileId];

                // Find offset from the most left/bottom grid to the current 
                double localX = (parentTile.GridX - (mapData.MinGridX)) * mapData.TileSize + busStop.Value.TileX;
                double localY = (parentTile.GridY - (mapData.MinGridY)) * mapData.TileSize + busStop.Value.TileY;
                localY = mapData.WorldHeight - localY; // Game map is inverted

                localX /= mapData.ScaleFactor;
                localY /= mapData.ScaleFactor;

                busStop.Value.WriteLocalCoordinates(localX, localY);
            }
        }
    }
}
