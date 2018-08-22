using System;
using System.Runtime.InteropServices;
using System.Text;
using ShellDll;

namespace FileExplorer.Client.Native
{
    internal static class NativeMethods
    {
        [DllImport("shell32.dll", CallingConvention = CallingConvention.Winapi, CharSet = CharSet.Unicode)]
        internal static extern IntPtr SHGetLocalizedName(string pszPath, StringBuilder pszResModule, ref uint cch,
            out int pidsRes);

        [DllImport("shell32.dll")]
        internal static extern IntPtr SHGetFileInfo(string pszPath, uint dwFileAttributes, ref ShellAPI.SHFILEINFO psfi,
            uint cbSizeFileInfo, ShellAPI.SHGFI uFlags);
    }
}