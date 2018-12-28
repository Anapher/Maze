using System;
using System.Reflection;
using System.Windows.Input;
using Prism.Commands;

namespace Maze.Administration.Library.ViewModels
{
    /// <summary>
    ///     An <see cref="ICommand" /> whose delegates can be attached for <see cref="Execute(T)" /> and
    ///     <see cref="CanExecute(T)" />.
    /// </summary>
    /// <typeparam name="T">Parameter type.</typeparam>
    /// <typeparam name="TContext">The type of the context</typeparam>
    /// <remarks>
    ///     The constructor deliberately prevents the use of value types.
    ///     Because ICommand takes an object, having a value type for T would cause unexpected behavior when CanExecute(null)
    ///     is called during XAML initialization for command bindings.
    ///     Using default(T) was considered and rejected as a solution because the implementor would not be able to distinguish
    ///     between a valid and defaulted values.
    ///     <para />
    ///     Instead, callers should support a value type by using a nullable value type and checking the HasValue property
    ///     before using the Value property.
    ///     <example>
    ///         <code>
    /// public MyClass()
    /// {
    ///     this.submitCommand = new DelegateCommand&lt;int?&gt;(this.Submit, this.CanSubmit);
    /// }
    /// 
    /// private bool CanSubmit(int? customerId)
    /// {
    ///     return (customerId.HasValue &amp;&amp; customers.Contains(customerId.Value));
    /// }
    ///     </code>
    ///     </example>
    /// </remarks>
    public class ContextDelegateCommand<T, TContext> : DelegateCommandBase
    {
        private readonly Action<T, TContext> _executeMethod;
        private readonly Func<T, TContext, bool> _canExecuteMethod;

        /// <summary>
        ///     Initializes a new instance of <see cref="ContextDelegateCommand{T, TContext}" />.
        /// </summary>
        /// <param name="executeMethod">
        ///     Delegate to execute when Execute is called on the command. This can be null to just hook up
        ///     a CanExecute delegate.
        /// </param>
        /// <remarks><see cref="CanExecute(T)" /> will always return true.</remarks>
        public ContextDelegateCommand(Action<T, TContext> executeMethod) : this(executeMethod, (_, x) => true)
        {
        }

        /// <summary>
        ///     Initializes a new instance of <see cref="ContextDelegateCommand{T, TContext}" />.
        /// </summary>
        /// <param name="executeMethod">
        ///     Delegate to execute when Execute is called on the command. This can be null to just hook up
        ///     a CanExecute delegate.
        /// </param>
        /// <param name="canExecuteMethod">Delegate to execute when CanExecute is called on the command. This can be null.</param>
        /// <exception cref="ArgumentNullException">
        ///     When both <paramref name="executeMethod" /> and
        ///     <paramref name="canExecuteMethod" /> ar <see langword="null" />.
        /// </exception>
        public ContextDelegateCommand(Action<T, TContext> executeMethod, Func<T, TContext, bool> canExecuteMethod)
        {
            if (executeMethod == null || canExecuteMethod == null)
                throw new ArgumentNullException(nameof(executeMethod), "DelegateCommandDelegatesCannotBeNull");

            var genericTypeInfo = typeof(T).GetTypeInfo();

            // DelegateCommand allows object or Nullable<>.  
            // note: Nullable<> is a struct so we cannot use a class constraint.
            if (genericTypeInfo.IsValueType)
                if (!genericTypeInfo.IsGenericType || !typeof(Nullable<>).GetTypeInfo()
                        .IsAssignableFrom(genericTypeInfo.GetGenericTypeDefinition().GetTypeInfo()))
                    throw new InvalidCastException("DelegateCommandInvalidGenericPayloadType");

            _executeMethod = executeMethod;
            _canExecuteMethod = canExecuteMethod;
        }

        /// <summary>
        ///     Executes the command and invokes the <see cref="Action{T}" /> provided during construction.
        /// </summary>
        /// <param name="parameter">Data used by the command.</param>
        public void Execute(T parameter, TContext context)
        {
            _executeMethod(parameter, context);
        }

        /// <summary>
        ///     Determines if the command can execute by invoked the <see cref="Func{T,Bool}" /> provided during construction.
        /// </summary>
        /// <param name="parameter">Data used by the command to determine if it can execute.</param>
        /// <returns>
        ///     <see langword="true" /> if this command can be executed; otherwise, <see langword="false" />.
        /// </returns>
        public bool CanExecute(T parameter, TContext context) => _canExecuteMethod(parameter, context);

        /// <summary>
        ///     Handle the internal invocation of <see cref="ICommand.Execute(object)" />
        /// </summary>
        /// <param name="parameter">Command Parameter</param>
        protected override void Execute(object parameter)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        ///     Handle the internal invocation of <see cref="ICommand.CanExecute(object)" />
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns><see langword="true" /> if the Command Can Execute, otherwise <see langword="false" /></returns>
        protected override bool CanExecute(object parameter) => throw new NotSupportedException();
    }
}