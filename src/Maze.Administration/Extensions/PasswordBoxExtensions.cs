using System;
using System.Security;
using System.Windows;
using System.Windows.Controls;

namespace Orcus.Administration.Extensions
{
    public static class PasswordBoxExtensions
    {
        public static readonly DependencyProperty ListenToPasswordChangeProperty = DependencyProperty.RegisterAttached(
            "ListenToPasswordChange", typeof(bool), typeof(PasswordBoxExtensions),
            new PropertyMetadata(default(bool), ListenToPasswordChangePropertyChangedCallback));

        public static readonly DependencyProperty IsEmptyProperty = DependencyProperty.RegisterAttached(
            "IsEmpty", typeof(bool), typeof(PasswordBoxExtensions), new PropertyMetadata(default(bool)));

        public static readonly DependencyProperty SecurePasswordProperty = DependencyProperty.RegisterAttached(
            "SecurePassword", typeof(SecureString), typeof(PasswordBoxExtensions),
            new PropertyMetadata(default(SecureString)));

        public static void SetSecurePassword(DependencyObject element, SecureString value)
        {
            element.SetValue(SecurePasswordProperty, value);
        }

        public static SecureString GetSecurePassword(DependencyObject element)
        {
            return (SecureString) element.GetValue(SecurePasswordProperty);
        }

        public static void SetIsEmpty(DependencyObject element, bool value)
        {
            element.SetValue(IsEmptyProperty, value);
        }

        public static bool GetIsEmpty(DependencyObject element)
        {
            return (bool) element.GetValue(IsEmptyProperty);
        }

        private static void ListenToPasswordChangePropertyChangedCallback(DependencyObject dependencyObject,
            DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var passwordBox = dependencyObject as PasswordBox;
            if (passwordBox == null)
                throw new ArgumentException("The dependency object must be a password box", nameof(dependencyObject));

            SetPasswordBoxStatus(passwordBox);
            passwordBox.PasswordChanged -= PasswordBoxOnPasswordChanged;
            passwordBox.PasswordChanged += PasswordBoxOnPasswordChanged;
        }

        private static void PasswordBoxOnPasswordChanged(object sender, RoutedEventArgs routedEventArgs)
        {
            SetPasswordBoxStatus((PasswordBox) sender);
        }

        private static void SetPasswordBoxStatus(PasswordBox passwordBox)
        {
            var password = passwordBox.SecurePassword;
            SetIsEmpty(passwordBox, password.Length == 0);
            SetSecurePassword(passwordBox, password);
        }

        public static void SetListenToPasswordChange(DependencyObject element, bool value)
        {
            element.SetValue(ListenToPasswordChangeProperty, value);
        }

        public static bool GetListenToPasswordChange(DependencyObject element)
        {
            return (bool) element.GetValue(ListenToPasswordChangeProperty);
        }
    }
}