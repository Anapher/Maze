using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Maze.Modules.Api;

namespace Maze.Server.Library.Interfaces
{
    /// <summary>
    ///     An action that will be invoked when the server pipeline is configured.
    /// </summary>
    public interface IConfigureServerPipelineAction : IActionInterface<PipelineInfo>
    {
    }

    /// <summary>
    ///     Information about the current server pipeline
    /// </summary>
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