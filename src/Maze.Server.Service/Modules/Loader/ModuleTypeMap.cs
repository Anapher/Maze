using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;
using Autofac;
using Autofac.Core;
using Autofac.Core.Registration;
using AutoMapper;
using Microsoft.Extensions.Configuration;
using NuGet.Packaging.Core;

namespace Orcus.Server.Service.Modules.Loader
{
    public class ModuleTypeMap
    {
        private readonly IConfiguration _configuration;

        public ModuleTypeMap(IConfiguration configuration)
        {
            _configuration = configuration;
            Controllers = new ConcurrentDictionary<PackageIdentity, List<Type>>();
            Assemblies = new ConcurrentBag<Assembly>();
        }

        public ConcurrentDictionary<PackageIdentity, List<Type>> Controllers { get; }
        public ConcurrentBag<Assembly> Assemblies { get; }

        public void Configure(ContainerBuilder builder)
        {
            //https://github.com/autofac/Autofac/blob/41044d7d1a4fa277c628021537d5a12016137c3b/src/Autofac/ModuleRegistrationExtensions.cs#L156
            var moduleFinder = new ContainerBuilder();

            moduleFinder.RegisterInstance(_configuration);
            moduleFinder.RegisterAssemblyTypes(Assemblies.ToArray())
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

            builder.RegisterAssemblyTypes(Assemblies.ToArray()).AssignableTo<Profile>().As<Profile>();
        }
    }
}