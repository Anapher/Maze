namespace Tasks.Infrastructure.Administration.ViewModels.CreateTask
{
    public interface ITreeViewItem
    {
        bool IsSelected { get; set; }
        object NodeViewModel { get; }
    }
}