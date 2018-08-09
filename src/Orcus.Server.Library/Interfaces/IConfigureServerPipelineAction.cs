using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Orcus.Modules.Api;

namespace Orcus.Server.Library.Interfaces
{
    public interface IConfigureServerPipelineAction : IActionInterface<PipelineInfo>
    {
    }

    public class PipelineInfo
    {
        public PipelineInfo(IApplicationBuilder applicationBuilder, IHostingEnvironment hostingEnvironment)
        {
            ApplicationBuilder = applicationBuilder;
            HostingEnvironment = hostingEnvironment;
        }

        public IApplicationBuilder ApplicationBuilder { get; }
        public IHostingEnvironment HostingEnvironment { get; }
    }
}