using System;
using System.IO;
using ShellDll;

namespace FileExplorer.Client.Extensions
{
    public static class DirectoryExExtensions
    {
        public static bool TryGetKnownFolderType(this DirectoryInfoEx directoryInfoEx, out KnownFolder knownFolder)
        {
            try
            {
                knownFolder = directoryInfoEx.KnownFolderType;
                return knownFolder != null;
            }
            catch (Exception)
            {
                knownFolder = null;
                return false;
            }
        }
    }
}