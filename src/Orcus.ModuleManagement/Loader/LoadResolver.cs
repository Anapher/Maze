using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NuGet.Common;
using NuGet.Frameworks;
using NuGet.Packaging;
using NuGet.RuntimeModel;

namespace Orcus.ModuleManagement.Loader
{
    public class LoadResolver
    {
        public static (DirectoryInfo, NuGetFramework) ResolveNuGetFolder(string packageDirectory, NuGetFramework framework, Runtime runtime, Architecture architecture)
        {
            var libFolder = new DirectoryInfo(Path.Combine(packageDirectory, "lib"));
            if (libFolder.Exists)
            {
                var frameworkFolders = libFolder.GetDirectories()
                    .ToDictionary(x => NuGetFramework.ParseFolder(x.Name), x => x, NuGetFramework.Comparer);

                var bestFramework =
                    NuGetFrameworkUtility.GetNearest(frameworkFolders.Select(x => x.Key), framework, x => x);

                if (bestFramework != null)
                    return (frameworkFolders[bestFramework], bestFramework);
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

            throw new InvalidOperationException($"The libraries were not found in directory {packageDirectory}");
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