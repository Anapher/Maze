using Microsoft.Extensions.Configuration;

namespace Maze.Core.Modules
{
    public interface IConfigurationRootProvider
    {
        IConfiguration ConfigurationRoot { get; }
    }
}