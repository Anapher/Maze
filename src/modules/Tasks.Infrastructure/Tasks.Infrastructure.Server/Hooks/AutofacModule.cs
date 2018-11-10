using Autofac;
using CodeElements.BizRunner;
using Microsoft.Extensions.Configuration;
using Orcus.Server.Library.Extensions;
using Orcus.Server.Library.Interfaces;
using Tasks.Infrastructure.Core;
using Tasks.Infrastructure.Management;
using Tasks.Infrastructure.Server.Core;
using Tasks.Infrastructure.Server.Options;

namespace Tasks.Infrastructure.Server.Hooks
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
            builder.RegisterType<OrcusTaskManager>().SingleInstance();
            builder.RegisterAssemblyTypes(ThisAssembly).AssignableTo<IBizActionStatus>().AsImplementedInterfaces();
            builder.RegisterType<OnServerStartupEvent>().As<IConfigureServerPipelineAction>();
        }
    }
}