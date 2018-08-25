using System.Windows;
using System.Windows.Controls;
using FileExplorer.Administration.Extensions;

namespace FileExplorer.Administration.Controls
{
    public class TreeViewItemEx : TreeViewItem
    {
        public static readonly DependencyProperty IsBringIntoViewProperty =
            DependencyProperty.Register("IsBringIntoView", typeof(bool), typeof(TreeViewItemEx),
                new PropertyMetadata(default(bool)));

        public TreeViewItemEx()
        {
            AddHandler(SelectedEvent,
                new RoutedEventHandler(delegate(object obj, RoutedEventArgs args)
                {
                    (args.OriginalSource as TreeViewItem)?.BringIntoView();
                }));

            this.AddValueChanged(IsBringIntoViewProperty, (sender, args) =>
            {
                var treeViewItem = (TreeViewItemEx) sender;

                if (treeViewItem.IsBringIntoView)
                {
                    treeViewItem.BringIntoView();
                    treeViewItem.IsBringIntoView = false;
                }
            });
        }

        public bool IsBringIntoView
        {
            get => (bool) GetValue(IsBringIntoViewProperty);
            set => SetValue(IsBringIntoViewProperty, value);
        }

        protected override DependencyObject GetContainerForItemOverride() => new TreeViewItemEx();

        protected override bool IsItemItsOwnContainerOverride(object item) => item is TreeViewItemEx;
    }
}