using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Orcus.Modules.Api;

namespace Orcus.Server.Library.Interfaces
{
    public interface IConfigureServerPipelineAction : IExecutableInterface<IApplicationBuilder, IHostingEnvironment>
    {
    }
}