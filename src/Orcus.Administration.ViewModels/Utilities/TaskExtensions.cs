using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Orcus.Administration.ViewModels.Utilities
{
    public static class TaskExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Forget(this Task task)
        {
            //Nothing here
        }
    }
}