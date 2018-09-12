using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using Prism.Mvvm;
using TaskManager.Shared.Dtos;

namespace TaskManager.Administration.ViewModels
{
    public class ProcessViewModel : BindableBase
    {
        private bool _canChangePriorityClass;
        private string _commandLine;
        private string _companyName;
        private string _description;
        private string _executablePath;
        private string _fileVersion;
        private long? _mainWindowHandle;
        private string _name;
        private int _parentProcessId;
        private ProcessPriorityClass _priorityClass;
        private long _privateBytes;
        private string _processOwner;
        private string _productVersion;
        private DateTimeOffset _creationDate;
        private ProcessStatus _status;
        private long _workingSet;
        private bool _isExpanded = true;
        private bool _isFiltered;

        public ProcessViewModel()
        {
            Childs = new ObservableCollection<ProcessViewModel>();
            CollectionView = new ListCollectionView(Childs);
        }

        public ListCollectionView CollectionView { get; }
        public ObservableCollection<ProcessViewModel> Childs { get; }
        public ProcessViewModel ParentViewModel { get; set; }

        public bool IsExpanded
        {
            get => _isExpanded || _isFiltered;
            set => SetProperty(ref _isExpanded, value);
        }

        public BitmapImage Icon { get; set; }

        public int Id { get; set; }

        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        public string Description
        {
            get => _description;
            set => SetProperty(ref _description, value);
        }

        public string CompanyName
        {
            get => _companyName;
            set => SetProperty(ref _companyName, value);
        }

        public long WorkingSet
        {
            get => _workingSet;
            set => SetProperty(ref _workingSet, value);
        }

        public long PrivateBytes
        {
            get => _privateBytes;
            set => SetProperty(ref _privateBytes, value);
        }

        public DateTimeOffset CreationDate
        {
            get => _creationDate;
            set => SetProperty(ref _creationDate, value);
        }

        public ProcessPriorityClass PriorityClass
        {
            get => _priorityClass;
            set => SetProperty(ref _priorityClass, value);
        }

        public bool CanChangePriorityClass
        {
            get => _canChangePriorityClass;
            set => SetProperty(ref _canChangePriorityClass, value);
        }

        public int ParentProcessId
        {
            get => _parentProcessId;
            set => SetProperty(ref _parentProcessId, value);
        }

        public string ProcessOwner
        {
            get => _processOwner;
            set => SetProperty(ref _processOwner, value);
        }

        public ProcessStatus Status
        {
            get => _status;
            set => SetProperty(ref _status, value);
        }

        public string ExecutablePath
        {
            get => _executablePath;
            set => SetProperty(ref _executablePath, value);
        }

        public string CommandLine
        {
            get => _commandLine;
            set => SetProperty(ref _commandLine, value);
        }

        public string ProductVersion
        {
            get => _productVersion;
            set => SetProperty(ref _productVersion, value);
        }

        public string FileVersion
        {
            get => _fileVersion;
            set => SetProperty(ref _fileVersion, value);
        }

        public long? MainWindowHandle
        {
            get => _mainWindowHandle;
            set => SetProperty(ref _mainWindowHandle, value);
        }

        public void UpdateView()
        {
            CollectionView.Refresh();

            var isFiltered = CollectionView.Count != Childs.Count;
            if (isFiltered != _isFiltered)
            {
                _isFiltered = isFiltered;
                RaisePropertyChanged(nameof(IsExpanded));
            }
        }

        public void Apply(ProcessDto processDto)
        {
            Id = processDto.ProcessId;

            if (processDto.TryGetProperty("Description", out string description))
                Description = description;
            if (processDto.TryGetProperty("CompanyName", out string companyName))
                CompanyName = companyName;
            if (processDto.TryGetProperty("ProductVersion", out string productVersion))
                ProductVersion = productVersion;
            if (processDto.TryGetProperty("FileVersion", out string fileVersion))
                FileVersion = fileVersion;

            if (processDto.TryGetProperty("Icon", out byte[] iconData))
                UpdateIcon(iconData);

            if (processDto.TryGetProperty("ProcessOwner", out string processOwner))
                ProcessOwner = processOwner;
            if (processDto.TryGetProperty("Status", out int status))
                Status = (ProcessStatus) status;

            if (processDto.TryGetProperty("PrivateBytes", out long privateBytes))
                PrivateBytes = privateBytes;
            if (processDto.TryGetProperty("WorkingSet", out long workingSet))
                WorkingSet = workingSet;
            if (processDto.TryGetProperty("MainWindowHandle", out long mainWindowHandle))
                MainWindowHandle = mainWindowHandle;
            if (processDto.TryGetProperty("PriorityClass", out int priorityClass))
            {
                PriorityClass = (ProcessPriorityClass) priorityClass;
                CanChangePriorityClass = true;
            }

            if (processDto.TryGetProperty("Name", out string name))
                Name = name;
            if (processDto.TryGetProperty("CreationDate", out DateTimeOffset creationDate))
                CreationDate = creationDate;
            if (processDto.TryGetProperty("ExecutablePath", out string executablePath))
                ExecutablePath = executablePath;
            if (processDto.TryGetProperty("CommandLine", out string commandLine))
                CommandLine = commandLine;
            if (processDto.TryGetProperty("ParentProcessId", out int parentProcessId))
                ParentProcessId = parentProcessId;
        }

        private void UpdateIcon(byte[] iconData)
        {
            if (iconData == null)
                return;

            using (var memoryStream = new MemoryStream(iconData, false))
            {
                var bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.StreamSource = memoryStream;
                bitmapImage.EndInit(); ;

                Icon = bitmapImage;
            }
        }

        public void UpdatePriorityClass() => RaisePropertyChanged(nameof(PriorityClass));
    }
}