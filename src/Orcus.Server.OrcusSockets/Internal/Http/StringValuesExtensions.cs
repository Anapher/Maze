using System.Linq;
using Microsoft.Extensions.Primitives;

namespace Orcus.Server.OrcusSockets.Internal.Http
{
    public static class StringValuesExtensions
    {
        public static string FormatStringValues(StringValues values)
        {
            if (StringValues.IsNullOrEmpty(values))
                return string.Empty;

            if (values.Count == 0)
                return QuoteIfNeeded(values[0]);

            return string.Join(",", values.Select(QuoteIfNeeded));
        }

        // Quote items that contain commas and are not already quoted.
        private static string QuoteIfNeeded(string value)
        {
            if (!string.IsNullOrEmpty(value) && value.Contains(',') &&
                (value[0] != '"' || value[value.Length - 1] != '"'))
            {
                return $"\"{value}\"";
            }

            return value;
        }
    }
}