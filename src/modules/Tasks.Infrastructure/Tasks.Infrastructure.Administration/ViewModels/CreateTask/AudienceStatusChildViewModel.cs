using Prism.Mvvm;

namespace Tasks.Infrastructure.Administration.ViewModels.CreateTask
{
    public class AudienceStatusChildViewModel : BindableBase, ITreeViewItem
    {
        private int _estimatedClients;

        public int EstimatedClients
        {
            get => _estimatedClients;
            set => SetProperty(ref _estimatedClients, value);
        }

        public bool IsSelected { get; set; }
        public object NodeViewModel { get; set; }
    }
}