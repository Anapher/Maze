using System.Collections.Concurrent;
using System.Diagnostics;
using System.Reflection;
using TaskManager.Shared.Dtos;

namespace TaskManager.Client.ProcessInfo
{
    public class FileVersionInfoProvider : IProcessValueProvider
    {
        public void ProvideValue(ProcessDto processDto, Process process, ConcurrentDictionary<string, string> otherProperties, object dtoLock)
        {
            var filename = process.MainModule.FileName;
            var fileVersionInfo = FileVersionInfo.GetVersionInfo(filename);
            processDto.Description = fileVersionInfo.FileDescription;
            processDto.CompanyName = fileVersionInfo.CompanyName;
            processDto.ProductVersion = fileVersionInfo.ProductVersion;
            processDto.FileVersion = fileVersionInfo.FileVersion;

            if (processDto.Status != ProcessStatus.Immersive)
            {
                AssemblyName.GetAssemblyName(filename);

                lock (dtoLock)
                {
                    //if this code is reached (AssemblyName.GetAssemblyName did not throw an exception)
                    if (processDto.Status != ProcessStatus.Immersive)
                        processDto.Status = ProcessStatus.NetAssembly;
                }
            }
        }
    }
}