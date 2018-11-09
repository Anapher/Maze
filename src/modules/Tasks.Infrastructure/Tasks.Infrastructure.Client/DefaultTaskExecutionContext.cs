using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Tasks.Infrastructure.Client.Library;
using Tasks.Infrastructure.Client.Utilities;
using Tasks.Infrastructure.Core.Dtos;

namespace Tasks.Infrastructure.Client
{
    public class DefaultTaskExecutionContext : TaskExecutionContext, IDisposable
    {
        private readonly MessageThrottleService<CommandProcessDto> _statusUpdate;
        private double? _progress;
        private string _message;

        public DefaultTaskExecutionContext(IServiceProvider services, Func<CommandProcessDto, Task> updateStatus)
        {
            Services = services;
            _statusUpdate = new MessageThrottleService<CommandProcessDto>(updateStatus);
        }

        public void Dispose()
        {
            _statusUpdate?.Dispose();
        }

        public override IServiceProvider Services { get; }

        public override void ReportProgress(double? progress)
        {
            _progress = progress;

            _statusUpdate.SendAsync(new CommandProcessDto {Progress = _progress, StatusMessage = _message});
        }

        public override void ReportStatus(string message)
        {
            _message = message;
            _statusUpdate.SendAsync(new CommandProcessDto {Progress = _progress, StatusMessage = _message});
        }
    }
}