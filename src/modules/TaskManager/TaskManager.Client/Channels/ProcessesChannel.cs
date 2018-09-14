using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.Threading;
using System.Threading.Tasks;
using Orcus.ControllerExtensions;
using Orcus.Modules.Api.Routing;
using Orcus.Utilities;
using TaskManager.Client.ProcessInfo;
using TaskManager.Client.Utilities;
using TaskManager.Shared.Channels;
using TaskManager.Shared.Dtos;

namespace TaskManager.Client.Channels
{
    [Route("processesProvider")]
    public class ProcessesChannel : CallTransmissionChannel<IProcessesProvider>, IProcessesProvider
    {
        private readonly IEnumerable<IProcessValueProvider> _processValueProviders;
        private HashSet<int> _latestProcessIds;
        private readonly ManagementObjectSearcher _searcher;
        private readonly SemaphoreSlim _getProcessesLock = new SemaphoreSlim(1, 1);

        public ProcessesChannel(IEnumerable<IProcessValueProvider> processValueProviders)
        {
            _processValueProviders = processValueProviders;
            _searcher = new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_Process");
        }

        public override void Dispose()
        {
            base.Dispose();
            _searcher.Dispose();
        }

        public async Task<List<ProcessDto>> GetProcesses()
        {
            await _getProcessesLock.WaitAsync();
            try
            {
                using (var processCollection = _searcher.Get())
                {
                    var dtos = (await TaskCombinators.ThrottledAsync(processCollection.Cast<ManagementObject>(), CreateProcessDto, CancellationToken))
                        .Where(x => x != null).ToList();

                    _latestProcessIds = new HashSet<int>(dtos.Select(x => x.ProcessId));
                    return dtos;
                }
            }
            finally
            {
                _getProcessesLock.Release();
            }
        }

        private Task<ProcessDto> CreateProcessDto(ManagementObject managementObject, CancellationToken cancellationToken)
        {
            Process process;
            if (managementObject.TryGetProperty("ProcessId", out uint processId))
                try
                {
                    process = Process.GetProcessById((int)processId);
                }
                catch (Exception)
                {
                    return null;
                }
            else
                return null;

            var processDto = new ProcessDto {ProcessId = (int) processId};
            var isUpdate = _latestProcessIds?.Contains((int) processId) == true;

            foreach (var processValueProvider in _processValueProviders)
            {
                try
                {
                    foreach (var property in processValueProvider.ProvideValues(managementObject, process, isUpdate))
                    {
                        processDto.Add(property.Key, property.Value);
                    }
                }
                catch (Exception)
                {
                    // ignored
                }
            }

            return Task.FromResult(processDto);
        }
    }
}