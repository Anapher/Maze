using System.IO.Abstractions;
using Autofac;
using Microsoft.Extensions.Configuration;
using Orcus.Client.Library.Extensions;
using Orcus.Client.Library.Interfaces;
using Tasks.Infrastructure.Client.Hooks;
using Tasks.Infrastructure.Client.Options;
using Tasks.Infrastructure.Core;

namespace Tasks.Infrastructure.Client
{
    public class AutofacModule : Module
    {
        private readonly IConfiguration _configuration;

        public AutofacModule(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            builder.Configure<TasksOptions>(_configuration.GetSection("Tasks"));
            builder.RegisterType<TaskComponentResolver>().As<ITaskComponentResolver>().SingleInstance();
            builder.RegisterType<TaskDirectory>().As<ITaskDirectory>().SingleInstance();
            builder.RegisterType<FileSystem>().As<IFileSystem>().SingleInstance();
            builder.RegisterType<TaskSessionManager>().As<ITaskSessionManager>().SingleInstance();

            builder.RegisterType<OnConnectedAction>().As<IConnectedAction>();
            builder.RegisterType<StartupAction>().As<IStartupAction>();
        }
    }
}
