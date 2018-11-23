using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Media;
using Tasks.Infrastructure.Administration.PropertyGrid;

namespace Tasks.Infrastructure.Administration.Controls.PropertyGrid
{
    //A lot of ideas and code were copied from http://wpftoolkit.codeplex.com/

    [TemplatePart(Name = "PART_DragThumb", Type = typeof(Thumb))]
    [StyleTypedProperty(Property = "PropertyContainerStyle", StyleTargetType = typeof(PropertyItem))]
    public class PropertyGrid : Control, INotifyPropertyChanged
    {
        public static readonly DependencyProperty PropertiesProviderProperty = DependencyProperty.Register("PropertiesProvider",
            typeof(IProvideEditableProperties), typeof(PropertyGrid),
            new PropertyMetadata(default(IProvideEditableProperties), OnPropertiesProviderChanged));

        public static readonly DependencyProperty NameColumnWidthProperty = DependencyProperty.Register("NameColumnWidth", typeof(double),
            typeof(PropertyGrid), new PropertyMetadata(125d, NameColumnWidthPropertyChangedCallback));

        public static readonly DependencyProperty PropertyContainerStyleProperty = DependencyProperty.Register("PropertyContainerStyle",
            typeof(Style), typeof(PropertyGrid), new PropertyMetadata(default(Style)));

        public static readonly DependencyProperty IsCategorizedProperty = DependencyProperty.Register("IsCategorized", typeof(bool),
            typeof(PropertyGrid), new PropertyMetadata(true, IsCategorizedPropertyChangedCallback));

        public static readonly DependencyProperty SelectedPropertyItemProperty = DependencyProperty.Register("SelectedPropertyItem",
            typeof(PropertyItem), typeof(PropertyGrid), new PropertyMetadata(default(PropertyItem), OnSelectedPropertyItemChanged));

        public static readonly DependencyProperty PropertyEditorFinderProperty = DependencyProperty.Register("PropertyEditorFinder",
            typeof(IPropertyEditorFinder), typeof(PropertyGrid), new PropertyMetadata(default(IPropertyEditorFinder), OnPropertyEditorFinderChanged));

        private Thumb _dragThumb;
        private TextBox _filterTextBox;
        private ICollectionView _properties;
        private ObservableCollection<PropertyItem> _propertiesCollection;

        public PropertyGrid()
        {
            AddHandler(PropertyItem.ItemSelectionChangedEvent, new RoutedEventHandler(OnItemSelectionChanged));
        }

        public IPropertyEditorFinder PropertyEditorFinder
        {
            get => (IPropertyEditorFinder) GetValue(PropertyEditorFinderProperty);
            set => SetValue(PropertyEditorFinderProperty, value);
        }

        public PropertyItem SelectedPropertyItem
        {
            get => (PropertyItem) GetValue(SelectedPropertyItemProperty);
            set => SetValue(SelectedPropertyItemProperty, value);
        }

        public bool IsCategorized
        {
            get => (bool) GetValue(IsCategorizedProperty);
            set => SetValue(IsCategorizedProperty, value);
        }

        public Style PropertyContainerStyle
        {
            get => (Style) GetValue(PropertyContainerStyleProperty);
            set => SetValue(PropertyContainerStyleProperty, value);
        }

        public double NameColumnWidth
        {
            get => (double) GetValue(NameColumnWidthProperty);
            set => SetValue(NameColumnWidthProperty, value);
        }

        public IProvideEditableProperties PropertiesProvider
        {
            get => (IProvideEditableProperties) GetValue(PropertiesProviderProperty);
            set => SetValue(PropertiesProviderProperty, value);
        }

