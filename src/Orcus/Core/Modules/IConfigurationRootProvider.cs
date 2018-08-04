using Microsoft.Extensions.Configuration;

namespace Orcus.Core.Modules
{
    public interface IConfigurationRootProvider
    {
        IConfiguration ConfigurationRoot { get; }
    }
}