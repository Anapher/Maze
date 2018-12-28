using System.Text;
using Maze.Sockets.Logging;

namespace Maze.Sockets.Internal.Extensions
{
    internal static class LoggingExtensions
    {
        public static void LogDataPackage(this ILog logger, string name, byte[] buffer, int offset, int count)
        {
            if (logger.IsDebugEnabled())
            {
                var hash = HashHelper.HashData(buffer, offset, count);
                var s = Encoding.UTF8.GetString(buffer, offset, count);
                logger.Debug(name + " [{size} => {hash}]:\r\n{data}", count, hash, s);
            }
        }
    }
}