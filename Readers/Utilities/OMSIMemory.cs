using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OMSI_RouteAdvisor.Readers.Utilities
{
    /// <summary>
    /// Holds all importand OMSI 2 Base offsets (...B) and Pointeroffsets (...P)
    /// </summary>
    internal class OMSIMemory
    {
        public static readonly int CurrentTileNumberB = 0x0045F584;
        public static readonly int CurrentTileNumberP = 0x14C;
        public static readonly int CurrentTileXB = 0x0045F584;
        public static readonly int CurrentTileXP = 0x144;
        public static readonly int CurrentTileYB = 0x0045F584;
        public static readonly int CurrentTileYP = 0x148;
        public static readonly int BusXB = 0x004876B0;
        public static readonly int BusXP = 0x290;
        public static readonly int BusYB = 0x0045F4FC;
        public static readonly int BusYP = 0xC;
        public static readonly int NextStopIdB = 0x0045F4FC;
        public static readonly int NextStopIdP = 0x6B0;
    }
}
