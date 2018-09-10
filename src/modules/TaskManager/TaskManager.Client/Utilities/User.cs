using System.Security.Principal;

namespace TaskManager.Client.Utilities
{
    internal static class User
    {
        static User()
        {
            UserIdentity = WindowsIdentity.GetCurrent();
        }
        
        public static WindowsIdentity UserIdentity { get; }
    }
}