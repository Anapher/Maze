using System;
using NuGet.Frameworks;

namespace ModulePacker
{
    public static class FrameworkExtensions
    {
        public static NuGetFramework ToNuGetFramework(this MazeFramework mazeFramework)
        {
            switch (mazeFramework)
            {
                case MazeFramework.Administration:
                    return FrameworkConstants.CommonFrameworks.MazeAdministration10;
                case MazeFramework.Server:
                    return FrameworkConstants.CommonFrameworks.MazeServer10;
                case MazeFramework.Client:
                    return FrameworkConstants.CommonFrameworks.MazeClient10;
                default:
                    throw new ArgumentOutOfRangeException(nameof(mazeFramework), mazeFramework, null);
            }
        }
    }
}