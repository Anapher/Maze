using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using Autofac.Core;

namespace Orcus.Client.Library.Interfaces
{
    public interface IConfigureOptionsModule
    {
    }

    public class Ads : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
            builder.registerass
        }
    }
}
