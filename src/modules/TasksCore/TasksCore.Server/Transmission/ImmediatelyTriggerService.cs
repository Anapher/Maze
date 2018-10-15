using System;
using System.Threading;
using System.Threading.Tasks;
using Orcus.Server.Library.Tasks;
using TasksCore.Shared.Transmission;

namespace TasksCore.Server.Transmission
{
    public class ImmediatelyTriggerService : ITriggerService<ImmediatelyTriggerInfo>
    {
        public async Task InvokeAsync(ImmediatelyTriggerInfo triggerInfo, TriggerContext context, CancellationToken cancellationToken)
        {
            var session = await context.GetSession(DateTimeOffset.UtcNow.ToString("G"));
            await session.InvokeAll();
        }
    }
}
