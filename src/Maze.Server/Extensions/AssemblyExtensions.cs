using System.Reflection;

namespace Maze.Server.Extensions
{
    public static class AssemblyExtensions
    {
        public static string GetInformationalVersion(this Assembly assembly)
        {
            var attr = assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>();
            if (attr != null)
            {
                return attr.InformationalVersion;
            }

            var versionAttr = assembly.GetCustomAttribute<AssemblyVersionAttribute>();
            if (versionAttr != null)
            {
                return versionAttr.Version;
            }

            return "Version not found";
        }
    }
}
