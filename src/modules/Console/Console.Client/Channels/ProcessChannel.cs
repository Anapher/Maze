using System;
using System.Buffers;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Console.Shared.Channels;
using Console.Shared.Dtos;
using Maze.ControllerExtensions;
using Maze.Modules.Api;
using Maze.Modules.Api.Routing;
using Maze.Utilities;

namespace Console.Client.Channels
{
    [Route("processChannel"), SynchronizedChannel]
    public class ProcessChannel : CallTransmissionChannel<IProcessChannel>, IProcessChannel
    {
        private StreamReader _errorReader;
        private StreamWriter _inputWriter;
        private StreamReader _outputReader;
        private Process _process;

        public bool IsRunning => _process?.HasExited != true;

        public event EventHandler<ProcessOutputEventArgs> Output;
        public event EventHandler<ProcessOutputEventArgs> Error;
        public event EventHandler<ProcessExitedEventArgs> Exited;

        public async Task WriteInput(string input)
        {
            if (IsRunning)
            {
                await _inputWriter.WriteLineAsync(input);
                await _inputWriter.FlushAsync();
            }
        }

        public Task StartProcess(string filename, string arguments)
        {
            //  Create the process start info.
            var processStartInfo = new ProcessStartInfo(filename, arguments);

            //  Set the options.
            processStartInfo.UseShellExecute = false;
            processStartInfo.ErrorDialog = false;
            processStartInfo.CreateNoWindow = true;

            //  Specify redirection.
            processStartInfo.RedirectStandardError = true;
            processStartInfo.RedirectStandardInput = true;
            processStartInfo.RedirectStandardOutput = true;

            //  Create the process.
            var process = new Process();
            process.EnableRaisingEvents = true;
            process.StartInfo = processStartInfo;
            process.Exited += ProcessOnExited;

            process.Start();

            _process = process;

            _inputWriter = process.StandardInput;
            _outputReader = process.StandardOutput;
            _errorReader = process.StandardError;

            ReadProcessStream(_outputReader, args => Output?.Invoke(this, args)).Forget();
            ReadProcessStream(_errorReader, args => Error?.Invoke(this, args)).Forget();

            return Task.CompletedTask;
        }

        private async Task ReadProcessStream(StreamReader reader, Action<ProcessOutputEventArgs> eventAction)
        {
            var readBuffer = ArrayPool<char>.Shared.Rent(1024);
            try
            {
                while (IsRunning)
                {
                    var count = await reader.ReadAsync(readBuffer, 0, readBuffer.Length);
                    eventAction(new ProcessOutputEventArgs(new string(readBuffer, 0, count)));
                    await Task.Delay(20); //no idea why but that fixes wrong ordered events
                }
            }
            finally
            {
                ArrayPool<char>.Shared.Return(readBuffer);
            }
        }

        public override void Dispose()
        {
            base.Dispose();

            if (_process?.HasExited == false)
            {
                _process.Kill();
            }

            _process?.Dispose();
            _process = null;
        }

        private void ProcessOnExited(object sender, EventArgs e)
        {
            Exited?.Invoke(this, new ProcessExitedEventArgs(_process.ExitCode));

            _process.Kill();
            _process.Dispose();
            _process = null;
        }
    }
}