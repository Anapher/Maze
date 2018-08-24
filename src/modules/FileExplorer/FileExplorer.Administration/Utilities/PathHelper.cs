namespace FileExplorer.Administration.Utilities
{
    public static class PathHelper
    {
        public static bool ContainsEnvironmentVariables(string path)
        {
            var hasPercent = false;
            foreach (var c in path)
                if (c == '%')
                {
                    if (hasPercent)
                        return true;
                    hasPercent = true;
                }
                else if (c == '\\')
                {
                    hasPercent = false;
                }

            return false;
        }
    }
}