namespace FileExplorer.Administration.ViewModels.Explorer
{
    public enum FileTransferState
    {
        Pending,
        Preparing,
        Extracting,
        Transferring,
        Failed,
        Succeeded,
        Canceled,
        NotFound
    }
}