using Microsoft.VisualBasic.Logging;
using OMSI_RouteAdvisor.Map;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OMSI_RouteAdvisor.Readers
{
    /// <summary>
    /// A class responsible for parsing each file and generating a wholeimage.bmp based on the given map. 
    /// Used instead of traditional editor function, which doesn't function properly.
    /// 
    /// !!! NOTES ABOUT SPLINES !!!
    /// Spline is a piece of road (not a crossing)
    /*
    [spline]
    0
    Splines\Marcel\str_2spur_8m_borkumer1.sli
    4181 - current spline ID
    4180 - prev spline ID
    4182 - next spline ID
    279.590911996854 - spline pivot point X coordinate (inconsistent, presumably bottom?)
    0                - Z? - not important
    38.6766306130526 - spline pivot point Y coordinate (bottom)
    630.000027908964 - rotation (around the pivot point)
    47.1238932634272 - spline length (y.top - y.bottom) in units
    29.9999995292025 - radius (if spline is curving)
    0
    0
    0
    0
    0
    0
    148.123887211643 - not important 
         */
    ///  !!! NOTES ABOUT CROSSINGS !!!
    ///  Crossing is not a spline, but a special object
    /*
Object Nr. 0
[object]
0
Sceneryobjects\Kreuz_MC\Einm_See_Paewesiner.sco
4176 - current object ID
249.590909031678 - crossing pivot point X coordinate (center) 
112.176625851826 - crossing pivot point Y coordinate (center)
0
180.000000611189 - object rotation
0
0
0
     */
    /// !!! MORE IMPORTANT INFO !!!
    /// THe map fills all "empty" tiles with gray areas, e.g. there is a tile 2399_11257. 
    /// Y is being used on other tiles, X is also being used on other tiles, but there is no tile 2399_11257 in the map.
    /// However this tile is still generated as a gray area in the wholeimage.bmp as a ghost tile, because in reality coordinate 2399 exists, 11257 exists, so the tile 2399_11257 must exist as well, even if it is empty.
    /// 
    /// !!!
    /// Single Tile bmp is of size 256x256, whole.roadmap.bmp is generated differently, getting downscaled according to the size.
    /// </summary>
    internal class MapImageReader
    {
        private static string _mapFolderPath = "";
        private static MapData? _mapData;

        public static void GenerateRoadmap(string mapFolderPath)
        {
            _mapFolderPath = mapFolderPath;
            try
            {
                _mapData = new(mapFolderPath, true);
            }
            catch
            {
                throw new Exception("Check if the map folder is correct!");
            }

            Debug.WriteLine(_mapData.MinGridX + " " + _mapData.MinGridY);
            Debug.WriteLine(_mapData.MaxGridX + " " + _mapData.MaxGridY);
            Debug.WriteLine(_mapData.WorldWidth + " " + _mapData.WorldHeight);

            // TODO:
            // Parse all used splines in a separate dictionary (with the size info, etc.)
            // Parse all used crossings(objects) in a separate dictionary.
            // Generate each tile bmp.
            return;
        }

        private void ParseAllTiles()
        {

        }

        private void ParseTile()
        {

        }

        private void CreateBMPImage()
        {

        }

        private void GenerateWholeImageBMP()
        {

        }
    }
}
