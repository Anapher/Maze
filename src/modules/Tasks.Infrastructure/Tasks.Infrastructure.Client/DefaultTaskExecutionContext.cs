using System;
using System.Threading.Tasks;
using Orcus.Utilities;
using Tasks.Infrastructure.Client.Library;
using Tasks.Infrastructure.Core.Dtos;
using Tasks.Infrastructure.Management.Utilities;

namespace Tasks.Infrastructure.Client
{
    public class DefaultTaskExecutionContext : TaskExecutionContext, IDisposable
    {
        private readonly MessageThrottleService<CommandProcessDto> _statusUpdate;
        private string _message;
        private double? _progress;

        public DefaultTaskExecutionContext(IServiceProvider services, Func<CommandProcessDto, Task> updateStatus)
        {
            Services = services;
            _statusUpdate = new MessageThrottleService<CommandProcessDto>(updateStatus);
        }

        public override IServiceProvider Services { get; }

        public void Dispose()
        {
            _statusUpdate?.Dispose();
        }

        public override void ReportProgress(double? progress)
        {
            _progress = progress;

            _statusUpdate.SendAsync(new CommandProcessDto {Progress = _progress, StatusMessage = _message}).Forget();
        }

        public override void ReportStatus(string message)
        {
            _message = message;
            _statusUpdate.SendAsync(new CommandProcessDto {Progress = _progress, StatusMessage = _message}).Forget();
        }
    }
}