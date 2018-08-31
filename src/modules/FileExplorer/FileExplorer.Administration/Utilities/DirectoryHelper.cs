using System;
using FileExplorer.Administration.Models.Cache;
using FileExplorer.Administration.Native;
using FileExplorer.Shared.Dtos;

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

        public static bool IsComputerDirectory(this DirectoryEntry directoryEntry)
        {
            return string.Equals(directoryEntry.Name, "::{20D04FE0-3AEA-1069-A2D8-08002B30309D}",
                StringComparison.OrdinalIgnoreCase);
        }

        public static bool IsComputerDirectory(string path)
        {
            return string.Equals(path, "::{20D04FE0-3AEA-1069-A2D8-08002B30309D}",
                StringComparison.OrdinalIgnoreCase);
        }
    }
}