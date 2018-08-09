using System;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using Microsoft.Extensions.DependencyInjection;
using Orcus.Modules.Api;

namespace Orcus.ModuleManagement
{
    public static class ActionInvokerExtensions
    {
        public static async Task Execute<TAction>(this ILifetimeScope lifetimeScope,
            CancellationToken cancellationToken) where TAction : IActionInterface
        {
            using (var scope = lifetimeScope.BeginLifetimeScope())
            {
                await scope.Resolve<IActionInterfaceInvoker<TAction>>().Invoke(cancellationToken);
            }
        }

        public static async Task Execute<TAction>(this IServiceProvider serviceProvider,
            CancellationToken cancellationToken) where TAction : IActionInterface
        {
            using (var scope = serviceProvider.CreateScope())
            {
                await scope.ServiceProvider.GetRequiredService<IActionInterfaceInvoker<TAction>>().Invoke(cancellationToken);
            }
        }

        public static Task Execute<TAction>(this ILifetimeScope lifetimeScope) where TAction : IActionInterface =>
            Execute<TAction>(lifetimeScope, CancellationToken.None);

        public static Task Execute<TAction>(this IServiceProvider serviceProvider) where TAction : IActionInterface =>
            Execute<TAction>(serviceProvider, CancellationToken.None);

        //Context

        public static async Task Execute<TAction, TContext>(this ILifetimeScope lifetimeScope, TContext context,
            CancellationToken cancellationToken) where TAction : IActionInterface<TContext>
        {
            using (var scope = lifetimeScope.BeginLifetimeScope())
            {
                await scope.Resolve<IActionInterfaceInvoker<TAction, TContext>>().Invoke(context, cancellationToken);
            }
        }

        public static async Task Execute<TAction, TContext>(this IServiceProvider serviceProvider, TContext context,
            CancellationToken cancellationToken) where TAction : IActionInterface<TContext>
        {
            using (var scope = serviceProvider.CreateScope())
            {
                await scope.ServiceProvider.GetRequiredService<IActionInterfaceInvoker<TAction, TContext>>().Invoke(context, cancellationToken);
            }
        }

        public static Task Execute<TAction, TContext>(this ILifetimeScope lifetimeScope, TContext context) where TAction : IActionInterface<TContext> =>
            Execute<TAction, TContext>(lifetimeScope, context, CancellationToken.None);

        public static Task Execute<TAction, TContext>(this IServiceProvider serviceProvider, TContext context) where TAction : IActionInterface<TContext> =>
            Execute<TAction, TContext>(serviceProvider, context, CancellationToken.None);
    }
}