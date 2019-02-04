using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Maze.Client.Administration.Core.Utilities;
using Maze.Client.Administration.Core.Wix.Tools;
using Maze.Client.Administration.Core.Wix.Tools.Cli;
using Microsoft.Extensions.Logging;

namespace Maze.Client.Administration.Core.Wix
{
    public class WixToolRunner : IWixToolRunner
    {
        private readonly string _wixToolsDirectory;

        public WixToolRunner(string wixToolsDirectory)
        {
            _wixToolsDirectory = wixToolsDirectory;
        }

        public async Task Run(string toolName, ILogger logger, CancellationToken cancellationToken, params ICommandLinePart[] parts)
        {
            var arguments = string.Join(" ", parts.Select(x => x.GetArgument()));
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = Path.Combine(_wixToolsDirectory, toolName),
                    Arguments = arguments,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                }
            };

            logger.LogInformation($"Execute {process.StartInfo.FileName} {process.StartInfo.Arguments}");

            process.OutputDataReceived += (sender, args) =>
            {
                if (!string.IsNullOrEmpty(args.Data))
                    logger.LogDebug(args.Data);
            };
            process.ErrorDataReceived += (sender, args) =>
            {
                if (!string.IsNullOrEmpty(args.Data))
                    logger.LogWarning(args.Data);
            };

            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();

            using (var cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken))
            {
                cancellationTokenSource.Token.Register(() =>
                {
                    if (!process.HasExited) process.Kill();
                });

                await process.WaitForExitAsync(cancellationTokenSource.Token);
            }

            logger.LogInformation("Process exited with code {exitCode}.", process.ExitCode);
            if (process.ExitCode != 0)
                throw new InvalidOperationException($"Process exited with code {process.ExitCode}");
        }
    }
}