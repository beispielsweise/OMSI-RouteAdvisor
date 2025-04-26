using OMSI_RouteAdvisor.Map;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OMSI_RouteAdvisor.Helpers
{
    class CoordinatesConverter
    {
        public static (double minX, double minY, double maxX, double maxY) GetGridMinMaxValues(MapData mapData)
        {
            double minX = mapData.Tiles.Min(kvp => kvp.Value.GridX);
            double minY = mapData.Tiles.Min(kvp => kvp.Value.GridY);
            double maxX = mapData.Tiles.Max(kvp => kvp.Value.GridX);
            double maxY = mapData.Tiles.Max(kvp => kvp.Value.GridY);

            return (minX, minY, maxX, maxY);
        }

        public static (double worldWidth, double worldHeight) GetWorldSize(MapData mapData) 
        {
            double worldWidth = (mapData.MaxGridX - mapData.MinGridX + 1) * mapData.TileSize;
            double worldHeight = (mapData.MaxGridY - mapData.MinGridY + 1) * mapData.TileSize;

            return (worldWidth, worldHeight);
        }

        public static double GetScaleFactor(MapData mapData)
        {
            return mapData.WorldWidth / mapData.BgImg.Width;
        }

        public static void BusStopsToLocal(MapData mapData)
        {
            foreach (KeyValuePair<int, BusStop> busStop in mapData.BusStops)
            {
                Tile parentTile = mapData.Tiles[busStop.Value.ParentTileId];

                double localX = (parentTile.GridX - (mapData.MinGridX)) * mapData.TileSize + busStop.Value.TileX;
                double localY = (parentTile.GridY - (mapData.MinGridY)) * mapData.TileSize + busStop.Value.TileY;
                localY = mapData.WorldHeight - localY;

                localX /= mapData.ScaleFactor;
                localY /= mapData.ScaleFactor;

                busStop.Value.WriteLocalCoordinates(localX, localY);
            }
        }
    }
}
