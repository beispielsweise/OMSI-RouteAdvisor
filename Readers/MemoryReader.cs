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
    class MemoryReader
    {
        [DllImport("kernel32.dll")]
        private static extern IntPtr OpenProcess(int dwDesiredAccess, bool bInheritHandle, int dwProcessId);

        [DllImport("kernel32.dll")]
        private static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, int dwSize, out int lpNumberOfBytesRead);
        private const int PROCESS_WM_READ = 0x0010;

        private static IntPtr _omsiHandle = IntPtr.Zero;
        private static IntPtr _omsiBaseAddress = IntPtr.Zero;

        // Memory access adresses for OMSI 2
        // BO - Base offset
        // PO - PointerOffset
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

        /// <summary>
        /// Initialise MemoryReader instance
        /// </summary>
        /// <exception cref="Exception">Process not found</exception>
        public MemoryReader()
        {
            Process omsiProcess = Process.GetProcessesByName("omsi").FirstOrDefault();
            if (omsiProcess == null)
            {
                throw new Exception("No process under name OMSI exists...");
            }

            _omsiHandle = OpenProcess(PROCESS_WM_READ, false, omsiProcess.Id);
            _omsiBaseAddress = omsiProcess.MainModule.BaseAddress;
        }

        /// <summary>
        /// Read int value from memory
        /// </summary>
        /// <param name="baseOffset">Base offset value (B)</param>
        /// <param name="pointerOffset">Pointer offset value (P)</param>
        /// <returns>integer value</returns>
        public int ReadInt32(int baseOffset, int pointerOffset)
        {
            IntPtr address = IntPtr.Add(_omsiBaseAddress, baseOffset);

            byte[] buffer = new byte[4];
            ReadProcessMemory(_omsiHandle, address, buffer, 4, out _);
            IntPtr pointerValue = (IntPtr)BitConverter.ToInt32(buffer, 0);

            IntPtr finalAddress = IntPtr.Add(pointerValue, pointerOffset);
            ReadProcessMemory(_omsiHandle, finalAddress, buffer, 4, out _);

            return BitConverter.ToInt32(buffer, 0);
        }

        /// <summary>
        /// Reads float value from memory
        /// </summary>
        /// <param name="baseOffset">Base offset value (B)</param>
        /// <param name="pointerOffset">Pointer offset value (P)</param>
        /// <returns>flolat value</returns>
        public float ReadFloat(int baseOffset, int pointerOffset)
        {
            IntPtr address = IntPtr.Add(_omsiBaseAddress, baseOffset);

            byte[] buffer = new byte[4];
            ReadProcessMemory(_omsiHandle, address, buffer, 4, out _);
            IntPtr pointerValue = (IntPtr)BitConverter.ToInt32(buffer, 0);

            IntPtr finalAddress = IntPtr.Add(pointerValue, pointerOffset);
            ReadProcessMemory(_omsiHandle, finalAddress, buffer, 4, out _);

            return BitConverter.ToSingle(buffer, 0);
        }
    }
}
