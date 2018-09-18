using System;
using System.Runtime.InteropServices;

namespace RemoteDesktop.TestApp
{
    //public class OpenH264
    //{
    //    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    //    private delegate int WelsCreateDecoderFunc(ref IntPtr ppDecoder);

    //    public OpenH264(string dllName)
    //    {
    //        var hDll = NativeMethods.LoadLibrary(dllName);
    //        if (hDll == IntPtr.Zero)
    //            throw new DllNotFoundException($"Unable to load '{dllName}'");

    //        var createDecoderFunc = NativeMethods.GetProcAddress(hDll, "WelsCreateDecoder");

    //    }
    //}

    //public static class NativeMethods
    //{
    //    [DllImport("kernel32", SetLastError = true, CharSet = CharSet.Ansi)]
    //    public static extern IntPtr LoadLibrary([MarshalAs(UnmanagedType.LPStr)] string lpFileName);

    //    [DllImport("kernel32", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
    //    public static extern IntPtr GetProcAddress(IntPtr hModule, string procName);

    //}
}