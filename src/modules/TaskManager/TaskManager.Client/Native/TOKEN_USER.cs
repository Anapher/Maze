using System.Runtime.InteropServices;

namespace TaskManager.Client.Native
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct TOKEN_USER
    {
        public _SID_AND_ATTRIBUTES User;
    }
}