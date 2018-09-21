using System.Collections.Generic;
using System.Linq;
using SystemInformation.Shared.Dtos;
using SystemInformation.Shared.Dtos.Value;
using Unclassified.TxLib;

namespace SystemInformation.Administration.ViewModels
{
    public class SystemInfoViewModel
    {
        public SystemInfoViewModel(SystemInfoDto systemInfoDto)
        {
            Value = systemInfoDto.Value;


            if (systemInfoDto.Name[0] == '@')
                Name = Tx.T($"SystemInformation:Labels.{systemInfoDto.Name.TrimStart('@')}");
            else
                Name = systemInfoDto.Name;

            Childs = systemInfoDto.Childs?.Select(x => new SystemInfoViewModel(x)).ToList() ?? new List<SystemInfoViewModel>(0);
        }

        public string Name { get; }
        public ValueDto Value { get; }
        public string Category { get; }
        public List<SystemInfoViewModel> Childs { get; }
    }
}