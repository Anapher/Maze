using System;
using System.ComponentModel;
using System.Windows;

namespace RegistryEditor.Administration.Extensions
{
    public static class UiTools
    {
        public static void AddValueChanged<T>(this T obj, DependencyProperty property, EventHandler handler)
            where T : DependencyObject
        {
            var desc = DependencyPropertyDescriptor.FromProperty(property, typeof(T));
            desc.AddValueChanged(obj, handler);
        }

        public static void RemoveValueChanged<T>(this T obj, DependencyProperty property, EventHandler handler)
            where T : DependencyObject
        {
            var desc = DependencyPropertyDescriptor.FromProperty(property, typeof(T));
            desc.RemoveValueChanged(obj, handler);
        }
    }
}