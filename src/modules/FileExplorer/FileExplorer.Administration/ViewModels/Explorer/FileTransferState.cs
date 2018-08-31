namespace FileExplorer.Administration.ViewModels.Explorer
{
    public enum FileTransferState
    {
        Waiting,
        Preparing,
        Transferring,
        Failed,
        Succeeded,
        Canceled,
        NotFound
    }
}