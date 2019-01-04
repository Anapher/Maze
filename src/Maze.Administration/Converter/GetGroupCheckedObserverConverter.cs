using System;
using System.Collections;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using Maze.Administration.Library.Models;
using Prism.Mvvm;

namespace Maze.Administration.Converter
{
    public class GetGroupCheckedObserverConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values?.FirstOrDefault() == null)
                return Binding.DoNothing;

            var group = (ClientGroupViewModel) values[0];
            var items = (IList) values[1];

            return new GroupCheckedObserver(group, items);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) => throw new NotSupportedException();
    }

    public class GroupCheckedObserver : BindableBase
    {
        private readonly ClientGroupViewModel _group;
        private readonly IList _items;
        private bool? _isChecked;

        public GroupCheckedObserver(ClientGroupViewModel group, IList items)
        {
            _group = group;
            _items = items;

            if (group.Clients is INotifyCollectionChanged collectionChanged)
                collectionChanged.CollectionChanged += CollectionChangedOnCollectionChanged;

            if (items is INotifyCollectionChanged collectionChanged2)
                collectionChanged2.CollectionChanged += CollectionChangedOnCollectionChanged;

            UpdateIsChecked();
        }

        public bool? IsChecked
        {
            get => _isChecked;
            set => SetProperty(ref _isChecked, value);
        }

        private void CollectionChangedOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            UpdateIsChecked();
        }

        private void UpdateIsChecked()
        {
            var someAreInGroup = false;
            var someAreNotInGroup = false;

            foreach (var clientViewModel in _items.Cast<ClientViewModel>())
            {
                if (_group.Clients.Contains(clientViewModel))
                    someAreInGroup = true;
                else someAreNotInGroup = true;
            }

            if (someAreInGroup && !someAreNotInGroup)
                IsChecked = true;
            else if (!someAreInGroup && someAreNotInGroup)
                IsChecked = false;
            else
                IsChecked = null;
        }
    }
}