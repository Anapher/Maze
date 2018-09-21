using System.Collections.Generic;
using System.Linq;
using SystemInformation.Shared.Dtos;
using Unclassified.TxLib;

namespace SystemInformation.Administration.ViewModels
{
    public class SystemInfoGroupViewModel
    {
        public SystemInfoGroupViewModel(string name, IEnumerable<SystemInfoDto> systemInfos)
        {
            Name = Tx.T($"SystemInformation:Category.{name}");
            Childs = systemInfos.Select(x => new SystemInfoViewModel(x)).ToList();
        }

        public string Name { get; }
        public List<SystemInfoViewModel> Childs { get; }
    }
}