namespace FileExplorer.Administration.ViewModels.Explorer
{
    public enum FileTransferState
    {
        Pending,
        Preparing,
        Transferring,
        Failed,
        Succeeded,
        Canceled,
        NotFound
    }
}