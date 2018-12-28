using System;

namespace Maze.ModuleManagement.PackageManagement
{
    public static class PackageManagementConstants
    {
        /// <summary>
        /// Default MaxDegreeOfParallelism to use for restores and other threaded operations.
        /// </summary>
        public static readonly int DefaultMaxDegreeOfParallelism = Environment.ProcessorCount;

        /// <summary>
        /// Default amount of time a source request can take before timing out. This includes both UNC shares
        /// and online sources.
        /// </summary>
        public static readonly TimeSpan DefaultRequestTimeout = TimeSpan.FromMinutes(15);
    }
}