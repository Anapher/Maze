using System;
using System.Threading;
using System.Threading.Tasks;
using Tasks.Common.Triggers;

#if LIBRARY_CLIENT
using Tasks.Infrastructure.Client.Library;

#else
using Tasks.Infrastructure.Server.Library;

#endif

namespace Tasks.Common.ServerClientShared.Triggers
{
    public class OnAppStartupTriggerService : ITriggerService<OnAppStartupTriggerInfo>
    {
        public async Task InvokeAsync(OnAppStartupTriggerInfo triggerInfo, TriggerContext context, CancellationToken cancellationToken)
        {
            var session = await context.CreateSession(SessionKey.Create("OnAppStartup", DateTimeOffset.UtcNow));
            await session.Invoke();

            await Task.Delay(int.MaxValue, cancellationToken);
        }
    }
}