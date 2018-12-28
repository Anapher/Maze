using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Prism.Commands;

namespace Maze.Administration.Library.Menu.MenuBase
{
    public class ItemCommand<TItem> : IItemCommandMenuEntry
    {
        private readonly Lazy<ICommand> _lazyRoutingCommand;

        public ItemCommand()
        {
            _lazyRoutingCommand = new Lazy<ICommand>(CreateRoutingCommand);
        }

        public DelegateCommand<TItem> Command { get; set; }
        public DelegateCommand<IList<TItem>> MultipleCommand { get; set; }

        public object Header { get; set; }
        public object Icon { get; set; }

        ICommand IItemCommandMenuEntry.SingleItemCommand => Command;
        ICommand IItemCommandMenuEntry.MultipleItemsCommand => MultipleCommand;

        ICommand ICommandMenuEntry.Command => _lazyRoutingCommand.Value;

        private ICommand CreateRoutingCommand()
        {
            if (Command != null && MultipleCommand != null)
            {
                var command = new DelegateCommand<object>(o =>
                {
                    if (o is TItem item)
                        Command.Execute(item);
                    else if (o is IList list)
                        if (list.Count == 1)
                            Command.Execute(list.Cast<TItem>().Single());
                        else
                            MultipleCommand.Execute(list.Cast<TItem>().ToList());
                }, o =>
                {
                    if (o is TItem item)
                        return Command.CanExecute(item);
                    if (o is IList list)
                        if (list.Count == 1)
                            return Command.CanExecute(list.Cast<TItem>().Single());
                        else
                            return MultipleCommand.CanExecute(list.Cast<TItem>().ToList());

                    return true;
                });

                Command.CanExecuteChanged += (sender, args) => command.RaiseCanExecuteChanged();
                MultipleCommand.CanExecuteChanged += (sender, args) => command.RaiseCanExecuteChanged();
                return command;
            }

            if (MultipleCommand != null)
            {
                var command = new DelegateCommand<IList>(list => MultipleCommand.Execute(list.Cast<TItem>().ToList()),
                    list => MultipleCommand.CanExecute(list.Cast<TItem>().ToList()));
                MultipleCommand.CanExecuteChanged += (sender, args) => command.RaiseCanExecuteChanged();
                return command;
            }

            return Command;
        }
    }
}