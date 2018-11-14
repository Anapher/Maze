using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SystemInformation.Shared.Dtos;
using Orcus.Administration.Library.Clients;
using Orcus.Administration.Library.Clients.Helpers;

namespace SystemInformation.Administration.Rest
{
    public class SystemInformationResource : ResourceBase<SystemInformationResource>
    {
        public SystemInformationResource() : base("SystemInformation")
        {
        }

        public static async Task<List<SystemInfoDto>> FetchInformation(ITargetedRestClient restClient) =>
            await CreateRequest(HttpVerb.Get, "").Execute(restClient).Return<List<SystemInfoDto>>();
        //{
        //    var response = ;
        //    var readAsStringAsync = await response.Content.ReadAsStringAsync();
        //    throw new ArgumentException();
        //}
    }
}