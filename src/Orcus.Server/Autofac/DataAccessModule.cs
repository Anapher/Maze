using Autofac;
using Orcus.Server.BusinessDataAccess.Accounts;

namespace Orcus.Server.Autofac
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