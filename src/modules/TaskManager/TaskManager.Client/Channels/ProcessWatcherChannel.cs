using System;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.Threading.Tasks;
using Maze.ControllerExtensions;
using Maze.Modules.Api.Routing;
using TaskManager.Client.Utilities;
using TaskManager.Shared.Channels;
using TaskManager.Shared.Dtos;

namespace TaskManager.Client.Channels
{
    [Route("processWatcher")]
    public class ProcessWatcherChannel : CallTransmissionChannel<IProcessWatcher>, IProcessWatcher
    {
        private readonly object _watchLock = new object();
        private bool _isWatching;
        private Process _process;
        private ManagementObjectSearcher _searcher;

        public event EventHandler<ProcessExitedEventArgs> Exited;

        public Task WatchProcess(int processId)
        {
            lock (_watchLock)
            {
                if (_isWatching)
                    throw new InvalidOperationException("Already watching a process");
                _isWatching = true;
            }

            _process = Process.GetProcessById(processId);
            _process.Exited += ProcessOnExited;
            _process.EnableRaisingEvents = true;
            _searcher = new ManagementObjectSearcher("root\\CIMV2", $"SELECT * FROM Win32_Process WHERE ProcessId = {processId}");

            return Task.CompletedTask;
        }

        public Task<ChangingProcessPropertiesDto> GetInfo()
        {
            if (!_isWatching)
                throw new InvalidOperationException("The watcher must first be initialized using WatchProcess()");

            _process.Refresh();

            var result = new ChangingProcessPropertiesDto();
            using (var results = _searcher.Get())
            {
                var wmiProcess = results.Cast<ManagementObject>().SingleOrDefault();
                if (wmiProcess == null || _process.HasExited)
                    result.Status = ProcessStatus.Exited;
                else
                    result.ApplyProperties(_process, wmiProcess);
            }

            return Task.FromResult(result);
        }

        public override void Dispose()
        {
            base.Dispose();
            _process?.Dispose();
            _searcher?.Dispose();
        }

        private void ProcessOnExited(object sender, EventArgs e)
        {
            Exited?.Invoke(this, new ProcessExitedEventArgs {ExitCode = _process.ExitCode});
        }
    }
}