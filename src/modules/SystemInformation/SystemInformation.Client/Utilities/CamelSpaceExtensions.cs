using System.Collections.Generic;
using System.Linq;

namespace SystemInformation.Client.Utilities
{
    public static class CamelSpaceExtensions
    {
        public static string SpaceCamelCase(this string input) => new string(InsertSpacesBeforeCaps(input).ToArray());

        private static IEnumerable<char> InsertSpacesBeforeCaps(IEnumerable<char> input)
        {
            foreach (var c in input)
            {
                if (char.IsUpper(c)) yield return ' ';

                yield return c;
            }
        }
    }
}