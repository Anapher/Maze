using System;
using System.Text;
using FileExplorer.Administration.Native;

namespace FileExplorer.Administration.Utilities
{
    public class LoadedLabelLibrary : IDisposable
    {
        public LoadedLabelLibrary(IntPtr hwnd)
        {
            Hwnd = hwnd;
        }

        public IntPtr Hwnd { get; private set; }

        public string LoadString(int labelId)
        {
            if (Hwnd == IntPtr.Zero)
                return null;

            var sb = new StringBuilder(500);
            if (NativeMethods.LoadString(Hwnd, labelId, sb, sb.Capacity) != 0)
                return sb.ToString();

            return null;
        }

        public void Dispose()
        {
            NativeMethods.FreeLibrary(Hwnd);
            Hwnd = IntPtr.Zero;
        }
    }
}