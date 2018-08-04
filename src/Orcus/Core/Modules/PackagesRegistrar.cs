using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using Autofac.Core;
using Autofac.Core.Registration;
using Microsoft.Extensions.Options;
using Orcus.Options;

namespace Orcus.Core.Modules
{
    public interface IPackagesRegistrar
    {
        void Configure(ContainerBuilder builder, IEnumerable<PackageCarrier> packages);
    }

    public class PackagesRegistrar : IPackagesRegistrar
    {
        private readonly IConfigurationRootProvider _configurationProvider;

        public PackagesRegistrar(IConfigurationRootProvider configurationProvider)
        {
            _configurationProvider = configurationProvider;
        }

        public void Configure(ContainerBuilder builder, IEnumerable<PackageCarrier> packages)
        {
            //https://github.com/autofac/Autofac/blob/41044d7d1a4fa277c628021537d5a12016137c3b/src/Autofac/ModuleRegistrationExtensions.cs#L156
            var moduleFinder = new ContainerBuilder();
            moduleFinder.RegisterInstance(_configurationProvider.ConfigurationRoot);

            moduleFinder.RegisterAssemblyTypes(packages.Select(x => x.Assembly).ToArray())
                .Where(t => typeof(IModule).IsAssignableFrom(t))
                .As<IModule>();

            IModuleRegistrar registrar = null;
            using (var moduleContainer = moduleFinder.Build())
            {
                foreach (var module in moduleContainer.Resolve<IEnumerable<IModule>>())
                {
                    if (registrar == null)
                        registrar = builder.RegisterModule(module);
                    else
                        registrar.RegisterModule(module);
                }
            }
        }
    }
}
