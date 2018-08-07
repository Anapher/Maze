using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using Microsoft.Extensions.DependencyInjection;
using Orcus.Modules.Api;
using Orcus.Server.Service.Extensions;

namespace Orcus.Server.Extensions
{
    public static class ModuleActionExecutor
    {
        public static async Task Execute<TAction>(this ILifetimeScope lifetimeScope, CancellationToken cancellationToken)
            where TAction : IExecutableInterface
        {
            using (var scope = lifetimeScope.BeginLifetimeScope())
            {
                var actions = scope.Resolve<IEnumerable<TAction>>();
                await TaskCombinators.ThrottledAsync(actions, (action, token) => action.Execute(), cancellationToken);
            }
        }

        public static async Task Execute<TAction>(this IServiceProvider serviceProvider, CancellationToken cancellationToken)
            where TAction : IExecutableInterface
        {
            using (var scope = serviceProvider.CreateScope())
            {
                var actions = scope.ServiceProvider.GetServices<TAction>();
                await TaskCombinators.ThrottledAsync(actions, (action, token) => action.Execute(), cancellationToken);
            }
        }

        public static Task Execute<TAction>(this ILifetimeScope lifetimeScope) where TAction : IExecutableInterface =>
            Execute<TAction>(lifetimeScope, CancellationToken.None);

        public static Task Execute<TAction>(this IServiceProvider serviceProvider) where TAction : IExecutableInterface =>
            Execute<TAction>(serviceProvider, CancellationToken.None);

        //1 Parameter

        public static async Task Execute<TAction, TParam>(this ILifetimeScope lifetimeScope, TParam param,
            CancellationToken cancellationToken) where TAction : IExecutableInterface<TParam>
        {
            using (var scope = lifetimeScope.BeginLifetimeScope())
            {
                var actions = scope.Resolve<IEnumerable<TAction>>();
                await TaskCombinators.ThrottledAsync(actions, (action, token) => action.Execute(param), cancellationToken);
            }
        }

        public static async Task Execute<TAction, TParam>(this IServiceProvider serviceProvider, TParam param, CancellationToken cancellationToken)
            where TAction : IExecutableInterface<TParam>
        {
            using (var scope = serviceProvider.CreateScope())
            {
                var actions = scope.ServiceProvider.GetServices<TAction>();
                await TaskCombinators.ThrottledAsync(actions, (action, token) => action.Execute(param), cancellationToken);
            }
        }

        public static Task Execute<TAction, TParam>(this ILifetimeScope lifetimeScope, TParam param) where TAction : IExecutableInterface<TParam> =>
            Execute<TAction, TParam>(lifetimeScope, param, CancellationToken.None);

        public static Task Execute<TAction, TParam>(this IServiceProvider serviceProvider, TParam param) where TAction : IExecutableInterface<TParam> =>
            Execute<TAction, TParam>(serviceProvider, param, CancellationToken.None);

        //2 Parameters

        public static async Task Execute<TAction, TParam1, TParam2>(this ILifetimeScope lifetimeScope, TParam1 param1, TParam2 param2,
            CancellationToken cancellationToken) where TAction : IExecutableInterface<TParam1, TParam2>
        {
            using (var scope = lifetimeScope.BeginLifetimeScope())
            {
                var actions = scope.Resolve<IEnumerable<TAction>>();
                await TaskCombinators.ThrottledAsync(actions, (action, token) => action.Execute(param1, param2), cancellationToken);
            }
        }

        public static async Task Execute<TAction, TParam1, TParam2>(this IServiceProvider serviceProvider, TParam1 param1, TParam2 param2,
            CancellationToken cancellationToken) where TAction : IExecutableInterface<TParam1, TParam2>
        {
            using (var scope = serviceProvider.CreateScope())
            {
                var actions = scope.ServiceProvider.GetServices<TAction>();
                await TaskCombinators.ThrottledAsync(actions, (action, token) => action.Execute(param1, param2), cancellationToken);
            }
        }

        public static Task Execute<TAction, TParam1, TParam2>(this ILifetimeScope lifetimeScope, TParam1 param1, TParam2 param2) where TAction : IExecutableInterface<TParam1, TParam2> =>
            Execute<TAction, TParam1, TParam2>(lifetimeScope, param1, param2, CancellationToken.None);

        public static Task Execute<TAction, TParam1, TParam2>(this IServiceProvider serviceProvider, TParam1 param1, TParam2 param2) where TAction : IExecutableInterface<TParam1, TParam2> =>
            Execute<TAction, TParam1, TParam2>(serviceProvider, param1, param2, CancellationToken.None);
    }
}