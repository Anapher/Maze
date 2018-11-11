using System;
using System.Threading.Tasks;
using Orcus.Utilities;
using Tasks.Infrastructure.Core;
using Tasks.Infrastructure.Core.Dtos;
using Tasks.Infrastructure.Management.Data;
using Tasks.Infrastructure.Management.Utilities;
using Tasks.Infrastructure.Server.Library;

namespace Tasks.Infrastructure.Server.Core
{
    public class DefaultTaskExecutionContext : TaskExecutionContext, IDisposable
    {
        private readonly MessageThrottleService<CommandProcessDto> _statusUpdate;
        private string _message;
        private double? _progress;

        public DefaultTaskExecutionContext(TaskSession session, OrcusTask orcusTask, IServiceProvider services, Func<CommandProcessDto, Task> updateStatus)
        {
            Session = session;
            OrcusTask = orcusTask;
            Services = services;

            _statusUpdate = new MessageThrottleService<CommandProcessDto>(updateStatus);
        }

        public void Dispose()
        {
            _statusUpdate.Dispose();
        }

        public override TaskSession Session { get; }
        public override OrcusTask OrcusTask { get; }
        public override IServiceProvider Services { get; }

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