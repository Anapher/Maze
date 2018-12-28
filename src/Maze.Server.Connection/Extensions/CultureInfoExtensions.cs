using System.Globalization;

namespace Maze.Server.Connection.Extensions
{
    public static class CultureInfoExtensions
    {
        public static bool TryGet(string name, out CultureInfo cultureInfo)
        {
            try
            {
                cultureInfo = CultureInfo.GetCultureInfo(name);
                return true;
            }
            catch (CultureNotFoundException)
            {
                cultureInfo = null;
                return false;
            }
        }
    }
}