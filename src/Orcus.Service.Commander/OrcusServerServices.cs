using System;
using Autofac;
using Microsoft.Extensions.Options;
using Orcus.Modules.Api.Response;
using Orcus.Modules.Api.Services;
using Orcus.Service.Commander.Commanding.Formatters.Abstractions;
using Orcus.Service.Commander.Commanding.ModelBinding;
using Orcus.Service.Commander.Infrastructure;
using Orcus.Service.Commander.Routing;
using Orcus.Service.Commander.Routing.Trie;

namespace Orcus.Service.Commander
{
    public static class OrcusServerServices
    {
        public static void RegisterOrcusServices(this ContainerBuilder builder, Action<RouteCache> configureRouteCache)
        {
            var routeCache = new RouteCache();
            configureRouteCache(routeCache);

            builder.RegisterType<TrieNodeFactory>().As<ITrieNodeFactory>().InstancePerLifetimeScope();
            builder.RegisterType<RouteResolverTrie>().As<IRouteResolverTrie>().SingleInstance();
            builder.RegisterType<RouteResolver>().As<IRouteResolver>().InstancePerLifetimeScope();
            builder.RegisterInstance(routeCache).As<IRouteCache>();
            builder.RegisterType<OrcusRequestExecuter>().As<IOrcusRequestExecuter>().SingleInstance();

            builder.RegisterType<ModelBinderFactory>().As<IModelBinderFactory>().SingleInstance();
            builder.RegisterInstance(new OptionsWrapper<OrcusServerOptions>(new OrcusServerOptions()))
                .As<IOptions<OrcusServerOptions>>();

            builder.RegisterType<DefaultOutputFormatterSelector>().As<OutputFormatterSelector>().SingleInstance();
            builder.RegisterType<MemoryPoolHttpResponseStreamWriterFactory>().As<IHttpResponseStreamWriterFactory>()
                .SingleInstance();

            builder.RegisterType<ObjectResultExecutor>().As<IActionResultExecutor<ObjectResult>>().SingleInstance();
            builder.RegisterType<FileStreamResultExecutor>().As<IActionResultExecutor<FileStreamResult>>()
                .SingleInstance();
        }
    }
}