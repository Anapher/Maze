using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace RegistryEditor.Administration.Controls
{
    /// <summary>
    ///     Interaction logic for IntegerValueControl.xaml
    /// </summary>
    public partial class IntegerValueControl : UserControl
    {
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(object),
            typeof(IntegerValueControl), new FrameworkPropertyMetadata(0U, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnValueChanged));

        private bool _isSize64;

        public IntegerValueControl()
        {
            InitializeComponent();
        }

        public object Value
        {
            get => GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }

        private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((IntegerValueControl) d).OnValueChanged(e.NewValue);
        }

        private void OnValueChanged(object newValue)
        {
            if (newValue == null) newValue = 0U;

            if (newValue is uint)
                _isSize64 = false;
            else if (newValue is ulong)
                _isSize64 = true;

            if (HexdecimalRadioButton.IsChecked == true)
            {
                var hexString = _isSize64 ? ((ulong) newValue).ToString("X") : ((uint) newValue).ToString("X");

                if (!string.Equals(hexString, ValueTextBox.Text, StringComparison.OrdinalIgnoreCase))
                    ValueTextBox.Text = hexString;
            }
            else
            {
                ValueTextBox.Text = newValue.ToString();
            }
        }

        private void ValueTextBoxOnPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (char.IsDigit(e.Text, e.Text.Length - 1) || HexdecimalRadioButton.IsChecked == true && IsHex(e.Text))
                return;

            e.Handled = true;
        }

        private bool IsHex(IEnumerable<char> chars)
        {
            return chars.Select(c => c >= '0' && c <= '9' || c >= 'a' && c <= 'f' || c >= 'A' && c <= 'F').All(isHex => isHex);
        }

        private void ValueTextBoxOnTextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(ValueTextBox.Text))
            {
                SetCurrentValue(ValueProperty, _isSize64 ? 0UL : 0U);
                return;
            }

            if (_isSize64)
            {
                if (ulong.TryParse(ValueTextBox.Text,
                    HexdecimalRadioButton.IsChecked == true ? NumberStyles.HexNumber | NumberStyles.AllowHexSpecifier : NumberStyles.Integer,
                    CultureInfo.CurrentCulture, out var value))
                    SetCurrentValue(ValueProperty, value);
                else
                    ValueTextBox.Text = "0";
            }
            else
            {
                if (uint.TryParse(ValueTextBox.Text,
                    HexdecimalRadioButton.IsChecked == true ? NumberStyles.HexNumber | NumberStyles.AllowHexSpecifier : NumberStyles.Integer,
                    CultureInfo.CurrentCulture, out var value))
                    SetCurrentValue(ValueProperty, value);
                else
                    ValueTextBox.Text = "0";
            }
        }

        private void HexdecimalRadioButtonOnChecked(object sender, RoutedEventArgs e)
        {
            ValueTextBox.Text = _isSize64 ? ((ulong) Value).ToString("X") : ((uint) Value).ToString("X");
        }

        private void DecimalRadioButtonOnChecked(object sender, RoutedEventArgs e)
        {
            ValueTextBox.Text = Value.ToString();
        }
    }
}