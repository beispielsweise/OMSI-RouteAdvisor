using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OMSI_RouteAdvisor.Map
{
    /// <summary>
    /// Class <c>BusStop</c> contains information about a bus stop object located on the map.
    /// </summary>
    public class BusStop
    {
        public int Id { get; }
        public int ParentTileId { get; }
        public double TileX { get; }
        public double TileY { get; }
        public double LocalX { get; set; }
        public double LocalY { get; set; }
        public string Name { get; }

        public BusStop(int id, int parentTileId, double x, double y, string name)
        {
            Id = id;
            ParentTileId = parentTileId;
            TileX = x;
            TileY = y;
            Name = name;
            LocalX = 0;
            LocalY = 0;
        }

        /// <summary>
        /// Writes converted local coordinates to an instance of the class
        /// </summary>
        /// <param name="x">new local X coordinate</param>
        /// <param name="y">new local Y coordinate</param>
        public void WriteLocalCoordinates(double x, double y)
        {
            this.LocalX = x;
            this.LocalY = y;
        }
    }
}
