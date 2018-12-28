using System;
using Prism.Commands;

namespace Maze.Administration.Library.Menu.Internal
{
    internal class ContextAwareDelegateCommand<T> : DelegateCommand<T>, IContextAwareCommand
    {
        public ContextAwareDelegateCommand(Action<T> executeMethod) : base(executeMethod)
        {
        }

        public ContextAwareDelegateCommand(Action<T> executeMethod, Func<T, bool> canExecuteMethod) : base(
            executeMethod, canExecuteMethod)
        {
        }

        public object Context { get; set; }
    }
}