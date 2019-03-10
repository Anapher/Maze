using System.Collections.Generic;
using System.Threading.Tasks;
using DeviceManager.Shared.Dtos;
using Maze.Administration.Library.Clients;
using Maze.Administration.Library.Clients.Helpers;

namespace DeviceManager.Administration.Rest
{
    public class DeviceManagerResource : ResourceBase<DeviceManagerResource>
    {
        public DeviceManagerResource() : base(null)
        {
        }

        public static Task<List<DeviceInfoDto>> QueryDevices(ITargetedRestClient client) =>
            CreateRequest().Execute(client).Return<List<DeviceInfoDto>>();
    }
}