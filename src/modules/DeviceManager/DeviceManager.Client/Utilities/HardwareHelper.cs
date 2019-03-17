using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Text;
using DeviceManager.Client.Native;
using DeviceManager.Client.Native.SetupApi;

namespace DeviceManager.Client.Utilities
{
    //With help from https://github.com/rmoritz/EnableDisableWin32Device
    public static class HardwareHelper
    {
        public static IEnumerable<TemporaryDeviceInfo> EnumerateDevices()
        {
            var loupGarou = Guid.Empty;
            var hDevInfo = NativeMethods.SetupDiGetClassDevs(ref loupGarou, IntPtr.Zero, IntPtr.Zero,
                DiGetClassFlags.DIGCF_ALLCLASSES | DiGetClassFlags.DIGCF_PRESENT);

            if (hDevInfo == new IntPtr(-1))
                throw new Win32Exception("INVALID_HANDLE_VALUE");

            var deviceInfoData = new SP_DEVINFO_DATA();
            deviceInfoData.Size = Marshal.SizeOf(deviceInfoData);

            try
            {
                for (uint i = 0; NativeMethods.SetupDiEnumDeviceInfo(hDevInfo, i, ref deviceInfoData); i++)
                    yield return new TemporaryDeviceInfo(hDevInfo, deviceInfoData);
            }
            finally
            {
                if (!NativeMethods.SetupDiDestroyDeviceInfoList(hDevInfo))
                    throw new Exception("Failed to destroy device list");
            }
        }

        public static void EnableDevice(IntPtr handle, SP_DEVINFO_DATA diData, bool enable)
        {
            var parameters = new PropertyChangeParameters();
            // The size is just the size of the header, but we've flattened the structure.
            // The header comprises the first two fields, both integer.
            parameters.Size = 8;
            parameters.DiFunction = DiFunction.PropertyChange;
            parameters.Scope = Scopes.Global;
            parameters.StateChange = enable ? StateChangeAction.Enable : StateChangeAction.Disable;

            if (!NativeMethods.SetupDiSetClassInstallParams(handle, ref diData, ref parameters, Marshal.SizeOf(parameters)))
                throw new Win32Exception(Marshal.GetLastWin32Error());

            if (!NativeMethods.SetupDiCallClassInstaller(DiFunction.PropertyChange, handle, ref diData))
            {
                var err = Marshal.GetLastWin32Error();
                if (err == (int) SetupApiError.NotDisableable)
                    throw new ArgumentException("Device can't be disabled (programmatically or in Device Manager).");
                if (err >= (int) SetupApiError.NoAssociatedClass && err <= (int) SetupApiError.OnlyValidateViaAuthenticode)
                    throw new Win32Exception("SetupAPI error: " + (SetupApiError) err);
                throw new Win32Exception(err);
            }
        }

        private enum SetupApiError
        {
            NoAssociatedClass = unchecked((int) 0xe0000200),
            NotDisableable = unchecked((int) 0xe0000231),
            OnlyValidateViaAuthenticode = unchecked((int) 0xe0000245)
        }

        public class TemporaryDeviceInfo
        {
            private SP_DEVINFO_DATA _devinfoData;

            public TemporaryDeviceInfo(IntPtr hDevInfo, SP_DEVINFO_DATA deviceData)
            {
                HDevInfo = hDevInfo;
                _devinfoData = deviceData;
            }

            public SP_DEVINFO_DATA DeviceData => _devinfoData;
            public IntPtr HDevInfo { get; }

            public string GetProperty(SPDRP property)
            {
                var stringBuilder = new StringBuilder(1024);
                if (!NativeMethods.SetupDiGetDeviceRegistryProperty(HDevInfo, ref _devinfoData, SPDRP.SPDRP_HARDWAREID, out _,
                    stringBuilder, (uint) stringBuilder.Capacity, out _))
                    return null;

                return stringBuilder.ToString();
            }
        }
    }
}