using Microsoft.Extensions.Configuration;

namespace Orcus.Core.Modules
{
    public class ConfigurationRootProvider : IConfigurationRootProvider
    {
        public ConfigurationRootProvider(IConfiguration configurationRoot)
        {
            ConfigurationRoot = configurationRoot;
        }

        public IConfiguration ConfigurationRoot { get; }
    }
}