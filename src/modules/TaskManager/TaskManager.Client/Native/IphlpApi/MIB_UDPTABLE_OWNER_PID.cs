using System.Runtime.InteropServices;

// ReSharper disable InconsistentNaming

namespace TaskManager.Client.Native
{
    [StructLayout(LayoutKind.Sequential)]
    public struct MIB_UDPTABLE_OWNER_PID
    {
        public uint dwNumEntries;
        // ReSharper disable once FieldCanBeMadeReadOnly.Local
        private MIB_UDPROW_OWNER_PID table;
    }
}