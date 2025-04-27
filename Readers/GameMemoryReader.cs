using OMSI_RouteAdvisor.Helpers;
using OMSI_RouteAdvisor.Map;
using OMSI_RouteAdvisor.Readers.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace OMSI_RouteAdvisor.Readers
{
    /// <summary>
    /// Manages live in-game memory reading
    /// </summary>
    public class GameMemoryReader
    {
        private MemoryReadingUtilities reader {  get; }
        public int PreviousBusStopId { get; set; }

        public GameMemoryReader()
        {
            try
            {
                reader = new MemoryReadingUtilities();
            }
            catch {
                throw new Exception("Please open the game first");
            }

            PreviousBusStopId = -1;
        }

        /// <summary>
        /// Reads current Bus X and Y and transforms it to local coordinates
        /// </summary>
        /// <param name="mapData">Map Data instance</param>
        /// <returns>A tuple of X and Y</returns>
        public (double x, double y) GetCurrentBusPosition(MapData mapData)
        {
            double tileX = reader.ReadFloat(OMSIMemory.BusXB, OMSIMemory.BusXP);
            double tileY = reader.ReadFloat(OMSIMemory.BusYB, OMSIMemory.BusYP);
            int gridX = reader.ReadInt32(OMSIMemory.CurrentTileXB, OMSIMemory.CurrentTileXP);
            int gridY = reader.ReadInt32(OMSIMemory.CurrentTileYB, OMSIMemory.CurrentTileYP);

            double x, y;
            (x, y) = CoordinatesConverter.XYToLocal(mapData, gridX, gridY, tileX, tileY);

            return (x, y);
        }

        /// <summary>
        /// Reads next Bus stop Id
        /// </summary>
        /// <returns></returns>
        public int GetNextBusStopId()
        {
            return reader.ReadInt32(OMSIMemory.NextStopIdB, OMSIMemory.NextStopIdP);
        }
    }
}
