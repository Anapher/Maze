using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace FileExplorer.Administration.Controls
{
    [TemplatePart(Name = "Content_On", Type = typeof(TextBlock))]
    [TemplatePart(Name = "Content_Off", Type = typeof(TextBox))]
    public class RenameControl : Control
    {
        private TextBlock _contentOn;
        private TextBox _contentOff;

        public static readonly DependencyProperty DisplayTextProperty = DependencyProperty.Register("DisplayText",
            typeof(string), typeof(RenameControl), new PropertyMetadata(default(string)));

        public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string),
            typeof(RenameControl), new PropertyMetadata(default(string)));

        public static readonly DependencyProperty IsEditingProperty = DependencyProperty.Register("IsEditing",
            typeof(bool), typeof(RenameControl),
            new FrameworkPropertyMetadata(false)
            {
                PropertyChangedCallback = OnIsEditingChanged,
                BindsTwoWayByDefault = true,
                DefaultUpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
            });

        public static readonly DependencyProperty IsFilenameProperty = DependencyProperty.Register("IsFilename",
            typeof(bool), typeof(RenameControl), new PropertyMetadata(default(bool)));

        public static readonly DependencyProperty CommandProperty = DependencyProperty.Register("Command",
            typeof(ICommand), typeof(RenameControl), new PropertyMetadata(default(ICommand)));

        static RenameControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(RenameControl),
                new FrameworkPropertyMetadata(typeof(RenameControl)));
        }

        private static void OnIsEditingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((RenameControl)d).OnIsEditingChanged((bool)e.NewValue);
        }

        public bool IsFilename
        {
            get => (bool)GetValue(IsFilenameProperty);
            set => SetValue(IsFilenameProperty, value);
        }

        public ICommand Command
        {
            get => (ICommand)GetValue(CommandProperty);
            set => SetValue(CommandProperty, value);
        }

        public string DisplayText
        {
            get => (string)GetValue(DisplayTextProperty);
            set => SetValue(DisplayTextProperty, value);
        }

        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        public bool IsEditing
        {
            get => (bool)GetValue(IsEditingProperty);
            set => SetValue(IsEditingProperty, value);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _contentOn = (TextBlock) Template.FindName("ContentOn", this);
            _contentOff = (TextBox) Template.FindName("ContentOff", this);

            _contentOff.PreviewKeyDown += ContentOffOnPreviewKeyDown;
            _contentOff.PreviewLostKeyboardFocus += ContentOffOnPreviewLostKeyboardFocus;
            _contentOff.PreviewMouseRightButtonDown += ContentOffOnPreviewMouseRightButtonDown;
        }

        private void OnIsEditingChanged(bool newValue)
        {
            _contentOn.Visibility = newValue ? Visibility.Hidden : Visibility.Visible;
            _contentOff.Visibility = newValue ? Visibility.Visible : Visibility.Hidden;

            if (newValue)
            {
                _contentOff.Text = Text;

                int index;
                if (IsFilename && (index = Text.LastIndexOf('.')) > -1)
                    _contentOff.Select(0, index);
                else
                    _contentOff.SelectAll();

                _contentOff.Focus();
            }
        }

        private void ContentOffOnPreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
        }

        private void ContentOffOnPreviewLostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (!IsEditing)
                return;

            if (_contentOff.ContextMenu.IsOpen)
                return;

            if (e.NewFocus == _contentOff)
                return;

            IsEditing = false;
            InvokeTextChanged();
        }

        private void ContentOffOnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Enter:
                case Key.Tab:
                    IsEditing = false;
                    InvokeTextChanged();
                    break;
                case Key.Escape:
                    IsEditing = false;
                    break;
            }
        }

        private void InvokeTextChanged()
        {
            if (Text == _contentOff.Text)
                return;

            Command?.Execute(_contentOff.Text);
        }
    }
}