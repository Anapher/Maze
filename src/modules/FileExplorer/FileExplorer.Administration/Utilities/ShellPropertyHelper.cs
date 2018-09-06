using System;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using FileExplorer.Administration.Native;
using FileExplorer.Shared.Dtos;
using Microsoft.WindowsAPICodePack.Shell.PropertySystem;

namespace FileExplorer.Administration.Utilities
{
    public static class ShellPropertyHelper
    {
        public static string GetShellDisplayName(this FileProperty shellProperty)
        {
            var propertyKey = new PropertyKey(shellProperty.FormatId.Value, shellProperty.PropertyId.Value);
            var guid = new Guid("6F79D558-3E96-4549-A1D1-7D75D2288814");

            NativeMethods.PSGetPropertyDescription(ref propertyKey, ref guid, out var nativePropertyDescription);

            if (nativePropertyDescription != null)
                try
                {
                    var hr = nativePropertyDescription.GetDisplayName(out var dispNamePtr);

                    if (hr >= 0 && dispNamePtr != IntPtr.Zero)
                    {
                        var name = Marshal.PtrToStringUni(dispNamePtr);

                        // Free the string
                        Marshal.FreeCoTaskMem(dispNamePtr);

                        return name;
                    }
                }
                finally
                {
                    Marshal.ReleaseComObject(nativePropertyDescription);
                }

            return FormatString(shellProperty.Name);
        }

        private static string FormatString(string rawString)
        {
            if (string.IsNullOrEmpty(rawString))
                return null;

            var lastPartIndex = rawString.LastIndexOf('.');
            return Regex.Replace(lastPartIndex > -1 ? rawString.Substring(lastPartIndex + 1) : rawString,
                @"(\B[A-Z]+?(?=[A-Z][^A-Z])|\B[A-Z]+?(?=[^A-Z]))", " $1");
        }
    }
}