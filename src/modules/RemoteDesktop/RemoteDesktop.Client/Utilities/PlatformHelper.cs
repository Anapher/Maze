using System;

namespace RemoteDesktop.Client.Utilities
{
    internal class PlatformHelper
    {
        public static bool IsWindows8OrNewer()
        {
            var os = Environment.OSVersion;
            return os.Platform == PlatformID.Win32NT && (os.Version.Major > 6 || os.Version.Major == 6 && os.Version.Minor >= 2);
        }
    }
}