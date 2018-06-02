using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Orcus.Modules.Api;
using Orcus.Modules.Api.Response;

namespace Orcus.Server.Service.Commanding
{
    public abstract class ActionMethodExecutor
    {
        private static readonly ActionMethodExecutor[] Executors =
        {
            // Executors for sync methods
            new VoidResultExecutor(),
            new SyncActionResultExecutor(),

            // Executors for async methods
            new TaskOfIActionResultExecutor(),
        };

        protected abstract bool CanExecute(ActionMethodMetadata metadata);

        public abstract ValueTask<IActionResult> Execute(ObjectMethodExecutor executor, object controller,
            object[] arguments, ActionMethodMetadata metadata);

        public static ActionMethodExecutor GetExecutor(ActionMethodMetadata metadata)
        {
            for (var i = 0; i < Executors.Length; i++)
            {
                if (Executors[i].CanExecute(metadata))
                    return Executors[i];
            }

            Debug.Fail("Should not get here");
            throw new Exception();
        }

        // void LogMessage(..)
        private class VoidResultExecutor : ActionMethodExecutor
        {
            protected override bool CanExecute(ActionMethodMetadata metadata) =>
                !metadata.IsAsync && metadata.MethodReturnType == typeof(void);

            public override ValueTask<IActionResult> Execute(ObjectMethodExecutor executor, object controller,
                object[] arguments, ActionMethodMetadata metadata)
            {
                executor.Execute(controller, arguments);
                return new ValueTask<IActionResult>(new OkResult());
            }
        }

        // IActionResult Post(..)
        // CreatedAtResult Put(..)
        private class SyncActionResultExecutor : ActionMethodExecutor
        {
            public override ValueTask<IActionResult> Execute(ObjectMethodExecutor executor, object controller,
                object[] arguments, ActionMethodMetadata metadata)
            {
                var actionResult = (IActionResult) executor.Execute(controller, arguments);
                EnsureActionResultNotNull(metadata, actionResult);

                return new ValueTask<IActionResult>(actionResult);
            }

            protected override bool CanExecute(ActionMethodMetadata methodMetadata)
                => !methodMetadata.IsAsync && typeof(IActionResult).IsAssignableFrom(methodMetadata.MethodReturnType);
        }

        // Task<IActionResult> Post(..)
        private class TaskOfIActionResultExecutor : ActionMethodExecutor
        {
            protected override bool CanExecute(ActionMethodMetadata metadata) =>
                typeof(Task<IActionResult>).IsAssignableFrom(metadata.MethodReturnType);

            public override async ValueTask<IActionResult> Execute(ObjectMethodExecutor executor, object controller, object[] arguments, ActionMethodMetadata metadata)
            {
                // Async method returning Task<IActionResult>
                // Avoid extra allocations by calling Execute rather than ExecuteAsync and casting to Task<IActionResult>.
                var returnValue = executor.Execute(controller, arguments);
                var actionResult = await (Task<IActionResult>) returnValue;
                EnsureActionResultNotNull(metadata, actionResult);

                return actionResult;
            }
        }

        private static void EnsureActionResultNotNull(ActionMethodMetadata metadata, IActionResult actionResult)
        {
            if (actionResult == null)
            {
                throw new InvalidOperationException(
                    $"Cannot return null from an action method with a return type of '{metadata.MethodReturnType}'.");
            }
        }
    }
}