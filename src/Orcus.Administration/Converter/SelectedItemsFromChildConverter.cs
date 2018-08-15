using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using Orcus.Administration.Utilities;

namespace Orcus.Administration.Converter
{
    public class SelectedItemsFromChildConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var item = value as DataGridRow;
            if (item == null)
                return Binding.DoNothing;

            //WHY THE HELL doesn't ListBox inherit from MultiSelector?!
            var itemsControl = item.GetVisualParent<ItemsControl>();
            if (itemsControl is MultiSelector multiSelector)
                return multiSelector.SelectedItems;
            else if (itemsControl is ListBox listBox) //ListView
                return listBox.SelectedItems;

            throw new InvalidOperationException("The ItemsControl in the visual upwards search must be a DataGrid or ListBox (/ListView)");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
            throw new NotImplementedException();
    }
}