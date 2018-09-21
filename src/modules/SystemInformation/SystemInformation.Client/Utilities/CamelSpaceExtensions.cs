using System.Collections.Generic;
using System.Linq;

namespace SystemInformation.Client.Utilities
{
    public static class CamelSpaceExtensions
    {
        public static string SpaceCamelCase(this string input) => new string(InsertSpacesBeforeCaps(input.ToCharArray()).ToArray());

        private static IEnumerable<char> InsertSpacesBeforeCaps(IReadOnlyList<char> input)
        {
            for (var i = 0; i < input.Count; i++)
            {
                var c = input[i];

                //aaaaAaaa
                if (char.IsUpper(c) && i > 0 && char.IsLower(input[i - 1]))
                    yield return ' ';
                else if (char.IsUpper(c) && i > 0 && i != input.Count - 1 && char.IsLower(input[i + 1]))
                    yield return ' ';

                yield return c;
            }
        }
    }
}