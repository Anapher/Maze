using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Maze.Client.Administration.Core.Wix.Tools;
using Maze.Client.Administration.Core.Wix.Tools.Cli;
using Microsoft.Extensions.Logging;

namespace Maze.Client.Administration.Tests.Core.Wix.Tools
{
    public class MockWixToolRunner : IWixToolRunner
    {
        public string CommandLine { get; set; }
        public bool IsExecuted { get; set; }

        public Task Run(string toolName, ILogger logger, CancellationToken cancellationToken, params ICommandLinePart[] parts)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var arguments = string.Join(" ", parts.Select(x => x.GetArgument()));

            CommandLine = $"{toolName} {arguments}";
            IsExecuted = true;

            return Task.CompletedTask;
        }
    }
}