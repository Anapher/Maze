using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Tasks.Common.Triggers;
using Tasks.Infrastructure.Client.Library;

namespace Tasks.Common.Client.Triggers
{
    public class OnAppStartTrigger : ITriggerService<OnAppStartupTriggerInfo>
    {
        public async Task InvokeAsync(OnAppStartupTriggerInfo triggerInfo, TriggerContext context, CancellationToken cancellationToken)
        {
            var session = await context.CreateSession(SessionKey.Create("OnAppStart", DateTime.Now.Date));
            if (triggerInfo.OncePerDay)
            {
                if (session.Info.Executions.Any())
                    await Task.Delay(int.MaxValue, cancellationToken);
            }

            await session.Invoke();
            await Task.Delay(int.MaxValue, cancellationToken);
        }
    }
}
