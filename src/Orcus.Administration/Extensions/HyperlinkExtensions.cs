using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Navigation;

namespace Orcus.Administration.Extensions
{
    public static class HyperlinkExtensions
    {
        public static readonly DependencyProperty OpenInBrowserProperty = DependencyProperty.RegisterAttached(
            "OpenInBrowser", typeof(bool), typeof(HyperlinkExtensions),
            new PropertyMetadata(default(bool), OpenInBrowserPropertyChangedCallback));

        private static void OpenInBrowserPropertyChangedCallback(DependencyObject dependencyObject,
            DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            if (!(dependencyObject is Hyperlink hyperlink))
                throw new ArgumentException("The extension can only be applied on a Hyperlink");

            hyperlink.RequestNavigate -= HyperlinkOnRequestNavigate;

            if ((bool) dependencyPropertyChangedEventArgs.NewValue)
                hyperlink.RequestNavigate += HyperlinkOnRequestNavigate;
        }

        private static void HyperlinkOnRequestNavigate(object sender, RequestNavigateEventArgs requestNavigateEventArgs)
        {
            Process.Start(requestNavigateEventArgs.Uri.AbsoluteUri);
            requestNavigateEventArgs.Handled = true;
        }

        public static void SetOpenInBrowser(DependencyObject element, bool value)
        {
            element.SetValue(OpenInBrowserProperty, value);
        }

        public static bool GetOpenInBrowser(DependencyObject element)
        {
            return (bool) element.GetValue(OpenInBrowserProperty);
        }
    }
}