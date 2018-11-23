using Tasks.Common.Client.Native;

namespace Tasks.Common.Client.Utilities
{
    public static class FileHelper
    {
        public static void RemoveOnReboot(string path)
        {
            NativeMethods.MoveFileEx(path, null, MoveFileFlags.MOVEFILE_DELAY_UNTIL_REBOOT);
        }
    }
}
