using System.Collections.ObjectModel;
using Anapher.Wpf.Toolkit.Utilities;

namespace Maze.Administration.Library.ViewModels
{
    //wtf this name
    public class CheckableMappedObservableCollection<T> : MappedObservableCollection<CheckableModel<T>, T>
    {
        public CheckableMappedObservableCollection(ObservableCollection<T> sourceCollection) : base(sourceCollection, ViewModelFactory)
        {
        }

        private static CheckableModel<T> ViewModelFactory(T arg) => new CheckableModel<T>(arg);
    }
}