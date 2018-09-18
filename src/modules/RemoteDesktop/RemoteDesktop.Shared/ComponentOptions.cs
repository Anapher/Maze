using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RemoteDesktop.Shared
{
    public class ComponentOptions
    {
        public ComponentOptions(string componentName)
        {
            ComponentName = componentName;
            Options = new Dictionary<string, string>();
        }

        public string ComponentName { get; }
        public Dictionary<string, string> Options { get; }

        public static ComponentOptions Parse(string s)
        {
            var firstSpace = s.IndexOf(' ');
            if (firstSpace == -1)
                return new ComponentOptions(s);

            var result = new ComponentOptions(s.Substring(0, firstSpace));

            var parameterStart = firstSpace + 2;

            var i = parameterStart;
            while (i++ < s.Length - 1)
            {
                var c = s[i];
                if (c == ' ' || c == ',' || c == ')')
                {
                    var name = s.Substring(parameterStart, i - parameterStart);
                    if (s[i] == ',' || s[i] == ')')
                    {
                        result.Options.Add(name, null);
                        i += 2;
                        parameterStart = i;
                    }
                    else
                    {
                        var valueStart = i + 1;
                        string value = null;

                        if (s[valueStart] == '"')
                        {
                            valueStart++;
                            i += 2;

                            while (i++ < s.Length - 1)
                                if (s[i] == '"' && s[i - 1] != '\\')
                                {
                                    value = s.Substring(valueStart, i - valueStart).Replace("\\\"", "\"");
                                    break;
                                }

                            i += 3;
                            parameterStart = i;
                        }
                        else
                        {
                            var nextParameterStart = s.IndexOf(',', valueStart);
                            if (nextParameterStart != -1)
                            {
                                value = s.Substring(valueStart, nextParameterStart - valueStart);
                                parameterStart = nextParameterStart + 2;
                                i = nextParameterStart + 1;
                            }
                            else
                            {
                                value = s.Substring(valueStart, s.Length - 1 - valueStart);
                                i = s.Length;
                            }
                        }

                        result.Options.Add(name, value);
                    }
                }
            }

            return result;
        }

        public override string ToString()
        {
            var quoteRequiredChars = new[] {'"', ' ', ','};

            var sb = new StringBuilder();
            sb.Append(ComponentName);
            if (Options.Any())
            {
                sb.Append(" (");
                foreach (var keyValuePair in Options.OrderBy(x => x.Key))
                {
                    sb.Append(keyValuePair.Key);
                    if (!string.IsNullOrEmpty(keyValuePair.Value))
                    {
                        if (keyValuePair.Value.Any(x => quoteRequiredChars.Contains(x)))
                            sb.AppendFormat(" \"{0}\", ", keyValuePair.Value.Replace("\"", "\\\""));
                        else sb.AppendFormat(" {0}, ", keyValuePair.Value);
                    }
                    else
                    {
                        sb.Append(", ");
                    }
                }

                sb.Length -= 2;
                sb.Append(")");
            }

            return sb.ToString();
        }
    }
}