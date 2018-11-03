using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Tasks.Infrastructure.Administration.PropertyGrid;

namespace Tasks.Infrastructure.Administration.Controls.PropertyGrid
{
    [TemplatePart(Name = "PART_ValueContainer", Type = typeof(ContentControl))]
    [TemplatePart(Name = "PART_FilterTextBox", Type = typeof(ContentControl))]
    public class PropertyItem : Control
    {
        public static readonly DependencyProperty DisplayNameProperty = DependencyProperty.Register(
            "DisplayName", typeof(string), typeof(PropertyItem), new PropertyMetadata(default(string)));

        public static readonly DependencyProperty IsSelectedProperty = DependencyProperty.Register("IsSelected", typeof(bool), typeof(PropertyItem),
            new PropertyMetadata(default(bool), IsSelectedPropertyChangedCallback));

        internal static readonly RoutedEvent ItemSelectionChangedEvent = EventManager.RegisterRoutedEvent("ItemSelectionChangedEvent",
            RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(PropertyItem));

        public static readonly DependencyProperty EditorProperty = DependencyProperty.Register("Editor", typeof(FrameworkElement),
            typeof(PropertyItem), new PropertyMetadata(default(FrameworkElement)));

        public static readonly DependencyProperty IsReadOnlyProperty = DependencyProperty.Register(
            "IsReadOnly", typeof(bool), typeof(PropertyItem), new PropertyMetadata(default(bool)));

        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(object), typeof(PropertyItem),
            new PropertyMetadata(default, ValuePropertyChangedCallback));

        public PropertyItem(IProperty property, IPropertyEditorFinder finder)
        {
            Property = property;
            DisplayName = property.Name;
            Description = property.Description;
            Category = property.Category;
            Value = property.Value;

            Editor = finder.FindAndCreateEditor(this);
            Editor.GotFocus += EditorOnGotFocus;
        }

        public PropertyItem()
        {
        }

        public IProperty Property { get; }

        public object Value
        {
            get => GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }

        public bool IsReadOnly
        {
            get => (bool) GetValue(IsReadOnlyProperty);
            set => SetValue(IsReadOnlyProperty, value);
        }

        public FrameworkElement Editor
        {
            get => (FrameworkElement) GetValue(EditorProperty);
            set => SetValue(EditorProperty, value);
        }

        public bool IsSelected
        {
            get => (bool) GetValue(IsSelectedProperty);
            set => SetValue(IsSelectedProperty, value);
        }

        public string DisplayName
        {
            get => (string) GetValue(DisplayNameProperty);
            set => SetValue(DisplayNameProperty, value);
        }

        public string Description { get; }
        public string Category { get; }

        private void EditorOnGotFocus(object sender, RoutedEventArgs routedEventArgs)
        {
            IsSelected = true;
        }

        private static void ValuePropertyChangedCallback(DependencyObject dependencyObject,
            DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            ((PropertyItem) dependencyObject).OnValuePropertyChanged(dependencyPropertyChangedEventArgs.NewValue);
        }

        protected virtual void OnValuePropertyChanged(object newValue)
        {
            Property.Value = newValue;
        }

        private static void IsSelectedPropertyChangedCallback(DependencyObject dependencyObject,
            DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            ((PropertyItem) dependencyObject).OnIsSelectedChanged((bool) dependencyPropertyChangedEventArgs.OldValue,
                (bool) dependencyPropertyChangedEventArgs.NewValue);
        }

        protected virtual void OnIsSelectedChanged(bool oldValue, bool newValue)
        {
            RaiseItemSelectionChangedEvent();
        }

        private void RaiseItemSelectionChangedEvent()
        {
            RaiseEvent(new RoutedEventArgs(ItemSelectionChangedEvent));
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);
            IsSelected = true;
            if (!IsKeyboardFocusWithin)
                Focus();

            e.Handled = true;
        }

        protected override void OnGotFocus(RoutedEventArgs e)
        {
            base.OnGotFocus(e);

            IsSelected = true;
            e.Handled = true;
        }
    }
}