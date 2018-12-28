using Autofac;
using Maze.Server.BusinessLogic;

namespace Maze.Server.Autofac
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