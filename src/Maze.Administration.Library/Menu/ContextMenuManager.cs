using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace Maze.Administration.Library.Menu
{
    public abstract class ContextMenuManager
    {
        public abstract void Build();

        public void Fill(ContextMenu contextMenu, object context)
        {
            var items = GetItems(context);
            foreach (var item in items)
                contextMenu.Items.Add(item);
        }

        protected abstract IEnumerable<UIElement> GetItems(object context);
    }
}