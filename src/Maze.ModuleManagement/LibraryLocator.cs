using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Maze.ModuleManagement.Loader;

namespace Maze.ModuleManagement
{
    public static class LibraryLocator
    {
        public static IEnumerable<FileInfo> GetFilesToLoad(DirectoryInfo libFolder, PackageLoadingContext context)
        {
            var dlls = libFolder.GetFiles("*.dll");

            if (!dlls.Any())
            {
                if (libFolder.GetFiles().SingleOrDefault()?.Name == "_._") return Enumerable.Empty<FileInfo>(); //implemented in GAC

                throw new InvalidOperationException($"Cannot load package {context.Package} ({context.PackageDirectory}) because there are no dlls");
            }

            return dlls;
        }
    }
}