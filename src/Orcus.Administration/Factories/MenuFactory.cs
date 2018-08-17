using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Orcus.Administration.Library.Menu.MenuBase;
using Prism.Commands;

namespace Orcus.Administration.Factories
{
    public class MenuFactory : IMenuFactory
    {
        public IEnumerable<UIElement> Create<T>(IEnumerable<IMenuEntry<T>> menuEntries)
        {
            var (result, _, _) = CreateInternal(menuEntries);
            return result;
        }

        private static (IReadOnlyList<UIElement>, bool visibleForSingleItem, bool visibleForMultipleItems)
            CreateInternal<T>(IEnumerable<IMenuEntry<T>> menuEntries)
        {
            var result = new List<UIElement>();
            var forSingleItem = false;
            var forMultipleItems = false;

            foreach (var menuEntry in menuEntries)
                if (menuEntry is MenuSection<T> section)
                {
                    var (items, singleItem, multipleItems) = CreateInternal(section);

                    if (singleItem)
                        forSingleItem = true;
                    if (multipleItems)
                        forMultipleItems = true;

                    foreach (var menuItem in items)
                        result.Add(menuItem);

                    result.Add(CreateSeparator(singleItem, multipleItems));
                }
                else if (menuEntry is NavigationalEntry<T> navigationalEntry)
                {
                    var (items, singleItem, multipleItems) = CreateInternal(navigationalEntry);

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
                else if (menuEntry is CommandMenuEntry<T> commandMenuEntry)
                {
                    if (commandMenuEntry.Command != null)
                        forSingleItem = true;
                    if (commandMenuEntry.MultipleCommand != null)
                        forMultipleItems = true;

                    result.Add(CreateCommandMenuItem(commandMenuEntry));
                }

            //dont finish with separator
            if (result.Any() && result.Last() is Separator)
                result.RemoveAt(result.Count - 1);

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

        private static MenuItem CreateMenuItem<T>(IVisibleMenuitem<T> menuItemInfo, bool visibleForSingle,
            bool visibleForMultiple)
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

        private static MenuItem CreateCommandMenuItem<T>(CommandMenuEntry<T> menuItemInfo)
        {
            var menuItem = CreateMenuItem(menuItemInfo, menuItemInfo.Command != null,
                menuItemInfo.MultipleCommand != null);

            if (menuItemInfo.Command != null && menuItemInfo.MultipleCommand != null)
                menuItem.Command = new DelegateCommand<object>(o =>
                {
                    if (o is T item)
                        menuItemInfo.Command.Execute(item);
                    else if (o is IList list)
                        menuItemInfo.MultipleCommand.Execute(list.Cast<T>().ToList());
                }, o =>
                {
                    if (o is T item)
                        return menuItemInfo.Command.CanExecute(item);
                    if (o is IList list)
                        return menuItemInfo.MultipleCommand.CanExecute(list.Cast<T>().ToList());

                    return false;
                });
            else if (menuItemInfo.Command != null)
                menuItem.Command = menuItemInfo.Command;
            else if (menuItemInfo.MultipleCommand != null)
                menuItem.Command = new DelegateCommand<IList>(
                    list => menuItemInfo.MultipleCommand.Execute(list.Cast<T>().ToList()),
                    list => menuItemInfo.MultipleCommand.CanExecute(list.Cast<T>().ToList()));

            return menuItem;
        }
    }
}