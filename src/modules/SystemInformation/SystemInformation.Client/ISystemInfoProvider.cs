using System.Collections.Generic;
using SystemInformation.Shared.Dtos;

namespace SystemInformation.Client
{
    public interface ISystemInfoProvider
    {
        IEnumerable<SystemInfoDto> FetchInformation();
    }
}