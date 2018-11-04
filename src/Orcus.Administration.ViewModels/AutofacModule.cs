using Autofac;
using Orcus.Administration.Library.Services;
using Orcus.Administration.ViewModels.Overview.Clients;
using Orcus.Server.Connection.Utilities;

namespace Orcus.Administration.ViewModels
{
    public class AutofacModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            builder.RegisterType<ClientManager>().As<IClientManager>();
            builder.RegisterType<XmlSerializerCache>().As<IXmlSerializerCache>().SingleInstance();
        }
    }
}