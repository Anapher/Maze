using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace FileExplorer.Administration.Controls
{
    /// <summary>
    ///     User update Suggestions when TextChangedEvent raised.
    /// </summary>
    public class SuggestBoxBase : TextBox
    {
        public static readonly RoutedEvent ValueChangedEvent = EventManager.RegisterRoutedEvent("ValueChanged",
            RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(SuggestBoxBase));

        public static readonly DependencyProperty DisplayMemberPathProperty =
            DependencyProperty.Register("DisplayMemberPath", typeof(string), typeof(SuggestBoxBase),
                new PropertyMetadata("Header"));

        public static readonly DependencyProperty ValuePathProperty = DependencyProperty.Register("ValuePath",
            typeof(string), typeof(SuggestBoxBase), new PropertyMetadata("Value"));

        public static readonly DependencyProperty SuggestionsProperty = DependencyProperty.Register("Suggestions",
            typeof(IList<object>), typeof(SuggestBoxBase), new PropertyMetadata(null, OnSuggestionsChanged));

        public static readonly DependencyProperty HeaderTemplateProperty =
            HeaderedItemsControl.HeaderTemplateProperty.AddOwner(typeof(SuggestBoxBase));

        public static readonly DependencyProperty IsPopupOpenedProperty = DependencyProperty.Register("IsPopupOpened",
            typeof(bool), typeof(SuggestBoxBase), new UIPropertyMetadata(false));

        public static readonly DependencyProperty DropDownPlacementTargetProperty =
            DependencyProperty.Register("DropDownPlacementTarget", typeof(object), typeof(SuggestBoxBase));

        public static readonly DependencyProperty HintProperty =
            DependencyProperty.Register("Hint", typeof(string), typeof(SuggestBoxBase), new PropertyMetadata(""));

        public static readonly DependencyProperty IsHintVisibleProperty = DependencyProperty.Register("IsHintVisible",
            typeof(bool), typeof(SuggestBoxBase), new PropertyMetadata(true));

        protected ScrollViewer Host;
        protected ListBox ItemList;
        protected Popup Popup;
        protected bool PrevState;
        protected Grid Root;
        protected UIElement TextBoxView;

        static SuggestBoxBase()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SuggestBoxBase),
                new FrameworkPropertyMetadata(typeof(SuggestBoxBase)));
        }

        public string DisplayMemberPath
        {
            get => (string) GetValue(DisplayMemberPathProperty);
            set => SetValue(DisplayMemberPathProperty, value);
        }

        public string ValuePath
        {
            get => (string) GetValue(ValuePathProperty);
            set => SetValue(ValuePathProperty, value);
        }

        public IList<object> Suggestions
        {
            get => (IList<object>) GetValue(SuggestionsProperty);
            set => SetValue(SuggestionsProperty, value);
        }

        public DataTemplate HeaderTemplate
        {
            get => (DataTemplate) GetValue(HeaderTemplateProperty);
            set => SetValue(HeaderTemplateProperty, value);
        }

        public bool IsPopupOpened
        {
            get => (bool) GetValue(IsPopupOpenedProperty);
            set => SetValue(IsPopupOpenedProperty, value);
        }

        public object DropDownPlacementTarget
        {
            get => GetValue(DropDownPlacementTargetProperty);
            set => SetValue(DropDownPlacementTargetProperty, value);
        }

        public string Hint
        {
            get => (string) GetValue(HintProperty);
            set => SetValue(HintProperty, value);
        }

        public bool IsHintVisible
        {
            get => (bool) GetValue(IsHintVisibleProperty);
            set => SetValue(IsHintVisibleProperty, value);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            Popup = Template.FindName("PART_Popup", this) as Popup;
            ItemList = Template.FindName("PART_ItemList", this) as ListBox;
            Host = Template.FindName("PART_ContentHost", this) as ScrollViewer;
            TextBoxView = LogicalTreeHelper.GetChildren(Host).OfType<UIElement>().First();
            Root = Template.FindName("root", this) as Grid;

            GotKeyboardFocus += (o, e) =>
            {
                PopupIfSuggest();
                IsHintVisible = false;
            };
            LostKeyboardFocus += (o, e) =>
            {
                if (!IsKeyboardFocusWithin) HidePopup();
                IsHintVisible = string.IsNullOrEmpty(Text);
            };

            //09-04-09 Based on SilverLaw's approach 
            Popup.CustomPopupPlacementCallback += (popupSize, targetSize, offset) => new[]
            {
                new CustomPopupPlacement(new Point(0.01 - offset.X, Root.ActualHeight - offset.Y),
                    PopupPrimaryAxis.None)
            };

            ItemList.MouseDoubleClick += (o, e) =>
            {
                UpdateValueFromListBox();
                AcceptUpdate();
            };

            ItemList.PreviewMouseUp += (o, e) =>
            {
                if (ItemList.SelectedValue != null)
                    UpdateValueFromListBox();
            };

            ItemList.PreviewKeyDown += (o, e) =>
            {
                if (e.OriginalSource is ListBoxItem)
                {
                    e.Handled = true;
                    switch (e.Key)
                    {
                        case Key.Enter:
                            //Handle in OnPreviewKeyDown
                            break;
                        case Key.Oem5:
                            UpdateValueFromListBox(false);
                            SetValue(TextProperty, Text + "\\");
                            break;
                        case Key.Escape:
                            Focus();
                            HidePopup();
                            break;
                        default:
                            e.Handled = false;
                            break;
                    }

                    if (e.Handled)
                    {
                        Keyboard.Focus(this);
                        HidePopup();
                        Select(Text.Length, 0); //Select last char
                    }
                }
            };

            var parentWindow = Window.GetWindow(this);
            if (parentWindow != null)
            {
                parentWindow.Deactivated += delegate
                {
                    PrevState = IsPopupOpened;
                    IsPopupOpened = false;
                };
                parentWindow.Activated += delegate { IsPopupOpened = PrevState; };
            }
        }

        private void UpdateValueFromListBox(bool updateSrc = true)
        {
            SetValue(TextProperty, ItemList.SelectedValue);

            if (updateSrc)
                UpdateSource();
            HidePopup();
            Focus();
            CaretIndex = Text.Length;
            AcceptUpdate();
        }

        protected virtual void AcceptUpdate()
        {
        }

        protected virtual void UpdateSource()
        {
            var txtBindingExpr = GetBindingExpression(TextProperty);
            if (txtBindingExpr == null)
                return;

            txtBindingExpr.UpdateSource();
            RaiseEvent(new RoutedEventArgs(ValueChangedEvent));
        }

        protected void PopupIfSuggest()
        {
            if (IsFocused)
                if (Suggestions != null && Suggestions.Count > 0)
                    IsPopupOpened = true;
                else
                    IsPopupOpened = false;
        }

        protected void HidePopup()
        {
            IsPopupOpened = false;
        }

        protected static string GetDirectoryName(string path)
        {
            if (path.EndsWith("\\"))
                return path;
            //path = path.Substring(0, path.Length - 1); //Remove ending slash.

            var idx = path.LastIndexOf('\\');
            if (idx == -1)
                return "";
            return path.Substring(0, idx);
        }

        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            base.OnPreviewKeyDown(e);

            switch (e.Key)
            {
                case Key.Up:
                case Key.Down:
                case Key.Prior:
                case Key.Next:
                    if (Suggestions != null && Suggestions.Count > 0 && !(e.OriginalSource is ListBoxItem))
                    {
                        PopupIfSuggest();
                        ItemList.Focus();
                        ItemList.SelectedIndex = 0;
                        var lbi = ItemList.ItemContainerGenerator.ContainerFromIndex(0) as ListBoxItem;
                        lbi?.Focus();
                        e.Handled = true;
                    }

                    break;
                case Key.Tab:
                case Key.Return:
                case Key.Space:
                    if (ItemList.IsKeyboardFocusWithin)
                        UpdateValueFromListBox();
                    HidePopup();
                    UpdateSource();
                    e.Handled = true;
                    break;
                case Key.Back:
                    if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
                    {
                        if (Text.EndsWith("\\"))
                            SetValue(TextProperty, Text.Substring(0, Text.Length - 1));
                        else
                            SetValue(TextProperty, GetDirectoryName(Text) + "\\");

                        Select(Text.Length, 0);
                        e.Handled = true;
                    }

                    break;
            }
        }

        protected static void OnSuggestionsChanged(object sender, DependencyPropertyChangedEventArgs args)
        {
            if (args.OldValue != args.NewValue)
                ((SuggestBoxBase) sender).PopupIfSuggest();
        }

        public event RoutedEventHandler ValueChanged
        {
            add => AddHandler(ValueChangedEvent, value);
            remove => RemoveHandler(ValueChangedEvent, value);
        }
    }
}