using System;
using System.Runtime.InteropServices;

namespace TaskManager.Client.Native
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct _SID_AND_ATTRIBUTES
    {
        public IntPtr Sid;
        public int Attributes;
    }
}