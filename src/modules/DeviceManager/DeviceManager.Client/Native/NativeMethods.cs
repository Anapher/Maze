using System;
using System.Runtime.InteropServices;
using System.Text;
using DeviceManager.Client.Native.SetupApi;

namespace DeviceManager.Client.Native
{
    internal static class NativeMethods
    {
        // 1st form using a ClassGUID only, with null Enumerator
        [DllImport("setupapi.dll", CharSet = CharSet.Auto)]
        internal static extern IntPtr SetupDiGetClassDevs(ref Guid ClassGuid, IntPtr Enumerator, IntPtr hwndParent, DiGetClassFlags Flags);

        [DllImport("setupapi.dll", SetLastError = true)]
        internal static extern bool SetupDiEnumDeviceInfo(IntPtr DeviceInfoSet, uint MemberIndex, ref SP_DEVINFO_DATA DeviceInfoData);

        [DllImport("setupapi.dll", SetLastError = true)]
        internal static extern bool SetupDiDestroyDeviceInfoList(IntPtr DeviceInfoSet);

        [DllImport("setupapi.dll", CharSet = CharSet.Auto, SetLastError = true)]
        internal static extern bool SetupDiGetDeviceRegistryProperty(IntPtr deviceInfoSet, ref SP_DEVINFO_DATA deviceInfoData, SPDRP property,
            out uint propertyRegDataType, StringBuilder propertyBuffer, uint propertyBufferSize, out uint requiredSize);

        [DllImport("setupapi.dll", CallingConvention = CallingConvention.Winapi, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool SetupDiSetClassInstallParams(IntPtr deviceInfoSet, [In] ref SP_DEVINFO_DATA deviceInfoData,
            [In] ref PropertyChangeParameters classInstallParams, int classInstallParamsSize);

        [DllImport("setupapi.dll", CallingConvention = CallingConvention.Winapi, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool SetupDiCallClassInstaller(DiFunction installFunction, IntPtr deviceInfoSet,
            [In] ref SP_DEVINFO_DATA deviceInfoData);
    }
}