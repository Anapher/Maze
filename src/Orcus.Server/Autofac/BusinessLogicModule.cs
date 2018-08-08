using Autofac;
using Orcus.Server.BusinessLogic;

namespace Orcus.Server.Autofac
{
    public class BusinessLogicModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
            builder.RegisterAssemblyTypes(typeof(LoggingBusinessActionErrors).Assembly).AsImplementedInterfaces();
        }
    }
}