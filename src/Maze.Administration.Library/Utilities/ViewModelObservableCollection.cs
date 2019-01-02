using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;

namespace Maze.Administration.Library.Utilities
{
    public class ViewModelObservableCollection<TViewModel, TModel> : ObservableCollection<TViewModel>
    {
        private readonly Func<TModel, TViewModel> _viewModelFactory;
        private readonly Dictionary<TModel, TViewModel> _viewModels = new Dictionary<TModel, TViewModel>();

        public ViewModelObservableCollection(ObservableCollection<TModel> sourceCollection, Func<TModel, TViewModel> viewModelFactory)
        {
            _viewModelFactory = viewModelFactory;

            foreach (var model in sourceCollection)
                Add(CreateViewModel(model));

            sourceCollection.CollectionChanged += SourceCollectionOnCollectionChanged;
        }

        private TViewModel CreateViewModel(TModel model)
        {
            var viewModel = _viewModelFactory(model);
            _viewModels[model] = viewModel;
            return viewModel;
        }

        private void SourceCollectionOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (var model in e.NewItems.Cast<TModel>()) Add(CreateViewModel(model));
                    break;
                case NotifyCollectionChangedAction.Remove:
                    foreach (var model in e.OldItems.Cast<TModel>())
                    {
                        var viewModel = _viewModels[model];
                        _viewModels.Remove(model);

                        Remove(viewModel);
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}