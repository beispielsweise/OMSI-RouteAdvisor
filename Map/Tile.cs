using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OMSI_RouteAdvisor.Map
{
    /// <summary>
    /// A class containing information about one grid tile
    /// </summary>
    public class Tile
    {
        public int GridX { get; }
        public int GridY { get; }

        public Tile(int gridX, int gridY)
        {
            this.GridX = gridX;
            this.GridY = gridY;
        }

        public string GetTileFilename()
        {
            return "tile_" + this.GridX + "_" + this.GridY + ".map";
        }
    }
}
