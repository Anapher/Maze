namespace Tasks.Infrastructure.Server.Extensions
{
    internal static class ConfigurationExtensions
    {
        public static string Or(this string configurationString, string defaultString)
        {
            if (string.IsNullOrEmpty(configurationString))
                return defaultString;

            return configurationString;
        }
    }
}