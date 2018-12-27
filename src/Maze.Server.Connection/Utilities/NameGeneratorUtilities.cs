using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Orcus.Server.Connection.Utilities
{
    public static class NameGeneratorUtilities
    {
        public static string ToFilename(string s, bool includeSpace)
        {
            var sb = new StringBuilder();
            var invalidChars = new HashSet<char>(Path.GetInvalidFileNameChars());
            var nextCharUpperCase = false;

            foreach (var c in s)
            {
                if (invalidChars.Contains(c))
                    sb.Append("_");
                else if (!includeSpace && c == ' ')
                {
                    nextCharUpperCase = true;
                }
                else
                {
                    if (nextCharUpperCase)
                    {
                        sb.Append(char.ToUpper(c));
                        nextCharUpperCase = false;
                    }
                    else sb.Append(c);
                }
            }

            return sb.ToString();
        }

        public static string MakeUnique(string filename, string suffixTemplate, Func<string, bool> checkExists)
        {
            if (!checkExists(filename))
                return filename;

            var extension = Path.GetExtension(filename);
            var name = Path.GetFileNameWithoutExtension(filename);

            var counter = 1;
            while (true)
            {
                var suffix = suffixTemplate.Replace("[N]", counter.ToString());
                filename = name + suffix + extension;

                if (!checkExists(filename))
                    return filename;

                counter++;
            }
        }
    }
}
