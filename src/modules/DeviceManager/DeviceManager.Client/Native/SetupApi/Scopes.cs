using System;

namespace DeviceManager.Client.Native.SetupApi
{
    [Flags]
    public enum Scopes
    {
        Global = 1,
        ConfigSpecific = 2,
        ConfigGeneral = 4
    }
}