        public ICollectionView Properties
        {
            get => _properties;
            private set
            {
                if (_properties != value)
                {
                    _properties = value;
                    OnPropertyChanged();
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private static void OnSelectedPropertyItemChanged(DependencyObject dependencyObject,
            DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            ((PropertyGrid) dependencyObject).OnSelectedPropertyItemChanged((PropertyItem) dependencyPropertyChangedEventArgs.OldValue,
                (PropertyItem) dependencyPropertyChangedEventArgs.NewValue);
        }

        protected virtual void OnSelectedPropertyItemChanged(PropertyItem oldValue, PropertyItem newValue)
        {
            if (oldValue != null)
                oldValue.IsSelected = false;

            if (newValue != null)
                newValue.IsSelected = true;
        }

        private void OnItemSelectionChanged(object sender, RoutedEventArgs routedEventArgs)
        {
            var item = (PropertyItem) routedEventArgs.OriginalSource;
            if (item.IsSelected)
                SelectedPropertyItem = item;
            else if (ReferenceEquals(SelectedPropertyItem, item)) SelectedPropertyItem = null;
        }

        private static void NameColumnWidthPropertyChangedCallback(DependencyObject dependencyObject,
            DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            ((PropertyGrid) dependencyObject).OnNameColumnWidthChanged((double) dependencyPropertyChangedEventArgs.OldValue,
                (double) dependencyPropertyChangedEventArgs.NewValue);
        }

        private static void IsCategorizedPropertyChangedCallback(DependencyObject dependencyObject,
            DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            ((PropertyGrid) dependencyObject).OnIsCategorizedPropertyChanged((bool) dependencyPropertyChangedEventArgs.OldValue,
                (bool) dependencyPropertyChangedEventArgs.NewValue);
        }

        protected virtual void OnIsCategorizedPropertyChanged(bool oldValue, bool newValue)
        {
            UpdateThumb();
            ApplyGroupDescription(Properties);
        }

        private void ApplyGroupDescription(ICollectionView collectionView)
        {
            if (collectionView == null)
                return;

            if (IsCategorized)
                collectionView.GroupDescriptions.Add(new PropertyGroupDescription(nameof(PropertyItem.Category)));
            else
                collectionView.GroupDescriptions.Clear();
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (_dragThumb != null)
                _dragThumb.DragDelta -= DragThumbOnDragDelta;

            if (_filterTextBox != null)
                _filterTextBox.TextChanged -= FilterTextBoxOnTextChanged;

            _dragThumb = GetTemplateChild("PART_DragThumb") as Thumb;
            if (_dragThumb != null)
            {
                _dragThumb.DragDelta += DragThumbOnDragDelta;
                var moveTransform = new TranslateTransform {X = NameColumnWidth};
                _dragThumb.RenderTransform = moveTransform;
                UpdateThumb();
            }

            _filterTextBox = GetTemplateChild("PART_FilterTextBox") as TextBox;
            if (_filterTextBox != null)
                _filterTextBox.TextChanged += FilterTextBoxOnTextChanged;
        }

        private void FilterTextBoxOnTextChanged(object sender, TextChangedEventArgs textChangedEventArgs)
        {
            Properties?.Refresh();
            SelectedPropertyItem = Properties?.Cast<PropertyItem>().FirstOrDefault();
        }

        protected virtual void OnNameColumnWidthChanged(double oldValue, double newValue)
        {
            if (_dragThumb != null)
                ((TranslateTransform) _dragThumb.RenderTransform).X = newValue;
        }

        private void DragThumbOnDragDelta(object sender, DragDeltaEventArgs dragDeltaEventArgs)
        {
            NameColumnWidth = Math.Max(0, NameColumnWidth + dragDeltaEventArgs.HorizontalChange);
        }

        private void UpdateThumb()
        {
            if (_dragThumb != null)
            {
                if (IsCategorized)
                    _dragThumb.Margin = new Thickness(6, 0, 0, 0);
                else
                    _dragThumb.Margin = new Thickness(-1, 0, 0, 0);
            }
        }

        private static void OnPropertiesProviderChanged(DependencyObject d,
            DependencyPropertyChangedEventArgs e)
        {
            var propertyGrid = (PropertyGrid)d;
            propertyGrid.InitializeProperties(e.NewValue as IProvideEditableProperties, propertyGrid.PropertyEditorFinder);
        }


        private static void OnPropertyEditorFinderChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var propertyGrid = (PropertyGrid) d;
            propertyGrid.InitializeProperties(propertyGrid.PropertiesProvider, e.NewValue as IPropertyEditorFinder);
        }

        private void InitializeProperties(IProvideEditableProperties commandProperties, IPropertyEditorFinder editorFinder)
        {
            if (commandProperties == null || editorFinder == null)
            {
                _propertiesCollection = null;
                Properties = null;
                return;
            }

            _propertiesCollection = new ObservableCollection<PropertyItem>(commandProperties.Properties.Select(x => new PropertyItem(x, editorFinder)));
            var properties = new ListCollectionView(_propertiesCollection);
            properties.SortDescriptions.Add(new SortDescription(nameof(PropertyItem.Category), ListSortDirection.Ascending));
            properties.SortDescriptions.Add(new SortDescription(nameof(PropertyItem.DisplayName), ListSortDirection.Ascending));
            properties.Filter = FilterProperties;

            ApplyGroupDescription(properties);

            Properties = properties;

            SelectedPropertyItem = null;
        }

        private bool FilterProperties(object o)
        {
            if (string.IsNullOrWhiteSpace(_filterTextBox?.Text))
                return true;

            var propertyItem = (PropertyItem) o;
            return propertyItem.DisplayName.IndexOf(_filterTextBox.Text, StringComparison.OrdinalIgnoreCase) > -1;
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}