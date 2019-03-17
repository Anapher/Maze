using System.Runtime.InteropServices;

namespace DeviceManager.Client.Native.SetupApi
{
    [StructLayout(LayoutKind.Sequential)]
    public struct PropertyChangeParameters
    {
        public int Size;
        // part of header. It's flattened out into 1 structure.
        public DiFunction DiFunction;
        public StateChangeAction StateChange;
        public Scopes Scope;
        public int HwProfile;
    }
}