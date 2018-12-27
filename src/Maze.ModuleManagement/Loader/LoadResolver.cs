using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NuGet.Frameworks;
using NuGet.Packaging;

namespace Orcus.ModuleManagement.Loader
{
    public class LoadResolver
    {
        public static ResolvedLibraryDirectory ResolveNuGetFolder(string packageDirectory, NuGetFramework framework, Runtime runtime, Architecture architecture)
        {
            var libFolder = new DirectoryInfo(Path.Combine(packageDirectory, "lib"));
            if (libFolder.Exists)
            {
                var frameworkFolders = libFolder.GetDirectories()
                    .ToDictionary(x => NuGetFramework.ParseFolder(x.Name), x => x, NuGetFramework.Comparer);

                if (!frameworkFolders.Any() && libFolder.GetFiles("*.dll").Any())
                    return new ResolvedLibraryDirectory(libFolder, framework);

                var bestFramework =
                    NuGetFrameworkUtility.GetNearest(frameworkFolders.Select(x => x.Key), framework, x => x);

                if (bestFramework != null)
                    return new ResolvedLibraryDirectory(frameworkFolders[bestFramework], bestFramework);
            }

            var runtimesFolder = new DirectoryInfo(Path.Combine(packageDirectory, "runtimes"));
            if (runtimesFolder.Exists)
            {
                var expectedFolders = GetExpectedRuntimeFolders(runtime, architecture);
                var baseFolder = runtimesFolder.GetDirectories().FirstOrDefault(x => expectedFolders.Contains(x.Name));
                if (baseFolder != null)
                {
                    libFolder = new DirectoryInfo(Path.Combine(baseFolder.FullName, "lib"));
                    if (libFolder.Exists)
                        return ResolveNuGetFolder(baseFolder.FullName, framework, runtime, architecture);
                }
            }

            return null;
        }

        private static ISet<string> GetExpectedRuntimeFolders(Runtime runtime, Architecture architecture)
        {
            var result = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            switch (runtime)
            {
                case Runtime.Windows:
                    result.Add("win");
                    result.Add("windows");
                    break;
                case Runtime.Linux:
                    result.Add("unix");
                    result.Add("linux");
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(runtime), runtime, null);
            }

            switch (architecture)
            {
                case Architecture.x64:
                    result.AddRange(result.Select(x => x + "-x64").ToList());
                    break;
                case Architecture.x86:
                    result.AddRange(result.Select(x => x + "-x86").ToList());
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(architecture), architecture, null);
            }

            return result;
        }
    }

    public class ResolvedLibraryDirectory
    {
        public ResolvedLibraryDirectory(DirectoryInfo directory, NuGetFramework framework)
        {
            Directory = directory;
            Framework = framework;
        }

        public DirectoryInfo Directory { get; }
        public NuGetFramework Framework { get; }
    }

    public enum Runtime
    {
        Windows,
        Linux
    }

    public enum Architecture
    {
        x64,
        x86
    }
}