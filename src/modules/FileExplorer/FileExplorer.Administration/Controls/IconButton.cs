using System.Windows;
using System.Windows.Controls;
using MahApps.Metro.IconPacks;

namespace FileExplorer.Administration.Controls
{
    public class IconButton : Button
    {
        public static readonly DependencyProperty IconKindProperty = DependencyProperty.Register("IconKind",
            typeof(PackIconMaterialKind), typeof(IconButton), new PropertyMetadata(default(PackIconMaterialKind)));

        static IconButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(IconButton),
                new FrameworkPropertyMetadata(typeof(IconButton)));
        }

        public PackIconMaterialKind IconKind
        {
            get => (PackIconMaterialKind) GetValue(IconKindProperty);
            set => SetValue(IconKindProperty, value);
        }
    }
}