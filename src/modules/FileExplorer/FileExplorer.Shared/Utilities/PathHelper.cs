using System.Collections.Generic;

namespace FileExplorer.Shared.Utilities
{
    public static class PathHelper
    {
        /// <summary>
        ///     Get all directories of a path, e. g. [C:\, C:\test] of C:\test\hello. The actual path is not included in the result
        /// </summary>
        /// <param name="path">The path which directories should be extracted</param>
        /// <returns>Return all directories on the way to the path directory</returns>
        public static IEnumerable<string> GetPathDirectories(string path)
        {
            var index = 0;
            //forward loop, e. g.
            //C:\
            //C:\Windows
            //C:\Windows\System32 ...
            while (true)
            {
                index = path.IndexOf('\\', index + 1);
                if (index == -1 || index == path.Length - 1) //we don't want the actual path occurring in the result
                    break;

                yield return path.Substring(0, index + 1);
            }
        }
    }
}