using Microsoft.AspNetCore.Hosting;
using System;
using System.IO;
using System.Security.Cryptography;

namespace Maze.Server.AppStart
{
    public static class ConfigSecretPatcher
    {
        public static IWebHostBuilder GenerateProductionSettings(this IWebHostBuilder builder)
        {
            var productionSettingsFile = new FileInfo("appsettings.Production.json");
            if (!productionSettingsFile.Exists)
            {
                var secret = new byte[32];

                using (var rng = new RNGCryptoServiceProvider())
                {
                    rng.GetBytes(secret);
                }

                var base64Secret = Convert.ToBase64String(secret);

                var fileContent = $@"{{
  ""Authentication"": {{
    ""Secret"": ""{base64Secret}""
  }}
}}";

                File.WriteAllText(productionSettingsFile.FullName, fileContent);
            }

            return builder;
        }
    }
}
