using System.Windows;
using System.Windows.Controls;

namespace FileExplorer.Administration.Controls
{
    public class TreeViewItemEx : TreeViewItem
    {
        public static readonly DependencyProperty BringIntoViewTokenProperty =
            DependencyProperty.Register("BringIntoViewToken", typeof(object), typeof(TreeViewItemEx),
                new PropertyMetadata(default, OnBringIntoViewTokenChanged));

        public object BringIntoViewToken
        {
            get => GetValue(BringIntoViewTokenProperty);
            set => SetValue(BringIntoViewTokenProperty, value);
        }

        private static void OnBringIntoViewTokenChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != e.OldValue)
            {
                var treeViewItem = (TreeViewItemEx) d;
                treeViewItem.BringIntoView();
            }
        }

        protected override DependencyObject GetContainerForItemOverride() => new TreeViewItemEx();

        protected override bool IsItemItsOwnContainerOverride(object item) => item is TreeViewItemEx;
    }
}