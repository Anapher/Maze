using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Maze.Administration.Extensions;
using Maze.Administration.Library.Menu;
using Maze.Administration.Library.Menu.Internal;
using Maze.Administration.Library.Menu.MenuBase;

namespace Maze.Administration.Factories
{
    public class DefaultMenuFactory : IMenuFactory
    {
        public IEnumerable<UIElement> Create<TCommand>(IEnumerable<IMenuEntry<TCommand>> menuEntries, object context)
            where TCommand : ICommandMenuEntry =>
            CreateInternal(menuEntries, context);

        private static IReadOnlyList<UIElement> CreateInternal<TCommand>(IEnumerable<IMenuEntry<TCommand>> menuEntries, object context)
            where TCommand : ICommandMenuEntry
        {
            var result = new List<UIElement>();

            foreach (var menuEntry in menuEntries)
                if (menuEntry is MenuSection<TCommand> section)
                {
                    var items = CreateInternal(section, context);

                    if (!items.Any())
                        continue;

                    foreach (var menuItem in items)
                        result.Add(menuItem);

                    result.Add(CreateSeparator());
                }
                else if (menuEntry is NavigationalEntry<TCommand> navigationalEntry)
                {
                    var items = CreateInternal(navigationalEntry, context);

                    if (!items.Any())
                        continue;

                    var menuItem = CreateMenuItem(navigationalEntry);

                    foreach (var item in items)
                        menuItem.Items.Add(item);
                    result.Add(menuItem);
                }
                else if (menuEntry is CommandWrapper<TCommand> commandWrapper)
                {
                    result.Add(CreateCommandMenuItem(commandWrapper.CommandEntry, context));
                }
                else if (menuEntry is MenuItemEntry<TCommand> menuItemEntry)
                {
                    result.Add(menuItemEntry.MenuItem);
                }

            //dont finish with separator
            if (result.Any() && result.Last() is Separator)
                result.RemoveAt(result.Count - 1);

            return result;
        }

        private static Separator CreateSeparator() => new Separator();

        private static MenuItem CreateMenuItem(IVisibleMenuItem menuItemInfo) =>
            new MenuItem {Header = menuItemInfo.Header, Icon = menuItemInfo.Icon};

        private static MenuItem CreateCommandMenuItem(ICommandMenuEntry menuItemInfo, object context)
        {
            var menuItem = CreateMenuItem(menuItemInfo);
            menuItem.Command = menuItemInfo.Command;

            if (menuItem.Command is IContextAwareCommand contextAwareCommand)
                contextAwareCommand.Context = context;

            return menuItem;
        }
    }

    public class ItemMenuFactory : IItemMenuFactory
    {
        public IEnumerable<UIElement> Create<TCommand>(IEnumerable<IMenuEntry<TCommand>> menuEntries, object context)
            where TCommand : IItemCommandMenuEntry
        {
            var (result, _, _) = CreateInternal(menuEntries, context, false);
            return result;
        }

        private static (IReadOnlyList<UIElement>, bool visibleForSingleItem, bool visibleForMultipleItems) CreateInternal<TCommand>(
            IEnumerable<IMenuEntry<TCommand>> menuEntries, object context, bool sort) where TCommand : IItemCommandMenuEntry
        {
            var result = new List<UIElement>();
            var forSingleItem = false;
            var forMultipleItems = false;

            foreach (var menuEntry in menuEntries)
                if (menuEntry is MenuSection<TCommand> section)
                {
                    var (items, singleItem, multipleItems) = CreateInternal(section, context, section.IsOrdered);

                    if (!items.Any())
                        continue;

                    if (result.Any())
                        result.Add(CreateSeparator(singleItem && forSingleItem, multipleItems && forMultipleItems));

                    if (singleItem)
                        forSingleItem = true;
                    if (multipleItems)
                        forMultipleItems = true;

                    foreach (var menuItem in items)
                        result.Add(menuItem);
                }
                else if (menuEntry is NavigationalEntry<TCommand> navigationalEntry)
                {
                    var (items, singleItem, multipleItems) = CreateInternal(navigationalEntry, context, navigationalEntry.IsOrdered);

                    if (!items.Any())
                        continue;

                    if (singleItem)
                        forSingleItem = true;
                    if (multipleItems)
                        forMultipleItems = true;

                    var menuItem = CreateMenuItem(navigationalEntry, singleItem, multipleItems);

                    foreach (var item in items)
                        menuItem.Items.Add(item);
                    result.Add(menuItem);
                }
                else if (menuEntry is CommandWrapper<TCommand> commandWrapper)
                {
                    if (commandWrapper.CommandEntry.SingleItemCommand != null)
                        forSingleItem = true;
                    if (commandWrapper.CommandEntry.MultipleItemsCommand != null)
                        forMultipleItems = true;

                    result.Add(CreateCommandMenuItem(commandWrapper.CommandEntry, context));
                }
                else if (menuEntry is MenuItemEntry<TCommand> menuItemEntry)
                {
                    if (menuItemEntry.VisibleForSingle)
                        forSingleItem = true;
                    if (menuItemEntry.VisibleForMultiple)
                        forMultipleItems = true;

                    result.Add(menuItemEntry.MenuItem);
                }

            //dont finish with separator
            if (result.Any() && result.First() is Separator)
                result.RemoveAt(0);

            if (sort)
            {
                int start = 0;
                for (int i = 0; i < result.Count; i++)
                {
                    if (result[i] is MenuItem)
                        continue;

                    if (start != i)
                        result.PartialSort(start, i - start, element => ((MenuItem)element).Header as string);

                    start = i + 1;
                }

                if (start != result.Count - 1)
                    result.PartialSort(start, result.Count - start, element => ((MenuItem) element).Header as string);
            }

            return (result, forSingleItem, forMultipleItems);
        }

        private static Separator CreateSeparator(bool visibleForSingle, bool visibleForMultiple)
        {
            var separator = new Separator();

            string styleName;
            if (visibleForSingle && !visibleForMultiple)
                styleName = "SeparatorVisibleForSingleSelection";
            else if (!visibleForSingle && visibleForMultiple)
                styleName = "SeparatorVisibleForMultipleSelection";
            else return separator;

            separator.Style = (Style) Application.Current.FindResource(styleName);
            return separator;
        }

        private static MenuItem CreateMenuItem(IVisibleMenuItem menuItemInfo, bool visibleForSingle, bool visibleForMultiple)
        {
            var menuItem = new MenuItem {Header = menuItemInfo.Header, Icon = menuItemInfo.Icon};

            string styleName;
            if (visibleForSingle && !visibleForMultiple)
                styleName = "MenuItemVisibleForSingleSelection";
            else if (!visibleForSingle && visibleForMultiple)
                styleName = "MenuItemVisibleForMultipleSelection";
            else styleName = "MenuItemVisibleEverything";

            menuItem.Style = (Style) Application.Current.FindResource(styleName);
            return menuItem;
        }

        private static MenuItem CreateCommandMenuItem(IItemCommandMenuEntry menuItemInfo, object context)
        {
            var menuItem = CreateMenuItem(menuItemInfo, menuItemInfo.SingleItemCommand != null, menuItemInfo.MultipleItemsCommand != null);

            menuItem.Command = menuItemInfo.Command;

            if (menuItem.Command is IContextAwareCommand contextAwareCommand)
                contextAwareCommand.Context = context;

            return menuItem;
        }
    }
}