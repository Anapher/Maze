using System;
using System.Threading;
using System.Threading.Tasks;
using Orcus.Server.Library.Tasks;
using TasksCore.Services.Shared.Transmission;

namespace TasksCore.Server.Transmission
{
    public class ImmediatelyTriggerService : ITriggerService<ImmediatelyTriggerInfo>
    {
        public async Task InvokeAsync(ImmediatelyTriggerInfo triggerInfo, TriggerContext context, CancellationToken cancellationToken)
        {
            var session = await context.CreateSession(SessionKey.Create("Immediate", DateTimeOffset.UtcNow));
            await session.Invoke();
        }
    }
}
