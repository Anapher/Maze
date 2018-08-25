using System.Windows;
using System.Windows.Controls;

namespace FileExplorer.Administration.Controls
{
    public class TreeViewEx : TreeView
    {
        protected override DependencyObject GetContainerForItemOverride()
        {
            return new TreeViewItemEx();
        }

        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is TreeViewItemEx;
        }
    }
}