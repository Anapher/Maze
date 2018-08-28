using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Orcus.Administration.Library.Menu.Internal;
using Orcus.Administration.Library.ViewModels;

namespace Orcus.Administration.Library.Menu.MenuBase
{
    public class ContextItemCommand<TItem, TContext> : IItemCommandMenuEntry
    {
        public ContextDelegateCommand<TItem, TContext> Command { get; set; }
        public ContextDelegateCommand<IList<TItem>, TContext> MultipleCommand { get; set; }

        public object Header { get; set; }
        public object Icon { get; set; }

        ICommand IItemCommandMenuEntry.SingleItemCommand => Command;
        ICommand IItemCommandMenuEntry.MultipleItemsCommand => MultipleCommand;

        ICommand ICommandMenuEntry.Command => CreateRoutingCommand();

        private ICommand CreateRoutingCommand()
        {
            if (Command != null && MultipleCommand != null)
            {
                ContextAwareDelegateCommand<object> command = null;
                command = new ContextAwareDelegateCommand<object>(o =>
                {
                    if (o is TItem item)
                        Command.Execute(item, (TContext) command.Context);
                    else if (o is IList list)
                        MultipleCommand.Execute(list.Cast<TItem>().ToList(), (TContext) command.Context);
                }, o =>
                {
                    if (o is TItem item)
                        return Command.CanExecute(item, (TContext) command.Context);
                    if (o is IList list)
                        return MultipleCommand.CanExecute(list.Cast<TItem>().ToList(), (TContext) command.Context);

                    return false;
                });

                Command.CanExecuteChanged += (sender, args) => command.RaiseCanExecuteChanged();
                MultipleCommand.CanExecuteChanged += (sender, args) => command.RaiseCanExecuteChanged();
                return command;
            }

            if (MultipleCommand != null)
            {
                ContextAwareDelegateCommand<IList> command = null;
                command = new ContextAwareDelegateCommand<IList>(
                    list => MultipleCommand.Execute(list.Cast<TItem>().ToList(), (TContext) command.Context),
                    list => MultipleCommand.CanExecute(list.Cast<TItem>().ToList(), (TContext) command.Context));
                MultipleCommand.CanExecuteChanged += (sender, args) => command.RaiseCanExecuteChanged();
                return command;
            }
            else
            {
                ContextAwareDelegateCommand<object> command = null;
                command = new ContextAwareDelegateCommand<object>(
                    o => Command.Execute((TItem) o, (TContext) command.Context),
                    o => Command.CanExecute((TItem) o, (TContext) command.Context));
                return command;
            }
        }
    }
}