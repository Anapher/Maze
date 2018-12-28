using System;
using Autofac;
using Microsoft.Extensions.Options;
using Maze.Modules.Api.Response;
using Maze.Modules.Api.Services;
using Maze.Service.Commander.Commanding.Formatters.Abstractions;
using Maze.Service.Commander.Commanding.ModelBinding;
using Maze.Service.Commander.Infrastructure;
using Maze.Service.Commander.Routing;
using Maze.Service.Commander.Routing.Trie;

namespace Maze.Service.Commander
{
    public static class MazeServerServices
    {
        public static void RegisterMazeServices(this ContainerBuilder builder, Action<RouteCache> configureRouteCache)
        {
            var routeCache = new RouteCache();
            configureRouteCache(routeCache);

            builder.RegisterType<TrieNodeFactory>().As<ITrieNodeFactory>().InstancePerLifetimeScope();
            builder.RegisterType<RouteResolverTrie>().As<IRouteResolverTrie>().SingleInstance();
            builder.RegisterType<RouteResolver>().As<IRouteResolver>().InstancePerLifetimeScope();
            builder.RegisterInstance(routeCache).As<IRouteCache>();
            builder.RegisterType<MazeRequestExecuter>().As<IMazeRequestExecuter>().SingleInstance();

            builder.RegisterType<ModelBinderFactory>().As<IModelBinderFactory>().SingleInstance();
            builder.RegisterInstance(new OptionsWrapper<MazeServerOptions>(new MazeServerOptions()))
                .As<IOptions<MazeServerOptions>>();

            builder.RegisterType<DefaultOutputFormatterSelector>().As<OutputFormatterSelector>().SingleInstance();
            builder.RegisterType<MemoryPoolHttpResponseStreamWriterFactory>().As<IHttpResponseStreamWriterFactory>()
                .SingleInstance();

            builder.RegisterType<ObjectResultExecutor>().As<IActionResultExecutor<ObjectResult>>().SingleInstance();
            builder.RegisterType<FileStreamResultExecutor>().As<IActionResultExecutor<FileStreamResult>>()
                .SingleInstance();
        }
    }
}