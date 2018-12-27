using System.Windows;
using System.Windows.Controls;

namespace Orcus.Administration.Controls
{
    public class HeadFooterTabControl : TabControl
    {
        public static readonly DependencyProperty HeadProperty = DependencyProperty.Register(
            "Head", typeof(object), typeof(HeadFooterTabControl), new PropertyMetadata(default(object)));

        public static readonly DependencyProperty FooterProperty = DependencyProperty.Register(
            "Footer", typeof(object), typeof(HeadFooterTabControl), new PropertyMetadata(default(object)));

        static HeadFooterTabControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(HeadFooterTabControl),
                new FrameworkPropertyMetadata(typeof(HeadFooterTabControl)));
        }

        public object Head
        {
            get => GetValue(HeadProperty);
            set => SetValue(HeadProperty, value);
        }

        public object Footer
        {
            get => GetValue(FooterProperty);
            set => SetValue(FooterProperty, value);
        }
    }
}