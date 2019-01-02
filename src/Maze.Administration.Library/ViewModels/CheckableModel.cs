using Prism.Mvvm;

namespace Maze.Administration.Library.ViewModels
{
    public class CheckableModel<TModel> : BindableBase
    {
        private bool _isChecked;

        public CheckableModel(TModel model)
        {
            Model = model;
        }

        public TModel Model { get; }

        public bool IsChecked
        {
            get => _isChecked;
            set => SetProperty(ref _isChecked, value);
        }
    }
}