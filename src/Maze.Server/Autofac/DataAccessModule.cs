using Autofac;
using Maze.Server.BusinessDataAccess.Accounts;

namespace Maze.Server.Autofac
{
    public class DataAccessModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
            builder.RegisterAssemblyTypes(typeof(AccountDbAccess).Assembly).AsImplementedInterfaces();
        }
    }
}