using RegistryEditor.Administration.Model;
using TreeViewEx.Controls.Models;
using TreeViewEx.Helpers;

namespace RegistryEditor.Administration.ViewModels.Helpers
{
    public class RegistryPathComparer : PathComparer, ICompareHierarchy<IntegratedRegistryKey>
    {
        public HierarchicalResult CompareHierarchy(IntegratedRegistryKey value1, IntegratedRegistryKey value2)
        {
            return CompareHierarchyInternal(value1.Path, value2.Path);
        }
    }
}