using System;
using System.IO;
using System.Reflection;

namespace RemoteDesktop.Administration.Utilities
{
    public static class OpenH264LibFinder
    {
        public static string Locate()
        {
            var assembly = Assembly.GetCallingAssembly();
            var codeBase = assembly.CodeBase;

            var uri = new UriBuilder(codeBase);
            var path = Uri.UnescapeDataString(uri.Path);
            var directory = Path.GetDirectoryName(path);

            var architecture = Environment.Is64BitProcess ? "64" : "86";
            var architecture2 = Environment.Is64BitProcess ? "64" : "32";

            return Path.Combine(directory, $"x{architecture}", $"openh264-1.8.0-win{architecture2}.dll");
        }
    }
}