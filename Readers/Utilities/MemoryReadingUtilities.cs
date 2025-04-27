using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace OMSI_RouteAdvisor.Readers.Utilities
{
    /// <summary>
    /// Allows to read live memory
    /// </summary>
    internal class MemoryReadingUtilities
    {
        [DllImport("kernel32.dll")]
        private static extern IntPtr OpenProcess(int dwDesiredAccess, bool bInheritHandle, int dwProcessId);
        [DllImport("kernel32.dll")]
        private static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, int dwSize, out int lpNumberOfBytesRead);
        private const int PROCESS_WM_READ = 0x0010;
        private static IntPtr _omsiHandle = IntPtr.Zero;
        private static IntPtr _omsiBaseAddress = IntPtr.Zero;

        /// <summary>
        /// Initialise MemoryReader instance
        /// </summary>
        /// <exception cref="Exception">Process not found</exception>
        public MemoryReadingUtilities()
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
