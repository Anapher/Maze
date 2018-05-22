namespace Orcus.Administration.Modules.Models
{
    /// <summary>
    /// List of possible statuses of items loading operation (search).
    /// Utilized by item loader and UI for progress tracking.
    /// </summary>
    public enum LoadingStatus
    {
        Unknown, // not initialized
        Cancelled, // loading cancelled
        ErrorOccurred, // error occured
        Loading, // loading is running in background
        NoItemsFound, // loading complete, no items found
        NoMoreItems, // loading complete, no more items discovered beyond current page
        Ready // loading of current page is done, next page is available
    }
}