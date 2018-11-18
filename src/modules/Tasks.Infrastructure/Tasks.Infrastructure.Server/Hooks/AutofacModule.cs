using System;
using Autofac;
using CodeElements.BizRunner;
using Dapper;
using Dapper.Contrib.Extensions;
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

            SqlMapperExtensions.TableNameMapper = type => type.Name;

            SqlMapper.RemoveTypeMap(typeof(Guid?));
            SqlMapper.RemoveTypeMap(typeof(Guid));
            SqlMapper.RemoveTypeMap(typeof(DateTimeOffset));
            SqlMapper.RemoveTypeMap(typeof(DateTimeOffset?));
            SqlMapper.RemoveTypeMap(typeof(DateTime));
            SqlMapper.RemoveTypeMap(typeof(DateTime?));

            SqlMapper.AddTypeHandler(new GuidTypeHandler());
            SqlMapper.AddTypeHandler(new DateTimeHandler());
            SqlMapper.AddTypeHandler(new DateTimeOffsetHandler());

            builder.Configure<TasksOptions>(_configuration.GetSection("Tasks"));
            builder.RegisterType<TaskComponentResolver>().As<ITaskComponentResolver>().SingleInstance();
            builder.RegisterType<TaskDirectory>().As<ITaskDirectory>().SingleInstance();
            builder.RegisterType<OrcusTaskManager>().As<IOrcusTaskManager>().SingleInstance();
            builder.RegisterAssemblyTypes(ThisAssembly).AssignableTo<IBizActionStatus>().AsImplementedInterfaces();
            builder.RegisterType<OnServerStartupEvent>().As<IConfigureServerPipelineAction>();
            builder.RegisterAssemblyTypes(ThisAssembly).Where(x => x.Namespace.EndsWith(".Business") || x.Namespace.EndsWith(".BusinessDataAccess"))
                .AsImplementedInterfaces();
            builder.RegisterType<ActiveTasksManager>().SingleInstance();
        }
    }
}