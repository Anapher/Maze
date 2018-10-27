using System;
using System.Threading;
using System.Threading.Tasks;

#if LIBRARY_CLIENT
using Tasks.Infrastructure.Client.Library;

#else
using Tasks.Infrastructure.Server.Library;

#endif

namespace Tasks.Common.Triggers
{
    /// <summary>
    ///     Immediately trigger the task commands
    /// </summary>
    public class ImmediatelyTriggerService : ITriggerService<ImmediatelyTriggerInfo>
    {
        public async Task InvokeAsync(ImmediatelyTriggerInfo triggerInfo, TriggerContext context, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var session = await context.CreateSession(SessionKey.Create("Immediate", DateTimeOffset.UtcNow));
            await session.Invoke();
        }
    }
}
