using Tasks.Infrastructure.Administration.Library;
using Tasks.Infrastructure.Administration.PropertyGrid;
using Unclassified.TxLib;

namespace Tasks.Common.Administration.Commands.Utils
{
    public static class FileSourceExtensions
    {
        public static void RegisterFileSource(this PropertyGridViewModel propertyGridViewModel, FileSourceViewModel fileSource,
            string category = null)
        {
            if (category == null)
                category = Tx.T("TasksCommon:Utilities.FileSource");

            propertyGridViewModel.RegisterProperty(() => fileSource.Url, Tx.T("TasksCommon:Utilities.FileSource.Url"),
                Tx.T("TasksCommon:Utilities.FileSource.Url.Description"), category);
            propertyGridViewModel.RegisterProperty(() => fileSource.LocalPath, Tx.T("TasksCommon:Utilities.FileSource.LocalPath"),
                Tx.T("TasksCommon:Utilities.FileSource.LocalPath.Description"), category);

            var hashCategory = category + " - " + Tx.T("TasksCommon:Utilities.FileSource.Hash");
            propertyGridViewModel.RegisterProperty(() => fileSource.HashAlgorithm, Tx.T("TasksCommon:Utilities.FileSource.Algorithm"),
                Tx.T("TasksCommon:Utilities.FileSource.Algorithm.Description"), hashCategory);
            propertyGridViewModel.RegisterProperty(() => fileSource.FileHash, Tx.T("TasksCommon:Utilities.FileSource.Hash"),
                Tx.T("TasksCommon:Utilities.FileSource.Hash.Description"), hashCategory);
        }
    }
}