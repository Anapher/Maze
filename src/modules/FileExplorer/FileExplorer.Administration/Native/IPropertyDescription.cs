using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Microsoft.WindowsAPICodePack.Shell.PropertySystem;

namespace FileExplorer.Administration.Native
{
    [ComImport]
    [Guid("6F79D558-3E96-4549-A1D1-7D75D2288814")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IPropertyDescription
    {
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void GetPropertyKey(out PropertyKey pkey);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void GetCanonicalName([MarshalAs(UnmanagedType.LPWStr)] out string ppszName);

        [PreserveSig]
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        int GetPropertyType(out VarEnum pvartype);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        [PreserveSig]
        int GetDisplayName(out IntPtr ppszName);
    }
}