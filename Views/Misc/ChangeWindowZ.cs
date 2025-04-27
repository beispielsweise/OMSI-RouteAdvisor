using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace OMSI_RouteAdvisor.Views.Misc
{
    /// <summary>
    /// Changes internal Z position of a window
    /// </summary>
    internal class ChangeWindowZ
    {
        [DllImport("user32.dll")]
        private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

        private static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);
        private static readonly IntPtr HWND_NOTOPMOST = new IntPtr(-2);
        private const uint SWP_NOSIZE = 0x0001;
        private const uint SWP_NOMOVE = 0x0002;
        private const uint TOPMOST_FLAGS = SWP_NOMOVE | SWP_NOSIZE;

        /// <summary>
        /// Makes window Topmost over any fullscreen application
        /// </summary>
        /// <param name="mapWindow"></param>
        public static void MakeTopmost(Window window)
        {
            var handle = new System.Windows.Interop.WindowInteropHelper(window).Handle;
            SetWindowPos(handle, HWND_TOPMOST, 0, 0, 0, 0, TOPMOST_FLAGS);
        }

        /// <summary>
        /// Reverts window Topmost parameter back to normal
        /// </summary>
        /// <param name="mapWindow"></param>
        public static void RevertTopmost(Window window)
        {
            var handle = new System.Windows.Interop.WindowInteropHelper(window).Handle;
            SetWindowPos(handle, HWND_NOTOPMOST, 0, 0, 0, 0, TOPMOST_FLAGS);
        }
    }
}
