using System;
using FileExplorer.Administration.Controls.Models;

namespace FileExplorer.Administration.ViewModels.Explorer.Helpers
{
    public abstract class PathComparer
    {
        private const char Separator = '\\';
        protected StringComparison StringComparison = StringComparison.OrdinalIgnoreCase;

        protected HierarchicalResult CompareHierarchy(string path1, string path2)
        {
            if (path1 == null || path2 == null)
                return HierarchicalResult.Unrelated;

            if (path1.Equals(path2, StringComparison))
                return HierarchicalResult.Current;

            var path1Split = path1.Split(new[] {Separator}, StringSplitOptions.RemoveEmptyEntries);
            var path2Split = path2.Split(new[] {Separator}, StringSplitOptions.RemoveEmptyEntries);

            for (var i = 0; i < Math.Min(path1Split.Length, path2Split.Length); i++)
                if (!path1Split[i].Equals(path2Split[i], StringComparison))
                    return HierarchicalResult.Unrelated;

            return path1Split.Length > path2Split.Length ? HierarchicalResult.Parent : HierarchicalResult.Child;
        }
    }
}