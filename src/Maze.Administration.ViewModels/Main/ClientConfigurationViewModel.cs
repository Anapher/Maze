using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Maze.Administration.Library.Clients;
using Maze.Administration.Library.Extensions;
using Maze.Administration.Library.Rest.ClientConfigurations.V1;
using Maze.Administration.Library.Views;
using Maze.Administration.ViewModels.Utilities;
using Maze.Server.Connection.Clients;
using Prism.Commands;
using Prism.Mvvm;

namespace Maze.Administration.ViewModels.Main
{
    public class ClientConfigurationViewModel : BindableBase
    {
        private readonly IRestClient _restClient;
        private readonly IWindowService _windowService;
        private int? _groupId;
        private DateTimeOffset? _lastUpdate;
        private bool? _dialogResult;
        private AsyncRelayCommand<string> _okCommand;

        public ClientConfigurationViewModel(IRestClient restClient, IWindowService windowService)
        {
            _restClient = restClient;
            _windowService = windowService;
        }

        public string Content { get; private set; }
        public bool IsCreating { get; private set; }

        public DateTimeOffset? LastUpdate
        {
            get => _lastUpdate;
            set => SetProperty(ref _lastUpdate, value);
        }

        public bool? DialogResult
        {
            get => _dialogResult;
            set => SetProperty(ref _dialogResult, value);
        }

        public void InitializeCreate(int groupId)
        {
            _groupId = groupId;

            Content = "{\r\n    \r\n}";

            IsCreating = true;
        }

        public void InitializeUpdate(ClientConfigurationDto clientConfiguration)
        {
            _groupId = clientConfiguration.ClientGroupId;
            LastUpdate = clientConfiguration.UpdatedOn;

            Content = clientConfiguration.Content;
        }

        public AsyncRelayCommand<string> OkCommand
        {
            get
            {
                return _okCommand ?? (_okCommand = new AsyncRelayCommand<string>(async parameter =>
                {
                    if (Content == parameter)
                    {
                        DialogResult = true;
                        return;
                    }

                    var dto = new ClientConfigurationDto {ClientGroupId = _groupId, Content = parameter};
                    var errors = dto.Validate(new ValidationContext(dto)).ToList();
                    if (errors.Any())
                    {
                        _windowService.ShowErrorMessageBox(string.Join(Environment.NewLine, errors.Select(x => x.ErrorMessage)));
                        return;
                    }

                    try
                    {
                        if (IsCreating)
                            await ClientConfigurationsResource.PostClientConfiguration(dto, _restClient);
                        else await ClientConfigurationsResource.PutClientConfiguration(dto, _restClient);
                    }
                    catch (Exception e)
                    {
                        e.ShowMessage(_windowService);
                        return;
                    }

                    DialogResult = true;
                }));
            }
        }

        private DelegateCommand _cancelCommand;

        public DelegateCommand CancelCommand
        {
            get
            {
                return _cancelCommand ?? (_cancelCommand = new DelegateCommand(() => { DialogResult = false; }));
            }
        }
    }
}