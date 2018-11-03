using System;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using Tasks.Infrastructure.Administration.PropertyGrid.Attributes;

namespace Tasks.Infrastructure.Administration.Controls.PropertyGrid.Editors
{
    public class MultilineStringEditor : PropertyEditor<ExpandableTextBox>
    {
        public override int Priority { get; } = 10;

        public override bool IsSupported(PropertyItem propertyItem, Type propertyType)
        {
            if (propertyType == typeof(string) && propertyItem.Property.PropertyInfo.GetCustomAttribute<MultilineStringAttribute>() != null)
                return true;

            return false;
        }

        protected override DependencyProperty GetDependencyProperty() => TextBox.TextProperty;
    }

    public class ExpandableTextBox : TextBox
    {
        public static readonly DependencyProperty IsDropDownOpenProperty = DependencyProperty.Register(
            "IsDropDownOpen", typeof(bool), typeof(ExpandableTextBox), new PropertyMetadata(default(bool)));

        public static readonly DependencyProperty DropDownHeightProperty = DependencyProperty.Register(
            "DropDownHeight", typeof(double), typeof(ExpandableTextBox), new PropertyMetadata(300d));

        public static readonly DependencyProperty DropDownWidthProperty = DependencyProperty.Register(
            "DropDownWidth", typeof(double), typeof(ExpandableTextBox), new PropertyMetadata(400d));

        static ExpandableTextBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ExpandableTextBox), new FrameworkPropertyMetadata(typeof(ExpandableTextBox)));
        }

        public bool IsDropDownOpen
        {
            get => (bool) GetValue(IsDropDownOpenProperty);
            set => SetValue(IsDropDownOpenProperty, value);
        }

        public double DropDownHeight
        {
            get => (double) GetValue(DropDownHeightProperty);
            set => SetValue(DropDownHeightProperty, value);
        }

        public double DropDownWidth
        {
            get => (double) GetValue(DropDownWidthProperty);
            set => SetValue(DropDownWidthProperty, value);
        }
    }
}