using System;
using System.Windows.Data;
using Prism.Mvvm;
using Tasks.Infrastructure.Administration.Core;
using Tasks.Infrastructure.Administration.ViewModels.TaskOverview;
using Unclassified.TxLib;

namespace Tasks.Infrastructure.Administration.ViewModels
{
    public class TaskOverviewViewModel : BindableBase, IDisposable
    {
        private readonly ICommandResultViewFactory _commandResultViewFactory;

        private object _selectedItem;

        public TaskOverviewViewModel(ICommandResultViewFactory commandResultViewFactory)
        {
            _commandResultViewFactory = commandResultViewFactory;
        }

        public string Title { get; private set; }

        public ListCollectionView Sessions { get; private set; }

        public object SelectedItem
        {
            get => _selectedItem;
            set
            {
                if (SetProperty(ref _selectedItem, value))
                    if (value is CommandResultViewModel commandResult && commandResult.View == null)
                        commandResult.View = _commandResultViewFactory.GetView(commandResult.CommandResult, commandResult.Result);
            }
        }

        public void Dispose()
        {
        }

        public void Initialize(TaskActivityWatcher taskActivityWatcher, string name)
        {
            Sessions = new ListCollectionView(taskActivityWatcher.Sessions);

            Title = Tx.T("TasksInfrastructure:TaskOverview.Title", "name", name);
        }
    }
}