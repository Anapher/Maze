using System.Windows.Input;
using Orcus.Administration.Library.Menu.Internal;
using Prism.Commands;

namespace Orcus.Administration.Library.Menu.MenuBase
{
    public class ContextCommand<TContext> : ICommandMenuEntry
    {
        public DelegateCommand<TContext> Command { get; set; }

        public object Header { get; set; }
        public object Icon { get; set; }

        ICommand ICommandMenuEntry.Command => CreateRoutingCommand();

        private ICommand CreateRoutingCommand()
        {
            if (Command == null)
                return null;

            ContextAwareDelegateCommand<object> command = null;
            command = new ContextAwareDelegateCommand<object>(o => Command.Execute((TContext) command.Context),
                o => Command.CanExecute((TContext) command.Context));
            Command.CanExecuteChanged += (sender, args) => command.RaiseCanExecuteChanged();

            return command;
        }
    }
}