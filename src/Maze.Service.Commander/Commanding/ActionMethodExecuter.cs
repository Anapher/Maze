// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Orcus.Modules.Api;
using Orcus.Modules.Api.Response;

#if NETSTANDARD
using ReturnTask = System.Threading.Tasks.Task<Orcus.Modules.Api.IActionResult>;
#else
using ReturnTask = System.Threading.Tasks.ValueTask<Orcus.Modules.Api.IActionResult>;
#endif

namespace Orcus.Service.Commander.Commanding
{
    /// <summary>
    ///     Executes different signatures of controller methods and unifies them
    /// </summary>
    public abstract class ActionMethodExecutor
    {
        private static readonly ActionMethodExecutor[] Executors =
        {
            // Executors for sync methods
            new VoidResultExecutor(),
            new SyncActionResultExecutor(),
            new SyncObjectResultExecutor(), 

            // Executors for async methods
            new TaskResultExecutor(),
            new TaskOfIActionResultExecutor(),
            new TaskOfActionResultExecutor(),
            new AwaitableObjectResultExecutor()
        };

        protected abstract bool CanExecute(ActionMethodMetadata metadata);

        public abstract ReturnTask Execute(ObjectMethodExecutor executor, object controller,
            object[] arguments, ActionMethodMetadata metadata);

        /// <summary>
        ///     Get the correct executer for a given <see cref="ActionMethodMetadata" />
        /// </summary>
        /// <param name="metadata">The metadata of the method</param>
        /// <returns>Return the correct executer for the method signature</returns>
        public static ActionMethodExecutor GetExecutor(ActionMethodMetadata metadata)
        {
            for (var i = 0; i < Executors.Length; i++)
                if (Executors[i].CanExecute(metadata))
                    return Executors[i];

            Debug.Fail("Should not get here");
            throw new Exception();
        }

        private static void EnsureActionResultNotNull(ActionMethodMetadata metadata, IActionResult actionResult)
        {
            if (actionResult == null)
                throw new InvalidOperationException(
                    $"Cannot return null from an action method with a return type of '{metadata.MethodReturnType}'.");
        }

        // void LogMessage(..)
        private class VoidResultExecutor : ActionMethodExecutor
        {
            protected override bool CanExecute(ActionMethodMetadata metadata)
            {
                return !metadata.IsAsync && metadata.MethodReturnType == typeof(void);
            }

            public override ReturnTask Execute(ObjectMethodExecutor executor, object controller,
                object[] arguments, ActionMethodMetadata metadata)
            {
                executor.Execute(controller, arguments);

#if NETSTANDARD
                return Task.FromResult<IActionResult>(new OkResult());
#else
                return new ValueTask<IActionResult>(new OkResult());
#endif
            }
        }

        // IActionResult Post(..)
        // CreatedAtResult Put(..)
        private class SyncActionResultExecutor : ActionMethodExecutor
        {
            public override ReturnTask Execute(ObjectMethodExecutor executor, object controller,
                object[] arguments, ActionMethodMetadata metadata)
            {
                var actionResult = (IActionResult) executor.Execute(controller, arguments);
                EnsureActionResultNotNull(metadata, actionResult);

#if NETSTANDARD
                return Task.FromResult(actionResult);
#else
                return new ValueTask<IActionResult>(actionResult);
#endif
            }

            protected override bool CanExecute(ActionMethodMetadata methodMetadata)
            {
                return !methodMetadata.IsAsync &&
                       typeof(IActionResult).IsAssignableFrom(methodMetadata.MethodReturnType);
            }
        }

        // Person GetPerson(..)
        // object Index(..)
        private class SyncObjectResultExecutor : ActionMethodExecutor
        {
            public override ReturnTask Execute(ObjectMethodExecutor executor, object controller, object[] arguments,
                ActionMethodMetadata metadata)
            {
                // Sync method returning arbitrary object
                var returnValue = executor.Execute(controller, arguments);
                var actionResult = new ObjectResult(returnValue) {DeclaredType = metadata.MethodReturnType};

#if NETSTANDARD
                return Task.FromResult<IActionResult>(actionResult);
#else
                return new ValueTask<IActionResult>(actionResult); 
#endif
            }

            protected override bool CanExecute(ActionMethodMetadata metadata) => !metadata.IsAsync;
        }

        // Task<IActionResult> Post(..)
        private class TaskOfIActionResultExecutor : ActionMethodExecutor
        {
            protected override bool CanExecute(ActionMethodMetadata metadata)
            {
                return typeof(Task<IActionResult>).IsAssignableFrom(metadata.MethodReturnType);
            }

            public override async ReturnTask Execute(ObjectMethodExecutor executor, object controller,
                object[] arguments, ActionMethodMetadata metadata)
            {
                // Async method returning Task<IActionResult>
                // Avoid extra allocations by calling Execute rather than ExecuteAsync and casting to Task<IActionResult>.
                var returnValue = executor.Execute(controller, arguments);
                var actionResult = await (Task<IActionResult>) returnValue;
                EnsureActionResultNotNull(metadata, actionResult);

                return actionResult;
            }
        }

        // Task SaveState(..)
        private class TaskResultExecutor : ActionMethodExecutor
        {
            public override async ReturnTask Execute(ObjectMethodExecutor executor, object controller,
                object[] arguments, ActionMethodMetadata metadata)
            {
                await (Task) executor.Execute(controller, arguments);
                return new EmptyResult();
            }

            protected override bool CanExecute(ActionMethodMetadata metadata)
            {
                return metadata.MethodReturnType == typeof(Task);
            }
        }

        // Task<PhysicalfileResult> DownloadFile(..)
        // ValueTask<ViewResult> GetViewsAsync(..)
        private class TaskOfActionResultExecutor : ActionMethodExecutor
        {
            public override async ReturnTask Execute(ObjectMethodExecutor executor, object controller, object[] arguments,
                ActionMethodMetadata metadata)
            {
                // Async method returning awaitable-of-IActionResult (e.g., Task<ViewResult>)
                // We have to use ExecuteAsync because we don't know the awaitable's type at compile time.
                var task = (Task) executor.Execute(controller, arguments);
                await task;

                var resultProperty = typeof(Task<>).MakeGenericType(metadata.AsyncResultType).GetProperty("Result");
                var actionResult = (IActionResult) resultProperty.GetValue(task);

                EnsureActionResultNotNull(metadata, actionResult);
                return actionResult;
            }

            protected override bool CanExecute(ActionMethodMetadata metadata) =>
                metadata.IsAsync && typeof(IActionResult).IsAssignableFrom(metadata.AsyncResultType);
        }

        // Task<object> GetPerson(..)
        // Task<Customer> GetCustomerAsync(..)
        private class AwaitableObjectResultExecutor : ActionMethodExecutor
        {
            protected override bool CanExecute(ActionMethodMetadata metadata) => true;

            public override async ReturnTask Execute(ObjectMethodExecutor executor, object controller,
                object[] arguments, ActionMethodMetadata metadata)
            {
                // Async method returning awaitable-of-nonvoid
                var task = (Task) executor.Execute(controller, arguments);
                await task;

                var resultProperty = typeof(Task<>).MakeGenericType(metadata.AsyncResultType).GetProperty("Result");
                var result = resultProperty.GetValue(task);

                var actionResult = new ObjectResult(result) {DeclaredType = metadata.AsyncResultType};
                return actionResult;
            }
        }
    }
}