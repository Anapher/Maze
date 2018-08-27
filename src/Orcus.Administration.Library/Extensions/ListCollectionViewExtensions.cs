using System.Windows.Data;

namespace Orcus.Administration.Library.Extensions
{
    public static class ListCollectionViewExtensions
    {
        public static void SafeRefresh(this ListCollectionView collectionView)
        {
            if (collectionView.IsEditingItem)
                collectionView.CancelEdit();
            collectionView.Refresh();
        }
    }
}