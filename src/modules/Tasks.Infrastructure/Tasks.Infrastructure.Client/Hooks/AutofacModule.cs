using System.IO.Abstractions;
using Autofac;
using Microsoft.Extensions.Configuration;
using Orcus.Client.Library.Extensions;
using Orcus.Client.Library.Interfaces;
using Tasks.Infrastructure.Client.Options;
using Tasks.Infrastructure.Core;
using Tasks.Infrastructure.Management;

namespace Tasks.Infrastructure.Client.Hooks
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
            builder.RegisterType<ClientTaskManager>().As<IClientTaskManager>().SingleInstance();

            builder.RegisterType<OnConnectedAction>().As<IConnectedAction>();
        }
    }
}
