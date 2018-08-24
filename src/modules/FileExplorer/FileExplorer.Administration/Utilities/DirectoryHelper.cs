using System;
using FileExplorer.Administration.Native;

namespace FileExplorer.Administration.Utilities
{
    public static class DirectoryHelper
    {
        private const uint DONT_RESOLVE_DLL_REFERENCES = 0x00000001;
        private const uint LOAD_LIBRARY_AS_DATAFILE = 0x00000002;

        //http://archives.miloush.net/michkap/archive/2007/01/18/1487464.html
        public static LoadedLabelLibrary GetLabelLibrary(string path)
        {
            var dllPath = Environment.ExpandEnvironmentVariables(path);
            var hMod = NativeMethods.LoadLibraryEx(dllPath, IntPtr.Zero,
                DONT_RESOLVE_DLL_REFERENCES | LOAD_LIBRARY_AS_DATAFILE);

            return new LoadedLabelLibrary(hMod);
        }
    }
}