using System.Windows.Data;

namespace Maze.Administration.Library.Extensions
{
    /// <summary>
    ///     Extensions for <see cref="ListCollectionView"/>
    /// </summary>
    public static class ListCollectionViewExtensions
    {
        /// <summary>
        ///     Cancel pending edits and then execute a refresh on the collection view to prevent any exceptions thrown.
        /// </summary>
        /// <param name="collectionView">The list collection view that should be refreshed.</param>
        public static void SafeRefresh(this ListCollectionView collectionView)
        {
            if (collectionView.IsEditingItem)
                collectionView.CancelEdit();
            collectionView.Refresh();
        }
    }
}