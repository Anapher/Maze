using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Orcus.ModuleManagement.Logging;
using Orcus.Modules.Api;
using Orcus.Modules.Api.Utilities;

namespace Orcus.ModuleManagement
{
    public interface IActionInterfaceInvoker<TInterface> where TInterface : IActionInterface
    {
        Task Invoke(CancellationToken cancellationToken);
        Task Invoke();
    }

    public interface IActionInterfaceInvoker<TInterface, in TContext> where TInterface : IActionInterface<TContext>
    {
        Task Invoke(TContext context, CancellationToken cancellationToken);
        Task Invoke(TContext context);
    }

    public class ActionInterfaceInvoker<TInterface, TContext> : IActionInterfaceInvoker<TInterface, TContext>
        where TInterface : IActionInterface<TContext>
    {
        private readonly IReadOnlyCollection<TInterface> _actions;
        private static readonly ILog Logger = LogProvider.For<ActionInterfaceInvoker<TInterface, TContext>>();

        public ActionInterfaceInvoker(IReadOnlyCollection<TInterface> actions)
        {
            _actions = actions;
            Logger.Debug("Resolved {actionsCount} actions for {actionInterface}", actions.Count, typeof(TInterface).Name);
        }

        public async Task Invoke(TContext context, CancellationToken cancellationToken)
        {
            var exceptions =
                await TaskCombinators.ThrottledCatchErrorsAsync(_actions, (action, token) => action.Execute(context), cancellationToken);

            foreach (var keyValuePair in exceptions)
            {
                Logger.Warn(keyValuePair.Value, "The action '{actionName}' threw an error on execution.",
                    keyValuePair.Key.GetType().FullName);
            }
        }

        public Task Invoke(TContext context)
        {
            return Invoke(context, CancellationToken.None);
        }
    }

    public class ActionInterfaceInvoker<TInterface> : IActionInterfaceInvoker<TInterface> where TInterface : IActionInterface
    {
        private readonly IReadOnlyCollection<TInterface> _actions;
        private static readonly ILog Logger = LogProvider.For<ActionInterfaceInvoker<TInterface>>();

        public ActionInterfaceInvoker(IReadOnlyCollection<TInterface> actions)
        {
            _actions = actions;
            Logger.Debug("Resolved {actionsCount} actions for {actionInterface}", actions.Count, typeof(TInterface).Name);
        }

        public async Task Invoke(CancellationToken cancellationToken)
        {
            var exceptions =
                await TaskCombinators.ThrottledCatchErrorsAsync(_actions, (action, token) => action.Execute(), cancellationToken);

            foreach (var keyValuePair in exceptions)
            {
                Logger.Warn(keyValuePair.Value, "The action '{actionName}' threw an error on execution.",
                    keyValuePair.Key.GetType().FullName);
            }
        }

        public Task Invoke()
        {
            return Invoke(CancellationToken.None);
        }
    }
}