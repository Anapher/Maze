using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using FileExplorer.Shared.Dtos;

namespace FileExplorer.Client.FileProperties
{
    public class AssemblyInfoProvider : IFilePropertyValueProvider
    {
        public IEnumerable<FileProperty> ProvideValues(FileInfo fileInfo, FilePropertiesDto dto)
        {
            var assemblyName = AssemblyName.GetAssemblyName(fileInfo.FullName);

            yield return new FileProperty
            {
                Name = "IsAssembly",
                Value = true.ToString(),
                Group = FilePropertyGroup.Executable
            };
            yield return new FileProperty
            {
                Name = "AssemblyName",
                Value = assemblyName.FullName,
                Group = FilePropertyGroup.Executable
            };
            yield return new FileProperty
            {
                Name = "AssemblyProcessorArchitecture",
                Value = assemblyName.ProcessorArchitecture.ToString(),
                Group = FilePropertyGroup.Executable
            };
            yield return new FileProperty
            {
                Name = "AssemblyHashAlgorithm",
                Value = assemblyName.HashAlgorithm.ToString(),
                Group = FilePropertyGroup.Executable
            };

            if ((assemblyName.Flags & AssemblyNameFlags.PublicKey) != 0)
            {
                yield return new FileProperty
                {
                    Name = "AssemblyPublicKeyToken",
                    Value = BitConverter.ToString(assemblyName.GetPublicKeyToken()),
                    Group = FilePropertyGroup.Executable
                };
                yield return new FileProperty
                {
                    Name = "AssemblyPublicKey",
                    Value = BitConverter.ToString(assemblyName.GetPublicKey()),
                    Group = FilePropertyGroup.Executable
                };
            }
        }
    }
}