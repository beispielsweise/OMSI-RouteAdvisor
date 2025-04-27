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
using System.Windows.Controls;

namespace OMSI_RouteAdvisor.Readers
{
    /// <summary>
    /// Manages live in-game memory reading
    /// </summary>
    public class GameMemoryReader
    {
        private MemoryReadingUtilities reader { get; }
        public int PreviousBusStopId { get; set; }
        private double previousBusX = 0;
        private double previousBusY = 0;
        private int filterDifference = 5;
        private int passThreshold = 3;
        private int passCounter = 0;
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
        /// Returns the current x and y bus position, filtered to prevent big jumps.
        /// </summary>
        /// <param name="mapData">Map data instance</param>
        /// <returns>A tuple of X and Y </returns>
        public (double x, double y) GetCurrentBusPosition(MapData mapData)
        {
            double tileX = reader.ReadFloat(OMSIMemory.BusXB, OMSIMemory.BusXP);
            double tileY = reader.ReadFloat(OMSIMemory.BusYB, OMSIMemory.BusYP);
            int gridX = reader.ReadInt32(OMSIMemory.CurrentTileXB, OMSIMemory.CurrentTileXP);
            int gridY = reader.ReadInt32(OMSIMemory.CurrentTileYB, OMSIMemory.CurrentTileYP);

            double busX, busY;
            (busX, busY) = CoordinatesConverter.XYToLocal(mapData, gridX, gridY, tileX, tileY);

            if ((busX - previousBusX) > filterDifference && previousBusX != 0 && previousBusY != 0)
            {
                passCounter++;

                if (passThreshold > passCounter)
                {
                    passCounter = 0;
                    previousBusX = busX;
                    previousBusY = busY;
                    return (busX, busY);
                }

                return (previousBusX,  previousBusY);
            }

            previousBusX = busX;
            previousBusY = busY;
            return (busX, busY);    
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
