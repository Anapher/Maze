using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using FileExplorer.Client.Native;
using FileExplorer.Shared.Dtos;

namespace FileExplorer.Client.FileProperties
{
    public class OpenWithProvider : IFilePropertyValueProvider
    {
        public IEnumerable<FileProperty> ProvideValues(FileInfo fileInfo, FilePropertiesDto dto)
        {
            dto.OpenWithProgramPath = AssocQueryString(AssocStr.Executable, fileInfo.Extension);
            dto.OpenWithProgramName = AssocQueryString(AssocStr.FriendlyAppName, fileInfo.Extension);

            return Enumerable.Empty<FileProperty>();
        }

        private static string AssocQueryString(AssocStr association, string extension)
        {
            const int S_OK = 0;
            const int S_FALSE = 1;

            uint length = 0;
            var ret = NativeMethods.AssocQueryString(AssocF.None, association, extension, null, null, ref length);
            if (ret != S_FALSE)
                throw new InvalidOperationException("Could not determine associated string");

            var sb = new StringBuilder((int) length);
            // (length-1) will probably work too as the marshaller adds null termination
            ret = NativeMethods.AssocQueryString(AssocF.None, association, extension, null, sb, ref length);
            if (ret != S_OK)
                throw new InvalidOperationException("Could not determine associated string");

            return sb.ToString();
        }
    }
}