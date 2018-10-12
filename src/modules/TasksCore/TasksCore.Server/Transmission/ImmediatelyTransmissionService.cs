using System;
using System.Threading;
using System.Threading.Tasks;
using Orcus.Server.Library.Tasks;
using TasksCore.Shared;

namespace TasksCore.Server.Transmission
{
    public class ImmediatelyTransmissionService : ITransmissionService<ImmediatelyTransmissionInfo>
    {
        public async Task InvokeAsync(ImmediatelyTransmissionInfo transmissionInfo, TransmissionContext context, CancellationToken cancellationToken)
        {
            var session = await context.GetSession(DateTimeOffset.UtcNow.ToString("G"));
            await session.InvokeAll();
        }
    }
}
