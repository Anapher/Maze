using Microsoft.Extensions.Configuration;

namespace Maze.Core.Modules
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