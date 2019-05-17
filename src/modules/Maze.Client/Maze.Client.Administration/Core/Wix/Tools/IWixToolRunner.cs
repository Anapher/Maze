using System.Threading;
using System.Threading.Tasks;
using Maze.Client.Administration.Core.Wix.Tools.Cli;
using Microsoft.Extensions.Logging;

namespace Maze.Client.Administration.Core.Wix.Tools
{
    public interface IWixToolRunner
    {
        bool IsAvailable { get; }
        string Path { get; }
        Task Run(string toolName, ILogger logger, CancellationToken cancellationToken, params ICommandLinePart[] parts);
    }
}