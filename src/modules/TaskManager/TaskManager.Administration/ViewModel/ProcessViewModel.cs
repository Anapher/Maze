using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using Prism.Mvvm;
using TaskManager.Shared.Dtos;

namespace TaskManager.Administration.ViewModel
{
    public class ProcessViewModel : BindableBase
    {
        private bool _canChangePriorityClass;
        private string _commandLine;
        private string _companyName;
        private string _description;
        private string _fileName;
        private string _fileVersion;
        private long? _mainWindowHandle;
        private string _name;
        private int _parentProcess;
        private ProcessPriorityClass _priorityClass;
        private long _privateBytes;
        private string _processOwner;
        private string _productVersion;
        private DateTimeOffset _startTime;
        private ProcessStatus _status;
        private long _workingSet;
        private bool _isExpanded;
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

        public DateTimeOffset StartTime
        {
            get => _startTime;
            set => SetProperty(ref _startTime, value);
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

        public int ParentProcess
        {
            get => _parentProcess;
            set => SetProperty(ref _parentProcess, value);
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

        public string FileName
        {
            get => _fileName;
            set => SetProperty(ref _fileName, value);
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

        public IDictionary<string, string> Properties { get; set; }

        public void UpdateIcon(byte[] iconData)
        {
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
    }
}