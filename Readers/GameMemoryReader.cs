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
        private MemoryReadingUtilities _reader; 
        public int PreviousBusStopId { get; set; }
        private double _previousBusX = 0;
        private double _previousBusY = 0;
        private int _filterDifference = 5;
        private int _passThreshold = 3;
        private int _passCounter = 0;
        public GameMemoryReader()
        {
            try
            {
                _reader = new MemoryReadingUtilities();
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
            double tileX = _reader.ReadFloat(OMSIMemory.BusXB, OMSIMemory.BusXP);
            double tileY = _reader.ReadFloat(OMSIMemory.BusYB, OMSIMemory.BusYP);
            int gridX = _reader.ReadInt32(OMSIMemory.CurrentTileXB, OMSIMemory.CurrentTileXP);
            int gridY = _reader.ReadInt32(OMSIMemory.CurrentTileYB, OMSIMemory.CurrentTileYP);

            double busX, busY;
            (busX, busY) = CoordinatesConverter.XYToLocal(mapData, gridX, gridY, tileX, tileY);

            if ((busX - _previousBusX) > _filterDifference && _previousBusX != 0 && _previousBusY != 0)
            {
                _passCounter++;

                if (_passThreshold > _passCounter)
                {
                    _passCounter = 0;
                    _previousBusX = busX;
                    _previousBusY = busY;
                    return (busX, busY);
                }

                return (_previousBusX,  _previousBusY);
            }

            _previousBusX = busX;
            _previousBusY = busY;
            return (busX, busY);    
        }

        /// <summary>
        /// Reads next Bus stop Id
        /// </summary>
        /// <returns></returns>
        public int GetNextBusStopId()
        {
            return _reader.ReadInt32(OMSIMemory.NextStopIdB, OMSIMemory.NextStopIdP);
        }
    }
}
