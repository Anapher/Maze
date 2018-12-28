using Autofac;
using Maze.Administration.Core.Clients;
using Maze.Administration.Library.Services;
using Maze.Server.Connection.Utilities;

namespace Maze.Administration.ViewModels
{
    public class AutofacModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            builder.RegisterType<ClientManager>().As<IClientManager>().SingleInstance();
            builder.RegisterType<XmlSerializerCache>().As<IXmlSerializerCache>().SingleInstance();
        }
    }
}