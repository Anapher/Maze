using System;
using Tasks.Infrastructure.Client.Library;

namespace Tasks.Infrastructure.Client
{
    public class DefaultTaskExecutionContext : TaskExecutionContext
    {
        public DefaultTaskExecutionContext(IServiceProvider services)
        {
            Services = services;
        }

        public override IServiceProvider Services { get; }
    }
}