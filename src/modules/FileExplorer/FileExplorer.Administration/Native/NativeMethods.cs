using System;
using System.Runtime.InteropServices;
using System.Text;

namespace FileExplorer.Administration.Native
{
    public static class NativeMethods
    {
        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, ExactSpelling = true, EntryPoint = "LoadLibraryExW")]
        internal static extern IntPtr LoadLibraryEx(string lpFileName, IntPtr hFile, uint dwFlags);

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool FreeLibrary(IntPtr hModule);

        [DllImport("user32.dll", EntryPoint = "LoadStringW", CallingConvention = CallingConvention.Winapi,
            CharSet = CharSet.Unicode)]
        internal static extern int LoadString(IntPtr hModule, int resourceId, StringBuilder resourceValue, int len);

        [DllImport("shell32")]
        internal static extern IntPtr SHGetFileInfo(string pszPath, uint dwFileAttributes, out SHFILEINFO psfi,
            uint cbFileInfo, uint flags);

        [DllImport("User32.dll")]
        internal static extern int DestroyIcon(IntPtr hIcon);
    }
}