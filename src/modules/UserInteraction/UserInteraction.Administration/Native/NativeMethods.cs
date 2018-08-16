using System;
using System.Runtime.InteropServices;

namespace UserInteraction.Administration.Native
{
    internal class NativeMethods
    {
        [DllImport("Shell32.dll", SetLastError = false)]
        internal static extern int SHGetStockIconInfo(SHSTOCKICONID siid, SHGSI uFlags, ref SHSTOCKICONINFO psii);

        [DllImport("user32.dll", SetLastError = true)]
        internal static extern bool DestroyIcon(IntPtr hIcon);
    }
}