using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Tasks.Common.Triggers;
using Tasks.Infrastructure.Client.Library;

namespace Tasks.Common.ServerClientShared.Triggers
{
    public class SystemRestartTrigger : ITriggerService<SystemRestartTriggerInfo>
    {
        [DllImport("kernel32")]
        private static extern UInt64 GetTickCount64();

        public async Task InvokeAsync(SystemRestartTriggerInfo triggerInfo, TriggerContext context, CancellationToken cancellationToken)
        {
            var now = DateTime.UtcNow;
            var uptime = TimeSpan.FromMilliseconds(GetTickCount64());
            var startupTime = now.Add(-uptime);

            var session = await context.CreateSession(SessionKey.Create("SystemRestart"));
            if (!session.Info.Executions.Any(x => x.CreatedOn > startupTime))
            {
                await session.Invoke();
            }

            await Task.Delay(TimeSpan.MaxValue, cancellationToken);
        }
    }
}
