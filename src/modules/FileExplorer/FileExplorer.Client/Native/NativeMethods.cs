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
        internal static extern IntPtr SHGetFileInfo(string pszPath, uint dwFileAttributes, ref SHFILEINFO psfi,
            uint cbSizeFileInfo, ShellAPI.SHGFI uFlags);
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct SHFILEINFO
    {
        public IntPtr hIcon;
        public int iIcon;
        public uint dwAttributes;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)] public string szDisplayName;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)] public string szTypeName;
    }
}