using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
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
        private readonly object _getProcessesLock = new object();
        private HashSet<int> _latestProcessIds;

        public ProcessesChannel(IEnumerable<IProcessValueProvider> processValueProviders)
        {
            _processValueProviders = processValueProviders;
        }

        public async Task<List<ChangeSet<ProcessDto>>> GetProcesses()
        {
            var processes = Process.GetProcesses();
            var processDtos = await TaskCombinators.ThrottledAsync(processes, CreateProcessDto, CancellationToken);
            var changes = new List<ChangeSet<ProcessDto>>();
            var newProcesses = new List<ProcessDto>();

            lock (_getProcessesLock)
            {
                var newProcessList = new HashSet<int>();
                foreach (var processDto in processDtos)
                {
                    newProcessList.Add(processDto.Id);

                    if (_latestProcessIds?.Contains(processDto.Id) != true)
                    {
                        newProcesses.Add(processDto);
                    }

                    changes.Add(new ChangeSet<ProcessDto> {Action = EntryAction.Add, Value = processDto});
                }

                if (_latestProcessIds != null)
                    foreach (var latestProcessId in _latestProcessIds)
                    {
                        if (!newProcessList.Contains(latestProcessId))
                            changes.Add(new ChangeSet<ProcessDto> {Action = EntryAction.Remove, Value = new ProcessDto {Id = latestProcessId}});
                    }

                _latestProcessIds = newProcessList;
            }

            await TaskCombinators.ThrottledCatchErrorsAsync(newProcesses, LoadProcessIcon, CancellationToken);
            return changes;
        }

        private static Task LoadProcessIcon(ProcessDto processDto, CancellationToken arg2)
        {
            if (!string.IsNullOrEmpty(processDto.FileName))
                processDto.IconData = FileUtilities.GetFileIcon(processDto.FileName);

            return Task.CompletedTask;
        }

        private Task<ProcessDto> CreateProcessDto(Process processDto, CancellationToken arg2)
        {
            var dto = new ProcessDto();
            var properties = new ConcurrentDictionary<string, string>();
            var lockObj = new object();

            foreach (var processValueProvider in _processValueProviders)
            {
                try
                {
                    processValueProvider.ProvideValue(dto, processDto, properties, lockObj);
                }
                catch (Exception)
                {
                    // ignored
                }
            }

            dto.Properties = properties;
            return Task.FromResult(dto);
        }
    }
}