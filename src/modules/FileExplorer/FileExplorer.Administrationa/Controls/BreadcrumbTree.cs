using System.Windows;
using System.Windows.Controls;

namespace FileExplorer.Administration.Controls
{
    public class BreadcrumbTree : TreeView
    {
        public static readonly DependencyProperty OverflowedItemContainerStyleProperty =
            DependencyProperty.Register("OverflowedItemContainerStyle", typeof (Style), typeof (BreadcrumbTree));

        public static readonly DependencyProperty MenuItemTemplateProperty =
            DependencyProperty.Register("MenuItemTemplate", typeof (DataTemplate), typeof (BreadcrumbTree));

        static BreadcrumbTree()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof (BreadcrumbTree),
                new FrameworkPropertyMetadata(typeof (BreadcrumbTree)));
        }

        public Style OverflowedItemContainerStyle
        {
            get => (Style) GetValue(OverflowedItemContainerStyleProperty);
            set => SetValue(OverflowedItemContainerStyleProperty, value);
        }

        public DataTemplate MenuItemTemplate
        {
            get => (DataTemplate) GetValue(MenuItemTemplateProperty);
            set => SetValue(MenuItemTemplateProperty, value);
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            return new BreadcrumbTreeItem();
        }
    }
}